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
#include "behaviac/behaviortree/nodes/composites/sequence.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"

namespace behaviac {
    Sequence::Sequence()
    {}

    Sequence::~Sequence()
    {}

#if BEHAVIAC_USE_HTN

    bool Sequence::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        bool bOk = false;
        Sequence* sequence = (Sequence*)node;
        int childCount = sequence->GetChildrenCount();
        int i = 0;

        for (; i < childCount; ++i) {
            BehaviorNode* childNode = (BehaviorNode*)sequence->GetChild(i);
            PlannerTask* childTask = planner->decomposeNode(childNode, depth);

            if (childTask == NULL) {
                break;
            }

            //clear the log cache so that the next node can log all properites
            //LogManager.PLanningClearCache();
            seqTask->AddChild(childTask);
        }

        if (i == childCount) {
            bOk = true;
        }

        return bOk;
    }
#endif

    void Sequence::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool Sequence::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Sequence::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    bool Sequence::Evaluate(Agent* pAgent) {
        bool ret = true;

        for (behaviac::vector<BehaviorNode*>::iterator c = this->m_children->begin(); c != this->m_children->end(); c++) {
            ret = (*c)->Evaluate(pAgent);

            if (!ret) {
                break;
            }
        }

        return ret;
    }
    EBTStatus Sequence::SequenceUpdate(Agent* pAgent, EBTStatus childStatus, int& activeChildIndex, behaviac::vector<BehaviorTask*>& children) {
        EBTStatus s = childStatus;
        int childSize = (int)children.size();

        for (;;) {
            BEHAVIAC_ASSERT(activeChildIndex < childSize);

            if (s == BT_RUNNING) {
                BehaviorTask* pBehavior = children[activeChildIndex];

                if (this->CheckIfInterrupted(pAgent)) {
                    return BT_FAILURE;
                }

                s = pBehavior->exec(pAgent);
            }

            // If the child fails, or keeps running, do the same.
            if (s != BT_SUCCESS) {
                return s;
            }

            // Hit the end of the array, job done!
            ++activeChildIndex;

            if (activeChildIndex >= childSize) {
                return BT_SUCCESS;
            }

            s = BT_RUNNING;
        }

        return s;
    }

    bool  Sequence::CheckIfInterrupted(Agent* pAgent) {
        bool bInterrupted = this->EvaluteCustomCondition(pAgent);
        return bInterrupted;
    }
    BehaviorTask* Sequence::createTask() const {
        SequenceTask* pTask = BEHAVIAC_NEW SequenceTask();

        return pTask;
    }

    SequenceTask::SequenceTask() : CompositeTask() {
    }

    SequenceTask::~SequenceTask()
    {}

    void SequenceTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void SequenceTask::save(IIONode* node) const {
        super::save(node);
    }

    void SequenceTask::load(IIONode* node) {
        super::load(node);
    }

    bool SequenceTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        this->m_activeChildIndex = 0;

        return true;
    }

    void SequenceTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
        super::onexit(pAgent, s);
    }

    EBTStatus SequenceTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(childStatus);

        BEHAVIAC_ASSERT(this->m_activeChildIndex < (int)this->m_children.size());

        Sequence* node = (Sequence*)this->m_node;
        return node->SequenceUpdate(pAgent, childStatus, this->m_activeChildIndex, this->m_children);
    }
}
