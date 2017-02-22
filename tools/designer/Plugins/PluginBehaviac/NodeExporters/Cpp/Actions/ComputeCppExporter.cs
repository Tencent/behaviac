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
    public class ComputeCppExporter : NodeCppExporter
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
                RightValueCppExporter.GenerateClassConstructor(node, compute.Opr1, stream, indent, "opr1");
            }

            if (compute.Opr2 != null)
            {
                RightValueCppExporter.GenerateClassConstructor(node, compute.Opr2, stream, indent, "opr2");
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
                RightValueCppExporter.GenerateClassMember(compute.Opr1, stream, indent, "opr1");
            }

            if (compute.Opr2 != null)
            {
                RightValueCppExporter.GenerateClassMember(compute.Opr2, stream, indent, "opr2");
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

            stream.WriteLine("{0}\t\tvirtual EBTStatus update_impl(Agent* pAgent, EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(pAgent);", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(childStatus);", indent);
            stream.WriteLine("{0}\t\t\tEBTStatus result = BT_SUCCESS;", indent);

            if (compute.Opl != null && compute.Opr1 != null && compute.Opr2 != null)
            {
                string typeName = DataCppExporter.GetGeneratedNativeType(compute.Opr1.ValueType);

                RightValueCppExporter.GenerateCode(node, compute.Opr1, stream, indent + "\t\t\t", typeName, "opr1", "opr1");
                RightValueCppExporter.GenerateCode(node, compute.Opr2, stream, indent + "\t\t\t", typeName, "opr2", "opr2");

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
                    string property = PropertyCppExporter.GetProperty(node, prop, compute.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "opl", "compute");
                    string propName = prop.BasicName.Replace("[]", "");

                    if (prop.IsArrayElement && compute.Opl.ArrayIndexElement != null)
                    {
                        ParameterCppExporter.GenerateCode(node, compute.Opl.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opl_index", "compute_opl");
                        property = string.Format("({0})[opl_index]", property);
                    }

                    if (!prop.IsArrayElement && (prop.IsPar || prop.IsCustomized))
                    {
                        string propBasicName = prop.BasicName.Replace("[]", "");
                        uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);
                        string agentName = PropertyCppExporter.GetGenerateAgentName(prop, "opl", "compute");

                        stream.WriteLine("{0}\t\t\t{1}->SetVariable(\"{2}\", {3}u, {4});", indent, agentName, propBasicName, id, oprStr);
                    }
                    else
                    {
                        stream.WriteLine("{0}\t\t\t{1} = {2};", indent, property, oprStr);
                    }
                }

                if (compute.Opr1.IsMethod)
                {
                    RightValueCppExporter.PostGenerateCode(compute.Opr1, stream, indent + "\t\t\t", compute.Opr1.NativeType, "opr1", string.Empty);
                }

                if (compute.Opr2.IsMethod)
                {
                    RightValueCppExporter.PostGenerateCode(compute.Opr2, stream, indent + "\t\t\t", compute.Opr2.NativeType, "opr2", string.Empty);
                }
            }

            stream.WriteLine("{0}\t\t\treturn result;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
