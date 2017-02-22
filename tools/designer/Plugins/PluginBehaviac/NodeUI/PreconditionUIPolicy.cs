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
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;

namespace Behaviac.Design.ObjectUI
{
    class PreconditionUIPolicy : AttachActionUIPolicy
    {
        public override bool ShouldAddProperty(DesignerPropertyInfo property)
        {
            if (_obj != null)
            {
                DesignerPropertyInfo binaryOperator = DesignerProperty.GetDesignerProperty(_obj.GetType(), "BinaryOperator");

                if (property.Property == binaryOperator.Property)
                {
                    Attachments.Attachment attach = _obj as Attachments.Attachment;

                    if (attach != null && attach.Node != null && attach.Node.Attachments != null
                        && attach.Node.Attachments.Count > 0 && attach.Node.Attachments[0] == attach)
                    {
                        return false;
                    }
                }
            }

            return base.ShouldAddProperty(property);
        }
    }
}
