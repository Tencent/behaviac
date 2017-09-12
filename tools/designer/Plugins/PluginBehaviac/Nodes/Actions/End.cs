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
    [NodeDesc("Actions", "transition_icon")]
    public class End : Node
    {
        public End()
            : base(Resources.End, Resources.EndDesc)
        {
            _exportName = "End";
        }

        public override string ExportClass
        {
            get
            {
                return "End";
            }
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/end/";
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

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.EndUIPolicy();
        }

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            object[] status = new object[] { EBTStatus.BT_INVALID, EBTStatus.BT_RUNNING, XMLPluginBehaviac.behaviac_EBTStatus.BT_INVALID, XMLPluginBehaviac.behaviac_EBTStatus.BT_RUNNING };

            return status;
        }

        protected RightValueDef _endStatus = new RightValueDef(new VariableDef(XMLPluginBehaviac.behaviac_EBTStatus.BT_SUCCESS));
        [DesignerRightValueEnum("EndStatus", "EndStatusDesc", "End", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "")]
        public RightValueDef EndStatus
        {
            get
            {
                if (_endStatus == null)
                {
                    _endStatus = new RightValueDef(new VariableDef(XMLPluginBehaviac.behaviac_EBTStatus.BT_SUCCESS));
                }

                return _endStatus;
            }

            set
            {
                this._endStatus = value;
            }
        }

        protected bool _endOutside = false;
        [DesignerBoolean("EndOutside", "EndOutsideDesc", "End", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public bool EndOutside
        {
            get
            {
                return _endOutside;
            }
            set
            {
                _endOutside = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            End end = (End)newnode;
            end._endOutside = this._endOutside;

            if (_endStatus != null)
            {
                end._endStatus = (RightValueDef)this._endStatus.Clone();
            }
        }
    }
}
