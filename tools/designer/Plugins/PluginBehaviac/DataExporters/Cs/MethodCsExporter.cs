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
    public class MethodCsExporter
    {
        public static void GenerateClassConstructor(DefaultObject defaultObj, MethodDef method, StringWriter stream, string indent, string var)
        {
            Debug.Check(!string.IsNullOrEmpty(var));

            string paramsName = getParamsName(var, "");

            if (!method.IsPublic)
            {
                if (method.Params.Count == 0)
                {
                    stream.WriteLine("{0}\t\t\t{1} = null;", indent, paramsName);
                }
                else
                {
                    stream.WriteLine("{0}\t\t\t{1} = new object[{2}];", indent, paramsName, method.Params.Count);
                }
            }

            for (int i = 0; i < method.Params.Count; ++i)
            {
                // const value
                if (!method.Params[i].IsProperty && !method.Params[i].IsLocalVar)
                {
                    object obj = method.Params[i].Value;

                    if (obj != null)
                    {
                        string param = getParamName(var, "", i);
                        string paramName = string.Format("{0}[{1}]", paramsName, i);

                        Type type = obj.GetType();

                        if (Plugin.IsArrayType(type))
                        {
                            string typename = DataCsExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                            int startIndex = typename.IndexOf('<');
                            int endIndex = typename.LastIndexOf('>');
                            string itemType = typename.Substring(startIndex + 1, endIndex - startIndex - 1);

                            ArrayCsExporter.GenerateCode(obj, defaultObj, stream, indent + "\t\t\t", itemType, param);

                            if (!method.IsPublic)
                            {
                                stream.WriteLine("{0}\t\t\t{1} = {2};", indent, paramName, param);
                            }
                        }
                        else if (Plugin.IsCustomClassType(type))
                        {
                            if (DesignerStruct.IsPureConstDatum(obj, method, method.Params[i].Name))
                            {
                                if (Plugin.IsRefType(type))
                                {
                                    stream.WriteLine("{0}\t\t\t{1} = null;", indent, param);
                                }

                                string paramType = DataCsExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                                StructCsExporter.GenerateCode(obj, defaultObj, stream, indent + "\t\t\t", param, paramType, null, "");

                                if (!method.IsPublic)
                                {
                                    stream.WriteLine("{0}\t\t\t{1} = {2};", indent, paramName, param);
                                }
                            }
                        }
                        else
                        {
                            string retStr = DataCsExporter.GenerateCode(obj, defaultObj, stream, string.Empty, method.Params[i].NativeType, string.Empty, string.Empty);

                            if (!method.IsPublic)
                            {
                                param = paramName;
                            }

                            stream.WriteLine("{0}\t\t\t{1} = {2};", indent, param, retStr);
                        }
                    }
                }
            }
        }

        public static void GenerateClassMember(Behaviac.Design.MethodDef method, StringWriter stream, string indent, string var)
        {
            Debug.Check(!string.IsNullOrEmpty(var));

            if (!method.IsPublic)
            {
                string paramsName = getParamsName(var, "");
                stream.WriteLine("{0}\t\tobject[] {1};", indent, paramsName);
            }

            for (int i = 0; i < method.Params.Count; ++i)
            {
                // const value
                if (!method.Params[i].IsProperty && !method.Params[i].IsLocalVar && method.Params[i].Value != null)
                {
                    Type type = method.Params[i].Value.GetType();

                    if (method.IsPublic || Plugin.IsArrayType(type) || Plugin.IsCustomClassType(type))
                    {
                        string param = getParamName(var, "", i);
                        string nativeType = DataCsExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                        stream.WriteLine("{0}\t\t{1} {2};", indent, nativeType, param);
                    }
                }
            }
        }

        public static string GenerateCode(DefaultObject defaultObj, Behaviac.Design.MethodDef method, StringWriter stream, string indent, string typename, string var, string caller)
        {
            Debug.Check(!string.IsNullOrEmpty(var) || !string.IsNullOrEmpty(caller));

            string allParams = string.Empty;
            string paramsName = getParamsName(var, caller);

            for (int i = 0; i < method.Params.Count; ++i)
            {
                string nativeType = DataCsExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                string param = string.Empty;

                if (method.IsPublic)
                {
                    param = getParamName(var, caller, i);
                }
                else
                {
                    param = string.Format("{0}[{1}]", paramsName, i);
                }

                if (method.Params[i].IsProperty || method.Params[i].IsLocalVar) // property
                {
                    if ((method.Params[i].Property != null && method.Params[i].Property.IsCustomized) || method.Params[i].IsLocalVar)
                    {
                        ParameterCsExporter.GenerateCode(defaultObj, method.Params[i], stream, indent, method.IsPublic ? nativeType : "", param, caller);
                    }
                    else
                    {
                        if (method.IsPublic)
                        {
                            param = ParameterCsExporter.GenerateCode(defaultObj, method.Params[i], stream, indent, nativeType, "", param);
                        }
                        else
                        {
                            ParameterCsExporter.GenerateCode(defaultObj, method.Params[i], stream, indent, "", param, caller);
                        }
                    }

                    VariableDef v = method.Params[i].Value as VariableDef;

                    if (v != null && v.ArrayIndexElement != null)
                    {
                        PropertyDef prop = method.Params[i].Property;

                        if (prop != null && prop.IsArrayElement)
                        {
                            ParameterCsExporter.GenerateCode(defaultObj, v.ArrayIndexElement, stream, indent, "int", param + "_index", param + caller);

                            if (string.IsNullOrEmpty(param))
                            {
                                string property = PropertyCsExporter.GetProperty(defaultObj, prop, v.ArrayIndexElement, stream, indent, param, caller);
                                param = string.Format("({0})[{1}_index]", property, param);
                            }
                            else
                            {
                                param = string.Format("{0}[{0}_index]", param);
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

                        if (Plugin.IsCustomClassType(type) && !DesignerStruct.IsPureConstDatum(obj, method, method.Params[i].Name))
                        {
                            string paramName = getParamName(var, caller, i);
                            string paramType = DataCsExporter.GetGeneratedNativeType(method.Params[i].NativeType);

                            StructCsExporter.GenerateCode(obj, defaultObj, stream, indent, paramName, paramType, method, method.Params[i].Name);

                            if (!method.IsPublic)
                            {
                                stream.WriteLine("{0}{1} = {2};", indent, param, paramName);
                            }
                        }
                    }
                }

                if (i > 0)
                {
                    allParams += ", ";
                }

                if (method.Params[i].IsRef)
                {
                    param = "ref " + param;
                }
                else if (method.Params[i].IsOut)
                {
                    param = "out " + param;
                }

                allParams += param;
            }

            string agentName = "pAgent";

            if (method.Owner != Behaviac.Design.VariableDef.kSelf && (!method.IsPublic || !method.IsStatic))
            {
                string instanceName = method.Owner.Replace("::", ".");
                agentName = "pAgent_" + caller;

                bool isGlobal = Plugin.IsInstanceName(instanceName, null);
                PropertyDef ownerProperty = null;

                if (!isGlobal)
                {
                    Debug.Check(defaultObj != null && defaultObj.Behavior != null && defaultObj.Behavior.AgentType != null);
                    if (defaultObj != null && defaultObj.Behavior != null && defaultObj.Behavior.AgentType != null)
                    {
                        ownerProperty = defaultObj.Behavior.AgentType.GetPropertyByName(instanceName);
                    }
                }

                if (isGlobal || ownerProperty == null || ownerProperty.IsCustomized || ownerProperty.IsPar) // global or customized instance
                {
                    stream.WriteLine("{0}behaviac.Agent {1} = behaviac.Utils.GetParentAgent(pAgent, \"{2}\");", indent, agentName, instanceName);
                }
                else // member instance
                {
                    string prop = "";

                    if (ownerProperty.IsPublic)
                    {
                        string className = ownerProperty.ClassName.Replace("::", ".");

                        if (ownerProperty.IsStatic)
                        {
                            prop = string.Format("{0}.{1}", className, instanceName);
                        }
                        else
                        {
                            prop = string.Format("(({0})pAgent).{1}", className, instanceName);
                        }
                    }
                    else
                    {
                        string nativeType = DataCsExporter.GetGeneratedNativeType(ownerProperty.NativeType);
                        prop = string.Format("({0})AgentMetaVisitor.GetProperty(pAgent, \"{1}\")", nativeType, instanceName);
                    }

                    stream.WriteLine("{0}behaviac.Agent {1} = {2};", indent, agentName, prop);
                }

                //stream.WriteLine("{0}Debug.Check(!System.Object.ReferenceEquals({1}, null) || Utils.IsStaticClass(\"{2}\"));", indent, agentName, instanceName);
            }

            string retStr = string.Empty;
            string nativeReturnType = DataCsExporter.GetGeneratedNativeType(method.NativeReturnType);

            if (method.IsPublic)
            {
                string className = method.ClassName.Replace("::", ".");

                if (method.IsStatic)
                {
                    retStr = string.Format("{0}.{1}({2})", className, method.BasicName, allParams);
                }
                else
                {
                    retStr = string.Format("(({0}){1}).{2}({3})", className, agentName, method.BasicName, allParams);
                }
            }
            else
            {
                retStr = string.Format("AgentMetaVisitor.ExecuteMethod({0}, \"{1}\", {2})", agentName, method.BasicName, paramsName);
                string typeConvertStr = (nativeReturnType == "void") ? string.Empty : "(" + nativeReturnType + ")";
                retStr = typeConvertStr + retStr;
            }

            if (!string.IsNullOrEmpty(var))
            {
                stream.WriteLine("{0}{1} {2} = {3};", indent, nativeReturnType, var, retStr);
            }

            return retStr;
        }

        public static void PostGenerateCode(Behaviac.Design.MethodDef method, StringWriter stream, string indent, string typename, string var, string caller)
        {
            string paramsName = getParamsName(var, caller);

            for (int i = 0; i < method.Params.Count; ++i)
            {
                if (method.Params[i].IsRef || method.Params[i].IsOut)
                {
                    object obj = method.Params[i].Value;

                    if (obj != null)
                    {
                        string nativeType = DataCsExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                        string param = string.Empty;

                        if (method.IsPublic)
                        {
                            param = getParamName(var, caller, i);
                        }
                        else
                        {
                            param = string.Format("{0}[{1}]", paramsName, i);
                        }

                        string paramName = string.Format("(({0}){1}[{2}])", nativeType, paramsName, i);

                        if (!method.Params[i].IsProperty && !method.Params[i].IsLocalVar)
                        {
                            Type type = obj.GetType();

                            if (!Plugin.IsArrayType(type) && !Plugin.IsCustomClassType(type))
                            {
                                param = paramName;
                            }
                        }
                        else
                        {
                            paramName = null;
                        }

                        ParameterCsExporter.PostGenerateCode(method.Params[i], stream, indent, nativeType, param, caller, method, paramName);
                    }
                }
            }
        }

        private static string getParamsName(string var, string caller)
        {
            Debug.Check(!string.IsNullOrEmpty(var) || !string.IsNullOrEmpty(caller));

            return (string.IsNullOrEmpty(var) ? caller : var) + "_params";
        }

        private static string getParamName(string var, string caller, int index)
        {
            Debug.Check(!string.IsNullOrEmpty(var) || !string.IsNullOrEmpty(caller));

            return (string.IsNullOrEmpty(var) ? caller : var) + "_p" + index;
        }
    }
}
