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
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;

namespace Behaviac.Design
{
    /// <summary>
    /// The layout manager manages the scaling and the offset of the currently drawn graph.
    /// </summary>
    internal class NodeLayoutManager
    {
        protected Pen _edgePen;

        /// <summary>
        /// The pen which is used to draw the edges between the nodes.
        /// </summary>
        internal Pen EdgePen
        {
            get
            {
                return _edgePen;
            }
        }

        protected Pen _edgePenSelected;

        /// <summary>
        /// The pen which is used to draw the edges between the nodes when being selected.
        /// </summary>
        internal Pen EdgePenSelected
        {
            get
            {
                return _edgePenSelected;
            }
        }

        protected Pen _edgePenHighLighted;

        /// <summary>
        /// The pen which is used to draw the edges between the nodes when being highlighted.
        /// </summary>
        internal Pen EdgePenHighlighted
        {
            get
            {
                return _edgePenHighLighted;
            }
        }

        protected Pen _edgePenUpdate;

        /// <summary>
        /// The pen which is used to draw the edges between the nodes when being updated.
        /// </summary>
        internal Pen EdgePenUpdate
        {
            get
            {
                return _edgePenUpdate;
            }
        }

        protected Pen _edgePenReadOnly;

        /// <summary>
        /// The pen which is used to draw the edges between sub-referenced nodes.
        /// </summary>
        internal Pen EdgePenReadOnly
        {
            get
            {
                return _edgePenReadOnly;
            }
        }

        protected NodeViewData _rootNodeLayout;

        /// <summary>
        /// The root node of the layouted nodes.
        /// </summary>
        internal NodeViewData RootNodeLayout
        {
            get
            {
                return _rootNodeLayout;
            }
        }

        protected float _scale = 1.0f;

        /// <summary>
        /// The scale of the graph shown.
        /// </summary>
        internal float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        protected PointF _offset = new PointF();

        /// <summary>
        /// The offset of the shown graph.
        /// </summary>
        internal PointF Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
            }
        }

        protected SizeF _padding = new SizeF(20.0f, 10.0f);

        /// <summary>
        /// The distance between the nodes.
        /// </summary>
        internal SizeF Padding
        {
            get
            {
                return _padding;
            }
            set
            {
                _padding = value;
            }
        }

        /// <summary>
        /// Defines if the node layout manager should draw labels or not.
        /// </summary>
        protected bool _skipLabels;

        protected int _renderDepth = int.MaxValue;

        /// <summary>
        /// Defines how deep the tree will be rendered.
        /// </summary>
        internal int RenderDepth
        {
            get
            {
                return _renderDepth;
            }
            set
            {
                _renderDepth = value;
            }
        }

        /// <summary>
        /// Creates a new NodeLayoutManager.
        /// </summary>
        /// <param name="edgePen">The pen which is used to draw the edges connecting the nodes.</param>
        /// <param name="edgePenSubReferenced">The pen which is used to draw the edges connecting sub-referenced nodes.</param>
        /// <param name="skipLabels">Defines if labels are drawn or not.</param>
        /// <param name="rootNode">The root of the nodes shown in the view.</param>
        internal NodeLayoutManager(NodeViewData rootNode, Pen edgePen, Pen edgePenSelected, Pen edgePenHighlighted, Pen edgePenUpdate, Pen edgePenSubReferenced, bool skipLabels)
        {
            _rootNodeLayout = rootNode;
            _edgePen = edgePen;
            _edgePenSelected = edgePenSelected;
            _edgePenHighLighted = edgePenHighlighted;
            _edgePenUpdate = edgePenUpdate;
            _edgePenReadOnly = edgePenSubReferenced;
            _skipLabels = skipLabels;
        }

        /// <summary>
        /// Marks the current layout as being modified so it is recalculated the next time the graph is drawn.
        /// </summary>
        internal void MarkLayoutChanged()
        {
            _layoutChanged = true;
        }

        /// <summary>
        /// Converts a position in the control (e.g. mouse position) into a position in the untransformed graph.
        /// </summary>
        /// <param name="pos">The transformed position you want to convert.</param>
        /// <returns>The untransformed position of the argument.</returns>
        internal PointF ViewToGraph(PointF pos)
        {
            return new PointF((pos.X - _offset.X) / _scale, (pos.Y - _offset.Y) / _scale);
        }

        /// <summary>
        /// Converts a position in the graph (e.g. bounding box location) into a position on the control
        /// </summary>
        /// <param name="pos">The untransformed position you want to convert.</param>
        /// <returns>The transformed position of the argument.</returns>
        internal PointF GraphToView(PointF pos)
        {
            return new PointF(pos.X * _scale + _offset.X, pos.Y * _scale + _offset.Y);
        }

        private bool _layoutChanged = true;

        /// <summary>
        /// Shows if the layout was modified or not.
        /// </summary>
        internal bool LayoutChanged
        {
            get
            {
                return _layoutChanged;
            }
        }

        /// <summary>
        /// The root node of the graph the layout is created for.
        /// </summary>
        internal BehaviorNode RootNode
        {
            get
            {
                return (BehaviorNode)_rootNodeLayout.Node;
            }
        }

        /// <summary>
        /// Recalculates the layout if necessary.
        /// </summary>
        /// <param name="graphics">The graphics object which is used to measure the strings.</param>
        internal void UpdateLayout(Graphics graphics, bool bForce = false)
        {
            if (_layoutChanged)
            {
                _layoutChanged = false;

                _rootNodeLayout.SetBackgroundBrush();

                // synchronize layout
                ProcessedBehaviors processedBehaviors = new ProcessedBehaviors();
                _rootNodeLayout.SynchronizeWithNode(processedBehaviors, bForce);

                // calculate the size of each node
                _rootNodeLayout.UpdateFinalSize(graphics, _rootNodeLayout.RootBehavior);
                _rootNodeLayout.UpdateExtent();

                // calculate the total size of the branches
                _rootNodeLayout.CalculateLayoutSize(_padding);

                // align the branches
                _rootNodeLayout.Layout(_padding);

                // align the parents at the centre of their children
                _rootNodeLayout.UpdateLocation();
            }
        }

        /// <summary>
        /// Draws the graph.
        /// </summary>
        /// <param name="graphics">The graphics object we want to draw to.</param>
        /// <param name="currentNode">The node the mouse is currently hovering over.</param>
        /// <param name="selectedNode">The node which is currently selected.</param>
        /// <param name="graphMousePos">The mouse's position in the graph.</param>
        internal void DrawGraph(Graphics graphics, PointF graphMousePos,
                                NodeViewData currentNode = null,
                                NodeViewData selectedNode = null,
                                List<string> highlightedNodeIds = null,
                                List<string> updatedNodeIds = null,
                                List<string> highligltedTransitionIds = null,
                                HighlightBreakPoint highlightBreakPoint = null,
                                Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> profileInfos = null)
        {
            // draw the node count of the root node
            if (_rootNodeLayout.RootBehavior == _rootNodeLayout.Node)
            {
                _rootNodeLayout.DrawCount(graphics);
            }

            // setup drawing
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.Transform = new System.Drawing.Drawing2D.Matrix(_scale, 0.0f, 0.0f, _scale, _offset.X, _offset.Y);

            // update display bounding boxes
            _rootNodeLayout.UpdateDisplay(_offset.X, _offset.Y, _scale);

            // draw comment backgrounds
            if (!_skipLabels)
            {
                _rootNodeLayout.DrawCommentBackground(graphics, _renderDepth, _padding);
            }

            // draw the edges
            if (_edgePen != null && _renderDepth > 0)
            {
                _rootNodeLayout.DrawEdges(graphics, highlightedNodeIds, updatedNodeIds, highligltedTransitionIds, _edgePen, _edgePenSelected, _edgePenHighLighted, _edgePenUpdate, _edgePenReadOnly, _renderDepth - 1);
            }

            // draw the nodes
            _rootNodeLayout.Draw(graphics, _skipLabels, graphMousePos, _renderDepth, currentNode, selectedNode, highlightedNodeIds, updatedNodeIds, highlightBreakPoint, profileInfos);

            // draw comment text
            if (!_skipLabels)
            {
                _rootNodeLayout.DrawCommentText(graphics, _renderDepth);
            }

            // draw last mouse pos
            //e.Graphics.DrawRectangle(Pens.Red, graphMousePos.X -1.0f, graphMousePos.Y -1.0f, 2.0f, 2.0f);
        }
    }
}
