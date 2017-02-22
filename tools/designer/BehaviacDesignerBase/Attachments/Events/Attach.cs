////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Nodes;

namespace Behaviac.Design.Attachments
{
    public enum BinaryOperator
    {
        [Behaviac.Design.EnumMemberDesc("And", "&&")]
        And,

        [Behaviac.Design.EnumMemberDesc("Or", "||")]
        Or
    }

    public class TransitionEffector : UIObject
    {
        public TransitionEffector()
        {
        }

        public TransitionEffector(TransitionEffector other)
        {
            this.Operator = other.Operator;

            if (other.Opl != null)
            {
                this.Opl = other.Opl.Clone() as RightValueDef;
            }

            if (other.Opr1 != null)
            {
                this.Opr1 = other.Opr1.Clone() as RightValueDef;
            }

            if (other.Opr2 != null)
            {
                this.Opr2 = other.Opr2.Clone() as RightValueDef;
            }
        }

        private RightValueDef _opl;
        [DesignerRightValueEnum("OperandLeft", "OperandLeftDesc", "Operation", DesignerProperty.DisplayMode.Parameter, 2, DesignerProperty.DesignerFlags.NoFlags | DesignerProperty.DesignerFlags.NoReadonly, DesignerPropertyEnum.AllowStyles.AttributesMethod, MethodType.Method, "", "", ValueTypes.All)]
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

        private RightValueDef _opr1;
        [DesignerRightValueEnum("Operand1", "OperandDesc1", "Operation", DesignerProperty.DisplayMode.Parameter, 3, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "Opr2", ValueTypes.All)]
        public RightValueDef Opr1
        {
            get
            {
                return _opr1;
            }
            set
            {
                this._opr1 = value;
            }
        }

        private OperatorTypes _operator = OperatorTypes.Assign;
        [DesignerEnum("Operator", "OperatorDesc", "Operation", DesignerProperty.DisplayMode.Parameter, 4, DesignerProperty.DesignerFlags.NoFlags, "EffectorOperaptor")]
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
                return _opr2;
            }
            set
            {
                this._opr2 = value;
            }
        }

        private bool IsAction()
        {
            return this._opl != null && this._opl.IsMethod && this._opl.Method != null;
        }

        public IList<DesignerPropertyInfo> GetDesignerProperties()
        {
            return DesignerProperty.GetDesignerProperties(this.GetType());
        }

        public Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.TransitionEffectorUIPolicy();
        }

        public object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            ArrayList enums = new ArrayList();
            enums.Add(OperatorTypes.Invalid);
            enums.Add(OperatorTypes.Equal);
            enums.Add(OperatorTypes.NotEqual);
            enums.Add(OperatorTypes.Greater);
            enums.Add(OperatorTypes.Less);
            enums.Add(OperatorTypes.GreaterEqual);
            enums.Add(OperatorTypes.LessEqual);

            if (enumAttr != null && enumAttr.ExcludeTag == "EffectorOperaptor")
            {
                if (this.Opl != null && !string.IsNullOrEmpty(this.Opl.NativeType))
                {
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
                    }

                    if (this.Opl.IsMethod && this.Opl.Method != null)
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

        public bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
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

            return bReset;
        }
    }

    /// <summary>
    /// This class represents an predicate which is attached to a node.
    /// </summary>
    public class Attach : Attachment
    {
        /// <summary>
        /// Create a new node event.
        /// </summary>
        /// <param name="node">The node this event belongs to.</param>
        public Attach(Nodes.Node node, string label, string description)
        : base(node, label, description)
        {
        }

        protected override void CloneProperties(Attachment newattach)
        {
            base.CloneProperties(newattach);
        }

        public override NodeViewData.SubItemAttachment CreateSubItem()
        {
            return new NodeViewData.SubItemEvent(this);
        }

    }
}
