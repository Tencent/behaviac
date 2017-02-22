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
    public class ConditionCsExporter : NodeCsExporter
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
                RightValueCsExporter.GenerateClassConstructor(node, condition.Opl, stream, indent, "opl");
            }

            if (condition.Opr != null)
            {
                RightValueCsExporter.GenerateClassConstructor(node, condition.Opr, stream, indent, "opr");
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
                RightValueCsExporter.GenerateClassMember(condition.Opl, stream, indent, "opl");
            }

            if (condition.Opr != null)
            {
                RightValueCsExporter.GenerateClassMember(condition.Opr, stream, indent, "opr");
            }
        }

        public static void GenerateOperand(DefaultObject defaultObj, StringWriter stream, string indent, RightValueDef operand, string operandName, string nodeName)
        {
            if (operand != null)
            {
                string typeName = DataCsExporter.GetGeneratedNativeType(operand.ValueType);
                typeName = typeName.Replace("::", ".");

                if (operand.IsMethod) // method
                {
                    RightValueCsExporter.GenerateCode(defaultObj, operand, stream, indent, typeName, operandName, string.Empty);
                    RightValueCsExporter.PostGenerateCode(operand, stream, indent, typeName, operandName, string.Empty);
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
                                string property = PropertyCsExporter.GetProperty(defaultObj, prop, var.ArrayIndexElement, stream, indent, operandName, nodeName);
                                string propName = prop.BasicName.Replace("[]", "");

                                if (prop.IsArrayElement && var.ArrayIndexElement != null)
                                {
                                    ParameterCsExporter.GenerateCode(defaultObj, var.ArrayIndexElement, stream, indent, "int", operandName + "_index", nodeName + "_opl");
                                    property = string.Format("({0})[{1}_index]", property, operandName);
                                }

                                stream.WriteLine("{0}{1} {2} = {3};", indent, typeName, operandName, property);
                            }
                        }
                        else if (var.IsConst) // const
                        {
                            RightValueCsExporter.GenerateCode(defaultObj, operand, stream, indent, typeName, operandName, string.Empty);
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

            stream.WriteLine("{0}\t\tprotected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);

            string typeName = DataCsExporter.GetGeneratedNativeType(condition.Opl.ValueType);

            // opl
            ConditionCsExporter.GenerateOperand(node, stream, indent + "\t\t\t", condition.Opl, "opl", "condition");

            // opr
            ConditionCsExporter.GenerateOperand(node, stream, indent + "\t\t\t", condition.Opr, "opr", "condition");

            // Operator
            switch (condition.Operator)
            {
                case OperatorType.Equal:
                    stream.WriteLine("{0}\t\t\tbool op = opl == opr;", indent);
                    break;

                case OperatorType.NotEqual:
                    stream.WriteLine("{0}\t\t\tbool op = opl != opr;", indent);
                    break;

                case OperatorType.Greater:
                    stream.WriteLine("{0}\t\t\tbool op = opl > opr;", indent);
                    break;

                case OperatorType.GreaterEqual:
                    stream.WriteLine("{0}\t\t\tbool op = opl >= opr;", indent);
                    break;

                case OperatorType.Less:
                    stream.WriteLine("{0}\t\t\tbool op = opl < opr;", indent);
                    break;

                case OperatorType.LessEqual:
                    stream.WriteLine("{0}\t\t\tbool op = opl <= opr;", indent);
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

            stream.WriteLine("{0}\t\t\treturn op ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
