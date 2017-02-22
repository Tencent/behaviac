/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR IfExistS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Behaviac.Design;
using PluginBehaviac.Properties;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("HTN", "method_icon")]
    public class Method : Behaviac.Design.Nodes.Method
    {
        public Method()
        : base(Resources.Method, Resources.MethodDesc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "Method";
            }
        }

        public override bool CanAdopt(BaseNode child)
        {
            if (base.CanAdopt(child))
            {
                return child is Sequence ||
                       child is Selector ||
                       child is Parallel ||
                       child is DecoratorLoop ||
                       child is Action ||
                       child is DecoratorIterator ||
                       child is Behaviac.Design.Nodes.ReferencedBehavior ||
                       child is Behaviac.Design.Nodes.Behavior;
            }

            return false;
        }

        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            return base.CanBeAdoptedBy(parent) && (parent is Task);
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (_Task.Child == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.NoMethosError));
            }

            if (!(this.Parent is Task))
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.MethodParentError));
            }

            base.CheckForErrors(rootBehavior, result);
        }
    }
}
