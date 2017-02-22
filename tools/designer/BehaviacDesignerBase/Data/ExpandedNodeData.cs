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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Behaviac.Design.Data
{
    public class ExpandedNodePool
    {
        [Serializable]
        public class ExpandedDatum : ISerializable
        {
            // self expand
            public bool isExpanded = false;

            // all expaned connectors
            public List<string> expandedConnectors = new List<string>();

            public ExpandedDatum()
            {
            }

            protected ExpandedDatum(SerializationInfo info, StreamingContext context)
            {
                this.isExpanded = info.GetBoolean("isExpanded");
                this.expandedConnectors = info.GetValue("expandedConnectors", typeof(List<string>)) as List<string>;
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("isExpanded", isExpanded);
                info.AddValue("expandedConnectors", expandedConnectors);
            }
        }

        // <filename, <fullId, isExpaned>>
        private static Dictionary<string, Dictionary<string, ExpandedDatum>> _expandedNodeDict = new Dictionary<string, Dictionary<string, ExpandedDatum>>();

        public static void Clear()
        {
            _expandedNodeDict.Clear();
        }

        public static bool HasSetExpandedNodes(NodeViewData nvd)
        {
            Debug.Check(nvd != null && nvd.Node != null);
            if (nvd != null && nvd.Node != null)
            {
                Nodes.Behavior behavior = nvd.Node.Behavior as Nodes.Behavior;

                if (behavior != null)
                {
                    behavior = behavior.GetTopBehavior();
                }

                string filename = (behavior != null) && !string.IsNullOrEmpty(behavior.Filename) ? behavior.RelativePath : string.Empty;

                if (!string.IsNullOrEmpty(filename) && _expandedNodeDict.ContainsKey(filename))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsExpandedNode(NodeViewData nvd)
        {
            Debug.Check(nvd != null && nvd.Node != null);
            if (nvd != null && nvd.Node != null)
            {
                bool defaultExpanded = !(nvd.Node is Nodes.ReferencedBehavior);

                if (defaultExpanded)
                {
                    //if there is a leaf node, collapse it
                    foreach (NodeViewData child in nvd.Children)
                    {
                        if (!child.CanBeExpanded())
                        {
                            //leaf node can't be expanded, there is a leaf node!
                            defaultExpanded = false;
                            break;
                        }
                    }
                }

                Nodes.Behavior behavior = nvd.Node.Behavior as Nodes.Behavior;

                if (behavior != null)
                {
                    behavior = behavior.GetTopBehavior();
                }

                string filename = (behavior != null) && !string.IsNullOrEmpty(behavior.Filename) ? behavior.RelativePath : string.Empty;

                if (!string.IsNullOrEmpty(filename) && _expandedNodeDict.ContainsKey(filename))
                {
                    Dictionary<string, ExpandedDatum> expandedNodes = _expandedNodeDict[filename];

                    string id = nvd.FullId;

                    if (!expandedNodes.ContainsKey(id))
                    {
                        return defaultExpanded;
                    }

                    return expandedNodes[id].isExpanded;

                }
                else
                {
                    //defaultExpanded = true;
                }

                return defaultExpanded;
            }

            return false;
        }

        public static bool SetExpandedNode(NodeViewData nvd, bool isExpanded)
        {
            Debug.Check(nvd != null && nvd.Node != null);
            if (nvd != null && nvd.Node != null)
            {
                Nodes.Behavior b = nvd.Node.Behavior as Nodes.Behavior;

                if (b != null)
                {
                    b = b.GetTopBehavior();

                    Debug.Check(b != null);

                    if (!string.IsNullOrEmpty(b.Filename))
                    {
                        return SetExpandedNode(b.RelativePath, nvd.FullId, isExpanded);
                    }
                }
            }

            return false;
        }


        private static bool SetExpandedNode(string relativePath, string fullId, bool isExpanded)
        {
            if (!string.IsNullOrEmpty(relativePath))
            {
                if (!_expandedNodeDict.ContainsKey(relativePath))
                {
                    _expandedNodeDict[relativePath] = new Dictionary<string, ExpandedDatum>();
                }

                Dictionary<string, ExpandedDatum> expandedNodes = _expandedNodeDict[relativePath];

                if (!expandedNodes.ContainsKey(fullId))
                {
                    expandedNodes[fullId] = new ExpandedDatum();
                }

                ExpandedDatum datum = expandedNodes[fullId];
                datum.isExpanded = isExpanded;

                return true;
            }

            return false;
        }

        public static bool IsExpandedConnector(NodeViewData nvd, string connector)
        {
            Debug.Check(nvd != null && nvd.Node != null);
            if (nvd != null && nvd.Node != null)
            {
                Nodes.Behavior behavior = nvd.Node.Behavior as Nodes.Behavior;

                if (behavior != null)
                {
                    behavior = behavior.GetTopBehavior();
                }

                string filename = (behavior != null) && !string.IsNullOrEmpty(behavior.Filename) ? behavior.RelativePath : string.Empty;

                return IsExpandedConnector(filename, nvd.Node.Id.ToString(), connector);
            }

            return false;
        }

        public static bool IsExpandedConnector(string relativePath, string id, string connector)
        {
            if (!string.IsNullOrEmpty(relativePath) && _expandedNodeDict.ContainsKey(relativePath))
            {
                Dictionary<string, ExpandedDatum> expandedNodes = _expandedNodeDict[relativePath];

                if (expandedNodes.ContainsKey(id))
                {
                    return expandedNodes[id].expandedConnectors.Contains(connector);
                }
            }

            return false;
        }

        public static bool SetExpandedConnector(NodeViewData nvd, string connector, bool isConnectorExpanded)
        {
            Debug.Check(nvd != null && nvd.Node != null);
            if (nvd != null && nvd.Node != null)
            {
                Nodes.Behavior behavior = nvd.Node.Behavior as Nodes.Behavior;

                if (behavior != null)
                {
                    behavior = behavior.GetTopBehavior();
                }

                if (behavior != null && !string.IsNullOrEmpty(behavior.Filename))
                {
                    return SetExpandedConnector(behavior.RelativePath, nvd.Node.Id.ToString(), connector, isConnectorExpanded);
                }
            }

            return false;
        }

        public static bool SetExpandedConnector(string relativePath, string fullId, string connector, bool isConnectorExpanded)
        {
            if (!string.IsNullOrEmpty(relativePath))
            {
                if (!_expandedNodeDict.ContainsKey(relativePath))
                {
                    _expandedNodeDict[relativePath] = new Dictionary<string, ExpandedDatum>();
                }

                Dictionary<string, ExpandedDatum> expandedNodes = _expandedNodeDict[relativePath];

                if (!expandedNodes.ContainsKey(fullId))
                {
                    expandedNodes[fullId] = new ExpandedDatum();
                }

                ExpandedDatum datum = expandedNodes[fullId];

                if (isConnectorExpanded)
                {
                    if (!datum.expandedConnectors.Contains(connector))
                    {
                        datum.expandedConnectors.Add(connector);
                        return true;
                    }

                }
                else
                {
                    return datum.expandedConnectors.Remove(connector);
                }
            }

            return false;
        }

        public static void Serialize(Stream stream, BinaryFormatter formatter)
        {
            formatter.Serialize(stream, _expandedNodeDict);
        }

        public static void Deserialize(Stream stream, BinaryFormatter formatter)
        {
            Clear();

            try
            {
                Dictionary<string, Dictionary<string, ExpandedDatum>> nodes = formatter.Deserialize(stream) as Dictionary<string, Dictionary<string, ExpandedDatum>>;

                if (nodes != null)
                {
                    _expandedNodeDict = nodes;
                }

            }
            catch
            {
            }
        }
    }
}
