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

#if !BEHAVIAC_RELEASE
#if !UNITY_WEBPLAYER && (UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX)
#define BEHAVIAC_HOTRELOAD
#endif
#endif

// please define BEHAVIAC_NOT_USE_UNITY in your project file if you are not using unity
#if !BEHAVIAC_NOT_USE_UNITY
// if you have compiling errors complaining the following using 'UnityEngine',
//usually, you need to define BEHAVIAC_NOT_USE_UNITY in your project file
using UnityEngine;
#endif//!BEHAVIAC_NOT_USE_UNITY


using System;
using System.IO;

using System.Collections.Generic;
using System.Reflection;
using System.Threading;

#if BEHAVIAC_USE_SYSTEM_XML
using System.Xml;
#else
using System.Security;
using MiniXml;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE_WIN

using System.Runtime.InteropServices;

#endif

namespace behaviac
{
    #region Config

    public static class Config
    {
        private static bool m_bProfiling = false;

        public static void LogInfo()
        {
            Debug.Log(string.Format("Config::IsProfiling {0}", Config.IsProfiling ? "true" : "false"));
            Debug.Log(string.Format("Config::IsLogging {0}", Config.IsLogging ? "true" : "false"));
            Debug.Log(string.Format("Config::IsLoggingFlush {0}", Config.IsLoggingFlush ? "true" : "false"));
            Debug.Log(string.Format("Config::IsSocketing {0}", Config.IsSocketing ? "true" : "false"));
            Debug.Log(string.Format("Config::IsSocketBlocking {0}", Config.IsSocketBlocking ? "true" : "false"));
            Debug.Log(string.Format("Config::IsHotReload {0}", Config.IsHotReload ? "true" : "false"));
            Debug.Log(string.Format("Config::SocketPort {0}", Config.SocketPort));
        }

        public static bool IsProfiling
        {
            get
            {
                return m_bProfiling;
            }
            set
            {
#if !BEHAVIAC_RELEASE
                m_bProfiling = value;
#else

                if (m_bProfiling)
                {
                    behaviac.Debug.LogWarning("Profiling can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
                }

#endif
            }
        }

        public static bool IsLoggingOrSocketing
        {
            get
            {
                return IsLogging || IsSocketing;
            }
        }

#if !BEHAVIAC_RELEASE
        private static bool m_bIsLogging = false;
#else
        private static bool m_bIsLogging = false;
#endif

        ///it is disable on pc by default
        public static bool IsLogging
        {
            get
            {
                //logging is only enabled on pc platform, it is disabled on android, ios, etc.
                return m_bIsLogging;
            }
            set
            {
#if !BEHAVIAC_RELEASE
                m_bIsLogging = value;
#else

                if (m_bIsLogging)
                {
                    behaviac.Debug.LogWarning("Logging can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
                }

#endif
            }
        }

#if !BEHAVIAC_RELEASE
        private static bool m_bIsLoggingFlush = false;
#else
        private static bool m_bIsLoggingFlush = false;
#endif

        ///it is disable on pc by default
        public static bool IsLoggingFlush
        {
            get
            {
                //logging is only enabled on pc platform, it is disabled on android, ios, etc.
                return m_bIsLoggingFlush;
            }
            set
            {
#if !BEHAVIAC_RELEASE
                m_bIsLoggingFlush = value;
#else

                if (m_bIsLoggingFlush)
                {
                    behaviac.Debug.LogWarning("Logging can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
                }

#endif
            }
        }

#if !BEHAVIAC_RELEASE
        private static bool m_bIsSocketing = true;
#else
        private static bool m_bIsSocketing = false;
#endif

        //it is enabled on pc by default
        public static bool IsSocketing
        {
            get
            {
                return m_bIsSocketing;
            }
            set
            {
                Debug.Check(!Workspace.Instance.IsInited, "please call Config.IsSocketing at the very begining!");

#if !BEHAVIAC_RELEASE
                m_bIsSocketing = value;
#else

                if (m_bIsLogging)
                {
                    behaviac.Debug.LogWarning("Socketing can't be enabled on Release! please don't define BEHAVIAC_RELEASE to enable it!\n");
                }

#endif
            }
        }

        private static bool m_bIsSocketBlocking = false;

        public static bool IsSocketBlocking
        {
            get
            {
                return m_bIsSocketBlocking;
            }
            set
            {
                Debug.Check(!Workspace.Instance.IsInited, "please call Config.IsSocketBlocking at the very begining!");
                m_bIsSocketBlocking = value;
            }
        }

        private static ushort m_socketPort = 60636;

        public static ushort SocketPort
        {
            get
            {
                return m_socketPort;
            }
            set
            {
                Debug.Check(!Workspace.Instance.IsInited, "please call Config.SocketPort at the very begining!");
                m_socketPort = value;
            }
        }

        private static bool m_bIsHotReload = true;

        public static bool IsHotReload
        {
            get
            {
                return m_bIsHotReload;
            }
            set
            {
                Debug.Check(!Workspace.Instance.IsInited, "please call Config.IsHotReload at the very begining!");
                m_bIsHotReload = value;
            }
        }

        private static bool m_bIsSuppressingNonPublicWarning;

        /// <summary>
        /// Gets or sets a value indicating is supressing non public warning.
        /// </summary>
        /// <value><c>true</c> if is supressing non public warning; otherwise, <c>false</c>.</value>
        public static bool IsSuppressingNonPublicWarning
        {
            get
            {
                return m_bIsSuppressingNonPublicWarning;
            }
            set
            {
                m_bIsSuppressingNonPublicWarning = value;
            }
        }

        private static bool m_bPreloadBehaviors = true;

        public static bool PreloadBehaviors
        {
            get
            {
                return m_bPreloadBehaviors;
            }
            set
            {
                m_bPreloadBehaviors = value;
            }
        }
    }

    #endregion Config

    public class Workspace : IDisposable
    {
        #region Singleton

        private static Workspace ms_instance = null;

        public Workspace()
        {
            Debug.Check(ms_instance == null);
            ms_instance = this;
        }

        public void Dispose()
        {
            ms_instance = null;
        }

        public static Workspace Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    Workspace instance = new Workspace();
                    Debug.Check(instance != null);
                    Debug.Check(ms_instance != null);
                }

                return ms_instance;
            }
        }

        #endregion Singleton

        [Flags]
        public enum EFileFormat
        {
            EFF_xml = 1,		                    //specify to use xml
            EFF_bson = 2,		                    //specify to use bson
            EFF_cs = 4,                            //specify to use cs
            EFF_default = EFF_xml | EFF_bson | EFF_cs,  //use the format specified by SetWorkspaceSettings
        };

        private EFileFormat fileFormat_ = EFileFormat.EFF_xml;

        public EFileFormat FileFormat
        {
            get
            {
                return fileFormat_;
            }
            set
            {
                fileFormat_ = value;
            }
        }

        private static string GetDefaultFilePath()
        {
            string path = "";

#if !BEHAVIAC_NOT_USE_UNITY
            string relativePath = "/Resources/behaviac/exported";

            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsEditor)
            {
                path = UnityEngine.Application.dataPath + relativePath;
            }
            else if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer)
            {
                path = UnityEngine.Application.dataPath + relativePath;
            }
            else
            {
                path = "Assets" + relativePath;
            }

#endif

            return path;
        }

        private string m_filePath = null;

        //read from 'WorkspaceFile', prepending with 'WorkspacePath', relative to the exe's path
        public string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_filePath))
                {
                    m_filePath = GetDefaultFilePath();
                }

                return this.m_filePath;
            }
            set
            {
                this.m_filePath = value;
            }
        }

        private string m_metaFile = null;

        public string MetaFile
        {
            get
            {
                return this.m_metaFile;
            }
            set
            {
                this.m_metaFile = value;
            }
        }

        private int m_frameSinceStartup = -1;

        //
        // Summary:
        //     The frames since the game started.
        public virtual int FrameSinceStartup
        {
            get
            {
#if !BEHAVIAC_NOT_USE_UNITY
                return (m_frameSinceStartup < 0) ? UnityEngine.Time.frameCount : m_frameSinceStartup;
#else
                return m_frameSinceStartup;
#endif
            }

            set
            {
                m_frameSinceStartup = value;
            }
        }

        private bool _useIntValue = false;
        public bool UseIntValue
        {
            get
            {
                return _useIntValue;
            }
            set
            {
                _useIntValue = value;
            }
        }

        private double m_doubleValueSinceStartup = -1.0;

        // Deprecated property, use DoubleValueSinceStartup insteadly.
        public virtual double TimeSinceStartup
        {
            get
            {
                Debug.Check(!UseIntValue);

#if !BEHAVIAC_NOT_USE_UNITY

                if (this.m_doubleValueSinceStartup >= 0)
                {
                    return this.m_doubleValueSinceStartup * 0.001;
                }

                return UnityEngine.Time.realtimeSinceStartup;
#else
                return this.m_doubleValueSinceStartup * 0.001;
#endif
            }

            set
            {
                Debug.Check(!UseIntValue);

                this.m_doubleValueSinceStartup = value * 1000;
            }
        }

        public virtual double DoubleValueSinceStartup
        {
            get
            {
                Debug.Check(!UseIntValue);

#if !BEHAVIAC_NOT_USE_UNITY

                if (this.m_doubleValueSinceStartup >= 0)
                {
                    return this.m_doubleValueSinceStartup;
                }

                return UnityEngine.Time.realtimeSinceStartup * 1000;
#else
                return this.m_doubleValueSinceStartup;
#endif
            }

            set
            {
                Debug.Check(!UseIntValue);

                this.m_doubleValueSinceStartup = value;
            }
        }

        private long m_intValueSinceStartup = -1;

        public virtual long IntValueSinceStartup
        {
            get
            {
                Debug.Check(UseIntValue);

#if !BEHAVIAC_NOT_USE_UNITY

                if (this.m_intValueSinceStartup >= 0)
                {
                    return this.m_intValueSinceStartup;
                }

                return (long)(UnityEngine.Time.realtimeSinceStartup * 1000);
#else
                return this.m_intValueSinceStartup;
#endif
            }

            set
            {
                Debug.Check(UseIntValue);

                this.m_intValueSinceStartup = value;
            }
        }

#if !BEHAVIAC_RELEASE
        private string m_workspaceExportPathAbs;
#endif

        public delegate void BehaviorNodeLoader(string nodeType, List<property_t> properties);
        event BehaviorNodeLoader BehaviorNodeLoaded;
        /**
            'BehaviorNodeLoaded' will be called for ever behavior node.
        */
        public void OnBehaviorNodeLoaded(string nodeType, List<property_t> properties)
        {
            if (BehaviorNodeLoaded != null)
            {
                BehaviorNodeLoaded(nodeType, properties);
            }
        }

        public delegate void DRespondToBreakHandler(string msg, string title);

        public event DRespondToBreakHandler RespondToBreakHandler;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(int hWnd, String text, String caption, int options);

#endif

        // respond to msg, where msg = string.Format("BehaviorTreeTask Breakpoints at: '{0}{1}'\n\nOk to continue.", btMsg, actionResultStr);
        // display a message box to block the execution and then continue the execution after closing the message box
        public void RespondToBreak(string msg, string title)
        {
            if (RespondToBreakHandler != null)
            {
                RespondToBreakHandler(msg, title);

                return;
            }
            else
            {
                this.WaitforContinue();

                //#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                //				if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
                //				{
                //					const int MB_SYSTEMMODAL = 0x00001000;
                //					MessageBox(0, msg, title, MB_SYSTEMMODAL);
                //				}
                //#endif
            }

            //MessageBoxEvent
            System.Threading.Thread.Sleep(500);
        }

        private bool m_bRegistered = false;
        /**
        set the workspace settings

        'workspaceRootPath_' specifies the file path of of the exported path of the workspace file which is configured in the workspace file(.workspace.xml),
        it can be either an absolute path or relative to the current path.
        'format' specify the format to use, xml or bson,

        the default format is xml.

        @return false if 'path' is not a valid path holding valid data
        */
        private bool m_bInited = false;
        internal bool IsInited
        {
            get
            {
                return m_bInited;
            }
        }

        public bool TryInit()
        {
            if (this.m_bInited)
            {
                return true;
            }

            this.m_bInited = true;

            ComparerRegister.Init();
            ComputerRegister.Init();

            Workspace.Instance.RegisterStuff();

            Config.LogInfo();

            if (string.IsNullOrEmpty(this.FilePath))
            {
                behaviac.Debug.LogError("No FilePath file is specified!");
                behaviac.Debug.Check(false);

                return false;
            }

            Debug.Log(string.Format("FilePath: {0}\n", this.FilePath));

            Debug.Check(!this.FilePath.EndsWith("\\"), "use '/' instead of '\\'");

            m_frameSinceStartup = -1;

#if !BEHAVIAC_RELEASE
            this.m_workspaceExportPathAbs = Path.GetFullPath(this.FilePath).Replace('\\', '/');

            if (!this.m_workspaceExportPathAbs.EndsWith("/"))
            {
                this.m_workspaceExportPathAbs += '/';
            }

#if BEHAVIAC_HOTRELOAD

            // set the file watcher
            if (behaviac.Config.IsHotReload)
            {
                if (this.FileFormat != EFileFormat.EFF_cs)
                {
                    if (m_DirectoryMonitor == null)
                    {
                        m_DirectoryMonitor = new DirectoryMonitor();
                        m_DirectoryMonitor.Changed += new DirectoryMonitor.FileSystemEvent(OnFileChanged);
                    }

                    string filter = "*.*";

                    if (this.FileFormat == EFileFormat.EFF_xml)
                    {
                        filter = "*.xml";
                    }
                    else if (this.FileFormat == EFileFormat.EFF_bson)
                    {
                        filter = "*.bson.bytes";
                    }

                    if (File.Exists(this.FilePath))
                    {
                        m_DirectoryMonitor.Start(this.FilePath, filter);
                    }
                }
            }

#endif//BEHAVIAC_HOTRELOAD

            //LogWorkspaceInfo();
#endif

            if (Config.IsSocketing)
            {
                bool isBlockSocket = Config.IsSocketBlocking;
                ushort port = Config.SocketPort;
                behaviac.SocketUtils.SetupConnection(isBlockSocket, port);
            }

            return true;
        }

        public void Cleanup()
        {
            if (Config.IsSocketing)
            {
                behaviac.SocketUtils.ShutdownConnection();
            }

            this.UnLoadAll();

            Debug.Check(this.m_bRegistered);

            ComparerRegister.Cleanup();
            ComputerRegister.Cleanup();

            this.UnRegisterStuff();

            Context.Cleanup(-1);

#if BEHAVIAC_HOTRELOAD

            if (behaviac.Config.IsHotReload)
            {
                m_modifiedFiles.Clear();

                if (m_DirectoryMonitor != null)
                {
                    m_DirectoryMonitor.Changed -= new DirectoryMonitor.FileSystemEvent(OnFileChanged);
                    m_DirectoryMonitor.Stop();
                    m_DirectoryMonitor = null;
                }
            }

#endif

#if BEHAVIAC_USE_HTN
            PlannerTask.Cleanup();
#endif//

            LogManager.Instance.Close();

            this.m_bInited = false;
        }

        internal void RegisterStuff()
        {
            //only register metas and others at the 1st time
            if (!this.m_bRegistered)
            {
                this.m_bRegistered = true;

                AgentMeta.Register();

                //#if !BEHAVIAC_RELEASE
                //                this.RegisterMetas();
                //#endif
            }
        }

        private void UnRegisterStuff()
        {
            Debug.Check(this.m_bRegistered);

            this.UnRegisterBehaviorNode();
            //#if !BEHAVIAC_RELEASE
            //            this.UnRegisterMetas();
            //#endif

            AgentMeta.UnRegister();

            this.m_bRegistered = false;
        }

        public void LogWorkspaceInfo()
        {
            OperatingSystem osInfo = Environment.OSVersion;
            string platformID = osInfo.Platform.ToString();

            string msg = string.Format("[platform] {0}\n", platformID);
            LogManager.Instance.LogWorkspace(msg);

            Workspace.EFileFormat format = this.FileFormat;
            string formatString = (format == Workspace.EFileFormat.EFF_bson ? "bson.bytes" : (format == Workspace.EFileFormat.EFF_cs ? "cs" : "xml"));

            msg = string.Format("[workspace] {0} \"{1}\"\n", formatString, "");
            LogManager.Instance.LogWorkspace(msg);
        }

        private bool LoadWorkspaceSetting(string file, string ext, ref string workspaceFile)
        {
            try
            {
                byte[] pBuffer = ReadFileToBuffer(file, ext);

                if (pBuffer != null)
                {
                    string xml = System.Text.Encoding.UTF8.GetString(pBuffer);
#if BEHAVIAC_USE_SYSTEM_XML
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xml);

                    XmlNode rootNode = xmlDoc.DocumentElement;

                    if (rootNode.Name == "workspace")
                    {
                        workspaceFile = rootNode.Attributes["path"].Value;
                        return true;
                    }

#else
                    SecurityParser xmlDoc = new SecurityParser();
                    xmlDoc.LoadXml(xml);

                    SecurityElement rootNode = xmlDoc.ToXml();

                    if (rootNode.Tag == "workspace")
                    {
                        workspaceFile = rootNode.Attribute("path");
                        return true;
                    }

#endif
                }
            }
            catch (Exception e)
            {
                string errorInfo = string.Format("Load Workspace {0} Error : {1}", file, e.Message);
                behaviac.Debug.LogError(errorInfo);
            }

            return false;
        }

        #region HotReload
#if BEHAVIAC_HOTRELOAD
        #region DirectoryMonitor

        public class DirectoryMonitor
        {
            public delegate void FileSystemEvent(string path);

            public event FileSystemEvent Changed;

            private readonly FileSystemWatcher m_fileSystemWatcher = new FileSystemWatcher();
            private readonly Dictionary<string, DateTime> m_pendingEvents = new Dictionary<string, DateTime>();
            private readonly Timer m_timer;
            private bool m_timerStarted = false;

            public DirectoryMonitor()
            {
                m_fileSystemWatcher.IncludeSubdirectories = true;
                m_fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                m_fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
                m_fileSystemWatcher.EnableRaisingEvents = false;

                m_timer = new Timer(OnTimeout, null, Timeout.Infinite, Timeout.Infinite);
            }

            public void Start(string dirPath, string filter)
            {
                m_fileSystemWatcher.Path = dirPath;
                m_fileSystemWatcher.Filter = filter;
                m_fileSystemWatcher.EnableRaisingEvents = true;
            }

            public void Stop()
            {
                m_fileSystemWatcher.EnableRaisingEvents = false;
            }

            private void OnChanged(object sender, FileSystemEventArgs e)
            {
                // Don't want other threads messing with the pending events right now
                lock (m_pendingEvents)
                {
                    // Save a timestamp for the most recent event for this path
                    m_pendingEvents[e.FullPath] = DateTime.Now;

                    // Start a timer if not already started
                    if (!m_timerStarted)
                    {
                        m_timer.Change(100, 100);
                        m_timerStarted = true;
                    }
                }
            }

            private void OnTimeout(object state)
            {
                List<string> paths;

                // Don't want other threads messing with the pending events right now
                lock (m_pendingEvents)
                {
                    // Get a list of all paths that should have events thrown
                    paths = FindReadyPaths(m_pendingEvents);

                    // Remove paths that are going to be used now
                    paths.ForEach(delegate(string path)
                    {
                        m_pendingEvents.Remove(path);
                    });

                    // Stop the timer if there are no more events pending
                    if (m_pendingEvents.Count == 0)
                    {
                        m_timer.Change(Timeout.Infinite, Timeout.Infinite);
                        m_timerStarted = false;
                    }
                }

                // Fire an event for each path that has changed
                paths.ForEach(delegate(string path)
                {
                    FireEvent(path);
                });
            }

            private List<string> FindReadyPaths(Dictionary<string, DateTime> events)
            {
                List<string> results = new List<string>();
                DateTime now = DateTime.Now;

                var e = events.GetEnumerator();

                while (e.MoveNext())
                {
                    // If the path has not received a new event in the last 75ms
                    // an event for the path should be fired
                    double diff = now.Subtract(e.Current.Value).TotalMilliseconds;

                    if (diff >= 75)
                    {
                        results.Add(e.Current.Key);
                    }
                }

                return results;
            }

            private void FireEvent(string path)
            {
                FileSystemEvent evt = this.Changed;

                if (evt != null)
                {
                    evt(path);
                }
            }
        }

        #endregion DirectoryMonitor

        private DirectoryMonitor m_DirectoryMonitor = null;
        private List<string> m_ModifiedFiles = new List<string>();

        private void OnFileChanged(string fullpath)
        {
            if (string.IsNullOrEmpty(fullpath))
            {
                return;
            }

            //behaviac.behaviac.Debug.LogWarning(string.Format("OnFileChanged:{0}", fullpath));

            int index = -1;

            for (int i = 0; i < fullpath.Length - 1; ++i)
            {
                if (fullpath[i] == '.' && fullpath[i + 1] != '.'
                    && fullpath[i + 1] != '/' && fullpath[i + 1] != '\\')
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                string filename = fullpath.Substring(0, index).Replace('\\', '/');
                string ext = fullpath.Substring(index).ToLowerInvariant();

                if (!string.IsNullOrEmpty(ext) &&
                    (this.FileFormat & EFileFormat.EFF_xml) == EFileFormat.EFF_xml && ext == ".xml" ||
                    (this.FileFormat & EFileFormat.EFF_bson) == EFileFormat.EFF_bson && ext == ".bson.bytes")
                {
                    int pos = filename.IndexOf(m_workspaceExportPathAbs);

                    if (pos != -1)
                    {
                        filename = filename.Substring(m_workspaceExportPathAbs.Length + pos);

                        lock (m_ModifiedFiles)
                        {
                            if (!m_ModifiedFiles.Contains(filename))
                            {
                                m_ModifiedFiles.Add(filename);
                            }
                        }
                    }
                }
            }
        }

        private List<string> m_modifiedFiles = new List<string>();

        private List<string> ModifiedFiles
        {
            get
            {
                return m_modifiedFiles;
            }
        }

        private bool GetModifiedFiles()
        {
            if (m_ModifiedFiles.Count > 0)
            {
                lock (m_ModifiedFiles)
                {
                    m_modifiedFiles.Clear();
                    m_modifiedFiles.AddRange(m_ModifiedFiles);
                    m_ModifiedFiles.Clear();
                }

                return true;
            }

            return false;
        }

        protected void HotReload()
        {
#if !BEHAVIAC_RELEASE

            if (behaviac.Config.IsHotReload)
            {
                if (GetModifiedFiles())
                {
                    for (int i = 0; i < ModifiedFiles.Count; ++i)
                    {
                        string relativePath = ModifiedFiles[i];

                        if (m_allBehaviorTreeTasks.ContainsKey(relativePath))
                        {
                            behaviac.Debug.LogWarning(string.Format("Hotreload:{0}", relativePath));

                            if (Load(relativePath, true))
                            {
                                BTItem_t btItem = m_allBehaviorTreeTasks[relativePath];
                                BehaviorTree behaviorTree = m_behaviortrees[relativePath];

                                for (int j = 0; j < btItem.bts.Count; ++j)
                                {
                                    BehaviorTreeTask behaviorTreeTask = btItem.bts[j];
                                    behaviorTreeTask.reset(null);
                                    behaviorTreeTask.Clear();
                                    behaviorTreeTask.Init(behaviorTree);
                                }

                                for (int j = 0; j < btItem.agents.Count; ++j)
                                {
                                    Agent agent = btItem.agents[j];
                                    agent.bthotreloaded(behaviorTree);
                                }
                            }
                        }
                    }
                }
            }

#endif
        }

#else
        public void SetAutoHotReload(bool enable)
        {
        }

        public bool GetAutoHotReload()
        {
            return false;
        }

        public void HotReload()
        {
        }
#endif//#if BEHAVIAC_HOTRELOAD

        #endregion HotReload

        #region Development

#if !BEHAVIAC_RELEASE
        private string m_applogFilter;
#endif

        //[breakpoint] set TestBehaviorGroup\btunittest.xml Sequence[3] enter
        //[breakpoint] set TestBehaviorGroup\btunittest.xml Sequence[3] exit
        //[breakpoint] clear TestBehaviorGroup\btunittest.xml Sequence[3] enter
        private class BreakpointInfo_t
        {
            public string btname;

            public ushort hit_config;

            public EActionResult action_result;

            public BreakpointInfo_t()
            {
                hit_config = 0;
                action_result = EActionResult.EAR_all;
            }
        };

        private Dictionary<uint, BreakpointInfo_t> m_breakpoints = new Dictionary<uint, BreakpointInfo_t>();

        private Dictionary<CStringID, int> m_actions_count = new Dictionary<CStringID, int>();

        //[breakpoint] add TestBehaviorGroup\btunittest.xml.Sequence[3]:enter all Hit=1
        //[breakpoint] add TestBehaviorGroup\btunittest.xml.Sequence[3]:exit all Hit=1
        //[breakpoint] add TestBehaviorGroup\btunittest.xml.Sequence[3]:exit success Hit=1
        //[breakpoint] add TestBehaviorGroup\btunittest.xml.Sequence[3]:exit failure Hit=1
        //[breakpoint] remove TestBehaviorGroup\btunittest.x1ml.Sequence[3]:enter all Hit=10
        private void ParseBreakpoint(string[] tokens)
        {
            BreakpointInfo_t bp = new BreakpointInfo_t();

            bool bAdd = false;
            bool bRemove = false;

            if (tokens[1] == "add")
            {
                bAdd = true;
            }
            else if (tokens[1] == "remove")
            {
                bRemove = true;
            }
            else
            {
                Debug.Check(false);
            }

            bp.btname = tokens[2];

            if (tokens[3] == "all")
            {
                Debug.Check(bp.action_result == EActionResult.EAR_all);
            }
            else if (tokens[3] == "success")
            {
                bp.action_result = EActionResult.EAR_success;
            }
            else if (tokens[3] == "failure")
            {
                bp.action_result = EActionResult.EAR_failure;
            }
            else
            {
                Debug.Check(false);
            }

            const string kHitNumber = "Hit=";
            int posb = tokens[4].IndexOf(kHitNumber);

            if (posb != -1)
            {
                posb = tokens[4].IndexOf('=');
                Debug.Check(posb != -1);

                int size = -1;
                //tokens[4] is the last one with '\n'
                int pose = tokens[4].IndexOf('\n');

                if (pose != -1)
                {
                    size = pose - posb - 1;
                }
                else
                {
                    size = tokens[4].Length - posb - 1;
                }

                string numString = tokens[4].Substring(posb + 1, size);
                bp.hit_config = ushort.Parse(numString);
            }

            uint bpid = Utils.MakeVariableId(bp.btname);

            if (bAdd)
            {
                m_breakpoints[bpid] = bp;
            }
            else if (bRemove)
            {
                m_breakpoints.Remove(bpid);
            }
        }

        private void ParseProfiling(string[] tokens)
        {
            if (tokens[1] == "true")
            {
                Config.IsProfiling = true;
            }
            else if (tokens[1] == "false")
            {
                Config.IsProfiling = false;
            }
            else
            {
                Debug.Check(false);
            }
        }

        private void ParseAppLogFilter(string[] tokens)
        {
#if !BEHAVIAC_RELEASE
            m_applogFilter = tokens[1];
#endif
        }

        //[property]Player#@Player int Index->0
        private void ParseProperty(string[] tokens)
        {
#if !BEHAVIAC_RELEASE
            string agentName = tokens[1];
            Agent pAgent = Agent.GetAgent(agentName);

            //pAgent could be 0
            if (!System.Object.ReferenceEquals(pAgent, null) && tokens.Length == 4)
            {
                //string varTypeName = tokens[2];
                string varNameValue = tokens[3];

                int size = -1;
                int posb = varNameValue.IndexOf("->");
                //varNameValue is the last one with '\n'
                int pose = varNameValue.IndexOf('\n');

                if (pose == -1)
                {
                    pose = varNameValue.Length;
                }

                size = pose - posb - 2;

                string varName = varNameValue.Substring(0, posb);
                string varValue = varNameValue.Substring(posb + 2, size);

                // If pAgent.name is "null", pAgent != null will return false.
                if (pAgent != null && !System.Object.ReferenceEquals(pAgent, null))
                {
                    pAgent.SetVariableFromString(varName, varValue);
                }//end of if (pAgent)
            }
#endif
        }

        private int m_frame = 0;

        protected void LogFrames()
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                LogManager.Instance.Log("[frame]{0}\n", (this.FrameSinceStartup >= 0) ? this.FrameSinceStartup : (this.m_frame++));
            }

#endif
        }

        protected void WaitforContinue()
        {
#if !BEHAVIAC_RELEASE

            while (!HandleRequests())
            {
                System.Threading.Thread.Sleep(200);
            }

#endif//BEHAVIAC_RELEASE
        }

        protected bool HandleRequests()
        {
            bool bContinue = false;

#if !BEHAVIAC_RELEASE

            if (Config.IsSocketing)
            {
                string command = "";

                if (SocketUtils.ReadText(ref command))
                {
                    const string kBreakpoint = "[breakpoint]";
                    const string kProperty = "[property]";
                    const string kProfiling = "[profiling]";
                    const string kStart = "[start]";
                    const string kAppLogFilter = "[applogfilter]";
                    const string kContinue = "[continue]";
                    const string kCloseConnection = "[closeconnection]";

                    string[] cs = command.Split('\n');

                    foreach (string c in cs)
                    {
                        if (string.IsNullOrEmpty(c))
                        {
                            continue;
                        }

                        string[] tokens = c.Split(' ');

                        if (tokens[0] == kBreakpoint)
                        {
                            ParseBreakpoint(tokens);
                        }
                        else if (tokens[0] == kProperty)
                        {
                            ParseProperty(tokens);
                        }
                        else if (tokens[0] == kProfiling)
                        {
                            ParseProfiling(tokens);
                        }
                        else if (tokens[0] == kStart)
                        {
                            m_breakpoints.Clear();
                            bContinue = true;
                        }
                        else if (tokens[0] == kAppLogFilter)
                        {
                            ParseAppLogFilter(tokens);
                        }
                        else if (tokens[0] == kContinue)
                        {
                            bContinue = true;
                        }
                        else if (tokens[0] == kCloseConnection)
                        {
                            m_breakpoints.Clear();
                            bContinue = true;
                        }
                        else
                        {
                            Debug.Check(false);
                        }
                    }//end of for
                }//end of if (Socket::ReadText(command))

                else
                {
                    if (!SocketUtils.IsConnected())
                    {
                        //connection has something wrong
                        bContinue = true;
                    }
                }
            }
            else
            {
                bContinue = true;
            }

#endif

            return bContinue;
        }

        private bool m_bExecAgents = false;

        public bool IsExecAgents
        {
            get
            {
                return this.m_bExecAgents;
            }
            set
            {
                this.m_bExecAgents = value;
            }
        }

        public void DebugUpdate()
        {
            this.LogFrames();
            this.HandleRequests();

            if (behaviac.Config.IsHotReload)
            {
                this.HotReload();
            }
        }

        public void Update()
        {
            this.DebugUpdate();

            if (this.m_bExecAgents)
            {
                int contextId = -1;

                Context.execAgents(contextId);
            }
        }

        public void LogCurrentStates()
        {
            int contextId = -1;
            Context.LogCurrentStates(contextId);
        }

        public bool CheckBreakpoint(Agent pAgent, BehaviorNode b, string action, EActionResult actionResult)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsSocketing)
            {
                string bpStr = BehaviorTask.GetTickInfo(pAgent, b, action);

                uint bpid = Utils.MakeVariableId(bpStr);

                if (m_breakpoints.ContainsKey(bpid))
                {
                    BreakpointInfo_t bp = m_breakpoints[bpid];

                    if ((bp.action_result & actionResult) != 0)
                    {
                        int count = GetActionCount(bpStr);
                        Debug.Check(count > 0);

                        if (bp.hit_config == 0 || bp.hit_config == count)
                        {
                            return true;
                        }
                    }
                }
            }

#endif
            return false;
        }

        public bool CheckAppLogFilter(string filter)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsSocketing)
            {
                //m_applogFilter is UPPER
                if (!string.IsNullOrEmpty(m_applogFilter))
                {
                    if (m_applogFilter == "ALL")
                    {
                        return true;
                    }
                    else
                    {
                        string f = filter.ToUpper();

                        if (m_applogFilter == f)
                        {
                            return true;
                        }
                    }
                }
            }

#endif
            return false;
        }

        public int UpdateActionCount(string actionString)
        {
            lock (m_actions_count)
            {
                int count = 1;
                CStringID actionId = new CStringID(actionString);

                if (!m_actions_count.ContainsKey(actionId))
                {
                    m_actions_count[actionId] = count;
                }
                else
                {
                    count = m_actions_count[actionId];
                    count++;
                    m_actions_count[actionId] = count;
                }

                return count;
            }
        }

        public int GetActionCount(string actionString)
        {
            lock (m_actions_count)
            {
                int count = 0;
                CStringID actionId = new CStringID(actionString);

                if (m_actions_count.ContainsKey(actionId))
                {
                    count = m_actions_count[actionId];
                }

                return count;
            }
        }

        #endregion Development

        #region Load

        private Dictionary<string, BehaviorTree> m_behaviortrees;

        private Dictionary<string, BehaviorTree> BehaviorTrees
        {
            get
            {
                if (m_behaviortrees == null)
                {
                    m_behaviortrees = new Dictionary<string, BehaviorTree>();
                }

                return m_behaviortrees;
            }
        }

        private Dictionary<string, MethodInfo> m_btCreators;

        private Dictionary<string, MethodInfo> BTCreators
        {
            get
            {
                if (m_btCreators == null)
                {
                    m_btCreators = new Dictionary<string, MethodInfo>();
                }

                return m_btCreators;
            }
        }

        public void RecordBTAgentMapping(string relativePath, Agent agent)
        {
            if (m_allBehaviorTreeTasks == null)
            {
                m_allBehaviorTreeTasks = new Dictionary<string, BTItem_t>();
            }

            if (!m_allBehaviorTreeTasks.ContainsKey(relativePath))
            {
                m_allBehaviorTreeTasks[relativePath] = new BTItem_t();
            }

            BTItem_t btItems = m_allBehaviorTreeTasks[relativePath];
            //bool bFound = false;

            if (btItems.agents.IndexOf(agent) == -1)
            {
                btItems.agents.Add(agent);
            }
        }

        public void UnLoad(string relativePath)
        {
            Debug.Check(string.IsNullOrEmpty(StringUtils.FindExtension(relativePath)), "no extention to specify");
            Debug.Check(this.IsValidPath(relativePath));

            if (BehaviorTrees.ContainsKey(relativePath))
            {
                BehaviorTrees.Remove(relativePath);
            }
        }

        public void UnLoadAll()
        {
            m_allBehaviorTreeTasks.Clear();
            BehaviorTrees.Clear();
            BTCreators.Clear();
        }

        public byte[] ReadFileToBuffer(string file, string ext)
        {
            byte[] pBuffer = FileManager.Instance.FileOpen(file, ext);

            return pBuffer;
        }

        public void PopFileFromBuffer(string file, string ext, byte[] pBuffer)
        {
            FileManager.Instance.FileClose(file, ext, pBuffer);
        }

        private string getValidFilename(string filename)
        {
            filename = filename.Replace("/", "_");
            filename = filename.Replace("-", "_");
            return filename;
        }

        /**
        Load the specified behavior tree

        the workspace export path is provided by Workspace.FilePath
        the file format(xml/bson) is provided by Workspace.FileFormat

        generally, you need to derive Workspace and override FilePath and FileFormat,
        then, instantiate your derived Workspace at the very beginning

        @param relativePath
        a path relateve to the workspace exported path. relativePath should not include extension.
        @param bForce
        force to load, otherwise it just uses the one in the cache
        */
        public bool Load(string relativePath, bool bForce)
        {
            Debug.Check(string.IsNullOrEmpty(StringUtils.FindExtension(relativePath)), "no extention to specify");
            Debug.Check(this.IsValidPath(relativePath));

            TryInit();

            BehaviorTree pBT = null;

            if (BehaviorTrees.ContainsKey(relativePath))
            {
                if (!bForce)
                {
                    return true;
                }

                pBT = BehaviorTrees[relativePath];
            }

            string fullPath = Path.Combine(this.FilePath, relativePath);
            fullPath = fullPath.Replace('\\', '/');

            string ext = "";
            EFileFormat f = this.FileFormat;

            this.HandleFileFormat(fullPath, ref ext, ref f);

            bool bLoadResult = false;

            bool bCleared = false;
            bool bNewly = false;

            if (pBT == null)
            {
                bNewly = true;
                pBT = new BehaviorTree();

                //in case of circular referencebehavior
                BehaviorTrees[relativePath] = pBT;
            }

            Debug.Check(pBT != null);

            if (f == EFileFormat.EFF_xml || f == EFileFormat.EFF_bson)
            {
                byte[] pBuffer = ReadFileToBuffer(fullPath, ext);

                if (pBuffer != null)
                {
                    //if forced to reload
                    if (!bNewly)
                    {
                        bCleared = true;
                        pBT.Clear();
                    }

                    if (f == EFileFormat.EFF_xml)
                    {
                        bLoadResult = pBT.load_xml(pBuffer);
                    }
                    else
                    {
                        bLoadResult = pBT.load_bson(pBuffer);
                    }

                    PopFileFromBuffer(fullPath, ext, pBuffer);
                }
                else
                {
                    Debug.LogError(string.Format("'{0}' doesn't exist!, Please set Workspace.FilePath", fullPath));
                    Debug.Check(false);
                }
            }
            else if (f == EFileFormat.EFF_cs)
            {
                if (!bNewly)
                {
                    bCleared = true;
                    pBT.Clear();
                }

                try
                {
                    MethodInfo m = null;

                    if (BTCreators.ContainsKey(relativePath))
                    {
                        m = BTCreators[relativePath];
                    }
                    else
                    {
                        string clsName = "behaviac.bt_" + getValidFilename(relativePath);
                        Type type = Utils.GetType(clsName);

                        if (type != null)
                        {
                            m = type.GetMethod("build_behavior_tree", BindingFlags.Public | BindingFlags.Static);
                            Debug.Check(m != null);

                            if (m != null)
                            {
                                BTCreators[relativePath] = m;
                            }
                        }
                    }

                    if (m != null)
                    {
                        object[] args = { pBT };
                        bLoadResult = (bool)m.Invoke(null, args);
                    }
                    else
                    {
                        Debug.Check(false);
                        Debug.LogError("The generated_behaviors.cs file should be added into the app.");
                    }
                }
                catch (Exception e)
                {
                    string errorInfo = string.Format("The behavior {0} failed to be loaded : {1}", relativePath, e.Message);
                    Debug.LogError(errorInfo);
                }
            }
            else
            {
                Debug.Check(false);
            }

            if (bLoadResult)
            {
                Debug.Check(pBT.GetName() == relativePath);

                if (!bNewly)
                {
                    Debug.Check(BehaviorTrees[pBT.GetName()] == pBT);
                }
            }
            else
            {
                if (bNewly)
                {
                    bool removed = BehaviorTrees.Remove(relativePath);
                    Debug.Check(removed);
                }
                else if (bCleared)
                {
                    //it has been cleared but failed to load, to remove it
                    BehaviorTrees.Remove(relativePath);
                }

                behaviac.Debug.LogError(string.Format("{0} is not loaded!", fullPath));
            }

            return bLoadResult;
        }

        public void HandleFileFormat(string fullPath, ref string ext, ref EFileFormat f)
        {
            if (f == EFileFormat.EFF_default)
            {
                // try to load the behavior in xml
                ext = ".xml";

                if (FileManager.Instance.FileExist(fullPath, ext))
                {
                    f = EFileFormat.EFF_xml;
                }
                else
                {
                    // try to load the behavior in bson
                    ext = ".bson";

                    if (FileManager.Instance.FileExist(fullPath, ext))
                    {
                        f = EFileFormat.EFF_bson;
                    }
                    else
                    {
                        // try to load the behavior in cs
                        f = EFileFormat.EFF_cs;
                    }
                }
            }
            else if (f == EFileFormat.EFF_xml || f == EFileFormat.EFF_cs)
            {
                ext = ".xml";
            }
            else if (f == EFileFormat.EFF_bson)
            {
                ext = ".bson";
            }

            //else if (f == EFileFormat.EFF_cs)
            //{
            //}
        }

        public bool Load(string relativePath)
        {
            return this.Load(relativePath, false);
        }

        public BehaviorTree LoadBehaviorTree(string relativePath)
        {
            if (BehaviorTrees.ContainsKey(relativePath))
            {
                return BehaviorTrees[relativePath];
            }
            else
            {
                bool bOk = this.Load(relativePath, true);

                if (bOk)
                {
                    return BehaviorTrees[relativePath];
                }
            }

            return null;
        }

        public bool IsValidPath(string relativePath)
        {
            Debug.Check(!string.IsNullOrEmpty(relativePath));

            if (relativePath[0] == '.' && (relativePath[1] == '/' || relativePath[1] == '\\'))
            {
                // ./dummy_bt
                return false;
            }
            else if (relativePath[0] == '/' || relativePath[0] == '\\')
            {
                // /dummy_bt
                return false;
            }

            return true;
        }

        private class BTItem_t
        {
            public List<BehaviorTreeTask> bts = new List<BehaviorTreeTask>();
            public List<Agent> agents = new List<Agent>();
        };

        private Dictionary<string, BTItem_t> m_allBehaviorTreeTasks = new Dictionary<string, BTItem_t>();

        /**
        uses the behavior tree in the cache, if not loaded yet, it loads the behavior tree first
        */

        public BehaviorTreeTask CreateBehaviorTreeTask(string relativePath)
        {
            Debug.Check(string.IsNullOrEmpty(Path.GetExtension(relativePath)), "no extention to specify");
            Debug.Check(this.IsValidPath(relativePath));

            BehaviorTree bt = null;

            if (BehaviorTrees.ContainsKey(relativePath))
            {
                bt = BehaviorTrees[relativePath];
            }
            else
            {
                bool bOk = this.Load(relativePath);

                if (bOk)
                {
                    bt = BehaviorTrees[relativePath];
                }
            }

            if (bt != null)
            {
                BehaviorTask task = bt.CreateAndInitTask();
                Debug.Check(task is BehaviorTreeTask);
                BehaviorTreeTask behaviorTreeTask = task as BehaviorTreeTask;

                if (!m_allBehaviorTreeTasks.ContainsKey(relativePath))
                {
                    m_allBehaviorTreeTasks[relativePath] = new BTItem_t();
                }

                BTItem_t btItem = m_allBehaviorTreeTasks[relativePath];

                if (!btItem.bts.Contains(behaviorTreeTask))
                {
                    btItem.bts.Add(behaviorTreeTask);
                }

                return behaviorTreeTask;
            }

            return null;
        }

        public void DestroyBehaviorTreeTask(BehaviorTreeTask behaviorTreeTask, Agent agent)
        {
            if (behaviorTreeTask != null)
            {
                if (m_allBehaviorTreeTasks.ContainsKey(behaviorTreeTask.GetName()))
                {
                    BTItem_t btItem = m_allBehaviorTreeTasks[behaviorTreeTask.GetName()];
                    btItem.bts.Remove(behaviorTreeTask);

                    if (!System.Object.ReferenceEquals(agent, null))
                    {
                        btItem.agents.Remove(agent);
                    }
                }

                BehaviorTask.DestroyTask(behaviorTreeTask);
            }
        }

        public Dictionary<string, BehaviorTree> GetBehaviorTrees()
        {
            return m_behaviortrees;
        }

        #endregion Load

        private Dictionary<string, Type> m_behaviorNodeTypes = new Dictionary<string, Type>();

        private void UnRegisterBehaviorNode()
        {
            Debug.Check(m_behaviorNodeTypes != null);
            m_behaviorNodeTypes.Clear();
        }

        public BehaviorNode CreateBehaviorNode(string className)
        {
            Type type = null;

            if (m_behaviorNodeTypes.ContainsKey(className))
            {
                type = m_behaviorNodeTypes[className];
            }
            else
            {
                string fullClassName = "behaviac." + className.Replace("::", ".");
                type = this.CallingAssembly.GetType(fullClassName, false);
                Debug.Check(type != null);

                if (type == null)
                {
                    type = this.CallingAssembly.GetType(className, false);
                }

                if (type != null)
                {
                    m_behaviorNodeTypes[className] = type;
                }
            }

            if (type != null)
            {
                object p = Activator.CreateInstance(type);
                return p as BehaviorNode;
            }

            return null;
        }


        private Assembly m_callingAssembly = null;
        private Assembly CallingAssembly
        {
            get
            {
                if (m_callingAssembly == null)
                {
                    m_callingAssembly = Assembly.GetCallingAssembly();
                }

                return m_callingAssembly;
            }
        }

        public bool ExportMetas(string xmlMetaFilePath, bool onlyExportPublicMembers)
        {
            Debug.LogWarning("deprecated, please remove calling of ExportMetas");

            return false;
        }

        public bool ExportMetas(string exportPathRelativeToWorkspace)
        {
            return ExportMetas(exportPathRelativeToWorkspace, false);
        }

    }
}
