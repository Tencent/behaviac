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
namespace behaviac
{
    template<typename T>
    class  CVariable;
    
    template<typename T>
    class CProperty;
    
    template<typename T>
    class CArrayItemVariable;


    template<typename TAGENT>
    BEHAVIAC_FORCEINLINE TAGENT* Agent::GetAgentInstance(const char* agentInstanceName, int contextId, bool& bToBind)
    {
        TAGENT* pA = 0;
        bToBind = false;

        if (Agent::IsInstanceNameRegistered(agentInstanceName))
        {
            Context& c = Context::GetContext(contextId);

            Agent* a = c.GetInstance(agentInstanceName);

            if (a)
            {
                BEHAVIAC_ASSERT(TAGENT::DynamicCast(a) != 0, "the same agentInstanceName is duplicated, please specify a unique name!");
                pA = (TAGENT*)a;
            }
            else
            {
                //instance will be created so that it need to bind it to the name
                bToBind = true;
            }
        }

        return pA;
    }

    BEHAVIAC_FORCEINLINE void Agent::InitAgent(Agent* pAgent, const char* agentInstanceName, const char* agentInstanceNameAny, bool bToBind, int contextId, short priority)
    {
        const char* szAgentInstanceName = (agentInstanceName || bToBind) ? agentInstanceNameAny : agentInstanceName;

        Init_(contextId, pAgent, priority, szAgentInstanceName);

        if (bToBind)
        {
            Context& c = Context::GetContext(contextId);
            c.BindInstance(agentInstanceNameAny, pAgent);
        }
    }

	BEHAVIAC_API bool TryStart();

    template<typename TAGENT>
    BEHAVIAC_FORCEINLINE TAGENT* Agent::Create(const char* agentInstanceName, int contextId, short priority)
    {
        const char* agentInstanceNameAny = agentInstanceName;
        if (StringUtils::IsNullOrEmpty(agentInstanceName))
        {
            agentInstanceNameAny = TAGENT::GetClassTypeName();
        }

		TryStart();

        bool bToBind = false;
        TAGENT* pA = GetAgentInstance<TAGENT>(agentInstanceNameAny, contextId, bToBind);

        if (pA == 0)
        {
            //TAGENT should be derived from Agent
            Agent* pAgent = BEHAVIAC_NEW TAGENT();
            BEHAVIAC_ASSERT(TAGENT::DynamicCast(pAgent));

            pA = (TAGENT*)pAgent;
            InitAgent(pA, agentInstanceName, agentInstanceNameAny, bToBind, contextId, priority);
        }

        return pA;
    }

    template<typename TAGENT, typename T1>
    BEHAVIAC_FORCEINLINE TAGENT* Agent::Create(T1 p1, const char* agentInstanceName, int contextId, short priority)
    {
        const char* agentInstanceNameAny = agentInstanceName;
        if (StringUtils::IsNullOrEmpty(agentInstanceName))
        {
            agentInstanceNameAny = TAGENT::GetClassTypeName();
        }

        bool bToBind = false;
        TAGENT* pA = GetAgentInstance<TAGENT>(agentInstanceNameAny, contextId, bToBind);

        if (pA == 0)
        {
            //TAGENT should be derived from Agent
            Agent* pAgent = BEHAVIAC_NEW TAGENT(p1);
            BEHAVIAC_ASSERT(TAGENT::DynamicCast(pAgent));

            pA = (TAGENT*)pAgent;
            InitAgent(pA, agentInstanceName, agentInstanceNameAny, bToBind, contextId, priority);
        }

        return pA;
    }

    template<typename TAGENT, typename T1, typename T2>
    BEHAVIAC_FORCEINLINE TAGENT* Agent::Create(T1 p1, T2 p2, const char* agentInstanceName, int contextId, short priority)
    {
        const char* agentInstanceNameAny = agentInstanceName;
        if (StringUtils::IsNullOrEmpty(agentInstanceName))
        {
            agentInstanceNameAny = TAGENT::GetClassTypeName();
        }

        bool bToBind = false;
        TAGENT* pA = GetAgentInstance<TAGENT>(agentInstanceNameAny, contextId, bToBind);

        if (pA == 0)
        {
            //TAGENT should be derived from Agent
            Agent* pAgent = BEHAVIAC_NEW TAGENT(p1, p2);
            BEHAVIAC_ASSERT(TAGENT::DynamicCast(pAgent));

            pA = (TAGENT*)pAgent;
            InitAgent(pA, agentInstanceName, agentInstanceNameAny, bToBind, contextId, priority);
        }

        return pA;
    }

    template<typename TAGENT, typename T1, typename T2, typename T3>
    BEHAVIAC_FORCEINLINE TAGENT* Agent::Create(T1 p1, T2 p2, T3 p3, const char* agentInstanceName, int contextId, short priority)
    {
        const char* agentInstanceNameAny = agentInstanceName;
        if (StringUtils::IsNullOrEmpty(agentInstanceName))
        {
            agentInstanceNameAny = TAGENT::GetClassTypeName();
        }

        bool bToBind = false;
        TAGENT* pA = GetAgentInstance<TAGENT>(agentInstanceNameAny, contextId, bToBind);

        if (pA == 0)
        {
            //TAGENT should be derived from Agent
            Agent* pAgent = BEHAVIAC_NEW TAGENT(p1, p2, p3);
            BEHAVIAC_ASSERT(TAGENT::DynamicCast(pAgent));

            pA = (TAGENT*)pAgent;
            InitAgent(pA, agentInstanceName, agentInstanceNameAny, bToBind, contextId, priority);
        }

        return pA;
    }

    template<typename TAGENT, typename T1, typename T2, typename T3, typename T4>
    BEHAVIAC_FORCEINLINE TAGENT* Agent::Create(T1 p1, T2 p2, T3 p3, T4 p4, const char* agentInstanceName, int contextId, short priority)
    {
        const char* agentInstanceNameAny = agentInstanceName;
        if (StringUtils::IsNullOrEmpty(agentInstanceName))
        {
            agentInstanceNameAny = TAGENT::GetClassTypeName();
        }

        bool bToBind = false;
        TAGENT* pA = GetAgentInstance<TAGENT>(agentInstanceNameAny, contextId, bToBind);

        if (pA == 0)
        {
            //TAGENT should be derived from Agent
            Agent* pAgent = BEHAVIAC_NEW TAGENT(p1, p2, p3, p4);
            BEHAVIAC_ASSERT(TAGENT::DynamicCast(pAgent));

            pA = (TAGENT*)pAgent;
            InitAgent(pA, agentInstanceName, agentInstanceNameAny, bToBind, contextId, priority);
        }

        return pA;
    }

    BEHAVIAC_FORCEINLINE void Agent::Destroy(Agent* pAgent)
    {
        if (pAgent)
        {
            const char* agentInstanceNameAny = pAgent->GetName().c_str();

            int contextId = pAgent->GetContextId();
            Context& c = Context::GetContext(contextId);
            Agent* a = c.GetInstance(agentInstanceNameAny);

            if (a && a == pAgent)
            {
                BEHAVIAC_ASSERT(Agent::IsInstanceNameRegistered(agentInstanceNameAny), "Don't UnRegisterInstanceName before Destory!");
                c.UnbindInstance(agentInstanceNameAny);
            }

            BEHAVIAC_DELETE(pAgent);
        }
    }

    template<typename TAGENT>
    BEHAVIAC_FORCEINLINE TAGENT* Agent::GetInstance(const char* agentInstanceName, int contextId)
    {
        const char* agentInstanceNameAny = agentInstanceName;

        if (StringUtils::IsNullOrEmpty(agentInstanceName))
        {
            agentInstanceNameAny = TAGENT::GetClassTypeName();
        }

        Agent* a = Agent::GetInstance(agentInstanceNameAny, contextId);

        BEHAVIAC_ASSERT(!a || TAGENT::DynamicCast(a));
        TAGENT* pA = (TAGENT*)a;
        return pA;
    }

    BEHAVIAC_FORCEINLINE void Agent::FireEvent(const char* eventName)
    {
        this->btonevent(eventName, NULL);
    }

    template<class ParamType1>
    BEHAVIAC_FORCEINLINE void Agent::FireEvent(const char* eventName, const ParamType1& param1)
    {
        //Agent::FireEvent(this, eventName, param1);
        behaviac::map<uint32_t, IInstantiatedVariable*> eventParams;
        char paramName[1024];

        string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
        uint32_t paramId = MakeVariableId(paramName);
        eventParams[paramId] = BEHAVIAC_NEW CVariable<ParamType1>(paramName, param1);

        this->btonevent(eventName, &eventParams);
    }

    template<class ParamType1, class ParamType2>
    BEHAVIAC_FORCEINLINE void Agent::FireEvent(const char* eventName, const ParamType1& param1, const ParamType2& param2)
    {
        //Agent::FireEvent(this, eventName, param1, param2);
        behaviac::map<uint32_t, IInstantiatedVariable*> eventParams;
        char paramName[1024];

        string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
        uint32_t paramId = MakeVariableId(paramName);
        eventParams[paramId] = BEHAVIAC_NEW CVariable<ParamType1>(paramName, param1);

        string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
        paramId = MakeVariableId(paramName);
        eventParams[paramId] = BEHAVIAC_NEW CVariable<ParamType2>(paramName, param2);

        this->btonevent(eventName, &eventParams);
    }

    template<class ParamType1, class ParamType2, class ParamType3>
    BEHAVIAC_FORCEINLINE void Agent::FireEvent(const char* eventName, const ParamType1& param1, const ParamType2& param2, const ParamType3& param3)
    {
        behaviac::map<uint32_t, IInstantiatedVariable*> eventParams;
        char paramName[1024];

        string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
        uint32_t paramId = MakeVariableId(paramName);
        eventParams[paramId] = BEHAVIAC_NEW CVariable<ParamType1>(paramName, param1);

        string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
        paramId = MakeVariableId(paramName);
        eventParams[paramId] = BEHAVIAC_NEW CVariable<ParamType2>(paramName, param2);

        string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
        paramId = MakeVariableId(paramName);
        eventParams[paramId] = BEHAVIAC_NEW CVariable<ParamType3>(paramName, param3);

        this->btonevent(eventName, &eventParams);
    }


    BEHAVIAC_FORCEINLINE bool Agent::IsVariableExisting(const char* variableName) const
    {
        uint32_t variableId = MakeVariableId(variableName);

        return m_variables->IsExisting(variableId);
    }

    //template<typename VariableType, bool bRefType>
    //struct VariableGettterDispatcher
    //{
    //    static const VariableType* Get(const AgentState& variables, const Agent* pAgent, IInstanceMember* pProperty, uint32_t variableId)
    //    {
    //        const behaviac::IMemberBase* pMember = pProperty != 0 ? pProperty : 0;
    //        return variables.Get<VariableType>(pAgent, true, pMember, variableId);
    //    }
    //};

    template<typename VariableType, bool bAgentType>
    struct AgentVariableGettterDispatcher
    {
        static void Behaviac_Assert(const VariableType* pp)
        {
        }
    };

    template<typename VariableType>
    struct AgentVariableGettterDispatcher<VariableType, true>
    {
		static void Behaviac_Assert(const VariableType* pp)
        {
            if (pp && *pp)
            {
                VariableType p = *pp;
                typedef PARAM_BASETYPE(VariableType)	BaseAgentType;
                const char* nameBaseAgentType = GetClassTypeName((BaseAgentType*)0);

                BEHAVIAC_ASSERT(p->IsAKindOf(CStringCRC(nameBaseAgentType)));
                BEHAVIAC_UNUSED_VAR(p);
                BEHAVIAC_UNUSED_VAR(nameBaseAgentType);

                BEHAVIAC_ASSERT(Agent::DynamicCast(p));
            }
        }
    };

    //template<typename VariableType>
    //struct VariableGettterDispatcher<VariableType, true>
    //{
    //    static const VariableType* Get(const AgentState& variables, const Agent* pAgent, IInstanceMember* pProperty, uint32_t variableId)
    //    {
    //        const VariableType* pV = variables.Get<VariableType>(pAgent, true, pProperty, variableId);
    //        const VariableType* pp = pV;

    //        AgentVariableGettterDispatcher<VariableType, behaviac::Meta::IsAgent<VariableType>::Result>::Assert(pp);
    //        return pp;
    //    }
    //};

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE const VariableType& Agent::GetVariable(const char* variableName) const
    {
        //return *m_variables.Get<VariableType>(this, variableName);
        uint32_t variableId = MakeVariableId(variableName);
        return GetVariable<VariableType>(variableId);
    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE const VariableType& Agent::GetVariable(uint32_t variableId) const
    {
        //static VariableType _value = VariableType();
        //VariableType* value = &_value;

        VariableType* value = NULL;
        //var 
        if (this->GetVarValue(variableId, value))
        {
            BEHAVIAC_ASSERT(value != NULL);
            return *value;
        }

        //property
        IProperty* prop = this->GetProperty(variableId);
        if (prop != NULL)
        {
            CProperty<VariableType>* p = (CProperty<VariableType>*)(prop);
            BEHAVIAC_ASSERT(p != NULL);
            value = (VariableType*)p->GetValue(this);
            return *value;
        }
        BEHAVIAC_ASSERT(false, "The variable \"%d\" can not be found!\n", variableId);
        return *value;

    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE const VariableType& Agent::GetVariable(uint32_t variableId, int index) const
    {

        //static VariableType _value = VariableType();
        //VariableType* value = &_value;  

        VariableType* pValue = NULL;
        //var 
        if (this->GetVarValue<VariableType>(variableId, index, pValue))
        {
            BEHAVIAC_ASSERT(pValue != NULL);
            return *pValue;
        }

        //property
        IProperty* prop = this->GetProperty(variableId);
        if (prop != NULL)
        {
            CProperty<VariableType>* p = (CProperty<VariableType>*)prop;
            BEHAVIAC_ASSERT(p != NULL);
            //return p->GetValue(this, index);
            pValue = (VariableType*)p->GetValueElement(this, index);
            return *pValue;
        }

        BEHAVIAC_ASSERT(false, "The variable \"%d\" can not be found!\n", variableId);
        return *pValue;

    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE bool Agent::GetVarValue(uint32_t variableId, VariableType*& pValue) const
    {
        IInstantiatedVariable* v = this->GetInstantiatedVariable(variableId);

        if (v != NULL)
        {
            pValue = (VariableType*)((Agent*)this)->GetValueObject(v);
            return true;
        }

        pValue = 0;

        return false;
    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE bool  Agent::GetVarValue(uint32_t variableId, int index, VariableType*& pValue) const
    {
        IInstantiatedVariable* v = this->GetInstantiatedVariable(variableId);

        if (v != NULL)
        {
            CArrayItemVariable<VariableType>* arrayItemVar = (CArrayItemVariable<VariableType>*)v;
            BEHAVIAC_ASSERT(arrayItemVar != NULL);
            pValue = (VariableType*)arrayItemVar->GetValueElement(this, index);
            return true;
        }

        pValue = 0;

        return false;
    }

    template<typename VariableType, bool bRefType>
    struct VariableSettterDispatcher
    {
        static void Set(AgentState& variables, bool bMemberSet, Agent* pAgent, bool bLocal, const behaviac::IMemberBase* pMember, const char* variableName, const VariableType& value, uint32_t variableId)
        {
            variables.Set(bMemberSet, pAgent, bLocal, pMember, variableName, value, variableId);
        }
    };

    template<typename VariableType>
    struct VariableSettterDispatcher<VariableType, true>
    {
        static void Set(AgentState& variables, bool bMemberSet, Agent* pAgent, bool bLocal, const behaviac::IMemberBase* pMember, const char* variableName, const VariableType& value, uint32_t variableId)
        {
            variables.Set(bMemberSet, pAgent, bLocal, pMember, variableName, (void*)value, variableId);
        }
    };

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE bool Agent::SetVarValue(uint32_t varId, const VariableType* value)
    {
        IInstantiatedVariable* v = this->GetInstantiatedVariable(varId);
        if (v != NULL)
        {
            CVariable<VariableType>* var = (CVariable<VariableType>*)v;

            var->SetValue(this, value);
            return true;
        }

        return false;
    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE bool Agent::SetVarValue(uint32_t varId, int index, const VariableType* value)
    {
        IInstantiatedVariable* v = this->GetInstantiatedVariable(varId);
        if (v != NULL)
        {
            CArrayItemVariable<VariableType>* arrayItemVar = (CArrayItemVariable<VariableType>*)v;

            arrayItemVar->SetValueElement(this, value, index);
            return true;
        }

        return false;
    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE void Agent::SetVariable(const char* variableName, const VariableType& value)
    {
        uint32_t variableId = MakeVariableId(variableName);

        this->SetVariable(variableName, variableId, value);
    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE void Agent::SetVariable(const char* variableName, uint32_t variableId, const VariableType& value)
    {
        if (variableId == 0)
        {
            variableId = MakeVariableId(variableName);
        }
        //var 
        if (this->SetVarValue(variableId, &value))
        {
            return;
        }
        //property
        IProperty* prop = this->GetProperty(variableId);
        if (prop != NULL)
        {
            CProperty<VariableType>* p = (CProperty<VariableType>*)prop;
            p->SetValue(this, &value);
            return;
        }
        BEHAVIAC_ASSERT(false, "The variable \"%s\" can not be found!", variableName);
    }

    template<typename VariableType>
    BEHAVIAC_FORCEINLINE void Agent::SetVariable(const char* variableName, uint32_t variableId, const VariableType& value, int index)
    {
        if (variableId == 0)
        {
            variableId = MakeVariableId(variableName);
        }
        //var 
        if (this->SetVarValue(variableId, index, &value))
        {
            return;
        }

        //property
        IProperty* prop = this->GetProperty(variableId);
        if (prop != NULL)
        {
            CProperty<VariableType>* p = (CProperty<VariableType>*)prop;

            p->SetValueElement(this, &value, index);
            return;
        }
        BEHAVIAC_ASSERT(false, "The variable \"%s\" can not be found!", variableName);
    }

    //template <typename P1>
    //BEHAVIAC_FORCEINLINE bool Agent::Invoke(Agent* pAgent, const char* methodName, P1 p1)
    //{
    //    const behaviac::CMethodBase* pMethod = Agent::FindMethodBase(methodName);

    //    if (pMethod)
    //    {
    //        const_cast<behaviac::CMethodBase*>(pMethod)->vCall(pAgent, &p1);
    //        return true;
    //    }

    //    return false;
    //}

    //template <typename P1, typename P2>
    //BEHAVIAC_FORCEINLINE bool Agent::Invoke(Agent* pAgent, const char* methodName, P1 p1, P2 p2)
    //{
    //    const behaviac::CMethodBase* pMethod = Agent::FindMethodBase(methodName);

    //    if (pMethod)
    //    {
    //        const_cast<behaviac::CMethodBase*>(pMethod)->vCall(pAgent, &p1, &p2);
    //        return true;
    //    }

    //    return false;
    //}

    //template <typename P1, typename P2, typename P3>
    //BEHAVIAC_FORCEINLINE bool Agent::Invoke(Agent* pAgent, const char* methodName, P1 p1, P2 p2, P3 p3)
    //{
    //    const behaviac::CMethodBase* pMethod = Agent::FindMethodBase(methodName);

    //    if (pMethod)
    //    {
    //        const_cast<behaviac::CMethodBase*>(pMethod)->vCall(pAgent, &p1, &p2, &p3);
    //        return true;
    //    }

    //    return false;
    //}

    //template <typename R>
    //bool Agent::GetInvokeReturn(Agent* pAgent, const char* methodName, R& returnValue)
    //{
    //    const behaviac::CMethodBase* pMethod = Agent::FindMethodBase(methodName);

    //    if (pMethod)
    //    {
    //        const_cast<behaviac::CMethodBase*>(pMethod)->GetReturnValue(pAgent, returnValue);
    //        return true;
    //    }

    //    return false;
    //}

    template<typename TAGENT>
    BEHAVIAC_FORCEINLINE bool Agent::RegisterInstanceName(const char* agentInstanceName, const wchar_t* displayName, const wchar_t* desc)
    {
        const char* arrStr[] = { " " };
        BEHAVIAC_ASSERT(StringUtils::FindString(agentInstanceName, arrStr, 1) == -1);
        BEHAVIAC_UNUSED_VAR(arrStr);

        const char* agentInstanceNameAny = agentInstanceName;

        if (StringUtils::IsNullOrEmpty(agentInstanceName))
        {
            agentInstanceNameAny = TAGENT::GetClassTypeName();
        }

        AgentNames_t::iterator it = Agent::Names().find(agentInstanceNameAny);

        if (it == Agent::Names().end())
        {
            const char* classFullName = TAGENT::GetClassTypeName();

            Agent::Names()[agentInstanceNameAny] = AgentName_t(agentInstanceNameAny, classFullName, displayName, desc);

            return true;
        }

        return false;
    }

    template<typename TAGENT>
    BEHAVIAC_FORCEINLINE void Agent::UnRegisterInstanceName(const char* agentInstanceName)
    {
		if (Agent::NamesPtr() != NULL)
		{
			const char* agentInstanceNameAny = agentInstanceName;

			if (StringUtils::IsNullOrEmpty(agentInstanceName))
			{
				agentInstanceNameAny = TAGENT::GetClassTypeName();
			}

			AgentNames_t::iterator it = Agent::Names().find(agentInstanceNameAny);

			if (it != Agent::Names().end())
			{
				Agent::Names().erase(it);
			}
		}
    }

    //BEHAVIAC_FORCEINLINE void Variables::SetFromString(Agent* pAgent, const char* variableName, const char* valueStr)
    //{
    //    BEHAVIAC_ASSERT(variableName && variableName[0] != '\0');

    //    //to skip class name
    //    const char* variableNameOnly = GetNameWithoutClassName(variableName);

    //    // const behaviac::IMemberBase* pMember = pAgent->FindMember(variableNameOnly); //TODO: 3 mark for remove

    //    uint32_t varId = MakeVariableId(variableNameOnly);
    //    Variables_t::iterator it = this->m_variables.find(varId);

    //    if (it != this->m_variables.end())
    //    {
    //        // IInstantiatedVariable* pVar = (IInstantiatedVariable*)it->second;
    //        BEHAVIAC_ASSERT(false);
    //    }
    //}

    template<typename VariableType>
    void AgentState::Set(bool bMemberSet, Agent* pAgent, bool bLocal, const behaviac::IMemberBase* pMember, const char* variableName, const VariableType& value, uint32_t varId)
    {
        // not in planning
        if (pAgent->m_planningTop == -1 && !bLocal)
        {
            // Variables::Set(bMemberSet, pAgent, bLocal, pMember, variableName, value, varId);
            BEHAVIAC_ASSERT(false);
            return;
        }

        if (this->state_stack.size() > 0)
        {
            size_t stackIndex = 0;

            if (bLocal)
            {
                //top
                stackIndex = this->state_stack.size() - 1;
            }
            else
            {
                //bottom
                stackIndex = pAgent->m_planningTop;
            }

            AgentState* t = this->state_stack[stackIndex];

            //if there are something in the state stack, it is used for planning, so, don't really set member
            t->Set(false, pAgent, bLocal, NULL, variableName, value, varId);
        }
        else
        {
            // Variables::Set(bMemberSet, pAgent, bLocal, pMember, variableName, value, varId);
            BEHAVIAC_ASSERT(false);
        }
    }

    template<typename VariableType>
    const VariableType* AgentState::Get(const Agent* pAgent, bool bMemberGet, const behaviac::IMemberBase* pMember, uint32_t varId) const
    {
        if (this->state_stack.size() > 0)
        {
            for (int i = (int)this->state_stack.size() - 1; i >= 0; --i)
            {
                AgentState* t = this->state_stack[i];
                const VariableType* result = t->Get<VariableType>(pAgent, false, pMember, varId);

                if (result != NULL)
                {
                    return result;
                }
            }
        }

        //behaviac::CTagObject* result1 = Variables::Get(pAgent, bMemberGet, pMember, varId);

        // const VariableType* result1 = Variables::Get<VariableType>(pAgent, bMemberGet, pMember, varId); //(pAgent, bMemberGet, pMember, varId);
        BEHAVIAC_ASSERT(false);
        return 0;
    }

    template<typename T, bool bStruct>
    struct UserDefinedTypeAssert
    {
        static bool IsAKindOf(T p, const char* szType)
        {
            return false;
        }
    };

    template<typename T>
    struct UserDefinedTypeAssert<T, true>
    {
        static bool IsAKindOf(T p, const char* szType)
        {
            return p->IsAKindOf(CStringCRC(szType));
        }
    };

    template<typename T>
    static bool IsChildOf(T child, const char* baseClass)
    {
        //agent or user defined struct
        return UserDefinedTypeAssert<T, behaviac::Meta::IsPtr<T>::Result && behaviac::Meta::IsRefType<T>::Result>::IsAKindOf(child, baseClass);
    }

#if BEHAVIAC_DEBUG_DEFINED
    template<typename T>
    static bool CheckCompatibleType(const behaviac::string& typeName) {
        behaviac::string t = GetTypeDescString<T>();

        if (typeName == t) {
            return true;
        }

        t = GetTypeDescString<T&>();

        if (typeName == t) {
            return true;
        }

        return false;
    }
#endif
}//namespace behaviac

