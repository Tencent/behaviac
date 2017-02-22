using System;
using System.Collections.Generic;

#if BEHAVIAC_USE_HTN
namespace behaviac
{
    public class Planner
    {
        #region private fields

        /// <summary>
        /// Gets or sets the agent script instance that taskner will be generating plans for
        /// </summary>
        private Agent agent;

        public Agent GetAgent()
        {
            return this.agent;
        }
        /// <summary>
        /// Gets or sets whether the planner will automatically perform periodic replanning
        /// </summary>
        private bool AutoReplan = true;

        private Task m_rootTaskNode;

        #endregion private fields

        #region Public properties

        private PlannerTask m_rootTask;

        #endregion Public properties

        #region events

        public void Init(Agent pAgent, Task rootTask)
        {
            this.agent = pAgent;
            this.m_rootTaskNode = rootTask;
        }

        public void Uninit()
        {
            this.OnDisable();
        }

        private void OnDisable()
        {
            if (this.m_rootTask != null)
            {
                if (this.m_rootTask.GetStatus() == EBTStatus.BT_RUNNING)
                {
                    this.m_rootTask.abort(this.agent);
                    BehaviorTask.DestroyTask(this.m_rootTask);
                }

                this.m_rootTask = null;
            }
        }

        public EBTStatus Update()
        {
            if (this.agent == null)
            {
                throw new InvalidOperationException("The Planner.Agent field has not been assigned");
            }

            doAutoPlanning();

            if (this.m_rootTask == null)
            {
                return EBTStatus.BT_FAILURE;
            }

            // Need a local reference in case the this.m_rootTask is cleared by an event handler
            var rootTask = this.m_rootTask;

            var taskStatus = rootTask.exec(this.agent);

            return taskStatus;
        }

        #endregion events

        /// Generate a new task
        private PlannerTask GeneratePlan()
        {
            // If the planner is currently executing a task marked NotInterruptable, do not generate
            // any new plans.
            if (!canInterruptCurrentPlan())
            {
                return null;
            }

            try
            {
                PlannerTask newPlan = this.BuildPlan(this.m_rootTaskNode);

                if (newPlan == null)
                {
                    return null;
                }

                if (!newPlan.IsHigherPriority(this.m_rootTask))
                {
                    return null;
                }

                return newPlan;
            }

            finally
            {
            }
        }

        #region Private utility methods

        private bool canInterruptCurrentPlan()
        {
            if (this.m_rootTask == null)
            {
                return true;
            }

            if (this.m_rootTask.GetStatus() != EBTStatus.BT_RUNNING)
            {
                return true;
            }

            var task = this.m_rootTask;

            if (task == null || !task.NotInterruptable)
            {
                return true;
            }

            return task.GetStatus() == EBTStatus.BT_FAILURE || task.GetStatus() == EBTStatus.BT_SUCCESS;
        }

        private void doAutoPlanning()
        {
            if (!this.AutoReplan)
            {
                return;
            }

            var noPlan = this.m_rootTask == null || this.m_rootTask.GetStatus() != EBTStatus.BT_RUNNING;

            if (noPlan)
            {
                PlannerTask newPlan = this.GeneratePlan();

                if (newPlan != null)
                {
                    if (this.m_rootTask != null)
                    {
                        if (this.m_rootTask.GetStatus() == EBTStatus.BT_RUNNING)
                        {
                            this.m_rootTask.abort(this.agent);
                        }

                        BehaviorTask.DestroyTask(this.m_rootTask);
                    }

                    this.m_rootTask = newPlan;
                }
            }
        }

        #endregion Private utility methods

        #region Log

        private void LogPlanBegin(Agent a, Task root)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string agentClassName = a.GetClassTypeName();
                string agentInstanceName = a.GetName();

                agentClassName = agentClassName.Replace(".", "::");
                agentInstanceName = agentInstanceName.Replace(".", "::");

                string ni = BehaviorTask.GetTickInfo(a, root, "plan");
                int count = Workspace.Instance.GetActionCount(ni) + 1;
                string buffer = string.Format("[plan_begin]{0}#{1} {2} {3}\n", agentClassName, agentInstanceName, ni, count);

                LogManager.Instance.Log(buffer);

                a.LogVariables(true);
            }

#endif
        }

        private void LogPlanEnd(Agent a, Task root)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string agentClassName = a.GetClassTypeName();
                string agentInstanceName = a.GetName();

                agentClassName = agentClassName.Replace(".", "::");
                agentInstanceName = agentInstanceName.Replace(".", "::");

                string ni = BehaviorTask.GetTickInfo(a, root, null);
                string buffer = string.Format("[plan_end]{0}#{1} {2}\n", agentClassName, agentInstanceName, ni);

                LogManager.Instance.Log(buffer);
            }

#endif
        }

        private void LogPlanNodeBegin(Agent a, BehaviorNode n)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, n, null);

                LogManager.Instance.Log("[plan_node_begin]{0}\n", ni);
                a.LogVariables(true);
            }

#endif
        }

        private void LogPlanNodePreconditionFailed(Agent a, BehaviorNode n)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, n, null);

                LogManager.Instance.Log("[plan_node_pre_failed]{0}\n", ni);
            }

#endif
        }

        private void LogPlanNodeEnd(Agent a, BehaviorNode n, string result)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, n, null);

                LogManager.Instance.Log("[plan_node_end]{0} {1}\n", ni, result);
            }

#endif
        }

        public void LogPlanReferenceTreeEnter(Agent a, ReferencedBehavior referencedNode)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, referencedNode, null);
                LogManager.Instance.Log("[plan_referencetree_enter]{0} {1}.xml\n", ni, referencedNode.GetReferencedTree(a));
            }

#endif
        }

        public void LogPlanReferenceTreeExit(Agent a, ReferencedBehavior referencedNode)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, referencedNode, null);
                LogManager.Instance.Log("[plan_referencetree_exit]{0} {1}.xml\n", ni, referencedNode.GetReferencedTree(a));
            }

#endif
        }

        private void LogPlanMethodBegin(Agent a, BehaviorNode m)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, m, null);
                LogManager.Instance.Log("[plan_method_begin]{0}\n", ni);

                a.LogVariables(true);
            }

#endif
        }

        private void LogPlanMethodEnd(Agent a, BehaviorNode m, string result)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, m, null);
                LogManager.Instance.Log("[plan_method_end]{0} {1}\n", ni, result);
            }

#endif
        }

        public void LogPlanForEachBegin(Agent a, DecoratorIterator pForEach, int index, int count)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, pForEach, null);
                LogManager.Instance.Log("[plan_foreach_begin]{0} {1} {2}\n", ni, index, count);
                a.LogVariables(true);
            }

#endif
        }

        public void LogPlanForEachEnd(Agent a, DecoratorIterator pForEach, int index, int count, string result)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, pForEach, null);
                LogManager.Instance.Log("[plan_foreach_end]{0} {1} {2} {3}\n", ni, index, count, result);
            }

#endif
        }

        #endregion Log

        #region Plan Builder

        private PlannerTask BuildPlan(Task root)
        {
            LogManager.Instance.PLanningClearCache();

            int depth = this.agent.Variables.Depth;

            PlannerTask rootTask = null;
            using(var currentState = this.agent.Variables.Push(true))
            {
                this.agent.PlanningTop = this.agent.Variables.Top;
                Debug.Check(this.agent.PlanningTop >= 0);

                LogPlanBegin(this.agent, root);

                rootTask = this.decomposeNode(root, 0);

                LogPlanEnd(this.agent, root);

#if !BEHAVIAC_RELEASE
                BehaviorTask.CHECK_BREAKPOINT(this.agent, root, "plan", EActionResult.EAR_all);
#endif

                this.agent.PlanningTop = -1;
            }

            Debug.Check(this.agent.Variables.Depth == depth);

            return rootTask;
        }

        #endregion Plan Builder

        #region Private decompose methods

        public PlannerTask decomposeNode(BehaviorNode node, int depth)
        {
            try
            {
                // Ensure that the planner does not get stuck in an infinite loop
                if (depth >= 256)
                {
                    Debug.LogError("Exceeded task nesting depth. Does the graph contain an invalid cycle?");
                    return null;
                }

                LogPlanNodeBegin(this.agent, node);

                int depth1 = this.agent.Variables.Depth;
                PlannerTask taskAdded = null;

                bool isPreconditionOk = node.CheckPreconditions(this.agent, false);

                if (isPreconditionOk)
                {
                    bool bOk = true;
                    taskAdded = PlannerTask.Create(node, this.agent);

                    if (node is Action)
                    {
                        //nothing to do for action
                        Debug.Check(true);
                    }
                    else
                    {
                        Debug.Check(taskAdded is PlannerTaskComplex);
                        PlannerTaskComplex seqTask = taskAdded as PlannerTaskComplex;

                        bOk = this.decomposeComplex(node, seqTask, depth);
                    }

                    if (bOk)
                    {
                        node.ApplyEffects(this.agent, Effector.EPhase.E_SUCCESS);
                    }
                    else
                    {
                        BehaviorTask.DestroyTask(taskAdded);
                        taskAdded = null;
                    }
                }
                else
                {
                    //precondition failed
                    LogPlanNodePreconditionFailed(this.agent, node);
                }

                LogPlanNodeEnd(this.agent, node, taskAdded != null ? "success" : "failure");

                Debug.Check(this.agent.Variables.Depth == depth1);

                return taskAdded;
            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return null;
        }

        private bool decomposeComplex(BehaviorNode node, PlannerTaskComplex seqTask, int depth)
        {
            try
            {

                int depth1 = this.agent.Variables.Depth;
                bool bOk = false;
                bOk = node.decompose(node, seqTask, depth, this);

                Debug.Check(this.agent.Variables.Depth == depth1);
                return bOk;
            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return false;
        }

        public PlannerTask decomposeTask(Task task, int depth)
        {
            var methodsCount = task.GetChildrenCount();

            if (methodsCount == 0)
            {
                return null;
            }

            int depth1 = this.agent.Variables.Depth;
            PlannerTask methodTask = null;

            for (int i = 0; i < methodsCount; i++)
            {
                BehaviorNode method = task.GetChild(i);
                Debug.Check(method is Method);
                int depth2 = this.agent.Variables.Depth;
                using(var currentState = this.agent.Variables.Push(false))
                {
                    LogPlanMethodBegin(this.agent, method);
                    methodTask = this.decomposeNode(method, depth + 1);
                    LogPlanMethodEnd(this.agent, method, methodTask != null ? "success" : "failure");

                    if (methodTask != null)
                    {
                        // succeeded
                        break;
                    }
                }

                Debug.Check(this.agent.Variables.Depth == depth2);
            }

            Debug.Check(this.agent.Variables.Depth == depth1);
            return methodTask;
        }

        #endregion Private decompose methods
    }
}
#endif//
