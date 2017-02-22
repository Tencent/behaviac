////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Behaviac.Design
{
    partial class BehaviorTreeViewDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BehaviorTreeViewDock));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeOthersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyNameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.floatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // contextMenu
            //
            resources.ApplyResources(this.contextMenu, "contextMenu");
            this.contextMenu.BackColor = System.Drawing.Color.DarkGray;
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.saveMenuItem,
                this.closeMenuItem,
                this.closeOthersMenuItem,
                this.toolStripSeparator1,
                this.copyNameMenuItem,
                this.showMenuItem,
                this.openFolderMenuItem,
                this.toolStripSeparator2,
                this.floatMenuItem,
                this.dockMenuItem
            });
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            //
            // saveMenuItem
            //
            resources.ApplyResources(this.saveMenuItem, "saveMenuItem");
            this.saveMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.saveMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            //
            // closeMenuItem
            //
            resources.ApplyResources(this.closeMenuItem, "closeMenuItem");
            this.closeMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.closeMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Click += new System.EventHandler(this.closeMenuItem_Click);
            //
            // closeOthersMenuItem
            //
            resources.ApplyResources(this.closeOthersMenuItem, "closeOthersMenuItem");
            this.closeOthersMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.closeOthersMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.closeOthersMenuItem.Name = "closeOthersMenuItem";
            this.closeOthersMenuItem.Click += new System.EventHandler(this.closeOthersMenuItem_Click);
            //
            // toolStripSeparator1
            //
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            //
            // copyNameMenuItem
            //
            resources.ApplyResources(this.copyNameMenuItem, "copyNameMenuItem");
            this.copyNameMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.copyNameMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.copyNameMenuItem.Name = "copyNameMenuItem";
            this.copyNameMenuItem.Click += new System.EventHandler(this.copyNameMenuItem_Click);
            //
            // showMenuItem
            //
            resources.ApplyResources(this.showMenuItem, "showMenuItem");
            this.showMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.showMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.showMenuItem.Name = "showMenuItem";
            this.showMenuItem.Click += new System.EventHandler(this.showMenuItem_Click);
            //
            // openFolderMenuItem
            //
            resources.ApplyResources(this.openFolderMenuItem, "openFolderMenuItem");
            this.openFolderMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.openFolderMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.openFolderMenuItem.Name = "openFolderMenuItem";
            this.openFolderMenuItem.Click += new System.EventHandler(this.openFolderMenuItem_Click);
            //
            // toolStripSeparator2
            //
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            //
            // floatMenuItem
            //
            resources.ApplyResources(this.floatMenuItem, "floatMenuItem");
            this.floatMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.floatMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.floatMenuItem.Name = "floatMenuItem";
            this.floatMenuItem.Click += new System.EventHandler(this.floatMenuItem_Click);
            //
            // dockMenuItem
            //
            resources.ApplyResources(this.dockMenuItem, "dockMenuItem");
            this.dockMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.dockMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.dockMenuItem.Name = "dockMenuItem";
            this.dockMenuItem.Click += new System.EventHandler(this.dockMenuItem_Click);
            //
            // BehaviorTreeViewDock
            //
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Name = "BehaviorTreeViewDock";
            this.TabText = "Behavior Tree";
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeOthersMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem copyNameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem floatMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dockMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showMenuItem;


    }
}
