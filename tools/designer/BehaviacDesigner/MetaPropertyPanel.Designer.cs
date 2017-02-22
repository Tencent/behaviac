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
    partial class MetaPropertyPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaPropertyPanel));
            this.descGroupBox = new System.Windows.Forms.GroupBox();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.dispTextBox = new System.Windows.Forms.TextBox();
            this.valueLabel = new System.Windows.Forms.Label();
            this.dispLabel = new System.Windows.Forms.Label();
            this.descLabel = new System.Windows.Forms.Label();
            this.customizedCheckBox = new System.Windows.Forms.CheckBox();
            this.isLocalCheckBox = new System.Windows.Forms.CheckBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.isPublicCheckBox = new System.Windows.Forms.CheckBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.isConstCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.arrayCheckBox = new System.Windows.Forms.CheckBox();
            this.isStaticCheckBox = new System.Windows.Forms.CheckBox();
            this.descGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // descGroupBox
            // 
            resources.ApplyResources(this.descGroupBox, "descGroupBox");
            this.descGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.descGroupBox.Controls.Add(this.descTextBox);
            this.descGroupBox.Controls.Add(this.dispTextBox);
            this.descGroupBox.Controls.Add(this.valueLabel);
            this.descGroupBox.Controls.Add(this.dispLabel);
            this.descGroupBox.Controls.Add(this.descLabel);
            this.descGroupBox.Controls.Add(this.customizedCheckBox);
            this.descGroupBox.Controls.Add(this.isLocalCheckBox);
            this.descGroupBox.Controls.Add(this.nameLabel);
            this.descGroupBox.Controls.Add(this.isPublicCheckBox);
            this.descGroupBox.Controls.Add(this.nameTextBox);
            this.descGroupBox.Controls.Add(this.isConstCheckBox);
            this.descGroupBox.Controls.Add(this.label1);
            this.descGroupBox.Controls.Add(this.typeComboBox);
            this.descGroupBox.Controls.Add(this.arrayCheckBox);
            this.descGroupBox.Controls.Add(this.isStaticCheckBox);
            this.descGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.descGroupBox.Name = "descGroupBox";
            this.descGroupBox.TabStop = false;
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
            // dispTextBox
            // 
            resources.ApplyResources(this.dispTextBox, "dispTextBox");
            this.dispTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.dispTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dispTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.dispTextBox.Name = "dispTextBox";
            this.dispTextBox.TextChanged += new System.EventHandler(this.dispTextBox_TextChanged);
            // 
            // valueLabel
            // 
            resources.ApplyResources(this.valueLabel, "valueLabel");
            this.valueLabel.Name = "valueLabel";
            // 
            // dispLabel
            // 
            resources.ApplyResources(this.dispLabel, "dispLabel");
            this.dispLabel.Name = "dispLabel";
            // 
            // descLabel
            // 
            resources.ApplyResources(this.descLabel, "descLabel");
            this.descLabel.ForeColor = System.Drawing.Color.LightGray;
            this.descLabel.Name = "descLabel";
            // 
            // customizedCheckBox
            // 
            resources.ApplyResources(this.customizedCheckBox, "customizedCheckBox");
            this.customizedCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.customizedCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.customizedCheckBox.Name = "customizedCheckBox";
            this.customizedCheckBox.UseVisualStyleBackColor = false;
            this.customizedCheckBox.CheckedChanged += new System.EventHandler(this.customizedCheckBox_CheckedChanged);
            // 
            // isLocalCheckBox
            // 
            resources.ApplyResources(this.isLocalCheckBox, "isLocalCheckBox");
            this.isLocalCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.isLocalCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.isLocalCheckBox.Name = "isLocalCheckBox";
            this.isLocalCheckBox.UseVisualStyleBackColor = false;
            this.isLocalCheckBox.CheckedChanged += new System.EventHandler(this.isLocalCheckBox_CheckedChanged);
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.ForeColor = System.Drawing.Color.LightGray;
            this.nameLabel.Name = "nameLabel";
            // 
            // isPublicCheckBox
            // 
            resources.ApplyResources(this.isPublicCheckBox, "isPublicCheckBox");
            this.isPublicCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.isPublicCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.isPublicCheckBox.Name = "isPublicCheckBox";
            this.isPublicCheckBox.UseVisualStyleBackColor = false;
            this.isPublicCheckBox.CheckedChanged += new System.EventHandler(this.isPublicCheckBox_CheckedChanged);
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            this.nameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nameTextBox_KeyDown);
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_Leave);
            // 
            // isConstCheckBox
            // 
            resources.ApplyResources(this.isConstCheckBox, "isConstCheckBox");
            this.isConstCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.isConstCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.isConstCheckBox.Name = "isConstCheckBox";
            this.isConstCheckBox.UseVisualStyleBackColor = false;
            this.isConstCheckBox.CheckedChanged += new System.EventHandler(this.isConstcheckBox_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.LightGray;
            this.label1.Name = "label1";
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
            // arrayCheckBox
            // 
            resources.ApplyResources(this.arrayCheckBox, "arrayCheckBox");
            this.arrayCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.arrayCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.arrayCheckBox.Name = "arrayCheckBox";
            this.arrayCheckBox.UseVisualStyleBackColor = false;
            this.arrayCheckBox.CheckedChanged += new System.EventHandler(this.arrayCheckBox_CheckedChanged);
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
            // MetaPropertyPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.descGroupBox);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "MetaPropertyPanel";
            this.descGroupBox.ResumeLayout(false);
            this.descGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox descGroupBox;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.TextBox dispTextBox;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.Label dispLabel;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.CheckBox customizedCheckBox;
        private System.Windows.Forms.CheckBox isLocalCheckBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.CheckBox isPublicCheckBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.CheckBox isConstCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.CheckBox arrayCheckBox;
        private System.Windows.Forms.CheckBox isStaticCheckBox;

    }
}
