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
    partial class CallStackDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CallStackDock));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.callstackListBox = new System.Windows.Forms.ListBox();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // contextMenu
            //
            this.contextMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.copyMenuItem,
                this.toolStripSeparator,
                this.clearAllMenuItem
            });
            this.contextMenu.Name = "contextMenu";
            resources.ApplyResources(this.contextMenu, "contextMenu");
            //
            // copyMenuItem
            //
            this.copyMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.copyMenuItem.ForeColor = System.Drawing.Color.LightGray;
            this.copyMenuItem.Name = "copyMenuItem";
            resources.ApplyResources(this.copyMenuItem, "copyMenuItem");
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            //
            // toolStripSeparator
            //
            this.toolStripSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.toolStripSeparator.ForeColor = System.Drawing.Color.LightGray;
            this.toolStripSeparator.Name = "toolStripSeparator";
            resources.ApplyResources(this.toolStripSeparator, "toolStripSeparator");
            //
            // clearAllMenuItem
            //
            this.clearAllMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.clearAllMenuItem.ForeColor = System.Drawing.Color.LightGray;
            this.clearAllMenuItem.Name = "clearAllMenuItem";
            resources.ApplyResources(this.clearAllMenuItem, "clearAllMenuItem");
            this.clearAllMenuItem.Click += new System.EventHandler(this.clearAllMenuItem_Click);
            //
            // callstackListBox
            //
            this.callstackListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.callstackListBox.ContextMenuStrip = this.contextMenu;
            resources.ApplyResources(this.callstackListBox, "callstackListBox");
            this.callstackListBox.ForeColor = System.Drawing.Color.LightGray;
            this.callstackListBox.FormattingEnabled = true;
            this.callstackListBox.Name = "callstackListBox";
            this.callstackListBox.SelectedIndexChanged += new System.EventHandler(this.logListBox_SelectedIndexChanged);
            this.callstackListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.logListBox_KeyDown);
            //
            // CallStackDock
            //
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.callstackListBox);
            this.Name = "CallStackDock";
            this.ShowIcon = false;
            this.TabText = "Tree Exec Stack";
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox callstackListBox;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;

    }
}
