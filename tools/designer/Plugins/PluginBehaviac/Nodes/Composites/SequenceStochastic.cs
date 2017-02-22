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
using PluginBehaviac.Properties;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Composites:Sequences", "sequenceStochastic_ico")]
    public class SequenceStochastic : Behaviac.Design.Nodes.Sequence
    {
        public SequenceStochastic()
        : base(Resources.SequenceStochastic, Resources.SequenceStochasticDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/language/zh/sequencestochastic/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "SequenceStochastic";
            }
        }

        private MethodDef _method;
        [DesignerMethodEnum("RandomGenerator", "RandomGeneratorDesc", "Action", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, MethodType.Getter, ValueTypes.Int)]
        public MethodDef RandomGenerator
        {
            get
            {
                return _method;
            }
            set
            {
                this._method = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            SequenceStochastic prec = (SequenceStochastic)newnode;

            if (_method != null)
            {
                prec._method = (MethodDef)_method.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (this._method == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Warning, Resources.RandomGeneratorNotSpecified));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
