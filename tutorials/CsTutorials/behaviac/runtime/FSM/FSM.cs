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
    public class FSM : BehaviorNode
    {
#if BEHAVIAC_USE_HTN
        public override bool decompose(BehaviorNode node, PlannerTaskComplex seqTask, int depth, Planner planner)
        {
            Debug.Check(false);

            return false;
        }
#endif//

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "initialid")
                {
                    this.m_initialid = System.Convert.ToInt32(p.value);
                }
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is FSM))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        private int m_initialid = -1;
        public int InitialId
        {
            get
            {
                return this.m_initialid;
            }
            set
            {
                this.m_initialid = value;
            }
        }

        protected override BehaviorTask createTask()
        {
            return new FSMTask();
        }

        public class FSMTask : CompositeTask
        {
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

            //used for FSM
            private int m_currentNodeId = -1;

            protected override bool onenter(Agent pAgent)
            {
                Debug.Check(this.m_node != null);
                FSM fsm = (FSM)this.m_node;

                this.m_activeChildIndex = 0;

                this.m_currentNodeId = fsm.InitialId;

                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
                this.m_currentNodeId = -1;

                base.onexit(pAgent, s);
            }

            private EBTStatus UpdateFSM(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.m_node != null);
                Debug.Check(this.m_currentNodeId != -1);

#if !BEHAVIAC_RELEASE
                int kMaxCount = 10;
                Dictionary<int, int> state_update_count = new Dictionary<int, int>();
#endif//#if !BEHAVIAC_RELEASE

                EBTStatus status = childStatus;
                bool bLoop = true;

                while (bLoop)
                {
                    int nextStateId = -1;
                    BehaviorTask currentState = this.GetChildById(this.m_currentNodeId);

                    if (currentState != null)
                    {
                        currentState.exec(pAgent);

                        if (currentState is State.StateTask)
                        {
                            State.StateTask pStateTask = (State.StateTask)currentState;

                            if (pStateTask != null && pStateTask.IsEndState)
                            {
                                return EBTStatus.BT_SUCCESS;
                            }
                        }

                        nextStateId = currentState.GetNextStateId();
                    }

                    if (nextStateId < 0)
                    {
                        //if not transitioned, don't go on next state, to exit
                        bLoop = false;
                    }
                    else
                    {
#if !BEHAVIAC_RELEASE

                        if (state_update_count.ContainsKey(this.m_currentNodeId))
                        {
                            state_update_count[this.m_currentNodeId]++;
                        }
                        else
                        {
                            state_update_count.Add(this.m_currentNodeId, 1);
                        }

                        if (state_update_count[this.m_currentNodeId] > kMaxCount)
                        {
                            string treeName = BehaviorTask.GetParentTreeName(pAgent, this.GetNode());
                            Debug.LogError(string.Format("{0} might be updating an FSM('{1}') endlessly, possibly a dead loop, please redesign it!\n", pAgent.GetName(), treeName));
                            Debug.Check(false);
                        }

#endif

                        //if transitioned, go on next state
                        this.m_currentNodeId = nextStateId;
                    }
                }

                return status;
            }

            protected override EBTStatus update_current(Agent pAgent, EBTStatus childStatus)
            {
                EBTStatus status = this.update(pAgent, childStatus);

                return status;
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Debug.Check(this.m_activeChildIndex < this.m_children.Count);
                Debug.Check(this.m_node is FSM);

                EBTStatus s = this.UpdateFSM(pAgent, childStatus);

                return s;
            }
        }
    }
}
