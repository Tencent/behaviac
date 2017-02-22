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
#ifndef _BEHAVIAC_FSM_ALWAYSTRANSITION_H_
#define _BEHAVIAC_FSM_ALWAYSTRANSITION_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"
#include "behaviac/fsm/transitioncondition.h"

namespace behaviac {
    class BEHAVIAC_API AlwaysTransition : public Transition {
    protected:
        enum ETransitionPhase {
            ETP_Always,
            ETP_Success,
            ETP_Failure,
            ETP_Exit,
        };

    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(AlwaysTransition, Transition);

        AlwaysTransition() : m_transitionPhase(ETP_Always) {
        }
        virtual ~AlwaysTransition() {
        }

        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    protected:
        virtual void load(int version, const char* agentType, const properties_t& properties);
        virtual bool Evaluate(Agent* pAgent);
        virtual bool Evaluate(Agent* pAgent, EBTStatus status);
        virtual BehaviorTask* createTask() const {
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

        ETransitionPhase m_transitionPhase;
    };

}
#endif//_BEHAVIAC_FSM_ALWAYSTRANSITION_H_
