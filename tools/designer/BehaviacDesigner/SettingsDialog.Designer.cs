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
    partial class SettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.acceptButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.nodeToolTipsCheckBox = new System.Windows.Forms.CheckBox();
            this.showControlsCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBoxChecktheLatest = new System.Windows.Forms.CheckBox();
            this.dumpConnectDataCheckBox = new System.Windows.Forms.CheckBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.showVersionCheckBox = new System.Windows.Forms.CheckBox();
            this.useBasicDisplayNameCheckBox = new System.Windows.Forms.CheckBox();
            this.showProfileCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBoxTweatAsError = new System.Windows.Forms.CheckBox();
            this.limitDisplayLengthCheckBox = new System.Windows.Forms.CheckBox();
            this.displayLengthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.themLabel = new System.Windows.Forms.Label();
            this.resetLayoutButton = new System.Windows.Forms.Button();
            this.concurrentProcessBehaviorsCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.displayLengthNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // acceptButton
            // 
            this.acceptButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.acceptButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            resources.ApplyResources(this.acceptButton, "acceptButton");
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.UseVisualStyleBackColor = false;
            this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // nodeToolTipsCheckBox
            // 
            resources.ApplyResources(this.nodeToolTipsCheckBox, "nodeToolTipsCheckBox");
            this.nodeToolTipsCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.nodeToolTipsCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.nodeToolTipsCheckBox.Name = "nodeToolTipsCheckBox";
            this.nodeToolTipsCheckBox.UseVisualStyleBackColor = false;
            // 
            // showControlsCheckBox
            // 
            resources.ApplyResources(this.showControlsCheckBox, "showControlsCheckBox");
            this.showControlsCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.showControlsCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.showControlsCheckBox.Name = "showControlsCheckBox";
            this.showControlsCheckBox.UseVisualStyleBackColor = false;
            // 
            // checkBoxChecktheLatest
            // 
            resources.ApplyResources(this.checkBoxChecktheLatest, "checkBoxChecktheLatest");
            this.checkBoxChecktheLatest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.checkBoxChecktheLatest.ForeColor = System.Drawing.Color.LightGray;
            this.checkBoxChecktheLatest.Name = "checkBoxChecktheLatest";
            this.checkBoxChecktheLatest.UseVisualStyleBackColor = false;
            // 
            // dumpConnectDataCheckBox
            // 
            resources.ApplyResources(this.dumpConnectDataCheckBox, "dumpConnectDataCheckBox");
            this.dumpConnectDataCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.dumpConnectDataCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.dumpConnectDataCheckBox.Name = "dumpConnectDataCheckBox";
            this.dumpConnectDataCheckBox.UseVisualStyleBackColor = false;
            // 
            // languageLabel
            // 
            resources.ApplyResources(this.languageLabel, "languageLabel");
            this.languageLabel.Name = "languageLabel";
            // 
            // languageComboBox
            // 
            this.languageComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.languageComboBox, "languageComboBox");
            this.languageComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Items.AddRange(new object[] {
            resources.GetString("languageComboBox.Items"),
            resources.GetString("languageComboBox.Items1"),
            resources.GetString("languageComboBox.Items2")});
            this.languageComboBox.Name = "languageComboBox";
            // 
            // showVersionCheckBox
            // 
            resources.ApplyResources(this.showVersionCheckBox, "showVersionCheckBox");
            this.showVersionCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.showVersionCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.showVersionCheckBox.Name = "showVersionCheckBox";
            this.showVersionCheckBox.UseVisualStyleBackColor = false;
            // 
            // useBasicDisplayNameCheckBox
            // 
            resources.ApplyResources(this.useBasicDisplayNameCheckBox, "useBasicDisplayNameCheckBox");
            this.useBasicDisplayNameCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.useBasicDisplayNameCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.useBasicDisplayNameCheckBox.Name = "useBasicDisplayNameCheckBox";
            this.useBasicDisplayNameCheckBox.UseVisualStyleBackColor = false;
            // 
            // showProfileCheckBox
            // 
            resources.ApplyResources(this.showProfileCheckBox, "showProfileCheckBox");
            this.showProfileCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.showProfileCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.showProfileCheckBox.Name = "showProfileCheckBox";
            this.showProfileCheckBox.UseVisualStyleBackColor = false;
            // 
            // checkBoxTweatAsError
            // 
            resources.ApplyResources(this.checkBoxTweatAsError, "checkBoxTweatAsError");
            this.checkBoxTweatAsError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.checkBoxTweatAsError.ForeColor = System.Drawing.Color.LightGray;
            this.checkBoxTweatAsError.Name = "checkBoxTweatAsError";
            this.checkBoxTweatAsError.UseVisualStyleBackColor = false;
            // 
            // limitDisplayLengthCheckBox
            // 
            resources.ApplyResources(this.limitDisplayLengthCheckBox, "limitDisplayLengthCheckBox");
            this.limitDisplayLengthCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.limitDisplayLengthCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.limitDisplayLengthCheckBox.Name = "limitDisplayLengthCheckBox";
            this.limitDisplayLengthCheckBox.UseVisualStyleBackColor = false;
            // 
            // displayLengthNumericUpDown
            // 
            this.displayLengthNumericUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.displayLengthNumericUpDown.ForeColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.displayLengthNumericUpDown, "displayLengthNumericUpDown");
            this.displayLengthNumericUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.displayLengthNumericUpDown.Name = "displayLengthNumericUpDown";
            this.displayLengthNumericUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // themeComboBox
            // 
            this.themeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.themeComboBox, "themeComboBox");
            this.themeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Items.AddRange(new object[] {
            resources.GetString("themeComboBox.Items"),
            resources.GetString("themeComboBox.Items1")});
            this.themeComboBox.Name = "themeComboBox";
            // 
            // themLabel
            // 
            resources.ApplyResources(this.themLabel, "themLabel");
            this.themLabel.Name = "themLabel";
            // 
            // resetLayoutButton
            // 
            this.resetLayoutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.resetLayoutButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.resetLayoutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.resetLayoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            resources.ApplyResources(this.resetLayoutButton, "resetLayoutButton");
            this.resetLayoutButton.Name = "resetLayoutButton";
            this.resetLayoutButton.UseVisualStyleBackColor = false;
            this.resetLayoutButton.Click += new System.EventHandler(this.resetLayoutButton_Click);
            // 
            // concurrentProcessBehaviorsCheckBox
            // 
            resources.ApplyResources(this.concurrentProcessBehaviorsCheckBox, "concurrentProcessBehaviorsCheckBox");
            this.concurrentProcessBehaviorsCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.concurrentProcessBehaviorsCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.concurrentProcessBehaviorsCheckBox.Name = "concurrentProcessBehaviorsCheckBox";
            this.concurrentProcessBehaviorsCheckBox.UseVisualStyleBackColor = false;
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this.acceptButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.cancelButton;
            this.ControlBox = false;
            this.Controls.Add(this.concurrentProcessBehaviorsCheckBox);
            this.Controls.Add(this.resetLayoutButton);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(this.themLabel);
            this.Controls.Add(this.displayLengthNumericUpDown);
            this.Controls.Add(this.limitDisplayLengthCheckBox);
            this.Controls.Add(this.checkBoxTweatAsError);
            this.Controls.Add(this.showProfileCheckBox);
            this.Controls.Add(this.useBasicDisplayNameCheckBox);
            this.Controls.Add(this.showVersionCheckBox);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.languageLabel);
            this.Controls.Add(this.dumpConnectDataCheckBox);
            this.Controls.Add(this.checkBoxChecktheLatest);
            this.Controls.Add(this.showControlsCheckBox);
            this.Controls.Add(this.nodeToolTipsCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.acceptButton);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            ((System.ComponentModel.ISupportInitialize)(this.displayLengthNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nodeToolTipsCheckBox;
        private System.Windows.Forms.CheckBox showControlsCheckBox;
        private System.Windows.Forms.CheckBox checkBoxChecktheLatest;
        private System.Windows.Forms.CheckBox dumpConnectDataCheckBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.CheckBox showVersionCheckBox;
        private System.Windows.Forms.CheckBox useBasicDisplayNameCheckBox;
        private System.Windows.Forms.CheckBox showProfileCheckBox;
        private System.Windows.Forms.CheckBox checkBoxTweatAsError;
        private System.Windows.Forms.CheckBox limitDisplayLengthCheckBox;
        private System.Windows.Forms.NumericUpDown displayLengthNumericUpDown;
        private System.Windows.Forms.ComboBox themeComboBox;
        private System.Windows.Forms.Label themLabel;
        private System.Windows.Forms.Button resetLayoutButton;
        private System.Windows.Forms.CheckBox concurrentProcessBehaviorsCheckBox;
    }
}