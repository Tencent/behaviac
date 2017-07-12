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
    public class SelectorProbability : BehaviorNode
    {
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "RandomGenerator")
                {
                    this.m_method = AgentMeta.ParseMethod(p.value);
                }
            }
        }

        public override void AddChild(BehaviorNode pBehavior)
        {
            Debug.Check(pBehavior is DecoratorWeight);
            DecoratorWeight pDW = (DecoratorWeight)(pBehavior);

            if (pDW != null)
            {
                base.AddChild(pBehavior);
            }
            else
            {
                Debug.Check(false, "only DecoratorWeightTask can be children");
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is SelectorProbability))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            SelectorProbabilityTask pTask = new SelectorProbabilityTask();

            return pTask;
        }

        protected IMethod m_method;

        ///Executes behaviors randomly, based on a given set of weights.
        /** The weights are not percentages, but rather simple ratios.
        For example, if there were two children with a weight of one, each would have a 50% chance of being executed.
        If another child with a weight of eight were added, the previous children would have a 10% chance of being executed, and the new child would have an 80% chance of being executed.
        This weight system is intended to facilitate the fine-tuning of behaviors.
        */

        private class SelectorProbabilityTask : CompositeTask
        {
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
				
				//if the following assert failed, just comment it out
                //Debug.Check(this.m_activeChildIndex == CompositeTask.InvalidChildIndex);

				//to reset it anyway in case onexit is not called for some reason
				this.m_activeChildIndex = CompositeTask.InvalidChildIndex;

                //SelectorProbability pSelectorProbabilityNode = this.GetNode() is SelectorProbability;

                this.m_weightingMap.Clear();
                this.m_totalSum = 0;

                for (int i = 0; i < this.m_children.Count; ++i)
                {
                    BehaviorTask task = this.m_children[i];

                    Debug.Check(task is DecoratorWeight.DecoratorWeightTask);
                    DecoratorWeight.DecoratorWeightTask pWT = (DecoratorWeight.DecoratorWeightTask)task;

                    int weight = pWT.GetWeight(pAgent);
                    this.m_weightingMap.Add(weight);
                    this.m_totalSum += weight;
                }

                Debug.Check(this.m_weightingMap.Count == this.m_children.Count);

                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                this.m_activeChildIndex = CompositeTask.InvalidChildIndex;
                base.onexit(pAgent, s);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.GetNode() is SelectorProbability);
                SelectorProbability pSelectorProbabilityNode = (SelectorProbability)(this.GetNode());

                if (childStatus != EBTStatus.BT_RUNNING)
                {
                    return childStatus;
                }

                //check if we've already chosen a node to run
                if (this.m_activeChildIndex != CompositeTask.InvalidChildIndex)
                {
                    BehaviorTask pNode = this.m_children[this.m_activeChildIndex];

                    EBTStatus status = pNode.exec(pAgent);

                    return status;
                }

                Debug.Check(this.m_weightingMap.Count == this.m_children.Count);

                //generate a number between 0 and the sum of the weights
                float chosen = this.m_totalSum * CompositeStochastic.CompositeStochasticTask.GetRandomValue(pSelectorProbabilityNode.m_method, pAgent);

                float sum = 0;

                for (int i = 0; i < this.m_children.Count; ++i)
                {
                    int w = this.m_weightingMap[i];

                    sum += w;

                    if (w > 0 && sum >= chosen)   //execute this node
                    {
                        BehaviorTask pChild = this.m_children[i];

                        EBTStatus status = pChild.exec(pAgent);

                        if (status == EBTStatus.BT_RUNNING)
                        {
                            this.m_activeChildIndex = i;
                        }
                        else
                        {
                            this.m_activeChildIndex = CompositeTask.InvalidChildIndex;
                        }

                        return status;
                    }
                }

                return EBTStatus.BT_FAILURE;
            }

            private List<int> m_weightingMap = new List<int>();
            private int m_totalSum;
        }
    }
}
