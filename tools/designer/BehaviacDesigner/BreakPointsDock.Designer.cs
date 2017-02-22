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
    partial class BreakPointsDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BreakPointsDock));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel = new System.Windows.Forms.Panel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.showButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.enableButton = new System.Windows.Forms.ToolStripButton();
            this.disableButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteAllButton = new System.Windows.Forms.ToolStripButton();
            this.enableAllButton = new System.Windows.Forms.ToolStripButton();
            this.disableAllButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // panel
            //
            resources.ApplyResources(this.panel, "panel");
            this.panel.Controls.Add(this.dataGridView);
            this.panel.Controls.Add(this.toolStrip);
            this.panel.Name = "panel";
            //
            // dataGridView
            //
            resources.ApplyResources(this.dataGridView, "dataGridView");
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.Name = "dataGridView";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
            //
            // toolStrip
            //
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.showButton,
                this.toolStripSeparator1,
                this.deleteButton,
                this.enableButton,
                this.disableButton,
                this.toolStripSeparator2,
                this.deleteAllButton,
                this.enableAllButton,
                this.disableAllButton,
                this.toolStripSeparator3,
                this.saveButton
            });
            this.toolStrip.Name = "toolStrip";
            //
            // showButton
            //
            resources.ApplyResources(this.showButton, "showButton");
            this.showButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showButton.Name = "showButton";
            this.showButton.Click += new System.EventHandler(this.showButton_Click);
            //
            // toolStripSeparator1
            //
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            //
            // deleteButton
            //
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            //
            // enableButton
            //
            resources.ApplyResources(this.enableButton, "enableButton");
            this.enableButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.enableButton.Name = "enableButton";
            this.enableButton.Click += new System.EventHandler(this.enableButton_Click);
            //
            // disableButton
            //
            resources.ApplyResources(this.disableButton, "disableButton");
            this.disableButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.disableButton.Name = "disableButton";
            this.disableButton.Click += new System.EventHandler(this.disableButton_Click);
            //
            // toolStripSeparator2
            //
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            //
            // deleteAllButton
            //
            resources.ApplyResources(this.deleteAllButton, "deleteAllButton");
            this.deleteAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteAllButton.Name = "deleteAllButton";
            this.deleteAllButton.Click += new System.EventHandler(this.deleteAllButton_Click);
            //
            // enableAllButton
            //
            resources.ApplyResources(this.enableAllButton, "enableAllButton");
            this.enableAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.enableAllButton.Name = "enableAllButton";
            this.enableAllButton.Click += new System.EventHandler(this.enableAllButton_Click);
            //
            // disableAllButton
            //
            resources.ApplyResources(this.disableAllButton, "disableAllButton");
            this.disableAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.disableAllButton.Name = "disableAllButton";
            this.disableAllButton.Click += new System.EventHandler(this.disableAllButton_Click);
            //
            // toolStripSeparator3
            //
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            //
            // saveButton
            //
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Name = "saveButton";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            //
            // BreakPointsDock
            //
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.panel);
            this.Name = "BreakPointsDock";
            this.ShowIcon = false;
            this.TabText = "Breakpoints";
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripButton enableButton;
        private System.Windows.Forms.ToolStripButton disableButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripButton showButton;
        private System.Windows.Forms.ToolStripButton deleteAllButton;
        private System.Windows.Forms.ToolStripButton enableAllButton;
        private System.Windows.Forms.ToolStripButton disableAllButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
