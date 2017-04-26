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
using PluginBehaviac.Properties;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Composites:EventHandling", "selectorLoop_ico")]
    public class SelectorLoop : Behaviac.Design.Nodes.Sequence
    {
        public SelectorLoop()
        : base(Resources.SelectorLoop, Resources.SelectorLoopDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/selectormonitor/";
            }
        }

        protected override void CreateInterruptChild()
        {
        }

        public override string ExportClass
        {
            get
            {
                return "SelectorLoop";
            }
        }

        protected bool _resetChildren = false;
        [DesignerBoolean("ResetChildren", "ResetChildrenDesc", "SelectorLoop", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags)]
        public bool ResetChildren
        {
            get
            {
                return _resetChildren;
            }
            set
            {
                _resetChildren = value;
            }
        }

        public override bool AddChild(Connector connector, Node node)
        {
            if (node is WithPrecondition)
            {
                return base.AddChild(connector, node);
            }
            else
            {
                WithPrecondition withPrecondition = new WithPrecondition();

                if (base.AddChild(connector, withPrecondition))
                {
                    withPrecondition.ResetId(false);
                    return withPrecondition.AddChild(withPrecondition.GetConnector("Action"), node);
                }
            }

            return false;
        }

        public override bool AddChild(Connector connector, Node node, int index)
        {
            if (node is WithPrecondition)
            {
                return base.AddChild(connector, node, index);
            }
            else
            {
                WithPrecondition withPrecondition = new WithPrecondition();

                if (base.AddChild(connector, withPrecondition, index))
                {
                    withPrecondition.ResetId(false);
                    return withPrecondition.AddChild(withPrecondition.GetConnector("Action"), node);
                }
            }

            return false;
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            foreach (BaseNode child in _genericChildren.Children)
            {
                if (!(child is WithPrecondition))
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.SelectorLoopChildChildError));
                }
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
