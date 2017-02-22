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
using System.Globalization;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DesignerMethodEnum : DesignerProperty
    {
        private MethodType _methodType;
        public MethodType MethodType
        {
            get
            {
                return this._methodType;
            }
        }

        private ValueTypes _methodReturnType;
        public ValueTypes MethodReturnType
        {
            get
            {
                return this._methodReturnType;
            }
        }

        /// <summary>
        /// Creates a new designer attribute for handling a string value.
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        public DesignerMethodEnum(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags, MethodType methodType, ValueTypes methodReturnType = ValueTypes.All, string linkedToProperty = "")
            : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerMethodComboEnumEditor), linkedToProperty)
        {
            _methodType = methodType;
            _methodReturnType = methodReturnType;
        }

        public override string GetDisplayValue(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            MethodDef methodDef = obj as MethodDef;
            Debug.Check(methodDef != null);

            return methodDef.GetDisplayValue();
        }

        public override string GetExportValue(object owner, object obj)
        {
            if (obj == null)
            {
                return "\"\"";
            }

            MethodDef methodDef = obj as MethodDef;
            Debug.Check(methodDef != null);

            return methodDef.GetExportValue();
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (type != typeof(MethodDef))
            {
                throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            }

            Nodes.Behavior behavior = node.Behavior as Nodes.Behavior;

            if (behavior != null && behavior.AgentType != null)
            {
                MethodDef method = parseMethodString(result, node, behavior.AgentType, this.MethodType, str);

                if (method == null)
                {
                    string className = Plugin.GetClassName(str);
                    method = parseMethodString(result, node, Plugin.GetInstanceAgentType(className, behavior, null), this.MethodType, str);
                }

                return method;
            }

            return null;
        }

        public static MethodDef parseMethodString(List<Nodes.Node.ErrorCheck> result, DefaultObject node, AgentType agentType, MethodType methodType, string str)
        {
            try
            {
                if (agentType != null)
                {
                    int pos = str.IndexOf('(');

                    if (pos < 0)
                    {
                        return null;
                    }

                    string ownerName = agentType.ToString();
                    int pointIndex = str.IndexOf('.');

                    if (pointIndex > -1 && pointIndex < pos)
                    {
                        ownerName = str.Substring(0, pointIndex);
                        Nodes.Behavior behavior = node.Behavior as Nodes.Behavior;

                        if (ownerName != VariableDef.kSelf && !Plugin.IsInstanceName(ownerName, behavior))
                        {
                            throw new Exception("The instance does not exist.");
                        }

                        str = str.Substring(pointIndex + 1, str.Length - pointIndex - 1);
                        agentType = Plugin.GetInstanceAgentType(ownerName, behavior, agentType);
                        //if (agentType == node.Behavior.AgentType)
                        //    ownerName = VariableDef.kSelf;
                        pos = str.IndexOf('(');
                    }

                    IList<MethodDef> actions = agentType.GetMethods(true, methodType);
                    string actionName = str.Substring(0, pos);
                    // basic name
                    int lastIndex = actionName.LastIndexOf("::");

                    if (lastIndex >= 0)
                    {
                        actionName = actionName.Substring(lastIndex + 2, actionName.Length - lastIndex - 2);
                    }

                    foreach (MethodDef actionTypeIt in actions)
                    {
                        if (actionTypeIt.BasicName == actionName
#if BEHAVIAC_NAMESPACE_FIX
                            || actionTypeIt.Name.EndsWith(actionName)
#endif
)
                        {
                            MethodDef method = new MethodDef(actionTypeIt);
                            method.Owner = ownerName;

                            List<string> paras = parseParams(str.Substring(pos + 1, str.Length - pos - 2));
                            //Debug.Check((paras.Count == actionTypeIt.Params.Count));

                            //if (paras.Count == actionTypeIt.Params.Count)
                            {
                                for (int i = 0; i < paras.Count; ++i)
                                {
                                    if (i >= method.Params.Count)
                                    {
                                        break;
                                    }

                                    string param = paras[i];
                                    MethodDef.Param par = method.Params[i];
                                    bool bOk = parseParam(result, node, method, par, param);

                                    if (!bOk)
                                    {
                                        throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalFloatValue, str));
                                    }
                                }
                            }

                            return method;
                        }
                    }
                }

            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show(str, Resources.LoadError, System.Windows.Forms.MessageBoxButtons.OK);
                if (result != null)
                {
                    Nodes.Node n = node as Nodes.Node;
                    string label = "";

                    if (n == null)
                    {
                        Attachments.Attachment a = node as Attachments.Attachment;

                        if (a != null)
                        {
                            n = a.Node;
                            label = a.Label;
                        }
                    }
                    else
                    {
                        label = n.Label;
                    }

                    Nodes.Node.ErrorCheck error = new Nodes.Node.ErrorCheck(n, node.Id, label, Nodes.ErrorCheckLevel.Error, str);
                    result.Add(error);
                }
            }

            return null;
        }

        public static bool parseParam(List<Nodes.Node.ErrorCheck> result, DefaultObject node, MethodDef method, MethodDef.Param par, string param)
        {
            string propName = null;

            if (param[0] == '\"')
            {
                param = param.Substring(1, param.Length - 2);
            }
            else if (param[0] == '{')     //struct
            {
                //to set it as action.Method is used in the following parsing
                Nodes.Action action = node as Nodes.Action;

                if (action != null && action.Method == null)
                {
                    action.Method = method;
                }
            }
            else
            {
                string noStaticParam = param.Replace("static ", "");
                int index = noStaticParam.IndexOf(" ");

                if (index >= 0)
                {
                    propName = noStaticParam.Substring(index + 1);
                }
            }

            bool bOk = false;

            if (propName != null)
            {
                VariableDef var = setParameter(result, node, propName);

                if (var != null)
                {
                    par.Value = var;
                    bOk = true;
                }
            }
            else
            {
                bOk = Plugin.InvokeTypeParser(result, par.Type, param,
                                              (object value) => par.Value = value,
                                              node, par.Name);
            }

            return bOk;
        }

        //suppose params are seprated by ','
        static private List<string> parseParams(string tsrc)
        {
            List<string> paras = new List<string>();
            int tsrcLen = (int)tsrc.Length;
            int startIndex = 0;
            int index = 0;
            int quoteDepth = 0;

            for (; index < tsrcLen; ++index)
            {
                if (tsrc[index] == '"')
                {
                    quoteDepth++;

                    if ((quoteDepth & 0x1) == 0)
                    {
                        //closing quote
                        quoteDepth -= 2;
                        Debug.Check(quoteDepth >= 0);
                    }

                }
                else if (quoteDepth == 0 && tsrc[index] == ',')
                {
                    //skip ',' inside quotes, like "count, count"
                    int lengthTemp = index - startIndex;
                    string strTemp = tsrc.Substring(startIndex, lengthTemp);
                    paras.Add(strTemp);
                    startIndex = index + 1;
                }
            }//end for

            // the last param
            if (index > startIndex)
            {
                string strTemp = tsrc.Substring(startIndex, index - startIndex);
                paras.Add(strTemp);
            }

            return paras;
        }

        private static VariableDef createVariable(List<Nodes.Node.ErrorCheck> result, DefaultObject node, AgentType agentType, string instacneName, string propertyName)
        {
            List<string> tokens = DesignerPropertyEnum.SplitTokens(propertyName);
            Debug.Check(tokens.Count > 0);
            string arrayIndexStr = null;

            if (tokens.Count > 1)
            {
                propertyName = tokens[0] + "[]";
                arrayIndexStr = tokens[1];
            }

            Nodes.Behavior behavior = node.Behavior as Nodes.Behavior;
            agentType = Plugin.GetInstanceAgentType(instacneName, behavior, agentType);

            if (agentType != null)
            {
                IList<PropertyDef> properties = agentType.GetProperties();

                foreach (PropertyDef p in properties)
                {
                    if (p.Name == propertyName
#if BEHAVIAC_NAMESPACE_FIX
                        || p.Name.EndsWith(propertyName)
#endif
)
                    {
                        VariableDef v = new VariableDef(p, instacneName);

                        if (v != null && !string.IsNullOrEmpty(arrayIndexStr))
                        {
                            v.ArrayIndexElement = new MethodDef.Param("ArrayIndex", typeof(int), "int", "ArrayIndex", "ArrayIndex");
                            v.ArrayIndexElement.IsArrayIndex = true;
                            DesignerMethodEnum.parseParam(result, node, null, v.ArrayIndexElement, arrayIndexStr);
                        }

                        return v;
                    }
                }
            }

            return null;
        }

        public static VariableDef setParameter(List<Nodes.Node.ErrorCheck> result, DefaultObject node, string propertyName)
        {
            Nodes.Behavior behavior = node.Behavior as Nodes.Behavior;
            // Try to find the Agent property with the name.
            if (behavior != null && behavior.AgentType != null)
            {
                // Convert the Par to the Property
                if (!propertyName.Contains(".") && !propertyName.Contains(":"))
                {
                    propertyName = "Self." + behavior.AgentType.Name + "::" + propertyName;
                }

                VariableDef var = null;
                string instance = Plugin.GetInstanceName(propertyName);

                if (!string.IsNullOrEmpty(instance))
                {
                    propertyName = propertyName.Substring(instance.Length + 1, propertyName.Length - instance.Length - 1);

                    var = createVariable(result, node, behavior.AgentType, instance, propertyName);

                    if (var != null)
                    {
                        return var;
                    }
                }

                instance = "Self";
                var = createVariable(result, node, behavior.AgentType, instance, propertyName);

                if (var != null)
                {
                    return var;
                }

                // Try to find the global property with the name.
                string instacneName = Plugin.GetClassName(propertyName);

                if (!string.IsNullOrEmpty(instacneName) && Plugin.GetInstanceAgentType(instacneName, behavior, null) != null)
                {
                    var = createVariable(result, node, behavior.AgentType, instacneName, propertyName);

                    if (var != null)
                    {
                        return var;
                    }
                }
            }

            return null;
        }
    }
}
