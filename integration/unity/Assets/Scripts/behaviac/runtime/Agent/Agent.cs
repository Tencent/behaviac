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

#if BEHAVIAC_CS_ONLY || BEHAVIAC_NOT_USE_UNITY
#define BEHAVIAC_NOT_USE_MONOBEHAVIOUR
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace behaviac
{
    [behaviac.TypeMetaInfo()]
#if BEHAVIAC_NOT_USE_MONOBEHAVIOUR
    public class Agent
#else
    public class Agent : UnityEngine.MonoBehaviour
#endif
    {
        #region State

        public class State_t
        {
            protected Variables m_vars = new Variables();

            public Variables Vars
            {
                get
                {
                    return this.m_vars;
                }
            }

            protected BehaviorTreeTask m_bt;

            public BehaviorTreeTask BT
            {
                get
                {
                    return m_bt;
                }
                set
                {
                    m_bt = value;
                }
            }

            public State_t(State_t c)
            {
                c.m_vars.CopyTo(null, this.m_vars);

                if (c.m_bt != null)
                {
                    BehaviorNode pNode = c.m_bt.GetNode();

                    if (pNode != null)
                    {
                        this.m_bt = (BehaviorTreeTask)pNode.CreateAndInitTask();

                        c.m_bt.CopyTo(this.m_bt);
                    }
                }
            }

            //~State_t()
            //{
            //    this.Clear(true);
            //}

            public bool SaveToFile(string fileName)
            {
                //XmlNodeRef xmlInfo = CreateXmlNode("AgentState");

                //CTextNode node(xmlInfo);

                //this.m_vars.Save(node);

                //if (this.m_bt)
                //{
                //    this.m_bt.Save(node);
                //}

                //CFileSystem::MakeSureDirectoryExist(fileName);
                //return xmlInfo.saveToFile(fileName);
                return false;
            }

            public bool LoadFromFile(string fileName)
            {
                //XmlNodeRef xmlInfo = CreateXmlNode("AgentState");

                //CTextNode node(xmlInfo);

                //if (node.LoadFromFile(fileName))
                //{
                //    this.m_vars.Load(&node);

                //    return true;
                //}

                return false;
            }
        }

        #endregion State

#if BEHAVIAC_NOT_USE_MONOBEHAVIOUR
        protected Agent()
        {
            Init();
        }

        ~Agent()
        {
            OnDestroy();
        }
#endif

        protected void Init()
        {
            Awake();
        }

        void Awake()
        {
            Init_(this.m_contextId, this, this.m_priority);

#if !BEHAVIAC_RELEASE
            //this.SetName(this.name);
            this._members.Clear();
#endif
        }

#if BEHAVIAC_NOT_USE_MONOBEHAVIOUR
        private string name;
#endif

        void OnDestroy()
        {
#if !BEHAVIAC_RELEASE
            string agentClassName = this.GetClassTypeName();
            string agentInstanceName = this.GetName();

            string aName = string.Format("{0}#{1}", agentClassName, agentInstanceName);

            ms_agents.Remove(aName);
#endif

            Context.RemoveAgent(this);

            if (this.m_behaviorTreeTasks != null)
            {
                for (int i = 0; i < this.m_behaviorTreeTasks.Count; ++i)
                {
                    BehaviorTreeTask bt = this.m_behaviorTreeTasks[i];
                    Workspace.Instance.DestroyBehaviorTreeTask(bt, this);
                }

                this.m_behaviorTreeTasks.Clear();
                this.m_behaviorTreeTasks = null;
            }
        }

#if !BEHAVIAC_RELEASE
        private static Dictionary<string, Agent> ms_agents = new Dictionary<string, Agent>();

        public static Agent GetAgent(string agentName)
        {
            Agent pAgent = Agent.GetInstance(agentName);

            if (!System.Object.ReferenceEquals(pAgent, null))
            {
                return pAgent;
            }

            if (ms_agents.ContainsKey(agentName))
            {
                Agent pA = ms_agents[agentName];
                return pA;
            }

            return null;
        }
#endif//BEHAVIAC_RELEASE

        private List<BehaviorTreeTask> m_behaviorTreeTasks;

        private List<BehaviorTreeTask> BehaviorTreeTasks
        {
            get
            {
                if (m_behaviorTreeTasks == null)
                {
                    m_behaviorTreeTasks = new List<BehaviorTreeTask>();
                }

                return m_behaviorTreeTasks;
            }
        }

        private class BehaviorTreeStackItem_t
        {
            public BehaviorTreeTask bt;
            public TriggerMode triggerMode;
            public bool triggerByEvent;

            public BehaviorTreeStackItem_t(BehaviorTreeTask bt_, TriggerMode tm, bool bByEvent)
            {
                bt = bt_;
                triggerMode = tm;
                triggerByEvent = bByEvent;
            }
        }

        private List<BehaviorTreeStackItem_t> m_btStack;

        private List<BehaviorTreeStackItem_t> BTStack
        {
            get
            {
                if (m_btStack == null)
                {
                    m_btStack = new List<BehaviorTreeStackItem_t>();
                }

                return m_btStack;
            }
        }

        private BehaviorTreeTask m_currentBT;
        public BehaviorTreeTask CurrentTreeTask
        {
            get
            {
                return m_currentBT;
            }

            private set
            {
                m_currentBT = value;
                //m_excutingTreeTask = m_currentBT;
            }
        }

        private BehaviorTreeTask m_excutingTreeTask;
        public BehaviorTreeTask ExcutingTreeTask
        {
            get
            {
                return m_excutingTreeTask;
            }
            set
            {
                m_excutingTreeTask = value;
            }
        }

        private int m_id = -1;
        private bool m_bActive = true;

        private bool m_referencetree = false;

        public int m_priority;
        public int m_contextId;

        public int GetId()
        {
            return this.m_id;
        }

        public int GetPriority()
        {
            return (int)this.m_priority;
        }

        public string GetClassTypeName()
        {
            return this.GetType().FullName;
        }

        private static uint ms_idMask = 0xffffffff;
        private uint m_idFlag = 0xffffffff;

        /**
        Each agent can be assigned to an id flag by 'SetIdFlag'. A global mask can be specified by SetIdMask.
        the id flag will be checked with this global mask.

        @sa SetIdFlag SetIdMask
        */

        public bool IsMasked()
        {
            return (this.m_idFlag & Agent.IdMask()) != 0;
        }

        /**
        @sa SetIdMask IsMasked
        */

        public void SetIdFlag(uint idMask)
        {
            this.m_idFlag = idMask;
        }

        public static bool IsDerived(Agent pAgent, string agentType)
        {
            bool bIsDerived = false;
            Type type = pAgent.GetType();

            while (type != null)
            {
                if (type.FullName == agentType)
                {
                    bIsDerived = true;
                    break;
                }

                type = type.BaseType;
            }

            return bIsDerived;
        }

        /**
        @sa SetIdFlag IsMasked
        */

        public static void SetIdMask(uint idMask)
        {
            ms_idMask = idMask;
        }

        public static uint IdMask()
        {
            return ms_idMask;
        }

#if !BEHAVIAC_RELEASE
        private string m_debug_name;
#endif

        public string GetName()
        {
#if !BEHAVIAC_RELEASE

            if (!string.IsNullOrEmpty(this.m_debug_name))
            {
                return this.m_debug_name;
            }

            return this.name;
#else
            return this.name;
#endif
        }

        private static int ms_agent_index;
        private static Dictionary<string, int> ms_agent_type_index;

        public void SetName(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                int typeId = 0;
                string typeFullName = this.GetType().FullName;
                string typeName = null;
                int pIt = typeFullName.LastIndexOf(':');

                if (pIt != -1)
                {
                    typeName = typeFullName.Substring(pIt + 1);
                }
                else
                {
                    typeName = typeFullName;
                }

                if (ms_agent_type_index == null)
                {
                    ms_agent_type_index = new Dictionary<string, int>();
                }

                if (!ms_agent_type_index.ContainsKey(typeFullName))
                {
                    typeId = 0;
                    ms_agent_type_index[typeFullName] = 1;
                }
                else
                {
                    typeId = ms_agent_type_index[typeFullName]++;
                }

                this.name += string.Format("{0}_{1}_{2}", typeName, typeId, this.m_id);
            }
            else
            {
                this.name = instanceName;
            }

#if !BEHAVIAC_RELEASE
            this.m_debug_name = this.name.Replace(" ", "_");
#endif
        }

        public int GetContextId()
        {
            return this.m_contextId;
        }

        /**
        return if the agent is active or not.

        an active agent is ticked automatiacally by the world it is added.
        if it is inactive, it is not ticked automatiacally by the world it is added.

        @sa SetActive
        */

        public bool IsActive()
        {
            return this.m_bActive;
        }

        /**
        set the agent active or inactive
        */

        public void SetActive(bool bActive)
        {
            this.m_bActive = bActive;
        }

#if BEHAVIAC_USE_HTN
        private int m_planningTop = -1;

        internal int PlanningTop
        {
            get
            {
                return this.m_planningTop;
            }
            set
            {
                this.m_planningTop = value;
            }
        }
#endif//

        internal struct AgentName_t
        {
            public string instantceName_;
            public string className_;
            public string displayName_;
            public string desc_;

            public AgentName_t(string instanceName, string className,
                               string displayName, string desc)
            {
                instantceName_ = instanceName;
                className_ = className;

                if (!string.IsNullOrEmpty(displayName))
                {
                    displayName_ = displayName;
                }
                else
                {
                    displayName_ = instantceName_.Replace(".", "::");
                }

                if (!string.IsNullOrEmpty(desc))
                {
                    desc_ = desc;
                }
                else
                {
                    desc_ = displayName_;
                }
            }

            public string ClassName
            {
                get
                {
                    return this.className_;
                }
            }
        }

        private static Dictionary<string, AgentName_t> ms_names;

        internal static Dictionary<string, AgentName_t> Names
        {
            get
            {
                if (ms_names == null)
                {
                    ms_names = new Dictionary<string, AgentName_t>();
                }

                return ms_names;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////
        /**
        A name can be bound to an instance. before a name is bound to an instance, that name has to be registered by 'RegisterInstanceName'

        @param agentInstanceName
        the specified name to be used to access an instance of type 'TAGENT' or its derivative.
        if 'agentInstanceName' is 0, the class name of 'TAGENT' will be used to be registered.

        @sa CreateInstance
        */
        public static bool RegisterInstanceName<TAGENT>(string agentInstanceName, string displayName, string desc) where TAGENT : Agent
        {
            Debug.Check(string.IsNullOrEmpty(agentInstanceName) || !agentInstanceName.Contains(" "));

            string agentInstanceNameAny = agentInstanceName;

            if (string.IsNullOrEmpty(agentInstanceNameAny))
            {
                agentInstanceNameAny = typeof(TAGENT).FullName;
            }

            if (!Agent.Names.ContainsKey(agentInstanceNameAny))
            {
                string className = typeof(TAGENT).FullName;
                Agent.Names[agentInstanceNameAny] = new AgentName_t(agentInstanceNameAny, className, displayName, desc);

                return true;
            }

            return false;
        }

        public static bool RegisterInstanceName<TAGENT>(string agentInstanceName) where TAGENT : Agent
        {
            return Agent.RegisterInstanceName<TAGENT>(agentInstanceName, null, null);
        }

        public static bool RegisterInstanceName<TAGENT>() where TAGENT : Agent
        {
            return Agent.RegisterInstanceName<TAGENT>(null, null, null);
        }

        public static bool RegisterStaticClass(Type type, string displayName, string desc)
        {
            Debug.Check(Utils.IsStaticType(type));

            string agentInstanceNameAny = type.FullName;

            if (!Agent.Names.ContainsKey(agentInstanceNameAny))
            {
                Agent.Names[agentInstanceNameAny] = new AgentName_t(agentInstanceNameAny, agentInstanceNameAny, displayName, desc);
                Utils.AddStaticClass(type);

                return true;
            }

            return false;
        }

        public static void UnRegisterInstanceName<TAGENT>(string agentInstanceName) where TAGENT : Agent
        {
            string agentInstanceNameAny = agentInstanceName;

            if (string.IsNullOrEmpty(agentInstanceNameAny))
            {
                agentInstanceNameAny = typeof(TAGENT).FullName;
            }

            if (Agent.Names.ContainsKey(agentInstanceNameAny))
            {
                Agent.Names.Remove(agentInstanceNameAny);
            }
        }

        public static void UnRegisterInstanceName<TAGENT>() where TAGENT : Agent
        {
            Agent.UnRegisterInstanceName<TAGENT>(null);
        }


        /**
        return if 'agentInstanceName' is registerd.

        @sa RegisterInstanceName
        */

        public static bool IsNameRegistered(string agentInstanceName)
        {
            return Names.ContainsKey(agentInstanceName);
        }

        /**
        return the registered class name

        @sa RegisterInstanceName
        */

        public static string GetRegisteredClassName(string agentInstanceName)
        {
            if (Names.ContainsKey(agentInstanceName))
            {
                return Names[agentInstanceName].ClassName;
            }

            return null;
        }

        /**
        bind 'agentInstanceName' to 'pAgentInstance'.
        'agentInstanceName' should have been registered to the class of 'pAgentInstance' or its parent class.

        if 'agentInstanceName' is not specified, the class type name of 'pAgentInstance' will be used.
        @sa RegisterInstanceName
        */

        public static bool BindInstance(Agent pAgentInstance, string agentInstanceName, int contextId)
        {
            Context c = Context.GetContext(contextId);

            if (c != null)
            {
                return c.BindInstance(pAgentInstance, agentInstanceName);
            }

            return false;
        }

        public static bool BindInstance(Agent pAgentInstance, string agentInstanceName)
        {
            return Agent.BindInstance(pAgentInstance, agentInstanceName, 0);
        }

        /**
        bind 'pAgentInstance' to the class type name of 'pAgentInstance'.

        RegisterInstanceName<TAGENT>() should have been called to regiser 'the class type name'.
        @sa RegisterInstanceName
        */

        public static bool BindInstance(Agent pAgentInstance)
        {
            return Agent.BindInstance(pAgentInstance, null, 0);
        }

        /**
        unbind 'agentInstanceName' from 'pAgentInstance'.
        'agentInstanceName' should have been bound to 'pAgentInstance'.

        @sa RegisterInstanceName, BindInstance, CreateInstance
        */

        public static bool UnbindInstance(string agentInstanceName, int contextId)
        {
            Context c = Context.GetContext(contextId);

            if (c != null)
            {
                return c.UnbindInstance(agentInstanceName);
            }

            return false;
        }

        public static bool UnbindInstance(string agentInstanceName)
        {
            return Agent.UnbindInstance(agentInstanceName, 0);
        }

        public static bool UnbindInstance<T>()
        {
            string agentInstanceName = typeof(T).FullName;
            return Agent.UnbindInstance(agentInstanceName);
        }

        public static Agent GetInstance(string agentInstanceName, int contextId)
        {
            Context c = Context.GetContext(contextId);

            if (c != null)
            {
                return c.GetInstance(agentInstanceName);
            }

            return null;
        }

        public static Agent GetInstance(string agentInstanceName)
        {
            return Agent.GetInstance(agentInstanceName, 0);
        }

        public static TAGENT GetInstance<TAGENT>(string agentInstanceName, int contextId) where TAGENT : Agent, new()
        {
            string agentInstanceNameAny = agentInstanceName;

            if (string.IsNullOrEmpty(agentInstanceNameAny))
            {
                agentInstanceNameAny = typeof(TAGENT).FullName;
            }

            Agent a = Agent.GetInstance(agentInstanceNameAny, contextId);

            Debug.Check(System.Object.ReferenceEquals(a, null) || a is TAGENT);
            TAGENT pA = (TAGENT)a;
            return pA;
        }

        public static TAGENT GetInstance<TAGENT>(string agentInstanceName) where TAGENT : Agent, new()
        {
            return Agent.GetInstance<TAGENT>(agentInstanceName, 0);
        }

        public static TAGENT GetInstance<TAGENT>() where TAGENT : Agent, new()
        {
            return Agent.GetInstance<TAGENT>(null, 0);
        }

#if !BEHAVIAC_RELEASE
        public class CTagObjectDescriptor
        {
            public void Load(Agent parent, ISerializableNode node)
            {
                for (int i = 0; i < ms_members.Count; ++i)
                {
                    ms_members[i].Load(parent, node);
                }

                if (this.m_parent != null)
                {
                    this.m_parent.Load(parent, node);
                }
            }

            public void Save(Agent parent, ISerializableNode node)
            {
                if (this.m_parent != null)
                {
                    this.m_parent.Save(parent, node);
                }

                for (int i = 0; i < ms_members.Count; ++i)
                {
                    ms_members[i].Save(parent, node);
                }
            }

            public static void PushBackMember(List<CMemberBase> inMembers, CMemberBase toAddMember)
            {
                inMembers.Add(toAddMember);
            }

            public CMemberBase GetMember(string memberName)
            {
                if (this.ms_members != null)
                {
                    //CMemberBase pMember = this.ms_members.Find(delegate (CMemberBase m) {return m.GetName() == memberName;});
                    for (int i = 0; i < this.ms_members.Count; ++i)
                    {
                        CMemberBase pMember = this.ms_members[i];

                        if (pMember.Name == memberName)
                        {
                            return pMember;
                        }
                    }
                }

                if (this.m_parent != null)
                {
                    return this.m_parent.GetMember(memberName);
                }

                return null;
            }

            public CMemberBase GetMember(uint memberId)
            {
                if (this.ms_members != null)
                {
                    CMemberBase pMember = this.ms_members.Find(delegate(CMemberBase m)
                    {
                        return m.GetId().GetId() == memberId;
                    });

                    if (pMember != null)
                    {
                        return pMember;
                    }
                }

                if (this.m_parent != null)
                {
                    return this.m_parent.GetMember(memberId);
                }

                return null;
            }

            public List<CMemberBase> ms_members = new List<CMemberBase>();
            public List<CMethodBase> ms_methods = new List<CMethodBase>();

            public Type type;
            public string displayName;
            public string desc;
            public CTagObjectDescriptor m_parent = null;
        }

        public static CTagObjectDescriptor GetDescriptorByName(string className)
        {
            className = className.Replace("::", ".");
            className = className.Replace("+", ".");
            CStringID classNameid = new CStringID(className);

            if (Metas.ContainsKey(classNameid))
            {
                return Metas[classNameid];
            }

            CTagObjectDescriptor od = new CTagObjectDescriptor();
            Metas.Add(classNameid, od);

            return od;
        }

        private CTagObjectDescriptor m_objectDescriptor = null;

        public CTagObjectDescriptor GetDescriptor()
        {
            if (m_objectDescriptor == null)
            {
                m_objectDescriptor = Agent.GetDescriptorByName(this.GetType().FullName);
            }

            return m_objectDescriptor;
        }

        private static Dictionary<CStringID, CTagObjectDescriptor> ms_metas;

        public static Dictionary<CStringID, CTagObjectDescriptor> Metas
        {
            get
            {
                if (ms_metas == null)
                {
                    ms_metas = new Dictionary<CStringID, CTagObjectDescriptor>();
                }

                return ms_metas;
            }
        }

        //public static Type GetTypeFromName(string typeName)
        //{
        //    CStringID typeNameId = new CStringID(typeName);

        //    if (Metas.ContainsKey(typeNameId))
        //    {
        //        CTagObjectDescriptor objectDesc = Metas[typeNameId];

        //        return objectDesc.type;
        //    }

        //    return null;
        //}

        public static bool IsTypeRegisterd(string typeName)
        {
            CStringID typeId = new CStringID(typeName);

            return ms_metas.ContainsKey(typeId);
        }

        public CMemberBase FindMember(string propertyName)
        {
            uint propertyId = Utils.MakeVariableId(propertyName);

            CMemberBase m = this.FindMember(propertyId);
            return m;
        }

        public CMemberBase FindMember(uint propertyId)
        {
            CTagObjectDescriptor objectDesc = this.GetDescriptor();

            if (objectDesc != null)
            {
                CMemberBase pMember = objectDesc.GetMember(propertyId);

                return pMember;
            }

            return null;
        }

        private static int ParsePropertyNames(string fullPropertnName, ref string agentClassName)
        {
            //test_ns::AgentActionTest::Property1
            int pBeginProperty = fullPropertnName.LastIndexOf(':');

            if (pBeginProperty != -1)
            {
                //skip '::'
                Debug.Check(fullPropertnName[pBeginProperty] == ':' && fullPropertnName[pBeginProperty - 1] == ':');
                pBeginProperty += 1;

                int pos = pBeginProperty - 2;

                agentClassName = fullPropertnName.Substring(pBeginProperty);

                return pos;
            }

            return -1;
        }
#endif

        private Dictionary<uint, IInstantiatedVariable> GetCustomizedVariables()
        {
            string agentClassName = this.GetClassTypeName();
            uint agentClassId = Utils.MakeVariableId(agentClassName);
            AgentMeta meta = AgentMeta.GetMeta(agentClassId);

            if (meta != null)
            {
                Dictionary<uint, IInstantiatedVariable> vars = meta.InstantiateCustomizedProperties();

                return vars;
            }

            return null;
        }

#if BEHAVIAC_USE_HTN
        private AgentState m_variables = null;
        public AgentState Variables
        {
            get
            {
                if (m_variables == null)
                {
                    Dictionary<uint, IInstantiatedVariable> vars = this.GetCustomizedVariables();
                    this.m_variables = new AgentState(vars);
                }

                return m_variables;
            }
        }

#else
        private Variables m_variables = null;

        public Variables Variables
        {
            get
            {
                if (m_variables == null)
                {
                    Dictionary<uint, IInstantiatedVariable> vars = this.GetCustomizedVariables();

                    this.m_variables = new Variables(vars);
                }

                return m_variables;
            }
        }

#endif//


#if !BEHAVIAC_RELEASE
        //for log only, to remember its last value
        private Dictionary<uint, IValue> _members = new Dictionary<uint, IValue>();
#endif

        internal IInstantiatedVariable GetInstantiatedVariable(uint varId)
        {
            // local var
            if (this.ExcutingTreeTask != null && this.ExcutingTreeTask.LocalVars.ContainsKey(varId))
            {
                return this.ExcutingTreeTask.LocalVars[varId];
            }

            // customized var
            IInstantiatedVariable pVar = this.Variables.GetVariable(varId);

            return pVar;
        }

        private IProperty GetProperty(uint propId)
        {
            string className = this.GetClassTypeName();
            uint classId = Utils.MakeVariableId(className);
            AgentMeta meta = AgentMeta.GetMeta(classId);

            if (meta != null)
            {
                IProperty prop = meta.GetProperty(propId);

                if (prop != null)
                {
                    return prop;
                }
            }

            return null;
        }

        internal bool GetVarValue<VariableType>(uint varId, out VariableType value)
        {
            IInstantiatedVariable v = this.GetInstantiatedVariable(varId);

            if (v != null)
            {
                if (typeof(VariableType).IsValueType)
                {
                    Debug.Check(v is CVariable<VariableType>);

                    CVariable<VariableType> var = (CVariable<VariableType>)v;

                    if (var != null)
                    {
                        value = var.GetValue(this);
                        return true;
                    }
                }
                else
                {
                    value = (VariableType)v.GetValueObject(this);
                    return true;
                }
            }

            value = default(VariableType);
            return false;
        }

        private bool GetVarValue<VariableType>(uint varId, int index, out VariableType value)
        {
            IInstantiatedVariable v = this.GetInstantiatedVariable(varId);

            if (v != null)
            {
                Debug.Check(v is CArrayItemVariable<VariableType>);
                CArrayItemVariable<VariableType> arrayItemVar = (CArrayItemVariable<VariableType>)v;

                if (arrayItemVar != null)
                {
                    value = arrayItemVar.GetValue(this, index);
                    return true;
                }
            }

            value = default(VariableType);
            return false;
        }

        internal bool SetVarValue<VariableType>(uint varId, VariableType value)
        {
            IInstantiatedVariable v = this.GetInstantiatedVariable(varId);

            if (v != null)
            {
                Debug.Check(v is CVariable<VariableType>);
                CVariable<VariableType> var = (CVariable<VariableType>)v;

                if (var != null)
                {
                    var.SetValue(this, value);
                    return true;
                }
            }

            return false;
        }

        private bool SetVarValue<VariableType>(uint varId, int index, VariableType value)
        {
            IInstantiatedVariable v = this.GetInstantiatedVariable(varId);

            if (v != null)
            {
                Debug.Check(v is CArrayItemVariable<VariableType>);
                CArrayItemVariable<VariableType> arrayItemVar = (CArrayItemVariable<VariableType>)v;

                if (arrayItemVar != null)
                {
                    arrayItemVar.SetValue(this, value, index);
                    return true;
                }
            }

            return false;
        }

        public bool IsValidVariable(string variableName)
        {
            uint variableId = Utils.MakeVariableId(variableName);

            IInstantiatedVariable v = this.GetInstantiatedVariable(variableId);

            if (v != null)
            {
                return true;
            }

            IProperty prop = this.GetProperty(variableId);
            return (prop != null);
        }

        public VariableType GetVariable<VariableType>(string variableName)
        {
            uint variableId = Utils.MakeVariableId(variableName);

            return GetVariable<VariableType>(variableId);
        }

        internal VariableType GetVariable<VariableType>(uint variableId)
        {
            VariableType value;

            // var
            if (this.GetVarValue<VariableType>(variableId, out value))
            {
                return value;
            }

            // property
            IProperty prop = this.GetProperty(variableId);

            if (prop != null)
            {
                if (typeof(VariableType).IsValueType)
                {
                    Debug.Check(prop is CProperty<VariableType>);
                    CProperty<VariableType> p = (CProperty<VariableType>)prop;
                    Debug.Check(p != null);

                    if (p != null)
                    {
                        return p.GetValue(this);
                    }
                }
                else
                {
                    return (VariableType)prop.GetValueObject(this);
                }
            }

            Debug.Check(false, string.Format("The variable \"{0}\" with type \"{1}\" can not be found!", variableId, typeof(VariableType).Name));
            return default(VariableType);
        }

        internal VariableType GetVariable<VariableType>(uint variableId, int index)
        {
            VariableType value;

            // var
            if (this.GetVarValue<VariableType>(variableId, index, out value))
            {
                return value;
            }

            // property
            IProperty prop = this.GetProperty(variableId);

            if (prop != null)
            {
                if (typeof(VariableType).IsValueType)
                {
                    Debug.Check(prop is CProperty<VariableType>);
                    CProperty<VariableType> p = (CProperty<VariableType>)prop;

                    if (p != null)
                    {
                        return p.GetValue(this, index);
                    }
                }
                else
                {
                    return (VariableType)prop.GetValueObject(this, index);
                }
            }

            Debug.Check(false, string.Format("The variable \"{0}\" with type \"{1}\" can not be found!", variableId, typeof(VariableType).Name));
            return default(VariableType);
        }

        public void SetVariable<VariableType>(string variableName, VariableType value)
        {
            uint variableId = Utils.MakeVariableId(variableName);

            SetVariable<VariableType>(variableName, variableId, value);
        }

        public void SetVariable<VariableType>(string variableName, uint variableId, VariableType value)
        {
            if (variableId == 0)
            {
                variableId = Utils.MakeVariableId(variableName);
            }

            // var
            if (this.SetVarValue<VariableType>(variableId, value))
            {
                return;
            }

            // property
            IProperty prop = this.GetProperty(variableId);

            if (prop != null)
            {
                Debug.Check(prop is CProperty<VariableType>);
                CProperty<VariableType> p = (CProperty<VariableType>)prop;

                if (p != null)
                {
                    p.SetValue(this, value);
                    return;
                }
            }

            Debug.Check(false, string.Format("The variable \"{0}\" with type \"{1}\" can not be found! please check the variable name or be after loading type info(btload)!", variableName, typeof(VariableType).Name));
        }

        public void SetVariable<VariableType>(string variableName, uint variableId, VariableType value, int index)
        {
            if (variableId == 0)
            {
                variableId = Utils.MakeVariableId(variableName);
            }

            // var
            if (this.SetVarValue<VariableType>(variableId, index, value))
            {
                return;
            }

            // property
            IProperty prop = this.GetProperty(variableId);

            if (prop != null)
            {
                Debug.Check(prop is CProperty<VariableType>);
                CProperty<VariableType> p = (CProperty<VariableType>)prop;

                if (p != null)
                {
                    p.SetValue(this, value, index);
                    return;
                }
            }

            Debug.Check(false, string.Format("The variable \"{0}\" with type \"{1}\" can not be found!", variableName, typeof(VariableType).Name));
        }

        internal void SetVariableFromString(string variableName, string valueStr)
        {
            uint variableId = Utils.MakeVariableId(variableName);

            IInstantiatedVariable v = this.GetInstantiatedVariable(variableId);

            if (v != null)
            {
                v.SetValueFromString(this, valueStr);
                return;
            }

            IProperty prop = this.GetProperty(variableId);
            if (prop != null)
            {
                prop.SetValueFromString(this, valueStr);
            }
        }

        public void LogVariables(bool bForce)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                // property
                string className = this.GetClassTypeName();
                uint classId = Utils.MakeVariableId(className);
                AgentMeta meta = AgentMeta.GetMeta(classId);

                if (meta != null)
                {
                    // local var
                    if (this.ExcutingTreeTask != null)
                    {
                        var e = this.ExcutingTreeTask.LocalVars.Keys.GetEnumerator();

                        while (e.MoveNext())
                        {
                            uint id = e.Current;
                            IInstantiatedVariable pVar = this.ExcutingTreeTask.LocalVars[id];

                            if (pVar != null)
                            {
                                pVar.Log(this);
                            }
                        }
                    }//

                    //customized property
                    this.Variables.Log(this);

                    //member property
                    {
                        Dictionary<uint, IProperty> memberProperties = meta.GetMemberProperties();

                        if (memberProperties != null)
                        {
                            var e = memberProperties.Keys.GetEnumerator();

                            while (e.MoveNext())
                            {
                                uint id = e.Current;
                                IProperty pProperty = memberProperties[id];

                                if (!pProperty.IsArrayItem)
                                {
                                    bool bNew = false;
                                    IValue pVar = null;

                                    if (this._members.ContainsKey(id))
                                    {
                                        pVar = this._members[id];
                                    }
                                    else
                                    {
                                        bNew = true;
                                        pVar = pProperty.CreateIValue();
                                        this._members[id] = pVar;
                                    }

                                    if (pVar != null)
                                    {
                                        pVar.Log(this, pProperty.Name, bNew);
                                    }
                                }
                            }
                        }
                    }
                }
            }
#endif
        }

        public void LogRunningNodes()
        {
#if !BEHAVIAC_RELEASE
            if (Config.IsLoggingOrSocketing && this.m_currentBT != null)
            {
                List<BehaviorTask> runningNodes = this.m_currentBT.GetRunningNodes(false);
                var e = runningNodes.GetEnumerator();

                while (e.MoveNext())
                {
                    BehaviorTask behaviorTask = e.Current;
                    string btStr = BehaviorTask.GetTickInfo(this, behaviorTask, "enter");

                    //empty btStr is for internal BehaviorTreeTask
                    if (!string.IsNullOrEmpty(btStr))
                    {
                        LogManager.Instance.Log(this, btStr, EActionResult.EAR_success, LogMode.ELM_tick);
                    }
                }
            }
#endif
        }

#if !BEHAVIAC_RELEASE
        public int m_debug_in_exec;
        public int m_debug_count;
        //private const int kAGENT_DEBUG_VERY = 0x01010101;
#endif//#if !BEHAVIAC_RELEASE

        protected static void Init_(int contextId, Agent pAgent, int priority)
        {
            Debug.Check(contextId >= 0, "invalid context id");

            pAgent.m_contextId = contextId;
            pAgent.m_id = ms_agent_index++;
            pAgent.m_priority = priority;
            pAgent.SetName(pAgent.name);

            Context.AddAgent(pAgent);

#if !BEHAVIAC_RELEASE
            pAgent.m_debug_in_exec = 0;

            string agentClassName = pAgent.GetClassTypeName();
            string instanceName = pAgent.GetName();

            string aName = string.Format("{0}#{1}", agentClassName, instanceName);

            ms_agents[aName] = pAgent;
#endif
        }

        public void btresetcurrrent()
        {
            if (this.m_currentBT != null)
            {
                this.m_currentBT.reset(this);
            }
        }

        /**
        before set the specified one as the current bt, it aborts the current one
        */

        public void btsetcurrent(string relativePath)
        {
            _btsetcurrent(relativePath, TriggerMode.TM_Transfer, false);
        }

        public void btreferencetree(string relativePath)
        {
            this._btsetcurrent(relativePath, TriggerMode.TM_Return, false);
            this.m_referencetree = true;
        }

        public void bteventtree(Agent pAgent, string relativePath, TriggerMode triggerMode)
        {
            this._btsetcurrent(relativePath, triggerMode, true);
        }

        private void _btsetcurrent(string relativePath, TriggerMode triggerMode, bool bByEvent)
        {
            bool bEmptyPath = string.IsNullOrEmpty(relativePath);
            Debug.Check(!bEmptyPath && string.IsNullOrEmpty(Path.GetExtension(relativePath)));
            Debug.Check(Workspace.Instance.IsValidPath(relativePath));

            if (!bEmptyPath)
            {
                // if (this.m_currentBT != null && this.m_currentBT.GetName() == relativePath) {
                // //the same bt is set again return; }

                bool bLoaded = Workspace.Instance.Load(relativePath);

                if (!bLoaded)
                {
                    string agentName = this.GetType().FullName;
                    agentName += "::";
                    agentName += this.name;

                    Debug.Check(false);
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} is not a valid loaded behavior tree of {1}", relativePath, agentName));
                }
                else
                {
                    Workspace.Instance.RecordBTAgentMapping(relativePath, this);

                    if (this.m_currentBT != null)
                    {
                        //if trigger mode is 'return', just push the current bt 'oldBt' on the stack and do nothing more
                        //'oldBt' will be restored when the new triggered one ends
                        if (triggerMode == TriggerMode.TM_Return)
                        {
                            BehaviorTreeStackItem_t item = new BehaviorTreeStackItem_t(this.m_currentBT, triggerMode, bByEvent);
                            Debug.Check(this.BTStack.Count < 200, "recursive?");

                            this.BTStack.Add(item);
                        }
                        else if (triggerMode == TriggerMode.TM_Transfer)
                        {
                            //don't use the bt stack to restore, we just abort the current one.
                            //as the bt node has onenter/onexit, the abort can make them paired
                            //Debug.Check (this.m_currentBT.GetName() != relativePath);

                            if (this.m_currentBT.GetName() != relativePath)
                            {
                                this.m_currentBT.abort(this);
                            }
                            else
                            {
                                Debug.Check(true);
                            }
                        }
                    }

                    //BehaviorTreeTask pTask = this.BehaviorTreeTasks.Find(delegate (BehaviorTreeTask task) {return task.GetName() == relativePath;});
                    BehaviorTreeTask pTask = null;

                    for (int i = 0; i < this.BehaviorTreeTasks.Count; ++i)
                    {
                        BehaviorTreeTask t = this.BehaviorTreeTasks[i];

                        if (t.GetName() == relativePath)
                        {
                            pTask = t;
                            break;
                        }
                    }

                    bool bRecursive = false;

                    if (pTask != null && this.BTStack.Count > 0)
                    {
                        //bRecursive = this.BTStack.FindIndex(delegate (BehaviorTreeStackItem_t item){return item.bt.GetName() == relativePath;}) > -1;
                        for (int i = 0; i < this.BTStack.Count; ++i)
                        {
                            BehaviorTreeStackItem_t item = this.BTStack[i];

                            if (item.bt.GetName() == relativePath)
                            {
                                bRecursive = true;
                                break;
                            }
                        }

                        if (pTask.GetStatus() != EBTStatus.BT_INVALID)
                        {
                            pTask.reset(this);
                        }
                    }

                    if (pTask == null || bRecursive)
                    {
                        pTask = Workspace.Instance.CreateBehaviorTreeTask(relativePath);
                        Debug.Check(pTask != null);
                        this.BehaviorTreeTasks.Add(pTask);
                    }

                    this.CurrentTreeTask = pTask;

                    //string pThisTree = this.m_currentBT.GetName();
                    //this.LogJumpTree(pThisTree);
                }
            }
        }

        private EBTStatus btexec_()
        {
            if (this.m_currentBT != null)
            {
                //the following might modify this.m_currentBT if the invoked function called btsetcurrent/FireEvent
                BehaviorTreeTask pCurrentBT = this.m_currentBT;
                EBTStatus s = this.m_currentBT.exec(this);
                //Debug.Check(s == EBTStatus.BT_RUNNING || pCurrentBT == this.m_currentBT,
                //    "btsetcurrent/FireEvent is not allowed in the invoked function.");

                while (s != EBTStatus.BT_RUNNING)
                {
                    if (this.BTStack.Count > 0)
                    {
                        //get the last one
                        BehaviorTreeStackItem_t lastOne = this.BTStack[this.BTStack.Count - 1];

                        this.CurrentTreeTask = lastOne.bt;
                        this.BTStack.RemoveAt(this.BTStack.Count - 1);

                        bool bExecCurrent = false;

                        if (lastOne.triggerMode == TriggerMode.TM_Return)
                        {
                            if (!lastOne.triggerByEvent)
                            {
                                if (this.m_currentBT != pCurrentBT)
                                {
                                    s = this.m_currentBT.resume(this, s);
                                }
                                else
                                {
                                    //pCurrentBT ends and while pCurrentBT is exec, it internally calls 'btsetcurrent'
                                    //to modify m_currentBT as the new one, and after pop from the stack, m_currentBT would be pCurrentBT
                                    Debug.Check(true);
                                }
                            }
                            else
                            {
                                bExecCurrent = true;
                            }
                        }
                        else
                        {
                            bExecCurrent = true;
                        }

                        if (bExecCurrent)
                        {
                            pCurrentBT = this.m_currentBT;
                            s = this.m_currentBT.exec(this);
                            break;
                        }
                    }
                    else
                    {
                        //this.CurrentBT = null;
                        break;
                    }
                }

                if (s != EBTStatus.BT_RUNNING) {
                    this.ExcutingTreeTask = null;
                }

                return s;
            }
            else
            {
                //behaviac.Debug.LogWarning("NO ACTIVE BT!\n");
            }

            return EBTStatus.BT_INVALID;
        }

        public void LogJumpTree(string newTree)
        {
#if !BEHAVIAC_RELEASE
            string msg = string.Format("{0}.xml", newTree);
            LogManager.Instance.Log(this, msg, EActionResult.EAR_none, LogMode.ELM_jump);
#endif
        }

        public void LogReturnTree(string returnFromTree)
        {
#if !BEHAVIAC_RELEASE
            string msg = string.Format("{0}.xml", returnFromTree);
            LogManager.Instance.Log(this, msg, EActionResult.EAR_none, LogMode.ELM_return);
#endif
        }

        public virtual EBTStatus btexec()
        {
            if (this.m_bActive)
            {
#if !BEHAVIAC_NOT_USE_UNITY
                UnityEngine.Profiler.BeginSample("btexec");
#endif

#if !BEHAVIAC_RELEASE
                this.m_debug_in_exec = 1;
                this.m_debug_count = 0;
#endif

                EBTStatus s = this.btexec_();

                while (this.m_referencetree && s == EBTStatus.BT_RUNNING)
                {
                    this.m_referencetree = false;
                    s = this.btexec_();
                }

                if (this.IsMasked())
                {
                    this.LogVariables(false);
                }

#if !BEHAVIAC_NOT_USE_UNITY
                UnityEngine.Profiler.EndSample();
#endif

#if !BEHAVIAC_RELEASE
                this.m_debug_in_exec = 0;
#endif

                return s;
            }

            return EBTStatus.BT_INVALID;
        }

        public bool btload(string relativePath, bool bForce /*= false*/)
        {
            bool bOk = Workspace.Instance.Load(relativePath, bForce);

            if (bOk)
            {
                Workspace.Instance.RecordBTAgentMapping(relativePath, this);
            }

            return bOk;
        }

        public bool btload(string relativePath)
        {
            bool bOk = this.btload(relativePath, false);

            return bOk;
        }

        public void btunload(string relativePath)
        {
            Debug.Check(string.IsNullOrEmpty(Path.GetExtension(relativePath)), "no extention to specify");
            Debug.Check(Workspace.Instance.IsValidPath(relativePath));

            //clear the current bt if it is the current bt
            if (this.m_currentBT != null && this.m_currentBT.GetName() == relativePath)
            {
                BehaviorNode btNode = this.m_currentBT.GetNode();
                Debug.Check(btNode is BehaviorTree);
                BehaviorTree bt = btNode as BehaviorTree;
                this.btunload_pars(bt);

                this.CurrentTreeTask = null;
            }

            //remove it from stack
            for (int i = 0; i < this.BTStack.Count; ++i)
            {
                BehaviorTreeStackItem_t item = this.BTStack[i];

                if (item.bt.GetName() == relativePath)
                {
                    this.BTStack.Remove(item);
                    break;
                }
            }

            for (int i = 0; i < this.BehaviorTreeTasks.Count; ++i)
            {
                BehaviorTreeTask task = this.BehaviorTreeTasks[i];

                if (task.GetName() == relativePath)
                {
                    Workspace.Instance.DestroyBehaviorTreeTask(task, this);

                    this.BehaviorTreeTasks.Remove(task);
                    break;
                }
            }

            Workspace.Instance.UnLoad(relativePath);
        }

        /**
        called when hotreloaded

        the default implementation is unloading all pars.

        it can be overridden to do some clean up, like to reset some internal states, etc.
        */

        public virtual void bthotreloaded(BehaviorTree bt)
        {
            this.btunload_pars(bt);
        }

        public void btunloadall()
        {
            List<BehaviorTree> bts = new List<BehaviorTree>();

            for (int i = 0; i < this.BehaviorTreeTasks.Count; ++i)
            {
                BehaviorTreeTask btTask = this.BehaviorTreeTasks[i];
                BehaviorNode btNode = btTask.GetNode();
                Debug.Check(btNode is BehaviorTree);
                BehaviorTree bt = (BehaviorTree)btNode;

                bool bFound = false;

                for (int t = 0; t < bts.Count; ++t)
                {
                    BehaviorTree it = bts[t];

                    if (it == bt)
                    {
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    bts.Add(bt);
                }

                Workspace.Instance.DestroyBehaviorTreeTask(btTask, this);
            }

            for (int t = 0; t < bts.Count; ++t)
            {
                BehaviorTree it = bts[t];
                this.btunload_pars(it);

                Workspace.Instance.UnLoad(it.GetName());
            }

            this.BehaviorTreeTasks.Clear();

            //just clear the name vector, don't unload it from cache
            this.CurrentTreeTask = null;
            this.BTStack.Clear();

            this.Variables.Unload();
        }

        public void btreloadall()
        {
            this.CurrentTreeTask = null;
            this.BTStack.Clear();

            if (this.m_behaviorTreeTasks != null)
            {
                List<string> bts = new List<string>();

                //collect the bts
                for (int i = 0; i < this.m_behaviorTreeTasks.Count; ++i)
                {
                    BehaviorTreeTask bt = this.m_behaviorTreeTasks[i];
                    string btName = bt.GetName();

                    if (bts.IndexOf(btName) == -1)
                    {
                        bts.Add(btName);
                    }
                }

                for (int i = 0; i < bts.Count; ++i)
                {
                    string btName = bts[i];
                    Workspace.Instance.Load(btName, true);
                }

                this.BehaviorTreeTasks.Clear();
            }

            this.Variables.Unload();
        }

        public bool btsave(State_t state)
        {
            this.Variables.CopyTo(null, state.Vars);

            if (this.m_currentBT != null)
            {
                Workspace.Instance.DestroyBehaviorTreeTask(state.BT, this);

                BehaviorNode pNode = this.m_currentBT.GetNode();

                if (pNode != null)
                {
                    state.BT = (BehaviorTreeTask)pNode.CreateAndInitTask();
                    this.m_currentBT.CopyTo(state.BT);

                    return true;
                }
            }

            return false;
        }

        public bool btload(State_t state)
        {
            state.Vars.CopyTo(this, this.m_variables);

            if (state.BT != null)
            {
                if (this.m_currentBT != null)
                {
                    for (int i = 0; i < this.m_behaviorTreeTasks.Count; ++i)
                    {
                        BehaviorTreeTask task = this.m_behaviorTreeTasks[i];

                        if (task == this.m_currentBT)
                        {
                            Workspace.Instance.DestroyBehaviorTreeTask(task, this);

                            this.m_behaviorTreeTasks.Remove(task);
                            break;
                        }
                    }
                }

                BehaviorNode pNode = state.BT.GetNode();

                if (pNode != null)
                {
                    this.m_currentBT = (BehaviorTreeTask)pNode.CreateAndInitTask();
                    state.BT.CopyTo(this.m_currentBT);

                    return true;
                }
            }

            return false;
        }

        private void btunload_pars(BehaviorTree bt)
        {
            if (bt.LocalProps != null)
            {
                bt.LocalProps.Clear();
            }
        }

        public void btonevent(string btEvent, Dictionary<uint, IInstantiatedVariable> eventParams)
        {
            if (this.m_currentBT != null)
            {
                string agentClassName = this.GetClassTypeName();
                uint agentClassId = Utils.MakeVariableId(agentClassName);
                AgentMeta meta = AgentMeta.GetMeta(agentClassId);

                if (meta != null)
                {
                    uint eventId = Utils.MakeVariableId(btEvent);
                    IMethod e = meta.GetMethod(eventId);

                    if (e != null)
                    {
#if !BEHAVIAC_RELEASE
                        Debug.Check(this.m_debug_in_exec == 0, "FireEvent should not be called during the Agent is in btexec");
#endif
                        this.m_currentBT.onevent(this, btEvent, eventParams);
                    }
                    else
                    {
                        Debug.Check(false, string.Format("unregistered event {0}", btEvent));
                    }
                }
            }
        }

        public void FireEvent(string eventName)
        {
            this.btonevent(eventName, null);
        }

        public void FireEvent<ParamType>(string eventName, ParamType param)
        {
            Dictionary<uint, IInstantiatedVariable> eventParams = new Dictionary<uint, IInstantiatedVariable>();

            string paramName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 0);
            uint paramId = Utils.MakeVariableId(paramName);
            eventParams[paramId] = new CVariable<ParamType>(paramName, param);

            this.btonevent(eventName, eventParams);
        }

        public void FireEvent<ParamType1, ParamType2>(string eventName, ParamType1 param1, ParamType2 param2)
        {
            Dictionary<uint, IInstantiatedVariable> eventParams = new Dictionary<uint, IInstantiatedVariable>();

            string paramName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 0);
            uint paramId = Utils.MakeVariableId(paramName);
            eventParams[paramId] = new CVariable<ParamType1>(paramName, param1);

            paramName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 1);
            paramId = Utils.MakeVariableId(paramName);
            eventParams[paramId] = new CVariable<ParamType2>(paramName, param2);

            this.btonevent(eventName, eventParams);
        }

        public void FireEvent<ParamType1, ParamType2, ParamType3>(string eventName, ParamType1 param1, ParamType2 param2, ParamType3 param3)
        {
            Dictionary<uint, IInstantiatedVariable> eventParams = new Dictionary<uint, IInstantiatedVariable>();

            string paramName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 0);
            uint paramId = Utils.MakeVariableId(paramName);
            eventParams[paramId] = new CVariable<ParamType1>(paramName, param1);

            paramName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 1);
            paramId = Utils.MakeVariableId(paramName);
            eventParams[paramId] = new CVariable<ParamType2>(paramName, param2);

            paramName = string.Format("{0}{1}", Task.LOCAL_TASK_PARAM_PRE, 2);
            paramId = Utils.MakeVariableId(paramName);
            eventParams[paramId] = new CVariable<ParamType3>(paramName, param3);

            this.btonevent(eventName, eventParams);
        }

        [behaviac.MethodMetaInfo()]
        public static void LogMessage(string message)
        {
            int frames = behaviac.Workspace.Instance.FrameSinceStartup;

            behaviac.Debug.Log(string.Format("[{0}]{1}\n", frames, message));
        }

        [behaviac.MethodMetaInfo()]
        public static int VectorLength(IList vector)
        {
            if (vector != null)
            {
                return vector.Count;
            }

            return 0;
        }

        [behaviac.MethodMetaInfo()]
        public static void VectorAdd(IList vector, object element)
        {
            if (vector != null)
            {
                vector.Add(element);
            }
        }

        [behaviac.MethodMetaInfo()]
        public static void VectorRemove(IList vector, object element)
        {
            if (vector != null)
            {
                vector.Remove(element);
            }
        }

        [behaviac.MethodMetaInfo()]
        public static bool VectorContains(IList vector, object element)
        {
            if (vector != null)
            {
                bool bOk = vector.IndexOf(element) > -1;

                return bOk;
            }

            return false;
        }

        [behaviac.MethodMetaInfo()]
        public static void VectorClear(IList vector)
        {
            if (vector != null)
            {
                vector.Clear();
            }
        }


        [TypeConverter()]
        public class StructConverter : TypeConverter
        {
            // Overrides the CanConvertFrom method of TypeConverter. The ITypeDescriptorContext
            // interface provides the context for the conversion. Typically, this interface is used
            // at design time to provide information about the design-time container.
            public override bool CanConvertFrom(ITypeDescriptorContext context,
                                                Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }

                return base.CanConvertFrom(context, sourceType);
            }

            // Overrides the ConvertFrom method of TypeConverter.
            public override object ConvertFrom(ITypeDescriptorContext context,
                                               CultureInfo culture, object value)
            {
                if (value is string)
                {
                }

                return base.ConvertFrom(context, culture, value);
            }

            // Overrides the ConvertTo method of TypeConverter.
            public override object ConvertTo(ITypeDescriptorContext context,
                                             CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
