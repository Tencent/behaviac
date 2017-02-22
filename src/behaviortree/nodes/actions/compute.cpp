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
#include "behaviac/behaviortree/nodes/actions/compute.h"

#include "behaviac/common/profiler/profiler.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    Compute::Compute() : m_opl(0), m_opr1(0), m_opr2(0), m_operator(E_INVALID) {
    }

    Compute::~Compute() {
        BEHAVIAC_DELETE(m_opl);
        BEHAVIAC_DELETE(m_opr1);
        BEHAVIAC_DELETE(m_opr2);
    }

    ////behaviac::CMethodBase* LoadMethod(const char* value);
    //Property* LoadLeft(const char* value, behaviac::string& propertyName, const char* constValue);
    //Property* LoadRight(const char* value, const behaviac::string& propertyName, behaviac::string& typeName);

    void Compute::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        behaviac::string typeName;
        behaviac::string propertyName;

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Opl")) {
                //this->m_opl = Condition::LoadLeft(p.value, typeName);
                this->m_opl = AgentMeta::ParseProperty(p.value);
            } else if (StringUtils::StringEqual(p.name, "Operator")) {
                BEHAVIAC_ASSERT(StringUtils::StringEqual(p.value, "Add")
                                || StringUtils::StringEqual(p.value, "Sub")
                                || StringUtils::StringEqual(p.value, "Mul")
                                || StringUtils::StringEqual(p.value, "Div"));

                this->m_operator = OperationUtils::ParseOperatorType(p.value);

            } else if (StringUtils::StringEqual(p.name, "Opr1")) {
                const char* pParenthesis = strchr(p.value, '(');

                if (pParenthesis == 0) {
                    //this->m_opr1 = Condition::LoadRight(p.value, typeName);
                    this->m_opr1 = AgentMeta::ParseProperty(p.value);
                } else {
                    //method
                    //this->m_opr1_m = Action::LoadMethod(p.value);
                    this->m_opr1 = AgentMeta::ParseMethod(p.value);
                }
            } else if (StringUtils::StringEqual(p.name, "Opr2")) {
                const char* pParenthesis = strchr(p.value, '(');

                if (pParenthesis == 0) {
                    this->m_opr2 = AgentMeta::ParseProperty(p.value);
                    //this->m_opr2 = Condition::LoadRight(p.value, typeName);
                } else {
                    //method
                    //this->m_opr2_m = Action::LoadMethod(p.value);
                    this->m_opr2 = AgentMeta::ParseMethod(p.value);
                }
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }

        this->m_typeName = typeName;
    }

    bool Compute::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Compute::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* Compute::createTask() const {
        ComputeTask* pTask = BEHAVIAC_NEW ComputeTask();

        return pTask;
    }

    ComputeTask::ComputeTask() : LeafTask() {
    }

    ComputeTask::~ComputeTask() {
    }

    void ComputeTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void ComputeTask::save(IIONode* node) const {
        super::save(node);
    }

    void ComputeTask::load(IIONode* node) {
        super::load(node);
    }

    bool ComputeTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        return true;
    }

    void ComputeTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }
    EBTStatus ComputeTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(childStatus == BT_RUNNING);

        EBTStatus result = BT_SUCCESS;

        BEHAVIAC_ASSERT(Compute::DynamicCast(this->GetNode()));
        Compute* pComputeNode = (Compute*)(this->GetNode());

        //bool bValid = Compute::EvaluteCompute(pAgent, pComputeNode->m_typeName, pComputeNode->m_opl, pComputeNode->m_opr1, pComputeNode->m_opr1_m,
        //    pComputeNode->m_operator, pComputeNode->m_opr2, pComputeNode->m_opr2_m);
        if (pComputeNode->m_opl != NULL) {
            pComputeNode->m_opl->Compute(pAgent, pComputeNode->m_opr1, pComputeNode->m_opr2, pComputeNode->m_operator);
        } else {
            result = pComputeNode->update_impl(pAgent, childStatus);
        }

        return result;
    }
}
