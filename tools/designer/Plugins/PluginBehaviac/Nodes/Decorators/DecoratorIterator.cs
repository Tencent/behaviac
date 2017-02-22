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
using Behaviac.Design;
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Decorators", "try_icon")]
    class DecoratorIterator : Decorator
    {
        public DecoratorIterator()
        : base(Resources.DecoratorIterator, Resources.DecoratorIteratorDesc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorIterator";
            }
        }

        private VariableDef _opl;
        [DesignerPropertyEnum("OperandLeft", "OperandLeftDesc", "Predicate", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.Attributes, "", "Opr")]
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

        private RightValueDef _opr;
        [DesignerRightValueEnum("OperandRight", "OperandRightDesc", "Predicate", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "", ValueTypes.Array)]
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

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            return null;
        }

        public override string GenerateNewLabel()
        {
            string baseLabel = this.Label + "(";

            if (Opl != null)
            {
                baseLabel += Opl.GetDisplayValue() + " in ";
            }

            if (Opr != null)
            {
                baseLabel += Opr.GetDisplayValue();
            }

            baseLabel += ")";

            return baseLabel;
        }

        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            if (base.CanBeAdoptedBy(parent))
            {
                // check if there is a Method node on the parent path
                while (parent != null)
                {
                    if (parent is Nodes.Method)
                    {
                        return true;
                    }

                    parent = parent.Parent;
                }

                parent = this.Parent;

                while (parent != null)
                {
                    if (parent is Nodes.Method)
                    {
                        return true;
                    }

                    parent = parent.Parent;
                }
            }

            return false;
        }

        protected override void CloneProperties(Behaviac.Design.Nodes.Node newattach)
        {
            base.CloneProperties(newattach);

            DecoratorIterator prec = (DecoratorIterator)newattach;

            if (_opl != null)
            {
                prec._opl = (VariableDef)_opl.Clone();
            }

            if (_opr != null)
            {
                prec._opr = (RightValueDef)_opr.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<Node.ErrorCheck> result)
        {
            if (this.Opl == null || this.Opr == null || this.Opl.ToString() == "" || this.Opr.ToString() == "" ||
                !Plugin.IsArrayType(this.Opr.ValueType) || this.Opl.ValueType != this.Opr.ValueType.GetGenericArguments()[0])
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.OperandError));
            }

            base.CheckForErrors(rootBehavior, result);
        }

    }
}
