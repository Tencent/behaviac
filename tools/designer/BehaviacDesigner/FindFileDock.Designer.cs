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
    partial class FindFileDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindFileDock));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultListView = new System.Windows.Forms.ListView();
            this.logColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.resultLabel = new System.Windows.Forms.Label();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // contextMenu
            //
            this.contextMenu.BackColor = System.Drawing.Color.DimGray;
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.selectAllMenuItem,
                this.copyMenuItem
            });
            this.contextMenu.Name = "contextMenu";
            resources.ApplyResources(this.contextMenu, "contextMenu");
            //
            // selectAllMenuItem
            //
            this.selectAllMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.selectAllMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.selectAllMenuItem.Name = "selectAllMenuItem";
            resources.ApplyResources(this.selectAllMenuItem, "selectAllMenuItem");
            this.selectAllMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
            //
            // copyMenuItem
            //
            this.copyMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.copyMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.copyMenuItem.Name = "copyMenuItem";
            resources.ApplyResources(this.copyMenuItem, "copyMenuItem");
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            //
            // resultListView
            //
            resources.ApplyResources(this.resultListView, "resultListView");
            this.resultListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.resultListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
            {
                this.logColumnHeader
            });
            this.resultListView.ContextMenuStrip = this.contextMenu;
            this.resultListView.ForeColor = System.Drawing.Color.LightGray;
            this.resultListView.FullRowSelect = true;
            this.resultListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.resultListView.Name = "resultListView";
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            this.resultListView.VirtualMode = true;
            this.resultListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.resultListView_RetrieveVirtualItem);
            this.resultListView.SizeChanged += new System.EventHandler(this.resultListView_SizeChanged);
            this.resultListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.resultListView_KeyDown);
            this.resultListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.resultListView_MouseDoubleClick);
            this.resultListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.resultListView_MouseDown);
            this.resultListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.resultListView_MouseMove);
            this.resultListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.resultListView_MouseUp);
            //
            // logColumnHeader
            //
            resources.ApplyResources(this.logColumnHeader, "logColumnHeader");
            //
            // resultLabel
            //
            resources.ApplyResources(this.resultLabel, "resultLabel");
            this.resultLabel.Name = "resultLabel";
            //
            // FindFileDock
            //
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.resultListView);
            this.Controls.Add(this.resultLabel);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "FindFileDock";
            this.ShowIcon = false;
            this.TabText = "Output";
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ListView resultListView;
        private System.Windows.Forms.ColumnHeader logColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
        private System.Windows.Forms.Label resultLabel;

    }
}
