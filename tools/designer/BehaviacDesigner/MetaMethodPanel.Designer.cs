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
    partial class MetaMethodPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaMethodPanel));
            this.descGroupBox = new System.Windows.Forms.GroupBox();
            this.isPublicCheckBox = new System.Windows.Forms.CheckBox();
            this.isStaticCheckBox = new System.Windows.Forms.CheckBox();
            this.arrayCheckBox = new System.Windows.Forms.CheckBox();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.descLabel = new System.Windows.Forms.Label();
            this.dispTextBox = new System.Windows.Forms.TextBox();
            this.dispLabel = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new Behaviac.Design.BehaviacTableLayoutPanel();
            this.displaynameLabel = new System.Windows.Forms.Label();
            this.byReferLabel = new System.Windows.Forms.Label();
            this.parameterLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.isArrayLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.isConstlabel = new System.Windows.Forms.Label();
            this.removeParamButton = new System.Windows.Forms.Button();
            this.addParamButton = new System.Windows.Forms.Button();
            this.paramLabel = new System.Windows.Forms.Label();
            this.returnTypeComboBox = new System.Windows.Forms.ComboBox();
            this.typeLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.agentLabel = new System.Windows.Forms.Label();
            this.descGroupBox.SuspendLayout();
            this.panel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // descGroupBox
            // 
            resources.ApplyResources(this.descGroupBox, "descGroupBox");
            this.descGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.descGroupBox.Controls.Add(this.isPublicCheckBox);
            this.descGroupBox.Controls.Add(this.isStaticCheckBox);
            this.descGroupBox.Controls.Add(this.arrayCheckBox);
            this.descGroupBox.Controls.Add(this.descTextBox);
            this.descGroupBox.Controls.Add(this.descLabel);
            this.descGroupBox.Controls.Add(this.dispTextBox);
            this.descGroupBox.Controls.Add(this.dispLabel);
            this.descGroupBox.Controls.Add(this.panel);
            this.descGroupBox.Controls.Add(this.removeParamButton);
            this.descGroupBox.Controls.Add(this.addParamButton);
            this.descGroupBox.Controls.Add(this.paramLabel);
            this.descGroupBox.Controls.Add(this.returnTypeComboBox);
            this.descGroupBox.Controls.Add(this.typeLabel);
            this.descGroupBox.Controls.Add(this.nameTextBox);
            this.descGroupBox.Controls.Add(this.agentLabel);
            this.descGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.descGroupBox.Name = "descGroupBox";
            this.descGroupBox.TabStop = false;
            // 
            // isPublicCheckBox
            // 
            resources.ApplyResources(this.isPublicCheckBox, "isPublicCheckBox");
            this.isPublicCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.isPublicCheckBox.Checked = true;
            this.isPublicCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.isPublicCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.isPublicCheckBox.Name = "isPublicCheckBox";
            this.isPublicCheckBox.UseVisualStyleBackColor = false;
            this.isPublicCheckBox.CheckedChanged += new System.EventHandler(this.isPublicCheckBox_CheckedChanged);
            // 
            // isStaticCheckBox
            // 
            resources.ApplyResources(this.isStaticCheckBox, "isStaticCheckBox");
            this.isStaticCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.isStaticCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.isStaticCheckBox.Name = "isStaticCheckBox";
            this.isStaticCheckBox.UseVisualStyleBackColor = false;
            this.isStaticCheckBox.CheckedChanged += new System.EventHandler(this.isStaticCheckBox_CheckedChanged);
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
            // descTextBox
            // 
            resources.ApplyResources(this.descTextBox, "descTextBox");
            this.descTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.descTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.descTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.descTextBox.Name = "descTextBox";
            this.descTextBox.TextChanged += new System.EventHandler(this.descTextBox_TextChanged);
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
            // 
            // dispLabel
            // 
            resources.ApplyResources(this.dispLabel, "dispLabel");
            this.dispLabel.Name = "dispLabel";
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Controls.Add(this.tableLayoutPanel);
            this.panel.Name = "panel";
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.displaynameLabel, 6, 0);
            this.tableLayoutPanel.Controls.Add(this.byReferLabel, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.parameterLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.isArrayLabel, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.nameLabel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.isConstlabel, 5, 0);
            this.tableLayoutPanel.ForeColor = System.Drawing.Color.LightGray;
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // displaynameLabel
            // 
            resources.ApplyResources(this.displaynameLabel, "displaynameLabel");
            this.displaynameLabel.ForeColor = System.Drawing.Color.LightGray;
            this.displaynameLabel.Name = "displaynameLabel";
            // 
            // byReferLabel
            // 
            resources.ApplyResources(this.byReferLabel, "byReferLabel");
            this.byReferLabel.ForeColor = System.Drawing.Color.LightGray;
            this.byReferLabel.Name = "byReferLabel";
            // 
            // parameterLabel
            // 
            resources.ApplyResources(this.parameterLabel, "parameterLabel");
            this.parameterLabel.ForeColor = System.Drawing.Color.LightGray;
            this.parameterLabel.Name = "parameterLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Name = "label1";
            // 
            // isArrayLabel
            // 
            resources.ApplyResources(this.isArrayLabel, "isArrayLabel");
            this.isArrayLabel.ForeColor = System.Drawing.Color.LightGray;
            this.isArrayLabel.Name = "isArrayLabel";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.ForeColor = System.Drawing.Color.LightGray;
            this.nameLabel.Name = "nameLabel";
            // 
            // isConstlabel
            // 
            resources.ApplyResources(this.isConstlabel, "isConstlabel");
            this.isConstlabel.ForeColor = System.Drawing.Color.LightGray;
            this.isConstlabel.Name = "isConstlabel";
            // 
            // removeParamButton
            // 
            resources.ApplyResources(this.removeParamButton, "removeParamButton");
            this.removeParamButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.removeParamButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.removeParamButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.removeParamButton.Name = "removeParamButton";
            this.removeParamButton.UseVisualStyleBackColor = false;
            this.removeParamButton.Click += new System.EventHandler(this.removeParamButton_Click);
            // 
            // addParamButton
            // 
            resources.ApplyResources(this.addParamButton, "addParamButton");
            this.addParamButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.addParamButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.addParamButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.addParamButton.Name = "addParamButton";
            this.addParamButton.UseVisualStyleBackColor = false;
            this.addParamButton.Click += new System.EventHandler(this.addParamButton_Click);
            // 
            // paramLabel
            // 
            resources.ApplyResources(this.paramLabel, "paramLabel");
            this.paramLabel.Name = "paramLabel";
            // 
            // returnTypeComboBox
            // 
            resources.ApplyResources(this.returnTypeComboBox, "returnTypeComboBox");
            this.returnTypeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.returnTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.returnTypeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.returnTypeComboBox.FormattingEnabled = true;
            this.returnTypeComboBox.Name = "returnTypeComboBox";
            this.returnTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.returnTypeComboBox_SelectedIndexChanged);
            // 
            // typeLabel
            // 
            resources.ApplyResources(this.typeLabel, "typeLabel");
            this.typeLabel.Name = "typeLabel";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged_1);
            this.nameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nameTextBox_KeyDown);
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_Leave);
            // 
            // agentLabel
            // 
            resources.ApplyResources(this.agentLabel, "agentLabel");
            this.agentLabel.Name = "agentLabel";
            // 
            // MetaMethodPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.descGroupBox);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "MetaMethodPanel";
            this.descGroupBox.ResumeLayout(false);
            this.descGroupBox.PerformLayout();
            this.panel.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox descGroupBox;
        private System.Windows.Forms.CheckBox isPublicCheckBox;
        private System.Windows.Forms.CheckBox isStaticCheckBox;
        private System.Windows.Forms.CheckBox arrayCheckBox;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.TextBox dispTextBox;
        private System.Windows.Forms.Label dispLabel;
        private System.Windows.Forms.Panel panel;
        private BehaviacTableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label displaynameLabel;
        private System.Windows.Forms.Label byReferLabel;
        private System.Windows.Forms.Label parameterLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label isArrayLabel;
        private System.Windows.Forms.Button removeParamButton;
        private System.Windows.Forms.Button addParamButton;
        private System.Windows.Forms.Label paramLabel;
        private System.Windows.Forms.ComboBox returnTypeComboBox;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label agentLabel;
        private System.Windows.Forms.Label isConstlabel;

    }
}
