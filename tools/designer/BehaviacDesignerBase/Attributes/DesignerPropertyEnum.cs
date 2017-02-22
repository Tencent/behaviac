////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DesignerPropertyEnum : DesignerProperty
    {
        [Flags]
        public enum AllowStyles
        {
            None = 0,
            Const = 1,
            Par = 2,
            Self = 4,
            Instance = 8,
            SelfMethod = 16,
            InstanceMethod = 32,
            Attributes = Par | Self | Instance,
            Method = SelfMethod | InstanceMethod,
            ConstAttributes = Const | Attributes,
            AttributesMethod = Attributes | Method,
            ConstAttributesMethod = Const | Attributes | Method
        }

        private string _dependedProperty = "";
        public string DependedProperty
        {
            get
            {
                return _dependedProperty;
            }
        }

        private string _dependingProperty = "";
        public string DependingProperty
        {
            get
            {
                return _dependingProperty;
            }
        }

        private AllowStyles _styles = AllowStyles.Self;
        public bool HasStyles(AllowStyles styles)
        {
            return (_styles & styles) == styles;
        }

        private double _minValue = double.MinValue;
        public double MinValue
        {
            get
            {
                return _minValue;
            }
        }

        private double _maxValue = double.MaxValue;
        public double MaxValue
        {
            get
            {
                return _maxValue;
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
        public DesignerPropertyEnum(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags, AllowStyles styles, string dependedProperty, string dependingProperty, ValueTypes filterType = ValueTypes.All, double min = double.MinValue, double max = double.MaxValue)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerPropertyEnumEditor), null, filterType)
        {
            _styles = styles;
            _dependedProperty = dependedProperty;
            _dependingProperty = dependingProperty;
            _minValue = min;
            _maxValue = max;
        }

        public override string GetDisplayValue(object obj)
        {
            return DesignerPropertyUtility.RetrieveDisplayValue(obj, null, null);
        }

        public override string GetExportValue(object owner, object obj)
        {
            return DesignerPropertyUtility.RetrieveExportValue(obj, null, null);
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (type != typeof(VariableDef))
            {
                throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            }

            if (str.Length == 0 ||
                str.Length == 2 && str == "\"\"")
            {
                return null;
            }

            if (!str.StartsWith("const"))
            {
                return DesignerPropertyEnum.parsePropertyVar(result, node, str);

            }
            else
            {
                return this.parseConstVar(result, node, parentObject, str);
            }

            //return null;
        }

        public static List<string> SplitTokens(string str)
        {
            List<string> ret = new List<string>();
            //"int Self.AgentArrayAccessTest::ListInts[int Self.AgentArrayAccessTest::l_index]"
            int pB = 0;
            int i = 0;

            bool bBeginIndex = false;

            while (i < str.Length)
            {
                bool bFound = false;
                char c = str[i];

                if (c == ' ' && !bBeginIndex)
                {
                    bFound = true;

                }
                else if (c == '[')
                {
                    bBeginIndex = true;
                    bFound = true;

                }
                else if (c == ']')
                {
                    bBeginIndex = false;
                    bFound = true;
                }

                if (bFound)
                {
                    string strT = ReadToken(str, pB, i);
                    Debug.Check(strT.Length > 0);
                    ret.Add(strT);

                    pB = i + 1;
                }

                i++;
            }

            string t = ReadToken(str, pB, i);

            if (t.Length > 0)
            {
                ret.Add(t);
            }

            return ret;
        }

        private static string ReadToken(string str, int pB, int end)
        {
            string strT = "";
            int p = pB;

            while (p < end)
            {
                strT += str[p++];
            }

            return strT;
        }

        public static VariableDef parsePropertyVar(List<Nodes.Node.ErrorCheck> result, DefaultObject node, string str)
        {
            Debug.Check(!str.StartsWith("const"));

            List<string> tokens = SplitTokens(str);

            if (tokens.Count < 2)
            {
                return null;
            }

            string arrayIndexStr = string.Empty;
            string propertyType = string.Empty;
            string propertyName = string.Empty;

            if (tokens[0] == "static")
            {
                if (tokens.Count == 3)
                {
                    //e.g. static int Property;
                    propertyType = tokens[1];
                    propertyName = tokens[2];

                }
                else
                {
                    Debug.Check(tokens.Count == 4);
                    //e.g. static int Property[int Property1];
                    propertyType = tokens[1];
                    propertyName = tokens[2] + "[]";
                    arrayIndexStr = tokens[3];
                }

            }
            else
            {
                if (tokens.Count == 2)
                {
                    //e.g. int Property;
                    propertyType = tokens[0];
                    propertyName = tokens[1];

                }
                else
                {
                    Debug.Check(tokens.Count == 3);
                    //e.g. int Property;
                    propertyType = tokens[0];
                    propertyName = tokens[1] + "[]";
                    arrayIndexStr = tokens[2];
                }
            }

            AgentType agentType = node.Behavior.AgentType;

            // Convert the Par to the Property
            if (!propertyName.Contains(".") && !propertyName.Contains(":"))
            {
                propertyName = "Self." + agentType.Name + "::" + propertyName;
            }

            VariableDef v = null;
            Nodes.Behavior behavior = node.Behavior as Nodes.Behavior;
            int pointIndex = propertyName.IndexOf('.');

            if (pointIndex > -1)
            {
                string ownerName = propertyName.Substring(0, pointIndex);
                propertyName = propertyName.Substring(pointIndex + 1, propertyName.Length - pointIndex - 1);

                agentType = Plugin.GetInstanceAgentType(ownerName, behavior, agentType);
                string valueType = ownerName;

                v = setProperty(result, node, agentType, propertyName, arrayIndexStr, valueType);
            }
            else
            {
                string className = Plugin.GetClassName(propertyName);

                // Assume it was global type.
                if (className != null)
                {
                    v = setProperty(result, node, Plugin.GetInstanceAgentType(className, behavior, agentType), propertyName, arrayIndexStr, className);

                    if (v == null && behavior != null)
                    {
                        // Assume it was Agent type.
                        v = setProperty(result, node, behavior.AgentType, propertyName, arrayIndexStr, VariableDef.kSelf);
                    }
                }
            }

            return v;
        }

        protected VariableDef parseConstVar(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, string str)
        {
            Debug.Check(str.StartsWith("const"));

            //const Int32 1
            object propertyMemberDepended = null;
            Type objType = node.GetType();

            if (this.DependedProperty != "")
            {
                System.Reflection.PropertyInfo pi = objType.GetProperty(this.DependedProperty);

                if (pi != null)
                {
                    propertyMemberDepended = pi.GetValue(node, null);

                }
                else if (pi == null && parentObject != null)
                {
                    Type parentType = parentObject.GetType();
                    pi = parentType.GetProperty(this.DependedProperty);
                    propertyMemberDepended = pi.GetValue(parentObject, null);
                }
            }

            Type valueType = null;
            VariableDef variableDepended = propertyMemberDepended as VariableDef;

            if (variableDepended != null)
            {
                valueType = variableDepended.ValueType;

            }
            else if (propertyMemberDepended != null)
            {
                MethodDef methodDepended = propertyMemberDepended as MethodDef;

                if (methodDepended != null)
                {
                    valueType = methodDepended.ReturnType;

                }
                else
                {
                    RightValueDef varRV = propertyMemberDepended as RightValueDef;

                    if (varRV != null)
                    {
                        valueType = varRV.ValueType;
                    }
                }

            }
            else
            {
                string[] tokens = str.Split(' ');
                Debug.Check(tokens.Length >= 3);

                valueType = Plugin.GetTypeFromName(tokens[1]);
            }

            if (valueType != null)
            {
                VariableDef variable = new VariableDef(null);

                //string[] tokens = str.Split(' ');
                //Debug.Check(tokens.Length == 3);
                Debug.Check(str.StartsWith("const"));
                //skip 'const '
                int pos = str.IndexOf(' ');
                Debug.Check(pos != -1);
                pos = str.IndexOf(' ', pos + 1);
                string token = str.Substring(pos + 1);

                Plugin.InvokeTypeParser(result, valueType, token,
                                        (object value) => variable.Value = value,
                                        node);

                return variable;
            }

            return null;
        }

        protected static VariableDef setProperty(List<Nodes.Node.ErrorCheck> result, DefaultObject node, AgentType agentType, string propertyName, string arrayIndexStr, string valueType)
        {
            if (agentType != null)
            {
                // basic name
                int lastIndex = propertyName.LastIndexOf("::");

                if (lastIndex >= 0)
                {
                    propertyName = propertyName.Substring(lastIndex + 2, propertyName.Length - lastIndex - 2);
                }

                IList<PropertyDef> properties = agentType.GetProperties();

                foreach (PropertyDef p in properties)
                {
                    if (p.BasicName == propertyName
#if BEHAVIAC_NAMESPACE_FIX
                        || p.Name.EndsWith(propertyName)
#endif
                       )
                    {
                        PropertyDef prop = p.Clone();
                        prop.Owner = valueType;

                        VariableDef v = new VariableDef(prop, valueType);

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

        public override Type GetEditorType(object obj)
        {
            if (_styles == AllowStyles.Self)
            {
                return typeof(DesignerPropertyEnumEditor);
            }

            return typeof(DesignerPropertyComboEnumEditor);
        }
    }
}
