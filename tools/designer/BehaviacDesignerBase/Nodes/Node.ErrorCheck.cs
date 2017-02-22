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

namespace Behaviac.Design.Nodes
{
    /// <summary>
    /// Defines if a message from the error check is a message, warning or error.
    /// </summary>
    public enum ErrorCheckLevel
    {
        Message = 1,
        Warning = 2,
        Error   = 4
    };

    public partial class Node
    {
        /// <summary>
        /// This class contains an item from the error check.
        /// </summary>
        public struct ErrorCheck
        {
            private Node _node;

            /// <summary>
            /// The node which was checked.
            /// </summary>
            public Node Node
            {
                get
                {
                    return _node;
                }
            }

            private ErrorCheckLevel _level;

            /// <summary>
            /// The elevel of the message.
            /// </summary>
            public ErrorCheckLevel Level
            {
                get
                {
                    return _level;
                }
            }

            private string _description;

            /// <summary>
            /// The description of the error.
            /// </summary>
            public string Description
            {
                get
                {
                    return _description;
                }
            }

            /// <summary>
            /// Creates a result from the error check.
            /// </summary>
            /// <param name="node">The node we checked.</param>
            /// <param name="level">The type of message we want to post.</param>
            /// <param name="description">The posted message.</param>
            public ErrorCheck(Node node, ErrorCheckLevel level, string description)
            {
                _node = node;
                _level = level;
                _description = description;

                if (_node != null)
                {
                    _description = string.Format("{0}({1}) : {2}", _node.Label, _node.Id, description);
                }
            }

            public ErrorCheck(Node node, int id, string label, ErrorCheckLevel level, string description)
            {
                _node = node;
                _level = level;
                _description = string.Format("{0}({1}) : {2}", label, id, description);
            }
        }
    }
}
