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
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;

namespace Behaviac.Design.Nodes
{
    /// <summary>
    /// This node represents a decorator which can be attached to the behaviour tree.
    /// </summary>
    public class Decorator : Node
    {
        protected ConnectorSingle _genericChildren;

        public Decorator(string label, string description)
        : base(label, description)
        {
            CreateInterruptChild();
            _genericChildren = new ConnectorSingle(_children, string.Empty, Connector.kGeneric);
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/decorator/";
            }
        }

        protected virtual void CreateInterruptChild()
        {
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorNode";
            }
        }

        protected bool _bDecorateWhenChildEnds = false;
        [DesignerBoolean("DecorateWhenChildEnds", "DecorateWhenChildEndsDesc", "CategoryBasic", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags)]
        public bool DecorateWhenChildEnds
        {
            get
            {
                return _bDecorateWhenChildEnds;
            }
            set
            {
                _bDecorateWhenChildEnds = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            Decorator dec = (Decorator)newnode;
            dec._bDecorateWhenChildEnds = _bDecorateWhenChildEnds;
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(96, 74, 123));
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
            nvd.ChangeShape(Behaviac.Design.NodeShape.Ellipse);

            return nvd;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (_genericChildren.ChildCount < 1)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.DecoratorHasNoChildError));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
