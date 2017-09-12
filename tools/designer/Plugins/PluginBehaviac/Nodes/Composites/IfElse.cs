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

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Composites:Sequences", "query_icon")]
    public class IfElse : Behaviac.Design.Nodes.Node
    {
        private ConnectorSingle _condition;
        private ConnectorSingle _if;
        private ConnectorSingle _else;

        public IfElse()
        : base(Resources.IfElse, Resources.IfElseDesc)
        {
            _condition = new ConnectorSingle(_children, Resources.IfElseCondition, "_condition");
            _if = new ConnectorSingle(_children, Resources.IfElseIf, "_if");
            _else = new ConnectorSingle(_children, Resources.IfElseElse, "_else");
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/ifelse/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "IfElse";
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

        public override void CheckForErrors(Behaviac.Design.Nodes.BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (_condition.Child == null)
            {
                result.Add(new Behaviac.Design.Nodes.Node.ErrorCheck(this, Behaviac.Design.Nodes.ErrorCheckLevel.Error,
                                                                     "You have to specify the condition"));
            }

            if (_if.Child == null)
            {
                result.Add(new Behaviac.Design.Nodes.Node.ErrorCheck(this, Behaviac.Design.Nodes.ErrorCheckLevel.Error,
                                                                     "'If' is not specified"));
            }

            if (_else.Child == null)
            {
                result.Add(new Behaviac.Design.Nodes.Node.ErrorCheck(this, Behaviac.Design.Nodes.ErrorCheckLevel.Error,
                                                                     "'Else' is not specified"));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
