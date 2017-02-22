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
using System.IO;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal class UIUtilities
    {
        /// <summary>
        /// Show the behavior tree view.
        /// </summary>
        /// <param name="behaviorFilename">The behavior filename in the workspace folder.</param>
        public static BehaviorNode ShowBehavior(string behaviorFilename)
        {
            if (string.IsNullOrEmpty(behaviorFilename))
            {
                return null;
            }

            if (!Path.IsPathRooted(behaviorFilename))
            {
                behaviorFilename = FileManagers.FileManager.GetFullPath(behaviorFilename);
            }

            if (!File.Exists(behaviorFilename))
            {
                if (!Plugin.WrongWorksapceReported)
                {
                    Plugin.WrongWorksapceReported = true;
                    string errorInfo = string.Format(Resources.WorkspaceDebugErrorInfo, behaviorFilename);

                    MessageBox.Show(errorInfo, Resources.WorkspaceError, MessageBoxButtons.OK);

                    ErrorInfoDock.Inspect();
                    ErrorInfoDock.WriteLine(errorInfo);
                }

                return null;
            }

            BehaviorNode behavior = null;

            bool bForceLoad = false;

            if (Plugin.EditMode == EditModes.Analyze && _agent_plannings.Count > 0)
            {
                bForceLoad = true;
            }

            if (!bForceLoad)
            {
                behavior = BehaviorManager.Instance.GetBehavior(behaviorFilename);
            }

            if (behavior == null)
            {
                behavior = BehaviorManager.Instance.LoadBehavior(behaviorFilename, bForceLoad);
            }

            if (behavior is Node)
            {
                BehaviorTreeList behaviorTreeList = BehaviorManager.Instance as BehaviorTreeList;

                if (behaviorTreeList != null)
                {
                    behaviorTreeList.ShowNode(behavior as Node);
                }
            }

            return behavior;
        }

        public static BehaviorTreeView ShowBehaviorTree(string behaviorFilename)
        {
            BehaviorNode behavior = ShowBehavior(behaviorFilename);
            return BehaviorTreeViewDock.GetBehaviorTreeView(behavior);
        }

        /// <summary>
        /// Show the behavior tree view with highlights.
        /// </summary>
        /// <param name="agentFullname">The fullname of an agent instance, as the format of "agnetType::instanceName".</param>
        /// <param name="frame">The current frame when connecting or playing.</param>
        public static BehaviorNode ShowBehaviorTree(string agentFullname, int frame, List<string> highlightedTransitionIds, List<string> highlightNodeIds, List<string> updatedNodeIds, HighlightBreakPoint highlightBreakPoint, Dictionary<string, FrameStatePool.NodeProfileInfos.ProfileInfo> profileInfos)
        {
            string behaviorFilename = (highlightBreakPoint != null) ? highlightBreakPoint.BehaviorFilename : FrameStatePool.GetBehaviorFilename(agentFullname, frame);

            if (!string.IsNullOrEmpty(behaviorFilename))
            {
                BehaviorTreeView behaviorTreeView = ShowBehaviorTree(behaviorFilename);

                if (behaviorTreeView != null)
                {
                    if (!Settings.Default.ShowProfilingInfo)
                    {
                        profileInfos = null;
                    }

                    behaviorTreeView.SetHighlights(highlightedTransitionIds, highlightNodeIds, updatedNodeIds, highlightBreakPoint, profileInfos);
                    //behaviorTreeView.Focus();

                    return behaviorTreeView.RootNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Sort the treeview by name and child count.
        /// </summary>
        /// <param name="nodes">All child tree nodes.</param>
        public static void SortTreeview(TreeNodeCollection nodes)
        {
            if (nodes.Count == 0)
            {
                return;
            }

            List<TreeNode> leaves = new List<TreeNode>();
            List<TreeNode> branches = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                if (node.GetNodeCount(false) > 0)
                {
                    branches.Add(node);
                    SortTreeview(node.Nodes);

                }
                else
                {
                    leaves.Add(node);
                }
            }

            nodes.Clear();

            if (leaves.Count > 0)
            {
                TreeNode[] leafArray = new TreeNode[leaves.Count];
                leaves.CopyTo(leafArray, 0);

                Comparison<TreeNode> compare = delegate(TreeNode tx, TreeNode ty)
                {
                    if (tx.Text.Length != ty.Text.Length)
                    {
                        return tx.Text.Length - ty.Text.Length;
                    }

                    return string.Compare(tx.Text, ty.Text);
                };

                Array.Sort<TreeNode>(leafArray, compare);

                nodes.AddRange(leafArray);
            }

            foreach (TreeNode node in branches)
            {
                nodes.Add(node);
            }
        }

        private static Dictionary<string, Dictionary<string, BehaviorNode>> _agent_plannings = new Dictionary<string, Dictionary<string, BehaviorNode>>();

        public static BehaviorTreeView ShowPlanning(FrameStatePool.PlanningProcess planning)
        {
            string behaviorFilename = FileManagers.FileManager.GetFullPath(planning._behaviorTree);

            if (!File.Exists(behaviorFilename))
            {
                return null;
            }

            string planningName = string.Format("PLanning_{0}_{1}", planning._frame, planning._index);

            Dictionary<string, BehaviorNode> list_plannings = null;

            if (!_agent_plannings.ContainsKey(planning._agentFullName))
            {
                list_plannings = new Dictionary<string, BehaviorNode>();
                _agent_plannings.Add(planning._agentFullName, list_plannings);

            }
            else
            {
                list_plannings = _agent_plannings[planning._agentFullName];
            }

            BehaviorNode behavior = null;

            if (!list_plannings.ContainsKey(planningName))
            {
                Plugin.PlanningProcess = planning;
                behavior = BehaviorManager.Instance.LoadBehavior(behaviorFilename, true);
                Behavior b = behavior as Behavior;
                b.PlanningProcess = planning;
                Plugin.PlanningProcess = null;

                b.PlanIsCollapseFailedBranch = Behavior.kPlanIsCollapseFailedBranch;

                list_plannings.Add(planningName, behavior);
                behavior.Filename = planningName;
                ((Node)behavior).Label = planningName;

            }
            else
            {
                behavior = list_plannings[planningName];
            }

            Debug.Check(behavior is Node);

            BehaviorTreeList behaviorTreeList = BehaviorManager.Instance as BehaviorTreeList;
            Debug.Check(behaviorTreeList != null);
            behaviorTreeList.ShowNode(behavior as Node);

            BehaviorTreeView view = BehaviorTreeViewDock.GetBehaviorTreeView(behavior);

            return view;
        }

        public static void ShowErrorDialog(ref ErrorCheckDialog errorDialog, BehaviorTreeList behaviorTreeList, BehaviorTreeView behaviorTreeView, Form owner, string title, List<Node.ErrorCheck> result)
        {
            // store the old position of the check dialogue and close it
            if (errorDialog != null)
            {
                errorDialog.Close();
            }

            // prepare the new dialogue
            errorDialog = new ErrorCheckDialog();
            errorDialog.Owner = owner;
            errorDialog.BehaviorTreeList = behaviorTreeList;
            errorDialog.BehaviorTreeView = behaviorTreeView;
            errorDialog.Text = title;

            // add the errors to the check dialogue
            foreach (Node.ErrorCheck check in result)
            {
                BehaviorNode behavior = check.Node != null ? check.Node.Behavior : null;

                // group the errors by the behaviour their occured in

                // get the group for the error's behaviour
                ListViewGroup group = null;

                foreach (ListViewGroup grp in errorDialog.listView.Groups)
                {
                    if (grp.Tag == behavior)
                    {
                        group = grp;
                        break;
                    }
                }

                // if there is no group, create it
                if (group == null)
                {
                    group = new ListViewGroup(behavior != null ? ((Node)behavior).Label : "Error");
                    group.Tag = behavior;
                    errorDialog.listView.Groups.Add(group);
                }

                // create an item for the error in the group
                ListViewItem item = new ListViewItem(check.Description);
                item.Group = group;
                item.Tag = check.Node;

                switch (check.Level)
                {
                    case (ErrorCheckLevel.Message):
                        item.ImageIndex = 0;
                        break;

                    case (ErrorCheckLevel.Warning):
                        item.ImageIndex = 1;
                        break;

                    case (ErrorCheckLevel.Error):
                        item.ImageIndex = 2;
                        break;
                }

                errorDialog.listView.Items.Add(item);
            }

            // if no errors were found, tell the user so
            if (result.Count < 1)
            {
                errorDialog.listView.Items.Add(new ListViewItem("No Errors.", (int)ErrorCheckLevel.Message));
            }

            // show the dialogue
            errorDialog.Show();
        }
    }
}
