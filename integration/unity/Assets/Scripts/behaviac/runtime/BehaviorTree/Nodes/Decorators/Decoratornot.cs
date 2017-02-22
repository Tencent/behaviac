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
    public class DecoratorNot : DecoratorNode
    {
        public DecoratorNot()
        {
        }

        //~DecoratorNot()
        //{
        //}

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorNot))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public override bool Evaluate(Agent pAgent)
        {
            Debug.Check(this.m_children.Count == 1);
            bool ret = this.m_children[0].Evaluate(pAgent);
            return !ret;
        }

        protected override BehaviorTask createTask()
        {
            DecoratorNotTask pTask = new DecoratorNotTask();

            return pTask;
        }

        private class DecoratorNotTask : DecoratorTask
        {
            public DecoratorNotTask()
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
                if (status == EBTStatus.BT_FAILURE)
                {
                    return EBTStatus.BT_SUCCESS;
                }

                if (status == EBTStatus.BT_SUCCESS)
                {
                    return EBTStatus.BT_FAILURE;
                }

                return status;
            }
        }
    }
}
