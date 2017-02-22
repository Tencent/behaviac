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
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Actions", NodeIcon.Noop)]
    public class Noop : Node
    {
        public Noop()
        : base(Resources.Noop, Resources.NoopDesc)
        {
            _exportName = "Noop";
        }

        public override string ExportClass
        {
            get
            {
                return "Noop";
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
    }
}
