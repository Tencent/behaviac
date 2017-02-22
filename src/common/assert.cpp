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

namespace behaviac {
    const char* FormatString() {
        return "BEHAVIAC_ASSERT failed!";
    }

    const char* FormatString(const char* fromat, ...) {
        va_list argList;
        va_start(argList, fromat);

        static char pBuffer[4096];
        string_vnprintf(pBuffer, sizeof(pBuffer), fromat, argList);

        va_end(argList);

        return pBuffer;
    }

#if	BEHAVIAC_DEBUG_DEFINED
    static bool gs_bAssertEnabled = true;

    bool IsAssertEnabled() {
        return gs_bAssertEnabled;
    }
#endif//#if	BEHAVIAC_DEBUG_DEFINED
}//namespace behaviac
