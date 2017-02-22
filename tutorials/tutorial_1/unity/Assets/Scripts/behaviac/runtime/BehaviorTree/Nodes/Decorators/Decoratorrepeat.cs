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
    public class DecoratorRepeat : DecoratorCount
    {
        public DecoratorRepeat()
        {
        }

        //~DecoratorRepeat()
        //{
        //}

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public int Count(Agent pAgent)
        {
            return base.GetCount(pAgent);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorRepeat))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorRepeatTask pTask = new DecoratorRepeatTask();

            return pTask;
        }

        ///Returns EBTStatus.BT_FAILURE for the specified number of iterations, then returns EBTStatus.BT_SUCCESS after that
        private class DecoratorRepeatTask : DecoratorCountTask
        {
            public DecoratorRepeatTask()
            {
            }

            //~DecoratorRepeatTask()
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
                Debug.Check(false, "unsurpported");

                return EBTStatus.BT_INVALID;
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.m_node is DecoratorNode);
                DecoratorNode node = (DecoratorNode)this.m_node;

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
        }
    }
}
