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
    partial class TimelineDock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimelineDock));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.startButton = new System.Windows.Forms.Button();
            this.backwardButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.forwardButton = new System.Windows.Forms.Button();
            this.endButton = new System.Windows.Forms.Button();
            this.zoomOutButton = new System.Windows.Forms.Button();
            this.zoomInButton = new System.Windows.Forms.Button();
            this.comboBoxLogFilter = new System.Windows.Forms.ComboBox();
            this.numericUpDownFPS = new System.Windows.Forms.NumericUpDown();
            this.gotoLabel = new System.Windows.Forms.Label();
            this.gotoNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.promptLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.startLabel = new System.Windows.Forms.Label();
            this.endLabel = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.effectTimer = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gotoNumericUpDown)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            //
            // flowLayoutPanel1
            //
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.startButton);
            this.flowLayoutPanel1.Controls.Add(this.backwardButton);
            this.flowLayoutPanel1.Controls.Add(this.playButton);
            this.flowLayoutPanel1.Controls.Add(this.forwardButton);
            this.flowLayoutPanel1.Controls.Add(this.endButton);
            this.flowLayoutPanel1.Controls.Add(this.zoomOutButton);
            this.flowLayoutPanel1.Controls.Add(this.zoomInButton);
            this.flowLayoutPanel1.Controls.Add(this.comboBoxLogFilter);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownFPS);
            this.flowLayoutPanel1.Controls.Add(this.gotoLabel);
            this.flowLayoutPanel1.Controls.Add(this.gotoNumericUpDown);
            this.flowLayoutPanel1.Controls.Add(this.promptLabel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1114, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            //
            // startButton
            //
            this.startButton.AutoSize = true;
            this.startButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this.startButton.Location = new System.Drawing.Point(20, 3);
            this.startButton.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(23, 23);
            this.startButton.TabIndex = 1;
            this.toolTip.SetToolTip(this.startButton, "Return to the first frame.");
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            //
            // backwardButton
            //
            this.backwardButton.AutoSize = true;
            this.backwardButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.backwardButton.Image = ((System.Drawing.Image)(resources.GetObject("backwardButton.Image")));
            this.backwardButton.Location = new System.Drawing.Point(49, 3);
            this.backwardButton.Name = "backwardButton";
            this.backwardButton.Size = new System.Drawing.Size(22, 23);
            this.backwardButton.TabIndex = 2;
            this.toolTip.SetToolTip(this.backwardButton, "Return to the previous frame.");
            this.backwardButton.UseVisualStyleBackColor = true;
            this.backwardButton.Click += new System.EventHandler(this.backwardButton_Click);
            //
            // playButton
            //
            this.playButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.playButton.Image = global::Behaviac.Design.Properties.Resources.Play;
            this.playButton.Location = new System.Drawing.Point(77, 3);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(22, 23);
            this.playButton.TabIndex = 3;
            this.toolTip.SetToolTip(this.playButton, "Continue/Break");
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            //
            // forwardButton
            //
            this.forwardButton.AutoSize = true;
            this.forwardButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.forwardButton.Image = ((System.Drawing.Image)(resources.GetObject("forwardButton.Image")));
            this.forwardButton.Location = new System.Drawing.Point(105, 3);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(22, 23);
            this.forwardButton.TabIndex = 4;
            this.toolTip.SetToolTip(this.forwardButton, "Go to the next frame.");
            this.forwardButton.UseVisualStyleBackColor = true;
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            //
            // endButton
            //
            this.endButton.AutoSize = true;
            this.endButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.endButton.Image = ((System.Drawing.Image)(resources.GetObject("endButton.Image")));
            this.endButton.Location = new System.Drawing.Point(133, 3);
            this.endButton.Name = "endButton";
            this.endButton.Size = new System.Drawing.Size(22, 23);
            this.endButton.TabIndex = 5;
            this.toolTip.SetToolTip(this.endButton, "Go to the last frame.");
            this.endButton.UseVisualStyleBackColor = true;
            this.endButton.Click += new System.EventHandler(this.endButton_Click);
            //
            // zoomOutButton
            //
            this.zoomOutButton.AutoSize = true;
            this.zoomOutButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.zoomOutButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomOutButton.Image")));
            this.zoomOutButton.Location = new System.Drawing.Point(161, 3);
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Size = new System.Drawing.Size(22, 23);
            this.zoomOutButton.TabIndex = 7;
            this.toolTip.SetToolTip(this.zoomOutButton, "Zoom out");
            this.zoomOutButton.UseVisualStyleBackColor = true;
            this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
            //
            // zoomInButton
            //
            this.zoomInButton.AutoSize = true;
            this.zoomInButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.zoomInButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomInButton.Image")));
            this.zoomInButton.Location = new System.Drawing.Point(189, 3);
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Size = new System.Drawing.Size(22, 23);
            this.zoomInButton.TabIndex = 8;
            this.toolTip.SetToolTip(this.zoomInButton, "Zoon in");
            this.zoomInButton.UseVisualStyleBackColor = true;
            this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
            //
            // comboBoxLogFilter
            //
            this.comboBoxLogFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.comboBoxLogFilter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxLogFilter.ForeColor = System.Drawing.Color.LightGray;
            this.comboBoxLogFilter.FormattingEnabled = true;
            this.comboBoxLogFilter.Items.AddRange(new object[]
            {
                "ALL",
                "RED",
                "ORANGE",
                "YELLOW",
                "GREEN",
                "BLUE",
                "INDIGO",
                "PURPLE"
            });
            this.comboBoxLogFilter.Location = new System.Drawing.Point(217, 3);
            this.comboBoxLogFilter.Name = "comboBoxLogFilter";
            this.comboBoxLogFilter.Size = new System.Drawing.Size(94, 21);
            this.comboBoxLogFilter.TabIndex = 19;
            this.toolTip.SetToolTip(this.comboBoxLogFilter, "Log filter");
            this.comboBoxLogFilter.Visible = false;
            this.comboBoxLogFilter.SelectedIndexChanged += new System.EventHandler(this.comboBoxLogFilter_SelectedIndexChanged);
            this.comboBoxLogFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxLogFilter_KeyDown);
            //
            // numericUpDownFPS
            //
            this.numericUpDownFPS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.numericUpDownFPS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDownFPS.Dock = System.Windows.Forms.DockStyle.Left;
            this.numericUpDownFPS.ForeColor = System.Drawing.Color.LightGray;
            this.numericUpDownFPS.Location = new System.Drawing.Point(317, 5);
            this.numericUpDownFPS.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.numericUpDownFPS.Maximum = new decimal(new int[]
            {
                1000,
                0,
                0,
                0
            });
            this.numericUpDownFPS.Minimum = new decimal(new int[]
            {
                1,
                0,
                0,
                0
            });
            this.numericUpDownFPS.Name = "numericUpDownFPS";
            this.numericUpDownFPS.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.numericUpDownFPS.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownFPS.TabIndex = 17;
            this.toolTip.SetToolTip(this.numericUpDownFPS, "Simulating FPS");
            this.numericUpDownFPS.Value = new decimal(new int[]
            {
                60,
                0,
                0,
                0
            });
            //
            // gotoLabel
            //
            this.gotoLabel.AutoSize = true;
            this.gotoLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.gotoLabel.Location = new System.Drawing.Point(405, 0);
            this.gotoLabel.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.gotoLabel.Name = "gotoLabel";
            this.gotoLabel.Size = new System.Drawing.Size(36, 29);
            this.gotoLabel.TabIndex = 13;
            this.gotoLabel.Text = "Frame";
            this.gotoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gotoNumericUpDown
            //
            this.gotoNumericUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.gotoNumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gotoNumericUpDown.Dock = System.Windows.Forms.DockStyle.Left;
            this.gotoNumericUpDown.ForeColor = System.Drawing.Color.LightGray;
            this.gotoNumericUpDown.Location = new System.Drawing.Point(447, 5);
            this.gotoNumericUpDown.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.gotoNumericUpDown.Name = "gotoNumericUpDown";
            this.gotoNumericUpDown.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.gotoNumericUpDown.Size = new System.Drawing.Size(79, 20);
            this.gotoNumericUpDown.TabIndex = 14;
            this.toolTip.SetToolTip(this.gotoNumericUpDown, "Go to which frame?");
            this.gotoNumericUpDown.ValueChanged += new System.EventHandler(this.gotoNumericUpDown_ValueChanged);
            //
            // promptLabel
            //
            this.promptLabel.AutoSize = true;
            this.promptLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.promptLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.promptLabel.ForeColor = System.Drawing.Color.Gold;
            this.promptLabel.Location = new System.Drawing.Point(539, 0);
            this.promptLabel.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Size = new System.Drawing.Size(104, 29);
            this.promptLabel.TabIndex = 15;
            this.promptLabel.Text = "break prompt";
            this.promptLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panel1
            //
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.trackBar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1114, 45);
            this.panel1.TabIndex = 0;
            //
            // trackBar
            //
            this.trackBar.BackColor = System.Drawing.SystemColors.Control;
            this.trackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackBar.LargeChange = 10;
            this.trackBar.Location = new System.Drawing.Point(0, 0);
            this.trackBar.Maximum = 100;
            this.trackBar.Name = "trackBar";
            this.trackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.trackBar.Size = new System.Drawing.Size(1114, 45);
            this.trackBar.SmallChange = 5;
            this.trackBar.TabIndex = 15;
            this.trackBar.TickFrequency = 10;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            //
            // startLabel
            //
            this.startLabel.AutoSize = true;
            this.startLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.startLabel.Location = new System.Drawing.Point(0, 74);
            this.startLabel.Name = "startLabel";
            this.startLabel.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.startLabel.Size = new System.Drawing.Size(21, 13);
            this.startLabel.TabIndex = 16;
            this.startLabel.Text = "0";
            //
            // endLabel
            //
            this.endLabel.AutoSize = true;
            this.endLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.endLabel.Location = new System.Drawing.Point(1089, 74);
            this.endLabel.Name = "endLabel";
            this.endLabel.Size = new System.Drawing.Size(25, 13);
            this.endLabel.TabIndex = 17;
            this.endLabel.Text = "100";
            //
            // effectTimer
            //
            this.effectTimer.Interval = 250;
            this.effectTimer.Tick += new System.EventHandler(this.effectTimer_Tick);
            //
            // TimelineDock
            //
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(1114, 117);
            this.Controls.Add(this.endLabel);
            this.Controls.Add(this.startLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.LightGray;
            this.Name = "TimelineDock";
            this.ShowIcon = false;
            this.TabText = "Timeline";
            this.Text = "Timeline";
            this.Load += new System.EventHandler(this.TimelineDock_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFPS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gotoNumericUpDown)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label gotoLabel;
        private System.Windows.Forms.NumericUpDown gotoNumericUpDown;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.Label startLabel;
        private System.Windows.Forms.Label endLabel;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button backwardButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button forwardButton;
        private System.Windows.Forms.Button endButton;
        private System.Windows.Forms.Button zoomOutButton;
        private System.Windows.Forms.Button zoomInButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label promptLabel;
        private System.Windows.Forms.NumericUpDown numericUpDownFPS;
        private System.Windows.Forms.Timer effectTimer;
        private System.Windows.Forms.ComboBox comboBoxLogFilter;
    }
}
