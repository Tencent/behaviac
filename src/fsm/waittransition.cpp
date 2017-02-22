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
#include "behaviac/fsm/waittransition.h"
#include "behaviac/behaviortree/attachments/effector.h"

namespace behaviac {
    bool WaitTransition::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (WaitTransition::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    void WaitTransition::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool WaitTransition::Evaluate(Agent* pAgent) {
        return super::Evaluate(pAgent);
    }

    bool WaitTransition::Evaluate(Agent* pAgent, EBTStatus status) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(status);

        return true;
    }
}
