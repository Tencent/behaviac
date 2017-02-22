using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace DockSample.Customization
{
	internal class DockHelper
	{
		public static bool IsDockStateAutoHide(DockState dockState)
		{
			if (dockState == DockState.DockLeftAutoHide ||
				dockState == DockState.DockRightAutoHide ||
				dockState == DockState.DockTopAutoHide ||
				dockState == DockState.DockBottomAutoHide)
				return true;
			else
				return false;
		}

		public static bool IsDockStateDocked(DockState dockState)
		{
			return (dockState == DockState.DockLeft ||
				dockState == DockState.DockRight ||
				dockState == DockState.DockTop ||
				dockState == DockState.DockBottom);
		}

		public static bool IsDockBottom(DockState dockState)
		{
			return (dockState == DockState.DockBottom || dockState == DockState.DockBottomAutoHide) ? true : false;
		}

		public static bool IsDockLeft(DockState dockState)
		{
			return (dockState == DockState.DockLeft || dockState == DockState.DockLeftAutoHide) ? true : false;
		}

		public static bool IsDockRight(DockState dockState)
		{
			return (dockState == DockState.DockRight || dockState == DockState.DockRightAutoHide) ? true : false;
		}

		public static bool IsDockTop(DockState dockState)
		{
			return (dockState == DockState.DockTop || dockState == DockState.DockTopAutoHide ) ? true : false;
		}

		public static bool IsDockStateValid(DockState dockState, DockAreas dockableAreas)
		{
			if (((dockableAreas & DockAreas.Float) == 0) &&
				(dockState == DockState.Float))
				return false;
			else if (((dockableAreas & DockAreas.Document) == 0) &&
				(dockState == DockState.Document))
				return false;
			else if (((dockableAreas & DockAreas.DockLeft) == 0) &&
				(dockState == DockState.DockLeft || dockState == DockState.DockLeftAutoHide))
				return false;
			else if (((dockableAreas & DockAreas.DockRight) == 0) &&
				(dockState == DockState.DockRight || dockState == DockState.DockRightAutoHide))
				return false;
			else if (((dockableAreas & DockAreas.DockTop) == 0) &&
				(dockState == DockState.DockTop || dockState == DockState.DockTopAutoHide))
				return false;
			else if (((dockableAreas & DockAreas.DockBottom) == 0) &&
				(dockState == DockState.DockBottom || dockState == DockState.DockBottomAutoHide))
				return false;
			else
				return true;
		}

		public static bool IsDockWindowState(DockState state)
		{
			if (state == DockState.DockTop || state == DockState.DockBottom || state == DockState.DockLeft ||
				state == DockState.DockRight || state == DockState.Document)
				return true;
			else
				return false;
		}

		public static bool IsValidRestoreState(DockState state)
		{
			if (state == DockState.DockLeft || state == DockState.DockRight || state == DockState.DockTop ||
				state == DockState.DockBottom || state == DockState.Document)
				return true;
			else
				return false;
		}

		public static DockState ToggleAutoHideState(DockState state)
		{
			if (state == DockState.DockLeft)
				return DockState.DockLeftAutoHide;
			else if (state == DockState.DockRight)
				return DockState.DockRightAutoHide;
			else if (state == DockState.DockTop)
				return DockState.DockTopAutoHide;
			else if (state == DockState.DockBottom)
				return DockState.DockBottomAutoHide;
			else if (state == DockState.DockLeftAutoHide)
				return DockState.DockLeft;
			else if (state == DockState.DockRightAutoHide)
				return DockState.DockRight;
			else if (state == DockState.DockTopAutoHide)
				return DockState.DockTop;
			else if (state == DockState.DockBottomAutoHide)
				return DockState.DockBottom;
			else
				return state;
		}
	}
}
