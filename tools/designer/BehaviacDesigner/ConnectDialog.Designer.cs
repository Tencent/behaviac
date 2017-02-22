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
    partial class ConnectDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectDialog));
            this.tbServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.localIPCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // tbServer
            //
            resources.ApplyResources(this.tbServer, "tbServer");
            this.tbServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.tbServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbServer.ForeColor = System.Drawing.Color.LightGray;
            this.tbServer.Name = "tbServer";
            //
            // label1
            //
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            //
            // btnOk
            //
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = false;
            //
            // tbPort
            //
            resources.ApplyResources(this.tbPort, "tbPort");
            this.tbPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.tbPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPort.ForeColor = System.Drawing.Color.LightGray;
            this.tbPort.Name = "tbPort";
            this.tbPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPort_KeyPress);
            //
            // label2
            //
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            //
            // btnCancel
            //
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DarkGray;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkGray;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            //
            // localIPCheckBox
            //
            resources.ApplyResources(this.localIPCheckBox, "localIPCheckBox");
            this.localIPCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.localIPCheckBox.Checked = true;
            this.localIPCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.localIPCheckBox.ForeColor = System.Drawing.Color.LightGray;
            this.localIPCheckBox.Name = "localIPCheckBox";
            this.localIPCheckBox.UseVisualStyleBackColor = false;
            this.localIPCheckBox.CheckedChanged += new System.EventHandler(this.localIPCheckBox_CheckedChanged);
            //
            // ConnectDialog
            //
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.localIPCheckBox);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbServer);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectDialog";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox localIPCheckBox;
    }
}