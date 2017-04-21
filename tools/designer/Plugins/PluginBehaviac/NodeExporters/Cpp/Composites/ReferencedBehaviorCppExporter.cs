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
    public class ReferencedBehaviorCppExporter : NodeCppExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            ReferencedBehavior referencedBehavior = node as ReferencedBehavior;
            return (referencedBehavior != null);
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            ReferencedBehavior pReferencedNode = node as ReferencedBehavior;

            if (pReferencedNode == null)
            {
                return;
            }

            if (pReferencedNode.ReferenceBehavior != null)
            {
                RightValueCppExporter.GenerateClassMember(pReferencedNode.ReferenceBehavior, stream, indent, "Behavior");
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            ReferencedBehavior referencedBehavior = node as ReferencedBehavior;

            if (referencedBehavior == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tvirtual const char* GetReferencedTree(const Agent* pAgent) const", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(pAgent);", indent);

            string retStr = RightValueCppExporter.GenerateCode(node, referencedBehavior.ReferenceBehavior, stream, indent + "\t\t\t", "const char*", string.Empty, "_referencedBehavior");

            bool bConst = false;

            if (referencedBehavior.ReferenceBehavior.Var != null && referencedBehavior.ReferenceBehavior.Var.IsConst)
            {
                bConst = true;
            }

            if (!bConst)
            {
                stream.WriteLine("{0}\t\t\tif (pAgent) {{", indent);
            }

            stream.WriteLine("{0}\t\t\treturn {1};", indent, retStr);

            if (!bConst)
            {
                stream.WriteLine("{0}\t\t\t}}", indent);
                stream.WriteLine("{0}\t\t\treturn 0;", indent);
            }

            stream.WriteLine("{0}\t\t}}", indent);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            ReferencedBehavior referencedBehavior = node as ReferencedBehavior;

            if (referencedBehavior == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\t\tconst char* szTreePath = this->GetReferencedTree(0);", indent);
            stream.WriteLine("{0}\t\t\tif (szTreePath) {{", indent);
            stream.WriteLine("{0}\t\t\tBehaviorTree* behaviorTree = Workspace::GetInstance()->LoadBehaviorTree(szTreePath);", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_ASSERT(behaviorTree);", indent);
            stream.WriteLine("{0}\t\t\tif (behaviorTree)", indent);
            stream.WriteLine("{0}\t\t\t{{", indent);
            stream.WriteLine("{0}\t\t\t\tthis->m_bHasEvents |= behaviorTree->HasEvents();", indent);
            stream.WriteLine("{0}\t\t\t}}", indent);
            stream.WriteLine("{0}\t\t\t}}", indent);

            if (referencedBehavior.Task != null)
            {
                string method = referencedBehavior.Task.GetExportValue();
                method = method.Replace("\"", "\\\"");
                stream.WriteLine("{0}\t\t\tm_taskMethod = AgentMeta::ParseMethod(\"{1}\");", indent, method);
                stream.WriteLine("{0}\t\t\tBEHAVIAC_ASSERT(m_taskMethod);", indent);
            }
        }
    }
}
