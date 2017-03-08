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
    public class DesignerInteger : DesignerProperty
    {
        protected int _min, _max, _steps;
        protected string _units;

        /// <summary>
        /// The units the value is represented in.
        /// </summary>
        public string Units
        {
            get
            {
                return Plugin.GetResourceString(_units);
            }
        }

        /// <summary>
        /// The minimum value of the property.
        /// </summary>
        public int Min
        {
            get
            {
                return _min;
            }
        }

        /// <summary>
        /// The maximum value of the property.
        /// </summary>
        public int Max
        {
            get
            {
                return _max;
            }
        }

        /// <summary>
        /// The minimum value added or substracted when changing the property's value.
        /// </summary>
        public int Steps
        {
            get
            {
                return _steps;
            }
        }

        /// <summary>
        /// Creates a new designer attribute for handling an integer value.
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        /// <param name="linkedToProperty">The restrictions of this property are defined by another property.</param>
        /// <param name="min">The minimum value of the property.</param>
        /// <param name="max">The maximum value of the property.</param>
        /// <param name="steps">The minimum value added or substracted when changing the property's value.</param>
        /// <param name="units">The units the value is represented in.</param>
        public DesignerInteger(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags, string linkedToProperty, int min, int max, int steps, string units)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerNumberEditor), linkedToProperty)
        {
            _min = min;
            _max = max;
            _steps = steps;
            _units = units;
        }

        public DesignerInteger(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerNumberEditor), null)
        {
            _min = int.MinValue;
            _max = int.MaxValue;
            _steps = 1;
            _units = null;
        }

        public override string GetDisplayValue(object obj)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", (int)obj);
        }

        public override string GetExportValue(object owner, object obj)
        {
            return obj.ToString();
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (!Plugin.IsIntergerNumberType(type))
            {
                throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            }

            int resultValue = 0;

            if (int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out resultValue))
            {
                if (type == typeof(uint))
                {
                    return (uint)resultValue;
                }

                if (type == typeof(short))
                {
                    return (short)resultValue;
                }

                if (type == typeof(ushort))
                {
                    return (ushort)resultValue;
                }

                if (type == typeof(sbyte))
                {
                    return (sbyte)resultValue;
                }

                if (type == typeof(byte))
                {
                    return (byte)resultValue;
                }

                if (type == typeof(long))
                {
                    return (long)resultValue;
                }

                if (type == typeof(ulong))
                {
                    return (ulong)resultValue;
                }

                return resultValue;
            }

            throw new Exception(string.Format(Resources.ExceptionDesignerAttributeIllegalIntegerValue, str));
        }
    }
}
