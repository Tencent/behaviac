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

using System.Collections.Generic;

namespace behaviac
{
    public class SelectorLoop : BehaviorNode
    {
        protected bool m_bResetChildren = false;

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "ResetChildren")
                {
                    this.m_bResetChildren = (p.value == "true");
                    break;
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is SelectorLoop))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public override bool IsManagingChildrenAsSubTrees()
        {
            return true;
        }

        protected override BehaviorTask createTask()
        {
            SelectorLoopTask pTask = new SelectorLoopTask();

            return pTask;
        }

        // ============================================================================
        /**
        behavives similarly to SelectorTask, i.e. executing chidren until the first successful one.

        however, in the following ticks, it constantly monitors the higher priority nodes.
        if any one's precondtion node returns success, it picks it and execute it, and before executing,
        it first cleans up the original executing one.

        all its children are WithPreconditionTask or its derivatives.
        */

        public class SelectorLoopTask : CompositeTask
        {
            protected override void addChild(BehaviorTask pBehavior)
            {
                base.addChild(pBehavior);

                Debug.Check(pBehavior is WithPreconditionTask);
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is SelectorLoopTask);
                SelectorLoopTask ttask = (SelectorLoopTask)target;

                ttask.m_activeChildIndex = this.m_activeChildIndex;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID activeChildId = new CSerializationID("activeChild");
                node.setAttr(activeChildId, this.m_activeChildIndex);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                //reset the action child as it will be checked in the update
                this.m_activeChildIndex = CompositeTask.InvalidChildIndex;
                Debug.Check(this.m_activeChildIndex == CompositeTask.InvalidChildIndex);

                return base.onenter(pAgent);
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
            }

            //no current task, as it needs to update every child for every update
            protected override EBTStatus update_current(Agent pAgent, EBTStatus childStatus)
            {
                EBTStatus s = this.update(pAgent, childStatus);

                return s;
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                int idx = -1;

                if (childStatus != EBTStatus.BT_RUNNING)
                {
                    Debug.Check(this.m_activeChildIndex != CompositeTask.InvalidChildIndex);

                    if (childStatus == EBTStatus.BT_SUCCESS)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                    else if (childStatus == EBTStatus.BT_FAILURE)
                    {
                        //the next for starts from (idx + 1), so that it starts from next one after this failed one
                        idx = this.m_activeChildIndex;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }

                //checking the preconditions and take the first action tree
                int index = (int) - 1;

                for (int i = (idx + 1); i < this.m_children.Count; ++i)
                {
                    Debug.Check(this.m_children[i] is WithPreconditionTask);
                    WithPreconditionTask pSubTree = (WithPreconditionTask)this.m_children[i];

                    BehaviorTask pre = pSubTree.PreconditionNode;

                    EBTStatus status = pre.exec(pAgent);

                    if (status == EBTStatus.BT_SUCCESS)
                    {
                        index = i;
                        break;
                    }
                }

                //clean up the current ticking action tree
                if (index != (int) - 1)
                {
                    if (this.m_activeChildIndex != CompositeTask.InvalidChildIndex)
                    {
                        bool abortChild = (this.m_activeChildIndex != index);
                        if (!abortChild)
                        {
                            SelectorLoop pSelectorLoop = (SelectorLoop)(this.GetNode());
                            Debug.Check(pSelectorLoop != null);

                            if (pSelectorLoop != null)
                            {
                                abortChild = pSelectorLoop.m_bResetChildren;
                            }
                        }

                        if (abortChild)
                        {
                            WithPreconditionTask pCurrentSubTree = (WithPreconditionTask)this.m_children[this.m_activeChildIndex];
                            //BehaviorTask action = pCurrentSubTree.ActionNode;
                            pCurrentSubTree.abort(pAgent);

                            //don't set it here
                            //this.m_activeChildIndex = index;
                        }
                    }

                    for (int i = index; i < this.m_children.Count; ++i)
                    {
                        WithPreconditionTask pSubTree = (WithPreconditionTask)this.m_children[i];

                        if (i > index)
                        {
                            BehaviorTask pre = pSubTree.PreconditionNode;
                            EBTStatus status = pre.exec(pAgent);

                            //to search for the first one whose precondition is success
                            if (status != EBTStatus.BT_SUCCESS)
                            {
                                continue;
                            }
                        }

                        BehaviorTask action = pSubTree.ActionNode;
                        EBTStatus s = action.exec(pAgent);

                        if (s == EBTStatus.BT_RUNNING)
                        {
                            this.m_activeChildIndex = i;
                            pSubTree.m_status = EBTStatus.BT_RUNNING;
                        }
                        else
                        {
                            pSubTree.m_status = s;

                            if (s == EBTStatus.BT_FAILURE)
                            {
                                //THE ACTION failed, to try the next one
                                continue;
                            }
                        }

                        Debug.Check(s == EBTStatus.BT_RUNNING || s == EBTStatus.BT_SUCCESS);

                        return s;
                    }
                }

                return EBTStatus.BT_FAILURE;
            }
        }
    }
}
