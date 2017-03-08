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
#include "behaviac/behaviortree/nodes/composites/selector.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"

namespace behaviac {
    Selector::Selector()
    {}

    Selector::~Selector()
    {}

    void Selector::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

#if BEHAVIAC_USE_HTN

    bool Selector::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        bool bOk = false;
        Selector* sel = (Selector*)node;
        int childCount = sel->GetChildrenCount();
        int i = 0;

        for (; i < childCount; ++i) {
            BehaviorNode* childNode = (BehaviorNode*)sel->GetChild(i);
            PlannerTask* childTask = planner->decomposeNode(childNode, depth);

            if (childTask != NULL) {
                seqTask->AddChild(childTask);
                bOk = true;
                break;
            }
        }

        return bOk;
    }
#endif

    bool Selector::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Selector::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    bool Selector::Evaluate(Agent* pAgent) {
        bool ret = true;

        for (behaviac::vector<BehaviorNode*> ::iterator c = this->m_children->begin(); c != this->m_children->end(); c++) {
            BehaviorNode* p = *c;
            ret = p->Evaluate(pAgent);

            if (ret) {
                break;
            }
        }

        return ret;
    }
    EBTStatus Selector::SelectorUpdate(Agent* pAgent, EBTStatus childStatus, int& activeChildIndex, behaviac::vector<BehaviorTask*>& children) {
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

            // If the child succeeds, or keeps running, do the same.
            if (s != BT_FAILURE) {
                return s;
            }

            // Hit the end of the array, job done!
            ++activeChildIndex;

            if (activeChildIndex >= childSize) {
                return BT_FAILURE;
            }

            s = BT_RUNNING;
        }
    }
    bool  Selector::CheckIfInterrupted(Agent* pAgent) {
        bool bInterrupted = this->EvaluteCustomCondition(pAgent);

        return bInterrupted;
    }
    BehaviorTask* Selector::createTask() const {
        SelectorTask* pTask = BEHAVIAC_NEW SelectorTask();

        return pTask;
    }

    SelectorTask::SelectorTask() : CompositeTask() {
    }

    SelectorTask::~SelectorTask() {
    }

    void SelectorTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void SelectorTask::save(IIONode* node) const {
        super::save(node);
    }

    void SelectorTask::load(IIONode* node) {
        super::load(node);
    }

    bool SelectorTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_ASSERT(this->m_children.size() > 0);
        this->m_activeChildIndex = 0;
        return true;
    }

    EBTStatus SelectorTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        // bool bFirst = true;

        BEHAVIAC_ASSERT(this->m_activeChildIndex < (int)this->m_children.size());

        Selector* node = (Selector*)this->m_node;
        return node->SelectorUpdate(pAgent, childStatus, this->m_activeChildIndex, this->m_children);
    }

    void SelectorTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }
}
