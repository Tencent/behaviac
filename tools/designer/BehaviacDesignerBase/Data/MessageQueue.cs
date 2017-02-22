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
using System.Text;
using System.Timers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Behaviac.Design.Data
{
    public class MessageQueue
    {
        private static Object _lockObject = new Object();
        private static List<string> _messages = new List<string>();
        private static Dictionary<int, int> _messsageFrameStartIndex = new Dictionary<int, int>();
        private static int _currentIndex = 0;
        private static int _savingCount = 0;

        private static Concurrent.SPSCQueue<string> _messagesBuffer = new Concurrent.SPSCQueue<string>();

        public delegate UpdateModes ProcessMessageDelegate(string message);
        public static event ProcessMessageDelegate ProcessMessageHandler;

        public delegate void ContinueDelegate(string btMsg);
        public static ContinueDelegate ContinueHandler;

        private static bool _limitMessageCount = false;
        public static bool LimitMessageCount
        {
            get
            {
                return _limitMessageCount;
            }
            set
            {
                _limitMessageCount = value;
            }
        }

        private static int _maxSavingFileSize = 1024 * 1024 * 1024; // bytes

        private static int _maxMessageCount = 10000;
        public static int MaxMessageCount
        {
            get
            {
                return _maxMessageCount;
            }
            set
            {
                _maxMessageCount = value;
            }
        }

        private static bool _connected = false;
        public static bool IsConnected
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
            }
        }

        public static void Clear()
        {
            lock (_lockObject)
            {
                _connected = false;
                _messages.Clear();
                _messsageFrameStartIndex.Clear();
                _currentIndex = 0;
                _savingCount = 0;
            }
        }

        public static List<string> Messages
        {
            get
            {
                lock (_lockObject)
                {
                    return _messages;
                }
            }
        }

        public static int MessageStartIndex(int frame)
        {
            lock (_lockObject)
            {
                if (_messsageFrameStartIndex.ContainsKey(frame))
                {
                    return _messsageFrameStartIndex[frame];
                }
            }

            return -1;
        }

        public static int MessageFirstFrame()
        {
            lock (_lockObject)
            {
                foreach (int frame in _messsageFrameStartIndex.Keys)
                {
                    return frame;
                }
            }

            return -1;
        }

        public static int CurrentIndex()
        {
            lock (_lockObject)
            {
                return _currentIndex;
            }
        }

        public static void PostMessage(string msg)
        {
            lock (_lockObject)
            {
                _messages.Add(msg);
            }
        }

        public static void PostMessageBuffer(string msg)
        {
            //lock (_lockObject)
            //{
            //    _messages.Add(msg);
            //}
            _messagesBuffer.Enqueue(msg);
        }

        const long kTimeThreshold = 100;
        private static System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
        public static void Tick()
        {
            if (Plugin.EditMode != EditModes.Design && Plugin.UpdateMode != UpdateModes.Break)
            {
                string msgN = null;

                while (_messagesBuffer.Dequeue(out msgN))
                {
                    lock (_lockObject)
                    {
                        _messages.Add(msgN);
                    }
                }

                lock (_lockObject)
                {
                    if (_currentIndex < _messages.Count)
                    {
                        if (_currentIndex < _messages.Count)
                        {
                            _stopwatch.Reset();
                            _stopwatch.Start();

                            while (_currentIndex < _messages.Count)
                            {
                                string msg = _messages[_currentIndex];
                                _currentIndex++;

                                if (msg.IndexOf("[frame]") == 10)
                                {
                                    int frame = (int.Parse(msg.Substring(17)));
                                    _messsageFrameStartIndex[frame] = _messages.Count - 1;
                                }

                                Debug.Check(ProcessMessageHandler != null);
                                if (ProcessMessageHandler != null)
                                {
                                    ProcessMessageHandler(msg);
                                }

                                long ms = _stopwatch.ElapsedMilliseconds;

                                if (ms >= kTimeThreshold)
                                {
                                    break;
                                }
                            }

                            if (Plugin.EditMode == EditModes.Connect && _limitMessageCount)
                            {
                                if (_messages.Count > 2 * _maxMessageCount)
                                {
                                    int count = _messages.Count - _maxMessageCount;

                                    if (count > _currentIndex)
                                    {
                                        count = _currentIndex;
                                    }

                                    AppendLog(Workspace.Current.FileName, count);

                                    _savingCount += count;
                                    _currentIndex -= count;

                                    // update messages
                                    _messages.RemoveRange(0, count);

                                    // update message indexes
                                    updateMessageIndexes();
                                }
                            }

                            _stopwatch.Stop();
                        }
                    }
                }
            }
        }


        public static void UpdateMessages()
        {
            lock (_lockObject)
            {
                while (_currentIndex < _messages.Count)
                {
                    string msg = _messages[_currentIndex];

                    if (msg.IndexOf("[frame]") == 0)
                    {
                        int frame = (int.Parse(msg.Substring(7)));
                        _messsageFrameStartIndex[frame] = _currentIndex;
                    }

                    ProcessMessageHandler(msg);
                    _currentIndex++;
                }
            }
        }


        private static void updateMessageIndexes()
        {
            try
            {
                _messsageFrameStartIndex.Clear();

                for (int i = 0; i < _messages.Count; ++i)
                {
                    if (_messages[i].StartsWith("[frame]"))
                    {
                        int frame = (int.Parse(_messages[i].Substring(7)));
                        _messsageFrameStartIndex[frame] = i;
                    }
                }

            }
            catch
            {
            }
        }

        public static void Serialize(Stream stream, BinaryFormatter formatter)
        {
            lock (_lockObject)
            {
                formatter.Serialize(stream, _messages);
            }
        }

        public static void Deserialize(Stream stream, BinaryFormatter formatter)
        {
            try
            {
                Clear();

                List<string> messages = LoadLog(Workspace.Current.FileName);

                lock (_lockObject)
                {
                    _messages.AddRange(messages);

                    messages = formatter.Deserialize(stream) as List<string>;

                    if (messages != null)
                    {
                        _messages.AddRange(messages);
                    }

                    _currentIndex = _messages.Count;

                    updateMessageIndexes();
                }
            }
            catch
            {
            }
        }

        private static string getLogFile(string workspaceFile)
        {
            workspaceFile = Path.GetFullPath(workspaceFile);
            uint h = (uint)workspaceFile.ToLowerInvariant().GetHashCode();
            string filename = Path.GetFileName(workspaceFile).Replace('.', '_');
            string logFile = string.Format("behaviac_{0}_{1}.log", filename, h.ToString());

            return Path.Combine(Utilities.GetLogFileDirectory(), logFile);
        }

        public static bool AppendLog(string workspaceFile, int count)
        {
            try
            {
                string filename = getLogFile(workspaceFile);

                if (File.Exists(filename))
                {
                    FileInfo info = new FileInfo(filename);

                    if (info.Length > _maxSavingFileSize)   // bytes
                    {
                        return false;
                    }
                }

                Stream stream = File.Open(filename, FileMode.Append);
                BinaryWriter writer = new BinaryWriter(stream);

                for (int i = 0; i < count; ++i)
                {
                    writer.Write(_messages[i]);
                }

                stream.Close();

                return true;

            }
            catch
            {
            }

            return false;
        }

        private static List<string> LoadLog(string workspaceFile)
        {
            List<string> messages = new List<string>();

            try
            {
                string filename = getLogFile(workspaceFile);

                if (File.Exists(filename))
                {
                    Stream stream = File.Open(filename, FileMode.Open);
                    BinaryReader reader = new BinaryReader(stream);

                    try
                    {
                        while (true)
                        {
                            string msg = reader.ReadString();
                            messages.Add(msg);
                        }

                    }
                    catch
                    {
                    }

                    stream.Close();
                }

            }
            catch
            {
            }

            return messages;
        }
    }
}
