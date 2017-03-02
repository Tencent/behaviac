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
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public partial class ControlsDialog : Form
    {
        public ControlsDialog(bool bShowMeTip)
        {
            InitializeComponent();

            this.checkBoxNext.Visible = bShowMeTip;

            if (bShowMeTip)
            {
                this.checkBoxNext.Checked = !Settings.Default.ShowControlsOnStartUp;
            }

            initWorkspaces();
        }

        private void ControlsDialog_Load(object sender, EventArgs e)
        {
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);
            string controlFile = (Settings.Default.Language == (int)Language.English || System.Threading.Thread.CurrentThread.CurrentUICulture.Name != "zh-CN")
                                 ? "..\\doc\\ControlHelp.html" : "..\\doc\\ControlHelp.zh-CN.html";
            controlFile = Path.Combine(appDir, controlFile);
            controlFile = Path.GetFullPath(controlFile);

            if (File.Exists(controlFile))
            {
                webBrowser.Url = new Uri(controlFile);
            }
        }

        private Dictionary<string, string> _workspaces = new Dictionary<string,string>();

        private void initWorkspaces()
        {
#if DEBUG
            _workspaces.Add("C++ Unit Test", "../../../test/btunittest/BehaviacData/BehaviacUnitTestCpp.workspace.xml");
            _workspaces.Add("C++ Ship Demo", "../../../example/spaceship/data/ships.workspace.xml");
#endif
            _workspaces.Add("C++ Tutorial_1", "../../../tutorials/tutorial_1/workspace/tutorial_1_cpp.workspace.xml");
            _workspaces.Add("C++ Tutorial_1_1", "../../../tutorials/tutorial_1_1/workspace/tutorial_1_1_cpp.workspace.xml");
            _workspaces.Add("C++ Tutorial_2", "../../../tutorials/tutorial_2/workspace/tutorial_2_cpp.workspace.xml");
            _workspaces.Add("C++ Tutorial_3", "../../../tutorials/tutorial_3/workspace/tutorial_3_cpp.workspace.xml");
            _workspaces.Add("C++ Tutorial_5", "../../../tutorials/tutorial_5/workspace/tutorial_5_cpp.workspace.xml");
            _workspaces.Add("C# Tutorial_1", "../../../tutorials/tutorial_1/workspace/tutorial_1_cs.workspace.xml");
            _workspaces.Add("C# Tutorial_1_1", "../../../tutorials/tutorial_1_1/workspace/tutorial_1_1_cs.workspace.xml");
            _workspaces.Add("C# Tutorial_2", "../../../tutorials/tutorial_2/workspace/tutorial_2_cs.workspace.xml");
            _workspaces.Add("C# Tutorial_3", "../../../tutorials/tutorial_3/workspace/tutorial_3_cs.workspace.xml");
            _workspaces.Add("C# Tutorial_5", "../../../tutorials/tutorial_5/workspace/tutorial_5_cs.workspace.xml");
#if DEBUG
            _workspaces.Add("Unity Unit Test", "../../../integration/unity/Assets/behaviac/workspace/behaviacunittest.workspace.xml");
            _workspaces.Add("Unity Tank Demo", "../../../integration/BattleCityDemo/Assets/behaviac/workspace/BattleCity.workspace.xml");
#endif
            _workspaces.Add("Unity Tutorial_1", "../../../tutorials/tutorial_1/workspace/tutorial_1_unity.workspace.xml");

            foreach (string ws in _workspaces.Keys)
            {
                workspacesComboBox.Items.Add(ws);
            }
        }

        private void loadWorkspace(string wksFile)
        {
            try
            {
                wksFile = Path.Combine(Application.StartupPath, wksFile);
                wksFile = Path.GetFullPath(wksFile);
                MainWindow.Instance.BehaviorTreeList.OpenWorkspace(wksFile);
            }
            catch
            {
            }
        }

        private void webBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void ControlsDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ControlsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.checkBoxNext.Visible)
            {
                //don't show me next time
                Settings.Default.ShowControlsOnStartUp = !this.checkBoxNext.Checked;
            }
        }

        private void overviewButton_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.OpenOverViewURL();
        }

        private void tutorialsButton_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.OpenTutorialsURL();
        }

        private void workspacesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ws = (string)workspacesComboBox.SelectedItem;
            if (_workspaces.ContainsKey(ws))
            {
                loadWorkspace(_workspaces[ws]);
            }
        }
    }
}
