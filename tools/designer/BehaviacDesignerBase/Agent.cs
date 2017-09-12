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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public class Agent
    {
        public Agent()
        {
        }

        [Behaviac.Design.MethodDesc("behaviac::Agent", false, true, true, false, false, "int", "VectorLength", "VectorLength")]
        public delegate int VectorLength(
            [Behaviac.Design.ParamDesc("const IList&", "param0", "param0", "param0", "", false, false, false)]
            IList param0
        );

        [Behaviac.Design.MethodDesc("behaviac::Agent", false, true, true, false, false, "void", "VectorAdd", "VectorAdd")]
        public delegate void VectorAdd(
            [Behaviac.Design.ParamDesc("IList&", "param0", "param0", "param0", "", false, false, false)]
            IList param0,
            [Behaviac.Design.ParamDesc("const System::Object&", "param1", "param1", "param1", "", false, false, false)]
            System.Object param1
        );

        [Behaviac.Design.MethodDesc("behaviac::Agent", false, true, true, false, false, "void", "VectorRemove", "VectorRemove")]
        public delegate void VectorRemove(
            [Behaviac.Design.ParamDesc("IList&", "param0", "param0", "param0", "", false, false, false)]
            IList param0,
            [Behaviac.Design.ParamDesc("const System::Object&", "param1", "param1", "param1", "", false, false, false)]
            System.Object param1
        );

        [Behaviac.Design.MethodDesc("behaviac::Agent", false, true, true, false, false, "bool", "VectorContains", "VectorContains")]
        public delegate bool VectorContains(
            [Behaviac.Design.ParamDesc("IList&", "param0", "param0", "param0", "", false, false, false)]
            IList param0,
            [Behaviac.Design.ParamDesc("const System::Object&", "param1", "param1", "param1", "", false, false, false)]
            System.Object param1
        );

        [Behaviac.Design.MethodDesc("behaviac::Agent", false, true, true, false, false, "void", "VectorClear", "VectorClear")]
        public delegate void VectorClear(
            [Behaviac.Design.ParamDesc("IList&", "param0", "param0", "param0", "", false, false, false)]
            IList param0
        );

        [Behaviac.Design.MethodDesc("behaviac::Agent", false, true, true, false, false, "void", "LogMessage", "LogMessage")]
        public delegate void LogMessage(
            [Behaviac.Design.ParamDesc("const char*", "param0", "param0", "param0", "", false, false, false)]
            string param0
        );
    }

    public class AgentType
    {
        struct PropertyComparer
        {
            public static int Compare(PropertyDef x, PropertyDef y)
            {
                if (x.IsPar == y.IsPar)
                {
                    if (x.IsCustomized == y.IsCustomized)
                    {
                        if (x.IsInherited == y.IsInherited)
                        {
                            return 0;
                        }

                        if (x.IsInherited)
                        {
                            return 1;
                        }

                        return -1;
                    }

                    if (x.IsCustomized)
                    {
                        return -1;
                    }

                    return 1;
                }

                if (x.IsPar)
                {
                    return -1;
                }

                return 1;
            }
        }

        struct PropertyStrComparer : IComparer<PropertyDef>
        {
            public int Compare(PropertyDef x, PropertyDef y)
            {
                return string.Compare(x.DisplayName, y.DisplayName);
            }
        }

        struct MethodComparer
        {
            public static int Compare(MethodDef x, MethodDef y)
            {
                if (x.IsInherited == y.IsInherited)
                {
                    return 0;
                }

                if (x.IsInherited)
                {
                    return -1;
                }

                return 1;
            }
        }

        struct MethodStrComparer : IComparer<MethodDef>
        {
            public int Compare(MethodDef x, MethodDef y)
            {
                return string.Compare(x.DisplayName, y.DisplayName);
            }
        }

        private static MethodDef _nullMethod = new MethodDef(null, MemberType.Method, false, false, false, "NullAgent", "NullAgent", "null_method", "null", "null method", "void", typeof(void), false, new List<MethodDef.Param>());

        private Type _agentType;

        private List<PropertyDef> _propertyList = new List<PropertyDef>();
        private List<MethodDef> _methodsList = new List<MethodDef>();

        public AgentType(Type type, string fullname, string oldName, bool isInherited, bool isStaticClass, string displayName, string description, bool isCustomized, bool isImplemented, string exportLocation)
        {
            if (fullname == "behaviac::Agent")
            {
                isImplemented = true;
            }

            this._agentType = type;
            this._fullname = fullname;
            this._oldName = oldName;
            this._inherited = isInherited;
            this._isStatic = isStaticClass;
            this._displayName = displayName;
            this._description = description;
            this._isCustomized = isCustomized;
            this._isImplemented = isImplemented;
            this._exportLocation = exportLocation;

            this.Base = Plugin.GetAgentType(type.BaseType.Name);

            string owner = Plugin.GetInstanceNameFromClassName(fullname);
            FieldInfo[] fields = this._agentType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (FieldInfo f in fields)
            {
                Attribute[] attributes = (Attribute[])f.GetCustomAttributes(typeof(Behaviac.Design.MemberDescAttribute), false);
                string typeName = type.Name;
                bool isStatic = false;

                if (attributes.Length > 0)
                {
                    MemberDescAttribute memberDesc = (MemberDescAttribute)attributes[0];
                    typeName = memberDesc.ClassName;
                    isStatic = memberDesc.IsStatic;
                }

                string propertyDisplayName = string.Empty;
                PropertyDef pd = CreateProperty(this, f, owner, typeName, displayName, isStatic, out propertyDisplayName);

                //this.AddProperty(pd, getPropertyIndex(typeName, propertyDisplayName));
                this.AddProperty(pd);
            }

            List<Type> delegates = getHierarchyTypes(this._agentType);

            foreach (Type d in delegates)
            {
                Attribute[] attributes = (Attribute[])d.GetCustomAttributes(typeof(Behaviac.Design.MethodDescAttribute), false);
                string typeName = type.Name;

                bool isActionMethodOnly = false;
                bool isNamedEvent = false;

                if (attributes.Length > 0)
                {
                    MethodDescAttribute methodDesc = (MethodDescAttribute)attributes[0];
                    typeName = methodDesc.ClassName;
                    isNamedEvent = methodDesc.IsNamedEvent;
                    isActionMethodOnly = methodDesc.IsActionMethodOnly;
                }

                string methodDisplayName = string.Empty;
                MethodDef ad = CreateAction(this, d, owner, typeName, displayName, isNamedEvent, isActionMethodOnly, out methodDisplayName);

                //this._methodsList.Insert(getMethodIndex(typeName, methodDisplayName), ad);
                this._methodsList.Add(ad);
            }
        }

        // Customized Agent
        public AgentType(bool isCustomized, bool isImplemented, string name, string oldName, AgentType baseAgent, string exportLocation, string disp, string desc)
        {
            this._agentType = this.GetType();
            this._isCustomized = isCustomized;

            this.Reset(isImplemented, name, oldName, baseAgent, exportLocation, disp, desc);
        }

        public AgentType(AgentType other)
        {
            if (other != null)
            {
                this._agentType = other._agentType;
                this._isCustomized = other._isCustomized;

                this.Reset(other._isImplemented, other._fullname, other._oldName, other._base, other._exportLocation, other._displayName, other._description);
            }
        }

        public void Reset(bool isImplemented, string name, string oldName, AgentType baseAgent, string exportLocation, string disp, string desc)
        {
            _isImplemented = isImplemented;
            _oldName = oldName;
            _base = baseAgent;
            _exportLocation = exportLocation;
            _displayName = disp;
            _description = desc;

            setFullname(name);
        }

        private List<Type> getHierarchyTypes(Type type)
        {
            List<Type> types = new List<Type>();

            while (type != typeof(Behaviac.Design.Agent))
            {
                types.AddRange(type.GetNestedTypes(BindingFlags.Public));

                if (this.IsStatic)
                {
                    break;
                }

                type = type.BaseType;
            }

            if (type == typeof(Behaviac.Design.Agent))
            {
                Type[] intricicMethods = type.GetNestedTypes(BindingFlags.Public);

                foreach (Type t in intricicMethods)
                {
                    if (types.Find((i) => i.Name == t.Name) == null)
                    {
                        types.Add(t);
                    }
                }
            }

            return types;
        }

        public void ResetPars(IList<ParInfo> pars)
        {
            this.ClearPars();

            if (pars != null)
            {
                for (int i = 0; i < pars.Count; ++i)
                {
                    this.AddProperty(pars[i]);
                }
            }
        }

        public void ClearPars()
        {
            for (int i = _propertyList.Count - 1; i >= 0; --i)
            {
                PropertyDef p = _propertyList[i];

                if (p.IsPar)
                {
                    _propertyList.RemoveAt(i);
                }
            }
        }

        public PropertyDef GetPropertyByName(string propName)
        {
            if (!string.IsNullOrEmpty(propName))
            {
                foreach (PropertyDef prop in _propertyList)
                {
                    if (prop.BasicName.ToLowerInvariant() == propName.ToLowerInvariant())
                    {
                        return prop;
                    }
                }
            }

            return null;
        }

        public int AddProperty(PropertyDef property, int index = -1)
        {
            if (property != null)
            {
                foreach (PropertyDef prop in _propertyList)
                {
                    if (prop == property || prop.BasicName == property.BasicName)
                    {
                        return -1;
                    }
                }

                if (index < 0 && property.IsPar)
                {
                    index = getLastParIndex();
                }

                if (index < 0)
                {
                    _propertyList.Add(property);
                    index = _propertyList.Count - 1;
                }
                else
                {
                    _propertyList.Insert(index, property);
                }

                if (Plugin.IsArrayType(property.Type))
                {
                    this.AddArrayAccesorProperty(property, index);
                }

                return index;
            }

            return -1;
        }

        private int getLastParIndex()
        {
            int index = 0;

            for (int i = 0; i < _propertyList.Count; ++i)
            {
                if (_propertyList[i].IsPar)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

        private void AddArrayAccesorProperty(PropertyDef property, int index)
        {
            PropertyDef elementProperty = property.Clone();

            elementProperty.SetArrayElement(property);

            this.AddProperty(elementProperty, index);
        }

        public bool RemoveProperty(PropertyDef prop)
        {
            if (prop != null && prop.CanBeRemoved())
            {
                return this._propertyList.Remove(prop);
            }

            return false;
        }

        public bool SwapTwoProperties(PropertyDef property1, PropertyDef property2)
        {
            if (property1 != null && (property1.IsCustomized || property1.IsPar) &&
                property2 != null && (property2.IsCustomized || property2.IsPar) &&
                (property1.IsCustomized == property2.IsCustomized || property1.IsPar == property2.IsPar))
            {
                int index1 = getPropertyIndex(property1);
                int index2 = getPropertyIndex(property2);

                if (index1 >= 0 && index2 >= 0)
                {
                    this._propertyList[index1] = property2;
                    this._propertyList[index2] = property1;

                    return true;
                }
            }

            return false;
        }

        private int getPropertyIndex(PropertyDef property)
        {
            if (property != null)
            {
                for (int i = 0; i < this._propertyList.Count; ++i)
                {
                    if (property == this._propertyList[i])
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private int getPropertyIndex(string typeName, string propertyDisplayName)
        {
            int typeIndex = this._propertyList.Count;

            for (int i = 0; i < this._propertyList.Count; ++i)
            {
                if (this._propertyList[i].ClassName == typeName)
                {
                    typeIndex = i;
                    break;
                }
            }

            int propertyIndex = this._propertyList.Count;

            for (int i = typeIndex; i < this._propertyList.Count; ++i)
            {
                // Compare two string value with Chinese pronunciation
                if (string.Compare(propertyDisplayName, this._propertyList[i].FullDisplayName, false, CultureInfo.GetCultureInfo("zh-CN")) < 0)
                {
                    propertyIndex = i;
                    break;
                }
            }

            return propertyIndex;
        }

        public MethodDef GetMethodByName(string methodName)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                IList<MethodDef> methods = this.GetMethods();
                foreach (MethodDef method in methods)
                {
                    if (method.BasicName.ToLowerInvariant() == methodName.ToLowerInvariant())
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        public bool AddMethod(MethodDef method, bool append = true)
        {
            if (method != null)
            {
                IList<MethodDef> methods = this.GetMethods();
                foreach (MethodDef m in methods)
                {
                    if (m == method || m.BasicName == method.BasicName)
                    {
                        return false;
                    }
                }

                if (append)
                {
                    _methodsList.Add(method);
                }

                else
                {
                    _methodsList.Insert(0, method);
                }

                return true;
            }

            return false;
        }

        public bool RemoveMethod(MethodDef method)
        {
            if (method != null && method.CanBeRemoved())
            {
                return this._methodsList.Remove(method);
            }

            return false;
        }

        public bool SwapTwoMethods(MethodDef method1, MethodDef method2)
        {
            if (method1 != null && method2 != null)
            {
                int index1 = this._methodsList.FindIndex((MethodDef m) => m == method1);
                int index2 = this._methodsList.FindIndex((MethodDef m) => m == method2);

                Debug.Check(index1 > -1 && index2 > -1);

                this._methodsList[index1] = method2;
                this._methodsList[index2] = method1;

                return true;
            }

            return false;
        }

        private int getMethodIndex(string typeName, string methodDisplayName)
        {
            int typeIndex = this._methodsList.Count;

            for (int i = 0; i < this._methodsList.Count; ++i)
            {
                if (this._methodsList[i].ClassName == typeName)
                {
                    typeIndex = i;
                    break;
                }
            }

            int methodIndex = this._methodsList.Count;

            for (int i = typeIndex; i < this._methodsList.Count; ++i)
            {
                // Compare two string value with Chinese pronunciation
                if (string.Compare(methodDisplayName, this._methodsList[i].FullDisplayName, false, CultureInfo.GetCultureInfo("zh-CN")) < 0)
                {
                    methodIndex = i;
                    break;
                }
            }

            return methodIndex;
        }

        private AgentType _base;
        public AgentType Base
        {
            get
            {
                return _base;
            }
            set
            {
                _base = value;
            }
        }

        public Type Type
        {
            get
            {
                return this._agentType;
            }
        }

        private bool _isImplemented = false;
        public bool IsImplemented
        {
            get
            {
                return _isImplemented;
            }
            set
            {
                _isImplemented = value;
            }
        }

        private bool _isCustomized = false;
        public bool IsCustomized
        {
            //get { return this._agentType == this.GetType(); }
            get
            {
                return _isCustomized;
            }
            set
            {
                _isCustomized = value;
            }
        }

        private void setFullname(string fullname)
        {
            fullname = fullname.Replace(".", "::");

            if (this._fullname != fullname)
            {
                foreach (PropertyDef prop in this._propertyList)
                {
                    if (prop.ClassName == this._fullname)
                    {
                        prop.ClassName = fullname;
                        prop.Name = prop.ClassName + "::" + prop.BasicName;
                    }
                }

                foreach (MethodDef method in this._methodsList)
                {
                    if (method.ClassName == this._fullname)
                    {
                        method.ClassName = fullname;
                        method.Name = method.ClassName + "::" + method.BasicName;
                    }
                }

                this._fullname = fullname;
            }
        }

        private string _fullname = "";
        public string Name
        {
            get
            {
                this._fullname = this._fullname.Replace(".", "::");
                return this._fullname;
            }
            set
            {
                setFullname(value);
            }
        }

        private string _oldName = "";
        public string OldName
        {
            get
            {
                return this._oldName;
            }
            set
            {
                this._oldName = value;
            }
        }

        public string Namespace
        {
            get
            {
                if (!string.IsNullOrEmpty(this._fullname))
                {
                    int index = this._fullname.LastIndexOf(":");

                    if (index > 1)
                    {
                        return this._fullname.Substring(0, index - 1);
                    }
                }

                return string.Empty;
            }
        }

        private string GetBasicName(string fullname)
        {
            if (!string.IsNullOrEmpty(fullname))
            {
                int index = fullname.LastIndexOf(":");

                if (index > -1)
                {
                    return fullname.Substring(index + 1, fullname.Length - index - 1);
                }
            }

            return fullname;
        }

        public string BasicName
        {
            get
            {
                return GetBasicName(this._fullname);
            }
        }

        public string BasicOldName
        {
            get
            {
                return GetBasicName(this._oldName);
            }
        }

        private string _exportLocation = "";
        public string ExportLocation
        {
            get
            {
                if (!string.IsNullOrEmpty(_exportLocation))
                {
                    _exportLocation = _exportLocation.Replace("\\\\", "/");
                    _exportLocation = _exportLocation.Replace("\\", "/");
                }

                return _exportLocation;
            }
            set
            {
                _exportLocation = value;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        private bool _inherited = false;
        public bool IsInherited
        {
            get
            {
                return this._inherited;
            }
        }

        private bool _isStatic = false;
        public bool IsStatic
        {
            get
            {
                return this._isStatic;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(this._displayName) ? this.Name : this._displayName;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return this._description;
            }
        }

        private bool findProperty(List<PropertyDef> properties, string propertyName)
        {
            foreach (PropertyDef property in properties)
            {
                if (property.BasicName == propertyName)
                {
                    return true;
                }
            }

            return false;
        }

        private void GroupProperties(List<PropertyDef> properties)
        {
            List<PropertyDef> parProperties = new List<PropertyDef>();
            List<PropertyDef> customizedProperties = new List<PropertyDef>();
            List<PropertyDef> inheritedProperties = new List<PropertyDef>();
            List<PropertyDef> memberProperties = new List<PropertyDef>();

            foreach (PropertyDef prop in properties)
            {
                if (prop.IsPar)
                {
                    parProperties.Add(prop);
                }
                else if (prop.IsInherited)
                {
                    inheritedProperties.Add(prop);
                }
                else if (prop.IsCustomized)
                {
                    customizedProperties.Add(prop);
                }
                else
                {
                    memberProperties.Add(prop);
                }
            }

            properties.Clear();
            properties.AddRange(parProperties);
            properties.AddRange(customizedProperties);
            properties.AddRange(memberProperties);
            properties.AddRange(inheritedProperties);
        }

        public IList<PropertyDef> GetProperties(bool isStrSorted = false)
        {
            List<PropertyDef> properties = new List<PropertyDef>();
            AgentType baseType = this.Base;

            if (!this.IsStatic)
            {
                while (baseType != null)
                {
                    int index = properties.Count;

                    for (int i = baseType._propertyList.Count - 1; i >= 0; i--)
                    {
                        PropertyDef property = baseType._propertyList[i];

                        if (!property.IsPar && !findProperty(properties, property.BasicName))
                        {
                            PropertyDef prop = property.Clone();
                            prop.IsInherited = true;

                            properties.Insert(0, prop);
                        }
                    }

                    if (baseType == baseType.Base)
                    {
                        break;
                    }

                    baseType = baseType.Base;
                }
            }

            foreach (PropertyDef prop in _propertyList)
            {
                if (!prop.IsInherited && !findProperty(properties, prop.BasicName))
                {
                    properties.Add(prop);
                }
            }

            if (isStrSorted)
            {
                properties.Sort(new PropertyStrComparer());
            }
            else
            {
                GroupProperties(properties);
            }

            return properties.AsReadOnly();
        }

        public IList<MethodDef> GetMethods(bool isStrSorted, MethodType methodType, ValueTypes methodReturnType = ValueTypes.All, MethodDef linkedMethod = null)
        {
            IList<MethodDef> allMethods = this.GetMethods(isStrSorted);
            List<MethodDef> methods = new List<MethodDef>();

            if ((methodType & MethodType.AllowNullMethod) == MethodType.AllowNullMethod)
            {
                methods.Add(_nullMethod);
            }

            if ((methodType & MethodType.Method) == MethodType.Method)
            {
                foreach (MethodDef m in allMethods)
                {
                    if (m.MemberType == MemberType.Method)
                    {
                        if (!methods.Contains(m))
                        {
                            methods.Add(m);
                        }
                    }
                }
            }

            if ((methodType & MethodType.Task) == MethodType.Task)
            {
                foreach (MethodDef m in allMethods)
                {
                    if (m.MemberType == MemberType.Task)
                    {
                        Debug.Check(m.ReturnType == typeof(void) || m.ReturnType == typeof(bool));

                        if (!methods.Contains(m))
                        {
                            methods.Add(m);
                        }
                    }
                }
            }

            if ((methodType & MethodType.Getter) == MethodType.Getter)
            {
                foreach (MethodDef m in allMethods)
                {
                    if (m.ReturnType != typeof(void) /*&& !m.IsActionMethodOnly*/ && !m.IsNamedEvent)
                    {
                        if (methodReturnType == ValueTypes.All)
                        {
                            if (!methods.Contains(m))
                            {
                                methods.Add(m);
                            }
                        }
                        else
                        {
                            if ((methodReturnType & ValueTypes.Int) == ValueTypes.Int &&
                                Plugin.IsIntergerType(m.ReturnType))
                            {
                                if (!methods.Contains(m))
                                {
                                    methods.Add(m);
                                }
                            }
                            else if ((methodReturnType & ValueTypes.Float) == ValueTypes.Float &&
                                     Plugin.IsFloatType(m.ReturnType))
                            {
                                if (!methods.Contains(m))
                                {
                                    methods.Add(m);
                                }
                            }
                            else if ((methodReturnType & ValueTypes.Bool) == ValueTypes.Bool &&
                                     Plugin.IsBooleanType(m.ReturnType))
                            {
                                if (!methods.Contains(m))
                                {
                                    methods.Add(m);
                                }
                            }
                            else if ((methodReturnType & ValueTypes.String) == ValueTypes.String &&
                                     Plugin.IsStringType(m.ReturnType))
                            {
                                if (!methods.Contains(m))
                                {
                                    methods.Add(m);
                                }
                            }
                            else if ((methodReturnType & ValueTypes.RefType) == ValueTypes.RefType &&
                                     Plugin.IsRefType(m.ReturnType))
                            {
                                if (!methods.Contains(m))
                                {
                                    methods.Add(m);
                                }
                            }
                        }
                    }
                }//for each
            }

            if ((methodType & MethodType.Status) == MethodType.Status)
            {
                foreach (MethodDef m in allMethods)
                {
                    if (linkedMethod == null || Plugin.IsMatchedStatusMethod(linkedMethod, m))
                    {
                        if (!methods.Contains(m))
                        {
                            methods.Add(m);
                        }
                    }
                }
            }

            return methods.AsReadOnly();
        }

        private bool findMethod(List<MethodDef> methods, string methodName)
        {
            foreach (MethodDef method in methods)
            {
                if (method.BasicName == methodName)
                {
                    return true;
                }
            }

            return false;
        }

        private void GroupMethods(List<MethodDef> methods)
        {
            List<MethodDef> inheritedMethods = new List<MethodDef>();
            List<MethodDef> memberMethods = new List<MethodDef>();

            foreach (MethodDef method in methods)
            {
                if (method.IsInherited)
                {
                    inheritedMethods.Add(method);
                }
                else
                {
                    memberMethods.Add(method);
                }
            }

            methods.Clear();
            methods.AddRange(memberMethods);
            methods.AddRange(inheritedMethods);
        }

        public IList<MethodDef> GetMethods(bool isStrSorted = false)
        {
            List<MethodDef> methods = new List<MethodDef>();

            if (!this.IsStatic)
            {
                AgentType baseType = this.Base;

                while (baseType != null)
                {
                    for (int i = baseType._methodsList.Count - 1; i >= 0; i--)
                    {
                        MethodDef method = baseType._methodsList[i];

                        if (!findMethod(methods, method.BasicName))
                        {
                            MethodDef m = method.Clone() as MethodDef;
                            m.IsInherited = true;

                            methods.Insert(0, m);
                        }
                    }

                    if (baseType == baseType.Base)
                    {
                        break;
                    }

                    baseType = baseType.Base;
                }
            }

            foreach (MethodDef method in _methodsList)
            {
                if (!method.IsInherited && !findMethod(methods, method.BasicName))
                {
                    methods.Add(method);
                }
            }

            if (isStrSorted)
            {
                methods.Sort(new MethodStrComparer());
            }
            else
            {
                GroupMethods(methods);
            }

            return methods.AsReadOnly();
        }

        private static MethodInfo GetMethodInfo(Type delegateType)
        {
            MethodInfo method = delegateType.GetMethod("Invoke");

            return method;
        }

        private static Type GetMethodReturnType(MethodInfo method)
        {
            return method.ReturnType;
        }

        private static ParameterInfo[] GetMethodParams(MethodInfo method)
        {
            return method.GetParameters();
        }

        public string GetSignature(bool withProperty = false)
        {
            string str = this.Name;

            if (withProperty)
            {
                IList<PropertyDef> props = this.GetProperties(true);

                foreach (PropertyDef prop in props)
                {
                    if (!prop.IsPar && !prop.IsArrayElement)
                    {
                        str += prop.NativeItemType + prop.Name;
                    }
                }
            }

            IList<MethodDef> methods = this.GetMethods(true);

            foreach (MethodDef method in methods)
            {
                str += method.NativeReturnType + method.Name;

                foreach (MethodDef.Param param in method.Params)
                {
                    str += param.NativeType;
                }
            }

            return str;
        }

        private static MethodDef CreateAction(AgentType agentType, Type delegateType, string owner, string typeName, string typeDisplayName, bool isNamedEvent, bool isActionMethodOnly, out string displayName)
        {
            System.Reflection.MethodInfo method = AgentType.GetMethodInfo(delegateType);

            if (method != null)
            {
                System.Reflection.ParameterInfo[] parameters = AgentType.GetMethodParams(method);

                bool isChangeableType = false;
                bool isPublic = false;
                bool isStatic = false;
                string name = typeName + "::" + delegateType.Name;
                string nativeReturnType = "void";
                displayName = name;
                string description = name;
                Attribute[] attributes = (Attribute[])delegateType.GetCustomAttributes(typeof(Behaviac.Design.MethodDescAttribute), false);

                if (attributes.Length > 0)
                {
                    MethodDescAttribute methodAttr = (MethodDescAttribute)attributes[0];

                    isChangeableType = methodAttr.IsChangeableType;
                    isPublic = methodAttr.IsPublic;
                    isStatic = methodAttr.IsStatic;
                    nativeReturnType = methodAttr.NativeReturnType;
                    displayName = typeDisplayName + "::" + methodAttr.DisplayName;
                    description = methodAttr.Description;
                }

                MethodDef methodDef = new MethodDef(agentType, isNamedEvent ? MemberType.Task : MemberType.Method, isChangeableType, isPublic, isStatic,
                                                    typeName, owner, name, displayName, description, nativeReturnType, method.ReturnType, isActionMethodOnly, new List<MethodDef.Param>());

                string category = "Arguments";

                foreach (System.Reflection.ParameterInfo par in parameters)
                {
                    Attribute[] paramAttributes = (Attribute[])par.GetCustomAttributes(typeof(Behaviac.Design.ParamDescAttribute), false);
                    string paramNativeType = Plugin.GetNativeTypeName(par.ParameterType);
                    string paramName = par.Name;
                    string paramDisplayName = par.Name;
                    string paramDescription = paramDisplayName;
                    string defaultValue = "";
                    bool isOut = false;
                    bool isRef = false;
                    bool isConst = false;
                    float rangeMin = float.MinValue;
                    float rangeMax = float.MaxValue;

                    if (paramAttributes.Length > 0)
                    {
                        ParamDescAttribute paramDescAttr = ((ParamDescAttribute)paramAttributes[0]);
                        paramNativeType = paramDescAttr.NativeType;
                        paramName = paramDescAttr.Name;
                        paramDisplayName = paramDescAttr.DisplayName;
                        paramDescription = paramDescAttr.Description;
                        defaultValue = paramDescAttr.DefaultValue;
                        isOut = paramDescAttr.IsOut;
                        isRef = paramDescAttr.IsRef;
                        isConst = paramDescAttr.IsConst;
                        rangeMin = paramDescAttr.RangeMin;
                        rangeMax = paramDescAttr.RangeMax;
                    }

                    //object value = par.ParameterType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, null);
                    object value = Plugin.DefaultValue(par.ParameterType, defaultValue);
                    MethodDef.Param p = new MethodDef.Param(category, par, value, paramNativeType, paramName, paramDisplayName, paramDescription, isOut, isRef, isConst, rangeMin, rangeMax);
                    methodDef.Params.Add(p);
                }

                return methodDef;
            }

            displayName = "";

            return null;
        }

        private static PropertyDef CreateProperty(AgentType agentType, FieldInfo field, string owner, string typeName, string typeDisplayName, bool isStatic, out string displayName)
        {
            bool isChangeableType = false;
            bool isReadonly = false;
            bool isPublic = false;
            bool isProperty = false;
            string name = typeName + "::" + field.Name;
            displayName = name;
            string description = name;
            string nativeType = string.Empty;
            bool isCustomized = false;
            string defaultValue = null;
            Attribute[] attributes = (Attribute[])field.GetCustomAttributes(typeof(Behaviac.Design.MemberDescAttribute), false);

            if (attributes.Length > 0)
            {
                MemberDescAttribute memAttr = (MemberDescAttribute)attributes[0];

                isChangeableType = memAttr.IsChangeableType;
                isPublic = memAttr.IsPublic;
                isProperty = memAttr.IsProperty;
                isReadonly = memAttr.IsReadonly;
                nativeType = memAttr.NativeType;
                displayName = typeDisplayName + "::" + memAttr.DisplayName;
                description = memAttr.Description;
                isCustomized = memAttr.IsCustomized;
                defaultValue = memAttr.DefaultValue;
            }

            return new PropertyDef(agentType, field, isChangeableType, isStatic, isPublic, isProperty, isReadonly, typeName, owner, name, nativeType, displayName, description, isCustomized, defaultValue);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BehaviorNodeDescAttribute : Attribute
    {
        public BehaviorNodeDescAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClassDescAttribute : Attribute
    {
        public ClassDescAttribute(string fullname, string oldName, string baseName, bool isInherited, bool isRefType, bool isStatic, string displayName, string description, bool isCustomized, bool isImplemented, string exportLocation)
        {
            _fullname = fullname;
            _oldName = oldName;
            _baseName = baseName;
            _isInherited = isInherited;
            _isRefType = isRefType;
            _isStatic = isStatic;
            _displayName = displayName;
            _description = description;
            _isCustomized = isCustomized;
            _isImplemented = isImplemented;
            _exportLocation = exportLocation;
        }

        public ClassDescAttribute(bool isStruct, bool isRefType)
        {
            _isStruct = isStruct;
            _isRefType = isRefType;
        }

        private bool _isStruct = false;
        public bool IsStruct
        {
            get
            {
                return this._isStruct;
            }
        }

        private bool _isRefType = false;
        public bool IsRefType
        {
            get
            {
                return this._isRefType;
            }
        }

        private bool _isStatic = false;
        public bool IsStatic
        {
            get
            {
                return this._isStatic;
            }
        }

        private string _fullname = "";
        public string Fullname
        {
            get
            {
                return this._fullname;
            }
        }

        private string _oldName = "";
        public string OldName
        {
            get
            {
                return this._oldName;
            }
        }

        private string _baseName = "";
        public string BaseName
        {
            get
            {
                return _baseName;
            }
        }

        private bool _isInherited = false;
        public bool IsInherited
        {
            get
            {
                return _isInherited;
            }
        }

        private string _displayName = "";
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        private string _description = "";
        public string Description
        {
            get
            {
                return _description;
            }
        }

        private bool _isCustomized = false;
        public bool IsCustomized
        {
            get
            {
                return this._isCustomized;
            }
        }

        private bool _isImplemented = true;
        public bool IsImplemented
        {
            get
            {
                return this._isImplemented;
            }
        }

        private string _exportLocation = "";
        public string ExportLocation
        {
            get
            {
                return _exportLocation;
            }
        }
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MemberDescAttribute : Attribute
    {
        public MemberDescAttribute(string className, bool isChangeableType, bool isStatic, bool isPublic, bool isProperty, bool isReadonly,
                                   string nativeType, string displayName, string description, bool isCustomized, string defaultValue)
        {
            _className = className;
            _isChangeableType = isChangeableType;
            _isStatic = isStatic;
            _isPublic = isPublic;
            _isProperty = isProperty;
            _isReadonly = isReadonly;
            _nativeType = nativeType;
            _displayName = displayName;
            _description = description;
            _isCustomized = isCustomized;
            _defaultValue = defaultValue;
        }

        private string _className;
        public string ClassName
        {
            get
            {
                return _className;
            }
        }

        private bool _isChangeableType = false;
        public bool IsChangeableType
        {
            get
            {
                return _isChangeableType;
            }
        }

        private bool _isStatic;
        public bool IsStatic
        {
            get
            {
                return _isStatic;
            }
        }

        private bool _isPublic;
        public bool IsPublic
        {
            get
            {
                return _isPublic;
            }
        }

        private bool _isProperty;
        public bool IsProperty
        {
            get
            {
                return _isProperty;
            }
        }

        private bool _isReadonly;
        public bool IsReadonly
        {
            get
            {
                return _isReadonly;
            }
        }

        private string _nativeType;
        public string NativeType
        {
            get
            {
                return _nativeType;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
        }

        private bool _isCustomized;
        public bool IsCustomized
        {
            get
            {
                return _isCustomized;
            }
        }

        private string _defaultValue;
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }
    }


    [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MethodDescAttribute : Attribute
    {
        public MethodDescAttribute(string className, bool isChangeableType, bool isStatic, bool isPublic, bool isNamedEvent, bool isActionMethodOnly, string nativeReturnType, string displayName, string description)
        {
            _className = className;
            _isChangeableType = isChangeableType;
            _isStatic = isStatic;
            _isPublic = isPublic;
            _isNamedEvent = isNamedEvent;
            _isActionMethodOnly = isActionMethodOnly;
            _nativeReturnType = nativeReturnType;
            _displayName = displayName;
            _description = description;
        }

        private string _className;
        public string ClassName
        {
            get
            {
                return _className;
            }
        }

        private bool _isChangeableType = false;
        public bool IsChangeableType
        {
            get
            {
                return _isChangeableType;
            }
        }

        private bool _isStatic;
        public bool IsStatic
        {
            get
            {
                return _isStatic;
            }
        }

        private bool _isPublic;
        public bool IsPublic
        {
            get
            {
                return _isPublic;
            }
        }

        private bool _isNamedEvent;
        public bool IsNamedEvent
        {
            get
            {
                return _isNamedEvent;
            }
        }

        private bool _isActionMethodOnly;
        public bool IsActionMethodOnly
        {
            get
            {
                return _isActionMethodOnly;
            }
        }

        private string _nativeReturnType;
        public string NativeReturnType
        {
            get
            {
                return _nativeReturnType;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
        }
    }


    //[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    //public class EventDescAttribute : MethodDescAttribute
    //{
    //    public EventDescAttribute(string className, bool isChangeableType, bool isStatic, bool isPublic, string nativeReturnType, string displayName, string description) :
    //        base(className, isChangeableType, isStatic, isPublic, true, false, nativeReturnType, displayName, description, false)
    //    {
    //    }
    //}


    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ParamDescAttribute : Attribute
    {
        public ParamDescAttribute(string nativeType, string name, string displayName, string description, string defaultValue, bool isOut, bool isRef, bool isConst)
        {
            _nativeType = nativeType;
            _name = name;
            _displayName = displayName;
            _description = description;
            _defaultValue = defaultValue;
            _isOut = isOut;
            _isRef = isRef;
            _isConst = isConst;
            _rangeMin = float.MinValue;
            _rangeMax = float.MaxValue;
        }

        public ParamDescAttribute(string nativeType, string name, string displayName, string description, string defaultValue, bool isOut, bool isRef, bool isConst, float rangeMin, float rangeMax)
        {
            _nativeType = nativeType;
            _name = name;
            _displayName = displayName;
            _description = description;
            _defaultValue = defaultValue;
            _isOut = isOut;
            _isRef = isRef;
            _isConst = isConst;
            _rangeMin = rangeMin;
            _rangeMax = rangeMax;
        }

        private string _nativeType;
        public string NativeType
        {
            get
            {
                return _nativeType;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private bool _isOut;
        public bool IsOut
        {
            get
            {
                return _isOut;
            }
        }

        private bool _isRef;
        public bool IsRef
        {
            get
            {
                return _isRef;
            }
        }

        private bool _isConst;
        public bool IsConst
        {
            get
            {
                return _isConst;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
        }

        private string _defaultValue;
        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }

        private float _rangeMin = float.MinValue;
        public float RangeMin
        {
            get
            {
                return _rangeMin;
            }
        }

        private float _rangeMax = float.MaxValue;
        public float RangeMax
        {
            get
            {
                return _rangeMax;
            }
        }
    }


    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class EnumDescAttribute : Attribute
    {
        public EnumDescAttribute(string fullname, string displayName, string description)
        {
            _fullname = fullname;
            _displayName = displayName;
            _description = description;
        }

        public EnumDescAttribute(string fullname, bool useResources, string displayName, string description)
        {
            _fullname = fullname;

            if (useResources)
            {
                _displayName = Plugin.GetResourceString(displayName);
                _description = Plugin.GetResourceString(description);

            }
            else
            {
                _displayName = displayName;
                _description = description;
            }
        }

        private string _fullname = "";
        public string Fullname
        {
            get
            {
                return this._fullname;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
        }
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EnumMemberDescAttribute : Attribute
    {
        public EnumMemberDescAttribute(string nativeValue)
        {
            _nativeValue = nativeValue;
            _displayName = _nativeValue;
            _description = _displayName;
        }

        public EnumMemberDescAttribute(string nativeValue, string displayName)
        {
            _nativeValue = nativeValue;
            _displayName = displayName;
            _description = _displayName;
        }

        public EnumMemberDescAttribute(string nativeValue, string displayName, string description)
        {
            _nativeValue = nativeValue;
            _displayName = displayName;
            _description = description;
        }

        public EnumMemberDescAttribute(string nativeValue, bool useResources, string displayName, string description)
        {
            _nativeValue = nativeValue;

            if (useResources)
            {
                _displayName = Plugin.GetResourceString(displayName);
                _description = Plugin.GetResourceString(description);

            }
            else
            {
                _displayName = displayName;
                _description = description;
            }
        }

        private string _nativeValue;
        public string NativeValue
        {
            get
            {
                return _nativeValue;
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeDescAttribute : Attribute
    {
        public NodeDescAttribute(string group, NodeIcon icon = NodeIcon.Behavior)
        {
            _group = group;
            _icon = icon;
        }

        public NodeDescAttribute(string group, string imageName)
        {
            Debug.Check(!string.IsNullOrEmpty(imageName));

            _group = group;
            _imageName = imageName;
        }

        private string _group = string.Empty;
        public string Group
        {
            get
            {
                return _group;
            }
        }

        private NodeIcon _icon = NodeIcon.Behavior;
        public NodeIcon Icon
        {
            get
            {
                return _icon;
            }
        }

        private string _imageName = string.Empty;
        public string ImageName
        {
            get
            {
                return _imageName;
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TypeHandlerAttribute : Attribute
    {
        private Type type;

        public TypeHandlerAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get
            {
                return this.type;
            }
        }
    }


    [TypeHandler(typeof(bool))]
    public class BoolTypeHandler
    {
        public static object Create()
        {
            bool instance = (bool)DefaultValue("");
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            parStr = parStr.ToLowerInvariant();

            if (parStr == "true")
            {
                setter(true);

                return true;

            }
            else if (parStr == "false")
            {
                setter(false);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalBooleanValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(bool));

            return new DesignerBoolean(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                if (defaultValue.ToLower() == "true")
                {
                    return true;

                }
                else if (defaultValue.ToLower() == "false")
                {
                    return false;
                }
            }

            return false;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerBooleanEditor);
        }
    }


    [TypeHandler(typeof(int))]
    public class IntTypeHandler
    {
        public static object Create()
        {
            int instance = (int)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            int result;

            if (int.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(int));

            int rmin = rangeMin < int.MinValue ? int.MinValue : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                int result;

                if (int.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (int)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [TypeHandler(typeof(uint))]
    public class UIntTypeHandler
    {
        public static object Create()
        {
            uint instance = (uint)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            uint result;

            if (uint.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(uint));

            int rmin = rangeMin < 0 ? 0 : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                uint result;

                if (uint.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (uint)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }

    [TypeHandler(typeof(float))]
    public class FloatTypeHandler
    {
        private static float bigNum = (float)System.Math.Pow(10, 20);

        public static object Create()
        {
            float instance = (float)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            float result;

            if (float.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalFloatValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(float));

            int rmin = rangeMin < int.MinValue ? int.MinValue : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerFloat(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1.0f, 0, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                float result;

                if (float.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (float)0.0f;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }

    [TypeHandler(typeof(double))]
    public class DoubleTypeHandler
    {
        private static float bigNum = (float)System.Math.Pow(10, 20);

        public static object Create()
        {
            double instance = (double)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            double result;

            if (double.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalFloatValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(double));

            int rmin = rangeMin < int.MinValue ? int.MinValue : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerFloat(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1.0f, 0, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                double result;

                if (double.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (double)0.0f;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }

    [TypeHandler(typeof(string))]
    public class StringTypeHandler
    {
        public static object Create()
        {
            string instance = (string)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            if (!string.IsNullOrEmpty(parStr))
            {
                bool bSet = false;

                if (parStr.Length >= 2)
                {
                    if (parStr[0] == '\'' || parStr[0] == '\"')
                    {
                        string strTrimed = parStr.Substring(1, parStr.Length - 2);

                        setter(strTrimed);

                        bSet = true;
                    }
                }

                if (!bSet)
                {
                    setter(parStr);
                }

            }
            else
            {
                setter(parStr);
            }

            return true;
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(string));

            return new DesignerString(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                return defaultValue;
            }

            return "";
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerStringEditor);
        }
    }


    [TypeHandler(typeof(short))]
    public class ShortTypeHandler
    {
        public static object Create()
        {
            short instance = (short)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            short result;

            if (short.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(short));

            int rmin = rangeMin < short.MinValue ? short.MinValue : (int)rangeMin;
            int rmax = rangeMax > short.MaxValue ? short.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                short result;

                if (short.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (short)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [TypeHandler(typeof(ushort))]
    public class UShortTypeHandler
    {
        public static object Create()
        {
            ushort instance = (ushort)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            ushort result;

            if (ushort.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(ushort));

            int rmin = rangeMin < ushort.MinValue ? ushort.MinValue : (int)rangeMin;
            int rmax = rangeMax > ushort.MaxValue ? ushort.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                ushort result;

                if (ushort.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (ushort)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }

    [TypeHandler(typeof(sbyte))]
    public class SByteTypeHandler
    {
        public static object Create()
        {
            sbyte instance = (sbyte)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            sbyte result;

            if (sbyte.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(sbyte));

            int rmin = rangeMin < sbyte.MinValue ? sbyte.MinValue : (int)rangeMin;
            int rmax = rangeMax > sbyte.MaxValue ? sbyte.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                sbyte result;

                if (sbyte.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (sbyte)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [TypeHandler(typeof(byte))]
    public class USByteTypeHandler
    {
        public static object Create()
        {
            byte instance = (byte)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            byte result;

            if (byte.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(byte));

            int rmin = rangeMin < byte.MinValue ? byte.MinValue : (int)rangeMin;
            int rmax = rangeMax > byte.MaxValue ? byte.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                byte result;

                if (byte.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (byte)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [TypeHandler(typeof(long))]
    public class LongTypeHandler
    {
        public static object Create()
        {
            long instance = (long)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            long result;

            if (long.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(long));

            int rmin = rangeMin < int.MinValue ? int.MinValue : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                long result;

                if (long.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (long)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [Behaviac.Design.ClassDesc("Behaviac::Design::llong", "", "llong", true, false, false, "llong", "llong", false, true, "")]
    public class llong
    {
    }

    [TypeHandler(typeof(llong))]
    public class LLongTypeHandler
    {
        public static object Create()
        {
            long instance = (long)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            long result;

            if (long.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            int rmin = rangeMin < int.MinValue ? int.MinValue : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                long result;

                if (long.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (long)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [TypeHandler(typeof(ulong))]
    public class ULongTypeHandler
    {
        public static object Create()
        {
            ulong instance = (ulong)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            ulong result;

            if (ulong.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(type == typeof(ulong));

            int rmin = rangeMin < 0 ? 0 : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                ulong result;

                if (ulong.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (ulong)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [Behaviac.Design.ClassDesc("Behaviac::Design::ullong", "", "ullong", true, false, false, "ullong", "ullong", false, true, "")]
    public class ullong
    {
    }

    [TypeHandler(typeof(ullong))]
    public class ULLongTypeHandler
    {
        public static object Create()
        {
            ulong instance = (ulong)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            ulong result;

            if (ulong.TryParse(parStr, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
            {
                setter(result);

                return true;
            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            int rmin = rangeMin < 0 ? 0 : (int)rangeMin;
            int rmax = rangeMax > int.MaxValue ? int.MaxValue : (int)rangeMax;

            return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                ulong result;

                if (ulong.TryParse(defaultValue, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out result))
                {
                    return result;
                }
            }

            return (ulong)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerNumberEditor);
        }
    }


    [TypeHandler(typeof(char))]
    public class CharTypeHandler
    {
        public static object Create()
        {
            char instance = (char)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            char result;

            if (char.TryParse(parStr, out result))
            {
                setter(result);

                return true;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
            }
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            Debug.Check(Plugin.IsCharType(type));

            int rmin = rangeMin < char.MinValue ? char.MinValue : (int)rangeMin;
            int rmax = rangeMax > char.MaxValue ? char.MaxValue : (int)rangeMax;

            //return new DesignerInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, null, rmin, rmax, 1, null);
            return new DesignerString(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            if (!string.IsNullOrEmpty(defaultValue))
            {
                char result;

                if (char.TryParse(defaultValue, out result))
                {
                    return result;
                }
            }

            return (char)0;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerStringEditor);
        }
    }

    [TypeHandler(typeof(IList))]
    public class IListTypeHandler
    {
        public static object Create()
        {
            IList instance = (IList)DefaultValue();
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Plugin.SetValue setter)
        {
            throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, parStr));
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            return new DesignerPropertyEnum(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport, DesignerPropertyEnum.AllowStyles.ConstAttributes, "", "", ValueTypes.Array);
        }

        public static object DefaultValue(string defaultValue = "")
        {
            return null;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerPropertyEnumEditor);
        }
    }

    public class Vector2
    {
        private float _x = 0.0f;
        [DesignerFloat("X", "X", "Vector2", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        private float _y = 0.0f;
        [DesignerFloat("Y", "Y", "Vector2", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }
    }

    public class Vector3
    {
        private float _x = 0.0f;
        [DesignerFloat("X", "X", "Vector3", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        private float _y = 0.0f;
        [DesignerFloat("Y", "Y", "Vector3", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        private float _z = 0.0f;
        [DesignerFloat("Z", "Z", "Vector3", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.NoFlags)]
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }
    }

    public class Vector4
    {
        private float _x = 0.0f;
        [DesignerFloat("X", "X", "Vector4", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        private float _y = 0.0f;
        [DesignerFloat("Y", "Y", "Vector4", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        private float _z = 0.0f;
        [DesignerFloat("Z", "Z", "Vector4", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.NoFlags)]
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

        private float _w = 1.0f;
        [DesignerFloat("W", "W", "Vector4", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.NoFlags)]
        public float W
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
            }
        }
    }

    public class Quaternion
    {
        private float _x = 0.0f;
        [DesignerFloat("X", "X", "Quaternion", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        private float _y = 0.0f;
        [DesignerFloat("Y", "Y", "Quaternion", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        private float _z = 0.0f;
        [DesignerFloat("Z", "Z", "Quaternion", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.NoFlags)]
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

        private float _w = 0.0f;
        [DesignerFloat("W", "W", "Quaternion", DesignerProperty.DisplayMode.NoDisplay, 4, DesignerProperty.DesignerFlags.NoFlags)]
        public float W
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
            }
        }
    }

    public class Aabb3
    {
        private Vector3 _min = new Vector3();
        [DesignerStruct("Min", "Min", "Aabb3", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public Vector3 Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }

        private Vector3 _max = new Vector3();
        [DesignerStruct("Max", "Max", "Aabb3", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public Vector3 Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }
    }

    public class Ray3
    {
        private Vector3 _origin = new Vector3();
        [DesignerStruct("Origin", "Origin", "Ray3", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public Vector3 Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
            }
        }

        private Vector3 _dir = new Vector3();
        [DesignerStruct("Direction", "Direction", "Ray3", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public Vector3 Max
        {
            get
            {
                return _dir;
            }
            set
            {
                _dir = value;
            }
        }
    }

    public class Sphere
    {
        private Vector3 _center = new Vector3();
        [DesignerStruct("Center", "Center", "Sphere", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public Vector3 Center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
            }
        }

        private float _radius = 0.0f;
        [DesignerFloat("Radius", "Radius", "Sphere", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.NoFlags, null, 0.0f, 180.0f, 1.0f, 1, "")]
        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }
    }

    public class Angle3F
    {
        private float _x = 0.0f;
        [DesignerFloat("X", "X", "Angle3F", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        private float _y = 0.0f;
        [DesignerFloat("Y", "Y", "Angle3F", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags)]
        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        private float _z = 0.0f;
        [DesignerFloat("Z", "Z", "Angle3F", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.NoFlags)]
        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }
    }

    [Behaviac.Design.EnumDesc("behaviac::EBTStatus", "函数返回检查类型", "函数返回检查类型选择")]
    public enum EBTStatus
    {
        [Behaviac.Design.EnumMemberDesc("behaviac::BT_INVALID", "Invalid", "无效，表示不采用结果选项（ResultOption），而采用结果函数（ResultFunctor）")]
        BT_INVALID,

        [Behaviac.Design.EnumMemberDesc("behaviac::BT_SUCCESS", "Success", "成功")]
        BT_SUCCESS,

        [Behaviac.Design.EnumMemberDesc("behaviac::BT_FAILURE", "Failure", "失败")]
        BT_FAILURE,

        [Behaviac.Design.EnumMemberDesc("behaviac::BT_RUNNING", "Running", "正在运行")]
        BT_RUNNING
    }

    [Behaviac.Design.TypeHandler(typeof(EBTStatus))]
    public class EBTStatusTypeHandler
    {
        public static object Create()
        {
            EBTStatus instance = (EBTStatus)DefaultValue("");
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Behaviac.Design.Plugin.SetValue setter)
        {
            EBTStatus result = (EBTStatus)Enum.Parse(typeof(EBTStatus), parStr, true);
            setter(result);
            return true;
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            return new DesignerEnum(name, name, category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, "");
        }

        public static object DefaultValue(string defaultValue)
        {
            Array values = Enum.GetValues(typeof(EBTStatus));

            foreach (object enumVal in values)
            {
                string enumValueName = Enum.GetName(typeof(EBTStatus), enumVal);

                if (enumValueName == defaultValue)
                {
                    return enumVal;
                }
            }

            return EBTStatus.BT_INVALID;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerEnumEditor);
        }
    }
}//namespace Behaviac.Design

namespace XMLPluginBehaviac
{
    [Behaviac.Design.ClassDesc(true, true)]
    public class System_Object
    {
    }

    [Behaviac.Design.TypeHandler(typeof(System_Object))]
    public class System_ObjectTypeHandler
    {
        public static object Create()
        {
            System_Object instance = (System_Object)DefaultValue("");
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Behaviac.Design.Plugin.SetValue setter)
        {
            Behaviac.Design.DefaultObject node = parent as Behaviac.Design.DefaultObject;
            System_Object result = (System_Object)DesignerStruct.ParseStringValue(null, typeof(System_Object), paramName, parStr, node);
            setter(result);
            return true;
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            return new DesignerStruct(name, name, category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags);
        }

        public static object DefaultValue(string defaultValue)
        {
            return new System_Object();
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerCompositeEditor);
        }
    }

    [Behaviac.Design.EnumDesc("behaviac::EBTStatus", "函数返回检查类型", "函数返回检查类型选择")]
    public enum behaviac_EBTStatus
    {
        [Behaviac.Design.EnumMemberDesc("behaviac::BT_INVALID", "Invalid", "无效，表示不采用结果选项（ResultOption），而采用结果函数（ResultFunctor）")]
        BT_INVALID,

        [Behaviac.Design.EnumMemberDesc("behaviac::BT_SUCCESS", "Success", "成功")]
        BT_SUCCESS,

        [Behaviac.Design.EnumMemberDesc("behaviac::BT_FAILURE", "Failure", "失败")]
        BT_FAILURE,

        [Behaviac.Design.EnumMemberDesc("behaviac::BT_RUNNING", "Running", "正在运行")]
        BT_RUNNING
    }

    [Behaviac.Design.TypeHandler(typeof(behaviac_EBTStatus))]
    public class behaviac_EBTStatusTypeHandler
    {
        public static object Create()
        {
            behaviac_EBTStatus instance = (behaviac_EBTStatus)DefaultValue("");
            return instance;
        }

        public static bool Parse(object parent, string paramName, string parStr, Behaviac.Design.Plugin.SetValue setter)
        {
            behaviac_EBTStatus result = (behaviac_EBTStatus)Enum.Parse(typeof(behaviac_EBTStatus), parStr, true);
            setter(result);
            return true;
        }

        public static DesignerProperty CreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            return new DesignerEnum(name, name, category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, "");
        }

        public static object DefaultValue(string defaultValue)
        {
            Array values = Enum.GetValues(typeof(behaviac_EBTStatus));

            foreach (object enumVal in values)
            {
                string enumValueName = Enum.GetName(typeof(behaviac_EBTStatus), enumVal);

                if (enumValueName == defaultValue)
                {
                    return enumVal;
                }
            }

            return behaviac_EBTStatus.BT_INVALID;
        }

        public static Type GetEditorType()
        {
            return typeof(DesignerEnumEditor);
        }
    }
}//namespace XMLPluginBehaviac
