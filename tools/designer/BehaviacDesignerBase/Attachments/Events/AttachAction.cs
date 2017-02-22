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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;
using System.Reflection;

namespace Behaviac.Design.Attachments
{
    public class AttachAction : Behaviac.Design.Attachments.Attach
    {
        public AttachAction()
        : base(null, "AttachAction", "AttachAction")
        {
        }

        public AttachAction(Node node, string label, string desc)
        : base(node, label, desc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "AttachAction";
            }
        }

        protected RightValueDef _opl;
        [DesignerRightValueEnum("OperandLeft", "OperandLeftDesc", "Operation", DesignerProperty.DisplayMode.Parameter, 2, DesignerProperty.DesignerFlags.NoFlags | DesignerProperty.DesignerFlags.NoReadonly, DesignerPropertyEnum.AllowStyles.AttributesMethod, MethodType.Method, "", "Opr2", ValueTypes.All)]
        public virtual RightValueDef Opl
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
        [DesignerRightValueEnum("Operand1", "OperandDesc1", "Operation", DesignerProperty.DisplayMode.Parameter, 3, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "Opr2", ValueTypes.All)]
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

        protected OperatorTypes _operator = OperatorTypes.Equal;
        [DesignerEnum("Operator", "OperatorDesc", "Operation", DesignerProperty.DisplayMode.Parameter, 4, DesignerProperty.DesignerFlags.NoFlags, "AttachActionOperaptor")]
        public OperatorTypes Operator
        {
            get
            {
                if (this.IsAction())
                {
                    return OperatorTypes.Invalid;
                }

                return _operator;
            }
            set
            {
                _operator = value;
            }
        }

        private RightValueDef _opr2;
        [DesignerRightValueEnum("Operand2", "OperandDesc2", "Operation", DesignerProperty.DisplayMode.Parameter, 5, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "", ValueTypes.All)]
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

        public override NodeViewData.SubItemAttachment CreateSubItem()
        {
            return new NodeViewData.SubItemAttachAction(this);
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.AttachActionUIPolicy();
        }

        public override string Description
        {
            get
            {
                string str = base.Description;

                if (_opl != null)
                {
                    str += "\n" + _opl.GetExportValue();
                }

                if (!this.IsAction())
                {
                    if (this.IsCompute())
                    {
                        if (_opr1 != null)
                        {
                            str += "\n" + _opr1.GetExportValue();
                        }
                    }

                    str += "\n" + this.Operator.ToString();

                    if (_opr2 != null)
                    {
                        str += "\n" + _opr2.GetExportValue();
                    }
                }

                return str;
            }
        }

        public virtual bool IsAction()
        {
            return this._opl != null && this._opl.IsMethod && this._opl.Method != null && this._opl.Method.ReturnType == typeof(void);
        }

        public bool IsAssign()
        {
            return this.Operator == OperatorTypes.Assign;
        }

        public bool IsCompute()
        {
            return this.Operator >= OperatorTypes.Add && this.Operator <= OperatorTypes.Div;
        }

        public bool IsCompare()
        {
            return this.Operator >= OperatorTypes.Equal && this.Operator <= OperatorTypes.LessEqual;
        }

        protected override string GeneratePropertiesLabel()
        {
            string str = string.Empty;

            // action
            if (this.IsAction())
            {
                str = this._opl.Method.GetDisplayValue();
            }

            // assign
            else if (this.IsAssign())
            {
                if (_opl != null)
                {
                    str += _opl.GetDisplayValue();
                }

                str += " = ";

                if (_opr2 != null)
                {
                    str += _opr2.GetDisplayValue();
                }
            }

            // compute or compare
            else
            {
                bool isCompute = this.IsCompute();
                bool isCompare = this.IsCompare();

                if (isCompute || isCompare)
                {
                    if (_opl != null)
                    {
                        str += _opl.GetDisplayValue();
                    }

                    if (isCompute)
                    {
                        str += " = ";

                        if (_opr1 != null)
                        {
                            str += _opr1.GetDisplayValue();
                        }
                    }

                    string opr = "";
                    System.Reflection.FieldInfo fi = this.Operator.GetType().GetField(this.Operator.ToString());
                    Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(EnumMemberDescAttribute), false);

                    if (attributes.Length > 0)
                    {
                        opr = ((EnumMemberDescAttribute)attributes[0]).DisplayName;
                    }

                    str += " " + opr + " ";

                    if (_opr2 != null)
                    {
                        str += _opr2.GetDisplayValue();
                    }
                }
            }

            return str;
        }

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            ArrayList enums = new ArrayList();
            enums.Add(OperatorTypes.Invalid);

            if (this.Node != null && this.Node.IsFSM)
            {
                enums.Add(OperatorTypes.Equal);
                enums.Add(OperatorTypes.NotEqual);
                enums.Add(OperatorTypes.Greater);
                enums.Add(OperatorTypes.Less);
                enums.Add(OperatorTypes.GreaterEqual);
                enums.Add(OperatorTypes.LessEqual);
            }

            if (enumAttr != null && enumAttr.ExcludeTag == "AttachActionOperaptor")
            {
                if (this.Opl != null && !string.IsNullOrEmpty(this.Opl.NativeType))
                {
                    bool isPropReadonly = false;

                    if (this.Opl.Var != null && this.Opl.Var.IsProperty && this.Opl.Var.Property != null)
                    {
                        isPropReadonly = this.Opl.Var.Property.IsReadonly;
                    }

                    bool bIsBool = false;
                    bool bIsNumber = false;

                    Type type = Plugin.GetTypeFromName(Plugin.GetNativeTypeName(this.Opl.NativeType));

                    if (type != null)
                    {
                        bIsBool = Plugin.IsBooleanType(type);
                        bIsNumber = (Plugin.IsIntergerType(type) || Plugin.IsFloatType(type));
                    }

                    if (bIsBool || !bIsNumber)
                    {
                        enums.Add(OperatorTypes.Add);
                        enums.Add(OperatorTypes.Sub);
                        enums.Add(OperatorTypes.Mul);
                        enums.Add(OperatorTypes.Div);

                        enums.Add(OperatorTypes.Greater);
                        enums.Add(OperatorTypes.Less);
                        enums.Add(OperatorTypes.GreaterEqual);
                        enums.Add(OperatorTypes.LessEqual);
                    }

                    if (isPropReadonly || this.Opl.IsMethod && this.Opl.Method != null)
                    {
                        enums.Add(OperatorTypes.Assign);

                        if (!enums.Contains(OperatorTypes.Add))
                        {
                            enums.Add(OperatorTypes.Add);
                            enums.Add(OperatorTypes.Sub);
                            enums.Add(OperatorTypes.Mul);
                            enums.Add(OperatorTypes.Div);
                        }
                    }
                }
            }

            return enums.ToArray();
        }

        public override IList<DesignerPropertyInfo> GetDesignerProperties(bool bCustom)
        {
            if (!bCustom)
            {
                return base.GetDesignerProperties(bCustom);
            }

            List<DesignerPropertyInfo> result = new List<DesignerPropertyInfo>();

            //action only needs Operator and Opl
            PropertyInfo pi = this.GetType().GetProperty("Operator");
            DesignerPropertyInfo propertyInfo = new DesignerPropertyInfo(pi);
            result.Add(propertyInfo);

            pi = this.GetType().GetProperty("Opl");
            propertyInfo = new DesignerPropertyInfo(pi);
            result.Add(propertyInfo);

            //compare and compute needs Opr1 and Opr2 as well
            if (this.IsCompare() || this.IsCompute())
            {
                pi = this.GetType().GetProperty("Opr1");
                propertyInfo = new DesignerPropertyInfo(pi);
                result.Add(propertyInfo);
            }

            //assgin doesn't need Opr1
            if (this.IsCompare() || this.IsCompute() || this.IsAssign())
            {
                pi = this.GetType().GetProperty("Opr2");
                propertyInfo = new DesignerPropertyInfo(pi);
                result.Add(propertyInfo);
            }

            return result;
        }

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            AttachAction prec = (AttachAction)newattach;

            if (_opl != null)
            {
                prec._opl = (RightValueDef)_opl.Clone();
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

        public override void CheckForErrors(BehaviorNode rootBehavior, List<Node.ErrorCheck> result)
        {
            if (this._opl == null)
            {
                result.Add(new Node.ErrorCheck(this.Node, ErrorCheckLevel.Error, "Effector Left operand is not specified!"));
            }

            if (!this.IsAction())
            {
                // compute
                if (this.IsCompute())
                {
                    if (this._opr1 == null)
                    {
                        result.Add(new Node.ErrorCheck(this.Node, ErrorCheckLevel.Error, "Effector Right operand1 is not specified!"));
                    }
                }

                // assign, compute or compare
                if (this.Operator >= OperatorTypes.Assign)
                {
                    if (this._opr2 == null || this._opr2.IsMethod && this._opr2.Method == null || !this._opr2.IsMethod && this._opr2.Var == null)
                    {
                        result.Add(new Node.ErrorCheck(this.Node, ErrorCheckLevel.Error, "Effector Right operand2 is not specified!"));
                    }
                }
            }

            base.CheckForErrors(rootBehavior, result);
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

            if (bReset && metaOperation != MetaOperations.CheckProperty && metaOperation != MetaOperations.CheckMethod)
            {
                OnPropertyValueChanged(false);
            }

            return bReset;
        }
    }
}
