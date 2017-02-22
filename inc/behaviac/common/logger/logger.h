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

#ifndef _BEHAVIAC_COMMON_LOGGER_H_
#define _BEHAVIAC_COMMON_LOGGER_H_

#include "behaviac/common/config.h"

#include <stdarg.h>

enum ETagLogLevel {
    BEHAVIAC_LOG_NONE = 0x00000000,			// no display
    BEHAVIAC_LOG_INFO = 0x00000001,			// info
    BEHAVIAC_LOG_WARNING = 0x00000004,      // Warning
    BEHAVIAC_LOG_ERROR = 0x00000008,        // error
    BEHAVIAC_LOG_USER_DEFINED = 0x00000010  // Users
};

namespace behaviac {
    enum ELogOutput {
        ELOG_CONSOLE = 1,
        ELOG_FILE = 2,
        ELOG_VCOUTPUT = 4
    };

    class BEHAVIAC_API CLogger {
    public:
        static void SetLoggingLevel(ETagLogLevel NewLevel);
        static bool CanLog(ETagLogLevel level);

    public:

        //you can call CLogger::SetEnableMask(0) to disable the output
        //you can call CLogger::SetEnableMask(ELOG_CONSOLE) to enable only the console output
        static void SetEnableMask(unsigned int enableMask);

        static void Print(ETagLogLevel LogFilter, const char* format, ...);

        static void PrintLines(ETagLogLevel LogFilter, const char* pStr);
    private:
        static void Print(const char* format, ...);
        static void VPrint(const char* format, va_list& argList);
        static void VPrint(unsigned int LogFilter, const char* format, va_list& argList);
    protected:
        static void Output(unsigned int Filter, const char* pStr);

        static void OutputLine(const char* pStr);
        static void OutputDecoratedLine(unsigned int LogFilter, const char* pStr);

        static void TestInit(void);

        static bool              IsInit;
        static unsigned int      EnableMask;

        static unsigned int	  	 m_TagLogLevel;
    };
}

#define BEHAVIAC_LOGV(Level, fmt, ...)																\
    {																								\
        if(behaviac::CLogger::CanLog(Level)) ::behaviac::CLogger::Print(Level, fmt, ##__VA_ARGS__); \
    }

#define BEHAVIAC_LOGINFO(fmt, ...)		BEHAVIAC_LOGV(BEHAVIAC_LOG_INFO, fmt, ##__VA_ARGS__)
#define BEHAVIAC_LOGWARNING(fmt, ...)	BEHAVIAC_LOGV(BEHAVIAC_LOG_WARNING, fmt, ##__VA_ARGS__)
#define BEHAVIAC_LOGERROR(fmt, ...)		BEHAVIAC_LOGV(BEHAVIAC_LOG_ERROR, fmt, ##__VA_ARGS__)

#endif//_BEHAVIAC_COMMON_LOGGER_H_
