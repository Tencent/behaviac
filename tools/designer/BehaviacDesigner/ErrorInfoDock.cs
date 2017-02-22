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
    internal partial class ErrorInfoDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static ErrorInfoDock _dock = null;
        private static bool _shouldScroll = true;

        internal static void Inspect()
        {
            if (_dock == null)
            {
                _dock = new ErrorInfoDock();

                _dock.Show(MainWindow.Instance.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);

            }
            else
            {
                _dock.Show();
            }
        }

        internal static void CloseAll()
        {
            if (_dock != null)
            {
                _dock.Close();
            }
        }

        internal static void Clear()
        {
            if (_dock != null)
            {
                _dock.errorListBox.Items.Clear();
            }
        }

        internal static void WriteLineWithTime(string log)
        {
            string dt = DateTime.Now.ToString();
            string msg = string.Format("[{0}] {1}", dt, log);
            WriteLine(msg);
        }

        internal static void WriteLine(string log)
        {
            if (_dock != null && log != null)
            {
                _dock.errorListBox.BeginUpdate();

                string[] lines = log.Split('\n');

                foreach (string line in lines)
                {
                    _dock.errorListBox.Items.Add(line);
                }

                if (_shouldScroll)
                {
                    scrollToEnd();
                }

                _dock.errorListBox.EndUpdate();
            }
        }

        public static void WriteExportTypeInfo()
        {
            string exportPath = Workspace.Current.GetExportAbsoluteFolder(Workspace.Current.Language);
            exportPath = Path.Combine(exportPath, "behaviac_generated/types");
            exportPath = Path.GetFullPath(exportPath);

            string msg = string.Format(Resources.ExportMessages, exportPath);

            ErrorInfoDock.Inspect();
            ErrorInfoDock.WriteLineWithTime(msg);
        }

        private static void scrollToEnd()
        {
            //_dock.errorListBox.SelectedIndex = _dock.errorListBox.Items.Count - 1;
            _dock.errorListBox.TopIndex = _dock.errorListBox.Items.Count - 1;
            _shouldScroll = true;
        }

        public ErrorInfoDock()
        {
            if (_dock == null)
            {
                _dock = this;
            }

            InitializeComponent();

            this.TabText = Resources.ErrorList;

            FrameStatePool.AddLogHandler += FrameStatePool_AddLogHandler;
        }

        protected override void OnClosed(EventArgs e)
        {
            _dock = null;

            FrameStatePool.AddLogHandler -= FrameStatePool_AddLogHandler;

            base.OnClosed(e);
        }

        private void FrameStatePool_AddLogHandler(int frame, string log)
        {
            errorListBox.Items.Add(log);

            if (_shouldScroll)
            {
                scrollToEnd();
            }
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            if (errorListBox.SelectedIndices.Count > 0)
            {
                string log = string.Empty;

                foreach (int index in errorListBox.SelectedIndices)
                {
                    log += errorListBox.Items[index];
                }

                if (!string.IsNullOrEmpty(log))
                {
                    Clipboard.SetText(log);
                }
            }
        }

        private void clearAllMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void logListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _shouldScroll = (errorListBox.SelectedIndex == errorListBox.Items.Count - 1);
        }

        private void logListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.End)
            {
                _shouldScroll = true;

            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                errorListBox.BeginUpdate();
                SelectionMode selectionMode = errorListBox.SelectionMode;
                errorListBox.SelectionMode = SelectionMode.MultiSimple;

                errorListBox.SelectedIndices.Clear();

                for (int i = 0; i < errorListBox.Items.Count; ++i)
                {
                    errorListBox.SelectedIndices.Add(i);
                }

                errorListBox.SelectionMode = selectionMode;
                errorListBox.EndUpdate();
            }
        }
    }
}
