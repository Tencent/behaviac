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

namespace PluginBehaviac.NodeExporters
{
    public class QueryCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            Query query = node as Query;
            return (query != null);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            Query query = node as Query;

            if (query == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\t\tthis.Initialize(\"{1}\", \"{2}\");",
                             indent, query.Domain, DesignerPropertyUtility.RetrieveExportValue(query.Descriptors));
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            Query query = node as Query;

            if (query == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tpublic void Initialize(string domain, string descriptors)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tthis.m_domain = domain;", indent);
            stream.WriteLine("{0}\t\t\tthis.SetDescriptors(descriptors);", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
