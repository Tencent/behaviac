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

#include "behaviac/common/thread/wrapper.h"
#include "behaviac/common/thread/thread.h"
#include "behaviac/common/thread/mutex_lock.h"

namespace behaviac {
#if BEHAVIAC_CCDEFINE_MSVC
    void ThreadInt::Init() {
        if (!m_inited) {
            m_inited = true;
        }
    }

    ThreadInt::ThreadInt() {
        m_inited = false;

        //this is a global, m_inited is 0 anyway
        this->Init();
    }

    ThreadInt::~ThreadInt() {
        m_inited = false;
    }

    void ThreadInt::set(long v) {
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();
        long* value = m_threadInt.find((long)threadId);
        BEHAVIAC_ASSERT(value);
        *value = v;
    }

    long ThreadInt::value() const {
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();
        long* v = m_threadInt.find((long)threadId);

        if (v) {
            long ret = *v;
            return ret;
        }

        return 0;
    }

    long ThreadInt::operator++() {
        this->Init();

        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();
        long* value = m_threadInt.find((long)threadId);

        if (value) {
            (*value)++;
            //InterlockedIncrement(value);
            return *value;

        } else {
            behaviac::ScopedLock lock(m_csMemory);
            m_threadInt.add((long)threadId, 1);
            return 1;
        }
    }

    void ThreadInt::operator--() {
        this->Init();

        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();
        long* value = m_threadInt.find((long)threadId);
        BEHAVIAC_ASSERT(value);

        if (value) {
            (*value)--;

        } else {
            //behaviac::ScopedLock lock(m_csMemory);
            BEHAVIAC_ASSERT(false);
        }
    }
#endif
}
