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
using PluginBehaviac.Properties;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Composites:Selectors", "selectorProbability_ico")]
    public class SelectorProbability : Behaviac.Design.Nodes.Selector
    {
        public SelectorProbability()
        : base(Resources.SelectorProbability, Resources.SelectorProbabilityDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/selectorprobability/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "SelectorProbability";
            }
        }

        private MethodDef _method;
        [DesignerMethodEnum("RandomGenerator", "RandomGeneratorDesc", "Action", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, MethodType.Getter, ValueTypes.Float)]
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

        public override bool AddChild(Connector connector, Node node)
        {
            if (node is DecoratorWeight)
            {
                return base.AddChild(connector, node);
            }
            else
            {
                DecoratorWeight weightNode = new DecoratorWeight();

                if (base.AddChild(connector, weightNode))
                {

                    bool add = weightNode.AddChild(weightNode.GetConnector(connector.Identifier), node);

                    weightNode.ResetId(true);

                    return add;
                }
            }

            return false;
        }

        public override bool AddChild(Connector connector, Node node, int index)
        {
            if (node is DecoratorWeight)
            {
                return base.AddChild(connector, node, index);
            }
            else
            {
                DecoratorWeight weightNode = new DecoratorWeight();

                if (base.AddChild(connector, weightNode, index))
                {

                    bool add = weightNode.AddChild(weightNode.GetConnector(connector.Identifier), node);

                    weightNode.ResetId(true);

                    return add;
                }
            }

            return false;
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            SelectorProbability prec = (SelectorProbability)newnode;

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
