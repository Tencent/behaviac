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

#ifndef _BEHAVIAC_COMMON_CONTAINER_FIXEDHASH_H_
#define _BEHAVIAC_COMMON_CONTAINER_FIXEDHASH_H_

#include "behaviac/common/assert.h"
#include "behaviac/common/basictypes.h"
#include "behaviac/common/container/fixed_buffer.h"

template<typename T, int kBufferLength, int kHashMax = 64>
class fixed_hash {
    struct HashItem {
        uint32_t	key;
        T			data;
        HashItem*	next;

        HashItem() : key((uint32_t) - 1), next(0)
        {}
    };

    fixed_buffer<HashItem, kBufferLength> m_buffer;

    HashItem* m_hash[kHashMax];

    uint32_t hash(uint32_t k) const {
        uint32_t h = k % kHashMax;
        return h;
    }
public:
    fixed_hash() {
        for (int i = 0; i < kHashMax; ++i) {
            m_hash[i] = 0;
        }
    }

    T* find(uint32_t k) const {
        uint32_t h = hash(k);
        HashItem* p = m_hash[h];

        while (p) {
            if (p->key == k) {
                return &p->data;
            }

            p = p->next;
        }

        return 0;
    }

    void add(uint32_t k, const T& d) {
        BEHAVIAC_ASSERT(!this->find(k));
        {
            uint32_t h = hash(k);
            HashItem* p = this->m_buffer.get();
            BEHAVIAC_ASSERT(p);
            p->key = k;
            p->data = d;
            p->next = m_hash[h];
            m_hash[h] = p;
        }
    }
};

#endif//_BEHAVIAC_COMMON_CONTAINER_FIXEDHASH_H_
