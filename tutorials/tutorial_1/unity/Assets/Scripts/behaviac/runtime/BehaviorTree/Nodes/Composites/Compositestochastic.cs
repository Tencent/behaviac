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
    public abstract class CompositeStochastic : BehaviorNode
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

        public bool CheckIfInterrupted(Agent pAgent)
        {
            bool bInterrupted = this.EvaluteCustomCondition(pAgent);

            return bInterrupted;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is CompositeStochastic))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected IMethod m_method;

        public class CompositeStochasticTask : CompositeTask
        {
            //generate a random float value between 0 and 1.
            public static float GetRandomValue(IMethod method, Agent pAgent)
            {
                float value = 0;

                if (method != null)
                {
                    value = ((CInstanceMember<float>)method).GetValue(pAgent);
                }
                else
                {
                    value = RandomGenerator.GetInstance().GetRandom();
                }

                Debug.Check(value >= 0.0f && value < 1.0f);
                return value;
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is CompositeStochasticTask);
                CompositeStochasticTask ttask = (CompositeStochasticTask)target;

                ttask.m_set = this.m_set;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID setId = new CSerializationID("set");
                node.setAttr(setId, this.m_set);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                Debug.Check(this.m_children.Count > 0);

                this.random_child(pAgent);

                this.m_activeChildIndex = 0;
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
            }

            private void random_child(Agent pAgent)
            {
                Debug.Check(this.GetNode() == null || this.GetNode() is CompositeStochastic);
                CompositeStochastic pNode = (CompositeStochastic)(this.GetNode());

                int n = this.m_children.Count;

                if (this.m_set.Count != n)
                {
                    this.m_set.Clear();

                    for (int i = 0; i < n; ++i)
                    {
                        this.m_set.Add(i);
                    }
                }

                for (int i = 0; i < n; ++i)
                {
                    int index1 = (int)(n * GetRandomValue(pNode != null ? pNode.m_method : null, pAgent));
                    Debug.Check(index1 < n);

                    int index2 = (int)(n * GetRandomValue(pNode != null ? pNode.m_method : null, pAgent));
                    Debug.Check(index2 < n);

                    //swap
                    if (index1 != index2)
                    {
                        int old = this.m_set[index1];
                        this.m_set[index1] = this.m_set[index2];
                        this.m_set[index2] = old;
                    }
                }
            }

            protected List<int> m_set = new List<int>();
        }
    }
}
