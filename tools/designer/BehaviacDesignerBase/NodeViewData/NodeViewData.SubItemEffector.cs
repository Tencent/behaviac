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
using System.Drawing;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;

namespace Behaviac.Design
{
    public partial class NodeViewData
    {
        /// <summary>
        /// A subitem used to visualise Effector on a node.
        /// </summary>
        public class SubItemEffector : NodeViewData.SubItemEvent
        {
            /// <summary>
            /// Creates a new subitm which can render an Effector on the node.
            /// </summary>
            /// <param name="e">The effector we want to draw.</param>
            public SubItemEffector(Attach e)
            : base(e)
            {
            }

            public override Brush BackgroundBrush
            {
                get
                {
                    if (!IsSelected && _attachment != null && _attachment is AttachAction)
                    {
                        return _theBothBrush;
                    }

                    return base.BackgroundBrush;
                }
            }

            public override NodeViewData.SubItem Clone(Node newnode)
            {
                return new SubItemEffector((Attach)_attachment.Clone(newnode));
            }
        }
    }
}
