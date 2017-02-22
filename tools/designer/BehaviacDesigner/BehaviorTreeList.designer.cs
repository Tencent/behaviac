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
    partial class BehaviorTreeList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BehaviorTreeList));
            this.treeView = new System.Windows.Forms.TreeView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openBehaviorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newBehaviorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createGroupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.saveBehaviorContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsBehaviorContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_workspace = new System.Windows.Forms.ToolStripButton();
            this.connectButton = new System.Windows.Forms.ToolStripButton();
            this.analyzeDumpButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.exportAllButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.newBehaviorButton = new System.Windows.Forms.ToolStripButton();
            this.createGroupButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.findFileButton = new System.Windows.Forms.ToolStripButton();
            this.expandButton = new System.Windows.Forms.ToolStripButton();
            this.collapseButton = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // treeView
            //
            this.treeView.AllowDrop = true;
            this.treeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.treeView.ContextMenuStrip = this.contextMenuStrip;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.ForeColor = System.Drawing.Color.LightGray;
            this.treeView.FullRowSelect = true;
            this.treeView.ImageList = this.imageList;
            this.treeView.LabelEdit = true;
            this.treeView.Name = "treeView";
            this.treeView.ShowNodeToolTips = true;
            this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_BeforeLabelEdit);
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.behaviorTreeView_NodeMouseDoubleClick);
            this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
            this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyDown);
            //
            // contextMenuStrip
            //
            this.contextMenuStrip.BackColor = System.Drawing.Color.DarkGray;
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.expandMenuItem,
                this.collapseMenuItem,
                this.toolStripSeparator4,
                this.openBehaviorMenuItem,
                this.newBehaviorMenuItem,
                this.createGroupMenuItem,
                this.renameMenuItem,
                this.deleteMenuItem,
                this.toolStripSeparator10,
                this.saveBehaviorContextMenuItem,
                this.saveAsBehaviorContextMenuItem,
                this.exportMenuItem
            });
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            //
            // expandMenuItem
            //
            this.expandMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.expandMenuItem.Name = "expandMenuItem";
            resources.ApplyResources(this.expandMenuItem, "expandMenuItem");
            this.expandMenuItem.Click += new System.EventHandler(this.expandMenuItem_Click);
            //
            // collapseMenuItem
            //
            this.collapseMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.collapseMenuItem.Name = "collapseMenuItem";
            resources.ApplyResources(this.collapseMenuItem, "collapseMenuItem");
            this.collapseMenuItem.Click += new System.EventHandler(this.collapseMenuItem_Click);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            //
            // openBehaviorMenuItem
            //
            this.openBehaviorMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.openBehaviorMenuItem.Name = "openBehaviorMenuItem";
            resources.ApplyResources(this.openBehaviorMenuItem, "openBehaviorMenuItem");
            this.openBehaviorMenuItem.Click += new System.EventHandler(this.openBehaviorMenuItem_Click);
            //
            // newBehaviorMenuItem
            //
            this.newBehaviorMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.newBehaviorMenuItem.Name = "newBehaviorMenuItem";
            resources.ApplyResources(this.newBehaviorMenuItem, "newBehaviorMenuItem");
            this.newBehaviorMenuItem.Click += new System.EventHandler(this.newBehaviorMenuItem_Click);
            //
            // createGroupMenuItem
            //
            this.createGroupMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.createGroupMenuItem.Name = "createGroupMenuItem";
            resources.ApplyResources(this.createGroupMenuItem, "createGroupMenuItem");
            this.createGroupMenuItem.Click += new System.EventHandler(this.createGroupMenuItem_Click);
            //
            // renameMenuItem
            //
            this.renameMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.renameMenuItem.Name = "renameMenuItem";
            resources.ApplyResources(this.renameMenuItem, "renameMenuItem");
            this.renameMenuItem.Click += new System.EventHandler(this.renameMenuItem_Click);
            //
            // deleteMenuItem
            //
            this.deleteMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.deleteMenuItem.Name = "deleteMenuItem";
            resources.ApplyResources(this.deleteMenuItem, "deleteMenuItem");
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            //
            // toolStripSeparator10
            //
            this.toolStripSeparator10.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            //
            // saveBehaviorContextMenuItem
            //
            this.saveBehaviorContextMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.saveBehaviorContextMenuItem.Name = "saveBehaviorContextMenuItem";
            resources.ApplyResources(this.saveBehaviorContextMenuItem, "saveBehaviorContextMenuItem");
            this.saveBehaviorContextMenuItem.Click += new System.EventHandler(this.saveBehaviorContextMenuItem_Click);
            //
            // saveAsBehaviorContextMenuItem
            //
            this.saveAsBehaviorContextMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.saveAsBehaviorContextMenuItem.Name = "saveAsBehaviorContextMenuItem";
            resources.ApplyResources(this.saveAsBehaviorContextMenuItem, "saveAsBehaviorContextMenuItem");
            this.saveAsBehaviorContextMenuItem.Click += new System.EventHandler(this.saveAsBehaviorContextMenuItem_Click);
            //
            // exportMenuItem
            //
            this.exportMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.exportMenuItem.Name = "exportMenuItem";
            resources.ApplyResources(this.exportMenuItem, "exportMenuItem");
            this.exportMenuItem.Click += new System.EventHandler(this.exportMenuItem_Click);
            //
            // imageList
            //
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "flag_blue");
            this.imageList.Images.SetKeyName(1, "flag_green");
            this.imageList.Images.SetKeyName(2, "flag_red");
            this.imageList.Images.SetKeyName(3, "behavior");
            this.imageList.Images.SetKeyName(4, "behavior_loaded");
            this.imageList.Images.SetKeyName(5, "behavior_modified");
            this.imageList.Images.SetKeyName(6, "condition");
            this.imageList.Images.SetKeyName(7, "impulse");
            this.imageList.Images.SetKeyName(8, "action");
            this.imageList.Images.SetKeyName(9, "decorator");
            this.imageList.Images.SetKeyName(10, "sequence");
            this.imageList.Images.SetKeyName(11, "selector");
            this.imageList.Images.SetKeyName(12, "parallel");
            this.imageList.Images.SetKeyName(13, "folder_closed");
            this.imageList.Images.SetKeyName(14, "folder_open");
            this.imageList.Images.SetKeyName(15, "event");
            this.imageList.Images.SetKeyName(16, "override");
            this.imageList.Images.SetKeyName(17, "primitiveTask");
            this.imageList.Images.SetKeyName(18, "compoundTask");
            this.imageList.Images.SetKeyName(19, "method");
            this.imageList.Images.SetKeyName(20, "precondition");
            this.imageList.Images.SetKeyName(21, "operator");
            this.imageList.Images.SetKeyName(22, "prefab");
            //
            // timer
            //
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            //
            // toolStrip
            //
            this.toolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.toolStripButton_workspace,
                this.connectButton,
                this.analyzeDumpButton,
                this.toolStripSeparator3,
                this.refreshButton,
                this.exportAllButton,
                this.toolStripSeparator12,
                this.newBehaviorButton,
                this.createGroupButton,
                this.deleteButton,
                this.toolStripSeparator5,
                this.findFileButton,
                this.expandButton,
                this.collapseButton
            });
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Name = "toolStrip";
            //
            // toolStripButton_workspace
            //
            this.toolStripButton_workspace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripButton_workspace, "toolStripButton_workspace");
            this.toolStripButton_workspace.Name = "toolStripButton_workspace";
            this.toolStripButton_workspace.Click += new System.EventHandler(this.toolStripButton_workspace_Click);
            //
            // connectButton
            //
            this.connectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.connectButton.Image = global::Behaviac.Design.Properties.Resources.connect;
            resources.ApplyResources(this.connectButton, "connectButton");
            this.connectButton.Name = "connectButton";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            //
            // analyzeDumpButton
            //
            this.analyzeDumpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.analyzeDumpButton.Image = global::Behaviac.Design.Properties.Resources.File_Open;
            resources.ApplyResources(this.analyzeDumpButton, "analyzeDumpButton");
            this.analyzeDumpButton.Name = "analyzeDumpButton";
            this.analyzeDumpButton.Click += new System.EventHandler(this.toolStripButton_dump_Click);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            //
            // refreshButton
            //
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            //
            // exportAllButton
            //
            this.exportAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.exportAllButton, "exportAllButton");
            this.exportAllButton.Name = "exportAllButton";
            this.exportAllButton.Click += new System.EventHandler(this.exportAllButton_Click);
            //
            // toolStripSeparator12
            //
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            //
            // newBehaviorButton
            //
            this.newBehaviorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.newBehaviorButton, "newBehaviorButton");
            this.newBehaviorButton.Name = "newBehaviorButton";
            this.newBehaviorButton.Click += new System.EventHandler(this.newBehaviorButton_Click);
            //
            // createGroupButton
            //
            this.createGroupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.createGroupButton, "createGroupButton");
            this.createGroupButton.Name = "createGroupButton";
            this.createGroupButton.Click += new System.EventHandler(this.createGroupButton_Click);
            //
            // deleteButton
            //
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            //
            // toolStripSeparator5
            //
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            //
            // findFileButton
            //
            this.findFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.findFileButton, "findFileButton");
            this.findFileButton.Name = "findFileButton";
            this.findFileButton.Click += new System.EventHandler(this.findFileButton_Click);
            //
            // expandButton
            //
            this.expandButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.expandButton, "expandButton");
            this.expandButton.Name = "expandButton";
            this.expandButton.Click += new System.EventHandler(this.expandButton_Click);
            //
            // collapseButton
            //
            this.collapseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.collapseButton, "collapseButton");
            this.collapseButton.Name = "collapseButton";
            this.collapseButton.Click += new System.EventHandler(this.collapseButton_Click);
            //
            // openFileDialog
            //
            this.openFileDialog.DefaultExt = "workspace";
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            //
            // BehaviorTreeList
            //
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.toolStrip);
            this.Name = "BehaviorTreeList";
            this.contextMenuStrip.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton newBehaviorButton;
        private System.Windows.Forms.ToolStripButton createGroupButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripButton toolStripButton_workspace;
        private System.Windows.Forms.ToolStripButton connectButton;
        private System.Windows.Forms.ToolStripButton analyzeDumpButton;
        private System.Windows.Forms.ToolStripButton expandButton;
        private System.Windows.Forms.ToolStripButton collapseButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem expandMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem collapseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportMenuItem;
        private System.Windows.Forms.ToolStripButton exportAllButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripButton findFileButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem newBehaviorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createGroupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBehaviorContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsBehaviorContextMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openBehaviorMenuItem;
    }
}
