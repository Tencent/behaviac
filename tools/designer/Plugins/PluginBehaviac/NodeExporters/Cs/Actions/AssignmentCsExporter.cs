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
using PluginBehaviac.Nodes;
using PluginBehaviac.DataExporters;

namespace PluginBehaviac.NodeExporters
{
    public class AssignmentCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            Assignment assignment = node as Assignment;
            return (assignment != null);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            Assignment assignment = node as Assignment;

            if (assignment == null)
            {
                return;
            }

            if (assignment.IsCasting)
            {
                stream.WriteLine("{0}\t\t\tm_bCast = true;", indent);
            }

            if (assignment.Opr != null)
            {
                RightValueCsExporter.GenerateClassConstructor(node, assignment.Opr, stream, indent, "opr");
            }
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            Assignment assignment = node as Assignment;

            if (assignment == null)
            {
                return;
            }

            if (assignment.Opr != null)
            {
                RightValueCsExporter.GenerateClassMember(assignment.Opr, stream, indent, "opr");
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            Assignment assignment = node as Assignment;

            if (assignment == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tprotected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tEBTStatus result = EBTStatus.BT_SUCCESS;", indent);

            if (assignment.Opl != null && assignment.Opr != null)
            {
                PropertyDef prop = assignment.Opl.Property;

                if (prop != null)
                {
                    RightValueCsExporter.GenerateCode(node, assignment.Opr, stream, indent + "\t\t\t", assignment.Opr.NativeType.Replace("::", "."), "opr", "opr");

                    string property = PropertyCsExporter.GetProperty(node, prop, assignment.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "opl", "assignment");
                    string propName = prop.BasicName.Replace("[]", "");

                    if (prop.IsArrayElement && assignment.Opl.ArrayIndexElement != null)
                    {
                        ParameterCsExporter.GenerateCode(node, assignment.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opl_index", "assignment_opl");
                        property = string.Format("({0})[opl_index]", property);
                    }

                    string oprStr = "opr";

                    if (!Plugin.IsArrayType(prop.Type))
                    {
                        if (assignment.Opr.Var != null && assignment.Opr.Var.ArrayIndexElement != null)
                        {
                            ParameterCsExporter.GenerateCode(node, assignment.Opr.Var.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opr_index", "assignment_opr");
                            oprStr = string.Format("({0})[opr_index]", oprStr);
                        }
                    }

                    if (!prop.IsArrayElement && (!prop.IsPublic || prop.IsPar || prop.IsCustomized))
                    {
                        string propBasicName = prop.BasicName.Replace("[]", "");
                        uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);
                        string agentName = PropertyCsExporter.GetGenerateAgentName(prop, "opl", "assignment");
                        string typename = DataCsExporter.GetGeneratedNativeType(prop.NativeType);

                        stream.WriteLine("{0}\t\t\t{1}.SetVariable<{2}>(\"{3}\", {4}u, {5});", indent, agentName, typename, propBasicName, id, oprStr);
                    }
                    else
                    {
                        if (assignment.IsCasting)
                        {
                            stream.WriteLine("{0}\t\t\t{1} = ({2}){3};", indent, property, DataCsExporter.GetGeneratedNativeType(assignment.Opl.ValueType), oprStr);
                        }
                        else
                        {
                            stream.WriteLine("{0}\t\t\t{1} = {2};", indent, property, oprStr);
                        }
                    }
                }

                if (assignment.Opr.IsMethod)
                {
                    RightValueCsExporter.PostGenerateCode(assignment.Opr, stream, indent + "\t\t\t", assignment.Opr.NativeType.Replace("::", "."), "opr", string.Empty);
                }
            }

            stream.WriteLine("{0}\t\t\treturn result;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
