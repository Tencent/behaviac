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
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"

#include "behaviac/htn/task.h"
#include "behaviac/behaviortree/nodes/actions/action.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    Task::Task() : m_task(0), m_bHTN(false) {
    }
    Task::~Task() {
        BEHAVIAC_DELETE(m_task);
    }

    void Task::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        //for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it)
        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t p = *it;

            if (StringUtils::StringEqual(p.name, "Prototype")) {
                if (!StringUtils::IsNullOrEmpty(p.value)) {
                    this->m_task = AgentMeta::ParseMethod(p.value);
                }//if (p.value[0] != '\0')

            } else if (StringUtils::StringEqual(p.name, "IsHTN")) {
                if (StringUtils::StringEqual(p.value, "true")) {
                    this->m_bHTN = true;
                }
            }
        }
    }

#if BEHAVIAC_USE_HTN
    bool Task::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        bool bOk = false;
        Task* task = (Task*)node;
        PlannerTask* childTask = planner->decomposeTask(task, depth);

        if (childTask != NULL) {
            seqTask->AddChild(childTask);
            bOk = true;
        }

        return bOk;
    }
#endif //BEHAVIAC_USE_HTN


    bool Task::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        //if (!(pTask->GetNode() is Task))
        if (Task::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    BehaviorTask* Task::createTask() const {
        TaskTask* pTask = BEHAVIAC_NEW TaskTask();
        return pTask;
    }
    int Task::FindMethodIndex(Method* method) {
        for (uint32_t i = 0; i < this->GetChildrenCount(); ++i) {
            BehaviorNode* child = (BehaviorNode*) this->GetChild(i);

            if (child == method) {
                return i;
            }
        }

        return -1;
    }
    bool Task::IsHTN() {
        return this->m_bHTN;
    }

    TaskTask::TaskTask() : SequenceTask() {
#if BEHAVIAC_USE_HTN
        this->_planner = BEHAVIAC_NEW Planner();
#endif //BEHAVIAC_USE_HTN
    }

    TaskTask::~TaskTask() {
#if BEHAVIAC_USE_HTN
        BEHAVIAC_DELETE this->_planner;
#endif //BEHAVIAC_USE_HTN
    }

    void TaskTask::addChild(BehaviorTask* pBehavior) {
        super::addChild(pBehavior);
    }

    void TaskTask::Init(const BehaviorNode* node) {
        BEHAVIAC_ASSERT(Task::DynamicCast(node) != 0, "node is not an Method");
        Task* pTaskNode = (Task*)(node);

        if (pTaskNode->IsHTN()) {
            BranchTask::Init(node);

        } else {
            super::Init(node);
        }
    }

    void TaskTask::copyto(BehaviorTask* taeget) const {
        super::copyto(taeget);
    }
    void TaskTask::save(IIONode* node) const {
        super::save(node);
    }
    void TaskTask::load(IIONode* node) {
        super::load(node);
    }
    bool TaskTask::onenter(Agent* pAgent) {
        this->m_activeChildIndex = CompositeTask::InvalidChildIndex;
        BEHAVIAC_ASSERT(this->m_activeChildIndex == CompositeTask::InvalidChildIndex);
        Task* pMethodNode = (Task*)(this->GetNode());

#if BEHAVIAC_USE_HTN
        _planner->Init(pAgent, pMethodNode);
#endif //BEHAVIAC_USE_HTN
        BEHAVIAC_UNUSED_VAR(pMethodNode);

        return super::onenter(pAgent);
    }
    void TaskTask::onexit(Agent* pAgent, EBTStatus s) {

#if BEHAVIAC_USE_HTN
        _planner->Uninit();
#endif //BEHAVIAC_USE_HTN
        super::onexit(pAgent, s);
    }

    EBTStatus TaskTask::update(Agent* pAgent, EBTStatus childStatus) {
        EBTStatus status = childStatus;

        if (childStatus == BT_RUNNING) {
            BEHAVIAC_ASSERT(Task::DynamicCast(this->GetNode()) != 0, "node is not an Method");
            Task* pTaskNode = (Task*)(this->GetNode());

            if (pTaskNode->IsHTN()) {
#if BEHAVIAC_USE_HTN
                status = _planner->Update();
#endif //BEHAVIAC_USE_HTN
            } else {
                BEHAVIAC_ASSERT(this->m_children.size() == 1);
                BehaviorTask* c = this->m_children[0];
                status = c->exec(pAgent);
            }
        } else {
            BEHAVIAC_ASSERT(true);
        }

        return status;
    }
}
