using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;

namespace DockSample.Customization
{
	internal class VS2003DockPaneStrip : DockPaneStripBase
	{
        private class TabVS2003 : Tab
        {
            internal TabVS2003(IDockContent content)
                : base(content)
            {
            }

            private int m_tabX;
            protected internal int TabX
            {
                get { return m_tabX; }
                set { m_tabX = value; }
            }

            private int m_tabWidth;
            protected internal int TabWidth
            {
                get { return m_tabWidth; }
                set { m_tabWidth = value; }
            }

            private int m_maxWidth;
            protected internal int MaxWidth
            {
                get { return m_maxWidth; }
                set { m_maxWidth = value; }
            }

            private bool m_flag;
            protected internal bool Flag
            {
                get { return m_flag; }
                set { m_flag = value; }
            }
        }

        protected override DockPaneStripBase.Tab CreateTab(IDockContent content)
        {
            return new TabVS2003(content);
        }

        private class DocumentButton : Label
        {
            public DocumentButton(Image image)
            {
                Image = image;
            }


        }

		#region consts
		private const int _ToolWindowStripGapLeft = 4;
		private const int _ToolWindowStripGapRight = 3;
		private const int _ToolWindowImageHeight = 16;
		private const int _ToolWindowImageWidth = 16;
		private const int _ToolWindowImageGapTop = 3;
		private const int _ToolWindowImageGapBottom = 1;
		private const int _ToolWindowImageGapLeft = 3;
		private const int _ToolWindowImageGapRight = 2;
		private const int _ToolWindowTextGapRight = 1;
		private const int _ToolWindowTabSeperatorGapTop = 3;
		private const int _ToolWindowTabSeperatorGapBottom = 3;

		private const int _DocumentTabMaxWidth = 200;
		private const int _DocumentButtonGapTop = 5;
		private const int _DocumentButtonGapBottom = 5;
		private const int _DocumentButtonGapBetween = 0;
		private const int _DocumentButtonGapRight = 3;
		private const int _DocumentTabGapTop = 3;
		private const int _DocumentTabGapLeft = 3;
		private const int _DocumentTabGapRight = 3;
		private const int _DocumentIconGapLeft = 6;
		private const int _DocumentIconHeight = 16;
		private const int _DocumentIconWidth = 16;
		#endregion

		private InertButton m_buttonClose, m_buttonScrollLeft, m_buttonScrollRight;
		private IContainer m_components;
		private ToolTip m_toolTip;

		/// <exclude/>
		protected IContainer Components
		{
			get	{	return m_components;	}
		}

		private int m_offsetX = 0;
		private int OffsetX
		{
			get	{	return m_offsetX;	}
			set
			{	
				m_offsetX = value;
				#if DEBUG
				if (m_offsetX > 0)
					throw new InvalidOperationException();
				#endif
			}
		}

		#region Customizable Properties
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowStripGapLeft"]/*'/>
		protected virtual int ToolWindowStripGapLeft
		{
			get	{	return _ToolWindowStripGapLeft;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowStripGapRight"]/*'/>
		protected virtual int ToolWindowStripGapRight
		{
			get	{	return _ToolWindowStripGapRight;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageHeight"]/*'/>
		protected virtual int ToolWindowImageHeight
		{
			get	{	return _ToolWindowImageHeight;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageWidth"]/*'/>
		protected virtual int ToolWindowImageWidth
		{
			get	{	return _ToolWindowImageWidth;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapTop"]/*'/>
		protected virtual int ToolWindowImageGapTop
		{
			get	{	return _ToolWindowImageGapTop;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapBottom"]/*'/>
		protected virtual int ToolWindowImageGapBottom
		{
			get	{	return _ToolWindowImageGapBottom;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapLeft"]/*'/>
		protected virtual int ToolWindowImageGapLeft
		{
			get	{	return _ToolWindowImageGapLeft;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapRight"]/*'/>
		protected virtual int ToolWindowImageGapRight
		{
			get	{	return _ToolWindowImageGapRight;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowTextGapRight"]/*'/>
		protected virtual int ToolWindowTextGapRight
		{
			get	{	return _ToolWindowTextGapRight;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowSeperatorGaptop"]/*'/>
		protected virtual int ToolWindowTabSeperatorGapTop
		{
			get	{	return _ToolWindowTabSeperatorGapTop;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowSeperatorGapBottom"]/*'/>
		protected virtual int ToolWindowTabSeperatorGapBottom
		{
			get	{	return _ToolWindowTabSeperatorGapBottom;	}
		}

		private static Image _imageCloseEnabled = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageCloseEnabled"]/*'/>
		protected virtual Image ImageCloseEnabled
		{
			get
			{	
				if (_imageCloseEnabled == null)
					_imageCloseEnabled = Resources.DockPaneStrip_CloseEnabled;
				return _imageCloseEnabled;
			}
		}

		private static Image _imageCloseDisabled = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageCloseDisabled"]/*'/>
		protected virtual Image ImageCloseDisabled
		{
			get
			{	
				if (_imageCloseDisabled == null)
					_imageCloseDisabled = Resources.DockPaneStrip_CloseDisabled;
				return _imageCloseDisabled;
			}
		}

		private static Image _imageScrollLeftEnabled = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollLeftEnabled"]/*'/>
		protected virtual Image ImageScrollLeftEnabled
		{
			get
			{	
				if (_imageScrollLeftEnabled == null)
					_imageScrollLeftEnabled = Resources.DockPaneStrip_ScrollLeftEnabled;
				return _imageScrollLeftEnabled;
			}
		}

		private static Image _imageScrollLeftDisabled = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollLeftDisabled"]/*'/>
		protected virtual Image ImageScrollLeftDisabled
		{
			get
			{	
				if (_imageScrollLeftDisabled == null)
					_imageScrollLeftDisabled = Resources.DockPaneStrip_ScrollLeftDisabled;
				return _imageScrollLeftDisabled;
			}
		}

		private static Image _imageScrollRightEnabled = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollRightEnabled"]/*'/>
		protected virtual Image ImageScrollRightEnabled
		{
			get
			{	
				if (_imageScrollRightEnabled == null)
					_imageScrollRightEnabled = Resources.DockPaneStrip_ScrollRightEnabled;
				return _imageScrollRightEnabled;
			}
		}

		private static Image _imageScrollRightDisabled = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollRightDisabled"]/*'/>
		protected virtual Image ImageScrollRightDisabled
		{
			get
			{	
				if (_imageScrollRightDisabled == null)
					_imageScrollRightDisabled = Resources.DockPaneStrip_ScrollRightDisabled;
				return _imageScrollRightDisabled;
			}
		}

		private static string _toolTipClose = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolTipClose"]/*'/>
		protected virtual string ToolTipClose
		{
			get
			{	
				if (_toolTipClose == null)
					_toolTipClose = Strings.DockPaneStrip_ToolTipClose;
				return _toolTipClose;
			}
		}

		private static string _toolTipScrollLeft = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolTipScrollLeft"]/*'/>
		protected virtual string ToolTipScrollLeft
		{
			get
			{	
				if (_toolTipScrollLeft == null)
					_toolTipScrollLeft = Strings.DockPaneStrip_ToolTipScrollLeft;
				return _toolTipScrollLeft;
			}
		}

		private static string _toolTipScrollRight = null;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolTipScrollRight"]/*'/>
		protected virtual string ToolTipScrollRight
		{
			get
			{	
				if (_toolTipScrollRight == null)
					_toolTipScrollRight = Strings.DockPaneStrip_ToolTipScrollRight;
				return _toolTipScrollRight;
			}
		}

        private static TextFormatFlags _toolWindowTextFormat =
            TextFormatFlags.EndEllipsis |
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.SingleLine |
            TextFormatFlags.VerticalCenter;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowTextStringFormat"]/*'/>
		protected virtual TextFormatFlags ToolWindowTextFormat
		{
			get	{	return _toolWindowTextFormat;	}
		}

        private static TextFormatFlags _documentTextFormat =
            TextFormatFlags.PathEllipsis |
            TextFormatFlags.SingleLine |
            TextFormatFlags.VerticalCenter;
		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTextStringFormat"]/*'/>
		public static TextFormatFlags DocumentTextFormat
		{
        	get	{	return _documentTextFormat;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabMaxWidth"]/*'/>
		protected virtual int DocumentTabMaxWidth
		{
			get	{	return _DocumentTabMaxWidth;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapTop"]/*'/>
		protected virtual int DocumentButtonGapTop
		{
			get	{	return _DocumentButtonGapTop;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapBottom"]/*'/>
		protected virtual int DocumentButtonGapBottom
		{
			get	{	return _DocumentButtonGapBottom;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapBetween"]/*'/>
		protected virtual int DocumentButtonGapBetween
		{
			get	{	return _DocumentButtonGapBetween;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapRight"]/*'/>
		protected virtual int DocumentButtonGapRight
		{
			get	{	return _DocumentButtonGapRight;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabGapTop"]/*'/>
		protected virtual int DocumentTabGapTop
		{
			get	{	return _DocumentTabGapTop;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabGapLeft"]/*'/>
		protected virtual int DocumentTabGapLeft
		{
			get	{	return _DocumentTabGapLeft;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabGapRight"]/*'/>
		protected virtual int DocumentTabGapRight
		{
			get	{	return _DocumentTabGapRight;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentIconGapLeft"]/*'/>
		protected virtual int DocumentIconGapLeft
		{
			get	{	return _DocumentIconGapLeft;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentIconWidth"]/*'/>
		protected virtual int DocumentIconWidth
		{
			get	{	return _DocumentIconWidth;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentIconHeight"]/*'/>
		protected virtual int DocumentIconHeight
		{
			get	{	return _DocumentIconHeight;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="OutlineInnerPen"]/*'/>
		protected virtual Pen OutlineInnerPen
		{
			get	{	return SystemPens.ControlText;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="OutlineOuterPen"]/*'/>
		protected virtual Pen OutlineOuterPen
		{
			get	{	return SystemPens.ActiveCaptionText;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ActiveBackBrush"]/*'/>
		protected virtual Brush ActiveBackBrush
		{
			get	{	return SystemBrushes.Control;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ActiveTextBrush"]/*'/>
		protected virtual Color ActiveTextColor
		{
			get	{	return SystemColors.ControlText;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="TabSeperatorPen"]/*'/>
		protected virtual Pen TabSeperatorPen
		{
			get	{	return SystemPens.GrayText;	}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="InactiveTextBrush"]/*'/>
		protected virtual Color InactiveTextColor
		{
			get	{	return SystemColors.ControlDarkDark;	}
		}
		#endregion

		public VS2003DockPaneStrip(DockPane pane) : base(pane)
		{
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			SuspendLayout();

			Font = SystemInformation.MenuFont;
			BackColor = Color.WhiteSmoke;
			
			m_components = new Container();
			m_toolTip = new ToolTip(Components);

			m_buttonClose = new InertButton(ImageCloseEnabled, ImageCloseDisabled);
			m_buttonScrollLeft = new InertButton(ImageScrollLeftEnabled, ImageScrollLeftDisabled);
			m_buttonScrollRight = new InertButton(ImageScrollRightEnabled, ImageScrollRightDisabled);

			m_buttonClose.ToolTipText = ToolTipClose;
			m_buttonClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonClose.Click += new EventHandler(Close_Click);

			m_buttonScrollLeft.Enabled = false;
			m_buttonScrollLeft.RepeatClick = true;
			m_buttonScrollLeft.ToolTipText = ToolTipScrollLeft;
			m_buttonScrollLeft.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonScrollLeft.Click += new EventHandler(ScrollLeft_Click);

			m_buttonScrollRight.Enabled = false;
			m_buttonScrollRight.RepeatClick = true;
			m_buttonScrollRight.ToolTipText = ToolTipScrollRight;
			m_buttonScrollRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			m_buttonScrollRight.Click += new EventHandler(ScrollRight_Click);

			Controls.AddRange(new Control[] {	m_buttonClose,
												m_buttonScrollLeft,
												m_buttonScrollRight	});

			ResumeLayout();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Components.Dispose();
			}
			base.Dispose (disposing);
		}

		protected override int MeasureHeight()
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				return MeasureHeight_ToolWindow();
			else
				return MeasureHeight_Document();
		}

		private int MeasureHeight_ToolWindow()
		{
			if (DockPane.IsAutoHide || Tabs.Count <= 1)
				return 0;

			int height = Math.Max(Font.Height, ToolWindowImageHeight)
				+ ToolWindowImageGapTop + ToolWindowImageGapBottom;

			return height;
		}

		private int MeasureHeight_Document()
		{
			int height = Math.Max(Font.Height + DocumentTabGapTop,
				ImageCloseEnabled.Height + DocumentButtonGapTop + DocumentButtonGapBottom);

			return height;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);
			CalculateTabs();
			DrawTabStrip(e.Graphics);
		}

		protected override void OnRefreshChanges()
		{
			CalculateTabs();
			SetInertButtons();
			Invalidate();
		}

		protected override GraphicsPath GetOutline(int index)
		{
			Point[] pts = new Point[8];

			if (Appearance == DockPane.AppearanceStyle.Document)
			{
				Rectangle rectTab = GetTabRectangle(index);
				rectTab.Intersect(TabsRectangle);
				int y = DockPane.PointToClient(PointToScreen(new Point(0, rectTab.Bottom))).Y;
				Rectangle rectPaneClient = DockPane.ClientRectangle;
				pts[0] = DockPane.PointToScreen(new Point(rectPaneClient.Left, y));
				pts[1] = PointToScreen(new Point(rectTab.Left, rectTab.Bottom));
				pts[2] = PointToScreen(new Point(rectTab.Left, rectTab.Top));
				pts[3] = PointToScreen(new Point(rectTab.Right, rectTab.Top));
				pts[4] = PointToScreen(new Point(rectTab.Right, rectTab.Bottom));
				pts[5] = DockPane.PointToScreen(new Point(rectPaneClient.Right, y));
				pts[6] = DockPane.PointToScreen(new Point(rectPaneClient.Right, rectPaneClient.Bottom));
				pts[7] = DockPane.PointToScreen(new Point(rectPaneClient.Left, rectPaneClient.Bottom));
			}
			else
			{
				Rectangle rectTab = GetTabRectangle(index);
				rectTab.Intersect(TabsRectangle);
				int y = DockPane.PointToClient(PointToScreen(new Point(0, rectTab.Top))).Y;
				Rectangle rectPaneClient = DockPane.ClientRectangle;
				pts[0] = DockPane.PointToScreen(new Point(rectPaneClient.Left, rectPaneClient.Top));
				pts[1] = DockPane.PointToScreen(new Point(rectPaneClient.Right, rectPaneClient.Top));
				pts[2] = DockPane.PointToScreen(new Point(rectPaneClient.Right, y));
				pts[3] = PointToScreen(new Point(rectTab.Right, rectTab.Top));
				pts[4] = PointToScreen(new Point(rectTab.Right, rectTab.Bottom));
				pts[5] = PointToScreen(new Point(rectTab.Left, rectTab.Bottom));
				pts[6] = PointToScreen(new Point(rectTab.Left, rectTab.Top));
				pts[7] = DockPane.PointToScreen(new Point(rectPaneClient.Left, y));
			}

			GraphicsPath path = new GraphicsPath();
			path.AddLines(pts);
			return path;
		}

		private void CalculateTabs()
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				CalculateTabs_ToolWindow();
			else
				CalculateTabs_Document();
		}

		private void CalculateTabs_ToolWindow()
		{
			if (Tabs.Count <= 1 || DockPane.IsAutoHide)
				return;

			Rectangle rectTabStrip = ClientRectangle;

			// Calculate tab widths
			int countTabs = Tabs.Count;
			foreach (TabVS2003 tab in Tabs)
			{
				tab.MaxWidth = GetTabOriginalWidth(Tabs.IndexOf(tab));
				tab.Flag = false;
			}

			// Set tab whose max width less than average width
			bool anyWidthWithinAverage = true;
			int totalWidth = rectTabStrip.Width - ToolWindowStripGapLeft - ToolWindowStripGapRight;
			int totalAllocatedWidth = 0;
			int averageWidth = totalWidth / countTabs;
			int remainedTabs = countTabs;
			for (anyWidthWithinAverage=true; anyWidthWithinAverage && remainedTabs>0;)
			{
				anyWidthWithinAverage = false;
				foreach (TabVS2003 tab in Tabs)
				{
					if (tab.Flag)
						continue;

					if (tab.MaxWidth <= averageWidth)
					{
						tab.Flag = true;
						tab.TabWidth = tab.MaxWidth;
						totalAllocatedWidth += tab.TabWidth;
						anyWidthWithinAverage = true;
						remainedTabs--;
					}
				}
				if (remainedTabs != 0)
					averageWidth = (totalWidth - totalAllocatedWidth) / remainedTabs;
			}

			// If any tab width not set yet, set it to the average width
			if (remainedTabs > 0)
			{
				int roundUpWidth = (totalWidth - totalAllocatedWidth) - (averageWidth * remainedTabs);
				foreach (TabVS2003 tab in Tabs)
				{
					if (tab.Flag)
						continue;

					tab.Flag = true;
					if (roundUpWidth > 0)
					{
						tab.TabWidth = averageWidth + 1;
						roundUpWidth --;
					}
					else
						tab.TabWidth = averageWidth;
				}
			}

			// Set the X position of the tabs
			int x = rectTabStrip.X + ToolWindowStripGapLeft;
			foreach (TabVS2003 tab in Tabs)
			{
				tab.TabX = x;
				x += tab.TabWidth;
			}
		}

		private void CalculateTabs_Document()
		{
			Rectangle rectTabStrip = TabsRectangle;

			int totalWidth = 0;
			foreach (TabVS2003 tab in Tabs)
			{
				tab.TabWidth = Math.Min(GetTabOriginalWidth(Tabs.IndexOf(tab)), DocumentTabMaxWidth);
				totalWidth += tab.TabWidth;
			}

			if (totalWidth + OffsetX < rectTabStrip.Width && OffsetX < 0)
				OffsetX = Math.Min(0, rectTabStrip.Width - totalWidth);

			int x = rectTabStrip.X + OffsetX;
			foreach (TabVS2003 tab in Tabs)
			{
				tab.TabX = x;
				x += tab.TabWidth;
			}
		}

		protected override void EnsureTabVisible(IDockContent content)
		{
			if (Appearance != DockPane.AppearanceStyle.Document || !Tabs.Contains(content))
				return;

			Rectangle rectTabStrip = TabsRectangle;
			Rectangle rectTab = GetTabRectangle(Tabs.IndexOf(content));

			if (rectTab.Right > rectTabStrip.Right)
			{
				OffsetX -= rectTab.Right - rectTabStrip.Right;
				rectTab.X -= rectTab.Right - rectTabStrip.Right;
			}

			if (rectTab.Left < rectTabStrip.Left)
				OffsetX += rectTabStrip.Left - rectTab.Left;

			OnRefreshChanges();
		}

		private int GetTabOriginalWidth(int index)
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				return GetTabOriginalWidth_ToolWindow(index);
			else
				return GetTabOriginalWidth_Document(index);
		}

		private int GetTabOriginalWidth_ToolWindow(int index)
		{
			IDockContent content = Tabs[index].Content;
			using (Graphics g = CreateGraphics())
			{
				Size sizeString = TextRenderer.MeasureText(g, content.DockHandler.TabText, Font);
				return ToolWindowImageWidth + sizeString.Width + ToolWindowImageGapLeft
					+ ToolWindowImageGapRight + ToolWindowTextGapRight;
			}
		}

		private int GetTabOriginalWidth_Document(int index)
		{
			IDockContent content = Tabs[index].Content;

            int height = GetTabRectangle_Document(index).Height;

			using (Graphics g = CreateGraphics())
			{
				Size sizeText;
				if (content == DockPane.ActiveContent && DockPane.IsActiveDocumentPane)
				{
					using (Font boldFont = new Font(this.Font, FontStyle.Bold))
					{
						sizeText = TextRenderer.MeasureText(g, content.DockHandler.TabText, boldFont, new Size(DocumentTabMaxWidth, height), DocumentTextFormat);
					}
				}
				else
					sizeText = TextRenderer.MeasureText(content.DockHandler.TabText, Font, new Size(DocumentTabMaxWidth, height), DocumentTextFormat);

				if (DockPane.DockPanel.ShowDocumentIcon)
					return sizeText.Width + DocumentIconWidth + DocumentIconGapLeft;
				else
					return sizeText.Width;
			}
		}

		private void DrawTabStrip(Graphics g)
		{
			OnBeginDrawTabStrip();

			if (Appearance == DockPane.AppearanceStyle.Document)
				DrawTabStrip_Document(g);
			else
				DrawTabStrip_ToolWindow(g);

			OnEndDrawTabStrip();
		}

		private void DrawTabStrip_Document(Graphics g)
		{
			int count = Tabs.Count;
			if (count == 0)
				return;

			Rectangle rectTabStrip = ClientRectangle;
			g.DrawLine(OutlineOuterPen, rectTabStrip.Left, rectTabStrip.Bottom - 1,
				rectTabStrip.Right, rectTabStrip.Bottom - 1);

			// Draw the tabs
			Rectangle rectTabOnly = TabsRectangle;
			Rectangle rectTab = Rectangle.Empty;
			g.SetClip(rectTabOnly);
			for (int i=0; i<count; i++)
			{
				rectTab = GetTabRectangle(i);
				if (rectTab.IntersectsWith(rectTabOnly))
					DrawTab(g, Tabs[i] as TabVS2003, rectTab);
			}
		}

		private void DrawTabStrip_ToolWindow(Graphics g)
		{
			Rectangle rectTabStrip = ClientRectangle;

			g.DrawLine(OutlineInnerPen, rectTabStrip.Left, rectTabStrip.Top,
				rectTabStrip.Right, rectTabStrip.Top);

			for (int i=0; i<Tabs.Count; i++)
				DrawTab(g, Tabs[i] as TabVS2003, GetTabRectangle(i));
		}

		private Rectangle GetTabRectangle(int index)
		{
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				return GetTabRectangle_ToolWindow(index);
			else
				return GetTabRectangle_Document(index);
		}

		private Rectangle GetTabRectangle_ToolWindow(int index)
		{
			Rectangle rectTabStrip = ClientRectangle;

			TabVS2003 tab = (TabVS2003)(Tabs[index]);
			return new Rectangle(tab.TabX, rectTabStrip.Y, tab.TabWidth, rectTabStrip.Height);
		}

		private Rectangle GetTabRectangle_Document(int index)
		{
			Rectangle rectTabStrip = ClientRectangle;
			TabVS2003 tab = (TabVS2003)Tabs[index];

			return new Rectangle(tab.TabX, rectTabStrip.Y + DocumentTabGapTop, tab.TabWidth, rectTabStrip.Height - DocumentTabGapTop);
		}

        private void DrawTab(Graphics g, TabVS2003 tab, Rectangle rect)
		{
			OnBeginDrawTab(tab);

			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				DrawTab_ToolWindow(g, tab, rect);
			else
				DrawTab_Document(g, tab, rect);

			OnEndDrawTab(tab);
		}

		private void DrawTab_ToolWindow(Graphics g, TabVS2003 tab, Rectangle rect)
		{
			Rectangle rectIcon = new Rectangle(
				rect.X + ToolWindowImageGapLeft,
				rect.Y + rect.Height - 1 - ToolWindowImageGapBottom - ToolWindowImageHeight,
				ToolWindowImageWidth, ToolWindowImageHeight);
			Rectangle rectText = rectIcon;
			rectText.X += rectIcon.Width + ToolWindowImageGapRight;
			rectText.Width = rect.Width - rectIcon.Width - ToolWindowImageGapLeft - 
				ToolWindowImageGapRight - ToolWindowTextGapRight;

			if (DockPane.ActiveContent == tab.Content)
			{
				g.FillRectangle(ActiveBackBrush, rect);
				g.DrawLine(OutlineOuterPen,
					rect.X, rect.Y, rect.X, rect.Y + rect.Height - 1);
				g.DrawLine(OutlineInnerPen,
					rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
				g.DrawLine(OutlineInnerPen,
					rect.X + rect.Width - 1, rect.Y, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
                TextRenderer.DrawText(g, tab.Content.DockHandler.TabText, Font, rectText, ActiveTextColor, ToolWindowTextFormat);
			}
			else
			{
				if (Tabs.IndexOf(DockPane.ActiveContent) != Tabs.IndexOf(tab) + 1)
					g.DrawLine(TabSeperatorPen,
						rect.X + rect.Width - 1,
						rect.Y + ToolWindowTabSeperatorGapTop,
						rect.X + rect.Width - 1,
						rect.Y + rect.Height - 1 - ToolWindowTabSeperatorGapBottom);
				TextRenderer.DrawText(g, tab.Content.DockHandler.TabText, Font, rectText, InactiveTextColor, ToolWindowTextFormat);
			}

			if (rect.Contains(rectIcon))
				g.DrawIcon(tab.Content.DockHandler.Icon, rectIcon);
		}

		private void DrawTab_Document(Graphics g, TabVS2003 tab, Rectangle rect)
		{
			Rectangle rectText = rect;
			if (DockPane.DockPanel.ShowDocumentIcon)
			{
				rectText.X += DocumentIconWidth + DocumentIconGapLeft;
				rectText.Width -= DocumentIconWidth + DocumentIconGapLeft;
			}

			if (DockPane.ActiveContent == tab.Content)
			{
				g.FillRectangle(ActiveBackBrush, rect);
				g.DrawLine(OutlineOuterPen, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
				g.DrawLine(OutlineOuterPen, rect.X, rect.Y, rect.X + rect.Width - 1, rect.Y);
				g.DrawLine(OutlineInnerPen,
					rect.X + rect.Width - 1, rect.Y,
					rect.X + rect.Width - 1, rect.Y + rect.Height - 1);

				if (DockPane.DockPanel.ShowDocumentIcon)
				{
					Icon icon = (tab.Content as Form).Icon;
					Rectangle rectIcon = new Rectangle(
						rect.X + DocumentIconGapLeft,
						rect.Y + (rect.Height - DocumentIconHeight) / 2,
						DocumentIconWidth, DocumentIconHeight);

					g.DrawIcon(tab.ContentForm.Icon, rectIcon);
				}

				if (DockPane.IsActiveDocumentPane)
				{
					using (Font boldFont = new Font(this.Font, FontStyle.Bold))
					{
						TextRenderer.DrawText(g, tab.Content.DockHandler.TabText, boldFont, rectText, ActiveTextColor, DocumentTextFormat);
					}
				}
				else
					TextRenderer.DrawText(g, tab.Content.DockHandler.TabText, Font, rectText, InactiveTextColor, DocumentTextFormat);
			}
			else
			{
				if (Tabs.IndexOf(DockPane.ActiveContent) != Tabs.IndexOf(tab) + 1)
					g.DrawLine(TabSeperatorPen,
						rect.X + rect.Width - 1, rect.Y,
						rect.X + rect.Width - 1, rect.Y + rect.Height - 1 - DocumentTabGapTop);

				if (DockPane.DockPanel.ShowDocumentIcon)
				{
					Icon icon = tab.ContentForm.Icon;
					Rectangle rectIcon = new Rectangle(
						rect.X + DocumentIconGapLeft,
						rect.Y + (rect.Height - DocumentIconHeight) / 2,
						DocumentIconWidth, DocumentIconHeight);

					g.DrawIcon(tab.ContentForm.Icon, rectIcon);
				}

				TextRenderer.DrawText(g, tab.Content.DockHandler.TabText, Font, rectText, InactiveTextColor, DocumentTextFormat);
			}
		}

		private Rectangle TabsRectangle
		{
			get	
			{
				if (Appearance == DockPane.AppearanceStyle.ToolWindow)
					return ClientRectangle;

				Rectangle rectWindow = ClientRectangle;
				int x = rectWindow.X;
				int y = rectWindow.Y;
				int width = rectWindow.Width;
				int height = rectWindow.Height;

				x += DocumentTabGapLeft;
				width -= DocumentTabGapLeft + 
						DocumentTabGapRight +
						DocumentButtonGapRight +
						m_buttonClose.Width +
						m_buttonScrollRight.Width +
						m_buttonScrollLeft.Width +
						2 * DocumentButtonGapBetween;

				return new Rectangle(x, y, width, height);
			}
		}

		private void ScrollLeft_Click(object sender, EventArgs e)
		{
			Rectangle rectTabStrip = TabsRectangle;

			int index;
			for (index=0; index<Tabs.Count; index++)
				if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
					break;

			Rectangle rectTab = GetTabRectangle(index);
			if (rectTab.Left < rectTabStrip.Left)
				OffsetX += rectTabStrip.Left - rectTab.Left;
			else if (index == 0)
				OffsetX = 0;
			else
				OffsetX += rectTabStrip.Left - GetTabRectangle(index - 1).Left;

			OnRefreshChanges();
		}
	
		private void ScrollRight_Click(object sender, EventArgs e)
		{
			Rectangle rectTabStrip = TabsRectangle;

			int index;
			int count = Tabs.Count;
			for (index=0; index<count; index++)
				if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
					break;

			if (index + 1 < count)
			{
				OffsetX -= GetTabRectangle(index + 1).Left - rectTabStrip.Left;
				CalculateTabs();
			}

			Rectangle rectLastTab = GetTabRectangle(count - 1);
			if (rectLastTab.Right < rectTabStrip.Right)
				OffsetX += rectTabStrip.Right - rectLastTab.Right;

			OnRefreshChanges();
		}

		private void SetInertButtons()
		{
			// Set the visibility of the inert buttons
			m_buttonScrollLeft.Visible = m_buttonScrollRight.Visible = m_buttonClose.Visible = (DockPane.DockState == DockState.Document);

			m_buttonClose.ForeColor = m_buttonScrollRight.ForeColor	= m_buttonScrollLeft.ForeColor = SystemColors.ControlDarkDark;
			m_buttonClose.BorderColor = m_buttonScrollRight.BorderColor	= m_buttonScrollLeft.BorderColor = SystemColors.ControlDarkDark;
		
			// Enable/disable scroll buttons
			int count = Tabs.Count;

			Rectangle rectTabOnly = TabsRectangle;
			Rectangle rectTab = (count == 0) ? Rectangle.Empty : GetTabRectangle(count - 1);
			m_buttonScrollLeft.Enabled = (OffsetX < 0);
			m_buttonScrollRight.Enabled = rectTab.Right > rectTabOnly.Right;

			// show/hide close button
			if (Appearance == DockPane.AppearanceStyle.ToolWindow)
				m_buttonClose.Visible = false;
			else
			{
				bool showCloseButton = DockPane.ActiveContent == null ? true : DockPane.ActiveContent.DockHandler.CloseButton;
				if (m_buttonClose.Visible != showCloseButton)
				{
					m_buttonClose.Visible = showCloseButton;
					PerformLayout();
				}
			}
		}

		/// <exclude/>
		protected override void OnLayout(LayoutEventArgs levent)
		{
			Rectangle rectTabStrip = ClientRectangle;

			// Set position and size of the buttons
			int buttonWidth = ImageCloseEnabled.Width;
			int buttonHeight = ImageCloseEnabled.Height;
			int height = rectTabStrip.Height - DocumentButtonGapTop - DocumentButtonGapBottom;
			if (buttonHeight < height)
			{
				buttonWidth = buttonWidth * (height / buttonHeight);
				buttonHeight = height;
			}
			Size buttonSize = new Size(buttonWidth, buttonHeight);
			m_buttonClose.Size = m_buttonScrollLeft.Size = m_buttonScrollRight.Size = buttonSize;
			int x = rectTabStrip.X + rectTabStrip.Width - DocumentTabGapLeft
				- DocumentButtonGapRight - buttonWidth;
			int y = rectTabStrip.Y + DocumentButtonGapTop;
			m_buttonClose.Location = new Point(x, y);
			Point point = m_buttonClose.Location;
			bool showCloseButton = DockPane.ActiveContent == null ? true : DockPane.ActiveContent.DockHandler.CloseButton;
			if (showCloseButton)
				point.Offset(-(DocumentButtonGapBetween + buttonWidth), 0);
			m_buttonScrollRight.Location = point;
			point.Offset(-(DocumentButtonGapBetween + buttonWidth), 0);
			m_buttonScrollLeft.Location = point;

			OnRefreshChanges();

			base.OnLayout (levent);
		}

		private void Close_Click(object sender, EventArgs e)
		{
			DockPane.CloseActiveContent();
		}

		/// <exclude/>
		protected override int HitTest(Point ptMouse)
		{
			Rectangle rectTabStrip = TabsRectangle;

			for (int i=0; i<Tabs.Count; i++)
			{
				Rectangle rectTab = GetTabRectangle(i);
				rectTab.Intersect(rectTabStrip);
				if (rectTab.Contains(ptMouse))
					return i;
			}
			return -1;
		}

		/// <exclude/>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			int index = HitTest(PointToClient(Control.MousePosition));
			string toolTip = string.Empty;

			base.OnMouseMove(e);

			if (index != -1)
			{
				Rectangle rectTab = GetTabRectangle(index);
				if (Tabs[index].Content.DockHandler.ToolTipText != null)
					toolTip = Tabs[index].Content.DockHandler.ToolTipText;
				else if (rectTab.Width < GetTabOriginalWidth(index))
					toolTip = Tabs[index].Content.DockHandler.TabText;
			}

			if (m_toolTip.GetToolTip(this) != toolTip)
			{
				m_toolTip.Active = false;
				m_toolTip.SetToolTip(this, toolTip);
				m_toolTip.Active = true;
			}
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnBeginDrawTabStrip()"]/*'/>
		protected virtual void OnBeginDrawTabStrip()
		{
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnEndDrawTabStrip()"]/*'/>
		protected virtual void OnEndDrawTabStrip()
		{
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnBeginDrawTab(DockPaneTab)"]/*'/>
		protected virtual void OnBeginDrawTab(Tab tab)
		{
		}

		/// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnEndDrawTab(DockPaneTab)"]/*'/>
		protected virtual void OnEndDrawTab(Tab tab)
		{
		}
	}
}
