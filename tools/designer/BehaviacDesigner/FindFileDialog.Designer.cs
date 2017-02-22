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
    partial class FindFileDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindFileDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.filesGridView = new System.Windows.Forms.DataGridView();
            this.filepathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modifiedTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filenameTextBox = new System.Windows.Forms.TextBox();
            this.exportFolderLabel = new System.Windows.Forms.Label();
            this.searchIncludeDirCheckBox = new System.Windows.Forms.CheckBox();
            this.fileCountLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.filesGridView)).BeginInit();
            this.SuspendLayout();
            //
            // imageList
            //
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "VSFolder_closed.bmp");
            this.imageList.Images.SetKeyName(1, "DocumentHS.png");
            //
            // filesGridView
            //
            this.filesGridView.AllowUserToAddRows = false;
            this.filesGridView.AllowUserToDeleteRows = false;
            this.filesGridView.AllowUserToResizeRows = false;
            resources.ApplyResources(this.filesGridView, "filesGridView");
            this.filesGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.filesGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.filesGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.filesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.filesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
            {
                this.filepathColumn,
                this.sizeColumn,
                this.modifiedTimeColumn
            });
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.filesGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.filesGridView.MultiSelect = false;
            this.filesGridView.Name = "filesGridView";
            this.filesGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.filesGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.filesGridView.RowHeadersVisible = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.LightGray;
            this.filesGridView.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.filesGridView.RowTemplate.Height = 23;
            this.filesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.filesGridView.VirtualMode = true;
            this.filesGridView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.filesGridView_CellMouseDoubleClick);
            this.filesGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.filesGridView_CellValueNeeded);
            this.filesGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.filesGridView_ColumnHeaderMouseClick);
            this.filesGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filesGridView_KeyDown);
            //
            // filepathColumn
            //
            this.filepathColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.filepathColumn.FillWeight = 80F;
            resources.ApplyResources(this.filepathColumn, "filepathColumn");
            this.filepathColumn.Name = "filepathColumn";
            this.filepathColumn.ReadOnly = true;
            //
            // sizeColumn
            //
            this.sizeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.sizeColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.sizeColumn.FillWeight = 10F;
            resources.ApplyResources(this.sizeColumn, "sizeColumn");
            this.sizeColumn.Name = "sizeColumn";
            this.sizeColumn.ReadOnly = true;
            //
            // modifiedTimeColumn
            //
            this.modifiedTimeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.modifiedTimeColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.modifiedTimeColumn.FillWeight = 20F;
            resources.ApplyResources(this.modifiedTimeColumn, "modifiedTimeColumn");
            this.modifiedTimeColumn.Name = "modifiedTimeColumn";
            this.modifiedTimeColumn.ReadOnly = true;
            this.modifiedTimeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            //
            // filenameTextBox
            //
            resources.ApplyResources(this.filenameTextBox, "filenameTextBox");
            this.filenameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.filenameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filenameTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.filenameTextBox.Name = "filenameTextBox";
            this.filenameTextBox.TextChanged += new System.EventHandler(this.filenameTextBox_TextChanged);
            this.filenameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filenameTextBox_KeyDown);
            //
            // exportFolderLabel
            //
            resources.ApplyResources(this.exportFolderLabel, "exportFolderLabel");
            this.exportFolderLabel.Name = "exportFolderLabel";
            //
            // searchIncludeDirCheckBox
            //
            resources.ApplyResources(this.searchIncludeDirCheckBox, "searchIncludeDirCheckBox");
            this.searchIncludeDirCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.searchIncludeDirCheckBox.Checked = true;
            this.searchIncludeDirCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.searchIncludeDirCheckBox.Name = "searchIncludeDirCheckBox";
            this.searchIncludeDirCheckBox.UseVisualStyleBackColor = false;
            this.searchIncludeDirCheckBox.CheckedChanged += new System.EventHandler(this.searchIncludeDirCheckBox_CheckedChanged);
            //
            // fileCountLabel
            //
            resources.ApplyResources(this.fileCountLabel, "fileCountLabel");
            this.fileCountLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.fileCountLabel.Name = "fileCountLabel";
            //
            // closeButton
            //
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = false;
            //
            // FindFileDialog
            //
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.fileCountLabel);
            this.Controls.Add(this.searchIncludeDirCheckBox);
            this.Controls.Add(this.exportFolderLabel);
            this.Controls.Add(this.filenameTextBox);
            this.Controls.Add(this.filesGridView);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindFileDialog";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.filesGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.DataGridView filesGridView;
        private System.Windows.Forms.TextBox filenameTextBox;
        private System.Windows.Forms.Label exportFolderLabel;
        private System.Windows.Forms.CheckBox searchIncludeDirCheckBox;
        private System.Windows.Forms.Label fileCountLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn filepathColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modifiedTimeColumn;
    }
}