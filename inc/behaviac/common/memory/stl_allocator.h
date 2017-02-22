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

#ifndef _BEHAVIAC_COMMON_MEMORY_STL_ALLOCATOR_H_
#define _BEHAVIAC_COMMON_MEMORY_STL_ALLOCATOR_H_

#include "behaviac/common/config.h"
#include "behaviac/common/memory/memory.h"

namespace behaviac {
    template <typename T> class stl_allocator;
    template <> class stl_allocator<void> {
    public:
        typedef void* pointer;
        typedef const void* const_pointer;
        // reference to void members are impossible.
        typedef void value_type;
        template <class U>
        struct rebind {
            typedef stl_allocator<U> other;
        };
    };

    namespace internal {
        BEHAVIAC_FORCEINLINE void destruct(char*) {}
        BEHAVIAC_FORCEINLINE void destruct(wchar_t*) {}
        template <typename T>
        BEHAVIAC_FORCEINLINE void destruct(T* t) {
            ((void)&t);
            t->~T();
        }
    } // namespace internal

    template <typename T>
    class stl_allocator {
    public:
        typedef size_t size_type;
        typedef ptrdiff_t difference_type;
        typedef T* pointer;
        typedef const T* const_pointer;
        typedef T& reference;
        typedef const T& const_reference;
        typedef T value_type;

        template <class U>
        struct rebind {
            typedef stl_allocator<U> other;
        };

        stl_allocator() {}
        pointer address(reference x) {
            return&x;
        }
        const_pointer address(const_reference x) const {
            return &x;
        }
        pointer allocate(size_type size, stl_allocator<void>::const_pointer hint = 0) const {
            BEHAVIAC_UNUSED_VAR(hint);

            if (size == 1) {
                return static_cast<pointer>(BEHAVIAC_MALLOCALIGNED(sizeof(T), BEHAVIAC_ALIGNOF(T)));
            }

            return static_cast<pointer>(BEHAVIAC_MALLOCALIGNED(sizeof(T) * size, BEHAVIAC_ALIGNOF(T)));
        }

        // For Dinkumware (VC6SP5):
        char* _Charalloc(size_type n) {
            return static_cast<char*>(BEHAVIAC_MALLOCALIGNED(n, BEHAVIAC_ALIGNOF(T)));
        }
        // end Dinkumware

        template <class U> stl_allocator(const stl_allocator<U>&) {}
        stl_allocator(const stl_allocator<T>&) {}
        void deallocate(pointer p, size_type n) const {
            BEHAVIAC_UNUSED_VAR(n);
            BEHAVIAC_FREEALIGNED(p, BEHAVIAC_ALIGNOF(T));
        }
        void deallocate(void* p, size_type n) const {
            BEHAVIAC_UNUSED_VAR(n);
            BEHAVIAC_FREEALIGNED(p, BEHAVIAC_ALIGNOF(T));
        }
        size_type max_size() const throw() {
            return size_t(-1) / sizeof(value_type);
        }
        void construct(pointer p, const T& val) {
            ::new(static_cast<void*>(p)) T(val);
        }
        //void construct(pointer p)
        //{
        //    new(static_cast<void*>(p)) T();
        //}
        void destroy(pointer p) {
            internal::destruct(p);
        }
        //static void dump() {mem_.dump();}
    private:
    };

    template <typename T, typename U>
    BEHAVIAC_FORCEINLINE bool operator==(const stl_allocator<T>&, const stl_allocator<U>) {
        return true;
    }

    template <typename T, typename U>
    BEHAVIAC_FORCEINLINE bool operator!=(const stl_allocator<T>&, const stl_allocator<U>) {
        return false;
    }
}//namespace behaviac

#endif//_BEHAVIAC_COMMON_MEMORY_STL_ALLOCATOR_H_
