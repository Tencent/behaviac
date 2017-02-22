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
#include "behaviac/fsm/startcondition.h"
#include "behaviac/behaviortree/attachments/effector.h"

namespace behaviac {
    StartCondition::~StartCondition() {
    }

    bool StartCondition::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (StartCondition::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    void StartCondition::load(int version, const char* agentType, const properties_t& properties) {
        if (this->m_loadAttachment) {
            Effector::EffectorConfig* effectorConfig = BEHAVIAC_NEW Effector::EffectorConfig();

            if (effectorConfig->load(properties)) {
                this->m_effectors.push_back(effectorConfig);
            }

            return;
        }

        super::load(version, agentType, properties);

        for (propertie_const_iterator_t p = properties.begin(); p != properties.end(); ++p) {
            if (StringUtils::StringEqual(p->name, "TargetFSMNodeId")) {
                this->m_targetId = atoi(p->value);
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }
    }
    void StartCondition::ApplyEffects(Agent* pAgent, BehaviorNode::EPhase  phase) const {
        BEHAVIAC_UNUSED_VAR(phase);

        for (int i = 0; i < (int)this->m_effectors.size(); ++i) {
            const Effector::EffectorConfig* effector = this->m_effectors[i];

            effector->Execute(pAgent);
        }
    }
}
