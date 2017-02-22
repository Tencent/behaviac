/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Nodes;

namespace Behaviac.Design.Attachments
{
    /// <summary>
    /// This class represents an event which is attached to a node.
    /// </summary>
    public class Predicate : Attach
    {
        /// <summary>
        /// Create a new node event.
        /// </summary>
        /// <param name="node">The node this event belongs to.</param>
        public Predicate(Nodes.Node node, string label, string description)
        : base(node, label, description)
        {
        }

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            return null;
        }
    }
}
