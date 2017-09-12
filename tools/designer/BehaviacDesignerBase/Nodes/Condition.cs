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
using Behaviac.Design.Attachments;
using Behaviac.Design.Attributes;

namespace Behaviac.Design.Nodes
{
    [Behaviac.Design.EnumDesc("Behaviac.Design.Nodes.OperatorType", true, "OperatorType", "OperatorTypeDesc")]
    public enum OperatorType
    {
        //Noop,
        [Behaviac.Design.EnumMemberDesc("Assignment", true, "=", "OperatorType_Assignment")]
        Assignment,

        //[Behaviac.Design.EnumMemberDesc("In", true, "in", "OperatorType_In")]
        //In,

        [Behaviac.Design.EnumMemberDesc("And", true, "&&", "OperatorType_And")]
        And,

        [Behaviac.Design.EnumMemberDesc("Or", true, "||", "OperatorType_Or")]
        Or,

        [Behaviac.Design.EnumMemberDesc("Equal", true, "==", "OperatorType_Equal")]
        Equal,

        [Behaviac.Design.EnumMemberDesc("NotEqual", true, "!=", "OperatorType_NonEqual")]
        NotEqual,

        [Behaviac.Design.EnumMemberDesc("Greater", true, ">", "OperatorType_Greater")]
        Greater,

        [Behaviac.Design.EnumMemberDesc("Less", true, "<", "OperatorType_Less")]
        Less,

        [Behaviac.Design.EnumMemberDesc("GreaterEqual", true, ">=", "OperatorType_GreaterEqual")]
        GreaterEqual,

        [Behaviac.Design.EnumMemberDesc("LessEqual", true, "<=", "OperatorType_LessEqual")]
        LessEqual
    }

    /// <summary>
    /// This node represents a condition which can be attached to the behaviour tree.
    /// </summary>
    public class Condition : Node
    {
        public Condition(string label, string description)
        : base(label, description)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/condition/";
            }
        }

        public override bool IsCondition
        {
            get
            {
                return true;
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(200, 100, 39));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        public override NodeViewData CreateNodeViewData(NodeViewData parent, BehaviorNode rootBehavior)
        {
            NodeViewData nvd = base.CreateNodeViewData(parent, rootBehavior);
            nvd.ChangeShape(NodeShape.AngleRectangle);

            return nvd;
        }

        public override bool AcceptsAttachment(DefaultObject obj)
        {
            return false;
        }
    }
}
