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

// please define BEHAVIAC_NOT_USE_UNITY in your project file if you are not using unity
#if !BEHAVIAC_NOT_USE_UNITY
// if you have compiling errors complaining the following using 'UnityEngine',
//usually, you need to define BEHAVIAC_NOT_USE_UNITY in your project file
using UnityEngine;
#endif//!BEHAVIAC_NOT_USE_UNITY

using System.Collections.Generic;

namespace behaviac
{
    /**
     Return values of tick/update and valid states for behaviors.
    */

    [behaviac.TypeMetaInfo()]
    public enum EBTStatus
    {
        BT_INVALID,
        BT_SUCCESS,
        BT_FAILURE,
        BT_RUNNING,
    }

    /**
    trigger mode to control the bt switching and back
    */

    public enum TriggerMode
    {
        TM_Transfer,
        TM_Return
    }

    ///return false to stop traversing
    public delegate bool NodeHandler_t(BehaviorTask task, Agent agent, object user_data);

    /**
    Base class for the BehaviorTreeTask's runtime execution management.
    */

    public abstract class BehaviorTask
    {
        public static void DestroyTask(BehaviorTask task)
        {
        }

        public virtual void Init(BehaviorNode node)
        {
            Debug.Check(node != null);

            this.m_node = node;
            this.m_id = this.m_node.GetId();
        }

        public virtual void Clear()
        {
            this.m_status = EBTStatus.BT_INVALID;
            this.m_parent = null;
            this.m_id = -1;
        }

        public virtual void copyto(BehaviorTask target)
        {
            target.m_status = this.m_status;
        }

        public virtual void save(ISerializableNode node)
        {
            //CSerializationID  classId = new CSerializationID("class");
            //node.setAttr(classId, this.GetClassNameString());

            //CSerializationID  idId = new CSerializationID("id");
            //node.setAttr(idId, this.GetId());

            //CSerializationID  statusId = new CSerializationID("status");
            //node.setAttr(statusId, this.m_status);
        }

        public virtual void load(ISerializableNode node)
        {
        }

        public BehaviorTreeTask RootTask
        {
            get
            {
                BehaviorTask task = this;

                while (task.m_parent != null)
                {
                    task = task.m_parent;
                }

                Debug.Check(task is BehaviorTreeTask);
                BehaviorTreeTask tree = (BehaviorTreeTask)task;

                return tree;
            }
        }

        public string GetClassNameString()
        {
            if (this.m_node != null)
            {
                return this.m_node.GetClassNameString();
            }

            string subBT = "SubBT";
            return subBT;
        }

        public int GetId()
        {
            return this.m_id;
        }

        public virtual int GetNextStateId()
        {
            return -1;
        }

        public virtual BehaviorTask GetCurrentTask()
        {
            return null;
        }

#if !BEHAVIAC_RELEASE
        struct AutoProfileBlockSend
        {
            string classId_;
            Agent agent_;
            float time_;

            public AutoProfileBlockSend(string taskClassid, Agent agent)
            {
                this.classId_ = taskClassid;
                this.agent_ = agent;
#if !BEHAVIAC_NOT_USE_UNITY
                this.time_ = UnityEngine.Time.realtimeSinceStartup;
#else
                this.time_ = System.DateTime.Now.Millisecond;
#endif
            }

            public void Close()
            {
#if !BEHAVIAC_NOT_USE_UNITY
                float endTime = UnityEngine.Time.realtimeSinceStartup;
#else
                float endTime = System.DateTime.Now.Millisecond;
#endif

                //micro second
                long duration = (long)(float)((endTime - this.time_) * 1000000.0f);

                LogManager.Instance.Log(this.agent_, this.classId_, duration);
            }
        }
#endif

        public EBTStatus exec(Agent pAgent)
        {
            EBTStatus childStatus = EBTStatus.BT_RUNNING;

            return this.exec(pAgent, childStatus);
        }

        public EBTStatus exec(Agent pAgent, EBTStatus childStatus)
        {
#if !BEHAVIAC_RELEASE
            Debug.Check(this.m_node == null || this.m_node.IsValid(pAgent, this),
                        string.Format("Agent In BT:{0} while the Agent used for: {1}", this.m_node.GetAgentType(), pAgent.GetClassTypeName()));

            string classStr = (this.m_node != null ? this.m_node.GetClassNameString() : "BT");
            int nodeId = (this.m_node != null ? this.m_node.GetId() : -1);
            string taskClassid = string.Format("{0}[{1}]", classStr, nodeId);

            AutoProfileBlockSend profiler_block = new AutoProfileBlockSend(taskClassid, pAgent);
#endif//#if !BEHAVIAC_RELEASE
            bool bEnterResult = false;

            if (this.m_status == EBTStatus.BT_RUNNING)
            {
                bEnterResult = true;
            }
            else
            {
                //reset it to invalid when it was success/failure
                this.m_status = EBTStatus.BT_INVALID;
                bEnterResult = this.onenter_action(pAgent);
            }

            if (bEnterResult)
            {
#if !BEHAVIAC_RELEASE

                if (Config.IsLoggingOrSocketing)
                {
                    string btStr = BehaviorTask.GetTickInfo(pAgent, this, "update");

                    //empty btStr is for internal BehaviorTreeTask
                    if (!string.IsNullOrEmpty(btStr))
                    {
                        LogManager.Instance.Log(pAgent, btStr, EActionResult.EAR_none, LogMode.ELM_tick);
                    }
                }

#endif
                bool bValid = this.CheckParentUpdatePreconditions(pAgent);

                if (bValid)
                {
                    this.m_status = this.update_current(pAgent, childStatus);
                }
                else
                {
                    this.m_status = EBTStatus.BT_FAILURE;

                    if (this.GetCurrentTask() != null)
                    {
                        this.update_current(pAgent, EBTStatus.BT_FAILURE);
                    }
                }

                if (this.m_status != EBTStatus.BT_RUNNING)
                {
                    //clear it

                    this.onexit_action(pAgent, this.m_status);

                    //this node is possibly ticked by its parent or by the topBranch who records it as currrent node
                    //so, we can't here reset the topBranch's current node
                }
                else
                {
                    BranchTask tree = this.GetTopManageBranchTask();

                    if (tree != null)
                    {
                        tree.SetCurrentTask(this);
                    }
                }
            }
            else
            {
                this.m_status = EBTStatus.BT_FAILURE;
            }

#if !BEHAVIAC_RELEASE
            profiler_block.Close();
#endif

            return this.m_status;
        }

        private const int kMaxParentsCount = 512;
        private static BehaviorTask[] ms_parents = new BehaviorTask[kMaxParentsCount];
        private bool CheckParentUpdatePreconditions(Agent pAgent)
        {
            bool bValid = true;

            if (this.m_bHasManagingParent)
            {
                bool bHasManagingParent = false;
                int parentsCount = 0;

                BranchTask parentBranch = this.GetParent();

                ms_parents[parentsCount++] = this;

                //back track the parents until the managing branch
                while (parentBranch != null)
                {
                    Debug.Check(parentsCount < kMaxParentsCount, "weird tree!");

                    ms_parents[parentsCount++] = parentBranch;

                    if (parentBranch.GetCurrentTask() == this)
                    {
                        //Debug.Check(parentBranch->GetNode()->IsManagingChildrenAsSubTrees());

                        bHasManagingParent = true;
                        break;
                    }

                    parentBranch = parentBranch.GetParent();
                }

                if (bHasManagingParent)
                {
                    for (int i = parentsCount - 1; i >= 0; --i)
                    {
                        BehaviorTask pb = ms_parents[i];

                        bValid = pb.CheckPreconditions(pAgent, true);

                        if (!bValid)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                bValid = this.CheckPreconditions(pAgent, true);
            }

            return bValid;
        }

        private BranchTask GetTopManageBranchTask()
        {
            BranchTask tree = null;
            BehaviorTask task = this.m_parent;

            while (task != null)
            {
                if (task is BehaviorTreeTask)
                {
                    //to overwrite the child branch
                    tree = (BranchTask)task;
                    break;
                }
                else if (task.m_node.IsManagingChildrenAsSubTrees())
                {
                    //until it is Parallel/SelectorLoop, it's child is used as tree to store current task
                    break;
                }
                else if (task is BranchTask)
                {
                    //this if must be after BehaviorTreeTask and IsManagingChildrenAsSubTrees
                    tree = (BranchTask)task;
                }
                else
                {
                    Debug.Check(false);
                }

                task = task.m_parent;
            }

            return tree;
        }

        private static bool getRunningNodes_handler(BehaviorTask node, Agent pAgent, object user_data)
        {
            if (node.m_status == EBTStatus.BT_RUNNING)
            {
                ((List<BehaviorTask>)user_data).Add(node);
            }

            return true;
        }

        private static bool end_handler(BehaviorTask node, Agent pAgent, object user_data)
        {
            if (node.m_status == EBTStatus.BT_RUNNING || node.m_status == EBTStatus.BT_INVALID)
            {
                EBTStatus status = (EBTStatus)user_data;

                node.onexit_action(pAgent, status);

                node.m_status = status;

                node.SetCurrentTask(null);
            }

            return true;
        }

        private static bool abort_handler(BehaviorTask node, Agent pAgent, object user_data)
        {
            if (node.m_status == EBTStatus.BT_RUNNING)
            {
                node.onexit_action(pAgent, EBTStatus.BT_FAILURE);

                node.m_status = EBTStatus.BT_FAILURE;

                node.SetCurrentTask(null);
            }

            return true;
        }

        private static bool reset_handler(BehaviorTask node, Agent pAgent, object user_data)
        {
            node.m_status = EBTStatus.BT_INVALID;

            node.onreset(pAgent);
            node.SetCurrentTask(null);

            return true;
        }

        protected static NodeHandler_t getRunningNodes_handler_ = getRunningNodes_handler;
        protected static NodeHandler_t end_handler_ = end_handler;
        protected static NodeHandler_t abort_handler_ = abort_handler;
        protected static NodeHandler_t reset_handler_ = reset_handler;

        public List<BehaviorTask> GetRunningNodes(bool onlyLeaves = true)
        {
            List<BehaviorTask> nodes = new List<BehaviorTask>();
            this.traverse(true, getRunningNodes_handler_, null, nodes);

            if (onlyLeaves && nodes.Count > 0)
            {
                List<BehaviorTask> leaves = new List<BehaviorTask>();

                for (int i = 0; i < nodes.Count; ++i)
                {
                    if (nodes[i] is LeafTask)
                    {
                        leaves.Add(nodes[i]);
                    }
                }

                return leaves;
            }

            return nodes;
        }

        public void abort(Agent pAgent)
        {
            this.traverse(true, abort_handler_, pAgent, null);
        }

        ///reset the status to invalid
        public void reset(Agent pAgent)
        {
            //BEHAVIAC_PROFILE("BehaviorTask.reset");

            this.traverse(true, reset_handler_, pAgent, null);
        }

        public EBTStatus GetStatus()
        {
            return this.m_status;
        }

        public void SetStatus(EBTStatus s)
        {
            this.m_status = s;
        }

        public BehaviorNode GetNode()
        {
            return this.m_node;
        }

        public void SetParent(BranchTask parent)
        {
            this.m_parent = parent;
        }

        public BranchTask GetParent()
        {
            return this.m_parent;
        }

        public abstract void traverse(bool childFirst, NodeHandler_t handler, Agent pAgent, object user_data);

        /**
        return false if the event handling needs to be stopped

        an event can be configured to stop being checked if triggered
        */

        public bool CheckEvents(string eventName, Agent pAgent, Dictionary<uint, IInstantiatedVariable> eventParams)
        {
            return this.m_node.CheckEvents(eventName, pAgent, eventParams);
        }

        public virtual void onreset(Agent pAgent)
        {
        }

        /**
        return false if the event handling  needs to be stopped
        return true, the event hanlding will be checked furtherly
        */

        public virtual bool onevent(Agent pAgent, string eventName, Dictionary<uint, IInstantiatedVariable> eventParams)
        {
            if (this.m_status == EBTStatus.BT_RUNNING && this.m_node.HasEvents())
            {
                if (!this.CheckEvents(eventName, pAgent, eventParams))
                {
                    return false;
                }
            }

            return true;
        }

        protected BehaviorTask()
        {
            m_status = EBTStatus.BT_INVALID;
            m_node = null;
            m_parent = null;
            m_bHasManagingParent = false;
        }

        protected virtual EBTStatus update_current(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus s = this.update(pAgent, childStatus);

            return s;
        }

        protected virtual EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            return EBTStatus.BT_SUCCESS;
        }

        protected virtual bool onenter(Agent pAgent)
        {
            return true;
        }

        protected virtual void onexit(Agent pAgent, EBTStatus status)
        {
        }

        public static string GetTickInfo(Agent pAgent, BehaviorTask bt, string action)
        {
            string result = GetTickInfo(pAgent, bt.GetNode(), action);

            return result;
        }

        public static string GetTickInfo(Agent pAgent, BehaviorNode n, string action)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                if (!System.Object.ReferenceEquals(pAgent, null) && pAgent.IsMasked())
                {
                    //BEHAVIAC_PROFILE("GetTickInfo", true);

                    string bClassName = n.GetClassNameString();

                    //filter out intermediate bt, whose class name is empty
                    if (!string.IsNullOrEmpty(bClassName))
                    {
                        string btName = GetParentTreeName(pAgent, n);

                        string bpstr = "";

                        if (!string.IsNullOrEmpty(btName))
                        {
                            bpstr = string.Format("{0}.xml->", btName);
                        }

                        int nodeId = n.GetId();
                        bpstr += string.Format("{0}[{1}]", bClassName, nodeId);

                        if (!string.IsNullOrEmpty(action))
                        {
                            bpstr += string.Format(":{0}", action);
                        }

                        return bpstr;
                    }
                }
            }

#endif
            return string.Empty;
        }

        public static string GetParentTreeName(Agent pAgent, BehaviorNode n)
        {
            string btName = null;

            if (n is ReferencedBehavior)
            {
                n = n.Parent;
            }

            bool bIsTree = false;
            bool bIsRefTree = false;

            while (n != null)
            {
                bIsTree = (n is BehaviorTree);
                bIsRefTree = (n is ReferencedBehavior);

                if (bIsTree || bIsRefTree)
                {
                    break;
                }

                n = n.Parent;
            }

            if (bIsTree)
            {
                BehaviorTree bt = n as BehaviorTree;
                btName = bt.GetName();
            }
            else if (bIsRefTree)
            {
                ReferencedBehavior refTree = n as ReferencedBehavior;
                btName = refTree.GetReferencedTree(pAgent);
            }
            else
            {
                Debug.Check(false);
            }

            return btName;
        }

        private static string GetActionResultStr(EActionResult actionResult)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string actionResultStr = "";

                if (actionResult == EActionResult.EAR_success)
                {
                    actionResultStr = " [success]";
                }
                else if (actionResult == EActionResult.EAR_failure)
                {
                    actionResultStr = " [failure]";
                }
                else if (actionResult == EActionResult.EAR_all)
                {
                    actionResultStr = " [all]";
                }
                else
                {
                    //although actionResult can be EAR_none or EAR_all, but, as this is the real result of an action
                    //it can only be success or failure
                    Debug.Check(false);
                }

                return actionResultStr;
            }

#endif
            return string.Empty;
        }

        private static void _MY_BREAKPOINT_BREAK_(Agent pAgent, string btMsg, EActionResult actionResult)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string actionResultStr = GetActionResultStr(actionResult);
                string msg = string.Format("BehaviorTreeTask Breakpoints at: '{0}{1}'\n\nOk to continue.", btMsg, actionResultStr);

                Workspace.Instance.RespondToBreak(msg, "BehaviorTreeTask Breakpoints");
            }

#endif
        }

        //CheckBreakpoint should be after log of onenter/onexit/update, as it needs to flush msg to the client
        public static void CHECK_BREAKPOINT(Agent pAgent, BehaviorNode b, string action, EActionResult actionResult)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string bpstr = GetTickInfo(pAgent, b, action);

                if (!string.IsNullOrEmpty(bpstr))
                {
                    LogManager.Instance.Log(pAgent, bpstr, actionResult, LogMode.ELM_tick);

                    if (Workspace.Instance.CheckBreakpoint(pAgent, b, action, actionResult))
                    {
                        //log the current variables, otherwise, its value is not the latest
                        pAgent.LogVariables(false);
                        LogManager.Instance.Log(pAgent, bpstr, actionResult, LogMode.ELM_breaked);
                        LogManager.Instance.Flush(pAgent);
                        SocketUtils.Flush();

                        _MY_BREAKPOINT_BREAK_(pAgent, bpstr, actionResult);

                        LogManager.Instance.Log(pAgent, bpstr, actionResult, LogMode.ELM_continue);
                        LogManager.Instance.Flush(pAgent);
                        SocketUtils.Flush();
                    }
                }
            }

#endif
        }

        protected virtual bool CheckPreconditions(Agent pAgent, bool bIsAlive)
        {
            bool bResult = true;

            if (this.m_node != null)
            {
                if (this.m_node.PreconditionsCount > 0)
                {
                    bResult = this.m_node.CheckPreconditions(pAgent, bIsAlive);
                }
            }

            return bResult;
        }

        public bool onenter_action(Agent pAgent)
        {
            bool bResult = this.CheckPreconditions(pAgent, false);

            if (bResult)
            {
                this.m_bHasManagingParent = false;
                this.SetCurrentTask(null);

                bResult = this.onenter(pAgent);

                if (!bResult)
                {
                    return false;
                }
                else
                {
#if !BEHAVIAC_RELEASE
                    //BEHAVIAC_PROFILE_DEBUGBLOCK("Debug", true);

                    BehaviorTask.CHECK_BREAKPOINT(pAgent, this.m_node, "enter", bResult ? EActionResult.EAR_success : EActionResult.EAR_failure);
#endif
                }
            }

            return bResult;
        }

        public void onexit_action(Agent pAgent, EBTStatus status)
        {
            this.onexit(pAgent, status);

            if (this.m_node != null)
            {
                Effector.EPhase phase = Effector.EPhase.E_SUCCESS;

                if (status == EBTStatus.BT_FAILURE)
                {
                    phase = Effector.EPhase.E_FAILURE;
                }
                else
                {
                    Debug.Check(status == EBTStatus.BT_SUCCESS);
                }

                this.m_node.ApplyEffects(pAgent, phase);
            }

#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                //BEHAVIAC_PROFILE_DEBUGBLOCK("Debug", true);
                if (status == EBTStatus.BT_SUCCESS)
                {
                    BehaviorTask.CHECK_BREAKPOINT(pAgent, this.m_node, "exit", EActionResult.EAR_success);
                }
                else
                {
                    BehaviorTask.CHECK_BREAKPOINT(pAgent, this.m_node, "exit", EActionResult.EAR_failure);
                }
            }

#endif
        }

        public void SetHasManagingParent(bool bHasManagingParent)
        {
            this.m_bHasManagingParent = bHasManagingParent;
        }

        public virtual void SetCurrentTask(BehaviorTask task)
        {
        }

        public EBTStatus m_status;
        protected BehaviorNode m_node;
        protected BranchTask m_parent;
        protected int m_id;
        protected bool m_bHasManagingParent;
    }

    // ============================================================================
    public class AttachmentTask : BehaviorTask
    {
        protected AttachmentTask()
        {
        }

        public override void traverse(bool childFirst, NodeHandler_t handler, Agent pAgent, object user_data)
        {
            handler(this, pAgent, user_data);
        }
    }

    // ============================================================================
    public class LeafTask : BehaviorTask
    {
        public override void traverse(bool childFirst, NodeHandler_t handler, Agent pAgent, object user_data)
        {
            handler(this, pAgent, user_data);
        }

        protected LeafTask()
        {
        }
    }

    // ============================================================================
    public abstract class BranchTask : BehaviorTask
    {
        protected BranchTask()
        {
        }

        public override void Clear()
        {
            base.Clear();

            this.m_currentTask = null;
        }

        protected override bool onenter(Agent pAgent)
        {
            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus status)
        {
            //this.m_currentTask = null;
        }

        public override void onreset(Agent pAgent)
        {
            //this.m_currentTask = null;
        }

        private bool oneventCurrentNode(Agent pAgent, string eventName, Dictionary<uint, IInstantiatedVariable> eventParams)
        {
            Debug.Check(this.m_currentTask != null);

            if (this.m_currentTask != null)
            {
                EBTStatus s = this.m_currentTask.GetStatus();

                Debug.Check(s == EBTStatus.BT_RUNNING && this.m_node.HasEvents());

                bool bGoOn = this.m_currentTask.onevent(pAgent, eventName, eventParams);

                //give the handling back to parents
                if (bGoOn && this.m_currentTask != null)
                {
                    BranchTask parentBranch = this.m_currentTask.GetParent();

                    //back track the parents until the branch
                    while (parentBranch != null && parentBranch != this)
                    {
                        Debug.Check(parentBranch.GetStatus() == EBTStatus.BT_RUNNING);

                        bGoOn = parentBranch.onevent(pAgent, eventName, eventParams);

                        if (!bGoOn)
                        {
                            return false;
                        }

                        parentBranch = parentBranch.GetParent();
                    }
                }

                return bGoOn;
            }

            return false;
        }

        // return false if the event handling needs to be stopped return true, the event hanlding
        // will be checked furtherly
        public override bool onevent(Agent pAgent, string eventName, Dictionary<uint, IInstantiatedVariable> eventParams)
        {
            if (this.m_node.HasEvents())
            {
                bool bGoOn = true;

                if (this.m_currentTask != null)
                {
                    bGoOn = this.oneventCurrentNode(pAgent, eventName, eventParams);
                }

                if (bGoOn)
                {
                    bGoOn = base.onevent(pAgent, eventName, eventParams);
                }

                return bGoOn;
            }

            return true;
        }

        private EBTStatus execCurrentTask(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_currentTask != null && this.m_currentTask.GetStatus() == EBTStatus.BT_RUNNING);

            //this.m_currentTask could be cleared in ::tick, to remember it
            EBTStatus status = this.m_currentTask.exec(pAgent, childStatus);

            //give the handling back to parents
            if (status != EBTStatus.BT_RUNNING)
            {
                Debug.Check(status == EBTStatus.BT_SUCCESS || status == EBTStatus.BT_FAILURE);
                Debug.Check(this.m_currentTask.m_status == status);

                BranchTask parentBranch = this.m_currentTask.GetParent();

                this.m_currentTask = null;

                //back track the parents until the branch
                while (parentBranch != null)
                {
                    if (parentBranch == this)
                    {
                        status = parentBranch.update(pAgent, status);
                    }
                    else
                    {
                        status = parentBranch.exec(pAgent, status);
                    }

                    if (status == EBTStatus.BT_RUNNING)
                    {
                        return EBTStatus.BT_RUNNING;
                    }

                    Debug.Check(parentBranch == this || parentBranch.m_status == status);

                    if (parentBranch == this)
                    {
                        break;
                    }

                    parentBranch = parentBranch.GetParent();
                }
            }

            return status;
        }

        protected override EBTStatus update_current(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_INVALID;

            if (this.m_currentTask != null)
            {
                status = this.execCurrentTask(pAgent, childStatus);
                Debug.Check(status == EBTStatus.BT_RUNNING ||
                            (status != EBTStatus.BT_RUNNING && this.m_currentTask == null));
            }
            else
            {
                status = this.update(pAgent, childStatus);
            }

            return status;
        }

        protected EBTStatus resume_branch(Agent pAgent, EBTStatus status)
        {
            Debug.Check(this.m_currentTask != null);
            Debug.Check(status == EBTStatus.BT_SUCCESS || status == EBTStatus.BT_FAILURE);

            BranchTask parent = null;

            if (this.m_currentTask.GetNode().IsManagingChildrenAsSubTrees())
            {
                parent = (BranchTask)this.m_currentTask;
            }
            else
            {
                parent = this.m_currentTask.GetParent();
            }

            //clear it as it ends and the next exec might need to set it
            this.m_currentTask = null;

            if (parent != null)
            {
                EBTStatus s = parent.exec(pAgent, status);

                return s;
            }

            return EBTStatus.BT_INVALID;
        }

        protected abstract void addChild(BehaviorTask pBehavior);

        //bookmark the current running node, it is different from m_activeChildIndex
        private BehaviorTask m_currentTask;

        public override BehaviorTask GetCurrentTask()
        {
            return this.m_currentTask;
        }

        public override void SetCurrentTask(BehaviorTask task)
        {
            if (task != null)
            {
                //if the leaf node is running, then the leaf's parent node is also as running,
                //the leaf is set as the tree's current task instead of its parent
                if (this.m_currentTask == null)
                {
                    Debug.Check(this.m_currentTask != this);
                    this.m_currentTask = task;
                    task.SetHasManagingParent(true);
                }
            }
            else
            {
                if (this.m_status != EBTStatus.BT_RUNNING)
                {
                    this.m_currentTask = task;
                }
            }
        }
    }

    // ============================================================================
    public class CompositeTask : BranchTask
    {
        public override void traverse(bool childFirst, NodeHandler_t handler, Agent pAgent, object user_data)
        {
            if (childFirst)
            {
                for (int i = 0; i < this.m_children.Count; ++i)
                {
                    BehaviorTask task = this.m_children[i];
                    task.traverse(childFirst, handler, pAgent, user_data);
                }

                handler(this, pAgent, user_data);
            }
            else
            {
                if (handler(this, pAgent, user_data))
                {
                    for (int i = 0; i < this.m_children.Count; ++i)
                    {
                        BehaviorTask task = this.m_children[i];
                        task.traverse(childFirst, handler, pAgent, user_data);
                    }
                }
            }
        }

        protected CompositeTask()
        {
            m_activeChildIndex = InvalidChildIndex;
        }

        //~CompositeTask()
        //{
        //    this.m_children.Clear();
        //}

        protected bool m_bIgnoreChildren = false;

        public override void Init(BehaviorNode node)
        {
            base.Init(node);

            if (!this.m_bIgnoreChildren)
            {
                Debug.Check(node.GetChildrenCount() > 0);

                int childrenCount = node.GetChildrenCount();

                for (int i = 0; i < childrenCount; i++)
                {
                    BehaviorNode childNode = node.GetChild(i);
                    BehaviorTask childTask = childNode.CreateAndInitTask();

                    this.addChild(childTask);
                }
            }
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);

            Debug.Check(target is CompositeTask);
            CompositeTask ttask = target as CompositeTask;

            ttask.m_activeChildIndex = this.m_activeChildIndex;

            Debug.Check(this.m_children.Count > 0);
            Debug.Check(this.m_children.Count == ttask.m_children.Count);

            int count = this.m_children.Count;

            for (int i = 0; i < count; ++i)
            {
                BehaviorTask childTask = this.m_children[i];
                BehaviorTask childTTask = ttask.m_children[i];

                childTask.copyto(childTTask);
            }
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);

            //BehaviorTasks_t.size_type count = this.m_children.Count;
            //for (BehaviorTasks_t.size_type i = 0; i < count; ++i)
            //{
            //    BehaviorTask childTask = this.m_children[i];

            //    CSerializationID  nodeId = new CSerializationID("node");
            //    ISerializableNode chidlNode = node.newChild(nodeId);
            //    childTask.save(chidlNode);
            //}
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        protected override void addChild(BehaviorTask pBehavior)
        {
            pBehavior.SetParent(this);

            this.m_children.Add(pBehavior);
        }

        protected List<BehaviorTask> m_children = new List<BehaviorTask>();

        protected BehaviorTask GetChildById(int nodeId)
        {
            if (this.m_children != null && this.m_children.Count > 0)
            {
                for (int i = 0; i < this.m_children.Count; ++i)
                {
                    BehaviorTask c = this.m_children[i];

                    if (c.GetId() == nodeId)
                    {
                        return c;
                    }
                }
            }

            return null;
        }


        //book mark the current child
        protected int m_activeChildIndex = InvalidChildIndex;

        protected const int InvalidChildIndex = -1;
    }

    // ============================================================================
    public class SingeChildTask : BranchTask
    {
        public override void traverse(bool childFirst, NodeHandler_t handler, Agent pAgent, object user_data)
        {
            if (childFirst)
            {
                if (this.m_root != null)
                {
                    this.m_root.traverse(childFirst, handler, pAgent, user_data);
                }

                handler(this, pAgent, user_data);
            }
            else
            {
                if (handler(this, pAgent, user_data))
                {
                    if (this.m_root != null)
                    {
                        this.m_root.traverse(childFirst, handler, pAgent, user_data);
                    }
                }
            }
        }

        protected SingeChildTask()
        {
            m_root = null;
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);

            Debug.Check(node.GetChildrenCount() <= 1);

            if (node.GetChildrenCount() == 1)
            {
                BehaviorNode childNode = node.GetChild(0);
                Debug.Check(childNode != null);

                if (childNode != null)
                {
                    BehaviorTask childTask = childNode.CreateAndInitTask();

                    this.addChild(childTask);
                }
            }
            else
            {
                Debug.Check(true);
            }
        }

        public override void copyto(BehaviorTask target)
        {
            base.copyto(target);

            Debug.Check(target is SingeChildTask);
            SingeChildTask ttask = target as SingeChildTask;

            if (this.m_root != null && ttask != null)
            {
                //referencebehavior/query, etc.
                if (ttask.m_root == null)
                {
                    BehaviorNode pNode = this.m_root.GetNode();
                    Debug.Check(pNode is BehaviorTree);
                    if (pNode != null)
                    {
                        ttask.m_root = pNode.CreateAndInitTask();
                    }

                    //Debug.Check(ttask.m_root is BehaviorTreeTask);
                    //BehaviorTreeTask btt = ttask.m_root as BehaviorTreeTask;
                    //btt.ModifyId(ttask);
                }

                Debug.Check(ttask.m_root != null);
                if (ttask.m_root != null)
                {
                    this.m_root.copyto(ttask.m_root);
                }
            }
        }

        public override void save(ISerializableNode node)
        {
            base.save(node);

            if (this.m_root != null)
            {
                //CSerializationID  nodeId = new CSerializationID("root");
                //ISerializableNode chidlNode = node.newChild(nodeId);
                //this.m_root.save(chidlNode);
            }
        }

        public override void load(ISerializableNode node)
        {
            base.load(node);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            if (this.m_root != null)
            {
                EBTStatus s = this.m_root.exec(pAgent, childStatus);
                return s;
            }

            return EBTStatus.BT_FAILURE;
        }

        protected override void addChild(BehaviorTask pBehavior)
        {
            pBehavior.SetParent(this);

            this.m_root = pBehavior;
        }

        protected BehaviorTask m_root;
    }

    // ============================================================================
    public abstract class DecoratorTask : SingeChildTask
    {
        protected DecoratorTask()
        {
        }

        protected override EBTStatus update_current(Agent pAgent, EBTStatus childStatus)
        {
            return base.update_current(pAgent, childStatus);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is DecoratorNode);
            DecoratorNode node = (DecoratorNode)this.m_node;

            EBTStatus status = EBTStatus.BT_INVALID;

            if (childStatus != EBTStatus.BT_RUNNING)
            {
                status = childStatus;

                if (!node.m_bDecorateWhenChildEnds || status != EBTStatus.BT_RUNNING)
                {
                    EBTStatus result = this.decorate(status);

                    if (result != EBTStatus.BT_RUNNING)
                    {
                        return result;
                    }

                    return EBTStatus.BT_RUNNING;
                }
            }

            status = base.update(pAgent, childStatus);

            if (!node.m_bDecorateWhenChildEnds || status != EBTStatus.BT_RUNNING)
            {
                EBTStatus result = this.decorate(status);

                return result;
            }

            return EBTStatus.BT_RUNNING;
        }

        /*
        called when the child's tick returns success or failure.
        please note, it is not called if the child's tick returns running
        */

        protected abstract EBTStatus decorate(EBTStatus status);
    }

    // ============================================================================
    public class BehaviorTreeTask : SingeChildTask
    {
        private Dictionary<uint, IInstantiatedVariable> m_localVars = new Dictionary<uint, IInstantiatedVariable>();
        private BehaviorTreeTask m_excutingTreeTask = null;
        public Dictionary<uint, IInstantiatedVariable> LocalVars
        {
            get
            {
                return m_localVars;
            }
        }

        internal void SetVariable<VariableType>(string variableName, VariableType value)
        {
            uint variableId = Utils.MakeVariableId(variableName);

            if (this.LocalVars.ContainsKey(variableId))
            {
                IInstantiatedVariable v = this.LocalVars[variableId];
                CVariable<VariableType> var = (CVariable<VariableType>)v;

                if (var != null)
                {
                    var.SetValue(null, value);
                    return;
                }
            }

            Debug.Check(false, string.Format("The variable \"{0}\" with type \"{1}\" can not be found!", variableName, typeof(VariableType).Name));
        }

        internal void AddVariables(Dictionary<uint, IInstantiatedVariable> vars)
        {
            if (vars != null)
            {
                var e = vars.Keys.GetEnumerator();

                while (e.MoveNext())
                {
                    uint varId = e.Current;
                    this.LocalVars[varId] = vars[varId];
                }
            }
        }

        public override void Init(BehaviorNode node)
        {
            base.Init(node);

            if (this.m_node != null)
            {
                Debug.Check(this.m_node is BehaviorTree);
                ((BehaviorTree)this.m_node).InstantiatePars(this.LocalVars);
            }
        }

        public override void Clear()
        {
            this.m_root = null;

            if (this.m_node != null)
            {
                Debug.Check(this.m_node is BehaviorTree);
                ((BehaviorTree)this.m_node).UnInstantiatePars(this.LocalVars);
            }

            base.Clear();
        }

        public void SetRootTask(BehaviorTask pRoot)
        {
            this.addChild(pRoot);
        }

        public void CopyTo(BehaviorTreeTask target)
        {
            this.copyto(target);
        }

        public void Save(ISerializableNode node)
        {
            //CSerializationID  btId = new CSerializationID("BehaviorTree");
            //ISerializableNode btNodeRoot = node.newChild(btId);

            //Debug.Check(this.GetNode() is BehaviorTree);
            //BehaviorTree bt = (BehaviorTree)this.GetNode();

            //CSerializationID  sourceId = new CSerializationID("source");
            //btNodeRoot.setAttr(sourceId, bt.GetName());

            //CSerializationID  nodeId = new CSerializationID("node");
            //ISerializableNode btNode = btNodeRoot.newChild(nodeId);

            //this.save(btNode);
        }

        public void Load(ISerializableNode node)
        {
            this.load(node);
        }

        /**
        return the path relative to the workspace path
        */

        public string GetName()
        {
            Debug.Check(this.m_node is BehaviorTree);
            BehaviorTree bt = this.m_node as BehaviorTree;
            Debug.Check(bt != null);
            return bt.GetName();
        }

        private EBTStatus m_endStatus = EBTStatus.BT_INVALID;
        public void setEndStatus(EBTStatus status)
        {
            this.m_endStatus = status;
        }

        public EBTStatus resume(Agent pAgent, EBTStatus status)
        {
            EBTStatus s = base.resume_branch(pAgent, status);

            return s;
        }

        protected override bool onenter(Agent pAgent)
        {
            pAgent.LogJumpTree(this.GetName());

            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            pAgent.ExcutingTreeTask = this.m_excutingTreeTask;
            pAgent.LogReturnTree(this.GetName());

            base.onexit(pAgent, s);
        }

        #region FSM
        private List<BehaviorTask> m_states = null;

        public BehaviorTask GetChildById(int nodeId)
        {
            if (this.m_states != null && this.m_states.Count > 0)
            {
                for (int i = 0; i < this.m_states.Count; ++i)
                {
                    BehaviorTask c = this.m_states[i];

                    if (c.GetId() == nodeId)
                    {
                        return c;
                    }
                }
            }

            return null;
        }
        #endregion FSM

        protected override EBTStatus update_current(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node != null);
            Debug.Check(this.m_node is BehaviorTree);

            this.m_excutingTreeTask = pAgent.ExcutingTreeTask;
            pAgent.ExcutingTreeTask = this;

            BehaviorTree tree = (BehaviorTree)this.m_node;
            EBTStatus status = EBTStatus.BT_RUNNING;

            if (tree.IsFSM)
            {
                status = this.update(pAgent, childStatus);
            }
            else
            {
                status = base.update_current(pAgent, childStatus);
            }

            return status;
        }

        private void end(Agent pAgent, EBTStatus status)
        {
            this.traverse(true, end_handler_, pAgent, status);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node != null);
            Debug.Check(this.m_root != null);

            if (childStatus != EBTStatus.BT_RUNNING)
            {
                return childStatus;
            }

            EBTStatus status = EBTStatus.BT_INVALID;

            status = base.update(pAgent, childStatus);

            Debug.Check(status != EBTStatus.BT_INVALID);

            // When the End node takes effect, it always returns BT_RUNNING
            // and m_endStatus should always be BT_SUCCESS or BT_FAILURE
            if ((status == EBTStatus.BT_RUNNING) && (this.m_endStatus != EBTStatus.BT_INVALID))
            {
                this.end(pAgent, this.m_endStatus);
                return this.m_endStatus;
            }

            return status;
        }
    }
}
