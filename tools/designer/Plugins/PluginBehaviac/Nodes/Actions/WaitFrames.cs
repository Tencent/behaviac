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
using PluginBehaviac.Properties;
using Behaviac.Design.Nodes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Actions", NodeIcon.WaitFrame)]
    public class WaitFrames : Behaviac.Design.Nodes.Node
    {
        public WaitFrames()
        : base(Resources.WaitFrames, Resources.WaitFramesDesc)
        {
            _exportName = "WaitFrames";
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/waitframes/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "WaitFrames";
            }
        }

        private RightValueDef _frames = new RightValueDef(new VariableDef((int)100));
        [DesignerRightValueEnum("Frames", "FramesDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Int)]
        public RightValueDef Frames
        {
            get
            {
                return _frames;
            }
            set
            {
                _frames = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            WaitFrames dec = (WaitFrames)newnode;

            if (_frames != null)
            {
                dec._frames = (RightValueDef)_frames.Clone();
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

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (this._frames != null)
            {
                bReset |= this._frames.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

            return bReset;
        }
    }
}
