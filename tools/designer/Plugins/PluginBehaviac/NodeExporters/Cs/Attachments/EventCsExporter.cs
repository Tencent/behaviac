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
using Behaviac.Design.Attachments;

namespace PluginBehaviac.NodeExporters
{
    public class EventCsExporter : AttachmentCsExporter
    {
        protected override bool ShouldGenerateClass()
        {
            return true;
        }

        protected override void GenerateConstructor(Attachment attachment, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(attachment, stream, indent, className);

            Event evt = attachment as Event;

            if (evt == null)
            {
                return;
            }

            string triggerMode = (evt.TriggerMode == TriggerMode.Transfer) ? "TriggerMode.TM_Transfer" : "TriggerMode.TM_Return";
            string triggeredOnce = evt.TriggeredOnce ? "true" : "false";

            string method = evt.Task.GetExportValue();
            method = method.Replace("\"", "\\\"");

            stream.WriteLine("{0}\t\t\tthis.Initialize(\"{1}\", \"{2}\", {3}, {4});",
                             indent, method, evt.ReferenceFilename, triggerMode, triggeredOnce);
        }

        protected override void GenerateMethod(Attachment attachment, StringWriter stream, string indent)
        {
            Event evt = attachment as Event;

            if (evt == null)
            {
                return;
            }

            stream.WriteLine("{0}\t\tpublic void Initialize(string eventName, string referencedBehavior, TriggerMode mode, bool once)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tthis.m_event = AgentMeta.ParseMethod(eventName, ref this.m_eventName);", indent);
            stream.WriteLine("{0}\t\t\tthis.m_referencedBehaviorPath = referencedBehavior;", indent);
            stream.WriteLine("{0}\t\t\tthis.m_triggerMode = mode;", indent);
            stream.WriteLine("{0}\t\t\tthis.m_bTriggeredOnce = once;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
