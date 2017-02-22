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
    partial class FindDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindDock));
            this.whatLabel = new System.Windows.Forms.Label();
            this.whatComboBox = new System.Windows.Forms.ComboBox();
            this.optionGroupBox = new System.Windows.Forms.GroupBox();
            this.nodeTypeCheckBox = new System.Windows.Forms.CheckBox();
            this.nodeIdCheckBox = new System.Windows.Forms.CheckBox();
            this.matchWholeWordCheckBox = new System.Windows.Forms.CheckBox();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.findNextButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.rangeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.findPreviousButton = new System.Windows.Forms.Button();
            this.optionGroupBox.SuspendLayout();
            this.SuspendLayout();
            //
            // whatLabel
            //
            resources.ApplyResources(this.whatLabel, "whatLabel");
            this.whatLabel.Name = "whatLabel";
            //
            // whatComboBox
            //
            resources.ApplyResources(this.whatComboBox, "whatComboBox");
            this.whatComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.whatComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.whatComboBox.FormattingEnabled = true;
            this.whatComboBox.Name = "whatComboBox";
            //
            // optionGroupBox
            //
            resources.ApplyResources(this.optionGroupBox, "optionGroupBox");
            this.optionGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.optionGroupBox.Controls.Add(this.nodeTypeCheckBox);
            this.optionGroupBox.Controls.Add(this.nodeIdCheckBox);
            this.optionGroupBox.Controls.Add(this.matchWholeWordCheckBox);
            this.optionGroupBox.Controls.Add(this.matchCaseCheckBox);
            this.optionGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.optionGroupBox.Name = "optionGroupBox";
            this.optionGroupBox.TabStop = false;
            //
            // nodeTypeCheckBox
            //
            resources.ApplyResources(this.nodeTypeCheckBox, "nodeTypeCheckBox");
            this.nodeTypeCheckBox.Name = "nodeTypeCheckBox";
            this.nodeTypeCheckBox.UseVisualStyleBackColor = true;
            this.nodeTypeCheckBox.CheckedChanged += new System.EventHandler(this.nodeTypeCheckBox_CheckedChanged);
            //
            // nodeIdCheckBox
            //
            resources.ApplyResources(this.nodeIdCheckBox, "nodeIdCheckBox");
            this.nodeIdCheckBox.Name = "nodeIdCheckBox";
            this.nodeIdCheckBox.UseVisualStyleBackColor = true;
            this.nodeIdCheckBox.CheckedChanged += new System.EventHandler(this.nodeIdCheckBox_CheckedChanged);
            //
            // matchWholeWordCheckBox
            //
            resources.ApplyResources(this.matchWholeWordCheckBox, "matchWholeWordCheckBox");
            this.matchWholeWordCheckBox.Name = "matchWholeWordCheckBox";
            this.matchWholeWordCheckBox.UseVisualStyleBackColor = true;
            //
            // matchCaseCheckBox
            //
            resources.ApplyResources(this.matchCaseCheckBox, "matchCaseCheckBox");
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            //
            // findNextButton
            //
            resources.ApplyResources(this.findNextButton, "findNextButton");
            this.findNextButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.findNextButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.findNextButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.UseVisualStyleBackColor = false;
            this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
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
            // rangeComboBox
            //
            resources.ApplyResources(this.rangeComboBox, "rangeComboBox");
            this.rangeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.rangeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rangeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.rangeComboBox.FormattingEnabled = true;
            this.rangeComboBox.Items.AddRange(new object[]
            {
                resources.GetString("rangeComboBox.Items"),
                resources.GetString("rangeComboBox.Items1"),
                resources.GetString("rangeComboBox.Items2")
            });
            this.rangeComboBox.Name = "rangeComboBox";
            //
            // label1
            //
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            //
            // findPreviousButton
            //
            resources.ApplyResources(this.findPreviousButton, "findPreviousButton");
            this.findPreviousButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.findPreviousButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.findPreviousButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.findPreviousButton.Name = "findPreviousButton";
            this.findPreviousButton.UseVisualStyleBackColor = false;
            this.findPreviousButton.Click += new System.EventHandler(this.findPreviousButton_Click);
            //
            // FindDock
            //
            this.AcceptButton = this.findNextButton;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.closeButton;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.findPreviousButton);
            this.Controls.Add(this.rangeComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.findNextButton);
            this.Controls.Add(this.optionGroupBox);
            this.Controls.Add(this.whatComboBox);
            this.Controls.Add(this.whatLabel);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindDock";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindDock_FormClosing);
            this.optionGroupBox.ResumeLayout(false);
            this.optionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label whatLabel;
        private System.Windows.Forms.ComboBox whatComboBox;
        private System.Windows.Forms.GroupBox optionGroupBox;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.CheckBox matchWholeWordCheckBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.ComboBox rangeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button findPreviousButton;
        private System.Windows.Forms.CheckBox nodeIdCheckBox;
        private System.Windows.Forms.CheckBox nodeTypeCheckBox;

    }
}
