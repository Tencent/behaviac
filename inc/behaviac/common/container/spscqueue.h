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

#ifndef _BEHAVIAC_COMMON_CONTAINER_SPSCQUEUE_H_
#define _BEHAVIAC_COMMON_CONTAINER_SPSCQUEUE_H_

namespace behaviac {
    template<typename T, uint32_t TSize>
    struct SPSCQueue {
    public:
        SPSCQueue() :
            m_pushIndex(0),
            m_popIndex(0) {
        }

        uint32_t Size() const {
            const uint32_t popIndex = Load_Acquire(m_popIndex);
            const uint32_t pushIndex = Load_Acquire(m_pushIndex);
            return pushIndex - popIndex;
        }
        bool IsFull() const {
            const uint32_t c = Size();
            return c == TSize;
        }

        void Push(const T& t) {
            uint32_t pushIndex = Load_Relaxed(m_pushIndex);
            const uint32_t index = (pushIndex & (TSize - 1));
            m_buffer[index] = t;
            Store_Release(m_pushIndex, pushIndex + 1);
        }

        // NULL if queue is empty.
        T* Peek() {
            const uint32_t popIndex = Load_Relaxed(m_popIndex);
            const uint32_t pushIndex = Load_Acquire(m_pushIndex);

            if (pushIndex <= popIndex) {
                return NULL;
            }

            const uint32_t index = popIndex & (TSize - 1);
            return &m_buffer[index];
        }
        void Pop() { // use in conjuction with Peek()
            const uint32_t popIndex = Load_Relaxed(m_popIndex);
            Store_Release(m_popIndex, popIndex + 1);
        }

    private:
        SPSCQueue(const SPSCQueue&);
        SPSCQueue& operator=(const SPSCQueue&);

        static const int kCacheLineSize = 32;
        typedef uint8_t	PadBuffer[kCacheLineSize - 4];

        uint32_t		m_pushIndex;
        PadBuffer		m_padding0;
        uint32_t		m_popIndex;
        PadBuffer		m_padding1;
        T				m_buffer[TSize];
    };
} // behaviac

#endif//_BEHAVIAC_COMMON_CONTAINER_SPSCQUEUE_H_
