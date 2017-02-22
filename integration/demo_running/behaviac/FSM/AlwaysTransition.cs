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
using System;
using System.Collections.Generic;

namespace behaviac
{
    class AlwaysTransition : Transition
    {
        public enum ETransitionPhase
        {
            ETP_Always,
            ETP_Success,
            ETP_Failure,
            ETP_Exit,
        }

        public AlwaysTransition()
        {
        }

        //~AlwaysTransition()
        //{
        //}

        protected ETransitionPhase m_transitionPhase = ETransitionPhase.ETP_Always;

        public ETransitionPhase TransitionPhase
        {
            get
            {
                return this.m_transitionPhase;
            }
            set
            {
                this.m_transitionPhase = value;
            }
        }


        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "TransitionPhase")
                {
                    if (p.value == "ETP_Exit")
                    {
                        this.m_transitionPhase = ETransitionPhase.ETP_Exit;
                    }
                    else if (p.value == "ETP_Success")
                    {
                        this.m_transitionPhase = ETransitionPhase.ETP_Success;
                    }
                    else if (p.value == "ETP_Failure")
                    {
                        this.m_transitionPhase = ETransitionPhase.ETP_Failure;
                    }
                    else if (p.value == "ETP_Always")
                    {
                        this.m_transitionPhase = ETransitionPhase.ETP_Always;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
            }

        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is AlwaysTransition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        protected override BehaviorTask createTask()
        {
            //return new StartConditionTask();
            Debug.Check(false);
            return null;
        }

        public override bool Evaluate(Agent pAgent, EBTStatus status)
        {
            if (this.m_transitionPhase == ETransitionPhase.ETP_Always)
            {
                return true;
            }
            else if (status == EBTStatus.BT_SUCCESS && (this.m_transitionPhase == ETransitionPhase.ETP_Success || this.m_transitionPhase == ETransitionPhase.ETP_Exit))
            {
                return true;
            }
            else if (status == EBTStatus.BT_FAILURE && (this.m_transitionPhase == ETransitionPhase.ETP_Failure || this.m_transitionPhase == ETransitionPhase.ETP_Exit))
            {
                return true;
            }

            return false;
        }
    }

}
