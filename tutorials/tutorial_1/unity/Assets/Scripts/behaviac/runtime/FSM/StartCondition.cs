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

using System;
using System.Collections.Generic;

namespace behaviac
{
    public class StartCondition : Precondition
    {
        protected List<Effector.EffectorConfig> m_effectors = new List<Effector.EffectorConfig>();
        protected int m_targetId = -1;

        public int TargetStateId
        {
            get
            {
                return this.m_targetId;
            }
            set
            {
                this.m_targetId = value;
            }
        }

        public override void ApplyEffects(Agent pAgent, Effector.EPhase phase)
        {
            for (int i = 0; i < this.m_effectors.Count; ++i)
            {
                Effector.EffectorConfig effector = this.m_effectors[i];

                effector.Execute(pAgent);
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is StartCondition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public int TargetNodeId()
        {
            return this.m_targetId;
        }

        protected override BehaviorTask createTask()
        {
            Debug.Check(false);
            return null;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            if (this.m_loadAttachment)
            {
                Effector.EffectorConfig effectorConfig = new Effector.EffectorConfig();

                if (effectorConfig.load(properties))
                {
                    this.m_effectors.Add(effectorConfig);
                }

                return;
            }

            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "TargetFSMNodeId")
                {
                    this.m_targetId = Convert.ToInt32(p.value);
                }
                else
                {
                    //Debug.Check(0, "unrecognised property %s", p.name);
                }
            }
        }
    }
}
