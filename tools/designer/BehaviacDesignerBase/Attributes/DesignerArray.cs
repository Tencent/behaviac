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
    public class DesignerArray : DesignerProperty
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
        public DesignerArray(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerCompositeEditor), null)
        {
        }

        public override string GetDisplayValue(object obj)
        {
            return RetrieveDisplayValue(obj);
        }

        public override string GetExportValue(object owner, object obj)
        {
            return RetrieveExportValue(obj);
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            return ParseStringValue(result, type, str, node);
        }

        public static string RetrieveDisplayValue(object obj)
        {
            //if (obj != null)
            //{
            //    Type type = obj.GetType();
            //    if (Plugin.IsArrayType(type))
            //    {
            //        Type itemType = type.GetGenericArguments()[0];
            //        System.Collections.IList list = (System.Collections.IList)obj;

            //        return string.Format("{0}[{1}]", Plugin.GetNativeTypeName(itemType.Name), list.Count);
            //    }
            //}

            //return string.Empty;
            return RetrieveExportValue(obj);
        }

        public static string RetrieveExportValue(object obj)
        {
            if (obj != null)
            {
                Type type = obj.GetType();

                if (Plugin.IsArrayType(type))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    System.Collections.IList list = (System.Collections.IList)obj;

                    string str = string.Empty;

                    foreach (object item in list)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            str += "|";
                        }

                        str += DesignerPropertyUtility.RetrieveExportValue(item, null, null);
                    }

                    return string.Format("{0}:{1}", list.Count, str);
                }
            }

            return string.Empty;
        }

        public static object ParseStringValue(List<Nodes.Node.ErrorCheck> result, Type type, string str, DefaultObject node)
        {
            Debug.Check(Plugin.IsArrayType(type));

            Type itemType = type.GetGenericArguments()[0];

            if (Plugin.IsCustomClassType(itemType))
            {
                return DesignerArrayStruct.ParseStringValue(result, type, str, node);
            }

            object obj = Plugin.CreateInstance(type);
            Debug.Check(obj != null);

            if (!string.IsNullOrEmpty(str))
            {
                System.Collections.IList list = (System.Collections.IList)obj;

                int index = str.IndexOf(':');

                if (index >= 0)
                {
                    str = str.Substring(index + 1);
                }

                if (!string.IsNullOrEmpty(str))
                {
                    string[] tokens = str.Split('|');

                    foreach (string s in tokens)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            Plugin.InvokeTypeParser(result, itemType, s, (object value) => { list.Add(value); }, node);
                        }
                    }
                }
            }

            return obj;
        }
    }
}
