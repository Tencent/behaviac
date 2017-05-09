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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Nodes;
using Behaviac.Design.Data;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public partial class NodeTreeList : UserControl
    {
        private int _defaultIconCount = 0;
        private Timer toolTipTimer = new Timer();
        private TreeNode toolTipNode = null;

        public NodeTreeList()
        {
            InitializeComponent();

            _defaultIconCount = this.imageList.Images.Count;

            this.toolTipTimer.Interval = 1000;
            this.toolTipTimer.Tick += new EventHandler(toolTipTimer_Tick);

            this.Disposed += new EventHandler(NodeTreeList_Disposed);
        }

        private void NodeTreeList_Disposed(object sender, EventArgs e)
        {
            AgentInstancePool.AddInstanceHandler -= AgentInstancePool_AddInstanceHandler;
            FrameStatePool.AddPlanningHanlder -= AgentInstancePool_AddPlanningHandler;
            Plugin.DebugAgentHandler -= DebugAgentInstance_SetHandler;
        }

        internal TreeView TreeView
        {
            get
            {
                return this.treeView;
            }
        }

        internal ImageList ImageList
        {
            get
            {
                return this.imageList;
            }
        }

        internal void Clear()
        {
            this.treeView.Nodes.Clear();

            for (int i = _defaultIconCount; i < this.imageList.Images.Count; ++i)
            {
                this.imageList.Images.RemoveAt(i);
                --i;
            }
        }

        internal void SetNodeList()
        {
            bool isDesignMode = (Plugin.EditMode == EditModes.Design);

            this.showSelectedNodeButton.Visible = isDesignMode;
            this.settingButton.Visible = isDesignMode;
            this.separator3.Visible = isDesignMode;
            this.debugLabel.Visible = !isDesignMode;
            this.cancelButton.Visible = !isDesignMode;

            AgentInstancePool.AddInstanceHandler -= AgentInstancePool_AddInstanceHandler;
            FrameStatePool.AddPlanningHanlder -= AgentInstancePool_AddPlanningHandler;

            if (!isDesignMode)
            {
                this.treeView.Nodes.Clear();

                setAgentTreeNode(this.treeView.Nodes);

                this.treeView.ExpandAll();

                this.treeView.SelectedNode = this.getFirstLeaf(this.treeView.Nodes);
                setDebugInstance();

                AgentInstancePool.AddInstanceHandler += AgentInstancePool_AddInstanceHandler;
                FrameStatePool.AddPlanningHanlder += AgentInstancePool_AddPlanningHandler;

                Plugin.DebugAgentHandler -= DebugAgentInstance_SetHandler;
                Plugin.DebugAgentHandler += DebugAgentInstance_SetHandler;
            }
        }

        private TreeNode getFirstLeaf(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count == 0)
                {
                    return node;
                }

                TreeNode leaf = getFirstLeaf(node.Nodes);

                if (leaf != null)
                {
                    return leaf;
                }
            }

            return null;
        }

        private void DebugAgentInstance_SetHandler(string agentName)
        {
            if (string.IsNullOrEmpty(agentName))
            {
                this.debugLabel.Text = Resources.DebugOperation;
            }

            else
            {
                this.debugLabel.Text = agentName;
            }
        }

        private void setAgentTreeNode(TreeNodeCollection treeNodes)
        {
            foreach (AgentType agent in Plugin.AgentTypes)
            {
                if (!agent.IsInherited)
                {
                    string agentType = agent.ToString();
                    List<string> instances = AgentInstancePool.GetInstances(agentType);

                    if (instances != null && instances.Count > 0)
                    {
                        TreeNode agentTreeNode = treeNodes.Add(agentType, agentType, (int)NodeIcon.FolderClosed, (int)NodeIcon.FolderClosed);

                        foreach (string instance in instances)
                        {
                            agentTreeNode.Nodes.Add(agentType + "#" + instance, instance, (int)NodeIcon.FlagRed, (int)NodeIcon.FlagRed);
                        }
                    }
                }
            }
        }

        private void AgentInstancePool_AddPlanningHandler(string agentFullName, int frame, FrameStatePool.PlanningProcess planning)
        {
            string[] types = agentFullName.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Check(types.Length == 2);
            string agentType = types[0];
            string agentName = types[1];

            TreeNode agentTreeNode = this.treeView.Nodes[agentType];
            Debug.Check(agentTreeNode != null);

            TreeNode[] agentInstance = agentTreeNode.Nodes.Find(agentFullName, true);

            if (agentInstance != null)
            {
                string planningName = string.Format("PLanning_{0}_{1}", planning._frame, planning._index);
                TreeNode planningNode = agentInstance[0].Nodes.Insert(agentInstance[0].GetNodeCount(false), planningName);

                planningNode.Tag = planning;
            }
        }

        private void AgentInstancePool_AddInstanceHandler(string agentType, string agentName)
        {
            if (!this.treeView.Nodes.ContainsKey(agentType))
            {
                this.treeView.Nodes.Add(agentType, agentType, (int)NodeIcon.FolderClosed, (int)NodeIcon.FolderClosed);
            }

            TreeNode agentTreeNode = this.treeView.Nodes[agentType];

            if (agentTreeNode != null)
            {
                string agentKey = agentType + "#" + agentName;

                if (!agentTreeNode.Nodes.ContainsKey(agentKey))
                {
                    agentTreeNode.Nodes.Insert(agentTreeNode.GetNodeCount(false), agentKey, agentName, (int)NodeIcon.FlagRed, (int)NodeIcon.FlagRed);
                }

                agentTreeNode.Expand();
            }

            //if (string.IsNullOrEmpty(Plugin.DebugAgentInstance))
            //{
            //    this.treeView.SelectedNode = curNode;

            //    setDebugInstance();
            //}
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (Plugin.EditMode == EditModes.Design)
            {
                return;
            }

            Plugin.DebugAgentInstance = string.Empty;
        }

        private bool isFoldNode(TreeNode treeNode)
        {
            return (treeNode != null && treeNode.ImageIndex == (int)NodeIcon.FolderClosed);
        }

        private void treeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            TreeNode treeNode = this.treeView.SelectedNode;

            if (treeNode == null || isFoldNode(treeNode))
            {
                return;
            }

            if (Plugin.EditMode == EditModes.Design)
            {
                documentMenuItem.Visible = true;
                debugMenuItem.Visible = false;
                separator.Visible = false;
                deleteMenuItem.Visible = false;
                deleteAllMenuItem.Visible = false;
                separator2.Visible = false;
                showPropMenuItem.Visible = false;
                showPlanningMenuItem.Visible = false;
            }
            else
            {
                documentMenuItem.Visible = false;
                debugMenuItem.Visible = true;
                separator.Visible = true;
                deleteMenuItem.Visible = true;
                deleteAllMenuItem.Visible = true;
                separator2.Visible = true;
                showPropMenuItem.Visible = true;
                showPlanningMenuItem.Visible = true;
            }

            this.contextMenuStrip.Show(this, new Point(e.X, e.Y));
        }

        private void setDebugInstance()
        {
            if (Plugin.EditMode == EditModes.Design)
            {
                return;
            }

            TreeNode treeNode = this.treeView.SelectedNode;

            if (treeNode == null)
            {
                return;
            }

            if (treeNode.Tag != null && treeNode.Tag is FrameStatePool.PlanningProcess)
            {
                //planning
                FrameStatePool.PlanningProcess planning = treeNode.Tag as FrameStatePool.PlanningProcess;
                showPlanning(planning);
            }
            else if (!isFoldNode(treeNode))
            {
                //this.debugLabel.Text = "Debug : " + treeNode.Text;
                Plugin.DebugAgentInstance = treeNode.Name;

                ShowInstanceProperty();
            }
        }

        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            setDebugInstance();
        }

        private void expandButton_Click(object sender, EventArgs e)
        {
            this.treeView.ExpandAll();
        }

        private void collapseButton_Click(object sender, EventArgs e)
        {
            this.treeView.CollapseAll();
        }

        private void debugMenuItem_Click(object sender, EventArgs e)
        {
            setDebugInstance();
        }

        public void ShowInstanceProperty()
        {
            if (Plugin.EditMode == EditModes.Design)
            {
                return;
            }

            TreeNode treeNode = this.treeView.SelectedNode;

            if (treeNode == null)
            {
                return;
            }

            string agentInstanceName = treeNode.Name;
            FrameStatePool.PlanningState nodeState = null;

            if (treeNode.Tag != null && treeNode.Tag is FrameStatePool.PlanningProcess)
            {
                FrameStatePool.PlanningProcess planning = (FrameStatePool.PlanningProcess)treeNode.Tag;

                agentInstanceName = planning._agentFullName;
                nodeState = planning._rootState;
            }

            ParametersDock.Inspect(agentInstanceName, nodeState);
        }

        private void parameterMenuItem_Click(object sender, EventArgs e)
        {
            ShowInstanceProperty();
        }

        /// <summary>
        /// Handles when a tree node is dragged from the node explorer
        /// </summary>
        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode node = (TreeNode)e.Item;

                if (node.Tag is NodeTag)
                {
                    NodeTag nodetag = (NodeTag)node.Tag;

                    if (nodetag.Type == NodeTagType.Node || nodetag.Type == NodeTagType.Attachment)
                    {
                        DoDragDrop(e.Item, DragDropEffects.Move);
                    }
                }
            }
        }

        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            this.toolTipTimer.Stop();

            if (this.toolTipNode != null)
            {
                Point mousePos = this.treeView.PointToClient(Control.MousePosition);

                // Show the ToolTip if the mouse is still over the same node.
                if (this.toolTipNode.Bounds.Contains(mousePos))
                {
                    mousePos.X += 10;
                    mousePos.Y += 10;
                    this.toolTip.Show(this.toolTipNode.ToolTipText, this, mousePos);
                }
            }
        }

        private void treeView_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            if (this.toolTipNode != e.Node)
            {
                this.toolTipTimer.Stop();
                this.toolTip.Hide(this);

                this.toolTipNode = e.Node;
                this.toolTipTimer.Start();
            }
        }

        private void treeView_MouseLeave(object sender, EventArgs e)
        {
            this.toolTipNode = null;
            this.toolTipTimer.Stop();
            this.toolTip.Hide(this);
        }

        public void ToggleShowSelectedNodes(bool showSelectedNodes, TreeView root = null)
        {
            Plugin.OnlyShowFrequentlyUsedNodes = showSelectedNodes;
            Settings.Default.OnlyShowFrequentlyUsedNodes = Plugin.OnlyShowFrequentlyUsedNodes;

            Plugin.SetFrequentlyUsedNodeGroups();

            if (root == null)
            {
                root = this.treeView;
            }

            root.BeginUpdate();
            root.Nodes.Clear();

            IList<NodeGroup> nodeGroups = Plugin.OnlyShowFrequentlyUsedNodes ? Plugin.FrequentlyUsedNodeGroups : Plugin.NodeGroups;

            foreach (NodeGroup group in nodeGroups)
            {
                group.Register(root.Nodes);
            }

            if (root.GetNodeCount(false) > 0)
            {
                UIUtilities.SortTreeview(root.Nodes);
                root.SelectedNode = this.treeView.Nodes[0];
            }

            root.EndUpdate();
        }

        private void showSelectedNodeButton_Click(object sender, EventArgs e)
        {
            ToggleShowSelectedNodes(!Plugin.OnlyShowFrequentlyUsedNodes);
        }

        private void settingButton_Click(object sender, EventArgs e)
        {
            FrequentlyUsedNodesDialog dialog = new FrequentlyUsedNodesDialog(this.treeView);

            if (DialogResult.OK == dialog.ShowDialog())
            {
                ToggleShowSelectedNodes(true);
            }
        }

        private void showPlanningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = this.treeView.SelectedNode;

            if (treeNode == null)
            {
                return;
            }

            if (treeNode.Tag != null && treeNode.Tag is FrameStatePool.PlanningProcess)
            {
                FrameStatePool.PlanningProcess planning = treeNode.Tag as FrameStatePool.PlanningProcess;
                showPlanning(planning);
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            TreeNode treeNode = this.treeView.SelectedNode;

            if (treeNode == null)
            {
                return;
            }

            if (treeNode.Tag != null && treeNode.Tag is FrameStatePool.PlanningProcess)
            {
                this.debugMenuItem.Enabled = false;
                this.showPropMenuItem.Enabled = true;
                this.showPlanningMenuItem.Enabled = true;
            }
            else
            {
                this.debugMenuItem.Enabled = true;
                this.showPropMenuItem.Enabled = true;
                this.showPlanningMenuItem.Enabled = false;
            }
        }

        public static void ShowPlanning(string agentFullName, int frame, int index)
        {
            FrameStatePool.PlanningProcess planning = FrameStatePool.GetPlanning(agentFullName, frame, index);

            if (planning != null)
            {
                showPlanning(planning);
            }
        }

        private static void showPlanning(FrameStatePool.PlanningProcess planning)
        {
            BehaviorTreeView view = UIUtilities.ShowPlanning(planning);

            if (view != null)
            {
                view.ClickNode += new BehaviorTreeView.ClickNodeEventDelegate(Planning_ClikcNode);
            }
        }

        private static void Planning_ClikcNode(NodeViewData nvd)
        {
            if (nvd != null)
            {
                Behavior b = nvd.Node.Behavior as Behavior;

                if (b != null && b.PlanningProcess != null)
                {
                    b.AgentType.ResetPars(b.LocalVars);

                    FrameStatePool.PlanningState nodeState = b.PlanningProcess._rootState;

                    if (nvd.Parent != null)
                    {
                        nodeState = b.PlanningProcess.GetLastNode(nvd);
                    }

                    ParametersDock.Inspect(b.PlanningProcess._agentFullName, nodeState);
                }
            }
        }

        private void toolStrip_SizeChanged(object sender, EventArgs e)
        {
            if (Plugin.EditMode != EditModes.Design)
            {
                this.debugLabel.Visible = false;
                this.debugLabel.Width = this.toolStrip.Width - 140;
                this.debugLabel.Visible = true;
            }
        }

        private void showDoc()
        {
            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Tag is NodeTag)
                {
                    NodeTag nodetag = (NodeTag)treeView.SelectedNode.Tag;

                    if (nodetag.Defaults != null)
                    {
                        MainWindow.Instance.OpenURL(nodetag.Defaults.DocLink);
                    }
                }
            }
        }

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                showDoc();

                e.Handled = true;
            }
        }

        private void documentMenuItem_Click(object sender, EventArgs e)
        {
            showDoc();
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = this.treeView.SelectedNode;

            if (treeNode != null)
            {
                this.treeView.Nodes.Remove(treeNode);
            }
        }

        private void deleteAllMenuItem_Click(object sender, EventArgs e)
        {
            this.treeView.Nodes.Clear();
        }
    }
}
