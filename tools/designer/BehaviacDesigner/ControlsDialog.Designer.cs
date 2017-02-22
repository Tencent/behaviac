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
    partial class ControlsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlsDialog));
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.buttonClose = new System.Windows.Forms.Button();
            this.checkBoxNext = new System.Windows.Forms.CheckBox();
            this.documentLabel = new System.Windows.Forms.Label();
            this.noteLabel = new System.Windows.Forms.Label();
            this.overviewButton = new System.Windows.Forms.Button();
            this.tutorialsButton = new System.Windows.Forms.Button();
            this.workspacesComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            resources.ApplyResources(this.webBrowser, "webBrowser");
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Url = new System.Uri("", System.UriKind.Relative);
            this.webBrowser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.webBrowser_PreviewKeyDown);
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.buttonClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.buttonClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.UseVisualStyleBackColor = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // checkBoxNext
            // 
            resources.ApplyResources(this.checkBoxNext, "checkBoxNext");
            this.checkBoxNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.checkBoxNext.Name = "checkBoxNext";
            this.checkBoxNext.UseVisualStyleBackColor = false;
            // 
            // documentLabel
            // 
            resources.ApplyResources(this.documentLabel, "documentLabel");
            this.documentLabel.Name = "documentLabel";
            // 
            // noteLabel
            // 
            resources.ApplyResources(this.noteLabel, "noteLabel");
            this.noteLabel.Name = "noteLabel";
            // 
            // overviewButton
            // 
            resources.ApplyResources(this.overviewButton, "overviewButton");
            this.overviewButton.Name = "overviewButton";
            this.overviewButton.UseVisualStyleBackColor = true;
            this.overviewButton.Click += new System.EventHandler(this.overviewButton_Click);
            // 
            // tutorialsButton
            // 
            resources.ApplyResources(this.tutorialsButton, "tutorialsButton");
            this.tutorialsButton.Name = "tutorialsButton";
            this.tutorialsButton.UseVisualStyleBackColor = true;
            this.tutorialsButton.Click += new System.EventHandler(this.tutorialsButton_Click);
            // 
            // workspacesComboBox
            // 
            resources.ApplyResources(this.workspacesComboBox, "workspacesComboBox");
            this.workspacesComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.workspacesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.workspacesComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.workspacesComboBox.FormattingEnabled = true;
            this.workspacesComboBox.Name = "workspacesComboBox";
            this.workspacesComboBox.SelectedIndexChanged += new System.EventHandler(this.workspacesComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ControlsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.label1);
            this.Controls.Add(this.workspacesComboBox);
            this.Controls.Add(this.tutorialsButton);
            this.Controls.Add(this.overviewButton);
            this.Controls.Add(this.noteLabel);
            this.Controls.Add(this.documentLabel);
            this.Controls.Add(this.checkBoxNext);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.webBrowser);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.KeyPreview = true;
            this.Name = "ControlsDialog";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlsDialog_FormClosed);
            this.Load += new System.EventHandler(this.ControlsDialog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControlsDialog_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.CheckBox checkBoxNext;
        private System.Windows.Forms.Label documentLabel;
        private System.Windows.Forms.Label noteLabel;
        private System.Windows.Forms.Button overviewButton;
        private System.Windows.Forms.Button tutorialsButton;
        private System.Windows.Forms.ComboBox workspacesComboBox;
        private System.Windows.Forms.Label label1;


    }
}