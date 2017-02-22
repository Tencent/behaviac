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
    public class IfElse : BehaviorNode
    {
        public IfElse()
        {
        }

        //~IfElse()
        //{
        //}

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is IfElse))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            IfElseTask pTask = new IfElseTask();

            return pTask;
        }

        /**
        this node has three children: 'condition' branch, 'if' branch, 'else' branch

        first, it executes conditon, until it returns success or failure.
        if it returns success, it then executes 'if' branch,
        else if it returns failure, it then executes 'else' branch.
        */

        private class IfElseTask : CompositeTask
        {
            public IfElseTask()
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

            protected override bool onenter(Agent pAgent)
            {
                //reset it as it will be checked for the condition execution at the first time
                this.m_activeChildIndex = CompositeTask.InvalidChildIndex;

                if (this.m_children.Count == 3)
                {
                    return true;
                }

                Debug.Check(false, "IfElseTask has to have three children: condition, if, else");

                return false;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                base.onexit(pAgent, s);
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.m_children.Count == 3);

                if (childStatus != EBTStatus.BT_RUNNING)
                {
                    Debug.Check(this.m_activeChildIndex != CompositeTask.InvalidChildIndex);

                    return childStatus;
                }

                if (this.m_activeChildIndex == CompositeTask.InvalidChildIndex)
                {
                    BehaviorTask pCondition = this.m_children[0];

                    EBTStatus conditionResult = pCondition.exec(pAgent);

                    //Debug.Check (conditionResult == EBTStatus.BT_SUCCESS || conditionResult == EBTStatus.BT_FAILURE,
                    //	"conditionResult should be either EBTStatus.BT_SUCCESS of EBTStatus.BT_FAILURE");

                    if (conditionResult == EBTStatus.BT_SUCCESS)
                    {
                        //BehaviorTask pIf = this.m_children[1];

                        this.m_activeChildIndex = 1;
                    }
                    else if (conditionResult == EBTStatus.BT_FAILURE)
                    {
                        //BehaviorTask pElse = this.m_children[2];

                        this.m_activeChildIndex = 2;
                    }
                }

                if (this.m_activeChildIndex != CompositeTask.InvalidChildIndex)
                {
                    BehaviorTask pBehavior = this.m_children[this.m_activeChildIndex];
                    EBTStatus s = pBehavior.exec(pAgent);

                    return s;
                }

                return EBTStatus.BT_RUNNING;
            }
        }
    }
}
