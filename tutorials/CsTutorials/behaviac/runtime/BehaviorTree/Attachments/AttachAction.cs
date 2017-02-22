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
    public class AttachAction : BehaviorNode
    {
        public class ActionConfig
        {
            protected IInstanceMember m_opl;
            protected IInstanceMember m_opr1;
            protected IInstanceMember m_opr2;
            public EOperatorType m_operator = EOperatorType.E_INVALID;

            protected ActionConfig()
            {
            }

            public virtual bool load(List<property_t> properties)
            {
                for (int i = 0; i < properties.Count; ++i)
                {
                    property_t p = properties[i];

                    if (p.name == "Opl")
                    {
                        if (StringUtils.IsValidString(p.value))
                        {
                            int pParenthesis = p.value.IndexOf('(');

                            if (pParenthesis == -1)
                            {
                                this.m_opl = AgentMeta.ParseProperty(p.value);
                            }
                            else
                            {
                                this.m_opl = AgentMeta.ParseMethod(p.value);
                            }
                        }
                    }
                    else if (p.name == "Opr1")
                    {
                        if (StringUtils.IsValidString(p.value))
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
                    }
                    else if (p.name == "Operator")
                    {
                        this.m_operator = OperationUtils.ParseOperatorType(p.value);
                    }
                    else if (p.name == "Opr2")
                    {
                        if (StringUtils.IsValidString(p.value))
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

                return this.m_opl != null;
            }

            public bool Execute(Agent pAgent)
            {
                bool bValid = false;

                // action
                if (this.m_operator == EOperatorType.E_INVALID)
                {
                    if (this.m_opl != null)
                    {
                        Debug.Check(this.m_opl is IMethod);
                        IMethod method = this.m_opl as IMethod;

                        if (method != null)
                        {
                            method.Run(pAgent);

                            bValid = true;
                        }
                    }
                }

                // assign
                else if (this.m_operator == EOperatorType.E_ASSIGN)
                {
                    if (this.m_opl != null)
                    {
                        this.m_opl.SetValue(pAgent, this.m_opr2);

                        bValid = true;
                    }
                }

                // compute
                else if (this.m_operator >= EOperatorType.E_ADD && this.m_operator <= EOperatorType.E_DIV)
                {
                    if (this.m_opl != null)
                    {
                        this.m_opl.Compute(pAgent, this.m_opr1, this.m_opr2, m_operator);

                        bValid = true;
                    }
                }

                // compare
                else if (this.m_operator >= EOperatorType.E_EQUAL && this.m_operator <= EOperatorType.E_LESSEQUAL)
                {
                    if (this.m_opl != null)
                    {
                        bValid = this.m_opl.Compare(pAgent, this.m_opr2, m_operator);
                    }
                }

                return bValid;
            }


        }

        protected ActionConfig m_ActionConfig;

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            this.m_ActionConfig.load(properties);
        }

        public override bool Evaluate(Agent pAgent)
        {
            bool bValid = this.m_ActionConfig.Execute(pAgent);

            if (!bValid)
            {
                EBTStatus childStatus = EBTStatus.BT_INVALID;
                bValid = (EBTStatus.BT_SUCCESS == this.update_impl(pAgent, childStatus));
            }

            return bValid;
        }

        public virtual bool Evaluate(Agent pAgent, EBTStatus status)
        {
            bool bValid = this.m_ActionConfig.Execute(pAgent);

            if (!bValid)
            {
                EBTStatus childStatus = EBTStatus.BT_INVALID;
                bValid = (EBTStatus.BT_SUCCESS == this.update_impl(pAgent, childStatus));
            }

            return bValid;
        }

        protected override BehaviorTask createTask()
        {
            Debug.Check(false);
            return null;
        }
    }
}
