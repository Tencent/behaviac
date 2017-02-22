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

#ifndef _BEHAVIAC_HTN_AGENTSTATE_H_
#define _BEHAVIAC_HTN_AGENTSTATE_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/properties.h"
#include "behaviac/common/thread/mutex_lock.h"

#define BEHAVIAC_ENABLE_PUSH_OPT	1

namespace behaviac {
    class BEHAVIAC_API AgentState : public Variables {
    public:
        struct AgentStateScope {
            AgentState* m_state;
            AgentStateScope(AgentState* currentState) : m_state(currentState) {

            }

            ~AgentStateScope() {
                m_state->Pop();
            }
        };
    private:

        behaviac::vector<AgentState*>			state_stack;
        static behaviac::Mutex					ms_mutex;
        static behaviac::vector<AgentState*>	pool;
        AgentState* 							parent;
#if BEHAVIAC_ENABLE_PUSH_OPT
        bool									m_forced;
        int										m_pushed;
#endif
    public:
        AgentState();
        virtual ~AgentState();
        AgentState(AgentState* parent);
        AgentState(behaviac::map<uint32_t, IInstantiatedVariable*> vars);
        void Dispose();
        int Depth();
        int Top();
        AgentState* Push(bool bForcePush);
        void Pop();

        virtual void Clear(bool bFull);

        template<typename VariableType>
        void Set(bool bMemberSet, Agent* pAgent, bool bLocal, const behaviac::IMemberBase* pMember, const char* variableName, const VariableType& value, uint32_t varId = 0);

        virtual void AddVariable(uint32_t varId, IInstantiatedVariable* pVar, int stackIndex);

        template<typename VariableType>
        const VariableType* Get(const Agent* pAgent, bool bMemberGet, const behaviac::IMemberBase* pMember, uint32_t varId) const;

        virtual IInstantiatedVariable* GetVariable(uint32_t varId) const;

    private:
        void PopTop();
    };
}
#endif
