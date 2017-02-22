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
using System.IO;
using System.Reflection;
using Behaviac.Design;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.DataExporters
{
    public class DataCsExporter : CsExporter
    {
        /// <summary>
        /// Generate code for the given value object.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <param name="stream">The file stream for generating the codes.</param>
        /// <param name="indent">The indent string when generating the line of codes.</param>
        /// <param name="typename">The native type of the variable.</param>
        /// <param name="var">The variable for the given object when generating the codes.</param>
        /// <param name="caller">The caller for the method or the agent.</param>
        /// <returns>Returns the string generated value.</returns>
        public static string GenerateCode(object obj, DefaultObject defaultObj, StringWriter stream, string indent, string typename, string var, string caller, string setValue = null)
        {
            string retStr = string.Empty;

            if (obj != null)
            {
                Type type = obj.GetType();

                if (obj is Behaviac.Design.MethodDef)
                {
                    Behaviac.Design.MethodDef method = obj as Behaviac.Design.MethodDef;
                    retStr = MethodCsExporter.GenerateCode(defaultObj, method, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.MethodDef.Param)
                {
                    Behaviac.Design.MethodDef.Param param = obj as Behaviac.Design.MethodDef.Param;
                    retStr = ParameterCsExporter.GenerateCode(defaultObj, param, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.ParInfo)
                {
                    Behaviac.Design.ParInfo par = obj as Behaviac.Design.ParInfo;
                    retStr = ParInfoCsExporter.GenerateCode(par, false, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.PropertyDef)
                {
                    Behaviac.Design.PropertyDef property = obj as Behaviac.Design.PropertyDef;
                    retStr = PropertyCsExporter.GenerateCode(defaultObj, property, null, false, stream, indent, typename, var, caller, setValue);
                }
                else if (obj is Behaviac.Design.VariableDef)
                {
                    Behaviac.Design.VariableDef variable = obj as Behaviac.Design.VariableDef;
                    retStr = VariableCsExporter.GenerateCode(defaultObj, variable, false, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.RightValueDef)
                {
                    Behaviac.Design.RightValueDef rightValue = obj as Behaviac.Design.RightValueDef;
                    retStr = RightValueCsExporter.GenerateCode(defaultObj, rightValue, stream, indent, typename, var, caller);
                }
                // Array type
                else if (Plugin.IsArrayType(type))
                {
                    retStr = var;

                    if (!string.IsNullOrEmpty(typename))
                    {
                        stream.WriteLine("{0}{1} {2};", indent, typename, var);
                    }
                    else
                    {
                        typename = DataCsExporter.GetGeneratedNativeType(type);
                    }

                    int startIndex = typename.IndexOf('<');
                    int endIndex = typename.LastIndexOf('>');
                    string itemType = typename.Substring(startIndex + 1, endIndex - startIndex - 1);

                    ArrayCsExporter.GenerateCode(obj, defaultObj, stream, indent, itemType, var);
                }
                // Struct type
                else if (Plugin.IsCustomClassType(type))
                {
                    retStr = var;

                    if (!string.IsNullOrEmpty(typename))
                    {
                        stream.WriteLine("{0}{1} {2};", indent, typename, var);
                    }

                    StructCsExporter.GenerateCode(obj, defaultObj, stream, indent, var, typename, null, "");
                }
                // Other types
                else
                {
                    retStr = obj.ToString();

                    if (Plugin.IsStringType(type)) // string
                    {
                        retStr = string.Format("\"{0}\"", retStr);
                    }
                    else if (Plugin.IsCharType(type)) // char
                    {
                        char c = 'A';

                        if (retStr.Length >= 1)
                        {
                            c = retStr[0];
                        }

                        retStr = string.Format("\'{0}\'", c);
                    }
                    else if (Plugin.IsBooleanType(type)) // bool
                    {
                        retStr = retStr.ToLowerInvariant();
                    }
                    else if (Plugin.IsEnumType(type)) // enum
                    {
                        retStr = EnumCsExporter.GeneratedCode(obj);
                    }
                    else if (type == typeof(float)) // float
                    {
                        retStr += "f";
                    }

                    if (!string.IsNullOrEmpty(var))
                    {
                        if (string.IsNullOrEmpty(typename))
                        {
                            stream.WriteLine("{0}{1} = {2};", indent, var, retStr);
                        }
                        else
                        {
                            stream.WriteLine("{0}{1} {2} = {3};", indent, typename, var, retStr);
                        }
                    }
                }
            }

            return retStr;
        }
    }
}
