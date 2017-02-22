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
    class ActionUIPolicy : ObjectUIPolicy
    {
        public override void Update(object sender, DesignerPropertyInfo property)
        {
            if (_obj != null)
            {
                DesignerPropertyEditor resultOptionEditor = GetEditor(_obj, "ResultOption");
                DesignerPropertyEditor resultFunctorEditor = GetEditor(_obj, "ResultFunctor");
                Debug.Check(resultOptionEditor != null && resultFunctorEditor != null);
                if (resultOptionEditor != null && resultFunctorEditor != null)
                {
                    MethodDef method = GetProperty(_obj, "Method") as MethodDef;
                    MethodDef checkMethod = GetProperty(_obj, "ResultFunctor") as MethodDef;

                    if (method == null || method.NativeReturnType == "behaviac::EBTStatus")
                    {
                        resultOptionEditor.Enabled = false;
                        resultFunctorEditor.Enabled = false;

                        //ResultOption is set to be SUCCESS by default
                        SetProperty(_obj, "ResultOption", EBTStatus.BT_INVALID);

                    }
                    else
                    {
                        bool enableMethod = true;
                        object prop = GetProperty(_obj, "ResultOption");

                        if (prop is EBTStatus)
                        {
                            EBTStatus checkStatusdProp = (EBTStatus)prop;

                            if (EBTStatus.BT_INVALID != checkStatusdProp)
                            {
                                enableMethod = false;
                            }
                        }

                        resultOptionEditor.Enabled = true;
                        resultFunctorEditor.Enabled = enableMethod;
                    }

                    if (!resultFunctorEditor.Enabled)
                    {
                        SetProperty(_obj, "ResultFunctor", null);
                        resultFunctorEditor.Clear();
                    }
                }
            }
        }
    }
}
