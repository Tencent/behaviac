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
using Behaviac.Design;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Conditions", NodeIcon.And)]
    public class And : OperatorCondition
    {
        protected ConnectorMultiple _conditions;

        public And() : base(Resources.And, Resources.AndDesc)
        {
            _conditions = new ConnectorCondition(_children, "Condition {0}", "Conditions", 2, int.MaxValue);
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/and/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "And";
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (this.Children.Count < 2)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "There should be at least 2 condition children."));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
