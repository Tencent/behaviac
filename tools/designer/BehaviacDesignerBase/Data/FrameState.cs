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
using System.Runtime.Serialization.Formatters.Binary;

namespace Behaviac.Design.Data
{
    #region Planning
    public class FrameStatePool
    {
        [Serializable]
        public class PlanningState
        {
            public string _id;
            public string _nodeId;
            public bool _bPreFailed = false;
            public bool _bIndexFailed = false;
            public bool _bOk = false;
            public int _index = -1;
            public int _count = -1;

            public PlanningState _parent = null;
            public List<PlanningState> _children = new List<PlanningState>();
            public Dictionary<string, Dictionary<string, object>> _agents = new Dictionary<string, Dictionary<string, object>>();

            public PlanningState(string fullId, string nodeId, PlanningState parent)
            {
                this._id = fullId;
                this._nodeId = nodeId;
                this._parent = parent;

                if (parent != null)
                {
                    if (parent._id.IndexOf("DecoratorIterator[") >= 0 && parent._index == -1 && fullId.IndexOf("+") == -1)
                    {
                        Debug.Check(true);
                    }

                    parent._children.Add(this);
                }
            }
        }


        [Serializable]
        public class PlanningProcess
        {
            public string _agentFullName;
            public int _frame;
            public int _index;

            public string _behaviorTree;

            public Dictionary<string, PlanningState> _states = new Dictionary<string, PlanningState>();

            public List<string> _parentNodeIds = new List<string>();

            public PlanningProcess(string a, int f, int index)
            {
                this._agentFullName = a;
                this._frame = f;
                this._index = index;
            }

            public static PlanningProcess _planning = null;
            public PlanningState _rootState = null;
            public PlanningState _currentState = null;

            public PlanningState GetNode(string fullId)
            {
                if (_states.ContainsKey(fullId))
                {
                    return _states[fullId];
                }

                return null;
            }

            //find the last recent parent which contains state data if the current one has no data
            public PlanningState GetLastNode(NodeViewData nvd)
            {
                while (nvd != null && !_states.ContainsKey(nvd.FullId))
                {
                    nvd = nvd.Parent;
                }

                if (nvd != null && _states.ContainsKey(nvd.FullId))
                {
                    PlanningState ps = _states[nvd.FullId];

                    while (ps != null && ps._agents.Count == 0)
                    {
                        ps = ps._parent;
                    }

                    return ps;
                }

                return null;
            }

        }

        private static Dictionary<string, List<PlanningProcess>> _planningDb = new Dictionary<string, List<PlanningProcess>>();
        public static PlanningProcess GetPlanning(string agentFullName, int frame, int index)
        {
            if (_planningDb.ContainsKey(agentFullName))
            {
                PlanningProcess planning = _planningDb[agentFullName].Find((p) => p._frame == frame && p._index == index);

                return planning;
            }

            return null;
        }

        public delegate void AgentInstance_AddPlanningHandler(string agentFullName, int frame, FrameStatePool.PlanningProcess planning);
        public static event AgentInstance_AddPlanningHandler AddPlanningHanlder;
        #endregion


        [Serializable]
        public class FrameState
        {
            [Serializable]
            public class Action
            {
                private string _nodeId;
                public string NodeId
                {
                    get
                    {
                        return _nodeId;
                    }
                    set
                    {
                        _nodeId = value;
                    }
                }

                private string _name;
                public string Name
                {
                    get
                    {
                        return _name;
                    }
                    set
                    {
                        _name = value;
                    }
                }

                private string _result;
                public string Result
                {
                    get
                    {
                        return _result;
                    }
                    set
                    {
                        _result = value;
                    }
                }

                private int _hitCount = 0;
                public int HitCount
                {
                    get
                    {
                        return _hitCount;
                    }
                    set
                    {
                        _hitCount = value;
                    }
                }
            }

            private int _frame = -1;
            public int Frame
            {
                get
                {
                    return _frame;
                }
                set
                {
                    _frame = value;
                }
            }

            private string _behaviorFilename;
            public string BehaviorFilename
            {
                get
                {
                    return _behaviorFilename;
                }
                set
                {
                    _behaviorFilename = value;
                }
            }

            private List<Action> _actions = new List<Action>();
            public List<Action> Actions
            {
                get
                {
                    return _actions;
                }
            }

            private List<string> _highlightNodeIds = new List<string>();
            public List<string> HighlightNodeIds
            {
                get
                {
                    return _highlightNodeIds;
                }
            }

            private List<string> _updatedNodeIds = new List<string>();
            public List<string> UpdatedNodeIds
            {
                get
                {
                    return _updatedNodeIds;
                }
            }

            private List<string> _transitionIds = new List<string>();
            public List<string> TransitionIds
            {
                get
                {
                    return _transitionIds;
                }
            }
        }

        // key : agentName
        private static Dictionary<string, List<FrameState>> _frameStates = new Dictionary<string, List<FrameState>>();
        public static Dictionary<string, List<FrameState>> FrameStates
        {
            get
            {
                return _frameStates;
            }
        }

        [Serializable]
        public class NodeProfileInfos
        {
            [Serializable]
            public class ProfileInfo
            {
                private int _count = 0;
                public int Count
                {
                    get
                    {
                        return _count;
                    }
                    set
                    {
                        _count = value;
                    }
                }

                private float _time = -1;
                public float Time
                {
                    get
                    {
                        return _time;
                    }
                    set
                    {
                        _time = value;
                    }
                }

                private float _totalTime = 0;
                public float TotalTime
                {
                    get
                    {
                        return _totalTime;
                    }
                    set
                    {
                        _totalTime = value;
                    }
                }

                public ProfileInfo Clone()
                {
                    ProfileInfo clone = new ProfileInfo();
                    clone.Count = this.Count;
                    clone.Time = this.Time;
                    clone.TotalTime = this.TotalTime;

                    return clone;
                }

                public override bool Equals(object obj)
                {
                    if (obj is ProfileInfo)
                    {
                        ProfileInfo other = obj as ProfileInfo;
                        return (Count == other.Count) &&
                               (Time == other.Time) &&
                               (TotalTime == other.TotalTime);
                    }

                    return false;
                }

                public override int GetHashCode()
                {
                    return base.GetHashCode();
                }
            }

            private int _lastValidFrame = -1;
            public int LastValidFrame
            {
                get
                {
                    return _lastValidFrame;
                }
                set
                {
                    _lastValidFrame = value;
                }
            }

            // key : frame
            private Dictionary<int, ProfileInfo> _profileInfos = new Dictionary<int, ProfileInfo>();
            public Dictionary<int, ProfileInfo> ProfileInfos
            {
                get
                {
                    return _profileInfos;
                }
                set
                {
                    _profileInfos = value;
                }
            }
        }

        // key1 : behaviorFilename, key2 : nodeId
        private static Dictionary<string, Dictionary<string, NodeProfileInfos>> _nodeFrameProfiles = new Dictionary<string, Dictionary<string, NodeProfileInfos>>();
        public static Dictionary<string, Dictionary<string, NodeProfileInfos>> NodeFrameProfiles
        {
            get
            {
                return _nodeFrameProfiles;
            }
        }

        public static void SetProfileInfo(int frame, string behaviorFilename, string nodeId, float timeMs)
        {
            //CheckJump(agentFullName, ref behaviorFilename, ref nodeId);

            if (frame < 0 || string.IsNullOrEmpty(behaviorFilename) || string.IsNullOrEmpty(nodeId))
            {
                return;
            }

            if (!_nodeFrameProfiles.ContainsKey(behaviorFilename))
            {
                _nodeFrameProfiles[behaviorFilename] = new Dictionary<string, NodeProfileInfos>();
            }

            Dictionary<string, NodeProfileInfos> nodeProfileInfos = _nodeFrameProfiles[behaviorFilename];

            if (!nodeProfileInfos.ContainsKey(nodeId))
            {
                nodeProfileInfos[nodeId] = new NodeProfileInfos();
            }

            NodeProfileInfos profileInfos = nodeProfileInfos[nodeId];

            if (!profileInfos.ProfileInfos.ContainsKey(frame))
            {
                profileInfos.ProfileInfos[frame] = new NodeProfileInfos.ProfileInfo();
            }

            NodeProfileInfos.ProfileInfo profileInfo = profileInfos.ProfileInfos[frame];
            profileInfo.Time = timeMs;

            if (profileInfos.LastValidFrame < 0)
            {
                profileInfo.Count = 1;
                profileInfo.TotalTime = timeMs;

            }
            else
            {
                Debug.Check(profileInfos.ProfileInfos.ContainsKey(profileInfos.LastValidFrame));
                NodeProfileInfos.ProfileInfo lastProfileInfo = profileInfos.ProfileInfos[profileInfos.LastValidFrame];
                profileInfo.Count = lastProfileInfo.Count + 1;
                profileInfo.TotalTime = lastProfileInfo.TotalTime + timeMs;
            }

            profileInfos.LastValidFrame = frame;
        }

        public static Dictionary<string, NodeProfileInfos.ProfileInfo> GetProfileInfos(int frame, string behaviorFilename)
        {
            Dictionary<string, NodeProfileInfos.ProfileInfo> frameProfileInfos = new Dictionary<string, NodeProfileInfos.ProfileInfo>();

            if (frame < 0 || string.IsNullOrEmpty(behaviorFilename))
            {
                return frameProfileInfos;
            }

            if (_nodeFrameProfiles.ContainsKey(behaviorFilename))
            {
                Dictionary<string, NodeProfileInfos> nodeProfileInfos = _nodeFrameProfiles[behaviorFilename];

                foreach (string nodeId in nodeProfileInfos.Keys)
                {
                    NodeProfileInfos profileInfos = nodeProfileInfos[nodeId];

                    if (profileInfos.ProfileInfos.ContainsKey(frame))
                    {
                        frameProfileInfos[nodeId] = profileInfos.ProfileInfos[frame].Clone();
                    }
                    else if (profileInfos.LastValidFrame > -1)
                    {
                        Debug.Check(profileInfos.ProfileInfos.ContainsKey(profileInfos.LastValidFrame));
                        frameProfileInfos[nodeId] = profileInfos.ProfileInfos[profileInfos.LastValidFrame].Clone();
                        frameProfileInfos[nodeId].Time *= -1;
                    }
                }
            }

            return frameProfileInfos;
        }

        // key : frame
        private static Dictionary<int, Dictionary<string, List<string>>> _appLogs = new Dictionary<int, Dictionary<string, List<string>>>();
        public static List<string> GetAppLog(int frame, string filter)
        {
            if (_appLogs.ContainsKey(frame) && !string.IsNullOrEmpty(filter))
            {
                string filterStr = filter.ToUpper();

                if (filterStr != "ALL")
                {
                    if (_appLogs[frame].ContainsKey(filterStr))
                    {
                        return _appLogs[frame][filter];
                    }

                    return null;

                }
                else
                {
                    List<string> result = new List<string>();

                    foreach (List<string> msgs in _appLogs[frame].Values)
                    {
                        result.AddRange(msgs);
                    }

                    return result;
                }
            }

            return null;
        }

        public static void AddAppLog(int frame, string log)
        {
            //[applog]RED:door opened
            if (!_appLogs.ContainsKey(frame))
            {
                _appLogs[frame] = new Dictionary<string, List<string>>();
            }

            string msg = log.Substring(8);

            int pos = msg.IndexOf(':');

            if (pos != -1)
            {
                string filter = msg.Substring(0, pos);
                //string evtMsg = msg.Substring(pos + 1);

                if (!_appLogs[frame].ContainsKey(filter))
                {
                    _appLogs[frame][filter] = new List<string>();
                }

                _appLogs[frame][filter].Add(log);
            }
        }

        public delegate void AddLogDelegate(int frame, string log);
        public static AddLogDelegate AddLogHandler;

        public delegate void UpdateStackDelegate(string tree, bool bAdd);
        public static UpdateStackDelegate UpdateStack;

        // key : frame
        private static Dictionary<int, List<string>> _logs = new Dictionary<int, List<string>>();
        public static List<string> GetLog(int frame)
        {
            return _logs.ContainsKey(frame) ? _logs[frame] : null;
        }

        public static void AddLog(int frame, string log)
        {
            //[log]warning:*****
            //[log]error:*****
            if (!_logs.ContainsKey(frame))
            {
                _logs[frame] = new List<string>();
            }

            string msg = log.Substring(5);

            int pos = msg.IndexOf(':');

            if (pos != -1)
            {
                _logs[frame].Add(msg);

                //don't update ui when anlalyzing log
                if (Plugin.EditMode != EditModes.Analyze)
                {
                    if (AddLogHandler != null)
                    {
                        AddLogHandler(frame, msg);
                    }
                }
            }
        }

        public static void Clear()
        {
            HighlightBreakPoint.Instance = null;
            _frameStates.Clear();
            _nodeFrameProfiles.Clear();
            _appLogs.Clear();
            _logs.Clear();

            _btStack.Clear();
        }

        private static Dictionary<string, List<string>> _btStack = new Dictionary<string, List<string>>();
        public static void SetJumpInfo(string agentFullName, string jumpTree, bool bCheck)
        {
            List<string> stack = null;

            if (!_btStack.ContainsKey(agentFullName))
            {
                stack = new List<string>();
                _btStack[agentFullName] = stack;

            }
            else
            {
                stack = _btStack[agentFullName];
            }

            //if (jumpTree == "root.xml" && _btStack[agentFullName].Count > 0)
            //{
            //    Debug.Check(true);
            //}
            bool bAdd = false;

            if (!bCheck)
            {
                //for 'jump' message
                bAdd = true;

            }
            else
            {
                //only add if there is no any item. this is when for 'tick' message
                if (stack.Count == 0)
                {
                    bAdd = true;
                }
            }

            if (bAdd)
            {
                //push at the end
                stack.Add(jumpTree);

                //don't update ui when anlalyzing log
                if (Plugin.EditMode != EditModes.Analyze)
                {
                    if (UpdateStack != null)
                    {
                        if (agentFullName == Plugin.DebugAgentInstance)
                        {
                            UpdateStack(jumpTree, true);
                        }
                    }
                }
            }
        }

        public static string GetCurrentBehaviorTree(string agentFullName, string behaviorFilename)
        {
            if (_btStack.ContainsKey(agentFullName))
            {
                List<string> stack = _btStack[agentFullName];

                if (stack.Count > 0)
                {
                    return stack[stack.Count - 1];
                }
            }

            return behaviorFilename;
        }

        public static void SetReturnInfo(string agentFullName, string returnFromTree)
        {
            if (_btStack.ContainsKey(agentFullName))
            {
                List<string> stack = _btStack[agentFullName];

                if (stack.Count > 0 && stack[stack.Count - 1] == returnFromTree)
                {
                    //pop last one
                    stack.RemoveAt(stack.Count - 1);

                    //don't update ui when anlalyzing log
                    if (Plugin.EditMode != EditModes.Analyze)
                    {
                        if (UpdateStack != null)
                        {
                            if (agentFullName == Plugin.DebugAgentInstance)
                            {
                                UpdateStack(returnFromTree, false);
                            }
                        }
                    }
                }

            }
            else
            {
                //in case it is connected after the jumping
                //Debug.Check(false);
            }
        }

        public static string GetTopBehaviorTree(string agentFullName)
        {
            if (_btStack.ContainsKey(agentFullName))
            {
                List<string> stack = _btStack[agentFullName];

                if (stack.Count > 0)
                {
                    string top = stack[stack.Count - 1];

                    return top;
                }
            }

            return null;
        }

        public static void EnterNode(string agentFullName, int frame, string behaviorFilename, string nodeId, string actionResult, int hitCount)
        {
            if (frame < 0 || string.IsNullOrEmpty(agentFullName) || string.IsNullOrEmpty(nodeId))
            {
                return;
            }

            FrameState frameState = setFrameState(agentFullName, frame, behaviorFilename, nodeId, HighlightBreakPoint.kEnter, actionResult, hitCount);

            if (frameState != null)
            {
                frameState.UpdatedNodeIds.Remove(nodeId);

                if (!frameState.HighlightNodeIds.Contains(nodeId))
                {
                    frameState.HighlightNodeIds.Add(nodeId);
                }
            }
        }

        public static void UpdateTransition(string agentFullName, int frame, string behaviorFilename, string nodeId, string transitionId)
        {
            if (frame < 0 || string.IsNullOrEmpty(agentFullName) || string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(transitionId))
            {
                return;
            }

            FrameState frameState = setFrameState(agentFullName, frame, behaviorFilename, nodeId);

            if (frameState != null && !frameState.TransitionIds.Contains(transitionId))
            {
                frameState.TransitionIds.Add(transitionId);
            }
        }

        public static void ExitNode(string agentFullName, int frame, string behaviorFilename, string nodeId, string actionResult, int hitCount)
        {
            if (frame < 0 || string.IsNullOrEmpty(agentFullName) || string.IsNullOrEmpty(nodeId))
            {
                return;
            }

            FrameState frameState = setFrameState(agentFullName, frame, behaviorFilename, nodeId, HighlightBreakPoint.kExit, actionResult, hitCount);

            if (frameState != null)
            {
                frameState.HighlightNodeIds.Remove(nodeId);

                if (!frameState.UpdatedNodeIds.Contains(nodeId))
                {
                    frameState.UpdatedNodeIds.Add(nodeId);
                }
            }
        }

        public static List<string> GetHighlightNodeIds(string agentFullName, int frame, string behaviorFilename)
        {
            if (!_frameStates.ContainsKey(agentFullName))
            {
                return null;
            }

            List<FrameState> frameStates = _frameStates[agentFullName];
            int index = findNearestFrame(frameStates, frame);

            if (index < 0)
            {
                return null;
            }

            FrameState frameState = frameStates[index];

            if (frameState.BehaviorFilename != behaviorFilename)
            {
                return null;
            }

            return frameState.HighlightNodeIds;
        }

        private static FrameState getFrameState(string agentFullName, int frame, string behaviorFilename)
        {
            if (!_frameStates.ContainsKey(agentFullName))
            {
                return null;
            }

            List<FrameState> frameStates = _frameStates[agentFullName];
            int index = findFrame(frameStates, frame);

            if (index < 0)
            {
                return null;
            }

            FrameState frameState = frameStates[index];

            if (frameState.BehaviorFilename != behaviorFilename)
            {
                return null;
            }

            return frameState;
        }

        public static List<string> GetHighlightTransitionIds(string agentFullName, int frame, string behaviorFilename)
        {
            FrameState frameState = getFrameState(agentFullName, frame, behaviorFilename);
            return (frameState != null) ? frameState.TransitionIds : null;
        }

        public static List<string> GetUpdatedNodeIds(string agentFullName, int frame, string behaviorFilename)
        {
            FrameState frameState = getFrameState(agentFullName, frame, behaviorFilename);

            return (frameState != null) ? frameState.UpdatedNodeIds : null;
        }

        public static List<FrameState.Action> GetActions(string agentFullName, int frame, string behaviorFilename)
        {
            FrameState frameState = getFrameState(agentFullName, frame, behaviorFilename);

            return (frameState != null) ? frameState.Actions : null;
        }

        public static string GetBehaviorFilename(string agentFullName, int frame)
        {
            if (!string.IsNullOrEmpty(agentFullName) && frame > -1)
            {
                if (!_frameStates.ContainsKey(agentFullName))
                {
                    return string.Empty;
                }

                List<FrameState> frameStates = _frameStates[agentFullName];
                int index = findNearestFrame(frameStates, frame);

                if (index < 0)
                {
                    return string.Empty;
                }

                return frameStates[index].BehaviorFilename;
            }

            return null;
        }

        #region Planning
        private static PlanningProcess AddPlanning(string agentFullName, int frame, int index)
        {
            List<PlanningProcess> planning = null;
            PlanningProcess p = new PlanningProcess(agentFullName, frame, index);

            if (_planningDb.ContainsKey(agentFullName))
            {
                planning = _planningDb[agentFullName];

            }
            else
            {
                planning = new List<PlanningProcess>();
                _planningDb.Add(agentFullName, planning);
            }

            Debug.Check(planning != null);

            if (planning != null)
            {
                planning.Add(p);
            }

            return p;
        }


        public static void PlanBegin(string agentFullName, string btId, string planIndexStr)
        {
            int frame = AgentDataPool.TotalFrames;
            int planIndex = int.Parse(planIndexStr);
            PlanningProcess._planning = AddPlanning(agentFullName, frame, planIndex);

            string bt = null;
            string nodeClassName = null;
            string nodeId = ParseNodeId(btId, ref bt, ref nodeClassName);

            PlanningProcess._planning._behaviorTree = bt;
            Debug.Check(PlanningProcess._planning._currentState == null);
            SetCurrentState(btId);
            PlanningProcess._planning._rootState = PlanningProcess._planning._currentState;
        }

        private static PlanningState SetCurrentState(string btId)
        {
            string nodeId = ParseNodeId(btId);

            if (PlanningProcess._planning._currentState != null && PlanningProcess._planning._parentNodeIds.Count > 0)
            {
                int n = PlanningProcess._planning._parentNodeIds.Count;

                for (int i = n - 1; i >= 0; --i)
                {
                    nodeId = string.Format("{0}:{1}", PlanningProcess._planning._parentNodeIds[i], nodeId);
                }
            }

            if (PlanningProcess._planning._currentState != null &&
                PlanningProcess._planning._currentState._id == btId && PlanningProcess._planning._currentState._nodeId == nodeId)
            {
                return PlanningProcess._planning._currentState;
            }

            //bool bForEachChild = false;
            if (PlanningProcess._planning._currentState != null && PlanningProcess._planning._currentState._index != -1)
            {
                Debug.Check(PlanningProcess._planning._currentState._count != -1);
                //bForEachChild = true;
            }

            PlanningState parentState = PlanningProcess._planning._currentState;
            PlanningState newState = new PlanningState(btId, nodeId, parentState);

            if (!PlanningProcess._planning._states.ContainsKey(nodeId))
            {
                PlanningProcess._planning._states.Add(nodeId, newState);

            }
            else
            {
                PlanningProcess._planning._states[nodeId] = newState;
            }

            PlanningProcess._planning._currentState = newState;

            return newState;
        }

        private static string ParseNodeId(string btId, ref string bt, ref string nodeClassName)
        {
            string nodeId = null;
            string[] nodes = btId.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

            if (nodes.Length == 2)
            {
                bt = nodes[0];

                string[] actions = nodes[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                if (actions.Length >= 2)
                {
                    nodeClassName = actions[0];
                    nodeId = actions[1];
                }
            }

            return nodeId;
        }

        private static string ParseNodeId(string btId)
        {
            string nodeId = null;
            string[] nodes = btId.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);

            if (nodes.Length == 2)
            {
                string behaviorFilename = nodes[0];

                string[] actions = nodes[1].Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

                if (actions.Length >= 2)
                {
                    nodeId = actions[1];
                }
            }

            return nodeId;
        }

        public static void PlanEnd(string agentFullName, string btId)
        {
            //string bt = null;
            //string nodeClassName = null;
            //string nodeId = ParseNodeId(btId, ref bt, ref nodeClassName);

            if (AddPlanningHanlder != null)
            {
                PlanningProcess p = PlanningProcess._planning;
                int frame = AgentDataPool.TotalFrames;
                AddPlanningHanlder(agentFullName, frame, p);
            }

            PlanningProcess._planning._currentState = null;
            PlanningProcess._planning = null;
        }

        public static void PlanningMethodBegin(string btId)
        {
            SetCurrentState(btId);
        }

        public static void PlanningMethodEnd(string btId, string result)
        {
            //Debug.Check(PlanningProcess._planning._currentState._fullId == btId);
        }

        public static void PlanningNodeBegin(string btId)
        {
            SetCurrentState(btId);
        }

        public static void PlanningNodeEnd(string btId, string result)
        {
            if (PlanningProcess._planning._currentState._id == btId)
            {
                if (result == "success")
                {
                    PlanningProcess._planning._currentState._bOk = true;

                }
                else
                {
                    Debug.Check(PlanningProcess._planning._currentState._bPreFailed ||
                                PlanningProcess._planning._currentState._index == -1 ||
                                PlanningProcess._planning._currentState._bIndexFailed);
                }

                if (PlanningProcess._planning._currentState != null)
                {
                    PlanningProcess._planning._currentState = PlanningProcess._planning._currentState._parent;
                }

            }
            else
            {
                Debug.Check(true);
            }
        }

        public static void PlanningNodePreFailed(string btId)
        {
            if (PlanningProcess._planning._currentState._id == btId)
            {
                PlanningProcess._planning._currentState._bPreFailed = true;

            }
            else
            {
                Debug.Check(true);
            }
        }


        public static void PlanningReferencedEnter(string btId, string bt)
        {
            if (PlanningProcess._planning._currentState._id == btId)
            {
                string nodeId = ParseNodeId(btId);
                PlanningProcess._planning._parentNodeIds.Add(nodeId);

            }
            else
            {
                Debug.Check(true);
            }
        }

        public static void PlanningReferencedExit(string btId, string bt)
        {
            if (PlanningProcess._planning._currentState._id == btId)
            {
                if (PlanningProcess._planning._parentNodeIds.Count > 0)
                {
                    string nodeId = ParseNodeId(btId);

                    if (PlanningProcess._planning._parentNodeIds[PlanningProcess._planning._parentNodeIds.Count - 1] == nodeId)
                    {
                        PlanningProcess._planning._parentNodeIds.RemoveAt(PlanningProcess._planning._parentNodeIds.Count - 1);
                    }
                }

            }
            else
            {
                Debug.Check(true);
            }
        }

        public static void PlanningForEachBegin(string btId, string indexStr, string countStr)
        {
            string bt = null;
            string nodeClassName = null;
            string nodeId = ParseNodeId(btId, ref bt, ref nodeClassName);

            string nodeId2 = string.Format("{0}+{1}", nodeId, indexStr);
            string fullId = string.Format("{0}->{1}[{2}]", bt, nodeClassName, nodeId2);
            SetCurrentState(fullId);

            PlanningProcess._planning._currentState._index = int.Parse(indexStr);
            PlanningProcess._planning._currentState._count = int.Parse(countStr);
        }

        public static void PlanningForEachEnd(string btId, string indexStr, string countStr, string result)
        {
            string bt = null;
            string nodeClassName = null;
            string nodeId = ParseNodeId(btId, ref bt, ref nodeClassName);

            string nodeId2 = string.Format("{0}+{1}", nodeId, indexStr);
            string fullId = string.Format("{0}->{1}[{2}]", bt, nodeClassName, nodeId2);

            if (result == "success")
            {
                Debug.Check(true);
            }
            else if (result == "failure")
            {
                PlanningProcess._planning._currentState._bIndexFailed = true;
            }
            else
            {
                Debug.Check(false);
            }

            PlanningNodeEnd(fullId, result);
        }


        public static void PlanProperty(string agentFullname, string valueName, string value)
        {
            Debug.Check(PlanningProcess._planning._currentState != null);
            if (PlanningProcess._planning._currentState != null)
            {
                Dictionary<string, object> d = null;

                if (!PlanningProcess._planning._currentState._agents.ContainsKey(agentFullname))
                {
                    d = new Dictionary<string, object>();
                    PlanningProcess._planning._currentState._agents[agentFullname] = d;

                }
                else
                {
                    d = PlanningProcess._planning._currentState._agents[agentFullname];
                }

                if (!d.ContainsKey(valueName))
                {
                    d.Add(valueName, value);

                }
                else
                {
                    d[valueName] = value;
                }
            }
        }

        #endregion

        private static int findFrame(List<FrameState> frameStates, int frame)
        {
            if (frame > -1 && frameStates.Count > 0)
            {
                //for (int i = frameStates.Count - 1; i >= 0; i--)
                //{
                //    if (frameStates[i].Frame == frame)
                //        return i;
                //    else if (frameStates[i].Frame < frame)
                //        return -1;
                //}

                int first = 0;
                int last = frameStates.Count - 1;
                int current = -1;

                while (first <= last)
                {
                    if (frame == frameStates[last].Frame)
                    {
                        return last;
                    }

                    if (frame == frameStates[first].Frame)
                    {
                        return first;
                    }

                    if (frame > frameStates[last].Frame || frame < frameStates[first].Frame)
                    {
                        return -1;
                    }

                    current = (first + last + 1) / 2;

                    if (frame < frameStates[current].Frame)
                    {
                        last = current - 1;
                    }

                    else if (frame > frameStates[current].Frame)
                    {
                        first = current + 1;
                    }

                    else
                    {
                        return current;
                    }
                }
            }

            return -1;
        }

        private static int findNearestFrame(List<FrameState> frameStates, int frame)
        {
            if (frame > -1 && frameStates.Count > 0)
            {
                //for (int i = frameStates.Count - 1; i >= 0; i--)
                //{
                //    if (frame >= frameStates[i].Frame)
                //        return i;
                //}

                int first = 0;
                int last = frameStates.Count - 1;
                int current = -1;

                while (first <= last)
                {
                    if (frame >= frameStates[last].Frame)
                    {
                        return last;
                    }

                    if (frame == frameStates[first].Frame)
                    {
                        return first;
                    }

                    if (frame < frameStates[first].Frame)
                    {
                        return -1;
                    }

                    current = (first + last + 1) / 2;

                    if (frame < frameStates[current].Frame)
                    {
                        last = current - 1;
                    }

                    else if (frame > frameStates[current].Frame)
                    {
                        first = current + 1;
                    }

                    else
                    {
                        return current;
                    }
                }
            }

            return -1;
        }

        private static FrameState setFrameState(string agentFullName, int frame, string behaviorFilename, string nodeId)
        {
            if (frame < 0 || string.IsNullOrEmpty(agentFullName) || string.IsNullOrEmpty(nodeId))
            {
                return null;
            }

            if (!_frameStates.ContainsKey(agentFullName))
            {
                _frameStates[agentFullName] = new List<FrameState>();
            }

            FrameState frameState = null;
            List<FrameState> frameStates = _frameStates[agentFullName];
            int index = findFrame(frameStates, frame);

            if (index < 0)
            {
                frameState = new FrameState();
                frameState.Frame = frame;
                frameState.BehaviorFilename = behaviorFilename;

                int neareatIndex = findNearestFrame(frameStates, frame);

                if (neareatIndex >= 0)
                {
                    foreach (string id in frameStates[neareatIndex].HighlightNodeIds)
                    {
                        frameState.HighlightNodeIds.Add(id);
                    }
                }

                frameStates.Add(frameState);

            }
            else
            {
                frameState = frameStates[index];

                if (frameState.BehaviorFilename != behaviorFilename)
                {
                    frameState.BehaviorFilename = behaviorFilename;
                    frameState.HighlightNodeIds.Clear();
                }
            }

            return frameState;
        }

        private static FrameState setFrameState(string agentFullName, int frame, string behaviorFilename, string nodeId, string actionName, string actionResult, int hitCount)
        {
            FrameState frameState = setFrameState(agentFullName, frame, behaviorFilename, nodeId);

            if (frameState != null)
            {
                FrameState.Action action = new FrameState.Action();
                action.NodeId = nodeId;
                action.Name = actionName;
                action.Result = actionResult;
                action.HitCount = hitCount;
                frameState.Actions.Add(action);

                if (MessageQueue.IsConnected)
                {
                    RespondToAPPBreak(agentFullName, frame, behaviorFilename, nodeId, actionName, actionResult, hitCount);
                }
            }

            return frameState;
        }

        public static void RespondToAPPBreak(string agentFullName, int frame, string behaviorFilename, string nodeId, string actionName, string actionResult, int hitCount)
        {
            if (behaviorFilename == GetBehaviorFilename(agentFullName, frame))
            {
                behaviorFilename = behaviorFilename.Replace('/', '\\');
                DebugDataPool.BreakPoint breakPoint = DebugDataPool.FindBreakPoint(behaviorFilename, nodeId, actionName);

                if (breakPoint != null && breakPoint.IsActive(actionName, actionResult, hitCount))
                {
                    HighlightBreakPoint.Instance = new HighlightBreakPoint(behaviorFilename, nodeId, breakPoint.NodeType, actionName, actionResult);
                    AgentDataPool.BreakFrame = frame;

                }
                else
                {
                    HighlightBreakPoint.Instance = null;
                    AgentDataPool.BreakFrame = -1;
                }
            }
        }

        public delegate void UpdateAppLogDelegate(int frame, string agentName);
        public static UpdateAppLogDelegate UpdateAppLogHandler;

        public static void RespondToCPPBreak(int frame, string appLog)
        {
            if (Plugin.UpdateMode != UpdateModes.Break)
            {
                //don't update ui when anlalyzing log
                if (Plugin.EditMode != EditModes.Analyze)
                {
                    UpdateAppLogHandler(frame, appLog);
                }
            }
        }

        public static void Serialize(Stream stream, BinaryFormatter formatter)
        {
            formatter.Serialize(stream, _frameStates);
            formatter.Serialize(stream, _nodeFrameProfiles);
            formatter.Serialize(stream, _appLogs);
        }

        public static void Deserialize(Stream stream, BinaryFormatter formatter)
        {
            Clear();

            _frameStates = formatter.Deserialize(stream) as Dictionary<string, List<FrameState>>;
            _nodeFrameProfiles = formatter.Deserialize(stream) as Dictionary<string, Dictionary<string, NodeProfileInfos>>;
            _appLogs = formatter.Deserialize(stream) as Dictionary<int, Dictionary<string, List<string>>>;
        }
    }
}
