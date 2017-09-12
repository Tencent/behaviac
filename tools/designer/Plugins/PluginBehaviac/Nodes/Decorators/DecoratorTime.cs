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
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Decorators", NodeIcon.Time)]
    public class DecoratorTime : Decorator
    {
        public DecoratorTime()
        : base(Resources.DecoratorTime, Resources.DecoratorTimeDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/decorator/#time";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorTime";
            }
        }

        protected RightValueDef _time = new RightValueDef(new VariableDef(1000.0f));
        [DesignerRightValueEnum("DecoratorTime", "DecoratorTimeDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef Time
        {
            get
            {
                return _time;
            }
            set
            {
                this._time = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            DecoratorTime dec = (DecoratorTime)newnode;

            if (_time != null)
            {
                dec._time = (RightValueDef)_time.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            Type valueType = (this._time != null) ? this._time.ValueType : null;

            if (valueType == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Time is not set!"));
            }
            else if (!Plugin.IsIntergerType(valueType) && !Plugin.IsFloatType(valueType))
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Time must be a float type!"));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
