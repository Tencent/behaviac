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
#include "behaviac/behaviortree/nodes/composites/selectorstochastic.h"

namespace behaviac {
    SelectorStochastic::SelectorStochastic()
    {}

    SelectorStochastic::~SelectorStochastic()
    {}

    void SelectorStochastic::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool SelectorStochastic::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!SelectorStochastic::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* SelectorStochastic::createTask() const {
        SelectorStochasticTask* pTask = BEHAVIAC_NEW SelectorStochasticTask();

        return pTask;
    }
	bool SelectorStochastic::CheckIfInterrupted(const Agent* pAgent) {
		bool bInterrupted = this->EvaluteCustomCondition(pAgent);

		return bInterrupted;
	}

    void SelectorStochasticTask::addChild(BehaviorTask* pBehavior) {
        super::addChild(pBehavior);
    }

    void SelectorStochasticTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void SelectorStochasticTask::save(IIONode* node) const {
        super::save(node);
    }

    void SelectorStochasticTask::load(IIONode* node) {
        super::load(node);
    }

    bool SelectorStochasticTask::onenter(Agent* pAgent) {
        CompositeStochasticTask::onenter(pAgent);

        return true;
    }

    EBTStatus SelectorStochasticTask::update(Agent* pAgent, EBTStatus childStatus) {
        bool bFirst = true;

        BEHAVIAC_ASSERT(this->m_activeChildIndex < (int)this->m_children.size());
		
		SelectorStochastic* node = (SelectorStochastic*)this->m_node;

        // Keep going until a child behavior says its running.
        for (;;) {
            EBTStatus s = childStatus;

            if (!bFirst || s == BT_RUNNING) {
                uint32_t childIndex = this->m_set[this->m_activeChildIndex];
                BehaviorTask* pBehavior = this->m_children[childIndex];

				if (node->CheckIfInterrupted(pAgent)) {
					return BT_FAILURE;
				}

                s = pBehavior->exec(pAgent);
            }

            bFirst = false;

            // If the child succeeds, or keeps running, do the same.
            if (s != BT_FAILURE) {
                return s;
            }

            // Hit the end of the array, job done!
            ++this->m_activeChildIndex;

            if (this->m_activeChildIndex >= (int)this->m_children.size()) {
                return BT_FAILURE;
            }
        }
    }

    void SelectorStochasticTask::onexit(Agent* pAgent, EBTStatus s) {
        CompositeStochasticTask::onexit(pAgent, s);
    }
}
