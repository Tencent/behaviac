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
#ifndef _BEHAVIAC_FSM_STARTCONDITION_H_
#define _BEHAVIAC_FSM_STARTCONDITION_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"
#include "behaviac/behaviortree/attachments/precondition.h"

namespace behaviac {
    class BEHAVIAC_API StartCondition : public Precondition {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(StartCondition, Precondition);

        StartCondition() {
            m_targetId = -1;
        }
        virtual ~StartCondition();

        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
        virtual void ApplyEffects(Agent* pAgent, BehaviorNode::EPhase  phase) const;
        int GetTargetStateId() {
            return this->m_targetId;
        }

        void SetTargetStateId(int value) {
            this->m_targetId = value;
        }

    protected:
        virtual void load(int version, const char* agentType, const properties_t& properties);

        virtual BehaviorTask* createTask() const {
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

        // ============================================================================
    protected:
        int m_targetId ;
        behaviac::vector<Effector::EffectorConfig*> m_effectors;
    };
}
#endif
