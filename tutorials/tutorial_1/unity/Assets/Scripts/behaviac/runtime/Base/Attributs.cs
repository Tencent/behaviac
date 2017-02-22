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

namespace behaviac
{
    public enum ERefType
    {
        /**by default, a class is a ref type while a struct is a value type
         *
         * a ref type in designer will be displayed as a 'pointer, which is not expanded so that its members can't be configured individually
         **/
        ERT_Undefined,

        //forced to be a value type
        ERT_ValueType
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class TypeMetaInfoAttribute : Attribute
    {
        public TypeMetaInfoAttribute(string displayName, string description)
        {
            this.displayName_ = displayName;
            this.desc_ = description;
        }

        public TypeMetaInfoAttribute(string displayName, string description, ERefType refType)
        {
            this.displayName_ = displayName;
            this.desc_ = description;
            this.refType_ = refType;
        }

        public TypeMetaInfoAttribute()
        {
        }

        public TypeMetaInfoAttribute(ERefType refType)
        {
            this.refType_ = refType;
        }

        private string displayName_;
        private string desc_;

        public string DisplayName
        {
            get
            {
                return this.displayName_;
            }
        }

        public string Description
        {
            get
            {
                return this.desc_;
            }
        }

        //0, by default, class is reftype while struct is value type
        //1, even it is a class, it is still as a value type in designer
        private ERefType refType_ = ERefType.ERT_Undefined;
        public ERefType RefType
        {
            get
            {
                return refType_;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class GeneratedTypeMetaInfoAttribute : Attribute
    {
        public GeneratedTypeMetaInfoAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MemberMetaInfoAttribute : TypeMetaInfoAttribute
    {
        public MemberMetaInfoAttribute(string displayName, string description, bool bReadOnly) :
        this(displayName, description)
        {
            m_bIsReadonly = bReadOnly;
        }

        public MemberMetaInfoAttribute(string displayName, string description)
        : this(displayName, description, 1.0f)
        {
        }

        public MemberMetaInfoAttribute(string displayName, string description, float range)
        : base(displayName, description)
        {
            m_range = range;
        }

        public MemberMetaInfoAttribute()
        {
        }

        public MemberMetaInfoAttribute(bool bReadonly)
        : this()
        {
            this.m_bIsReadonly = bReadonly;
        }

        private bool m_bIsReadonly = false;

        public bool IsReadonly
        {
            get
            {
                return this.m_bIsReadonly;
            }
        }

        private static string getEnumName(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            Type type = obj.GetType();

            if (!type.IsEnum)
            {
                return string.Empty;
            }

            string enumName = Enum.GetName(type, obj);

            if (string.IsNullOrEmpty(enumName))
            {
                return string.Empty;
            }

            return enumName;
        }

        public static string GetEnumDisplayName(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            string enumName = getEnumName(obj);

            System.Reflection.FieldInfo fi = obj.GetType().GetField(obj.ToString());
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);

            if (attributes.Length > 0)
            {
                enumName = ((MemberMetaInfoAttribute)attributes[0]).DisplayName;
            }

            return enumName;
        }

        public static string GetEnumDescription(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            string enumName = getEnumName(obj);

            System.Reflection.FieldInfo fi = obj.GetType().GetField(obj.ToString());
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);

            if (attributes.Length > 0)
            {
                enumName = ((MemberMetaInfoAttribute)attributes[0]).Description;
            }

            return enumName;
        }

        private float m_range = 1.0f;

        public float Range
        {
            get
            {
                return this.m_range;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MethodMetaInfoAttribute : TypeMetaInfoAttribute
    {
        public MethodMetaInfoAttribute(string displayName, string description)
        : base(displayName, description)
        {
        }

        public MethodMetaInfoAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ParamMetaInfoAttribute : TypeMetaInfoAttribute
    {
        public ParamMetaInfoAttribute()
        {
        }

        public ParamMetaInfoAttribute(string displayName, string description, string defaultValue)
        : base(displayName, description)
        {
            defaultValue_ = defaultValue;
            rangeMin_ = float.MinValue;
            rangeMax_ = float.MaxValue;
        }

        public ParamMetaInfoAttribute(string displayName, string description, string defaultValue, float rangeMin, float rangeMax)
        : base(displayName, description)
        {
            defaultValue_ = defaultValue;
            rangeMin_ = rangeMin;
            rangeMax_ = rangeMax;
        }

        private string defaultValue_;

        public string DefaultValue
        {
            get
            {
                return defaultValue_;
            }
        }

        private float rangeMin_ = float.MinValue;

        public float RangeMin
        {
            get
            {
                return rangeMin_;
            }
        }

        private float rangeMax_ = float.MaxValue;

        public float RangeMax
        {
            get
            {
                return rangeMax_;
            }
        }
    }
}
