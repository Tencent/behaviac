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
using System.Windows.Forms;

namespace Behaviac.Design.Attributes
{
    [AttributeUsage(/*AttributeTargets.Field | */AttributeTargets.Property)]
    public class DesignerStruct : DesignerProperty
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
        public DesignerStruct(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerCompositeEditor), null)
        {
        }

        public override string GetDisplayValue(object obj)
        {
            return RetrieveDisplayValue(obj, null, null);
        }

        public override string GetExportValue(object owner, object obj)
        {
            return RetrieveExportValue(obj, null, null, false);
        }

        public override string GetSaveValue(object owner, object obj)
        {
            return RetrieveExportValue(obj, null, null, true);
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (Plugin.IsCustomClassType(type))
            {
                return ParseStringValue(result, type, null, str, node);
            }
            else if (Plugin.IsArrayType(type))
            {
                return DesignerArray.ParseStringValue(result, type, str, node);
            }

            throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
        }

        public static string RetrieveDisplayValue(object obj, object parent, string paramName, int indexInArray = -1)
        {
            string str = RetrieveExportValue(obj, parent, paramName, false, indexInArray);

            if (str.Length > 25)
            {
                str = str.Substring(0, 20);
                str += "...}";
            }

            //string str = string.Empty;
            //if (obj != null)
            //{
            //    Type type = obj.GetType();
            //    Debug.Check(Plugin.IsCustomClassType(type));

            //    str = type.Name;
            //    string[] tokens = str.Split('.');
            //    str = tokens[tokens.Length - 1];
            //}

            return str;
        }

        public static string RetrieveExportValue(object obj, object parent, string paramName, bool bSave, int indexInArray = -1)
        {
            string str = "";

            Debug.Check(obj != null);

            if (obj != null)
            {
                Type type = obj.GetType();

                if (Plugin.IsRefType(type))
                {
                    return "null";
                }

                bool bStructAsBasic = Plugin.IsRegisteredTypeName(type.Name);

                //struct as basic type, like Tag::Vector3, etc.
                //these types are exported as (W=0 X=0 Y=0 Z=0)
                if (!bSave && bStructAsBasic)
                {
                    str = "(";

                }
                else
                {
                    str = "{";
                }

                if (Plugin.IsCustomClassType(type))
                {
                    MethodDef method = parent as MethodDef;

                    bool bFirst = true;

                    IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(type, null);

                    foreach (DesignerPropertyInfo property in properties)
                    {
                        if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                        {
                            if (!bSave && bStructAsBasic && !bFirst)
                            {
                                str += " ";
                            }

                            bFirst = false;

                            if (!bSave)
                            {
                                if (bStructAsBasic)
                                {
                                    str += property.Property.Name + "=";
                                }

                            }
                            else
                            {
                                str += property.Property.Name + "=";
                            }

                            object member = property.GetValue(obj);

                            Type memberType = null;

                            if (member != null)
                            {
                                memberType = member.GetType();
                            }
                            else
                            {
                                memberType = property.GetTypeFallback();
                            }

                            if (Plugin.IsArrayType(memberType))
                            {
                                str += DesignerArray.RetrieveExportValue(member);
                            }
                            else
                            {
                                if (property.Attribute is DesignerStruct)
                                {
                                    str += RetrieveExportValue(member, parent, paramName, bSave);
                                }
                                else
                                {
                                    bool bStructProperty = false;

                                    if (method != null)
                                    {
                                        MethodDef.Param param = method.GetParam(paramName, property, indexInArray);

                                        if (param != null)
                                        {
                                            bStructProperty = true;
                                            string s = param.GetExportValue(null);

                                            if (Plugin.IsStringType(param.Value.GetType()))
                                            {
                                                str += string.Format("\"{0}\"", s);

                                            }
                                            else
                                            {
                                                str += s;
                                            }
                                        }
                                    }

                                    if (!bStructProperty)
                                    {
                                        string s = property.GetExportValue(obj);

                                        if (Plugin.IsStringType(property.Property.PropertyType))
                                        {
                                            str += string.Format("\"{0}\"", s);

                                        }
                                        else
                                        {
                                            str += s;
                                        }
                                    }
                                }
                            }

                            if (!bSave && bStructAsBasic)
                            {
                            }
                            else
                            {
                                str += ";";
                            }
                        }
                    }

                }
                else
                {
                }

                if (!bSave && bStructAsBasic)
                {
                    str += ")";

                }
                else
                {
                    str += "}";
                }
            }

            return str;
        }

        public static bool IsPureConstDatum(object obj, object parent, string paramName)
        {
            Debug.Check(obj != null);

            if (obj != null)
            {
                Type type = obj.GetType();

                if (!Plugin.IsCustomClassType(type))
                {
                    return false;
                }

                MethodDef method = parent as MethodDef;
                IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(type, null);

                foreach (DesignerPropertyInfo property in properties)
                {
                    if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                    {
                        object member = property.GetValue(obj);

                        if (property.Attribute is DesignerStruct)
                        {
                            if (!IsPureConstDatum(member, parent, paramName))
                            {
                                return false;
                            }

                        }
                        else
                        {
                            if (method != null)
                            {
                                MethodDef.Param param = method.GetParam(paramName, property);

                                if (param != null)
                                {
                                    if (!param.IsPureConstDatum)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static object ParseStringValue(List<Nodes.Node.ErrorCheck> result, Type type, string paramName, string str, DefaultObject node)
        {
            Debug.Check(Plugin.IsCustomClassType(type));

            object obj = Plugin.CreateInstance(type);
            parseStringValue(result, node, obj, type, paramName, str, 0, str.Length - 1);

            return obj;
        }

        private static int getProperty(string str, int startIndex, int endIndex, out string propertyName, out string propertyValue)
        {
            propertyName = string.Empty;
            propertyValue = string.Empty;

            for (int i = startIndex; i <= endIndex; ++i)
            {
                if (str[i] == '=')
                {
                    propertyName = str.Substring(startIndex, i - startIndex);

                    int brackets = 0;

                    for (int k = i + 1; k <= endIndex; ++k)
                    {
                        if (str[k] == '{')
                        {
                            brackets++;

                        }
                        else if (str[k] == '}')
                        {
                            Debug.Check(brackets > 0);
                            brackets--;

                        }
                        else if (str[k] == ';' && brackets == 0)
                        {
                            propertyValue = str.Substring(i + 1, k - i - 1);
                            break;
                        }
                    }

                    return i + 1;
                }
            }

            return -1;
        }

        private static bool getPropertyInfo(Type type, string propertyName, out DesignerPropertyInfo p)
        {
            IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(type, null);

            foreach (DesignerPropertyInfo property in properties)
            {
                if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave) &&
                    property.Property.Name == propertyName)
                {
                    p = property;
                    return true;
                }
            }

            p = new DesignerPropertyInfo();
            //throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            return false;
        }

        private static void parseStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object obj, Type type, string paramName, string str, int startIndex, int endIndex)
        {
            string propertyName = string.Empty;
            string propertyValue = string.Empty;

            try
            {
                if (startIndex >= endIndex)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(str))
                {
                    if (startIndex < str.Length && str[startIndex] == '{')
                    {
                        startIndex++;

                        if (endIndex < str.Length && str[endIndex] == '}')
                        {
                            endIndex--;
                        }
                    }
                }

                int valueIndex = getProperty(str, startIndex, endIndex, out propertyName, out propertyValue);

                //if (propertyName == "code")
                //{
                //    Debug.Check(true);
                //}

                if (valueIndex >= 0)
                {
                    Debug.Check(!string.IsNullOrEmpty(propertyName));

                    DesignerPropertyInfo property;

                    if (getPropertyInfo(type, propertyName, out property))
                    {
                        // Primitive type
                        if (string.IsNullOrEmpty(propertyValue) || propertyValue[0] != '{')
                        {
                            MethodDef.Param parParam = null;
                            Nodes.Action action = node as Nodes.Action;

                            if (action != null)
                            {
                                MethodDef method = action.Method;

                                if (method != null)
                                {
                                    string pn = paramName;

                                    if (string.IsNullOrEmpty(paramName))
                                    {
                                        pn = propertyName;
                                    }

                                    parParam = method.GetParam(pn, type, obj, property);
                                }
                            }

                            bool bParamFromStruct = false;
                            string[] tokens = Plugin.Split(propertyValue, ' ');

                            if (tokens != null && tokens.Length > 1)
                            {
                                //par
                                if (parParam != null)
                                {
                                    int propertyNameIndex = 1;

                                    if (tokens.Length == 2)
                                    {
                                        propertyNameIndex = 1;

                                    }
                                    else if (tokens.Length == 3)
                                    {
                                        Debug.Check(tokens[0] == "static");
                                        propertyNameIndex = 2;

                                    }
                                    else
                                    {
                                        Debug.Check(false);
                                    }

                                    parParam.Value = DesignerMethodEnum.setParameter(result, node, tokens[propertyNameIndex]);
                                    bParamFromStruct = true;
                                }
                            }

                            if (!bParamFromStruct)
                            {
                                property.SetValueFromString(result, obj, propertyValue, node);

                                if (parParam != null && (parParam.Value == null || !Plugin.IsArrayType(parParam.Value.GetType())))
                                {
                                    parParam.Value = property.GetValue(obj);
                                }
                            }
                        }

                        // Struct type
                        else
                        {
                            object member = property.GetValue(obj);
                            Debug.Check(member != null);

                            if (member != null)
                            {
                                string structStr = str.Substring(valueIndex + 1, propertyValue.Length - 2);
                                parseStringValue(result, node, member, member.GetType(), paramName, structStr, 0, structStr.Length - 1);
                            }
                        }
                    }

                    // Parse next property
                    parseStringValue(result, node, obj, type, paramName, str, valueIndex + propertyValue.Length + 1, str.Length - 1);
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("{0}\n{1}:{2}", ex.Message, propertyName, propertyValue);
                MessageBox.Show(msg, Resources.LoadError, MessageBoxButtons.OK);
            }
        }
    }
}
