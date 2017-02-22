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
using System.ComponentModel;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            languageComboBox.SelectedIndex = Settings.Default.Language;
            themeComboBox.SelectedIndex = Settings.Default.ColorTheme;
            nodeToolTipsCheckBox.Checked = Settings.Default.ShowNodeToolTips;
            showControlsCheckBox.Checked = Settings.Default.ShowControlsOnStartUp;
            checkBoxChecktheLatest.Checked = Settings.Default.CheckLatestVersion;
            dumpConnectDataCheckBox.Checked = Settings.Default.DumpConnectData;
            showVersionCheckBox.Checked = Settings.Default.ShowVersionInfo;
            useBasicDisplayNameCheckBox.Checked = Settings.Default.UseBasicDisplayName;
            showProfileCheckBox.Checked = Settings.Default.ShowProfilingInfo;
            checkBoxTweatAsError.Checked = Settings.Default.NoResultTreatAsError;
            limitDisplayLengthCheckBox.Checked = Settings.Default.IsDisplayLengthLimited;
            displayLengthNumericUpDown.Value = (decimal)Settings.Default.LimitedDisplayLength;
            concurrentProcessBehaviorsCheckBox.Checked = Settings.Default.ConcurrentProcessBehaviors;

            base.OnShown(e);
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {
            bool themeChanged = (Settings.Default.ColorTheme != themeComboBox.SelectedIndex);
            bool basicDisplayNameChanged = (Settings.Default.UseBasicDisplayName != useBasicDisplayNameCheckBox.Checked);
            bool showVersionChanged = (Settings.Default.ShowVersionInfo != showVersionCheckBox.Checked);
            bool limitDisplayLengthChanged = (Settings.Default.IsDisplayLengthLimited != limitDisplayLengthCheckBox.Checked) ||
                                             (Settings.Default.LimitedDisplayLength != (int)displayLengthNumericUpDown.Value);
            bool languageChanged = (Settings.Default.Language != languageComboBox.SelectedIndex);

            Settings.Default.Language = languageComboBox.SelectedIndex;
            Settings.Default.ColorTheme = themeComboBox.SelectedIndex;
            Settings.Default.ShowNodeToolTips = nodeToolTipsCheckBox.Checked;
            Settings.Default.ShowControlsOnStartUp = showControlsCheckBox.Checked;
            Settings.Default.CheckLatestVersion = checkBoxChecktheLatest.Checked;
            Settings.Default.DumpConnectData = dumpConnectDataCheckBox.Checked;
            Settings.Default.ShowVersionInfo = showVersionCheckBox.Checked;
            Settings.Default.UseBasicDisplayName = useBasicDisplayNameCheckBox.Checked;
            Settings.Default.NoResultTreatAsError = checkBoxTweatAsError.Checked;
            Settings.Default.IsDisplayLengthLimited = limitDisplayLengthCheckBox.Checked;
            Settings.Default.LimitedDisplayLength = (int)displayLengthNumericUpDown.Value;
            Settings.Default.ConcurrentProcessBehaviors = concurrentProcessBehaviorsCheckBox.Checked;

            Nodes.Node.ColorTheme = (Nodes.Node.ColorThemes)Settings.Default.ColorTheme;
            Behaviac.Design.Nodes.Action.NoResultTreatAsError = Settings.Default.NoResultTreatAsError;

            if (Settings.Default.ShowProfilingInfo != showProfileCheckBox.Checked)
            {
                Settings.Default.ShowProfilingInfo = showProfileCheckBox.Checked;
                Network.NetworkManager.Instance.SendProfiling(Settings.Default.ShowProfilingInfo);
            }

            Plugin.UseBasicDisplayName = Settings.Default.UseBasicDisplayName;
            //Plugin.ConcurrentProcessBehaviors = Settings.Default.ConcurrentProcessBehaviors;

            NodeViewData.ShowNodeId = Settings.Default.ShowVersionInfo;
            NodeViewData.IsDisplayLengthLimited = Settings.Default.IsDisplayLengthLimited;
            NodeViewData.LimitedDisplayLength = Settings.Default.LimitedDisplayLength;

            if (themeChanged || basicDisplayNameChanged || showVersionChanged || limitDisplayLengthChanged)
            {
                BehaviorTreeViewDock.RefreshAll();
                PropertiesDock.UpdatePropertyGrids();
            }

            if (languageChanged)
            {
                MessageBox.Show(Resources.LanguageChangedWarning, Resources.Warning, MessageBoxButtons.OK);
                //MainWindow.Instance.ReloadLayout();
            }
        }

        private void resetLayoutButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(Resources.LayoutResetWarning, Resources.Warning, MessageBoxButtons.YesNo))
            {
                MainWindow.Instance.ResetLayout();
            }
        }
    }
}