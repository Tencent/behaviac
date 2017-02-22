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
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;
using System.Management;
using System.Threading;
using System.Net.NetworkInformation;

using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    public class Utilities
    {
        public static void ClearDirectory(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(folderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearDirectory(di.FullName);
                di.Delete();
            }
        }

        public static string GetDebugFileDirectory()
        {
            string dbgFileDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(dbgFileDir, "BehaviacDesigner");
        }

        public static string GetLogFileDirectory()
        {
            return Path.Combine(Path.GetTempPath(), "BehaviacDesigner");
        }

        public static bool IPOnlyNumbersAndDots(String s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }

            for (int i = 0; i < s.Length; ++i)
            {
                if (!char.IsDigit(s[i]) && s[i] != '.')
                {
                    return false;
                }
            }

            return true;
        }

        private static string _runtimePlatform = "";
        public static String RuntimePlatform
        {
            set
            {
                _runtimePlatform = value;
            }

            get
            {
                return _runtimePlatform;
            }
        }

        private static string _localIP = "";
        public static String GetLocalIP()
        {
            if (string.IsNullOrEmpty(_localIP))
            {
                try
                {
                    String strHostName = Dns.GetHostName();
                    _localIP = GetIP(strHostName);
                }
                catch
                {
                }
            }

            return _localIP;
        }

        public static String GetIP(String strHostName)
        {
            String IPStr = "";

            try
            {
                // Find host by name
                IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

                // Grab the first IP addresses
                foreach (IPAddress ipaddress in iphostentry.AddressList)
                {
                    IPStr = ipaddress.ToString();

                    if (IPOnlyNumbersAndDots(IPStr))
                    {
                        return IPStr;
                    }
                }
            }
            catch
            {
            }

            return IPStr;
        }

        private static string _localMac = "";
        public static string GetLocalMac()
        {
            if (string.IsNullOrEmpty(_localMac))
            {
                try
                {
                    ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
                    ManagementObjectCollection queryCollection = query.Get();

                    foreach (ManagementObject mo in queryCollection)
                    {
                        if (mo["IPEnabled"].ToString() == "True")
                        {
                            _localMac = mo["MacAddress"].ToString();
                        }
                    }
                }
                catch
                {
                }
            }

            return _localMac;
        }

        private static string _cpuID = "";
        private static string GetCpuID()
        {
            if (string.IsNullOrEmpty(_cpuID))
            {
                try
                {
                    using(ManagementClass cimObject = new ManagementClass("Win32_Processor"))
                    {
                        ManagementObjectCollection moc = cimObject.GetInstances();

                        foreach (ManagementObject mo in moc)
                        {
                            _cpuID = mo.Properties["ProcessorId"].Value.ToString();
                            mo.Dispose();
                        }
                    }
                }
                catch
                {
                }
            }

            return _cpuID;
        }

        private static string _hdID = "";
        private static string GetHarddiskID()
        {
            if (string.IsNullOrEmpty(_hdID))
            {
                try
                {
                    using(ManagementClass cimObject = new ManagementClass("Win32_DiskDrive"))
                    {
                        ManagementObjectCollection moc = cimObject.GetInstances();

                        foreach (ManagementObject mo in moc)
                        {
                            _hdID = mo.Properties["Model"].Value.ToString();
                            mo.Dispose();
                        }
                    }
                }
                catch
                {
                }
            }

            return _hdID;
        }

        private static string _gatwway = "";
        private static IPStatus _netWorkStatus = IPStatus.Unknown;
        private static bool CheckNetWork()
        {
            try
            {
                if (_netWorkStatus == IPStatus.Unknown)
                {
                    if (string.IsNullOrEmpty(_gatwway))
                    {
                        using(ManagementClass cimObject = new ManagementClass("Win32_NetWorkAdapterConfiguration"))
                        {
                            ManagementObjectCollection moc = cimObject.GetInstances();

                            foreach (ManagementObject mo in moc)
                            {
                                if ((bool)mo["IPEnabled"])
                                {
                                    string[] gateways = (string[])mo["DefaultIPGateway"];

                                    foreach (string gw in gateways)
                                    {
                                        _gatwway += gw;
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(_gatwway))
                    {
                        Ping p = new Ping();
                        PingReply pr = p.Send(_gatwway);

                        _netWorkStatus = pr.Status;
                    }
                }
            }
            catch
            {
            }

            return (_netWorkStatus == IPStatus.Success);
        }

        private static bool ReportToTQOS(int intNum, string intList, int strNum, string strList)
        {
            try
            {
                if (!CheckNetWork())
                {
                    return false;
                }

                string qosData = "http://stats.behaviac.com/web/index.php?r=log/add&tqos={\"Head\":{\"Cmd\":5},\"Body\":{\"QOSRep\":{\"BusinessID\":1,\"QosNum\":1,\"QosList\":[{\"QosID\":7001,\"QosVal\":0,\"AppendDescFlag\":2,\"AppendDesc\":{\"Comm\":{\"IntNum\":";
                qosData += intNum;
                qosData += ",\"IntList\":[";
                qosData += intList;
                qosData += "],\"StrNum\":";
                qosData += strNum;
                qosData += ",\"StrList\":[";
                qosData += strList;
                qosData += "]}}}]}}}";

                using(var client = new WebClient())
                {
                    Uri uri = new Uri(qosData);
                    client.OpenReadAsync(uri);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        [Serializable]
        enum OperationTypes
        {
            kOpenEditor = 1,
            kOpenWorkspace,
            kLoadBehavior,
            kExportBehavior,
            kConnectGame,
            kOpenDoc,
        }

        [Serializable]
        class OperationData
        {
            public OperationTypes Type;
            public int Count;

            public OperationData(OperationTypes type)
            {
                Type = type;
                Count = 1;
            }
        }

        private static Object _lockObject = new Object();
        private static Thread _backgroundThread = new Thread(SendAllOperations);

        private static List<OperationData> _allOperations = new List<OperationData>();
        private static OperationData FindOperation(OperationTypes type)
        {
            lock (_lockObject)
            {
                for (int i = 0; i < _allOperations.Count; ++i)
                {
                    if (_allOperations[i].Type == type)
                    {
                        return _allOperations[i];
                    }
                }
            }

            return null;
        }

        private static string getHeaderString()
        {
            string workspaceName = (Workspace.Current != null) ? Workspace.Current.Name : string.Empty;

            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                 GetLocalIP(), GetCpuID(), GetHarddiskID(), System.Reflection.Assembly.GetEntryAssembly().GetName().Version,
                                 Dns.GetHostName(), GetLocalMac(), workspaceName, RuntimePlatform);
        }

        private static string getTqosFile()
        {
            string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BehaviacDesigner\\operations.tqos");
            filename = Path.GetFullPath(filename);
            return filename;
        }

        public static bool LoadOperations()
        {
            try
            {
                string filename = getTqosFile();
                Stream stream = File.Open(filename, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();

                lock (_lockObject)
                {
                    _allOperations = formatter.Deserialize(stream) as List<OperationData>;
                }
            }
            catch
            {
                return false;
            }

            SendOperations();

            return true;
        }

        public static bool SaveOperations()
        {
            if (SendOperations())
            {
                return false;
            }

            try
            {
                string filename = getTqosFile();
                Stream stream = File.Open(filename, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();

                lock (_lockObject)
                {
                    formatter.Serialize(stream, _allOperations);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static void SendAllOperations()
        {
            while (true)
            {
                lock (_lockObject)
                {
                    if (_allOperations.Count > 0)
                    {
                        SendOperations();
                    }
                }

                Thread.Sleep(10000);
            }
        }

        private static bool SendOperations()
        {
            bool sendSuccess = true;

            lock (_lockObject)
            {
                foreach (OperationData operation in _allOperations)
                {
                    int intNum = 8;
                    string intList = string.Format("0,0,0,0,0,0,{0},{1}", (int)operation.Type, operation.Count);

                    int strNum = 8;
                    string strList = getHeaderString();

                    if (!ReportToTQOS(intNum, intList, strNum, strList))
                    {
                        sendSuccess = false;
                        break;
                    }
                }

                if (sendSuccess)
                {
                    _allOperations.Clear();
                }
            }

            return sendSuccess;
        }

        public static void ReportOpenEditor()
        {
            _backgroundThread.IsBackground = true;
            _backgroundThread.Start();

            OperationData operation = FindOperation(OperationTypes.kOpenEditor);

            if (operation == null)
            {
                operation = new OperationData(OperationTypes.kOpenEditor);

                lock (_lockObject)
                {
                    _allOperations.Add(operation);
                }
            }
            else
            {
                operation.Count++;
            }
        }

        public static void ReportOpenWorkspace()
        {
            OperationData operation = FindOperation(OperationTypes.kOpenWorkspace);

            if (operation == null)
            {
                operation = new OperationData(OperationTypes.kOpenWorkspace);

                lock (_lockObject)
                {
                    _allOperations.Add(operation);
                }
            }
            else
            {
                operation.Count++;
            }
        }

        public static void ReportLoadBehavior()
        {
            OperationData operation = FindOperation(OperationTypes.kLoadBehavior);

            if (operation == null)
            {
                operation = new OperationData(OperationTypes.kLoadBehavior);

                lock (_lockObject)
                {
                    _allOperations.Add(operation);
                }
            }
            else
            {
                operation.Count++;
            }
        }

        public static void ReportExportBehavior()
        {
            OperationData operation = FindOperation(OperationTypes.kExportBehavior);

            if (operation == null)
            {
                operation = new OperationData(OperationTypes.kExportBehavior);

                lock (_lockObject)
                {
                    _allOperations.Add(operation);
                }
            }
            else
            {
                operation.Count++;
            }
        }

        public static void ReportConnectGame()
        {
            OperationData operation = FindOperation(OperationTypes.kConnectGame);

            if (operation == null)
            {
                operation = new OperationData(OperationTypes.kConnectGame);

                lock (_lockObject)
                {
                    _allOperations.Add(operation);
                }
            }
            else
            {
                operation.Count++;
            }
        }

        public static void ReportOpenDoc()
        {
            OperationData operation = FindOperation(OperationTypes.kOpenDoc);

            if (operation == null)
            {
                operation = new OperationData(OperationTypes.kOpenDoc);

                lock (_lockObject)
                {
                    _allOperations.Add(operation);
                }
            }
            else
            {
                operation.Count++;
            }
        }
    }
}
