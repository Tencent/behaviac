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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class ParametersDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static List<ParametersDock> _parameterDocks = new List<ParametersDock>();
        internal static IList<ParametersDock> Docks()
        {
            return _parameterDocks.AsReadOnly();
        }

        internal static void CloseAll()
        {
            ParametersDock[] docks = _parameterDocks.ToArray();

            foreach (ParametersDock dock in docks)
            {
                dock.Close();
            }

            _parameterDocks.Clear();
        }

        internal static void Inspect(AgentType agentType, string agentName, string agentFullName, FrameStatePool.PlanningState nodeState)
        {
            ParametersDock dock = findParametersDock(agentType, agentName);

            if (dock == null)
            {
                dock = new ParametersDock();
                dock.Show(MainWindow.Instance.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            }

            dock.InspectObject(agentType, agentName, agentFullName, nodeState);
        }

        internal static void Inspect(string agentFullName, FrameStatePool.PlanningState nodeState)
        {
            Debug.Check(!string.IsNullOrEmpty(agentFullName));

            string[] tokens = agentFullName.Split('#');
            string agentName = tokens[tokens.Length - 1];

            Debug.Check(tokens.Length == 2);

            // Agent
            if (tokens.Length > 1)
            {
                AgentType agentType = Plugin.GetAgentType(tokens[0]);

                Inspect(agentType, agentName, agentFullName, nodeState);
            }

            // Global
            else
            {
                Inspect(Plugin.GetInstanceAgentType(agentFullName, null, null), agentName, agentFullName, nodeState);
            }
        }

        internal static void SetProperty(BehaviorNode behavior, string agentTypename, string agentName, string valueName, string valueStr)
        {
            foreach (ParametersDock dock in _parameterDocks)
            {
                if (dock.AgentName == agentName)
                {
                    dock.setProperty(behavior, valueName, valueStr);
                    break;
                }
            }
        }

        private static ParametersDock findParametersDock(AgentType agentType, string agentName)
        {
            foreach (ParametersDock dock in _parameterDocks)
            {
                if (dock.AgentType == agentType && (string.IsNullOrEmpty(dock.AgentName) || dock.AgentName == agentName))
                {
                    return dock;
                }
            }

            return null;
        }

        public ParametersDock()
        {
            InitializeComponent();

            _parameterDocks.Add(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            _parameterDocks.Remove(this);

            base.OnClosed(e);
        }

        private string _agentName = string.Empty;
        private AgentType _agentType = null;

        private AgentType AgentType
        {
            get
            {
                return _agentType;
            }
        }

        private string AgentName
        {
            get
            {
                return _agentName;
            }
        }

        private void InspectObject(AgentType agentType, string agentName, string agentFullName, FrameStatePool.PlanningState nodeState)
        {
            Nodes.Node node = null;

            if (!string.IsNullOrEmpty(agentFullName))
            {
                int frame = AgentDataPool.CurrentFrame > -1 ? AgentDataPool.CurrentFrame : 0;
                string behaviorFilename = FrameStatePool.GetBehaviorFilename(agentFullName, frame);
                List<string> transitionIds = FrameStatePool.GetHighlightTransitionIds(agentFullName, frame, behaviorFilename);
                List<string> highlightNodeIds = FrameStatePool.GetHighlightNodeIds(agentFullName, frame, behaviorFilename);
                List<string> updatedNodeIds = FrameStatePool.GetUpdatedNodeIds(agentFullName, frame, behaviorFilename);
                Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> profileInfos = FrameStatePool.GetProfileInfos(frame, behaviorFilename);

                BehaviorNode behavior = UIUtilities.ShowBehaviorTree(agentFullName, frame, transitionIds, highlightNodeIds, updatedNodeIds, HighlightBreakPoint.Instance, profileInfos);
                node = behavior as Nodes.Node;
            }

            _agentType = agentType;
            _agentName = agentName;

            Hide();

            setText(agentType, agentName);

            if (nodeState != null)
            {
                foreach (string agentFullName1 in nodeState._agents.Keys)
                {
                    string[] tokens = agentFullName1.Split('#');
                    Debug.Check(tokens.Length == 2);
                    string at = tokens[0];
                    string an = tokens[1];

                    AgentType agentType1 = Plugin.GetAgentType(at);

                    ParametersDock dock = findParametersDock(agentType1, an);
                    if (dock != null)
                    {
                        dock.InspectObject(agentType1, agentFullName1);

                        dock.setProperty(nodeState, agentFullName1);
                    }
                }
            }
            else if (AgentDataPool.CurrentFrame > -1 && !string.IsNullOrEmpty(agentName))
            {
                ParametersDock dock = findParametersDock(agentType, agentName);
                if (dock != null)
                {
                    dock.InspectObject(agentType, agentFullName);

                    List<AgentDataPool.ValueMark> valueSet = AgentDataPool.GetValidValues(agentType, agentFullName, AgentDataPool.CurrentFrame);

                    foreach (AgentDataPool.ValueMark value in valueSet)
                    {
                        dock.setProperty(null, value.Name, value.Value);
                    }
                }
            }

            lostAnyFocus();
            Show();
        }

        public void InspectObject(AgentType agentType, string agentFullName)
        {
            this.parametersPanel.InspectObject(agentType, agentFullName);
        }

        private bool setProperty(BehaviorNode behavior, string valueName, string valueStr)
        {
            return parametersPanel.SetProperty(behavior, valueName, valueStr);
        }

        private void setProperty(FrameStatePool.PlanningState nodeState, string agentFullName)
        {
            parametersPanel.SetProperty(nodeState, agentFullName);
        }


        private void setText(AgentType agentType, string agentName)
        {
            // Par
            if (agentType == null)
            {
                Text = TabText = string.IsNullOrEmpty(agentName) ? Resources.Pars : string.Format(Resources.ParsOf, agentName);
            }

            // Global
            else if (Plugin.IsGlobalInstanceAgentType(agentType))
            {
                Text = TabText = string.Format(Resources.PropertiesOf, agentType.ToString());
            }

            // Agent
            else
            {
                Text = TabText = string.Format(Resources.PropertiesOf + "::{1}", agentType.ToString(), agentName);
            }
        }

        private AgentType getAgentType(string typeName)
        {
            foreach (AgentType agentType in Plugin.AgentTypes)
            {
                if (agentType.ToString() == typeName)
                {
                    return agentType;
                }
            }

            return null;
        }

        private void lostAnyFocus()
        {
            this.Enabled = false;
            this.Enabled = true;
        }

        private void ParametersDock_Click(object sender, EventArgs e)
        {
            lostAnyFocus();
        }
    }
}
