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
    public class Selector : BehaviorNode
    {
        public Selector()
        {
        }

        //~Selector()
        //{
        //}

#if BEHAVIAC_USE_HTN
        public override bool decompose(BehaviorNode node, PlannerTaskComplex seqTask, int depth, Planner planner)
        {
            Selector sel = (Selector)node;

            bool bOk = false;
            int childCount = sel.GetChildrenCount();
            int i = 0;

            for (; i < childCount; ++i)
            {
                BehaviorNode childNode = sel.GetChild(i);
                PlannerTask childTask = planner.decomposeNode(childNode, depth);

                if (childTask != null)
                {
                    seqTask.AddChild(childTask);
                    bOk = true;
                    break;
                }
            }

            return bOk;
        }
#endif//

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Selector))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public override bool Evaluate(Agent pAgent)
        {
            bool ret = true;

            for (int i = 0; i < this.m_children.Count; ++i)
            {
                BehaviorNode c = this.m_children[i];
                ret = c.Evaluate(pAgent);

                if (ret)
                {
                    break;
                }
            }

            return ret;
        }

        public EBTStatus SelectorUpdate(Agent pAgent, EBTStatus childStatus, ref int activeChildIndex, List<BehaviorTask> children)
        {
            EBTStatus s = childStatus;

            for (; ;)
            {
                Debug.Check(activeChildIndex < children.Count);

                if (s == EBTStatus.BT_RUNNING)
                {
                    BehaviorTask pBehavior = children[activeChildIndex];

                    if (this.CheckIfInterrupted(pAgent))
                    {
                        return EBTStatus.BT_FAILURE;
                    }

                    s = pBehavior.exec(pAgent);
                }

                // If the child succeeds, or keeps running, do the same.
                if (s != EBTStatus.BT_FAILURE)
                {
                    return s;
                }

                // Hit the end of the array, job done!
                ++activeChildIndex;

                if (activeChildIndex >= children.Count)
                {
                    return EBTStatus.BT_FAILURE;
                }

                s = EBTStatus.BT_RUNNING;
            }
        }

        public bool CheckIfInterrupted(Agent pAgent)
        {
            bool bInterrupted = this.EvaluteCustomCondition(pAgent);

            return bInterrupted;
        }

        protected override BehaviorTask createTask()
        {
            SelectorTask pTask = new SelectorTask();

            return pTask;
        }

        // ============================================================================
        public class SelectorTask : CompositeTask
        {
            public SelectorTask()
            {
            }

            //~SelectorTask()
            //{
            //}

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                Debug.Check(this.m_children.Count > 0);
                this.m_activeChildIndex = 0;
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.m_activeChildIndex < this.m_children.Count);
                Selector node = this.m_node as Selector;

                return node.SelectorUpdate(pAgent, childStatus, ref this.m_activeChildIndex, this.m_children);
            }
        }
    }
}
