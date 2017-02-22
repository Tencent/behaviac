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
    public class Assignment : BehaviorNode
    {
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "CastRight")
                {
                    this.m_bCast = (p.value == "true");
                }
                else if (p.name == "Opl")
                {
                    this.m_opl = AgentMeta.ParseProperty(p.value);
                }
                else if (p.name == "Opr")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opr = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_opr = AgentMeta.ParseMethod(p.value);
                    }
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Assignment))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            return new AssignmentTask();
        }

        protected IInstanceMember m_opl;
        protected IInstanceMember m_opr;
        protected bool m_bCast = false;

        private class AssignmentTask : LeafTask
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

                Debug.Check(this.GetNode() is Assignment);
                Assignment pAssignmentNode = (Assignment)(this.GetNode());

                EBTStatus result = EBTStatus.BT_SUCCESS;

                if (pAssignmentNode.m_opl != null)
                {
                    if (pAssignmentNode.m_bCast)
                    {
                        pAssignmentNode.m_opl.SetValueAs(pAgent, pAssignmentNode.m_opr);
                    }
                    else
                    {
                        pAssignmentNode.m_opl.SetValue(pAgent, pAssignmentNode.m_opr);
                    }
                }
                else
                {
                    result = pAssignmentNode.update_impl(pAgent, childStatus);
                }

                return result;
            }
        }
    }
}
