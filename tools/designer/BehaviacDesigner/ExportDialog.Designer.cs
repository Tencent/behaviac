////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Behaviac.Design
{
    partial class ExportDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.exportAllButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.exportBehaviorsLabel = new System.Windows.Forms.Label();
            this.exportSettingLabel = new System.Windows.Forms.Label();
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.fileCountLabel = new System.Windows.Forms.Label();
            this.exportSettingGridView = new System.Windows.Forms.DataGridView();
            this.Enable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Format = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Setting = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.onlyShowErrorsCheckBox = new System.Windows.Forms.CheckBox();
            this.exportTypesCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.exportSettingGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // exportAllButton
            // 
            resources.ApplyResources(this.exportAllButton, "exportAllButton");
            this.exportAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.exportAllButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.exportAllButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.exportAllButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.exportAllButton.Name = "exportAllButton";
            this.exportAllButton.UseVisualStyleBackColor = false;
            this.exportAllButton.Click += new System.EventHandler(this.exportAllButton_Click);
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
            // exportBehaviorsLabel
            // 
            resources.ApplyResources(this.exportBehaviorsLabel, "exportBehaviorsLabel");
            this.exportBehaviorsLabel.Name = "exportBehaviorsLabel";
            // 
            // exportSettingLabel
            // 
            resources.ApplyResources(this.exportSettingLabel, "exportSettingLabel");
            this.exportSettingLabel.Name = "exportSettingLabel";
            // 
            // treeView
            // 
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.treeView.CheckBoxes = true;
            this.treeView.ForeColor = System.Drawing.Color.LightGray;
            this.treeView.ImageList = this.imageList;
            this.treeView.Name = "treeView";
            this.treeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeCheck);
            this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "VSFolder_closed.bmp");
            this.imageList.Images.SetKeyName(1, "DocumentHS.png");
            // 
            // fileCountLabel
            // 
            resources.ApplyResources(this.fileCountLabel, "fileCountLabel");
            this.fileCountLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.fileCountLabel.ForeColor = System.Drawing.Color.Orange;
            this.fileCountLabel.Name = "fileCountLabel";
            // 
            // exportSettingGridView
            // 
            this.exportSettingGridView.AllowUserToAddRows = false;
            this.exportSettingGridView.AllowUserToDeleteRows = false;
            this.exportSettingGridView.AllowUserToResizeRows = false;
            resources.ApplyResources(this.exportSettingGridView, "exportSettingGridView");
            this.exportSettingGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.exportSettingGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.exportSettingGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.exportSettingGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.exportSettingGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Enable,
            this.Format,
            this.Setting});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.exportSettingGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.exportSettingGridView.MultiSelect = false;
            this.exportSettingGridView.Name = "exportSettingGridView";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.exportSettingGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.exportSettingGridView.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.LightGray;
            this.exportSettingGridView.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.exportSettingGridView.RowTemplate.Height = 23;
            this.exportSettingGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.exportSettingGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.exportSettingGridView_CellContentClick);
            this.exportSettingGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.exportSettingGridView_CellValueChanged);
            // 
            // Enable
            // 
            resources.ApplyResources(this.Enable, "Enable");
            this.Enable.Name = "Enable";
            this.Enable.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Format
            // 
            this.Format.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Format, "Format");
            this.Format.Name = "Format";
            this.Format.ReadOnly = true;
            this.Format.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Format.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Setting
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.LightGray;
            this.Setting.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.Setting, "Setting");
            this.Setting.MaxInputLength = 4;
            this.Setting.Name = "Setting";
            this.Setting.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Setting.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // onlyShowErrorsCheckBox
            // 
            resources.ApplyResources(this.onlyShowErrorsCheckBox, "onlyShowErrorsCheckBox");
            this.onlyShowErrorsCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.onlyShowErrorsCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.onlyShowErrorsCheckBox.Name = "onlyShowErrorsCheckBox";
            this.onlyShowErrorsCheckBox.UseVisualStyleBackColor = false;
            this.onlyShowErrorsCheckBox.CheckedChanged += new System.EventHandler(this.onlyShowErrorsCheckBox_CheckedChanged);
            // 
            // exportTypesCheckBox
            // 
            resources.ApplyResources(this.exportTypesCheckBox, "exportTypesCheckBox");
            this.exportTypesCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.exportTypesCheckBox.Checked = true;
            this.exportTypesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportTypesCheckBox.Name = "exportTypesCheckBox";
            this.exportTypesCheckBox.UseVisualStyleBackColor = false;
            this.exportTypesCheckBox.CheckedChanged += new System.EventHandler(this.exportTypesCheckBox_CheckedChanged);
            // 
            // ExportDialog
            // 
            this.AcceptButton = this.exportAllButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.exportTypesCheckBox);
            this.Controls.Add(this.onlyShowErrorsCheckBox);
            this.Controls.Add(this.exportSettingGridView);
            this.Controls.Add(this.fileCountLabel);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.exportSettingLabel);
            this.Controls.Add(this.exportBehaviorsLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.exportAllButton);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportDialog";
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportDialog_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.exportSettingGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button exportAllButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label exportBehaviorsLabel;
        private System.Windows.Forms.Label exportSettingLabel;
        internal System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Label fileCountLabel;
        private System.Windows.Forms.DataGridView exportSettingGridView;
        private System.Windows.Forms.CheckBox onlyShowErrorsCheckBox;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Enable;
        private System.Windows.Forms.DataGridViewTextBoxColumn Format;
        private System.Windows.Forms.DataGridViewTextBoxColumn Setting;
        private System.Windows.Forms.CheckBox exportTypesCheckBox;
    }
}