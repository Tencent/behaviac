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
using System.Reflection;
using System.Drawing;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;

namespace Behaviac.Design
{
    public partial class NodeViewData
    {
        /// <summary>
        /// A subitem used to draw a property of the node on it.
        /// </summary>
        public class SubItemProperty : SubItemText
        {
            protected Node _owner;
            protected PropertyInfo _property;
            protected DesignerProperty _attribute;

            private static Font __font = new Font("Calibri,Arial", 6.0f, FontStyle.Regular);

            /// <summary>
            /// Creates a new subitem which can show a property on the node.
            /// </summary>
            /// <param name="owner">The node whose property we want to show. MUST be the same as the one the subitem belongs to.</param>
            /// <param name="property">The property we want to show.</param>
            /// <param name="att">The attribute associated with the property.</param>
            public SubItemProperty(Node owner, PropertyInfo property, DesignerProperty att)
            : base(null, null, __font, Brushes.White, Alignment.Center, false)
            {
                _owner = owner;
                _property = property;
                _attribute = att;
            }

            private string getPropertyName(string str)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    int quotationCount = 0;
                    int doubleQuotationCount = 0;
                    int lastSpaceIndex = -1;

                    str = str.Trim();

                    for (int i = 0; i < str.Length; ++i)
                    {
                        if (str[i] == '\'')
                        {
                            if (doubleQuotationCount == 0)
                            {
                                if (quotationCount == 0)
                                {
                                    quotationCount++;
                                }

                                else
                                {
                                    quotationCount--;
                                }
                            }

                        }
                        else if (str[i] == '"')
                        {
                            if (quotationCount == 0)
                            {
                                if (doubleQuotationCount == 0)
                                {
                                    doubleQuotationCount++;
                                }

                                else
                                {
                                    doubleQuotationCount--;
                                }
                            }

                        }
                        else if (str[i] == ' ')
                        {
                            if (quotationCount == 0 && doubleQuotationCount == 0)
                            {
                                lastSpaceIndex = i;
                            }
                        }
                    }

                    return str.Substring(lastSpaceIndex + 1);
                }

                return string.Empty;
            }

            protected override string Label
            {
                get
                {
                    // get the value from the property
                    object obj = _property.GetValue(_owner, null);
                    string str = _attribute.GetDisplayValue(obj);
                    //string[] tokens = str.Split(' ');
                    //str = tokens[tokens.Length - 1];
                    return _attribute.DisplayName + " = " + getPropertyName(str);
                }
            }

            public override SubItem Clone(Node newnode)
            {
                // this subitem is automatically added it does not need to be duplicated
                return null;
            }
        }
    }
}
