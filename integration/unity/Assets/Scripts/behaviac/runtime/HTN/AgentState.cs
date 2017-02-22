/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions and limitations under
// the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define BEHAVIAC_ENABLE_PUSH_OPT

using System;
using System.Collections.Generic;

namespace behaviac
{
    public class Variables
    {
        public Variables(Dictionary<uint, IInstantiatedVariable> vars)
        {
            this.m_variables = vars;
        }

        public Variables()
        {
            this.m_variables = new Dictionary<uint, IInstantiatedVariable>();
        }

        public bool IsExisting(uint varId)
        {
            return this.m_variables.ContainsKey(varId);
        }

        public virtual IInstantiatedVariable GetVariable(uint varId)
        {
            if (this.m_variables != null && this.m_variables.ContainsKey(varId))
            {
                return this.m_variables[varId];
            }

            return null;
        }

        public virtual void AddVariable(uint varId, IInstantiatedVariable pVar, int stackIndex)
        {
            Debug.Check(!this.m_variables.ContainsKey(varId));

            this.m_variables[varId] = pVar;
        }

        public void Log(Agent agent)
        {
#if !BEHAVIAC_RELEASE
            var e = this.m_variables.Keys.GetEnumerator();

            while (e.MoveNext())
            {
                uint id = e.Current;
                IInstantiatedVariable pVar = this.m_variables[id];

                pVar.Log(agent);
            }

#endif
        }

        public void UnLoad(string variableName)
        {
            Debug.Check(!string.IsNullOrEmpty(variableName));

            uint varId = Utils.MakeVariableId(variableName);

            if (this.m_variables.ContainsKey(varId))
            {
                this.m_variables.Remove(varId);
            }
        }

        public void Unload()
        {
        }

        public void CopyTo(Agent pAgent, Variables target)
        {
            target.m_variables.Clear();

            var e = this.m_variables.Keys.GetEnumerator();

            while (e.MoveNext())
            {
                uint id = e.Current;
                IInstantiatedVariable pVar = this.m_variables[id];
                IInstantiatedVariable pNew = pVar.clone();

                target.m_variables[id] = pNew;
            }

            if (!Object.ReferenceEquals(pAgent, null))
            {
                e = target.m_variables.Keys.GetEnumerator();

                while (e.MoveNext())
                {
                    uint id = e.Current;
                    IInstantiatedVariable pVar = this.m_variables[id];

                    pVar.CopyTo(pAgent);
                }
            }
        }

        private void Save(ISerializableNode node)
        {
            CSerializationID variablesId = new CSerializationID("vars");
            ISerializableNode varsNode = node.newChild(variablesId);

            var e = this.m_variables.Values.GetEnumerator();

            while (e.MoveNext())
            {
                e.Current.Save(varsNode);
            }
        }

        protected Dictionary<uint, IInstantiatedVariable> m_variables = new Dictionary<uint, IInstantiatedVariable>();

        public Dictionary<uint, IInstantiatedVariable> Vars
        {
            get
            {
                return this.m_variables;
            }
        }
    }

#if BEHAVIAC_USE_HTN
    public class AgentState : Variables, IDisposable
    {
        private List<AgentState> state_stack = null;

        public AgentState(Dictionary<uint, IInstantiatedVariable> vars) : base(vars)
        {

        }

        public AgentState()
        {
        }

        public AgentState(AgentState parent)
        {
            this.parent = parent;
        }

        public void Dispose()
        {
            this.Pop();
        }

        private static Stack<AgentState> pool = new Stack<AgentState>();

        private AgentState parent = null;

#if BEHAVIAC_ENABLE_PUSH_OPT
        private bool m_forced;
        private int m_pushed;
#endif

        public int Depth
        {
            get
            {
                int d = 1;

                if (this.state_stack != null && this.state_stack.Count > 0)
                {
                    for (int i = this.state_stack.Count - 1; i >= 0; --i)
                    {
                        AgentState t = this.state_stack[i];
                        d += 1 + t.m_pushed;
                    }
                }

                return d;
            }
        }

        public int Top
        {
            get
            {
                if (this.state_stack != null && this.state_stack.Count > 0)
                {
                    return this.state_stack.Count - 1;
                }

                return -1;
            }
        }

        public override void AddVariable(uint varId, IInstantiatedVariable pVar, int stackIndex)
        {
            if (this.state_stack != null && this.state_stack.Count > 0 &&
                stackIndex > 0 && stackIndex < this.state_stack.Count)
            {
                AgentState t = this.state_stack[stackIndex];
                t.AddVariable(varId, pVar, -1);
            }
            else
            {
                base.AddVariable(varId, pVar, -1);
            }
        }

        public override IInstantiatedVariable GetVariable(uint varId)
        {
            if (this.state_stack != null && this.state_stack.Count > 0)
            {
                for (int i = this.state_stack.Count - 1; i >= 0; --i)
                {
                    AgentState t = this.state_stack[i];

                    IInstantiatedVariable pVar = t.GetVariable(varId);

                    if (pVar != null)
                    {
                        return pVar;
                    }
                }
            }

            return base.GetVariable(varId);
        }


        public AgentState Push(bool bForcePush)
        {
#if BEHAVIAC_ENABLE_PUSH_OPT

            if (!bForcePush)
            {
                //if the top has nothing new added, to use it again
                if (this.state_stack != null && this.state_stack.Count > 0)
                {
                    AgentState t = this.state_stack[this.state_stack.Count - 1];

                    if (!t.m_forced && t.m_variables.Count == 0)
                    {
                        t.m_pushed++;
                        return t;
                    }
                }
            }

#endif

            AgentState newly = null;

            lock (pool)
            {
                if (pool.Count > 0)
                {
                    newly = pool.Pop();
                    //set the parent
                    newly.parent = this;
                }
                else
                {
                    newly = new AgentState(this);
                }

                newly.m_forced = bForcePush;

                if (bForcePush)
                {
                    //base.CopyTo(null, newly);
                    this.CopyTopValueTo(newly);
                }
            }

            if (this.state_stack == null)
            {
                this.state_stack = new List<AgentState>();
            }

            //add the newly one at the end of the list as the top
            this.state_stack.Add(newly);

            return newly;
        }

        private void CopyTopValueTo(AgentState newly)
        {
            var e = this.m_variables.Keys.GetEnumerator();

            while (e.MoveNext())
            {
                uint id = e.Current;

                IInstantiatedVariable pVar = this.GetVariable(id);
                IInstantiatedVariable pNew = pVar.clone();

                newly.m_variables[id] = pNew;
            }
        }

        public void Pop()
        {
#if BEHAVIAC_ENABLE_PUSH_OPT

            if (this.m_pushed > 0)
            {
                this.m_pushed--;

                if (this.m_variables.Count > 0)
                {
                    this.m_variables.Clear();
                    return;
                }

                return;
            }

#endif

            if (this.state_stack != null && this.state_stack.Count > 0)
            {
                AgentState top = this.state_stack[this.state_stack.Count - 1];
                top.Pop();
                return;
            }

#if BEHAVIAC_ENABLE_PUSH_OPT
            this.m_pushed = 0;
            this.m_forced = false;
#endif

            if (this.state_stack != null)
            {
                this.state_stack.Clear();
            }

            this.m_variables.Clear();

            Debug.Check(this.state_stack == null);
            Debug.Check(this.parent != null);

            this.parent.PopTop();
            this.parent = null;

            lock (pool)
            {
                Debug.Check(!pool.Contains(this));
                pool.Push(this);
            }
        }

        private void PopTop()
        {
            Debug.Check(this.state_stack != null);
            Debug.Check(this.state_stack.Count > 0);
            //remove the last one
            this.state_stack.RemoveAt(this.state_stack.Count - 1);
        }

    }
#endif//BEHAVIAC_USE_HTN
}
