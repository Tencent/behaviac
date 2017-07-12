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

#ifndef _BEHAVIAC_COMMON_META_H_
#define _BEHAVIAC_COMMON_META_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"

#include "behaviac/common/base.h"
#include "behaviac/common/string/stringutils.h"
#include "behaviac/common/meta/ifthenelse.h"
#include "behaviac/common/meta/pointertype.h"

#include "behaviac/common/meta/isstruct.h"
#include "behaviac/common/rttibase.h"
#include "behaviac/behaviortree/behaviortree.h"

namespace behaviac {
    class IProperty;
    class IInstantiatedVariable;
    class Type;
    class IInstanceMember;
    class BehaviorLoader;
    class BsonDeserizer;

    BEHAVIAC_API uint32_t MakeVariableId(const char* idString);

    class BEHAVIAC_API BehaviorLoader {
    public:
        virtual ~BehaviorLoader() {}
        virtual bool load() = 0;
        virtual bool unLoad() = 0;
    };

	BEHAVIAC_API void InitBehaviorLoader();
	BEHAVIAC_API void DestroyBehaviorLoader();

    class BEHAVIAC_API TypeCreator {
    public:
        typedef IProperty* (*PropertyCreator)(uint32_t propId, const char* propName, const char* valueStr);
        typedef IProperty* (*ArrayItemPropertyCreator)(uint32_t parentId, const char* parentName);
        typedef IInstanceMember* (*InstancePropertyCreator)(const char* instance, IInstanceMember* indexMember, uint32_t id);
        typedef IInstanceMember* (*InstanceConstCreator)(const char* value);
        typedef IProperty* (*CustomizedPropertyCreator)(uint32_t propId, const char* propName, const char* valueStr);
        typedef IProperty* (*CustomizedArrayItemPropertyCreator)(uint32_t parentId, const char* parentName);

        PropertyCreator _propertyCreator;
        ArrayItemPropertyCreator _arrayItemPropertyCreator;
        InstancePropertyCreator _instancePropertyCreator;
        InstanceConstCreator _instanceConstCreator;
        CustomizedPropertyCreator _customizedPropertyCreator;
        CustomizedArrayItemPropertyCreator _customizedArrayItemPropertyCreator;

    public:
        TypeCreator(PropertyCreator propCreator,
                    ArrayItemPropertyCreator arrayItemPropCreator,
                    InstancePropertyCreator instancePropertyCreator,
                    InstanceConstCreator instanceConstCreator,
                    CustomizedPropertyCreator customizedPropertyCreator,
                    CustomizedArrayItemPropertyCreator customizedArrayItemPropertyCreator) {
            _propertyCreator = propCreator;
            _arrayItemPropertyCreator = arrayItemPropCreator;
            _instancePropertyCreator = instancePropertyCreator;
            _instanceConstCreator = instanceConstCreator;
            _customizedPropertyCreator = customizedPropertyCreator;
            _customizedArrayItemPropertyCreator = customizedArrayItemPropertyCreator;
        }

        IProperty* CreateProperty(uint32_t propId, const char* propName, const char* valueStr) {
            return _propertyCreator(propId, propName, valueStr);
        }

        IProperty* CreateArrayItemProperty(uint32_t parentId, const char* parentName) {
            return _arrayItemPropertyCreator(parentId, parentName);
        }

        IInstanceMember* CreateInstanceProperty(const char* instance, IInstanceMember* indexMember, uint32_t id) {
            return _instancePropertyCreator(instance, indexMember, id);
        }

        IInstanceMember* CreateInstanceConst(const char* value) {
            return _instanceConstCreator(value);
        }

        IProperty* CreateCustomizedProperty(uint32_t propId, const char* propName, const char* valueStr) {
            return _customizedPropertyCreator(propId, propName, valueStr);
        }

        IProperty* CreateCustomizedArrayItemProperty(uint32_t parentId, const char* parentName) {
            return _customizedArrayItemPropertyCreator(parentId, parentName);
        }
    };

    template <typename T>
    struct RemovePointerForAgentType {
        typedef REAL_BASETYPE(T) BaseType;

        typedef typename behaviac::Meta::IfThenElse < Meta::IsAgent<BaseType>::Result,
                typename behaviac::Meta::RemovePtr<T>::Result, T >::Result Result;
    };

	template <typename T>
	struct SzStringMapper {
		typedef T Result;
	};

	template <>
	struct SzStringMapper<char*> {
		typedef behaviac::string Result;
	};

	template <>
	struct SzStringMapper<const char*> {
		typedef behaviac::string Result;
	};

    class BEHAVIAC_API AgentMeta {
    private:
        behaviac::map<uint32_t, IProperty*>                     _memberProperties;
        behaviac::map<uint32_t, IProperty*>                     _customizedProperties;
        behaviac::map<uint32_t, IProperty*>                     _customizedStaticProperties;
        behaviac::map<uint32_t, IInstantiatedVariable*>         _customizedStaticVars;
        behaviac::map<uint32_t, IInstanceMember*>               _methods;

        static behaviac::map<uint32_t, AgentMeta*>              _agentMetas;
        static BehaviorLoader*                                  ms_behaviorLoader;

        static behaviac::map<behaviac::string, TypeCreator*>    _Creators;

    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(AgentMeta);

        AgentMeta(unsigned int signature = 0);
        ~AgentMeta();

        static void Register();
        static void UnRegister();
        static void RegisterMeta();
        static void UnRegisterMeta();

        static AgentMeta*                                           GetMeta(uint32_t classId);
        static behaviac::map<uint32_t, AgentMeta*>&                 GetAgentMetas();
		static void                                                 SetBehaviorLoader(BehaviorLoader* cppBehaviorLoader);
        behaviac::map<uint32_t, IInstantiatedVariable*>             InstantiateCustomizedProperties();

        void RegisterMemberProperty(uint32_t propId, IProperty* property);
        void RegisterCustomizedProperty(uint32_t propId, IProperty* property);
        void RegisterStaticCustomizedProperty(uint32_t propId, IProperty* property);
        void RegisterMethod(uint32_t methodId, IInstanceMember* method);

        IProperty* GetProperty(uint32_t propId);
        IProperty* GetMemberProperty(uint32_t propId);
        const behaviac::map<uint32_t, IProperty*>& GetMemberProperties();
        IInstanceMember* GetMethod(uint32_t methodId);

        static IProperty* CreateProperty(const char* typeName, uint32_t propId, const char* propName, const char* valueStr);
		static IProperty* CreateArrayItemProperty(const char* typeName, uint32_t parentId, const char* parentName);
		static IInstanceMember* CreateInstanceProperty(const char* typeName, const char* instance, IInstanceMember* indexMember, uint32_t varId);
		static IInstanceMember* CreateInstanceConst(const char* typeName, const char* valueStr);
		static IProperty* CreateCustomizedProperty(const char* typeName, uint32_t propId, const char* propName, const char* valueStr);
		static IProperty* CreateCustomizedArrayItemProperty(const char* typeName, uint32_t parentId, const char* parentName);

		static TypeCreator* GetTypeCreator(const char* typeName);
		static void AddTypeCreator(const char* typeName, TypeCreator* tc);
		static void RemoveTypeCreator(const char* typeName);

        static const char* ParseInstanceNameProperty(const char* fullName, char* instanceName, char* agentType);

        template<typename T>
        static IProperty* CreatorProperty(uint32_t propId, const char* propName, const char* valueStr);

        template<typename T>
        static IProperty* CreatorArrayItemProperty(uint32_t parentId, const char* parentName);

        template<typename T>
        static IInstanceMember* CreatorInstanceProperty(const char* instance, IInstanceMember* indexMember, uint32_t varId);

        template<typename T>
        static IInstanceMember* CreatorInstanceConst(const char* value);

        template<typename T>
        static IProperty* CreatorCustomizedProperty(uint32_t propId, const char* propName, const char* valueStr);

        template<typename T>
        static IProperty* CreatorCustomizedArrayItemProperty(uint32_t parentId, const char* parentName);

        static bool TypeNameIsRegistered(const char* typeName);

        template<typename T>
        static bool Register(const char* typeName) {
            typedef typename behaviac::Meta::IsRefType<T>::MappedType MappedType;
            typedef typename behaviac::Meta::PointerType<MappedType>::Result ThenType;

            // if ptr type, to use the pointer type of its mapped type
            typedef typename behaviac::Meta::IfThenElse<behaviac::Meta::IsRefType<T>::Result, ThenType, T>::Result RegisteredType;

            if (TypeNameIsRegistered(typeName) == false) {
                TypeCreator* tc = BEHAVIAC_NEW TypeCreator(
                                      &CreatorProperty<RegisteredType>,
                                      &CreatorArrayItemProperty<RegisteredType>,
                                      &CreatorInstanceProperty<RegisteredType>,
                                      &CreatorInstanceConst<RegisteredType>,
                                      &CreatorCustomizedProperty<RegisteredType>,
                                      &CreatorCustomizedArrayItemProperty<RegisteredType>
                                  );

				AddTypeCreator(typeName, tc);

                char vectorTypeName[1024];
                string_sprintf(vectorTypeName, "vector<%s>", typeName);

                TypeCreator* tcl = BEHAVIAC_NEW TypeCreator(
                                       CreatorProperty<behaviac::vector<RegisteredType> >,
                                       CreatorArrayItemProperty<behaviac::vector<RegisteredType> >,
                                       CreatorInstanceProperty<behaviac::vector<RegisteredType> >,
                                       CreatorInstanceConst<behaviac::vector<RegisteredType> >,
                                       &CreatorCustomizedProperty<behaviac::vector<RegisteredType> >,
                                       &CreatorCustomizedArrayItemProperty<behaviac::vector<RegisteredType> >);

				AddTypeCreator(vectorTypeName, tcl);
            }

            return true;
        }

        template<typename T>
        static void UnRegister(const char* typeName) {
			RemoveTypeCreator(typeName);

            char vectorTypeName[1024];
            string_sprintf(vectorTypeName, "vector<%s>", typeName);

			RemoveTypeCreator(vectorTypeName);
        }

    private:
        static void RegisterBasicTypes();
        static void UnRegisterBasicTypes();

        void DestroyCustomizedProperty(uint32_t propId);

    public:
        template<typename T>
        static IInstanceMember* TParseProperty(const char* value) {
            if (StringUtils::IsNullOrEmpty(value)) {
                return NULL;
            }

			// struct
			bool isStructOrConst = StringUtils::StartsWith(value, "{");

			if (!isStructOrConst)
			{
				behaviac::vector<behaviac::string> tokens = StringUtils::SplitTokens(value);

				// const
				isStructOrConst = (tokens.size() == 1);
			}

			if (isStructOrConst)
			{
                typedef typename RemovePointerForAgentType<T>::Result Type_;
				typedef typename SzStringMapper<Type_>::Result Type;

				behaviac::string typeName = behaviac::GetTypeDescString<Type>();

				return AgentMeta::CreateInstanceConst(typeName.c_str(), value);
            }

            return ParseProperty(value);
        }

        static IInstanceMember* ParseProperty(const char* value);
        static IInstanceMember* ParseMethod(const char* valueStr, char* methodName);
        static IInstanceMember* ParseMethod(const char* valueStr);

        static const char* strrchr(const char* start, const char* end, char c);

        static void LoadAllMetaFiles();
        static bool LoadMeta(const string& metaFile);
        static bool load_xml(char* pBuffer);
        static bool load_bson(const char* pBuffer);
        static bool load_agent(int version, BsonDeserizer* d);
        static void load_methods(BsonDeserizer* d, const char* agentType, BsonDeserizer::BsonTypes type);
        static void registerCustomizedProperty(AgentMeta* meta, const char* propName, const char* typeName, const char* valueStr, bool isStatic);

        static void SetTotalSignature(unsigned int value);
        static unsigned int GetTotalSignature();

        unsigned int GetSignature();

    private:
        static unsigned int ms_totalSignature;
        unsigned int m_signature;

        static bool checkSignature(const char* signatureStr);

        static const char* ParseMethodNames(const char* fullName, char* agentIntanceName, char* agentClassName, char* methodName);

        //suppose params are seprated by ','
        static behaviac::vector<behaviac::string> ParseForParams(const char* tsrc);
    };
}
#endif // _BEHAVIAC_COMMON_META_H_
