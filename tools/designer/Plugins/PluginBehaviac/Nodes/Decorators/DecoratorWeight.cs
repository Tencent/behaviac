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
using System.Drawing;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Nodes
{
    public class DecoratorWeight : Decorator
    {
        public DecoratorWeight()
        : base(Resources.DecoratorWeight, Resources.DecoratorWeightDesc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorWeight";
            }
        }

        public override bool CanBeDragged()
        {
            return false;
        }

        public override bool CanBeDeleted()
        {
            return (Children.Count == 0 || !(this.Parent is SelectorProbability)) && base.CanBeDeleted();
        }

        public override bool AlwaysExpanded()
        {
            return true;
        }

        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            return base.CanBeAdoptedBy(parent) && (parent is SelectorProbability);
        }

        protected VariableDef _weight = new VariableDef((int)1);
        [DesignerPropertyEnum("DecoratorWeight", "DecoratorWeightDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributes, "", "", ValueTypes.Int, 0, 100000)]
        public VariableDef Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                this._weight = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            if (_weight != null)
            {
                DecoratorWeight dec = (DecoratorWeight)newnode;
                dec._weight = (VariableDef)_weight.Clone();
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(100, 60, 40));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        public override NodeViewData CreateNodeViewData(NodeViewData parent, BehaviorNode rootBehavior)
        {
            NodeViewData nvd = base.CreateNodeViewData(parent, rootBehavior);
            nvd.ChangeShape(NodeShape.Ellipse);

            return nvd;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (!(this.Parent is SelectorProbability))
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.WeightParentError));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
