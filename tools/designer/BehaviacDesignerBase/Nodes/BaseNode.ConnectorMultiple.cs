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

        public class ConnectorMultiple : Connector
        {
            protected List<BaseNode> _children = new List<BaseNode>();

            /// <summary>
            /// The children connected to the connector.
            /// </summary>
            public IList<BaseNode> Children
            {
                get
                {
                    return _children.AsReadOnly();
                }
            }

            /// <summary>
            /// Creates a new connector which can hold multiple children.
            /// </summary>
            /// <param name="connectedChildren">Usually the _children member of a node.</param>
            /// <param name="label">The label which is used to generate the individual label for each connected child. May contain {0} to include the index.</param>
            /// <param name="identifier">The identifier of the connector.</param>
            /// <param name="minCount">The minimum number of connectors shown on the node.</param>
            /// <param name="maxCount">The maximum number of children which can be connected via this connector.</param>
            public ConnectorMultiple(ConnectedChildren connectedChildren, string label, string identifier, int minCount, int maxCount)
            : base(connectedChildren, label, identifier, minCount, maxCount)
            {
            }

            public override int EnableChildCount
            {
                get
                {
                    int count = 0;

                    foreach (BaseNode node in _children)
                    {
                        if (node is Node && ((Node)node).Enable)
                        {
                            count++;
                        }
                    }

                    return count;
                }
            }

            public override int ChildCount
            {
                get
                {
                    return _children.Count;
                }
            }

            public override BaseNode GetChild(int index)
            {
                return _children[index];
            }

            public override bool AddChild(BaseNode node)
            {
                // check if we we may add the node
                if (_children.Count >= _maxCount || _isReadOnly)
                {
                    return false;
                }

                node._parentConnector = this;
                _children.Add(node);
                _connectedChildren.RequiresRebuild();

                if (node is NodeViewData)
                {
                    AddSubItem(node);
                }

                return true;
            }

            public override bool AddChild(BaseNode node, int index)
            {
                // check if we we may add the node
                if (_children.Count >= _maxCount || _isReadOnly)
                {
                    return false;
                }

                node._parentConnector = this;
                _children.Insert(index, node);
                _connectedChildren.RequiresRebuild();

                //AddSubItem(node, index);

                return true;
            }

            public override bool AcceptsChildren(IList<BaseNode> children, bool acceptEvenFull = false)
            {
                if (_children == null || children.Count < 1)
                {
                    return true;
                }

                if (_isReadOnly)
                {
                    return false;
                }

                int total = _children.Count + children.Count;

                return acceptEvenFull ? true : (total <= _maxCount);
            }

            public override bool RemoveChild(BaseNode node)
            {
                // check if we can remove the node
                if (!_isReadOnly && _children.Remove(node))
                {
                    node._parentConnector = null;

                    _connectedChildren.RequiresRebuild();

                    //RemoveSubItem(node);

                    return true;
                }

                return false;
            }

            public override void ClearChildren()
            {
                // clear the connector for every child
                foreach (BaseNode node in _children)
                {
                    node._parentConnector = null;
                }

                _children.Clear();
                _connectedChildren.RequiresRebuild();

                ClearSubItems();
            }

            public override void ClearChildrenInternal()
            {
                _children.Clear();
                _connectedChildren.RequiresRebuild();

                //ClearSubItems();
            }

            public override int GetChildIndex(BaseNode node)
            {
                return _children.IndexOf(node);
            }

            public override Connector Clone(ConnectedChildren connectedChildren)
            {
                return new ConnectorMultiple(connectedChildren, _label, _identifier, _minCount, _maxCount);
            }
        }
    }
}
