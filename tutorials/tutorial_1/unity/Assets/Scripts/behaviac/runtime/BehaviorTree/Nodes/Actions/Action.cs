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
    // ============================================================================
    public class Action : BehaviorNode
    {
        protected IMethod m_method;
        protected IMethod m_resultFunctor;
        protected EBTStatus m_resultOption = EBTStatus.BT_INVALID;

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Method")
                {
                    if (!string.IsNullOrEmpty(p.value))
                    {
                        this.m_method = AgentMeta.ParseMethod(p.value);
                    }
                }
                else if (p.name == "ResultOption")
                {
                    if (p.value == "BT_INVALID")
                    {
                        m_resultOption = EBTStatus.BT_INVALID;
                    }
                    else if (p.value == "BT_FAILURE")
                    {
                        m_resultOption = EBTStatus.BT_FAILURE;
                    }
                    else if (p.value == "BT_RUNNING")
                    {
                        m_resultOption = EBTStatus.BT_RUNNING;
                    }
                    else
                    {
                        m_resultOption = EBTStatus.BT_SUCCESS;
                    }
                }
                else if (p.name == "ResultFunctor")
                {
                    if (!string.IsNullOrEmpty(p.value))
                    {
                        this.m_resultFunctor = AgentMeta.ParseMethod(p.value); ;
                    }
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Action))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public EBTStatus Execute(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus result = EBTStatus.BT_SUCCESS;

            if (this.m_method != null)
            {
                if (this.m_resultOption != EBTStatus.BT_INVALID)
                {
                    this.m_method.Run(pAgent);

                    result = this.m_resultOption;
                }
                else
                {
                    if (this.m_resultFunctor != null)
                    {
                        IValue returnValue = this.m_resultFunctor.GetIValue(pAgent, this.m_method);

                        result = ((TValue<EBTStatus>)returnValue).value;
                    }
                    else
                    {
                        IValue returnValue = this.m_method.GetIValue(pAgent);

                        Debug.Check(returnValue is TValue<EBTStatus>, "method's return type is not EBTStatus");

                        result = ((TValue<EBTStatus>)returnValue).value;
                    }
                }
            }
            else
            {
                result = this.update_impl(pAgent, childStatus);
            }

            return result;
        }

        protected override BehaviorTask createTask()
        {
            ActionTask pTask = new ActionTask();

            return pTask;
        }

        private class ActionTask : LeafTask
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

                Debug.Check(this.GetNode() is Action, "node is not an Action");
                Action pActionNode = (Action)(this.GetNode());

                EBTStatus result = pActionNode.Execute(pAgent, childStatus);

                return result;
            }
        }
    }
}
