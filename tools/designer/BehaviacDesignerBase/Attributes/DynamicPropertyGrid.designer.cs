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

namespace CustomPropertyGridTest
{
    partial class DynamicPropertyGrid
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
            this.propertyPanel = new System.Windows.Forms.Panel();
            this.propertiesSplitContainer = new System.Windows.Forms.SplitContainer();
            this.propertyDescriptionLabel = new System.Windows.Forms.Label();
            this.propertyNameLabel = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.descriptionPanel = new System.Windows.Forms.Panel();
            this.propertyPanel.SuspendLayout();
            this.propertiesSplitContainer.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.descriptionPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // propertyPanel
            //
            this.propertyPanel.AutoScroll = true;
            this.propertyPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.propertyPanel.Controls.Add(this.propertiesSplitContainer);
            this.propertyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyPanel.ForeColor = System.Drawing.Color.LightGray;
            this.propertyPanel.Location = new System.Drawing.Point(0, 0);
            this.propertyPanel.Name = "propertyPanel";
            this.propertyPanel.Size = new System.Drawing.Size(555, 357);
            this.propertyPanel.TabIndex = 2;
            //
            // propertiesSplitContainer
            //
            this.propertiesSplitContainer.BackColor = System.Drawing.Color.Gray;
            this.propertiesSplitContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertiesSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.propertiesSplitContainer.Name = "propertiesSplitContainer";
            //
            // propertiesSplitContainer.Panel1
            //
            this.propertiesSplitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.propertiesSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(3);
            //
            // propertiesSplitContainer.Panel2
            //
            this.propertiesSplitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.propertiesSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3);
            this.propertiesSplitContainer.Size = new System.Drawing.Size(555, 62);
            this.propertiesSplitContainer.SplitterDistance = 168;
            this.propertiesSplitContainer.SplitterWidth = 2;
            this.propertiesSplitContainer.TabIndex = 1;
            this.propertiesSplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.propertiesSplitContainer_SplitterMoved);
            this.propertiesSplitContainer.Resize += new System.EventHandler(this.propertiesSplitContainer_Resize);
            //
            // propertyDescriptionLabel
            //
            this.propertyDescriptionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.propertyDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyDescriptionLabel.ForeColor = System.Drawing.Color.LightGray;
            this.propertyDescriptionLabel.Location = new System.Drawing.Point(0, 20);
            this.propertyDescriptionLabel.Name = "propertyDescriptionLabel";
            this.propertyDescriptionLabel.Size = new System.Drawing.Size(555, 51);
            this.propertyDescriptionLabel.TabIndex = 1;
            this.propertyDescriptionLabel.Text = "Property Description";
            //
            // propertyNameLabel
            //
            this.propertyNameLabel.BackColor = System.Drawing.Color.Gray;
            this.propertyNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertyNameLabel.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyNameLabel.ForeColor = System.Drawing.Color.LightGray;
            this.propertyNameLabel.Location = new System.Drawing.Point(0, 0);
            this.propertyNameLabel.Name = "propertyNameLabel";
            this.propertyNameLabel.Size = new System.Drawing.Size(555, 20);
            this.propertyNameLabel.TabIndex = 0;
            this.propertyNameLabel.Text = "Property Name";
            this.propertyNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // splitContainer
            //
            this.splitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            //
            // splitContainer.Panel1
            //
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.splitContainer.Panel1.Controls.Add(this.propertyPanel);
            //
            // splitContainer.Panel2
            //
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.splitContainer.Panel2.Controls.Add(this.descriptionPanel);
            this.splitContainer.Panel2MinSize = 35;
            this.splitContainer.Size = new System.Drawing.Size(555, 432);
            this.splitContainer.SplitterDistance = 357;
            this.splitContainer.TabIndex = 3;
            //
            // descriptionPanel
            //
            this.descriptionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.descriptionPanel.Controls.Add(this.propertyDescriptionLabel);
            this.descriptionPanel.Controls.Add(this.propertyNameLabel);
            this.descriptionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionPanel.Location = new System.Drawing.Point(0, 0);
            this.descriptionPanel.Name = "descriptionPanel";
            this.descriptionPanel.Size = new System.Drawing.Size(555, 71);
            this.descriptionPanel.TabIndex = 1;
            //
            // DynamicPropertyGrid
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.splitContainer);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "DynamicPropertyGrid";
            this.Size = new System.Drawing.Size(555, 432);
            this.propertyPanel.ResumeLayout(false);
            this.propertiesSplitContainer.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.descriptionPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel propertyPanel;
        private System.Windows.Forms.SplitContainer propertiesSplitContainer;
        private System.Windows.Forms.Label propertyNameLabel;
        private System.Windows.Forms.Label propertyDescriptionLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel descriptionPanel;


    }
}
