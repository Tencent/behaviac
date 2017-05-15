/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/nodes/composites/parallel.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"

namespace behaviac {
    Parallel::Parallel() : m_failPolicy(FAIL_ON_ONE), m_succeedPolicy(SUCCEED_ON_ALL), m_exitPolicy(EXIT_NONE), m_childFinishPolicy(CHILDFINISH_LOOP)
    {}

    Parallel::~Parallel()
    {}

#if BEHAVIAC_USE_HTN

    bool Parallel::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        bool bOk = false;
        Parallel* parallel = (Parallel*)node;// as Parallel;
        int childCount = parallel->GetChildrenCount();
        int i = 0;

        for (; i < childCount; ++i) {
            BehaviorNode* childNode = (BehaviorNode*)parallel->GetChild(i);
            PlannerTask* childTask = planner->decomposeNode(childNode, depth);

            if (childTask == NULL) {
                break;
            }

            seqTask->AddChild(childTask);
        }

        if (i == childCount) {
            bOk = true;
        }

        return bOk;

    }
#endif

    bool Parallel::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Parallel::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* Parallel::createTask() const {
        ParallelTask* pTask = BEHAVIAC_NEW ParallelTask();

        return pTask;
    }

    bool Parallel::IsManagingChildrenAsSubTrees() const {
        return true;
    }

    EBTStatus Parallel::ParallelUpdate(Agent* pAgent, behaviac::vector<BehaviorTask*> children) {
        bool sawSuccess = false;
        bool sawFail = false;
        bool sawRunning = false;
        bool sawAllFails = true;
        bool sawAllSuccess = true;

        bool bLoop = (this->m_childFinishPolicy == CHILDFINISH_LOOP);

        // go through all m_children
        for (uint32_t i = 0; i < children.size(); ++i) {
            BehaviorTask* pChild = children[i];

            EBTStatus treeStatus = pChild->GetStatus();

            if (bLoop || (treeStatus == BT_RUNNING || treeStatus == BT_INVALID)) {
                EBTStatus status = pChild->exec(pAgent);

                if (status == BT_FAILURE) {
                    sawFail = true;
                    sawAllSuccess = false;

                } else if (status == BT_SUCCESS) {
                    sawSuccess = true;
                    sawAllFails = false;

                } else if (status == BT_RUNNING) {
                    sawRunning = true;
                    sawAllFails = false;
                    sawAllSuccess = false;
                }
            } else if (treeStatus == BT_SUCCESS) {
                sawSuccess = true;
                sawAllFails = false;

            } else {
                BEHAVIAC_ASSERT(treeStatus == BT_FAILURE);

                sawFail = true;
                sawAllSuccess = false;
            }
        }

        EBTStatus result = sawRunning ? BT_RUNNING : BT_FAILURE;

        if ((this->m_failPolicy == FAIL_ON_ALL && sawAllFails) ||
            (this->m_failPolicy == FAIL_ON_ONE && sawFail)) {
            result = BT_FAILURE;

        } else if ((this->m_succeedPolicy == SUCCEED_ON_ALL && sawAllSuccess) ||
                   (this->m_succeedPolicy == SUCCEED_ON_ONE && sawSuccess)) {
            result = BT_SUCCESS;
        }

        if (this->m_exitPolicy == EXIT_ABORT_RUNNINGSIBLINGS && (result == BT_FAILURE || result == BT_SUCCESS)) {
            for (uint32_t i = 0; i < children.size(); ++i) {
                BehaviorTask* pChild = children[i];
                //BEHAVIAC_ASSERT(BehaviorTreeTask.DynamicCast(pChild));
                EBTStatus treeStatus = pChild->GetStatus();

                if (treeStatus == BT_RUNNING) {
                    pChild->abort(pAgent);
                }
            }
        }

        return result;
    }

    void Parallel::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "FailurePolicy")) {
                if (StringUtils::StringEqual(p.value, "FAIL_ON_ONE")) {
                    this->m_failPolicy = FAIL_ON_ONE;

                } else if (StringUtils::StringEqual(p.value, "FAIL_ON_ALL")) {
                    this->m_failPolicy = FAIL_ON_ALL;

                } else {
                    BEHAVIAC_ASSERT(0);
                }
            } else if (StringUtils::StringEqual(p.name, "SuccessPolicy")) {
                if (StringUtils::StringEqual(p.value, "SUCCEED_ON_ONE")) {
                    this->m_succeedPolicy = SUCCEED_ON_ONE;

                } else if (StringUtils::StringEqual(p.value, "SUCCEED_ON_ALL")) {
                    this->m_succeedPolicy = SUCCEED_ON_ALL;

                } else {
                    BEHAVIAC_ASSERT(0);
                }
            } else if (StringUtils::StringEqual(p.name, "ExitPolicy")) {
                if (StringUtils::StringEqual(p.value, "EXIT_NONE")) {
                    this->m_exitPolicy = EXIT_NONE;

                } else if (StringUtils::StringEqual(p.value, "EXIT_ABORT_RUNNINGSIBLINGS")) {
                    this->m_exitPolicy = EXIT_ABORT_RUNNINGSIBLINGS;

                } else {
                    BEHAVIAC_ASSERT(0);
                }
            } else if (StringUtils::StringEqual(p.name, "ChildFinishPolicy")) {
                if (StringUtils::StringEqual(p.value, "CHILDFINISH_ONCE")) {
                    this->m_childFinishPolicy = CHILDFINISH_ONCE;

                } else if (StringUtils::StringEqual(p.value, "CHILDFINISH_LOOP")) {
                    this->m_childFinishPolicy = CHILDFINISH_LOOP;

                } else {
                    BEHAVIAC_ASSERT(0);
                }
            } else {
                //BEHAVIAC_ASSERT(0);
            }
        }
    }

    ParallelTask::ParallelTask() : CompositeTask() {
    }

    ParallelTask::~ParallelTask() {
        for (BehaviorTasks_t::iterator it = this->m_children.begin(); it != this->m_children.end(); ++it) {
            BEHAVIAC_DELETE(*it);
        }

        this->m_children.clear();
    }

    void ParallelTask::Init(const BehaviorNode* node) {
        super::Init(node);
    }

    void ParallelTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void ParallelTask::save(IIONode* node) const {
        super::save(node);
    }

    void ParallelTask::load(IIONode* node) {
        super::load(node);
    }

    bool ParallelTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        BEHAVIAC_ASSERT(this->m_activeChildIndex == CompositeTask::InvalidChildIndex);

		// reset the status cache of the children
		//for (uint32_t i = 0; i < this->m_children.size(); ++i) {
		//	BehaviorTask* pChild = this->m_children[i];

		//	pChild->reset(pAgent);
		//}

        return true;
    }

    void ParallelTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus ParallelTask::update_current(Agent* pAgent, EBTStatus childStatus) {
        EBTStatus s = this->update(pAgent, childStatus);

        return s;
    }

    EBTStatus ParallelTask::update(Agent* pAgent, EBTStatus childStatus) {
		BEHAVIAC_UNUSED_VAR(childStatus);
		Parallel* node = (Parallel*)this->m_node;
        EBTStatus result = node->ParallelUpdate(pAgent, this->m_children);

        return result;
    }
}
