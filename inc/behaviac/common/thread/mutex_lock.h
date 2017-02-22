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

#ifndef _BEHAVIAC_COMMON_THREAD_MUTEXLOCK_H_
#define _BEHAVIAC_COMMON_THREAD_MUTEXLOCK_H_

#include "behaviac/common/config.h"
#include "behaviac/common/defines.h"
#include "behaviac/common/basictypes.h"

#include "behaviac/common/assert.h"

namespace behaviac {
    class BEHAVIAC_API Mutex {
    public:
        Mutex();
        ~Mutex();

        void Lock();
        void Unlock();

    private:
        struct MutexImpl;
        struct MutexImpl* _impl;

        static const int kMutexBufferSize = 40;

        uint8_t        m_buffer[kMutexBufferSize];
    };

    class BEHAVIAC_API ScopedLock {
        Mutex& m_mutex_;
        ScopedLock& operator=(ScopedLock& cs);
    public:
        ScopedLock(Mutex& cs) : m_mutex_(cs) {
            m_mutex_.Lock();
        }

        ~ScopedLock() {
            m_mutex_.Unlock();
        }
    };
}//namespace behaviac

#endif //_BEHAVIAC_COMMON_THREAD_MUTEXLOCK_H_
