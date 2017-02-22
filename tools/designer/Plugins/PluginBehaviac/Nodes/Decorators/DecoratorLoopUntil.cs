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
    [NodeDesc("Decorators", NodeIcon.LoopUntil)]
    public class DecoratorLoopUntil : DecoratorCount
    {
        public DecoratorLoopUntil()
        : base(Resources.DecoratorLoopUntil, Resources.DecoratorLoopUntilDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/language/zh/decorator/#loopuntil";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorLoopUntil";
            }
        }

        protected bool _until = true;
        [DesignerBoolean("DecoratorUntil", "DecoratorUntilDesc", "CategoryBasic", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags)]
        public bool Until
        {
            get
            {
                return _until;
            }
            set
            {
                _until = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            DecoratorLoopUntil node = (DecoratorLoopUntil)newnode;
            node._until = this._until;
        }
    }
}
