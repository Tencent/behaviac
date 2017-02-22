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
    partial class MetaStoreDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaStoreDock));
            this.instanceLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.memberListBox = new System.Windows.Forms.ListBox();
            this.instanceComboBox = new System.Windows.Forms.ComboBox();
            this.memberTypeComboBox = new System.Windows.Forms.ComboBox();
            this.memberTypeLabel = new System.Windows.Forms.Label();
            this.memberFilterLabel = new System.Windows.Forms.Label();
            this.memberFilterTextBox = new System.Windows.Forms.TextBox();
            this.memberListLabel = new System.Windows.Forms.Label();
            this.memberCountLabel = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.addMemberButton = new System.Windows.Forms.Button();
            this.removeMemberButton = new System.Windows.Forms.Button();
            this.typeListBox = new System.Windows.Forms.ListBox();
            this.addTypeButton = new System.Windows.Forms.Button();
            this.removeTypeButton = new System.Windows.Forms.Button();
            this.memberFilterCheckBox = new System.Windows.Forms.CheckBox();
            this.downMemberButton = new System.Windows.Forms.Button();
            this.upMemberButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.addInstanceButton = new System.Windows.Forms.Button();
            this.removeInstanceButton = new System.Windows.Forms.Button();
            this.headerFileButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openPathButton = new System.Windows.Forms.Button();
            this.previewCodeButton = new System.Windows.Forms.Button();
            this.clearPathButton = new System.Windows.Forms.Button();
            this.metaTypePanel = new Behaviac.Design.MetaTypePanel();
            this.SuspendLayout();
            // 
            // instanceLabel
            // 
            resources.ApplyResources(this.instanceLabel, "instanceLabel");
            this.instanceLabel.Name = "instanceLabel";
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
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // memberListBox
            // 
            resources.ApplyResources(this.memberListBox, "memberListBox");
            this.memberListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.memberListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.memberListBox.ForeColor = System.Drawing.Color.LightGray;
            this.memberListBox.FormattingEnabled = true;
            this.memberListBox.Name = "memberListBox";
            this.memberListBox.SelectedIndexChanged += new System.EventHandler(this.memberListBox_SelectedIndexChanged);
            this.memberListBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.memberListBox_Format);
            this.memberListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.memberListBox_KeyDown);
            this.memberListBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.memberListBox_MouseDown);
            // 
            // instanceComboBox
            // 
            resources.ApplyResources(this.instanceComboBox, "instanceComboBox");
            this.instanceComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.instanceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.instanceComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.instanceComboBox.FormattingEnabled = true;
            this.instanceComboBox.Name = "instanceComboBox";
            this.instanceComboBox.SelectedIndexChanged += new System.EventHandler(this.instanceComboBox_SelectedIndexChanged);
            // 
            // memberTypeComboBox
            // 
            this.memberTypeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.memberTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.memberTypeComboBox, "memberTypeComboBox");
            this.memberTypeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.memberTypeComboBox.FormattingEnabled = true;
            this.memberTypeComboBox.Name = "memberTypeComboBox";
            this.memberTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.memberTypeComboBox_SelectedIndexChanged);
            // 
            // memberTypeLabel
            // 
            resources.ApplyResources(this.memberTypeLabel, "memberTypeLabel");
            this.memberTypeLabel.Name = "memberTypeLabel";
            // 
            // memberFilterLabel
            // 
            resources.ApplyResources(this.memberFilterLabel, "memberFilterLabel");
            this.memberFilterLabel.Name = "memberFilterLabel";
            // 
            // memberFilterTextBox
            // 
            resources.ApplyResources(this.memberFilterTextBox, "memberFilterTextBox");
            this.memberFilterTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.memberFilterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.memberFilterTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.memberFilterTextBox.Name = "memberFilterTextBox";
            this.memberFilterTextBox.TextChanged += new System.EventHandler(this.memberFilterTextBox_TextChanged);
            // 
            // memberListLabel
            // 
            resources.ApplyResources(this.memberListLabel, "memberListLabel");
            this.memberListLabel.Name = "memberListLabel";
            // 
            // memberCountLabel
            // 
            resources.ApplyResources(this.memberCountLabel, "memberCountLabel");
            this.memberCountLabel.Name = "memberCountLabel";
            // 
            // typeLabel
            // 
            resources.ApplyResources(this.typeLabel, "typeLabel");
            this.typeLabel.Name = "typeLabel";
            // 
            // addMemberButton
            // 
            resources.ApplyResources(this.addMemberButton, "addMemberButton");
            this.addMemberButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.addMemberButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.addMemberButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.addMemberButton.Name = "addMemberButton";
            this.addMemberButton.UseVisualStyleBackColor = false;
            this.addMemberButton.Click += new System.EventHandler(this.addMemberButton_Click);
            // 
            // removeMemberButton
            // 
            resources.ApplyResources(this.removeMemberButton, "removeMemberButton");
            this.removeMemberButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.removeMemberButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.removeMemberButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.removeMemberButton.Name = "removeMemberButton";
            this.removeMemberButton.UseVisualStyleBackColor = false;
            this.removeMemberButton.Click += new System.EventHandler(this.removeMemberButton_Click);
            // 
            // typeListBox
            // 
            this.typeListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.typeListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.typeListBox.ForeColor = System.Drawing.Color.LightGray;
            this.typeListBox.FormattingEnabled = true;
            resources.ApplyResources(this.typeListBox, "typeListBox");
            this.typeListBox.Name = "typeListBox";
            this.typeListBox.SelectedIndexChanged += new System.EventHandler(this.typeListBox_SelectedIndexChanged);
            // 
            // addTypeButton
            // 
            resources.ApplyResources(this.addTypeButton, "addTypeButton");
            this.addTypeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.addTypeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.addTypeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.addTypeButton.Name = "addTypeButton";
            this.addTypeButton.UseVisualStyleBackColor = false;
            this.addTypeButton.Click += new System.EventHandler(this.addTypeButton_Click);
            // 
            // removeTypeButton
            // 
            resources.ApplyResources(this.removeTypeButton, "removeTypeButton");
            this.removeTypeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.removeTypeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.removeTypeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.removeTypeButton.Name = "removeTypeButton";
            this.removeTypeButton.UseVisualStyleBackColor = false;
            this.removeTypeButton.Click += new System.EventHandler(this.removeTypeButton_Click);
            // 
            // memberFilterCheckBox
            // 
            resources.ApplyResources(this.memberFilterCheckBox, "memberFilterCheckBox");
            this.memberFilterCheckBox.Name = "memberFilterCheckBox";
            this.memberFilterCheckBox.UseVisualStyleBackColor = true;
            this.memberFilterCheckBox.CheckedChanged += new System.EventHandler(this.memberFilterCheckBox_CheckedChanged);
            // 
            // downMemberButton
            // 
            resources.ApplyResources(this.downMemberButton, "downMemberButton");
            this.downMemberButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.downMemberButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.downMemberButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.downMemberButton.Name = "downMemberButton";
            this.downMemberButton.UseVisualStyleBackColor = false;
            this.downMemberButton.Click += new System.EventHandler(this.downMemberButton_Click);
            // 
            // upMemberButton
            // 
            resources.ApplyResources(this.upMemberButton, "upMemberButton");
            this.upMemberButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.upMemberButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.upMemberButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.upMemberButton.Name = "upMemberButton";
            this.upMemberButton.UseVisualStyleBackColor = false;
            this.upMemberButton.Click += new System.EventHandler(this.upMemberButton_Click);
            // 
            // exportButton
            // 
            resources.ApplyResources(this.exportButton, "exportButton");
            this.exportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.exportButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.exportButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.exportButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.exportButton.Name = "exportButton";
            this.exportButton.UseVisualStyleBackColor = false;
            this.exportButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // addInstanceButton
            // 
            resources.ApplyResources(this.addInstanceButton, "addInstanceButton");
            this.addInstanceButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.addInstanceButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.addInstanceButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.addInstanceButton.Name = "addInstanceButton";
            this.addInstanceButton.UseVisualStyleBackColor = false;
            this.addInstanceButton.Click += new System.EventHandler(this.addInstanceButton_Click);
            // 
            // removeInstanceButton
            // 
            resources.ApplyResources(this.removeInstanceButton, "removeInstanceButton");
            this.removeInstanceButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.removeInstanceButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.removeInstanceButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.removeInstanceButton.Name = "removeInstanceButton";
            this.removeInstanceButton.UseVisualStyleBackColor = false;
            this.removeInstanceButton.Click += new System.EventHandler(this.removeInstanceButton_Click);
            // 
            // headerFileButton
            // 
            resources.ApplyResources(this.headerFileButton, "headerFileButton");
            this.headerFileButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.headerFileButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.headerFileButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.headerFileButton.Name = "headerFileButton";
            this.headerFileButton.UseVisualStyleBackColor = false;
            this.headerFileButton.Click += new System.EventHandler(this.headerFileButton_Click);
            this.headerFileButton.MouseEnter += new System.EventHandler(this.headerFileButton_MouseEnter);
            this.headerFileButton.MouseLeave += new System.EventHandler(this.headerFileButton_MouseLeave);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // openPathButton
            // 
            resources.ApplyResources(this.openPathButton, "openPathButton");
            this.openPathButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.openPathButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.openPathButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.openPathButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.openPathButton.Name = "openPathButton";
            this.openPathButton.UseVisualStyleBackColor = false;
            this.openPathButton.Click += new System.EventHandler(this.openPathButton_Click);
            // 
            // previewCodeButton
            // 
            resources.ApplyResources(this.previewCodeButton, "previewCodeButton");
            this.previewCodeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.previewCodeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.previewCodeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.previewCodeButton.Name = "previewCodeButton";
            this.previewCodeButton.UseVisualStyleBackColor = false;
            this.previewCodeButton.Click += new System.EventHandler(this.previewCodeButton_Click);
            this.previewCodeButton.MouseEnter += new System.EventHandler(this.previewCodeButton_MouseEnter);
            this.previewCodeButton.MouseLeave += new System.EventHandler(this.previewCodeButton_MouseLeave);
            // 
            // clearPathButton
            // 
            resources.ApplyResources(this.clearPathButton, "clearPathButton");
            this.clearPathButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.clearPathButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.clearPathButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.clearPathButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.clearPathButton.Name = "clearPathButton";
            this.clearPathButton.UseVisualStyleBackColor = false;
            this.clearPathButton.Click += new System.EventHandler(this.clearPathButton_Click);
            // 
            // metaTypePanel
            // 
            resources.ApplyResources(this.metaTypePanel, "metaTypePanel");
            this.metaTypePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.metaTypePanel.ForeColor = System.Drawing.Color.LightGray;
            this.metaTypePanel.IsModified = false;
            this.metaTypePanel.Name = "metaTypePanel";
            // 
            // MetaStoreDock
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.closeButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.clearPathButton);
            this.Controls.Add(this.previewCodeButton);
            this.Controls.Add(this.openPathButton);
            this.Controls.Add(this.headerFileButton);
            this.Controls.Add(this.removeInstanceButton);
            this.Controls.Add(this.addInstanceButton);
            this.Controls.Add(this.metaTypePanel);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.downMemberButton);
            this.Controls.Add(this.upMemberButton);
            this.Controls.Add(this.memberFilterCheckBox);
            this.Controls.Add(this.addTypeButton);
            this.Controls.Add(this.removeTypeButton);
            this.Controls.Add(this.typeListBox);
            this.Controls.Add(this.removeMemberButton);
            this.Controls.Add(this.addMemberButton);
            this.Controls.Add(this.typeLabel);
            this.Controls.Add(this.memberCountLabel);
            this.Controls.Add(this.memberListLabel);
            this.Controls.Add(this.memberFilterTextBox);
            this.Controls.Add(this.memberFilterLabel);
            this.Controls.Add(this.memberTypeComboBox);
            this.Controls.Add(this.memberTypeLabel);
            this.Controls.Add(this.instanceComboBox);
            this.Controls.Add(this.memberListBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.instanceLabel);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MetaStoreDock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MetaStoreDock_FormClosing);
            this.Resize += new System.EventHandler(this.MetaStoreDock_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label instanceLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ListBox memberListBox;
        private System.Windows.Forms.ComboBox instanceComboBox;
        private System.Windows.Forms.ComboBox memberTypeComboBox;
        private System.Windows.Forms.Label memberTypeLabel;
        private System.Windows.Forms.Label memberFilterLabel;
        private System.Windows.Forms.TextBox memberFilterTextBox;
        private System.Windows.Forms.Label memberListLabel;
        private System.Windows.Forms.Label memberCountLabel;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Button addMemberButton;
        private System.Windows.Forms.Button removeMemberButton;
        private System.Windows.Forms.ListBox typeListBox;
        private System.Windows.Forms.Button addTypeButton;
        private System.Windows.Forms.Button removeTypeButton;
        private System.Windows.Forms.CheckBox memberFilterCheckBox;
        private System.Windows.Forms.Button downMemberButton;
        private System.Windows.Forms.Button upMemberButton;
        private System.Windows.Forms.Button exportButton;
        private MetaTypePanel metaTypePanel;
        private System.Windows.Forms.Button addInstanceButton;
        private System.Windows.Forms.Button removeInstanceButton;
        private System.Windows.Forms.Button headerFileButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button openPathButton;
        private System.Windows.Forms.Button previewCodeButton;
        private System.Windows.Forms.Button clearPathButton;

    }
}
