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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;
using System.Reflection;

namespace PluginBehaviac.Events
{
    [Behaviac.Design.EnumDesc("PluginBehaviac.Nodes.EffectorPhase", true, "EffectorPhase", "EffectorPhaseDesc")]
    public enum EffectorPhase
    {
        [Behaviac.Design.EnumMemberDesc("Success", "Success")]
        Success,

        [Behaviac.Design.EnumMemberDesc("Failure", "Failure")]
        Failure,

        [Behaviac.Design.EnumMemberDesc("Both", "Both")]
        Both
    }

    [NodeDesc("Attachments", "effector_icon")]
    class Effector : Behaviac.Design.Attachments.AttachAction
    {
        public Effector(Node node)
        : base(node, Resources.Effector, Resources.EffectorDesc)
        {
            this._operator = OperatorTypes.Assign;
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/attachment/#section";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "Effector";
            }
        }

        public override bool IsEffector
        {
            get
            {
                return true;
            }
        }

        public override NodeViewData.SubItemAttachment CreateSubItem()
        {
            return new SubItemEffector(this);
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.EffectorUIPolicy();
        }

        private EffectorPhase _phase = EffectorPhase.Success;
        [DesignerEnum("EffectorPhase", "EffectorPhaseDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, "EffectorOperaptor")]
        public EffectorPhase Phase
        {
            get
            {
                return _phase;
            }
            set
            {
                _phase = value;
            }
        }

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            Effector prec = (Effector)newattach;

            prec._phase = _phase;
        }

        public override bool IsAction()
        {
            return this.Opl != null && this.Opl.IsMethod && this.Opl.Method != null;
        }

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            object[] excludedEnums = base.GetExcludedEnums(enumAttr);

            Debug.Check(excludedEnums.Length >= 1 && (OperatorTypes)excludedEnums[0] == OperatorTypes.Invalid);

            ArrayList enums = new ArrayList(excludedEnums);
            List<OperatorTypes> compareEnums = new List<OperatorTypes> { OperatorTypes.Equal, OperatorTypes.Greater, OperatorTypes.Less, OperatorTypes.GreaterEqual, OperatorTypes.LessEqual, OperatorTypes.NotEqual };

            foreach (object e in compareEnums)
            {
                if (!enums.Contains(e))
                {
                    enums.Add(e);
                }
            }

            return enums.ToArray();
        }

        public override IList<DesignerPropertyInfo> GetDesignerProperties(bool bCustom = false)
        {
            IList<DesignerPropertyInfo> result = base.GetDesignerProperties(bCustom);

            if (bCustom)
            {
                PropertyInfo pi = this.GetType().GetProperty("Phase");
                DesignerPropertyInfo propertyInfo = new DesignerPropertyInfo(pi);
                result.Add(propertyInfo);
            }

            return result;
        }
    }

    public class SubItemEffector : NodeViewData.SubItemEvent
    {
        public SubItemEffector(Attach e)
        : base(e)
        {
        }

        public override Brush BackgroundBrush
        {
            get
            {
                if (!IsSelected && _attachment != null && _attachment is Effector)
                {
                    Effector e = _attachment as Effector;

                    if (e.Phase == EffectorPhase.Success)
                    {
                        return _theSuccessBrush;
                    }
                    else if (e.Phase == EffectorPhase.Failure)
                    {
                        return _theFailureBrush;
                    }

                    return _theBothBrush;
                }

                return base.BackgroundBrush;
            }
        }

        public override NodeViewData.SubItem Clone(Node newnode)
        {
            return new SubItemEffector((Attach)_attachment.Clone(newnode));
        }
    }
}
