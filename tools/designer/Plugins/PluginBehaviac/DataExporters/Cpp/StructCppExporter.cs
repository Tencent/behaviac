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
    public class StructCppExporter
    {
        public static void GenerateCode(object obj, DefaultObject defaultObj, StringWriter stream, string indent, string var, object parent, string paramName)
        {
            Debug.Check(obj != null);

            if (obj != null)
            {
                Type type = obj.GetType();
                Debug.Check(Plugin.IsCustomClassType(type));

                MethodDef method = parent as MethodDef;
                IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(type);

                foreach (DesignerPropertyInfo property in properties)
                {
                    if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                    {
                        object member = property.GetValue(obj);

                        Type memberType = member.GetType();

                        if (Plugin.IsArrayType(memberType))
                        {
                            string memberNativeType = Plugin.GetNativeTypeName(memberType);
                            string nativeTypeStr = DataCppExporter.GetGeneratedNativeType(memberNativeType);
                            int startIndex = nativeTypeStr.IndexOf('<');
                            int endIndex = nativeTypeStr.LastIndexOf('>');
                            string itemType = nativeTypeStr.Substring(startIndex + 1, endIndex - startIndex - 1);

                            ArrayCppExporter.GenerateCode(member, defaultObj, stream, indent, itemType, var + "." + property.Property.Name);
                        }
                        else
                        {
                            if (property.Attribute is DesignerStruct)
                            {
                                GenerateCode(member, defaultObj, stream, indent, var + "." + property.Property.Name, parent, paramName);
                            }
                            else
                            {
                                bool bStructProperty = false;

                                if (method != null)
                                {
                                    MethodDef.Param param = method.GetParam(paramName, property);

                                    if (param != null)
                                    {
                                        bStructProperty = true;
                                        ParameterCppExporter.GenerateCode(defaultObj, param, stream, indent, string.Empty, var + "." + property.Property.Name, string.Empty);
                                    }
                                }

                                if (!bStructProperty)
                                {
                                    DataCppExporter.GenerateCode(member, defaultObj, stream, indent, string.Empty, var + "." + property.Property.Name, string.Empty);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void PostGenerateCode(object obj, StringWriter stream, string indent, string var, object parent, string paramName)
        {
            Debug.Check(obj != null);

            if (obj != null)
            {
                Type type = obj.GetType();
                Debug.Check(Plugin.IsCustomClassType(type));

                MethodDef method = parent as MethodDef;
                IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(type);

                foreach (DesignerPropertyInfo property in properties)
                {
                    if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                    {
                        object member = property.GetValue(obj);

                        if (property.Attribute is DesignerStruct)
                        {
                            PostGenerateCode(member, stream, indent, var + "." + property.Property.Name, parent, paramName);
                        }
                        else
                        {
                            if (method != null)
                            {
                                MethodDef.Param param = method.GetParam(paramName, property);

                                if (param != null)
                                {
                                    string nativeType = DataCppExporter.GetBasicGeneratedNativeType(param.NativeType);
                                    ParameterCppExporter.PostGenerateCode(param, stream, indent, nativeType, var + "." + property.Property.Name, string.Empty, method);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
