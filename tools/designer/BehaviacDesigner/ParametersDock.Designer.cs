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
    partial class ParametersDock
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
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.parametersPanel = new Behaviac.Design.ParametersPanel();
            this.SuspendLayout();
            //
            // flowLayoutPanel
            //
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(526, 0);
            this.flowLayoutPanel.TabIndex = 0;
            //
            // parametersPanel
            //
            this.parametersPanel.AutoScroll = true;
            this.parametersPanel.AutoSize = true;
            this.parametersPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.parametersPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.parametersPanel.ForeColor = System.Drawing.Color.LightGray;
            this.parametersPanel.Location = new System.Drawing.Point(0, 0);
            this.parametersPanel.Margin = new System.Windows.Forms.Padding(0);
            this.parametersPanel.Name = "parametersPanel";
            this.parametersPanel.Size = new System.Drawing.Size(526, 50);
            this.parametersPanel.TabIndex = 5;
            //
            // ParametersDock
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(526, 262);
            this.Controls.Add(this.parametersPanel);
            this.Controls.Add(this.flowLayoutPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "ParametersDock";
            this.ShowIcon = false;
            this.TabText = "Properties";
            this.Text = "Properties";
            this.Click += new System.EventHandler(this.ParametersDock_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private ParametersPanel parametersPanel;
    }
}
