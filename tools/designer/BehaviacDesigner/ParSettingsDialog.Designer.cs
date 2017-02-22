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
    partial class ParSettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParSettingsDialog));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.descLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.arrayLabel = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.valueLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.arrayCheckBox = new System.Windows.Forms.CheckBox();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.valueEditor = new Behaviac.Design.Attributes.DesignerPropertyEditor();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanel
            //
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.descLabel, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.nameLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.arrayLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.typeLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.valueLabel, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.nameTextBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.arrayCheckBox, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.typeComboBox, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.valueEditor, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.descTextBox, 1, 4);
            this.tableLayoutPanel.ForeColor = System.Drawing.Color.LightGray;
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            //
            // descLabel
            //
            resources.ApplyResources(this.descLabel, "descLabel");
            this.descLabel.ForeColor = System.Drawing.Color.LightGray;
            this.descLabel.Name = "descLabel";
            //
            // nameLabel
            //
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.ForeColor = System.Drawing.Color.LightGray;
            this.nameLabel.Name = "nameLabel";
            //
            // arrayLabel
            //
            resources.ApplyResources(this.arrayLabel, "arrayLabel");
            this.arrayLabel.ForeColor = System.Drawing.Color.LightGray;
            this.arrayLabel.Name = "arrayLabel";
            //
            // typeLabel
            //
            resources.ApplyResources(this.typeLabel, "typeLabel");
            this.typeLabel.ForeColor = System.Drawing.Color.LightGray;
            this.typeLabel.Name = "typeLabel";
            //
            // valueLabel
            //
            resources.ApplyResources(this.valueLabel, "valueLabel");
            this.valueLabel.ForeColor = System.Drawing.Color.LightGray;
            this.valueLabel.Name = "valueLabel";
            //
            // nameTextBox
            //
            this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            //
            // arrayCheckBox
            //
            resources.ApplyResources(this.arrayCheckBox, "arrayCheckBox");
            this.arrayCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.arrayCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.arrayCheckBox.Name = "arrayCheckBox";
            this.arrayCheckBox.UseVisualStyleBackColor = false;
            this.arrayCheckBox.CheckedChanged += new System.EventHandler(this.arrayCheckBox_CheckedChanged);
            //
            // typeComboBox
            //
            this.typeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            resources.ApplyResources(this.typeComboBox, "typeComboBox");
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            //
            // valueEditor
            //
            resources.ApplyResources(this.valueEditor, "valueEditor");
            this.valueEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.valueEditor.FilterType = null;
            this.valueEditor.ForeColor = System.Drawing.Color.LightGray;
            this.valueEditor.Name = "valueEditor";
            this.valueEditor.ValueType = ((Behaviac.Design.ValueTypes)(((Behaviac.Design.ValueTypes.Bool | Behaviac.Design.ValueTypes.Int)
                                                                        | Behaviac.Design.ValueTypes.Float)));
            //
            // descTextBox
            //
            this.descTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.descTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.descTextBox, "descTextBox");
            this.descTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.descTextBox.Name = "descTextBox";
            this.descTextBox.TextChanged += new System.EventHandler(this.descTextBox_TextChanged);
            //
            // okButton
            //
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.okButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.okButton.ForeColor = System.Drawing.Color.LightGray;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            //
            // cancelButton
            //
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.cancelButton.ForeColor = System.Drawing.Color.LightGray;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = false;
            //
            // ParSettingsDialog
            //
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.cancelButton;
            this.ControlBox = false;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.tableLayoutPanel);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ParSettingsDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ParSettingsDialog_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label arrayLabel;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.CheckBox arrayCheckBox;
        private System.Windows.Forms.ComboBox typeComboBox;
        private Attributes.DesignerPropertyEditor valueEditor;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.TextBox descTextBox;

    }
}
