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
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class CallStackDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static CallStackDock _dock = null;
        private static bool _shouldScroll = true;

        internal static void Inspect()
        {
            if (_dock == null)
            {
                _dock = new CallStackDock();
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
                _dock.callstackListBox.Items.Clear();
            }
        }

        internal static void WriteLine(string log)
        {
            if (_dock != null)
            {
                _dock.callstackListBox.BeginUpdate();

                _dock.callstackListBox.Items.Add(log);

                if (_shouldScroll)
                {
                    scrollToEnd();
                }

                _dock.callstackListBox.EndUpdate();
            }
        }

        private static void scrollToEnd()
        {
            //_dock.errorListBox.SelectedIndex = _dock.errorListBox.Items.Count - 1;
            _dock.callstackListBox.TopIndex = _dock.callstackListBox.Items.Count - 1;
            _shouldScroll = true;
        }

        public CallStackDock()
        {
            if (_dock == null)
            {
                _dock = this;
            }

            InitializeComponent();

            this.TabText = Resources.Callstack;
            FrameStatePool.UpdateStack += UpdateStackCb;
        }

        protected override void OnClosed(EventArgs e)
        {
            _dock = null;

            FrameStatePool.UpdateStack -= UpdateStackCb;
            base.OnClosed(e);
        }

        private void UpdateStackCb(string tree, bool bAdd)
        {
            if (bAdd)
            {
                callstackListBox.Items.Insert(0, tree);

            }
            else
            {
                //Debug.Check(callstackListBox.Items.Count < 2 || ((string)callstackListBox.Items[0]) == tree);
                if (callstackListBox.Items.Count > 0 && ((string)callstackListBox.Items[0]) == tree)
                {
                    callstackListBox.Items.RemoveAt(0);
                }
            }

            if (callstackListBox.Items.Count > 0)
            {
                //callstackListBox.SelectedIndex = -1;
                callstackListBox.SelectedIndex = 0;

                string currentBt = (string)callstackListBox.Items[0];
                UIUtilities.ShowBehaviorTree(currentBt);
            }
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            if (callstackListBox.SelectedIndices.Count > 0)
            {
                string log = string.Empty;

                foreach (int index in callstackListBox.SelectedIndices)
                {
                    log += callstackListBox.Items[index];
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
            _shouldScroll = (callstackListBox.SelectedIndex == callstackListBox.Items.Count - 1);
        }

        private void logListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.End)
            {
                _shouldScroll = true;

            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                callstackListBox.BeginUpdate();
                SelectionMode selectionMode = callstackListBox.SelectionMode;
                callstackListBox.SelectionMode = SelectionMode.MultiSimple;

                callstackListBox.SelectedIndices.Clear();

                for (int i = 0; i < callstackListBox.Items.Count; ++i)
                {
                    callstackListBox.SelectedIndices.Add(i);
                }

                callstackListBox.SelectionMode = selectionMode;
                callstackListBox.EndUpdate();
            }
        }
    }
}
