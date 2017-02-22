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
    public class WithPrecondition : BehaviorNode
    {
        public WithPrecondition()
        {
        }

        //~WithPrecondition()
        //{
        //}

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is WithPrecondition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            WithPreconditionTask pTask = new WithPreconditionTask();

            return pTask;
        }
    }

    internal class WithPreconditionTask : Sequence.SequenceTask
    {
        public WithPreconditionTask()
        : base()
        {
        }

        protected override void addChild(BehaviorTask pBehavior)
        {
            base.addChild(pBehavior);
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

        protected override bool onenter(Agent pAgent)
        {
            BehaviorTask pParent = this.GetParent();

            //when as child of SelctorLoop, it is not ticked normally
            Debug.Check(pParent is SelectorLoop.SelectorLoopTask);

            return true;
        }

        protected override void onexit(Agent pAgent, EBTStatus s)
        {
            BehaviorTask pParent = this.GetParent();

            Debug.Check(pParent is SelectorLoop.SelectorLoopTask);
        }

        public BehaviorTask PreconditionNode
        {
            get
            {
                Debug.Check(this.m_children.Count == 2);

                return (this.m_children)[0];
            }
        }

        public BehaviorTask ActionNode
        {
            get
            {
                Debug.Check(this.m_children.Count == 2);

                return (this.m_children)[1];
            }
        }

        protected override EBTStatus update_current(Agent pAgent, EBTStatus childStatus)
        {
            return this.update(pAgent, childStatus);
        }

        protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
        {
            BehaviorTask pParent = this.GetParent();
            Debug.Check(pParent is SelectorLoop.SelectorLoopTask);

            Debug.Check(false);

            return EBTStatus.BT_RUNNING;
        }
    }
}
