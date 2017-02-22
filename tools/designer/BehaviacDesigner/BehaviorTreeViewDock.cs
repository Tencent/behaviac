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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Behaviac.Design
{
    internal partial class BehaviorTreeViewDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static List<string> __saved_bt_paths = new List<string>();
        public static List<string> LastOpenedBehaviors
        {
            get
            {
                return __saved_bt_paths;
            }
        }

        private static List<BehaviorTreeViewDock> __instances = new List<BehaviorTreeViewDock>();
        internal static IList<BehaviorTreeViewDock> Instances
        {
            get
            {
                return __instances.AsReadOnly();
            }
        }

        private static BehaviorTreeViewDock __lastFocusedInstance = null;
        internal static BehaviorTreeViewDock LastFocused
        {
            get
            {
                return __lastFocusedInstance;
            }
        }

        internal static bool ReadOnly
        {
            set
            {
                foreach (BehaviorTreeViewDock dock in __instances)
                {
                    dock.BehaviorTreeView.ReadOnly = value;
                }
            }
        }

        internal static void ClearHighlights()
        {
            foreach (BehaviorTreeViewDock dock in __instances)
            {
                dock.BehaviorTreeView.ClearHighlights();
            }
        }

        internal static void ClearHighlightBreakPoint()
        {
            foreach (BehaviorTreeViewDock dock in __instances)
            {
                dock.BehaviorTreeView.ClearHighlightBreakPoint();
            }
        }

        internal static void RefreshAll()
        {
            foreach (BehaviorTreeViewDock dock in __instances)
            {
                dock.BehaviorTreeView.Redraw();
            }
        }

        internal static void CloseAll()
        {
            try
            {
                BehaviorTreeViewDock[] behaviorTreeViewDocks = __instances.ToArray();

                foreach (BehaviorTreeViewDock dock in behaviorTreeViewDocks)
                {
                    __saved_bt_paths.Add(dock._behaviorTreeView.RootNode.RelativePath);
                    dock.Hide();
                    dock.Close();
                }
            }
            catch
            {
            }

            __instances.Clear();
            __lastFocusedInstance = null;
        }

        internal static void CloseBehaviorTreeViewDock(Nodes.BehaviorNode node)
        {
            try
            {
                foreach (BehaviorTreeViewDock dock in __instances)
                {
                    if (dock.BehaviorTreeView.RootNode == node)
                    {
                        dock.Hide();
                        dock.Close();

                        __instances.Remove(dock);

                        break;
                    }
                }
            }
            catch
            {
            }
        }

        internal static BehaviorTreeViewDock GetBehaviorTreeViewDock(Nodes.BehaviorNode node)
        {
            foreach (BehaviorTreeViewDock dock in __instances)
            {
                if (dock.BehaviorTreeView.RootNode == node)
                {
                    return dock;
                }
            }

            return null;
        }

        internal static BehaviorTreeView GetBehaviorTreeView(Nodes.BehaviorNode node)
        {
            foreach (BehaviorTreeViewDock dock in __instances)
            {
                if (dock.BehaviorTreeView.RootNode == node)
                {
                    return dock.BehaviorTreeView;
                }
            }

            return null;
        }

        internal void MakeFocused()
        {
            if (__lastFocusedInstance != this)
            {
                __lastFocusedInstance = this;

                _behaviorTreeView.Redraw();
            }
        }

        internal bool SaveBehaviorWhenClosing
        {
            set
            {
                if (_behaviorTreeView != null)
                {
                    _behaviorTreeView.SaveBehaviorWhenClosing = value;
                }
            }
        }

        private BehaviorTreeView _behaviorTreeView;
        internal BehaviorTreeView BehaviorTreeView
        {
            get
            {
                return _behaviorTreeView;
            }

            set
            {
                if (_behaviorTreeView != null)
                {
                    throw new Exception("BehaviorTreeView already assigned");
                }

                _behaviorTreeView = value;
                Controls.Add(_behaviorTreeView);

                if (_behaviorTreeView != null && _behaviorTreeView.RootNode != null)
                {
                    _behaviorTreeView.MouseDown += BehaviorTreeView_MouseDown;
                    _behaviorTreeView.RootNode.WasModified += RootNode_WasModified;
                    _behaviorTreeView.RootNode.WasSaved += RootNode_WasSaved;
                }
            }
        }

        void BehaviorTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            MakeFocused();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            MakeFocused();

            base.OnGotFocus(e);
        }

        private void RootNode_WasModified(Nodes.BehaviorNode root, Nodes.Node node)
        {
            if (root == _behaviorTreeView.RootNode)
            {
                Text = TabText = "*" + ((Nodes.Node)root).Label;
            }
        }

        private void RootNode_WasSaved(Nodes.BehaviorNode root)
        {
            if (root == _behaviorTreeView.RootNode)
            {
                Text = TabText = ((Nodes.Node)root).Label;
            }
        }

        public BehaviorTreeViewDock()
        {
            InitializeComponent();

            this.TabPageContextMenuStrip = this.contextMenu;

            __instances.Add(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (__lastFocusedInstance == this)
            {
                __lastFocusedInstance = null;
            }

            __instances.Remove(this);

            if (_behaviorTreeView != null && _behaviorTreeView.RootNode != null)
            {
                _behaviorTreeView.RootNode.WasModified -= RootNode_WasModified;
                _behaviorTreeView.RootNode.WasSaved -= RootNode_WasSaved;
            }

            base.OnClosed(e);
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.SaveBehavior(false);
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closeOthersMenuItem_Click(object sender, EventArgs e)
        {
            BehaviorTreeViewDock[] behaviorTreeViewDocks = __instances.ToArray();

            foreach (BehaviorTreeViewDock dock in behaviorTreeViewDocks)
            {
                if (dock != this)
                {
                    dock.Hide();
                    dock.Close();

                    __instances.Remove(dock);
                }
            }
        }

        private void copyNameMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = FileManagers.FileManager.GetRelativePath(this.BehaviorTreeView.RootNode.Filename);
                Clipboard.SetText(filename);

            }
            catch
            {
            }
        }

        private void showMenuItem_Click(object sender, EventArgs e)
        {
            string filename = this.BehaviorTreeView.RootNode.Filename;
            MainWindow.Instance.BehaviorTreeList.ShowBehaviorTreeNode(filename);
        }

        private void openFolderMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string folder = System.IO.Path.GetDirectoryName(this.BehaviorTreeView.RootNode.Filename);
                System.Diagnostics.Process.Start(folder);

            }
            catch
            {
            }
        }

        private void floatMenuItem_Click(object sender, EventArgs e)
        {
            this.IsFloat = true;
        }

        private void dockMenuItem_Click(object sender, EventArgs e)
        {
            this.IsFloat = false;
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            this.floatMenuItem.Enabled = !this.IsFloat;
            this.dockMenuItem.Enabled = this.IsFloat;
        }
    }
}
