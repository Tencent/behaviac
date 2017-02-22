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
using Behaviac.Design.Properties;
using Behaviac.Design.Attachments;
using Behaviac.Design.Attributes;

namespace Behaviac.Design.Nodes
{
    /// <summary>
    /// This node represents a sequence which can be attached to the behaviour tree.
    /// </summary>
    public class Sequence : Node
    {
        protected ConnectorSingle _interruptChild;
        protected ConnectorMultiple _genericChildren;

        public Sequence(string label, string description)
        : base(label, description)
        {
            CreateInterruptChild();
            _genericChildren = new ConnectorMultiple(_children, "", Connector.kGeneric, 2, int.MaxValue);
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/language/zh/sequence/";
            }
        }

        protected virtual void CreateInterruptChild()
        {
            _interruptChild = new ConnectorSingle(_children, "", Connector.kInterupt);
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(79, 129, 189));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        protected bool _do_sequence_error_check = true;
        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (_do_sequence_error_check)
            {
                if (_genericChildren.EnableChildCount < 1)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.SequenceNoChildrenError));
                }

                else if (_genericChildren.EnableChildCount < 2)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Warning, Resources.SequenceOnlyOneChildError));
                }
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
