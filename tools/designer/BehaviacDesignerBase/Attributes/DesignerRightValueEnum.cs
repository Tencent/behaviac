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
using System.Globalization;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DesignerRightValueEnum : DesignerPropertyEnum
    {
        private MethodType _methodType;
        public MethodType MethodType
        {
            get
            {
                return this._methodType;
            }
        }

        /// <summary>
        /// Creates a new designer attribute for handling a string value.
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        public DesignerRightValueEnum(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags, AllowStyles styles, MethodType methodType, string dependedProperty, string dependingProperty, ValueTypes filterType = ValueTypes.All)
        : base(displayName, description, category, displayMode, displayOrder, flags, styles, dependedProperty, dependingProperty, filterType)
        {
            _methodType = methodType;
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (type != typeof(RightValueDef))
            {
                throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            }

            if (str.Length == 0 ||
                str.Length == 2 && str == "\"\"")
            {
                return null;
            }

            if (!str.StartsWith("const"))
            {
                int pos = str.IndexOf('(');

                if (pos < 0)
                {
                    VariableDef var = DesignerPropertyEnum.parsePropertyVar(result, node, str);

                    return new RightValueDef(var);

                }
                else
                {
                    Nodes.Behavior behavior = node.Behavior as Nodes.Behavior;
                    AgentType agentType = (behavior != null) ? behavior.AgentType : null;

                    string valueClass = VariableDef.kSelfMethod;
                    MethodDef method = DesignerMethodEnum.parseMethodString(result, node, agentType, this.MethodType, str);

                    if (method == null)
                    {
                        string className = Plugin.GetClassName(str);
                        method = DesignerMethodEnum.parseMethodString(result, node, Plugin.GetInstanceAgentType(className, behavior, null), this.MethodType, str);
                        valueClass = className + VariableDef.kMethod;
                    }

                    string instanceName = Plugin.GetInstanceName(str);

                    if (!string.IsNullOrEmpty(instanceName))
                    {
                        valueClass = instanceName + VariableDef.kMethod;
                    }

                    return new RightValueDef(method, valueClass);
                }

            }
            else
            {
                VariableDef var = this.parseConstVar(result, node, parentObject, str);

                if (var != null)
                {
                    return new RightValueDef(var);
                }
            }

            return null;
        }
    }
}
