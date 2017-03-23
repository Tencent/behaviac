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

#include "behaviac/common/logger/logmanager.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/agent/agent.h"
#include "behaviac/common/socket/socketconnect.h"
#include "behaviac/common/thread/mutex_lock.h"
#include "behaviac/common/member.h"
#include "behaviac/common/profiler/profiler.h"

#include <stdio.h>
#include <time.h>
#include <stdlib.h>

namespace behaviac {
    LogManager* LogManager::ms_instance = 0;

    LogManager::LogManager() : m_logFilePath(0) {
        BEHAVIAC_ASSERT(ms_instance == NULL);
        ms_instance = this;
    }

    LogManager::~LogManager() {
        for (Logs_t::iterator it = this->m_logs.begin(); it != this->m_logs.end(); ++it) {
            FILE* fp = it->second;
            fclose(fp);
        }

        ms_instance = 0;
    }

    LogManager* LogManager::GetInstance() {
        if (ms_instance == NULL) {
            //if new  an Workspace class ,then the staict variable will be set value
            LogManager* _workspace = BEHAVIAC_NEW LogManager();
            BEHAVIAC_UNUSED_VAR(_workspace);
            BEHAVIAC_ASSERT(ms_instance != NULL);
        }

        return ms_instance;
    }

    void LogManager::Cleanup() {
        BEHAVIAC_DELETE ms_instance;
    }

    void LogManager::SetLogFilePath(const char* logFilePath) {
        m_logFilePath = logFilePath;
    }

    FILE* LogManager::GetFile(const behaviac::Agent* pAgent) {
        if (Config::IsLogging()) {
            BEHAVIAC_UNUSED_VAR(pAgent);

            FILE* fp = 0;
            //int agentId = pAgent->GetId();
            int agentId = -1;

            Logs_t::iterator it = this->m_logs.find(agentId);

            if (it == this->m_logs.end()) {
                const char* pLogFile = 0;
                char buffer[64];

                if (m_logFilePath == 0) {
                    if (agentId == -1) {
                        string_snprintf(buffer, 64, "_behaviac_$_.log");
                    } else {
                        string_sprintf(buffer, "Agent_$_%03d.log", agentId);
                    }

                    pLogFile = buffer;
                } else {
                    pLogFile = m_logFilePath;
                }

                fp = fopen(pLogFile, "wt");
                this->m_logs[agentId] = fp;
            } else {
                fp = it->second;
            }

            return fp;
        }

        return 0;
    }

    void LogManager::Output(const behaviac::Agent* pAgent, const char* msg) {
        if (Config::IsLogging()) {
            FILE* fp = this->GetFile(pAgent);

            char szTime[64];

            time_t tTime = time(NULL);
            tm* ptmCurrent = localtime(&tTime);

            string_snprintf(szTime, sizeof(szTime) - 1,
                            "%.2d:%.2d:%.2d",
                            ptmCurrent->tm_hour, ptmCurrent->tm_min, ptmCurrent->tm_sec);

            //behaviac::string buffer = string_sprintf("[%s]%s\n", szTime, msg);
            char buffer[1024];
            string_sprintf(buffer, "[%s]%s", szTime, msg);

            //printf(buffer.c_str());

            if (fp) {
                behaviac::Mutex cs;
                behaviac::ScopedLock lock(cs);

                fwrite(buffer, 1, strlen(buffer), fp);

                if (behaviac::Config::IsLoggingFlush()) {
                    fflush(fp);
                }
            }
        }
    }

    //TODO:log in to buffer and asyncly write to file
    void LogManager::Log(const behaviac::Agent* pAgent, const char* btMsg, behaviac::EActionResult actionResult, behaviac::LogMode mode) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(btMsg);
        BEHAVIAC_UNUSED_VAR(actionResult);
        BEHAVIAC_UNUSED_VAR(mode);

        if (Config::IsLoggingOrSocketing()) {
            //BEHAVIAC_PROFILE("LogManager::LogAction");

            if (pAgent && pAgent->IsMasked()) {
                if (btMsg) {
                    const char* agentClassName = pAgent->GetObjectTypeName();
                    behaviac::string agentName(agentClassName);
                    agentName += "#";
                    agentName += pAgent->GetName();

                    const char* actionResultStr = "";

                    if (actionResult == EAR_success) {
                        actionResultStr = "success";
                    } else if (actionResult == EAR_failure) {
                        actionResultStr = "failure";
                    } else {
                        //although actionResult can be EAR_none or EAR_all, but, as this is the real result of an action
                        //it can only be success or failure
                        //when it is EAR_none, it is for update
                        if (actionResult == behaviac::EAR_none && mode == behaviac::ELM_tick) {
                            actionResultStr = "running";
                        } else {
                            actionResultStr = "none";
                        }
                    }

                    if (mode == behaviac::ELM_continue) {
                        //[continue]Ship::Ship_1 ships\suicide.xml->BehaviorTreeTask[0]:enter [all/success/failure] [1]
                        int count = Workspace::GetInstance()->GetActionCount(btMsg);
                        BEHAVIAC_ASSERT(count > 0);

                        char buffer[1024];
                        string_sprintf(buffer, "[continue]%s %s [%s] [%d]\n", agentName.c_str(), btMsg, actionResultStr, count);

                        this->Output(pAgent, buffer);
                        Socket::SendText(buffer);
                    } else if (mode == behaviac::ELM_breaked) {
                        //[breaked]Ship::Ship_1 ships\suicide.xml->BehaviorTreeTask[0]:enter [all/success/failure] [1]
                        int count = Workspace::GetInstance()->GetActionCount(btMsg);
                        BEHAVIAC_ASSERT(count > 0);
                        char buffer[1024];
                        string_sprintf(buffer, "[breaked]%s %s [%s] [%d]\n", agentName.c_str(), btMsg, actionResultStr, count);

                        this->Output(pAgent, buffer);
                        Socket::SendText(buffer);
                    } else if (mode == behaviac::ELM_tick) {
                        //[tick]Ship::Ship_1 ships\suicide.xml->BehaviorTreeTask[0]:enter [all/success/failure] [1]
                        //[tick]Ship::Ship_1 ships\suicide.xml->BehaviorTreeTask[0]:update [1]
                        //[tick]Ship::Ship_1 ships\suicide.xml->Selector[1]:enter [all/success/failure] [1]
                        //[tick]Ship::Ship_1 ships\suicide.xml->Selector[1]:update [1]
                        int count = Workspace::GetInstance()->UpdateActionCount(btMsg);
						BEHAVIAC_UNUSED_VAR(count);

						if (!StringUtils::Compare(actionResultStr, "running"))
						{
							char buffer[1024];
							string_sprintf(buffer, "[tick]%s %s [%s] [%d]\n", agentName.c_str(), btMsg, actionResultStr, count);

							this->Output(pAgent, buffer);
							Socket::SendText(buffer);
						}
                    } else if (mode == behaviac::ELM_jump) {
                        char buffer[1024];
                        string_sprintf(buffer, "[jump]%s %s\n", agentName.c_str(), btMsg);

                        this->Output(pAgent, buffer);
                        Socket::SendText(buffer);
                    } else if (mode == behaviac::ELM_return) {
                        char buffer[1024];
                        string_sprintf(buffer, "[return]%s %s\n", agentName.c_str(), btMsg);

                        this->Output(pAgent, buffer);
                        Socket::SendText(buffer);
                    } else {
                        BEHAVIAC_ASSERT(0);
                    }
                }
            }
        }
    }

    void LogManager::Log(const behaviac::Agent* pAgent, const char* typeName, const char* varName, const char* value) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(typeName);
        BEHAVIAC_UNUSED_VAR(varName);
        BEHAVIAC_UNUSED_VAR(value);

        if (Config::IsLoggingOrSocketing()) {
            //BEHAVIAC_PROFILE("LogManager::LogVar");

            if (pAgent && pAgent->IsMasked()) {
                const char* agentClassName = pAgent->GetObjectTypeName();
                const behaviac::string agentInstanceName = pAgent->GetName();

                //[property]WorldState::World WorldState::time->276854364
                //[property]Ship::Ship_1 GameObject::HP->100
                //[property]Ship::Ship_1 GameObject::age->0
                //[property]Ship::Ship_1 GameObject::speed->0.000000
                char buffer[1024];

                string_sprintf(buffer, "[property]%s#%s %s->%s\n", agentClassName, agentInstanceName.c_str(), varName, value);

                bool bOutput = true;
#if BEHAVIAC_USE_HTN

                if (pAgent->m_planningTop >= 0) {
                    //TODO:1 implement Log var
                    //string agentFullName = string.Format("{0}#{1}", agentClassName, agentInstanceName);

                    //behaviac::map<string, string> p = null;

                    //if (!_planningLoggedProperties.ContainsKey(agentFullName))
                    //{
                    //    p = new Dictionary<string, string>();
                    //    _planningLoggedProperties.Add(agentFullName, p);
                    //}
                    //else
                    //{
                    //    p = _planningLoggedProperties[agentFullName];
                    //}

                    //if (p.ContainsKey(varName))
                    //{
                    //    if (p[varName] == value)
                    //    {
                    //        bOutput = false;
                    //    }
                    //    else
                    //    {
                    //        p[varName] = value;
                    //    }
                    //}
                    //else
                    //{
                    //    p.Add(varName, value);
                    //}
                }

#endif//

                if (bOutput) {
                    this->Output(pAgent, buffer);
                }

                Socket::SendText(buffer);
            }
        }
    }

    void LogManager::Log(const char* format, ...) {
        if (Config::IsLoggingOrSocketing()) {
            char buffer[4096] = "";

            // make result behaviac::string
            va_list		arglist;
            va_start(arglist, format);
            vsprintf(buffer, format, arglist);
            va_end(arglist);

            this->Output(0, buffer);
            Socket::SendText(buffer);
        }
    }

    void LogManager::LogWorkspace(bool bSend, const char* format, ...) {
        if (Config::IsLoggingOrSocketing()) {
            char buffer[4096] = "";

            // make result behaviac::string
            va_list		arglist;
            va_start(arglist, format);
            vsprintf(buffer, format, arglist);
            va_end(arglist);

            this->Output(0, buffer);

            if (bSend) {
                Socket::SendWorkspace(buffer);
            }
        }
    }

    void LogManager::Log(behaviac::LogMode mode, const char* filterString, const char* format, ...) {
        if (Config::IsLoggingOrSocketing()) {
            //BEHAVIAC_PROFILE("LogManager::LogMode");

            char buffer[1024] = "";
            char target[1024] = "";

            // make result behaviac::string
            va_list		arglist;
            va_start(arglist, format);
            vsprintf(buffer, format, arglist);
            va_end(arglist);

            const char* filterStr = filterString;

            if (!filterString || *filterString == '\0') {
                filterStr = "empty";
            }

            if (mode == behaviac::ELM_tick) {
                string_sprintf(target, "[applog]%s:%s\n", filterStr, buffer);
            } else if (mode == behaviac::ELM_continue) {
                string_sprintf(target, "[continue][applog]%s:%s\n", filterStr, buffer);
            } else if (mode == behaviac::ELM_breaked) {
                //[applog]door opened
                string_sprintf(target, "[breaked][applog]%s:%s\n", filterStr, buffer);
            } else if (mode == behaviac::ELM_log) {
                string_sprintf(target, "[log]%s:%s\n", filterStr, buffer);
            } else {
                BEHAVIAC_ASSERT(0);
            }

            this->Output(0, target);
            Socket::SendText(target);
        }
    }

    void LogManager::Log(const behaviac::Agent* pAgent, const char* btMsg, long time) {
        if (Config::IsLoggingOrSocketing()) {
            if (Config::IsProfiling()) {
                //BEHAVIAC_PROFILE("LogManager::LogProfiler");

                if (pAgent && pAgent->IsMasked()) {
                    //const char* agentClassName = pAgent->GetObjectTypeName();
                    //const behaviac::string agentInstanceName = pAgent->GetName();

                    const BehaviorTreeTask* bt = pAgent ? pAgent->btgetcurrent() : 0;

                    behaviac::string btName;

                    if (bt) {
                        btName = bt->GetName();
                    } else {
                        btName = "None";
                    }

                    //[profiler]Ship::Ship_1 ships\suicide.xml->BehaviorTree[0] 0.031
                    char buffer[1024];

                    //buffer = string_sprintf("[profiler]%s::%s %s->%s %d\n", agentClassName, agentInstanceName.c_str(), btName.c_str(), btMsg, time);
                    string_sprintf(buffer, "[profiler]%s.xml->%s %ld\n", btName.c_str(), btMsg, time);

                    this->Output(pAgent, buffer);
                    Socket::SendText(buffer);
                }
            }
        }
    }

    void LogManager::LogVar(const behaviac::Agent* pAgent, const char* propertyName, const behaviac::string& typeNameStr, const behaviac::string& valueStr) {
        behaviac::string full_name = propertyName;

        const behaviac::IProperty* pProperty = pAgent->FindMemberBase(propertyName);

        if (pProperty != 0) {
            const char* classFullName = pProperty->GetClassNameString();

            char temp[1024];
            string_sprintf(temp, "%s::%s", classFullName, propertyName);
            full_name = temp;
        }

        this->Log(pAgent, typeNameStr.c_str(), full_name.c_str(), valueStr.c_str());
    }

    void LogManager::Flush(const behaviac::Agent* pAgent) {
        if (Config::IsLogging()) {
            FILE* fp = this->GetFile(pAgent);

            if (fp) {
                behaviac::Mutex cs;
                behaviac::ScopedLock lock(cs);

                fflush(fp);
            }
        }
    }

    void LogManager::Warning(const char* format, ...) {
        if (Config::IsLoggingOrSocketing()) {
            va_list		arglist;
            va_start(arglist, format);

            this->Log(behaviac::ELM_log, "warning", format, arglist);

            va_end(arglist);
        }
    }

    void LogManager::Error(const char* format, ...) {
        if (Config::IsLoggingOrSocketing()) {
            va_list		arglist;
            va_start(arglist, format);

            this->Log(behaviac::ELM_log, "error", format, arglist);

            va_end(arglist);
        }
    }
}//namespace behaviac
