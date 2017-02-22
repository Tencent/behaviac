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
using System.IO;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class BreakPointsDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static BreakPointsDock _breakPointsDock = null;

        internal static void Inspect()
        {
            if (_breakPointsDock == null)
            {
                _breakPointsDock = new BreakPointsDock();
                _breakPointsDock.Show(MainWindow.Instance.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);

            }
            else
            {
                _breakPointsDock.Show();
            }

            _breakPointsDock.setDataGrid();
        }

        internal static void CloseAll()
        {
            if (_breakPointsDock != null)
            {
                _breakPointsDock.Close();
            }
        }

        public BreakPointsDock()
        {
            _breakPointsDock = this;

            InitializeComponent();

            this.TabText = this.Text;

            initDataGrid();

            dataGridView.CurrentCellDirtyStateChanged += new EventHandler(dataGridView_CurrentCellDirtyStateChanged);
            dataGridView.CellValueChanged += new DataGridViewCellEventHandler(dataGridView_CellValueChanged);
            dataGridView.CellMouseUp += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseUp);
            dataGridView.CellDoubleClick += new DataGridViewCellEventHandler(dataGridView_CellDoubleClick);

            DebugDataPool.LoadBreakPointsHandler += new DebugDataPool.LoadBreakPointsDelegate(setDataGrid);
            DebugDataPool.AddBreakPointHandler += new DebugDataPool.AddBreakPointDelegate(addBreakPoint);
            DebugDataPool.RemoveBreakPointHandler += new DebugDataPool.RemoveBreakPointDelegate(removeBreakPoint);
        }

        protected override void OnClosed(EventArgs e)
        {
            _breakPointsDock = null;

            DebugDataPool.LoadBreakPointsHandler -= setDataGrid;
            DebugDataPool.AddBreakPointHandler -= addBreakPoint;
            DebugDataPool.RemoveBreakPointHandler -= removeBreakPoint;

            base.OnClosed(e);
        }

        private void initDataGrid()
        {
            DataGridViewCheckBoxColumn enableColumn = new DataGridViewCheckBoxColumn();
            enableColumn.Name = "Enable";
            enableColumn.HeaderText = Resources.Enable;
            enableColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            enableColumn.MinimumWidth = 50;

            DataGridViewTextBoxColumn behaviorFileColumn = new DataGridViewTextBoxColumn();
            behaviorFileColumn.Name = "BehaviorFilename";
            behaviorFileColumn.HeaderText = Resources.BehaviorFilename;
            behaviorFileColumn.ReadOnly = true;
            behaviorFileColumn.MinimumWidth = 180;

            DataGridViewTextBoxColumn nodeTypeColumn = new DataGridViewTextBoxColumn();
            nodeTypeColumn.Name = "NodeType";
            nodeTypeColumn.HeaderText = Resources.NodeType;
            nodeTypeColumn.ReadOnly = true;
            nodeTypeColumn.MinimumWidth = 120;

            DataGridViewTextBoxColumn nodeIdColumn = new DataGridViewTextBoxColumn();
            nodeIdColumn.Name = "NodeId";
            nodeIdColumn.HeaderText = Resources.NodeId;
            nodeIdColumn.ReadOnly = true;
            nodeIdColumn.MinimumWidth = 100;

            DataGridViewTextBoxColumn actionNameColumn = new DataGridViewTextBoxColumn();
            actionNameColumn.Name = "ActionName";
            actionNameColumn.HeaderText = Resources.ActionName;
            actionNameColumn.ReadOnly = true;
            actionNameColumn.MinimumWidth = 100;

            DataGridViewComboBoxColumn actionResultColumn = new DataGridViewComboBoxColumn();
            actionResultColumn.Name = "ActionResult";
            actionResultColumn.HeaderText = Resources.ActionResult;
            actionResultColumn.MinimumWidth = 100;
            actionResultColumn.Items.Add(DebugDataPool.Action.kResultAll);
            actionResultColumn.Items.Add(DebugDataPool.Action.kResultSuccess);
            actionResultColumn.Items.Add(DebugDataPool.Action.kResultFailure);

            DataGridViewTextBoxColumn hitCountColumn = new DataGridViewTextBoxColumn();
            hitCountColumn.Name = "HitCount";
            hitCountColumn.HeaderText = Resources.HitCount;
            hitCountColumn.MinimumWidth = 100;

            DataGridViewTextBoxColumn dummyColumn = new DataGridViewTextBoxColumn();
            dummyColumn.Name = "Dummy";
            dummyColumn.HeaderText = "";
            dummyColumn.ReadOnly = true;
            dummyColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView.Columns.Add(enableColumn);
            dataGridView.Columns.Add(behaviorFileColumn);
            dataGridView.Columns.Add(nodeTypeColumn);
            dataGridView.Columns.Add(nodeIdColumn);
            dataGridView.Columns.Add(actionNameColumn);
            dataGridView.Columns.Add(actionResultColumn);
            dataGridView.Columns.Add(hitCountColumn);
            dataGridView.Columns.Add(dummyColumn);
        }

        private bool _isReady = true;

        private void setDataGrid()
        {
            dataGridView.Rows.Clear();

            // Add all breakpoints from the pool.
            foreach (string behaviorFilename in DebugDataPool.BreakPoints.Keys)
            {
                foreach (string nodeId in DebugDataPool.BreakPoints[behaviorFilename].Keys)
                {
                    DebugDataPool.BreakPoint breakPoint = DebugDataPool.BreakPoints[behaviorFilename][nodeId];

                    foreach (DebugDataPool.Action action in breakPoint.Actions)
                    {
                        newBreakPoint(behaviorFilename, breakPoint.NodeType, nodeId, action);
                    }
                }
            }

            dataGridView.CurrentCell = null;
        }

        private int newBreakPoint(string behaviorFilename, string nodeType, string nodeId, DebugDataPool.Action action)
        {
            _isReady = false;

            int index = getRowIndex(behaviorFilename, nodeId, action.Name);

            if (index < 0)
            {
                index = dataGridView.Rows.Add();
            }

            DataGridViewRow row = dataGridView.Rows[index];
            row.Tag = action;

            row.Cells["Enable"].Value = action.Enable;
            row.Cells["BehaviorFilename"].Value = behaviorFilename;
            row.Cells["NodeType"].Value = nodeType;
            row.Cells["NodeId"].Value = nodeId;
            row.Cells["ActionName"].Value = action.Name;
            row.Cells["ActionResult"].Value = action.Result;
            row.Cells["HitCount"].Value = action.HitCount;

            _isReady = true;

            return index;
        }

        private void addBreakPoint(string behaviorFilename, string nodeType, string nodeId, DebugDataPool.Action action)
        {
            if (!_isReady)
            {
                return;
            }

            int index = newBreakPoint(behaviorFilename, nodeType, nodeId, action);
            DataGridViewRow row = dataGridView.Rows[index];
            dataGridView.CurrentCell = row.Cells["BehaviorFilename"];

            dataGridView.Sort(dataGridView.Columns["BehaviorFilename"], System.ComponentModel.ListSortDirection.Ascending);
            selectRowNode(row);
        }

        private void removeBreakPoint(string behaviorFilename, string nodeType, string nodeId, DebugDataPool.Action action)
        {
            if (!_isReady)
            {
                return;
            }

            int index = getRowIndex(behaviorFilename, nodeId, action.Name);

            if (index > -1)
            {
                dataGridView.Rows.RemoveAt(index);

                if (dataGridView.CurrentCell != null)
                {
                    DataGridViewRow row = dataGridView.Rows[dataGridView.CurrentCell.RowIndex];
                    row.Selected = true;
                }
            }
        }

        private int getRowIndex(string behaviorFilename, string nodeId, string actionName)
        {
            for (int index = 0; index < dataGridView.Rows.Count; index++)
            {
                DataGridViewRow row = dataGridView.Rows[index];

                if (behaviorFilename == (string)row.Cells["BehaviorFilename"].Value &&
                    nodeId == (string)row.Cells["NodeId"].Value &&
                    actionName == getResourceName((string)row.Cells["ActionName"].Value))
                {
                    return index;
                }
            }

            return -1;
        }

        private void selectRowNode(DataGridViewRow row)
        {
            Debug.Check(row != null);
            if (row != null)
            {
                row.Selected = true;

                string behaviorFilename = (string)row.Cells["BehaviorFilename"].Value;
                BehaviorTreeView behaviorTreeView = UIUtilities.ShowBehaviorTree(behaviorFilename);

                if (behaviorTreeView != null)
                {
                    string nodeId = (string)row.Cells["NodeId"].Value;
                    NodeViewData nvd = behaviorTreeView.RootNodeView.FindNodeViewData(nodeId);

                    if (nvd != null)
                    {
                        behaviorTreeView.SelectedNode = nvd;
                        //if (behaviorTreeView.ClickNode != null)
                        //    behaviorTreeView.ClickNode(nvd);
                        behaviorTreeView.Invalidate();
                    }
                }
            }
        }

        private int getHitCount(DataGridViewRow row)
        {
            Debug.Check(row != null);

            int actionHitCount = 0;

            if (row != null)
            {
                try
                {
                    object hitCount = row.Cells["HitCount"].Value;

                    if (hitCount is string)
                    {
                        actionHitCount = int.Parse((string)hitCount);
                    }

                    else if (hitCount is int)
                    {
                        actionHitCount = (int)hitCount;
                    }

                }
                catch
                {
                }

                if (actionHitCount < 0)
                {
                    actionHitCount = 0;
                }

                row.Cells["HitCount"].Value = actionHitCount;
            }

            return actionHitCount;
        }

        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!_isReady)
            {
                return;
            }

            if (dataGridView.IsCurrentCellDirty)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private string getResourceName(string resourceLabel)
        {
            if (resourceLabel == Plugin.GetResourceString("enter"))
            {
                return "enter";
            }

            if (resourceLabel == Plugin.GetResourceString("exit"))
            {
                return "exit";
            }

            if (resourceLabel == Plugin.GetResourceString(DebugDataPool.Action.kResultAll))
            {
                return DebugDataPool.Action.kResultAll;
            }

            if (resourceLabel == Plugin.GetResourceString(DebugDataPool.Action.kResultSuccess))
            {
                return DebugDataPool.Action.kResultSuccess;
            }

            if (resourceLabel == Plugin.GetResourceString(DebugDataPool.Action.kResultFailure))
            {
                return DebugDataPool.Action.kResultFailure;
            }

            return resourceLabel;
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isReady)
            {
                return;
            }

            DataGridViewRow row = dataGridView.Rows[e.RowIndex];
            DebugDataPool.Action action = row.Tag as DebugDataPool.Action;

            if (action != null)
            {
                bool actionEnable = (bool)row.Cells["Enable"].Value;
                string behaviorFilename = (string)row.Cells["BehaviorFilename"].Value;
                string nodeType = (string)row.Cells["NodeType"].Value;
                string nodeId = (string)row.Cells["NodeId"].Value;
                string actionName = getResourceName((string)row.Cells["ActionName"].Value);
                string actionResult = getResourceName((string)row.Cells["ActionResult"].Value);
                int hitCount = getHitCount(row);

                selectRowNode(row);

                _isReady = false;
                DebugDataPool.AddBreakPoint(behaviorFilename, nodeId, nodeType, actionName, actionEnable, actionResult, hitCount);
                _isReady = true;
            }
        }

        private void dataGridView_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dataGridView.Rows[e.RowIndex];
            row.Selected = true;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dataGridView.Rows[e.RowIndex];
            selectRowNode(row);
        }

        private void deleteBreakpoints(string[] behaviorFilenames, string[] nodeIds, DebugDataPool.Action[] actions)
        {
            Debug.Check(behaviorFilenames.Length == nodeIds.Length && nodeIds.Length == actions.Length);

            for (int i = 0; i < behaviorFilenames.Length; i++)
            {
                DebugDataPool.RemoveBreakPoint(behaviorFilenames[i], nodeIds[i], actions[i]);
            }

            BehaviorTreeViewDock.RefreshAll();
            Inspect();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            int rowCount = dataGridView.SelectedRows.Count;

            if (rowCount < 0)
            {
                return;
            }

            DialogResult dr = MessageBox.Show(Resources.DeleteBreakpointsWarning,
                                              Resources.DeleteSelectedBreakpoints, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr != DialogResult.Yes)
            {
                return;
            }

            string[] behaviorFilenames = new string[rowCount];
            string[] nodeIds = new string[rowCount];
            DebugDataPool.Action[] actions = new DebugDataPool.Action[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                DataGridViewRow row = dataGridView.SelectedRows[i];

                behaviorFilenames[i] = (string)row.Cells["BehaviorFilename"].Value;
                nodeIds[i] = (string)row.Cells["NodeId"].Value;
                actions[i] = row.Tag as DebugDataPool.Action;
            }

            deleteBreakpoints(behaviorFilenames, nodeIds, actions);
        }

        private void deleteAllButton_Click(object sender, EventArgs e)
        {
            int rowCount = dataGridView.Rows.Count;

            if (rowCount < 0)
            {
                return;
            }

            DialogResult dr = MessageBox.Show(Resources.DeleteAllBreakpointsWarning,
                                              Resources.DeleteAllBreakpoints, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr != DialogResult.Yes)
            {
                return;
            }

            string[] behaviorFilenames = new string[rowCount];
            string[] nodeIds = new string[rowCount];
            DebugDataPool.Action[] actions = new DebugDataPool.Action[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                DataGridViewRow row = dataGridView.Rows[i];

                behaviorFilenames[i] = (string)row.Cells["BehaviorFilename"].Value;
                nodeIds[i] = (string)row.Cells["NodeId"].Value;
                actions[i] = row.Tag as DebugDataPool.Action;
            }

            deleteBreakpoints(behaviorFilenames, nodeIds, actions);
        }

        private void enableBreakpoints(DataGridViewRow[] rows, bool enable)
        {
            DataGridViewRow lastRow = null;

            foreach (DataGridViewRow row in rows)
            {
                _isReady = false;

                lastRow = row;
                row.Cells["Enable"].Value = enable;

                string behaviorFilename = (string)row.Cells["BehaviorFilename"].Value;
                string nodeType = (string)row.Cells["NodeType"].Value;
                string nodeId = (string)row.Cells["NodeId"].Value;
                string actionName = getResourceName((string)row.Cells["ActionName"].Value);
                string actionResult = getResourceName((string)row.Cells["ActionResult"].Value);
                int hitCount = getHitCount(row);

                DebugDataPool.AddBreakPoint(behaviorFilename, nodeId, nodeType, actionName, enable, actionResult, hitCount);
                _isReady = true;
            }

            if (lastRow != null)
            {
                selectRowNode(lastRow);
                dataGridView.Update();
            }
        }

        private void enableSelectedBreakpoints(bool enable)
        {
            int rowCount = dataGridView.SelectedRows.Count;

            if (rowCount < 0)
            {
                return;
            }

            DataGridViewRow[] rows = new DataGridViewRow[rowCount];
            dataGridView.SelectedRows.CopyTo(rows, 0);

            enableBreakpoints(rows, enable);
        }

        private void enableButton_Click(object sender, EventArgs e)
        {
            enableSelectedBreakpoints(true);
        }

        private void disableButton_Click(object sender, EventArgs e)
        {
            enableSelectedBreakpoints(false);
        }

        private void enableAllBreakpoints(bool enable)
        {
            int rowCount = dataGridView.Rows.Count;

            if (rowCount < 0)
            {
                return;
            }

            DataGridViewRow[] rows = new DataGridViewRow[rowCount];
            dataGridView.Rows.CopyTo(rows, 0);

            enableBreakpoints(rows, enable);
        }

        private void enableAllButton_Click(object sender, EventArgs e)
        {
            enableAllBreakpoints(true);
        }

        private void disableAllButton_Click(object sender, EventArgs e)
        {
            enableAllBreakpoints(false);
        }

        private void showButton_Click(object sender, EventArgs e)
        {
            HighlightBreakPoint.ShowBreakPoint = !HighlightBreakPoint.ShowBreakPoint;
            BehaviorTreeViewDock.RefreshAll();

            if (HighlightBreakPoint.ShowBreakPoint)
            {
                showButton.ToolTipText = Resources.BreakpointsHide;
            }

            else
            {
                showButton.ToolTipText = Resources.BreakpointsShow;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Debug.Check(Workspace.Current != null);
            DebugDataPool.Save(Workspace.Current.FileName);
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteButton_Click(sender, null);
            }
        }
    }
}
