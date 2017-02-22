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
using System.IO;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class FindFileDialog : Form
    {
        enum SortTypes
        {
            PathUp = 0,
            PathDown,
            SizeUp,
            SizeDown,
            TimeUp,
            TimeDown
        }

        struct PathUpComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x, y);
            }
        }

        struct PathDownComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(y, x);
            }
        }

        struct SizeUpComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return (int)((new FileInfo(x)).Length - (new FileInfo(y)).Length);
            }
        }

        struct SizeDownComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return (int)((new FileInfo(y)).Length - (new FileInfo(x)).Length);
            }
        }

        struct TimeUpComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return DateTime.Compare(File.GetLastWriteTime(x), File.GetLastWriteTime(y));
            }
        }

        struct TimeDownComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return DateTime.Compare(File.GetLastWriteTime(y), File.GetLastWriteTime(x));
            }
        }

        private static FindFileDialog _findFileDialog = null;
        private static string _lastFoundStr = string.Empty;
        private static string _lastSelectedStr = string.Empty;
        private static SortTypes _sortType = SortTypes.PathUp;

        private List<string> _filenames = new List<string>();

        internal static void Inspect()
        {
            if (_findFileDialog == null)
            {
                _findFileDialog = new FindFileDialog();
            }

            _findFileDialog.ShowDialog();
        }

        public FindFileDialog()
        {
            InitializeComponent();

            Workspace.WorkspaceChangedHandler += this.WorkspaceChanged;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            this.filenameTextBox.Text = _lastFoundStr;

            searchBehaviors();

            // select the last one
            foreach (DataGridViewRow row in this.filesGridView.Rows)
            {
                if (_lastSelectedStr == (string)row.Cells["filepathColumn"].Value)
                {
                    row.Selected = true;
                    break;
                }
            }

            // focus the filename textbox defaultly
            this.filenameTextBox.Focus();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // save the last selected row
            foreach (DataGridViewRow row in this.filesGridView.SelectedRows)
            {
                _lastSelectedStr = (string)row.Cells["filepathColumn"].Value;
                break;
            }
        }

        private void searchIncludeDirCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            searchBehaviors();
        }

        private void filenameTextBox_TextChanged(object sender, EventArgs e)
        {
            searchBehaviors();
        }

        private void filesGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            openSelectedFiles();
        }

        private void filesGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            switch (e.ColumnIndex)
            {
                case 0:
                    _sortType = (_sortType == SortTypes.PathUp) ? SortTypes.PathDown : SortTypes.PathUp;
                    break;

                case 1:
                    _sortType = (_sortType == SortTypes.SizeUp) ? SortTypes.SizeDown : SortTypes.SizeUp;
                    break;

                case 2:
                    _sortType = (_sortType == SortTypes.TimeUp) ? SortTypes.TimeDown : SortTypes.TimeUp;
                    break;
            }

            sortFiles();
        }

        private void filenameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                {
                    openSelectedFiles();
                    e.Handled = true;

                    break;
                }

                case Keys.Up:
                case Keys.Down:
                {
                    selectNextRow(e.KeyCode == Keys.Down);
                    e.Handled = true;

                    break;
                }
            }
        }

        private void filesGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                openSelectedFiles();
                e.Handled = true;
            }
        }

        private void WorkspaceChanged()
        {
            _lastFoundStr = string.Empty;
            _lastSelectedStr = string.Empty;
            _sortType = SortTypes.PathUp;
        }

        private void sortFiles()
        {
            try
            {
                switch (_sortType)
                {
                    case SortTypes.PathUp:
                        _filenames.Sort(new PathUpComparer());
                        break;

                    case SortTypes.PathDown:
                        _filenames.Sort(new PathDownComparer());
                        break;

                    case SortTypes.SizeUp:
                        _filenames.Sort(new SizeUpComparer());
                        break;

                    case SortTypes.SizeDown:
                        _filenames.Sort(new SizeDownComparer());
                        break;

                    case SortTypes.TimeUp:
                        _filenames.Sort(new TimeUpComparer());
                        break;

                    case SortTypes.TimeDown:
                        _filenames.Sort(new TimeDownComparer());
                        break;
                }

                this.filesGridView.Rows.Clear();
                this.filesGridView.RowCount = _filenames.Count;

            }
            catch
            {
            }
        }

        private void searchBehaviors()
        {
            _lastFoundStr = this.filenameTextBox.Text;

            try
            {
                _filenames.Clear();

                string[] files = Directory.GetFiles(Workspace.Current.SourceFolder, "*.xml", SearchOption.AllDirectories);

                if (string.IsNullOrEmpty(_lastFoundStr))
                {
                    _filenames.AddRange(files);

                }
                else
                {
                    string findStr = _lastFoundStr.ToLowerInvariant();
                    findStr = findStr.Replace("\\", "/");

                    foreach (string file in files)
                    {
                        if (this.searchIncludeDirCheckBox.Checked)
                        {
                            string relativeFile = Behaviac.Design.FileManagers.FileManager.MakeRelative(Workspace.Current.SourceFolder, file);
                            relativeFile = relativeFile.Replace("\\", "/");

                            if (relativeFile.ToLowerInvariant().Contains(findStr))
                            {
                                _filenames.Add(file);
                            }

                        }
                        else
                        {
                            string filename = Path.GetFileNameWithoutExtension(file);
                            filename = filename.Replace("\\", "/");

                            if (filename.ToLowerInvariant().Contains(findStr))
                            {
                                _filenames.Add(file);
                            }
                        }
                    }
                }

                sortFiles();

                this.fileCountLabel.Text = string.Format("{0}{1}", _filenames.Count, Resources.FileCountInfo);

            }
            catch
            {
            }
        }

        private void selectNextRow(bool down)
        {
            if (this.filesGridView.Rows.Count > 0)
            {
                int index = 0;

                foreach (DataGridViewRow row in this.filesGridView.SelectedRows)
                {
                    index = row.Index;

                    if (down && index < this.filesGridView.Rows.Count - 1)
                    {
                        index++;
                        break;

                    }
                    else if (!down && index > 0)
                    {
                        index--;
                        break;
                    }
                }

                if (index >= 0)
                {
                    DataGridViewRow row = this.filesGridView.Rows[index];
                    row.Selected = true;

                    if (!row.Displayed)
                    {
                        this.filesGridView.CurrentCell = row.Cells[0];
                    }
                }
            }
        }

        private void openSelectedFiles()
        {
            foreach (DataGridViewRow row in this.filesGridView.SelectedRows)
            {
                UIUtilities.ShowBehaviorTree((string)row.Cells["filepathColumn"].Value);

                Utilities.ReportLoadBehavior();
            }

            this.Close();
        }

        private void filesGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < _filenames.Count && e.ColumnIndex >= 0 && e.ColumnIndex < 3)
            {
                string filename = _filenames[e.RowIndex];

                switch (e.ColumnIndex)
                {
                    case 0:
                    {
                        string relativeFilename = FileManagers.FileManager.GetRelativePath(filename);

                        e.Value = relativeFilename;
                        //this.filesGridView.Rows[e.RowIndex].Cells[0].ToolTipText = filename;
                        break;
                    }

                    case 1:
                    {
                        string fileSize = string.Format("{0}K", ((new FileInfo(filename)).Length + 500) / 1000);

                        e.Value = fileSize;
                        break;
                    }

                    case 2:
                    {
                        string modifiedTime = File.GetLastWriteTime(filename).ToString();

                        e.Value = modifiedTime;
                        break;
                    }
                }
            }
        }
    }
}