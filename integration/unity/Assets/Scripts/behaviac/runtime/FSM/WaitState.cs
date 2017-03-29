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
    public class WaitState : State
    {
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Time")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_time = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_time = AgentMeta.ParseMethod(p.value);
                    }
                }
            }
        }

        protected virtual double GetTime(Agent pAgent)
        {
            double time = 0;

            if (this.m_time != null)
            {
                if (this.m_time is CInstanceMember<double>)
                {
                    time = ((CInstanceMember<double>)this.m_time).GetValue(pAgent);
                }
                else if (this.m_time is CInstanceMember<float>)
                {
                    time = ((CInstanceMember<float>)this.m_time).GetValue(pAgent);
                }
                else if (this.m_time is CInstanceMember<int>)
                {
                    time = ((CInstanceMember<int>)this.m_time).GetValue(pAgent);
                }
            }

            return time;
        }

        protected virtual int GetIntTime(Agent pAgent)
        {
            int time = 0;

            if (this.m_time != null)
            {
                if (this.m_time is CInstanceMember<int>)
                {
                    time = ((CInstanceMember<int>)this.m_time).GetValue(pAgent);
                }
            }

            return time;
        }

        protected IInstanceMember m_time;

        protected override BehaviorTask createTask()
        {
            WaitStateTask pTask = new WaitStateTask();

            return pTask;
        }

        private class WaitStateTask : State.StateTask
        {
            private double m_start = 0;
            private double m_time = 0;
            private long m_intStart = 0;
            private int m_intTime = 0;

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is WaitStateTask);
                WaitStateTask ttask = (WaitStateTask)target;

                ttask.m_start = this.m_start;
                ttask.m_time = this.m_time;

                ttask.m_intStart = this.m_intStart;
                ttask.m_intTime = this.m_intTime;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID startId = new CSerializationID("start");
                node.setAttr(startId, this.m_start);

                CSerializationID timeId = new CSerializationID("time");
                node.setAttr(timeId, this.m_time);

                CSerializationID intStartId = new CSerializationID("intstart");
                node.setAttr(intStartId, this.m_intStart);

                CSerializationID intTimeId = new CSerializationID("inttime");
                node.setAttr(intTimeId, this.m_intTime);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            private double GetTime(Agent pAgent)
            {
                WaitState pWaitNode = this.GetNode() as WaitState;

                return pWaitNode != null ? pWaitNode.GetTime(pAgent) : 0;
            }

            private int GetIntTime(Agent pAgent)
            {
                WaitState pWaitNode = this.GetNode() as WaitState;

                return pWaitNode != null ? pWaitNode.GetIntTime(pAgent) : 0;
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_nextStateId = -1;

                if (Workspace.Instance.UseIntValue)
                {
                    this.m_intStart = Workspace.Instance.IntValueSinceStartup;
                    this.m_intTime = this.GetIntTime(pAgent);

                    return (this.m_intTime >= 0);
                }
                else
                {
                    this.m_start = Workspace.Instance.DoubleValueSinceStartup;
                    this.m_time = this.GetTime(pAgent);

                    return (this.m_time >= 0);
                }
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);
                Debug.Check(this.m_node is WaitState, "node is not an WaitState");
                WaitState pStateNode = (WaitState)this.m_node;

                if (Workspace.Instance.UseIntValue)
                {
                    if (Workspace.Instance.IntValueSinceStartup - this.m_intStart >= this.m_intTime)
                    {
                        pStateNode.Update(pAgent, out this.m_nextStateId);
                        return EBTStatus.BT_SUCCESS;
                    }
                }
                else
                {
                    if (Workspace.Instance.DoubleValueSinceStartup - this.m_start >= this.m_time)
                    {
                        pStateNode.Update(pAgent, out this.m_nextStateId);
                        return EBTStatus.BT_SUCCESS;
                    }
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}
