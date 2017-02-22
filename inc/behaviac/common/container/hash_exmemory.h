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

#ifndef _BEHAVIAC_COMMON_CONTAINER_SIMPLEHASH_H_
#define _BEHAVIAC_COMMON_CONTAINER_SIMPLEHASH_H_

#include "behaviac/common/assert.h"

template<typename T, int kHashMax = 1024>
class hash_exmemory {
public:
    typedef size_t	KeyType;
    struct HashItem {
        KeyType		key;
        T			data;
        HashItem*	next;

        HashItem() : key((KeyType) - 1), next(0)
        {}
    };

    hash_exmemory() {
        for (int i = 0; i < kHashMax; ++i) {
            m_hash[i] = 0;
        }
    }

    T* find(KeyType k) const {
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

    void add(KeyType k, HashItem* p) {
        BEHAVIAC_ASSERT(p);
        p->key = k;
        {
            uint32_t h = hash(k);
            p->next = m_hash[h];
            m_hash[h] = p;
        }
    }

    HashItem* remove(KeyType k) {
        uint32_t h = hash(k);
        HashItem** prev = &m_hash[h];
        HashItem* p = m_hash[h];

        while (p) {
            if (p->key == k) {
                *prev = p->next;
                return p;
            }

            prev = &p->next;
            p = p->next;
        }

        return 0;
    }
private:
    HashItem* m_hash[kHashMax];

    uint32_t hash(KeyType k) const {
        uint32_t h = k % kHashMax;
        return h;
    }
};

#endif//_BEHAVIAC_COMMON_CONTAINER_SIMPLEHASH_H_
