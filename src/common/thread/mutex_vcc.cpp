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

#include "behaviac/common/thread/mutex_lock.h"

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>

namespace behaviac {
    struct Mutex::MutexImpl {
        CRITICAL_SECTION    _criticalSection;
    };

    ////////////////////////////////////////////////////////////////////////////////
    Mutex::Mutex() {
        // Be sure that the shadow is large enough
        BEHAVIAC_ASSERT(sizeof(m_buffer) >= sizeof(MutexImpl));

        // Use the shadow as memory space for the platform specific implementation
        _impl = (MutexImpl*)m_buffer;

        InitializeCriticalSection(&_impl->_criticalSection);
    }

    ////////////////////////////////////////////////////////////////////////////////
    Mutex::~Mutex() {
        DeleteCriticalSection(&_impl->_criticalSection);
    }

    ////////////////////////////////////////////////////////////////////////////////
    void Mutex::Lock() {
        EnterCriticalSection(&_impl->_criticalSection);
    }

    ////////////////////////////////////////////////////////////////////////////////
    void Mutex::Unlock() {
        LeaveCriticalSection(&_impl->_criticalSection);
    }

}//namespace behaviac

#endif//BEHAVIAC_CCDEFINE_MSVC
