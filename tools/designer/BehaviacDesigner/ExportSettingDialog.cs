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
    internal partial class ExportSettingDialog : Form
    {
        private ExporterInfo _exporterInfo = null;

        public ExportSettingDialog(ExporterInfo exporterInfo)
        {
            InitializeComponent();

            Debug.Check(exporterInfo != null);
            _exporterInfo = exporterInfo;

            if (_exporterInfo != null)
            {
                bool enableIncludedFiles = (_exporterInfo.ID == "cpp");
                this.includedFilesGridView.Visible = enableIncludedFiles;
                this.addFilenameButton.Visible = enableIncludedFiles;
                this.removeFilenameButton.Visible = enableIncludedFiles;

                string[] tokens = _exporterInfo.Description.Split(' ');
                this.Text = tokens[0] + " " + this.Text;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            loadSettings();
        }

        private void loadSettings()
        {
            if (Workspace.Current != null && _exporterInfo != null)
            {
                string exportFullPath = Workspace.Current.GetExportAbsoluteFolder(_exporterInfo.ID);
                List<string> exportIncludedFilenames = Workspace.Current.GetExportIncludedFilenames(_exporterInfo.ID);

                foreach (string filename in exportIncludedFilenames)
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        string includedFilename = FileManagers.FileManager.MakeAbsolute(exportFullPath, filename);
                        addFilename(includedFilename);
                    }
                }

                autoSetRelativePathCheckBox.Checked = Workspace.Current.UseRelativePath;
            }
        }

        private void saveSettings()
        {
            if (Workspace.Current != null && _exporterInfo != null)
            {
                string wsFilename = Workspace.Current.FileName;
                wsFilename = wsFilename.Replace('/', '\\');

                string exportFullPath = Workspace.Current.GetExportAbsoluteFolder(_exporterInfo.ID);
                string exportFolder = exportFullPath;
                exportFolder = Workspace.MakeRelative(exportFolder, wsFilename, true, true);
                exportFolder = exportFolder.Replace('\\', '/');

                List<string> exportIncludedFilenames = new List<string>();

                foreach (DataGridViewRow row in this.includedFilesGridView.Rows)
                {
                    string filename = (string)row.Cells["filenameColumn"].Value;
                    filename = filename.Replace('/', '\\');
                    filename = Workspace.MakeRelative(filename, exportFullPath, true, true);
                    filename = filename.Replace('\\', '/');
                    exportIncludedFilenames.Add(filename);
                }

                Workspace.Current.SetIncludedFilenames(_exporterInfo.ID, exportIncludedFilenames);

                Workspace.Current.UseRelativePath = autoSetRelativePathCheckBox.Checked;

                Workspace.SaveWorkspaceFile(Workspace.Current);
            }
        }

        private int getFilenameRowIndex(string filename)
        {
            for (int index = 0; index < this.includedFilesGridView.Rows.Count; index++)
            {
                DataGridViewRow row = this.includedFilesGridView.Rows[index];

                if (filename == (string)row.Cells["filenameColumn"].Value)
                {
                    return index;
                }
            }

            return -1;
        }

        private string getBrowseFilename(string pdbLoc)
        {
            using(OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Included File";
                dlg.Filter = "C++ Header Files(*.h)|*.h";
                dlg.Multiselect = false;

                if (DialogResult.OK == dlg.ShowDialog())
                {
                    return dlg.FileName;
                }
            }

            return string.Empty;
        }

        private void addFilename(string filename)
        {
            int index = this.getFilenameRowIndex(filename);

            if (index < 0)
            {
                index = this.includedFilesGridView.Rows.Add();
            }

            DataGridViewRow row = this.includedFilesGridView.Rows[index];
            row.Cells["filenameColumn"].Value = filename;
            row.Cells["browseColumn"].Value = "...";
            row.Selected = true;
        }

        private void removeSelectedRows(DataGridView dataGridView)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            List<DataGridViewRow> removingRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                removingRows.Add(row);
            }

            foreach (DataGridViewRow row in removingRows)
            {
                dataGridView.Rows.Remove(row);
            }
        }

        private void addFilenameButton_Click(object sender, EventArgs e)
        {
            this.addFilename("");
        }

        private void removeFilenameButton_Click(object sender, EventArgs e)
        {
            removeSelectedRows(this.includedFilesGridView);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void includedFilesGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            // Browse
            if (e.ColumnIndex == 1)
            {
                DataGridViewRow row = this.includedFilesGridView.Rows[e.RowIndex];
                string filename = getBrowseFilename((string)row.Cells["filenameColumn"].Value);

                if (!string.IsNullOrEmpty(filename))
                {
                    row.Cells["filenameColumn"].Value = filename;
                }
            }
        }
    }
}