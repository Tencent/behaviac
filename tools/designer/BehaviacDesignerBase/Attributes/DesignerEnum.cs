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
    [AttributeUsage(/*AttributeTargets.Field | */AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DesignerEnum : DesignerProperty
    {
        public enum ExportMode { Namespace_Type_Value, Type_Value, Value }
        protected static ExportMode __exportMode = ExportMode.Namespace_Type_Value;

        /// <summary>
        /// Defines is enumerations are exported as Namespace.Enum.Value or simply as Value.
        /// </summary>
        public static ExportMode ExportTextMode
        {
            get
            {
                return __exportMode;
            }
            set
            {
                __exportMode = value;
            }
        }

        protected static string __exportPrefix = string.Empty;

        /// <summary>
        /// This prefix is placed in front of the enum's value when exporting;
        /// </summary>
        public static string ExportPrefix
        {
            get
            {
                return __exportPrefix;
            }
            set
            {
                __exportPrefix = value;
            }
        }

        /// <summary>
        /// Creates a new designer attribute for handling an enumeration value.
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        /// <param name="exclude">The enum values which will be excluded from the values shown in the designer.</param>
        public DesignerEnum(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags, string excludeTag)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerEnumEditor), null)
        {
            excludeTag_ = excludeTag;
        }

        string excludeTag_;
        public string ExcludeTag
        {
            get
            {
                return excludeTag_;
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
                //throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, obj));
                Debug.Check(false);
                return "";
            }

            string enumName = Enum.GetName(type, obj);

            if (string.IsNullOrEmpty(enumName))
            {
                //throw new Exception(string.Format(Resources.ExceptionDesignerAttributeEnumValueIllegal, obj));
                Debug.Check(false);
                return "";
            }

            return enumName;
        }

        public static string GetDisplayName(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            string enumName = getEnumName(obj);

            System.Reflection.FieldInfo fi = obj.GetType().GetField(obj.ToString());
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(EnumMemberDescAttribute), false);

            if (attributes.Length > 0)
            {
                enumName = ((EnumMemberDescAttribute)attributes[0]).DisplayName;
            }

            return enumName;
        }

        public static string GetDescription(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            string enumName = getEnumName(obj);

            System.Reflection.FieldInfo fi = obj.GetType().GetField(obj.ToString());
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(EnumMemberDescAttribute), false);

            if (attributes.Length > 0)
            {
                enumName = ((EnumMemberDescAttribute)attributes[0]).Description;
            }

            return enumName;
        }

        public static string GeneratedCode(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            string enumName = getEnumName(obj);

            System.Reflection.FieldInfo fi = obj.GetType().GetField(obj.ToString());
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(EnumMemberDescAttribute), false);

            if (attributes.Length > 0)
            {
                enumName = ((EnumMemberDescAttribute)attributes[0]).NativeValue;
            }

            return enumName;
        }

        public override string GetDisplayValue(object obj)
        {
            return GetDisplayName(obj);
        }

        public override string GetExportValue(object owner, object obj)
        {
            return getEnumName(obj);
        }

        public override string GetSaveValue(object owner, object obj)
        {
            return getEnumName(obj);
        }

        public override string GetGeneratedValue(object owner, object obj)
        {
            return GeneratedCode(obj);
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (!type.IsEnum)
            {
                //throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, type));
                Debug.Check(false);
                return null;
            }

            string[] parts = str.Split(':');

            if (parts.Length != 2)
            {
                // keep compatibility with version 1
                //throw new Exception( string.Format(Resources.ExceptionDesignerAttributeEnumCouldNotReadValue, str) );
                parts = new string[] { str, "-1" };
            }

            string enumname = parts[0];

            int enumval;

            try
            {
                // try to get the enum value by name
                enumval = (int)Enum.Parse(type, enumname, true);

            }
            catch
            {
                // try to read the stored enum value index
                if (!int.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out enumval))
                {
                    throw new Exception(string.Format(Resources.ExceptionDesignerAttributeEnumCouldNotParseValue, str));
                }

                // try to get the enum value by index
                if (Enum.ToObject(type, enumval) == null)
                {
                    throw new Exception(string.Format(Resources.ExceptionDesignerAttributeEnumIllegalEnumIndex, enumval));
                }

                enumval = (int)Plugin.DefaultValue(type);
            }

            return enumval;
        }
    }
}
