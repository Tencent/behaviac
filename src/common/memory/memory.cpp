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

#include "behaviac/common/config.h"
#include "behaviac/common/memory/memory.h"
#include "behaviac/common/thread/mutex_lock.h"
#include "behaviac/common/container/hash_exmemory.h"
#include "behaviac/common/thread/wrapper.h"

#if !BEHAVIAC_RELEASE && BEHAVIAC_CCDEFINE_MSVC
#define BEHAVIAC_DEBUG_MEMORY_STATS		1
#endif//

namespace behaviac {
    class BEHAVIAC_API PtrSizeRegister {
    public:
        PtrSizeRegister() {
        }
        virtual ~PtrSizeRegister() {
        }

        virtual void RegisterPtrSize(const void* ptr, size_t bytes) = 0;
        virtual void UnRegisterPtr(const void* ptr) = 0;

        virtual size_t GetAllocatedSize() const = 0;
        virtual size_t GetMemoryUsage() const = 0;

        static PtrSizeRegister* Create();
        static void Destroy(PtrSizeRegister*);
    };

#if BEHAVIAC_DEBUG_MEMORY_STATS
    static ThreadInt*			gs_threadInt;
    static ThreadInt* GetThreadInt() {
        if (!gs_threadInt) {
            gs_threadInt = new ThreadInt;
        }

        return gs_threadInt;
    }

    static void ClearupThreadInt() {
        delete gs_threadInt;
        gs_threadInt = 0;
    }
#endif//BEHAVIAC_DEBUG_MEMORY_STATS

    ///Default Allocator used internally in Tag.   It can also be used externally using behaviac::GetDefaultMemoryAllocator();
    class MemDefaultAllocator : public IMemAllocator {
    public:
        MemDefaultAllocator();
        virtual ~MemDefaultAllocator();

        virtual size_t GetMaxAllocationSize(void) const;
        virtual size_t GetAllocatedSize() const;
        virtual void* Alloc(size_t size, const char* tag, const char* pFile, unsigned int Line);
        virtual void* Realloc(void* pOldPtr, size_t size, const char* tag, const char* pFile, unsigned int Line);
        virtual void Free(void* pData, const char* tag, const char* pFile, unsigned int Line);
        virtual void* AllocAligned(size_t size, size_t alignment, const char* tag, const char* pFile, unsigned int Line);
        virtual void* ReallocAligned(void* pOldPtr, size_t size, size_t alignment, const char* tag, const char* pFile, unsigned int Line);
        virtual void FreeAligned(void* pData, size_t alignment, const char* tag, const char* pFile, unsigned int Line);
        virtual const char* GetName(void) const;

    private:
#if BEHAVIAC_DEBUG_MEMORY_STATS
        const bool			m_bEnablePtrSizeRegister;
        PtrSizeRegister*	m_PtrSizeRegister;

        PtrSizeRegister& GetPtrSizeRegister() {
            BEHAVIAC_ASSERT(m_PtrSizeRegister && m_bEnablePtrSizeRegister);
            return *m_PtrSizeRegister;
        }

        const PtrSizeRegister& GetPtrSizeRegister() const {
            BEHAVIAC_ASSERT(m_PtrSizeRegister && m_bEnablePtrSizeRegister);
            return *m_PtrSizeRegister;
        }
#endif

        MemDefaultAllocator(const MemDefaultAllocator&);
        MemDefaultAllocator& operator=(const MemDefaultAllocator&);
    };

    MemDefaultAllocator::MemDefaultAllocator()
#if BEHAVIAC_DEBUG_MEMORY_STATS
        : m_PtrSizeRegister(0), m_bEnablePtrSizeRegister(true)
#endif
    {
#if BEHAVIAC_DEBUG_MEMORY_STATS

        if (m_bEnablePtrSizeRegister) {
            ScopedInt_t scopedInt(GetThreadInt());

            if (scopedInt.equal(1)) {
                m_PtrSizeRegister = PtrSizeRegister::Create();
            }
        }

#endif
    }

    MemDefaultAllocator::~MemDefaultAllocator() {
#if BEHAVIAC_DEBUG_MEMORY_STATS
        BEHAVIAC_ASSERT(!m_bEnablePtrSizeRegister || m_PtrSizeRegister);
        {
            ScopedInt_t scopedInt(GetThreadInt());

            if (scopedInt.equal(1)) {
                PtrSizeRegister::Destroy(m_PtrSizeRegister);
            }
        }

        m_PtrSizeRegister = 0;
#endif
    }

    size_t MemDefaultAllocator::GetMaxAllocationSize(void) const {
        return size_t(-1);
    }

    size_t MemDefaultAllocator::GetAllocatedSize() const {
#if BEHAVIAC_DEBUG_MEMORY_STATS

        if (m_bEnablePtrSizeRegister) {
            return GetPtrSizeRegister().GetAllocatedSize();
        }

#endif
        return 0;
    }

    void* MemDefaultAllocator::Alloc(size_t size, const char* tag, const char* pFile, unsigned int Line) {
        BEHAVIAC_UNUSED_VAR(tag);
        BEHAVIAC_UNUSED_VAR(pFile);
        BEHAVIAC_UNUSED_VAR(Line);

        void* p = malloc(size);
#if BEHAVIAC_DEBUG_MEMORY_STATS

        if (m_bEnablePtrSizeRegister) {
            ScopedInt_t scopedInt(GetThreadInt());

            if (scopedInt.equal(1)) {
                GetPtrSizeRegister().RegisterPtrSize(p, size);
            }
        }

#endif
        return p;
    }

    void* MemDefaultAllocator::Realloc(void* pOldPtr, size_t size, const char* tag, const char* pFile, unsigned int Line) {
        BEHAVIAC_UNUSED_VAR(tag);
        BEHAVIAC_UNUSED_VAR(pFile);
        BEHAVIAC_UNUSED_VAR(Line);
#if BEHAVIAC_DEBUG_MEMORY_STATS

        if (pOldPtr && m_bEnablePtrSizeRegister) {
            ScopedInt_t scopedInt(GetThreadInt());

            if (scopedInt.equal(1)) {
                GetPtrSizeRegister().UnRegisterPtr(pOldPtr);
            }
        }

#endif
        void* p = realloc(pOldPtr, size);
#if BEHAVIAC_DEBUG_MEMORY_STATS

        if (m_bEnablePtrSizeRegister) {
            ScopedInt_t scopedInt(GetThreadInt());

            if (scopedInt.equal(1)) {
                GetPtrSizeRegister().RegisterPtrSize(p, size);
            }
        }

#endif
        return p;
    }

    void MemDefaultAllocator::Free(void* pData, const char* tag, const char* pFile, unsigned int Line) {
        BEHAVIAC_UNUSED_VAR(tag);
        BEHAVIAC_UNUSED_VAR(pFile);
        BEHAVIAC_UNUSED_VAR(Line);

        if (pData) {
#if BEHAVIAC_DEBUG_MEMORY_STATS

            if (m_bEnablePtrSizeRegister) {
                ScopedInt_t scopedInt(GetThreadInt());

                if (scopedInt.equal(1)) {
                    GetPtrSizeRegister().UnRegisterPtr(pData);
                }
            }

#endif
            free(pData);
        }
    }

    void* MemDefaultAllocator::AllocAligned(size_t size, size_t alignment, const char* tag, const char* pFile, unsigned int Line) {
        BEHAVIAC_UNUSED_VAR(size);
        BEHAVIAC_UNUSED_VAR(alignment);
        BEHAVIAC_UNUSED_VAR(tag);
        BEHAVIAC_UNUSED_VAR(pFile);
        BEHAVIAC_UNUSED_VAR(Line);

#if BEHAVIAC_CCDEFINE_MSVC
        void* p = _aligned_malloc(size, alignment);
#elif BEHAVIAC_CCDEFINE_APPLE
        void* p = 0;

        if (alignment < sizeof(void*)) {
            alignment = sizeof(void*);
        }

        int errCode = posix_memalign(&p, alignment, size);

        if (errCode) {
            BEHAVIAC_ASSERT(false, "The alignment argument was not a power of two, or was not a multiple of sizeof(void*)"
                            "or there was insufficient memory to fulfill the allocate request\n");
        }

#else
        void* p = memalign(alignment, size);
#endif//BEHAVIAC_CCDEFINE_MSVC
#if BEHAVIAC_DEBUG_MEMORY_STATS

        if (m_bEnablePtrSizeRegister) {
            ScopedInt_t scopedInt(GetThreadInt());

            if (scopedInt.equal(1)) {
                GetPtrSizeRegister().RegisterPtrSize(p, size);
            }
        }

#endif
        return p;
    }

    void* MemDefaultAllocator::ReallocAligned(void* pOldPtr, size_t size, size_t alignment, const char* tag, const char* pFile, unsigned int Line) {
        BEHAVIAC_UNUSED_VAR(alignment);
        BEHAVIAC_UNUSED_VAR(tag);
        BEHAVIAC_UNUSED_VAR(pFile);
        BEHAVIAC_UNUSED_VAR(Line);

        if (pOldPtr) {
#if BEHAVIAC_DEBUG_MEMORY_STATS

            if (m_bEnablePtrSizeRegister) {
                ScopedInt_t scopedInt(GetThreadInt());

                if (scopedInt.equal(1)) {
                    GetPtrSizeRegister().UnRegisterPtr(pOldPtr);
                }
            }

#endif
        }

#if BEHAVIAC_CCDEFINE_MSVC
        void* p = _aligned_realloc(pOldPtr, size, alignment);
#else
        void* p = realloc(pOldPtr, size);
#endif//BEHAVIAC_CCDEFINE_MSVC
#if BEHAVIAC_DEBUG_MEMORY_STATS

        if (m_bEnablePtrSizeRegister) {
            ScopedInt_t scopedInt(GetThreadInt());

            if (scopedInt.equal(1)) {
                GetPtrSizeRegister().RegisterPtrSize(p, size);
            }
        }

#endif
        return p;
    }

    void MemDefaultAllocator::FreeAligned(void* pData, size_t alignment, const char* tag, const char* pFile, unsigned int Line) {
        BEHAVIAC_UNUSED_VAR(tag);
        BEHAVIAC_UNUSED_VAR(pFile);
        BEHAVIAC_UNUSED_VAR(Line);
        BEHAVIAC_UNUSED_VAR(alignment);

        if (pData) {
#if BEHAVIAC_DEBUG_MEMORY_STATS

            if (m_bEnablePtrSizeRegister) {
                ScopedInt_t scopedInt(GetThreadInt());

                if (scopedInt.equal(1)) {
                    GetPtrSizeRegister().UnRegisterPtr(pData);
                }
            }

#endif
#if BEHAVIAC_CCDEFINE_MSVC
            return _aligned_free(pData);
#else
            return free(pData);
#endif//#if BEHAVIAC_CCDEFINE_MSVC
        }
    }

    const char* MemDefaultAllocator::GetName(void) const {
        return "";
    }

    static IMemAllocator* gs_MemDefaultAllocator = 0;

    IMemAllocator& GetDefaultMemoryAllocator(void) {
        if (!gs_MemDefaultAllocator) {
            gs_MemDefaultAllocator = new MemDefaultAllocator;
        }

        BEHAVIAC_ASSERT(gs_MemDefaultAllocator);
        return *gs_MemDefaultAllocator;
    }

    void CleanupDefaultMemoryAllocator(void) {
        delete gs_MemDefaultAllocator;
        gs_MemDefaultAllocator = 0;
#if BEHAVIAC_DEBUG_MEMORY_STATS
        ClearupThreadInt();
#endif
    }

#if BEHAVIAC_DEBUG_MEMORY_STATS
    typedef hash_exmemory<size_t> HashTable_t;
    typedef HashTable_t::HashItem HashItem;

    class PtrSizePool : public ObjectPool<HashItem> {
    public:
        PtrSizePool(uint32_t objectCountPerSegment) : ObjectPool(objectCountPerSegment)
        {}

        virtual ~PtrSizePool() {
        }

    };

    class PtrSizeRegister_ : public PtrSizeRegister {
    private:
        size_t			m_allocated_size;

        HashTable_t		m_hashtable;
        PtrSizePool		m_pool;
        behaviac::Mutex	m_cs;

        size_t GetBlockSize(const void* ptr);
    public:
        PtrSizeRegister_();
        virtual ~PtrSizeRegister_();

        virtual void RegisterPtrSize(const void* ptr, size_t bytes);
        virtual void UnRegisterPtr(const void* ptr);

        virtual size_t GetAllocatedSize() const;
        virtual size_t GetMemoryUsage() const;
    };

    PtrSizeRegister* PtrSizeRegister::Create() {
        PtrSizeRegister* p = new PtrSizeRegister_;

        return p;
    }

    void PtrSizeRegister::Destroy(PtrSizeRegister* p) {
        delete p;
    }

    size_t PtrSizeRegister_::GetBlockSize(const void* ptr) {
        {
            size_t* size = this->m_hashtable.find((size_t)ptr);

            //BEHAVIAC_ASSERT(size);
            if (size) {
                return *size;
            }
        }

        return 0;
    }

    PtrSizeRegister_::PtrSizeRegister_() : m_pool(1024 * 16), m_allocated_size(0) {
    }

    PtrSizeRegister_::~PtrSizeRegister_() {
    }

    void PtrSizeRegister_::RegisterPtrSize(const void* ptr, size_t bytes) {
        behaviac::ScopedLock locker(m_cs);

        size_t* pSize = this->m_hashtable.find((size_t)ptr);

        if (!pSize) {
            HashItem* pP = this->m_pool.Allocate();

            BEHAVIAC_ASSERT(pP);

            if (pP) {
                pP->data = bytes;

                this->m_hashtable.add((size_t)ptr, pP);
            }

            m_allocated_size += bytes;

        } else {
            m_allocated_size += (bytes - *pSize);

            *pSize = bytes;
        }
    }

    void PtrSizeRegister_::UnRegisterPtr(const void* ptr) {
        behaviac::ScopedLock locker(m_cs);

        size_t bytes = GetBlockSize(ptr);
        HashItem* pP = this->m_hashtable.remove((size_t)ptr);

        BEHAVIAC_ASSERT(pP);

        if (pP) {
            this->m_pool.Delete(pP);
        }

        m_allocated_size -= bytes;
    }

    size_t PtrSizeRegister_::GetAllocatedSize() const {
        return m_allocated_size;
    }

    size_t PtrSizeRegister_::GetMemoryUsage() const {
        //size_t memUsage = sizeof (*this) + m_pool.GetMemoryUsage() + m_hashtable.GetMemoryUsage();
        size_t memUsage = sizeof(*this) + this->m_pool.GetMemoryUsage();

        return memUsage;
    }
#endif//#if BEHAVIAC_DEBUG_MEMORY_STATS
}

namespace behaviac {
    static IMemAllocator* gs_pMemoryAllocator;
    IMemAllocator& GetMemoryAllocator() {
        if (gs_pMemoryAllocator) {
            return *gs_pMemoryAllocator;
        }

        return behaviac::GetDefaultMemoryAllocator();
    }

    void SetMemoryAllocator(IMemAllocator& allocator) {
        gs_pMemoryAllocator = &allocator;
    }

	const BehaviacOperatorNewType_t& BehaviacOperatorNewType_t::GetInstance() {
		static BehaviacOperatorNewType_t DEFAULT_TYPE;
        return DEFAULT_TYPE;
    }

	const BehaviacOperatorNewArrayType_t& BehaviacOperatorNewArrayType_t::GetInstance() {
		static BehaviacOperatorNewArrayType_t DEFAULT_TYPE;
        return DEFAULT_TYPE;
    }
}
