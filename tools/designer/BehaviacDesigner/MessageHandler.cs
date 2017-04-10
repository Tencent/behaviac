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
using System.Xml;
using System.Windows.Forms;
using Behaviac.Design.Data;
using Behaviac.Design.Properties;
using Behaviac.Design.Network;

namespace Behaviac.Design
{
    class MessageHandler
    {
        internal static void RegisterMessageHandler()
        {
            MessageQueue.ProcessMessageHandler -= processMessage;
            MessageQueue.ProcessMessageHandler += processMessage;
        }

        private static Dictionary<string, bool> _checkedBehaviorFiles = new Dictionary<string, bool>();

        public static void Init()
        {
            _checkedBehaviorFiles.Clear();
        }

        private static string ms_fileFormat = "xml";

        private static UpdateModes processMessage(string _msg)
        {
            try
            {
                //skip index
                //string msg = _msg.Substring(10);
                int pos = _msg.IndexOf("][");
                string msg = _msg.Substring(pos + 1);

                if (msg.StartsWith("[platform]"))
                {
                    processPlatform(msg);
                }
                else if (msg.StartsWith("[workspace]"))
                {
                    processWorkspace(msg);

                    TimelineDock.Continue();

                }
                else if (msg.StartsWith("[connected]"))
                {
                    processConnected();

                }
                else if (msg.StartsWith("[frame]"))
                {
                    // [frame]0
                    AgentDataPool.TotalFrames = (int.Parse(msg.Substring(7)));

                }
                else if (msg.StartsWith("[property]"))
                {
                    processProperty(msg);

                }
                else if (msg.StartsWith("[tick]"))
                {
                    processTick(msg);

                }
                else if (msg.StartsWith("[jump]"))
                {
                    processJump(msg);

                }
                else if (msg.StartsWith("[return]"))
                {
                    processReturn(msg);

                }
                else if (msg.StartsWith("[plan_"))
                {
                    string m = msg.Trim();

                    //Console.WriteLine(m);

                    processPlanning(m);

                }
                else if (msg.StartsWith("[breaked]"))
                {
                    processBreaked(msg);

                }
                else if (msg.StartsWith("[continue]"))
                {
                    if (MessageQueue.ContinueHandler != null)
                    {
                        MessageQueue.ContinueHandler(msg);
                    }

                }
                else if (msg.StartsWith("[applog]"))
                {
                    int frame = AgentDataPool.TotalFrames;
                    FrameStatePool.AddAppLog(frame, msg);

                }
                else if (msg.StartsWith("[log]"))
                {
                    int frame = AgentDataPool.TotalFrames;
                    FrameStatePool.AddLog(frame, msg);

                }
                else if (msg.StartsWith("[profiler]"))
                {
                    //[profiler]ships\0_basic.xml->BehaviorTree[-1] 685
                    string[] tokens = msg.Substring(10).Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    if (tokens.Length == 2)
                    {
                        string[] nodes = tokens[0].Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

                        if (nodes.Length == 2)
                        {
                            string behaviorFilename = nodes[0];

                            string[] ids = nodes[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                            if (ids.Length == 2)
                            {
                                //convert microsecond to millisecond by mul 0.001
                                float milliseconds = 0.001f * int.Parse(tokens[1]);
                                FrameStatePool.SetProfileInfo(AgentDataPool.TotalFrames, behaviorFilename, ids[1], milliseconds);
                            }
                        }
                    }
                }

            }
            catch
            {
                string errorInfo = string.Format("The message \"{0}\" was not processed.", _msg);
                Console.WriteLine(errorInfo);
                MessageBox.Show(errorInfo, "Error", MessageBoxButtons.OK);
            }

            return Plugin.UpdateMode;
        }

        private static void processConnected()
        {
            MessageQueue.IsConnected = true;

            NetworkManager.Instance.SendProfiling(Settings.Default.ShowProfilingInfo);

            NetworkManager.Instance.SendText("[start]\n");

            NetworkManager.Instance.SendLoadedBreakpoints();

            Plugin.UpdateMode = UpdateModes.Continue;
        }

        private static void processPlatform(string msg)
        {
            Utilities.RuntimePlatform = msg.Substring(10).Trim();
        }

        private static void processWorkspace(string msg)
        {
            if (Plugin.WorkspaceDelegateHandler != null)
            {
                string str_ = msg.Substring(11);
                string format = "";

                //skip the space
                int pos = 1;

                for (; pos < str_.Length; ++pos)
                {
                    format += str_[pos];

                    if (str_[pos] == ' ')
                    {
                        break;
                    }
                }

                ms_fileFormat = format.Trim();

                Debug.Check(ms_fileFormat == "xml" || ms_fileFormat == "bson.bytes" || ms_fileFormat == Workspace.Current.Language);

                MainWindow.Instance.NodeTreeList.SetNodeList();

                AgentDataPool.TotalFrames = 0;
            }
        }

        private static void processBreaked(string msg)
        {
            string msg_real = msg.Substring(9);

            if (msg_real.StartsWith("[applog]"))
            {
                FrameStatePool.RespondToCPPBreak(AgentDataPool.TotalFrames, msg_real);
            }
            else
            {
                //[breaked]Ship::Ship_1 ships\basic.xml->BehaviorTree[0]:enter [all/success/failure] [1]
                string[] tokens = msg_real.Split(' ');

                if (tokens.Length == 4)
                {
                    string agentFullname = tokens[0];
                    //if (string.IsNullOrEmpty(Plugin.DebugAgentInstance))
                    {
                        Plugin.DebugAgentInstance = agentFullname;
                    }

                    string[] nodes = tokens[1].Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

                    if (nodes.Length == 2)
                    {
                        string[] actions = nodes[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                        if (actions.Length == 3)
                        {
                            Debug.Check(actions[2].StartsWith(":"));
                            string actionName = actions[2].Substring(1);
                            string[] actionResults = tokens[2].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                            //although actionResult can be EAR_none or EAR_all, but, as this is the real result of an action
                            //it can only be success or failure
                            Debug.Check(actionResults.Length == 1 && (actionResults[0] == "success" || actionResults[0] == "failure") || actionResults[0] == "all");

                            string[] hitCounts = tokens[3].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                            int hitCount = int.Parse(hitCounts[0]);

                            if (actionName == "plan")
                            {
                                NodeTreeList.ShowPlanning(agentFullname, AgentDataPool.TotalFrames, hitCount);
                            }

                            FrameStatePool.RespondToAPPBreak(agentFullname, AgentDataPool.TotalFrames, nodes[0], actions[1], actionName, actionResults[0], hitCount);
                        }
                    }
                }
            }

            Plugin.UpdateMode = UpdateModes.Break;
        }

        private static bool StartsWith(string msg, string keyWord, ref string[] tokens)
        {
            if (msg.StartsWith(keyWord))
            {
                int len = keyWord.Length;
                tokens = msg.Substring(len).Split(' ');

                return true;
            }

            return false;
        }


        private static bool _ms_planning = false;
        private static void processPlanning(string msg)
        {
            string[] tokens = null;

            if (MessageHandler.StartsWith(msg, "[plan_begin]", ref tokens))
            {
                //[plan_begin]CleaningRobot::RobotController#Controller CleanRooms.xml->Task[0]
                if (tokens.Length == 3)
                {
                    FrameStatePool.PlanBegin(tokens[0], tokens[1], tokens[2]);
                    _ms_planning = true;
                }

            }
            else if (FrameStatePool.PlanningProcess._planning != null)
            {
                if (MessageHandler.StartsWith(msg, "[plan_end]", ref tokens))
                {
                    //[plan_end]CleaningRobot::RobotController#Controller CleanRooms.xml->Task[0]
                    if (tokens.Length == 2)
                    {
                        FrameStatePool.PlanEnd(tokens[0], tokens[1]);
                        _ms_planning = false;
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_method_begin]", ref tokens))
                {
                    //[plan_method_begin]CleanRooms.xml->Method[13]
                    if (tokens.Length == 1)
                    {
                        FrameStatePool.PlanningMethodBegin(tokens[0]);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_method_end]", ref tokens))
                {
                    //[plan_method_end]CleanRooms.xml->Method[13] success
                    if (tokens.Length == 2)
                    {
                        FrameStatePool.PlanningMethodEnd(tokens[0], tokens[1]);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_node_begin]", ref tokens))
                {
                    //[plan_node_begin]CleanRooms.xml->Method[8]
                    if (tokens.Length == 1)
                    {
                        FrameStatePool.PlanningNodeBegin(tokens[0]);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_node_pre_failed]", ref tokens))
                {
                    //[plan_node_pre_failed]CleanRooms.xml->Method[1]
                    if (tokens.Length == 1)
                    {
                        FrameStatePool.PlanningNodePreFailed(tokens[0]);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_node_end]", ref tokens))
                {
                    //[plan_node_end]CleanRooms.xml->Method[1] failure
                    if (tokens.Length == 2)
                    {
                        FrameStatePool.PlanningNodeEnd(tokens[0], tokens[1]);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_referencetree_enter]", ref tokens))
                {
                    //[plan_referencetree_enter]CleanRooms.xml->ReferencedBehavior[10] MoveToRoom.xml
                    if (tokens.Length == 2)
                    {
                        string fullId = tokens[0];
                        string behaviorFilename = tokens[1];

                        FrameStatePool.PlanningReferencedEnter(fullId, behaviorFilename);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_referencetree_exit]", ref tokens))
                {
                    //[plan_referencetree_exit]CleanRooms.xml->ReferencedBehavior[10] MoveToRoom.xml
                    if (tokens.Length == 2)
                    {
                        string fullId = tokens[0];
                        string behaviorFilename = tokens[1];

                        FrameStatePool.PlanningReferencedExit(fullId, behaviorFilename);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_foreach_begin]", ref tokens))
                {
                    //[plan_foreach_begin]MoveToRoom.xml->DecoratorIterator[5] 0 2
                    if (tokens.Length == 3)
                    {
                        string indexStr = tokens[1];
                        string countStr = tokens[2];

                        FrameStatePool.PlanningForEachBegin(tokens[0], indexStr, countStr);
                    }

                }
                else if (MessageHandler.StartsWith(msg, "[plan_foreach_end]", ref tokens))
                {
                    //[plan_foreach_end]MoveToRoom.xml->DecoratorIterator[5] 0 2 success
                    if (tokens.Length == 4)
                    {
                        string indexStr = tokens[1];
                        string countStr = tokens[2];
                        string result = tokens[3];

                        FrameStatePool.PlanningForEachEnd(tokens[0], indexStr, countStr, result);
                    }

                }
                else
                {
                    Debug.Check(false);
                }
            }
        }

        private static void processReturn(string msg)
        {
            try
            {
                // [return]CleaningRobot::RobotController#Controller CleanRooms.xml
                string[] tokens = msg.Substring(8).Split(' ');

                if (tokens.Length == 2)
                {
                    string[] types = tokens[0].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Check(types.Length == 2);
                    string agentType = types[0];
                    string agentName = types[1];
                    string agentFullname = tokens[0];

                    //AgentInstancePool.AddInstance(agentType, agentName, true);

                    string returnFromTree = tokens[1].Trim(new char[] { '\n' });

                    FrameStatePool.SetReturnInfo(agentFullname, returnFromTree);
                }
            }
            catch
            {
            }
        }

        private static void processJump(string msg)
        {
            try
            {
                // [jump]CleaningRobot::RobotController#Controller CleanRooms.xml

                string[] tokens = msg.Substring(6).Split(' ');

                if (tokens.Length == 2)
                {
                    string[] types = tokens[0].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Check(types.Length == 2);
                    string agentType = types[0];
                    string agentName = types[1];
                    string agentFullname = tokens[0];

                    //AgentInstancePool.AddInstance(agentType, agentName, true);
                    string jumpTree = tokens[1].Trim(new char[] { '\n' });

                    checkBehaviorFiles(jumpTree);
                    FrameStatePool.SetJumpInfo(agentFullname, jumpTree, false);
                }
            }
            catch
            {
            }
        }

        private static void processTick(string msg)
        {
            // [tick]Ship::Ship_1 ships\basic.xml->BehaviorTree[0]:enter [all/success/failure] [1]
            // [tick]Ship::Ship_1 ships\basic.xml->BehaviorTree[0]:update [1]

            string[] tokens = msg.Substring(6).Split(' ');

            if (tokens.Length == 4)
            {
                string[] types = tokens[0].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Check(types.Length == 2);
                string agentType = types[0];
                string agentName = types[1];
                string agentFullname = tokens[0];

                AgentInstancePool.AddInstance(agentType, agentName);

                string[] nodes = tokens[1].Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

                if (nodes.Length == 2)
                {
                    string behaviorFilename = nodes[0];

                    checkBehaviorFiles(behaviorFilename);
                    FrameStatePool.SetJumpInfo(agentFullname, behaviorFilename, true);

                    behaviorFilename = FrameStatePool.GetCurrentBehaviorTree(agentFullname, behaviorFilename);

                    string[] actions = nodes[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                    if (actions.Length == 3)
                    {
                        string[] actionResults = tokens[2].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] hitCounts = tokens[3].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                        int hitCount = int.Parse(hitCounts[0]);
                        string nodeId = actions[1];

                        if (actions[2] == ":enter")
                        {
                            FrameStatePool.EnterNode(agentFullname, AgentDataPool.TotalFrames, behaviorFilename, nodeId, actionResults[0], hitCount);

                        }
                        else if (actions[2] == ":exit")
                        {
                            FrameStatePool.ExitNode(agentFullname, AgentDataPool.TotalFrames, behaviorFilename, nodeId, actionResults[0], hitCount);

                        }
                        else if (actions[2] == ":update")
                        {
                            List<string> highlightNodeIds = FrameStatePool.GetHighlightNodeIds(agentFullname, AgentDataPool.TotalFrames, behaviorFilename);

                            if (highlightNodeIds != null && !highlightNodeIds.Contains(nodeId))
                            {
                                FrameStatePool.EnterNode(agentFullname, AgentDataPool.TotalFrames, behaviorFilename, nodeId, actionResults[0], hitCount);
                            }

                        }
                        else if (actions[2] == ":transition")
                        {
                            FrameStatePool.UpdateTransition(agentFullname, AgentDataPool.TotalFrames, behaviorFilename, nodeId, actionResults[0]);

                        }
                    }
                }
            }
        }

        private static void processProperty(string msg)
        {
            // Par:   [property]a->10
            // World: [property]WorldTest::World WorldTest::Property3->10
            // Agent: [property]AgentTest::AgentTest_1 AgentTest::Property5::type::name->10

            string[] tokens = msg.Substring(10).Split(' ');
            Debug.Check(tokens.Length > 0);

            string agentType = string.Empty;
            string agentName = string.Empty;
            string agentFullname = string.Empty;

            // Par
            if (tokens.Length == 1)
            {
                agentType = string.Empty;
                agentFullname = tokens[0];
            }

            // Global or Agent
            else
            {
                string[] types = tokens[0].Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Check(types.Length == 2);
                agentType = types[0];
                agentName = types[1];
                agentFullname = tokens[0];

                AgentInstancePool.AddInstance(agentType, agentName);
            }

            Debug.Check(!string.IsNullOrEmpty(agentFullname));

            string[] values = tokens[tokens.Length - 1].Split(new string[] { "->", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (values.Length == 2)
            {
                string basicValueName = values[0];
                int index = basicValueName.LastIndexOf(":");
                basicValueName = basicValueName.Substring(index + 1);

                if (_ms_planning)
                {
                    FrameStatePool.PlanProperty(agentFullname, basicValueName, values[1]);

                }
                else
                {
                    if (AgentDataPool.TotalFrames > -1 && !string.IsNullOrEmpty(agentFullname))
                    {
                        AgentDataPool.AddValue(agentFullname, basicValueName, AgentDataPool.TotalFrames, values[1]);
                    }
                }
            }
        }

        private static void checkBehaviorFiles(string behaviorFilename)
        {
            if (_checkedBehaviorFiles.ContainsKey(behaviorFilename))
            {
                return;
            }

            _checkedBehaviorFiles[behaviorFilename] = true;

            try
            {
                string sourceFilename = Path.Combine(Workspace.Current.SourceFolder, behaviorFilename);
                string exportedFilename = Path.Combine(Workspace.Current.DefaultExportFolder, behaviorFilename);

                //remove the extension
                int pos = exportedFilename.IndexOf(".xml");

                if (pos != -1)
                {
                    exportedFilename = exportedFilename.Remove(pos);
                }

                exportedFilename += string.Format(".{0}", ms_fileFormat);

                if (ms_fileFormat == Workspace.Current.Language)
                {
                    string folder = Workspace.Current.GetExportAbsoluteFolder(Workspace.Current.Language);

                    if (Workspace.Current.Language == "cpp")
                    {
                        exportedFilename = Path.Combine(folder, "behaviac_generated/behaviors/behaviac_generated_behaviors.h");
                    }
                    else
                    {
                        exportedFilename = Path.Combine(folder, "behaviac_generated/behaviors/generated_behaviors.cs");
                    }
                }

                if (File.Exists(sourceFilename) && File.Exists(exportedFilename))
                {
                    FileInfo sourceFileInfo = new FileInfo(sourceFilename);
                    FileInfo exportedFileInfo = new FileInfo(exportedFilename);

                    //src file is modified after begin exported?
                    if (sourceFileInfo.LastWriteTime.CompareTo(exportedFileInfo.LastWriteTime) > 0)
                    {
                        string info = string.Format(Resources.FileModifiedInfo, behaviorFilename);
                        MessageBox.Show(info, Resources.FileModified, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    if (!Plugin.WrongWorksapceReported)
                    {
                        Plugin.WrongWorksapceReported = true;
                        string errorInfo = string.Format(Resources.WorkspaceDebugErrorInfo, behaviorFilename);

                        MessageBox.Show(errorInfo, Resources.WorkspaceError, MessageBoxButtons.OK);

                        ErrorInfoDock.Inspect();
                        ErrorInfoDock.WriteLine(errorInfo);
                    }
                }

            }
            catch (Exception)
            {
            }
        }
    }
}
