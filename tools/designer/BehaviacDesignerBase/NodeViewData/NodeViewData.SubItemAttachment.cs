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
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public partial class NodeViewData
    {
        /// <summary>
        /// A subitem used to visualise attachments on a node.
        /// </summary>
        public abstract class SubItemAttachment : SubItemText
        {
            protected Attachments.Attachment _attachment = null;

            /// <summary>
            /// The attachment stored in this subitem.
            /// </summary>
            public Attachments.Attachment Attachment
            {
                get
                {
                    return _attachment;
                }
            }

            protected System.Drawing.Drawing2D.GraphicsPath _drawnPath = null;
            public System.Drawing.Drawing2D.GraphicsPath DrawnPath
            {
                get
                {
                    return _drawnPath;
                }
                set
                {
                    _drawnPath = value;
                }
            }

            public override bool IsFSM
            {
                get
                {
                    return _attachment != null && _attachment.IsFSM;
                }
            }

            public override bool CanBeDraggedToTarget
            {
                get
                {
                    return _attachment != null && _attachment.CanBeDraggedToTarget;
                }
            }

            public override string ToolTip
            {
                get
                {
                    return _attachment.Description;
                }
            }

            public override DefaultObject SelectableObject
            {
                // when the subitem gets selected, show the stored attachment in the properties.
                get
                {
                    return _attachment;
                }
            }

            public override bool CanBeDeleted
            {
                get
                {
                    return _attachment != null ? _attachment.CanBeDeleted : true;
                }
            }

            protected override string Label
            {
                get
                {
                    return _attachment.Label;
                }
            }

            /// <summary>
            /// Creates a new subitm which can render an attachment on the node.
            /// </summary>
            /// <param name="backgroundNormal">The background brush used when the subitem is not selected.</param>
            /// <param name="backgroundSelected">The background brush used when the subitem is selected. If it cannot be selected, use null.</param>
            /// <param name="labelFont">The font used to draw the label.</param>
            /// <param name="labelBrush">The brush used to draw the label.</param>
            protected SubItemAttachment(Attachments.Attachment attach, Brush backgroundNormal, Brush backgroundSelected, Font labelFont, Brush labelBrush) :
            base(backgroundNormal, backgroundSelected, labelFont, labelBrush, Alignment.Center, false)
            {
                _attachment = attach;
            }
        }
    }
}
