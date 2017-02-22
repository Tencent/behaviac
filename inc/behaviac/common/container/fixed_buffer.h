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

#ifndef _BEHAVIAC_COMMON_CONTAINER_FIXEDBUFFER_H_
#define _BEHAVIAC_COMMON_CONTAINER_FIXEDBUFFER_H_

#include "behaviac/common/assert.h"

//a buffer with kLength and each item is of type T.
//no real memory alloc/free in usage
template<typename T, int kLength>
class fixed_buffer {
    int m_nextFree;
    int m_used;
    T m_buffer[kLength];

public:
    fixed_buffer() : m_nextFree(0), m_used(0) {
        BEHAVIAC_ASSERT(sizeof(T) >= sizeof(uint32_t));
        *(int*)&m_buffer[kLength - 1] = -1;

        for (int i = kLength - 2; i >= 0; --i) {
            *(int*)&m_buffer[i] = i + 1;
        }
    }

    int used() const {
        return m_used;
    }

    T* get() {
        if (m_nextFree != -1) {
            int freeIndex = m_nextFree;
            m_nextFree = *(int*)&m_buffer[m_nextFree];
            m_used++;
            return &m_buffer[freeIndex];
        }

        return 0;
    }

    void put(T* d) {
        int index = d - this->m_buffer;
        BEHAVIAC_ASSERT(index >= 0 && index < kLength);
        *(int*)d = m_nextFree;
        m_nextFree = index;
        BEHAVIAC_ASSERT(m_used <= kLength && m_used > 0);
        m_used--;
    }
};

#endif//_BEHAVIAC_COMMON_CONTAINER_FIXEDBUFFER_H_
