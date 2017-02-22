using System;
using System.Collections.Generic;


#if BEHAVIAC_USE_HTN
namespace behaviac
{
    #region PlannerTask

    public class PlannerTask : BehaviorTask
    {
        #region Public properties

        public PlannerTaskComplex Parent
        {
            get;
            set;
        }

        public bool PauseOnRun
        {
            get;
            set;
        }
        public bool NotInterruptable
        {
            get;
            set;
        }

        #endregion Public properties

        #region Constructor

        public PlannerTask(BehaviorNode node, Agent pAgent)
        : base()
        {
            this.m_node = node;
            this.m_id = this.m_node.GetId();
        }

        #endregion Constructor

        public delegate PlannerTask TaskCreator(BehaviorNode node, Agent pAgent);

        private static Dictionary<Type, TaskCreator> ms_factory = null;

        public static void Register<T>(TaskCreator c)
        {
            ms_factory[typeof(T)] = c;
        }

        public static PlannerTask Create(BehaviorNode node, Agent pAgent)
        {
            if (ms_factory == null)
            {
                ms_factory = new Dictionary<Type, TaskCreator>();
                Register<Action>((n, a) => new PlannerTaskAction(n, a));
                Register<Task>((n, a) => new PlannerTaskTask(n, a));
                Register<Method>((n, a) => new PlannerTaskMethod(n, a));
                Register<Sequence>((n, a) => new PlannerTaskSequence(n, a));
                Register<Selector>((n, a) => new PlannerTaskSelector(n, a));
                Register<Parallel>((n, a) => new PlannerTaskParallel(n, a));
                Register<ReferencedBehavior>((n, a) => new PlannerTaskReference(n, a));
                Register<DecoratorLoop>((n, a) => new PlannerTaskLoop(n, a));
                Register<DecoratorIterator>((n, a) => new PlannerTaskIterator(n, a));
            }

            Type type = node.GetType();

            while (!ms_factory.ContainsKey(type))
            {
                type = type.BaseType;
                Debug.Check(type != null);
            }

            if (ms_factory.ContainsKey(type))
            {
                TaskCreator c = ms_factory[type];

                PlannerTask task = c(node, pAgent);

                return task;
            }

            return null;
        }

        public static void Cleanup()
        {
            if (ms_factory != null)
            {
                ms_factory.Clear();
                ms_factory = null;
            }
        }

        public bool IsHigherPriority(PlannerTask other)
        {
            return true;
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            return EBTStatus.BT_SUCCESS;
        }

        public override void traverse(bool childFirst, NodeHandler_t handler, Agent pAgent, object user_data)
        { }
    }

    #endregion PlannerTask

    public class PlannerTaskAction : PlannerTask
    {
        //private object[] ParamsValue { get; set; }

        public PlannerTaskAction(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
            Debug.Check(node is Action);

            //Action action = node as Action;
            //this.ParamsValue = action.GetParamsValue(pAgent);
        }

        protected override bool onenter(Agent pAgent)
        {
            Debug.Check(true);
            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            Debug.Check(true);
            base.onexit(pAgent, s);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is Action);

            Action action = this.m_node as Action;

            //this.m_status = action.Execute(pAgent, this.ParamsValue);
            this.m_status = action.Execute(pAgent, childStatus);

            return this.m_status;
        }
    }

    public class PlannerTaskComplex : PlannerTask
    {
        protected int m_activeChildIndex = -1;
        protected List<BehaviorTask> m_children;

        public void AddChild(PlannerTask task)
        {
            if (this.m_children == null)
            {
                this.m_children = new List<BehaviorTask>();
            }

            this.m_children.Add(task);
            task.Parent = this;
        }

        public void RemoveChild(PlannerTask childTask)
        {
            Debug.Check(this.m_children.Count > 0 && this.m_children[this.m_children.Count - 1] == childTask);

            this.m_children.Remove(childTask);
        }

        public PlannerTaskComplex(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        //~PlannerTaskComplex()
        //{
        //    foreach (BehaviorTask t in this.m_children)
        //    {
        //        BehaviorTask.DestroyTask(t);
        //    }

        //    this.m_children.Clear();
        //}

        protected override bool onenter(Agent pAgent)
        {
            this.m_activeChildIndex = 0;
            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            base.onexit(pAgent, s);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            return EBTStatus.BT_SUCCESS;
        }
    }

    public class PlannerTaskSequence : PlannerTaskComplex
    {
        public PlannerTaskSequence(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is Sequence);
            Sequence node = this.m_node as Sequence;

            EBTStatus s = node.SequenceUpdate(pAgent, childStatus, ref this.m_activeChildIndex, this.m_children);

            return s;
        }
    }

    public class PlannerTaskSelector : PlannerTaskComplex
    {
        public PlannerTaskSelector(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is Selector);
            Selector node = this.m_node as Selector;

            EBTStatus s = node.SelectorUpdate(pAgent, childStatus, ref this.m_activeChildIndex, this.m_children);

            return s;
        }
    }

    public class PlannerTaskParallel : PlannerTaskComplex
    {
        public PlannerTaskParallel(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is Parallel);
            Parallel node = this.m_node as Parallel;

            EBTStatus s = node.ParallelUpdate(pAgent, this.m_children);

            return s;
        }
    }

    public class PlannerTaskLoop : PlannerTaskComplex
    {
        public PlannerTaskLoop(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        protected override bool onenter(Agent pAgent)
        {
            base.onenter(pAgent);

            //don't reset the m_n if it is restarted
            if (this.m_n == 0)
            {
                int count = this.GetCount(pAgent);

                if (count == 0)
                {
                    return false;
                }

                this.m_n = count;
            }
            else
            {
                Debug.Check(true);
            }

            return true;
        }

        public int GetCount(Agent pAgent)
        {
            Debug.Check(this.GetNode() is DecoratorLoop);
            DecoratorLoop pDecoratorCountNode = (DecoratorLoop)(this.GetNode());

            return pDecoratorCountNode != null ? pDecoratorCountNode.Count(pAgent) : 0;
        }

        protected int m_n;

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is DecoratorLoop);
            //DecoratorLoop node = this.m_node as DecoratorLoop;
            Debug.Check(this.m_children.Count == 1);
            BehaviorTask c = this.m_children[0];

            //EBTStatus s = c.exec(pAgent);
            c.exec(pAgent);

            if (this.m_n > 0)
            {
                this.m_n--;

                if (this.m_n == 0)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                return EBTStatus.BT_RUNNING;
            }

            if (this.m_n == -1)
            {
                return EBTStatus.BT_RUNNING;
            }

            Debug.Check(this.m_n == 0);

            return EBTStatus.BT_SUCCESS;
        }
    }

    public class PlannerTaskIterator : PlannerTaskComplex
    {
        public PlannerTaskIterator(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        private int m_index;

        public int Index
        {
            set
            {
                this.m_index = value;
            }
        }

        protected override bool onenter(Agent pAgent)
        {
            bool bOk = base.onenter(pAgent);

            DecoratorIterator pNode = this.m_node as DecoratorIterator;
            int count = 0;
            bOk = pNode.IterateIt(pAgent, this.m_index, ref count);

            return bOk;
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is DecoratorIterator);
            //DecoratorIterator pNode = this.m_node as DecoratorIterator;

            Debug.Check(this.m_children.Count == 1);
            BehaviorTask c = this.m_children[0];

            EBTStatus s = c.exec(pAgent);

            return s;
        }
    }

    public class PlannerTaskReference : PlannerTaskComplex
    {
        public PlannerTaskReference(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        private AgentState currentState;

        protected override bool CheckPreconditions(Agent pAgent, bool bIsAlive)
        {
            if (!bIsAlive)
            {
                //only try to Push when enter
                this.currentState = pAgent.Variables.Push(false);
                Debug.Check(currentState != null);
            }

            bool bOk = base.CheckPreconditions(pAgent, bIsAlive);

            if (!bIsAlive && !bOk)
            {
                this.currentState.Pop();
                this.currentState = null;
            }

            return bOk;
        }

#if !BEHAVIAC_RELEASE
        private bool _logged = false;
#endif

        BehaviorTreeTask oldTreeTask_ = null;

        BehaviorTreeTask m_subTree = null;
        public BehaviorTreeTask SubTreeTask
        {
            set
            {
                m_subTree = value;
            }
        }

        protected override bool onenter(Agent pAgent)
        {
            Debug.Check(this.m_node is ReferencedBehavior);
            ReferencedBehavior pNode = this.m_node as ReferencedBehavior;
            Debug.Check(pNode != null);

#if !BEHAVIAC_RELEASE
            _logged = false;
#endif

            //this.m_subTree = Workspace.Instance.CreateBehaviorTreeTask(pNode.GetReferencedTree(pAgent));
            Debug.Check(this.m_subTree != null);
            pNode.SetTaskParams(pAgent, this.m_subTree);

            this.oldTreeTask_ = pAgent.ExcutingTreeTask;
            pAgent.ExcutingTreeTask = this.m_subTree;

            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus status)
        {
            Debug.Check(this.m_node is ReferencedBehavior);
            ReferencedBehavior pNode = this.m_node as ReferencedBehavior;
            Debug.Check(pNode != null);

            this.m_subTree = null;
            pAgent.ExcutingTreeTask = this.oldTreeTask_;
#if !BEHAVIAC_RELEASE
            pAgent.LogReturnTree(pNode.GetReferencedTree(pAgent));
#endif

            Debug.Check(this.currentState != null);
            this.currentState.Pop();
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is ReferencedBehavior);
            ReferencedBehavior pNode = this.m_node as ReferencedBehavior;
            Debug.Check(pNode != null);

            EBTStatus status = EBTStatus.BT_RUNNING;

            if (pNode.RootTaskNode(pAgent) == null)
            {
                status = this.m_subTree.exec(pAgent);
            }
            else
            {
#if !BEHAVIAC_RELEASE

                if (!_logged)
                {
                    pAgent.LogJumpTree(pNode.GetReferencedTree(pAgent));
                    _logged = true;
                }

#endif
                Debug.Check(this.m_children.Count == 1);
                BehaviorTask c = this.m_children[0];

                BehaviorTreeTask oldTreeTask = pAgent.ExcutingTreeTask;
                pAgent.ExcutingTreeTask = this.m_subTree;

                status = c.exec(pAgent);

                pAgent.ExcutingTreeTask = oldTreeTask;
            }

            return status;
        }
    }

    public class PlannerTaskTask : PlannerTaskComplex
    {
        public PlannerTaskTask(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        protected override bool onenter(Agent pAgent)
        {
            //this.m_node.Parent.InstantiatePars(this.LocalVars);
            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            //this.m_node.Parent.UnInstantiatePars(this.LocalVars);
            base.onexit(pAgent, s);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is Task);
            //Task pNode = this.m_node as Task;

            Debug.Check(this.m_children.Count == 1);
            BehaviorTask c = this.m_children[0];

            EBTStatus s = c.exec(pAgent);

            return s;
        }
    }

    public class PlannerTaskMethod : PlannerTaskComplex
    {
        public PlannerTaskMethod(BehaviorNode node, Agent pAgent)
        : base(node, pAgent)
        {
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            Debug.Check(this.m_node is Method);
            //Method pNode = this.m_node as Method;

            Debug.Check(this.m_children.Count == 1);
            BehaviorTask c = this.m_children[0];

            EBTStatus s = c.exec(pAgent);

            return s;
        }
    }
}
#endif//#if BEHAVIAC_USE_HTN
