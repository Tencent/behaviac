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
    class AssignmentUIPolicy : ObjectUIPolicy
    {
        ValueTypes _valueTypesBackup = ValueTypes.All;

        public override void Update(object sender, DesignerPropertyInfo property)
        {
            if (_obj != null)
            {
                DesignerPropertyEditor oplEditor = GetEditor(_obj, "Opl");
                Debug.Check(oplEditor != null);

                if (oplEditor == sender)
                {
                    VariableDef opl = (VariableDef)GetProperty(_obj, "Opl");

                    if (opl != null)
                    {
                        RightValueDef opr = (RightValueDef)GetProperty(_obj, "Opr");

                        if (opr != null)
                        {
                            if (opl.ValueType != opr.ValueType)
                            {
                                DesignerPropertyEditor oprEditor = GetEditor(_obj, "Opr");
                                Debug.Check(oprEditor != null);
                                if (oprEditor != null)
                                {
                                    oprEditor.Clear();
                                }
                            }
                        }
                    }
                }
                else
                {
                    DesignerPropertyEditor oplEditorCast = GetEditor(_obj, "CastRight");
                    Debug.Check(oplEditorCast != null);

                    if (oplEditorCast == sender)
                    {
                        PluginBehaviac.Nodes.Assignment assignNode = _obj as PluginBehaviac.Nodes.Assignment;

                        if (assignNode != null)
                        {
                            RightValueDef opr = (RightValueDef)GetProperty(_obj, "Opr");

                            if (opr != null)
                            {
                                DesignerPropertyEditor oprEditor = GetEditor(_obj, "Opr");
                                Debug.Check(oprEditor != null);

                                // oprEditor.ValueType might be overwritten in SetupCastSettings if casting
                                // so that here to backup it first so that it can be restored later if not casting
                                if (assignNode.IsCasting)
                                {
                                    _valueTypesBackup = oprEditor.ValueType;
                                }
                                else
                                {
                                    oprEditor.ValueType = _valueTypesBackup;
                                }

                                oprEditor.Clear();
                                oprEditor.FilterType = null;
                            }
                        }
                    }
                }
            }
        }
    }
}
