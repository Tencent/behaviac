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
using System.Drawing.Imaging;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Attributes;
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;
using Behaviac.Design.Network;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    /// <summary>
    /// This control shows a graph of a behaviour and allows you to edit it.
    /// </summary>
    internal partial class BehaviorTreeView : UserControl
    {
        private Timer toolTipTimer = new Timer();

        public BehaviorTreeView()
        {
            InitializeComponent();

            this.Disposed += new EventHandler(BehaviorTreeView_Disposed);

            this.toolTipTimer.Interval = 1000;
            this.toolTipTimer.Tick += new EventHandler(toolTipTimer_Tick);
        }

        private void BehaviorTreeView_Disposed(object sender, EventArgs e)
        {
            this.Disposed -= BehaviorTreeView_Disposed;

            if (this.toolTip != null)
            {
                this.toolTip.Dispose();
                this.toolTip = null;
            }
        }

        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            this.toolTipTimer.Stop();

            if (_currentNode != null && this.toolTip != null)
            {
                if (_currentExpandNode != null)
                {
                    if (_nodeToolTip == Resources.ExpandAllInfo)
                    {
                        this.toolTip.Show(_nodeToolTip, this, new Point((int)_currentNode.DisplayBoundingBox.X - 20, (int)_currentNode.DisplayBoundingBox.Y - 5 - 18));
                    }
                    else
                    {
                        this.toolTip.Show(_nodeToolTip, this, new Point((int)(_currentNode.DisplayBoundingBox.X + _currentNode.DisplayBoundingBox.Width) - 20, (int)_currentNode.DisplayBoundingBox.Y - 5 - 18));
                    }
                }
                else
                {
                    string[] tokens = _nodeToolTip.Split('\n');
                    this.toolTip.Show(_nodeToolTip, this, new Point((int)_currentNode.DisplayBoundingBox.X - 20, (int)_currentNode.DisplayBoundingBox.Y - 5 - 18 * tokens.Length));
                }
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            this.ReadOnly = (Plugin.EditMode != EditModes.Design);

            ParentForm.FormClosing += ParentForm_FormClosing;
            DesignerPropertyEditor.PropertyChanged += RefreshProperty;
            UndoQueue.ModifyNodeHandler += UndoQueue_ModifyNodeHandler;
        }

        private bool _saveBehaviorWhenClosing = true;
        public bool SaveBehaviorWhenClosing
        {
            set
            {
                _saveBehaviorWhenClosing = value;
            }
        }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ParentForm.Visible || CloseReason.UserClosing != e.CloseReason)
            {
                return;
            }

            if (_saveBehaviorWhenClosing)
            {
                bool[] result;
                List<BehaviorNode> behaviors = new List<BehaviorNode>()
                {
                    RootNode
                };
                e.Cancel = (FileManagers.SaveResult.Cancelled == MainWindow.Instance.SaveBehaviors(behaviors, out result));

                if (e.Cancel)
                {
                    return;
                }
            }

            ParentForm.FormClosing -= ParentForm_FormClosing;
            DesignerPropertyEditor.PropertyChanged -= RefreshProperty;
            UndoQueue.ModifyNodeHandler -= UndoQueue_ModifyNodeHandler;
            _rootNodeView.RootBehavior.WasModified -= nodeViewData_WasModified;
            _rootNodeView.RootBehavior.AgentTypeChanged -= agentTypeChanged;
        }

        public bool ReadOnly
        {
            set
            {
                bool disabled = !value;
                saveButton.Enabled = disabled;
                exportButton.Enabled = disabled;
                checkButton.Enabled = disabled;
                saveAsButton.Enabled = disabled;
                imageButton.Enabled = disabled;
                //propertiesButton.Enabled = disabled;
                parameterSettingButton.Enabled = disabled;
            }
        }

        /// <summary>
        /// Registers referenced behaviours so when a referenced behaviour changes, this view gets updated, as the root node will not change.
        /// </summary>
        /// <param name="processedBehaviors">A list of processed behaviours to avoid circular references.</param>
        /// <param name="node">The node we want to register on and its children.</param>
        private void RegisterReferencedBehaviors(ProcessedBehaviors processedBehaviors, Node node)
        {
            if (!processedBehaviors.MayProcess(node))
            {
                return;
            }

            // check the children
            foreach (Node child in node.Children)
            {
                RegisterReferencedBehaviors(processedBehaviors.Branch(child), child);
            }
        }

        /// <summary>
        /// Holds the instance of the error check dialogue for this view/behaviour.
        /// </summary>
        private static ErrorCheckDialog _errorCheckDialog = null;

        /// <summary>
        /// The node layout manager used to layout the behaviour and its children.
        /// </summary>
        private NodeLayoutManager _nodeLayoutManager;

        /// <summary>
        /// The pen used to draw the edges connecting the nodes.
        /// </summary>
        private Pen _edgePen = new Pen(Brushes.Snow, 3.0f);

        /// <summary>
        /// The pen used to draw the edges when being selected.
        /// </summary>
        private Pen _edgePenSelected = new Pen(Brushes.Gold, 3.0f);

        /// <summary>
        /// The pen used to draw the edges when highlighting.
        /// </summary>
        private Pen _edgePenHighlighted = new Pen(Brushes.GreenYellow, 4.0f);

        /// <summary>
        /// The pen used to draw the edges when updating.
        /// </summary>
        private Pen _edgePenUpdate = new Pen(Brushes.Olive, 3.0f);

        /// <summary>
        /// The pen used to draw the edges connecting sub-referenced nodes.
        /// </summary>
        private Pen _edgePenReadOnly = new Pen(Brushes.LightGray, 3.0f);

        private NodeViewData _rootNodeView;
        public NodeViewData RootNodeView
        {
            get
            {
                return _rootNodeView;
            }
        }

        /// <summary>
        /// The behaviour visualised in this view.
        /// </summary>
        public Nodes.BehaviorNode RootNode
        {
            get
            {
                return (_rootNodeView != null) ? _rootNodeView.RootBehavior : null;
            }

            set
            {
                try
                {
                    // assign the new behaviour
                    _rootNodeView = ((Node)value).CreateNodeViewData(null, value);
                    _nodeLayoutManager = new NodeLayoutManager(_rootNodeView, _edgePen, _edgePenSelected, _edgePenHighlighted, _edgePenUpdate, _edgePenReadOnly, false);

                    // register the view to be updated when any referenced behaviour changes
                    ProcessedBehaviors processedBehaviors = new ProcessedBehaviors();
                    RegisterReferencedBehaviors(processedBehaviors, _rootNodeView.Node);

                    // automtically centre the behaviour
                    _pendingCenterBehavior = true;

                    _rootNodeView.RootBehavior.WasModified += nodeViewData_WasModified;
                    _rootNodeView.RootBehavior.AgentTypeChanged += agentTypeChanged;
                }
                catch
                {
                }
            }
        }

        private void UndoQueue_ModifyNodeHandler(Nodes.BehaviorNode behavior, bool reference)
        {
            if (this.RootNode == behavior)
            {
                //_behaviorTreeList.TriggerShowBehavior(behavior);
                this.Redraw();

                if (!reference)
                {
                    SelectedNode = null;
                    PropertiesDock.InspectObject(this.RootNode, null);

                    if (this.RootNode.AgentType != null)
                    {
                        this.RootNode.AgentType.ResetPars(((Behavior)this.RootNode).LocalVars);
                    }

                    if (MetaStoreDock.IsVisible())
                    {
                        MetaStoreDock.Inspect(this.Parent as BehaviorTreeViewDock);
                    }

                    this.ParentForm.Show();
                    this.ParentForm.Focus();
                }
            }
        }

        /// <summary>
        /// Handles when the root node changes.
        /// </summary>
        /// <param name="node">The node which was changed, in our case always the root node.</param>
        private void nodeViewData_WasModified(BehaviorNode root, Node node)
        {
            LayoutChanged();

            if (this.RootNodeView != null)
            {
                List<NodeViewData> allNodeViewDatas = new List<NodeViewData>();
                this.RootNodeView.FindNodeViewDatas(node, ref allNodeViewDatas);

                if (allNodeViewDatas.Count > 0)
                {
                    foreach (NodeViewData nvd in allNodeViewDatas)
                    {
                        nvd.OnNodeModified(root, node);
                    }
                }
            }
        }

        private PointF _startMousePosition;
        private PointF _lastMousePosition;

        /// <summary>
        /// Used to prevent any movment in the graph when the focus was lost.
        /// </summary>
        private bool _lostFocus = false;

        /// <summary>
        /// Defines if the last mouse position should be kept.
        /// </summary>
        private bool _maintainMousePosition = false;

        /// <summary>
        /// Defines if the position of a node should be kept.
        /// </summary>
        private NodeViewData _maintainNodePosition = null;

        private NodeViewData _selectedNode = null;

        /// <summary>
        /// The currently selected node.
        /// </summary>
        public NodeViewData SelectedNode
        {
            get
            {
                return _selectedNode;
            }

            set
            {
                if (_selectedNode != value)
                {
                    // set new selected node and update the graph
                    _selectedNode = value;

                    propertiesButton.Enabled = _selectedNode != null;

                    Invalidate();
                }
            }
        }

        HighlightBreakPoint _highlightBreakPoint = null;
        private List<string> _highlightedNodeIds = null;
        private List<string> _updatedNodeIds = null;
        private List<string> _highlightedTransitionIds = null;
        private Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> _profileInfos = null;

        public void ClearHighlights()
        {
            _highlightBreakPoint = null;
            _highlightedNodeIds = null;
            _updatedNodeIds = null;
            _highlightedTransitionIds = null;
            _profileInfos = null;

            Invalidate();
        }

        public void ClearHighlightBreakPoint()
        {
            _highlightBreakPoint = null;

            Invalidate();
        }

        public void SetHighlights(List<string> highlightedTransitionIds, List<string> highlightedNodeIds, List<string> updatedNodeIds, HighlightBreakPoint highlightBreakPoint, Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> profileInfos)
        {
            bool shouldRefresh = (_highlightBreakPoint != highlightBreakPoint);

            if (shouldRefresh && highlightBreakPoint != null)
            {
                SelectedNode = RootNodeView.FindNodeViewData(highlightBreakPoint.NodeId);

                if (ClickNode != null)
                {
                    ClickNode(SelectedNode);
                }
            }

            if (!shouldRefresh)
            {
                shouldRefresh = !compareTwoLists(_highlightedTransitionIds, highlightedTransitionIds);
            }

            if (!shouldRefresh)
            {
                shouldRefresh = !compareTwoLists(_updatedNodeIds, updatedNodeIds);
            }

            if (!shouldRefresh)
            {
                shouldRefresh = !compareTwoLists(_highlightedNodeIds, highlightedNodeIds);
            }

            if (!shouldRefresh)
            {
                shouldRefresh = !compareTwoDicts(_profileInfos, profileInfos);
            }

            _highlightedTransitionIds = highlightedTransitionIds;
            _highlightBreakPoint = highlightBreakPoint;
            _highlightedNodeIds = highlightedNodeIds;
            _updatedNodeIds = updatedNodeIds;
            _profileInfos = profileInfos;

            if (shouldRefresh)
            {
                List<string> allExpandedIds = new List<string>();

                if (_highlightedNodeIds != null)
                {
                    allExpandedIds.AddRange(_highlightedNodeIds);
                }

                if (_updatedNodeIds != null)
                {
                    foreach (string id in _updatedNodeIds)
                    {
                        if (!allExpandedIds.Contains(id))
                        {
                            allExpandedIds.Add(id);
                        }
                    }
                }

                bool layoutChanged = false;

                foreach (string id in allExpandedIds)
                {
                    NodeViewData nvd = RootNodeView.FindNodeViewData(id);

                    if (nvd != null && !nvd.CheckAllParentsExpanded())
                    {
                        nvd.SetAllParentsExpanded();
                        this._pendingCenterBehavior = true;
                        layoutChanged = true;
                    }
                }

                if (layoutChanged)
                {
                    LayoutChanged();
                }

                Refresh();
            }
        }

        private bool compareTwoLists(List<string> strA, List<string> strB)
        {
            if (strA == strB)
            {
                return true;
            }

            if (strA == null)
            {
                return (strB == null);
            }

            if (strB == null || strA.Count != strB.Count)
            {
                return false;
            }

            foreach (string str in strB)
            {
                if (!strA.Contains(str))
                {
                    return false;
                }
            }

            return true;
        }

        private bool compareTwoDicts(Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> dictA, Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> dictB)
        {
            if (dictA == dictB)
            {
                return true;
            }

            if (dictA == null)
            {
                return (dictB == null);
            }

            if (dictB == null || dictA.Count != dictB.Count)
            {
                return false;
            }

            foreach (string key in dictB.Keys)
            {
                if (!dictA.ContainsKey(key) || !dictA[key].Equals(dictB[key]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Stores the node you want to be selected when no layout currenty exists for the node.
        /// </summary>
        private Node _selectedNodePending = null;
        internal Node SelectedNodePending
        {
            set
            {
                _selectedNodePending = value;
            }
        }

        private Attachments.Attachment _selectedAttachmentPending = null;
        internal Attachments.Attachment SelectedAttachmentPending
        {
            set
            {
                _selectedAttachmentPending = value;
            }
        }

        /// <summary>
        /// Stores the parent of the node you want to be selected when no layout currenty exists for the node.
        /// </summary>
        private NodeViewData _selectedNodePendingParent = null;

        private enum FSMDragModes
        {
            kNone,
            kLeft,
            kRight
        }

        private FSMDragModes _fsmDragMode = FSMDragModes.kNone;
        private NodeViewData.SubItem _fsmSubItem = null;
        private RectangleF _fsmItemBoundingBox = RectangleF.Empty;

        /// <summary>
        /// The node the mouse is currently hovering over.
        /// </summary>
        private NodeViewData _currentNode = null;

        private NodeViewData _currentExpandNode = null;
        private string _nodeToolTip = "";

        /// <summary>
        /// The sub item attachment which the mouse is currently dragged.
        /// </summary>
        private NodeViewData.SubItemAttachment _dragAttachment = null;

        private NodeViewData.SubItemAttachment _dragTargetAttachment = null;

        /// <summary>
        /// The defaults of the node which is dragged in from the node explorer. This is needed for the child mode data.
        /// </summary>
        private DefaultObject _dragNodeDefaults = null;

        /// <summary>
        /// The node another node is dragged over which should always be the same as _currentNode.
        /// </summary>
        private NodeViewData _dragTargetNode = null;

        /// <summary>
        /// The connector another node is dragged over.
        /// </summary>
        private Node.Connector _dragTargetConnector = null;

        /// <summary>
        /// The way the dragged node is supposed to be attached to the _dragTargetNode.
        /// </summary>
        private NodeAttachMode _dragAttachMode = NodeAttachMode.None;

        private BehaviorTreeList _behaviorTreeList;

        /// <summary>
        /// The BehaviorTreeList of the editor which manages all the behaviours.
        /// </summary>
        internal BehaviorTreeList BehaviorTreeList
        {
            get
            {
                return _behaviorTreeList;
            }

            set
            {
                _behaviorTreeList = value;

                // update the status of the buttons
                bool isDesignMode = (Plugin.EditMode == EditModes.Design);
                saveButton.Enabled = isDesignMode & _behaviorTreeList.HasFileManagers();
                saveAsButton.Enabled = isDesignMode & _behaviorTreeList.HasFileManagers();
                exportButton.Enabled = isDesignMode & _behaviorTreeList.HasExporters();
            }
        }

        /// <summary>
        /// The padding between the nodes.
        /// </summary>
        internal SizeF NodePadding
        {
            get
            {
                return _nodeLayoutManager.Padding;
            }
            set
            {
                _nodeLayoutManager.Padding = value;
            }
        }

        private bool _forceChangeLayout = false;

        /// <summary>
        /// Marks the current layout as obsolete and causes the view to redraw the graph.
        /// </summary>
        private void LayoutChanged()
        {
            _nodeLayoutManager.MarkLayoutChanged();
            Invalidate();
        }

        public void Redraw()
        {
            _forceChangeLayout = true;
            LayoutChanged();
        }

        /// <summary>
        /// The way a dragged node is supposed to be attached to another node.
        /// </summary>
        private enum NodeAttachMode { None, Left, Right, Top, Bottom, Attachment, Center };

        //return true if rootBehavior's agent type is derived from the agent type of childBehavior.
        private static bool IsCompatibleAgentType(Behavior rootBehavior, Behavior childBehavior)
        {
            if (rootBehavior != null && childBehavior != null && rootBehavior.AgentType != null && childBehavior.AgentType != null)
            {
                string rootBTAgent = rootBehavior.AgentType.ToString();
                string childBTAgent = childBehavior.AgentType.ToString();

                // the agent type specified at root bt should be derived from the agent type at child bt
                if (!Plugin.IsAgentDerived(rootBTAgent, childBTAgent) && !Plugin.IsAgentDerived(childBTAgent, rootBTAgent))
                {
                    string errorMsg = string.Format(Resources.AgentErrorInfo, rootBTAgent, rootBehavior.Label, childBTAgent, childBehavior.Label);
                    MessageBox.Show(errorMsg, Resources.DesignerError, MessageBoxButtons.OK);
                    return false;
                }
            }

            return true;
        }

        private Attachments.Attachment addFSMNode(Node node, PointF mousePos)
        {
            if (node != null && node.IsFSM)
            {
                node.ScreenLocation = _nodeLayoutManager.ViewToGraph(mousePos);

                Node rootNode = _rootNodeView.RootBehavior as Node;
                rootNode.AddFSMNode(node);

                if (rootNode.FSMNodes.Count == 1)
                {
                    Attachments.Attachment startCondition = null;

                    foreach (Attachments.Attachment attachment in rootNode.Attachments)
                    {
                        if (attachment.IsStartCondition)
                        {
                            startCondition = attachment;
                            break;
                        }
                    }

                    if (startCondition == null)
                    {
                        startCondition = node.CreateStartCondition(rootNode);

                        if (startCondition != null)
                        {
                            rootNode.AddAttachment(startCondition);
                        }
                    }

                    return startCondition;
                }
            }

            return null;
        }

        /// <summary>
        /// Attaches a dragged node from the node explorer to an existing node.
        /// </summary>
        /// <param name="nvd">The node the new node will be attached to.</param>
        /// <param name="mode">The way the new node will be attached.</param>
        /// <param name="nodetag">The tag of the you want to create.</param>
        /// <param name="label">The label of the new node.</param>
        private void InsertNewNode(NodeViewData nvd, NodeAttachMode mode, NodeTag nodetag, PointF mousePos)
        {
            if (nodetag.Type != NodeTagType.Behavior && nodetag.Type != NodeTagType.Prefab && nodetag.Type != NodeTagType.Node)
            {
                throw new Exception("Only behaviours, prefabs and nodes can be attached to a behaviour tree");
            }

            Node newnode = null;
            bool isPrefabInstance = false;

            // when we attach a behaviour we must create a special referenced behaviour node
            if (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.Prefab)
            {
                if (!File.Exists(nodetag.Filename))
                {
                    if (!SaveBehavior(nodetag.Filename))
                    {
                        return;
                    }
                }

                // get the behavior we want to reference
                BehaviorNode behavior = _behaviorTreeList.LoadBehavior(nodetag.Filename);

                // a behaviour reference itself
                if (nvd != null && nvd.IsBehaviorReferenced(behavior))
                {
                    if (DialogResult.Cancel == MessageBox.Show(Resources.CircularReferencedInfo, Resources.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                    {
                        return;
                    }
                }

                if (nodetag.Type == NodeTagType.Prefab)
                {
                    behavior = (BehaviorNode)behavior.Clone();
                }

                Behavior rootB = _rootNodeView.RootBehavior as Behavior;
                Behavior b = behavior as Behavior;

                if (rootB.AgentType == null)
                {
                    rootB.AgentType = b.AgentType;

                }
                else if (!IsCompatibleAgentType(rootB, b))
                {
                    return;
                }

                // behavior
                if (nodetag.Type == NodeTagType.Behavior)
                {
                    // create the referenced behaviour node for the behaviour
                    ReferencedBehavior refnode = Node.CreateReferencedBehaviorNode(_rootNodeView.RootBehavior, behavior, mode == NodeAttachMode.None || ((Node)this.RootNode).IsFSM);

                    newnode = (Node)refnode;

                    if (newnode != null)
                    {
                        //the comment seems too long to overlap the node
                        //newnode.CommentText = Resources.ThisIsReferenceTree;
                        newnode.CommentBackground = Node.CommentColor.Gray;
                    }
                }

                // prefab
                else
                {
                    // Copy all Pars from the prefab file into the current behavior node.
                    List<ParInfo> pars = new List<ParInfo>();

                    foreach (ParInfo par in b.LocalVars)
                    {
                        bool found = false;

                        foreach (ParInfo rootPar in rootB.LocalVars)
                        {
                            if (par.Name == rootPar.Name)
                            {
                                if (par.Type != rootPar.Type)
                                {
                                    string errorMsg = string.Format(Resources.ParErrorInfo, par.Name, b.Label, rootB.Label);
                                    MessageBox.Show(errorMsg, Resources.LoadError, MessageBoxButtons.OK);
                                    return;
                                }

                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            pars.Add(par);
                        }
                    }

                    rootB.LocalVars.AddRange(pars);

                    // The first child should be the root node of the prefab tree.
                    Node behaviorNode = (Node)behavior;

                    if (behaviorNode.Children.Count > 0)
                    {
                        newnode = (Node)behaviorNode.Children[0];

                        string prefab = Path.GetFileNameWithoutExtension(behavior.Filename);
                        newnode.CommentText = string.Format("Prefab[{0}]", prefab);

                        isPrefabInstance = true;
                        string prefabName = FileManagers.FileManager.GetRelativePath(behavior.Filename);
                        newnode.SetPrefab(prefabName);
                    }
                }

            }
            else
            {
                // simply create the node which is supposed to be created.
                newnode = Node.Create(nodetag.NodeType);

                if (newnode != null)
                {
                    newnode.PostCreatedByEditor();
                }
            }

            if (newnode == null)
            {
                return;
            }

            // update label
            newnode.OnPropertyValueChanged(false);

            Node node = (nvd != null) ? nvd.Node : null;

            if (node == null)
            {
                mode = NodeAttachMode.None;
            }

            Attachments.Attachment startCondition = null;

            // attach the new node with the correct mode
            switch (mode)
            {
                    // the new node is inserted in front of the target node
                case NodeAttachMode.Left:
                    if (node != null)
                    {
                        Node parent = (Node)node.Parent;

                        int k = node.ParentConnector.GetChildIndex(node);

                        Node.Connector conn = node.ParentConnector;
                        Debug.Check(conn != null);

                        parent.RemoveChild(conn, node);
                        parent.AddChild(conn, newnode, k);

                        Node.Connector newconn = newnode.GetConnector(conn.Identifier);
                        Debug.Check(newconn != null);
                        newnode.AddChild(newconn, node);

                        // automatically select the new node
                        _selectedNodePending = newnode;
                        _selectedNodePendingParent = nvd.Parent;
                    }

                    break;

                    // the new node is simply added to the target node's children
                case NodeAttachMode.Right:
                    if (newnode != null && newnode.IsFSM)
                    {
                        startCondition = this.addFSMNode(newnode, mousePos);

                        newnode.ScreenLocation = new PointF(_rootNodeView.BoundingBox.Left + _rootNodeView.BoundingBox.Width * 1.5f, _rootNodeView.BoundingBox.Top);

                    }
                    else if (node != null)
                    {
                        node.AddChild(_dragTargetConnector, newnode);

                        _dragTargetConnector.IsExpanded = true;

                        // automatically select the new node
                        _selectedNodePending = newnode;
                        _selectedNodePendingParent = nvd;
                    }

                    break;

                    // the new node is placed above the target node
                case NodeAttachMode.Top:
                    if (node != null)
                    {
                        int n = _dragTargetNode.Node.ParentConnector.GetChildIndex(node);
                        ((Node)node.Parent).AddChild(_dragTargetNode.Node.ParentConnector, newnode, n);

                        // automatically select the new node
                        _selectedNodePending = newnode;
                        _selectedNodePendingParent = nvd.Parent;
                    }

                    break;

                    // the new node is placed below the target node
                case NodeAttachMode.Bottom:
                    if (node != null)
                    {
                        int m = _dragTargetNode.Node.ParentConnector.GetChildIndex(node);
                        ((Node)node.Parent).AddChild(_dragTargetNode.Node.ParentConnector, newnode, m + 1);

                        // automatically select the new node
                        _selectedNodePending = newnode;
                        _selectedNodePendingParent = nvd.Parent;
                    }

                    break;

                    // the node will replace the target node
                case NodeAttachMode.Center:
                    if (node != null && newnode != null && newnode.ReplaceNode(node))
                    {
                        // automatically select the new node
                        _selectedNodePending = newnode;
                        _selectedNodePendingParent = nvd.Parent;

                        this.Redraw();
                    }

                    break;

                case NodeAttachMode.None:
                    if (newnode != null && newnode.IsFSM)
                    {
                        startCondition = this.addFSMNode(newnode, mousePos);

                        // automatically select the new node
                        _selectedNodePending = newnode;
                        _selectedNodePendingParent = this.RootNodeView;
                    }

                    break;
            }

            // After being created, its Id should be reset.
            if (newnode != null)
            {
                newnode.ResetId(true);

                // set the prefab dirty for the current parent
                if (newnode.Parent != null)
                {
                    Node parent = (Node)newnode.Parent;

                    if (!string.IsNullOrEmpty(parent.PrefabName))
                    {
                        parent.HasOwnPrefabData = true;

                        if (!isPrefabInstance)
                        {
                            newnode.SetPrefab(parent.PrefabName);
                            newnode.HasOwnPrefabData = true;
                        }
                    }
                }

                if (startCondition != null)
                {
                    startCondition.TargetFSMNodeId = newnode.Id;
                }

                UndoManager.Save(this.RootNode);
            }

            // the layout needs to be recalculated
            LayoutChanged();
        }

        /// <summary>
        /// Handles when a referenced behaviour is modified.
        /// </summary>
        /// <param name="node">The referenced behaviour node whose referenced behaviour is modified.</param>
        void refnode_ReferencedBehaviorWasModified(ReferencedBehavior node)
        {
            LayoutChanged();
        }

        /// <summary>
        /// The position which will be kept when _maintainMousePosition or _maintainNodePosition is set.
        /// </summary>
        private PointF _graphOrigin = new PointF(0.0f, 0.0f);

        private bool AdoptNodeByAncestor(BaseNode target, BaseNode node)
        {
            if (target != null && target.CanAdopt(node))
            {
                target = target.Parent;

                // check if there is a Method node on the parent path
                while (target != null)
                {
                    if (target is Nodes.Method)
                    {
                        return target.CanAdopt(node);
                    }

                    target = target.Parent;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the drawing and updating of the graph.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // calculate the mouse position in the graph
            PointF graphMousePos = _nodeLayoutManager.ViewToGraph(_lastMousePosition);

            // when the layout was changed it needs to be recalculated
            bool layoutChanged = _nodeLayoutManager.LayoutChanged;

            if (layoutChanged)
            {
                _nodeLayoutManager.UpdateLayout(e.Graphics, _forceChangeLayout);
                _forceChangeLayout = false;
            }

            // center the root behaviour if requested
            if (_pendingCenterBehavior)
            {
                _pendingCenterBehavior = false;
                CenterNode(_rootNodeView);
            }

            // select the pending node
            if (_selectedNodePending != null)
            {
                if (_selectedNodePendingParent != null)
                {
                    if (_selectedNodePendingParent.CanBeExpanded() && !_selectedNodePendingParent.IsExpanded)
                    {
                        _selectedNodePendingParent.IsExpanded = true;
                        LayoutChanged();
                    }

                    SelectedNode = _selectedNodePendingParent.GetChild(_selectedNodePending);

                }
                else
                {
                    SelectedNode = RootNodeView.FindNodeViewData(_selectedNodePending);
                }

                if (SelectedNode != null)
                {
                    if (SelectedNode.CanBeExpanded() && !SelectedNode.IsExpanded)
                    {
                        SelectedNode.IsExpanded = true;
                        LayoutChanged();
                    }

                    SelectedNode.SelectedSubItem = SelectedNode.GetSubItem(_selectedAttachmentPending);
                    ShowNode(SelectedNode);
                }

                _selectedNodePending = null;
                _selectedNodePendingParent = null;
                _selectedAttachmentPending = null;

                if (ClickEvent != null)
                {
                    ClickEvent(SelectedNode);
                }
            }

            // check if we must keep the original position of the mouse
            if (_maintainMousePosition)
            {
                _maintainMousePosition = false;

                // move the graph so that _graphOrigin is at the same position in the view as it was before
                float mouseX = (graphMousePos.X - _graphOrigin.X) * _nodeLayoutManager.Scale + _nodeLayoutManager.Offset.X;
                float mouseY = (graphMousePos.Y - _graphOrigin.Y) * _nodeLayoutManager.Scale + _nodeLayoutManager.Offset.Y;

                _nodeLayoutManager.Offset = new PointF(mouseX, mouseY);
            }

            // check if we must keep the original position of _maintainNodePosition
            else if (_maintainNodePosition != null)
            {
                // move the graph so that _graphOrigin is at the same position in the view as it was before
                RectangleF bbox = _maintainNodePosition.IsFSM ? _maintainNodePosition.GetTotalBoundingBox() : _maintainNodePosition.BoundingBox;
                PointF viewpos = new PointF(bbox.X * _nodeLayoutManager.Scale, bbox.Y * _nodeLayoutManager.Scale);

                _nodeLayoutManager.Offset = new PointF(_graphOrigin.X - viewpos.X, _graphOrigin.Y - viewpos.Y);
            }

            // reset the node whose position we want to keep
            _maintainNodePosition = null;

            // draw the graph to the view
            _nodeLayoutManager.DrawGraph(e.Graphics, graphMousePos, _currentNode, SelectedNode, _highlightedNodeIds, _updatedNodeIds, _highlightedTransitionIds, _highlightBreakPoint, _profileInfos);

            // check if we are currently dragging a node and we must draw additional data
            if (_dragTargetNode != null && _dragAttachment == null && (KeyCtrlIsDown && _movedNode != null || _dragTargetNode.Node != _movedNode))
            {
                if (_dragAttachMode == NodeAttachMode.Attachment)
                {
                    // we could draw some stuff for attachements here
                }
                else
                {
                    // draw the arrows for the attach modes

                    // get the bounding box of the node
                    RectangleF bbox = _dragTargetNode.BoundingBox;

                    // get the bounding box of the connector
                    _dragTargetConnector = null;

                    // the depth of the area for the mouse
                    const float offset = 12.0f;

                    // the distance of the arrow from the border and its height
                    const float innerOffset = 2.0f;

                    // the horizintal middle of the node
                    float centerX = bbox.Left + bbox.Width * 0.5f;
                    float centerY = bbox.Top + bbox.Height * 0.5f;
                    float centerBoxX = bbox.X + bbox.Width * 0.4f;
                    float centerBoxWidth = bbox.Width * 0.2f;

                    // the half width of the arrow depending of the node's height
                    float arrowHalfWidth = (bbox.Height - innerOffset - innerOffset) * 0.5f;

                    // calculate the mouse areas for the different attach modes
                    RectangleF top    = new RectangleF(centerBoxX, bbox.Top, centerBoxWidth, offset);
                    RectangleF bottom = new RectangleF(centerBoxX, bbox.Bottom - offset, centerBoxWidth, offset);
                    RectangleF center = new RectangleF(centerBoxX, centerY - offset * 0.5f, centerBoxWidth, offset);
                    RectangleF left   = new RectangleF(bbox.X, bbox.Y, offset, bbox.Height);

                    // update for dragging in a new node
                    BehaviorNode behavior = _dragNodeDefaults as BehaviorNode;

                    if (behavior != null && behavior.FileManager == null)
                    {
                        behavior = null;
                    }

                    // the node that is currently dragged
                    Node draggedNode = (_movedNode != null) ? _movedNode : (_dragNodeDefaults as Node);

                    if (draggedNode == null)
                    {
                        return;
                    }

                    Node.Connector parentConnector = _dragTargetNode.Node.ParentConnector;
                    bool canBeAdoptedByParent = parentConnector != null && (!parentConnector.IsAsChild || AdoptNodeByAncestor(_dragTargetNode.Node.Parent, draggedNode));
                    //bool targetCanBeAdoptedByParent = (_movedNode == null) || (_movedNode.ParentConnector != null) && _movedNode.ParentConnector.AcceptsChild(_dragTargetNode.Node);
                    bool hasParentBehavior = _dragTargetNode.HasParentBehavior(behavior);
                    bool parentHasParentBehavior = (_dragTargetNode.Parent == null);

                    bool isFSM = _rootNodeView.IsFSM || (_rootNodeView.Children.Count == 0);

                    bool mayTop = !isFSM && canBeAdoptedByParent /*&& targetCanBeAdoptedByParent*/ && !parentHasParentBehavior &&
                                  parentConnector != null && parentConnector.AcceptsChild(draggedNode);

                    bool mayBottom = mayTop;

                    bool mayCenter = !parentHasParentBehavior && (_rootNodeView.IsFSM && draggedNode.IsFSM || canBeAdoptedByParent &&
                                                                  draggedNode.GetType() != _dragTargetNode.Node.GetType() &&
                                                                  parentConnector != null && !parentConnector.IsReadOnly && parentConnector.AcceptsChild(draggedNode, true) &&
                                                                  draggedNode.CanAdoptChildren(_dragTargetNode.Node));

                    bool mayLeft = !isFSM && (_dragTargetNode.Node.Parent != _movedNode) &&
                                   canBeAdoptedByParent && !parentHasParentBehavior && !hasParentBehavior &&
                                   parentConnector != null && !parentConnector.IsReadOnly && parentConnector.AcceptsChild(draggedNode, true) &&
                                   !(draggedNode is BehaviorNode) && draggedNode.CanAdoptNode(_dragTargetNode.Node);

                    // update for moving an existing node
                    bool dragTargetHasParentMovedNode = false;

                    if (_movedNode != null)
                    {
                        mayCenter = false;
                        dragTargetHasParentMovedNode = KeyShiftIsDown && _dragTargetNode.Node.HasParent(_movedNode);

                        // a node may not dragged on itself and may not dragged on one of its own children
                        if (_dragTargetNode.Node == _movedNode || dragTargetHasParentMovedNode)
                        {
                            //mayTop = KeyCtrlIsDown;
                            mayTop &= KeyCtrlIsDown && (_dragTargetNode.Node.ParentConnector != null) && _dragTargetNode.Node.ParentConnector.AcceptsChild(_movedNode);
                            mayBottom = mayTop;
                            mayLeft = false;

                        }
                        else
                        {
                            // a dragged node cannot be placed in the same position again
                            mayTop &= (KeyCtrlIsDown || _dragTargetNode.Node.PreviousNode != _movedNode);
                            mayBottom &= (KeyCtrlIsDown || _dragTargetNode.Node.NextNode != _movedNode);
                            mayLeft &= _movedNode.CanAdoptChildren(_dragTargetNode.Node) && (!KeyShiftIsDown || _movedNode.Children.Count == 0);
                        }
                    }

                    if (_copiedNode != null)
                    {
                        mayCenter = false;
                        mayLeft &= _copiedNode.CanAdoptChildren(_dragTargetNode.Node) && (!KeyShiftIsDown || _copiedNode.Children.Count == 0);
                        mayTop &= (_dragTargetNode.Node.ParentConnector != null) && _dragTargetNode.Node.ParentConnector.AcceptsChild(_copiedNode);
                        mayBottom = mayTop;

                    }
                    else if (_clipboardPasteMode)
                    {
                        mayCenter = false;
                        mayLeft &= !KeyShiftIsDown;
                    }

                    // reset the attach mode
                    _dragAttachMode = NodeAttachMode.None;

                    // the vertices needed to draw the arrows
                    PointF[] vertices = new PointF[3];

                    // draw the top arrow if this action is allowed
                    if (mayTop)
                    {
                        vertices[0] = new PointF(centerX - arrowHalfWidth, top.Bottom - innerOffset);
                        vertices[1] = new PointF(centerX, top.Top + innerOffset);
                        vertices[2] = new PointF(centerX + arrowHalfWidth, top.Bottom - innerOffset);

                        if (top.Contains(graphMousePos))
                        {
                            _dragAttachMode = NodeAttachMode.Top;
                            e.Graphics.FillPolygon(Brushes.White, vertices);

                        }
                        else
                        {
                            e.Graphics.FillPolygon(Brushes.Black, vertices);
                        }
                    }

                    // draw the bottom arrow if this action is allowed
                    if (mayBottom)
                    {
                        vertices[0] = new PointF(centerX - arrowHalfWidth, bottom.Top + innerOffset);
                        vertices[1] = new PointF(centerX + arrowHalfWidth, bottom.Top + innerOffset);
                        vertices[2] = new PointF(centerX, bottom.Bottom - innerOffset);

                        if (_dragAttachMode == NodeAttachMode.None && bottom.Contains(graphMousePos))
                        {
                            _dragAttachMode = NodeAttachMode.Bottom;
                            e.Graphics.FillPolygon(Brushes.White, vertices);

                        }
                        else
                        {
                            e.Graphics.FillPolygon(Brushes.Black, vertices);
                        }
                    }

                    // draw the center rectangle if this action is allowed
                    if (mayCenter)
                    {
                        if (center.Contains(graphMousePos))
                        {
                            _dragAttachMode = NodeAttachMode.Center;
                            e.Graphics.FillRectangle(Brushes.White, centerX - arrowHalfWidth * 0.5f, centerY - innerOffset * 2.0f, arrowHalfWidth, innerOffset * 4.0f);

                        }
                        else
                        {
                            e.Graphics.FillRectangle(Brushes.Black, centerX - arrowHalfWidth * 0.5f, centerY - innerOffset * 2.0f, arrowHalfWidth, innerOffset * 4.0f);
                        }
                    }

                    // draw the left arrow if this action is allowed
                    if (mayLeft)
                    {
                        vertices[0] = new PointF(left.Right - innerOffset, left.Top + innerOffset);
                        vertices[1] = new PointF(left.Right - innerOffset, left.Bottom - innerOffset);
                        vertices[2] = new PointF(left.Left + innerOffset, left.Top + left.Height * 0.5f);

                        if (_dragAttachMode == NodeAttachMode.None && left.Contains(graphMousePos))
                        {
                            _dragAttachMode = NodeAttachMode.Left;
                            e.Graphics.FillPolygon(Brushes.White, vertices);

                        }
                        else
                        {
                            e.Graphics.FillPolygon(Brushes.Black, vertices);
                        }
                    }

                    // draw the right arrow if this action is allowed
                    foreach (Node.Connector connector in _dragTargetNode.Connectors)
                    {
                        RectangleF bboxConnector = _dragTargetNode.GetConnectorBoundingBox(bbox, connector);
                        RectangleF right = new RectangleF(bboxConnector.Right - offset, bboxConnector.Y, offset, bboxConnector.Height);

                        bool mayRight = !_rootNodeView.IsFSM && !dragTargetHasParentMovedNode &&
                                        !hasParentBehavior &&
                                        !connector.IsReadOnly &&
                                        connector.AcceptsChild(draggedNode) &&
                                        (!connector.IsAsChild || AdoptNodeByAncestor(_dragTargetNode.Node, draggedNode));

                        if (mayRight && draggedNode != null && connector == draggedNode.ParentConnector)
                        {
                            mayRight = false;
                        }

                        if (mayRight)
                        {
                            float inOffset = bboxConnector.Height > innerOffset * 4.0f ? innerOffset : 3.0f;

                            vertices[0] = new PointF(right.Left + inOffset, right.Top + inOffset);
                            vertices[1] = new PointF(right.Right - inOffset, right.Top + right.Height * 0.5f);
                            vertices[2] = new PointF(right.Left + inOffset, right.Bottom - inOffset);

                            if (_dragAttachMode == NodeAttachMode.None && right.Contains(graphMousePos))
                            {
                                _dragTargetConnector = _dragTargetNode.Node.GetConnector(connector.Identifier);
                                _dragAttachMode = NodeAttachMode.Right;
                                e.Graphics.FillPolygon(Brushes.White, vertices);

                            }
                            else
                            {
                                e.Graphics.FillPolygon(Brushes.Black, vertices);
                            }
                        }
                    }
                }
            }

            // draw last mouse pos
            //e.Graphics.DrawRectangle(Pens.Red, graphMousePos.X -1.0f, graphMousePos.Y -1.0f, 2.0f, 2.0f);

            //when we are dragging an existing node we draw a small graph representing it
            if (_movedNodeGraph != null)
            {
                // update the layout for the graph. This happens only once inside the function.
                _movedNodeGraph.UpdateLayout(e.Graphics);

                // offset the graph to the mouse position
                _movedNodeGraph.Offset = new PointF(_nodeLayoutManager.Offset.X + graphMousePos.X * _nodeLayoutManager.Scale,
                                                    _nodeLayoutManager.Offset.Y + graphMousePos.Y * _nodeLayoutManager.Scale - _movedNodeGraph.RootNodeLayout.LayoutRectangle.Height * 0.5f * _movedNodeGraph.Scale);

                // draw the graph
                _movedNodeGraph.DrawGraph(e.Graphics, graphMousePos);
            }

            // attachment
            if (_currentNode != null && _dragTargetNode != null &&
                _dragAttachment != null && _dragTargetAttachment != null && _dragAttachment != _dragTargetAttachment &&
                _dragTargetNode.Node.AcceptsAttachment(_dragAttachment.Attachment))
            {
                _dragAttachMode = NodeAttachMode.None;

                Attachments.Attachment sourceAttach = _dragAttachment.SelectableObject as Attachments.Attachment;
                Attachments.Attachment targetAttach = _dragTargetAttachment.SelectableObject as Attachments.Attachment;

                if (sourceAttach != null && targetAttach != null &&
                    sourceAttach.IsPrecondition == targetAttach.IsPrecondition &&
                    sourceAttach.IsTransition == targetAttach.IsTransition &&
                    sourceAttach.IsEffector == targetAttach.IsEffector)
                {
                    SubItemRegin regin = _dragTargetNode.GetSubItemRegin(graphMousePos);
                    int itemIndex = _currentNode.GetSubItemIndex(_dragAttachment);
                    int targetItemIndex = _dragTargetNode.GetSubItemIndex(_dragTargetAttachment);

                    if (regin != SubItemRegin.Out && itemIndex >= 0 && targetItemIndex >= 0)
                    {
                        RectangleF bbox = _dragTargetNode.GetSubItemBoundingBox(graphMousePos);

                        const float offset = 8.0f;
                        const float innerOffset = 2.0f;

                        float centerX = bbox.Left + bbox.Width * 0.5f;
                        float centerY = bbox.Top + bbox.Height * 0.5f;
                        float centerBoxX = bbox.X + bbox.Width * 0.4f;
                        float arrowHalfWidth = bbox.Width * 0.12f;

                        RectangleF top = new RectangleF(centerX - arrowHalfWidth, bbox.Top, arrowHalfWidth * 2.0f, offset);
                        RectangleF bottom = new RectangleF(centerX - arrowHalfWidth, bbox.Bottom - offset, arrowHalfWidth * 2.0f, offset);

                        PointF[] vertices = new PointF[3];

                        switch (regin)
                        {
                            case SubItemRegin.Top:
                                if (KeyCtrlIsDown || _currentNode != _dragTargetNode || itemIndex != targetItemIndex - 1)
                                {
                                    vertices[0] = new PointF(centerX - arrowHalfWidth, top.Bottom - innerOffset);
                                    vertices[1] = new PointF(centerX, top.Top + innerOffset);
                                    vertices[2] = new PointF(centerX + arrowHalfWidth, top.Bottom - innerOffset);

                                    if (top.Contains(graphMousePos))
                                    {
                                        _dragAttachMode = NodeAttachMode.Top;
                                        e.Graphics.FillPolygon(Brushes.White, vertices);

                                    }
                                    else
                                    {
                                        e.Graphics.FillPolygon(Brushes.Black, vertices);
                                    }
                                }

                                break;

                            case SubItemRegin.Bottom:
                                if (KeyCtrlIsDown || _currentNode != _dragTargetNode || itemIndex != targetItemIndex + 1)
                                {
                                    vertices[0] = new PointF(centerX - arrowHalfWidth, bottom.Top + innerOffset);
                                    vertices[1] = new PointF(centerX + arrowHalfWidth, bottom.Top + innerOffset);
                                    vertices[2] = new PointF(centerX, bottom.Bottom - innerOffset);

                                    if (bottom.Contains(graphMousePos))
                                    {
                                        _dragAttachMode = NodeAttachMode.Bottom;
                                        e.Graphics.FillPolygon(Brushes.White, vertices);

                                    }
                                    else
                                    {
                                        e.Graphics.FillPolygon(Brushes.Black, vertices);
                                    }
                                }

                                break;
                        }
                    }
                }
            }

            if (_movedSubItem != null)
            {
                NodeViewData.SubItemText subitem = _movedSubItem as NodeViewData.SubItemText;

                if (subitem != null)
                {
                    RectangleF boundingBox = new RectangleF(graphMousePos, new SizeF(50, 12));
                    e.Graphics.FillRectangle(subitem.BackgroundBrush, boundingBox);
                }
            }

            // draw FSM related
            DrawFSMArrow(e, graphMousePos);
            DrawFSMDragCurve(e, graphMousePos);

            //the first time of paint, to collapse plan failed branch by default
            Behavior b = this.RootNode as Behavior;

            if (b.PlanIsCollapseFailedBranch > 0)
            {
                if (b.PlanIsCollapseFailedBranch == Behavior.kPlanIsCollapseFailedBranch)
                {
                    NodeViewData root = (NodeViewData)this.RootNodeView.Children[0];
                    CollapseFailedBrach(root);
                }

                b.PlanIsCollapseFailedBranch--;

                this.CenterNode(this._rootNodeView);
                this.LayoutChanged();
            }
        }

        private void DrawFSMArrow(PaintEventArgs e, PointF graphMousePos)
        {
            if (_currentNode != null)
            {
                NodeViewData.SubItem subItem;
                RectangleF bbox;
                NodeViewData.RangeType rangeType = _currentNode.CheckFSMArrowRange(graphMousePos, out subItem, out bbox);

                if (rangeType != NodeViewData.RangeType.kNode)
                {
                    const float offset = 8.0f;
                    PointF[] vertices = new PointF[3];
                    float centerX = bbox.Left + bbox.Width * 0.5f;
                    float centerY = bbox.Top + bbox.Height * 0.5f;

                    if (graphMousePos.X < bbox.Left + bbox.Width * 0.5f)
                    {
                        vertices[0] = new PointF(bbox.Left + offset, centerY - offset * 0.6f);
                        vertices[1] = new PointF(bbox.Left, centerY);
                        vertices[2] = new PointF(bbox.Left + offset, centerY + offset * 0.6f);

                    }
                    else
                    {
                        vertices[0] = new PointF(bbox.Right - offset, centerY - offset * 0.6f);
                        vertices[1] = new PointF(bbox.Right, centerY);
                        vertices[2] = new PointF(bbox.Right - offset, centerY + offset * 0.6f);
                    }

                    if (rangeType != NodeViewData.RangeType.kNode)
                    {
                        e.Graphics.FillPolygon(Brushes.White, vertices);
                    }

                    else
                    {
                        e.Graphics.FillPolygon(Brushes.Black, vertices);
                    }
                }
            }
        }

        private void DrawFSMDragCurve(PaintEventArgs e, PointF graphMousePos)
        {
            if (_fsmDragMode != FSMDragModes.kNone)
            {
                RectangleF bbox = _fsmItemBoundingBox;
                Pen pen = new Pen(System.Drawing.Brushes.LightGray, 3.0f);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                pen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(pen.Width, pen.Width);

                float penHalfWidth = pen.Width * 0.5f;
                float centerY = bbox.Top + bbox.Height * 0.5f;
                PointF startPos = new PointF(bbox.Left, centerY - penHalfWidth);

                if (_fsmDragMode == FSMDragModes.kRight)
                {
                    startPos.X = bbox.Right;
                }

                float middleX = startPos.X + (graphMousePos.X - startPos.X) * 0.5f;

                e.Graphics.DrawBezier(pen,
                                      startPos.X, startPos.Y,
                                      middleX, startPos.Y,
                                      middleX, graphMousePos.Y,
                                      graphMousePos.X, graphMousePos.Y);
            }
        }

        private void CollapseFailedBrach(NodeViewData nv)
        {
            Behavior b = nv.Node.Behavior as Behavior;

            if (b.PlanningProcess != null)
            {
                FrameStatePool.PlanningState nodeState = b.PlanningProcess.GetNode(nv.FullId);

                if (nodeState != null)
                {
                    //if (!nodeState._bOk || nodeState._bPreFailed)
                    if (nodeState._bPreFailed)
                    {
                        nv.IsExpanded = false;
                        return;
                    }

                }
                else
                {
                    nv.IsExpanded = false;
                    return;
                }
            }

            if (nv.Children.Count > 0)
            {
                nv.IsExpanded = true;

                foreach (NodeViewData c in nv.Children)
                {
                    CollapseFailedBrach(c);
                }
            }
        }

        private static Node _clipboardNode = null;
        private static NodeViewData.SubItem _clipboardSubItem = null;
        private static NodeViewData _clipboardRootNode = null;

        /// <summary>
        /// Stores if Ctrl+V is currently pressed.
        /// </summary>
        private bool _clipboardPasteMode = false;

        /// <summary>
        /// The node we dragged inside the graph.
        /// </summary>
        private Node _movedNode = null;

        /// <summary>
        /// The node we want to create a copy from.
        /// </summary>
        private Node _copiedNode = null;

        /// <summary>
        /// The layout manager which draws the small graph when dragging existing nodes.
        /// </summary>
        private NodeLayoutManager _movedNodeGraph = null;

        private NodeViewData.SubItem _movedSubItem = null;

        private void BehaviorTreeView_MouseLeave(object sender, EventArgs e)
        {
            this.toolTipTimer.Stop();
            this.toolTip.Hide(this);
        }

        /// <summary>
        /// Handles when the mouse is moved.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_lostFocus)
            {
                _lostFocus = false;

                // update the last ouse position
                _lastMousePosition = e.Location;

                base.OnMouseMove(e);

                return;
            }

            // returns the mouse under the mouse cursor
            NodeViewData nodeFound = _rootNodeView.GetInsideNode(e.Location);
            NodeViewData.SubItem subItemFound = null;

            if (nodeFound != null)
            {
                subItemFound = nodeFound.GetSubItem(nodeFound, _nodeLayoutManager.ViewToGraph(e.Location));

            }
            else
            {
                this.toolTip.Hide(this);
            }

            // clear previously stored node which can cause problems when dragging to another view
            //_dragTargetNode = null;

            if (nodeFound != null || _currentExpandNode != null || subItemFound != null)
            {
                if (_dragAttachment == null)
                {
                    _currentNode = nodeFound;
                }

                if (Settings.Default.ShowNodeToolTips)
                {
                    if (nodeFound != null)
                    {
                        _nodeToolTip = (subItemFound != null) ? subItemFound.ToolTip : nodeFound.ToolTip;
                    }

                    if (!string.IsNullOrEmpty(_nodeToolTip))
                    {
                        this.toolTipTimer.Start();
                    }
                }

                Invalidate();
            }

            // check if we are currently dragging the graph
            if ((e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && _objectDragType == ObjectDragTypes.kGraph)) && _lastMousePosition != e.Location && !this.contextMenu.Visible)
            {
                Cursor = Cursors.SizeAll;

                // move the graph according to the last mouse position
                _nodeLayoutManager.Offset = new PointF(_nodeLayoutManager.Offset.X - (_lastMousePosition.X - e.X), _nodeLayoutManager.Offset.Y - (_lastMousePosition.Y - e.Y));

                Invalidate();
            }

            // check if we start duplicating an existing node step 1
            else if (e.Button == MouseButtons.Left && KeyCtrlIsDown && _lastMousePosition != e.Location && _dragNodeDefaults == null && _copiedNode == null && _currentNode != null && !(_currentNode.Node is BehaviorNode))
            {
                if (_objectDragType == ObjectDragTypes.kNode)   // node
                {
                    _movedNode = null;
                    _copiedNode = _currentNode.Node;

                    // create the layout manager used to draw the graph
                    _movedNodeGraph = new NodeLayoutManager(_copiedNode.CloneBranch().CreateNodeViewData(null, _rootNodeView.RootBehavior), _nodeLayoutManager.EdgePen, _nodeLayoutManager.EdgePenSelected, _nodeLayoutManager.EdgePenHighlighted, _nodeLayoutManager.EdgePenUpdate, _nodeLayoutManager.EdgePenReadOnly, true);
                    _movedNodeGraph.Scale = 0.3f;
                    _movedNodeGraph.RenderDepth = KeyShiftIsDown ? int.MaxValue : 0;

                    // use the existing node as the node defaults
                    _dragNodeDefaults = _copiedNode;

                }
                else if (_objectDragType == ObjectDragTypes.kAttachment)     // attachment
                {
                    if (_dragAttachment == null)
                    {
                        NodeViewData.SubItem subItem = _currentNode.GetSubItem(_currentNode, _nodeLayoutManager.ViewToGraph(e.Location));
                        _dragAttachment = subItem as NodeViewData.SubItemAttachment;
                    }
                }

                Invalidate();
            }

            // check if we are duplicating an existing node step 2
            else if (e.Button == MouseButtons.Left && KeyCtrlIsDown && (_copiedNode != null || _dragAttachment != null))
            {
                if (_objectDragType == ObjectDragTypes.kNode)   // node
                {
                    _movedNodeGraph.RenderDepth = KeyShiftIsDown ? int.MaxValue : 0;

                    _dragTargetNode = _currentNode;

                    Cursor = _currentNode == null ? Cursors.No : Cursors.Default;

                    //Point movedGraphGraphPos= new Point(e.Location.X + _movedNodeGraph.Offset.X, e.Location.Y + _movedNodeGraph.Offset.Y /-2);
                    //_movedNodeGraph.Location= movedGraphGraphPos;

                }
                else if (_objectDragType == ObjectDragTypes.kAttachment)     // attachment
                {
                    _dragTargetNode = nodeFound;

                    if (_dragTargetNode != null)
                    {
                        NodeViewData.SubItem subItem = _dragTargetNode.GetSubItem(_dragTargetNode, _nodeLayoutManager.ViewToGraph(e.Location));
                        _dragTargetAttachment = subItem as NodeViewData.SubItemAttachment;
                    }
                }

                Invalidate();
            }

            // check if we start dragging an existing node step 1
            else if (e.Button == MouseButtons.Left && _lastMousePosition != e.Location && !KeyCtrlIsDown && _movedNode == null && _currentNode != null)
            {
                if (_objectDragType == ObjectDragTypes.kNode)   // node
                {
                    if (_currentNode.CanBeDragged())
                    {
                        if (_currentNode.IsFSM)
                        {
                            PointF currentGraphMousePos = _nodeLayoutManager.ViewToGraph(e.Location);
                            PointF lastGraphMousePos = _nodeLayoutManager.ViewToGraph(_lastMousePosition);

                            _currentNode.ScreenLocation = new PointF(_currentNode.ScreenLocation.X + currentGraphMousePos.X - lastGraphMousePos.X,
                                                                     _currentNode.ScreenLocation.Y + currentGraphMousePos.Y - lastGraphMousePos.Y);

                            LayoutChanged();

                        }
                        else if ((KeyShiftIsDown || _currentNode.Node.ParentCanAdoptChildren))
                        {
                            _movedNode = _currentNode.Node;

                            // create the layout manager used to draw the graph
                            if (_movedNodeGraph == null)
                            {
                                _movedNodeGraph = new NodeLayoutManager(_movedNode.CloneBranch().CreateNodeViewData(null, _rootNodeView.RootBehavior), _nodeLayoutManager.EdgePen, _nodeLayoutManager.EdgePenSelected, _nodeLayoutManager.EdgePenHighlighted, _nodeLayoutManager.EdgePenUpdate, _nodeLayoutManager.EdgePenReadOnly, true);
                            }

                            _movedNodeGraph.Scale = 0.3f;
                            //_movedNodeGraph.RenderDepth = KeyShiftIsDown ? int.MaxValue : 0;
                            _movedNodeGraph.RenderDepth = int.MaxValue;
                        }
                    }

                }
                else if (_objectDragType == ObjectDragTypes.kAttachment)     // attachment
                {
                    if (_fsmDragMode == FSMDragModes.kNone && Plugin.EditMode == EditModes.Design)
                    {
                        _movedNodeGraph = null;
                        _dragTargetNode = nodeFound;

                        if (_dragAttachment == null)
                        {
                            NodeViewData.SubItem subItem = _currentNode.GetSubItem(_currentNode, _nodeLayoutManager.ViewToGraph(e.Location));
                            _dragAttachment = subItem as NodeViewData.SubItemAttachment;

                            if (_dragAttachment != null)
                            {
                                _movedSubItem = _dragAttachment.Clone(_currentNode.Node);
                            }

                        }
                        else if (_dragTargetNode != null)
                        {
                            NodeViewData.SubItem subItem = _dragTargetNode.GetSubItem(_dragTargetNode, _nodeLayoutManager.ViewToGraph(e.Location));
                            _dragTargetAttachment = subItem as NodeViewData.SubItemAttachment;
                        }
                    }
                }

                Invalidate();
            }

            // check if we start dragging an existing node step 2
            else if (e.Button == MouseButtons.Left && _movedNode != null && !_clipboardPasteMode)
            {
                // create the layout manager used to draw the graph
                if (_movedNodeGraph == null)
                {
                    _movedNodeGraph = new NodeLayoutManager(_movedNode.CloneBranch().CreateNodeViewData(null, _rootNodeView.RootBehavior), _nodeLayoutManager.EdgePen, _nodeLayoutManager.EdgePenSelected, _nodeLayoutManager.EdgePenHighlighted, _nodeLayoutManager.EdgePenUpdate, _nodeLayoutManager.EdgePenReadOnly, true);
                }

                _movedNodeGraph.Scale = 0.3f;
                _movedNodeGraph.RenderDepth = KeyShiftIsDown ? int.MaxValue : 0;

                _dragNodeDefaults = _movedNode;
                _dragTargetNode = _currentNode;

                Cursor = _currentNode == null ? Cursors.No : Cursors.Default;

                Invalidate();

            }
            else if (_clipboardPasteMode)
            {
                if (_movedNodeGraph != null)
                {
                    //_movedNodeGraph.RenderDepth = KeyShiftIsDown ? int.MaxValue : 0;
                    _movedNodeGraph.RenderDepth = int.MaxValue;
                }

                _dragTargetNode = _currentNode;

                Cursor = _currentNode == null ? Cursors.No : Cursors.Default;

                Invalidate();

            }
            else if (_currentNode != null && _dragAttachment == null)
            {
                // Highlight the expand/collapse flag
                PointF graphMousePos = _nodeLayoutManager.ViewToGraph(e.Location);
                bool isInExandRange = _currentNode.IsInExpandRange(graphMousePos);
                bool isInExandConnectorRange = _currentNode.IsInExpandConnectorRange(graphMousePos);

                if (isInExandRange || isInExandConnectorRange)
                {
                    _currentExpandNode = _currentNode;
                    _nodeToolTip = isInExandRange ? Resources.ExpandAllInfo : Resources.ExpandConnectorInfo;
                    Invalidate();

                }
                else if (_currentExpandNode != null)
                {
                    _currentExpandNode = null;
                    Invalidate();
                }
            }

            // update the last ouse position
            _lastMousePosition = e.Location;

            base.OnMouseMove(e);
        }

        private void scaleGraph(float delta, bool maintainMousePos)
        {
            // calculate the new scale
            float newscale = _nodeLayoutManager.Scale + delta;

            if (newscale < 0.1f)
            {
                newscale = 0.1f;
            }

            if (newscale > 2.0f)
            {
                newscale = 2.0f;
            }

            // maintain the current mouse position while zooming
            if (maintainMousePos)
            {
                _maintainMousePosition = true;
                _graphOrigin = _nodeLayoutManager.ViewToGraph(_lastMousePosition);
            }

            // assign the new scale
            _nodeLayoutManager.Scale = newscale;
            Invalidate();
        }

        /// <summary>
        /// Handles when the mouse wheel is used.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            scaleGraph(0.001f * e.Delta, true);

            //base.OnMouseWheel(e);
        }

        /// <summary>
        /// Returns if the ctrl key is currently down or not.
        /// </summary>
        private bool KeyCtrlIsDown
        {
            get
            {
                return (Control.ModifierKeys & Keys.Control) != Keys.None;
            }
        }

        /// <summary>
        /// Returns if the shift key is currently down or not.
        /// </summary>
        private bool KeyShiftIsDown
        {
            get
            {
                return (Control.ModifierKeys & Keys.Shift) != Keys.None;
            }
        }

        /// <summary>
        /// Returns if the alt key is currently down or not.
        /// </summary>
        private bool KeyAltIsDown
        {
            get
            {
                return (Control.ModifierKeys & Keys.Alt) != Keys.None;
            }
        }

        private bool MoveSubItem(NodeViewData sourceNvd, NodeViewData targetNvd, NodeViewData.SubItemAttachment sourceAttachment, NodeViewData.SubItemAttachment targetAttachment, bool insertPreviously, bool isCopied)
        {
            if (sourceNvd != null && targetNvd != null && sourceAttachment != null &&
                sourceNvd.SetSubItem(targetNvd, sourceAttachment, targetAttachment, insertPreviously, isCopied))
            {
                // set the prefab dirty for the node
                if (!string.IsNullOrEmpty(sourceNvd.Node.PrefabName))
                {
                    sourceNvd.Node.HasOwnPrefabData = true;
                }

                if (!string.IsNullOrEmpty(targetNvd.Node.PrefabName))
                {
                    targetNvd.Node.HasOwnPrefabData = true;
                }

                SelectedNode = targetNvd;

                // call the ClickEvent event handler
                if (ClickEvent != null)
                {
                    ClickEvent(SelectedNode);
                }

                UndoManager.Save(this.RootNode);

                LayoutChanged();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles when a mouse button is let go of.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _movedSubItem = null;

            // check if we were dragging a transition for the FSM.
            if (e.Button == MouseButtons.Left && _currentNode != null && ((_objectDragType == ObjectDragTypes.kNode) && _currentNode.IsFSM || _fsmSubItem != null && _fsmDragMode != FSMDragModes.kNone))
            {
                if (Plugin.EditMode == EditModes.Design && _startMousePosition != e.Location)
                {
                    // drag and move the fsm node
                    if ((_objectDragType == ObjectDragTypes.kNode) && _currentNode.IsFSM)
                    {
                        UndoManager.Save(this.RootNode);

                        LayoutChanged();
                    }

                    // drag and connector the target node
                    else
                    {
                        NodeViewData targetNvd = _rootNodeView.GetInsideNode(e.Location);

                        if (targetNvd != null && targetNvd.IsFSM && (targetNvd.Parent != null) && _fsmSubItem is NodeViewData.SubItemAttachment)
                        {
                            NodeViewData.SubItemAttachment subItemAttachment = _fsmSubItem as NodeViewData.SubItemAttachment;

                            if (subItemAttachment.Attachment != null && subItemAttachment.Attachment.TargetFSMNodeId != targetNvd.Node.Id)
                            {
                                subItemAttachment.Attachment.TargetFSMNodeId = targetNvd.Node.Id;

                                UndoManager.Save(this.RootNode);

                                LayoutChanged();
                            }
                        }
                    }
                }
            }

            // check if we were dragging an existing sub item.
            else if (e.Button == MouseButtons.Left && _currentNode != null && _dragTargetNode != null && _dragAttachment != null)
            {
                NodeViewData.SubItem targetSubItem = _dragTargetNode.GetSubItem(_dragTargetNode, _nodeLayoutManager.ViewToGraph(e.Location));

                if (KeyCtrlIsDown || targetSubItem != _dragAttachment)
                {
                    NodeViewData.SubItemAttachment targetAttachment = targetSubItem as NodeViewData.SubItemAttachment;

                    if ((_currentNode != _dragTargetNode || targetAttachment != null) &&
                        this.MoveSubItem(_currentNode, _dragTargetNode, _dragAttachment, targetAttachment, _dragAttachMode == NodeAttachMode.Top, KeyCtrlIsDown))
                    {
                        _currentNode.ClickEvent(_currentNode, _nodeLayoutManager.ViewToGraph(e.Location));

                        LayoutChanged();
                    }
                }

                _dragAttachment = null;
                _dragTargetAttachment = null;
                _dragAttachMode = NodeAttachMode.None;
            }

            // check if we were dragging or copying an existing node.
            else if (e.Button == MouseButtons.Left && (_movedNode != null || _copiedNode != null || _clipboardPasteMode))
            {
                // if we have a valid target node continue
                if (_dragTargetNode != null)
                {
                    Node sourceNode = null;

                    if (_copiedNode != null)
                    {
                        bool cloneBranch = !(_copiedNode is ReferencedBehavior);
                        sourceNode = (KeyShiftIsDown && cloneBranch) ? (Nodes.Node)_copiedNode.CloneBranch() : (Nodes.Node)_copiedNode.Clone();

                    }
                    else if (_clipboardPasteMode)
                    {
                        bool cloneBranch = !(_clipboardNode is ReferencedBehavior);
                        sourceNode = (/*KeyShiftIsDown && */cloneBranch) ? (Nodes.Node)_clipboardNode.CloneBranch() : (Nodes.Node)_clipboardNode.Clone();

                    }
                    else if (_movedNode != null)
                    {
                        sourceNode = _movedNode;
                    }

                    Debug.Check(sourceNode != null);
                    Node sourceParent = (Node)sourceNode.Parent;
                    BehaviorNode sourceBehavior = sourceNode.Behavior;

                    if (_dragTargetNode.Node == sourceNode)
                    {
                        _dragAttachMode = NodeAttachMode.None;
                    }

                    if (_dragAttachMode == NodeAttachMode.Top ||
                        _dragAttachMode == NodeAttachMode.Bottom ||
                        _dragAttachMode == NodeAttachMode.Left ||
                        _dragAttachMode == NodeAttachMode.Right ||
                        _dragAttachMode == NodeAttachMode.Center)
                    {
                        // set the prefab dirty for its previous parent
                        if (sourceParent != null && !string.IsNullOrEmpty(sourceParent.PrefabName))
                        {
                            sourceParent.HasOwnPrefabData = true;
                        }

                        if (KeyShiftIsDown)
                        {
                            if (sourceParent != null)
                            {
                                sourceParent.RemoveChild(sourceNode.ParentConnector, sourceNode);
                            }

                        }
                        else
                        {
                            sourceNode.ExtractNode();
                        }
                    }

                    // move the dragged node to the target node, according to the attach mode
                    switch (_dragAttachMode)
                    {
                            // the node will be placed above the target node
                        case (NodeAttachMode.Top):
                            int n = _dragTargetNode.Node.ParentConnector.GetChildIndex(_dragTargetNode.Node);
                            ((Node)_dragTargetNode.Node.Parent).AddChild(_dragTargetNode.Node.ParentConnector, sourceNode, n);

                            _selectedNodePending = sourceNode;
                            _selectedNodePendingParent = _dragTargetNode.Parent;

                            LayoutChanged();
                            break;

                            // the node will be placed below the target node
                        case (NodeAttachMode.Bottom):
                            int m = _dragTargetNode.Node.ParentConnector.GetChildIndex(_dragTargetNode.Node);
                            ((Node)_dragTargetNode.Node.Parent).AddChild(_dragTargetNode.Node.ParentConnector, sourceNode, m + 1);

                            _selectedNodePending = sourceNode;
                            _selectedNodePendingParent = _dragTargetNode.Parent;

                            LayoutChanged();
                            break;

                            // the node will be placed in front of the target node
                        case (NodeAttachMode.Left):
                            Node parent = (Node)_dragTargetNode.Node.Parent;
                            Node.Connector conn = _dragTargetNode.Node.ParentConnector;

                            int o = conn.GetChildIndex(_dragTargetNode.Node);

                            parent.RemoveChild(conn, _dragTargetNode.Node);
                            parent.AddChild(conn, sourceNode, o);

                            BaseNode.Connector sourceConn = sourceNode.GetConnector(conn.Identifier);
                            Debug.Check(sourceConn != null);

                            sourceNode.AddChild(sourceConn, _dragTargetNode.Node);

                            _selectedNodePending = sourceNode;
                            _selectedNodePendingParent = _dragTargetNode.Parent;

                            LayoutChanged();
                            break;

                            // the node will simply attached to the target node
                        case (NodeAttachMode.Right):
                            _dragTargetNode.Node.AddChild(_dragTargetConnector, sourceNode);

                            _selectedNodePending = sourceNode;
                            _selectedNodePendingParent = _dragTargetNode;

                            LayoutChanged();
                            break;

                            // the node will replace the target node
                        case (NodeAttachMode.Center):
                            if (sourceNode != null && sourceNode.ReplaceNode(_dragTargetNode.Node))
                            {
                                LayoutChanged();
                            }

                            break;
                    }

                    if (_dragAttachMode != NodeAttachMode.None)
                    {
                        // If cloning a node, its Id should be reset.
                        if (_copiedNode != null || _clipboardPasteMode)
                        {
                            // Cross two different behavior files
                            if (_clipboardPasteMode && _clipboardRootNode != this.RootNodeView)
                            {
                                try
                                {
                                    // Copy the used Pars from the current behavior to the new one.
                                    if (_clipboardNode != null && _clipboardRootNode != null && _clipboardRootNode.Node is Behavior)
                                    {
                                        foreach (ParInfo par in((Behavior)(_clipboardRootNode.Node)).LocalVars)
                                        {
                                            List<Node.ErrorCheck> result = new List<Node.ErrorCheck>();
                                            Plugin.CheckPar(_clipboardNode, par, ref result);

                                            if (result.Count > 0)
                                            {
                                                bool bExist = false;

                                                foreach (ParInfo p in((Behavior)this.RootNode).LocalVars)
                                                {
                                                    if (p.Name == par.Name)
                                                    {
                                                        bExist = true;
                                                        break;
                                                    }
                                                }

                                                if (!bExist)
                                                {
                                                    ((Behavior)this.RootNode).LocalVars.Add(par);
                                                }
                                            }
                                        }
                                    }

                                    // reset its properties and methods
                                    sourceNode.ResetMembers(MetaOperations.ChangeAgentType, this.RootNode.AgentType, null, null, null);
                                }
                                catch
                                {
                                }
                            }

                            // reset its Id
                            sourceNode.ResetId(true);
                        }

                        // update the node's label
                        sourceNode.OnPropertyValueChanged(false);

                        // set the prefab dirty for its current parent
                        if (sourceNode.Parent != null)
                        {
                            Node parent = (Node)sourceNode.Parent;

                            if (!string.IsNullOrEmpty(parent.PrefabName))
                            {
                                parent.HasOwnPrefabData = true;
                                sourceNode.SetPrefab(parent.PrefabName, true);
                            }
                        }

                        UndoManager.Save(this.RootNode);
                    }
                }

                // reset all the drag data
                if (!_clipboardPasteMode)
                {
                    _copiedNode = null;
                    _movedNode = null;
                    _dragTargetNode = null;
                    _dragNodeDefaults = null;
                    _movedNodeGraph = null;
                }

                // redraw the graph
                Invalidate();
            }

            // popup the menu for the current hit node
            else if (e.Button == MouseButtons.Right && !KeyAltIsDown && !KeyCtrlIsDown && !KeyShiftIsDown)
            {
                bool itemEnabled = (SelectedNode != null && SelectedNode.Node.Parent != null);
                itemEnabled &= (Plugin.EditMode == EditModes.Design);

                deleteMenuItem.ShortcutKeys = Keys.Delete;
                deleteTreeMenuItem.ShortcutKeys = Keys.Shift | Keys.Delete;

                cutMenuItem.Enabled = SelectedNodeCanBeCut();
                cutTreeMenuItem.Enabled = (SelectedNode != null) && !SelectedNode.IsFSM && SelectedTreeCanBeCut();
                copyMenuItem.Enabled = itemEnabled;
                copySubtreeMenuItem.Enabled = (SelectedNode != null) && !SelectedNode.IsFSM;
                pasteMenuItem.Enabled = SelectedNodeCanBePasted();
                deleteMenuItem.Enabled = SelectedNodeCanBeDeleted();
                deleteTreeMenuItem.Enabled = (SelectedNode != null) && !SelectedNode.IsFSM && SelectedTreeCanBeDeleted();

                bool isReferencedBehavior = SelectedNode != null && SelectedNode.Node is ReferencedBehavior;
                bool isEvent = SelectedNode != null && SelectedNode.SelectedSubItem != null && SelectedNode.SelectedSubItem.SelectableObject is Attachments.Event;
                referenceMenuItem.Enabled = itemEnabled || isReferencedBehavior || isEvent;
                referenceMenuItem.Text = (isReferencedBehavior || isEvent) ? Resources.OpenReference : Resources.SaveReference;

                disableMenuItem.Enabled = false;

                if (itemEnabled)
                {
                    if (SelectedNode.SelectedSubItem != null && SelectedNode.SelectedSubItem.SelectableObject is Attachments.Attachment)
                    {
                        Attachments.Attachment attach = SelectedNode.SelectedSubItem.SelectableObject as Attachments.Attachment;
                        disableMenuItem.Enabled = attach.CanBeDisabled();
                        disableMenuItem.Text = attach.Enable ? Resources.DisableNode : Resources.EnableNode;
                    }
                    else
                    {
                        disableMenuItem.Enabled = SelectedNode.Node.CanBeDisabled();
                        disableMenuItem.Text = SelectedNode.Node.Enable ? Resources.DisableNode : Resources.EnableNode;
                    }
                }

                expandMenuItem.Enabled = (SelectedNode != null && SelectedNode.CanBeExpanded());
                collapseMenuItem.Enabled = expandMenuItem.Enabled;
                expandAllMenuItem.Enabled = expandMenuItem.Enabled;
                collapseAllMenuItem.Enabled = expandMenuItem.Enabled;

                bool isPrefabInstance = SelectedNode != null && !string.IsNullOrEmpty(SelectedNode.Node.PrefabName);
                breakPrefabMenuItem.Enabled = itemEnabled && isPrefabInstance;

                if (isPrefabInstance)
                {
                    string fullpath = FileManagers.FileManager.GetFullPath(SelectedNode.Node.PrefabName);
                    isPrefabInstance = File.Exists(fullpath);
                }

                savePrefabMenuItem.Enabled = itemEnabled;
                savePrefabMenuItem.Text = isPrefabInstance ? Resources.OpenPrefab : Resources.SavePrefab;

                if (SelectedNode != null)
                {
                    Node prefabRoot = SelectedNode.Node.GetPrefabRoot();
                    string relativeFilename = FileManagers.FileManager.GetRelativePath(this.RootNode.Filename);
                    applyMenuItem.Enabled = itemEnabled && isPrefabInstance && SelectedNode.Node.PrefabName != relativeFilename && prefabRoot.IsPrefabDataDirty();
                }

                breakpointMenuItem.Enabled = SelectedNode != null && SelectedNode.Parent != null;

                if (SelectedNode != null)
                {
                    enterBreakpointMenuItem.Text = SelectedNode.GetBreakpointOperation(HighlightBreakPoint.kEnter);
                    exitBreakpointMenuItem.Text = SelectedNode.GetBreakpointOperation(HighlightBreakPoint.kExit);

                    this.beakpointPlanning.Visible = false;

                    if (SelectedNode.Node is Task)
                    {
                        this.beakpointPlanning.Visible = true;
                        beakpointPlanning.Text = SelectedNode.GetBreakpointOperation(HighlightBreakPoint.kPlanning);
                    }
                }

                contextMenu.Show(this, new Point(e.X, e.Y));
            }

            Cursor = Cursors.Default;
            _fsmDragMode = FSMDragModes.kNone;

            // redraw the graph
            Invalidate();

            base.OnMouseUp(e);
        }

        public bool SelectedNodeCanBePasted()
        {
            return _clipboardNode != null && _clipboardNode.IsFSM ||
                   SelectedNode != null &&
                   (_clipboardSubItem != null &&
                    _clipboardSubItem.SelectableObject != null &&
                    SelectedNode.Node.AcceptsAttachment(_clipboardSubItem.SelectableObject));
        }

        public bool SelectedNodeCanBeCut()
        {
            if (Plugin.EditMode == EditModes.Design && SelectedNode != null)
            {
                // node
                if (SelectedNode.SelectedSubItem == null && SelectedNode.Node.Parent != null)
                {
                    return SelectedNode.CanBeDeleted();
                }

                // attachment
                return true;
            }

            return false;
        }

        public bool SelectedTreeCanBeCut()
        {
            return SelectedTreeCanBeDeleted();
        }

        public bool SelectedNodeCanBeDeleted()
        {
            if (Plugin.EditMode == EditModes.Design && SelectedNode != null)
            {
                if (SelectedNode.SelectedSubItem != null)
                {
                    if (SelectedNode.SelectedSubItem.CanBeDeleted)
                    {
                        return true;
                    }
                }
                else if (SelectedNode.Node.Parent != null && SelectedNode.CanBeDeleted())
                {
                    return true;
                }
            }

            return false;
        }

        public bool SelectedTreeCanBeDeleted()
        {
            return Plugin.EditMode == EditModes.Design &&
                   SelectedNode != null &&
                   SelectedNode.Node.Parent != null &&
                   SelectedNode.CanBeDeleted(true);
        }

        enum ObjectDragTypes
        {
            kNone,
            kNode,
            kAttachment,
            kGraph
        }

        private ObjectDragTypes _objectDragType = ObjectDragTypes.kNone;

        /// <summary>
        /// Handles when a mouse button was pressed.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // the graph has not yet been dragged
            _startMousePosition = e.Location;
            _lastMousePosition = e.Location;
            _objectDragType = ObjectDragTypes.kNone;
            _dragAttachment = null;
            _dragTargetAttachment = null;
            _movedSubItem = null;
            _fsmDragMode = FSMDragModes.kNone;

            if (!_clipboardPasteMode)
            {
                _currentNode = _rootNodeView.GetInsideNode(_lastMousePosition);
                _dragTargetNode = null;
                _movedNodeGraph = null;
                _movedNode = null;

                if (_currentNode != null)
                {
                    if (KeyCtrlIsDown || _currentNode.IsFSM || _currentNode.CanBeDragged() &&
                        (KeyShiftIsDown || _currentNode.Node.ParentCanAdoptChildren))
                    {
                        _objectDragType = ObjectDragTypes.kNode;
                    }
                }

            }
            else
            {
                if (_currentNode == null)
                {
                    _currentNode = _rootNodeView.GetInsideNode(_lastMousePosition);
                }

                if (_dragTargetNode == null)
                {
                    _dragTargetNode = _currentNode;
                }
            }

            if (e.Button == MouseButtons.Left && _currentNode != null)
            {
                Debug.Check(_nodeLayoutManager != null);
                if (_nodeLayoutManager != null)
                {
                    NodeViewData.SubItem subItem = _currentNode.GetSubItem(_currentNode, _nodeLayoutManager.ViewToGraph(e.Location));
                    NodeViewData.SubItemAttachment attach = subItem as NodeViewData.SubItemAttachment;

                    if (attach != null && attach.IsSelected)
                    {
                        _objectDragType = ObjectDragTypes.kAttachment;
                    }

                    PointF graphMousePos = _nodeLayoutManager.ViewToGraph(_lastMousePosition);
                    NodeViewData.RangeType rangeType = _currentNode.CheckFSMArrowRange(graphMousePos, out _fsmSubItem, out _fsmItemBoundingBox);

                    if (rangeType != NodeViewData.RangeType.kNode)
                    {
                        _objectDragType = ObjectDragTypes.kNone;

                        if (Plugin.EditMode == EditModes.Design)
                        {
                            if (rangeType == NodeViewData.RangeType.kFSMLeftArrow)
                            {
                                _fsmDragMode = FSMDragModes.kLeft;
                            }

                            else if (rangeType == NodeViewData.RangeType.kFSMRightArrow)
                            {
                                _fsmDragMode = FSMDragModes.kRight;
                            }
                        }
                    }
                }
            }

            if (_currentNode == null)
            {
                _objectDragType = ObjectDragTypes.kGraph;
            }

            // focus the view if not focused
            if (!Focused)
            {
                // we focus twice to avoid an issue with focusing the view
                Parent.Focus();

                //OnMouseDown will cal OnGotFocus as well
                //Focus();
            }

            base.OnMouseDown(e);
        }

        public delegate void ClickNodeEventDelegate(NodeViewData node);

        /// <summary>
        /// This event is called when the user clicks on a node in the graph which is not selected.
        /// </summary>
        public ClickNodeEventDelegate ClickNode;

        public delegate void ClickEventEventDelegate(NodeViewData node);

        /// <summary>
        /// This event is called when the user clicks on a node in the graph which is already selected which has events.
        /// </summary>
        public ClickEventEventDelegate ClickEvent;

        private void mouseClicked(MouseEventArgs e)
        {
            // check if the user did not drag the graph and clicked it instead
            if ((e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) && (_startMousePosition == e.Location))
            {
                PointF graphMousePos = _nodeLayoutManager.ViewToGraph(e.Location);

                if (e.Button == MouseButtons.Left && _currentNode != null && _dragAttachment == null)
                {
                    bool layoutChanged = false;

                    if (_currentNode.OnClick(KeyCtrlIsDown, graphMousePos, out layoutChanged))
                    {
                        if (layoutChanged)
                        {
                            // keep the position of the current node
                            KeepNodePosition(_currentNode);

                            LayoutChanged();
                        }

                        return;
                    }
                }

                // if the clicked ode is already selected and has events, we click the event instead of the node
                bool clickEvent = SelectedNode != null && SelectedNode == _currentNode && SelectedNode.SubItems.Count > 0;

                // assign the new selected node. Checks if the selected node is already this one.
                SelectedNode = _currentNode;

                if (clickEvent)
                {
                    // perform click event on the node as the node's have to handle their events.
                    SelectedNode.ClickEvent(SelectedNode, _nodeLayoutManager.ViewToGraph(e.Location));
                    Invalidate();

                }
                else
                {
                    Pen pen = _nodeLayoutManager.EdgePen.Clone() as Pen;
                    if (pen != null)
                    {
                        pen.Width = pen.Width * 4;
                        NodeViewData.SubItemAttachment attach = this.RootNodeView.GetSubItemByDrawnPath(graphMousePos, pen);

                        if (attach != null)
                        {
                            NodeViewData nvd = this.RootNodeView.FindNodeViewData(attach.Attachment.Node);

                            if (nvd != null)
                            {
                                SelectedNode = nvd;
                                nvd.SelectedSubItem = attach;

                                clickEvent = true;
                            }
                        }
                    }
                }

                // check if we click the event
                if (clickEvent)
                {
                    // call the ClickEvent event handler
                    if (ClickEvent != null)
                    {
                        ClickEvent(SelectedNode);
                        return;
                    }
                }

                // call the click node event handler
                if (ClickNode != null)
                {
                    ClickNode(SelectedNode);
                    return;
                }
            }
        }

        /// <summary>
        /// Handles when the user performs a click.
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            mouseClicked(e);

            base.OnMouseClick(e);
        }

        private void RefreshProperty()
        {
            if (BehaviorTreeViewDock.LastFocused == null || BehaviorTreeViewDock.LastFocused.BehaviorTreeView != this)
            {
                return;
            }

            object selectedNode = null;

            if (SelectedNode != null)
            {
                if (SelectedNode.SelectedSubItem != null && SelectedNode.SelectedSubItem.SelectableObject != null)
                {
                    selectedNode = SelectedNode.SelectedSubItem.SelectableObject;
                }

                else
                {
                    selectedNode = SelectedNode.Node;
                }
            }

            PropertiesDock.InspectObject(this.RootNode, selectedNode);
        }

        /// <summary>
        /// Handles when the user doucle-clicks.
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            // when the user double-clicked a node and the graph was not dragged and it was the left mouse button, continue
            if (_currentNode != null && e.Button == MouseButtons.Left)
            {
                // call double-clicked on the node
                PointF graphMousePos = _nodeLayoutManager.ViewToGraph(e.Location);
                bool layoutChanged = false;

                if (_currentNode.OnDoubleClick(graphMousePos, out layoutChanged))
                {
                    // check if the node requires the layout to be updated, for example when expanding or collapsing referenced behaviours
                    if (layoutChanged)
                    {
                        // keep the position of the current node
                        KeepNodePosition(_currentNode);

                        LayoutChanged();

                    }
                    else
                    {
                        Invalidate();
                    }
                }
                else if (!_currentNode.CanBeExpanded())
                {
                    openReferenceBehavior(false);
                }
            }

            base.OnMouseDoubleClick(e);
        }

        private bool SwitchSelection()
        {
            if (SelectedNode != null)
            {
                if (SelectedNode.SelectedSubItem == null)   // Node -> Attachment
                {
                    SelectedNode.SelectFirstSubItem();

                    if (SelectedNode.SelectedSubItem != null && ClickEvent != null)
                    {
                        ClickEvent(SelectedNode);
                    }

                    else
                    {
                        return false;
                    }

                }
                else     // Attachment -> Node
                {
                    SelectedNode.SelectedSubItem = null;

                    if (ClickNode != null)
                    {
                        ClickNode(SelectedNode);
                    }
                }

                return true;
            }

            return false;
        }

        private void MoveNode(bool insertPreviously)
        {
            if (SelectedNode != null)
            {
                Node node = SelectedNode.Node as Node;
                if (node != null)
                {
                    Node parent = node.Parent as Node;
                    BaseNode.Connector connector = node.ParentConnector;

                    if (parent != null && connector != null)
                    {
                        int n = connector.GetChildIndex(node);
                        Debug.Check(n >= 0 && n < connector.ChildCount);

                        if (insertPreviously)
                        {
                            Debug.Check(n > 0);

                            parent.RemoveChild(connector, node);
                            parent.AddChild(connector, node, n - 1);

                        }
                        else
                        {
                            Debug.Check(n < connector.ChildCount - 1);

                            parent.RemoveChild(connector, node);
                            parent.AddChild(connector, node, n + 1);
                        }

                        _selectedNodePending = node;
                        _selectedNodePendingParent = SelectedNode.Parent;

                        // set the prefab dirty for the current node
                        if (!string.IsNullOrEmpty(node.PrefabName))
                        {
                            node.HasOwnPrefabData = true;
                        }

                        UndoManager.Save(this.RootNode);
                    }
                }
            }
        }

        /// <summary>
        /// Handles when a key is released.
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.Enter):
                    if (SelectedNode != null)
                    {
                        if (KeyCtrlIsDown)
                        {
                            SelectedNode.ExpandAll(!SelectedNode.IsExpanded);
                        }

                        else
                        {
                            SelectedNode.IsExpanded = !SelectedNode.IsExpanded;
                        }

                        LayoutChanged();

                        e.Handled = true;
                    }

                    break;

                    // store when the shift key is released
                case (Keys.ShiftKey):

                    // update the drawn graph for dragging and duplicating
                    if (_movedNodeGraph != null)
                    {
                        _movedNodeGraph.RenderDepth = 0;
                        Invalidate();
                    }

                    break;

                    // paste from clipboard
                case (Keys.V):
                    PasteSelectedNode();

                    _clipboardPasteMode = false;

                    // reset all the drag data
                    _copiedNode = null;
                    _movedNode = null;
                    _dragTargetNode = null;
                    _dragNodeDefaults = null;
                    _movedNodeGraph = null;

                    // redraw the graph
                    Invalidate();
                    break;

                case (Keys.Left):
                case (Keys.Right):
                case (Keys.Up):
                case (Keys.Down):
                    if (e.Control)
                    {
                        break;
                    }

                    if (SelectedNode == null)
                    {
                        SelectedNode = this.RootNodeView;

                        if (ClickNode != null)
                        {
                            ClickNode(SelectedNode);
                        }

                        LayoutChanged();
                        break;
                    }

                    switch (e.KeyCode)
                    {
                        case (Keys.Left):
                            if (SelectedNode != null && SelectedNode.Parent != null)
                            {
                                SelectedNode = SelectedNode.Parent;

                                if (ClickNode != null)
                                {
                                    ClickNode(SelectedNode);
                                }

                                LayoutChanged();
                            }

                            break;

                        case (Keys.Right):
                            if (SelectedNode != null && SelectedNode.Children.Count > 0)
                            {
                                SelectedNode = SelectedNode.Children[0] as NodeViewData;

                                if (ClickNode != null)
                                {
                                    ClickNode(SelectedNode);
                                }

                                LayoutChanged();
                            }

                            break;

                        case (Keys.Up):
                            if (SelectedNode != null)
                            {
                                if (!KeyShiftIsDown || !SwitchSelection())
                                {
                                    // Node
                                    if (SelectedNode.SelectedSubItem == null)
                                    {
                                        if (SelectedNode.PreviousNode != null)
                                        {
                                            if (KeyCtrlIsDown)   // Move
                                            {
                                                MoveNode(true);

                                            }
                                            else     // Select
                                            {
                                                SelectedNode = SelectedNode.PreviousNode as NodeViewData;

                                                if (ClickNode != null)
                                                {
                                                    ClickNode(SelectedNode);
                                                }
                                            }
                                        }
                                    }

                                    // Attachment
                                    else
                                    {
                                        NodeViewData.SubItem previousItem = SelectedNode.PreviousSelectedSubItem;

                                        if (previousItem != null)
                                        {
                                            if (KeyCtrlIsDown)   // Move
                                            {
                                                NodeViewData.SubItemAttachment sourceItem = SelectedNode.SelectedSubItem as NodeViewData.SubItemAttachment;
                                                NodeViewData.SubItemAttachment targetItem = previousItem as NodeViewData.SubItemAttachment;
                                                MoveSubItem(SelectedNode, SelectedNode, sourceItem, targetItem, true, false);
                                                SelectedNode.SelectedSubItem = sourceItem;

                                            }
                                            else     // Select
                                            {
                                                SelectedNode.SelectedSubItem = previousItem;

                                                if (ClickEvent != null)
                                                {
                                                    ClickEvent(SelectedNode);
                                                }
                                            }
                                        }
                                    }
                                }

                                LayoutChanged();
                            }

                            break;

                        case (Keys.Down):
                            if (SelectedNode != null)
                            {
                                if (!KeyShiftIsDown || !SwitchSelection())
                                {
                                    // Node
                                    if (SelectedNode.SelectedSubItem == null)
                                    {
                                        if (SelectedNode.NextNode != null)
                                        {
                                            if (KeyCtrlIsDown)   // Move
                                            {
                                                MoveNode(false);

                                            }
                                            else     // Select
                                            {
                                                SelectedNode = SelectedNode.NextNode as NodeViewData;

                                                if (ClickNode != null)
                                                {
                                                    ClickNode(SelectedNode);
                                                }
                                            }
                                        }
                                    }

                                    // Attachment
                                    else
                                    {
                                        NodeViewData.SubItem nextItem = SelectedNode.NextSelectedSubItem;

                                        if (nextItem != null)
                                        {
                                            if (KeyCtrlIsDown)   // Move
                                            {
                                                NodeViewData.SubItemAttachment sourceItem = SelectedNode.SelectedSubItem as NodeViewData.SubItemAttachment;
                                                NodeViewData.SubItemAttachment targetItem = nextItem as NodeViewData.SubItemAttachment;
                                                MoveSubItem(SelectedNode, SelectedNode, sourceItem, targetItem, false, false);
                                                SelectedNode.SelectedSubItem = sourceItem;

                                            }
                                            else     // Select
                                            {
                                                SelectedNode.SelectedSubItem = nextItem;

                                                if (ClickEvent != null)
                                                {
                                                    ClickEvent(SelectedNode);
                                                }
                                            }
                                        }
                                    }
                                }

                                LayoutChanged();
                            }

                            break;
                    }

                    break;

                default:
                    base.OnKeyUp(e);
                    break;
            }
        }

        /// <summary>
        /// Handles when a key is pressed.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                    // store when the control key is pressed
                case (Keys.ControlKey):
                    if (_copiedNode == null && _movedNode == null)
                    {
                        Cursor = Cursors.Default;
                    }

                    break;

                    // store when the shift key is pressed
                case (Keys.ShiftKey):

                    // update the drawn graph for dragging and duplicating
                    if (_movedNodeGraph != null)
                    {
                        _movedNodeGraph.RenderDepth = int.MaxValue;
                        Invalidate();
                    }

                    break;

                    // cut to clipboard
                case (Keys.X):
                    if (KeyCtrlIsDown)
                    {
                        CutSelectedNode(KeyShiftIsDown);
                    }

                    break;

                    // copy to clipboard
                case (Keys.C):
                    if (KeyCtrlIsDown)
                    {
                        CopySelectedNode(KeyShiftIsDown);
                    }

                    break;

                    // paste from clipboard
                case (Keys.V):
                    if (!_clipboardPasteMode)
                    {
                        _clipboardPasteMode = KeyCtrlIsDown && _clipboardNode != null;

                        if (_clipboardPasteMode)
                        {
                            // create the layout manager used to draw the graph
                            _movedNodeGraph = new NodeLayoutManager(_clipboardNode.CloneBranch().CreateNodeViewData(null, _rootNodeView.RootBehavior), _nodeLayoutManager.EdgePen, _nodeLayoutManager.EdgePenSelected, _nodeLayoutManager.EdgePenHighlighted, _nodeLayoutManager.EdgePenUpdate, _nodeLayoutManager.EdgePenReadOnly, true);
                            _movedNodeGraph.Scale = 0.3f;
                            //_movedNodeGraph.RenderDepth = KeyShiftIsDown ? int.MaxValue : 0;
                            _movedNodeGraph.RenderDepth = int.MaxValue;

                            // use the existing node as the node defaults
                            _dragNodeDefaults = _clipboardNode;

                            Invalidate();
                        }
                    }

                    break;

                    // handle when the delete key is pressed
                case (Keys.Delete):
                    DeleteSelectedNode(KeyShiftIsDown);
                    break;

                case (Keys.E):
                    if (e.Control)
                    {
                        CenterNode(_rootNodeView);
                    }

                    break;

                case (Keys.Oemplus):
                    if (e.Control)
                    {
                        scaleGraph(0.1f * _nodeLayoutManager.Scale, false);
                    }

                    break;

                case (Keys.OemMinus):
                    if (e.Control)
                    {
                        scaleGraph(-0.1f * _nodeLayoutManager.Scale, false);
                    }

                    break;

                case (Keys.K):
                    if (e.Control)
                    {
                        CheckErrors(_rootNodeView.RootBehavior, false);
                    }

                    break;

                case (Keys.F9):
                    if (SelectedNode != null)
                    {
                        if (e.Shift)
                        {
                            SelectedNode.SetBreakpoint(HighlightBreakPoint.kExit);

                        }
                        else if (e.Control)
                        {
                            SelectedNode.SetBreakpoint(HighlightBreakPoint.kPlanning);

                        }
                        else
                        {
                            SelectedNode.SetBreakpoint(HighlightBreakPoint.kEnter);
                        }

                        LayoutChanged();
                    }

                    break;

                case (Keys.F8):
                    toggleEnableNode();
                    break;

                case (Keys.F1):
                    openDoc();
                    break;

                case (Keys.Left):
                case (Keys.Right):
                case (Keys.Up):
                case (Keys.Down):
                    if (e.Control)
                    {
                        switch (e.KeyCode)
                        {
                            case (Keys.Left):
                                _nodeLayoutManager.Offset = new PointF(_nodeLayoutManager.Offset.X - 10, _nodeLayoutManager.Offset.Y);
                                Invalidate();
                                break;

                            case (Keys.Right):
                                _nodeLayoutManager.Offset = new PointF(_nodeLayoutManager.Offset.X + 10, _nodeLayoutManager.Offset.Y);
                                Invalidate();
                                break;

                            case (Keys.Up):
                                _nodeLayoutManager.Offset = new PointF(_nodeLayoutManager.Offset.X, _nodeLayoutManager.Offset.Y - 10);
                                Invalidate();
                                break;

                            case (Keys.Down):
                                _nodeLayoutManager.Offset = new PointF(_nodeLayoutManager.Offset.X, _nodeLayoutManager.Offset.Y + 10);
                                Invalidate();
                                break;
                        }

                        break;
                    }

                    break;

                default:
                    base.OnKeyDown(e);
                    break;
            }
        }

        protected override bool ProcessKeyPreview(ref Message msg)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;
            const int WM_KEYUP = 0x101;
            const int WM_SYSKEYUP = 0x105;

            Keys keyData = ((Keys)((int)((long)msg.WParam))) | Control.ModifierKeys;
            KeyEventArgs e = new KeyEventArgs(keyData);

            emptyButton.Focus();

            if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
            {
                OnKeyDown(e);

            }
            else if (msg.Msg == WM_KEYUP || msg.Msg == WM_SYSKEYUP)
            {
                OnKeyUp(e);
            }

            if (e.Handled)
            {
                return true;
            }

            return base.ProcessKeyPreview(ref msg);
        }

        private void fitToViewButton_Click(object sender, EventArgs e)
        {
            CenterNode(_rootNodeView);

            emptyButton.Focus();
        }

        private void openDoc()
        {
            string docLink = "http://www.behaviac.com/";

            if (SelectedNode != null)
            {
                if (SelectedNode.SelectedSubItem != null && SelectedNode.SelectedSubItem.SelectableObject is Attachments.Attachment)
                {
                    Attachments.Attachment attach = SelectedNode.SelectedSubItem.SelectableObject as Attachments.Attachment;
                    docLink = attach.DocLink;
                }
                else if (SelectedNode.Node != null)
                {
                    docLink = SelectedNode.Node.DocLink;
                }
            }

            MainWindow.Instance.OpenURL(docLink);
        }

        private void docMenuItem_Click(object sender, EventArgs e)
        {
            openDoc();
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            scaleGraph(0.1f * _nodeLayoutManager.Scale, false);

            emptyButton.Focus();
        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            scaleGraph(-0.1f * _nodeLayoutManager.Scale, false);

            emptyButton.Focus();
        }

        private void fitToViewMenuItem_Click(object sender, EventArgs e)
        {
            CenterNode(_rootNodeView);
        }

        private void toggleEnableNode()
        {
            if (SelectedNode != null)
            {
                if (SelectedNode.SelectedSubItem != null && SelectedNode.SelectedSubItem.SelectableObject is Attachments.Attachment)
                {
                    Attachments.Attachment attach = SelectedNode.SelectedSubItem.SelectableObject as Attachments.Attachment;
                    attach.Enable = !attach.Enable;
                }
                else if (SelectedNode.Node != null)
                {
                    SelectedNode.Node.Enable = !SelectedNode.Node.Enable;

                    if (ClickNode != null)
                    {
                        ClickNode(SelectedNode);
                    }
                }

                UndoManager.Save(this.RootNode);
            }
        }

        private void disableMenuItem_Click(object sender, EventArgs e)
        {
            toggleEnableNode();
        }

        private void KeepNodePosition(NodeViewData nvd)
        {
            // keep the position of the current node
            if (nvd != null && !nvd.IsFSM)
            {
                _maintainNodePosition = nvd;
                _graphOrigin = _maintainNodePosition.DisplayBoundingBox.Location;
            }
        }

        private void expandMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.IsExpanded = true;

                KeepNodePosition(SelectedNode);
                LayoutChanged();
            }
        }

        private void collapseMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.IsExpanded = false;

                KeepNodePosition(SelectedNode);
                LayoutChanged();
            }
        }

        private void expandAllMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.ExpandAll(true);

                KeepNodePosition(SelectedNode);
                LayoutChanged();
            }
        }

        private void collapseAllMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.ExpandAll(false);

                KeepNodePosition(SelectedNode);
                LayoutChanged();
            }
        }

        private void cutMenuItem_Click(object sender, EventArgs e)
        {
            CutSelectedNode();
        }

        private void cutTreeMenuItem_Click(object sender, EventArgs e)
        {
            CutSelectedNode(true);
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedNode();
        }

        private void copySubtreeMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedNode(true);
        }

        private void pasteMenuItem_Click(object sender, EventArgs e)
        {
            PasteSelectedNode();
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedNode();
        }

        private void deleteTreeMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedNode(true);
        }

        public void CutSelectedNode(bool cutSubTree = false)
        {
            if (SelectedNode == null || Plugin.EditMode != EditModes.Design)
            {
                return;
            }

            CopySelectedNode(cutSubTree);

            DeleteSelectedNode(cutSubTree);
        }

        public void CopySelectedNode(bool copySubTree = false)
        {
            if (SelectedNode == null || Plugin.EditMode != EditModes.Design)
            {
                return;
            }

            _clipboardRootNode = this.RootNodeView;
            _clipboardNode = null;
            _clipboardSubItem = null;

            if (SelectedNode.SelectedSubItem == null)
            {
                if (SelectedNode.Node is ReferencedBehavior)
                {
                    _clipboardNode = (Node)SelectedNode.Node;
                }

                else
                {
                    _clipboardNode = copySubTree ? (Node)SelectedNode.Node.CloneBranch() : (Node)SelectedNode.Node.Clone();
                }

            }
            else
            {
                _clipboardSubItem = SelectedNode.SelectedSubItem;
            }
        }

        public void PasteSelectedNode()
        {
            if (Plugin.EditMode != EditModes.Design)
            {
                return;
            }

            if (_clipboardNode != null)
            {
                if (_clipboardNode.IsFSM)
                {
                    Node newnode = (Node)_clipboardNode.Clone();

                    // clear all targets
                    foreach (Attachments.Attachment attach in newnode.Attachments)
                    {
                        if (attach.IsFSM)
                        {
                            attach.TargetFSMNodeId = int.MinValue;
                        }
                    }

                    this.addFSMNode(newnode, _lastMousePosition);

                    newnode.ResetId(true);

                    // automatically select the new node
                    _selectedNodePending = newnode;

                    UndoManager.Save(this.RootNode);

                    LayoutChanged();
                }

            }
            else if (_clipboardSubItem != null && _clipboardSubItem.SelectableObject != null)
            {
                if (SelectedNode != null)
                {
                    Attachments.Attachment attach = _clipboardSubItem.SelectableObject as Attachments.Attachment;

                    if (attach != null && SelectedNode.Node.AcceptsAttachment(attach))
                    {
                        attach = attach.Clone(SelectedNode.Node);
                        attach.ResetId();

                        // clear its target
                        if (attach.IsFSM)
                        {
                            attach.TargetFSMNodeId = int.MinValue;
                        }

                        SelectedNode.Node.AddAttachment(attach);

                        SelectedNode.SelectedSubItem = _clipboardSubItem.Clone(SelectedNode.Node);
                        SelectedNode.AddSubItem(SelectedNode.SelectedSubItem);

                        if (ClickEvent != null)
                        {
                            ClickEvent(SelectedNode);
                        }

                        UndoManager.Save(this.RootNode);

                        LayoutChanged();
                    }
                }
            }
        }

        private void removeBreakpoints(NodeViewData nvd, bool removeChildren)
        {
            if (nvd == null || RootNode == null || string.IsNullOrEmpty(RootNode.Filename))
            {
                return;
            }

            string behaviorFilename = RootNode.MakeRelative(RootNode.Filename);

            if (string.IsNullOrEmpty(behaviorFilename))
            {
                return;
            }

            string[] actionNames = { HighlightBreakPoint.kEnter, HighlightBreakPoint.kExit };
            string fullId = nvd.FullId;

            foreach (string actionName in actionNames)
            {
                DebugDataPool.BreakPoint breakPoint = DebugDataPool.FindBreakPoint(behaviorFilename, fullId, actionName);

                if (breakPoint != null)
                {
                    DebugDataPool.Action action = breakPoint.FindAction(actionName);
                    Debug.Check(action != null);

                    DebugDataPool.RemoveBreakPoint(behaviorFilename, fullId, action);
                }
            }

            if (removeChildren)
            {
                foreach (BaseNode child in nvd.Children)
                {
                    NodeViewData childNvd = child as NodeViewData;

                    if (childNvd != null)
                    {
                        removeBreakpoints(childNvd, removeChildren);
                    }
                }
            }
        }

        public void DeleteSelectedNode(bool deleteSubTree = false)
        {
            if (SelectedNode == null || Plugin.EditMode != EditModes.Design)
            {
                return;
            }

            deleteSubTree &= SelectedNode.CanBeDeleted(true);

            // when we have a node selected which is not the root node, continue
            if (SelectedNode != null && (SelectedNode.SelectedSubItem != null || SelectedNode.Node.Parent != null))
            {
                BehaviorNode sourceBehavior = SelectedNode.Node.Behavior;

                // check whether we have to delete an event or a node
                if (SelectedNode.SelectedSubItem == null)
                {
                    if (deleteSubTree || SelectedNode.CanBeDeleted())
                    {
                        // store the selected node
                        Node node = SelectedNode.Node;

                        if (node.Parent != null)
                        {
                            Node parent = (Node)node.Parent;

                            if (!string.IsNullOrEmpty(parent.PrefabName))
                            {
                                parent.HasOwnPrefabData = true;
                            }
                        }

                        // select the next node automatically
                        if (node.NextNode != null)
                        {
                            _selectedNodePending = node.NextNode as Node;
                            _selectedNodePendingParent = SelectedNode.Parent;

                        }
                        else if (node.PreviousNode != null)
                        {
                            _selectedNodePending = node.PreviousNode as Node;
                            _selectedNodePendingParent = SelectedNode.Parent;

                        }
                        else if (node.Parent != null)
                        {
                            _selectedNodePending = node.Parent as Node;
                            _selectedNodePendingParent = SelectedNode.Parent != null ? SelectedNode.Parent.Parent : null;

                        }
                        else
                        {
                            _selectedNodePending = null;
                            _selectedNodePendingParent = null;
                        }

                        bool isDeleted = false;

                        if (deleteSubTree)
                        {
                            if (node.Parent != null)
                            {
                                ((Node)node.Parent).RemoveChild(node.ParentConnector, node);

                                // keep the root node in the view
                                KeepNodePosition(RootNodeView);

                                // remove all breakpoints
                                removeBreakpoints(SelectedNode, true);

                                isDeleted = true;
                            }

                        }
                        else if (node.IsFSM)
                        {
                            if (node.Parent != null && node.Parent.RemoveFSMNode(node))
                            {
                                // remove its breakpoints
                                removeBreakpoints(SelectedNode, false);

                                isDeleted = true;
                            }

                        }
                        else if (node.ExtractNode())
                        {
                            // remove its breakpoints
                            removeBreakpoints(SelectedNode, false);

                            isDeleted = true;

                        }
                        else
                        {
                            _selectedNodePending = null;
                            _selectedNodePendingParent = null;
                        }

                        if (isDeleted)
                        {
                            Node root = (Node)this.RootNode;

                            if (root.FSMNodes.Count == 0)
                            {
                                // remove the Start condition for the FSM root node
                                for (int i = 0; i < root.Attachments.Count; ++i)
                                {
                                    if (root.Attachments[i].IsStartCondition)
                                    {
                                        root.Attachments.RemoveAt(i);
                                        break;
                                    }
                                }
                            }

                            UndoManager.Save(this.RootNode);
                        }

                        SelectedNode = null;
                        _currentNode = null;
                    }

                }
                else
                {
                    NodeViewData.SubItem nextItem = SelectedNode.NextSelectedSubItem;

                    if (nextItem == null)
                    {
                        nextItem = SelectedNode.PreviousSelectedSubItem;
                    }

                    // just let the node delete the selected subitem
                    if (SelectedNode.RemoveSelectedSubItem())
                    {
                        Node node = SelectedNode.Node;

                        if (!string.IsNullOrEmpty(node.PrefabName))
                        {
                            node.HasOwnPrefabData = true;
                        }

                        UndoManager.Save(this.RootNode);

                        // select the next subitem automatically
                        if (nextItem != null)
                        {
                            SelectedNode.SelectedSubItem = nextItem;

                            // update all labels of its subitems
                            foreach (NodeViewData.SubItem subItem in SelectedNode.SubItems)
                            {
                                NodeViewData.SubItemAttachment attach = subItem as NodeViewData.SubItemAttachment;

                                if (attach != null && attach.Attachment != null)
                                {
                                    attach.Attachment.OnPropertyValueChanged(false);
                                }
                            }

                            if (ClickEvent != null)
                            {
                                ClickEvent(SelectedNode);
                            }

                        }
                        else
                        {
                            if (ClickNode != null)
                            {
                                ClickNode(SelectedNode);
                            }
                        }
                    }
                }

                // the layout needs to be recalculated
                LayoutChanged();
            }
        }

        private void enterBreakpointMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.SetBreakpoint(HighlightBreakPoint.kEnter);
                LayoutChanged();
            }
        }

        private void exitBreakpointMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.SetBreakpoint(HighlightBreakPoint.kExit);
                LayoutChanged();
            }
        }

        private void enterBreakpointPlanning_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                Debug.Check(SelectedNode.Node is Task);

                SelectedNode.SetBreakpoint(HighlightBreakPoint.kPlanning);
                LayoutChanged();
            }
        }

        private void openReferenceBehavior(bool shouldBeSaved)
        {
            if (SelectedNode == null)
            {
                return;
            }

            if (SelectedNode.SelectedSubItem != null && SelectedNode.SelectedSubItem.SelectableObject is Attachments.Event)
            {
                // Open the referenced tree file in the event.
                Attachments.Event evt = SelectedNode.SelectedSubItem.SelectableObject as Attachments.Event;
                UIUtilities.ShowBehaviorTree(evt.ReferenceFilename);

            }
            else if (SelectedNode.Node != null)
            {
                // Open the referenced tree file.
                if (SelectedNode.Node is ReferencedBehavior)
                {
                    ReferencedBehavior refBehavior = SelectedNode.Node as ReferencedBehavior;
                    UIUtilities.ShowBehaviorTree(refBehavior.ReferenceBehaviorString);
                }

                // Save as a referenced tree file.
                else if (shouldBeSaved)
                {
                    saveAsReferencedTree();
                }
            }
        }

        private void referenceMenuItem_Click(object sender, EventArgs e)
        {
            openReferenceBehavior(true);
        }

        private void saveAsReferencedTree()
        {
            if (SelectedNode == null ||
                SelectedNode == _rootNodeView ||
                _rootNodeView.RootBehavior.FileManager == null)
            {
                return;
            }

            string folder = _rootNodeView.RootBehavior.Folder;

            if (string.IsNullOrEmpty(folder))
            {
                folder = Path.GetDirectoryName(_rootNodeView.RootBehavior.FileManager.Filename);
            }

            string ext = Path.GetExtension(_rootNodeView.RootBehavior.FileManager.Filename);
            string filename = Path.Combine(folder, "rf_" + SelectedNode.Node.ExportClass);
            filename = Path.ChangeExtension(filename, ext);

            using(SaveAsDialog saveAsDialog = new SaveAsDialog(true))
            {
                saveAsDialog.Text = Resources.SaveAsReference;
                saveAsDialog.FileName = _behaviorTreeList.GetUniqueFileName(filename);

                if (saveAsDialog.ShowDialog() == DialogResult.OK)
                {
                    filename = saveAsDialog.FileName;

                    // Remove the selected node.
                    Node parentNode = SelectedNode.Node.Parent as Node;
                    BaseNode.Connector parentConnector = SelectedNode.Node.ParentConnector;
                    int index = parentConnector.GetChildIndex(SelectedNode.Node);
                    parentNode.RemoveChild(parentConnector, SelectedNode.Node);

                    // Create a new behavior node.
                    BehaviorNode behaviorNode = Node.CreateBehaviorNode(SelectedNode.Node.ExportClass);
                    ((Node)behaviorNode).AddChild(behaviorNode.GenericChildren, SelectedNode.Node);
                    ((Behavior)behaviorNode).AgentType = ((Behavior)_rootNodeView.RootBehavior).AgentType;

                    // Copy the used Pars from the current behavior to the new one.
                    foreach (ParInfo par in((Behavior)_rootNodeView.RootBehavior).LocalVars)
                    {
                        List<Node.ErrorCheck> result = new List<Node.ErrorCheck>();
                        Plugin.CheckPar(SelectedNode.Node, par, ref result);

                        if (result.Count > 0)
                        {
                            ((Behavior)behaviorNode).LocalVars.Add(par);
                        }
                    }

                    // Save the new behavior node.
                    behaviorNode.FileManager = _behaviorTreeList.GetFileManagers()[0].Create(filename, behaviorNode);
                    behaviorNode.FileManager.Save();

                    _behaviorTreeList.RebuildBehaviorList();

                    // get the behavior we want to reference
                    behaviorNode = _behaviorTreeList.LoadBehavior(filename);

                    // Create a referenced node to hold the new behavior node.
                    ReferencedBehavior refNode = Node.CreateReferencedBehaviorNode(_rootNodeView.RootBehavior, behaviorNode);

                    // Add the new referenced node.
                    Node newNode = refNode as Node;
                    parentNode.AddChild(parentConnector, newNode, index);

                    // Select the new node automatically.
                    _selectedNodePending = newNode;
                    _selectedNodePendingParent = SelectedNode.Parent;

                    newNode.ResetId(true);
                    UndoManager.Save(this.RootNode);

                    LayoutChanged();
                }
            }
        }

        private void savePrefabMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode == null)
            {
                return;
            }

            // If it is a prefab instance, open this prefab.
            if (!string.IsNullOrEmpty(SelectedNode.Node.PrefabName))
            {
                string fullpath = FileManagers.FileManager.GetFullPath(SelectedNode.Node.PrefabName);

                if (UIUtilities.ShowBehaviorTree(fullpath) != null)
                {
                    return;
                }
            }

            if (SelectedNode == RootNodeView || RootNodeView.RootBehavior.FileManager == null)
            {
                return;
            }

            DirectoryInfo parentDir = Directory.GetParent(Workspace.Current.SourceFolder);
            if (parentDir == null)
            {
                return;
            }

            string folder = parentDir.FullName;
            //string prefabGroupName = Plugin.GetResourceString("PrefabGroupName");
            string prefabGroupName = "Prefabs";
            folder = Path.Combine(folder, prefabGroupName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Get the full name of the file.
            string filename = Path.Combine(folder, "pf_" + SelectedNode.Node.ExportClass);
            string ext = Path.GetExtension(_rootNodeView.RootBehavior.FileManager.Filename);
            filename = Path.ChangeExtension(filename, ext);

            using(SaveAsDialog saveAsDialog = new SaveAsDialog(false))
            {
                saveAsDialog.Text = Resources.SaveAsPrefab;
                saveAsDialog.FileName = _behaviorTreeList.GetUniqueFileName(filename);

                if (saveAsDialog.ShowDialog() == DialogResult.OK)
                {
                    filename = saveAsDialog.FileName;

                    // Create a new behavior node.
                    BehaviorNode behaviorNode = Node.CreateBehaviorNode(SelectedNode.Node.ExportClass);
                    behaviorNode.AgentType = _rootNodeView.RootBehavior.AgentType;
                    ((Node)behaviorNode).AddChild(behaviorNode.GenericChildren, SelectedNode.Node.CloneBranch());

                    string prefabName = FileManagers.FileManager.GetRelativePath(filename);
                    ((Node)behaviorNode).RestorePrefab(prefabName);

                    SelectedNode.Node.SetPrefab(prefabName);
                    SelectedNode.IsExpanded = false;

                    if (string.IsNullOrEmpty(SelectedNode.Node.CommentText))
                    {
                        string prefab = Path.GetFileNameWithoutExtension(prefabName);
                        SelectedNode.Node.CommentText = string.Format("Prefab[{0}]", prefab);
                    }

                    // Copy the used Pars from the current behavior to the new one.
                    foreach (ParInfo par in((Behavior)_rootNodeView.RootBehavior).LocalVars)
                    {
                        List<Node.ErrorCheck> result = new List<Node.ErrorCheck>();
                        Plugin.CheckPar(SelectedNode.Node, par, ref result);

                        if (result.Count > 0)
                        {
                            ((Behavior)behaviorNode).LocalVars.Add(par);
                        }
                    }

                    // Save the new behavior node.
                    behaviorNode.FileManager = _behaviorTreeList.GetFileManagers()[0].Create(filename, behaviorNode);
                    behaviorNode.FileManager.Save();

                    UndoManager.Save(this.RootNode);

                    // Update the behavior list.
                    _behaviorTreeList.RebuildBehaviorList();
                }
            }
        }

        private void applyMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                BehaviorNode prefabBehavior = SelectedNode.Node.ApplyPrefabInstance();

                if (prefabBehavior != null)
                {
                    UndoManager.Save(prefabBehavior);
                }
            }
        }

        private void breakPrefabMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null && SelectedNode.Node.BreakPrefabInstance())
            {
                UndoManager.Save(this.RootNode);
            }
        }

        /// <summary>
        /// Handles when a tree node is dragged on the view.
        /// </summary>
        private void BehaviorTreeView_DragOver(object sender, DragEventArgs e)
        {
            // get the node we are dragging over
            Point pt = PointToClient(new Point(e.X, e.Y));
            NodeViewData nodeFound = _rootNodeView.GetInsideNode(new PointF(pt.X, pt.Y));

            // update last know mouse position
            _lastMousePosition = new PointF(pt.X, pt.Y);

            // update the current node
            if (nodeFound != _currentNode)
            {
                _currentNode = nodeFound;
                Invalidate();
            }

            // when we are moving on a node we must keep drawing as the ttach ode might change but not the node
            else if (nodeFound != null)
            {
                Invalidate();
            }

            // store the target node
            _dragTargetNode = _currentNode;

            // deny drop by default
            e.Effect = DragDropEffects.None;

            // process dragging the property and method string value from the Meta Browser
            string dragItem = (string)e.Data.GetData(DataFormats.Text);

            if (!string.IsNullOrEmpty(dragItem) &&
                _dragTargetNode != null && _dragTargetNode.Node != null &&
                _dragTargetNode.Node.AcceptDefaultPropertyByDragAndDrop())
            {
                e.Effect = DragDropEffects.Move;

                _dragAttachMode = NodeAttachMode.None;

                return;
            }

            // make sure the correct drag attach mode is set
            if (_dragTargetNode != null)
            {
                if (_dragNodeDefaults is Nodes.Node && _dragAttachMode == NodeAttachMode.Attachment)
                {
                    _dragAttachMode = NodeAttachMode.None;

                }
                else if (_dragNodeDefaults is Attachments.Attachment && _dragAttachMode != NodeAttachMode.Attachment)
                {
                    _dragAttachMode = NodeAttachMode.Attachment;
                }
            }

            // check if we are trying to drop a node on another one
            if (_dragTargetNode != null && (e.KeyState & 1/*left mouse button*/) > 0)
            {
                if (_dragNodeDefaults != null &&
                    (_dragNodeDefaults is Nodes.Node && (_dragAttachMode != NodeAttachMode.None || !(_dragNodeDefaults is Nodes.BehaviorNode) || !(_dragTargetNode.Node is Nodes.BehaviorNode)) ||
                     _dragNodeDefaults is Attachments.Attachment && _dragTargetNode.Node.AcceptsAttachment(_dragNodeDefaults)))
                {
                    e.Effect = DragDropEffects.Move;
                }
            }

            // If this is an empty node, no effect for it.
            if (_dragTargetNode == null || _dragTargetNode.Node == null || _dragNodeDefaults == null ||
                _dragAttachMode == NodeAttachMode.None && !_dragTargetNode.Node.AcceptsAttachment(_dragNodeDefaults))
            {
                e.Effect = DragDropEffects.None;
            }

            if (_rootNodeView.IsFSM || _rootNodeView.Children.Count == 0)   // fsm or empty behavior
            {
                if (_dragNodeDefaults != null)
                {
                    if (_dragNodeDefaults is Node)
                    {
                        Node dragNode = _dragNodeDefaults as Node;

                        if (dragNode.IsFSM || dragNode is Behavior)
                        {
                            if (_dragTargetNode == null)
                            {
                                e.Effect = DragDropEffects.Move;
                            }

                            //else if (_dragNodeDefaults is Nodes.BehaviorNode) {
                            //    e.Effect = DragDropEffects.None;
                            //}
                        }

                    }
                    else if (_dragNodeDefaults is Attachments.Attachment)
                    {
                        Attachments.Attachment dragAttachment = _dragNodeDefaults as Attachments.Attachment;

                        if (dragAttachment.IsFSM)
                        {
                            if (_dragTargetNode != null && !(_dragTargetNode.Node is Nodes.BehaviorNode) &&
                                _dragTargetNode.Node.AcceptsAttachment(dragAttachment))
                            {
                                e.Effect = DragDropEffects.Move;
                            }

                            else
                            {
                                e.Effect = DragDropEffects.None;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles when dropping a tree node on the view.
        /// </summary>
        private void BehaviorTreeView_DragDrop(object sender, DragEventArgs e)
        {
            // make sure the view is focused
            Focus();

            // drag the property or method into the node
            string dragItem = (string)e.Data.GetData(DataFormats.Text);

            if (!string.IsNullOrEmpty(dragItem) && _dragTargetNode != null && _dragTargetNode.Node != null)
            {
                if (_dragTargetNode.Node.SetDefaultPropertyByDragAndDrop(dragItem))
                {
                    if (ClickNode != null)
                    {
                        ClickNode(_dragTargetNode);
                    }

                    UndoManager.Save(this.RootNode);

                    LayoutChanged();
                }

                return;
            }

            // get source node
            TreeNode sourceNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

            if (sourceNode == null)
            {
                return;
            }

            NodeTag sourceNodeTag = (NodeTag)sourceNode.Tag;

            // keep the current node position
            //KeepNodePosition(_dragTargetNode);

            bool bDragBTOverNode = !((Node)this.RootNode).IsFSM && sourceNodeTag.Type == NodeTagType.Behavior && _dragTargetNode is Behaviac.Design.NodeViewData;

            // check if we are dropping an attach
            // or if we are dropping a bt to a node and the indicator is not left/right/up/bottom/center
            if (_dragAttachMode == NodeAttachMode.Attachment ||
                (bDragBTOverNode && (_dragAttachMode == NodeAttachMode.None || _dragAttachMode == NodeAttachMode.Attachment)))
            {
                Attachments.Attachment attach = null;

                // when we attach a behaviour we must create a special referenced behaviour node
                if (bDragBTOverNode)
                {
                    //drag an event(a bt) to a node
                    if (!File.Exists(sourceNodeTag.Filename))
                    {
                        MainWindow.Instance.SaveBehavior(sourceNodeTag.Defaults as Nodes.BehaviorNode, false);
                    }

                    if (File.Exists(sourceNodeTag.Filename))
                    {
                        // get the behavior we want to reference
                        BehaviorNode behavior = _behaviorTreeList.LoadBehavior(sourceNodeTag.Filename);
                        Behavior rootB = _rootNodeView.RootBehavior as Behavior;
                        Behavior b = behavior as Behavior;

                        if (!b.CanBeAttached || !IsCompatibleAgentType(rootB, b))
                        {
                            return;
                        }

                        attach = Behaviac.Design.Attachments.Attachment.Create(typeof(Behaviac.Design.Attachments.Event), _dragTargetNode.Node);
                        Behaviac.Design.Attachments.Event evt = (Behaviac.Design.Attachments.Event)attach;
                        evt.ReferencedBehavior = behavior;
                    }

                }
                else if (_dragTargetNode != null)
                {
                    Debug.Check(_dragAttachMode == NodeAttachMode.Attachment);

                    // add the attach to the target node
                    attach = Behaviac.Design.Attachments.Attachment.Create(sourceNodeTag.NodeType, _dragTargetNode.Node);
                }

                if (_dragTargetNode != null && attach != null && _dragTargetNode.Node.AcceptsAttachment(attach))
                {
                    attach.OnPropertyValueChanged(false);

                    attach.ResetId();
                    _dragTargetNode.Node.AddAttachment(attach);

                    NodeViewData.SubItemAttachment sub = attach.CreateSubItem();
                    _dragTargetNode.AddSubItem(sub);

                    SelectedNode = _dragTargetNode;
                    SelectedNode.SelectedSubItem = sub;

                    // call the ClickEvent event handler
                    if (ClickEvent != null)
                    {
                        ClickEvent(SelectedNode);
                    }

                    UndoManager.Save(this.RootNode);

                    LayoutChanged();
                }
            }

            //else if (_dragAttachMode != NodeAttachMode.None)
            else
            {
                // attach a new node to the target node
                InsertNewNode(_dragTargetNode, _dragAttachMode, sourceNodeTag, this.PointToClient(new Point(e.X, e.Y)));
            }

            // reset drag stuff
            _dragTargetNode = null;
            _dragNodeDefaults = null;
            _dragAttachMode = NodeAttachMode.None;

            Invalidate();
        }

        /// <summary>
        /// Handles when a tree node is dragged into the view.
        /// </summary>
        private void BehaviorTreeView_DragEnter(object sender, DragEventArgs e)
        {
            TreeNode sourceNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

            if (sourceNode != null)
            {
                NodeTag sourceNodeTag = sourceNode.Tag as NodeTag;

                if (sourceNodeTag == null)
                {
                    return;
                }

                // store the tree node's defaults
                _dragNodeDefaults = sourceNodeTag.Defaults;
            }
        }

        /// <summary>
        /// Holds if we have the position of a previous dialogue stored.
        /// </summary>
        private static bool _hasOldCheckDialogPosition = false;

        /// <summary>
        /// The previous position of the check dialogue.
        /// </summary>
        private static Point _previousCheckDialogPosition;

        /// <summary>
        /// Handles when the check for errors button is pressed.
        /// </summary>
        private void checkButton_Click(object sender, EventArgs e)
        {
            CheckErrors(_rootNodeView.RootBehavior, false);

            emptyButton.Focus();
        }

        private void agentTypeChanged(AgentType agentType)
        {
            //CheckErrors(_rootNode.RootBehavior, true);

            //PropertiesDock.UpdatePropertyGrids();

            this.Redraw();
        }

        public void CheckErrors(Behaviac.Design.Nodes.BehaviorNode node, bool errorDialogHidesIfNoError)
        {
            // check the current behaviour for errors
            List<Node.ErrorCheck> result = new List<Node.ErrorCheck>();
            _rootNodeView.Node.CheckForErrors(node, result);

            if (errorDialogHidesIfNoError && result.Count == 0)
            {
                return;
            }

            string groupLabel = node.GetPathLabel(_behaviorTreeList.BehaviorFolder);
            ShowErrorDialog(Resources.ErrorCheck, groupLabel, result);
        }

        public void ShowErrorDialog(string title, string groupLabel, List<Node.ErrorCheck> result)
        {
            // store the old position of the check dialogue and close it
            if (_errorCheckDialog != null)
            {
                _errorCheckDialog.Close();
            }

            // prepare the new dialogue
            _errorCheckDialog = new ErrorCheckDialog();
            _errorCheckDialog.Owner = this.ParentForm;
            _errorCheckDialog.BehaviorTreeList = _behaviorTreeList;
            _errorCheckDialog.BehaviorTreeView = this;
            _errorCheckDialog.Text = title;
            _errorCheckDialog.FormClosed += new FormClosedEventHandler(errorCheckDialog_FormClosed);

            // add the errors to the check dialogue
            foreach (Node.ErrorCheck check in result)
            {
                BehaviorNode behavior = check.Node.Behavior;

                // group the errors by the behaviour their occured in

                // get the group for the error's behaviour
                ListViewGroup group = null;

                foreach (ListViewGroup grp in _errorCheckDialog.listView.Groups)
                {
                    if (grp.Tag == behavior)
                    {
                        group = grp;
                        break;
                    }
                }

                // if there is no group, create it
                if (group == null)
                {
                    group = new ListViewGroup(groupLabel);
                    group.Tag = behavior;
                    _errorCheckDialog.listView.Groups.Add(group);
                }

                // create an item for the error in the group
                ListViewItem item = new ListViewItem(check.Description);
                item.Group = group;
                item.Tag = check.Node;

                switch (check.Level)
                {
                    case (ErrorCheckLevel.Message):
                        item.ImageIndex = 0;
                        break;

                    case (ErrorCheckLevel.Warning):
                        item.ImageIndex = 1;
                        break;

                    case (ErrorCheckLevel.Error):
                        item.ImageIndex = 2;
                        break;
                }

                _errorCheckDialog.listView.Items.Add(item);
            }

            // if no errors were found, tell the user so
            if (result.Count < 1)
            {
                _errorCheckDialog.listView.Items.Add(new ListViewItem("No Errors.", (int)ErrorCheckLevel.Message));
            }

            // show the dialogue
            _errorCheckDialog.Show();

            // set its position to the position of the previous dialogue
            if (_hasOldCheckDialogPosition)
            {
                _errorCheckDialog.Location = _previousCheckDialogPosition;
            }
        }

        /// <summary>
        /// Handles when the error check dialogue is closed
        /// </summary>
        void errorCheckDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            // store its previous position
            _previousCheckDialogPosition = _errorCheckDialog.Location;
            _hasOldCheckDialogPosition = true;

            // we have to create a new dialogue the next time so we clear it
            _errorCheckDialog = null;
        }

        public void Export()
        {
            try
            {
                List<Node.ErrorCheck> result = new List<Node.ErrorCheck>();
                _rootNodeView.Node.CheckForErrors(_rootNodeView.RootBehavior, result);

                bool hasErrors = Plugin.GetErrorChecks(result, ErrorCheckLevel.Error).Count > 0;
                bool hasWarnings = Plugin.GetErrorChecks(result, ErrorCheckLevel.Warning).Count > 0;
                bool ignoreErrors = false;

                if (!hasErrors && hasWarnings)
                {
                    DialogResult dr = MessageBox.Show(Resources.ExportWarningInfo, Resources.ExportWarning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (dr == DialogResult.Yes)
                    {
                        ignoreErrors = true;
                    }

                    else
                    {
                        hasErrors = true;
                    }
                }

                if (hasErrors)
                {
                    CheckErrors(_rootNodeView.RootBehavior, true);
                }

                else
                {
                    _behaviorTreeList.ExportBehavior(_rootNodeView.RootBehavior, "", ignoreErrors);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.ExportError, MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Handles when the export button is pressed
        /// </summary>
        private void exportButton_Click(object sender, EventArgs e)
        {
            Export();

            emptyButton.Focus();
        }

        /// <summary>
        /// Save the behavior file.
        /// </summary>
        public bool Save(bool saveAs)
        {
            return MainWindow.Instance.SaveBehavior(_rootNodeView.RootBehavior, saveAs);
        }

        private bool SaveBehavior(string filename)
        {
            Debug.Check(_behaviorTreeList != null);
            if (_behaviorTreeList != null)
            {
                BehaviorNode behavior = _behaviorTreeList.GetBehavior(filename);

                if (behavior != null)
                {
                    return MainWindow.Instance.SaveBehavior(behavior, false);
                }
            }

            return false;
        }

        /// <summary>
        /// Handles when the save button is pressed.
        /// </summary>
        private void saveButton_Click(object sender, EventArgs e)
        {
            Save(false);

            emptyButton.Focus();
        }

        /// <summary>
        /// Handles when the save-as button is pressed.
        /// </summary>
        private void saveAsButton_Click(object sender, EventArgs e)
        {
            Save(true);

            emptyButton.Focus();
        }

        /// <summary>
        /// Centres the given node in the view.
        /// </summary>
        /// <param name="node">The node which will be centred.</param>
        internal void CenterNode(NodeViewData node)
        {
            _maintainNodePosition = node;

            // we use the existing maintain position stuff for that
            RectangleF bbox = node.IsFSM ? node.GetTotalBoundingBox() : node.BoundingBox;

            float width = bbox.Width <= 0.0f ? node.MinWidth : bbox.Width;
            float height = bbox.Height <= 0.0f ? node.MinHeight : bbox.Height;

            // Scale the whole graph to the suitable size in the view.
            float viewWidth = ClientSize.Width - 30;
            float viewHeight = ClientSize.Height - 30;
            bool isWidthScale = false;

            if (viewWidth > 0 && viewHeight > 0)
            {
                SizeF totalSize = node.GetTotalSize(_nodeLayoutManager.Padding.Width, int.MaxValue);
                float scale = 1.0f;

                if (totalSize.Width / totalSize.Height < viewWidth / viewHeight)
                {
                    scale = viewHeight / totalSize.Height;
                    isWidthScale = false;
                }

                else
                {
                    scale = viewWidth / totalSize.Width;
                    isWidthScale = true;
                }

                const float MinScale = 0.1f;
                const float MaxScale = 2.0f;

                if (scale < MinScale)
                {
                    _nodeLayoutManager.Scale = MinScale;
                }

                else if (scale > MaxScale)
                {
                    _nodeLayoutManager.Scale = MaxScale;
                }

                else
                {
                    _nodeLayoutManager.Scale = scale;
                }
            }

            if (node.IsFSM)
            {
                if (isWidthScale)
                {
                    _graphOrigin = new PointF(0.0f, ClientSize.Height * 0.5f - height * 0.5f * _nodeLayoutManager.Scale);
                }
                else
                {
                    _graphOrigin = new PointF(ClientSize.Width * 0.5f - width * 0.5f * _nodeLayoutManager.Scale, 0.0f);
                }
            }
            else
            {
                _graphOrigin = new PointF(20.0f, ClientSize.Height * 0.5f - height * 0.5f);
            }

            Invalidate();
        }

        /// <summary>
        /// Shows the given node in the view.
        /// </summary>
        /// <param name="node">The node which will be centred.</param>
        internal void ShowNode(NodeViewData node)
        {
            // Expand all of its parents.
            bool layoutChanged = false;
            NodeViewData parent = node.Parent;

            while (parent != null)
            {
                if (!parent.IsExpanded)
                {
                    parent.IsExpanded = true;
                    layoutChanged = true;
                }

                parent = parent.Parent;
            }

            if (layoutChanged)
            {
                LayoutChanged();
            }

            // we use the existing maintain position stuff for that
            RectangleF bbox = node.BoundingBox;

            // If the node was not yet shown there is no bounding box,
            // so we simply use the min width and height for that.
            float width = bbox.Width <= 0.0f ? node.MinWidth : bbox.Width;
            float height = bbox.Height <= 0.0f ? node.MinHeight : bbox.Height;

            RectangleF displayBox = node.DisplayBoundingBox;

            // If this display bounding box is in the view, the node need not be moved.
            if (displayBox.X >= 0 && displayBox.X <= ClientSize.Width - width &&
                displayBox.Y >= 0 && displayBox.Y <= ClientSize.Height - height)
            {
                return;
            }

            _maintainNodePosition = node;

            _graphOrigin = new PointF(ClientSize.Width * 0.5f - width * 0.5f, ClientSize.Height * 0.5f - height * 0.5f);

            Invalidate();
        }

        /// <summary>
        /// Stores if the root behavior is supposed to be centres or not.
        /// </summary>
        protected bool _pendingCenterBehavior = false;

        /// <summary>
        /// Centres the given node in the view.
        /// </summary>
        /// <param name="node">The node which will be centred.</param>
        /// <returns>Returns the NodeViewData of the node which was centred.</returns>
        internal NodeViewData CenterNode(Node node)
        {
            NodeViewData nvd = _rootNodeView.FindNodeViewData(node);

            if (nvd != null)
            {
                CenterNode(nvd);
            }

            return nvd;
        }

        internal void SaveImage()
        {
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                NodeLayoutManager nlm = new NodeLayoutManager(_nodeLayoutManager.RootNodeLayout, _nodeLayoutManager.EdgePen, _nodeLayoutManager.EdgePenSelected, _nodeLayoutManager.EdgePenHighlighted, _nodeLayoutManager.EdgePenUpdate, _nodeLayoutManager.EdgePenReadOnly, false);
                nlm.Offset = new PointF(1.0f, 1.0f);

                using(Graphics g = CreateGraphics())
                {
                    nlm.UpdateLayout(g);
                }

                SizeF totalSize = nlm.RootNodeLayout.GetTotalSize(nlm.Padding.Width, int.MaxValue);
                Size newimageSize = new Size((int)Math.Ceiling(totalSize.Width) + 2, (int)Math.Ceiling(totalSize.Height) + 2);

                Graphics formGraphics = null;
                IntPtr hdc = new IntPtr();
                Image img = null;
                bool needsSave = true;
                //if (saveImageDialog.FilterIndex == 1)
                //{
                //    formGraphics = CreateGraphics();
                //    hdc = formGraphics.GetHdc();

                //    img = new Metafile(saveImageDialog.FileName, hdc);

                //    needsSave = false;
                //}
                //else if (saveImageDialog.FilterIndex == 2)
                {
                    img = new Bitmap(newimageSize.Width, newimageSize.Height);
                }

                using(Graphics graphics = Graphics.FromImage(img))
                {
                    nlm.DrawGraph(graphics, new PointF());
                }

                if (needsSave)
                {
                    img.Save(saveImageDialog.FileName);
                }

                img.Dispose();

                if (formGraphics != null)
                {
                    formGraphics.ReleaseHdc(hdc);
                    formGraphics.Dispose();
                }
            }
        }

        private void imageButton_Click(object sender, EventArgs e)
        {
            SaveImage();

            emptyButton.Focus();
        }

        private void propertiesButton_Click(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                PropertiesDock.CreateFloatDock(SelectedNode.RootBehavior, SelectedNode.Node);
            }

            emptyButton.Focus();
        }

        private void parameterSettingButton_Click(object sender, EventArgs e)
        {
            MetaStoreDock.Inspect(null);

            emptyButton.Focus();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            object selectedNode = null;

            if (SelectedNode != null)
            {
                if (SelectedNode.SelectedSubItem != null && SelectedNode.SelectedSubItem.SelectableObject != null)
                {
                    selectedNode = SelectedNode.SelectedSubItem.SelectableObject;

                }
                else
                {
                    selectedNode = SelectedNode.Node;
                }
            }

            PropertiesDock.InspectObject(this.RootNode, selectedNode, false);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            _lostFocus = true;
        }
    }
}
