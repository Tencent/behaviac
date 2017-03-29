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

#ifndef _BEHAVIAC_PROPERTY_PROPERTIES_H_
#define _BEHAVIAC_PROPERTY_PROPERTIES_H_

#include "behaviac/common/base.h"
#include "behaviac/common/object/tagobject.h"

#include "behaviac/common/logger/logmanager.h"
#include "behaviac/common/string/tostring.h"
#include "behaviac/property/operators.inl"
#include "behaviac/property/property.h"

namespace behaviac {
    class Agent;
    class IInstantiatedVariable;
    class IInstanceMember;
    BEHAVIAC_API uint32_t MakeVariableId(const char* idString);

    class BEHAVIAC_API Variables {
    public:
        Variables();
        Variables(const behaviac::map<uint32_t, IInstantiatedVariable*>& vars);
        virtual ~Variables();

        virtual void Clear(bool bFull);

        bool IsExisting(uint32_t varId) const {
            Variables_t::const_iterator it = this->m_variables.find(varId);

            if (it != this->m_variables.end()) {
                return true;
            }

            return false;
        }

        void Log(const Agent* pAgent, bool bForce);

        void Unload();
        void Unload(const char* variableName);

        static void Cleanup();

        void CopyTo(Agent* pAgent, Variables& target) const;

        void Save(IIONode* node) const;
        void Load(IIONode* node);

        static void LoadVars(const behaviac::string& agentTypeStr, IIONode* node, behaviac::map<uint32_t, IInstantiatedVariable*>& vars);

        virtual IInstantiatedVariable* GetVariable(uint32_t varId) const;
        virtual void AddVariable(uint32_t varId, IInstantiatedVariable* pVar, int stackIndex);

    protected:
        typedef behaviac::map<uint32_t, IInstantiatedVariable*> Variables_t;
        Variables_t m_variables;
    public:
        behaviac::map<uint32_t, IInstantiatedVariable*>& Vars() {
            return this->m_variables;
        };
    };
}//namespace behaviac

#endif//_BEHAVIAC_PROPERTY_PROPERTIES_H_
