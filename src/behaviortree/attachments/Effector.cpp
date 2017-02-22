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

#include "behaviac/behaviortree/attachments/effector.h"

#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/nodes/actions/assignment.h"
#include "behaviac/behaviortree/nodes/actions/compute.h"

#define null NULL
namespace behaviac {
    Effector::Effector() {
        m_ActionConfig = BEHAVIAC_NEW EffectorConfig();
    }
    Effector::~Effector() {
        BEHAVIAC_DELETE(m_ActionConfig);
    }

    bool Effector::EffectorConfig::load(const properties_t& properties) {
        bool loaded = ActionConfig::load(properties);

        for (propertie_const_iterator_t p = properties.begin(); p != properties.end(); ++p) {
            if (StringUtils::StringEqual(p->name, "Phase")) {
                if (StringUtils::StringEqual(p->value, "Success")) {
                    this->m_phase = E_SUCCESS;
                } else if (StringUtils::StringEqual(p->value, "Failure")) {
                    this->m_phase = E_FAILURE;
                } else if (StringUtils::StringEqual(p->value, "Both")) {
                    this->m_phase = E_BOTH;
                } else {
                    BEHAVIAC_ASSERT(false);
                }

                break;
            }
        }

        return loaded;
    }

    void Effector::SetPhase(EPhase value) {
        ((EffectorConfig*)(this->m_ActionConfig))->m_phase = value;
    }

    Effector::EPhase Effector::GetPhase() {
        return ((EffectorConfig*)(this->m_ActionConfig))->m_phase;
    }

    bool Effector::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Effector::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
}
