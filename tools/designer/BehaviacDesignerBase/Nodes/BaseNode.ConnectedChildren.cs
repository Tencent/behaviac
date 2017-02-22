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
using Behaviac.Design.Properties;

namespace Behaviac.Design.Nodes
{
    public partial class BaseNode
    {
        /// <summary>
        /// This class holds all connectors registered on a node. There may only be one ConnectedChildren class per node.
        /// </summary>
        public class ConnectedChildren : System.Collections.IEnumerable
        {
            /// <summary>
            /// Holds if the list of children needs to be rebuilt from the connectors.
            /// </summary>
            protected bool _requiresRebuild = true;

            /// <summary>
            /// The registered connecors.
            /// </summary>
            protected List<Connector> _connectors = new List<Connector>();

            protected BaseNode _owner;

            /// <summary>
            /// The node this class belongs to.
            /// </summary>
            public BaseNode Owner
            {
                get
                {
                    return _owner;
                }
            }

            protected List<BaseNode> _children = new List<BaseNode>();

            /// <summary>
            /// A list of all children connected via connectors.
            /// </summary>
            public IList<BaseNode> Children
            {
                get
                {
                    RebuildChildList();
                    return _children.AsReadOnly();
                }
            }

            /// <summary>
            /// All connectors registered on the ndoe.
            /// </summary>
            public IList<Connector> Connectors
            {
                get
                {
                    return _connectors.AsReadOnly();
                }
            }

            /// <summary>
            /// Creates a new instance to store all available connectors on an node.
            /// </summary>
            /// <param name="owner">The node this instance belongs to.</param>
            public ConnectedChildren(BaseNode owner)
            {
                _owner = owner;
            }

            /// <summary>
            /// Rebuilds the list of all children connected via connectors.
            /// </summary>
            protected void RebuildChildList()
            {
                if (_requiresRebuild)
                {
                    _requiresRebuild = false;

                    // clear the list
                    _children.Clear();

                    // for every connector
                    foreach (Connector connector in _connectors)
                    {
                        // add its children
                        for (int i = 0; i < connector.ChildCount; ++i)
                        {
                            BaseNode child = connector.GetChild(i);

                            if (child != null)
                            {
                                _children.Add(child);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Registeres a connector.
            /// </summary>
            /// <param name="connector">The connector which will be registered.</param>
            public void RegisterConnector(Connector connector)
            {
                // check if this connector has already been registered. The identifier must be unique on the node.
                foreach (Connector conn in _connectors)
                {
                    if (conn.Identifier == connector.Identifier)
                    {
                        throw new Exception(Resources.ExceptionDuplicatedConnectorIdentifier);
                    }
                }

                // add the connector and queue and update of the child list
                _connectors.Add(connector);
                _requiresRebuild = true;

                // add the visual subitems for the connector
                NodeViewData nvd = _owner as NodeViewData;

                if (nvd != null)
                {
                    for (int i = 0; i < connector.MinCount; ++i)
                    {
                        nvd.AddSubItem(new NodeViewData.SubItemConnector(connector, null, i));
                    }
                }
            }

            /// <summary>
            /// Queues a rebuild of the list of children.
            /// </summary>
            public void RequiresRebuild()
            {
                _requiresRebuild = true;
            }

            /// <summary>
            /// Gets a connector by one of its children.
            /// </summary>
            /// <param name="child">The child whose connector we are looking for.</param>
            /// <returns>Returns null if no connector could be found.</returns>
            public Connector GetConnector(BaseNode child)
            {
                foreach (Connector connector in _connectors)
                {
                    for (int i = 0; i < connector.ChildCount; ++i)
                    {
                        if (connector.GetChild(i) == child)
                        {
                            return connector;
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// Gets a connector by its identifier.
            /// </summary>
            /// <param name="identifier">The identifier we are looking for.</param>
            /// <returns>Returns null if no connector with this identifier exists.</returns>
            public Connector GetConnector(string identifier)
            {
                Connector defaultConnector = null;

                foreach (Connector connector in _connectors)
                {
                    if (connector.Identifier == identifier)
                    {
                        return connector;
                    }

                    defaultConnector = connector;
                }

                if (defaultConnector == null && _connectors.Count > 0)
                {
                    defaultConnector = _connectors[0];
                }

                return defaultConnector;
            }

            /// <summary>
            /// Checks if a connector is regsitered. Mainly for debug purposes.
            /// </summary>
            /// <param name="conn">The connector we want to check.</param>
            /// <returns>Returns true if the connector is registered.</returns>
            public bool HasConnector(Connector conn)
            {
                return _connectors.Contains(conn);
            }

            /// <summary>
            /// The number of children connected.
            /// </summary>
            public int ChildCount
            {
                get
                {
                    RebuildChildList();
                    return _children.Count;
                }
            }

            public System.Collections.IEnumerator GetEnumerator()
            {
                RebuildChildList();
                return _children.GetEnumerator();
            }

            /// <summary>
            /// Checks if a node can be adopted by this one.
            /// </summary>
            /// <param name="child">The node we want to adopt.</param>
            /// <returns>Returns true if this node can adopt the given child.</returns>
            public bool CanAdoptNode(BaseNode child)
            {
                Connector connector = (child != null && child.ParentConnector != null) ? GetConnector(child.ParentConnector.Identifier) : null;
                return (connector != null) ? connector.AcceptsChild(child) : false;
            }

            /// <summary>
            /// Clears all children from all connectors.
            /// </summary>
            public void ClearChildren()
            {
                foreach (Connector conn in _connectors)
                {
                    conn.ClearChildren();
                }
            }

            /// <summary>
            /// Exchanges any registered connector with the given one. This is used internally for subreferenced behaviours.
            /// </summary>
            /// <param name="connector">The connector which will replace all the others.</param>
            public void SetConnector(Connector connector)
            {
                _connectors.Clear();
                _connectors.Add(connector);

                _requiresRebuild = true;
            }

            /// <summary>
            /// This is used internally for subreferenced behaviours.
            /// </summary>
            public void ClearConnectors()
            {
                _connectors.Clear();
                _requiresRebuild = true;
            }
        }
    }
}
