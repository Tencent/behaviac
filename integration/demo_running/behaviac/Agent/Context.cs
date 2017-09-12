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

using System;
using System.Collections.Generic;

namespace behaviac
{
    public class Context
    {
        private static Dictionary<int, Context> ms_contexts = new Dictionary<int, Context>();

        private Dictionary<string, Agent> m_namedAgents = new Dictionary<string, Agent>();

        public struct HeapItem_t : IComparable<HeapItem_t>
        {
            public int priority;
            public Dictionary<int, Agent> agents;

            public int CompareTo(HeapItem_t other)
            {
                if (this.priority < other.priority)
                {
                    return -1;
                }
                else if (this.priority > other.priority)
                {
                    return 1;
                }

                return 0;
            }
        }

        private List<HeapItem_t> m_agents;

        public List<HeapItem_t> Agents
        {
            get
            {
                if (m_agents == null)
                {
                    m_agents = new List<HeapItem_t>();
                }

                return m_agents;
            }
            set
            {
                m_agents = value;
            }
        }

        private int m_context_id = -1;
        private bool m_IsExecuting = false;

        private Context(int contextId)
        {
            m_context_id = contextId;
            m_IsExecuting = false;
        }

        //~Context()
        //{
        //    m_IsExecuting = false;

        //    delayAddedAgents.Clear();
        //    delayRemovedAgents.Clear();

        //    this.CleanupInstances();
        //}

        private int GetContextId()
        {
            return this.m_context_id;
        }

        public static Context GetContext(int contextId)
        {
            Debug.Check(contextId >= 0);

            if (ms_contexts.ContainsKey(contextId))
            {
                Context pContext = ms_contexts[contextId];
                return pContext;
            }

            Context pC = new Context(contextId);
            ms_contexts[contextId] = pC;

            return pC;
        }

        public static void Cleanup(int contextId)
        {
            if (ms_contexts != null)
            {
                if (contextId == -1)
                {
                    ms_contexts.Clear();
                    //ms_contexts = null;
                }
                else
                {
                    if (ms_contexts.ContainsKey(contextId))
                    {
                        ms_contexts.Remove(contextId);
                    }
                    else
                    {
                        Debug.Check(false, "unused context id");
                    }
                }
            }
        }

        private List<Agent> delayAddedAgents = new List<Agent>();
        private List<Agent> delayRemovedAgents = new List<Agent>();

        public static void AddAgent(Agent pAgent)
        {
            if (!Object.ReferenceEquals(pAgent, null))
            {
                Context c = Context.GetContext(pAgent.GetContextId());

                if (c != null)
                {
                    if (c.m_IsExecuting)
                    {
                        c.delayAddedAgents.Add(pAgent);
                    }
                    else
                    {
                        c.addAgent_(pAgent);
                    }
                }
            }
        }

        public static void RemoveAgent(Agent pAgent)
        {
            if (!Object.ReferenceEquals(pAgent, null))
            {
                Context c = Context.GetContext(pAgent.GetContextId());

                if (c != null)
                {
                    if (c.m_IsExecuting)
                    {
                        c.delayRemovedAgents.Add(pAgent);
                    }
                    else
                    {
                        c.removeAgent_(pAgent);
                    }
                }
            }
        }

        private void DelayProcessingAgents()
        {
            if (delayAddedAgents.Count > 0)
            {
                for (int i = 0; i < delayAddedAgents.Count; ++i)
                {
                    addAgent_(delayAddedAgents[i]);
                }

                delayAddedAgents.Clear();
            }

            if (delayRemovedAgents.Count > 0)
            {
                for (int i = 0; i < delayRemovedAgents.Count; ++i)
                {
                    removeAgent_(delayRemovedAgents[i]);
                }

                delayRemovedAgents.Clear();
            }
        }

        private void addAgent_(Agent pAgent)
        {
            int agentId = pAgent.GetId();
            int priority = pAgent.GetPriority();
            int itemIndex = this.Agents.FindIndex(delegate(HeapItem_t h)
            {
                return h.priority == priority;
            });

            if (itemIndex == -1)
            {
                HeapItem_t pa = new HeapItem_t();
                pa.agents = new Dictionary<int, Agent>();
                pa.priority = priority;
                pa.agents[agentId] = pAgent;
                this.Agents.Add(pa);
            }
            else
            {
                this.Agents[itemIndex].agents[agentId] = pAgent;
            }
        }

        private void removeAgent_(Agent pAgent)
        {
            int agentId = pAgent.GetId();
            int priority = pAgent.GetPriority();
            int itemIndex = this.Agents.FindIndex(delegate(HeapItem_t h)
            {
                return h.priority == priority;
            });

            if (itemIndex != -1)
            {
                if (this.Agents[itemIndex].agents.ContainsKey(agentId))
                {
                    this.Agents[itemIndex].agents.Remove(agentId);
                }
            }
        }

        public static void execAgents(int contextId)
        {
            if (contextId >= 0)
            {
                Context pContext = Context.GetContext(contextId);

                if (pContext != null)
                {
                    pContext.execAgents_();
                }
            }
            else
            {
                var e = ms_contexts.GetEnumerator();

                while (e.MoveNext())
                {
                    Context pContext = e.Current.Value;

                    if (pContext != null)
                    {
                        pContext.execAgents_();
                    }
                }
            }
        }

        private void execAgents_()
        {
            if (!Workspace.Instance.IsExecAgents)
            {
                return;
            }

            m_IsExecuting = true;

            this.Agents.Sort();

            for (int i = 0; i < this.Agents.Count; ++i)
            {
                HeapItem_t pa = this.Agents[i];
                var e = pa.agents.GetEnumerator();

                while (e.MoveNext())
                {
                    Agent pAgent = e.Current.Value;
                    if (pAgent.IsActive())
                    {
                        pAgent.btexec();

                        // in case IsExecAgents was set to false by pA's bt
                        if (!Workspace.Instance.IsExecAgents)
                        {
                            break;
                        }
                    }
                }
            }

            m_IsExecuting = false;

            this.DelayProcessingAgents();
        }

        private void LogCurrentState()
        {
            string msg = string.Format("LogCurrentStates {0} {1}", this.m_context_id, this.Agents.Count);
            behaviac.Debug.Log(msg);

            //force to log vars
            for (int i = 0; i < this.Agents.Count; ++i)
            {
                HeapItem_t pa = this.Agents[i];
                var e = pa.agents.Values.GetEnumerator();

                while (e.MoveNext())
                {
                    if (e.Current.IsMasked())
                    {
                        e.Current.LogVariables(true);

                        e.Current.LogRunningNodes();
                    }
                }
            }
        }

        public static void LogCurrentStates(int contextId)
        {
            Debug.Check(ms_contexts != null);

            if (contextId >= 0)
            {
                Context pContext = Context.GetContext(contextId);

                if (pContext != null)
                {
                    pContext.LogCurrentState();
                }
            }
            else
            {
                var e = ms_contexts.Values.GetEnumerator();

                while (e.MoveNext())
                {
                    e.Current.LogCurrentState();
                }
            }
        }

        private void CleanupInstances()
        {
            //foreach (KeyValuePair<string, Agent> p in m_namedAgents)
            //{
            //    string msg = string.Format("{0}:{1}", p.Key,p.Value.GetName());
            //    behaviac.Debug.Log(msg);
            //}

            //Debug.Check(m_namedAgents.Count == 0, "you need to call DestroyInstance or UnbindInstance");

            m_namedAgents.Clear();
        }

        private static bool GetClassNameString(string variableName, ref string className)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            int pSep = variableName.LastIndexOf(':');

            if (pSep > 0)
            {
                Debug.Check(variableName[pSep - 1] == ':');
                className = variableName.Substring(0, pSep - 1);

                return true;
            }

            className = variableName;
            return true;
        }

        /**
        bind 'agentInstanceName' to 'pAgentInstance'.
        'agentInstanceName' should have been registered to the class of 'pAgentInstance' or its parent class.

        @sa RegisterInstanceName
        */

        public bool BindInstance(Agent pAgentInstance, string agentInstanceName)
        {
            if (string.IsNullOrEmpty(agentInstanceName))
            {
                agentInstanceName = pAgentInstance.GetType().FullName;
            }

            if (Agent.IsNameRegistered(agentInstanceName))
            {
                Debug.Check(GetInstance(agentInstanceName) == null, "the name has been bound to an instance already!");

                string className = Agent.GetRegisteredClassName(agentInstanceName);

                if (Agent.IsDerived(pAgentInstance, className))
                {
                    m_namedAgents[agentInstanceName] = pAgentInstance;

                    return true;
                }
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        public bool BindInstance(Agent pAgentInstance)
        {
            return BindInstance(pAgentInstance, null);
        }

        /**
        unbind 'agentInstanceName' from 'pAgentInstance'.
        'agentInstanceName' should have been bound to 'pAgentInstance'.

        @sa RegisterInstanceName, BindInstance, CreateInstance
        */

        public bool UnbindInstance(string agentInstanceName)
        {
            Debug.Check(!string.IsNullOrEmpty(agentInstanceName));

            if (Agent.IsNameRegistered(agentInstanceName))
            {
                if (m_namedAgents.ContainsKey(agentInstanceName))
                {
                    m_namedAgents.Remove(agentInstanceName);

                    return true;
                }
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        public bool UnbindInstance<T>()
        {
            string agentInstanceName = typeof(T).FullName;
            return UnbindInstance(agentInstanceName);
        }

        public Agent GetInstance(string agentInstanceName)
        {
            bool bValidName = !string.IsNullOrEmpty(agentInstanceName);

            if (bValidName)
            {
                string className = null;
                GetClassNameString(agentInstanceName, ref className);

                if (m_namedAgents.ContainsKey(className))
                {
                    Agent pA = m_namedAgents[className];

                    return pA;
                }

                return null;
            }

            return null;
        }
    }
}
