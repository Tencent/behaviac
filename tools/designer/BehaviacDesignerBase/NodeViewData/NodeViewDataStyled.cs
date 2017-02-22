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
using System.Drawing.Drawing2D;
using Behaviac.Design.Nodes;

namespace Behaviac.Design
{
    public class NodeViewDataStyled : NodeViewData
    {
        private readonly static Pen __defaultCurrentBorderPen = new Pen(Brushes.GreenYellow, 3.0f);
        protected static Pen DefaultCurrentBorderPen
        {
            get
            {
                __defaultCurrentBorderPen.DashStyle = DashStyle.Dash;
                return __defaultCurrentBorderPen;
            }
        }

        protected readonly static Pen __defaultSelectedBorderPen = new Pen(Brushes.GreenYellow, 5.0f);
        protected readonly static Pen __highlightedBorderPen = new Pen(Brushes.Gold, 4.0f);
        protected readonly static Pen __updatedBorderPen = new Pen(Brushes.Olive, 4.0f);
        protected readonly static Pen __prefabBorderPen = new Pen(Brushes.Linen, 2.0f);
        protected readonly static Font __defaultLabelFont = new Font("Calibri,Arial", 8.0f, FontStyle.Regular);
        protected readonly static Font __profileLabelFont = new Font("Calibri,Arial", 6.0f, FontStyle.Regular);
        protected readonly static Font __profileLabelBoldFont = new Font("Calibri,Arial", 6.0f, FontStyle.Bold);

        public NodeViewDataStyled(NodeViewData parent, BehaviorNode rootBehavior, Node node, Pen borderPen, Brush backgroundBrush, string label, string description, int minWidth = 120, int minHeight = 35) :
        base(parent, rootBehavior, node,
             NodeShape.RoundedRectangle,
             new Style(backgroundBrush, null, Brushes.White),
             new Style(null, DefaultCurrentBorderPen, null),
             new Style(null, __defaultSelectedBorderPen, null),
             new Style(GetDraggedBrush(backgroundBrush), null, null),
             new Style(null, __highlightedBorderPen, null),
             new Style(null, __updatedBorderPen, null),
             new Style(null, __prefabBorderPen, null),
             label, __defaultLabelFont, __profileLabelFont, __profileLabelBoldFont, minWidth, minHeight, description)
        {
        }

        public NodeViewDataStyled(NodeViewData parent, BehaviorNode rootBehavior, Node node, Pen borderPen, Brush backgroundBrush, Brush draggedBackgroundBrush, string label, string description, int minWidth = 120, int minHeight = 35) :
        base(parent, rootBehavior, node,
             NodeShape.RoundedRectangle,
             new Style(backgroundBrush, null, Brushes.White),
             new Style(null, DefaultCurrentBorderPen, null),
             new Style(null, __defaultSelectedBorderPen, null),
             new Style(draggedBackgroundBrush, null, null),
             new Style(null, __highlightedBorderPen, null),
             new Style(null, __updatedBorderPen, null),
             new Style(null, __prefabBorderPen, null),
             label, __defaultLabelFont, __profileLabelFont, __profileLabelBoldFont, minWidth, minHeight, description)
        {
        }
    }
}
