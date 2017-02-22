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

namespace Behaviac.Design.Attributes
{
    partial class DesignerPropertyEnumEditor
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
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            //
            // comboBox
            //
            this.comboBox.AllowDrop = true;
            this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                          | System.Windows.Forms.AnchorStyles.Left)
                                                                         | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.comboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox.ForeColor = System.Drawing.Color.LightGray;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(2, 2);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(200, 22);
            this.comboBox.TabIndex = 0;
            this.comboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox_DrawItem);
            this.comboBox.DropDown += new System.EventHandler(this.comboBox_DropDown);
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            this.comboBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.comboBox_DragDrop);
            this.comboBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.comboBox_DragEnter);
            this.comboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox_KeyPress);
            this.comboBox.MouseEnter += new System.EventHandler(this.comboBox_MouseEnter);
            this.comboBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.comboBox_PreviewKeyDown);
            //
            // DesignerPropertyEnumEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.comboBox);
            this.Name = "DesignerPropertyEnumEditor";
            this.Size = new System.Drawing.Size(204, 26);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox;
    }
}
