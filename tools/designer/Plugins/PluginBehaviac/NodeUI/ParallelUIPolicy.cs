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
using System.Text;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;

namespace Behaviac.Design.ObjectUI
{
    class ParallelUIPolicy : ObjectUIPolicy
    {
        public override void Update(object sender, DesignerPropertyInfo property)
        {
            if (_obj != null)
            {
                BaseNode p = _obj as BaseNode;

                if (p != null)
                {
                    bool bHTN = false;

                    while (p != null)
                    {
                        if (p.Parent != null && p.Parent.ToString() == "Branch")
                        {
                            bHTN = true;
                            break;
                        }

                        p = p.Parent;
                    }

                    //hide policy configs if for HTN
                    {
                        DesignerPropertyEditor FailurePolicyEditor = GetEditor(_obj, "FailurePolicy");
                        Debug.Check(FailurePolicyEditor != null);
                        if (FailurePolicyEditor != null)
                        {
                            FailurePolicyEditor.Visible = !bHTN;
                        }

                        DesignerPropertyEditor SuccessPolicyEditor = GetEditor(_obj, "SuccessPolicy");
                        Debug.Check(SuccessPolicyEditor != null);
                        if (SuccessPolicyEditor != null)
                        {
                            SuccessPolicyEditor.Visible = !bHTN;
                        }

                        DesignerPropertyEditor ExitPolicyEditor = GetEditor(_obj, "ExitPolicy");
                        Debug.Check(ExitPolicyEditor != null);
                        if (ExitPolicyEditor != null)
                        {
                            ExitPolicyEditor.Visible = !bHTN;
                        }

                        DesignerPropertyEditor ChildFinishPolicyEditor = GetEditor(_obj, "ChildFinishPolicy");
                        Debug.Check(ChildFinishPolicyEditor != null);
                        if (ChildFinishPolicyEditor != null)
                        {
                            ChildFinishPolicyEditor.Visible = !bHTN;
                        }
                    }
                }

            }
        }
    }
}
