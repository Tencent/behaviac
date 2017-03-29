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
#include "behaviac/property/properties.h"
#include "behaviac/common/workspace.h"
#include "behaviac/agent/agent.h"
#include "behaviac/common/member.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    Variables::Variables() {
        BEHAVIAC_ASSERT(this->m_variables.size() == 0);
    }
    Variables::Variables(const behaviac::map<uint32_t, IInstantiatedVariable*>& vars) {
        this->m_variables = vars;
    }

    Variables::~Variables() {
        this->Clear(true);
    }

    void Variables::Clear(bool bFull) {
        if (bFull) {
            for (Variables_t::iterator it = this->m_variables.begin();
                 it != this->m_variables.end(); ++it) {
                IInstantiatedVariable* pVar = it->second;

                BEHAVIAC_DELETE(pVar);
            }

            this->m_variables.clear();
        } else {
            for (Variables_t::iterator it = this->m_variables.begin();
                 it != this->m_variables.end();) {
                IInstantiatedVariable* pVar = it->second;

                Variables_t::iterator it_temp = it;
                ++it;

                BEHAVIAC_DELETE(pVar);
                this->m_variables.erase(it_temp);

            }
        }
    }

    void Variables::Log(const Agent* pAgent, bool bForce) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(bForce);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            for (Variables_t::iterator it = this->m_variables.begin();
                 it != this->m_variables.end(); ++it) {
                IInstantiatedVariable* pVar = it->second;

                pVar->Log(pAgent);
            }
        }

#endif//BEHAVIAC_RELEASE
    }

    //void IVariable::CopyTo(Agent* pAgent)
    //{
    //    BEHAVIAC_UNUSED_VAR(pAgent);
    //}

    void Variables::CopyTo(Agent* pAgent, Variables& target) const {
        for (Variables_t::iterator it = target.m_variables.begin();
             it != target.m_variables.end(); ++it) {
            IInstantiatedVariable* pVar = it->second;

            BEHAVIAC_DELETE(pVar);
        }

        target.m_variables.clear();

        for (Variables_t::const_iterator it = this->m_variables.begin();
             it != this->m_variables.end(); ++it) {
            IInstantiatedVariable* pVar = it->second;

            IInstantiatedVariable* pNew = pVar->clone();

            target.m_variables[it->first] = pNew;
        }

        if (pAgent) {
            for (Variables_t::iterator it = target.m_variables.begin();
                 it != target.m_variables.end(); ++it) {
                IInstantiatedVariable* pVar = it->second;
                pVar->CopyTo(pAgent);
            }
        }
    }

    void Variables::Save(IIONode* node) const {
        CIOID  variablesId("vars");
        IIONode* varsNode = node->newNodeChild(variablesId);

        for (Variables_t::const_iterator it = this->m_variables.begin();
             it != this->m_variables.end(); ++it) {
            IInstantiatedVariable* pVar = it->second;

            //skip agent members
            pVar->Save(varsNode);
        }
    }

    void Variables::LoadVars(const behaviac::string& agentTypeStr, IIONode* node, behaviac::map<uint32_t, IInstantiatedVariable*>& vars) {
        CIOID  variablesId("vars");
        IIONode* varsNode = node->findNodeChild(variablesId);

        if (varsNode) {
            CStringCRC agentType(agentTypeStr.c_str());
            AgentMeta* pAgentMeta = AgentMeta::GetMeta(agentType.GetUniqueID());

			if (pAgentMeta) {
				int varsCount = varsNode->getChildCount();

				for (int i = 0; i < varsCount; ++i) {
					IIONode* varNode = varsNode->getChild(i);

					CIOID  nameId("name");
					behaviac::string nameStr;
					varNode->getAttr(nameId, nameStr);

					CIOID  valueId("value");
					behaviac::string valueStr;
					varNode->getAttr(valueId, valueStr);

					CStringCRC memberId(nameStr.c_str());

					IProperty* pProperty = pAgentMeta->GetProperty(memberId.GetUniqueID());

					BEHAVIAC_ASSERT(pProperty);

					if (pProperty) {
						IInstantiatedVariable* p = pProperty->Instantiate();

						vars[memberId.GetUniqueID()] = p;
						p->SetValueFromString(valueStr.c_str());
					}
				}
			}
        }
    }

    void Variables::Load(IIONode* node) {
        CIOID  attrId("agentType");
        behaviac::string agentTypeStr;
        node->getAttr(attrId, agentTypeStr);

        this->LoadVars(agentTypeStr, node, this->m_variables);
    }

    IInstantiatedVariable* Variables::GetVariable(uint32_t varId) const {
        Variables_t::iterator it = ((Variables*)this)->m_variables.find(varId);

        if (it != this->m_variables.end()) {
            return it->second;
        }

        return NULL;
    }

    void Variables::AddVariable(uint32_t varId, IInstantiatedVariable* pVar, int stackIndex) {
		BEHAVIAC_UNUSED_VAR(stackIndex);
		BEHAVIAC_ASSERT(this->m_variables.find(varId) == this->m_variables.end());

        this->m_variables[varId] = pVar;
    }

    void Variables::Unload() {
        for (Variables_t::iterator it = this->m_variables.begin();
             it != this->m_variables.end();) {
            Variables_t::iterator itCurrent = it;
            Variables_t::iterator itNext = ++it;

            IInstantiatedVariable* pVar = itCurrent->second;
            this->m_variables.erase(itCurrent->first);
            it = itNext;
            BEHAVIAC_UNUSED_VAR(pVar);

        }
    }

    void Variables::Unload(const char* variableName) {
        BEHAVIAC_ASSERT(!StringUtils::IsNullOrEmpty(variableName));
        uint32_t varId = MakeVariableId(variableName);

        if (this->m_variables.find(varId) != this->m_variables.end()) {
            this->m_variables.erase(varId);
        }
    }

    void Variables::Cleanup()
    {}
}//namespace behaviac
