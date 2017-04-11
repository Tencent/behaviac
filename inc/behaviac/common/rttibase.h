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

#ifndef _BEHAVIAC_COMMON_RTTIBASE_H_
#define _BEHAVIAC_COMMON_RTTIBASE_H_

#include "behaviac/common/base.h"
#include "behaviac/common/container/string.h"
#include "behaviac/common/container/vector.h"
#include "behaviac/common/container/list.h"
#include "behaviac/common/container/map.h"
#include "behaviac/common/container/set.h"

#include "behaviac/common/string/stringcrc.h"
#include "behaviac/common/crc.h"
#include "behaviac/common/meta/meta.h"

namespace behaviac {
    // rtti
    class BEHAVIAC_API CRTTIBase {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(CRTTIBase);

    protected:
        struct CLayerInfoBase {
            char const*		m_szCassTypeName;
            uint32_t		m_hierarchyLevel;  // == NumLayers.
        };

    public:
        struct CLayerInfo : public CLayerInfoBase {
            behaviac::CStringCRC   m_hierarchy[1];
            inline const behaviac::CStringCRC& GetClassTypeId() const {
                return m_hierarchy[m_hierarchyLevel - 1];
            }
        };

    public:
        template<int NumLayers>
        struct TLayerInfoDecl : public CLayerInfoBase {
            friend class CRTTIBase;
        protected:
            struct FakeCStringCRC {
                char m_buf[sizeof(behaviac::CStringCRC)];
            };
            FakeCStringCRC	m_hierarchy[NumLayers];

        public:
            void InternalInitClassLayerInfo(char const* szCassTypeName, const CLayerInfoBase* pParentLayerInfo) {
                CLayerInfo*       target = reinterpret_cast<CLayerInfo*>(this);
                const CLayerInfo* parent = static_cast<const CLayerInfo*>(pParentLayerInfo);

#if BEHAVIAC_CCDEFINE_MSVC
                {
                    BEHAVIAC_STATIC_ASSERT(BEHAVIAC_OFFSETOF(TLayerInfoDecl, m_hierarchy) == BEHAVIAC_OFFSETOF(CLayerInfo, m_hierarchy));
                }
#endif
                // BEHAVIAC_ASSERT(BEHAVIAC_OFFSETOF_POD(TLayerInfoDecl, m_hierarchy) == BEHAVIAC_OFFSETOF_POD(CLayerInfo, m_hierarchy));
                {
                    BEHAVIAC_STATIC_ASSERT(sizeof(m_hierarchy[0]) == sizeof(parent->m_hierarchy[0]));
                }

                target->m_szCassTypeName = szCassTypeName;
                uint32_t parentLevel = 0;
                behaviac::CStringCRC* pTargetHierarchy = (behaviac::CStringCRC*)target->m_hierarchy;

                if (parent != NULL) {
                    parentLevel = parent->m_hierarchyLevel;
                    BEHAVIAC_ASSERT(parentLevel < 19);

                    const behaviac::CStringCRC* pParentHierarchy = (const behaviac::CStringCRC*)parent->m_hierarchy;

                    for (uint32_t i = 0; i < parentLevel; i++) {
                        pTargetHierarchy[i] = pParentHierarchy[i];
                    }
                }

                target->m_hierarchyLevel = parentLevel + 1;
                pTargetHierarchy[parentLevel] = behaviac::CStringCRC(szCassTypeName);
            }
            void InitClassLayerInfo(char const* szCassTypeName, const CLayerInfoBase* pParentLayerInfo);
        };

        static const uint32_t sm_HierarchyLevel = 0;

    public:
        virtual ~CRTTIBase() {}

        virtual const CLayerInfo* GetHierarchyInfo() const {
            return NULL;
        }

        const BEHAVIAC_FORCEINLINE behaviac::CStringCRC& GetObjectTypeId() const {
            return GetHierarchyInfo()->GetClassTypeId();
        }
        const BEHAVIAC_FORCEINLINE char* GetObjectTypeName() const {
            return GetHierarchyInfo()->m_szCassTypeName;
        }

        static CRTTIBase* DynamicCast(CRTTIBase* other) {
            return other;
        }
        static const CRTTIBase* DynamicCast(const CRTTIBase* other) {
            return other;
        }

        bool IsAKindOf(const behaviac::CStringCRC& typeId) const {
            const CLayerInfo* info = GetHierarchyInfo();

            for (uint32_t i = 0; i < info->m_hierarchyLevel; ++i) {
                const behaviac::CStringCRC* pTargetHierarchy = (const behaviac::CStringCRC*)info->m_hierarchy;

                if (pTargetHierarchy[i] == typeId) {
                    return true;
                }
            }

            return false;
        }


    public:
        bool IsParent_(uint32_t level, const behaviac::CStringCRC& classId) const {
            const CLayerInfo* info = GetHierarchyInfo();
            const behaviac::CStringCRC* pTargetHierarchy = (const behaviac::CStringCRC*)info->m_hierarchy;

            if (level > 0 && level <= info->m_hierarchyLevel) {
                const behaviac::CStringCRC& my = pTargetHierarchy[level - 1];

                if (my.GetUniqueID() == classId.GetUniqueID()) {
                    return true;
                }
            }

            return false;
        }

    };

    template<int NumLayers>
    void CRTTIBase::TLayerInfoDecl<NumLayers>::InitClassLayerInfo(char const* szCassTypeName, const CLayerInfoBase* pParentLayerInfo) {
        InternalInitClassLayerInfo(szCassTypeName, pParentLayerInfo);
    }

    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<1>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<2>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<3>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<4>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<5>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<6>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<7>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<8>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<9>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<10>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<11>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<12>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<13>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<14>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<15>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<16>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<17>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<18>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);
    template<> void BEHAVIAC_API CRTTIBase::TLayerInfoDecl<19>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* pParentLayerInfo);

    template<typename T>
    const char* GetClassTypeName(T*);
    template<typename T>
    const char* GetClassTypeName(T**);

    template<typename T, bool bVector, bool bMap>
    struct ClassTypeNameGetter {
        typedef REAL_BASETYPE(T) REALBASETYPE;
        static const char* GetClassTypeName() {
            const char* pType = REALBASETYPE::GetClassTypeName();
            return pType;
        }
    };

    template<typename T>
    struct ClassTypeNameGetter<T, true, false> {
        static const char* GetClassTypeName() {
            typedef typename behaviac::Meta::IsVector<T>::ElementType ElementType;

            const char* pType = behaviac::GetClassTypeName((ElementType*)0);

            static char s_buffer[256];
            string_sprintf(s_buffer, "vector<%s>", pType);
            return s_buffer;
        }
    };

    template<typename T>
    struct ClassTypeNameGetter<T, false, true> {
        static const char* GetClassTypeName() {
            typedef typename behaviac::Meta::IsMap<T>::KeyType KeyType;
            typedef typename behaviac::Meta::IsMap<T>::ValueType ValueType;

            const char* pKeyType = behaviac::GetClassTypeName((KeyType*)0);
            const char* pValueType = behaviac::GetClassTypeName((ValueType*)0);

            static char s_buffer[256];
            string_sprintf(s_buffer, "map<%s, %s>", pKeyType, pValueType);
            return s_buffer;
        }
    };

    template<typename T>
    inline const char* GetClassTypeName(T*) {
        const char* pType = ClassTypeNameGetter<T, behaviac::Meta::IsVector<T>::Result, behaviac::Meta::IsMap<T>::Result>::GetClassTypeName();
        return pType;
    }

    template<typename T>
    inline const char* GetClassTypeName(T**) {
        const char* pType = ClassTypeNameGetter<T, behaviac::Meta::IsVector<T>::Result, behaviac::Meta::IsMap<T>::Result>::GetClassTypeName();
        return pType;
    }

    template<typename T>
    inline behaviac::string GetTypeDescString() {
        typedef REAL_BASETYPE(T)				RealBaseType_t;
        typedef POINTER_TYPE(RealBaseType_t)	PointerType_t;
        PointerType_t pT = (PointerType_t)0;
        behaviac::string typeResult = behaviac::GetClassTypeName(pT);
        bool isConst = behaviac::Meta::IsConst<T>::Result;
        bool isPtr = behaviac::Meta::IsPtr<T>::Result;
        bool isRef = behaviac::Meta::IsRef<T>::Result;

        if (isConst) {
            typeResult = "const " + typeResult;
        }

        if (isPtr) {
            typeResult += "*";
        }

        if (isRef) {
            typeResult += "&";
        }

        return typeResult;
    }

    template < typename T, bool bAgent >
    struct GetClassTypeNumberIdSelector {
        static int GetClassTypeNumberId() {
            const char* szCassTypeName = behaviac::GetClassTypeName((T*)0);
            return CRC32::CalcCRC(szCassTypeName);
        }
    };

    template < typename T >
    struct GetClassTypeNumberIdSelector<T, true> {
        static int GetClassTypeNumberId() {
            const char* szCassTypeName = "void*";
            return CRC32::CalcCRC(szCassTypeName);
        }
    };

    template< typename T >
    struct TStruct_GetClassTypeNumberId {
        static int GetClassTypeNumberId() {
            int ret = GetClassTypeNumberIdSelector<T, behaviac::Meta::IsRefType<T>::Result>::GetClassTypeNumberId();

            return ret;
        }
    };

    template< typename T >
    struct TStruct_GetClassTypeNumberId< const T > {
        static int GetClassTypeNumberId() {
            //typedef typename behaviac::Meta::RemoveConst<T>::Result BaseType;
            //int ret = GetClassTypeNumberIdSelector<BaseType, behaviac::Meta::IsRefType<BaseType>::Result>::GetClassTypeNumberId();
            //int ret = GetClassTypeNumberId<BaseType>();
            int ret = GetClassTypeNumberIdSelector<T, behaviac::Meta::IsRefType<T>::Result>::GetClassTypeNumberId();

            return ret;
        }
    };

    template<typename T>
    inline int GetClassTypeNumberId() {
        //int ret = TStruct_GetClassTypeNumberId<T, behaviac::Meta::IsConst<T>::Result>::GetClassTypeNumberId();
        int ret = TStruct_GetClassTypeNumberId<T>::GetClassTypeNumberId();
        //int ret = GetClassTypeNumberIdSelector<T, behaviac::Meta::IsRefType<T>::Result>::GetClassTypeNumberId();

        return ret;
    }

}//

#define BEHAVIAC_INTERNAL_DECLARE_DYNAMIC_TYPE_COMPOSER(__type) \
    public: \
    static const char* GetClassTypeName() \
    { \
        return #__type; \
    }

//BEHAVIAC_OVERRIDE_TYPE_NAME_ can't be placed in a namespace
#define BEHAVIAC_OVERRIDE_TYPE_NAME_(typeFullClassNameWithNamespace, szCassTypeName)																		\
    namespace behaviac {																															\
        template <> inline const char* GetClassTypeName< typeFullClassNameWithNamespace >(typeFullClassNameWithNamespace*){ return #szCassTypeName; }		\
        template <> inline const char* GetClassTypeName< typeFullClassNameWithNamespace >(typeFullClassNameWithNamespace**){ return #szCassTypeName"*"; } \
    }

//BEHAVIAC_OVERRIDE_TYPE_NAME can't be placed in a namespace
#define BEHAVIAC_OVERRIDE_TYPE_NAME(typeFullNameWithNamespace)    BEHAVIAC_OVERRIDE_TYPE_NAME_(typeFullNameWithNamespace, typeFullNameWithNamespace)

namespace behaviac {
    BEHAVIAC_API char* MakeStringName1(const char* aT1, const char* aT2);
    BEHAVIAC_API char* MakeStringName2(const char* aT1, const char* aT2, const char* aT3);
    BEHAVIAC_API char* MakeStringName3(const char* aT1, const char* aT2, const char* aT3, const char* aT4);
    BEHAVIAC_API char* MakeStringName4(const char* aT1, const char* aT2, const char* aT3, const char* aT4, const char* aT5);
    BEHAVIAC_API char* MakeStringName5(const char* aT1, const char* aT2, const char* aT3, const char* aT4, const char* aT5, const char* aT6);
}

#define BEHAVIAC_INTERNAL_DECLARE_DYNAMIC_PUBLIC_METHODES(__type, __parent) \
    protected: \
    static const uint32_t sm_HierarchyLevel = __parent::sm_HierarchyLevel + 1; \
    \
    static BEHAVIAC_FORCEINLINE CRTTIBase::TLayerInfoDecl< sm_HierarchyLevel >* GetClassHierarchyInfoDecl() \
    { \
        static CRTTIBase::TLayerInfoDecl< sm_HierarchyLevel > sm_HierarchyInfo; \
        return &sm_HierarchyInfo; \
    } \
    public: \
    typedef __parent super; \
    virtual BEHAVIAC_FORCEINLINE const CRTTIBase::CLayerInfo* GetHierarchyInfo() const \
    { \
        CRTTIBase::TLayerInfoDecl< sm_HierarchyLevel >* decl = GetClassHierarchyInfoDecl(); \
        if (!decl->m_szCassTypeName) decl->InitClassLayerInfo( \
                                                                   __type::GetClassTypeName(), __parent::GetHierarchyInfo()); \
        return (const CRTTIBase::CLayerInfo*)decl; \
    } \
    static /*BEHAVIAC_FORCEINLINE*/ const behaviac::CStringCRC& GetClassTypeId() \
    { \
        CRTTIBase::TLayerInfoDecl< sm_HierarchyLevel >* decl = GetClassHierarchyInfoDecl(); \
        if (!decl->m_szCassTypeName) ((const __type*)NULL)->__type::GetHierarchyInfo(); \
        const behaviac::CStringCRC* pTargetHierarchy = (const behaviac::CStringCRC*)((const CRTTIBase::CLayerInfo*)decl)->m_hierarchy;\
        return pTargetHierarchy[__type::sm_HierarchyLevel - 1]; \
    } \
    static bool IsClassAKindOf(const behaviac::CStringCRC& typeId) \
    { \
        const CRTTIBase::TLayerInfoDecl< sm_HierarchyLevel >* decl = GetClassHierarchyInfoDecl(); \
        if (!decl->m_szCassTypeName) ((const __type*)NULL)->__type::GetHierarchyInfo(); \
        for(uint32_t i = 0; i < sm_HierarchyLevel; ++i) \
        { \
            const behaviac::CStringCRC* pTargetHierarchy = (const behaviac::CStringCRC*)((const CRTTIBase::CLayerInfo*)decl)->m_hierarchy;\
            if(pTargetHierarchy[i] == typeId) \
                return true; \
        } \
        return false; \
    } \
    static __type *DynamicCast( CRTTIBase *other ) \
    { \
        if( other != NULL && other->IsParent_(__type::sm_HierarchyLevel,__type::GetClassTypeId()) ) \
        { \
            return static_cast< __type * >( other ); \
        } \
        return NULL; \
    } \
    static const __type *DynamicCast( const CRTTIBase *other ) \
    { \
        if( other != NULL && other->IsParent_(__type::sm_HierarchyLevel,__type::GetClassTypeId()) ) \
        { \
            return static_cast< const __type * >( other ); \
        } \
        return NULL; \
    } 

#define BEHAVIAC_DECLARE_DYNAMIC_TYPE(__type, __parent)						\
    BEHAVIAC_INTERNAL_DECLARE_DYNAMIC_TYPE_COMPOSER(__type);				\
    BEHAVIAC_INTERNAL_DECLARE_DYNAMIC_PUBLIC_METHODES(__type, __parent);

#define BEHAVIAC_DECLARE_ROOT_DYNAMIC_TYPE(__type, __parent)    BEHAVIAC_DECLARE_DYNAMIC_TYPE(__type, __parent);

#undef BEHAVIAC_DECLARE_PRIMITE_TYPE
#define BEHAVIAC_DECLARE_PRIMITE_TYPE(type, szCassTypeName)							\
    BEHAVIAC_OVERRIDE_TYPE_NAME_(type, szCassTypeName);								\
    BEHAVIAC_OVERRIDE_TYPE_NAME_(const type, "const "#szCassTypeName);

#define  M_PRIMITIVE_NUMBER_TYPES()												\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(char, char);									\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(unsigned char, ubyte);						\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(signed char, sbyte);							\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(unsigned short, ushort);						\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(signed short, short);							\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(unsigned int, uint);							\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(signed int, int);								\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(unsigned long, ulong);						\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(signed long, long);							\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(unsigned long long, ullong);					\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(signed long long, llong);						\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(float, float);								\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(double, double);

#if BEHAVIAC_USE_CUSTOMSTRING
#define  BEHAVIAC_M_PRIMITIVE_TYPES()											\
    M_PRIMITIVE_NUMBER_TYPES()													\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(bool, bool);									\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(void*, void*);								\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(behaviac::string, string);					\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(behaviac::wstring, behaviac::wstring);		\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(std::string, std::string);					\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(std::wstring, std::wstring);
#else
#define  BEHAVIAC_M_PRIMITIVE_TYPES()											\
    M_PRIMITIVE_NUMBER_TYPES()													\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(bool, bool);									\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(void*, void*);								\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(std::string, std::string);					\
    BEHAVIAC_DECLARE_PRIMITE_TYPE(std::wstring, std::wstring);
#endif//BEHAVIAC_USE_CUSTOMSTRING

//BEHAVIAC_M_PRIMITIVE_TYPES preprocessing
BEHAVIAC_M_PRIMITIVE_TYPES();

//specialization of intrinsics types...
BEHAVIAC_OVERRIDE_TYPE_NAME(void);
BEHAVIAC_OVERRIDE_TYPE_NAME(const char*);

#define BEHAVIAC_BASICTYPE_NUMBER_ID(type, id)			\
    namespace behaviac {								\
        template<> inline int GetClassTypeNumberId<type>() \
        {\
            return id;\
        }\
        template<> inline int GetClassTypeNumberId<const type>() \
        {\
            return id; \
        }\
    }

BEHAVIAC_BASICTYPE_NUMBER_ID(bool, 1)
BEHAVIAC_BASICTYPE_NUMBER_ID(char, 2)
BEHAVIAC_BASICTYPE_NUMBER_ID(signed char, 3)
BEHAVIAC_BASICTYPE_NUMBER_ID(unsigned char, 4)
BEHAVIAC_BASICTYPE_NUMBER_ID(short, 5)
BEHAVIAC_BASICTYPE_NUMBER_ID(unsigned short, 6)
BEHAVIAC_BASICTYPE_NUMBER_ID(int, 7)
BEHAVIAC_BASICTYPE_NUMBER_ID(unsigned int, 8)
BEHAVIAC_BASICTYPE_NUMBER_ID(long, 9)
BEHAVIAC_BASICTYPE_NUMBER_ID(unsigned long, 10)
BEHAVIAC_BASICTYPE_NUMBER_ID(long long, 11)
BEHAVIAC_BASICTYPE_NUMBER_ID(unsigned long long, 12)
BEHAVIAC_BASICTYPE_NUMBER_ID(float, 13)
BEHAVIAC_BASICTYPE_NUMBER_ID(double, 14)

namespace behaviac {
    //
    inline bool IsNumberClassTypeNumberId(int typeId) {
        if (
            typeId == GetClassTypeNumberId<char>() ||
            typeId == GetClassTypeNumberId<signed char>() ||
            typeId == GetClassTypeNumberId<unsigned char>() ||
            typeId == GetClassTypeNumberId<short>() ||
            typeId == GetClassTypeNumberId<unsigned short>() ||
            typeId == GetClassTypeNumberId<int>() ||
            typeId == GetClassTypeNumberId<unsigned int>() ||
            typeId == GetClassTypeNumberId<long>() ||
            typeId == GetClassTypeNumberId<unsigned long>() ||
            typeId == GetClassTypeNumberId<long long>() ||
            typeId == GetClassTypeNumberId<unsigned long long>() ||
            typeId == GetClassTypeNumberId<float>() ||
            typeId == GetClassTypeNumberId<double>()
        ) {
            return true;
        }

        return false;
    }

}//

#endif  // _BEHAVIAC_COMMON_RTTIBASE_H_
