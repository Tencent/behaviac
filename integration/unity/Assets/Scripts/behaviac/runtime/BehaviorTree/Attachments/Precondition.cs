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
    public class Precondition : AttachAction
    {
        public enum EPhase
        {
            E_ENTER,
            E_UPDATE,
            E_BOTH
        }

        public class PreconditionConfig : ActionConfig
        {
            public EPhase m_phase = EPhase.E_ENTER;
            public bool m_bAnd = false;

            public override bool load(List<property_t> properties)
            {
                bool loaded = base.load(properties);

                try
                {
                    for (int i = 0; i < properties.Count; ++i)
                    {
                        property_t p = properties[i];

                        if (p.name == "BinaryOperator")
                        {
                            if (p.value == "Or")
                            {
                                this.m_bAnd = false;
                            }
                            else if (p.value == "And")
                            {
                                this.m_bAnd = true;
                            }
                            else
                            {
                                Debug.Check(false);
                            }
                        }
                        else if (p.name == "Phase")
                        {
                            if (p.value == "Enter")
                            {
                                this.m_phase = EPhase.E_ENTER;
                            }
                            else if (p.value == "Update")
                            {
                                this.m_phase = EPhase.E_UPDATE;
                            }
                            else if (p.value == "Both")
                            {
                                this.m_phase = EPhase.E_BOTH;
                            }
                            else
                            {
                                Debug.Check(false);
                            }

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Check(false, ex.Message);
                    loaded = false;
                }

                return loaded;
            }
        }

        public Precondition()
        {
            m_ActionConfig = new PreconditionConfig();
        }

        public EPhase Phase
        {
            get
            {
                return ((PreconditionConfig)(this.m_ActionConfig)).m_phase;
            }
            set
            {
                ((PreconditionConfig)(this.m_ActionConfig)).m_phase = value;
            }
        }

        public bool IsAnd
        {
            get
            {
                return ((PreconditionConfig)(this.m_ActionConfig)).m_bAnd;
            }
            set
            {
                ((PreconditionConfig)(this.m_ActionConfig)).m_bAnd = value;
            }
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is Precondition))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }
    }
}
