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
    internal partial class ConsoleDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static ConsoleDock _consoleDock = null;

        internal static void Inspect()
        {
            if (_consoleDock == null)
            {
                _consoleDock = new ConsoleDock();
                _consoleDock.Show(MainWindow.Instance.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);

            }
            else
            {
                _consoleDock.Show();
            }
        }

        internal static void CloseAll()
        {
            Clear();

            if (_consoleDock != null)
            {
                _consoleDock.Close();
            }
        }

        internal static void Clear()
        {
            if (_consoleDock != null)
            {
                _consoleDock.logListView.Items.Clear();
            }
        }

        internal static void SetMesssages(int frame)
        {
            if (_consoleDock != null && frame >= 0)
            {
                try
                {
                    _consoleDock.logListView.Hide();

                    int endIndex = MessageQueue.Messages.Count;

                    if (Plugin.EditMode == EditModes.Connect)
                    {
                        endIndex = MessageQueue.CurrentIndex();

                    }
                    else
                    {
                        endIndex = MessageQueue.MessageStartIndex(frame + 1);
                    }

                    _consoleDock.logListView.VirtualListSize = (endIndex > 0) ? endIndex : MessageQueue.Messages.Count;

                    bool setTop = false;

                    if (Plugin.EditMode == EditModes.Analyze)
                    {
                        int startIndex = MessageQueue.MessageStartIndex(frame);

                        if (startIndex >= 0 && startIndex < _consoleDock.logListView.Items.Count)
                        {
                            _consoleDock.logListView.TopItem = _consoleDock.logListView.Items[startIndex];
                            setTop = false;
                        }
                    }

                    if (!setTop)
                    {
                        if (_consoleDock.logListView.Items.Count > 0)
                        {
                            _consoleDock.logListView.EnsureVisible(_consoleDock.logListView.Items.Count - 1);
                        }
                    }

                    _consoleDock.logListView.Show();

                }
                catch
                {
                }
            }
        }

        public ConsoleDock()
        {
            if (_consoleDock == null)
            {
                _consoleDock = this;
            }

            InitializeComponent();

            this.TabText = Resources.Output;
        }

        protected override void OnShown(EventArgs e)
        {
            this.limitCountCheckBox.Visible = (Plugin.EditMode == EditModes.Connect);
            this.limitCountCheckBox.Checked = Settings.Default.LimitLogCount;
            this.logCountLabel.Visible = (Plugin.EditMode == EditModes.Connect) && Settings.Default.LimitLogCount;
            this.logCountNumericUpDown.Visible = (Plugin.EditMode == EditModes.Connect) && Settings.Default.LimitLogCount;
            this.logCountNumericUpDown.Value = Settings.Default.MaxLogCount;

            base.OnShown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _consoleDock = null;

            base.OnClosed(e);
        }

        private void selectAllItems()
        {
            try
            {
                if (logListView.Items.Count > 0)
                {
                    logListView.Focus();
                    logListView.Items[0].Selected = true;
                    SendKeys.Send("+{END}");
                }

            }
            catch
            {
            }
        }

        private void copySelectedItems()
        {
            try
            {
                if (logListView.SelectedIndices.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (int index in logListView.SelectedIndices)
                    {
                        sb.Append(logListView.Items[index].SubItems[0].Text);
                    }

                    if (sb.Length > 0)
                    {
                        Clipboard.SetText(sb.ToString());
                    }
                }

            }
            catch
            {
            }
        }

        private void logListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    copyAllToClipboard();
                }
            }
        }

        private void logListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                string item = string.Empty;

                if (e.ItemIndex >= 0 && e.ItemIndex < MessageQueue.Messages.Count)
                {
                    item = MessageQueue.Messages[e.ItemIndex];
                }

                e.Item = new ListViewItem(item);

            }
            catch
            {
            }
        }

        private void logListView_SizeChanged(object sender, EventArgs e)
        {
            logColumnHeader.Width = logListView.Width - 25;
        }

        private bool isDragging = false;
        private Point startMousePos;

        private void logListView_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            startMousePos = e.Location;
        }

        private void logListView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (isDragging && logListView.Items.Count > 0)
                {
                    Point endMousePos = e.Location;
                    int y = Math.Min(startMousePos.Y, endMousePos.Y);
                    int height = Math.Abs(endMousePos.Y - startMousePos.Y);

                    ListViewItem hitItem = logListView.HitTest(endMousePos).Item;

                    if (hitItem != null)
                    {
                        hitItem.Selected = true;
                    }

                    foreach (int index in logListView.SelectedIndices)
                    {
                        ListViewItem item = logListView.Items[index];
                        int itemY = item.Position.Y;

                        if (itemY < y - item.Bounds.Height || itemY > y + height)
                        {
                            item.Selected = false;
                        }
                    }
                }

            }
            catch
            {
            }
        }

        private void logListView_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void limitCountCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.LimitLogCount = this.limitCountCheckBox.Checked;
            MessageQueue.LimitMessageCount = Settings.Default.LimitLogCount;

            this.logCountLabel.Visible = Settings.Default.LimitLogCount;
            this.logCountNumericUpDown.Visible = Settings.Default.LimitLogCount;
        }

        private void logCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.MaxLogCount = (int)this.logCountNumericUpDown.Value;
            MessageQueue.MaxMessageCount = Settings.Default.MaxLogCount;
        }

        private void buttonCopyAll_Click(object sender, EventArgs e)
        {
            copyAllToClipboard();
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            copyAllToClipboard();
        }

        private void copyAllToClipboard()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int index = 0; index < logListView.Items.Count; ++index)
                {
                    sb.Append(logListView.Items[index].SubItems[0].Text);
                }

                if (sb.Length > 0)
                {
                    Clipboard.SetText(sb.ToString());
                }
            }
            catch
            {
            }
        }
    }
}
