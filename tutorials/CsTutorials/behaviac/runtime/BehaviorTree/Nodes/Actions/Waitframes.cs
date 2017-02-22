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
    public class WaitFrames : BehaviorNode
    {
        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Frames")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_frames = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_frames = AgentMeta.ParseMethod(p.value);
                    }
                }
            }
        }

        protected virtual int GetFrames(Agent pAgent)
        {
            if (this.m_frames != null)
            {
                Debug.Check(this.m_frames is CInstanceMember<int>);
                return ((CInstanceMember<int>)this.m_frames).GetValue(pAgent);
            }

            return 0;
        }

        protected override BehaviorTask createTask()
        {
            WaitFramesTask pTask = new WaitFramesTask();

            return pTask;
        }

        private IInstanceMember m_frames;

        private class WaitFramesTask : LeafTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is WaitFramesTask);
                WaitFramesTask ttask = (WaitFramesTask)target;
                ttask.m_start = this.m_start;
                ttask.m_frames = this.m_frames;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID startId = new CSerializationID("start");
                node.setAttr(startId, this.m_start);

                CSerializationID framesId = new CSerializationID("frames");
                node.setAttr(framesId, this.m_frames);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                this.m_start = Workspace.Instance.FrameSinceStartup;
                this.m_frames = this.GetFrames(pAgent);

                return (this.m_frames >= 0);
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(childStatus == EBTStatus.BT_RUNNING);

                if (Workspace.Instance.FrameSinceStartup - this.m_start + 1 >= this.m_frames)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                return EBTStatus.BT_RUNNING;
            }

            private int GetFrames(Agent pAgent)
            {
                Debug.Check(this.GetNode() is WaitFrames);
                WaitFrames pWaitNode = (WaitFrames)(this.GetNode());

                return pWaitNode != null ? pWaitNode.GetFrames(pAgent) : 0;
            }

            private int m_start;
            private int m_frames;
        }
    }
}
