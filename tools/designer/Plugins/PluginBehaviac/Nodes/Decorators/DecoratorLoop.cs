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
    [NodeDesc("Decorators", NodeIcon.Loop)]
    public class DecoratorLoop : DecoratorCount
    {
        public DecoratorLoop() : base(Resources.DecoratorLoop, Resources.DecoratorLoopDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/decorator/#loop";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorLoop";
            }
        }

        protected bool _bDoneWithinFrame = false;
        [DesignerBoolean("DoneWithinFrame", "DoneWithinFrameDesc", "CategoryBasic", DesignerProperty.DisplayMode.ListTrue, 0, DesignerProperty.DesignerFlags.NoFlags)]
        public bool DoneWithinFrame
        {
            get
            {
                return _bDoneWithinFrame;
            }
            set
            {
                _bDoneWithinFrame = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            DecoratorLoop dec = (DecoratorLoop)newnode;
            dec._bDoneWithinFrame = this._bDoneWithinFrame;
        }


        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            base.CheckForErrors(rootBehavior, result);

            if (this._bDoneWithinFrame)
            {
                long count = this.GetCount();

                if (count == -1)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "when 'DoneWithinFrame' is selected, Count should not be -1 as an endless loop!"));
                }
            }
        }
    }
}
