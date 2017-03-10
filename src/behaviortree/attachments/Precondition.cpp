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

#include "behaviac/behaviortree/attachments/precondition.h"

namespace behaviac {
    void Precondition::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool Precondition::PreconditionConfig::load(const properties_t& properties) {
        bool loaded = ActionConfig::load(properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = *it;

            if (StringUtils::StringEqual(p.name, "BinaryOperator")) {
                if (StringUtils::StringEqual(p.value, "Or")) {
                    this->m_bAnd = false;
                } else if (StringUtils::StringEqual(p.value, "And")) {
                    this->m_bAnd = true;
                } else {
                    BEHAVIAC_ASSERT(false);
                }
            } else if (StringUtils::StringEqual(p.name, "Phase")) {
                if (StringUtils::StringEqual(p.value, "Enter")) {
                    this->m_phase = E_ENTER;
                } else if (StringUtils::StringEqual(p.value, "Update")) {
                    this->m_phase = E_UPDATE;
                } else if (StringUtils::StringEqual(p.value, "Both")) {
                    this->m_phase = E_BOTH;
                } else {
                    BEHAVIAC_ASSERT(false);
                }

                break;
            }
        }

        return loaded;
    }

    ///implement the class of Precondition
    Precondition::Precondition() {
        m_ActionConfig = BEHAVIAC_NEW PreconditionConfig();
    }

    Precondition::~Precondition() {
        BEHAVIAC_DELETE m_ActionConfig;
    }

    BehaviorTask* Precondition::createTask() const {
        BEHAVIAC_ASSERT(false);
        return NULL;
    }

    Precondition::EPhase Precondition::GetPhase() {
        return ((PreconditionConfig*)(this->m_ActionConfig))->m_phase;
    }

    void Precondition::SetPhase(Precondition::EPhase value) {
        ((PreconditionConfig*)this->m_ActionConfig)->m_phase = value;
    }

    bool Precondition::IsAnd() {
        return ((PreconditionConfig*)(this->m_ActionConfig))->m_bAnd;
    }

    void Precondition::SetIsAnd(bool isAnd) {
        ((PreconditionConfig*)(this->m_ActionConfig))->m_bAnd = isAnd;
    }

    bool Precondition::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (Precondition::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
}
