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
    public class DecoratorLoop : DecoratorCount
    {
        public DecoratorLoop()
        {
        }

        //~DecoratorLoop()
        //{
        //}

#if BEHAVIAC_USE_HTN
        public override bool decompose(BehaviorNode node, PlannerTaskComplex seqTask, int depth, Planner planner)
        {
            DecoratorLoop loop = (DecoratorLoop)node;
            bool bOk = false;
            int childCount = loop.GetChildrenCount();
            Debug.Check(childCount == 1);
            BehaviorNode childNode = loop.GetChild(0);
            PlannerTask childTask = planner.decomposeNode(childNode, depth);

            if (childTask != null)
            {
                seqTask.AddChild(childTask);
                bOk = true;
            }

            return bOk;
        }
#endif//

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "DoneWithinFrame")
                {
                    if (p.value == "true")
                    {
                        this.m_bDoneWithinFrame = true;
                    }
                }
            }
        }

        public int Count(Agent pAgent)
        {
            return base.GetCount(pAgent);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorLoop))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected bool m_bDoneWithinFrame;

        protected override BehaviorTask createTask()
        {
            DecoratorLoopTask pTask = new DecoratorLoopTask();

            return pTask;
        }

        ///Returns EBTStatus.BT_FAILURE for the specified number of iterations, then returns EBTStatus.BT_SUCCESS after that
        private class DecoratorLoopTask : DecoratorCountTask
        {
            public DecoratorLoopTask()
            {
            }

            //~DecoratorLoopTask()
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

            protected override EBTStatus decorate(EBTStatus status)
            {
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

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.m_node is DecoratorLoop);
                DecoratorLoop node = (DecoratorLoop)this.m_node;

                if (node.m_bDoneWithinFrame)
                {
                    Debug.Check(this.m_n >= 0);
                    Debug.Check(this.m_root != null);

                    EBTStatus status = EBTStatus.BT_INVALID;

                    for (int i = 0; i < this.m_n; ++i)
                    {
                        status = this.m_root.exec(pAgent, childStatus);

                        if (node.m_bDecorateWhenChildEnds)
                        {
                            while (status == EBTStatus.BT_RUNNING)
                            {
                                status = base.update(pAgent, childStatus);
                            }
                        }

                        if (status == EBTStatus.BT_FAILURE)
                        {
                            return EBTStatus.BT_FAILURE;
                        }
                    }

                    return EBTStatus.BT_SUCCESS;
                }

                return base.update(pAgent, childStatus);
            }

        }
    }
}
