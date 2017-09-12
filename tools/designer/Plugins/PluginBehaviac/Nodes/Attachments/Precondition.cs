using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;
using PluginBehaviac.Properties;
using System.Reflection;

namespace PluginBehaviac.Events
{
    [Behaviac.Design.EnumDesc("PluginBehaviac.Nodes.PreconditionPhase", true, "PreconditionPhase", "PreconditionPhaseDesc")]
    public enum PreconditionPhase
    {
        [Behaviac.Design.EnumMemberDesc("Enter", "Enter")]
        Enter,

        [Behaviac.Design.EnumMemberDesc("Update", "Update")]
        Update,

        [Behaviac.Design.EnumMemberDesc("Both", "Both")]
        Both
    }

    [NodeDesc("Attachments", "precondition_icon")]
    class Precondition : Behaviac.Design.Attachments.AttachAction
    {
        public Precondition(Node node)
        : base(node, Resources.Precondition, Resources.PreconditionDesc)
        {
            if (node != null && node.IsFSM)
            {
                this._operator = OperatorTypes.Assign;
            }
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/attachment/#section";
            }
        }

        public override bool IsPrecondition
        {
            get
            {
                return true;
            }
        }

        public override string ExportClass
        {
            get
            {
                return "Precondition";
            }
        }

        public override NodeViewData.SubItemAttachment CreateSubItem()
        {
            return new SubItemPrecondition(this);
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.PreconditionUIPolicy();
        }

        private BinaryOperator _binary = BinaryOperator.And;
        [DesignerEnum("BinaryOperator", "BinaryOperatorDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, "")]
        public BinaryOperator BinaryOperator
        {
            get
            {
                return _binary;
            }
            set
            {
                _binary = value;
            }
        }

        private PreconditionPhase _phase = PreconditionPhase.Enter;
        [DesignerEnum("PreconditionPhase", "PreconditionPhaseDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, "EffectorOperaptor")]
        public PreconditionPhase Phase
        {
            get
            {
                return _phase;
            }
            set
            {
                _phase = value;
            }
        }

        [DesignerRightValueEnum("OperandLeft", "OperandLeftDesc", "Operation", DesignerProperty.DisplayMode.Parameter, 2, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.AttributesMethod, MethodType.Method, "", "Opr2", ValueTypes.All)]
        public override RightValueDef Opl
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

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            Precondition prec = (Precondition)newattach;

            prec._binary = _binary;
            prec._phase = _phase;
        }

        private bool isFirstCondition()
        {
            bool isFirst = (this.Node == null);

            if (this.Node != null)
            {
                int firstPreconditionIndex = -1;

                for (int i = 0; i < this.Node.Attachments.Count; ++i)
                {
                    if (this.Node.Attachments[i].IsPrecondition)
                    {
                        Precondition precondition = this.Node.Attachments[i] as Precondition;

                        if (precondition != null && precondition.IsCompare())
                        {
                            firstPreconditionIndex = i;
                            break;
                        }
                    }
                }

                isFirst = (firstPreconditionIndex < 0 || this.Node.Attachments[firstPreconditionIndex] == this);
            }

            return isFirst;
        }

        protected override string GeneratePropertiesLabel()
        {
            string str = base.GeneratePropertiesLabel();

            if (this.IsCompare() && !this.isFirstCondition())
            {
                System.Reflection.FieldInfo fi = this._binary.GetType().GetField(this._binary.ToString());
                Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(EnumMemberDescAttribute), false);

                if (attributes.Length > 0)
                {
                    str = ((EnumMemberDescAttribute)attributes[0]).DisplayName + "(" + str + ")";
                }
            }

            return str;
        }

        public override IList<DesignerPropertyInfo> GetDesignerProperties(bool bCustom = false)
        {
            IList<DesignerPropertyInfo> result = base.GetDesignerProperties(bCustom);

            if (bCustom)
            {
                PropertyInfo pi = this.GetType().GetProperty("BinaryOperator");
                DesignerPropertyInfo propertyInfo = new DesignerPropertyInfo(pi);
                result.Insert(0, propertyInfo);

                pi = this.GetType().GetProperty("Phase");
                propertyInfo = new DesignerPropertyInfo(pi);
                result.Add(propertyInfo);
            }

            return result;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<Node.ErrorCheck> result)
        {
            if (this.Node != null && this.Node.IsFSM && this._operator >= OperatorTypes.Equal && this._operator < OperatorTypes.Invalid)
            {
                result.Add(new Node.ErrorCheck(this.Node, ErrorCheckLevel.Error, "The operator can not be comparer!"));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }

    public class SubItemPrecondition : NodeViewData.SubItemEvent
    {
        public SubItemPrecondition(Attach e)
        : base(e)
        {
        }

        public override Brush BackgroundBrush
        {
            get
            {
                if (!IsSelected && _attachment != null && _attachment is Precondition)
                {
                    Precondition e = _attachment as Precondition;

                    if (e.Phase == PreconditionPhase.Enter)
                    {
                        return _theSuccessBrush;
                    }
                    else if (e.Phase == PreconditionPhase.Update)
                    {
                        return _theFailureBrush;
                    }

                    return _theBothBrush;
                }

                return base.BackgroundBrush;
            }
        }

        public override NodeViewData.SubItem Clone(Node newnode)
        {
            return new SubItemPrecondition((Attach)_attachment.Clone(newnode));
        }
    }
}
