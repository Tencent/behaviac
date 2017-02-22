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
using PluginBehaviac.Properties;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Composites:EventHandling", NodeIcon.EventHandle)]
    public class WithPrecondition : Behaviac.Design.Nodes.Node
    {
        protected ConnectorSingle _Precondition;
        protected ConnectorSingle _Action;

        public WithPrecondition()
        : base(Resources.WithPrecondition, Resources.WithPreconditionDesc)
        {
            _Precondition = new ConnectorSingle(_children, Resources.WithPreconditionPrecondition, "Precondition");
            _Action = new ConnectorSingle(_children, Resources.WithPreconditionAction, "Action");
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/docs/zh/tutorials/selectormonitor/";
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(79, 129, 189));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        public override string ExportClass
        {
            get
            {
                return "WithPrecondition";
            }
        }

        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            return base.CanBeAdoptedBy(parent) && (parent is SelectorLoop);
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (_Precondition.Child == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.NoPreconditionError));
            }

            if (_Action.Child == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.NoActionError));
            }

            if (!(this.Parent is SelectorLoop))
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.WithPreconditionParentError));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
