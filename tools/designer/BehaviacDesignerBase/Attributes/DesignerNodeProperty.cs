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
    public class DesignerNodeProperty : DesignerProperty
    {
        protected Type[] _supportedTypes;

        /// <summary>
        /// The type a property must be of to be shown in the designer.
        /// </summary>
        public Type[] SupportedTypes
        {
            get
            {
                return _supportedTypes;
            }
        }

        /// <summary>
        /// Creates a new designer attribute for handling a property of the an event/override is attached to.
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        /// <param name="supportedTypes">The enum values which will be excluded from the values shown in the designer.</param>
        public DesignerNodeProperty(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags, Type[] supportedTypes)
        : base(displayName, description, category, displayMode, displayOrder, flags, typeof(DesignerNodePropertyEditor), null)
        {
            _supportedTypes = supportedTypes;
        }

        public override string GetDisplayValue(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return ((string)obj).Replace("\"", string.Empty);
        }

        public override string GetExportValue(object owner, object obj)
        {
            if (obj == null)
            {
                return "\"\"";
            }

            string s = (string)obj;

            if (string.IsNullOrEmpty(s))
            {
                return "\"\"";
            }

            return string.Format("\"{0}\"", s);
        }

        public override object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            if (type != typeof(string))
            {
                throw new Exception(Resources.ExceptionDesignerAttributeInvalidType);
            }

            return str;
        }
    }
}
