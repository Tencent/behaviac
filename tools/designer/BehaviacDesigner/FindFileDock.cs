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
    internal partial class FindFileDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static FindFileDock _instance = null;

        internal static void Inspect(string findWhat, int findFileCount, List<ObjectPair> findObjects)
        {
            if (_instance != null)
            {
                _instance.Close();
                _instance = null;
            }

            _instance = new FindFileDock();
            _instance.Show(MainWindow.Instance.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);

            _instance.SetResults(findWhat, findFileCount, findObjects);
        }

        public FindFileDock()
        {
            if (_instance != null)
            {
                _instance.Close();
                _instance = null;
            }

            _instance = this;

            InitializeComponent();

            this.TabText = Resources.FindResults;
        }

        protected override void OnClosed(EventArgs e)
        {
            _instance = null;

            base.OnClosed(e);
        }

        private List<ObjectPair> _objects = null;
        private List<string> _results = new List<string>();

        private void SetResults(string findWhat, int findFileCount, List<ObjectPair> findObjects)
        {
            this._objects = findObjects;

            List<string> findFiles = new List<string>();
            this._results.Clear();

            foreach (ObjectPair objPair in this._objects)
            {
                if (!findFiles.Contains(objPair.Root.Behavior.Filename))
                {
                    findFiles.Add(objPair.Root.Behavior.Filename);
                }

                string result = "  " + FileManagers.FileManager.GetRelativePath(objPair.Root.Behavior.Filename);
                result += ":  " + objPair.Obj.Label + "[" + objPair.Obj.Id + "]";

                if (objPair.Obj is Nodes.Node)
                {
                    Nodes.Node node = (Nodes.Node)objPair.Obj;
                    string label = node.GenerateNewLabel();

                    if (!string.IsNullOrEmpty(label))
                    {
                        result += "  " + label;
                    }
                }

                this._results.Add(result);
            }

            this.resultLabel.Text = string.Format(Resources.FindResultsInfo, findWhat, this._objects.Count, findFiles.Count, findFileCount);

            this.resultListView.Items.Clear();
            this.resultListView.SelectedIndices.Clear();
            this.resultListView.VirtualListSize = this._results.Count;
        }

        private void selectAllItems()
        {
            try
            {
                if (this.resultListView.Items.Count > 0)
                {
                    this.resultListView.Focus();
                    this.resultListView.Items[0].Selected = true;
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
                if (this.resultListView.SelectedIndices.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (int index in this.resultListView.SelectedIndices)
                    {
                        sb.Append(this.resultListView.Items[index].SubItems[0].Text);
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

        private void showSelectedObject()
        {
            if (this._objects != null && this.resultListView.SelectedIndices.Count > 0)
            {
                foreach (int index in this.resultListView.SelectedIndices)
                {
                    if (index != _selectedIndex)
                    {
                        _selectedIndex = index;
                        break;
                    }
                }

                List<int> indexes = new List<int>();

                foreach (int index in this.resultListView.SelectedIndices)
                {
                    if (index != _selectedIndex)
                    {
                        indexes.Add(index);
                    }
                }

                foreach (int index in indexes)
                {
                    this.resultListView.SelectedIndices.Remove(index);
                }

                FindDock.ShowObject(this._objects[_selectedIndex]);
                this.resultListView.Select();
            }
        }

        private void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            this.selectAllItems();
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            this.copySelectedItems();
        }

        private void resultListView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (e.Control)
                    {
                        this.selectAllItems();
                    }

                    break;

                case Keys.C:
                    if (e.Control)
                    {
                        this.copySelectedItems();
                    }

                    break;

                case Keys.Enter:
                    showSelectedObject();
                    break;
            }
        }

        private int _selectedIndex = -1;
        private void resultListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                string item = string.Empty;

                if (e.ItemIndex >= 0 && e.ItemIndex < this._results.Count)
                {
                    item = this._results[e.ItemIndex];

                    if (_selectedIndex >= 0)
                    {
                        this.resultListView.SelectedIndices.Add(_selectedIndex);
                        _selectedIndex = -1;
                    }
                }

                e.Item = new ListViewItem(item);

            }
            catch
            {
            }
        }

        private void resultListView_SizeChanged(object sender, EventArgs e)
        {
            this.logColumnHeader.Width = this.resultListView.Width - 25;
        }

        private bool isDragging = false;
        private Point startMousePos;

        private void resultListView_MouseDown(object sender, MouseEventArgs e)
        {
            this.isDragging = true;
            this.startMousePos = e.Location;
        }

        private void resultListView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.isDragging && this.resultListView.Items.Count > 0)
                {
                    Point endMousePos = e.Location;
                    int y = Math.Min(this.startMousePos.Y, endMousePos.Y);
                    int height = Math.Abs(endMousePos.Y - startMousePos.Y);

                    ListViewItem hitItem = this.resultListView.HitTest(endMousePos).Item;

                    if (hitItem != null)
                    {
                        hitItem.Selected = true;
                    }

                    foreach (int index in this.resultListView.SelectedIndices)
                    {
                        ListViewItem item = this.resultListView.Items[index];
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

        private void resultListView_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void resultListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            showSelectedObject();
        }
    }
}
