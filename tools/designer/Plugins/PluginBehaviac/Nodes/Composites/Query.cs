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
using Behaviac.Design.Attributes;
using PluginBehaviac.Properties;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Actions", NodeIcon.Query)]
    class Query : Behaviac.Design.Nodes.Node
    {
        protected ConnectorSingle _requery;
        public Query()
        : base(Resources.Query, Resources.QueryDesc)
        {
            _requery = new ConnectorSingle(_children, Resources.Requery, Connector.kInterupt);
        }

        public override string ExportClass
        {
            get
            {
                return "Query";
            }
        }

        private string _domain = "";
        [DesignerString("BehaviorDomain", "BehaviorDomainDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags)]
        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                _domain = value;
            }
        }

        public class Descriptor_t
        {
            private VariableDef _attribute;
            [DesignerPropertyEnum("Attribute", "AttributeDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.Self | DesignerPropertyEnum.AllowStyles.Instance, "", "Reference")]
            public VariableDef Attribute
            {
                get
                {
                    return _attribute;
                }
                set
                {
                    _attribute = value;
                }
            }

            private VariableDef _reference;
            [DesignerPropertyEnum("Reference", "ReferenceDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.Const, "Attribute", "")]
            public VariableDef Reference
            {
                get
                {
                    return _reference;
                }
                set
                {
                    _reference = value;
                }
            }

            float _descriptorWeight = 0.0f;
            [DesignerFloat("Weight", "WeightDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.NoFlags, null, 0.0f, 100.0f, 1.0f, 1, "%")]
            public float Weight
            {
                get
                {
                    return _descriptorWeight;
                }
                set
                {
                    _descriptorWeight = value;
                }
            }

            public Descriptor_t Clone()
            {
                Descriptor_t descriptor = new Descriptor_t();
                descriptor._attribute = (VariableDef)_attribute.Clone();
                descriptor._reference = (VariableDef)_reference.Clone();
                descriptor._descriptorWeight = _descriptorWeight;

                return descriptor;
            }
        }

        private List<Descriptor_t> _descriptors = new List<Descriptor_t>();

        [DesignerArrayStruct("Descriptors", "DescriptorsDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public List<Descriptor_t> Descriptors
        {
            get
            {
                return _descriptors;
            }
            set
            {
                this._descriptors = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            Query newQuery = (Query)newnode;
            newQuery._domain = _domain;

            newQuery._descriptors.Clear();

            foreach (Descriptor_t descriptor in _descriptors)
            {
                newQuery._descriptors.Add(descriptor.Clone());
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(200, 120, 50));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        public override NodeViewData CreateNodeViewData(NodeViewData parent, Behaviac.Design.Nodes.BehaviorNode rootBehavior)
        {
            NodeViewData nvd = base.CreateNodeViewData(parent, rootBehavior);
            nvd.ChangeShape(NodeShape.Rectangle);

            return nvd;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (this.Domain == string.Empty && this._descriptors.Count == 0)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Invalid query without domain or any descriptors"));
            }

            for (int i = 0; i < this._descriptors.Count; ++i)
            {
                Descriptor_t dw = this._descriptors[i];

                if (dw.Attribute != null && dw.Weight == 0.0f)
                {
                    string msg = string.Format("Descriptors[{0}]'s Weight is 0.0f!", i);
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, msg));
                }
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
