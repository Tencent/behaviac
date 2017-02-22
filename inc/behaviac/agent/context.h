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

#ifndef _BEHAVIAC_CONTEXT_H_
#define _BEHAVIAC_CONTEXT_H_

#include "behaviac/common/base.h"
#include "behaviac/common/rttibase.h"
#include "behaviac/common/factory.h"
#include "behaviac/common/object/tagobject.h"
#include "behaviac/common/string/stringcrc.h"
#include "behaviac/common/string/stringutils.h"

#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/agent/state.h"
//#include "behaviac/agent/context.h"
namespace behaviac {
    class Agent;
    class BehaviorTreeTask;
    class Variables;
    class State_t;

    /*! \addtogroup Agent
    * @{
    * \addtogroup Context
    * @{ */

    /// The Context class
    /*!
    */
    class BEHAVIAC_API Context {
    private:
        typedef behaviac::map<int, Context*> Contexts_t;

        static Contexts_t* ms_contexts;

        void LogCurrentState();

    public:
        static void execAgents(int contextId);
        static Context& GetContext(int contextId);

        void AddAgent(Agent* pAgent);
        void RemoveAgent(Agent* pAgent);

        bool IsExecuting();

        /**
        to cleanup the specified context.

        by default, contextId = -1, it cleans up all the contexts
        */
        static void Cleanup(int contextId = -1);

        static void LogCurrentStates(int contextId);

        virtual ~Context();

        /**
        log changed static variables(propery) for the specified agent class or all agent classes

        @param agentClassName
        if null, it logs for all the agent class
        */
        void LogStaticVariables(const char* agentClassName = 0);

        /**
        bind 'agentInstanceName' to 'pAgentInstance'.
        'agentInstanceName' should have been registered to the class of 'pAgentInstance' or its parent class.

        @sa RegisterInstanceName
        */
        bool BindInstance(const char* agentInstanceName, Agent* pAgentInstance);

        /**
        unbind 'agentInstanceName' from 'pAgentInstance'.
        'agentInstanceName' should have been bound to 'pAgentInstance'.

        @sa RegisterInstanceName, BindInstance, CreateInstance
        */
        bool UnbindInstance(const char* agentInstanceName);

        Agent* GetInstance(const char* agentInstanceName);

        bool Save(States_t& states);
        bool Load(const States_t& states);

        typedef behaviac::map<int, Agent*> Agents_t;
        struct HeapItem_t {
            int priority;
            Agents_t agents;
        };
        behaviac::vector<HeapItem_t> m_agents;
        void SetAgents(behaviac::vector<HeapItem_t> value);
        behaviac::vector<HeapItem_t> GetAgents();

        struct HeapFinder_t {
            int priority;
            HeapFinder_t(int p) : priority(p)
            {}

            bool operator()(const HeapItem_t& item) const {
                return item.priority == priority;
            }
        };

        struct  HeapCompare_t {
            bool operator()(const HeapItem_t& lhs, const HeapItem_t& rhs) const {
                return lhs.priority < rhs.priority;
            }
        };

    protected:
        Context(int contextId);

        void CleanupStaticVariables();
        void CleanupInstances();

        void execAgents_();

    private:
        void DelayProcessingAgents();
        void addAgent_(Agent* pAgent);
        void removeAgent_(Agent* pAgent);

        behaviac::vector<Agent*> delayAddedAgents;
        behaviac::vector<Agent*> delayRemovedAgents;

        typedef behaviac::map<behaviac::string, Agent*> NamedAgents_t;
        NamedAgents_t m_namedAgents;

        typedef behaviac::map<behaviac::string, Variables> AgentTypeStaticVariables_t;
        AgentTypeStaticVariables_t	m_static_variables;

        int     m_context_id;
        bool    m_bCreatedByMe;
        bool	m_IsExecuting;
    };
    /*! @} */
    /*! @} */
}

#include "context.inl"

#endif//#ifndef _BEHAVIAC_CONTEXT_H_
