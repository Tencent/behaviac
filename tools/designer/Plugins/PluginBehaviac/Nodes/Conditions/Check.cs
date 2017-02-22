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
    public class Check : Behaviac.Design.Nodes.Condition
    {
        public Check()
        : base(Resources.Check, Resources.CheckDesc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "Check";
            }
        }


        private MethodDef _opl;

        [DesignerMethodEnum("OperandLeft", "OperandLeftDesc", "Check", DesignerProperty.DisplayMode.List, 0, DesignerProperty.DesignerFlags.NoFlags, MethodType.Getter)]
        public MethodDef Opl
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
        [DesignerEnum("Operator", "OperatorDesc", "Check", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, new object[] { OperatorType.And, OperatorType.Or })]
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

        private VariableDef _opr;
        [DesignerPropertyEnum("OperandRight", "OperandRightDesc", "Check", DesignerProperty.DisplayMode.List, 2, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAndTypes, "Opl")]
        public VariableDef Opr
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

        public override bool ShouldExclude()
        {
            if (this.Opl != null && this.Opl.ReturnType != typeof(bool))
            {
                //and and or are only valid for bool, so to exclude and and or when the type is not bool
                return true;
            }

            return false;
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            Check prec = (Check)newnode;

            //prec._negate = _negate;
            prec._opl = this._opl;
            prec._opr = this._opr;
            prec._operator = this._operator;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (this.Opl == null || this.Opr == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Operand is not complete!"));
            }
            else if (this.Opl.ReturnType != typeof(bool) &&
                     (this.Operator == OperatorType.And || this.Operator == OperatorType.Or))
            {
                //and and or are only valid for bool
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "And and Or are only valid for bool!"));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
