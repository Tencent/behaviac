using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;
using PluginBehaviac.Properties;
using PluginBehaviac.Nodes;

namespace PluginBehaviac.Events
{
    [NodeDesc("FSM:Transition", "transition_icon")]
    class TransitionCondition : StartCondition
    {
        public TransitionCondition(Node node)
        : base(node, Resources.TransitionCondition, Resources.TransitionConditionDesc)
        {
        }

        public TransitionCondition(Node node, string label, string desc)
        : base(node, label, desc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/fsm/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "Transition";
            }
        }

        public override bool IsFSM
        {
            get
            {
                return true;
            }
        }

        public override bool CanBeDisabled()
        {
            return true;
        }

        public override bool IsStartCondition
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeDeleted
        {
            get
            {
                return true;
            }
        }

        private RightValueDef _opl;
        [DesignerRightValueEnum("OperandLeft", "OperandLeftDesc", "Operation", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags | DesignerProperty.DesignerFlags.NoReadonly, DesignerPropertyEnum.AllowStyles.AttributesMethod, MethodType.Getter, "", "", ValueTypes.All)]
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
        [DesignerRightValueEnum("Operand1", "OperandDesc1", "Operation", DesignerProperty.DisplayMode.Parameter, 2, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "Opr2", ValueTypes.All)]
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

        private OperatorTypes _operator = OperatorTypes.Equal;
        [DesignerEnum("Operator", "OperatorDesc", "Operation", DesignerProperty.DisplayMode.Parameter, 3, DesignerProperty.DesignerFlags.NoFlags, "TransitionOperaptor")]
        public OperatorTypes Operator
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
        [DesignerRightValueEnum("Operand2", "OperandDesc2", "Operation", DesignerProperty.DisplayMode.Parameter, 4, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "", ValueTypes.All)]
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

        private List<TransitionEffector> _effectors = new List<TransitionEffector>();
        [DesignerArrayStruct("Effectors", "EffectorsDesc", "Transition", DesignerProperty.DisplayMode.NoDisplay, 4, DesignerProperty.DesignerFlags.NoFlags)]
        public List<TransitionEffector> Effectors
        {
            get
            {
                return _effectors;
            }
            set
            {
                this._effectors = value;
            }
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.TransitionUIPolicy();
        }

        public override string Description
        {
            get
            {
                string str = base.Description;

                {
                    if (_opl != null)
                    {
                        str += "\n" + _opl.GetExportValue();
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

        protected override string GeneratePropertiesLabel()
        {
            string str = string.Empty;

            {
                bool isCompare = this.Operator >= OperatorTypes.Equal && this.Operator <= OperatorTypes.LessEqual;

                if (isCompare)
                {
                    if (_opl != null)
                    {
                        str += _opl.GetDisplayValue();
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
            enums.Add(OperatorTypes.Assign);
            enums.Add(OperatorTypes.Add);
            enums.Add(OperatorTypes.Sub);
            enums.Add(OperatorTypes.Mul);
            enums.Add(OperatorTypes.Div);

            if (enumAttr != null && enumAttr.ExcludeTag == "TransitionOperaptor")
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
                        enums.Add(OperatorTypes.Greater);
                        enums.Add(OperatorTypes.Less);
                        enums.Add(OperatorTypes.GreaterEqual);
                        enums.Add(OperatorTypes.LessEqual);
                    }
                }
            }

            return enums.ToArray();
        }

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            TransitionCondition con = (TransitionCondition)newattach;

            con.TargetFSMNodeId = this.TargetFSMNodeId;

            if (this._opl != null)
            {
                con._opl = (RightValueDef)this._opl.Clone();
            }

            if (this._opr1 != null)
            {
                con._opr1 = (RightValueDef)this._opr1.Clone();
            }

            con._operator = this._operator;

            if (_opr2 != null)
            {
                con._opr2 = (RightValueDef)_opr2.Clone();
            }

            con._effectors = new List<TransitionEffector>();

            foreach (TransitionEffector effector in this._effectors)
            {
                con._effectors.Add(new TransitionEffector(effector));
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<Node.ErrorCheck> result)
        {
            {
                if (this._opl == null)
                {
                    result.Add(new Node.ErrorCheck(this.Node, ErrorCheckLevel.Error, "Left operand is not specified!"));
                }

                if (this._opr2 == null)
                {
                    result.Add(new Node.ErrorCheck(this.Node, ErrorCheckLevel.Error, "Right operand is not specified!"));
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

            foreach (TransitionEffector effector in Effectors)
            {
                bReset |= effector.ResetMembers(metaOperation, agentType, baseType, method, property);
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
