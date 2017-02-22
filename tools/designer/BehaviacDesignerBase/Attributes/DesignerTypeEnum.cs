using System;
using System.Collections.Generic;
using System.Text;
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

using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DesignerTypeEnum : DesignerProperty
    {
        /// <summary>
        /// Creates a new designer attribute for handling an enum type
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        public DesignerTypeEnum(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerTypeEnumEditor), null)
        {
        }

        public override string GetDisplayValue(object obj)
        {
            if (obj == null || !(obj is AgentType))
            {
                return string.Empty;
            }

            AgentType t = obj as AgentType;
            return t.DisplayName;
        }

        public override string GetExportValue(object owner, object obj)
        {
            if (obj == null)
            {
                return "\"\"";
            }

            return obj.ToString();
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            foreach (Plugin.InstanceName_t t in Plugin.InstanceNames)
            {
                if (str == t.AgentType.Name
#if BEHAVIAC_NAMESPACE_FIX
                    || t.AgentType.Name.EndsWith(str)
#endif
                    )
                {
                    return t.AgentType;
                }
            }

            foreach (AgentType t in Plugin.AgentTypes)
            {
                if (str == t.Name || str == t.OldName
#if BEHAVIAC_NAMESPACE_FIX
                    || t.Name.EndsWith(str) || t.OldName.EndsWith(str)
#endif
                    )
                {
                    return t;
                }
            }

            return null;
        }
    }
}
