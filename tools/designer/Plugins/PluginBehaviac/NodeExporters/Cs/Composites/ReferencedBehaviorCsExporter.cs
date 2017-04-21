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
using PluginBehaviac.DataExporters;

namespace PluginBehaviac.NodeExporters
{
    public class ReferencedBehaviorCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            ReferencedBehavior referencedBehavior = node as ReferencedBehavior;
            return (referencedBehavior != null);
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            ReferencedBehavior pReferencedBehavior = node as ReferencedBehavior;

            if (pReferencedBehavior == null)
            {
                return;
            }

            if (pReferencedBehavior.ReferenceBehavior != null)
            {
                RightValueCsExporter.GenerateClassMember(pReferencedBehavior.ReferenceBehavior, stream, indent, "Behavior");
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            ReferencedBehavior pReferencedBehavior = node as ReferencedBehavior;

            if (pReferencedBehavior == null)
            {
                return;
            }

            if (pReferencedBehavior.ReferenceBehavior != null)
            {
                stream.WriteLine("{0}\t\tpublic override string GetReferencedTree(Agent pAgent)", indent);
                stream.WriteLine("{0}\t\t{{", indent);

                string retStr = RightValueCsExporter.GenerateCode(node, pReferencedBehavior.ReferenceBehavior, stream, indent + "\t\t\t", string.Empty, string.Empty, "Behavior");

                bool bConst = false;

                if (pReferencedBehavior.ReferenceBehavior.Var != null && pReferencedBehavior.ReferenceBehavior.Var.IsConst)
                {
                    bConst = true;
                }

                if (!bConst)
                {
                    stream.WriteLine("{0}\t\t\tif (pAgent != null) {{", indent);
                }

                stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);

                if (!bConst)
                {
                    stream.WriteLine("{0}\t\t\t}}", indent);
                    stream.WriteLine("{0}\t\t\treturn null;", indent);
                }

                stream.WriteLine("{0}\t\t}}", indent);
            }
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            ReferencedBehavior pReferencedBehavior = node as ReferencedBehavior;

            if (pReferencedBehavior == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\t\tstring szTreePath = this.GetReferencedTree(null);", indent);
            stream.WriteLine("{0}\t\t\tif (!string.IsNullOrEmpty(szTreePath)) {{", indent);
            stream.WriteLine("{0}\t\t\tBehaviorTree behaviorTree = Workspace.Instance.LoadBehaviorTree(szTreePath);", indent);
            stream.WriteLine("{0}\t\t\tif (behaviorTree != null)", indent);
            stream.WriteLine("{0}\t\t\t{{", indent);
            stream.WriteLine("{0}\t\t\t\tthis.m_bHasEvents |= behaviorTree.HasEvents();", indent);
            stream.WriteLine("{0}\t\t\t}}", indent);
            stream.WriteLine("{0}\t\t\t}}", indent);

            if (pReferencedBehavior.Task != null)
            {
                string method = pReferencedBehavior.Task.GetExportValue();
                method = method.Replace("\"", "\\\"");
                stream.WriteLine("{0}\t\t\tthis.m_taskMethod = AgentMeta.ParseMethod(\"{1}\");", indent, method);
                stream.WriteLine("{0}\t\t\tDebug.Check(this.m_taskMethod != null);", indent);
            }
        }
    }
}
