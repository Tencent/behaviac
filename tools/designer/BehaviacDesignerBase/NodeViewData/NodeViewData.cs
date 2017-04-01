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
using System.Windows.Forms;
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;
using TTRider.UI;

namespace Behaviac.Design
{
    /// <summary>
    /// This enumeration defines the shape of the ode when it is showed in the graph.
    /// </summary>
    public enum NodeShape { Rectangle, RoundedRectangle, Capsule, Ellipse, AngleRectangle, CornerRectangle }

    public enum SubItemRegin { Out, Top, Bottom }

    /// <summary>
    /// This class represents a node which is drawn in a view.
    /// </summary>
    public partial class NodeViewData : BaseNode
    {
        public static bool ShowNodeId = false;
        public static bool IsDisplayLengthLimited = false;
        public static int LimitedDisplayLength = 30;

        protected Node _node;

        /// <summary>
        /// The node this view is for.
        /// </summary>
        public virtual Node Node
        {
            get
            {
                return _node;
            }
        }

        /// <summary>
        /// The parent NodeViewData of this node.
        /// </summary>
        public new NodeViewData Parent
        {
            get
            {
                return (NodeViewData)base.Parent;
            }
        }

        public NodeViewData RootNodeView
        {
            get
            {
                BaseNode parent = this;

                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                return parent as NodeViewData;
            }
        }

        protected BehaviorNode _rootBehavior;

        /// <summary>
        /// The behaviour which owns this view as it is the root of the shown graph.
        /// </summary>
        public BehaviorNode RootBehavior
        {
            get
            {
                return _rootBehavior;
            }
        }

        public override bool IsFSM
        {
            get
            {
                return (_node != null) && _node.IsFSM;
            }
        }

        private enum BreakPointStates
        {
            None,
            Normal,
            Disable,
            Highlight
        }

        private Brush getBrush(BreakPointStates state)
        {
            switch (state)
            {
                case BreakPointStates.Normal:
                    return Brushes.Purple;

                case BreakPointStates.Disable:
                    return Brushes.White;

                case BreakPointStates.Highlight:
                    return Brushes.Yellow;
            }

            return null;
        }

        private BreakPointStates getBreakPointState(HighlightBreakPoint highlightBreakPoint, string actionName)
        {
            Behavior behavior = RootBehavior as Behavior;

            if (behavior != null && !string.IsNullOrEmpty(behavior.Filename))
            {
                string behaviorName = behavior.MakeRelative(behavior.Filename);
                string fullId = FullId;
                DebugDataPool.BreakPoint breakPoint = DebugDataPool.FindBreakPoint(behaviorName, fullId, actionName);

                if (breakPoint != null)
                {
                    if (breakPoint.IsEnable(actionName))
                    {
                        if (highlightBreakPoint != null &&
                            highlightBreakPoint.NodeId == fullId &&
                            highlightBreakPoint.ActionName == actionName)
                        {
                            return BreakPointStates.Highlight;
                        }

                        return BreakPointStates.Normal;
                    }

                    return BreakPointStates.Disable;
                }
            }

            return BreakPointStates.None;
        }

        public bool CanBeDragged()
        {
            // The node in the referenced tree can't be dragged.
            return (Plugin.EditMode == EditModes.Design) && Node.CanBeDragged();
        }

        public bool CanBeDeleted(bool isSubTree = false)
        {
            // The node in the referenced tree can't be deleted.
            return (Plugin.EditMode == EditModes.Design) && (isSubTree || Node.CanBeDeleted());
        }

        public virtual bool CanBeExpanded()
        {
            return this.Children != null && this.Children.Count > 0 && !this.Node.AlwaysExpanded();
        }

        public virtual bool IsExpanded
        {
            get
            {
                if (this.Parent == null || this.Node.AlwaysExpanded())
                {
                    return true;
                }

                return ExpandedNodePool.IsExpandedNode(this);
            }

            set
            {
                if (CanBeExpanded())
                {
                    ExpandedNodePool.SetExpandedNode(this, value);
                }
            }
        }

        public bool CheckAllParentsExpanded()
        {
            NodeViewData parent = this;

            while (parent != null)
            {
                if (!parent.IsExpanded && parent.CanBeExpanded())
                {
                    return false;
                }

                parent = parent.Parent;
            }

            return true;
        }

        public void SetAllParentsExpanded()
        {
            NodeViewData parent = this;

            while (parent != null)
            {
                if (parent.CanBeExpanded())
                {
                    parent.IsExpanded = true;
                }

                parent = parent.Parent;
            }
        }

        public void ExpandAll(bool isExpanded)
        {
            this.IsExpanded = isExpanded;

            foreach (NodeViewData nvd in this.Children)
            {
                nvd.ExpandAll(isExpanded);
            }
        }

        private string _fullId = null;
        /// <summary>
        /// Get the full ID by the referenced nodes.
        /// </summary>
        public string FullId
        {
            get
            {
                if (string.IsNullOrEmpty(_fullId))
                {
                    Debug.Check(_node != null);

                    if (_node != null)
                    {
                        _fullId = _node.Id.ToString();

                        foreach (NodeViewData nvd in ReferencedNodeViews)
                        {
                            _fullId = nvd.Node.Id.ToString() + ":" + _fullId;
                        }
                    }
                }

                return _fullId;
            }
        }

        /// <summary>
        /// Return all referened nodeViewData parents.
        /// </summary>
        protected IList<NodeViewData> ReferencedNodeViews
        {
            get
            {
                IList<NodeViewData> referencedNodeViews = new List<NodeViewData>();

                NodeViewData nvd = this.Parent;

                while (nvd != null)
                {
                    if (nvd.Node is ReferencedBehavior)
                    {
                        referencedNodeViews.Add(nvd);
                    }

                    nvd = nvd.Parent;
                }

                return referencedNodeViews;
            }
        }

        public bool IsBehaviorReferenced(BehaviorNode behavior)
        {
            if (behavior != null && this.Parent != null)
            {
                foreach (NodeViewData nvd in ReferencedNodeViews)
                {
                    Debug.Check(nvd.Node is ReferencedBehavior);

                    if (((ReferencedBehavior)nvd.Node).ReferenceBehaviorNode == behavior)
                    {
                        return true;
                    }
                }

                if (this.RootBehavior == behavior)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add a new child but the behaviour does not need to be saved.
        /// Used for collapsed referenced behaviours which show the behaviours they reference.
        /// </summary>
        /// <param name="connector">The connector the node will be added to. Use null for default connector.</param>
        /// <param name="node">The node you want to append.</param>
        /// <returns>Returns true if the child could be added.</returns>
        public bool AddChildNotModified(Connector connector, NodeViewData node)
        {
            Debug.Check(connector != null && _children.HasConnector(connector));

            if (connector != null)
            {
                if (!connector.AcceptsChild(node.Node))
                {
                    throw new Exception(Resources.ExceptionNodeHasTooManyChildren);
                }

                if (!connector.AddChild(node))
                {
                    return false;
                }

                node._parent = this;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the exact size of a string.
        /// Code taken from http://www.codeproject.com/KB/GDI-plus/measurestring.aspx
        /// </summary>
        /// <param name="graphics">The graphics object used to calculate the string's size.</param>
        /// <param name="font">The font which will be used to draw the string.</param>
        /// <param name="text">The actual string which will be drawn.</param>
        /// <returns>Returns the untransformed size of the string when being drawn.</returns>
        static public SizeF MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            // set something to generate the minimum size
            bool minimum = false;

            if (text == string.Empty)
            {
                minimum = true;
                text = " ";
            }

            System.Drawing.StringFormat format = new System.Drawing.StringFormat();
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
            System.Drawing.CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return minimum ? new SizeF(0.0f, rect.Height) : rect.Size;
        }

        protected List<SubItem> _subItems = new List<SubItem>();

        /// <summary>
        /// The list of subitems handled by this node.
        /// </summary>
        public IList<SubItem> SubItems
        {
            get
            {
                return _subItems.AsReadOnly();
            }
        }

        /// <summary>
        /// Sorts the subitems so the parallel ones are drawn last, otherwise we get glitches with the backgrounds drawn by the non-prallel subitems.
        /// </summary>
        protected void SortSubItems()
        {
            // find the last parallel subitem from the beginning on
            int lastParallelIndex = -1;

            for (int i = 0; i < _subItems.Count; ++i)
            {
                if (_subItems[i].ShowParallelToLabel)
                {
                    lastParallelIndex = i;
                }

                else
                {
                    break;    // once we found a subitem which is not parallel we quit
                }
            }

            // sort the subitems
            for (int i = 0; i < _subItems.Count; ++i)
            {
                // if we found a parallel past the last one we sort it
                if (_subItems[i].ShowParallelToLabel && i > lastParallelIndex)
                {
                    SubItem parallel = _subItems[i];
                    _subItems.RemoveAt(i--);
                    _subItems.Insert(++lastParallelIndex, parallel);
                }
            }

            List<SubItem> transitionSubItems = new List<SubItem>();
            List<SubItem> effectorSubItems = new List<SubItem>();

            for (int i = _subItems.Count - 1; i >= 0; --i)
            {
                if (_subItems[i].SelectableObject != null)
                {
                    Attachments.Attachment attachment = _subItems[i].SelectableObject as Attachments.Attachment;

                    if (attachment != null && !attachment.IsPrecondition)
                    {
                        if (attachment.IsTransition)
                        {
                            transitionSubItems.Add(_subItems[i]);
                        }
                        else     //if (attachment.IsEffector)
                        {
                            effectorSubItems.Add(_subItems[i]);
                        }

                        _subItems.RemoveAt(i);
                    }
                }
            }

            for (int i = transitionSubItems.Count - 1; i >= 0; --i)
            {
                _subItems.Add(transitionSubItems[i]);
            }

            for (int i = effectorSubItems.Count - 1; i >= 0; --i)
            {
                _subItems.Add(effectorSubItems[i]);
            }
        }

        /// <summary>
        /// Attaches a subitem to this node.
        /// </summary>
        /// <param name="sub">The node subitem we want to attach.</param>
        public void AddSubItem(SubItem sub)
        {
            _subItems.Add(sub);

            SortSubItems();

            _labelChanged = true;
        }

        /// <summary>
        /// Attaches a subitem to this node.
        /// </summary>
        /// <param name="sub">The node subitem we want to attach.</param>
        /// <param name="index">The index where you want to insert the subitem.</param>
        public void AddSubItem(SubItem sub, int index)
        {
            _subItems.Insert(index, sub);

            SortSubItems();

            _labelChanged = true;
        }

        /// <summary>
        /// Removes a subitem from the node.
        /// </summary>
        /// <param name="sub">The subitem which will be removed.</param>
        public void RemoveSubItem(SubItem sub)
        {
            int index = _subItems.IndexOf(sub);

            if (sub == _selectedSubItem)
            {
                _selectedSubItem.IsSelected = false;
                _selectedSubItem = null;
            }

            if (index < 0)
            {
                throw new Exception(Resources.ExceptionSubItemIsNoChild);
            }

            _subItems.RemoveAt(index);
        }

        /// <summary>
        /// Removes the selected event from the node.
        /// </summary>
        public bool RemoveSelectedSubItem()
        {
            if (!_selectedSubItem.CanBeDeleted)
            {
                return false;
            }

            var attach = _selectedSubItem as SubItemAttachment;

            if (attach != null)
            {
                _node.RemoveAttachment(attach.Attachment);
            }

            RemoveSubItem(_selectedSubItem);
            return true;
        }

        /// <summary>
        /// The the currently selected subitem.
        /// </summary>
        private SubItem _selectedSubItem;

        /// <summary>
        /// Returns the currently selected subitem. Is null if no subitem is selected.
        /// </summary>
        public SubItem SelectedSubItem
        {
            get
            {
                return _selectedSubItem;
            }

            set
            {
                if (_selectedSubItem != null)
                {
                    _selectedSubItem.IsSelected = false;
                }

                _selectedSubItem = value;

                if (_selectedSubItem != null)
                {
                    _selectedSubItem.IsSelected = true;
                }
            }
        }

        public void SelectFirstSubItem()
        {
            for (int i = 0; i < _subItems.Count; ++i)
            {
                if (_subItems[i].SelectableObject != null)
                {
                    SelectedSubItem = _subItems[i];
                    break;
                }
            }
        }

        public SubItemAttachment GetSubItemByDrawnPath(PointF mousePos, Pen pen)
        {
            if (this.IsFSM)
            {
                foreach (SubItem item in this.SubItems)
                {
                    if (item is SubItemAttachment)
                    {
                        SubItemAttachment attach = item as SubItemAttachment;

                        if (attach.DrawnPath != null && attach.DrawnPath.IsOutlineVisible(mousePos, pen))
                        {
                            return attach;
                        }
                    }
                }

                foreach (NodeViewData child in this.GetChildNodes())
                {
                    SubItemAttachment attach = child.GetSubItemByDrawnPath(mousePos, pen);

                    if (attach != null)
                    {
                        return attach;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the subitem previous to the currently selected subitem. Is null if no subitem is previous.
        /// </summary>
        public SubItem PreviousSelectedSubItem
        {
            get
            {
                if (_selectedSubItem != null && _subItems.Count > 1)
                {
                    int index = -1;

                    for (int i = 0; i < _subItems.Count; ++i)
                    {
                        if (_subItems[i] == _selectedSubItem)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index > -1)
                    {
                        for (int i = index - 1; i >= 0; --i)
                        {
                            if (_subItems[i].SelectableObject != null)
                            {
                                return _subItems[i];
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the subitem next to the currently selected subitem. Is null if no subitem is next.
        /// </summary>
        public SubItem NextSelectedSubItem
        {
            get
            {
                if (_selectedSubItem != null && _subItems.Count > 1)
                {
                    int index = -1;

                    for (int i = 0; i < _subItems.Count; ++i)
                    {
                        if (_subItems[i] == _selectedSubItem)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index > -1)
                    {
                        for (int i = index + 1; i < _subItems.Count; ++i)
                        {
                            if (_subItems[i].SelectableObject != null)
                            {
                                return _subItems[i];
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// The tooltip for this node which is shown if the option is enabled in the settings.
        /// </summary>
        public virtual string ToolTip
        {
            get
            {
                return _node.Description;
            }
        }

        /// <summary>
        /// Returns the first NodeViewData which is associated with the given node. Notice that there might be other NodeViewDatas which are ignored.
        /// </summary>
        /// <param name="node">The node you want to get the NodeViewData for.</param>
        /// <returns>Returns the first NodeViewData found.</returns>
        public NodeViewData FindNodeViewData(Node node)
        {
            if (node == null)
            {
                return null;
            }

            // check if this is a fitting view
            if (_node == node)
            {
                return this;
            }

            // search the children
            foreach (NodeViewData child in this.GetChildNodes())
            {
                NodeViewData result = child.FindNodeViewData(node);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public void FindNodeViewDatas(Node node, ref List<NodeViewData> allNodeViewDatas)
        {
            if (node != null)
            {
                // check if this is a fitting view
                if (_node == node)
                {
                    allNodeViewDatas.Add(this);
                }

                // search the children
                foreach (NodeViewData child in this.GetChildNodes())
                {
                    child.FindNodeViewDatas(node, ref allNodeViewDatas);
                }
            }
        }

        /// <summary>
        /// Returns the NodeViewData with the nvdFullId.
        /// </summary>
        /// <param name="nvdFullId">The full id of the node you want to get the NodeViewData for.</param>
        /// <returns>Returns the first NodeViewData found.</returns>
        public NodeViewData FindNodeViewData(string nvdFullId)
        {
            // check if this is a fitting view
            if (this.FullId == nvdFullId)
            {
                return this;
            }

            // search the children
            foreach (NodeViewData child in this.GetChildNodes())
            {
                NodeViewData result = child.FindNodeViewData(nvdFullId);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns whether or not the view data needs to be rebuilt because the tree changed.
        /// </summary>
        /// <returns>Returns true when the tree needs to be rebuilt.</returns>
        protected virtual bool NeedsToSynchronizeWithNode()
        {
            // if the counts do not fit we must rebuild
            bool rebuild = _node.Children.Count != _children.ChildCount ||
                           _node.FSMNodes.Count != this.FSMNodes.Count;

            // check if all children are associated to the correct children of the node
            if (!rebuild)
            {
                foreach (Connector connector in _node.Connectors)
                {
                    // check if the child count is different
                    Connector localConnector = _children.GetConnector(connector.Identifier);

                    if (localConnector == null || connector.ChildCount != localConnector.ChildCount)
                    {
                        rebuild = true;
                        break;
                    }

                    // check if the children are still the same
                    for (int i = 0; i < localConnector.ChildCount; ++i)
                    {
                        NodeViewData nvd = (NodeViewData)localConnector.GetChild(i);

                        if (nvd.Node != connector.GetChild(i))
                        {
                            rebuild = true;
                            break;
                        }
                    }
                }
            }

            return rebuild;
        }

        /// <summary>
        /// Removes all sub items which are used for the connectors.
        /// </summary>
        protected void RemoveAllConnectorSubItems()
        {
            for (int i = 0; i < _subItems.Count; ++i)
            {
                RemoveSubItem(_subItems[i]);
                i--;
            }
        }

        /// <summary>
        /// Creates the view data for a node.
        /// </summary>
        /// <param name="processedBehaviors">The list of processed behaviours to handle circular references.</param>
        public virtual void DoSynchronizeWithNode(ProcessedBehaviors processedBehaviors)
        {
            _children.ClearConnectors();
            RemoveAllConnectorSubItems();

            // create subitems.
            setSubItems(_node);

            foreach (Connector connector in _node.Connectors)
            {
                connector.Clone(_children);
            }

            foreach (Connector connector in _children.Connectors)
            {
                Connector nodeConnector = _node.GetConnector(connector.Identifier);
                Debug.Check(nodeConnector != null);

                if (nodeConnector != null)
                {
                    for (int i = 0; i < nodeConnector.ChildCount; ++i)
                    {
                        Node node = (Node)nodeConnector.GetChild(i);
                        NodeViewData nvd = node.CreateNodeViewData(this, _rootBehavior);
                        Debug.Verify(AddChildNotModified(connector, nvd));
                    }
                }
            }

            // create FSM nodes
            this.FSMNodes.Clear();

            if (_node.FSMNodes.Count > 0)
            {
                foreach (Node node in _node.FSMNodes)
                {
                    NodeViewData nvd = node.CreateNodeViewData(this, _rootBehavior);
                    this.AddFSMNode(nvd);
                }
            }
        }

        /// <summary>
        /// This function adapts the children of the view that they represent the children of the node this view is for.
        /// Children are added and removed.
        /// </summary>
        /// <param name="processedBehaviors">A list of previously processed behaviours to deal with circular references.</param>
        public virtual void SynchronizeWithNode(ProcessedBehaviors processedBehaviors, bool bForce = false)
        {
            if (processedBehaviors.MayProcess(_node))
            {
                // check if we must rebuild the child list
                if (bForce || NeedsToSynchronizeWithNode())
                {
                    DoSynchronizeWithNode(processedBehaviors);
                }

                GenerateNewLabel();

                // synchronise the children as well
                foreach (NodeViewData child in this.GetChildNodes())
                {
                    Debug.Check(child.RootBehavior == _rootBehavior);

                    child.SynchronizeWithNode(processedBehaviors.Branch(child._node), bForce);
                }
            }
        }

        /// <summary>
        /// The label used to generate the final label which can include parameters and other stuff.
        /// </summary>
        protected string BaseLabel
        {
            get
            {
                return _node.Label;
            }
        }

        protected string _label;

        /// <summary>
        /// The label shown on the node.
        /// </summary>
        public string Label
        {
            get
            {
                return _label;
            }
        }

        public string DisplayLabel
        {
            get
            {
                if (NodeViewData.IsDisplayLengthLimited &&
                    !string.IsNullOrEmpty(this.Label) &&
                    this.Label.Length > NodeViewData.LimitedDisplayLength)
                {
                    return this.Label.Substring(0, NodeViewData.LimitedDisplayLength) + "...";
                }

                return this.Label;
            }
        }

        public override string ToString()
        {
            return _label;
        }

        protected NodeShape _shape;
        public void ChangeShape(NodeShape shape)
        {
            _shape = shape;
        }

        /// <summary>
        /// The shape of the node.
        /// </summary>
        public NodeShape Shape
        {
            get
            {
                return _shape;
            }
        }

        protected int _minHeight;

        /// <summary>
        /// The minimum height of the node. Can be expanded by events and the label.
        /// </summary>
        public int MinHeight
        {
            get
            {
                return _minHeight;
            }
        }

        protected int _minWidth;

        /// <summary>
        /// The minimum width of the node. Can be expanded by events and the label.
        /// </summary>
        public int MinWidth
        {
            get
            {
                return _minWidth;
            }
        }

        protected Font _font;
        protected Font _profileFont;
        protected Font _profileBoldFont;
        protected SizeF _labelSize, _realLabelSize;
        protected float _subItemParallelWidth;

        /// <summary>
        /// The default style of the node when it is neither hover over or selected.
        /// </summary>
        protected Style _defaultStyle;

        /// <summary>
        /// The style of the node when the mouse is hovering over it.
        /// </summary>
        protected Style _currentStyle;

        /// <summary>
        /// The style of the node when it is selected.
        /// </summary>
        protected Style _selectedStyle;

        /// <summary>
        /// The style of the node when it is moved inside the tree and shown as a small tree which is attached to the mouse.
        /// </summary>
        protected Style _draggedStyle;

        /// <summary>
        /// The style of the node when it is highlighted when debugging.
        /// </summary>
        protected Style _highlightedStyle;

        /// <summary>
        /// The style of the node when it is updated when debugging.
        /// </summary>
        protected Style _updatedStyle;

        protected Style _prefabStyle;

        protected SizeF _finalSize;

        protected bool _labelChanged = true;

        public static Brush GetDraggedBrush(Brush brush)
        {
            // extract the color
            Color clr = new Pen(brush).Color;

            // generate the hsb color
            HSBColor hsb = new HSBColor(clr);

            // generate the dragged color
            HSBColor draggedhsb = new HSBColor(hsb.A, hsb.H, hsb.S, hsb.B - 10.0f);

            // check if we got a solid brush so we also return one
            SolidBrush sb = brush as SolidBrush;

            if (sb != null)
            {
                return new SolidBrush(draggedhsb.Color);
            }

            // unhandled brush type
            Debug.Check(false);

            // if the brush type was not handled in release mode we return a solid brush by default
            return new SolidBrush(draggedhsb.Color);
        }

        public void SetBackgroundBrush()
        {
            _defaultStyle.Background = this.Node.BackgroundBrush;
            _draggedStyle.Background = GetDraggedBrush(_defaultStyle.Background);
        }

        /// <summary>
        /// Creates a new view for a given node.
        /// </summary>
        /// <param name="parent">The parent of the new NodeViewData.</param>
        /// <param name="rootBehavior">The behaviour which is the root of the graph the given node is shown in.</param>
        /// <param name="node">The node the view is created for.</param>
        /// <param name="shape">The shape of the node when being rendered.</param>
        /// <param name="defaultStyle">The stle of the node when being neither hovered over nor selected.</param>
        /// <param name="currentStyle">The style of the node when the mouse is hovering over it.</param>
        /// <param name="selectedStyle">The style of the node when it is selected.</param>
        /// <param name="draggedStyle">The style of the node when it is attached to the mouse cursor when moving nodes in the graph.</param>
        /// <param name="label">The default label of the node.</param>
        /// <param name="font">The font used for the label.</param>
        /// <param name="minWidth">The minimum width of the node.</param>
        /// <param name="minHeight">The minimum height of the node.</param>
        /// <param name="description">The description of the node shown to the designer.</param>
        public NodeViewData(NodeViewData parent, BehaviorNode rootBehavior, Node node, NodeShape shape,
                            Style defaultStyle, Style currentStyle, Style selectedStyle, Style draggedStyle, Style highlightedStyle, Style updatedStyle, Style prefabStyle,
                            string label, Font font, Font profileFont, Font profileBoldFont, int minWidth, int minHeight, string description)
        {
            Debug.Check(rootBehavior != null);

            if (rootBehavior != null)
            {
                _rootBehavior = rootBehavior;
            }

            if (node != null)
            {
                _node = node;
                _node.SubItemAdded += node_SubItemAdded;
            }

            _shape = shape;
            _font = font;
            _profileFont = profileFont;
            _profileBoldFont = profileBoldFont;
            _minWidth = minWidth;
            _minHeight = minHeight;

            if (defaultStyle == null)
            {
                throw new Exception(Resources.ExceptionDefaultStyleNull);
            }

            _defaultStyle = defaultStyle;
            _currentStyle = currentStyle;
            _selectedStyle = selectedStyle;
            _draggedStyle = draggedStyle;
            _highlightedStyle = highlightedStyle;
            _updatedStyle = updatedStyle;
            _prefabStyle = prefabStyle;

            if (node != null)
            {
                setSubItems(node);
            }

            GenerateNewLabel();
        }

        private void setSubItems(Node node)
        {
            // Add all listed properties
            IList<DesignerPropertyInfo> properties = node.GetDesignerProperties(DesignerProperty.SortByDisplayOrder);

            for (int p = 0; p < properties.Count; ++p)
            {
                DesignerProperty att = properties[p].Attribute;

                if (att.Display == DesignerProperty.DisplayMode.List || att.Display == DesignerProperty.DisplayMode.ListTrue)
                {
                    object pValue = properties[p].Property.GetValue(node, null);
                    bool bDo = pValue != null;

                    if (bDo)
                    {
                        bool bDisplay = true;

                        if (att.Display == DesignerProperty.DisplayMode.ListTrue)
                        {
                            if (pValue is bool)
                            {
                                bool bValue = (bool)pValue;

                                if (bValue)
                                {
                                    //bDisplay = true;
                                }
                                else
                                {
                                    bDisplay = false;
                                }
                            }
                        }

                        if (bDisplay)
                        {
                            AddSubItem(new SubItemProperty(node, properties[p].Property, att));
                        }
                    }
                }
            }

            // Add all attachments
            foreach (Attachment attach in node.Attachments)
            {
                AddSubItem(attach.CreateSubItem());
            }
        }

        /// <summary>
        /// Updates the with of the node. For internal use only. Used to give all children the same width.
        /// </summary>
        /// <param name="width">The untransformed with.</param>
        internal void SetWidth(float width)
        {
            _finalSize.Width = width;
        }

        const float Padding = 6.0f;

        /// <summary>
        /// Is called when a property of the selected event of this node was modified.
        /// </summary>
        /// <param name="wasModified">Holds if the node was modified.</param>
        public virtual void OnSubItemPropertyValueChanged(bool wasModified)
        {
            // when the label changes the size of the node might change as well
            _rootBehavior.TriggerWasModified(_node);
        }

        /// <summary>
        /// Calculates the final size of the node.
        /// </summary>
        /// <param name="graphics">The graphics used to measure the size of the labels.</param>
        /// <param name="rootBehavior">The behaviour this node belongs to.</param>
        public virtual void UpdateFinalSize(Graphics graphics, BehaviorNode rootBehavior)
        {
#if DEBUG
            //ensure consistency
            DebugCheckIntegrity();
#endif

            // find the widest node
            float maxWidth = 0.0f;

            foreach (NodeViewData node in this.GetChildNodes())
            {
                node.UpdateFinalSize(graphics, rootBehavior);

                maxWidth = Math.Max(maxWidth, node._finalSize.Width);
            }

            // give all non-fsm children the same width
            foreach (NodeViewData node in _children)
            {
                node.SetWidth(maxWidth);
            }

            // update the label if it has changed
            if (_labelChanged)
            {
                _labelChanged = false;
                _labelSize = MeasureDisplayStringWidth(graphics, this.DisplayLabel, _font);
                _labelSize.Width += 2.0f;

                // update the subitems
                float subItemHeight = 0.0f;
                float subItemWidth = 0.0f;
                float subItemParallelHeight = 0.0f;
                _subItemParallelWidth = 0.0f;

                foreach (SubItem subitem in _subItems)
                {
                    // call update
                    subitem.Update(this, graphics);

                    // store the required space depending on parallel and non-parallel subitems
                    if (subitem.ShowParallelToLabel)
                    {
                        if (this.IsExpanded)
                        {
                            subItemParallelHeight += subitem.Height;
                            _subItemParallelWidth = Math.Max(_subItemParallelWidth, subitem.Width);
                        }
                    }
                    else
                    {
                        subItemHeight += subitem.Height;
                        subItemWidth = Math.Max(subItemWidth, subitem.Width);
                    }
                }

                // if we have no parallel subitem, we also need no extra padding
                if (_subItemParallelWidth > 0.0f)
                {
                    _subItemParallelWidth += Padding;
                }

                // the height of the label is its own height or the height of all the parallel subitems
                _realLabelSize = _labelSize;
                _labelSize.Width = Math.Max(subItemWidth, _labelSize.Width) + _subItemParallelWidth;
                _labelSize.Height = Math.Max(_labelSize.Height + Padding * 2.0f, subItemParallelHeight);

                // calculate the final size of the node
                _finalSize.Width = Math.Max(_minWidth, _labelSize.Width + Padding * 2.0f);
                _finalSize.Height = Math.Max(_minHeight, _labelSize.Height + subItemHeight);
            }
        }

        /// <summary>
        /// Draws the background and shape of the node
        /// </summary>
        /// <param name="graphics">The grpahics object we render to.</param>
        /// <param name="boundingBox">The untransformed bounding box of the node.</param>
        /// <param name="brush">The brush used for the background.</param>
        public void DrawShapeBackground(Graphics graphics, RectangleF boundingBox, Brush brush)
        {
            switch (_shape)
            {
                case (NodeShape.Rectangle):
                    graphics.FillRectangle(brush, boundingBox);
                    break;

                case (NodeShape.Ellipse):
                    graphics.FillEllipse(brush, boundingBox);
                    break;

                case (NodeShape.Capsule):
                case (NodeShape.RoundedRectangle):
                {
                    float ratio = _shape == NodeShape.Capsule ? 0.5f : 0.3f;
                    System.Drawing.Extended.ExtendedGraphics extended = new System.Drawing.Extended.ExtendedGraphics(graphics);
                    extended.FillRoundRectangle(brush, boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height, ratio);
                }
                break;

                case (NodeShape.AngleRectangle):
                case (NodeShape.CornerRectangle):
                {
                    float ratio = _shape == NodeShape.AngleRectangle ? 0.5f : 0.33f;
                    System.Drawing.Extended.ExtendedGraphics extended = new System.Drawing.Extended.ExtendedGraphics(graphics);
                    extended.FillCornerRectangle(brush, boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height, ratio);
                }
                break;

                default:
                    throw new Exception(Resources.ExceptionUnhandledNodeShape);
            }
        }

        /// <summary>
        /// Draw the border of the node.
        /// </summary>
        /// <param name="graphics">The grpahics object we render to.</param>
        /// <param name="boundingBox">The untransformed bounding box of the node.</param>
        /// <param name="pen">The pen we use.</param>
        protected void DrawShapeBorder(Graphics graphics, RectangleF boundingBox, Pen pen)
        {
            switch (_shape)
            {
                case (NodeShape.Rectangle):
                    graphics.DrawRectangle(pen, boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
                    break;

                case (NodeShape.Ellipse):
                    graphics.DrawEllipse(pen, boundingBox);
                    break;

                case (NodeShape.Capsule):
                case (NodeShape.RoundedRectangle):
                {
                    float ratio = _shape == NodeShape.Capsule ? 0.5f : 0.3f;
                    System.Drawing.Extended.ExtendedGraphics extended = new System.Drawing.Extended.ExtendedGraphics(graphics);
                    extended.DrawRoundRectangle(pen, boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height, ratio);
                }
                break;

                case (NodeShape.AngleRectangle):
                case (NodeShape.CornerRectangle):
                {
                    float ratio = _shape == NodeShape.AngleRectangle ? 0.5f : 0.33f;
                    System.Drawing.Extended.ExtendedGraphics extended = new System.Drawing.Extended.ExtendedGraphics(graphics);
                    extended.DrawCornerRectangle(pen, boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height, ratio);
                }
                break;

                default:
                    throw new Exception(Resources.ExceptionUnhandledNodeShape);
            }
        }

        private float getTopAttachmentsHeight()
        {
            float height = 0;

            for (int i = 0; i < _subItems.Count; ++i)
            {
                if (_subItems[i].SelectableObject != null)
                {
                    Attachments.Attachment attachment = _subItems[i].SelectableObject as Attachments.Attachment;

                    if (attachment != null && attachment.IsPrecondition)
                    {
                        height += _subItems[i].Height;
                    }
                }
            }

            return height;
        }

        private int getTopAttachmentIndex(int subItemIndex)
        {
            Debug.Check(subItemIndex >= 0 && subItemIndex < _subItems.Count);

            int attachmentIndex = -1;

            if (_subItems[subItemIndex].SelectableObject != null)
            {
                Attachments.Attachment attachment = _subItems[subItemIndex].SelectableObject as Attachments.Attachment;

                if (attachment != null && attachment.IsPrecondition)
                {
                    attachmentIndex = 0;

                    for (int i = 0; i < subItemIndex; ++i)
                    {
                        if (_subItems[i].SelectableObject != null)
                        {
                            Attachments.Attachment otherAttachment = _subItems[i].SelectableObject as Attachments.Attachment;

                            if (otherAttachment != null && otherAttachment.IsPrecondition)
                            {
                                attachmentIndex++;
                            }
                        }
                    }
                }
            }

            return attachmentIndex;
        }

        private int getBottomAttachmentIndex(int subItemIndex)
        {
            Debug.Check(subItemIndex >= 0 && subItemIndex < _subItems.Count);

            int attachmentIndex = -1;

            if (_subItems[subItemIndex].SelectableObject != null)
            {
                Attachments.Attachment attachment = _subItems[subItemIndex].SelectableObject as Attachments.Attachment;

                if (attachment != null && !attachment.IsPrecondition)
                {
                    attachmentIndex = 0;

                    for (int i = 0; i < subItemIndex; ++i)
                    {
                        if (_subItems[i].SelectableObject != null)
                        {
                            Attachments.Attachment otherAttachment = _subItems[i].SelectableObject as Attachments.Attachment;

                            if (otherAttachment != null && !otherAttachment.IsPrecondition)
                            {
                                attachmentIndex++;
                            }
                        }
                    }
                }
            }

            return attachmentIndex;
        }

        /// <summary>
        /// Calculates the untransformed bounding box of a subitem.
        /// </summary>
        /// <param name="nodeBoundingBox">The untransformed bounding box of the node.</param>
        /// <param name="n">The index of the subitem.</param>
        /// <returns>Returns the untransformed bounding box of the subitem.</returns>
        protected RectangleF GetSubItemBoundingBox(RectangleF nodeBoundingBox, int n)
        {
            SubItem subitem = _subItems[n];
            float top = 0;
            float width = nodeBoundingBox.Width;

            if (subitem.ShowParallelToLabel)
            {
                // if our subitem is a parallel one, we center it around the middle of the node

                // first we collect some information about parallel shown subitems
                float totalParallelHeight = 0.0f;
                float previousParallelHeight = 0.0f;

                for (int i = 0; i < _subItems.Count; ++i)
                {
                    if (_subItems[i].ShowParallelToLabel)
                    {
                        if (i < n)
                        {
                            // store the height of all parallel subitems before the requested one
                            previousParallelHeight += _subItems[i].Height;
                        }

                        // store the height of all available subitems
                        totalParallelHeight += _subItems[i].Height;
                    }
                    else
                    {
                        // all parallel subitems must be next to each other
                        break;
                    }
                }

                // calculate the final top
                top = nodeBoundingBox.Top + previousParallelHeight;
                if (this.IsExpanded)
                {
                    top += (nodeBoundingBox.Height - totalParallelHeight) * 0.5f;
                }
            }
            else
            {
                int topAttachmentIndex = getTopAttachmentIndex(n);
                int bottomAttachmentIndex = getBottomAttachmentIndex(n);
                float attachmentHeight = _subItems[n].Height;

                if (topAttachmentIndex > -1)
                {
                    top = nodeBoundingBox.Top + attachmentHeight * topAttachmentIndex;
                }

                else if (bottomAttachmentIndex > -1)
                {
                    top = nodeBoundingBox.Top + _labelSize.Height + getTopAttachmentsHeight() + attachmentHeight * bottomAttachmentIndex;
                }

                else
                {
                    top = nodeBoundingBox.Top + _labelSize.Height;
                }

                if (_subItemParallelWidth > 0.0f)
                {
                    width -= _subItemParallelWidth + Padding;
                }
            }

            // return the bounding box of the requested subitem
            return new RectangleF(nodeBoundingBox.X, top, width, subitem.Height);
        }

        /// <summary>
        /// Calculates the untransformed bounding box of all connector subitems of a given connector.
        /// </summary>
        /// <param name="nodeBoundingBox">The untransformed bounding box of the node.</param>
        /// <param name="connector">The connector we want the counding box for.</param>
        /// <returns>Returns the untransformed bounding box of the connector's subitems.</returns>
        public RectangleF GetConnectorBoundingBox(RectangleF nodeBoundingBox, Connector connector)
        {
            for (int i = 0; i < _subItems.Count; ++i)
            {
                SubItemConnector subitemConn = _subItems[i] as SubItemConnector;

                if (subitemConn != null && subitemConn.Connector == connector && !connector.IsAsChild)
                {
                    return GetSubItemBoundingBox(nodeBoundingBox, i);
                }
            }

            // find the first and last parallel subitem
            int firstParallel = -1;
            int lastParallel = -1;

            for (int i = 0; i < _subItems.Count; ++i)
            {
                if (_subItems[i].ShowParallelToLabel)
                {
                    if (firstParallel < 0)
                    {
                        firstParallel = i;
                    }

                    lastParallel = i;

                }
                else
                {
                    // all parallel subitems must be next to each other
                    break;
                }
            }

            // ensure our retrieved information is correct
            Debug.Check(firstParallel >= 0 && lastParallel >= firstParallel);

            float top = -1.0f;
            float bottom = -1.0f;

            // search all subitems for the connector
            bool inConnector = false;

            for (int i = firstParallel; i <= lastParallel; ++i)
            {
                SubItemConnector subitemConn = _subItems[i] as SubItemConnector;

                // if we found a subitem for our connector and we have found none before, we have found the top of the bounding box
                if (!inConnector && subitemConn != null && subitemConn.Connector == connector)
                {
                    inConnector = true;

                    if (i == firstParallel)
                    {
                        top = nodeBoundingBox.Top;    // if this is the first parallel, simply extent it to the full height of the node
                    }

                    else
                    {
                        top = GetSubItemBoundingBox(nodeBoundingBox, i).Top;
                    }
                }

                // if we found no subitem for our connector and we have found one before, we have found the bottom of the bounding box
                if (inConnector && (subitemConn == null || subitemConn.Connector != connector))
                {
                    // the previous subitem was the last one for our connector
                    if (i - 1 == lastParallel)
                    {
                        bottom = nodeBoundingBox.Bottom;    // if this is the first parallel, simply extent it to the full height of the node
                    }

                    else
                    {
                        bottom = GetSubItemBoundingBox(nodeBoundingBox, i - 1).Bottom;
                    }

                    break;
                }

                // when we have reached the last parallel subitem, simply extent the bounding box to the height of the node we are the last parallel subitem
                if (i == lastParallel)
                {
                    bottom = nodeBoundingBox.Bottom;
                    break;
                }
            }

            // ensure our retrieved data is valid
            Debug.Check(bottom > top);

            // return the bounding box of all subitems belonging to the given connector
            return new RectangleF(nodeBoundingBox.X, top, nodeBoundingBox.Width, bottom - top);
        }

        private readonly static Pen __planFailedPen = new Pen(Brushes.Red, 4.0f);
        private readonly static Style _planFailedStyle = new Style(null, __planFailedPen, null);

        private readonly static Pen __planSucceededPen = new Pen(Brushes.Green, 4.0f);
        private readonly static Style _planSucceededStyle = new Style(null, __planSucceededPen, null);

        /// <summary>
        /// Draws the node to the graph.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="nvd">The view data of this node for drawing.</param>
        /// <param name="isCurrent">Determines if the node is currently hovered over.</param>
        /// <param name="isSelected">Determines if the node is selected.</param>
        /// <param name="isDragged">Determines if the node is currently being dragged.</param>
        /// <param name="graphMousePos">The mouse position in the untransformed graph.</param>
        public void Draw(Graphics graphics, NodeViewData nvd, PointF graphMousePos,
                         bool isCurrent, bool isSelected, bool isDragged,
                         bool isHighlighted,
                         bool isUpdated,
                         HighlightBreakPoint highlightBreakPoint,
                         Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> profileInfos)
        {
#if DEBUG
            //ensure consistency
            DebugCheckIntegrity();
#endif
            RectangleF boundingBox = nvd.BoundingBox;

            // assemble the correct style
            Style style = _defaultStyle;

            Behavior b = this.Node.Behavior as Behavior;

            if (b != null && b.PlanningProcess != null)
            {
                FrameStatePool.PlanningState nodeState = b.PlanningProcess.GetNode(this.FullId);

                if (nodeState != null)
                {
                    if (nodeState._bOk)
                    {
                        style += _planSucceededStyle;

                    }
                    else
                    {
                        style += _planFailedStyle;
                    }
                }
            }

            if (isDragged)
            {
                style += _draggedStyle;
            }

            else if (isSelected)
            {
                style += _selectedStyle;
            }

            else if (isCurrent)
            {
                style += _currentStyle;
            }

            else if (isHighlighted)
            {
                style += _highlightedStyle;
            }

            else if (isUpdated)
            {
                style += _updatedStyle;
            }

            if (style.Background != null)
            {
                DrawShapeBackground(graphics, boundingBox, style.Background);
            }

            // if the node is dragged, do not render the events
            if (!isDragged)
            {
                // if this node is not selected, deselect the event
                if (!isSelected && _selectedSubItem != null)
                {
                    _selectedSubItem.IsSelected = false;
                    _selectedSubItem = null;
                }

                if (_subItems.Count > 0)
                {
                    Region prevreg = graphics.Clip;

                    // draw non parallel subitems first
                    for (int i = 0; i < _subItems.Count; ++i)
                    {
                        if (!_subItems[i].ShowParallelToLabel)
                        {
                            // get the bounding box of the event
                            RectangleF newclip = GetSubItemBoundingBox(boundingBox, i);
                            graphics.Clip = new Region(newclip);

                            _subItems[i].Draw(graphics, nvd, newclip);
                        }
                    }

                    // draw parallel subitems second
                    for (int i = 0; i < _subItems.Count; ++i)
                    {
                        if (_subItems[i].ShowParallelToLabel)
                        {
                            // get the bounding box of the event
                            RectangleF newclip = GetSubItemBoundingBox(boundingBox, i);
                            graphics.Clip = new Region(newclip);

                            _subItems[i].Draw(graphics, nvd, newclip);
                        }
                    }

                    // restore rendering area
                    graphics.Clip = prevreg;
                }

                // draw the label of the node
                if (style.Label != null)
                {
                    // calculate the height of all non-parallel subitems so we can correctly center the label
                    float subItemsHeight = 0.0f;

                    foreach (SubItem sub in _subItems)
                    {
                        if (!sub.ShowParallelToLabel)
                        {
                            subItemsHeight += sub.Height;
                        }
                    }

                    float x = boundingBox.Left + (boundingBox.Width - _subItemParallelWidth) * 0.5f - _realLabelSize.Width * 0.5f;
                    float y = boundingBox.Top + boundingBox.Height * 0.5f - subItemsHeight * 0.5f - _realLabelSize.Height * 0.5f;
                    y += getTopAttachmentsHeight();
                    graphics.DrawString(this.DisplayLabel, _font, style.Label, x, y);

                    //graphics.DrawRectangle(Pens.Red, boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
                    //graphics.DrawRectangle(Pens.Red, x, y, _realLabelSize.Width, _realLabelSize.Height);
                    //graphics.DrawRectangle(Pens.Green, x, y, _labelSize.Width, _labelSize.Height);
                }
            }

            // draw the prefab border
            if (!string.IsNullOrEmpty(nvd.Node.PrefabName))
            {
                _prefabStyle.Border.DashStyle = nvd.Node.IsPrefabDataDirty() ? System.Drawing.Drawing2D.DashStyle.Dash : System.Drawing.Drawing2D.DashStyle.Solid;
                DrawShapeBorder(graphics, boundingBox, _prefabStyle.Border);
                _prefabStyle.Border.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            }

            // draw the nodes border
            if (style.Border != null)
            {
                if (isHighlighted && (isCurrent || isSelected))
                {
                    //highlight border
                    DrawShapeBorder(graphics, boundingBox, _highlightedStyle.Border);

                    //shrink it to draw the selected/current border
                    RectangleF rect = boundingBox;
                    rect.Inflate(-_highlightedStyle.Border.Width, -_highlightedStyle.Border.Width);

                    DrawShapeBorder(graphics, rect, style.Border);

                }
                else
                {
                    DrawShapeBorder(graphics, boundingBox, style.Border);
                }
            }

            // draw the profile info
            if (profileInfos != null && profileInfos.Count > 0)
            {
                string fullId = nvd.FullId;

                if (profileInfos.ContainsKey(fullId))
                {
                    FrameStatePool.NodeProfileInfos.ProfileInfo profileInfo = profileInfos[fullId];
                    string timeStr = string.Format("{0:F3}ms", Math.Abs(profileInfo.Time));
                    string avgTimeStr = (profileInfo.Count <= 0) ? "0" : string.Format("{0:F3}ms", profileInfo.TotalTime / profileInfo.Count);
                    string info = string.Format("{0}  {1}  {2}", timeStr, avgTimeStr, profileInfo.Count);
                    SizeF txtSize = MeasureDisplayStringWidth(graphics, info, _profileFont);
                    float x = boundingBox.Left;
                    float y = boundingBox.Top - txtSize.Height - 2;
                    graphics.DrawString(info, (profileInfo.Time >= 0) ? _profileBoldFont : _profileFont, Brushes.Yellow, x, y);
                }
            }

            // draw the attached condition
            foreach (Node.Connector connector in nvd.Connectors)
            {
                if (!connector.IsAsChild)
                {
                    PointF[] vertices = getConnectorTriangle(nvd, connector);

                    Brush brush = isCurrent && nvd.IsInExpandConnectorRange(graphMousePos) ? Brushes.Yellow : Brushes.Blue;

                    graphics.FillPolygon(Brushes.LightGray, vertices);

                    if (this.IsExpanded && connector.ChildCount > 0)
                    {
                        graphics.FillRectangle(brush, vertices[0].X + 0.5f, vertices[0].Y + 3.5f, 5.5f, 1.5f);

                        if (!connector.IsExpanded)
                        {
                            graphics.FillRectangle(brush, vertices[0].X + 2.5f, vertices[0].Y + 1.5f, 1.5f, 5.5f);
                        }
                    }
                }
            }

            // draw the breakpoints
            bool isBreakpointHighlighted = false;
            const float width = 18.0f;
            BreakPointStates enterState = getBreakPointState(highlightBreakPoint, HighlightBreakPoint.kEnter);

            if (HighlightBreakPoint.ShowBreakPoint && (enterState == BreakPointStates.Normal || enterState == BreakPointStates.Disable) ||
                enterState == BreakPointStates.Highlight)
            {
                isBreakpointHighlighted |= (enterState == BreakPointStates.Highlight);

                float x = boundingBox.X + width * 0.1f;
                float y = boundingBox.Y + (boundingBox.Height - width) * 0.5f;

                graphics.FillEllipse(getBrush(enterState), x, y, width, width);
            }

            BreakPointStates exitState = getBreakPointState(highlightBreakPoint, HighlightBreakPoint.kExit);

            if (HighlightBreakPoint.ShowBreakPoint && (exitState == BreakPointStates.Normal || exitState == BreakPointStates.Disable) ||
                exitState == BreakPointStates.Highlight)
            {
                isBreakpointHighlighted |= (exitState == BreakPointStates.Highlight);

                float x = boundingBox.X + (boundingBox.Width - width) - width * 0.1f;
                float y = boundingBox.Y + (boundingBox.Height - width) * 0.5f;

                graphics.FillEllipse(getBrush(exitState), x, y, width, width);
            }

            BreakPointStates planState = getBreakPointState(highlightBreakPoint, HighlightBreakPoint.kPlanning);

            if (HighlightBreakPoint.ShowBreakPoint && (planState == BreakPointStates.Normal || planState == BreakPointStates.Disable) ||
                planState == BreakPointStates.Highlight)
            {
                isBreakpointHighlighted |= (planState == BreakPointStates.Highlight);

                float x = boundingBox.X + (boundingBox.Width * 0.5f) - width * 0.5f;
                float y = boundingBox.Y + (boundingBox.Height - width) * 0.5f;

                graphics.FillEllipse(getBrush(planState), x, y, width, width);
            }

            // draw the expand or collapse symbol
            if (nvd.CanBeExpanded())
            {
                Brush brush = isCurrent && nvd.IsInExpandRange(graphMousePos) ? Brushes.Yellow : Brushes.LightGray;

                graphics.FillRectangle(brush, boundingBox.X + 5.0f, boundingBox.Y + 5.0f, 12.0f, 2.0f);

                if (!nvd.IsExpanded)
                {
                    graphics.FillRectangle(brush, boundingBox.X + 10.0f, boundingBox.Y, 2.0f, 12.0f);
                }
            }

            // draw node id
            if (ShowNodeId)
            {
                graphics.DrawString(nvd.FullId, _profileFont, isBreakpointHighlighted ? Brushes.Yellow : Brushes.White, boundingBox.X, boundingBox.Y + boundingBox.Height);
            }

            //graphics.DrawRectangle(Pens.Red, nvd.LayoutRectangle.X, nvd.LayoutRectangle.Y, nvd.LayoutRectangle.Width, nvd.LayoutRectangle.Height);
        }

        private static void traverse(NodeViewData n, ref int count)
        {
            count += 1;

            foreach (NodeViewData child in n.GetChildNodes())
            {
                traverse(child, ref count);
            }
        }

        public void DrawCount(Graphics graphics)
        {
            int nc = 0;

            traverse(this, ref nc);

            // draw node count
            string nodeCount = string.Format("Node Count : {0}", nc);
            graphics.DrawString(nodeCount, _font, Brushes.WhiteSmoke, 10, 60);
        }

        private void DrawFSMCurve(SubItemAttachment attachment, Graphics graphics, Pen edgePen, RectangleF startBox, RectangleF endBox, bool isPointedBySelf)
        {
            float penHalfWidth = edgePen.Width * 0.5f;
            float centerY = startBox.Top + startBox.Height * 0.5f;
            PointF startPos = new PointF(startBox.Right, centerY - penHalfWidth);
            PointF endPos = new PointF(endBox.Left, endBox.Top + 5.0f);

            if (endBox.Right < startBox.Left)
            {
                startPos.X = startBox.Left;
                endPos.X = endBox.Right;

            }
            else if (endBox.Left < startBox.Right)
            {
                startPos.X = startBox.Left;
                endPos.X = endBox.Left;
            }

            float middleX = startPos.X + (endPos.X - startPos.X) * 0.5f;
            float controlStartX = middleX;
            float controlEndX = middleX;

            if (endBox.Right < startBox.Left)
            {
            }
            else if (endBox.Left < startBox.Right)
            {
                if (endBox.Left < startBox.Left)
                {
                    controlEndX = endBox.Left - (middleX - endBox.Left);
                }

                else
                {
                    controlStartX = startBox.Left - (middleX - startBox.Left);
                }
            }

            if (isPointedBySelf)
            {
                controlStartX -= startBox.Width * 0.5f;
            }

            edgePen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            edgePen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(edgePen.Width, edgePen.Width);

            if (attachment.DrawnPath == null)
            {
                attachment.DrawnPath = new System.Drawing.Drawing2D.GraphicsPath();

            }
            else
            {
                attachment.DrawnPath.Reset();
            }

            attachment.DrawnPath.AddBezier(startPos.X, startPos.Y,
                                           controlStartX, startPos.Y,
                                           controlEndX, endPos.Y,
                                           endPos.X, endPos.Y);

            graphics.DrawPath(edgePen, attachment.DrawnPath);

            //bool isIn = attachment.DrawnPath.IsOutlineVisible(startPos.X + 0.01f, startPos.Y + 0.01f, edgePen);
            //Debug.Check(isIn);
        }

        /// <summary>
        /// Draws the edges connecting the nodes.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="nvd">The view data of this node in the current view.</param>
        /// <param name="edgePen">The pen used for normal connectors.</param>
        /// <param name="edgePenHighlight">The pen used for normal connectors when highlighting.</param>
        /// <param name="edgePenReadOnly">The pen used for read-only connectors.</param>
        public virtual void DrawEdges(Graphics graphics, NodeViewData nvd, List<string> highlightedNodeIds, List<string> updatedNodeIds, List<string> highlightedTransitionIds, Pen edgePen, Pen edgePenSelected, Pen edgePenHighlighted, Pen edgePenUpdate, Pen edgePenReadOnly)
        {
            RectangleF boundingBox = nvd.BoundingBox;

            // calculate an offset so we cannot see the end or beginning of the rendered edge
            float edgePenHalfWidth = edgePen.Width * 0.5f;

            if (this.IsFSM)
            {
                for (int i = 0; i < _subItems.Count; ++i)
                {
                    if (_subItems[i].IsFSM && _subItems[i] is SubItemAttachment)
                    {
                        SubItemAttachment subItemAttachment = _subItems[i] as SubItemAttachment;

                        if (subItemAttachment.Attachment != null)
                        {
                            NodeViewData targetNode = this.RootNodeView.FindNodeViewData(subItemAttachment.Attachment.TargetFSMNodeId.ToString());

                            if (targetNode != null)
                            {
                                Pen pen = edgePen;
                                bool isHighlighted = (highlightedTransitionIds != null) && highlightedTransitionIds.Contains(subItemAttachment.Attachment.Id.ToString());

                                if (isHighlighted)
                                {
                                    pen = edgePenUpdate;
                                }
                                else if (subItemAttachment.IsSelected)
                                {
                                    pen = edgePenSelected;
                                }

                                if (subItemAttachment.Attachment.Enable)
                                {
                                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                                }
                                else
                                {
                                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                                }

                                RectangleF subitemBoundingBox = this.GetSubItemBoundingBox(this.BoundingBox, i);

                                this.DrawFSMCurve(subItemAttachment, graphics, pen, subitemBoundingBox, targetNode.BoundingBox, targetNode == this);
                            }
                        }
                    }
                }

                return;
            }

            foreach (NodeViewData node in nvd.Children)
            {
                RectangleF nodeBoundingBox = node.BoundingBox;

                // calculate the centre between both nodes and of the edge
                float middle = boundingBox.Right + (nodeBoundingBox.Left - boundingBox.Right) * 0.5f;

                // end at the middle of the other node
                float nodeHeight = nodeBoundingBox.Top + nodeBoundingBox.Height * 0.5f;

                // find the correct connector for this node
                for (int i = 0; i < _subItems.Count; ++i)
                {
                    SubItemConnector conn = _subItems[i] as SubItemConnector;

                    if (conn != null && conn.Child == node && conn.Connector.IsExpanded)
                    {
                        // get the bounding box of the event
                        RectangleF subitemBoundingBox = GetSubItemBoundingBox(boundingBox, i);

                        // start at the middle of the connector
                        float connectorHeight = subitemBoundingBox.Top + subitemBoundingBox.Height * 0.5f;

                        if (conn.Connector.IsGeneric)
                        {
                            connectorHeight = boundingBox.Top + (boundingBox.Bottom - boundingBox.Top) * 0.5f;
                        }

                        Pen pen = edgePenReadOnly;

                        if (!conn.Connector.IsReadOnly)
                        {
                            if (highlightedNodeIds != null && highlightedNodeIds.Contains(node.FullId))
                            {
                                pen = edgePenSelected;
                            }

                            else if (updatedNodeIds != null && updatedNodeIds.Contains(node.FullId))
                            {
                                pen = edgePenUpdate;
                            }

                            else
                            {
                                pen = edgePen;
                            }
                        }

                        pen.DashStyle = node.Node.Enable ? System.Drawing.Drawing2D.DashStyle.Solid : System.Drawing.Drawing2D.DashStyle.Dot;

                        if (!conn.Connector.IsAsChild)
                        {
                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        }

                        graphics.DrawBezier(pen,
                                            boundingBox.Right - edgePenHalfWidth, connectorHeight,
                                            middle, connectorHeight,
                                            middle, nodeHeight,
                                            nodeBoundingBox.Left + edgePenHalfWidth, nodeHeight);

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the background of the comment.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="nvd">The view data of this node in the current view.</param>
        /// <param name="renderDepth">The depth which is still rendered.</param>
        /// <param name="padding">The padding between the nodes.</param>
        public void DrawCommentBackground(Graphics graphics, NodeViewData nvd, int renderDepth, SizeF padding)
        {
            if (_node.CommentObject != null)
            {
                _node.CommentObject.DrawBackground(graphics, nvd, renderDepth, padding);
            }
        }

        /// <summary>
        /// Draws the text of the comment.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="nvd">The view data of this node in the current view.</param>
        public void DrawCommentText(Graphics graphics, NodeViewData nvd)
        {
            if (_node.CommentObject != null)
            {
                _node.CommentObject.DrawText(graphics, nvd);
            }
        }

        public string GetBreakpointOperation(string actionName)
        {
            Debug.Check(actionName == HighlightBreakPoint.kEnter || actionName == HighlightBreakPoint.kExit || actionName == HighlightBreakPoint.kPlanning);

            Behavior behavior = this.RootBehavior as Behavior;
            string behaviorFilename = behavior.MakeRelative(behavior.Filename);
            string fullId = FullId;
            DebugDataPool.BreakPoint breakPoint = DebugDataPool.FindBreakPoint(behaviorFilename, fullId, actionName);

            string addBreakpoint = Resources.AddEnterBreakpoint;
            string removeBreakpoint = Resources.RemoveEnterBreakpoint;
            string disableBreakpoint = Resources.DisableEnterBreakpoint;

            if (actionName == HighlightBreakPoint.kExit)
            {
                addBreakpoint = Resources.AddExitBreakpoint;
                removeBreakpoint = Resources.RemoveExitBreakpoint;
                disableBreakpoint = Resources.DisableExitBreakpoint;

            }
            else if (actionName == HighlightBreakPoint.kPlanning)
            {
                addBreakpoint = Resources.AddPlanningBreakpoint;
                removeBreakpoint = Resources.RemovePlanningBreakpoint;
                disableBreakpoint = Resources.DisablePlanningBreakpoint;
            }

            if (breakPoint != null)
            {
                DebugDataPool.Action action = breakPoint.FindAction(actionName);
                Debug.Check(action != null);

                if (action != null)
                {
                    return action.Enable ? disableBreakpoint : removeBreakpoint;
                }

                return removeBreakpoint;
            }

            return addBreakpoint;
        }

        public void SetBreakpoint(string actionName)
        {
            Behavior behavior = this.RootBehavior as Behavior;
            string behaviorFilename = behavior.MakeRelative(behavior.Filename);
            string fullId = FullId;
            DebugDataPool.BreakPoint breakPoint = DebugDataPool.FindBreakPoint(behaviorFilename, fullId, actionName);

            if (breakPoint != null)
            {
                DebugDataPool.Action action = breakPoint.FindAction(actionName);
                Debug.Check(action != null);

                if (action != null)
                {
                    if (action.Enable)
                    {
                        DebugDataPool.AddBreakPoint(behaviorFilename, fullId, Node.ExportClass, action.Name, false, action.Result, action.HitCount);
                    }
                    else
                    {
                        DebugDataPool.RemoveBreakPoint(behaviorFilename, fullId, action);
                    }
                }
            }
            else
            {
                DebugDataPool.AddBreakPoint(behaviorFilename, fullId, Node.ExportClass, actionName, true, DebugDataPool.Action.kResultAll, 0);
            }
        }

        public enum RangeType
        {
            kNode,
            kExpand,
            kEnterBreakpoint,
            kExitBreakpoint,
            kConnector,
            kFSMLeftArrow,
            kFSMRightArrow
        }

        private RectangleF getConnectorRange(NodeViewData nvd, Node.Connector connector)
        {
            const float offset = 12.0f;
            RectangleF bboxConnector = nvd.GetConnectorBoundingBox(nvd.BoundingBox, connector);

            return new RectangleF(bboxConnector.Right - offset, bboxConnector.Y, offset, offset);
        }

        private PointF[] getConnectorTriangle(NodeViewData nvd, Node.Connector connector)
        {
            const float innerOffset = 2.0f;

            PointF[] vertices = new PointF[3];
            RectangleF range = getConnectorRange(nvd, connector);

            vertices[0] = new PointF(range.Left + innerOffset, range.Top + innerOffset);
            vertices[1] = new PointF(range.Right - innerOffset, range.Top + range.Height * 0.5f);
            vertices[2] = new PointF(range.Left + innerOffset, range.Bottom - innerOffset + 1.0f);

            return vertices;
        }

        private RangeType CheckMouseInNode(PointF graphMousePos, out Node.Connector selectedConnector)
        {
            selectedConnector = null;

            // check the range of the expand
            if (IsInExpandRange(graphMousePos))
            {
                return RangeType.kExpand;
            }

            // check the range of the attached condition
            foreach (Node.Connector connector in this.Connectors)
            {
                if (!connector.IsAsChild)
                {
                    RectangleF conditionRange = getConnectorRange(this, connector);

                    if (conditionRange.Contains(graphMousePos))
                    {
                        selectedConnector = connector;
                        return RangeType.kConnector;
                    }
                }
            }

            // check the range of the FSM arrow
            SubItem subItem;
            RectangleF bbox;
            RangeType rangeType = this.CheckFSMArrowRange(graphMousePos, out subItem, out bbox);

            if (rangeType != RangeType.kNode)
            {
                return rangeType;
            }

            // check the range of the breakpoint
            const float breakpointWidth = 18.0f;
            const float breakpointHeight = 10.0f;

            bool isYInBreakpointRange = graphMousePos.Y > BoundingBox.Y + BoundingBox.Height * 0.5f - breakpointHeight &&
                                        graphMousePos.Y < BoundingBox.Y + BoundingBox.Height * 0.5f + breakpointHeight;

            if (isYInBreakpointRange)
            {
                if (graphMousePos.X < BoundingBox.X + breakpointWidth)
                {
                    return RangeType.kEnterBreakpoint;
                }

                if (graphMousePos.X > BoundingBox.X + BoundingBox.Width - breakpointWidth)
                {
                    return RangeType.kExitBreakpoint;
                }
            }

            return RangeType.kNode;
        }

        public bool IsInExpandRange(PointF graphMousePos)
        {
            const float expandWidth = 25.0f;
            const float expandHeight = 15.0f;

            return graphMousePos.X < BoundingBox.X + expandWidth &&
                   graphMousePos.Y < BoundingBox.Y + expandHeight;
        }

        public bool IsInExpandConnectorRange(PointF graphMousePos)
        {
            Node.Connector selectedConnector;
            return RangeType.kConnector == CheckMouseInNode(graphMousePos, out selectedConnector);
        }

        public RangeType CheckFSMArrowRange(PointF graphMousePos, out SubItem subItem, out RectangleF bbox)
        {
            bbox = this.GetSubItemBoundingBox(graphMousePos);
            subItem = this.GetSubItem(this, graphMousePos);

            if (subItem != null && subItem.IsFSM && subItem.CanBeDraggedToTarget)
            {
                const float offset = 8.0f;

                if (graphMousePos.X <= bbox.Left + offset)
                {
                    return RangeType.kFSMLeftArrow;
                }

                else if (graphMousePos.X >= bbox.Right - offset)
                {
                    return RangeType.kFSMRightArrow;
                }
            }

            return RangeType.kNode;
        }

        public bool OnClick(bool ctrlIsDown, PointF graphMousePos, out bool layoutChanged)
        {
            layoutChanged = false;

            Node.Connector selectedConnector;
            RangeType range = CheckMouseInNode(graphMousePos, out selectedConnector);

            // Toggle expand
            if (range == RangeType.kExpand)
            {
                if (this.CanBeExpanded())
                {
                    if (ctrlIsDown)
                    {
                        this.ExpandAll(!this.IsExpanded);
                    }

                    else
                    {
                        this.IsExpanded = !this.IsExpanded;
                    }

                    layoutChanged = true;

                    return true;
                }
            }

            // Toggle expand connector
            else if (range == RangeType.kConnector)
            {
                if (selectedConnector != null)
                {
                    selectedConnector.IsExpanded = !selectedConnector.IsExpanded;
                    layoutChanged = true;

                    return true;
                }
            }

            // Set breakpoint
            else if (range == RangeType.kEnterBreakpoint || range == RangeType.kExitBreakpoint)
            {
                return true;
            }

            return false;
        }

        protected bool KeyCtrlIsDown
        {
            get
            {
                return (Control.ModifierKeys & Keys.Control) != Keys.None;
            }
        }

        /// <summary>
        /// Is called when the node was double-clicked. Used for referenced behaviours.
        /// </summary>
        /// <param name="layoutChanged">Does the layout need to be recalculated?</param>
        /// <returns>Returns if the node handled the double click or not.</returns>
        public bool OnDoubleClick(PointF graphMousePos, out bool layoutChanged)
        {
            layoutChanged = false;

            Node.Connector selectedConnector;
            RangeType range = CheckMouseInNode(graphMousePos, out selectedConnector);

            // Set breakpoint
            if (range == RangeType.kEnterBreakpoint || range == RangeType.kExitBreakpoint)
            {
                if (this.Parent != null)
                {
                    SetBreakpoint(range == RangeType.kEnterBreakpoint ? HighlightBreakPoint.kEnter : HighlightBreakPoint.kExit);

                    layoutChanged = true;

                    return true;
                }
            }

            // Toggle expand
            if (range == RangeType.kNode)
            {
                if (this.CanBeExpanded())
                {
                    if (KeyCtrlIsDown)
                    {
                        this.ExpandAll(!this.IsExpanded);
                    }

                    else
                    {
                        this.IsExpanded = !this.IsExpanded;
                    }

                    layoutChanged = true;

                    return true;
                }
            }

            // Toggle expand connector
            if (range == RangeType.kConnector)
            {
                if (selectedConnector != null)
                {
                    selectedConnector.IsExpanded = !selectedConnector.IsExpanded;
                    layoutChanged = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Generates a new label by adding the attributes to the label as arguments
        /// </summary>
        protected void GenerateNewLabel()
        {
            string newlabel = _node.GenerateNewLabel();

            _label = (string.IsNullOrEmpty(newlabel)) ? this.BaseLabel : newlabel;
            _labelChanged = true;
        }

        /// <summary>
        /// Set the source sub item before the target one.
        /// </summary>
        /// <param name="targetNvd">The target node view data in the current view.</param>
        /// <param name="sourceItem">The source sub item of the current node view data.</param>
        /// <param name="targetItem">The target sub item of the target node view data.</param>
        public bool SetSubItem(NodeViewData targetNvd, SubItemAttachment sourceItem, SubItemAttachment targetItem, bool insertPreviously, bool isCopied)
        {
            if (targetNvd == null ||
                sourceItem == null || sourceItem.Attachment == null ||
                !isCopied && targetItem == sourceItem ||
                !targetNvd.Node.AcceptsAttachment(sourceItem.Attachment))
            {
                return false;
            }

            if (!isCopied)
            {
                this.Node.RemoveAttachment(sourceItem.Attachment);
                this.RemoveSubItem(sourceItem);

            }
            else
            {
                sourceItem = sourceItem.Clone(targetNvd.Node) as SubItemAttachment;
                sourceItem.Attachment.ResetId();
            }

            if (targetItem == null)
            {
                targetNvd.Node.AddAttachment(sourceItem.Attachment);

            }
            else
            {
                Debug.Check(targetItem.Attachment != null);
                if (targetItem.Attachment != null)
                {
                    for (int i = 0; i < targetNvd.Node.Attachments.Count; ++i)
                    {
                        if (targetNvd.Node.Attachments[i] == targetItem.Attachment)
                        {
                            targetNvd.Node.AddAttachment(sourceItem.Attachment, insertPreviously ? i : i + 1);
                            break;
                        }
                    }
                }
            }

            if (targetItem == null)
            {
                targetNvd.AddSubItem(sourceItem);
            }
            else
            {
                for (int i = 0; i < targetNvd.SubItems.Count; ++i)
                {
                    if (targetNvd.SubItems[i] == targetItem)
                    {
                        targetNvd.AddSubItem(sourceItem, insertPreviously ? i : i + 1);
                        break;
                    }
                }
            }

            bool setDirty = false;

            if (!isCopied)
            {
                sourceItem.Attachment.OnPropertyValueChanged(!setDirty);
                setDirty = true;
            }

            for (int i = 0; i < targetNvd.SubItems.Count; ++i)
            {
                SubItemAttachment attach = targetNvd.SubItems[i] as SubItemAttachment;

                if (attach != null && attach != sourceItem)
                {
                    attach.Attachment.OnPropertyValueChanged(!setDirty);
                    setDirty = true;
                }
            }

            targetNvd.SelectedSubItem = sourceItem;

            return true;
        }

        public int GetSubItemIndex(SubItem item)
        {
            if (item != null)
            {
                for (int i = 0; i < _subItems.Count; ++i)
                {
                    if (_subItems[i] == item)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public RectangleF GetSubItemBoundingBox(PointF graphMousePos)
        {
            for (int i = 0; i < _subItems.Count; ++i)
            {
                if (_subItems[i].SelectableObject != null)
                {
                    RectangleF bbox = GetSubItemBoundingBox(this.BoundingBox, i);

                    if (bbox.Contains(graphMousePos))
                    {
                        return bbox;
                    }
                }
            }

            return RectangleF.Empty;
        }

        public SubItemRegin GetSubItemRegin(PointF graphMousePos)
        {
            for (int i = 0; i < _subItems.Count; ++i)
            {
                if (_subItems[i].SelectableObject != null)
                {
                    RectangleF bbox = GetSubItemBoundingBox(this.BoundingBox, i);

                    if (bbox.Contains(graphMousePos))
                    {
                        if (graphMousePos.Y < bbox.Y + bbox.Height * 0.5f)
                        {
                            return SubItemRegin.Top;
                        }

                        else
                        {
                            return SubItemRegin.Bottom;
                        }
                    }
                }
            }

            return SubItemRegin.Out;
        }

        /// <summary>
        /// Get the sub item according to the position of the node view data.
        /// </summary>
        /// <param name="nvd">The view data of the node in the current view.</param>
        /// <param name="graphMousePos">The mouse position in the untransformed graph.</param>
        public SubItem GetSubItem(NodeViewData nvd, PointF graphMousePos)
        {
            for (int i = 0; i < _subItems.Count; ++i)
            {
                if (_subItems[i].SelectableObject != null)
                {
                    RectangleF bbox = GetSubItemBoundingBox(nvd.BoundingBox, i);

                    if (bbox.Contains(graphMousePos))
                    {
                        return _subItems[i];
                    }
                }
            }

            return null;
        }

        public SubItem GetSubItem(Attachments.Attachment attach)
        {
            if (attach != null)
            {
                for (int i = 0; i < _subItems.Count; ++i)
                {
                    if (_subItems[i].SelectableObject == attach)
                    {
                        return _subItems[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Is called when a possible selection of an event occured.
        /// </summary>
        /// <param name="nvd">The view data of the node in the current view.</param>
        /// <param name="graphMousePos">The mouse position in the untransformed graph.</param>
        public void ClickEvent(NodeViewData nvd, PointF graphMousePos)
        {
            SelectedSubItem = GetSubItem(nvd, graphMousePos);
        }

        protected RectangleF _boundingBox;

        /// <summary>
        /// The untransformed bounding box of the node.
        /// </summary>
        public RectangleF BoundingBox
        {
            get
            {
                return _boundingBox;
            }
        }

        protected RectangleF _displayBoundingBox;

        /// <summary>
        /// The transformed bounding box of the node.
        /// </summary>
        public RectangleF DisplayBoundingBox
        {
            get
            {
                return _displayBoundingBox;
            }
        }

        protected RectangleF _layoutRectangle;

        /// <summary>
        /// The layout rectangle of the node.
        /// </summary>
        public RectangleF LayoutRectangle
        {
            get
            {
                return _layoutRectangle;
            }
        }

        /// <summary>
        /// The upper left corner of the layout rectangle. For internal use only.
        /// </summary>
        protected PointF Location
        {
            get
            {
                return _layoutRectangle.Location;
            }
            set
            {
                _layoutRectangle.Location = value;
            }
        }

        public override PointF ScreenLocation
        {
            get
            {
                return (this.Node != null && this.Node.IsFSM) ? this.Node.ScreenLocation : this.Location;
            }
            set
            {
                this.Node.ScreenLocation = value;
            }
        }

        /// <summary>
        /// Returns the node a given location is in.
        /// </summary>
        /// <param name="location">The location you want to check.</param>
        /// <returns>Returns null if the position is not inside any node.</returns>
        public NodeViewData GetInsideNode(PointF location)
        {
            if (_displayBoundingBox.Contains(location))
            {
                return this;
            }

            if (this.IsExpanded || this.IsFSM)
            {
                foreach (NodeViewData node in this.GetChildNodes())
                {
                    NodeViewData insidenode = node.GetInsideNode(location);

                    if (insidenode != null)
                    {
                        return insidenode;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Copies the ode's size as the size of the bounding box.
        /// </summary>
        public virtual void UpdateExtent()
        {
            foreach (NodeViewData node in this.GetChildNodes())
            {
                node.UpdateExtent();
            }

            _boundingBox.Size = _finalSize;
        }

        /// <summary>
        /// Adds an offset to the height and Y position of the  layout rectangle.
        /// Used when the parent is higher than the children.
        /// </summary>
        /// <param name="offset">The off set which will be added.</param>
        private void OffsetLayoutSize(float offset)
        {
            float yoffset = 0.0f;

            foreach (NodeViewData node in this.GetChildNodes())
            {
                node._layoutRectangle.Height += offset;
                node._layoutRectangle.Y += yoffset;

                yoffset += offset;

                node.OffsetLayoutSize(offset);
            }
        }

        private const float CommentTextHeight = 20.0f;

        /// <summary>
        /// Calculates the layout rectangles for this node and its children.
        /// </summary>
        /// <param name="padding">The padding which is used between the nodes.</param>
        public void CalculateLayoutSize(SizeF padding)
        {
            // update size for children
            if (this.IsExpanded || this.IsFSM)
            {
                foreach (NodeViewData node in this.GetChildNodes())
                {
                    node.CalculateLayoutSize(padding);
                }
            }

            // calculate my layout size
            _layoutRectangle.Height = _boundingBox.Height;
            _layoutRectangle.Width = _boundingBox.Width;

            if (!string.IsNullOrEmpty(this.Node.CommentText))
            {
                _layoutRectangle.Height += CommentTextHeight;
            }

            if (this.IsExpanded && !this.IsFSM)
            {
                // calculate the size my children have
                float childHeight = 0.0f;

                foreach (NodeViewData node in _children)
                {
                    childHeight += node.LayoutRectangle.Height;
                }

                // if we have multiple children, add the padding we keep between them.
                if (_children.ChildCount > 1)
                {
                    childHeight += (_children.ChildCount - 1) * padding.Height;
                }

                if (_layoutRectangle.Height > childHeight)
                {
                    // if this node is higher than its children we have to update them
                    if (_children.ChildCount > 0)
                    {
                        float heightDiff = _layoutRectangle.Height - childHeight;
                        float offset = heightDiff / _children.ChildCount;

                        OffsetLayoutSize(offset);
                    }

                }
                else
                {
                    _layoutRectangle.Height = childHeight;
                }
            }
        }

        /// <summary>
        /// Aligns the different layout rectangles in the graph.
        /// </summary>
        /// <param name="padding">The padding you want to keep between the layout rectangles.</param>
        public void Layout(SizeF padding)
        {
            // the upper left position of the children
            PointF pos = new PointF(_layoutRectangle.Right + padding.Width, _layoutRectangle.Y);

            // align children
            foreach (NodeViewData node in this.GetChildNodes())
            {
                // set the node to the correct position
                node.Location = pos;

                // adjust the location for the next child to come
                pos.Y += node.LayoutRectangle.Height + padding.Height;

                // layout the children of this node.
                node.Layout(padding);
            }
        }

        /// <summary>
        /// Centers this and its children node in front of its/their children.
        /// </summary>
        public void UpdateLocation()
        {
            // move the node to the left centre of its layout
            if (this.IsFSM)
            {
                _boundingBox.X = this.ScreenLocation.X;
                _boundingBox.Y = this.ScreenLocation.Y;

                _layoutRectangle.X = _boundingBox.X;
                _layoutRectangle.Y = _boundingBox.Y;

                if (!string.IsNullOrEmpty(this.Node.CommentText))
                {
                    _layoutRectangle.Y -= CommentTextHeight * 0.5f;
                }

            }
            else
            {
                _boundingBox.X = _layoutRectangle.X;
                _boundingBox.Y = _layoutRectangle.Y + _layoutRectangle.Height * 0.5f - _boundingBox.Height * 0.5f;
            }

            if (!string.IsNullOrEmpty(this.Node.CommentText))
            {
                _boundingBox.Y += CommentTextHeight * 0.5f;
            }

            // update the location for the children as well
            foreach (NodeViewData node in this.GetChildNodes())
            {
                node.UpdateLocation();
            }
        }

        /// <summary>
        /// Calculates the display bounding box for this node.
        /// </summary>
        /// <param name="offsetX">The X offset of the graph.</param>
        /// <param name="offsetY">The Y offset of the graph.</param>
        /// <param name="scale">The scale of the graph.</param>
        public void UpdateDisplay(float offsetX, float offsetY, float scale)
        {
            // transform the bounding box.
            _displayBoundingBox.X = _boundingBox.X * scale + offsetX;
            _displayBoundingBox.Y = _boundingBox.Y * scale + offsetY;
            _displayBoundingBox.Width = _boundingBox.Width * scale;
            _displayBoundingBox.Height = _boundingBox.Height * scale;

            // transform the children's bounding boxes.
            foreach (NodeViewData node in this.GetChildNodes())
            {
                node.UpdateDisplay(offsetX, offsetY, scale);
            }
        }

        /// <summary>
        /// Returns the width of this node and all its child nodes. Internal use only.
        /// </summary>
        /// <param name="paddingWidth">The width kept between the nodes.</param>
        /// <param name="depth">Defines how deep the search will be.</param>
        /// <returns>Returns untransformed width.</returns>
        private float GetTotalWidth(float paddingWidth, int depth)
        {
            if (!IsExpanded || depth < 1)
            {
                return _layoutRectangle.Width;
            }

            float width = _layoutRectangle.Width;

            // if we have children we must keep our distance
            if (_children.ChildCount > 0)
            {
                width += paddingWidth;
            }

            // find the child with the highest width
            float childwidth = 0.0f;

            foreach (NodeViewData node in _children)
            {
                childwidth = Math.Max(childwidth, node.GetTotalWidth(paddingWidth, depth - 1));
            }

            // add both
            return width + childwidth;
        }

        public RectangleF GetTotalBoundingBox(float paddingWidth = 30)
        {
            float left = this.BoundingBox.Left - paddingWidth;
            float right = this.BoundingBox.Right + paddingWidth;
            float top = this.BoundingBox.Top - paddingWidth;
            float bottom = this.BoundingBox.Bottom + paddingWidth;

            foreach (NodeViewData node in this.GetChildNodes())
            {
                RectangleF nodeBox = node.GetTotalBoundingBox(paddingWidth);

                if (nodeBox.Left < left)
                {
                    left = nodeBox.Left;
                }

                if (nodeBox.Right > right)
                {
                    right = nodeBox.Right;
                }

                if (nodeBox.Top < top)
                {
                    top = nodeBox.Top;
                }

                if (nodeBox.Bottom > bottom)
                {
                    bottom = nodeBox.Bottom;
                }
            }

            return new RectangleF(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Returns the total size of the node and its child nodes.
        /// </summary>
        /// <param name="paddingWidth">The width kept between the nodes.</param>
        /// <param name="depth">Defines how deep the search will be.</param>
        /// <returns>Returns the untransformed size.</returns>
        public SizeF GetTotalSize(float paddingWidth, int depth)
        {
            SizeF totalSize = new SizeF();

            if (this.IsFSM)
            {
                RectangleF totalBox = this.GetTotalBoundingBox(paddingWidth);

                totalSize.Width = totalBox.Width;
                totalSize.Height = totalBox.Height;

                if (!string.IsNullOrEmpty(this.Node.CommentText))
                {
                    totalSize.Height += CommentTextHeight;
                }
            }
            else
            {
                totalSize.Width = this.GetTotalWidth(paddingWidth, depth);
                totalSize.Height = _layoutRectangle.Height;
            }

            return totalSize;
        }

        public NodeViewData GetChild(Node node)
        {
            foreach (NodeViewData child in this.GetChildNodes())
            {
                if (child.Node == node)
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>
        /// Draws the edges connecting the nodes.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="edgePen">The pen we use for physical nodes.</param>
        /// <param name="edgePenHighlight">The pen we use for physical nodes when highlighting.</param>
        /// <param name="edgePenReadOnly">The pen we use for sub-referenced nodes.</param>
        /// <param name="renderDepth">The depth which is still rendered.</param>
        public void DrawEdges(Graphics graphics, List<string> highlightedNodeIds, List<string> updatedNodeIds, List<string> highlightedTransitionIds, Pen edgePen, Pen edgePenSelected, Pen edgePenHighlighted, Pen edgePenUpdate, Pen edgePenReadOnly, int renderDepth)
        {
            if (!IsExpanded && !this.IsFSM)
            {
                return;
            }

            DrawEdges(graphics, this, highlightedNodeIds, updatedNodeIds, highlightedTransitionIds, edgePen, edgePenSelected, edgePenHighlighted, edgePenUpdate, edgePenReadOnly);

            // draw children
            if (renderDepth > 0)
            {
                foreach (NodeViewData child in this.GetChildNodes())
                {
                    child.DrawEdges(graphics, highlightedNodeIds, updatedNodeIds, highlightedTransitionIds, edgePen, edgePenSelected, edgePenHighlighted, edgePenUpdate, edgePenReadOnly, renderDepth - 1);
                }
            }
        }

        /// <summary>
        /// Draws the node to the graph.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="isDragged">Determines if the node is currently being dragged.</param>
        /// <param name="graphMousePos">The mouse position in the untransformed graph.</param>
        /// <param name="renderDepth">The depth which is still rendered.</param>
        /// <param name="currentNode">The current node under the mouse cursor.</param>
        /// <param name="selectedNode">The currently selected node.</param>
        public void Draw(Graphics graphics, bool isDragged, PointF graphMousePos, int renderDepth,
                         NodeViewData currentNode, NodeViewData selectedNode,
                         List<string> highlightedNodeIds,
                         List<string> updatedNodeIds,
                         HighlightBreakPoint highlightBreakPoint,
                         Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> profileInfos)
        {
            bool isHighlighted = (highlightedNodeIds != null) && highlightedNodeIds.Contains(FullId);
            bool isUpdated = (updatedNodeIds != null) && updatedNodeIds.Contains(FullId);

            Draw(graphics, this, graphMousePos,
                 (currentNode == null) ? false : (Node == currentNode.Node),
                 (selectedNode == null) ? false : (Node == selectedNode.Node),
                 isDragged,
                 isHighlighted,
                 isUpdated,
                 highlightBreakPoint,
                 profileInfos);

            // draw children
            if (IsExpanded && renderDepth > 0)
            {
                foreach (NodeViewData child in _children)
                {
                    if (child.ParentConnector != null &&
                        (child.ParentConnector.IsAsChild || child.ParentConnector.IsExpanded))
                    {
                        child.Draw(graphics, isDragged, graphMousePos, renderDepth - 1, currentNode, selectedNode, highlightedNodeIds, updatedNodeIds, highlightBreakPoint, profileInfos);
                    }
                }
            }

            // draw FSM nodes
            foreach (NodeViewData child in this.FSMNodes)
            {
                child.Draw(graphics, isDragged, graphMousePos, renderDepth - 1, currentNode, selectedNode, highlightedNodeIds, updatedNodeIds, highlightBreakPoint, profileInfos);
            }
        }

        /// <summary>
        /// Draws the background of the node's comment.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="renderDepth">The depth which is still rendered.</param>
        /// <param name="padding">The padding between the nodes.</param>
        public void DrawCommentBackground(Graphics graphics, int renderDepth, SizeF padding)
        {
            // draw comment backgrounds
            DrawCommentBackground(graphics, this, renderDepth, padding);

            // draw children
            if (IsExpanded && renderDepth > 0)
            {
                foreach (NodeViewData child in this.GetChildNodes())
                {
                    child.DrawCommentBackground(graphics, renderDepth - 1, padding);
                }
            }
        }

        /// <summary>
        /// Draws the text of the node's comment.
        /// </summary>
        /// <param name="graphics">The graphics object we render to.</param>
        /// <param name="renderDepth">The depth which is still rendered.</param>
        public void DrawCommentText(Graphics graphics, int renderDepth)
        {
            // draw comment backgrounds
            DrawCommentText(graphics, this);

            // draw children
            if ((this.IsFSM || this.IsExpanded) && renderDepth > 0)
            {
                foreach (NodeViewData child in this.GetChildNodes())
                {
                    child.DrawCommentText(graphics, renderDepth - 1);
                }
            }
        }

        public void OnNodeModified(BehaviorNode root, Node node)
        {
            this._fullId = null;

            GenerateNewLabel();
        }

        protected void node_SubItemAdded(Node node, DesignerPropertyInfo property)
        {
            this.AddSubItem(new SubItemProperty(node, property.Property, property.Attribute));
        }

        /// <summary>
        /// Returns if any of the node's parents is a given behaviour.
        /// </summary>
        /// <param name="behavior">The behavior we want to check if it is an ancestor of this node.</param>
        /// <returns>Returns true if this node is a descendant of the given behavior.</returns>
        public virtual bool HasParentBehavior(BehaviorNode behavior)
        {
            if (behavior == null)
            {
                return false;
            }

            if (_node == behavior)
            {
                return true;
            }

            if (Parent == null)
            {
                return false;
            }

            return Parent.HasParentBehavior(behavior);
        }
    }
}
