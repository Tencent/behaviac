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
using System.Drawing;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public partial class NodeViewData
    {
        /// <summary>
        /// This class holds the style of a node when it is rendered.
        /// </summary>
        public class Style
        {
            private Brush _background;
            public Brush Background
            {
                get
                {
                    return _background;
                }
                set
                {
                    _background = value;
                }
            }

            private Pen _border;
            public Pen Border
            {
                get
                {
                    return _border;
                }
                set
                {
                    _border = value;
                }
            }

            private Brush _label;
            public Brush Label
            {
                get
                {
                    return _label;
                }
                set
                {
                    _label = value;
                }
            }

            public Style(Brush background, Pen border, Brush label)
            {
                _background = background;
                _border = border;
                _label = label;
            }

            /// <summary>
            /// Combines two styles, for example default style + selected style, so you must only define the values which have changed.
            /// </summary>
            /// <param name="a">Style with defaults.</param>
            /// <param name="b">Style with overrides.</param>
            /// <returns>Returns a combination of both styles.</returns>
            public static Style operator +(Style a, Style b)
            {
                if (a == null && b == null)
                {
                    throw new Exception(Resources.ExceptionBothStylesNull);
                }

                if (a == null)
                {
                    return b;
                }

                if (b == null)
                {
                    return a;
                }

                return new Style(b.Background != null ? b.Background : a.Background, b.Border != null ? b.Border : a.Border, b.Label != null ? b.Label : a.Label);
            }
        }
    }
}
