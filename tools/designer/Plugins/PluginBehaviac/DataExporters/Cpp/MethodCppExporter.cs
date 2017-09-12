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
    public class MethodCppExporter
    {
        public static void GenerateClassConstructor(DefaultObject defaultObj, Behaviac.Design.MethodDef method, StringWriter stream, string indent, string var)
        {
            for (int i = 0; i < method.Params.Count; ++i)
            {
                // const value
                if (!method.Params[i].IsProperty && !method.Params[i].IsLocalVar)
                {
                    object obj = method.Params[i].Value;

                    if (obj != null)
                    {
                        string param = var + "_p" + i;
                        Type type = obj.GetType();

                        if (Plugin.IsArrayType(type))
                        {
                            string nativeType = DataCppExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                            int startIndex = nativeType.IndexOf('<');
                            int endIndex = nativeType.LastIndexOf('>');
                            string itemType = nativeType.Substring(startIndex + 1, endIndex - startIndex - 1);

                            ArrayCppExporter.GenerateCode(obj, defaultObj, stream, indent + "\t\t\t", itemType, param);
                        }
                        else if (Plugin.IsCustomClassType(type))
                        {
                            if (Plugin.IsRefType(type))
                            {
                                stream.WriteLine("{0}\t\t\t{1} = NULL;", indent, param);
                            }
                            else if (DesignerStruct.IsPureConstDatum(obj, method, method.Params[i].Name))
                            {
                                StructCppExporter.GenerateCode(obj, defaultObj, stream, indent + "\t\t\t", param, null, "");
                            }
                        }
                        else
                        {
                            string nativeType = DataCppExporter.GetBasicGeneratedNativeType(method.Params[i].NativeType);
                            string retStr = DataCppExporter.GenerateCode(obj, defaultObj, stream, string.Empty, nativeType, string.Empty, string.Empty);
                            stream.WriteLine("{0}\t\t\t{1} = {2};", indent, param, retStr);
                        }
                    }
                }
            }
        }

        public static void GenerateClassMember(Behaviac.Design.MethodDef method, StringWriter stream, string indent, string var)
        {
            for (int i = 0; i < method.Params.Count; ++i)
            {
                // const value
                if (!method.Params[i].IsProperty && !method.Params[i].IsLocalVar)
                {
                    string basicNativeType = DataCppExporter.GetBasicGeneratedNativeType(method.Params[i].NativeType);
                    string param = var + "_p" + i;

                    stream.WriteLine("{0}\t\t{1} {2};", indent, basicNativeType, param);
                }
            }
        }

        public static string GenerateCode(DefaultObject defaultObj, Behaviac.Design.MethodDef method, StringWriter stream, string indent, string typename, string var, string caller)
        {
            string allParamTypes = string.Empty;
            string allParams = string.Empty;

            for (int i = 0; i < method.Params.Count; ++i)
            {
                string nativeType = DataCppExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                if (method.Params[i].IsConst)
                {
                    nativeType = "const " + nativeType;
                }
                string basicNativeType = DataCppExporter.GetBasicGeneratedNativeType(nativeType);
                string param = (string.IsNullOrEmpty(var) ? caller : var) + "_p" + i;

                allParamTypes += ", " + nativeType;

                if (method.Params[i].IsProperty || method.Params[i].IsLocalVar) // property
                {
                    VariableDef v = method.Params[i].Value as VariableDef;

                    if (v != null && v.ArrayIndexElement != null)
                    {
                        PropertyDef prop = method.Params[i].Property;

                        if (prop != null && prop.IsArrayElement)
                        {
                            string property = PropertyCppExporter.GetProperty(defaultObj, prop, v.ArrayIndexElement, stream, indent, param, caller);
                            string propName = prop.BasicName.Replace("[]", "");

                            ParameterCppExporter.GenerateCode(defaultObj, v.ArrayIndexElement, stream, indent, "int", param + "_index", param + caller);
                            param = string.Format("({0})[{1}_index]", property, param);
                        }
                    }
                    else
                    {
                        if ((method.Params[i].Property != null && method.Params[i].Property.IsCustomized) || method.Params[i].IsLocalVar)
                        {
                            ParameterCppExporter.GenerateCode(defaultObj, method.Params[i], stream, indent, basicNativeType, param, caller);
                        }
                        else
                        {
                            if (method.IsPublic)
                            {
                                param = ParameterCppExporter.GenerateCode(defaultObj, method.Params[i], stream, indent, basicNativeType, "", param);
                            }
                            else
                            {
                                ParameterCppExporter.GenerateCode(defaultObj, method.Params[i], stream, indent, basicNativeType, param, caller);
                            }
                        }
                    }
                }
                else // const value
                {
                    object obj = method.Params[i].Value;

                    if (obj != null)
                    {
                        Type type = obj.GetType();

                        //if (Plugin.IsArrayType(type))
                        //{
                        //    string nativeTypeStr = DataCppExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                        //    int startIndex = nativeTypeStr.IndexOf('<');
                        //    int endIndex = nativeTypeStr.LastIndexOf('>');
                        //    string itemType = nativeTypeStr.Substring(startIndex + 1, endIndex - startIndex - 1);

                        //    ArrayCppExporter.GenerateCode(obj, stream, indent, itemType, param);
                        //}
                        //else
                        if (Plugin.IsCustomClassType(type) && !DesignerStruct.IsPureConstDatum(obj, method, method.Params[i].Name))
                        {
                            StructCppExporter.GenerateCode(obj, defaultObj, stream, indent, param, method, method.Params[i].Name);
                        }
                    }
                }

                if (basicNativeType == "System::Object")
                {
                    param = "*(System::Object*)&" + param;
                }

                if (i > 0)
                {
                    allParams += ", ";
                }

                allParams += param;
            }

            string agentName = "pAgent";

            if (method.Owner != Behaviac.Design.VariableDef.kSelf)
            {
                agentName = "pAgent_" + caller;

                bool isGlobal = Plugin.IsInstanceName(method.Owner, null);
                PropertyDef ownerProperty = null;

                if (!isGlobal)
                {
                    Debug.Check(defaultObj != null && defaultObj.Behavior != null && defaultObj.Behavior.AgentType != null);
                    if (defaultObj != null && defaultObj.Behavior != null && defaultObj.Behavior.AgentType != null)
                    {
                        ownerProperty = defaultObj.Behavior.AgentType.GetPropertyByName(method.Owner);
                    }
                }

                if (isGlobal || ownerProperty == null || ownerProperty.IsCustomized || ownerProperty.IsPar) // global or customized instance
                {
                    stream.WriteLine("{0}Agent* {1} = Agent::GetInstance(pAgent, \"{2}\");", indent, agentName, method.Owner);
                }
                else // member instance
                {
                    string propName = ownerProperty.Name.Replace("::", "_");
                    string nativeType = DataCppExporter.GetGeneratedNativeType(ownerProperty.NativeType);
                    string prop = string.Format("(({0}*)pAgent)->_Get_Property_<{1}PROPERTY_TYPE_{2}, {3} >()", ownerProperty.ClassName, getNamespace(ownerProperty.ClassName), propName, nativeType);

                    stream.WriteLine("{0}Agent* {1} = {2};", indent, agentName, prop);
                }

                stream.WriteLine("{0}BEHAVIAC_ASSERT({1});", indent, agentName);
            }

            string nativeReturnType = DataCppExporter.GetGeneratedNativeType(method.NativeReturnType);

            if (method.NativeReturnType.StartsWith("const "))
            {
                nativeReturnType = "const " + nativeReturnType;
            }

            string retStr = "";

            if (method.IsPublic)
            {
                if (method.IsStatic)
                {
                    retStr = string.Format("{0}::{1}({2})", method.ClassName, method.BasicName, allParams);
                }
                else
                {
                    retStr = string.Format("(({0}*){1})->{2}({3})", method.ClassName, agentName, method.BasicName, allParams);
                }
            }
            else
            {
                retStr = string.Format("(({0}*){1})->_Execute_Method_<{2}METHOD_TYPE_{3}, {4}{5} >({6})", method.ClassName, agentName, getNamespace(method.ClassName), method.Name.Replace("::", "_"), nativeReturnType, allParamTypes, allParams);
            }

            if (!string.IsNullOrEmpty(var))
            {
                stream.WriteLine("{0}{1} {2} = {3};", indent, nativeReturnType, var, retStr);
            }

            return retStr;
        }

        public static void PostGenerateCode(Behaviac.Design.MethodDef method, StringWriter stream, string indent, string typename, string var, string caller)
        {
            for (int i = 0; i < method.Params.Count; ++i)
            {
                if (!method.Params[i].NativeType.StartsWith("const ") && method.Params[i].NativeType.EndsWith("&"))
                {
                    string param = (string.IsNullOrEmpty(var) ? caller : var) + "_p" + i;
                    string nativeType = DataCppExporter.GetBasicGeneratedNativeType(method.Params[i].NativeType);
                    ParameterCppExporter.PostGenerateCode(method.Params[i], stream, indent, nativeType, param, caller, method);
                }
            }
        }

        private static string getNamespace(string className)
        {
            if (!string.IsNullOrEmpty(className))
            {
                int index = className.LastIndexOf(":");

                if (index > 1)
                {
                    return className.Substring(0, index - 1) + "::";
                }
            }

            return string.Empty;
        }
    }
}
