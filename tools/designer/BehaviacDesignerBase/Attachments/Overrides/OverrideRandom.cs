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
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Nodes;

namespace Behaviac.Design.Attachments.Overrides
{
    public class OverrideRandom : Override
    {
        public OverrideRandom(Node node)
        : base(node, Resources.OverrideRandom, Resources.OverrideRandomDesc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "OverrideRandom";
            }
        }

        protected override void CloneProperties(Attachment newattach)
        {
            base.CloneProperties(newattach);

            Override newoverride = (Override)newattach;
        }

        protected float _min;

        [DesignerFloat("RandomMin", "RandomMinDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, "PropertyToOverride", 0, 0, 0, 0, null)]
        public float Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }

        protected float _max;

        [DesignerFloat("RandomMax", "RandomMaxDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, "PropertyToOverride", 0, 0, 0, 0, null)]
        public float Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }
    }
}
