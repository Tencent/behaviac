/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions and limitations under
// the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#if !BEHAVIAC_RELEASE
#define BEHAVIAC_DEBUG
#endif

// please define BEHAVIAC_NOT_USE_UNITY in your project file if you are not using unity
#if !BEHAVIAC_NOT_USE_UNITY
// if you have compiling errors complaining the following using 'UnityEngine',
//usually, you need to define BEHAVIAC_NOT_USE_UNITY in your project file
using UnityEngine;
#endif//!BEHAVIAC_NOT_USE_UNITY

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace behaviac
{
    public struct CStringID
    {
        private uint m_id;

        public CStringID(string str)
        {
            //SetId(str);
            m_id = CRC32.CalcCRC(str);
        }

        public void SetId(string str)
        {
            m_id = CRC32.CalcCRC(str);
        }

        public uint GetId()
        {
            return m_id;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to CStringID return false.
            if (obj is CStringID)
            {
                CStringID p = (CStringID)obj;

                // Return true if the fields match:
                return (m_id == p.m_id);
            }
            else if (obj is string)
            {
                CStringID p = new CStringID((string)obj);

                // Return true if the fields match:
                return (m_id == p.m_id);
            }

            return false;
        }

        public bool Equals(CStringID p)
        {
            // If parameter is null return false:
            if (((object)p) == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (m_id == p.m_id);
        }

        public override int GetHashCode()
        {
            return (int)m_id;
        }

        public static bool operator ==(CStringID a, CStringID b)
        {
            return a.m_id == b.m_id;
        }

        public static bool operator !=(CStringID a, CStringID b)
        {
            return a.m_id != b.m_id;
        }
    }

    public struct CSerializationID
    {
        public CSerializationID(string id)
        {
        }
    }

    public interface ISerializableNode
    {
        ISerializableNode newChild(CSerializationID chidlId);

        void setAttr(CSerializationID attrId, string value);

        void setAttr<VariableType>(CSerializationID attrId, VariableType value);
    }

    public struct CPropertyNode
    {
        public CPropertyNode(Agent agent, string className)
        {
        }
    }

    public class CMethodBase
    {
        private behaviac.MethodMetaInfoAttribute descAttrbute_;
        private System.Reflection.MethodBase method_;

        public CMethodBase(System.Reflection.MethodBase m, behaviac.MethodMetaInfoAttribute a, string methodNameOverride)
        {
            method_ = m;
            descAttrbute_ = a;
            this.m_variableName = !string.IsNullOrEmpty(methodNameOverride) ? methodNameOverride : method_.Name;
            m_id.SetId(this.m_variableName);
        }

        protected CMethodBase(CMethodBase copy)
        {
            this.m_variableName = copy.m_variableName;
            this.method_ = copy.method_;
            this.descAttrbute_ = copy.descAttrbute_;
            this.m_id = copy.m_id;
        }

        protected string m_variableName;

        public string Name
        {
            get
            {
                return this.m_variableName;
            }
        }

        private CStringID m_id = new CStringID();

        public CStringID GetId()
        {
            return m_id;
        }

        public virtual bool IsNamedEvent()
        {
            return false;
        }

        public virtual CMethodBase clone()
        {
            return new CMethodBase(this);
        }
    }

    public class CMemberBase
    {
        //behaviac.MemberMetaInfoAttribute descAttrbute_;
        protected string m_name;

        public CMemberBase(string name, behaviac.MemberMetaInfoAttribute a)
        {
            m_name = name;
            m_id.SetId(name);

            if (a != null)
            {
                m_range = a.Range;
            }
            else
            {
                m_range = 1.0f;
            }
        }

        public virtual Type MemberType
        {
            get
            {
                return null;
            }
        }

        public virtual bool ISSTATIC()
        {
            return false;
        }

        private float m_range = 1.0f;

        public float GetRange()
        {
            return this.m_range;
        }

        private CStringID m_id = new CStringID();

        public CStringID GetId()
        {
            return m_id;
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public virtual string GetClassNameString()
        {
            return null;
        }

        private string m_instanceName;

        public string InstanceName
        {
            get
            {
                return m_instanceName;
            }
            set
            {
                m_instanceName = value;
            }
        }

        public virtual int GetTypeId()
        {
            return 0;
        }

        public virtual void Load(Agent parent, ISerializableNode node)
        {
        }

        public virtual void Save(Agent parent, ISerializableNode node)
        {
        }

        public virtual object Get(object agentFrom)
        {
            return null;
        }

        public virtual void Set(object objectFrom, object v)
        {
        }
    }

    public class CFieldInfo : CMemberBase
    {
        protected System.Reflection.FieldInfo field_;

        public CFieldInfo(System.Reflection.FieldInfo f, behaviac.MemberMetaInfoAttribute a)
        : base(f.Name, a)
        {
            this.field_ = f;
        }

        public override Type MemberType
        {
            get
            {
                return field_.FieldType;
            }
        }

        public override bool ISSTATIC()
        {
            return this.field_.IsStatic;
        }

        public override string GetClassNameString()
        {
            return this.field_.DeclaringType.FullName;
        }

        public override object Get(object agentFrom)
        {
            if (this.ISSTATIC())
            {
                return this.field_.GetValue(null);
            }
            else
            {
                return this.field_.GetValue(agentFrom);
            }
        }

        public override void Set(object objectFrom, object v)
        {
            this.field_.SetValue(objectFrom, v);
        }
    }

    public class CProperyInfo : CMemberBase
    {
        private PropertyInfo property_;

        private static MethodInfo getGetMethod(PropertyInfo property)
        {
#if ( !UNITY_EDITOR && UNITY_METRO )
            return property.GetMethod;
#else
            return property.GetGetMethod();
#endif
        }

        private static MethodInfo getSetMethod(PropertyInfo property)
        {
#if ( !UNITY_EDITOR && UNITY_METRO )
            return property.SetMethod;
#else
            return property.GetSetMethod();
#endif
        }

        public CProperyInfo(System.Reflection.PropertyInfo p, behaviac.MemberMetaInfoAttribute a)
        : base(p.Name, a)
        {
            this.property_ = p;
        }

        public override Type MemberType
        {
            get
            {
                return this.property_.PropertyType;
            }
        }

        public override string GetClassNameString()
        {
            return this.property_.DeclaringType.FullName;
        }

        public override object Get(object agentFrom)
        {
            if (this.ISSTATIC())
            {
                var getMethod = getGetMethod(this.property_);

                if (getMethod != null)
                {
                    return getMethod.Invoke(null, null);
                }
            }
            else
            {
                var getMethod = getGetMethod(this.property_);

                if (getMethod != null)
                {
                    return getMethod.Invoke(agentFrom, null);
                }
            }

            return null;
        }

        public override void Set(object objectFrom, object v)
        {
            var setMethod = getSetMethod(this.property_);

            if (setMethod == null)
            {
            }
            else
            {
                var paramArray = new object[1];
                paramArray[0] = v;
                setMethod.Invoke(objectFrom, paramArray);
            }
        }
    }

    public class CTextNode
    {
        public CTextNode(string name)
        {
        }

        public void setAttr(string attrName, string attrValue)
        {
        }
    }

    static public class Utils
    {
        public static bool IsNull(System.Object aObj)
        {
            return aObj == null;
        }

        public static bool IsStaticType(Type type)
        {
            return type != null && type.IsAbstract && type.IsSealed;
        }

        private static Dictionary<string, bool> ms_staticClasses;

        private static Dictionary<string, bool> StaticClasses
        {
            get
            {
                if (ms_staticClasses == null)
                {
                    ms_staticClasses = new Dictionary<string, bool>();
                }

                return ms_staticClasses;
            }
        }

        public static void AddStaticClass(Type type)
        {
            if (Utils.IsStaticType(type))
            {
                Utils.StaticClasses[type.FullName] = true;
            }
        }

        public static bool IsStaticClass(string className)
        {
            return Utils.StaticClasses.ContainsKey(className);
        }

        public static Agent GetParentAgent(Agent pAgent, string instanceName)
        {
            Agent pParent = pAgent;

            if (!string.IsNullOrEmpty(instanceName) && instanceName != "Self")
            {
                pParent = Agent.GetInstance(instanceName, (pParent != null) ? pParent.GetContextId() : 0);

                if (pAgent != null && pParent == null && !Utils.IsStaticClass(instanceName))
                {
                    pParent = pAgent.GetVariable<Agent>(instanceName);

                    if (pParent == null)
                    {
                        string errorInfo = string.Format("[instance] The instance \"{0}\" can not be found, so please check the Agent.BindInstance(...) method has been called for this instance.\n", instanceName);

                        Debug.Check(false, errorInfo);

                        LogManager.Instance.Log(errorInfo);

#if !BEHAVIAC_NOT_USE_UNITY
                        UnityEngine.Debug.LogError(errorInfo);
#else
                        Console.WriteLine(errorInfo);
#endif
                    }
                }
            }

            return pParent;
        }

        public static bool IsDefault<T>(T obj)
        {
            return EqualityComparer<T>.Default.Equals(obj, default(T));
        }

        public static uint MakeVariableId(string idstring)
        {
            return CRC32.CalcCRC(idstring);
        }

        public static int GetClassTypeNumberId<T>()
        {
            return 0;
        }

        public static void ConvertFromInteger<T>(int v, ref T ret)
        {
        }

        public static uint ConvertToInteger<T>(T v)
        {
            return 0;
        }

        public static Type GetType(string typeName)
        {
            // search base class
            Type type = Type.GetType(typeName);

            if (type != null)
            {
                return type;
            }

            // search loaded plugins
            for (int i = 0; i < AppDomain.CurrentDomain.GetAssemblies().Length; ++i)
            {
                Assembly a = AppDomain.CurrentDomain.GetAssemblies()[i];
                type = a.GetType(typeName);

                if (type != null)
                {
                    return type;
                }
            }

            // it could be a List<> type
            if (typeName.StartsWith("System.Collections.Generic.List"))
            {
                int startIndex = typeName.IndexOf("[[");

                if (startIndex > -1)
                {
                    int endIndex = typeName.IndexOf(",");

                    if (endIndex < 0)
                    {
                        endIndex = typeName.IndexOf("]]");
                    }

                    if (endIndex > startIndex)
                    {
                        string item = typeName.Substring(startIndex + 2, endIndex - startIndex - 2);
                        type = Utils.GetType(item);

                        if (type != null)
                        {
                            type = typeof(List<>).MakeGenericType(type);
                            return type;
                        }
                    }
                }
            }

            return null;
        }

        private static Dictionary<string, string> ms_type_mapping = new Dictionary<string, string>()
        {
            {"Boolean"          , "bool"},
            {"System.Boolean"   , "bool"},
            {"Int32"            , "int"},
            {"System.Int32"     , "int"},
            {"UInt32"           , "uint"},
            {"System.UInt32"    , "uint"},
            {"Int16"            , "short"},
            {"System.Int16"     , "short"},
            {"UInt16"           , "ushort"},
            {"System.UInt16"    , "ushort"},
            {"Int8"             , "sbyte"},
            {"System.Int8"      , "sbyte"},
            {"SByte"            , "sbyte"},
            {"System.SByte"     , "sbyte"},
            {"UInt8"            , "ubyte"},
            {"System.UInt8"     , "ubyte"},
            {"Byte"             , "ubyte"},
            {"System.Byte"      , "ubyte"},
            {"Char"      		, "char"},
            {"System.Char"      , "char"},
            {"Int64"            , "long"},
            {"System.Int64"     , "long"},
            {"UInt64"           , "ulong"},
            {"System.UInt64"    , "ulong"},
            {"Single"           , "float"},
            {"System.Single"    , "float"},
            {"Double"           , "double"},
            {"System.Double"    , "double"},
            {"String"           , "string"},
            {"System.String"    , "string"},
            {"Void"             , "void"},
            {"System.Void"      , "void"}
        };

        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }

            return null;
        }

        public static object FromStringPrimitive(Type type, string valueStr)
        {
            if (valueStr != null)
            {
                if (type == typeof(string) && !string.IsNullOrEmpty(valueStr) &&
                    valueStr.Length > 1 && valueStr[0] == '\"' && valueStr[valueStr.Length - 1] == '\"')
                {
                    valueStr = valueStr.Substring(1, valueStr.Length - 2);
                }

                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    return converter.ConvertFromString(valueStr);
                }
                catch
                {
                    if (type == typeof(bool))
                    {
                        bool b;

                        if (bool.TryParse(valueStr, out b))
                        {
                            return b;
                        }
                    }
                    else if (type == typeof(int))
                    {
                        int i;

                        if (int.TryParse(valueStr, out i))
                        {
                            return i;
                        }
                    }
                    else if (type == typeof(uint))
                    {
                        uint ui;

                        if (uint.TryParse(valueStr, out ui))
                        {
                            return ui;
                        }
                    }
                    else if (type == typeof(short))
                    {
                        short s;

                        if (short.TryParse(valueStr, out s))
                        {
                            return s;
                        }
                    }
                    else if (type == typeof(ushort))
                    {
                        ushort us;

                        if (ushort.TryParse(valueStr, out us))
                        {
                            return us;
                        }
                    }
                    else if (type == typeof(char))
                    {
                        char c;

                        if (char.TryParse(valueStr, out c))
                        {
                            return c;
                        }
                    }
                    else if (type == typeof(sbyte))
                    {
                        sbyte sb;

                        if (sbyte.TryParse(valueStr, out sb))
                        {
                            return sb;
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        byte b;

                        if (byte.TryParse(valueStr, out b))
                        {
                            return b;
                        }
                    }
                    else if (type == typeof(long))
                    {
                        long l;

                        if (long.TryParse(valueStr, out l))
                        {
                            return l;
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        ulong ul;

                        if (ulong.TryParse(valueStr, out ul))
                        {
                            return ul;
                        }
                    }
                    else if (type == typeof(float))
                    {
                        float f;

                        if (float.TryParse(valueStr, out f))
                        {
                            return f;
                        }
                    }
                    else if (type == typeof(double))
                    {
                        double d;

                        if (double.TryParse(valueStr, out d))
                        {
                            return d;
                        }
                    }
                    else if (type == typeof(string))
                    {
                        return valueStr;
                    }
                    else if (type.IsEnum)
                    {
                        object ret = Enum.Parse(type, valueStr, true);

                        return ret;
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
            }

            return GetDefaultValue(type);
        }

        public static Type GetPrimitiveTypeFromName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            switch (typeName)
            {
                case "bool":
                case "Boolean":
                    return typeof(bool);

                case "int":
                case "Int32":
                    return typeof(int);

                case "uint":
                case "UInt32":
                    return typeof(uint);

                case "short":
                case "Int16":
                    return typeof(short);

                case "ushort":
                case "UInt16":
                    return typeof(ushort);

                case "char":
                case "Char":
                    return typeof(char);

                case "sbyte":
                case "SByte":
                    return typeof(sbyte);

                case "ubyte":
                case "Ubyte":
                case "byte":
                case "Byte":
                    return typeof(byte);

                case "long":
                case "llong":
                case "Int64":
                    return typeof(long);

                case "ulong":
                case "ullong":
                case "UInt64":
                    return typeof(ulong);

                case "float":
                case "Single":
                    return typeof(float);

                case "double":
                case "Double":
                    return typeof(double);

                case "string":
                case "String":
                    return typeof(string);
            }

            return Utils.GetType(typeName);
        }

        public static Type GetElementTypeFromName(string typeName)
        {
            bool bArrayType = false;

            //array type
            if (typeName.StartsWith("vector<"))
            {
                bArrayType = true;
            }

            if (bArrayType)
            {
                int bracket0 = typeName.IndexOf('<');
                int bracket1 = typeName.IndexOf('>');
                int len = bracket1 - bracket0 - 1;

                string elementTypeName = typeName.Substring(bracket0 + 1, len);
                Type elementType = Utils.GetTypeFromName(elementTypeName);

                return elementType;
            }

            return null;
        }

        public static Type GetTypeFromName(string typeName)
        {
            if (typeName == "void*")
            {
                return typeof(Agent);
            }

            //Type type = Agent.GetTypeFromName(typeName);
            Type type = AgentMeta.GetTypeFromName(typeName);

            if (type == null)
            {
                type = Utils.GetPrimitiveTypeFromName(typeName);

                if (type == null)
                {
                    Type elementType = Utils.GetElementTypeFromName(typeName);

                    if (elementType != null)
                    {
                        Type vectorType = typeof(List<>).MakeGenericType(elementType);
                        return vectorType;
                    }
                    else
                    {
                        typeName = typeName.Replace("::", ".");
                        type = Utils.GetType(typeName);
                    }
                }
            }

            return type;
        }

        //if it is an array, return the element type
        public static Type GetTypeFromName(string typeName, ref bool bIsArrayType)
        {
            Type elementType = Utils.GetElementTypeFromName(typeName);

            if (elementType != null)
            {
                bIsArrayType = true;
                return elementType;
            }

            Type type = Utils.GetTypeFromName(typeName);

            return type;
        }

        public static string GetNativeTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return string.Empty;
            }

            if (typeName.StartsWith("vector<"))
            {
                return typeName;
            }

            bool bRef = false;
            string key;

            if (typeName.EndsWith("&"))
            {
                //trim the last '&'
                key = typeName.Substring(0, typeName.Length - 1);
                bRef = true;
            }
            else
            {
                key = typeName;
            }

            if (ms_type_mapping.ContainsKey(key))
            {
                if (bRef)
                {
                    return ms_type_mapping[key] + "&";
                }
                else
                {
                    return ms_type_mapping[key];
                }
            }

            //string[] types = typeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            //return types[types.Length - 1];
            return typeName;
        }

        public static string GetNativeTypeName(Type type)
        {
            Debug.Check(type != null);

            if (Utils.IsArrayType(type))
            {
                Type itemType = type.GetGenericArguments()[0];
                return string.Format("vector<{0}>", Utils.GetNativeTypeName(itemType));
            }

            return Utils.GetNativeTypeName(type.FullName);
        }

        public static bool IsStringType(Type type)
        {
            return type == typeof(string);
        }

        public static bool IsEnumType(Type type)
        {
            return type != null && type.IsEnum;
        }

        public static bool IsArrayType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsCustomClassType(Type type)
        {
            return type != null && !type.IsByRef && (type.IsClass || type.IsValueType) && type != typeof(void) && !type.IsEnum && !type.IsPrimitive && !IsStringType(type) && !IsArrayType(type);
        }

        public static bool IsCustomStructType(Type type)
        {
            return type != null && !type.IsByRef && type.IsValueType && type != typeof(void) && !type.IsEnum && !type.IsPrimitive && !IsStringType(type) && !IsArrayType(type);
        }

        public static bool IsAgentType(Type type)
        {
            Debug.Check(type != null);
            return (type == typeof(Agent) || type.IsSubclassOf(typeof(Agent)));
        }

        public static bool IsGameObjectType(Type type)
        {
#if !BEHAVIAC_NOT_USE_UNITY
            return (type == typeof(UnityEngine.GameObject) || type.IsSubclassOf(typeof(UnityEngine.GameObject)));
#else
            return false;
#endif
        }

        public static bool IsRefNullType(Type type)
        {
            return type != null && type.IsClass && type != typeof(string);
        }

        public static bool IfEquals(object l, object r)
        {
            if (l == r)
            {
                //if both are null, then equals
                return true;
            }

            if (l == null || r == null)
            {
                //if one of them is null, not equal
                return false;
            }

            Type type = l.GetType();

            if (type != r.GetType())
            {
                return false;
            }

            bool bIsArrayType = Utils.IsArrayType(type);

            if (bIsArrayType)
            {
                IList la = (IList)l;
                IList ra = (IList)r;

                if (la.Count != ra.Count)
                {
                    return false;
                }

                for (int i = 0; i < la.Count; ++i)
                {
                    object li = la[i];
                    object ri = ra[i];

                    bool bi = IfEquals(li, ri);

                    if (!bi)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                bool bIsStruct = Utils.IsCustomClassType(type);

                if (bIsStruct)
                {
                    FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    for (int i = 0; i < fields.Length; ++i)
                    {
                        FieldInfo f = fields[i];
                        object lf = f.GetValue(l);
                        object rf = f.GetValue(r);
                        bool bi = IfEquals(lf, rf);

                        if (!bi)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            bool bIsEqual = System.Object.Equals(l, r);

            return bIsEqual;
        }

        public static void Clone<T>(ref T o, T c)
        {
            if (c == null)
            {
                o = default(T);
                return;
            }

            //Type type = c.GetType();
            Type type = typeof(T);

            if (type.IsPrimitive || type.IsEnum || type.IsValueType)
            {
                //c is boxed, it needs to make a new copy
                //if (o == null)
                //{
                //    o =(T) Activator.CreateInstance(type);
                //}

                o = c;
            }
            else if (type == typeof(string))
            {
                //string is immutable, it doesn't need to make a new copy
                o = c;
            }
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();

                if (elementType == null)
                {
                    elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                }

                var array = c as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);

                for (int i = 0; i < array.Length; i++)
                {
                    object item = null;
                    Utils.Clone(ref item, array.GetValue(i));
                    //object item = Utils.Clone(array.GetValue(i));

                    copied.SetValue(item, i);
                }

                if (o == null)
                {
                    o = (T)Convert.ChangeType(copied, type);
                }
            }
            else if (Utils.IsArrayType(type))
            {
                Type elementType = type.GetElementType();

                if (elementType == null)
                {
                    elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                }

                var array = c as IList;
                o = (T)Activator.CreateInstance(type);

                for (int i = 0; i < array.Count; i++)
                {
                    object item = null;
                    Utils.Clone(ref item, array[i]);

                    ((IList)o).Add(item);
                }
            }
            else
            {
                bool isClass = type.IsClass;

                if (isClass && Utils.IsRefNullType(type))
                {
                    o = c;
                }
                else
                {
                    Debug.Check(!type.IsPrimitive && !type.IsEnum);

                    bool isStruct = type.IsValueType;

                    if (o == null)
                    {
                        o = (T)Activator.CreateInstance(type);
                    }

                    if (isStruct || isClass)
                    {
                        FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                        for (int i = 0; i < fields.Length; ++i)
                        {
                            FieldInfo f = fields[i];

                            if (!f.IsLiteral)
                            {
                                object fv = f.GetValue(c);
                                object fv2 = null;
                                Utils.Clone(ref fv2, fv);

                                f.SetValue(o, fv2);
                            }
                        }
                    }
                    else
                    {
                        o = c;
                    }
                }
            }
        }
    }

    static public class Debug
    {
        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void CheckEqual<T>(T l, T r)
        {
            if (!EqualityComparer<T>.Default.Equals(l, r))
            {
                Break("CheckEqualFailed");
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b)
        {
            if (!b)
            {
                Break("CheckFailed");
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b, string message)
        {
            if (!b)
            {
                Break(message);
            }
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Check(bool b, string format, object arg0)
        {
            if (!b)
            {
                string message = string.Format(format, arg0);
                Break(message);
            }
        }

        //[Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
#if !BEHAVIAC_NOT_USE_UNITY
            UnityEngine.Debug.Log(message);
#else
            Console.WriteLine(message);
#endif
        }

        //[Conditional("UNITY_EDITOR")]
        public static void LogWarning(string message)
        {
#if !BEHAVIAC_NOT_USE_UNITY
            UnityEngine.Debug.LogWarning(message);
#else
            Console.WriteLine(message);
#endif
        }

        //[Conditional("UNITY_EDITOR")]
        public static void LogError(string message)
        {
            LogManager.Instance.Flush(null);
#if !BEHAVIAC_NOT_USE_UNITY
            UnityEngine.Debug.LogError(message);
#else
            Console.WriteLine(message);
#endif
        }

        //[Conditional("UNITY_EDITOR")]
        public static void LogError(Exception ex)
        {
            LogManager.Instance.Flush(null);
#if !BEHAVIAC_NOT_USE_UNITY
            UnityEngine.Debug.LogError(ex.Message);
#else
            Console.WriteLine(ex.Message);
#endif
        }

        [Conditional("BEHAVIAC_DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Break(string msg)
        {
            LogError(msg);

#if !BEHAVIAC_NOT_USE_UNITY
            UnityEngine.Debug.Break();
            //System.Diagnostics.Debug.Assert(false);
#else
            //throw new Exception();
            System.Diagnostics.Debug.Assert(false);
#endif
        }
    }

    //public static class ListExtra
    //{
    //    public static void Resize<T>(this List<T> list, int sz, T c = default(T))
    //    {
    //        int cur = list.Count;
    //        if (sz < cur)
    //        {
    //            list.RemoveRange(sz, cur - sz);
    //        }
    //        else if (sz > cur)
    //        {
    //            for (int i = 0; i < sz - cur; ++i)
    //            {
    //                list.Add(c);
    //            }
    //        }
    //    }
    //}

    static public class StringUtils
    {
        //it returns true if 'str' starts with a count followed by ':'
        //3:{....}
        private static bool IsArrayString(string str, int posStart, ref int posEnd)
        {
            //begin of the count of an array?
            //int posStartOld = posStart;

            bool bIsDigit = false;

            int strLen = (int)str.Length;

            while (posStart < strLen)
            {
                char c = str[posStart++];

                if (char.IsDigit(c))
                {
                    bIsDigit = true;
                }
                else if (c == ':' && bIsDigit)
                {
                    //transit_points = 3:{coordX = 0; coordY = 0; } | {coordX = 0; coordY = 0; } | {coordX = 0; coordY = 0; };
                    //skip array item which is possible a struct
                    int depth = 0;

                    for (int posStart2 = posStart; posStart2 < strLen; posStart2++)
                    {
                        char c1 = str[posStart2];

                        if (c1 == ';' && depth == 0)
                        {
                            //the last ';'
                            posEnd = posStart2;
                            break;
                        }
                        else if (c1 == '{')
                        {
                            Debug.Check(depth < 10);
                            depth++;
                        }
                        else if (c1 == '}')
                        {
                            Debug.Check(depth > 0);
                            depth--;
                        }
                    }

                    return true;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        ///the first char is '{', to return the paired '}'
        private static int SkipPairedBrackets(string src, int indexBracketBegin)
        {
            if (!string.IsNullOrEmpty(src) && src[indexBracketBegin] == '{')
            {
                int depth = 0;
                int pos = indexBracketBegin;

                while (pos < src.Length)
                {
                    if (src[pos] == '{')
                    {
                        depth++;
                    }
                    else if (src[pos] == '}')
                    {
                        if (--depth == 0)
                        {
                            return pos;
                        }
                    }

                    pos++;
                }
            }

            return -1;
        }

        public static List<string> SplitTokensForStruct(string src)
        {
            List<string> ret = new List<string>();

            if (string.IsNullOrEmpty(src))
            {
                return ret;
            }

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            //the first char is '{'
            //the last char is '}'
            int posCloseBrackets = SkipPairedBrackets(src, 0);
            Debug.Check(posCloseBrackets != -1);

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            //{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
            int posBegin = 1;
            int posEnd = src.IndexOf(';', posBegin);

            while (posEnd != -1)
            {
                Debug.Check(src[posEnd] == ';');

                //the last one might be empty
                if (posEnd > posBegin)
                {
                    int posEqual = src.IndexOf('=', posBegin);
                    Debug.Check(posEqual > posBegin);

                    int length = posEqual - posBegin;
                    string memmberName = src.Substring(posBegin, length);
                    string memberValueStr;
                    char c = src[posEqual + 1];

                    if (c != '{')
                    {
                        length = posEnd - posEqual - 1;

                        //to check if it is an array
                        IsArrayString(src, posEqual + 1, ref posEnd);

                        length = posEnd - posEqual - 1;
                        memberValueStr = src.Substring(posEqual + 1, length);
                    }
                    else
                    {
                        int pStructBegin = 0;
                        pStructBegin += posEqual + 1;
                        int posCloseBrackets_ = SkipPairedBrackets(src, pStructBegin);
                        length = posCloseBrackets_ - pStructBegin + 1;

                        memberValueStr = src.Substring(posEqual + 1, length);

                        posEnd = posEqual + 1 + length;
                    }

                    ret.Add(memberValueStr);
                }

                //skip ';'
                posBegin = posEnd + 1;

                //{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
                posEnd = src.IndexOf(';', posBegin);

                if (posEnd > posCloseBrackets)
                {
                    break;
                }
            }

            return ret;
        }

        private static object FromStringStruct(Type type, string src)
        {
            object objValue = Activator.CreateInstance(type);
            Dictionary<string, FieldInfo> structMembers = new Dictionary<string, FieldInfo>();
            FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo f = fields[i];

                if (!f.IsLiteral)
                {
                    structMembers.Add(f.Name, f);
                }
            }

            if (string.IsNullOrEmpty(src))
            {
                return objValue;
            }

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            //the first char is '{'
            //the last char is '}'
            int posCloseBrackets = SkipPairedBrackets(src, 0);
            Debug.Check(posCloseBrackets != -1);

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            //{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
            int posBegin = 1;
            int posEnd = src.IndexOf(';', posBegin);

            while (posEnd != -1)
            {
                Debug.Check(src[posEnd] == ';');

                //the last one might be empty
                if (posEnd > posBegin)
                {
                    int posEqual = src.IndexOf('=', posBegin);
                    Debug.Check(posEqual > posBegin);

                    int length = posEqual - posBegin;
                    string memmberName = src.Substring(posBegin, length);
                    string memberValueStr;
                    char c = src[posEqual + 1];

                    if (c != '{')
                    {
                        length = posEnd - posEqual - 1;

                        //to check if it is an array
                        IsArrayString(src, posEqual + 1, ref posEnd);

                        length = posEnd - posEqual - 1;
                        memberValueStr = src.Substring(posEqual + 1, length);
                    }
                    else
                    {
                        int pStructBegin = 0;
                        pStructBegin += posEqual + 1;
                        int posCloseBrackets_ = SkipPairedBrackets(src, pStructBegin);
                        length = posCloseBrackets_ - pStructBegin + 1;

                        memberValueStr = src.Substring(posEqual + 1, length);

                        posEnd = posEqual + 1 + length;
                    }

                    if (structMembers.ContainsKey(memmberName))
                    {
                        FieldInfo memberType = structMembers[memmberName];
                        Debug.Check(memberType != null);

                        if (memberType != null)
                        {
                            object memberValue = FromString(memberType.FieldType, memberValueStr, false);
                            memberType.SetValue(objValue, memberValue);
                        }
                    }
                }

                //skip ';'
                posBegin = posEnd + 1;

                //{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
                posEnd = src.IndexOf(';', posBegin);

                if (posEnd > posCloseBrackets)
                {
                    break;
                }
            }

            return objValue;
        }

        private static object FromStringVector(Type type, string src)
        {
            Type vectorType = typeof(List<>).MakeGenericType(type);
            IList objVector = (IList)Activator.CreateInstance(vectorType);

            if (string.IsNullOrEmpty(src))
            {
                return objVector;
            }

            int semiColon = src.IndexOf(':');
            Debug.Check(semiColon != -1);
            string countStr = src.Substring(0, semiColon);
            int count = int.Parse(countStr);

            int b = semiColon + 1;
            int sep = b;

            if (b < src.Length && src[b] == '{')
            {
                sep = SkipPairedBrackets(src, b);
                Debug.Check(sep != -1);
            }

            sep = src.IndexOf('|', sep);

            while (sep != -1)
            {
                int len = sep - b;
                string elemStr = src.Substring(b, len);
                object elemObject = FromString(type, elemStr, false);

                objVector.Add(elemObject);

                b = sep + 1;

                if (b < src.Length && src[b] == '{')
                {
                    sep = SkipPairedBrackets(src, b);
                    Debug.Check(b != -1);
                }
                else
                {
                    sep = b;
                }

                sep = src.IndexOf('|', sep);
            }

            if (b < src.Length)
            {
                int len = src.Length - b;
                string elemStr = src.Substring(b, len);
                object elemObject = FromString(type, elemStr, false);

                objVector.Add(elemObject);
            }

            Debug.Check(objVector.Count == count);

            return objVector;
        }

        public static bool IsValidString(string str)
        {
            if (string.IsNullOrEmpty(str) || (str[0] == '\"' && str[1] == '\"'))
            {
                return false;
            }

            return true;
        }

        public static object FromString(Type type, string valStr, bool bStrIsArrayType /*= false*/)
        {
            if (!string.IsNullOrEmpty(valStr) && valStr == "null")
            {
                Debug.Check(Utils.IsRefNullType(type));
                return null;
            }

            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            bool bIsArrayType = Utils.IsArrayType(type);

            object v = null;

            //customized type, struct or enum
            if (bStrIsArrayType || bIsArrayType)
            {
                if (bIsArrayType)
                {
                    Type elemType = type.GetGenericArguments()[0];
                    v = StringUtils.FromStringVector(elemType, valStr);
                }
                else
                {
                    v = StringUtils.FromStringVector(type, valStr);
                }
            }
            else if (type == typeof(behaviac.IProperty))
            {
                v = AgentMeta.ParseProperty(valStr);
            }
            else if (Utils.IsCustomClassType(type))
            {
                v = StringUtils.FromStringStruct(type, valStr);
            }
            else
            {
                v = Utils.FromStringPrimitive(type, valStr);
            }

            return v;
        }

        public static string ToString(object value)
        {
            string valueStr = "";

            if (value != null)
            {
                Type type = value.GetType();
                bool bIsArrayType = Utils.IsArrayType(type);

                //customized type, struct or enum
                if (bIsArrayType)
                {
                    IList list = value as IList;
                    valueStr = string.Format("{0}:", list.Count);

                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count - 1; ++i)
                        {
                            object e = list[i];
                            string eStr = StringUtils.ToString(e);
                            valueStr += string.Format("{0}|", eStr);
                        }

                        object eLast = list[list.Count - 1];
                        string eStrLast = StringUtils.ToString(eLast);
                        valueStr += string.Format("{0}", eStrLast);
                    }
                }
                else if (Utils.IsCustomClassType(type))
                {
                    bool bIsNullValueType = Utils.IsRefNullType(type);

                    if (bIsNullValueType)
                    {
                        valueStr = string.Format("{0:x08}", value.GetHashCode());
                    }
                    else
                    {
                        valueStr = "{";
                        //FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                        for (int i = 0; i < fields.Length; ++i)
                        {
                            FieldInfo f = fields[i];

                            if (!f.IsLiteral && !f.IsInitOnly)
                            {
                                object m = f.GetValue(value);
                                string mStr = StringUtils.ToString(m);
                                valueStr += string.Format("{0}={1};", f.Name, mStr);
                            }
                        }

                        valueStr += "}";
                    }
                }
                else
                {
                    valueStr = value.ToString();
                }
            }
            else
            {
                valueStr = "null";
            }

            return valueStr;
        }

        public static string FindExtension(string path)
        {
            return Path.GetExtension(path);
        }

        //public static bool IsNullOrEmpty(this string s)
        //{
        //    return string.IsNullOrEmpty(s);
        //}

        //
        public static int FirstToken(string params_, char sep, ref string token)
        {
            //const int 5
            int end = params_.IndexOf(sep);

            if (end != -1)
            {
                token = params_.Substring(0, end);
                return end;
            }

            return -1;
        }

        public static string RemoveQuot(string str)
        {
            const string kQuotStr = "&quot;";
            string ret = str;

            if (ret.StartsWith(kQuotStr))
            {
                //Debug.Check(ret.EndsWith(kQuotStr));
                ret = ret.Replace(kQuotStr, "\"");
            }

            return ret;
        }

        public static List<string> SplitTokens(ref string str)
        {
            List<string> ret = new List<string>();

            // "const string \"test string\""
            // "const int 10"
            // "int Self.AgentArrayAccessTest::ListInts[int Self.AgentArrayAccessTest::l_index]"

            str = RemoveQuot(str);

            if (str.StartsWith("\"") && str.EndsWith("\""))
            {
                ret.Add(str);

                return ret;
            }

            if (str.StartsWith("const string "))
            {
                ret.Add("const");
                ret.Add("string");

                string strValue = str.Substring(13);
                strValue = RemoveQuot(strValue);

                ret.Add(strValue);

                str = "const string " + strValue;

                return ret;
            }

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

        public static bool ParseForStruct(Type type, string str, ref string strT, Dictionary<string, IInstanceMember> props)
        {
            int pB = 0;
            int i = 0;

            while (i < str.Length)
            {
                char c = str[i];

                if (c == ';' || c == '{' || c == '}')
                {
                    int p = pB;

                    while (p <= i)
                    {
                        strT += str[p++];
                    }

                    pB = i + 1;
                }
                else if (c == ' ')
                {
                    //par or property
                    string propName = "";
                    int p = pB;

                    while (str[p] != '=')
                    {
                        propName += str[p++];
                    }

                    //skip '='
                    Debug.Check(str[p] == '=');
                    p++;

                    string valueStr = str.Substring(p);

                    string typeStr = "";

                    while (str[p] != ' ')
                    {
                        typeStr += str[p++];
                    }

                    //bool bStatic = false;

                    if (typeStr == "static")
                    {
                        //skip ' '
                        Debug.Check(str[p] == ' ');
                        p++;

                        while (str[p] != ' ')
                        {
                            typeStr += str[p++];
                        }

                        //bStatic = true;
                    }

                    string parName = "";

                    //skip ' '
                    Debug.Check(str[i] == ' ');
                    i++;

                    while (str[i] != ';')
                    {
                        parName += str[i++];
                    }

                    props[propName] = AgentMeta.ParseProperty(valueStr);

                    //skip ';'
                    Debug.Check(str[i] == ';');

                    pB = i + 1;
                }

                i++;
            }

            return true;
        }
    }
}
