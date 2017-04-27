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
    public class AssignmentCppExporter : NodeCppExporter
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
                RightValueCppExporter.GenerateClassConstructor(node, assignment.Opr, stream, indent, "opr");
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
                RightValueCppExporter.GenerateClassMember(assignment.Opr, stream, indent, "opr");
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

            stream.WriteLine("{0}\t\tvirtual EBTStatus update_impl(Agent* pAgent, EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(pAgent);", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(childStatus);", indent);
            stream.WriteLine("{0}\t\t\tEBTStatus result = BT_SUCCESS;", indent);

            if (assignment.Opl != null && assignment.Opr != null)
            {
                PropertyDef prop = assignment.Opl.Property;

                if (prop != null)
                {
                    RightValueCppExporter.GenerateCode(node, assignment.Opr, stream, indent + "\t\t\t", assignment.Opr.NativeType, "opr", "opr");

                    string property = PropertyCppExporter.GetProperty(node, prop, assignment.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "opl", "assignment");
                    string propName = prop.BasicName.Replace("[]", "");

                    if (prop.IsArrayElement && assignment.Opl.ArrayIndexElement != null)
                    {
                        ParameterCppExporter.GenerateCode(node, assignment.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opl_index", "assignment_opl");
                        property = string.Format("({0})[opl_index]", property);
                    }

                    string opr = "opr";

                    if (!Plugin.IsArrayType(prop.Type))
                    {
                        if (assignment.Opr.Var != null && assignment.Opr.Var.ArrayIndexElement != null)
                        {
                            ParameterCppExporter.GenerateCode(node, assignment.Opr.Var.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opr_index", "assignment_opr");
                            opr = string.Format("({0})[opr_index]", opr);
                        }
                    }

                    if (!prop.IsArrayElement && (prop.IsPar || prop.IsCustomized))
                    {
                        string propBasicName = prop.BasicName.Replace("[]", "");
                        uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);
                        string agentName = PropertyCppExporter.GetGenerateAgentName(prop, "opl", "assignment");

                        stream.WriteLine("{0}\t\t\t{1}->SetVariable(\"{2}\", {3}u, {4});", indent, agentName, propBasicName, id, opr);
                    }
                    else
                    {
                        if (assignment.IsCasting)
                        {
                            stream.WriteLine("{0}\t\t\t{1} = ({2}){3};", indent, property, DataCppExporter.GetGeneratedNativeType(assignment.Opl.ValueType), opr);
                        }
                        else
                        {
                            stream.WriteLine("{0}\t\t\t{1} = {2};", indent, property, opr);
                        }
                    }
                }

                if (assignment.Opr.IsMethod)
                {
                    RightValueCppExporter.PostGenerateCode(assignment.Opr, stream, indent + "\t\t\t", assignment.Opr.NativeType, "opr", string.Empty);
                }
            }

            stream.WriteLine("{0}\t\t\treturn result;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
