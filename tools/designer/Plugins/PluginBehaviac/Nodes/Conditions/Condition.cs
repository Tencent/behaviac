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
using System.Drawing;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Conditions:Leaf", NodeIcon.Condition)]
    public class Condition : Behaviac.Design.Nodes.Condition
    {
        public Condition()
        : base(Resources.Condition, Resources.ConditionDesc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "Condition";
            }
        }

        public override bool HasPrefixLabel
        {
            get
            {
                return false;
            }
        }

        private RightValueDef _opl;
        [DesignerRightValueEnum("OperandLeft", "OperandLeftDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.AttributesMethod, MethodType.Getter, "", "Opr")]
        public RightValueDef Opl
        {
            get
            {
                return _opl;
            }
            set
            {
                this._opl = value;
            }
        }

        private OperatorType _operator = OperatorType.Equal;
        [DesignerEnum("Operator", "OperatorDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, "ConditionOperaptor")]
        public OperatorType Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
            }
        }

        private RightValueDef _opr;
        [DesignerRightValueEnum("OperandRight", "OperandRightDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 2, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "")]
        public RightValueDef Opr
        {
            get
            {
                return _opr;
            }
            set
            {
                this._opr = value;
            }
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.ConditionUIPolicy();
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (this.Opl != null)
            {
                bReset |= this.Opl.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            if (this.Opr != null)
            {
                bReset |= this.Opr.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

            return bReset;
        }

        public override string Description
        {
            get
            {
                //not ideal, for the left node list
                if (_opl == null && _opr == null)
                {
                    return base.Description;
                }

                string str = base.Description;

                if (_opl != null)
                {
                    str += "\n" + _opl.GetExportValue();
                }

                str += "\n" + _operator.ToString();

                if (_opr != null)
                {
                    str += "\n" + _opr.GetExportValue();
                }

                return str;
            }
        }

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            //List<object> excludedOperatorsResult = new List<object>() { OperatorType.Assignment, OperatorType.In };
            List<object> excludedOperatorsResult = new List<object>()
            {
                OperatorType.Assignment
            };

            if (enumAttr != null && enumAttr.ExcludeTag == "ConditionOperaptor")
            {
                if (this.Opl != null)
                {
                    if (this.Opl.ValueType != typeof(bool))
                    {
                        //and and or are only valid for bool, so to exclude and and or when the type is not bool
                        object[] excludedOperators = new object[] { OperatorType.And, OperatorType.Or };
                        excludedOperatorsResult.AddRange(excludedOperators);
                    }
                    else if (this.Opl.ValueType == typeof(bool))
                    {
                        object[] excludedOperators = new object[] { OperatorType.Greater, OperatorType.GreaterEqual, OperatorType.Less, OperatorType.LessEqual };
                        excludedOperatorsResult.AddRange(excludedOperators);
                    }
                }
            }

            return excludedOperatorsResult.ToArray();
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            Condition prec = (Condition)newnode;

            prec._operator = _operator;

            if (_opl != null)
            {
                prec._opl = (RightValueDef)_opl.Clone();
            }

            if (_opr != null)
            {
                prec._opr = (RightValueDef)_opr.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (this.Opl == null || this.Opr == null || this.Opl.ToString() == "" || this.Opr.ToString() == "")
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.OperatandIsNotComplete));
            }
            else if (this.Opl.ValueType != typeof(bool) &&
                     (this.Operator == OperatorType.And || this.Operator == OperatorType.Or))
            {
                // And and Or are only valid for bool
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.AndOrOnlyValidForBool));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
