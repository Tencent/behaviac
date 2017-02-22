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
using System.Reflection;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using PluginBehaviac.DataExporters;

namespace PluginBehaviac.NodeExporters
{
    public class NodeCppExporter : NodeExporter
    {
        public static NodeCppExporter CreateInstance(Node node)
        {
            if (node != null)
            {
                Type exporterType = getExporterType(node.GetType());

                if (exporterType != null)
                {
                    return (NodeCppExporter)Activator.CreateInstance(exporterType);
                }
            }

            return new NodeCppExporter();
        }

        private static Type getExporterType(Type nodeType)
        {
            if (nodeType != null)
            {
                while (nodeType != typeof(Node))
                {
                    string nodeExporter = "PluginBehaviac.NodeExporters." + nodeType.Name + "CppExporter";
                    Type exporterType = Type.GetType(nodeExporter);

                    if (exporterType != null)
                    {
                        return exporterType;
                    }

                    foreach (Assembly assembly in Plugin.GetLoadedPlugins())
                    {
                        string filename = Path.GetFileNameWithoutExtension(assembly.Location);
                        nodeExporter = filename + ".NodeExporters." + nodeType.Name + "CppExporter";
                        exporterType = assembly.GetType(nodeExporter);

                        if (exporterType != null)
                        {
                            return exporterType;
                        }
                    }

                    nodeType = nodeType.BaseType;
                }
            }

            return null;
        }

        public override void GenerateClass(Node node, StringWriter stream, string indent, string nodeName, string agentType, string btClassName)
        {
            if (ShouldGenerateClass(node))
            {
                string className = GetGeneratedClassName(node, btClassName, nodeName);

                stream.WriteLine("{0}\tclass {1} : public {2}\r\n{0}\t{{", indent, className, node.ExportClass);
                stream.WriteLine("{0}\tpublic:\r\n{0}\t\tBEHAVIAC_DECLARE_DYNAMIC_TYPE({1}, {2});", indent, className, node.ExportClass);

                stream.WriteLine("{0}\t\t{1}()", indent, className);
                stream.WriteLine("{0}\t\t{{", indent);

                GenerateConstructor(node, stream, indent, className);

                stream.WriteLine("{0}\t\t}}", indent);

                GenerateMethod(node, stream, indent);

                GenerateMember(node, stream, indent);

                stream.WriteLine("{0}\t}};\r\n", indent);
            }
        }

        public override void GenerateInstance(Node node, StringWriter stream, string indent, string nodeName, string agentType, string btClassName)
        {
            string className = GetGeneratedClassName(node, btClassName, nodeName);

            // create a new instance of the node
            stream.WriteLine("{0}\t{1}* {2} = BEHAVIAC_NEW {1};", indent, className, nodeName);

            // set its basic properties
            stream.WriteLine("{0}\t{1}->SetClassNameString(\"{2}\");", indent, nodeName, node.ExportClass);
            stream.WriteLine("{0}\t{1}->SetId({2});", indent, nodeName, node.Id);
            stream.WriteLine("#if !BEHAVIAC_RELEASE");
            stream.WriteLine("{0}\t{1}->SetAgentType(\"{2}\");", indent, nodeName, agentType);
            stream.WriteLine("#endif");
        }

        protected string GetGeneratedClassName(Node node, string btClassName, string nodeName)
        {
            if (ShouldGenerateClass(node))
            {
                return string.Format("{0}_{1}_{2}", node.ExportClass, btClassName, nodeName);
            }

            return node.ExportClass;
        }

        protected virtual bool ShouldGenerateClass(Node node)
        {
            return false;
        }

        protected virtual void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
        }

        protected virtual void GenerateMember(Node node, StringWriter stream, string indent)
        {
        }

        protected virtual void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            stream.WriteLine("{0}\tprotected:", indent);
        }
    }
}
