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
    [NodeDesc("Actions", "compute_ico")]
    public class Compute : Behaviac.Design.Nodes.Node
    {
        public Compute()
        : base(Resources.Compute, Resources.ComputeDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/compute/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "Compute";
            }
        }

        public override bool HasPrefixLabel
        {
            get
            {
                return false;
            }
        }

        public override bool HasFirstLabel
        {
            get
            {
                return true;
            }
        }

        private VariableDef _opl;
        [DesignerPropertyEnum("OperandLeft", "OperandLeftDesc", "Compute", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags | DesignerProperty.DesignerFlags.NoReadonly, DesignerPropertyEnum.AllowStyles.Attributes, "", "Opr1", ValueTypes.Int | ValueTypes.Float)]
        public VariableDef Opl
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

        private RightValueDef _opr1;
        [DesignerRightValueEnum("Operand1", "OperandDesc1", "Compute", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "Opr2", ValueTypes.Int | ValueTypes.Float)]
        public RightValueDef Opr1
        {
            get
            {
                if (_opl != null && _opr1 != null)
                {
                    _opr1.NativeType = _opl.NativeType;
                }

                return _opr1;
            }

            set
            {
                this._opr1 = value;
            }
        }

        private Behaviac.Design.ComputeOperator _operator = Behaviac.Design.ComputeOperator.Add;
        [DesignerEnum("Operator", "OperatorDesc", "Compute", DesignerProperty.DisplayMode.Parameter, 2, DesignerProperty.DesignerFlags.NoFlags, "ComputeOperaptor")]
        public Behaviac.Design.ComputeOperator Operator
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

        private RightValueDef _opr2;
        [DesignerRightValueEnum("Operand2", "OperandDesc2", "Compute", DesignerProperty.DisplayMode.Parameter, 3, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "", ValueTypes.Int | ValueTypes.Float)]
        public RightValueDef Opr2
        {
            get
            {
                if (_opl != null && _opr2 != null)
                {
                    _opr2.NativeType = _opl.NativeType;
                }

                return _opr2;
            }

            set
            {
                this._opr2 = value;
            }
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (this.Opl != null)
            {
                bReset |= this.Opl.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            if (this.Opr1 != null)
            {
                bReset |= this.Opr1.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            if (this.Opr2 != null)
            {
                bReset |= this.Opr2.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

            return bReset;
        }

        public override string Description
        {
            get
            {
                //not ideal, for the left node list
                if (_opl == null && _opr1 == null)
                {
                    return base.Description;
                }

                string str = base.Description;

                if (_opl != null)
                {
                    str += "\n" + _opl.GetExportValue();
                }

                str += "\n" + "=";

                if (_opr1 != null)
                {
                    str += "\n" + _opr1.GetExportValue();
                }

                str += "\n" + _operator.ToString();

                if (_opr2 != null)
                {
                    str += "\n" + _opr2.GetExportValue();
                }

                return str;
            }
        }

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            if (enumAttr != null && enumAttr.ExcludeTag == "ComputeOperaptor")
            {
                if (this.Opl != null)
                {
                    if (string.IsNullOrEmpty(this.Opl.NativeType))
                    {
                        return null;
                    }

                    bool bIsBool = false;
                    bool bIsNumber = false;

                    Type type = this.Opl.ValueType;

                    if (type != null)
                    {
                        bIsBool = Plugin.IsBooleanType(type);
                        bIsNumber = (Plugin.IsIntergerType(type) || Plugin.IsFloatType(type));
                    }

                    if (bIsBool || !bIsNumber)
                    {
                        //+/-/*// are not valid for bool
                        object[] excludedOperators = new object[] { ComputeOperator.Add, ComputeOperator.Sub, ComputeOperator.Mul, ComputeOperator.Div };

                        return excludedOperators;
                    }
                    else
                    {
                        //object[] excludedOperators = new object[] { ComputeOperator.Invalid };

                        //return excludedOperators;
                    }
                }
            }

            return null;
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.ComputeUIPolicy();
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            Compute prec = (Compute)newnode;

            //prec._negate = _negate;
            if (_opl != null)
            {
                prec._opl = (VariableDef)_opl.Clone();
            }

            if (_opr1 != null)
            {
                prec._opr1 = (RightValueDef)_opr1.Clone();
            }

            prec._operator = _operator;

            if (_opr2 != null)
            {
                prec._opr2 = (RightValueDef)_opr2.Clone();
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(157, 75, 39));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        public override NodeViewData CreateNodeViewData(NodeViewData parent, Behaviac.Design.Nodes.BehaviorNode rootBehavior)
        {
            NodeViewData nvd = base.CreateNodeViewData(parent, rootBehavior);
            nvd.ChangeShape(NodeShape.Rectangle);

            return nvd;
        }
        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (this.Opl == null ||
                this.Opr1 == null || this.Opl.ToString() == "" || this.Opr1.ToString() == "" ||
                this.Opr2 == null || this.Opl.ToString() == "" || this.Opr2.ToString() == "")
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Operand is not complete!"));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
