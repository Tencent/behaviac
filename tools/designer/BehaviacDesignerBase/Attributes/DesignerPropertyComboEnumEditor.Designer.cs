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

namespace Behaviac.Design.Attributes
{
    partial class DesignerPropertyComboEnumEditor
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
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // typeComboBox
            //
            this.typeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.DropDownWidth = 140;
            this.typeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.typeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(2, 2);
            this.typeComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 1, 1);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(90, 20);
            this.typeComboBox.TabIndex = 0;
            this.typeComboBox.DropDown += new System.EventHandler(this.typeComboBox_DropDown);
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            this.typeComboBox.MouseEnter += new System.EventHandler(this.typeComboBox_MouseEnter);
            //
            // flowLayoutPanel
            //
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.Controls.Add(this.typeComboBox);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(269, 24);
            this.flowLayoutPanel.TabIndex = 2;
            this.flowLayoutPanel.Resize += new System.EventHandler(this.flowLayoutPanel_Resize);
            //
            // DesignerPropertyComboEnumEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "DesignerPropertyComboEnumEditor";
            this.Size = new System.Drawing.Size(269, 24);
            this.flowLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private DesignerPropertyEditor propertyEditor = null;
        private System.Windows.Forms.ComboBox typeComboBox;
    }
}
