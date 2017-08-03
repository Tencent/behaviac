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

namespace behaviac
{
    /**
    indicating the action result for enter/exit
    */

    [Flags]
    public enum EActionResult
    {
        EAR_none = 0,
        EAR_success = 1,
        EAR_failure = 2,
        EAR_all = EAR_success | EAR_failure
    };

    public enum LogMode
    {
        ELM_tick,
        ELM_breaked,
        ELM_continue,
        ELM_jump,
        ELM_return,

        ELM_log
    };

    public class LogManager : IDisposable
    {
        #region Singleton

        private static LogManager ms_instance = null;

        public LogManager()
        {
            Debug.Check(ms_instance == null);
            ms_instance = this;
        }

        public void Dispose()
        {
            ms_instance = null;
        }

        public static LogManager Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    LogManager instance = new LogManager();
                    Debug.Check(instance != null);
                    Debug.Check(ms_instance != null);
                }

                return ms_instance;
            }
        }

        #endregion Singleton

        /**
        by default, the log file is _behaviac_$_.log in the current path.

        you can call this function to specify where to log
        */

        public void SetLogFilePath(string logFilePath)
        {
#if !BEHAVIAC_RELEASE
            m_logFilePath = logFilePath;
#endif
        }

        //action
        public void Log(Agent pAgent, string btMsg, EActionResult actionResult, LogMode mode)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                //BEHAVIAC_PROFILE("LogManager.Instance.LogAction");
                if (!System.Object.ReferenceEquals(pAgent, null) && pAgent.IsMasked())
                {
                    if (!string.IsNullOrEmpty(btMsg))
                    {
                        string agentClassName = pAgent.GetClassTypeName();

                        agentClassName = agentClassName.Replace(".", "::");

                        string agentName = agentClassName;
                        agentName += "#";
                        agentName += pAgent.GetName();

                        string actionResultStr = "";

                        if (actionResult == EActionResult.EAR_success)
                        {
                            actionResultStr = "success";
                        }
                        else if (actionResult == EActionResult.EAR_failure)
                        {
                            actionResultStr = "failure";
                        }
                        else if (actionResult == EActionResult.EAR_all)
                        {
                            actionResultStr = "all";
                        }
                        else
                        {
                            //although actionResult can be EAR_none or EAR_all, but, as this is the real result of an action
                            //it can only be success or failure
                            //when it is EAR_none, it is for update
                            if (actionResult == behaviac.EActionResult.EAR_none && mode == behaviac.LogMode.ELM_tick)
                            {
                                actionResultStr = "running";
                            }
                            else
                            {
                                actionResultStr = "none";
                            }
                        }

                        if (mode == LogMode.ELM_continue)
                        {
                            //[continue]Ship.Ship_1 ships\suicide.xml.BehaviorTreeTask[0]:enter [all/success/failure] [1]
                            int count = Workspace.Instance.GetActionCount(btMsg);
                            Debug.Check(count > 0);
                            string buffer = string.Format("[continue]{0} {1} [{2}] [{3}]\n", agentName, btMsg, actionResultStr, count);

                            Output(pAgent, buffer);
                        }
                        else if (mode == LogMode.ELM_breaked)
                        {
                            //[breaked]Ship.Ship_1 ships\suicide.xml.BehaviorTreeTask[0]:enter [all/success/failure] [1]
                            int count = Workspace.Instance.GetActionCount(btMsg);
                            Debug.Check(count > 0);
                            string buffer = string.Format("[breaked]{0} {1} [{2}] [{3}]\n", agentName, btMsg, actionResultStr, count);

                            Output(pAgent, buffer);
                        }
                        else if (mode == LogMode.ELM_tick)
                        {
                            //[tick]Ship.Ship_1 ships\suicide.xml.BehaviorTreeTask[0]:enter [all/success/failure] [1]
                            //[tick]Ship.Ship_1 ships\suicide.xml.BehaviorTreeTask[0]:update [1]
                            //[tick]Ship.Ship_1 ships\suicide.xml.Selector[1]:enter [all/success/failure] [1]
                            //[tick]Ship.Ship_1 ships\suicide.xml.Selector[1]:update [1]
                            int count = Workspace.Instance.UpdateActionCount(btMsg);

                            if (actionResultStr != "running")
                            {
                                string buffer = string.Format("[tick]{0} {1} [{2}] [{3}]\n", agentName, btMsg, actionResultStr, count);

                                Output(pAgent, buffer);
                            }
                        }
                        else if (mode == LogMode.ELM_jump)
                        {
                            string buffer = string.Format("[jump]{0} {1}\n", agentName, btMsg);

                            Output(pAgent, buffer);
                        }
                        else if (mode == LogMode.ELM_return)
                        {
                            string buffer = string.Format("[return]{0} {1}\n", agentName, btMsg);

                            Output(pAgent, buffer);
                        }
                        else
                        {
                            Debug.Check(false);
                        }
                    }
                }
            }

#endif
        }

#if !BEHAVIAC_RELEASE
        private  Dictionary<string, Dictionary<string, string>> _planningLoggedProperties = new Dictionary<string, Dictionary<string, string>>();
#endif

        public  void PLanningClearCache()
        {
#if !BEHAVIAC_RELEASE
            _planningLoggedProperties.Clear();
#endif
        }

        //property
        public  void Log(Agent pAgent, string typeName, string varName, string value)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                //BEHAVIAC_PROFILE("LogManager.Instance.LogVar");

                if (!System.Object.ReferenceEquals(pAgent, null) && pAgent.IsMasked())
                {
                    string agentClassName = pAgent.GetClassTypeName();
                    agentClassName = agentClassName.Replace(".", "::");

                    string agentInstanceName = pAgent.GetName();

                    if (!string.IsNullOrEmpty(agentInstanceName))
                    {
                        agentInstanceName = agentInstanceName.Replace(".", "::");
                    }

                    //[property]WorldState.World WorldState.time.276854364
                    //[property]Ship.Ship_1 GameObject.HP.100
                    //[property]Ship.Ship_1 GameObject.age.0
                    //[property]Ship.Ship_1 GameObject.speed.0.000000
                    string buffer;

                    buffer = string.Format("[property]{0}#{1} {2}->{3}\n", agentClassName, agentInstanceName, varName, value);

                    bool bOutput = true;

#if BEHAVIAC_USE_HTN

                    if (pAgent.PlanningTop >= 0)
                    {
                        string agentFullName = string.Format("{0}#{1}", agentClassName, agentInstanceName);

                        Dictionary<string, string> p = null;

                        if (!_planningLoggedProperties.ContainsKey(agentFullName))
                        {
                            p = new Dictionary<string, string>();
                            _planningLoggedProperties.Add(agentFullName, p);
                        }
                        else
                        {
                            p = _planningLoggedProperties[agentFullName];
                        }

                        if (p.ContainsKey(varName))
                        {
                            if (p[varName] == value)
                            {
                                bOutput = false;
                            }
                            else
                            {
                                p[varName] = value;
                            }
                        }
                        else
                        {
                            p.Add(varName, value);
                        }
                    }

#endif//

                    if (bOutput)
                    {
                        Output(pAgent, buffer);
                    }
                }
            }

#endif
        }

        //profiler
        public  void Log(Agent pAgent, string btMsg, long time)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                if (Config.IsProfiling)
                {
                    //BEHAVIAC_PROFILE("LogManager.Instance.LogProfiler");

                    if (!System.Object.ReferenceEquals(pAgent, null) && pAgent.IsMasked())
                    {
                        //string agentClassName = pAgent.GetObjectTypeName();
                        //string agentInstanceName = pAgent.GetName();

                        BehaviorTreeTask bt = !System.Object.ReferenceEquals(pAgent, null) ? pAgent.CurrentTreeTask : null;

                        string btName;

                        if (bt != null)
                        {
                            btName = bt.GetName();
                        }
                        else
                        {
                            btName = "None";
                        }

                        //[profiler]Ship.Ship_1 ships\suicide.xml.BehaviorTree[0] 0.031
                        string buffer;

                        //buffer = FormatString("[profiler]%s.%s %s.%s %d\n", agentClassName, agentInstanceName, btName, btMsg, time);
                        buffer = string.Format("[profiler]{0}.xml->{1} {2}\n", btName, btMsg, time);

                        Output(pAgent, buffer);
                    }
                }
            }

#endif
        }

        //mode
        public  void Log(LogMode mode, string filterString, string format, params object[] args)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                //BEHAVIAC_PROFILE("LogManager.Instance.LogMode");

                // make result string
                string buffer = string.Format(format, args);

                string filterStr = filterString;

                if (string.IsNullOrEmpty(filterString))
                {
                    filterStr = "empty";
                }

                string target = "";

                if (mode == LogMode.ELM_tick)
                {
                    target = string.Format("[applog]{0}:{1}\n", filterStr, buffer);
                }
                else if (mode == LogMode.ELM_continue)
                {
                    target = string.Format("[continue][applog]{0}:{1}\n", filterStr, buffer);
                }
                else if (mode == LogMode.ELM_breaked)
                {
                    //[applog]door opened
                    target = string.Format("[breaked][applog]{0}:{1}\n", filterStr, buffer);
                }
                else if (mode == LogMode.ELM_log)
                {
                    target = string.Format("[log]{0}:{1}\n", filterStr, buffer);
                }
                else
                {
                    Debug.Check(false);
                }

                Output(null, target);
            }

#endif
        }

        public  void Log(string format, params object[] args)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                // make result string
                string buffer = string.Format(format, args);

                Output(null, buffer);
            }

#endif
        }

        public  void LogWorkspace(string format, params object[] args)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                // make result string
                string buffer = string.Format(format, args);

                Output(null, buffer);
            }

#endif
        }

        public  void LogVarValue(Agent pAgent, string name, object value)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string valueStr = StringUtils.ToString(value);
                string typeName = "";

                if (!Object.ReferenceEquals(value, null))
                {
                    typeName = Utils.GetNativeTypeName(value.GetType());
                }
                else
                {
                    typeName = "Agent";
                }

                string full_name = name;

                if (!Object.ReferenceEquals(pAgent, null))
                {
                    CMemberBase pMember = pAgent.FindMember(name);

                    if (pMember != null)
                    {
                        string classFullName = pMember.GetClassNameString().Replace(".", "::");
                        full_name = string.Format("{0}::{1}", classFullName, name);
                    }
                }

                LogManager.Instance.Log(pAgent, typeName, full_name, valueStr);
            }

#endif
        }

        public  void Warning(string format, params object[] args)
        {
            Log(LogMode.ELM_log, "warning", format, args);
        }

        public  void Error(string format, params object[] args)
        {
            Log(LogMode.ELM_log, "error", format, args);
        }

        public  void Flush(Agent pAgent)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLogging)
            {
                System.IO.StreamWriter fp = GetFile(pAgent);

                if (fp != null)
                {
                    lock (fp)
                    {
                        fp.Flush();
                    }
                }
            }

#endif
        }

        public void Close()
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLogging)
            {
                try
                {
                    var e = m_logs.Values.GetEnumerator();

                    while (e.MoveNext())
                    {
                        e.Current.Flush();
                        e.Current.Close();
                    }

                    m_logs.Clear();
                    _planningLoggedProperties.Clear();
                }
                catch
                {
                }
            }

#endif
        }

        virtual protected  System.IO.StreamWriter GetFile(Agent pAgent)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLogging)
            {
                System.IO.StreamWriter fp = null;
                //int agentId = pAgent.GetId();
                int agentId = -1;

                if (!m_logs.ContainsKey(agentId))
                {
                    string buffer;

                    if (string.IsNullOrEmpty(m_logFilePath))
                    {
                        if (agentId == -1)
                        {
                            buffer = "_behaviac_$_.log";
                        }
                        else
                        {
                            buffer = string.Format("Agent_$_{0:3}.log", agentId);
                        }

#if !BEHAVIAC_NOT_USE_UNITY
                        if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.WindowsEditor &&
                            UnityEngine.Application.platform != UnityEngine.RuntimePlatform.WindowsPlayer)
                        {
                            buffer = Path.Combine(UnityEngine.Application.persistentDataPath, buffer);
                        }
#endif
                    }
                    else
                    {
                        buffer = m_logFilePath;
                    }

                    fp = new System.IO.StreamWriter(buffer);

                    m_logs[agentId] = fp;
                }
                else
                {
                    fp = m_logs[agentId];
                }

                return fp;
            }

#endif
            return null;
        }

#if !BEHAVIAC_RELEASE
        private uint _msg_index = 0;
#endif

        private  void Output(Agent pAgent, string msg)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string txt = string.Format("[{0:00000000}]{1}", _msg_index++, msg);

                System.IO.StreamWriter fp = GetFile(pAgent);

                string szTime = DateTime.Now.ToString();

                string buffer = string.Format("[{0}]{1}", szTime, txt);

                //socket sending before logging as logging is a 'slow' process
                if (Config.IsSocketing)
                {
                    SocketUtils.SendText(txt);
                }

                if (Config.IsLogging)
                {
                    //printf(buffer);

                    if (fp != null)
                    {
                        lock (fp)
                        {
                            fp.Write(buffer);

                            if (Config.IsLoggingFlush)
                            {
                                fp.Flush();
                            }
                        }
                    }
                }
            }

#endif
        }

#if !BEHAVIAC_RELEASE
        private  Dictionary<int, System.IO.StreamWriter> m_logs = new Dictionary<int, StreamWriter>();
        private  string m_logFilePath;
#endif
    };
}//namespace behaviac
