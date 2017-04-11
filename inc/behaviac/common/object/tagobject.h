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

#ifndef _BEHAVIAC_COMMON_OBJECT_TAGOBJECT_H_
#define _BEHAVIAC_COMMON_OBJECT_TAGOBJECT_H_

#include "behaviac/common/base.h"
#include "behaviac/common/serialization/ionode.h"
#include "behaviac/common/swapbyte.h"
#include "behaviac/common/serialization/textnode.h"

#if BEHAVIAC_CCDEFINE_MSVC
//C4189: 'ms_members' : local variable is initialized but not referenced
#pragma warning (disable : 4189 )
#endif//#if BEHAVIAC_CCDEFINE_MSVC

namespace behaviac {
    class IMemberBase;
    class CTagObject;

    class BEHAVIAC_API CTagTypeDescriptor : public CRTTIBase {
    public:
        BEHAVIAC_DECLARE_ROOT_DYNAMIC_TYPE(CTagTypeDescriptor, CRTTIBase);

        typedef behaviac::map<const char*, const class CTagTypeDescriptor*> TypesMap_t;
    };

    class BEHAVIAC_API CTagObject : public CRTTIBase {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(behaviac::CTagObject);
        BEHAVIAC_DECLARE_ROOT_DYNAMIC_TYPE(behaviac::CTagObject, CRTTIBase);

		static void Load(const void* pObject, const behaviac::IIONode* node, const char* szClassName);
		static void Save(const void* pObject, behaviac::IIONode* node, const char* szClassName);
    };
}//namespace behaviac

namespace behaviac {
    namespace StringUtils {
        BEHAVIAC_API behaviac::XmlNodeReference MakeXmlNodeStruct(const char* str, const behaviac::string& typeNameT);

        template<typename T>
        inline bool FromString_Struct(const char* str, T& val) {
			const char* className = behaviac::GetClassTypeName((T*)0);

			behaviac::XmlNodeReference xmlNode = MakeXmlNodeStruct(str, className);

            if ((IXmlNode*)xmlNode) {
                CTextNode textNode(xmlNode);

				CTagObject::Load(&val, &textNode, className);

                return true;
            }

            return false;
        }

        BEHAVIAC_API bool MakeStringFromXmlNodeStruct(behaviac::XmlConstNodeRef xmlNode, behaviac::string& result);

        template<typename T>
		inline behaviac::string ToString_Struct(T& val, const char* szClassName) {
			//const char* szClassName = behaviac::GetClassTypeName((T*)0);
			behaviac::XmlNodeReference xmlNode = CreateXmlNode(szClassName);

            CTextNode textNode(xmlNode);

			CTagObject::Save(&val, &textNode, szClassName);

            behaviac::string result;

            if (MakeStringFromXmlNodeStruct(xmlNode, result)) {
                return result;
            }

            return "";
        }
    }

	BEHAVIAC_API bool Equal_Struct(void* lhs, void* rhs, const char* szClassName);
}

#define BEHAVIAC_ACCESS_PROPERTY												\
    template<typename T, typename R>											\
    BEHAVIAC_FORCEINLINE R& _Get_Property_();									\

// behaviac::CTagObject macros
#define BEHAVIAC_ACCESS_PROPERTY_METHOD											\
    BEHAVIAC_ACCESS_PROPERTY													\
																				\
    template<typename T, typename R>											\
    BEHAVIAC_FORCEINLINE R _Execute_Method_();									\
    template<typename T, typename R, typename P0>								\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0);								\
    template<typename T, typename R, typename P0, typename P1>					\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1);							\
    template<typename T, typename R, typename P0, typename P1, typename P2>		\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1, P2);																									\
    template<typename T, typename R, typename P0, typename P1, typename P2, typename P3>																	\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1, P2, P3);																								\
    template<typename T, typename R, typename P0, typename P1, typename P2, typename P3, typename P4>														\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1, P2, P3, P4);																							\
    template<typename T, typename R, typename P0, typename P1, typename P2, typename P3, typename P4, typename P5>											\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1, P2, P3, P4, P5);																						\
    template<typename T, typename R, typename P0, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>								\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1, P2, P3, P4, P5, P6);																					\
    template<typename T, typename R, typename P0, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7>				\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1, P2, P3, P4, P5, P6, P7);																				\
    template<typename T, typename R, typename P0, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8>	\
    BEHAVIAC_FORCEINLINE R _Execute_Method_(P0, P1, P2, P3, P4, P5, P6, P7, P8);


/////////////////////////////////////////////////////////////////////////////////////////
//classFullNameWithNamespace is the class full name with namespace, like test_ns::AgentTest
//even the class is delared in a namespace, it is still advised to use the full name with the name space.
#define BEHAVIAC_DECLARE_AGENTTYPE(classFullNameWithNamespace, parentClassName)			\
    BEHAVIAC_DECLARE_MEMORY_OPERATORS_AGENT(classFullNameWithNamespace)					\
    BEHAVIAC_DECLARE_ROOT_DYNAMIC_TYPE(classFullNameWithNamespace, parentClassName)		\
    BEHAVIAC_ACCESS_PROPERTY_METHOD

/////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////
// unused any more
#define BEHAVIAC_BEGIN_STRUCT(className)  struct BEHAVIAC_UNIQUE_NAME(__dummy_function_) { void init(){
#define BEHAVIAC_END_STRUCT() } };

//deparated, to use BEHAVIAC_STRUCT_DISPLAY_INFO
#define BEHAVIAC_STRUCT_DISPLAYNAME(displayName)
#define BEHAVIAC_STRUCT_DESC(desc)

#define BEHAVIAC_STRUCT_DISPLAY_INFO(displayName, desc)

////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
#define DECLARE_BEHAVIAC_OBJECT_BASE(className, bRefType)						\
    public:																		\
    enum { ms_bIsRefType = bRefType };


#define DECLARE_BEHAVIAC_STRUCT_BASE_(className, bRefType)	\
    DECLARE_BEHAVIAC_OBJECT_BASE(className, bRefType)						\
    BEHAVIAC_ACCESS_PROPERTY												\
    bool _Object_Equal_(const className& rhs)	const						\
    {																		\
        return behaviac::Equal_Struct(this, &rhs, this->GetClassTypeName());\
    }																		\
    bool ParseString(const char* str)										\
    {																		\
        return behaviac::StringUtils::FromString_Struct(str, *this);		\
    }																		\
    behaviac::string ToString() const										\
    {																		\
        return behaviac::StringUtils::ToString_Struct(*this, this->GetClassTypeName());\
    }

/////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////
//classFullNameWithNamespace is the class full name with namespace, like test_ns::AgentTest,
//even the class is delared in a namespace, it is still advised to use the full name with the name space.
//the corresponding BEHAVIAC_BEGIN_STRUCT/BEHAVIAC_END_STRUCT in the cpp can be put in or out of that namespace.
#define DECLARE_BEHAVIAC_OBJECT_STRUCT_V2(classFullNameWithNamespace, bRefType)		\
    DECLARE_BEHAVIAC_STRUCT_BASE_(classFullNameWithNamespace, bRefType)				\
    void Load(const behaviac::IIONode* node)										\
    {																				\
        const char* szClassName = behaviac::GetClassTypeName((classFullNameWithNamespace*)0);\
        behaviac::CTagObject::Load((behaviac::CTagObject*)this, node, szClassName);	\
    }																				\
    void Save(behaviac::IIONode* node) const										\
    {																				\
        const char* szClassName = behaviac::GetClassTypeName((classFullNameWithNamespace*)0);\
        behaviac::CTagObject::Save((behaviac::CTagObject*)this, node, szClassName);	\
    }																				\
    static const char* GetClassTypeName() { return #classFullNameWithNamespace; }

#define DECLARE_BEHAVIAC_OBJECT_STRUCT_V1(classFullNameWithNamespace) DECLARE_BEHAVIAC_OBJECT_STRUCT_V2(classFullNameWithNamespace, false)

//DECLARE_BEHAVIAC_STRUCT can accept 1 or 2 parameters
//
//the 1st param is the full class name with the namespace if any, like test_ns::AgentTest,
//even the class is delared in a namespace, it is still advised to use the full name with the name space.
//the corresponding BEHAVIAC_BEGIN_STRUCT/BEHAVIAC_END_STRUCT in the cpp can be put in or out of that namespace.
//
//the 2nd param is true or false indicating if the class is a ref type. a ref type is used as a pointer.
#define DECLARE_BEHAVIAC_STRUCT(...)
#define DECLARE_BEHAVIAC_STRUCT_EX(...) _BEHAVIAC_ARGUMENT_SELECTOR2_((__VA_ARGS__, DECLARE_BEHAVIAC_OBJECT_STRUCT_V2, DECLARE_BEHAVIAC_OBJECT_STRUCT_V1))(__VA_ARGS__)
#define _BEHAVIAC_ARGUMENT_SELECTOR2_(__args) _BEHAVIAC_GET_2TH_ARGUMENT_ __args
#define _BEHAVIAC_GET_2TH_ARGUMENT_(__p1,__p2,__n,...) __n

#define BEHAVIAC_EXTEND_EXISTING_TYPE(existingType, bRefType)
#define BEHAVIAC_EXTEND_EXISTING_TYPE_EX(existingType, bRefType)\
	BEHAVIAC_OVERRIDE_TYPE_NAME(existingType);					\
    namespace behaviac											\
    {															\
        namespace Meta {										\
            template <>											\
            struct TIsRefType<existingType> {					\
                enum {											\
                    Result = bRefType							\
                };												\
            };													\
            template <>											\
			struct HasToString<existingType> {					\
				enum {											\
					Result = 1									\
				};												\
			};													\
            template <>											\
			struct HasFromString<existingType> {				\
				enum {											\
					Result = 1									\
				};												\
			};													\
        }														\
    }


//BEHAVIAC_DECLARE_OBJECT is only used for IList and System::Object,
//in general, please use DECLARE_BEHAVIAC_STRUCT, unless you know what you are doing
#define BEHAVIAC_DECLARE_OBJECT(classFullNameWithNamespace)					\
    DECLARE_BEHAVIAC_OBJECT_BASE(classFullNameWithNamespace, false)			\
    static const char* GetClassTypeName()									\
    { return #classFullNameWithNamespace; }

namespace behaviac {
    /////////////////////////////////////////////////////////////////////////////////////////
    struct EnumValueItem_t {
        behaviac::string		nativeName;
        behaviac::string		name;
        behaviac::wstring		m_displayName;
        behaviac::wstring		m_desc;

        EnumValueItem_t& DISPLAYNAME(const wchar_t* _displayName) {
            m_displayName = _displayName;

            return *this;
        }

        EnumValueItem_t& DESC(const wchar_t* _desc) {
            m_desc = _desc;

            return *this;
        }

        EnumValueItem_t& DISPLAY_INFO(const wchar_t* _displayName, const wchar_t* _desc = 0) {
            m_displayName = _displayName;

            if (_desc) {
                m_desc = _desc;

            } else {
                m_desc = m_displayName;
            }

            return *this;
        }
    };

    typedef behaviac::map<uint32_t, EnumValueItem_t>	EnumValueNameMap_t;

    class EnumClassDescription_t : public CTagTypeDescriptor {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(EnumClassDescription_t, CTagTypeDescriptor);

        EnumValueNameMap_t		valueMaps;
        behaviac::wstring		m_displayName;
        behaviac::wstring		m_desc;
    };

    //template<typename T>
    //inline const EnumClassDescription_t& GetEnumClassValueNames(T*p)
    //{
    //	BEHAVIAC_ASSERT(0);
    //    return *((EnumClassDescription_t*)0);
    //}
    template<typename T>
    const EnumClassDescription_t& GetEnumClassValueNames(T* p);

    template<typename T>
    inline behaviac::string EnumValueToString(const T& v) {
        const EnumClassDescription_t& ecd = behaviac::GetEnumClassValueNames((T*)0);

        for (EnumValueNameMap_t::const_iterator it = ecd.valueMaps.begin(); it != ecd.valueMaps.end(); ++it) {
            const EnumValueItem_t& valueItem = it->second;

            if (((T)it->first) == v) {
                return valueItem.name;
            }
        }

        return "NotAnEnum";
    }

    template<typename T>
    inline bool EnumValueFromString(const char* valueStr, T& v) {
        const EnumClassDescription_t& ecd = behaviac::GetEnumClassValueNames((T*)0);

        for (EnumValueNameMap_t::const_iterator it = ecd.valueMaps.begin(); it != ecd.valueMaps.end(); ++it) {
            const EnumValueItem_t& valueItem = it->second;

            const char* pItemStr = valueItem.name.c_str();

            if (behaviac::StringUtils::StringEqual(valueStr, pItemStr)) {
                v = (T)it->first;
                return true;
            }
        }

        return false;
    }

    struct EnumClassDescriptionAuto_t {
        EnumClassDescription_t* descriptor;
    };

    typedef behaviac::map<behaviac::string, const EnumClassDescriptionAuto_t*>	EnumClassMap_t;
    BEHAVIAC_API EnumClassMap_t& GetEnumValueNameMaps();
    BEHAVIAC_API void CleanupEnumValueNameMaps();
}//

//you need to accompany DECLARE_BEHAVIAC_ENUM(ENUMCLASS_FullNameWithNamespace)
//in the cpp, BEHAVIAC_BEGIN_ENUM/BEHAVIAC_END_ENUM
//DECLARE_BEHAVIAC_ENUM(namespace::ENUMCLASS_FullNameWithNamespace, EnumClass) should be defined in the global namespace.
#define DECLARE_BEHAVIAC_ENUM_EX(ENUMCLASS_FullNameWithNamespace, EnumClassName)														\
    BEHAVIAC_OVERRIDE_TYPE_NAME(ENUMCLASS_FullNameWithNamespace);																	\
    BEHAVIAC_API behaviac::EnumClassDescriptionAuto_t& EnumClassName##GetEnumClassValueNames();\
    BEHAVIAC_API void RegisterEnumClass(ENUMCLASS_FullNameWithNamespace*);		\
    namespace behaviac {														\
        template<>																\
        BEHAVIAC_FORCEINLINE  const behaviac::EnumClassDescription_t& GetEnumClassValueNames<ENUMCLASS_FullNameWithNamespace>(ENUMCLASS_FullNameWithNamespace*p)\
        {																		\
            RegisterEnumClass(p);												\
            behaviac::EnumClassDescriptionAuto_t& descriptorAuto = EnumClassName##GetEnumClassValueNames();\
            return *descriptorAuto.descriptor;									\
        }																		\
    }																			\
    template< typename SWAPPER >												\
    inline void SwapByteImplement(ENUMCLASS_FullNameWithNamespace& s)			\
    {																			\
        int t = (int)s;															\
        behaviacSwapByte(t);													\
        s = (ENUMCLASS_FullNameWithNamespace)t;									\
    }

#define BEHAVIAC_BEGIN_ENUM_EX(ENUMCLASS, EnumClassName)							\
    behaviac::EnumClassDescriptionAuto_t& EnumClassName##GetEnumClassValueNames()\
    {																			\
        static behaviac::EnumClassDescriptionAuto_t s_ValueNameMap;				\
        if (!s_ValueNameMap.descriptor)											\
        {																		\
            s_ValueNameMap.descriptor = BEHAVIAC_NEW behaviac::EnumClassDescription_t;	\
        }																		\
        return s_ValueNameMap;													\
    }																			\
    void RegisterEnumClass(ENUMCLASS*)											\
    {																			\
        const char* enumClassName = behaviac::GetClassTypeName((ENUMCLASS*)0);	\
        behaviac::EnumClassMap_t& maps = behaviac::GetEnumValueNameMaps();		\
        behaviac::EnumClassMap_t::iterator it = maps.find(enumClassName);		\
        if (it != maps.end())													\
        {																		\
            return;																\
        }																		\
        behaviac::EnumClassDescriptionAuto_t& enumClassDescAuto = EnumClassName##GetEnumClassValueNames();\
        maps[enumClassName] = &enumClassDescAuto;								\
        behaviac::EnumClassDescription_t& enumClassDesc = *enumClassDescAuto.descriptor;

#define BEHAVIAC_ENUMCLASS_DISPLAY_INFO_EX(displayName_, desc_)		\
    enumClassDesc.m_displayName = displayName_;						\
    if (desc_) {enumClassDesc.m_desc = desc_;}						\
    else {enumClassDesc.m_desc = displayName_;}

namespace behaviac {
    inline EnumValueItem_t& _defineEnumName(EnumClassDescription_t& ecd, uint32_t value, const char* nativeName, const char* name) {
        EnumValueItem_t& e = ecd.valueMaps[value];
        e.nativeName = nativeName;
        e.name = name;

        return e;
    }
}

#define BEHAVIAC_ENUMCLASS_DISPLAYNAME_EX(displayName_)    enumClassDesc.m_displayName = displayName_;
#define BEHAVIAC_ENUMCLASS_DESC_EX(desc_)    enumClassDesc.m_desc = desc_;

#define BEHAVIAC_ENUM_ITEM_EX(value, name)	behaviac::_defineEnumName(enumClassDesc, value, #value, name)
#define BEHAVIAC_END_ENUM_EX()         }

/////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
// deprecated
#define DECLARE_BEHAVIAC_ENUM(ENUMCLASS_FullNameWithNamespace, EnumClassName)	
#define BEHAVIAC_BEGIN_ENUM(ENUMCLASS, EnumClassName) namespace behaviac
#define BEHAVIAC_ENUMCLASS_DISPLAY_INFO(displayName_, desc_)
#define BEHAVIAC_ENUMCLASS_DISPLAYNAME(displayName_)
#define BEHAVIAC_ENUMCLASS_DESC(desc_)
#define BEHAVIAC_ENUM_ITEM(value, name)
#define BEHAVIAC_END_ENUM()

#define BEGIN_ENUM_DESCRIPTION BEHAVIAC_BEGIN_ENUM
#define END_ENUM_DESCRIPTION BEHAVIAC_END_ENUM
#define DEFINE_ENUM_VALUE BEHAVIAC_ENUM_ITEM

#define DECLARE_BEHAVIAC_AGENT(agent, base) BEHAVIAC_DECLARE_AGENTTYPE(agent, base)
#define DECLARE_BEHAVIAC_OBJECT(agent, base) BEHAVIAC_DECLARE_AGENTTYPE(agent, base)

#define BEHAVIAC_BEGIN_PROPERTIES(struct_type) BEHAVIAC_BEGIN_STRUCT(struct_type)
#define BEHAVIAC_REGISTER_METHOD(method)
#define BEHAVIAC_REGISTER_PROPERTY(member) BEHAVIAC_REGISTER_STRUCT_PROPERTY(member)
#define BEHAVIAC_END_PROPERTIES() BEHAVIAC_END_STRUCT()

#define BEGIN_PROPERTIES_DESCRIPTION(agent)
#define END_PROPERTIES_DESCRIPTION()
#define BEHAVIAC_DECLARE_TYPE_VECTOR_HANDLER(type)

//deprecated, to use DECLARE_BEHAVIAC_ENUM
#define DECLARE_BEHAVIAC_OBJECT_ENUM DECLARE_BEHAVIAC_ENUM

#endif // #ifndef _BEHAVIAC_COMMON_OBJECT_TAGOBJECT_H_
