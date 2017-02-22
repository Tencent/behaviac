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
    public class DecoratorCountLimit : DecoratorCount
    {
        public DecoratorCountLimit()
        {
        }

        //~DecoratorCountLimit()
        //{
        //}

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorCountLimit))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            DecoratorCountLimitTask pTask = new DecoratorCountLimitTask();

            return pTask;
        }

        public bool CheckIfReInit(Agent pAgent)
        {
            bool bTriggered = this.EvaluteCustomCondition(pAgent);

            return bTriggered;
        }

        ///enter and tick the child for the specified number of iterations, then it will not enter and tick the child after that
        private class DecoratorCountLimitTask : DecoratorCountTask
        {
            public DecoratorCountLimitTask()
            {
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);

                Debug.Check(target is DecoratorCountLimitTask);
                DecoratorCountLimitTask ttask = (DecoratorCountLimitTask)target;

                ttask.m_bInited = this.m_bInited;
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);

                CSerializationID initId = new CSerializationID("inited");
                node.setAttr(initId, this.m_bInited);
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            protected override bool onenter(Agent pAgent)
            {
                DecoratorCountLimit node = this.m_node as DecoratorCountLimit;

                if (node.CheckIfReInit(pAgent))
                {
                    this.m_bInited = false;
                }

                if (!this.m_bInited)
                {
                    this.m_bInited = true;

                    int count = this.GetCount(pAgent);

                    this.m_n = count;
                }

                //if this.m_n is -1, it is endless
                if (this.m_n > 0)
                {
                    this.m_n--;
                    return true;
                }
                else if (this.m_n == 0)
                {
                    return false;
                }
                else if (this.m_n == -1)
                {
                    return true;
                }

                Debug.Check(false);

                return false;
            }

            protected override EBTStatus decorate(EBTStatus status)
            {
                Debug.Check(this.m_n >= 0 || this.m_n == -1);

                return status;
            }

            private bool m_bInited;
        }
    }
}
