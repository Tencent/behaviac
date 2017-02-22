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
    public class ParInfoCsExporter
    {
        public static string GenerateCode(Behaviac.Design.PropertyDef property, bool isRefParam, StringWriter stream, string indent, string typename, string var, string caller)
        {
            bool shouldDefineType = true;

            if (string.IsNullOrEmpty(typename))
            {
                shouldDefineType = false;
                typename = property.NativeType;
            }
            else if (typename == "System.Object" || typename == "System.Collections.IList")
            {
                typename = property.NativeType;
            }
            else
            {
                //
            }

            typename = DataCsExporter.GetGeneratedNativeType(typename);

            if (property.IsArrayElement && !typename.StartsWith("List<"))
            {
                typename = string.Format("List<{0}>", typename);
            }

            string propBasicName = property.BasicName.Replace("[]", "");
            uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);
            string retStr = string.Format("pAgent.GetVariable<{0}>({1}u)", typename, id);

            if (!string.IsNullOrEmpty(var))
            {
                stream.WriteLine("{0}Debug.Check(behaviac.Utils.MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);

                //if (isRefParam)
                //{
                //    if (shouldDefineType)
                //        stream.WriteLine("{0}{1} {2};", indent, typename, var);

                //    Type type = property.Type;
                //    if (type != null && (type.IsValueType || Plugin.IsEnumType(type) || Plugin.IsStringType(type)))
                //    {
                //        stream.WriteLine("{0}object var_{1} = pAgent.GetVariableObject({2}u);", indent, var, id);
                //        stream.WriteLine("{0}if (var_{1} != null)", indent, var);
                //        stream.WriteLine("{0}\t{1} = ({2})var_{1};", indent, var, typename);
                //        stream.WriteLine("{0}else", indent, var);

                //        if (!Plugin.IsArrayType(property.Type) && !Plugin.IsCustomClassType(property.Type))
                //        {
                //            object defaultObj = Plugin.DefaultValue(property.Type);
                //            Debug.Check(defaultObj != null);

                //            string objStr = DesignerPropertyUtility.RetrieveExportValue(defaultObj);
                //            if (defaultObj is char)
                //            {
                //                objStr = "(char)0";
                //            }
                //            else if (Plugin.IsEnumType(defaultObj.GetType()))
                //            {
                //                objStr = string.Format("{0}.{1}", typename, objStr);
                //            }

                //            stream.WriteLine("{0}\t{1} = {2};", indent, var, objStr);
                //        }
                //        else
                //        {
                //            stream.WriteLine("{0}\t{1} = new {2}();", indent, var, DataCsExporter.GetGeneratedNativeType(property.Type));
                //        }
                //    }
                //    else
                //    {
                //        stream.WriteLine("{0}{1} = ({2})pAgent.GetVariableObject({3}u);", indent, var, typename, id);
                //    }
                //}
                //else
                {
                    if (shouldDefineType)
                    {
                        stream.WriteLine("{0}{1} {2} = {3};", indent, typename, var, retStr);
                    }
                    else
                    {
                        stream.WriteLine("{0}{1} = {2};", indent, var, retStr);
                    }
                }
            }

            return retStr;
        }

        public static void PostGenerateCode(Behaviac.Design.PropertyDef property, MethodDef.Param arrayIndexElement, StringWriter stream, string indent, string typename, string var, string caller)
        {
            if (string.IsNullOrEmpty(typename))
            {
                typename = property.NativeType;
            }

            typename = DataCsExporter.GetGeneratedNativeType(typename);

            string propBasicName = property.BasicName.Replace("[]", "");
            uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);

            stream.WriteLine("{0}Debug.Check(behaviac.Utils.MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);
            stream.WriteLine("{0}pAgent.SetVariable<{1}>(\"{2}\", {3}u, ({1}){4});", indent, typename, property.Name, id, var);
        }

        public static string GetProperty(string agentName, Behaviac.Design.PropertyDef property, MethodDef.Param arrayIndexElement, StringWriter stream, string indent)
        {
            string retStr = string.Empty;

            if (property != null)
            {
                string typename = DataCsExporter.GetGeneratedNativeType(property.NativeType);

                if (property.IsArrayElement && !typename.StartsWith("List<"))
                {
                    typename = string.Format("List<{0}>", typename);
                }

                string propBasicName = property.BasicName.Replace("[]", "");
                uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);

                stream.WriteLine("{0}Debug.Check(behaviac.Utils.MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);
                retStr = string.Format("{0}.GetVariable<{1}>({2}u)", agentName, typename, id);
            }

            return retStr;
        }
    }
}
