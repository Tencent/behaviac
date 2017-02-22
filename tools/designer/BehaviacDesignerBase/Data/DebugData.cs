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
using System.Windows.Forms;
using Behaviac.Design.Properties;
using Behaviac.Design.Network;

namespace Behaviac.Design.Data
{
    public class HighlightBreakPoint
    {
        public const string kEnter = "enter";
        public const string kExit = "exit";
        public const string kPlanning = "plan";

        private static HighlightBreakPoint _instance = null;
        public static HighlightBreakPoint Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private static bool _showBreakPoint = true;
        public static bool ShowBreakPoint
        {
            get
            {
                return _showBreakPoint;
            }
            set
            {
                _showBreakPoint = value;
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

        private string _nodeType;
        public string NodeType
        {
            get
            {
                return _nodeType;
            }
            set
            {
                _nodeType = value;
            }
        }

        private string _actionName;
        public string ActionName
        {
            get
            {
                return _actionName;
            }
            set
            {
                _actionName = value;
            }
        }

        private string _actionResult;
        public string ActionResult
        {
            get
            {
                return _actionResult;
            }
            set
            {
                _actionResult = value;
            }
        }

        public HighlightBreakPoint(string behaviorFilename, string nodeId, string nodeType, string actionName, string actionResult)
        {
            _behaviorFilename = behaviorFilename;
            _nodeId = nodeId;
            _nodeType = nodeType;
            _actionName = actionName;
            _actionResult = actionResult;
        }
    }


    public class DebugDataPool
    {
        [Serializable]
        public class Action
        {
            public static string kResultAll = "all";
            public static string kResultSuccess = "success";
            public static string kResultFailure = "failure";

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

            private bool _enable = true;
            public bool Enable
            {
                get
                {
                    return _enable;
                }
                set
                {
                    _enable = value;
                }
            }

            private string _result = kResultAll;
            public string Result
            {
                get
                {
                    return isValidResult(_result) ? _result : kResultAll;
                }
                set
                {
                    _result = isValidResult(value) ? value : kResultAll;
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

            private bool isValidResult(string result)
            {
                return result == kResultAll || result == kResultSuccess || result == kResultFailure;
            }
        }

        [Serializable]
        public class BreakPoint
        {
            private string _nodeType;
            public string NodeType
            {
                get
                {
                    return _nodeType;
                }
                set
                {
                    _nodeType = value;
                }
            }

            private List<Action> _actions = new List<Action>();
            public List<Action> Actions
            {
                get
                {
                    return _actions;
                }
                set
                {
                    _actions = value;
                }
            }

            public Action FindAction(string actionName)
            {
                foreach (Action action in _actions)
                {
                    if (action.Name == actionName)
                    {
                        return action;
                    }
                }

                return null;
            }

            public bool RemoveAction(string actionName)
            {
                foreach (Action action in _actions)
                {
                    if (action.Name == actionName)
                    {
                        return _actions.Remove(action);
                    }
                }

                return false;
            }

            public bool IsEnable(string actionName)
            {
                Action action = FindAction(actionName);
                return (action != null) ? action.Enable : false;
            }

            public bool IsActive(string actionName, string actionResult, int hitCount = -1)
            {
                Action action = FindAction(actionName);
                return action != null && action.Enable &&
                       (action.Result == Action.kResultAll || action.Result == actionResult) &&
                       (hitCount < 0 || action.HitCount == 0 || action.HitCount == hitCount);
            }
        }

        // <behaviorFilename, <nodeId, breakPoint>>
        private static Dictionary<string, Dictionary<string, BreakPoint>> _breakPoints = new Dictionary<string, Dictionary<string, BreakPoint>>();
        public static Dictionary<string, Dictionary<string, BreakPoint>> BreakPoints
        {
            get
            {
                return _breakPoints;
            }
        }

        public static BreakPoint FindBreakPoint(string behaviorFilename, string nodeId, string actionName)
        {
            behaviorFilename = behaviorFilename.Replace('\\', '/');

            if (!_breakPoints.ContainsKey(behaviorFilename))
            {
                return null;
            }

            Dictionary<string, BreakPoint> breakPoints = _breakPoints[behaviorFilename];

            if (!breakPoints.ContainsKey(nodeId))
            {
                return null;
            }

            BreakPoint breakPoint = breakPoints[nodeId];
            Action action = breakPoint.FindAction(actionName);

            return (action != null) ? breakPoint : null;
        }

        public delegate void LoadBreakPointsDelegate();
        public static LoadBreakPointsDelegate LoadBreakPointsHandler;

        public delegate void AddBreakPointDelegate(string behaviorFilename, string nodeType, string nodeId, Action action);
        public static AddBreakPointDelegate AddBreakPointHandler;

        public delegate void RemoveBreakPointDelegate(string behaviorFilename, string nodeType, string nodeId, Action action);
        public static RemoveBreakPointDelegate RemoveBreakPointHandler;

        public static void AddBreakPoint(string behaviorFilename, string nodeId, string nodeType, string actionName, bool actionEnable, string actionResult, int hitCount)
        {
            if (string.IsNullOrEmpty(behaviorFilename) || string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(nodeType) || string.IsNullOrEmpty(actionName) || hitCount < 0)
            {
                return;
            }

            behaviorFilename = behaviorFilename.Replace('\\', '/');

            if (!_breakPoints.ContainsKey(behaviorFilename))
            {
                _breakPoints[behaviorFilename] = new Dictionary<string, BreakPoint>();
            }

            Dictionary<string, BreakPoint> breakPoints = _breakPoints[behaviorFilename];

            if (!breakPoints.ContainsKey(nodeId))
            {
                breakPoints[nodeId] = new BreakPoint();
            }

            BreakPoint breakPoint = breakPoints[nodeId];
            breakPoint.NodeType = nodeType;

            Action action = breakPoint.FindAction(actionName);

            if (action == null)
            {
                action = new Action();
                breakPoint.Actions.Add(action);
            }

            action.Name = actionName;
            action.Enable = actionEnable;
            action.Result = actionResult;
            action.HitCount = hitCount;

            if (AddBreakPointHandler != null)
            {
                AddBreakPointHandler(behaviorFilename, nodeType, nodeId, action);
            }
        }

        public static void RemoveBreakPoint(string behaviorFilename, string nodeId, Action action)
        {
            if (string.IsNullOrEmpty(behaviorFilename) || string.IsNullOrEmpty(nodeId) || action == null)
            {
                return;
            }

            behaviorFilename = behaviorFilename.Replace('\\', '/');

            if (!_breakPoints.ContainsKey(behaviorFilename))
            {
                return;
            }

            Dictionary<string, BreakPoint> breakPoints = _breakPoints[behaviorFilename];

            if (!breakPoints.ContainsKey(nodeId))
            {
                return;
            }

            BreakPoint breakPoint = breakPoints[nodeId];
            breakPoint.RemoveAction(action.Name);

            if (breakPoint.Actions.Count == 0)
            {
                breakPoints.Remove(nodeId);

                if (breakPoints.Count == 0)
                {
                    _breakPoints.Remove(behaviorFilename);
                }
            }

            if (RemoveBreakPointHandler != null)
            {
                RemoveBreakPointHandler(behaviorFilename, breakPoint.NodeType, nodeId, action);
            }
        }

        private static string getDebugFile(string workspaceFile)
        {
            workspaceFile = Path.GetFullPath(workspaceFile);
            uint h = (uint)workspaceFile.ToLowerInvariant().GetHashCode();
            string filename = Path.GetFileName(workspaceFile).Replace('.', '_');
            string dbgFile = string.Format("behaviac_{0}_{1}.dbg", filename, h.ToString());

            // Create the debug related folders if not existed
            string dbgFileDir = Utilities.GetDebugFileDirectory();

            if (!Directory.Exists(dbgFileDir))
            {
                Directory.CreateDirectory(dbgFileDir);
            }

            return Path.Combine(dbgFileDir, dbgFile);
        }

        public static bool Save(string workspaceFile)
        {
            try
            {
                string filename = getDebugFile(workspaceFile);
                Stream stream = File.Open(filename, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, _breakPoints);

                ExpandedNodePool.Serialize(stream, formatter);

                NetworkManager.Save(stream, formatter);

                stream.Close();

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Load(string workspaceFile)
        {
            _breakPoints.Clear();
            ExpandedNodePool.Clear();

            bool loadSucceeded = false;
            string filename = getDebugFile(workspaceFile);

            if (File.Exists(filename))
            {
                try
                {
                    Stream stream = File.Open(filename, FileMode.Open);
                    BinaryFormatter formatter = new BinaryFormatter();

                    _breakPoints = formatter.Deserialize(stream) as Dictionary<string, Dictionary<string, BreakPoint>>;

                    ExpandedNodePool.Deserialize(stream, formatter);

                    NetworkManager.Load(stream, formatter);

                    stream.Close();

                    loadSucceeded = true;

                }
                catch (Exception)
                {
                    //string msgError = string.Format(Resources.LoadConfigureError, filename, ex.Message);
                    //MessageBox.Show(msgError, Resources.LoadError, MessageBoxButtons.OK);
                }
            }

            if (LoadBreakPointsHandler != null)
            {
                LoadBreakPointsHandler();
            }

            return loadSucceeded;
        }
    }
}
