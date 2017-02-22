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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Behaviac.Design.Data;
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;

namespace Behaviac.Design
{
    internal partial class FindDock : Form
    {
        private enum FindRange
        {
            CurrentFile,
            AllOpenFiles,
            EntireWorkspace
        }

        private enum FindType
        {
            Previous,
            Next,
            All
        }

        private static FindDock _findDock = null;
        private bool _findAll = false;

        internal static void Inspect(bool findAll)
        {
            if (_findDock == null)
            {
                _findDock = new FindDock();
                _findDock.Owner = MainWindow.Instance;
            }

            _findDock._findAll = findAll;

            if (findAll)
            {
                _findDock.findPreviousButton.Visible = false;
                _findDock.findNextButton.Text = Resources.FindAll;

            }
            else
            {
                _findDock.findPreviousButton.Visible = true;
                _findDock.findNextButton.Text = Resources.FindNext;
            }

            _findDock.Show();

            if (Settings.Default.FindDockLocation.X > 0 && Settings.Default.FindDockLocation.Y > 0)
            {
                _findDock.Location = Settings.Default.FindDockLocation;
            }
        }

        public FindDock()
        {
            InitializeComponent();

            _findDock = this;

            loadFindSettings();
        }

        private void FindDock_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveFindSettings();

            //this.Hide();
            //e.Cancel = true;

            _findDock = null;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void findPreviousButton_Click(object sender, EventArgs e)
        {
            FindObject(FindType.Previous);
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            findNext();
        }

        protected override bool ProcessDialogKey(Keys key)
        {
            if (key == Keys.Enter && this.whatComboBox.Focused)
            {
                findNext();

                return true;
            }

            return base.ProcessDialogKey(key);
        }

        private void findNext()
        {
            if (this._findAll)
            {
                FindObject(FindType.All);
                //this.Close();

            }
            else
            {
                FindObject(FindType.Next);
            }
        }

        private void loadFindSettings()
        {
            this.rangeComboBox.SelectedIndex = Settings.Default.FindRange;
            this.matchCaseCheckBox.Checked = Settings.Default.FindOptionMatchCase;
            this.matchWholeWordCheckBox.Checked = Settings.Default.FindOptionMatchWholeWord;
            this.nodeIdCheckBox.Checked = Settings.Default.FindOptionNodeId;
            this.nodeTypeCheckBox.Checked = Settings.Default.FindOptionNodeType;

            if (Settings.Default.FindWhats != null)
            {
                foreach (string what in Settings.Default.FindWhats)
                {
                    this.whatComboBox.Items.Add(what);
                }

                if (this.whatComboBox.Items.Count > 0)
                {
                    this.whatComboBox.SelectedIndex = 0;
                }
            }
        }

        private void saveFindSettings()
        {
            Settings.Default.FindDockLocation = this.Location;

            Settings.Default.FindRange = this.rangeComboBox.SelectedIndex;
            Settings.Default.FindOptionMatchCase = this.matchCaseCheckBox.Checked;
            Settings.Default.FindOptionMatchWholeWord = this.matchWholeWordCheckBox.Checked;
            Settings.Default.FindOptionNodeId = this.nodeIdCheckBox.Checked;
            Settings.Default.FindOptionNodeType = this.nodeTypeCheckBox.Checked;

            if (Settings.Default.FindWhats == null)
            {
                Settings.Default.FindWhats = new System.Collections.Specialized.StringCollection();
            }

            else
            {
                Settings.Default.FindWhats.Clear();
            }

            foreach (string what in this.whatComboBox.Items)
            {
                Settings.Default.FindWhats.Add(what);
            }
        }

        private List<ObjectPair> _findObjects = new List<ObjectPair>();
        private int _objectIndex = -1;

        private void FindObject(string findWhat, FindRange findRange, bool matchCase,
                                bool matchWholeWord, bool onlyByNodeType, bool onlyByNodeId, FindType findType)
        {
            if (string.IsNullOrEmpty(findWhat))
            {
                return;
            }

            saveFindSettings();

            try
            {
                List<Nodes.Node> rootNodes = GetRootNodes(findRange);
                List<ObjectPair> findObjects = new List<ObjectPair>();

                if (!onlyByNodeType)
                {
                    // by Id
                    int id = int.MinValue;

                    if (int.TryParse(findWhat, out id))
                    {
                        foreach (Nodes.Node root in rootNodes)
                        {
                            DefaultObject obj = Plugin.GetObjectById(root, id);

                            if (obj != null)
                            {
                                findObjects.Add(new ObjectPair(root, obj));
                            }
                        }
                    }
                }

                if (!onlyByNodeId)
                {
                    // by Type
                    foreach (Nodes.Node root in rootNodes)
                    {
                        root.GetObjectsByType(root, findWhat, matchCase, matchWholeWord, ref findObjects);
                    }
                }

                if (!onlyByNodeId && !onlyByNodeType)
                {
                    foreach (Nodes.Node root in rootNodes)
                    {
                        root.GetObjectsByPropertyMethod(root, findWhat, matchCase, matchWholeWord, ref findObjects);
                    }
                }

                if (findObjects.Count > 0)
                {
                    if (Plugin.CompareTwoObjectLists(_findObjects, findObjects))
                    {
                        if (findType == FindType.Next)
                        {
                            _objectIndex++;

                            if (_objectIndex >= findObjects.Count)
                            {
                                _objectIndex = 0;
                            }

                        }
                        else if (findType == FindType.Previous)
                        {
                            _objectIndex--;

                            if (_objectIndex < 0)
                            {
                                _objectIndex = findObjects.Count - 1;
                            }
                        }

                    }
                    else
                    {
                        _objectIndex = 0;
                        _findObjects = findObjects;
                    }

                    if (findType == FindType.All)
                    {
                        FindFileDock.Inspect(findWhat, rootNodes.Count, findObjects);

                    }
                    else
                    {
                        ShowObject(findObjects[_objectIndex]);
                    }

                    return;
                }

            }
            catch (Exception)
            {
            }

            MessageBox.Show(Resources.FindWarningInfo, Resources.FindWarning, MessageBoxButtons.OK);
        }

        private List<Nodes.Node> GetRootNodes(FindRange findRange)
        {
            List<Nodes.Node> rootNodes = new List<Nodes.Node>();

            switch (findRange)
            {
                case FindRange.CurrentFile:
                    if (BehaviorTreeViewDock.LastFocused != null)
                    {
                        BehaviorTreeView behaviorTreeView = BehaviorTreeViewDock.LastFocused.BehaviorTreeView;

                        if (behaviorTreeView != null && behaviorTreeView.RootNodeView != null)
                        {
                            rootNodes.Add(behaviorTreeView.RootNodeView.Node);
                        }
                    }

                    break;

                case FindRange.AllOpenFiles:
                    foreach (BehaviorTreeViewDock dock in BehaviorTreeViewDock.Instances)
                    {
                        BehaviorTreeView behaviorTreeView = dock.BehaviorTreeView;

                        if (behaviorTreeView != null && behaviorTreeView.RootNodeView != null)
                        {
                            rootNodes.Add(behaviorTreeView.RootNodeView.Node);
                        }
                    }

                    break;

                case FindRange.EntireWorkspace:
                    BehaviorTreeList behaviorTreeList = BehaviorManager.Instance as BehaviorTreeList;

                    if (behaviorTreeList != null)
                    {
                        foreach (Nodes.BehaviorNode behavior in behaviorTreeList.GetAllBehaviors())
                        {
                            if (behavior != null && behavior is Nodes.Node)
                            {
                                rootNodes.Add((Nodes.Node)behavior);
                            }
                        }
                    }

                    break;
            }

            return rootNodes;
        }

        public static void ShowObject(ObjectPair obj)
        {
            BehaviorTreeList behaviorTreeList = BehaviorManager.Instance as BehaviorTreeList;

            if (behaviorTreeList == null)
            {
                return;
            }

            Nodes.Node node = null;
            Attachments.Attachment attach = null;

            if (obj.Obj is Nodes.Node)
            {
                node = (Nodes.Node)obj.Obj;

            }
            else if (obj.Obj is Attachments.Attachment)
            {
                attach = (Attachments.Attachment)obj.Obj;
                node = attach.Node;
            }

            if (node != null)
            {
                behaviorTreeList.ShowNode(node, obj.Root);

                if (BehaviorTreeViewDock.LastFocused != null)
                {
                    BehaviorTreeView behaviorTreeView = BehaviorTreeViewDock.LastFocused.BehaviorTreeView;

                    if (behaviorTreeView != null)
                    {
                        behaviorTreeView.SelectedNodePending = node;
                        behaviorTreeView.SelectedAttachmentPending = attach;

                        behaviorTreeView.Refresh();
                    }
                }
            }
        }

        private void FindObject(FindType findType)
        {
            if (!string.IsNullOrEmpty(this.whatComboBox.Text))
            {
                for (int i = 0; i < this.whatComboBox.Items.Count; ++i)
                {
                    string what = (string)this.whatComboBox.Items[i];

                    if (what == this.whatComboBox.Text)
                    {
                        this.whatComboBox.Items.RemoveAt(i);
                        this.whatComboBox.Text = what;
                        break;
                    }
                }

                this.whatComboBox.Items.Insert(0, this.whatComboBox.Text);

                while (this.whatComboBox.Items.Count > 20)
                {
                    this.whatComboBox.Items.RemoveAt(this.whatComboBox.Items.Count - 1);
                }

                if (this.rangeComboBox.SelectedIndex > -1)
                {
                    FindObject(this.whatComboBox.Text,
                               (FindRange)this.rangeComboBox.SelectedIndex,
                               this.matchCaseCheckBox.Checked,
                               this.matchWholeWordCheckBox.Checked,
                               this.nodeTypeCheckBox.Checked,
                               Settings.Default.ShowVersionInfo && this.nodeIdCheckBox.Checked,
                               findType);
                }
            }
        }

        private void nodeTypeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.nodeTypeCheckBox.Checked)
            {
                this.nodeIdCheckBox.Checked = false;
            }
        }

        private void nodeIdCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.nodeIdCheckBox.Checked)
            {
                this.nodeTypeCheckBox.Checked = false;
            }
        }
    }
}
