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
    partial class ErrorInfoDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorInfoDock));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.clearAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorListBox = new System.Windows.Forms.ListBox();
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
            // errorListBox
            //
            this.errorListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.errorListBox.ContextMenuStrip = this.contextMenu;
            resources.ApplyResources(this.errorListBox, "errorListBox");
            this.errorListBox.ForeColor = System.Drawing.Color.LightGray;
            this.errorListBox.FormattingEnabled = true;
            this.errorListBox.Name = "errorListBox";
            this.errorListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.errorListBox.SelectedIndexChanged += new System.EventHandler(this.logListBox_SelectedIndexChanged);
            this.errorListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.logListBox_KeyDown);
            //
            // ErrorInfoDock
            //
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.errorListBox);
            this.Name = "ErrorInfoDock";
            this.ShowIcon = false;
            this.TabText = "Error List";
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox errorListBox;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;

    }
}
