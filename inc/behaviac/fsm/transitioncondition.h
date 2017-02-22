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

#ifndef _BEHAVIAC_FSM_TRANSITIONCONDITION_H_
#define _BEHAVIAC_FSM_TRANSITIONCONDITION_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"
#include "behaviac/fsm/startcondition.h"

namespace behaviac {
    class BEHAVIAC_API Transition : public StartCondition {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Transition, StartCondition);

        Transition() {
        }

        virtual ~Transition();

    protected:
        virtual void load(int version, const char* agentType, const properties_t& properties) {
            super::load(version, agentType, properties);
        }

    public:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
    protected:
        virtual BehaviorTask* createTask() const {
            //return new StartConditionTask();
            BEHAVIAC_ASSERT(false);
            return NULL;
        }
    };
}
#endif
