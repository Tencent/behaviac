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
#include "behaviac/behaviortree/nodes/composites/ifelse.h"

namespace behaviac {
    IfElse::IfElse()
    {}

    IfElse::~IfElse()
    {}

    void IfElse::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool IfElse::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!IfElse::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* IfElse::createTask() const {
        IfElseTask* pTask = BEHAVIAC_NEW IfElseTask();

        return pTask;
    }

    void IfElseTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void IfElseTask::save(IIONode* node) const {
        super::save(node);
    }

    void IfElseTask::load(IIONode* node) {
        super::load(node);
    }

    bool IfElseTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        //reset it as it will be checked for the condition execution at the first time
        this->m_activeChildIndex = CompositeTask::InvalidChildIndex;

        if (this->m_children.size() == 3) {
            return true;
        }

        BEHAVIAC_ASSERT(false, "IfElseTask has to have three children: condition, if, else");

        return false;
    }

    void IfElseTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus IfElseTask::update(Agent* pAgent, EBTStatus childStatus) {
		BEHAVIAC_ASSERT(childStatus != BT_INVALID);
		BEHAVIAC_ASSERT(this->m_children.size() == 3);

		EBTStatus conditionResult = BT_INVALID;

		if (childStatus == BT_SUCCESS || childStatus == BT_FAILURE) {
			// if the condition returned running then ended with childStatus
			conditionResult = childStatus;
		}

		if (this->m_activeChildIndex == CompositeTask::InvalidChildIndex) {
            BehaviorTask* pCondition = this->m_children[0];

			if (conditionResult == BT_INVALID) {
				// condition has not been checked
				conditionResult = pCondition->exec(pAgent);
			}

            if (conditionResult == BT_SUCCESS) {
				// if
                this->m_activeChildIndex = 1;
            } else if (conditionResult == BT_FAILURE) {
				// else
                this->m_activeChildIndex = 2;
            }
        }
		else {
			return childStatus;
		}

        if (this->m_activeChildIndex != CompositeTask::InvalidChildIndex) {
            BehaviorTask* pBehavior = this->m_children[this->m_activeChildIndex];
            EBTStatus s = pBehavior->exec(pAgent);

            return s;
        }

        return BT_RUNNING;
    }
}
