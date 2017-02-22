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

#ifndef _BEHAVIAC_COMMON_MEMORY_H_
#define _BEHAVIAC_COMMON_MEMORY_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"

#if BEHAVIAC_CCDEFINE_APPLE
//#include <sys/malloc.h>
#else
#include <malloc.h>
#endif//BEHAVIAC_CCDEFINE_APPLE

#include <memory.h>
//#include <string.h>
#include <stdlib.h>

#include "behaviac/common/config.h"
#include "behaviac/common/thread/mutex_lock.h"


// http://msdn.microsoft.com/en-us/library/x98tx3cf.aspx
//#define _CRTDBG_MAP_ALLOC
//#include <stdlib.h>
//#include <crtdbg.h>

//_CrtSetDbgFlag ( _CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF );
//_CrtDumpMemoryLeaks();

#if _MSC_VER >= 1400
#include <crtdbg.h>
#endif//#if _MSC_VER >= 1400

//#if BEHAVIAC_CCDEFINE_64BITS
//	#define BEHAVIAC_DEFAULT_ALIGN	8
//#else
//	#define BEHAVIAC_DEFAULT_ALIGN	4
//#endif
#define BEHAVIAC_DEFAULT_ALIGN	8

#if BEHAVIAC_RELEASE
#define __BEHAVIAC_FILE__ "behaviac_release_file"
#define __BEHAVIAC_LINE__ 0
#else
#define __BEHAVIAC_FILE__ __FILE__
#define __BEHAVIAC_LINE__ __LINE__
#endif

namespace behaviac {
    class BEHAVIAC_API IMemAllocator {
    public:
        IMemAllocator() {
        }

        virtual ~IMemAllocator() {
        }

        virtual void* Alloc(size_t size, const char* tag, const char* pFile, unsigned int Line) = 0;
        virtual void* Realloc(void* pOldPtr, size_t size, const char* tag, const char* pFile, unsigned int Line) = 0;
        virtual void Free(void* pData, const char* tag, const char* pFile, unsigned int Line) = 0;
        virtual void* AllocAligned(size_t size, size_t alignment, const char* tag, const char* pFile, unsigned int Line) = 0;
        virtual void* ReallocAligned(void* pOldPtr, size_t size, size_t alignment, const char* tag, const char* pFile, unsigned int Line) = 0;
        virtual void FreeAligned(void* pData, size_t alignment, const char* tag, const char* pFile, unsigned int Line) = 0;
        virtual const char* GetName(void) const {
            return "IMemAllocator";
        }

        virtual size_t GetMaxAllocationSize(void) const {
            return (size_t) - 1;
        }

        virtual size_t GetAllocatedSize() const = 0;
    };

    BEHAVIAC_API IMemAllocator& GetDefaultMemoryAllocator();
    BEHAVIAC_API void CleanupDefaultMemoryAllocator();

}

///////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////
namespace behaviac {
    BEHAVIAC_API IMemAllocator& GetMemoryAllocator();

    template<typename T>
    behaviac::IMemAllocator* GetAllocator() {
        behaviac::IMemAllocator* pAllocator = &behaviac::GetMemoryAllocator();
        return pAllocator;
    }

    //you can call this to override/customize the memory allocator
    BEHAVIAC_API void SetMemoryAllocator(IMemAllocator& allocator);

    namespace internal {
        BEHAVIAC_FORCEINLINE void* _MemoryHelperAlloc(behaviac::IMemAllocator* pAllocator, size_t TotalSize, unsigned int alignment, const char* tag, const char* pFile, unsigned int Line) {
            BEHAVIAC_UNUSED_VAR(pFile);
            BEHAVIAC_UNUSED_VAR(Line);
            BEHAVIAC_UNUSED_VAR(tag);
            void* ptr = 0;

            if (alignment > BEHAVIAC_DEFAULT_ALIGN) {
                ptr = pAllocator->AllocAligned(TotalSize, alignment, tag, pFile, Line);
            } else {
                ptr = pAllocator->Alloc(TotalSize, tag, pFile, Line);
            }

            return ptr;
        }

        BEHAVIAC_FORCEINLINE void _MemoryHelperFree(behaviac::IMemAllocator* pAllocator, void* pObj, int alignment, const char* tag, const char* pFile, unsigned int Line) {
            BEHAVIAC_UNUSED_VAR(tag);
            BEHAVIAC_UNUSED_VAR(pFile);
            BEHAVIAC_UNUSED_VAR(Line);

            if (alignment > BEHAVIAC_DEFAULT_ALIGN) {
                pAllocator->FreeAligned(pObj, alignment, tag, pFile, Line);
            } else {
                pAllocator->Free(pObj, tag, pFile, Line);
            }
        }

        template<typename T>
        BEHAVIAC_FORCEINLINE void _MemoryHelperDelete(T* pObj, const char* tag, const char* pFile, unsigned int Line) {
            BEHAVIAC_UNUSED_VAR(tag);
            BEHAVIAC_UNUSED_VAR(pFile);
            BEHAVIAC_UNUSED_VAR(Line);

            if (pObj) {
                pObj->~T();

                behaviac::IMemAllocator* pAllocator = behaviac::GetAllocator<T>();
                behaviac::internal::_MemoryHelperFree(pAllocator, pObj, BEHAVIAC_ALIGNOF(T), tag, pFile, Line);
            }
        }

        BEHAVIAC_FORCEINLINE void* _MemoryHelperAllocAlignment(behaviac::IMemAllocator* pAllocator, size_t TotalSize, unsigned int alignment, const char* tag, const char* pFile, unsigned int Line) {
            BEHAVIAC_UNUSED_VAR(pFile);
            BEHAVIAC_UNUSED_VAR(Line);
            BEHAVIAC_UNUSED_VAR(tag);

            void* ptr = pAllocator->AllocAligned(TotalSize, alignment, tag, pFile, Line);

            return ptr;
        }

        BEHAVIAC_FORCEINLINE void _MemoryHelperFreeAlignment(behaviac::IMemAllocator* pAllocator, void* pObj, int alignment, const char* tag, const char* pFile, unsigned int Line) {
            BEHAVIAC_UNUSED_VAR(tag);
            BEHAVIAC_UNUSED_VAR(pFile);
            BEHAVIAC_UNUSED_VAR(Line);

            pAllocator->FreeAligned(pObj, alignment, tag, pFile, Line);
        }

        template<typename T>
        BEHAVIAC_FORCEINLINE void _MemoryHelperDeleteAlignment(T* pObj, int align, const char* tag, const char* pFile, unsigned int Line) {
            BEHAVIAC_UNUSED_VAR(tag);
            BEHAVIAC_UNUSED_VAR(pFile);
            BEHAVIAC_UNUSED_VAR(Line);

            if (pObj) {
                pObj->~T();

                behaviac::IMemAllocator* pAllocator = behaviac::GetAllocator<T>();
                behaviac::internal::_MemoryHelperFreeAlignment(pAllocator, pObj, align, tag, pFile, Line);
            }
        }

        template<typename T>
        struct _MemoryHelperArrayAllocator {
            static BEHAVIAC_FORCEINLINE T* AllocArray(behaviac::IMemAllocator* pAllocator, unsigned int Count, const char* tag, const char* pFile, unsigned int Line) {
                int alignment = BEHAVIAC_ALIGNOF(T);

                if (alignment < BEHAVIAC_DEFAULT_ALIGN) {
                    alignment = BEHAVIAC_DEFAULT_ALIGN;
                }

                BEHAVIAC_ASSERT(alignment >= BEHAVIAC_DEFAULT_ALIGN);
                unsigned int totalSize = BEHAVIAC_ROUND(sizeof(int32_t), alignment) + sizeof(T) * Count;
                void* pData = behaviac::internal::_MemoryHelperAlloc(pAllocator, totalSize, alignment, tag, pFile, Line);
                BEHAVIAC_ASSERT(pData);
                BEHAVIAC_ASSERT(BEHAVIAC_ALIGNED(pData, alignment));
                int32_t* pCount = (int32_t*)pData;
                BEHAVIAC_ASSERT(BEHAVIAC_ALIGNED(pCount, BEHAVIAC_DEFAULT_ALIGN));
                BEHAVIAC_ASSERT(alignment >= BEHAVIAC_DEFAULT_ALIGN);
                T* pArray = (T*)BEHAVIAC_ROUND(pCount + 1, alignment);
                *(((int32_t*)pArray) - 1) = (int32_t)Count;
                BEHAVIAC_ASSERT(BEHAVIAC_ALIGNED(pArray, BEHAVIAC_DEFAULT_ALIGN));

                for (int i = 0; i < (int)Count; i++) {
                    new(&pArray[i]) T;
                }

                return (T*)pArray;
            }
        };

        template<typename T>
        struct _MemoryHelperArrayAllocator<T*> {
            static BEHAVIAC_FORCEINLINE T** AllocArray(behaviac::IMemAllocator* pAllocator, unsigned int Count, const char* tag, const char* pFile, unsigned int Line) {
                return (T**)behaviac::internal::_MemoryHelperAlloc(pAllocator, sizeof(T*) * Count, BEHAVIAC_ALIGNOF(T*), tag, pFile, Line);
            }
        };

        template<typename T>
        BEHAVIAC_FORCEINLINE T* _MemoryHelperAllocArray(behaviac::IMemAllocator* pAllocator, unsigned int Count, const char* tag, const char* pFile, unsigned int Line) {
            return _MemoryHelperArrayAllocator<T>::AllocArray(pAllocator, Count, tag, pFile, Line);
        }

        template<typename T>
        struct _MemoryHelperArrayDeleter {
            static BEHAVIAC_FORCEINLINE void Delete(behaviac::IMemAllocator* pAllocator, T* pArray, const char* tag, const char* pFile, unsigned int Line) {
                BEHAVIAC_UNUSED_VAR(tag);
                BEHAVIAC_UNUSED_VAR(pFile);
                BEHAVIAC_UNUSED_VAR(Line);

                if (!pArray) {
                    return;
                }

                int alignment = BEHAVIAC_ALIGNOF(T);

                if (alignment < BEHAVIAC_DEFAULT_ALIGN) {
                    alignment = BEHAVIAC_DEFAULT_ALIGN;
                }

                BEHAVIAC_ASSERT(alignment >= BEHAVIAC_DEFAULT_ALIGN);
                BEHAVIAC_ASSERT(BEHAVIAC_ALIGNED(pArray, alignment));
                int32_t* pCount = ((int32_t*)pArray) - 1;
                int count = (int)(*pCount);

                for (int i = count - 1; i >= 0; i--) {
                    pArray[i].~T();
                }

                void* ptrOriginal = (void*)BEHAVIAC_FLOOR(pCount, alignment);
                behaviac::internal::_MemoryHelperFree(pAllocator, ptrOriginal, alignment, tag, pFile, Line);
            }
        };

        template<typename T>
        struct _MemoryHelperArrayDeleter<T*> {
            static BEHAVIAC_FORCEINLINE void Delete(behaviac::IMemAllocator* pAllocator, T** pArray, const char* tag, const char* pFilename, unsigned int LineNo) {
                BEHAVIAC_UNUSED_VAR(tag);
                BEHAVIAC_UNUSED_VAR(pFilename);
                BEHAVIAC_UNUSED_VAR(LineNo);

                behaviac::internal::_MemoryHelperFree(pAllocator, pArray, BEHAVIAC_ALIGNOF(T*), tag, pFilename, LineNo);
            }
        };

        template<typename T>
        BEHAVIAC_FORCEINLINE void _MemoryHelperDeleteArray(T* pArray, const char* tag, const char* pFilename, unsigned int LineNo) {
            behaviac::IMemAllocator* pAllocator = behaviac::GetAllocator<T>();
            _MemoryHelperArrayDeleter<T>::Delete(pAllocator, pArray, tag, pFilename, LineNo);
        }

        template<class T, bool hasDtor>
        struct _MemoryHelperArrayDeleterSystem {
            static BEHAVIAC_FORCEINLINE void Delete(T* pObj, size_t align, const char* tag, const char* pFile, unsigned int Line) {
                BEHAVIAC_UNUSED_VAR(tag);
                BEHAVIAC_UNUSED_VAR(pFile);
                BEHAVIAC_UNUSED_VAR(Line);

                if (pObj) {
                    behaviac::IMemAllocator* pAllocator = behaviac::GetAllocator<T>();
                    behaviac::internal::_MemoryHelperFreeAlignment(pAllocator, pObj, align, tag, pFile, Line);
                }
            }
        };

        template<class T>
        struct _MemoryHelperArrayDeleterSystem<T, true> {
            static BEHAVIAC_FORCEINLINE void Delete(T* pObj, size_t align, const char* tag, const char* pFile, unsigned int Line) {
                BEHAVIAC_UNUSED_VAR(tag);
                BEHAVIAC_UNUSED_VAR(pFile);
                BEHAVIAC_UNUSED_VAR(Line);

                if (pObj) {
                    void* ptr = (((int*)pObj) - 1);
                    int* pCount = (int*)BEHAVIAC_FLOOR(ptr, align);
                    int count = *pCount;

                    for (int i = 0; i < count; ++i, ++pObj) {
                        pObj->~T();
                    }

                    behaviac::IMemAllocator* pAllocator = behaviac::GetAllocator<T>();
                    behaviac::internal::_MemoryHelperFreeAlignment(pAllocator, pCount, align, tag, pFile, Line);
                }
            }
        };
    } // namespace internal

    struct BehaviacOperatorNewType_t {
        int m_dummyMember;
        static BEHAVIAC_API const BehaviacOperatorNewType_t& GetInstance();
    };

    struct BehaviacOperatorNewArrayType_t {
        int m_dummyMember;

        static BEHAVIAC_API const BehaviacOperatorNewArrayType_t& GetInstance();
    };

    template <typename T>
    inline void operator /(const BehaviacOperatorNewType_t& /*type*/, T* object) {
        int align = BEHAVIAC_ALIGNOF(T);

        if (align < BEHAVIAC_DEFAULT_ALIGN) {
            align = BEHAVIAC_DEFAULT_ALIGN;
        }

        BEHAVIAC_ASSERT(align == BEHAVIAC_DEFAULT_ALIGN, "please include BEHAVIAC_DECLARE_MEMORY_OPERATORS in your type or use BEHAVIAC_G_*!");

        behaviac::internal::_MemoryHelperDeleteAlignment(object, align, "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__);
    }

    template <typename T>
    inline void operator /(const BehaviacOperatorNewArrayType_t& /*type*/, T* object) {
        if (object) {
            //behaviac::internal::_MemoryHelperArrayDeleterSystem<T, behaviac::internal::has_destructor<T>::value>::Delete(object, BEHAVIAC_ALIGNOF(T), "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__);
            behaviac::internal::_MemoryHelperFreeAlignment(&behaviac::GetMemoryAllocator(), object, BEHAVIAC_DEFAULT_ALIGN, "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__);
        }
    }
}//namespace behaviac

BEHAVIAC_FORCEINLINE void* operator new(size_t size, const behaviac::BehaviacOperatorNewType_t& /*type*/, const char* tag, const char* file, int line) {
    return behaviac::internal::_MemoryHelperAllocAlignment(&behaviac::GetMemoryAllocator(), size, BEHAVIAC_DEFAULT_ALIGN, tag, file, line);
}

BEHAVIAC_FORCEINLINE void operator delete(void* ptr, const behaviac::BehaviacOperatorNewType_t& /*type*/, const char* tag, const char* file, int line) {
    behaviac::internal::_MemoryHelperFreeAlignment(&behaviac::GetMemoryAllocator(), ptr, BEHAVIAC_DEFAULT_ALIGN, tag, file, line);
}

BEHAVIAC_FORCEINLINE void* operator new[](size_t size, const behaviac::BehaviacOperatorNewArrayType_t& /*type*/, const char* tag, const char* file, int line) {
    return behaviac::internal::_MemoryHelperAllocAlignment(&behaviac::GetMemoryAllocator(), size, BEHAVIAC_DEFAULT_ALIGN, tag, file, line);
}

BEHAVIAC_FORCEINLINE void operator delete[](void* ptr, const behaviac::BehaviacOperatorNewArrayType_t& /*type*/, const char* tag, const char* file, int line) {
    behaviac::internal::_MemoryHelperFreeAlignment(&behaviac::GetMemoryAllocator(), ptr, BEHAVIAC_DEFAULT_ALIGN, tag, file, line);
}

#define BEHAVIAC_MALLOC_WITHTAG(size, tag)							behaviac::GetMemoryAllocator().Alloc(size, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_REALLOC_WITHTAG(ptr, size, tag)					behaviac::GetMemoryAllocator().Realloc(((void*)ptr), size, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_FREE_WITHTAG(ptr, tag)								behaviac::GetMemoryAllocator().Free(((void*)ptr), tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)

#define BEHAVIAC_MALLOC(size)										behaviac::GetMemoryAllocator().Alloc(size, "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_REALLOC(ptr, size)									behaviac::GetMemoryAllocator().Realloc(((void*)ptr), size, "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_FREE(ptr)											behaviac::GetMemoryAllocator().Free(((void*)ptr), "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)

#define BEHAVIAC_MALLOCALIGNED_WITHTAG(size, alignment, tag)		behaviac::GetMemoryAllocator().AllocAligned(size, alignment, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_REALLOCALIGNED_WITHTAG(ptr, size, alignment, tag)	behaviac::GetMemoryAllocator().Realloc(ptr, size, alignment, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_FREEALIGNED_WITHTAG(ptr, alignment, tag)			behaviac::GetMemoryAllocator().FreeAligned(ptr, alignment, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)

#define BEHAVIAC_MALLOCALIGNED(size, alignment)						BEHAVIAC_MALLOCALIGNED_WITHTAG(size, alignment, "behaviac")
#define BEHAVIAC_REALLOCALIGNED(ptr, size, alignment)				BEHAVIAC_REALLOCALIGNED_WITHTAG(ptr, size, alignment, "behaviac")
#define BEHAVIAC_FREEALIGNED(ptr, alignment)						BEHAVIAC_FREEALIGNED_WITHTAG(ptr, alignment, "behaviac")

#if BEHAVIAC_CCDEFINE_MSVC
// Allocates memory on the stack
#define BEHAVIAC_ALLOCA(s)  _alloca(s)
#else
// Allocates memory on the stack
#define BEHAVIAC_ALLOCA(s)  alloca(s)
#endif


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#define BEHAVIAC_G_NEW_WITHTAG(T, tag)							new(behaviac::internal::_MemoryHelperAlloc(behaviac::GetAllocator<T>(), sizeof(T), BEHAVIAC_ALIGNOF(T), tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)) T
#define BEHAVIAC_G_DELETE_WITHTAG(object, tag)					behaviac::internal::_MemoryHelperDelete(object, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_G_NEW_ARRAY_WITHTAG(T, count, tag)				behaviac::internal::_MemoryHelperAllocArray<T>(behaviac::GetAllocator<T>(), count, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_G_DELETE_ARRAY_WITHTAG(pArray, tag)			behaviac::internal::_MemoryHelperDeleteArray(pArray, tag, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)

#define BEHAVIAC_G_NEW(T)										BEHAVIAC_G_NEW_WITHTAG(T, 0)
#define BEHAVIAC_G_DELETE(object)								BEHAVIAC_G_DELETE_WITHTAG(object, 0)
#define BEHAVIAC_G_NEW_ARRAY(T, count)							BEHAVIAC_G_NEW_ARRAY_WITHTAG(T, count, 0)
#define BEHAVIAC_G_DELETE_ARRAY(pArray)							BEHAVIAC_G_DELETE_ARRAY_WITHTAG(pArray, 0)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#define BEHAVIAC_DECLARE_MEMORY_OPERATORS_COMMON_(CLASS, align) \
    public: \
    friend BEHAVIAC_FORCEINLINE void operator /(const behaviac::BehaviacOperatorNewType_t& type, CLASS * object) \
    { \
        if (object) object->tag_special_delete(type); \
    } \
    friend BEHAVIAC_FORCEINLINE void operator /(const behaviac::BehaviacOperatorNewArrayType_t& type, CLASS * object) \
    { \
        if (object) object->tag_special_delete_array(type); \
    }\
    BEHAVIAC_FORCEINLINE void * operator new(size_t, void* pWhere) \
    { \
        return pWhere; \
    } \
    BEHAVIAC_FORCEINLINE void operator delete(void*, void*) \
    { \
    } \
    BEHAVIAC_FORCEINLINE void * operator new(size_t size, const behaviac::BehaviacOperatorNewType_t& /*type*/, const char * tag, const char * file, int line) \
    { \
        return behaviac::internal::_MemoryHelperAllocAlignment(behaviac::GetAllocator<CLASS>(), size, align, tag, file, line); \
    } \
    BEHAVIAC_FORCEINLINE void* operator new[](size_t size, const behaviac::BehaviacOperatorNewArrayType_t& /*type*/, const char * tag, const char * file, int line) \
    { \
        return behaviac::internal::_MemoryHelperAllocAlignment(behaviac::GetAllocator<CLASS>(), size, align, tag, file, line); \
    } \
    BEHAVIAC_FORCEINLINE void operator delete(void* ptr) \
    { \
        behaviac::internal::_MemoryHelperFreeAlignment(behaviac::GetAllocator<CLASS>(), ptr, align, 0, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__); \
    } \
    BEHAVIAC_FORCEINLINE void operator delete[](void* ptr) \
    { \
        behaviac::internal::_MemoryHelperFreeAlignment(behaviac::GetAllocator<CLASS>(), ptr, align, 0, __BEHAVIAC_FILE__, __BEHAVIAC_LINE__); \
    }\
    BEHAVIAC_FORCEINLINE void operator delete(void * ptr, const behaviac::BehaviacOperatorNewType_t& /*type*/, const char * tag, const char * file, int line) \
    { \
        BEHAVIAC_UNUSED_VAR(tag);\
        BEHAVIAC_UNUSED_VAR(file);\
        BEHAVIAC_UNUSED_VAR(line);\
        behaviac::internal::_MemoryHelperFreeAlignment(behaviac::GetAllocator<CLASS>(), ptr, align, tag, file, line); \
    } \
    BEHAVIAC_FORCEINLINE void operator delete[](void * ptr, const behaviac::BehaviacOperatorNewArrayType_t& /*type*/, const char * tag, const char * file, int line) \
    { \
        BEHAVIAC_UNUSED_VAR(tag);\
        BEHAVIAC_UNUSED_VAR(file);\
        BEHAVIAC_UNUSED_VAR(line);\
        behaviac::internal::_MemoryHelperFreeAlignment(behaviac::GetAllocator<CLASS>(), ptr, align, tag, file, line); \
    } \
    private:\
    BEHAVIAC_FORCEINLINE void tag_special_delete_array(const behaviac::BehaviacOperatorNewArrayType_t& /*type*/) const \
    { \
        delete[] this;\
    } \
    public :

#define BEHAVIAC_DECLARE_MEMORY_OPERATORS_(CLASS, align) \
    BEHAVIAC_DECLARE_MEMORY_OPERATORS_COMMON_(CLASS, align) \
    BEHAVIAC_FORCEINLINE void tag_special_delete(const behaviac::BehaviacOperatorNewType_t& /*type*/) const \
    { \
        delete this;\
    } \
    public :

#define BEHAVIAC_DECLARE_MEMORY_OPERATORS_AGENT_(CLASS, align) \
    BEHAVIAC_DECLARE_MEMORY_OPERATORS_COMMON_(CLASS, align) \
    BEHAVIAC_FORCEINLINE void tag_special_delete(const behaviac::BehaviacOperatorNewType_t& /*type*/) \
    { \
        destroy_();\
    } \
    public :

#define BEHAVIAC_DECLARE_MEMORY_OPERATORS(CLASS) \
    BEHAVIAC_DECLARE_MEMORY_OPERATORS_(CLASS, BEHAVIAC_ALIGNOF(CLASS))

#define BEHAVIAC_DECLARE_MEMORY_OPERATORS_AGENT(CLASS) \
    BEHAVIAC_DECLARE_MEMORY_OPERATORS_AGENT_(CLASS, BEHAVIAC_ALIGNOF(CLASS))

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//BEHAVIAC_G_* is more reliable and flexible, but, the following macros' syntax is more similiar to the system's new/delete
//
//generally, the following macros should work together with BEHAVIAC_DECLARE_MEMORY_OPERATORS.
//
//BEHAVIAC_NEW	type
//BEHAVIAC_DELETE	type
//
//if the 'type' doesn't include 'BEHAVIAC_DECLARE_MEMORY_OPERATORS', it has the following risks:
//
//BEHAVIAC_DELETE calls behaviac::internal::_MemoryHelperDeleteAlignment to handle alignment
//while BEHAVIAC_NEW doesn't handle alignment(alignenmt is always 4).
//
//so if type's alignment > 4, it will assert-fail and crash in _aligned_free(was allocated by malloc instead of _aligned_malloc)
//please use BEHAVIAC_G_NEW/BEHAVIAC_G_DELETE instead or to include BEHAVIAC_DECLARE_MEMORY_OPERATORS
//
//the other risk is BEHAVIAC_NEW uses behaviac::GetMemoryAllocator() as the allocator
//while BEHAVIAC_DELETE uses behaviac::GetAllocator<T>() as the allocator,
//so if these two allocators are not the same, it causes problems.
//please use BEHAVIAC_G_NEW/BEHAVIAC_G_DELETE instead or to include BEHAVIAC_DECLARE_MEMORY_OPERATORS
//
//also, BEHAVIAC_NEW_ARRAY/BEHAVIAC_DELETE_ARRAY MUST work together with BEHAVIAC_DECLARE_MEMORY_OPERATORS,
//otherwise, it will assert-fail. this is when you need to new/delete an array,
//either you should use BEHAVIAC_G_NEW_ARRAY/BEHAVIAC_G_DELETE_ARRAY or you should include BEHAVIAC_DECLARE_MEMORY_OPERATORS int the type.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define BEHAVIAC_NEW					new(behaviac::BehaviacOperatorNewType_t::GetInstance(), "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_DELETE					behaviac::BehaviacOperatorNewType_t::GetInstance() /

#define BEHAVIAC_NEW_ARRAY				new(behaviac::BehaviacOperatorNewArrayType_t::GetInstance(), "behaviac", __BEHAVIAC_FILE__, __BEHAVIAC_LINE__)
#define BEHAVIAC_DELETE_ARRAY			behaviac::BehaviacOperatorNewArrayType_t::GetInstance() /

#include "behaviac/common/memory/objectpool.h"

#endif//_BEHAVIAC_COMMON_MEMORY_H_
