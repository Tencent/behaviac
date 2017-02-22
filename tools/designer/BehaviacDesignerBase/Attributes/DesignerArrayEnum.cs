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
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    [AttributeUsage(/*AttributeTargets.Field | */AttributeTargets.Property)]
    public class DesignerArrayEnum : DesignerArray
    {
        /// <summary>
        /// Creates a new designer attribute for handling a string value.
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        public DesignerArrayEnum(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags)
        : base(displayName, description, category, displayMode, displayOrder, flags)
        {
        }

        public override string GetExportValue(object owner, object obj)
        {
            string str = string.Empty;

            if (obj != null)
            {
                Type type = obj.GetType();

                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = type.GetGenericArguments()[0];

                    if (itemType.IsEnum)
                    {
                        System.Collections.IList itemList = (System.Collections.IList)(obj);

                        foreach (object item in itemList)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                str += "|";
                            }

                            str += Enum.GetName(itemType, item);
                        }

                        str = str.Insert(0, string.Format("{0}:", itemList.Count));
                    }
                }
            }

            return str;
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (type.GetGenericTypeDefinition() != typeof(List<>))
            {
                throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            }

            Type itemType = type.GetGenericArguments()[0];

            if (!itemType.IsEnum)
            {
                throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            }

            System.Collections.IList enumArray = (System.Collections.IList)Plugin.CreateInstance(type);

            if (!string.IsNullOrEmpty(str))
            {
                int index = str.IndexOf(':');

                if (index >= 0)
                {
                    str = str.Substring(index + 1);
                }

                string[] tokens = str.Split('|');

                foreach (string s in tokens)
                {
                    enumArray.Add(Enum.Parse(itemType, s, true));
                }
            }

            return enumArray;
        }
    }
}
