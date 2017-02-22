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

#ifndef _BEHAVIAC_COMMON_SOCKETCONNECT_H_
#define _BEHAVIAC_COMMON_SOCKETCONNECT_H_

#include "behaviac/common/base.h"
#include "behaviac/common/container/string.h"

#include "behaviac/common/staticassert.h"
#include "behaviac/common/thread/wrapper.h"
#include "behaviac/common/socket/socketconnect_base.h"

namespace behaviac {
    namespace Socket {
        BEHAVIAC_API bool IsConnected();

        BEHAVIAC_API void SendText(const char* text);
        BEHAVIAC_API void SendWorkspace(const char* text);

        //return true if it successfully reads data
        //return false if there is no text read
        BEHAVIAC_API bool ReadText(behaviac::string& text);

        //flush messsages
        BEHAVIAC_API void Flush();
    }
}//namespace behaviac

#endif//#if _BEHAVIAC_COMMON_SOCKETCONNECT_H_
