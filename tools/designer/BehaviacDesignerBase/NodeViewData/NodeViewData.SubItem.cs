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
        /// Represents a subitem which allows you to render stuff on a node.
        /// </summary>
        public abstract class SubItem
        {
            protected bool _showParallelToLabel;

            /// <summary>
            /// If true the subitem is rndered parallel to the label.
            /// </summary>
            public bool ShowParallelToLabel
            {
                get
                {
                    return _showParallelToLabel;
                }
            }

            private bool _selected = false;

            /// <summary>
            /// Holds if the subitem is currently selected.
            /// </summary>
            public bool IsSelected
            {
                get
                {
                    return _selected;
                }
                set
                {
                    _selected = value;
                }
            }

            public virtual bool IsFSM
            {
                get
                {
                    return false;
                }
            }

            public virtual bool CanBeDraggedToTarget
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// The displayed height of the item in the untransformed graph.
            /// </summary>
            public virtual float Height
            {
                get
                {
                    return 14.0f;
                }
            }

            /// <summary>
            /// The required untransformed width of the subitem.
            /// </summary>
            public abstract float Width
            {
                get;
            }

            /// <summary>
            /// Returns the object which can be selected. Is null when the subitem cannot be selected.
            /// </summary>
            public virtual DefaultObject SelectableObject
            {
                get
                {
                    return null;
                }
            }

            /// <summary>
            /// Holds if the subitem can be deleted by the user.
            /// </summary>
            public virtual bool CanBeDeleted
            {
                get
                {
                    return false;
                }
            }

            public virtual string ToolTip
            {
                get
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// Called when the node gets updated.
            /// </summary>
            /// <param name="node">The node the subitem belongs to.</param>
            /// <param name="graphics">The graphics object used for the update, NOT for drawing!</param>
            public abstract void Update(NodeViewData node, Graphics graphics);

            /// <summary>
            /// Called when the node is drawn.
            /// </summary>
            /// <param name="graphics">The graphics object used to draw the subitem.</param>
            /// <param name="nvd">The node view data of the node the subitem belongs to.</param>
            /// <param name="boundingBox">The bounding box of the subitem. Drawing is clipped to this.</param>
            public abstract void Draw(Graphics graphics, NodeViewData nvd, RectangleF boundingBox);

            /// <summary>
            /// Draws the node's shape as the background.
            /// </summary>
            /// <param name="graphics">The graphics object used for drawing.</param>
            /// <param name="nvd">The node view data of the node the subitem belongs to.</param>
            /// <param name="brush">The brush to draw the node's shape.</param>
            protected void DrawBackground(Graphics graphics, NodeViewData nvd, Brush brush)
            {
                if (brush != null)
                {
                    nvd.DrawShapeBackground(graphics, nvd.BoundingBox, brush);
                    //nvd.DrawShapeBorder(graphics, nvd.GetSubItemBoundingBox(nvd.BoundingBox, nvd.GetSubItemIndex(this)), Pens.WhiteSmoke);
                }
            }

            /// <summary>
            /// Clones the subitem.
            /// </summary>
            /// <param name="newnode">The node the cloned subitem will belong to.</param>
            /// <returns>Returns a new instance of this subitem.</returns>
            public abstract SubItem Clone(Node newnode);

            /// <summary>
            /// Creates a new subitem instance.
            /// </summary>
            /// <param name="showParallelToLabel">Holds if the subitem will be drawn next to the label.</param>
            protected SubItem(bool showParallelToLabel)
            {
                _showParallelToLabel = showParallelToLabel;

                // a parallel drawn subitem cannot be selected or deleted.
                Debug.Check(!_showParallelToLabel || SelectableObject == null && !CanBeDeleted);
            }
        }
    }
}
