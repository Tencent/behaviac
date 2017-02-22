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
    public class DecoratorLoopUntil : DecoratorCount
    {
        public DecoratorLoopUntil()
        {
            m_until = true;
        }

        //~DecoratorLoopUntil()
        //{
        //}

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Until")
                {
                    if (p.value == "true")
                    {
                        this.m_until = true;
                    }
                    else if (p.value == "false")
                    {
                        this.m_until = false;
                    }
                }
            }
        }

        protected override BehaviorTask createTask()
        {
            DecoratorLoopUntilTask pTask = new DecoratorLoopUntilTask();

            return pTask;
        }

        protected bool m_until;

        ///Returns EBTStatus.BT_RUNNING until the child returns EBTStatus.BT_SUCCESS. if the child returns EBTStatus.BT_FAILURE, it still returns EBTStatus.BT_RUNNING
        /**
        however, if m_until is false, the checking condition is inverted.
        i.e. it Returns EBTStatus.BT_RUNNING until the child returns EBTStatus.BT_FAILURE. if the child returns EBTStatus.BT_SUCCESS, it still returns EBTStatus.BT_RUNNING
        */

        private class DecoratorLoopUntilTask : DecoratorCountTask
        {
            public DecoratorLoopUntilTask()
            : base()
            {
            }

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
                if (this.m_n > 0)
                {
                    this.m_n--;
                }

                if (this.m_n == 0)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                Debug.Check(this.GetNode() is DecoratorLoopUntil);
                DecoratorLoopUntil pDecoratorLoopUntil = (DecoratorLoopUntil)(this.GetNode());

                if (pDecoratorLoopUntil.m_until)
                {
                    if (status == EBTStatus.BT_SUCCESS)
                    {
                        return EBTStatus.BT_SUCCESS;
                    }
                }
                else
                {
                    if (status == EBTStatus.BT_FAILURE)
                    {
                        return EBTStatus.BT_FAILURE;
                    }
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}
