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
    public static class DesignerPropertyUtility
    {
        /// <summary>
        /// Get the display string in the editor of the given object.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <returns>Returns the string value for displaying the object.</returns>
        public static string RetrieveDisplayValue(object obj, object parent = null, string paramName = null, int indexInArray = -1)
        {
            string str = string.Empty;

            if (obj != null)
            {
                Type type = obj.GetType();

                // ISerializableData type
                if (obj is ISerializableData)
                {
                    str = ((ISerializableData)obj).GetDisplayValue();
                }

                // Array type
                else if (Plugin.IsArrayType(type))
                {
                    str = DesignerArray.RetrieveDisplayValue(obj);
                }

                // Struct type
                else if (Plugin.IsCustomClassType(type))
                {
                    str = DesignerStruct.RetrieveDisplayValue(obj, parent, paramName, indexInArray);
                }

                // Enum type
                else if (type.IsEnum)
                {
                    str = DesignerEnum.GetDisplayName(obj);
                }

                // Other types
                else
                {
                    str = obj.ToString();

                    if (Plugin.IsStringType(type))
                    {
                        str = string.Format("\"{0}\"", str);

                    }
                    else
                    {
                        string[] tokens = str.Split(' ');
                        str = tokens[tokens.Length - 1];
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// Get the string value when saving or exporting the given object.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <returns>Returns the string value for saving or exporting the object.</returns>
        public static string RetrieveExportValue(object obj, object parent = null, string paramName = null)
        {
            string str = "\"\"";

            if (obj != null)
            {
                Type type = obj.GetType();

                // ISerializableData type
                if (obj is ISerializableData)
                {
                    str = ((ISerializableData)obj).GetExportValue();
                }

                // Array type
                else if (Plugin.IsArrayType(type))
                {
                    str = DesignerArray.RetrieveExportValue(obj);
                }

                // Struct type
                else if (Plugin.IsCustomClassType(type))
                {
                    str = DesignerStruct.RetrieveExportValue(obj, parent, paramName, true);
                }

                // Other types
                else
                {
                    if (obj is char)
                    {
                        char c = (char)obj;

                        if (c == '\0')
                        {
                            str = "";

                        }
                        else
                        {
                            str = obj.ToString();
                        }

                    }
                    else
                    {
                        str = obj.ToString();
                    }

                    if (Plugin.IsStringType(type))
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    else if (Plugin.IsBooleanType(type))
                    {
                        str = str.ToLowerInvariant();
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// Parse the string value to create an object.
        /// </summary>
        /// <param name="type">The type of the created object.</param>
        /// <param name="str">The string value of the created object.</param>
        /// <param name="node">The owner node of the created object.</param>
        /// <returns>Returns the created object.</returns>
        public static object ParseStringValue(List<Nodes.Node.ErrorCheck> result, Type type, string str, DefaultObject node)
        {
            object obj = null;
            Plugin.InvokeTypeParser(result, type, str, (object value) => obj = value, node);

            return obj;
        }
    }
}
