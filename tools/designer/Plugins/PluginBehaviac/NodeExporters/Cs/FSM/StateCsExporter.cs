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
    public class StateCsExporter : NodeCsExporter
    {
        private bool isNullMethod(MethodDef method)
        {
            return (method != null && method.BasicName == "null_method");
        }

        private string getResultOptionStr(EBTStatus status)
        {
            switch (status)
            {
                case EBTStatus.BT_SUCCESS:
                    return "EBTStatus.BT_SUCCESS";

                case EBTStatus.BT_FAILURE:
                    return "EBTStatus.BT_FAILURE";

                case EBTStatus.BT_RUNNING:
                    return "EBTStatus.BT_RUNNING";
            }

            return "EBTStatus.BT_INVALID";
        }

        protected override bool ShouldGenerateClass(Node node)
        {
            State state = node as State;
            return (state != null);
        }

        protected override void GenerateConstructor(Node node, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(node, stream, indent, className);

            State state = node as State;

            if (state == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\t\tthis.m_bIsEndState = {1};", indent, state.IsEndState ? "true" : "false");

            if (state.Method != null && !isNullMethod(state.Method))
            {
                MethodCsExporter.GenerateClassConstructor(node, state.Method, stream, indent, "method");
            }
        }

        protected override void GenerateMember(Node node, StringWriter stream, string indent)
        {
            base.GenerateMember(node, stream, indent);

            State state = node as State;

            if (state == null)
            {
                return;
            }

            if (state.Method != null && !isNullMethod(state.Method))
            {
                MethodCsExporter.GenerateClassMember(state.Method, stream, indent, "method");
            }
        }

        protected override void GenerateMethod(Node node, StringWriter stream, string indent)
        {
            base.GenerateMethod(node, stream, indent);

            State state = node as State;

            if (state == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tprotected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);

            if (state.Method != null && !isNullMethod(state.Method))
            {
                string method = MethodCsExporter.GenerateCode(node, state.Method, stream, indent + "\t\t\t", string.Empty, string.Empty, "method");
                stream.WriteLine("{0}\t\t\t{1};", indent, method);

                MethodCsExporter.PostGenerateCode(state.Method, stream, indent + "\t\t\t", string.Empty, string.Empty, "method");
            }

            stream.WriteLine("{0}\t\t\treturn behaviac.EBTStatus.BT_RUNNING;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
