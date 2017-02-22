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
using Behaviac.Design.Attachments;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;

namespace Behaviac.Design.ObjectUI
{
    public class AttachActionUIPolicy : ObjectUIPolicy
    {
        private bool isAction()
        {
            RightValueDef opl = (RightValueDef)GetProperty(_obj, "Opl");

            return (opl != null && opl.IsMethod && opl.Method != null && opl.Method.ReturnType == typeof(void));
        }

        public override bool ShouldAddProperty(DesignerPropertyInfo property)
        {
            if (_obj != null)
            {
                OperatorTypes operatorType = (OperatorTypes)GetProperty(_obj, "Operator");

                DesignerPropertyInfo oplProp = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Opl");
                DesignerPropertyInfo opr1Prop = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Opr1");
                DesignerPropertyInfo operatorProp = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Operator");
                DesignerPropertyInfo opr2Prop = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Opr2");

                // action
                if (this.isAction())
                {
                    return property.Property != opr1Prop.Property &&
                           property.Property != operatorProp.Property &&
                           property.Property != opr2Prop.Property;
                }

                // assign
                else if (operatorType == OperatorTypes.Assign)
                {
                    return property.Property != opr1Prop.Property;
                }

                // compute
                else if (operatorType >= OperatorTypes.Add && operatorType <= OperatorTypes.Div)
                {
                }

                // compare
                else if (operatorType >= OperatorTypes.Equal && operatorType <= OperatorTypes.LessEqual)
                {
                    return property.Property != opr1Prop.Property;
                }
            }

            return true;
        }

        public override bool ShouldUpdatePropertyGrids(DesignerPropertyInfo property)
        {
            DesignerPropertyInfo oplProp = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Opl");
            DesignerPropertyInfo operatorProp = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Operator");

            return property.Property == oplProp.Property ||
                   property.Property == operatorProp.Property;
        }

        public override string GetLabel(DesignerPropertyInfo property)
        {
            OperatorTypes operatorType = (OperatorTypes)GetProperty(_obj, "Operator");

            // action
            if (this.isAction())
            {
                DesignerPropertyInfo oplProp = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Opl");

                if (property.Property == oplProp.Property)
                {
                    return Resources.Method;
                }
            }

            // assign
            else if (operatorType == OperatorTypes.Assign)
            {
                DesignerPropertyInfo opr2Prop = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Opr2");

                if (property.Property == opr2Prop.Property)
                {
                    return Resources.Right;
                }
            }

            // compute
            else if (operatorType >= OperatorTypes.Add && operatorType <= OperatorTypes.Div)
            {
            }

            // compare
            else if (operatorType >= OperatorTypes.Equal && operatorType <= OperatorTypes.LessEqual)
            {
                DesignerPropertyInfo opr2Prop = DesignerProperty.GetDesignerProperty(_obj.GetType(), "Opr2");

                if (property.Property == opr2Prop.Property)
                {
                    return Resources.Right;
                }
            }

            return base.GetLabel(property);
        }
    }
}
