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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Behaviac.Design.Data;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Network
{
    public class NetworkManager
    {
        private static NetworkManager _instance = null;
        public static NetworkManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NetworkManager();
                }

                return _instance;
            }
        }

        private static bool _useLocalIP = true;
        public static bool UseLocalIP
        {
            get
            {
                return _useLocalIP;
            }
            set
            {
                _useLocalIP = value;
            }
        }

        private static string _serverIP = "";
        public static string ServerIP
        {
            get
            {
                return _serverIP;
            }
            set
            {
                _serverIP = value;
            }
        }

        private static int _serverPort = 60636;
        public static int ServerPort
        {
            get
            {
                return _serverPort;
            }
            set
            {
                _serverPort = value;
            }
        }

        public static void Load(Stream stream, BinaryFormatter formatter)
        {
            try
            {
                object obj = formatter.Deserialize(stream);

                if (obj is bool)
                {
                    UseLocalIP = (bool)obj;
                }

                obj = formatter.Deserialize(stream);

                if (obj is string)
                {
                    ServerIP = obj as string;
                }

                obj = formatter.Deserialize(stream);

                if (obj is int)
                {
                    ServerPort = (int)obj;
                }
            }
            catch
            {
            }
        }

        public static void Save(Stream stream, BinaryFormatter formatter)
        {
            try
            {
                formatter.Serialize(stream, UseLocalIP);
                formatter.Serialize(stream, ServerIP);
                formatter.Serialize(stream, ServerPort);
            }
            catch
            {
            }
        }

        //text + \0 + cmd
        private const int kMaxTextLength = 228 + 1 + 1;
        private const int BUFFER_SIZE = 16384 * 10;

        private uint m_packetsReceived = 0;
        private MsgReceiver m_msgReceiver = new MsgReceiver();
        private Socket m_clientSocket = null;
        private AsyncCallback m_pfnCallBack = null;

        class SocketPacket
        {
            public enum CommandID
            {
                INITIAL_SETTINGS = 1,
                TEXT,
                MAX
            };

            public byte[] dataBuffer;
        }

        private NetworkManager()
        {
            DebugDataPool.AddBreakPointHandler += new DebugDataPool.AddBreakPointDelegate(addBreakPoint);
            DebugDataPool.RemoveBreakPointHandler += new DebugDataPool.RemoveBreakPointDelegate(removeBreakPoint);
        }

        public bool IsConnected()
        {
            return m_clientSocket != null && m_clientSocket.Connected;
        }

        public bool Connect(string strIP, int iPort)
        {
            if (m_clientSocket == null)
            {
                try
                {
                    // Create the socket instance
                    m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    m_clientSocket.ReceiveBufferSize = BUFFER_SIZE;
                    m_clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
                    //m_clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    //m_clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);

                    IPAddress ip = IPAddress.Parse(strIP);
                    IPEndPoint ipEnd = new IPEndPoint(ip, iPort);
                    m_clientSocket.Connect(ipEnd);

                    if (m_clientSocket.Connected)
                    {
                        onConnect();
                        return waitForData();
                    }

                }
                catch (SocketException se)
                {
                    MessageBox.Show(se.Message, Resources.ConnectError);

                    m_clientSocket = null;
                }
            }

            return false;
        }

        private void onConnect()
        {
            this.m_packetsReceived = 0;
        }

        public void Disconnect()
        {
            string msg = string.Format("[closeconnection]\n");

            this.SendText(msg);

            System.Threading.Thread.Sleep(200);

            closeSocket();
        }

        private void closeSocket()
        {
            try
            {
                if (m_clientSocket != null)
                {
                    m_clientSocket.Shutdown(SocketShutdown.Both);
                    m_clientSocket.Close();
                }

            }
            catch (SocketException)
            {
            }

            m_clientSocket = null;
        }

        SocketPacket _theSocPkt = null;
        private bool waitForData()
        {
            try
            {
                if (m_clientSocket == null || !m_clientSocket.Connected)
                {
                    return false;
                }

                if (m_pfnCallBack == null)
                {
                    m_pfnCallBack = new AsyncCallback(onDataReceived);
                }

                if (_theSocPkt == null)
                {
                    _theSocPkt = new SocketPacket();
                    _theSocPkt.dataBuffer = new byte[BUFFER_SIZE];
                }

                if (m_clientSocket != null)
                {
                    // Start listening to the data asynchronously
                    m_clientSocket.BeginReceive(_theSocPkt.dataBuffer,
                                                0, _theSocPkt.dataBuffer.Length,
                                                SocketFlags.None,
                                                m_pfnCallBack,
                                                _theSocPkt);

                    return true;
                }

            }
            catch (SocketException se)
            {
                //MessageBox.Show(se.Message, Resources.ConnectError);
                Console.WriteLine(Resources.ConnectError + " : " + se.Message);
            }

            return false;
        }

        private void onDataReceived(IAsyncResult asyn)
        {
            try
            {
                if (m_clientSocket == null || !m_clientSocket.Connected)
                {
                    return;
                }

                SocketPacket packet = (SocketPacket)asyn.AsyncState;
                int receivedBytes = m_clientSocket.EndReceive(asyn);

                List<byte[]> messages = m_msgReceiver.OnDataReceived(packet.dataBuffer, receivedBytes);
                m_packetsReceived += (uint)messages.Count;

                for (int i = 0; i < messages.Count; ++i)
                {
                    handleMessage(messages[i]);
                }

                if (m_clientSocket != null && m_clientSocket.Connected)
                {
                    waitForData();
                }

            }
            catch (NullReferenceException)
            {
                // Socket closed (most probably)
            }
            catch (ObjectDisposedException)
            {
                // Socket closed
            }
            catch (SocketException exc)
            {
                //MessageBox.Show(exc.Message, Resources.ConnectError);
                Console.WriteLine(Resources.ConnectError + " : " + exc.Message);
                //Invoke(m_delegateOnDisconnect);

            }
            catch (Exception)
            {
                Debug.Check(true);
            }
        }

        public void SendText(string msg)
        {
            if (m_clientSocket != null && m_clientSocket.Connected)
            {
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(msg);
                m_clientSocket.Send(bytes);
            }
        }

        public uint PacketsReceived
        {
            get
            {
                return this.m_packetsReceived;
            }
        }

        static private uint GetInt(byte[] data, int i)
        {
            return (uint)((data[i + 3] << 24) + (data[i + 2] << 16) + (data[i + 1] << 8) + (data[i + 0]));
        }

        private static string GetStringFromBuffer(byte[] data, int dataIdx, int maxLen, bool isAsc)
        {
            Encoding ecode;

            if (isAsc)
            {
                ecode = new ASCIIEncoding();

            }
            else
            {
                ecode = new UTF8Encoding();
            }

            Debug.Check(data.Length <= maxLen);

            if (data.Length <= maxLen)
            {
                maxLen = data.Length - 1;

                string ret = ecode.GetString(data, dataIdx, maxLen);
                char[] zeroChars = { '\0', '?' };

                return ret.TrimEnd(zeroChars);
            }

            return "[Error]The length of the message is above the Max value!";
        }

        int[] m_packets = new int[(int)SocketPacket.CommandID.MAX];

        private void handleMessage(byte[] msgData)
        {
            try
            {
                if (msgData.Length > 0)
                {
                    SocketPacket.CommandID commandId = (SocketPacket.CommandID)(msgData[0]);

                    m_packets[(int)commandId]++;

                    switch (commandId)
                    {
                        case SocketPacket.CommandID.INITIAL_SETTINGS:
                        {
                            int platform = (int)msgData[1];
                            int processId = (int)GetInt(msgData, 2);
                            break;
                        }

                        case SocketPacket.CommandID.TEXT:
                        {
                            handleText(msgData);
                            break;
                        }

                        default:
                        {
                            System.Diagnostics.Debug.Fail("Unknown command ID: " + commandId);
                            break;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(Resources.ConnectError + " : " + e.Message);
                //MessageBox.Show(e.Message, Resources.ConnectError);
            }
        }

        //int _lastIndex = -1;

        private void handleText(byte[] msgData)
        {
            string text = GetStringFromBuffer(msgData, 1, kMaxTextLength, true);
            MessageQueue.PostMessageBuffer(text);
        }


        public void SendBreakpoint(string behaviorName, string nodeClass, string nodeId, string action, bool bSet, int hit, string actionResultStr)
        {
            string setStr = bSet ? "add" : "remove";

            Debug.Check(!string.IsNullOrEmpty(actionResultStr));
            behaviorName = behaviorName.Replace('\\', '/');
            string msg = string.Format("[breakpoint] {0} {1}->{2}[{3}]:{4} {5} Hit={6}\n", setStr, behaviorName, nodeClass, nodeId, action, actionResultStr, hit);

            this.SendText(msg);
        }

        public void SendLoadedBreakpoints()
        {
            foreach (string kbt in DebugDataPool.BreakPoints.Keys)
            {
                foreach (KeyValuePair<string, DebugDataPool.BreakPoint> p in DebugDataPool.BreakPoints[kbt])
                {
                    foreach (DebugDataPool.Action a in p.Value.Actions)
                    {
                        if (a.Enable)
                        {
                            SendBreakpoint(kbt, p.Value.NodeType, p.Key, a.Name, true, a.HitCount, a.Result);
                        }
                    }
                }
            }
        }

        public void SendBreakAPP(bool bBreakAPP)
        {
            string breakStr = bBreakAPP ? "true" : "false";
            string msg = string.Format("[breakapp] {0}\n", breakStr);

            this.SendText(msg);
        }

        public void SendProfiling(bool bProfiling)
        {
            string profilingStr = bProfiling ? "true" : "false";
            string msg = string.Format("[profiling] {0}\n", profilingStr);

            this.SendText(msg);
        }

        public void SendProperty(string agentFullName, string valueType, string valueName, string value)
        {
            //[property] WorldState::WorldState WorldState::time->185606213
            //[property] Ship::Ship_2_3 GameObject::age->91291
            //[property] Ship::Ship_2_3 bool par_a->true
            string msg = string.Format("[property] {0} {1} {2}->{3}\n", agentFullName, valueType, valueName, value);

            this.SendText(msg);
        }

        public void SendLogFilter(string filter)
        {
            string msg = string.Format("[applogfilter] {0}\n", filter.ToUpper());

            this.SendText(msg);
        }

        public void SendContinue()
        {
            string msg = string.Format("[continue]\n");

            this.SendText(msg);
        }

        private void addBreakPoint(string behaviorFilename, string nodeType, string nodeId, DebugDataPool.Action action)
        {
            if (action.Enable)
            {
                SendBreakpoint(behaviorFilename, nodeType, nodeId, action.Name, true, action.HitCount, action.Result);
            }

            else
            {
                removeBreakPoint(behaviorFilename, nodeType, nodeId, action);
            }
        }

        private void removeBreakPoint(string behaviorFilename, string nodeType, string nodeId, DebugDataPool.Action action)
        {
            SendBreakpoint(behaviorFilename, nodeType, nodeId, action.Name, false, 0, "all");
        }
    }
}
