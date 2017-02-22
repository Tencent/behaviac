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
    partial class BehaviorTreeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BehaviorTreeView));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.saveButton = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.exportButton = new System.Windows.Forms.Button();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.imageButton = new System.Windows.Forms.Button();
            this.propertiesButton = new System.Windows.Forms.Button();
            this.parameterSettingButton = new System.Windows.Forms.Button();
            this.fitToViewButton = new System.Windows.Forms.Button();
            this.checkButton = new System.Windows.Forms.Button();
            this.zoomInButton = new System.Windows.Forms.Button();
            this.zoomOutButton = new System.Windows.Forms.Button();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fitToViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.docMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.expandMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutTreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySubtreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTreeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.disableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.breakpointMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enterBreakpointMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitBreakpointMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beakpointPlanning = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.referenceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.savePrefabMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakPrefabMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyButton = new System.Windows.Forms.Button();
            this.saveImageDialog = new System.Windows.Forms.SaveFileDialog();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // toolTip
            //
            this.toolTip.AutomaticDelay = 400;
            //
            // saveButton
            //
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.saveButton.ImageList = this.imageList;
            this.saveButton.Name = "saveButton";
            this.saveButton.TabStop = false;
            this.toolTip.SetToolTip(this.saveButton, resources.GetString("saveButton.ToolTip"));
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            //
            // imageList
            //
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Magenta;
            this.imageList.Images.SetKeyName(0, "close");
            this.imageList.Images.SetKeyName(1, "save");
            this.imageList.Images.SetKeyName(2, "export");
            this.imageList.Images.SetKeyName(3, "check");
            this.imageList.Images.SetKeyName(4, "saveas");
            this.imageList.Images.SetKeyName(5, "properties");
            this.imageList.Images.SetKeyName(6, "propertysheets");
            //
            // exportButton
            //
            resources.ApplyResources(this.exportButton, "exportButton");
            this.exportButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.exportButton.ImageList = this.imageList;
            this.exportButton.Name = "exportButton";
            this.exportButton.TabStop = false;
            this.toolTip.SetToolTip(this.exportButton, resources.GetString("exportButton.ToolTip"));
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            //
            // saveAsButton
            //
            resources.ApplyResources(this.saveAsButton, "saveAsButton");
            this.saveAsButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.saveAsButton.ImageList = this.imageList;
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.TabStop = false;
            this.toolTip.SetToolTip(this.saveAsButton, resources.GetString("saveAsButton.ToolTip"));
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            //
            // imageButton
            //
            resources.ApplyResources(this.imageButton, "imageButton");
            this.imageButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.imageButton.Name = "imageButton";
            this.imageButton.TabStop = false;
            this.toolTip.SetToolTip(this.imageButton, resources.GetString("imageButton.ToolTip"));
            this.imageButton.UseVisualStyleBackColor = true;
            this.imageButton.Click += new System.EventHandler(this.imageButton_Click);
            //
            // propertiesButton
            //
            resources.ApplyResources(this.propertiesButton, "propertiesButton");
            this.propertiesButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.propertiesButton.ImageList = this.imageList;
            this.propertiesButton.Name = "propertiesButton";
            this.propertiesButton.TabStop = false;
            this.toolTip.SetToolTip(this.propertiesButton, resources.GetString("propertiesButton.ToolTip"));
            this.propertiesButton.UseVisualStyleBackColor = true;
            this.propertiesButton.Click += new System.EventHandler(this.propertiesButton_Click);
            //
            // parameterSettingButton
            //
            resources.ApplyResources(this.parameterSettingButton, "parameterSettingButton");
            this.parameterSettingButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.parameterSettingButton.ImageList = this.imageList;
            this.parameterSettingButton.Name = "parameterSettingButton";
            this.parameterSettingButton.TabStop = false;
            this.toolTip.SetToolTip(this.parameterSettingButton, resources.GetString("parameterSettingButton.ToolTip"));
            this.parameterSettingButton.UseVisualStyleBackColor = true;
            this.parameterSettingButton.Click += new System.EventHandler(this.parameterSettingButton_Click);
            //
            // fitToViewButton
            //
            this.fitToViewButton.BackColor = System.Drawing.Color.DimGray;
            this.fitToViewButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.fitToViewButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGray;
            this.fitToViewButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.fitToViewButton, "fitToViewButton");
            this.fitToViewButton.Name = "fitToViewButton";
            this.fitToViewButton.TabStop = false;
            this.toolTip.SetToolTip(this.fitToViewButton, resources.GetString("fitToViewButton.ToolTip"));
            this.fitToViewButton.UseVisualStyleBackColor = false;
            this.fitToViewButton.Click += new System.EventHandler(this.fitToViewButton_Click);
            //
            // checkButton
            //
            resources.ApplyResources(this.checkButton, "checkButton");
            this.checkButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.checkButton.ImageList = this.imageList;
            this.checkButton.Name = "checkButton";
            this.checkButton.TabStop = false;
            this.toolTip.SetToolTip(this.checkButton, resources.GetString("checkButton.ToolTip"));
            this.checkButton.UseVisualStyleBackColor = true;
            this.checkButton.Click += new System.EventHandler(this.checkButton_Click);
            //
            // zoomInButton
            //
            this.zoomInButton.BackColor = System.Drawing.Color.DimGray;
            this.zoomInButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.zoomInButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGray;
            this.zoomInButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.zoomInButton, "zoomInButton");
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.TabStop = false;
            this.toolTip.SetToolTip(this.zoomInButton, resources.GetString("zoomInButton.ToolTip"));
            this.zoomInButton.UseVisualStyleBackColor = false;
            this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
            //
            // zoomOutButton
            //
            this.zoomOutButton.BackColor = System.Drawing.Color.DimGray;
            this.zoomOutButton.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.zoomOutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGray;
            this.zoomOutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            resources.ApplyResources(this.zoomOutButton, "zoomOutButton");
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.TabStop = false;
            this.toolTip.SetToolTip(this.zoomOutButton, resources.GetString("zoomOutButton.ToolTip"));
            this.zoomOutButton.UseVisualStyleBackColor = false;
            this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
            //
            // contextMenu
            //
            this.contextMenu.BackColor = System.Drawing.Color.DarkGray;
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.fitToViewMenuItem,
                this.toolStripSeparator7,
                this.docMenuItem,
                this.toolStripSeparator1,
                this.expandMenuItem,
                this.collapseMenuItem,
                this.expandAllMenuItem,
                this.collapseAllMenuItem,
                this.toolStripSeparator2,
                this.cutMenuItem,
                this.cutTreeMenuItem,
                this.copyMenuItem,
                this.copySubtreeMenuItem,
                this.pasteMenuItem,
                this.deleteMenuItem,
                this.deleteTreeMenuItem,
                this.toolStripSeparator4,
                this.disableMenuItem,
                this.toolStripSeparator6,
                this.breakpointMenuItem,
                this.toolStripSeparator3,
                this.referenceMenuItem,
                this.toolStripSeparator5,
                this.savePrefabMenuItem,
                this.applyMenuItem,
                this.breakPrefabMenuItem
            });
            this.contextMenu.Name = "contextMenu";
            resources.ApplyResources(this.contextMenu, "contextMenu");
            //
            // fitToViewMenuItem
            //
            this.fitToViewMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.fitToViewMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.fitToViewMenuItem.Name = "fitToViewMenuItem";
            resources.ApplyResources(this.fitToViewMenuItem, "fitToViewMenuItem");
            this.fitToViewMenuItem.Click += new System.EventHandler(this.fitToViewMenuItem_Click);
            //
            // toolStripSeparator7
            //
            this.toolStripSeparator7.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator7.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            //
            // docMenuItem
            //
            this.docMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.docMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.docMenuItem.Name = "docMenuItem";
            resources.ApplyResources(this.docMenuItem, "docMenuItem");
            this.docMenuItem.Click += new System.EventHandler(this.docMenuItem_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            //
            // expandMenuItem
            //
            this.expandMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.expandMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.expandMenuItem.Name = "expandMenuItem";
            resources.ApplyResources(this.expandMenuItem, "expandMenuItem");
            this.expandMenuItem.Click += new System.EventHandler(this.expandMenuItem_Click);
            //
            // collapseMenuItem
            //
            this.collapseMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.collapseMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.collapseMenuItem.Name = "collapseMenuItem";
            resources.ApplyResources(this.collapseMenuItem, "collapseMenuItem");
            this.collapseMenuItem.Click += new System.EventHandler(this.collapseMenuItem_Click);
            //
            // expandAllMenuItem
            //
            this.expandAllMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.expandAllMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.expandAllMenuItem.Name = "expandAllMenuItem";
            resources.ApplyResources(this.expandAllMenuItem, "expandAllMenuItem");
            this.expandAllMenuItem.Click += new System.EventHandler(this.expandAllMenuItem_Click);
            //
            // collapseAllMenuItem
            //
            this.collapseAllMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.collapseAllMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.collapseAllMenuItem.Name = "collapseAllMenuItem";
            resources.ApplyResources(this.collapseAllMenuItem, "collapseAllMenuItem");
            this.collapseAllMenuItem.Click += new System.EventHandler(this.collapseAllMenuItem_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            //
            // cutMenuItem
            //
            this.cutMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.cutMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cutMenuItem.Name = "cutMenuItem";
            resources.ApplyResources(this.cutMenuItem, "cutMenuItem");
            this.cutMenuItem.Click += new System.EventHandler(this.cutMenuItem_Click);
            //
            // cutTreeMenuItem
            //
            this.cutTreeMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.cutTreeMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cutTreeMenuItem.Name = "cutTreeMenuItem";
            resources.ApplyResources(this.cutTreeMenuItem, "cutTreeMenuItem");
            this.cutTreeMenuItem.Click += new System.EventHandler(this.cutTreeMenuItem_Click);
            //
            // copyMenuItem
            //
            this.copyMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.copyMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.copyMenuItem.Name = "copyMenuItem";
            resources.ApplyResources(this.copyMenuItem, "copyMenuItem");
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            //
            // copySubtreeMenuItem
            //
            this.copySubtreeMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.copySubtreeMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.copySubtreeMenuItem.Name = "copySubtreeMenuItem";
            resources.ApplyResources(this.copySubtreeMenuItem, "copySubtreeMenuItem");
            this.copySubtreeMenuItem.Click += new System.EventHandler(this.copySubtreeMenuItem_Click);
            //
            // pasteMenuItem
            //
            this.pasteMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.pasteMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.pasteMenuItem.Name = "pasteMenuItem";
            resources.ApplyResources(this.pasteMenuItem, "pasteMenuItem");
            this.pasteMenuItem.Click += new System.EventHandler(this.pasteMenuItem_Click);
            //
            // deleteMenuItem
            //
            this.deleteMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.deleteMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.deleteMenuItem.Name = "deleteMenuItem";
            resources.ApplyResources(this.deleteMenuItem, "deleteMenuItem");
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            //
            // deleteTreeMenuItem
            //
            this.deleteTreeMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.deleteTreeMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.deleteTreeMenuItem.Name = "deleteTreeMenuItem";
            resources.ApplyResources(this.deleteTreeMenuItem, "deleteTreeMenuItem");
            this.deleteTreeMenuItem.Click += new System.EventHandler(this.deleteTreeMenuItem_Click);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            //
            // disableMenuItem
            //
            this.disableMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.disableMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.disableMenuItem.Name = "disableMenuItem";
            resources.ApplyResources(this.disableMenuItem, "disableMenuItem");
            this.disableMenuItem.Click += new System.EventHandler(this.disableMenuItem_Click);
            //
            // toolStripSeparator6
            //
            this.toolStripSeparator6.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator6.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            //
            // breakpointMenuItem
            //
            this.breakpointMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.enterBreakpointMenuItem,
                this.exitBreakpointMenuItem,
                this.beakpointPlanning
            });
            this.breakpointMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.breakpointMenuItem.Name = "breakpointMenuItem";
            resources.ApplyResources(this.breakpointMenuItem, "breakpointMenuItem");
            //
            // enterBreakpointMenuItem
            //
            this.enterBreakpointMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.enterBreakpointMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.enterBreakpointMenuItem.Name = "enterBreakpointMenuItem";
            resources.ApplyResources(this.enterBreakpointMenuItem, "enterBreakpointMenuItem");
            this.enterBreakpointMenuItem.Click += new System.EventHandler(this.enterBreakpointMenuItem_Click);
            //
            // exitBreakpointMenuItem
            //
            this.exitBreakpointMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.exitBreakpointMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.exitBreakpointMenuItem.Name = "exitBreakpointMenuItem";
            resources.ApplyResources(this.exitBreakpointMenuItem, "exitBreakpointMenuItem");
            this.exitBreakpointMenuItem.Click += new System.EventHandler(this.exitBreakpointMenuItem_Click);
            //
            // beakpointPlanning
            //
            this.beakpointPlanning.BackColor = System.Drawing.Color.DarkGray;
            this.beakpointPlanning.ForeColor = System.Drawing.SystemColors.WindowText;
            this.beakpointPlanning.Name = "beakpointPlanning";
            resources.ApplyResources(this.beakpointPlanning, "beakpointPlanning");
            this.beakpointPlanning.Click += new System.EventHandler(this.enterBreakpointPlanning_Click);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator3.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            //
            // referenceMenuItem
            //
            this.referenceMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.referenceMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.referenceMenuItem.Name = "referenceMenuItem";
            resources.ApplyResources(this.referenceMenuItem, "referenceMenuItem");
            this.referenceMenuItem.Click += new System.EventHandler(this.referenceMenuItem_Click);
            //
            // toolStripSeparator5
            //
            this.toolStripSeparator5.BackColor = System.Drawing.Color.DarkGray;
            this.toolStripSeparator5.ForeColor = System.Drawing.SystemColors.WindowText;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            //
            // savePrefabMenuItem
            //
            this.savePrefabMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.savePrefabMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.savePrefabMenuItem.Name = "savePrefabMenuItem";
            resources.ApplyResources(this.savePrefabMenuItem, "savePrefabMenuItem");
            this.savePrefabMenuItem.Click += new System.EventHandler(this.savePrefabMenuItem_Click);
            //
            // applyMenuItem
            //
            this.applyMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.applyMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.applyMenuItem.Name = "applyMenuItem";
            resources.ApplyResources(this.applyMenuItem, "applyMenuItem");
            this.applyMenuItem.Click += new System.EventHandler(this.applyMenuItem_Click);
            //
            // breakPrefabMenuItem
            //
            this.breakPrefabMenuItem.BackColor = System.Drawing.Color.DarkGray;
            this.breakPrefabMenuItem.ForeColor = System.Drawing.SystemColors.WindowText;
            this.breakPrefabMenuItem.Name = "breakPrefabMenuItem";
            resources.ApplyResources(this.breakPrefabMenuItem, "breakPrefabMenuItem");
            this.breakPrefabMenuItem.Click += new System.EventHandler(this.breakPrefabMenuItem_Click);
            //
            // emptyButton
            //
            this.emptyButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.emptyButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.emptyButton.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.emptyButton, "emptyButton");
            this.emptyButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.emptyButton.Name = "emptyButton";
            this.emptyButton.UseVisualStyleBackColor = false;
            //
            // saveImageDialog
            //
            resources.ApplyResources(this.saveImageDialog, "saveImageDialog");
            this.saveImageDialog.FilterIndex = 0;
            //
            // BehaviorTreeView
            //
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.zoomOutButton);
            this.Controls.Add(this.zoomInButton);
            this.Controls.Add(this.emptyButton);
            this.Controls.Add(this.parameterSettingButton);
            this.Controls.Add(this.fitToViewButton);
            this.Controls.Add(this.propertiesButton);
            this.Controls.Add(this.imageButton);
            this.Controls.Add(this.saveAsButton);
            this.Controls.Add(this.checkButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.saveButton);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Name = "BehaviorTreeView";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.BehaviorTreeView_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.BehaviorTreeView_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.BehaviorTreeView_DragOver);
            this.MouseLeave += new System.EventHandler(this.BehaviorTreeView_MouseLeave);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button saveAsButton;
        private System.Windows.Forms.Button imageButton;
        private System.Windows.Forms.SaveFileDialog saveImageDialog;
        private System.Windows.Forms.Button propertiesButton;
        private System.Windows.Forms.Button parameterSettingButton;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTreeMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem fitToViewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem referenceMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savePrefabMenuItem;
        private System.Windows.Forms.Button fitToViewButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutTreeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem applyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem breakPrefabMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem breakpointMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enterBreakpointMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitBreakpointMenuItem;
        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.Button emptyButton;
        private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beakpointPlanning;
        private System.Windows.Forms.ToolStripMenuItem copySubtreeMenuItem;
        private System.Windows.Forms.Button zoomInButton;
        private System.Windows.Forms.Button zoomOutButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem docMenuItem;
    }
}
