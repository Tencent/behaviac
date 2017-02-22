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
    public class DesignerArrayStruct : DesignerArray
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
        public DesignerArrayStruct(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags)
        : base(displayName, description, category, displayMode, displayOrder, flags)
        {
        }

        public override string GetExportValue(object owner, object obj)
        {
            string str = string.Empty;

            if (obj != null)
            {
                Type type = obj.GetType();

                if (Plugin.IsArrayType(type))
                {
                    Type itemType = type.GetGenericArguments()[0];

                    if (Plugin.IsCustomClassType(itemType))
                    {
                        System.Collections.IList itemList = (System.Collections.IList)(obj);

                        foreach (object item in itemList)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                str += "|";
                            }

                            str += "{" + getExportValue(item, itemType) + "}";
                        }

                        str = string.Format("{0}:{1}", itemList.Count, str);
                    }
                }
            }

            return str;
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            Debug.Check(Plugin.IsArrayType(type));
            return ParseStringValue(result, type, str, node);
        }

        public new static object ParseStringValue(List<Nodes.Node.ErrorCheck> result, Type type, string str, DefaultObject node)
        {
            Debug.Check(type != null && Plugin.IsArrayType(type));

            if (type != null)
            {
                object obj = Plugin.CreateInstance(type);
                Debug.Check(obj != null);

                if (obj != null && !string.IsNullOrEmpty(str))
                {
                    System.Collections.IList list = (System.Collections.IList)obj;
                    Type itemType = type.GetGenericArguments()[0];

                    int index = str.IndexOf(':');

                    if (index >= 0)
                    {
                        str = str.Substring(index + 1);
                    }

                    if (!string.IsNullOrEmpty(str))
                    {
                        System.Collections.IList structArray = (System.Collections.IList)Plugin.CreateInstance(type);
                        parseStringValue(result, node, structArray, itemType, str, 0, str.Length - 1);

                        return structArray;
                    }
                }

                return obj;
            }

            return null;
        }

        private string getExportValue(object item, Type itemType)
        {
            string str = string.Empty;

            if (item != null)
            {
                IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(itemType);

                foreach (DesignerPropertyInfo property in properties)
                {
                    if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                    {
                        str += property.Property.Name + "=" + property.GetExportValue(item) + ";";
                    }
                }
            }

            return str;
        }

        private static int getItemStr(string str, int startIndex, int endIndex, out string propertyName, out string propertyValue)
        {
            propertyName = string.Empty;
            propertyValue = string.Empty;

            int brackets = 0;
            int itemStartIndex = -1;

            for (int i = startIndex; i <= endIndex; ++i)
            {
                if (str[i] == '{')
                {
                    brackets++;

                    if (brackets == 1)
                    {
                        itemStartIndex = i;
                    }

                }
                else if (str[i] == '}')
                {
                    Debug.Check(brackets > 0);
                    brackets--;
                }

                if (brackets == 0 && (str[i] == '|' || i == endIndex))
                {
                    propertyValue = str.Substring(itemStartIndex + 1, i - itemStartIndex - 1);
                    return i + 1;
                }
            }

            return -1;
        }

        private static void parseStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, System.Collections.IList structArray, Type itemType, string str, int startIndex, int endIndex)
        {
            string propertyName = string.Empty;
            string propertyValue = string.Empty;

            int nextIndex = getItemStr(str, startIndex, endIndex, out propertyName, out propertyValue);

            if (nextIndex > 0)
            {
                object item = DesignerStruct.ParseStringValue(result, itemType, null, propertyValue, node);
                structArray.Add(item);

                parseStringValue(result, node, structArray, itemType, str, nextIndex, endIndex);
            }
        }
    }
}
