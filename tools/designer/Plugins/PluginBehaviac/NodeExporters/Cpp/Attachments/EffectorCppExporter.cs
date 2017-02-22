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
    public class EffectorCppExporter : AttachActionCppExporter
    {
        protected override void GenerateConstructor(Attachment attachment, StringWriter stream, string indent, string className)
        {
            base.GenerateConstructor(attachment, stream, indent, className);

            PluginBehaviac.Events.Effector effector = attachment as PluginBehaviac.Events.Effector;

            if (effector == null)
            {
                return;
            }

            string phase = "Effector::E_SUCCESS";

            switch (effector.Phase)
            {
                case EffectorPhase.Failure:
                    phase = "Effector::E_FAILURE";
                    break;

                case EffectorPhase.Both:
                    phase = "Effector::E_BOTH";
                    break;
            }

            stream.WriteLine("{0}\t\t\tthis->SetPhase({1});", indent, phase);
        }
    }
}
