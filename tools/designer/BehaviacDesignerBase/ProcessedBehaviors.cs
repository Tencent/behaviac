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
using Behaviac.Design.Nodes;

namespace Behaviac.Design
{
    /// <summary>
    /// This class is used to handle circular references.
    /// </summary>
    public class ProcessedBehaviors
    {
        protected List<ReferencedBehavior> _processedBehaviors = new List<ReferencedBehavior>();

        /// <summary>
        /// Used to create a new list of processed behaviours.
        /// </summary>
        public ProcessedBehaviors()
        {
        }

        /// <summary>
        /// Used to branch a new list for its children.
        /// </summary>
        /// <param name="previous">The list of processed behaviours the parent used.</param>
        protected ProcessedBehaviors(ProcessedBehaviors previous)
        {
            _processedBehaviors.AddRange(previous._processedBehaviors);
        }

        /// <summary>
        /// Checks if a node may be processed without running into circular references. The node is not added to the list.
        /// </summary>
        /// <param name="node">The node we want to process.</param>
        /// <returns>Returns true when the node may be processed, if not the calling function has to stop.</returns>
        public bool MayProcessCheckOnly(Node node)
        {
            ReferencedBehavior refnode = node as ReferencedBehavior;

            if (refnode != null)
            {
                return !_processedBehaviors.Contains(refnode);
            }

            return true;
        }

        /// <summary>
        /// Checks if a node may be processed without running into circular references and adds it to the list.
        /// </summary>
        /// <param name="node">The node we want to process.</param>
        /// <returns>Returns true when the node may be processed, if not the calling function has to stop.</returns>
        public bool MayProcess(Node node)
        {
            ReferencedBehavior refnode = node as ReferencedBehavior;

            if (refnode != null)
            {
                if (_processedBehaviors.Contains(refnode))
                {
                    return false;
                }

                _processedBehaviors.Add(refnode);
            }

            return true;
        }

        /// <summary>
        /// Branches the list for a node.
        /// </summary>
        /// <param name="node">The node we are branching for.</param>
        /// <returns>A new list which contains the previously processed behaviours for referenced behaviours. For other nodes it returns the same list.</returns>
        public ProcessedBehaviors Branch(Node node)
        {
            ReferencedBehavior refnode = node as ReferencedBehavior;

            if (refnode != null)
            {
                return new ProcessedBehaviors(this);
            }

            return this;
        }
    }
}
