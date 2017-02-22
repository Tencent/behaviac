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

namespace Behaviac.Design.Nodes
{
    public partial class BaseNode
    {
        /// <summary>
        /// Connector which allows a single child to be connected.
        /// </summary>
        public class ConnectorSingle : Connector
        {
            protected BaseNode _child = null;

            /// <summary>
            /// The child connected.
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
                }
            }

            /// <summary>
            /// Creates a new connector which can hold a single child.
            /// </summary>
            /// <param name="connectedChildren">Usually the _children member of a node.</param>
            /// <param name="label">The label which is used to generate the individual label for each connected child. May contain {0} to include the index.</param>
            /// <param name="identifier">The identifier of the connector.</param>
            public ConnectorSingle(ConnectedChildren connectedChildren, string label, string identifier)
            : base(connectedChildren, label, identifier, 1, 1)
            {
            }

            public override int EnableChildCount
            {
                get
                {
                    return (_child != null && _child is Node && ((Node)_child).Enable) ? 1 : 0;
                }
            }

            public override int ChildCount
            {
                get
                {
                    return _child == null ? 0 : 1;
                }
            }

            public override BaseNode GetChild(int index)
            {
                Debug.Check(index == 0 && _child != null);

                return _child;
            }

            public override bool AddChild(BaseNode node)
            {
                // check if we can accept the node
                if (_child == null && !_isReadOnly)
                {
                    node._parentConnector = this;
                    _child = node;
                    _connectedChildren.RequiresRebuild();

                    if (node is NodeViewData)
                    {
                        AddSubItem(node);
                    }

                    return true;
                }

                return false;
            }

            public override bool AddChild(BaseNode node, int index)
            {
                // we only have one child, the index does not matter to us
                return AddChild(node);
            }

            public override bool AcceptsChildren(IList<BaseNode> children, bool acceptEvenFull = false)
            {
                if (children == null || children.Count < 1)
                {
                    return true;
                }

                if (_isReadOnly)
                {
                    return false;
                }

                if (children.Count > 1)
                {
                    return false;
                }

                return acceptEvenFull ? true : (_child == null);
            }

            public override bool RemoveChild(BaseNode node)
            {
                // check if we can remove the node
                if (_child == node && !_isReadOnly)
                {
                    node._parentConnector = null;
                    _child = null;
                    _connectedChildren.RequiresRebuild();

                    //RemoveSubItem(node);

                    return true;
                }

                return false;
            }

            public override void ClearChildren()
            {
                // clear the child and its connector
                if (_child != null)
                {
                    _child._parentConnector = null;
                    _child = null;
                }

                _connectedChildren.RequiresRebuild();

                ClearSubItems();
            }

            public override void ClearChildrenInternal()
            {
                _child = null;

                _connectedChildren.RequiresRebuild();
            }

            public override int GetChildIndex(BaseNode node)
            {
                return _child == node ? 0 : -1;
            }

            public override Connector Clone(ConnectedChildren connectedChildren)
            {
                return new ConnectorSingle(connectedChildren, _label, _identifier);
            }
        }
    }
}
