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

#ifndef _BEHAVIAC_COMMON_MEMORY_OBJECTPOOL_H_
#define _BEHAVIAC_COMMON_MEMORY_OBJECTPOOL_H_

#include <new>

#include "behaviac/common/config.h"

namespace behaviac {
    template<typename T>
    class ObjectPool {
    private:
        struct _Block {
            void* _memory;
            size_t _capacity;
            _Block* _nextBlock;

            _Block(size_t capacity) {
                BEHAVIAC_ASSERT(capacity >= 1);
                _memory = BEHAVIAC_MALLOC(_itemSize * capacity);
                BEHAVIAC_ASSERT(_memory);
                _capacity = capacity;
                _nextBlock = NULL;
            }

            ~_Block() {
                BEHAVIAC_FREE(_memory);
            }
        };

        void* _blockMemory;
        T* _firstDeleted;
        size_t _countInBlock;
        size_t _blockCapacity;
        _Block* _firstBlock;
        _Block* _lastBlock;
        size_t _maxBlockLength;

        static const size_t _itemSize = sizeof(T);

        ObjectPool(const ObjectPool&);
        void operator = (const ObjectPool&);

        void _AllocateNewBlock() {
            size_t size = _countInBlock;

            if (size >= _maxBlockLength) {
                size = _maxBlockLength;
            } else {
                size *= 2;

                if (size >= _maxBlockLength) {
                    size = _maxBlockLength;
                }
            }

            _Block* newBlock = BEHAVIAC_NEW _Block(size);
            _lastBlock->_nextBlock = newBlock;
            _lastBlock = newBlock;
            _blockMemory = newBlock->_memory;
            _countInBlock = 0;
            _blockCapacity = size;
        }

    public:
        explicit ObjectPool(size_t initialCapacity = 32, size_t maxBlockLength = 1000000) :
            _firstDeleted(NULL),
            _countInBlock(0),
            _blockCapacity(initialCapacity),
            _maxBlockLength(maxBlockLength) {
            BEHAVIAC_ASSERT(maxBlockLength >= 1);

            _firstBlock = BEHAVIAC_NEW _Block(initialCapacity);
            _blockMemory = _firstBlock->_memory;
            _lastBlock = _firstBlock;
        }

        ~ObjectPool() {
            this->Free();
        }

        T* Allocate() {
            if (_firstDeleted) {
                T* result = _firstDeleted;
                _firstDeleted = *((T**)_firstDeleted);
                new(result)T();
                return result;
            }

            if (_countInBlock >= _blockCapacity) {
                this->_AllocateNewBlock();
            }

            char* address = (char*)_blockMemory;
            address += _countInBlock * _itemSize;
            T* result = new(address)T();
            _countInBlock++;
            return result;
        }

        T* GetNextWithoutInitializing() {
            if (_firstDeleted) {
                T* result = (T*)_firstDeleted;
                _firstDeleted = *((T**)_firstDeleted);
                return result;
            }

            if (_countInBlock >= _blockCapacity) {
                this->_AllocateNewBlock();
            }

            char* address = (char*)_blockMemory;
            address += _countInBlock * _itemSize;
            _countInBlock++;
            return (T*)address;
        }

        void Delete(T* content) {
            content->~T();

            *((T**)content) = _firstDeleted;
            _firstDeleted = content;
        }

        void Free() {
            _Block* node = _firstBlock;

            while (node) {
                _Block* nextNode = node->_nextBlock;
                BEHAVIAC_DELETE(node);
                node = nextNode;
            }

            // clear it
            _firstBlock = 0;
            _countInBlock = 0;
        }

        void ForEach(void prepareSend(void*, void*), void* pData) {
            size_t countInNode = this->_countInBlock;
            _Block* node = _firstBlock;

            while (node) {
                char* pNodeStart = (char*)node->_memory;

                for (size_t i = 0; i < countInNode; ++i) {
                    char* pAddress = pNodeStart + i * _itemSize;

                    T* pT = (T*)pAddress;

                    prepareSend(pT, pData);
                }

                node = node->_nextBlock;

                if (node) {
                    countInNode = node->_capacity;
                } else {
                    countInNode = 0;
                }
            };
        }

        void DeleteWithoutDestroying(T* content) {
            *((T**)content) = _firstDeleted;
            _firstDeleted = content;
        }

        size_t GetMemoryUsage() const {
            size_t count = 0;
            const _Block* node = _firstBlock;

            while (node) {
                count += node->_capacity * _itemSize;
                node = node->_nextBlock;
            };

            return count;
        }

    };

}//end of ns


#endif //_BEHAVIAC_COMMON_MEMORY_OBJECTPOOL_H_
