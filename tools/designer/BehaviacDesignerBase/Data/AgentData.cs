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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Behaviac.Design.Properties;
using Behaviac.Design.Nodes;

namespace Behaviac.Design.Data
{
    public class AgentInstancePool
    {
        public delegate void AddInstanceDelegate(string agentType, string agentName);
        public static event AddInstanceDelegate AddInstanceHandler;

        private static Dictionary<string, List<string>> _agentInstances = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> Instances
        {
            get
            {
                return _agentInstances;
            }
        }

        public static void Clear()
        {
            _agentInstances.Clear();
        }

        public static bool AddInstance(string agentType, string agentName)
        {
            if (!_agentInstances.ContainsKey(agentType))
            {
                _agentInstances[agentType] = new List<string>();
            }

            List<string> instances = _agentInstances[agentType];
            bool bAdded = false;

            if (!instances.Contains(agentName))
            {
                instances.Add(agentName);

                bAdded = true;
            }

            if (AddInstanceHandler != null)
            {
                AddInstanceHandler(agentType, agentName);
            }

            return bAdded;
        }

        public static List<string> GetInstances(string agentType)
        {
            List<string> instances = new List<string>();

            if (_agentInstances.ContainsKey(agentType))
            {
                instances = _agentInstances[agentType];
            }

            return instances;
        }

        public static void Serialize(Stream stream, BinaryFormatter formatter)
        {
            formatter.Serialize(stream, _agentInstances);
        }

        public static void Deserialize(Stream stream, BinaryFormatter formatter)
        {
            Clear();
            _agentInstances = formatter.Deserialize(stream) as Dictionary<string, List<string>>;

            if (_agentInstances != null && AddInstanceHandler != null)
            {
                foreach (string agentType in _agentInstances.Keys)
                {
                    foreach (string agentName in _agentInstances[agentType])
                    {
                        AddInstanceHandler(agentType, agentName);
                    }
                }
            }
        }
    }


    public class AgentDataPool
    {
        [Serializable]
        public class ValueMark
        {
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

            private string _value;
            public string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
        }

        [Serializable]
        public class AgentData
        {
            public class ValueComparer : IComparer<ValueMark>
            {
                public int Compare(ValueMark v1, ValueMark v2)
                {
                    return v1.Frame - v2.Frame;
                }
            }

            private Dictionary<string, List<ValueMark>> _valueSet = new Dictionary<string, List<ValueMark>>();
            public Dictionary<string, List<ValueMark>> ValueSet
            {
                get
                {
                    return _valueSet;
                }
            }

            private static ValueComparer _valueComparer = new ValueComparer();

            public void AddValue(string valueName, int frame, string value)
            {
                Debug.Check(!string.IsNullOrEmpty(valueName) && frame > -1 && value != null);

                if (!_valueSet.ContainsKey(valueName))
                {
                    _valueSet[valueName] = new List<ValueMark>();
                }

                List<ValueMark> values = _valueSet[valueName];

                ValueMark valueMark = new ValueMark();
                valueMark.Frame = frame;
                valueMark.Name = valueName;
                valueMark.Value = value;

                if (values.Count == 0 || values[values.Count - 1].Frame < frame)
                {
                    values.Add(valueMark);

                }
                else
                {
                    int index = values.BinarySearch(valueMark, _valueComparer);

                    if (index >= 0)
                    {
                        valueMark = values[index];
                        Debug.Check(valueMark.Frame == frame && valueMark.Name == valueName);
                        valueMark.Value = value;

                    }
                    else
                    {
                        index = -index - 1;
                        values.Insert(index, valueMark);
                    }
                }
            }

            public ValueMark GetValidValue(string valueName, int frame)
            {
                Debug.Check(!string.IsNullOrEmpty(valueName) && frame > -1);

                if (!_valueSet.ContainsKey(valueName))
                {
                    return null;
                }

                List<ValueMark> values = _valueSet[valueName];

                if (values.Count > 0)
                {
                    int previous = 0;
                    int last = values.Count - 1;
                    int current = 0;

                    while (previous < last)
                    {
                        current = (previous + last + 1) / 2;

                        if (frame < values[current].Frame)
                        {
                            last = current - 1;
                        }

                        else if (frame > values[current].Frame)
                        {
                            previous = current;
                        }

                        else
                        {
                            break;
                        }
                    }

                    return values[current];
                }

                return null;
            }
        }

        private static int _currentFrame = -1;
        public static int CurrentFrame
        {
            get
            {
                return _currentFrame;
            }
            set
            {
                _currentFrame = value;
            }
        }

        private static int _totalFrames = -1;
        public static int TotalFrames
        {
            get
            {
                return _totalFrames;
            }
            set
            {
                _totalFrames = value;
            }
        }

        private static int _breakFrame = -1;
        public static int BreakFrame
        {
            get
            {
                return _breakFrame;
            }
            set
            {
                _breakFrame = value;
            }
        }

        // key : agentName
        private static Dictionary<string, AgentData> _agentDatabase = new Dictionary<string, AgentData>();

        public static void Clear()
        {
            _totalFrames = -1;
            _currentFrame = -1;
            _breakFrame = -1;
            _agentDatabase.Clear();
        }

        public static void AddValue(string agentFullname, string valueName, int frame, string value)
        {
            Debug.Check(!string.IsNullOrEmpty(agentFullname) &&
                        !string.IsNullOrEmpty(valueName) &&
                        frame > -1);

            if (!_agentDatabase.ContainsKey(agentFullname))
            {
                _agentDatabase[agentFullname] = new AgentData();
            }

            AgentData agentData = _agentDatabase[agentFullname];
            agentData.AddValue(valueName, frame, value);
        }

        public static ValueMark GetValidValue(string agentFullname, string valueName, int frame)
        {
            Debug.Check(!string.IsNullOrEmpty(agentFullname) &&
                        !string.IsNullOrEmpty(valueName) &&
                        frame > -1);

            if (!_agentDatabase.ContainsKey(agentFullname))
            {
                return null;
            }

            AgentData agentData = _agentDatabase[agentFullname];
            return agentData.GetValidValue(valueName, frame);
        }

        public static List<ValueMark> GetValidValues(AgentType agentType, string agentFullname, int frame)
        {
            Debug.Check(!string.IsNullOrEmpty(agentFullname) && frame > -1);

            List<ValueMark> valueSet = new List<ValueMark>();

            if (_agentDatabase.ContainsKey(agentFullname))
            {
                AgentData agentData = _agentDatabase[agentFullname];

                // Properties
                if (agentType != null)
                {
                    //foreach (PropertyDef p in agentType.GetProperties())
                    //{
                    //    ValueMark value = agentData.GetValidValue(p.BasicName, frame);

                    //    if (value != null)
                    //        valueSet.Add(value);
                    //}

                    foreach (string valueName in agentData.ValueSet.Keys)
                    {
                        ValueMark value = agentData.GetValidValue(valueName, frame);

                        if (value != null)
                        {
                            valueSet.Add(value);
                        }
                    }
                }
            }

            return valueSet;
        }

        public static void Serialize(Stream stream, BinaryFormatter formatter)
        {
            formatter.Serialize(stream, _agentDatabase);
        }

        public static void Deserialize(Stream stream, BinaryFormatter formatter)
        {
            Clear();
            _agentDatabase = formatter.Deserialize(stream) as Dictionary<string, AgentData>;
        }

        public static bool Save(string filename, string currentWorkspace)
        {
            try
            {
                Stream stream = File.Open(filename, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();

                // Save the workspace.
                formatter.Serialize(stream, currentWorkspace);

                // Save all agent instances.
                AgentInstancePool.Serialize(stream, formatter);

                // Save all parameters.
                Serialize(stream, formatter);
                formatter.Serialize(stream, TotalFrames);

                // Save all frame states.
                FrameStatePool.Serialize(stream, formatter);

                // Save all messages.
                MessageQueue.Serialize(stream, formatter);

                stream.Close();

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool LoadLog(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader file = new StreamReader(fs))
                    {
                        string msg;

                        MessageQueue.Clear();

                        while ((msg = file.ReadLine()) != null)
                        {
                            // skip the time stamp
                            //string m = msg.Substring(21);
                            int pos = msg.IndexOf("][");
                            string m = msg.Substring(pos + 1);

                            MessageQueue.PostMessage(m);
                        }

                        file.Close();
                    }
                }

                MessageQueue.UpdateMessages();
                //TotalFrames = ;
                CurrentFrame = 0;

                return true;

            }
            catch (Exception ex)
            {
                string msgError = string.Format(Resources.LoadDebugDataError, filename, ex.Message);
                MessageBox.Show(msgError, Resources.LoadError, MessageBoxButtons.OK);
            }

            return false;
        }

        public static bool LoadDump(string filename, string currentWorkspace)
        {
            try
            {
                Stream stream = File.Open(filename, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();

                // Load the workspace.
                currentWorkspace = formatter.Deserialize(stream) as string;

                if (Plugin.WorkspaceDelegateHandler != null)
                {
                    Plugin.WorkspaceDelegateHandler(currentWorkspace, false);
                }

                // Load all agent instances.
                AgentInstancePool.Deserialize(stream, formatter);

                // Load all parameters.
                Deserialize(stream, formatter);
                TotalFrames = (int)formatter.Deserialize(stream);
                CurrentFrame = 0;

                // Load all frame states.
                FrameStatePool.Deserialize(stream, formatter);

                // Load all messages.
                MessageQueue.Deserialize(stream, formatter);

                stream.Close();

                return true;

            }
            catch (Exception ex)
            {
                string msgError = string.Format(Resources.LoadDebugDataError, filename, ex.Message);
                MessageBox.Show(msgError, Resources.LoadError, MessageBoxButtons.OK);
            }

            return false;
        }
    }
}
