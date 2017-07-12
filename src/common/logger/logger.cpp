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

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"
#include "behaviac/common/container/string.h"

#include "behaviac/common/string/stringutils.h"

#include "behaviac/common/thread/thread.h"
#include "behaviac/common/logger/logger.h"


#include <stdio.h>
#include <stdarg.h>
#include <memory.h>
#include <time.h>
#include <stdlib.h>

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#endif//BEHAVIAC_CCDEFINE_MSVC

#include <new>

#if BEHAVIAC_CCDEFINE_ANDROID
#include <jni.h>
#include <android/log.h>

#define  LOG_TAG    "libbehaviac"
#define  LOGI(str)  __android_log_print(ANDROID_LOG_INFO, LOG_TAG, "%s", str)
#else
#define  LOGI(str)  printf("%s", str)
#endif//#if BEHAVIAC_CCDEFINE_ANDROID

namespace behaviac {
    namespace internal {
        class SafeLock {
        public:
            SafeLock()
                : m_tagIsInitialized(false)
                , m_tagInitializationSignal(false) {
            }

            void Lock() {
                if (!m_tagIsInitialized) {
                    m_tagIsInitialized = m_tagInitializationSignal;
                }

                if (m_tagIsInitialized) {
                    m_mutex.Lock();
                }
            }

            void Unlock() {
                if (m_tagIsInitialized) {
                    m_mutex.Unlock();
                }
            }

            void SetInitializationSignal() {
                m_tagInitializationSignal = true;
            }

        private:
            Mutex m_mutex;
            bool m_tagIsInitialized;
            bool m_tagInitializationSignal;
        };
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    unsigned int CLogger::m_TagLogLevel = BEHAVIAC_LOG_USER_DEFINED;
    bool                            CLogger::IsInit = false;
    unsigned int                        CLogger::EnableMask = ELOG_VCOUTPUT | ELOG_FILE
#if BEHAVIAC_CCDEFINE_ANDROID
                                                              | ELOG_CONSOLE
#endif
                                                              ;

    static internal::SafeLock*		gs_lock;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void CLogger::TestInit(void) {
        if (!IsInit) {
            IsInit = true;
            static char s_buffer[sizeof(internal::SafeLock)];
            gs_lock = new(s_buffer)internal::SafeLock();
            gs_lock->SetInitializationSignal();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void CLogger::PrintLines(ETagLogLevel Filter, const char* pStr) {
        BEHAVIAC_UNUSED_VAR(Filter);

        TestInit();

        const int kMaxStringLength = 2048;
        int count = 0;
        char line[kMaxStringLength];

        while (*pStr != '\0' && *pStr != '\n') {
            line[count++] = *pStr++;
        }

        if (*pStr == '\n') {
            line[count++] = *pStr++;
        }

        line[count++] = '\0';

        BEHAVIAC_ASSERT(count < kMaxStringLength);
        count = 0;

        //the first line
        OutputDecoratedLine(Filter, line);

        while (*pStr != '\0') {
            while (*pStr != '\0' && *pStr != '\n') {
                line[count++] = *pStr++;
            }

            if (*pStr == '\n') {
                line[count++] = *pStr++;
            }

            line[count++] = '\0';

            BEHAVIAC_ASSERT(count < kMaxStringLength);
            count = 0;

            OutputLine(line);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void CLogger::Print(ETagLogLevel Filter, const char* Format, ...) {
        va_list ArgList;
        va_start(ArgList, Format);
        VPrint(Filter, Format, ArgList);
        va_end(ArgList);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void CLogger::Print(const char* Format, ...) {
        va_list ArgList;
        va_start(ArgList, Format);
        VPrint(Format, ArgList);
        va_end(ArgList);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void CLogger::VPrint(const char* Format, va_list& argList) {
        VPrint(BEHAVIAC_LOG_INFO, Format, argList);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void CLogger::VPrint(unsigned int Filter, const char* Format, va_list& argList) {
        TestInit();
        char Buffer[4096];
        {
            //weirdly, it is found, sometime it crashed in vsnprintf
            //so, to protect it for the multithreading
            gs_lock->Lock();
            string_vnprintf(Buffer, sizeof(Buffer), Format, argList);
            gs_lock->Unlock();
        }
        Output(Filter, Buffer);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void CLogger::Output(unsigned int LogFilter, const char* pStr) {
        BEHAVIAC_UNUSED_VAR(LogFilter);
        BEHAVIAC_UNUSED_VAR(pStr);

        //you can call CLogger::SetEnableMask(0) to disable the output
        if (EnableMask != 0) {
            OutputDecoratedLine(LogFilter, pStr);
        }
    }

    void CLogger::OutputLine(const char* temp) {
        gs_lock->Lock();

#if BEHAVIAC_CCDEFINE_MSVC
#if _MSC_VER >= 1500
        static BOOL s_debugger = IsDebuggerPresent();
#else
        static BOOL s_debugger = false;
#endif//#if _MSC_VER >= 1500

        if (s_debugger && (EnableMask & ELOG_VCOUTPUT)) {
            OutputDebugStringA(temp);
        }

#endif//

        //to console window
        if (EnableMask & ELOG_CONSOLE) {
            LOGI(temp);
        }

        if (EnableMask & ELOG_FILE) {
            static FILE* s_file = 0;

            if (!s_file) {
                s_file = fopen("_behaviac_$_$_.log", "wt");

                if (s_file) {
                    behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();

                    time_t tTime = time(NULL);
                    tm* ptmCurrent = localtime(&tTime);

                    char buffer[1024];
                    string_sprintf(buffer, "[behaviac][%05d][thread %04d]CREATED ON %d-%.2d-%.2d\n\n", 0, threadId, ptmCurrent->tm_year + 1900, ptmCurrent->tm_mon + 1, ptmCurrent->tm_mday);

                    fwrite(buffer, 1, strlen(buffer), s_file);
                }
            }

            if (s_file) {
                fwrite(temp, 1, strlen(temp), s_file);
                fflush(s_file);
            }
        }

        gs_lock->Unlock();
    }

    static const char* GetLogFilterString(unsigned int LogFilter) {
        const char* szStr = "NONE";

        if (LogFilter == BEHAVIAC_LOG_INFO) {
            szStr = "INFO";
        } else if (LogFilter == BEHAVIAC_LOG_WARNING) {
            szStr = "WARN";
        } else if (LogFilter == BEHAVIAC_LOG_ERROR) {
            szStr = "ERROR";
        } else {
            BEHAVIAC_ASSERT(true);
        }

        return szStr;
    }

    void CLogger::OutputDecoratedLine(unsigned int LogFilter, const char* pStr) {
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();
        time_t tTime = time(NULL);
        tm* ptmCurrent = localtime(&tTime);

        const char* LogFilterStr = GetLogFilterString(LogFilter);

        char szTime[64];
        string_snprintf(szTime, sizeof(szTime) - 1,

                        "%.2d:%.2d:%.2d",
                        ptmCurrent->tm_hour, ptmCurrent->tm_min, ptmCurrent->tm_sec);
        static int s_index = 0;
        int index = s_index++;
        const int kMaxStringLength = 2048;
        char temp[kMaxStringLength];
        string_snprintf(temp, kMaxStringLength, "[behaviac][%05d][thread %04d][%s][%s]%s", index, (int)threadId, szTime, LogFilterStr, pStr);
        temp[kMaxStringLength - 1] = '\0';

        OutputLine(temp);
    }

    void CLogger::SetEnableMask(unsigned int enableMask) {
        EnableMask = enableMask;
    }


    void CLogger::SetLoggingLevel(ETagLogLevel NewLevel) {
        m_TagLogLevel = NewLevel;
    }

    bool CLogger::CanLog(ETagLogLevel level) {
        return (unsigned int)level <= m_TagLogLevel;
    }

}
