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
#include "behaviac/behaviortree/nodes/actions/assignment.h"
#include "behaviac/common/profiler/profiler.h"


#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    Assignment::Assignment() : m_opl(0), m_opr(0), m_bCast(false) {
    }

    Assignment::~Assignment() {
        BEHAVIAC_DELETE(m_opl);
        BEHAVIAC_DELETE(m_opr);
    }

    //behaviac::CMethodBase* LoadMethod(const char* value);
    //Property* LoadLeft(const char* value, behaviac::string& propertyName, const char* constValue);
    //Property* LoadRight(const char* value, const behaviac::string& propertyName, behaviac::string& typeName);
    /**
    handle the Assignment property

    */
    void Assignment::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        behaviac::string propertyName;

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "CastRight")) {
                this->m_bCast = StringUtils::StringEqualNoCase(p.value, "true");
            } else if (StringUtils::StringEqual(p.name, "Opl")) {
                //this->m_opl = Condition::LoadLeft(p.value, propertyName);
                this->m_opl = AgentMeta::ParseProperty(p.value);
            } else if (StringUtils::StringEqual(p.name, "Opr")) {
                const char* pParenthesis = strchr(p.value, '(');

                if (pParenthesis == 0) {
                    behaviac::string typeName;
                    //this->m_opr = Condition::LoadRight(p.value, typeName);
                    this->m_opr = AgentMeta::ParseProperty(p.value);
                } else {
                    //method
                    //this->m_opr = Action::LoadMethod(p.value);
                    this->m_opr = AgentMeta::ParseMethod(p.value);
                }
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }
    }
    //  bool Assignment::EvaluteAssignment(const Agent* pAgent, IInstanceMember* opl, IInstanceMember* opr, behaviac::CMethodBase* opr_m)
    //  {
    //      bool bValid = false;

    //if (opl != NULL)
    //{
    //	if (opr_m != NULL)
    //	{
    //		Agent* pParentL = (Agent*)opl->GetParentAgent(pAgent);

    //		opl->SetFrom((Agent*)pAgent, opr_m, pParentL);

    //		bValid = true;
    //	}
    //	else if (opr != NULL)
    //	{
    //		Agent* pParentL = opl->GetParentAgent(pAgent);
    //		Agent* pParentR = opr->GetParentAgent(pAgent);

    //		opl->SetFrom(pParentR, opr, pParentL);

    //		bValid = true;
    //	}
    //}

    //      return bValid;
    //  }
    bool Assignment::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Assignment::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* Assignment::createTask() const {
        AssignmentTask* pTask = BEHAVIAC_NEW AssignmentTask();

        return pTask;
    }

    AssignmentTask::AssignmentTask() : LeafTask() {
    }

    AssignmentTask::~AssignmentTask() {
    }

    void AssignmentTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void AssignmentTask::save(IIONode* node) const {
        super::save(node);
    }

    void AssignmentTask::load(IIONode* node) {
        super::load(node);
    }

    bool AssignmentTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        return true;
    }

    void AssignmentTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus AssignmentTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(childStatus == BT_RUNNING);

        BEHAVIAC_ASSERT(Assignment::DynamicCast(this->GetNode()));
        Assignment* pAssignmentNode = (Assignment*)(this->GetNode());

        EBTStatus result = BT_SUCCESS;
        //bool bValid = Assignment::EvaluteAssignment(pAgent, pAssignmentNode->m_opl, pAssignmentNode->m_opr, pAssignmentNode->m_opr_m);

        if (pAssignmentNode->m_opl != NULL) {
            pAssignmentNode->m_opl->SetValueCast(pAgent, pAssignmentNode->m_opr, pAssignmentNode->m_bCast);
        } else {
            result = pAssignmentNode->update_impl(pAgent, childStatus);
        }

        return result;
    }
}
