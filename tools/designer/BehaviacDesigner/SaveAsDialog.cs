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
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public partial class SaveAsDialog : Form
    {
        public SaveAsDialog(bool isReferenceTree)
        {
            InitializeComponent();

            _isReferenceTree = isReferenceTree;
        }

        private string _filename = string.Empty;
        private string _folder = string.Empty;
        private bool _isReferenceTree = true;

        public string FileName
        {
            get
            {
                return _filename;
            }

            set
            {
                _filename = value;
                _folder = Path.GetDirectoryName(_filename);

                DirectoryInfo dirInfo = new DirectoryInfo(_folder);
                this.folderTextBox.Text = dirInfo.Name;

                this.nameTextBox.Text = Path.GetFileNameWithoutExtension(_filename);
            }
        }

        private void SaveAsDialog_Activated(object sender, EventArgs e)
        {
            this.nameTextBox.Focus();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.nameTextBox.Text))
            {
                return;
            }

            if (_isReferenceTree && !Plugin.IsValidFilename(this.nameTextBox.Text))
            {
                MessageBox.Show(Resources.FilenameWarning, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ext = Path.GetExtension(_filename);
            string filename = Path.Combine(_folder, this.nameTextBox.Text);
            filename = Path.ChangeExtension(filename, ext);

            if (!File.Exists(filename))
            {
                _filename = filename;

                this.DialogResult = DialogResult.OK;

                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void SaveAsDialog_VisibleChanged(object sender, EventArgs e)
        {
            this.nameTextBox.SelectAll();
            this.nameTextBox.Focus();
        }
    }
}
