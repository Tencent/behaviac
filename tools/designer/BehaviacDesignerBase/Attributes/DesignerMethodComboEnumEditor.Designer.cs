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
    partial class DesignerMethodComboEnumEditor
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
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.valueComboBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // flowLayoutPanel
            //
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.Controls.Add(this.typeComboBox);
            this.flowLayoutPanel.Controls.Add(this.valueComboBox);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(277, 24);
            this.flowLayoutPanel.TabIndex = 2;
            this.flowLayoutPanel.Resize += new System.EventHandler(this.flowLayoutPanel_Resize);
            //
            // typeComboBox
            //
            this.typeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.DropDownWidth = 140;
            this.typeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.typeComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(2, 1);
            this.typeComboBox.Margin = new System.Windows.Forms.Padding(2, 1, 1, 1);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(90, 20);
            this.typeComboBox.TabIndex = 0;
            this.typeComboBox.DropDown += new System.EventHandler(this.typeComboBox_DropDown);
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            this.typeComboBox.MouseEnter += new System.EventHandler(this.typeComboBox_MouseEnter);
            //
            // valueComboBox
            //
            this.valueComboBox.AllowDrop = true;
            this.valueComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.valueComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.valueComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.valueComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.valueComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.valueComboBox.ForeColor = System.Drawing.Color.LightGray;
            this.valueComboBox.FormattingEnabled = true;
            this.valueComboBox.Location = new System.Drawing.Point(94, 1);
            this.valueComboBox.Margin = new System.Windows.Forms.Padding(1);
            this.valueComboBox.Name = "valueComboBox";
            this.valueComboBox.Size = new System.Drawing.Size(182, 22);
            this.valueComboBox.TabIndex = 1;
            this.valueComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.valueComboBox_DrawItem);
            this.valueComboBox.DropDown += new System.EventHandler(this.valueComboBox_DropDown);
            this.valueComboBox.SelectedIndexChanged += new System.EventHandler(this.valueComboBox_SelectedIndexChanged);
            this.valueComboBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.valueComboBox_DragDrop);
            this.valueComboBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.valueComboBox_DragEnter);
            this.valueComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.valueComboBox_KeyPress);
            this.valueComboBox.MouseEnter += new System.EventHandler(this.valueComboBox_MouseEnter);
            this.valueComboBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.valueComboBox_PreviewKeyDown);
            //
            // DesignerMethodComboEnumEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "DesignerMethodComboEnumEditor";
            this.Size = new System.Drawing.Size(277, 24);
            this.flowLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox valueComboBox;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
    }
}
