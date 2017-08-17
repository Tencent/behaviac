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
using Behaviac.Design;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.DataExporters
{
    public class DataCppExporter
    {
        public static string GetExportNativeType(string typeName)
        {
            typeName = DataCsExporter.GetExportNativeType(typeName);

            typeName = typeName.Replace("byte", "ubyte");

            return typeName;
        }

        public static string GetGeneratedNativeType(Type type)
        {
            string typeName = Plugin.GetNativeTypeName(type);

            if (!typeName.Contains("*") && Plugin.IsRefType(type))
            {
                typeName += "*";
            }

            return GetGeneratedNativeType(typeName);
        }

        public static string GetGeneratedNativeType(string typeName)
        {
            typeName = Plugin.GetNativeTypeName(typeName);

            if (typeName.StartsWith("vector<"))
            {
                typeName = typeName.Replace("vector<", "behaviac::vector<");
            }
            else if (typeName.StartsWith("const vector<"))
            {
                typeName = typeName.Replace("const vector<", "const behaviac::vector<");
            }

            typeName = typeName.Replace("cszstring", "const char*");
            typeName = typeName.Replace("szstring", "char*");
            typeName = typeName.Replace("sbyte", "signed char");
            typeName = typeName.Replace("ubyte", "unsigned char");
            typeName = typeName.Replace("uchar", "unsigned char");
            typeName = typeName.Replace("ushort", "unsigned short");
            typeName = typeName.Replace("uint", "unsigned int");
            typeName = typeName.Replace("llong", "long long");
            typeName = typeName.Replace("ullong", "unsigned long long");
            typeName = typeName.Replace("ulong", "unsigned long");
            typeName = typeName.Replace("const ", "");

            //repalce "std::string" and "string" with "beahviac::string"
            if (typeName.Contains("std::string"))
            {
                typeName = typeName.Replace("std::string", "behaviac::string");
            }
            else if (typeName.Contains("std::wstring"))
            {
                typeName = typeName.Replace("std::wstring", "behaviac::wstring");
            }
            else if (!typeName.Contains("behaviac::string") && !typeName.Contains("behaviac::wstring"))
            {
                if (typeName.Contains("wstring"))
                {
                    typeName = typeName.Replace("wstring", "behaviac::wstring");
                }
                else
                {
                    typeName = typeName.Replace("string", "behaviac::string");
                }
            }

            if (Plugin.TypeRenames.Count > 0)
            {
                foreach (KeyValuePair<string, string> typePair in Plugin.TypeRenames)
                {
                    typeName = typeName.Replace(typePair.Key, typePair.Value);
                }
            }

            return typeName;
        }

        public static string GetGeneratedNativeType(Type type, string nativeTypeName)
        {
            if (!nativeTypeName.Contains("*"))
            {
                if (Plugin.IsRefType(type))
                {
                    nativeTypeName += "*";
                }
                else if (!nativeTypeName.EndsWith("&"))
                {
                    if (Plugin.IsCustomClassType(type))
                    {
                        nativeTypeName += "&";
                    }
                }
            }

            return GetGeneratedNativeType(nativeTypeName);
        }

        public static string GetBasicGeneratedNativeType(string typeName, bool removePointRef = true)
        {
            typeName = GetGeneratedNativeType(typeName);

            typeName = typeName.Replace("const ", "");

            if (removePointRef || !typeName.Contains("*&"))
            {
                typeName = typeName.Replace("&", "");
            }

            typeName = typeName.Trim();

            return typeName;
        }

        public static string GetGeneratedDefaultValue(Type type, string typename, string defaultValue = null)
        {
            if (type == typeof(void))
            {
                return null;
            }

            string value = defaultValue;

            if (string.IsNullOrEmpty(defaultValue))
            {
                if (!Plugin.IsStringType(type))
                {
                    value = DesignerPropertyUtility.RetrieveExportValue(Plugin.DefaultValue(type));
                }
                else
                {
                    value = "";
                }
            }

            if (type == typeof(char))
            {
                value = "(char)0";
            }
            else if (type == typeof(float))
            {
                if (value == "0")
                {
                    value = "0.0";
                }

                if (!string.IsNullOrEmpty(value) && !value.ToLowerInvariant().EndsWith("f"))
                {
                    value += "f";
                }
            }
            else if (Plugin.IsStringType(type))
            {
                if (typename.EndsWith("char*"))
                {
                    value = "NULL";
                }
                else
                {
                    value = "\"" + value + "\"";
                }
            }
            else if (Plugin.IsEnumType(type))
            {
                // remove the enum name
                int index = typename.LastIndexOf("::");
                if (index >= 0)
                {
                    typename = typename.Substring(0, index);
                    value = string.Format("{0}::{1}", typename, value);
                }
            }
            else if (Plugin.IsArrayType(type))
            {
                value = null;
            }
            else if (Plugin.IsCustomClassType(type))
            {
                if (Plugin.IsRefType(type) || typename.Contains("*"))
                {
                    value = "NULL";
                }
                else
                {
                    value = null;
                }
            }

            return value;
        }

        public static string GetGeneratedPropertyDefaultValue(PropertyDef prop, string typename)
        {
            return (prop != null) ? GetGeneratedDefaultValue(prop.Type, typename, prop.DefaultValue) : null;
        }

        public static void GeneratedPropertyDefaultValue(StringWriter file, string indent, PropertyDef prop)
        {
            string propType = GetGeneratedNativeType(prop.Type);
            string defaultValue = GetGeneratedDefaultValue(prop.Type, propType, prop.DefaultValue);

            if (defaultValue != null)
            {
                file.WriteLine("{0}{1} = {2};", indent, prop.BasicName, defaultValue);
            }
            else if (!string.IsNullOrEmpty(prop.DefaultValue) && Plugin.IsArrayType(prop.Type))
            {
                int index = prop.DefaultValue.IndexOf(":");
                if (index > 0)
                {
                    Type itemType = prop.Type.GetGenericArguments()[0];
                    if (!Plugin.IsArrayType(itemType) && !Plugin.IsCustomClassType(itemType))
                    {
                        string itemTypename = GetGeneratedNativeType(itemType);
                        string[] items = prop.DefaultValue.Substring(index + 1).Split('|');
                        for (int i = 0; i < items.Length; ++i)
                        {
                            string defaultItemValue = GetGeneratedDefaultValue(itemType, itemTypename, items[i]);
                            file.WriteLine("{0}{1}.push_back({2});", indent, prop.BasicName, defaultItemValue);
                        }
                    }
                }
            }
        }

        public static string GetPropertyBasicName(Behaviac.Design.PropertyDef property, MethodDef.Param arrayIndexElement)
        {
            if (property != null)
            {
                string propName = property.BasicName;

                if (property.IsArrayElement && arrayIndexElement != null)
                {
                    propName = propName.Replace("[]", "");
                }

                return propName;
            }

            return "";
        }

        public static string GetPropertyNativeType(Behaviac.Design.PropertyDef property, MethodDef.Param arrayIndexElement)
        {
            string nativeType = DataCppExporter.GetGeneratedNativeType(property.NativeType);

            return nativeType;
        }

        public static bool IsPtr(string typeName)
        {
            typeName = DataCppExporter.GetBasicGeneratedNativeType(typeName);
            return (typeName.Contains("*") && typeName != "char*" && typeName != "char *");
        }

        public static bool IsAgentPtr(string typeName)
        {
            if (DataCppExporter.IsPtr(typeName))
            {
                typeName = DataCppExporter.GetBasicGeneratedNativeType(typeName);
                typeName = typeName.Replace("*", "");
                typeName = typeName.Trim();

                return Plugin.IsAgentDerived(typeName, "Agent");
            }

            return false;
        }

        /// <summary>
        /// Generate the native code for the given value object.
        /// </summary>
        /// <param name="obj">The given object.</param>
        /// <param name="stream">The file stream for generating the codes.</param>
        /// <param name="indent">The indent string when generating the line of codes.</param>
        /// <param name="typename">The native type of the variable.</param>
        /// <param name="var">The variable for the given object when generating the codes.</param>
        /// <param name="caller">The caller for the method or the agent.</param>
        /// <returns>Returns the string generated value.</returns>
        public static string GenerateCode(object obj, DefaultObject defaultObj, StringWriter stream, string indent, string typename, string var, string caller)
        {
            string retStr = string.Empty;

            if (obj != null)
            {
                Type type = obj.GetType();

                if (obj is Behaviac.Design.MethodDef)
                {
                    Behaviac.Design.MethodDef method = obj as Behaviac.Design.MethodDef;
                    retStr = MethodCppExporter.GenerateCode(defaultObj, method, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.MethodDef.Param)
                {
                    Behaviac.Design.MethodDef.Param param = obj as Behaviac.Design.MethodDef.Param;
                    retStr = ParameterCppExporter.GenerateCode(defaultObj, param, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.ParInfo)
                {
                    Behaviac.Design.ParInfo par = obj as Behaviac.Design.ParInfo;
                    retStr = ParInfoCppExporter.GenerateCode(par, false, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.PropertyDef)
                {
                    Behaviac.Design.PropertyDef property = obj as Behaviac.Design.PropertyDef;
                    retStr = PropertyCppExporter.GenerateCode(defaultObj, property, null, false, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.VariableDef)
                {
                    Behaviac.Design.VariableDef variable = obj as Behaviac.Design.VariableDef;
                    retStr = VariableCppExporter.GenerateCode(defaultObj, variable, false, stream, indent, typename, var, caller);
                }
                else if (obj is Behaviac.Design.RightValueDef)
                {
                    Behaviac.Design.RightValueDef rightValue = obj as Behaviac.Design.RightValueDef;
                    retStr = RightValueCppExporter.GenerateCode(defaultObj, rightValue, stream, indent, typename, var, caller);
                }
                // Array type
                else if (Plugin.IsArrayType(type))
                {
                    retStr = var;

                    if (!string.IsNullOrEmpty(typename))
                    {
                        stream.WriteLine("{0}{1} {2};", indent, DataCppExporter.GetBasicGeneratedNativeType(typename), var);
                    }
                    else
                    {
                        typename = DataCppExporter.GetGeneratedNativeType(type);
                    }

                    int startIndex = typename.IndexOf('<');
                    int endIndex = typename.LastIndexOf('>');
                    string itemType = typename.Substring(startIndex + 1, endIndex - startIndex - 1);

                    ArrayCppExporter.GenerateCode(obj, defaultObj, stream, indent, itemType, var);
                }
                // Struct type
                else if (Plugin.IsCustomClassType(type))
                {
                    retStr = var;

                    if (!string.IsNullOrEmpty(typename))
                    {
                        if (typename.Contains("*"))
                        {
                            stream.WriteLine("{0}{1} {2} = NULL;", indent, DataCppExporter.GetBasicGeneratedNativeType(typename), var);
                        }
                        else
                        {
                            stream.WriteLine("{0}{1} {2};", indent, DataCppExporter.GetBasicGeneratedNativeType(typename), var);
                        }
                    }

                    StructCppExporter.GenerateCode(obj, defaultObj, stream, indent, var, null, "");
                }
                // Other types
                else
                {
                    retStr = obj.ToString();

                    if (Plugin.IsStringType(type)) // string
                    {
                        retStr = string.Format("\"{0}\"", retStr);

                        if (typename.StartsWith("behaviac::wstring"))
                        {
                            retStr = string.Format("StringUtils::MBSToWCS({0})", retStr);
                        }
                        else
                        {
                            retStr = string.Format("(char*)({0})", retStr);
                        }
                    }
                    else if (Plugin.IsCharType(type)) // char
                    {
                        retStr = string.Format("'{0}'", retStr);
                    }
                    else if (Plugin.IsBooleanType(type)) // bool
                    {
                        retStr = retStr.ToLowerInvariant();
                    }
                    else if (Plugin.IsEnumType(type)) // enum
                    {
                        retStr = EnumCppExporter.GeneratedCode(obj);
                    }
                    else if (Plugin.IsFloatType(type)) // float
                    {
                        if (retStr.Contains(".") && !retStr.EndsWith("f") && !retStr.EndsWith("F"))
                        {
                            retStr = retStr + "f";
                        }
                    }

                    if (!string.IsNullOrEmpty(var))
                    {
                        if (string.IsNullOrEmpty(typename))
                        {
                            stream.WriteLine("{0}{1} = {2};", indent, var, retStr);
                        }
                        else
                        {
                            typename = DataCppExporter.GetGeneratedNativeType(typename);

                            stream.WriteLine("{0}{1} {2} = {3};", indent, typename, var, retStr);
                        }
                    }
                }
            }

            return retStr;
        }
    }
}
