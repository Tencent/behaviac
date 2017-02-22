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

using System.Collections;
using System.Collections.Generic;

namespace behaviac
{
    public class DecoratorIterator : DecoratorNode
    {
#if BEHAVIAC_USE_HTN
        public override bool decompose(BehaviorNode node, PlannerTaskComplex seqTask, int depth, Planner planner)
        {
            DecoratorIterator pForEach = (DecoratorIterator)node;
            bool bOk = false;
            int childCount = pForEach.GetChildrenCount();
            Debug.Check(childCount == 1);
            BehaviorNode childNode = pForEach.GetChild(0);

            bool bGoOn = true;
            int count = 0;
            int index = 0;

            while (bGoOn)
            {
                int depth2 = planner.GetAgent().Variables.Depth;
                using(AgentState currentState = planner.GetAgent().Variables.Push(false))
                {
                    bGoOn = pForEach.IterateIt(planner.GetAgent(), index, ref count);

                    if (bGoOn)
                    {
                        planner.LogPlanForEachBegin(planner.GetAgent(), pForEach, index, count);
                        PlannerTask childTask = planner.decomposeNode(childNode, depth);
                        planner.LogPlanForEachEnd(planner.GetAgent(), pForEach, index, count, childTask != null ? "success" : "failure");

                        if (childTask != null)
                        {
                            Debug.Check(seqTask is PlannerTaskIterator);
                            PlannerTaskIterator pForEachTask = seqTask as PlannerTaskIterator;
                            pForEachTask.Index = index;

                            seqTask.AddChild(childTask);
                            bOk = true;
                            break;
                        }

                        index++;
                    }
                }

                Debug.Check(planner.GetAgent().Variables.Depth == depth2);
            }

            return bOk;
        }
#endif//

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "Opl")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opl = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
                else if (p.name == "Opr")
                {
                    int pParenthesis = p.value.IndexOf('(');

                    if (pParenthesis == -1)
                    {
                        this.m_opr = AgentMeta.ParseProperty(p.value);
                    }
                    else
                    {
                        this.m_opr = AgentMeta.ParseMethod(p.value);
                    }
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorIterator))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public bool IterateIt(Agent pAgent, int index, ref int count)
        {
            if (this.m_opl != null && this.m_opr != null)
            {
                count = this.m_opr.GetCount(pAgent);

                if (index >= 0 && index < count)
                {
                    this.m_opl.SetValue(pAgent, this.m_opr, index);

                    return true;
                }
            }
            else
            {
                Debug.Check(false);
            }

            return false;
        }

        protected override BehaviorTask createTask()
        {
            Debug.Check(false);
            return null;
        }

        protected IInstanceMember m_opl;
        protected IInstanceMember m_opr;
    }
}
