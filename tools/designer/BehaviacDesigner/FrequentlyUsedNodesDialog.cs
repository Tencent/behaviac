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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public partial class FrequentlyUsedNodesDialog : Form
    {
        public FrequentlyUsedNodesDialog(TreeView treeView)
        {
            InitializeComponent();

            initialize(treeView);
        }

        private bool isFoldNode(TreeNode treeNode)
        {
            return (treeNode != null && treeNode.ImageIndex == (int)NodeIcon.FolderClosed);
        }

        private void setCheckedNodes(TreeNodeCollection nodes, List<string> checkedNodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (!node.Checked && node.Tag is NodeTag)
                {
                    NodeTag nodetag = (NodeTag)node.Tag;

                    if (nodetag.NodeType != null && checkedNodes.Contains(nodetag.NodeType.FullName))
                    {
                        node.Checked = true;
                    }
                }

                setCheckedNodes(node.Nodes, checkedNodes);
            }
        }

        private void initialize(TreeView treeView)
        {
            this.nodeTreeView.ImageList = treeView.ImageList;

            this.nodeTreeView.Nodes.Clear();

            IList<NodeGroup> nodeGroups = Plugin.NodeGroups;

            foreach (NodeGroup group in nodeGroups)
            {
                group.Register(this.nodeTreeView.Nodes);
            }

            if (this.nodeTreeView.GetNodeCount(false) > 0)
            {
                UIUtilities.SortTreeview(this.nodeTreeView.Nodes);
                this.nodeTreeView.SelectedNode = this.nodeTreeView.Nodes[0];
            }

            setCheckedNodes(this.nodeTreeView.Nodes, Plugin.FrequentlyUsedNodes);
        }

        private void collectAllSelectedNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked && !isFoldNode(node) && node.Tag is NodeTag)
                {
                    NodeTag nodetag = (NodeTag)node.Tag;

                    if (nodetag.NodeType != null && !Plugin.FrequentlyUsedNodes.Contains(nodetag.NodeType.FullName))
                    {
                        Plugin.FrequentlyUsedNodes.Add(nodetag.NodeType.FullName);
                    }
                }

                collectAllSelectedNodes(node.Nodes);
            }
        }

        private void nodeTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (isFoldNode(e.Node))
            {
                foreach (TreeNode node in e.Node.Nodes)
                {
                    node.Checked = e.Node.Checked;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Plugin.FrequentlyUsedNodes.Clear();

            collectAllSelectedNodes(this.nodeTreeView.Nodes);

            Settings.Default.FrequentlyUsedNodes.Clear();

            foreach (string node in Plugin.FrequentlyUsedNodes)
            {
                Settings.Default.FrequentlyUsedNodes.Add(node);
            }
        }
    }
}
