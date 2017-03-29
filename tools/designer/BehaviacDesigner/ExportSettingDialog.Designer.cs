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

namespace Behaviac.Design
{
    partial class ExportSettingDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportSettingDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.okButton = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.includedFilesGridView = new System.Windows.Forms.DataGridView();
            this.filenameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.browseColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.removeFilenameButton = new System.Windows.Forms.Button();
            this.addFilenameButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.autoSetRelativePathCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.includedFilesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.okButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "VSFolder_closed.bmp");
            this.imageList.Images.SetKeyName(1, "DocumentHS.png");
            // 
            // includedFilesGridView
            // 
            this.includedFilesGridView.AllowUserToAddRows = false;
            this.includedFilesGridView.AllowUserToDeleteRows = false;
            this.includedFilesGridView.AllowUserToResizeRows = false;
            resources.ApplyResources(this.includedFilesGridView, "includedFilesGridView");
            this.includedFilesGridView.BackgroundColor = System.Drawing.SystemColors.WindowFrame;
            this.includedFilesGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.includedFilesGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.includedFilesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.includedFilesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.filenameColumn,
            this.browseColumn});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.includedFilesGridView.DefaultCellStyle = dataGridViewCellStyle6;
            this.includedFilesGridView.MultiSelect = false;
            this.includedFilesGridView.Name = "includedFilesGridView";
            this.includedFilesGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.includedFilesGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.includedFilesGridView.RowHeadersVisible = false;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.includedFilesGridView.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.includedFilesGridView.RowTemplate.Height = 23;
            this.includedFilesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.includedFilesGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.includedFilesGridView_CellContentClick);
            // 
            // filenameColumn
            // 
            this.filenameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.filenameColumn, "filenameColumn");
            this.filenameColumn.Name = "filenameColumn";
            // 
            // browseColumn
            // 
            resources.ApplyResources(this.browseColumn, "browseColumn");
            this.browseColumn.Name = "browseColumn";
            this.browseColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.browseColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // removeFilenameButton
            // 
            resources.ApplyResources(this.removeFilenameButton, "removeFilenameButton");
            this.removeFilenameButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.removeFilenameButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.removeFilenameButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.removeFilenameButton.Name = "removeFilenameButton";
            this.removeFilenameButton.UseVisualStyleBackColor = false;
            this.removeFilenameButton.Click += new System.EventHandler(this.removeFilenameButton_Click);
            // 
            // addFilenameButton
            // 
            resources.ApplyResources(this.addFilenameButton, "addFilenameButton");
            this.addFilenameButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.addFilenameButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.addFilenameButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.addFilenameButton.Name = "addFilenameButton";
            this.addFilenameButton.UseVisualStyleBackColor = false;
            this.addFilenameButton.Click += new System.EventHandler(this.addFilenameButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // autoSetRelativePathCheckBox
            // 
            resources.ApplyResources(this.autoSetRelativePathCheckBox, "autoSetRelativePathCheckBox");
            this.autoSetRelativePathCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.autoSetRelativePathCheckBox.Checked = true;
            this.autoSetRelativePathCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoSetRelativePathCheckBox.Name = "autoSetRelativePathCheckBox";
            this.autoSetRelativePathCheckBox.UseVisualStyleBackColor = false;
            // 
            // ExportSettingDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.autoSetRelativePathCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.removeFilenameButton);
            this.Controls.Add(this.addFilenameButton);
            this.Controls.Add(this.includedFilesGridView);
            this.Controls.Add(this.okButton);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportSettingDialog";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.includedFilesGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.DataGridView includedFilesGridView;
        private System.Windows.Forms.Button removeFilenameButton;
        private System.Windows.Forms.Button addFilenameButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn filenameColumn;
        private System.Windows.Forms.DataGridViewButtonColumn browseColumn;
        private System.Windows.Forms.CheckBox autoSetRelativePathCheckBox;
    }
}