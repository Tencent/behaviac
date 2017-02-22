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
using System.IO;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using PluginBehaviac.Nodes;
using PluginBehaviac.DataExporters;

namespace PluginBehaviac.NodeExporters
{
    public class WaitCppExporter : NodeCppExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            Wait wait = node as Wait;
            return (wait != null);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            Wait wait = node as Wait;

            if (wait == null)
            {
                return;
            }

            if (wait.Time != null)
            {
                RightValueCppExporter.GenerateClassConstructor(node, wait.Time, stream, indent, "Time");
            }
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            Wait wait = node as Wait;

            if (wait == null)
            {
                return;
            }

            if (wait.Time != null)
            {
                RightValueCppExporter.GenerateClassMember(wait.Time, stream, indent, "Time");
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            Wait wait = node as Wait;

            if (wait == null)
            {
                return;
            }

            if (wait.Time != null)
            {
                stream.WriteLine("{0}\t\tvirtual double GetTime(Agent* pAgent) const", indent);
                stream.WriteLine("{0}\t\t{{", indent);
                stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(pAgent);", indent);

                string retStr = RightValueCppExporter.GenerateCode(node, wait.Time, stream, indent + "\t\t\t", string.Empty, string.Empty, "Time");

                stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);
                stream.WriteLine("{0}\t\t}}", indent);
            }
        }
    }
}
