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

#ifndef _BEHAVIAC_COMMON_THREAD_WRAPPER_H_
#define _BEHAVIAC_COMMON_THREAD_WRAPPER_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"
#include "behaviac/common/staticassert.h"
#include "behaviac/common/container/fixed_hash.h"
#include "behaviac/common/thread/mutex_lock.h"


#if BEHAVIAC_CCDEFINE_MSVC
#include <intrin.h>

#define MemoryReadBarrier	_ReadBarrier
#define MemoryWriteBarrier	_WriteBarrier
#else
#define MemoryReadBarrier
#define MemoryWriteBarrier
#endif//BEHAVIAC_CCDEFINE_MSVC

#include <cassert>

namespace behaviac {
    typedef long			Atomic32;

    template<typename T>
    inline T Load_Relaxed(const T& v) {
        T ret = v;
        return ret;
    }

    template<typename T>
    inline T Load_Acquire(const T& v) {
        T ret = v;
#if _MSC_VER >= 1500
        MemoryReadBarrier();
#endif//#if _MSC_VER >= 1500
        return ret;
    }
    template<typename T>
    inline void Store_Release(T& dst, T v) {
#if _MSC_VER >= 1500
        MemoryWriteBarrier();
#endif//#if _MSC_VER >= 1500
        dst = v;
    }

    Atomic32 AtomicInc(volatile Atomic32& i);
    Atomic32 AtomicDec(volatile Atomic32& i);

    template<typename T>
    class ScopedInt {
        T* m_int;
    public:
        ScopedInt(T* i) : m_int(i) {
            ++(*m_int);
        }

        ~ScopedInt() {
            --(*m_int);
        }

        bool equal(long v) const {
            long vThis = m_int->value();
            return vThis == v;
        }
    };

#if BEHAVIAC_CCDEFINE_MSVC
    class BEHAVIAC_API ThreadInt {
        fixed_hash<long, 256* 2>	m_threadInt;
        behaviac::Mutex			m_csMemory;
        bool					m_inited;
    public:
        ThreadInt();
        ~ThreadInt();
        void set(long v);
        long value() const;
    private:
        void Init();
        long operator++();
        void operator--();

        friend class ScopedInt<ThreadInt>;
    };
} // behaviac

#else
    class BEHAVIAC_API ThreadInt {
        long					m_value;
        behaviac::Mutex			m_csMemory;
        bool					m_inited;
    public:
        ThreadInt();
        ~ThreadInt();
        void set(long v);
        long value() const;
    private:
        void Init();
        long operator++();
        void operator--();

        friend class ScopedInt<ThreadInt>;
    };
} // behaviac

#endif

typedef behaviac::ScopedInt<behaviac::ThreadInt> ScopedInt_t;

#endif//_BEHAVIAC_COMMON_THREAD_WRAPPER_H_
