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
    partial class EditEnumMemberPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditEnumMemberPanel));
            this.descGroupBox = new System.Windows.Forms.GroupBox();
            this.valueNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.valueLabel = new System.Windows.Forms.Label();
            this.dispTextBox = new System.Windows.Forms.TextBox();
            this.dispLabel = new System.Windows.Forms.Label();
            this.descTextBox = new System.Windows.Forms.TextBox();
            this.descLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.descGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // descGroupBox
            // 
            resources.ApplyResources(this.descGroupBox, "descGroupBox");
            this.descGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.descGroupBox.Controls.Add(this.valueNumericUpDown);
            this.descGroupBox.Controls.Add(this.valueLabel);
            this.descGroupBox.Controls.Add(this.dispTextBox);
            this.descGroupBox.Controls.Add(this.dispLabel);
            this.descGroupBox.Controls.Add(this.descTextBox);
            this.descGroupBox.Controls.Add(this.descLabel);
            this.descGroupBox.Controls.Add(this.nameTextBox);
            this.descGroupBox.Controls.Add(this.nameLabel);
            this.descGroupBox.ForeColor = System.Drawing.Color.LightGray;
            this.descGroupBox.Name = "descGroupBox";
            this.descGroupBox.TabStop = false;
            // 
            // valueNumericUpDown
            // 
            resources.ApplyResources(this.valueNumericUpDown, "valueNumericUpDown");
            this.valueNumericUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.valueNumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueNumericUpDown.ForeColor = System.Drawing.Color.LightGray;
            this.valueNumericUpDown.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.valueNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.valueNumericUpDown.Name = "valueNumericUpDown";
            this.valueNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // valueLabel
            // 
            resources.ApplyResources(this.valueLabel, "valueLabel");
            this.valueLabel.Name = "valueLabel";
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
            // descTextBox
            // 
            resources.ApplyResources(this.descTextBox, "descTextBox");
            this.descTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.descTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.descTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.descTextBox.Name = "descTextBox";
            // 
            // descLabel
            // 
            resources.ApplyResources(this.descLabel, "descLabel");
            this.descLabel.ForeColor = System.Drawing.Color.LightGray;
            this.descLabel.Name = "descLabel";
            // 
            // nameTextBox
            // 
            resources.ApplyResources(this.nameTextBox, "nameTextBox");
            this.nameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.ForeColor = System.Drawing.Color.LightGray;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.ForeColor = System.Drawing.Color.LightGray;
            this.nameLabel.Name = "nameLabel";
            // 
            // EditEnumMemberPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.descGroupBox);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "EditEnumMemberPanel";
            this.descGroupBox.ResumeLayout(false);
            this.descGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox descGroupBox;
        private System.Windows.Forms.NumericUpDown valueNumericUpDown;
        private System.Windows.Forms.Label valueLabel;
        private System.Windows.Forms.TextBox dispTextBox;
        private System.Windows.Forms.Label dispLabel;
        private System.Windows.Forms.TextBox descTextBox;
        private System.Windows.Forms.Label descLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label nameLabel;

    }
}
