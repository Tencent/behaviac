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
    public class ParInfoCppExporter
    {
        public static string GenerateCode(Behaviac.Design.PropertyDef property, bool isRefParam, StringWriter stream, string indent, string typename, string var, string caller)
        {
            bool shouldDefineType = true;

            if (string.IsNullOrEmpty(typename))
            {
                shouldDefineType = false;
                typename = property.NativeType;
            }
            else if (typename == "System::Object")
            {
                typename = property.NativeType;
            }

            if (!typename.Contains("*") && Plugin.IsRefType(property.Type))
            {
                typename += "*";
            }

            bool isListType = (typename == "IList");
            bool isString = (typename == "char*" || typename == "const char*");

            if (isListType)
            {
                typename = property.NativeType;
            }

            typename = DataCppExporter.GetGeneratedNativeType(typename);

            if (property.IsArrayElement && !typename.StartsWith("vector<") && !typename.StartsWith("behaviac::vector<"))
            {
                typename = string.Format("vector<{0} >", typename);
            }

            string varType = typename;
            string propBasicName = property.BasicName.Replace("[]", "");
            uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);
            string retStr = string.Format("({0}&)pAgent->GetVariable<{1} >({2}u)", typename, varType, id);

            if (isString)
            {
                retStr = string.Format("(char*)((behaviac::string&)pAgent->GetVariable<behaviac::string>({0}u)).c_str()", id);
            }

            if (!string.IsNullOrEmpty(var))
            {
                stream.WriteLine("{0}BEHAVIAC_ASSERT(behaviac::MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);

                if (shouldDefineType || isRefParam)
                {
                    if (isListType)
                    {
                        typename = string.Format("TList<{0} >", property.NativeType);
                        retStr = string.Format("&({0})", retStr);
                    }
                    else if (!isString)
                    {
                        typename = typename + "&";
                    }

                    stream.WriteLine("{0}{1} {2} = {3};", indent, typename, var, retStr);
                }
                else
                {
                    stream.WriteLine("{0}{1} = {2};", indent, var, retStr);
                }
            }

            return retStr;
        }

        public static void PostGenerateCode(Behaviac.Design.PropertyDef property, StringWriter stream, string indent, string typename, string var, string caller)
        {
            string propBasicName = property.BasicName.Replace("[]", "");
            uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);
            bool isListType = (typename == "IList");
            bool isString = (typename == "char*" || typename == "const char*");

            if (isListType)
            {
                var = string.Format("*{0}.vector_", var);
            }
            else if (isString)
            {
                var = string.Format("behaviac::string({0})", var);
            }

            stream.WriteLine("{0}BEHAVIAC_ASSERT(behaviac::MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);
            stream.WriteLine("{0}pAgent->SetVariable(\"{1}\", {2}u, {3});", indent, propBasicName, id, var);
        }

        public static string GetProperty(string agentName, Behaviac.Design.PropertyDef property, MethodDef.Param arrayIndexElement, StringWriter stream, string indent)
        {
            string retStr = string.Empty;

            if (property != null)
            {
                string typename = DataCppExporter.GetGeneratedNativeType(property.NativeType);

                if (!typename.Contains("*") && Plugin.IsRefType(property.Type))
                {
                    typename += "*";
                }

                if (property.IsArrayElement && !typename.StartsWith("behaviac::vector<"))
                {
                    typename = string.Format("behaviac::vector<{0} >", typename);
                }

                string propBasicName = property.BasicName.Replace("[]", "");
                uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);

                stream.WriteLine("{0}BEHAVIAC_ASSERT(behaviac::MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);
                retStr = string.Format("({0}&){1}->GetVariable<{0} >({2}u)", typename, agentName, id);
            }

            return retStr;
        }
    }
}
