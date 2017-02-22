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
#include "behaviac/behaviortree/nodes/composites/sequencestochastic.h"

namespace behaviac {
    SequenceStochastic::SequenceStochastic()
    {}

    SequenceStochastic::~SequenceStochastic()
    {}

    void SequenceStochastic::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }
    bool SequenceStochastic::CheckIfInterrupted(const Agent* pAgent) {
        bool bInterrupted = this->EvaluteCustomCondition(pAgent);

        return bInterrupted;
    }
    bool SequenceStochastic::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!SequenceStochastic::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* SequenceStochastic::createTask() const {
        SequenceStochasticTask* pTask = BEHAVIAC_NEW SequenceStochasticTask();

        return pTask;
    }

    void SequenceStochasticTask::addChild(BehaviorTask* pBehavior) {
        super::addChild(pBehavior);
    }

    void SequenceStochasticTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void SequenceStochasticTask::save(IIONode* node) const {
        super::save(node);
    }

    void SequenceStochasticTask::load(IIONode* node) {
        super::load(node);
    }

    bool SequenceStochasticTask::onenter(Agent* pAgent) {
        super::onenter(pAgent);

        return true;
    }

    EBTStatus SequenceStochasticTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(this->m_activeChildIndex < (int)this->m_children.size());

        bool bFirst = true;

        SequenceStochastic* node = (SequenceStochastic*)this->m_node;
        // Keep going until a child behavior says its running.
        EBTStatus s = childStatus;

        for (;;) {
            if (!bFirst || s == BT_RUNNING) {
                uint32_t childIndex = this->m_set[this->m_activeChildIndex];
                BehaviorTask* pBehavior = this->m_children[childIndex];

                if (node->CheckIfInterrupted(pAgent)) {
                    return BT_FAILURE;
                }

                s = pBehavior->exec(pAgent);
            }

            bFirst = false;

            // If the child fails, or keeps running, do the same.
            if (s != BT_SUCCESS) {
                return s;
            }

            // Hit the end of the array, job done!
            ++this->m_activeChildIndex;

            if (this->m_activeChildIndex >= (int)this->m_children.size()) {
                return BT_SUCCESS;
            }
        }
    }

    void SequenceStochasticTask::onexit(Agent* pAgent, EBTStatus s) {
        super::onexit(pAgent, s);
    }
}
