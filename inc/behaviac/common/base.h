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

#ifndef _BEHAVIAC_COMMON_BASE_H_
#define _BEHAVIAC_COMMON_BASE_H_

#include "behaviac/common/config.h"
#include "behaviac/common/defines.h"
#include "behaviac/common/assert.h"

#include "behaviac/common/memory/memory.h"

#include "behaviac/common/string/stringcrc.h"
#include "behaviac/common/logger/logger.h"

#include "behaviac/common/container/string.h"
#include "behaviac/common/container/vector.h"
#include "behaviac/common/container/map.h"

#include <functional>
#include <algorithm>


namespace behaviac {
    enum EOperatorType {
        E_INVALID,
        E_ASSIGN,        // =
        E_ADD,           // +
        E_SUB,           // -
        E_MUL,           // *
        E_DIV,           // /
        E_EQUAL,         // ==
        E_NOTEQUAL,      // !=
        E_GREATER,       // >
        E_LESS,          // <
        E_GREATEREQUAL,  // >=
        E_LESSEQUAL      // <=
    };

    BEHAVIAC_API void SetMainThread();
    BEHAVIAC_API bool IsMainThread();

    typedef void(*BreakpointPromptHandler_fn)(const char* breakPointDesc);
    BEHAVIAC_API void SetBreakpointPromptHandler(BreakpointPromptHandler_fn fn);

    BEHAVIAC_API BreakpointPromptHandler_fn GetBreakpointPromptHandler();

    BEHAVIAC_API const char* GetVersionString();
}//namespace behaviac

#define ASSERT_MAIN_THREAD()  BEHAVIAC_ASSERT(behaviac::IsMainThread(), "called in a thread different from the creating thread")

#endif // _BEHAVIAC_COMMON_BASE_H_
