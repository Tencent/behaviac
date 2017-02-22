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
    partial class ConsoleDock
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleDock));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logListView = new System.Windows.Forms.ListView();
            this.logColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.limitCountCheckBox = new System.Windows.Forms.CheckBox();
            this.logCountNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.logCountLabel = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCopyAll = new System.Windows.Forms.Button();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logCountNumericUpDown)).BeginInit();
            this.panel.SuspendLayout();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // contextMenu
            //
            resources.ApplyResources(this.contextMenu, "contextMenu");
            this.contextMenu.BackColor = System.Drawing.Color.DarkGray;
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.copyMenuItem
            });
            this.contextMenu.Name = "contextMenu";
            //
            // copyMenuItem
            //
            resources.ApplyResources(this.copyMenuItem, "copyMenuItem");
            this.copyMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.copyMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            //
            // logListView
            //
            resources.ApplyResources(this.logListView, "logListView");
            this.logListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.logListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
            {
                this.logColumnHeader
            });
            this.logListView.ContextMenuStrip = this.contextMenu;
            this.logListView.ForeColor = System.Drawing.Color.LightGray;
            this.logListView.FullRowSelect = true;
            this.logListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.logListView.Name = "logListView";
            this.logListView.UseCompatibleStateImageBehavior = false;
            this.logListView.View = System.Windows.Forms.View.Details;
            this.logListView.VirtualMode = true;
            this.logListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.logListView_RetrieveVirtualItem);
            this.logListView.SizeChanged += new System.EventHandler(this.logListView_SizeChanged);
            this.logListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.logListView_KeyDown);
            this.logListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.logListView_MouseDown);
            this.logListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.logListView_MouseMove);
            this.logListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.logListView_MouseUp);
            //
            // logColumnHeader
            //
            resources.ApplyResources(this.logColumnHeader, "logColumnHeader");
            //
            // limitCountCheckBox
            //
            resources.ApplyResources(this.limitCountCheckBox, "limitCountCheckBox");
            this.limitCountCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.limitCountCheckBox.Name = "limitCountCheckBox";
            this.limitCountCheckBox.UseVisualStyleBackColor = true;
            this.limitCountCheckBox.CheckedChanged += new System.EventHandler(this.limitCountCheckBox_CheckedChanged);
            //
            // logCountNumericUpDown
            //
            resources.ApplyResources(this.logCountNumericUpDown, "logCountNumericUpDown");
            this.logCountNumericUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.logCountNumericUpDown.ForeColor = System.Drawing.Color.LightGray;
            this.logCountNumericUpDown.Maximum = new decimal(new int[]
            {
                100000000,
                0,
                0,
                0
            });
            this.logCountNumericUpDown.Minimum = new decimal(new int[]
            {
                1,
                0,
                0,
                0
            });
            this.logCountNumericUpDown.Name = "logCountNumericUpDown";
            this.logCountNumericUpDown.Value = new decimal(new int[]
            {
                10000,
                0,
                0,
                0
            });
            this.logCountNumericUpDown.ValueChanged += new System.EventHandler(this.logCountNumericUpDown_ValueChanged);
            //
            // logCountLabel
            //
            resources.ApplyResources(this.logCountLabel, "logCountLabel");
            this.logCountLabel.ForeColor = System.Drawing.Color.LightGray;
            this.logCountLabel.Name = "logCountLabel";
            //
            // panel
            //
            resources.ApplyResources(this.panel, "panel");
            this.panel.Controls.Add(this.flowLayoutPanel);
            this.panel.Controls.Add(this.logListView);
            this.panel.Name = "panel";
            //
            // flowLayoutPanel
            //
            resources.ApplyResources(this.flowLayoutPanel, "flowLayoutPanel");
            this.flowLayoutPanel.Controls.Add(this.limitCountCheckBox);
            this.flowLayoutPanel.Controls.Add(this.logCountLabel);
            this.flowLayoutPanel.Controls.Add(this.logCountNumericUpDown);
            this.flowLayoutPanel.Controls.Add(this.buttonCopyAll);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            //
            // buttonCopyAll
            //
            resources.ApplyResources(this.buttonCopyAll, "buttonCopyAll");
            this.buttonCopyAll.ForeColor = System.Drawing.Color.LightGray;
            this.buttonCopyAll.Name = "buttonCopyAll";
            this.buttonCopyAll.UseVisualStyleBackColor = true;
            this.buttonCopyAll.Click += new System.EventHandler(this.buttonCopyAll_Click);
            //
            // ConsoleDock
            //
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.panel);
            this.Name = "ConsoleDock";
            this.ShowIcon = false;
            this.TabText = "Output";
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logCountNumericUpDown)).EndInit();
            this.panel.ResumeLayout(false);
            this.flowLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ListView logListView;
        private System.Windows.Forms.ColumnHeader logColumnHeader;
        private System.Windows.Forms.CheckBox limitCountCheckBox;
        private System.Windows.Forms.NumericUpDown logCountNumericUpDown;
        private System.Windows.Forms.Label logCountLabel;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Button buttonCopyAll;

    }
}
