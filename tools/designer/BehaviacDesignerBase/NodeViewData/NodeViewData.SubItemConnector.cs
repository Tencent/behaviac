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
using Behaviac.Design.Nodes;

namespace Behaviac.Design
{
    public partial class NodeViewData
    {
        /// <summary>
        /// A subitem used to draw a connector on the node.
        /// </summary>
        public class SubItemConnector : SubItemText
        {
            protected Connector _connector;

            /// <summary>
            /// The connector represented by this subitem.
            /// </summary>
            public Connector Connector
            {
                get
                {
                    return _connector;
                }
            }

            protected BaseNode _child;

            /// <summary>
            /// The child attached to the connector and represented by this subitem.
            /// </summary>
            public BaseNode Child
            {
                get
                {
                    return _child;
                }

                set
                {
                    _child = value;

#if DEBUG

                    // ensure that the child is indeed connected over the given connector
                    if (value != null)
                    {
                        Debug.Check(_connector.GetChildIndex(value) >= 0);
                    }

#endif
                }
            }

            protected int _index;

            /// <summary>
            /// The index of the child in the connector.
            /// </summary>
            public int Index
            {
                get
                {
                    return _index;
                }
                set
                {
                    _index = value;
                }
            }

            protected override string Label
            {
                get
                {
                    return _connector.GetLabel(Index);
                }
            }

            private static Font __font = new Font("Calibri,Arial", 6.0f, FontStyle.Regular);

            /// <summary>
            /// Creates a new subitem used to visualise a connected child on the node.
            /// </summary>
            /// <param name="connector">The connector we want to visualise.</param>
            /// <param name="child">The child represented by the subitem. Can be null.</param>
            /// <param name="index">The index of the child in the connector. Also a null child has an index!</param>
            public SubItemConnector(Connector connector, BaseNode child, int index)
            : base(null, null, __font, Brushes.White, Alignment.Right, true)
            {
                _connector = connector;
                _child = child;
                _index = index;
            }

            public override SubItem Clone(Node newnode)
            {
                // the property does not need to be cloned it will be automatically generated
                return null;
            }
        }
    }
}
