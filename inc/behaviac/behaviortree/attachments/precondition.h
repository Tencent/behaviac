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

#ifndef _BEHAVIAC_BEHAVIORTREE_PRECONDITION_H_
#define _BEHAVIAC_BEHAVIORTREE_PRECONDITION_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"
#include "behaviac/behaviortree/attachments/attachaction.h"
#include "behaviac/behaviortree/attachments/effector.h"

namespace behaviac {
    class BEHAVIAC_API Precondition : public AttachAction {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Precondition, AttachAction);

        enum EPhase {
            E_ENTER,
            E_UPDATE,
            E_BOTH
        };

        class PreconditionConfig : public ActionConfig {
        public:
            BEHAVIAC_DECLARE_MEMORY_OPERATORS(PreconditionConfig);
            EPhase m_phase;
            bool m_bAnd;

            PreconditionConfig() {
                m_phase = E_ENTER;
                m_bAnd = false;
            }

            virtual ~PreconditionConfig() {}

        public:
            virtual bool load(const properties_t& properties);
        };

        Precondition();
        virtual ~Precondition();

    public:
        EPhase GetPhase();
        void SetPhase(EPhase value);

        bool IsAnd();
        void SetIsAnd(bool isAnd);

    protected:
        virtual BehaviorTask* createTask() const;
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
        virtual void load(int version, const char* agentType, const properties_t& properties);
    };
}
#endif
