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
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;

namespace PluginBehaviac.Nodes
{
    public abstract class OperatorCondition : Node
    {
        public OperatorCondition(string label, string description)
        : base(label, description)
        {
        }

        public override bool IsCondition
        {
            get
            {
                return true;
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(110, 140, 189));
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
            nvd.ChangeShape(NodeShape.AngleRectangle);

            return nvd;
        }

        public override bool AcceptsAttachment(DefaultObject obj)
        {
            return false;
        }
    }
}
