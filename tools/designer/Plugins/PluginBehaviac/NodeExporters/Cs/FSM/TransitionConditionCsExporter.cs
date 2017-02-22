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
using Behaviac.Design.Attachments;
using PluginBehaviac.DataExporters;
using PluginBehaviac.Events;

namespace PluginBehaviac.NodeExporters
{
    public class TransitionConditionCsExporter : AttachmentCsExporter
    {
        protected override bool ShouldGenerateClass()
        {
            return true;
        }

        protected override void GenerateConstructor(Attachment attachment, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(attachment, stream, indent, className);

            TransitionCondition transition = attachment as TransitionCondition;

            if (transition == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\t\tthis.TargetStateId = {1};", indent, transition.TargetFSMNodeId);

            if (transition.Opl != null)
            {
                RightValueCsExporter.GenerateClassConstructor(attachment, transition.Opl, stream, indent, "opl");
            }

            if (transition.Opr2 != null)
            {
                RightValueCsExporter.GenerateClassConstructor(attachment, transition.Opr2, stream, indent, "opr2");
            }
        }

        protected override void GenerateMember(Attachment attachment, StringWriter stream, string indent)
        {
            base.GenerateMember(attachment, stream, indent);

            TransitionCondition transition = attachment as TransitionCondition;

            if (transition == null)
            {
                return;
            }

            if (transition.Opl != null)
            {
                RightValueCsExporter.GenerateClassMember(transition.Opl, stream, indent, "opl");
            }

            if (transition.Opr2 != null)
            {
                RightValueCsExporter.GenerateClassMember(transition.Opr2, stream, indent, "opr2");
            }
        }

        protected override void GenerateMethod(Behaviac.Design.Attachments.Attachment attachment, StringWriter stream, string indent)
        {
            base.GenerateMethod(attachment, stream, indent);

            TransitionCondition transition = attachment as TransitionCondition;

            if (transition == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tprotected override EBTStatus update_impl(behaviac.Agent pAgent, behaviac.EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tEBTStatus result = EBTStatus.BT_SUCCESS;", indent);

            {
                string typeName = Plugin.GetNativeTypeName(transition.Opl.ValueType);
                typeName = typeName.Replace("::", ".");

                RightValueCsExporter.GenerateCode(attachment, transition.Opl, stream, indent + "\t\t\t", typeName, "opl", "");
                RightValueCsExporter.GenerateCode(attachment, transition.Opr2, stream, indent + "\t\t\t", typeName, "opr2", "");

                if (transition.Opl != null && transition.Opl.IsMethod)
                {
                    RightValueCsExporter.PostGenerateCode(transition.Opl, stream, indent + "\t\t\t", typeName, "opl", "");
                }

                if (transition.Opr2 != null && transition.Opr2.IsMethod)
                {
                    RightValueCsExporter.PostGenerateCode(transition.Opr2, stream, indent + "\t\t\t", typeName, "opr2", "");
                }

                switch (transition.Operator)
                {
                    case OperatorTypes.Equal:
                        stream.WriteLine("{0}\t\t\tbool op = (opl == opr2);", indent);
                        break;

                    case OperatorTypes.NotEqual:
                        stream.WriteLine("{0}\t\t\tbool op = (opl != opr2);", indent);
                        break;

                    case OperatorTypes.Greater:
                        stream.WriteLine("{0}\t\t\tbool op = (opl > opr2);", indent);
                        break;

                    case OperatorTypes.GreaterEqual:
                        stream.WriteLine("{0}\t\t\tbool op = (opl >= opr2);", indent);
                        break;

                    case OperatorTypes.Less:
                        stream.WriteLine("{0}\t\t\tbool op = (opl < opr2);", indent);
                        break;

                    case OperatorTypes.LessEqual:
                        stream.WriteLine("{0}\t\t\tbool op = (opl <= opr2);", indent);
                        break;

                    default:
                        stream.WriteLine("{0}\t\t\tbool op = false;", indent);
                        break;
                }
            }

            stream.WriteLine("{0}\t\t\tif (!op)", indent);
            stream.WriteLine("{0}\t\t\t\tresult = EBTStatus.BT_FAILURE;", indent);

            stream.WriteLine("{0}\t\t\treturn result;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
