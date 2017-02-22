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
    public class ConditionCppExporter : NodeCppExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;
            return (condition != null);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;

            if (condition == null)
            {
                return;
            }

            if (condition.Opl != null)
            {
                RightValueCppExporter.GenerateClassConstructor(node, condition.Opl, stream, indent, "opl");
            }

            if (condition.Opr != null)
            {
                RightValueCppExporter.GenerateClassConstructor(node, condition.Opr, stream, indent, "opr");
            }
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;

            if (condition == null)
            {
                return;
            }

            if (condition.Opl != null)
            {
                RightValueCppExporter.GenerateClassMember(condition.Opl, stream, indent, "opl");
            }

            if (condition.Opr != null)
            {
                RightValueCppExporter.GenerateClassMember(condition.Opr, stream, indent, "opr");
            }
        }

        public static void GenerateOperand(DefaultObject defaltObj, StringWriter stream, string indent, RightValueDef operand, string operandName, string nodeName)
        {
            if (operand != null)
            {
                string typeName = DataCppExporter.GetGeneratedNativeType(operand.ValueType);

                if (operand.IsMethod) // method
                {
                    RightValueCppExporter.GenerateCode(defaltObj, operand, stream, indent, typeName, operandName, string.Empty);
                    RightValueCppExporter.PostGenerateCode(operand, stream, indent, typeName, operandName, string.Empty);
                }
                else
                {
                    VariableDef var = operand.Var;

                    if (var != null)
                    {
                        if (var.IsProperty) // property
                        {
                            PropertyDef prop = var.Property;

                            if (prop != null)
                            {
                                string property = PropertyCppExporter.GetProperty(defaltObj, prop, var.ArrayIndexElement, stream, indent, operandName, nodeName);
                                string propName = prop.BasicName.Replace("[]", "");

                                if (prop.IsArrayElement && var.ArrayIndexElement != null)
                                {
                                    ParameterCppExporter.GenerateCode(defaltObj, var.ArrayIndexElement, stream, indent, "int", operandName + "_index", nodeName + "_opl");
                                    property = string.Format("({0})[{1}_index]", property, operandName);
                                }

                                stream.WriteLine("{0}{1}& {2} = {3};", indent, typeName, operandName, property);
                            }
                        }
                        else if (var.IsConst) // const
                        {
                            RightValueCppExporter.GenerateCode(defaltObj, operand, stream, indent, typeName, operandName, string.Empty);
                        }
                    }
                }
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            PluginBehaviac.Nodes.Condition condition = node as PluginBehaviac.Nodes.Condition;

            if (condition == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tvirtual EBTStatus update_impl(Agent* pAgent, EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(pAgent);", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(childStatus);", indent);

            // opl
            ConditionCppExporter.GenerateOperand(node, stream, indent + "\t\t\t", condition.Opl, "opl", "condition");

            // opr
            ConditionCppExporter.GenerateOperand(node, stream, indent + "\t\t\t", condition.Opr, "opr", "condition");

            // Operator
            switch (condition.Operator)
            {
                case OperatorType.Equal:
                    stream.WriteLine("{0}\t\t\tbool op = PrivateDetails::Equal(opl, opr);", indent);
                    break;

                case OperatorType.NotEqual:
                    stream.WriteLine("{0}\t\t\tbool op = !PrivateDetails::Equal(opl, opr);", indent);
                    break;

                case OperatorType.Greater:
                    stream.WriteLine("{0}\t\t\tbool op = PrivateDetails::Greater(opl, opr);", indent);
                    break;

                case OperatorType.GreaterEqual:
                    stream.WriteLine("{0}\t\t\tbool op = PrivateDetails::GreaterEqual(opl, opr);", indent);
                    break;

                case OperatorType.Less:
                    stream.WriteLine("{0}\t\t\tbool op = PrivateDetails::Less(opl, opr);", indent);
                    break;

                case OperatorType.LessEqual:
                    stream.WriteLine("{0}\t\t\tbool op = PrivateDetails::LessEqual(opl, opr);", indent);
                    break;

                case OperatorType.And:
                    stream.WriteLine("{0}\t\t\tbool op = opl && opr;", indent);
                    break;

                case OperatorType.Or:
                    stream.WriteLine("{0}\t\t\tbool op = opl || opr;", indent);
                    break;

                default:
                    stream.WriteLine("{0}\t\t\tbool op = false;", indent);
                    break;
            }

            stream.WriteLine("{0}\t\t\treturn op ? BT_SUCCESS : BT_FAILURE;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
