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

namespace behaviac
{
    public abstract class ConditionBase : BehaviorNode
    {
        public ConditionBase()
        {
        }

        //~ConditionBase()
        //{
        //}

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is ConditionBase))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }
    }

    // ============================================================================
    internal class ConditionBaseTask : LeafTask
    {
        public ConditionBaseTask()
        {
        }

        //~ConditionBaseTask()
        //{
        //}

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

            return EBTStatus.BT_SUCCESS;
        }
    }
}
