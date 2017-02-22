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
#include "behaviac/behaviortree/nodes/conditions/condition.h"

#include "behaviac/common/profiler/profiler.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    Condition::Condition() : m_opl(0), m_opr(0), m_operator(E_EQUAL) {
    }

    Condition::~Condition() {
        BEHAVIAC_DELETE(m_opl);
        BEHAVIAC_DELETE(m_opr);
    }

    void Condition::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        behaviac::string typeName;

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Operator")) {
                this->m_operator = OperationUtils::ParseOperatorType(p.value);
            } else if (StringUtils::StringEqual(p.name, "Opl")) {
                const char* pParenthesis = strchr(p.value, '(');

                if (pParenthesis == 0) {
                    this->m_opl = AgentMeta::ParseProperty(p.value);
                } else {
                    this->m_opl = AgentMeta::ParseMethod(p.value);
                }
            } else if (StringUtils::StringEqual(p.name, "Opr")) {
                const char* pParenthesis = strchr(p.value, '(');

                if (pParenthesis == 0) {
                    this->m_opr = AgentMeta::ParseProperty(p.value);
                } else {
                    this->m_opr = AgentMeta::ParseMethod(p.value);
                }
            } else {
                BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }

    }

    bool Condition::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Condition::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    bool Condition::Evaluate(Agent* pAgent) {
        if (this->m_opl != NULL && this->m_opr != NULL) {
            return this->m_opl->Compare(pAgent, this->m_opr, this->m_operator);
        } else {
            EBTStatus childStatus = BT_INVALID;
            EBTStatus result = this->update_impl((Agent*)pAgent, childStatus);
            return result == BT_SUCCESS;
        }
    }

    BehaviorTask* Condition::createTask() const {
        ConditionTask* pTask = BEHAVIAC_NEW ConditionTask();

        return pTask;
    }

    void Condition::Cleanup() {
    }

    void ConditionTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void ConditionTask::save(IIONode* node) const {
        super::save(node);
    }

    void ConditionTask::load(IIONode* node) {
        super::load(node);
    }

    bool ConditionTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        return true;
    }

    void ConditionTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus ConditionTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);

        // EBTStatus result = BT_FAILURE;

        BEHAVIAC_ASSERT(Condition::DynamicCast(this->GetNode()));
        Condition* pConditionNode = (Condition*)(this->GetNode());

        bool ret = pConditionNode->Evaluate(pAgent);

        return ret ? BT_SUCCESS : BT_FAILURE;
    }
}
