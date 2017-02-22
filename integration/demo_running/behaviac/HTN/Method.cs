/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions and limitations under
// the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace behaviac
{
    public class Method : BehaviorNode
    {
        public Method()
        {
        }

        //~Method()
        //{
        //}

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Method))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            Debug.Check(false);
            return null;
        }

#if BEHAVIAC_USE_HTN
        public override bool decompose(BehaviorNode branch, PlannerTaskComplex seqTask, int depth, Planner planner)
        {
            bool bOk = false;
            int childCount = branch.GetChildrenCount();
            Debug.Check(childCount == 1);
            BehaviorNode childNode = branch.GetChild(0);
            PlannerTask childTask = planner.decomposeNode(childNode, depth);

            if (childTask != null)
            {
                seqTask.AddChild(childTask);
                bOk = true;
            }

            return bOk;
        }
#endif//

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
        }
    }
}
