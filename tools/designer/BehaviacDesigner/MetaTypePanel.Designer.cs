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
    partial class MetaTypePanel
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaTypePanel));
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.typeLabel = new System.Windows.Forms.Label();
            this.descGroupBox = new System.Windows.Forms.GroupBox();
            this.isRefCheckBox = new System.Windows.Forms.CheckBox();
            this.locationButton = new System.Windows.Forms.Button();
            this.locationTextBox = new System.Windows.Forms.TextBox();
            this.locationLabel = new System.Windows.Forms.Label();
            this.exportCodeCheckBox = new System.Windows.Forms.CheckBox();
            this.namespaceTextBox = new System.Windows.Forms.TextBox();
            this.namespaceLabel = new System.Windows.Forms.Label();
            this.baseComboBox = new System.Windows.Forms.ComboBox();
            this.baseLabel = new System.Windows.Forms.Label();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.descLabel = new System.Windows.Forms.Label();
            this.dispTextBox = new System.Windows.Forms.TextBox();
            this.dispLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.agentLabel = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.descGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // typeComboBox
            // 
            resources.ApplyResources(this.typeComboBox, "typeComboBox");
            this.typeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            // 
            // typeLabel
            // 
            resources.ApplyResources(this.typeLabel, "typeLabel");
            this.typeLabel.Name = "typeLabel";
            // 
            // descGroupBox
            // 
            resources.ApplyResources(this.descGroupBox, "descGroupBox");
            this.descGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.descGroupBox.Controls.Add(this.isRefCheckBox);
            this.descGroupBox.Controls.Add(this.locationButton);
            this.descGroupBox.Controls.Add(this.locationTextBox);
            this.descGroupBox.Controls.Add(this.locationLabel);
            this.descGroupBox.Controls.Add(this.exportCodeCheckBox);
            this.descGroupBox.Controls.Add(this.namespaceTextBox);
            this.descGroupBox.Controls.Add(this.namespaceLabel);
            this.descGroupBox.Controls.Add(this.typeComboBox);
            this.descGroupBox.Controls.Add(this.typeLabel);
            this.descGroupBox.Controls.Add(this.baseComboBox);
            this.descGroupBox.Controls.Add(this.baseLabel);
            this.descGroupBox.Controls.Add(this.descTextBox);
            this.descGroupBox.Controls.Add(this.descLabel);
            this.descGroupBox.Controls.Add(this.dispTextBox);
            this.descGroupBox.Controls.Add(this.dispLabel);
            this.descGroupBox.Controls.Add(this.nameTextBox);
            this.descGroupBox.Controls.Add(this.agentLabel);
            this.descGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.descGroupBox.Name = "descGroupBox";
            this.descGroupBox.TabStop = false;
            // 
            // isRefCheckBox
            // 
            resources.ApplyResources(this.isRefCheckBox, "isRefCheckBox");
            this.isRefCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.isRefCheckBox.Checked = true;
            this.isRefCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.isRefCheckBox.Name = "isRefCheckBox";
            this.isRefCheckBox.UseVisualStyleBackColor = false;
            this.isRefCheckBox.CheckedChanged += new System.EventHandler(this.isRefCheckBox_CheckedChanged);
            // 
            // locationButton
            // 
            resources.ApplyResources(this.locationButton, "locationButton");
            this.locationButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.locationButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.locationButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.locationButton.Name = "locationButton";
            this.locationButton.UseVisualStyleBackColor = false;
            this.locationButton.Click += new System.EventHandler(this.locationButton_Click);
            // 
            // locationTextBox
            // 
            resources.ApplyResources(this.locationTextBox, "locationTextBox");
            this.locationTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.locationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.locationTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.locationTextBox.Name = "locationTextBox";
            this.locationTextBox.ReadOnly = true;
            // 
            // locationLabel
            // 
            resources.ApplyResources(this.locationLabel, "locationLabel");
            this.locationLabel.Name = "locationLabel";
            // 
            // exportCodeCheckBox
            // 
            resources.ApplyResources(this.exportCodeCheckBox, "exportCodeCheckBox");
            this.exportCodeCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.exportCodeCheckBox.Checked = true;
            this.exportCodeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.exportCodeCheckBox.Name = "exportCodeCheckBox";
            this.exportCodeCheckBox.UseVisualStyleBackColor = false;
            this.exportCodeCheckBox.CheckedChanged += new System.EventHandler(this.exportCodeCheckBox_CheckedChanged);
            this.exportCodeCheckBox.MouseEnter += new System.EventHandler(this.exportCodeCheckBox_MouseEnter);
            this.exportCodeCheckBox.MouseLeave += new System.EventHandler(this.exportCodeCheckBox_MouseLeave);
            // 
            // namespaceTextBox
            // 
            resources.ApplyResources(this.namespaceTextBox, "namespaceTextBox");
            this.namespaceTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.namespaceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.namespaceTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.namespaceTextBox.Name = "namespaceTextBox";
            this.namespaceTextBox.TextChanged += new System.EventHandler(this.namespaceTextBox_TextChanged);
            this.namespaceTextBox.Leave += new System.EventHandler(this.namespaceTextBox_Leave);
            // 
            // namespaceLabel
            // 
            resources.ApplyResources(this.namespaceLabel, "namespaceLabel");
            this.namespaceLabel.Name = "namespaceLabel";
            // 
            // baseComboBox
            // 
            resources.ApplyResources(this.baseComboBox, "baseComboBox");
            this.baseComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.baseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baseComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.baseComboBox.FormattingEnabled = true;
            this.baseComboBox.Name = "baseComboBox";
            this.baseComboBox.SelectedIndexChanged += new System.EventHandler(this.baseComboBox_SelectedIndexChanged);
            // 
            // baseLabel
            // 
            resources.ApplyResources(this.baseLabel, "baseLabel");
            this.baseLabel.Name = "baseLabel";
            // 
            // descTextBox
            // 
            resources.ApplyResources(this.descTextBox, "descTextBox");
            this.descTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.descTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.descTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.descTextBox.Name = "descTextBox";
            this.descTextBox.TextChanged += new System.EventHandler(this.descTextBox_TextChanged);
            this.descTextBox.Leave += new System.EventHandler(this.descTextBox_Leave);
            // 
            // descLabel
            // 
            resources.ApplyResources(this.descLabel, "descLabel");
            this.descLabel.Name = "descLabel";
            // 
            // dispTextBox
            // 
            resources.ApplyResources(this.dispTextBox, "dispTextBox");
            this.dispTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.dispTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dispTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.dispTextBox.Name = "dispTextBox";
            this.dispTextBox.TextChanged += new System.EventHandler(this.dispTextBox_TextChanged);
            this.dispTextBox.Leave += new System.EventHandler(this.dispTextBox_Leave);
            // 
            // dispLabel
            // 
            resources.ApplyResources(this.dispLabel, "dispLabel");
            this.dispLabel.Name = "dispLabel";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_Leave);
            // 
            // agentLabel
            // 
            resources.ApplyResources(this.agentLabel, "agentLabel");
            this.agentLabel.Name = "agentLabel";
            // 
            // folderBrowserDialog
            // 
            resources.ApplyResources(this.folderBrowserDialog, "folderBrowserDialog");
            // 
            // MetaTypePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.descGroupBox);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "MetaTypePanel";
            this.descGroupBox.ResumeLayout(false);
            this.descGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.GroupBox descGroupBox;
        private System.Windows.Forms.ComboBox baseComboBox;
        private System.Windows.Forms.Label baseLabel;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.TextBox dispTextBox;
        private System.Windows.Forms.Label dispLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label agentLabel;
        private System.Windows.Forms.TextBox namespaceTextBox;
        private System.Windows.Forms.Label namespaceLabel;
        private System.Windows.Forms.CheckBox exportCodeCheckBox;
        private System.Windows.Forms.TextBox locationTextBox;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.Button locationButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.CheckBox isRefCheckBox;


    }
}
