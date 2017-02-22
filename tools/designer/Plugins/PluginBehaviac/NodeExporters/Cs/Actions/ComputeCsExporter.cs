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
    public class ComputeCsExporter : NodeCsExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            Compute compute = node as Compute;
            return (compute != null);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            Compute compute = node as Compute;

            if (compute == null)
            {
                return;
            }

            if (compute.Opr1 != null)
            {
                RightValueCsExporter.GenerateClassConstructor(node, compute.Opr1, stream, indent, "opr1");
            }

            if (compute.Opr2 != null)
            {
                RightValueCsExporter.GenerateClassConstructor(node, compute.Opr2, stream, indent, "opr2");
            }
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            Compute compute = node as Compute;

            if (compute == null)
            {
                return;
            }

            if (compute.Opr1 != null)
            {
                RightValueCsExporter.GenerateClassMember(compute.Opr1, stream, indent, "opr1");
            }

            if (compute.Opr2 != null)
            {
                RightValueCsExporter.GenerateClassMember(compute.Opr2, stream, indent, "opr2");
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            Compute compute = node as Compute;

            if (compute == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tprotected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tEBTStatus result = EBTStatus.BT_SUCCESS;", indent);

            if (compute.Opl != null && compute.Opr1 != null && compute.Opr2 != null)
            {
                string typeName = Plugin.GetNativeTypeName(compute.Opr1.ValueType);
                typeName = typeName.Replace("::", ".");

                RightValueCsExporter.GenerateCode(node, compute.Opr1, stream, indent + "\t\t\t", typeName, "opr1", "opr1");
                RightValueCsExporter.GenerateCode(node, compute.Opr2, stream, indent + "\t\t\t", typeName, "opr2", "opr2");

                string oprStr = string.Empty;

                switch (compute.Operator)
                {
                    case ComputeOperator.Add:
                        oprStr = "opr1 + opr2";
                        break;

                    case ComputeOperator.Sub:
                        oprStr = "opr1 - opr2";
                        break;

                    case ComputeOperator.Mul:
                        oprStr = "opr1 * opr2";
                        break;

                    case ComputeOperator.Div:
                        oprStr = "opr1 / opr2";
                        break;

                    default:
                        Debug.Check(false, "The operator is wrong!");
                        break;
                }

                oprStr = string.Format("({0})({1})", typeName, oprStr);

                PropertyDef prop = compute.Opl.Property;

                if (prop != null)
                {
                    string property = PropertyCsExporter.GetProperty(node, prop, compute.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "opl", "compute");
                    string propName = prop.BasicName.Replace("[]", "");

                    if (prop.IsArrayElement && compute.Opl.ArrayIndexElement != null)
                    {
                        ParameterCsExporter.GenerateCode(node, compute.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opl_index", "compute_opl");
                        property = string.Format("({0})[opl_index]", property);
                    }

                    if (!prop.IsArrayElement && (prop.IsPar || prop.IsCustomized))
                    {
                        string propBasicName = prop.BasicName.Replace("[]", "");
                        uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);
                        string agentName = PropertyCsExporter.GetGenerateAgentName(prop, "opl", "compute");
                        string typename = DataCsExporter.GetGeneratedNativeType(prop.NativeType);

                        stream.WriteLine("{0}\t\t\t{1}.SetVariable<{2}>(\"{3}\", {4}u, {5});", indent, agentName, typename, propBasicName, id, oprStr);
                    }
                    else
                    {
                        stream.WriteLine("{0}\t\t\t{1} = {2};", indent, property, oprStr);
                    }
                }

                if (compute.Opr1.IsMethod)
                {
                    RightValueCsExporter.PostGenerateCode(compute.Opr1, stream, indent + "\t\t\t", typeName, "opr1", string.Empty);
                }

                if (compute.Opr2.IsMethod)
                {
                    RightValueCsExporter.PostGenerateCode(compute.Opr2, stream, indent + "\t\t\t", typeName, "opr2", string.Empty);
                }
            }

            stream.WriteLine("{0}\t\t\treturn result;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
