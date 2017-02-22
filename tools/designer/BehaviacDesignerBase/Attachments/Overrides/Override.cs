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
using System.Windows.Forms;
using System.Reflection;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attachments.Overrides
{
    /// <summary>
    /// This class represents an override which is attached to a node.
    /// </summary>
    public abstract class Override : Attachment
    {
        protected string _propertyToOverride = string.Empty;

        [DesignerNodeProperty("OverrideProperty", "OverridePropertyDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, new Type[] { typeof(int), typeof(float), typeof(Enum) })]
        public string PropertyToOverride
        {
            get
            {
                return _propertyToOverride;
            }
            set
            {
                _propertyToOverride = value;
            }
        }


        public override string ExportClass
        {
            get
            {
                return "Override";
            }
        }

        /// <summary>
        /// Create a new node override.
        /// </summary>
        /// <param name="node">The node this override belongs to.</param>
        public Override(Nodes.Node node, string label, string description)
        : base(node, label, description)
        {
        }

        public override NodeViewData.SubItemAttachment CreateSubItem()
        {
            return new NodeViewData.SubItemOverride(this);
        }
    }
}
