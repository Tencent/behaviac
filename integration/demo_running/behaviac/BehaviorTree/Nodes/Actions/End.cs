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
    public class End : BehaviorNode
    {
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "EndStatus")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_endStatus = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_endStatus = AgentMeta.ParseMethod(p.value);
                    }
                }
                else if (p.name == "EndOutside")
                {
                    this.m_endOutside = (p.value == "true");
                }
            }
        }

        protected virtual EBTStatus GetStatus(Agent pAgent)
        {
            return this.m_endStatus != null ? ((CInstanceMember<EBTStatus>)this.m_endStatus).GetValue(pAgent) : EBTStatus.BT_SUCCESS;
        }

        protected bool GetEndOutside()
        {
            return this.m_endOutside;
        }

        protected IInstanceMember m_endStatus;
        protected bool            m_endOutside;

        protected override BehaviorTask createTask()
        {
            EndTask pTask = new EndTask();

            return pTask;
        }

        private class EndTask : LeafTask
        {
            private EBTStatus GetStatus(Agent pAgent)
            {
                End pEndNode = this.GetNode() as End;
                EBTStatus status = pEndNode != null ? pEndNode.GetStatus(pAgent) : EBTStatus.BT_SUCCESS;
                Debug.Check(status == EBTStatus.BT_SUCCESS || status == EBTStatus.BT_FAILURE);
                return status;
            }

            private bool GetEndOutside()
            {
                End pEndNode = this.GetNode() as End;
                return pEndNode != null ? pEndNode.GetEndOutside() : false;
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
                BehaviorTreeTask rooTask = null;
                if (!this.GetEndOutside())
                {
                    rooTask = this.RootTask;
                }
                else if (pAgent != null)
                {
                    rooTask = pAgent.CurrentTreeTask;
                }

                if (rooTask != null)
                {
                    rooTask.setEndStatus(this.GetStatus(pAgent));
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}
