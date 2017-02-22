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
    public class ActionCppExporter : NodeCppExporter
    {
        protected override bool ShouldGenerateClass(Node node)
        {
            Action action = node as Action;
            return (action != null);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            Action action = node as Action;

            if (action == null)
            {
                return;
            }

            if (action.Method != null)
            {
                MethodCppExporter.GenerateClassConstructor(node, action.Method, stream, indent, "method");
            }
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            Action action = node as Action;

            if (action == null)
            {
                return;
            }

            if (action.Method != null)
            {
                MethodCppExporter.GenerateClassMember(action.Method, stream, indent, "method");
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            Action action = node as Action;

            if (action == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tvirtual EBTStatus update_impl(Agent* pAgent, EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(pAgent);", indent);
            stream.WriteLine("{0}\t\t\tBEHAVIAC_UNUSED_VAR(childStatus);", indent);

            string resultStatus = "BT_SUCCESS";

            if (action.Method != null)
            {
                string method = MethodCppExporter.GenerateCode(node, action.Method, stream, indent + "\t\t\t", string.Empty, string.Empty, "method");

                if ("behaviac::EBTStatus" == action.Method.NativeReturnType)
                {
                    stream.WriteLine("{0}\t\t\t{1} result = {2};", indent, action.Method.NativeReturnType, method);
                    resultStatus = "result";

                    MethodCppExporter.PostGenerateCode(action.Method, stream, indent + "\t\t\t", string.Empty, string.Empty, "method");
                }
                else
                {
                    if (("void" == action.Method.NativeReturnType) || (EBTStatus.BT_INVALID != action.ResultOption) || action.ResultFunctor == null)
                    {
                        stream.WriteLine("{0}\t\t\t{1};", indent, method);
                    }
                    else
                    {
                        stream.WriteLine("{0}\t\t\t{1} result = {2};", indent, action.Method.NativeReturnType, method);
                    }

                    MethodCppExporter.PostGenerateCode(action.Method, stream, indent + "\t\t\t", string.Empty, string.Empty, "method");

                    if (EBTStatus.BT_INVALID != action.ResultOption)
                    {
                        resultStatus = action.ResultOption.ToString();
                    }
                    else if (Plugin.IsMatchedStatusMethod(action.Method, action.ResultFunctor))
                    {
                        if ("void" == action.Method.NativeReturnType)
                        {
                            resultStatus = MethodCppExporter.GenerateCode(node, action.ResultFunctor, stream, indent + "\t\t\t", string.Empty, string.Empty, "functor");
                        }
                        else
                        {
                            string agentName = "pAgent";

                            if (action.ResultFunctor.Owner != VariableDef.kSelf)
                            {
                                agentName = "pAgent_functor";

                                stream.WriteLine("{0}Agent* {1} = Agent::GetInstance(pAgent, \"{2}\");", indent, agentName, action.ResultFunctor.Owner);
                                stream.WriteLine("{0}BEHAVIAC_ASSERT({1});", indent, agentName);
                            }

                            resultStatus = string.Format("(({0}*){1})->{2}(result)", action.ResultFunctor.ClassName, agentName, action.ResultFunctor.Name);
                        }
                    }
                }
            }

            stream.WriteLine("{0}\t\t\treturn {1};", indent, resultStatus);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
