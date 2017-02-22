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
    partial class CompositeEditorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompositeEditorDialog));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.propertyGrid = new CustomPropertyGridTest.DynamicPropertyGrid();
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.insertButton = new System.Windows.Forms.Button();
            this.appendButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanel
            //
            this.tableLayoutPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.propertyGrid, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.upButton, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.downButton, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.closeButton, 1, 7);
            this.tableLayoutPanel.Controls.Add(this.removeButton, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.insertButton, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.appendButton, 1, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            //
            // propertyGrid
            //
            resources.ApplyResources(this.propertyGrid, "propertyGrid");
            this.propertyGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.propertyGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.propertyGrid.ForeColor = System.Drawing.Color.LightGray;
            this.propertyGrid.Name = "propertyGrid";
            this.tableLayoutPanel.SetRowSpan(this.propertyGrid, 8);
            //
            // upButton
            //
            resources.ApplyResources(this.upButton, "upButton");
            this.upButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.upButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.upButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.upButton.Name = "upButton";
            this.upButton.UseVisualStyleBackColor = false;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            //
            // downButton
            //
            resources.ApplyResources(this.downButton, "downButton");
            this.downButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.downButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.downButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.downButton.Name = "downButton";
            this.downButton.UseVisualStyleBackColor = false;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            //
            // closeButton
            //
            resources.ApplyResources(this.closeButton, "closeButton");
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            //
            // removeButton
            //
            resources.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.removeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.removeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.removeButton.Name = "removeButton";
            this.removeButton.UseVisualStyleBackColor = false;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            //
            // insertButton
            //
            resources.ApplyResources(this.insertButton, "insertButton");
            this.insertButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.insertButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.insertButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.insertButton.Name = "insertButton";
            this.insertButton.UseVisualStyleBackColor = false;
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            //
            // appendButton
            //
            resources.ApplyResources(this.appendButton, "appendButton");
            this.appendButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.appendButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.appendButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.appendButton.Name = "appendButton";
            this.appendButton.UseVisualStyleBackColor = false;
            this.appendButton.Click += new System.EventHandler(this.addButton_Click);
            //
            // CompositeEditorDialog
            //
            this.AcceptButton = this.closeButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.closeButton;
            this.Controls.Add(this.tableLayoutPanel);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompositeEditorDialog";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CompositeEditorDialog_FormClosing);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button appendButton;
        private CustomPropertyGridTest.DynamicPropertyGrid propertyGrid;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button insertButton;

    }
}