namespace DockSample
{
    partial class DummyDoc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DummyDoc));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCheckTest = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuTabPage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.mainMenu.SuspendLayout();
            this.contextMenuTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem1});
            this.mainMenu.Location = new System.Drawing.Point(0, 4);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(448, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Visible = false;
            // 
            // menuItem1
            // 
            this.menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem2,
            this.menuItemCheckTest});
            this.menuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItem1.MergeIndex = 1;
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(89, 20);
            this.menuItem1.Text = "&MDI Document";
            // 
            // menuItem2
            // 
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Size = new System.Drawing.Size(127, 22);
            this.menuItem2.Text = "Test";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItemCheckTest
            // 
            this.menuItemCheckTest.Name = "menuItemCheckTest";
            this.menuItemCheckTest.Size = new System.Drawing.Size(127, 22);
            this.menuItemCheckTest.Text = "Check Test";
            this.menuItemCheckTest.Click += new System.EventHandler(this.menuItemCheckTest_Click);
            // 
            // contextMenuTabPage
            // 
            this.contextMenuTabPage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem3,
            this.menuItem4,
            this.menuItem5});
            this.contextMenuTabPage.Name = "contextMenuTabPage";
            this.contextMenuTabPage.Size = new System.Drawing.Size(116, 70);
            // 
            // menuItem3
            // 
            this.menuItem3.Name = "menuItem3";
            this.menuItem3.Size = new System.Drawing.Size(115, 22);
            this.menuItem3.Text = "Option &1";
            // 
            // menuItem4
            // 
            this.menuItem4.Name = "menuItem4";
            this.menuItem4.Size = new System.Drawing.Size(115, 22);
            this.menuItem4.Text = "Option &2";
            // 
            // menuItem5
            // 
            this.menuItem5.Name = "menuItem5";
            this.menuItem5.Size = new System.Drawing.Size(115, 22);
            this.menuItem5.Text = "Option &3";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(448, 389);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // DummyDoc
            // 
            this.ClientSize = new System.Drawing.Size(448, 393);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "DummyDoc";
            this.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.TabPageContextMenuStrip = this.contextMenuTabPage;
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.contextMenuTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem menuItem1;
		private System.Windows.Forms.ToolStripMenuItem menuItem2;
		private System.Windows.Forms.ContextMenuStrip contextMenuTabPage;
		private System.Windows.Forms.ToolStripMenuItem menuItem3;
		private System.Windows.Forms.ToolStripMenuItem menuItem4;
		private System.Windows.Forms.ToolStripMenuItem menuItem5;
		private System.Windows.Forms.ToolStripMenuItem menuItemCheckTest;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}