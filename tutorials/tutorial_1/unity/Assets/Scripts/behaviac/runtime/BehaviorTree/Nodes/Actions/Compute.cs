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

using System;
using System.Collections.Generic;

namespace behaviac
{
    public class Compute : BehaviorNode
    {
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Opl")
                {
                    this.m_opl = AgentMeta.ParseProperty(p.value);
                }
                else if (p.name == "Operator")
                {
                    Debug.Check(p.value == "Add" || p.value == "Sub" || p.value == "Mul" || p.value == "Div");

                    this.m_operator = OperationUtils.ParseOperatorType(p.value);
                }
                else if (p.name == "Opr1")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opr1 = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_opr1 = AgentMeta.ParseMethod(p.value);
                    }
                }
                else if (p.name == "Opr2")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opr2 = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_opr2 = AgentMeta.ParseMethod(p.value);
                    }
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Compute))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            return new ComputeTask();
        }

        protected IInstanceMember m_opl;
        protected IInstanceMember m_opr1;
        protected IInstanceMember m_opr2;
        protected EOperatorType m_operator = EOperatorType.E_INVALID;

        private class ComputeTask : LeafTask
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
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);

                EBTStatus result = EBTStatus.BT_SUCCESS;

                Debug.Check(this.GetNode() is Compute);
                Compute pComputeNode = (Compute)(this.GetNode());

#if !BEHAVIAC_RELEASE

                // debug code only
                if (pComputeNode.m_operator == EOperatorType.E_DIV)
                {
                    object v = pComputeNode.m_opr2.GetValueObject(pAgent);

                    float f = Convert.ToSingle(v);

                    if (Math.Abs(f) < 0.00001f)
                    {
                        Debug.LogError(string.Format("Compute {0}: right is 0", this.m_id));
                    }

                    double d = Convert.ToDouble(v);

                    if (Math.Abs(d) < 0.00001)
                    {
                        Debug.LogError(string.Format("Compute {0}: right is 0", this.m_id));
                    }

                    int n = Convert.ToInt32(v);

                    if (n == 0)
                    {
                        Debug.LogError(string.Format("Compute {0} has right is 0", this.m_id));
                    }
                }

#endif

                if (pComputeNode.m_opl != null)
                {
                    pComputeNode.m_opl.Compute(pAgent, pComputeNode.m_opr1, pComputeNode.m_opr2, pComputeNode.m_operator);
                }
                else
                {
                    result = pComputeNode.update_impl(pAgent, childStatus);
                }

                return result;
            }
        }
    }
}
