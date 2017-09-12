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
    [NodeDesc("Decorators", NodeIcon.WaitFrame)]
    public class DecoratorFrames : Decorator
    {
        public DecoratorFrames()
        : base(Resources.DecoratorFrames, Resources.DecoratorFramesDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/decorator/#frames";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorFrames";
            }
        }

        protected RightValueDef _frames = new RightValueDef(new VariableDef((int)100));
        [DesignerRightValueEnum("DecoratorFrames", "DecoratorFramesDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Int)]
        public RightValueDef Frames
        {
            get
            {
                return _frames;
            }
            set
            {
                this._frames = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            DecoratorFrames dec = (DecoratorFrames)newnode;

            if (_frames != null)
            {
                dec._frames = (RightValueDef)_frames.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            Type valueType = (this._frames != null) ? this._frames.ValueType : null;

            if (valueType == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Frames is not set!"));
            }
            else
            {
                string typeName = Plugin.GetNativeTypeName(valueType.FullName);

                if (!Plugin.IsIntergerNumberType(typeName))
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Frames should be an integer number type!"));
                }
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
