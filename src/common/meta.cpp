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
#include "behaviac/common/meta.h"
#include "behaviac/common/member.h"
#include "behaviac/common/file/filesystem.h"

#if _MSC_VER
#define ConvertStringToLongLong _atoi64
#else
#define ConvertStringToLongLong atoll
#endif

namespace behaviac {
    behaviac::map<uint32_t, AgentMeta*>             AgentMeta::_agentMetas;
    behaviac::map<behaviac::string, TypeCreator*>   AgentMeta::_Creators;
    BehaviorLoader*                                 AgentMeta::ms_behaviorLoader = NULL;
    unsigned int									AgentMeta::ms_totalSignature = 0;

    const size_t kNameLength = 256;

    void AgentMeta::RegisterMeta() {
        if (ms_behaviorLoader) {
            bool result = ms_behaviorLoader->load();
            BEHAVIAC_UNUSED_VAR(result);
            BEHAVIAC_ASSERT(result);
        }
    }

    void AgentMeta::UnRegisterMeta() {
        bool result = ms_behaviorLoader->unLoad();
        BEHAVIAC_UNUSED_VAR(result);
        BEHAVIAC_ASSERT(result);
    }

    AgentMeta::AgentMeta(unsigned int signature)
        : m_signature(signature) {
    }

    AgentMeta::~AgentMeta() {
        for (behaviac::map<uint32_t, IProperty*>::iterator it = _memberProperties.begin(); it != _memberProperties.end(); ++it) {
            IProperty* p = it->second;
            BEHAVIAC_DELETE p;
        }

        for (behaviac::map<uint32_t, IProperty*>::iterator it = _customizedProperties.begin(); it != _customizedProperties.end(); ++it) {
            IProperty* p = it->second;
            BEHAVIAC_DELETE p;
        }

        for (behaviac::map<uint32_t, IProperty*>::iterator it = _customizedStaticProperties.begin(); it != _customizedStaticProperties.end(); ++it) {
            IProperty* p = it->second;
            BEHAVIAC_DELETE p;
        }

        for (behaviac::map<uint32_t, IInstantiatedVariable*>::iterator it = _customizedStaticVars.begin(); it != _customizedStaticVars.end(); ++it) {
            //IInstantiatedVariable* p = it->second;
            //BEHAVIAC_DELETE p;
        }

        for (behaviac::map<uint32_t, IInstanceMember*>::iterator it = _methods.begin(); it != _methods.end(); ++it) {
            IInstanceMember* p = it->second;
            BEHAVIAC_DELETE p;
        }

        _memberProperties.clear();
        _customizedProperties.clear();
        _customizedStaticProperties.clear();
        _customizedStaticVars.clear();
        _methods.clear();
    }

    void AgentMeta::SetTotalSignature(unsigned int value) {
        ms_totalSignature = value;
    }

    unsigned int AgentMeta::GetTotalSignature() {
        return ms_totalSignature;
    }

    unsigned int AgentMeta::GetSignature() {
        return m_signature;
    }

    void AgentMeta::Register() {
        RegisterBasicTypes();

        RegisterMeta();

        LoadAllMetaFiles();
    }

    void AgentMeta::UnRegister() {
        UnRegisterBasicTypes();
        UnRegisterMeta();

        for (behaviac::map<uint32_t, AgentMeta*>::iterator it = _agentMetas.begin(); it != _agentMetas.end(); ++it) {
            AgentMeta* p = it->second;
            BEHAVIAC_DELETE p;
        }

        _agentMetas.clear();

        _Creators.clear();
    }

    AgentMeta* AgentMeta::GetMeta(uint32_t classId) {
        if (_agentMetas.find(classId) != _agentMetas.end()) {
            return _agentMetas[classId];
        }

        return NULL;
    }

    behaviac::map<uint32_t, AgentMeta*>& AgentMeta::GetAgentMetas() {
        return _agentMetas;
    }

    void AgentMeta::SetBehaviorLoader(BehaviorLoader* cppBehaviorLoader) {
        ms_behaviorLoader = cppBehaviorLoader;
    }

    behaviac::map<uint32_t, IInstantiatedVariable*> AgentMeta::InstantiateCustomizedProperties() {
        behaviac::map<uint32_t, IInstantiatedVariable*> vars;

        for (behaviac::map<uint32_t, IProperty*>::iterator pair = _customizedProperties.begin(); pair != _customizedProperties.end(); ++pair) {
            vars[pair->first] = pair->second->Instantiate();
        }

        if (_customizedStaticVars.size() == 0) {
            for (behaviac::map<uint32_t, IProperty*>::iterator pair = _customizedStaticProperties.begin(); pair != _customizedStaticProperties.end(); ++pair) {
                _customizedStaticVars[pair->first] = pair->second->Instantiate();
            }
        }

        for (behaviac::map<uint32_t, IInstantiatedVariable*>::iterator pair = _customizedStaticVars.begin(); pair != _customizedStaticVars.end(); ++pair) {
            vars[pair->first] = pair->second;
        }

        return vars;
    }

    void AgentMeta::RegisterMemberProperty(uint32_t propId, IProperty* property) {
        _memberProperties[propId] = property;
    }

    void AgentMeta::DestroyCustomizedProperty(uint32_t propId) {
        behaviac::map<uint32_t, IProperty*>::iterator it = _customizedProperties.find(propId);

        if (it != _customizedProperties.end()) {
            IProperty* p = it->second;
            BEHAVIAC_DELETE p;
        } else {
            it = _customizedStaticProperties.find(propId);

            if (it != _customizedStaticProperties.end()) {
                IProperty* p = it->second;
                BEHAVIAC_DELETE p;
            }
        }
    }

    void AgentMeta::RegisterCustomizedProperty(uint32_t propId, IProperty* property) {
        DestroyCustomizedProperty(propId);

        _customizedProperties[propId] = property;
    }

    void AgentMeta::RegisterStaticCustomizedProperty(uint32_t propId, IProperty* property) {
        DestroyCustomizedProperty(propId);

        _customizedStaticProperties[propId] = property;
    }

    void AgentMeta::RegisterMethod(uint32_t methodId, IInstanceMember* method) {
        _methods[methodId] = method;
    }

    IProperty* AgentMeta::GetProperty(uint32_t propId) {
        if (_customizedStaticProperties.find(propId) != _customizedStaticProperties.end()) {
            return _customizedStaticProperties[propId];
        }

        if (_customizedProperties.find(propId) != _customizedProperties.end()) {
            return _customizedProperties[propId];
        }

        if (_memberProperties.find(propId) != _memberProperties.end()) {
            return _memberProperties[propId];
        }

        return NULL;
    }

    IProperty* AgentMeta::GetMemberProperty(uint32_t propId) {
        if (_memberProperties.find(propId) != _memberProperties.end()) {
            return _memberProperties[propId];
        }

        return NULL;
    }

    const behaviac::map<uint32_t, IProperty*>& AgentMeta::GetMemberProperties() {
        return _memberProperties;
    }

    IInstanceMember* AgentMeta::GetMethod(uint32_t methodId) {
        if (_methods.find(methodId) != _methods.end()) {
            return _methods[methodId];
        }

        return NULL;
    };

	static string GetTypeName(const char* typeName) {
		string typeNameStr = typeName;
		
		if (!StringUtils::Compare(typeName, "char*") && !StringUtils::Compare(typeName, "const char*") &&
			!StringUtils::Compare(typeName, "vector<char*>") && !StringUtils::Compare(typeName, "vector<const char*>")) {
			StringUtils::ReplaceStringInPlace(typeNameStr, "*", "");
		}

		return typeNameStr;
	}

	bool AgentMeta::TypeNameIsRegistered(const char* typeName) {
		string typeNameStr = GetTypeName(typeName);

		if (_Creators.find(typeNameStr) != _Creators.end()) {
			return true;
		}
		else {
			return false;
		}
	}

	TypeCreator* AgentMeta::GetTypeCreator(const char* typeName) {
		string typeNameStr = GetTypeName(typeName);

		if (_Creators.find(typeNameStr) != _Creators.end()) {
			return _Creators[typeNameStr];
		}

		BEHAVIAC_ASSERT(false);
		return NULL;
	}

	void AgentMeta::AddTypeCreator(const char* typeName, TypeCreator* tc) {
		string typeNameStr = GetTypeName(typeName);

		if (_Creators.find(typeNameStr) == _Creators.end()) {
			_Creators[typeNameStr] = tc;
		}
	}

	void AgentMeta::RemoveTypeCreator(const char* typeName) {
		string typeNameStr = GetTypeName(typeName);

		if (_Creators.find(typeNameStr) != _Creators.end()) {
			BEHAVIAC_DELETE _Creators[typeNameStr];
			_Creators.erase(typeNameStr);
		}
	}

	IProperty* AgentMeta::CreateProperty(const char* typeName, uint32_t propId, const char* propName, const char* valueStr) {
		TypeCreator* creator = GetTypeCreator(typeName);
		if (creator) {
			return creator->CreateProperty(propId, propName, valueStr);
		}

        return NULL;
    }

	IProperty* AgentMeta::CreateArrayItemProperty(const char* typeName, uint32_t parentId, const char* parentName) {
		TypeCreator* creator = GetTypeCreator(typeName);
		if (creator) {
			return creator->CreateArrayItemProperty(parentId, parentName);
		}

        return NULL;
    }

	IInstanceMember* AgentMeta::CreateInstanceProperty(const char* typeName, const char* instance, IInstanceMember* indexMember, uint32_t varId) {
		TypeCreator* creator = GetTypeCreator(typeName);
		if (creator) {
			return creator->CreateInstanceProperty(instance, indexMember, varId);
		}

        return NULL;
    }

	IInstanceMember* AgentMeta::CreateInstanceConst(const char* typeName, const char* valueStr) {
		TypeCreator* creator = GetTypeCreator(typeName);
		if (creator) {
			return creator->CreateInstanceConst(valueStr);
		}

        return NULL;
    }

	IProperty* AgentMeta::CreateCustomizedProperty(const char* typeName, uint32_t propId, const char* propName, const char* valueStr) {
		TypeCreator* creator = GetTypeCreator(typeName);
		if (creator) {
			return creator->CreateCustomizedProperty(propId, propName, valueStr);
		}

        return NULL;
    }

	IProperty* AgentMeta::CreateCustomizedArrayItemProperty(const char* typeName, uint32_t parentId, const char* parentName) {
		TypeCreator* creator = GetTypeCreator(typeName);
		if (creator) {
			return creator->CreateCustomizedArrayItemProperty(parentId, parentName);
		}

        return NULL;
    }

    const char* AgentMeta::ParseInstanceNameProperty(const char* fullName, char* instanceName, char* agentType) {
        const char* pClassBegin = strchr(fullName, '.');
        size_t kInstanceLength = 256;

        if (pClassBegin) {
            size_t posClass = pClassBegin - fullName;
            BEHAVIAC_ASSERT(posClass < kInstanceLength);
            string_ncpy(instanceName, fullName, posClass);
            instanceName[posClass] = '\0';

            const char* pAgentType = pClassBegin + 1;

            const char* pPropertyName = ::strrchr(pAgentType, ':');
            BEHAVIAC_ASSERT(pPropertyName);
            size_t agentTypeLength = pPropertyName - 1 - pAgentType;
            string_ncpy(agentType, pAgentType, agentTypeLength);
            agentType[agentTypeLength] = '\0';

            const char* propertyName = pPropertyName + 1;

            return propertyName;
        }

        BEHAVIAC_UNUSED_VAR(kInstanceLength);

        return fullName;
    }

    void AgentMeta::RegisterBasicTypes() {
        Register<bool>("bool");
        Register<bool>("Boolean");
        Register<char>("byte");
        Register<unsigned char>("ubyte");
        Register<char>("Byte");
        Register<char>("char");
        Register<Char>("Char");
        Register<int>("decimal");
        Register<int>("Decimal");
        Register<double>("double");
        Register<int>("Double");
        Register<float>("float");
        Register<int>("int");
        Register<int16_t>("Int16");
        Register<int32_t>("Int32");
        Register<int64_t>("Int64");
        Register<long>("long");
        Register<signed long long>("llong");

        Register<signed char>("sbyte");
        Register<char>("SByte");
        Register<short>("short");
        Register<unsigned short>("ushort");

        Register<unsigned int>("uint");
        Register<uint16_t>("UInt16");
        Register<uint32_t>("UInt32");
        Register<uint64_t>("UInt64");
        Register<unsigned long>("ulong");
        Register<unsigned long long>("ullong");
        Register<float>("Single");
		Register<char*>("char*");
		Register<const char*>("const char*");
#if BEHAVIAC_USE_CUSTOMSTRING
        Register<behaviac::string>("string");
        Register<behaviac::string>("String");
#else
		Register<std::string>("string");
		Register<std::string>("String");
		Register<std::string>("std::string");
#endif
        Register<behaviac::Agent>("behaviac::Agent");
        Register<behaviac::EBTStatus>("behaviac::EBTStatus");
    }

    void AgentMeta::UnRegisterBasicTypes() {

        UnRegister<bool>("bool");
        UnRegister<bool>("Boolean");
        UnRegister<char>("byte");
        UnRegister<unsigned char>("ubyte");
        UnRegister<char>("Byte");
        UnRegister<char>("char");
        UnRegister<Char>("Char");
        UnRegister<int>("decimal");
        UnRegister<int>("Decimal");
        UnRegister<double>("double");
        UnRegister<int>("Double");
        UnRegister<float>("float");
        UnRegister<int>("int");
        UnRegister<int16_t>("Int16");
        UnRegister<int32_t>("Int32");
        UnRegister<int64_t>("Int64");
        UnRegister<long>("long");
        UnRegister<signed long long>("llong");

        UnRegister<signed char>("sbyte");
        UnRegister<char>("SByte");
        UnRegister<short>("short");
        UnRegister<unsigned short>("ushort");

        UnRegister<unsigned int>("uint");
        UnRegister<uint16_t>("UInt16");
        UnRegister<uint32_t>("UInt32");
        UnRegister<uint64_t>("UInt64");
        UnRegister<unsigned long>("ulong");
        UnRegister<unsigned long long>("ullong");
        UnRegister<float>("Single");
		UnRegister<char*>("char*");
		UnRegister<const char*>("const char*");
#if BEHAVIAC_USE_CUSTOMSTRING
        UnRegister<behaviac::string>("string");
        UnRegister<behaviac::string>("String");
#else
		UnRegister<std::string>("string");
		UnRegister<std::string>("String");
		UnRegister<std::string>("std::string");
#endif
        UnRegister<behaviac::Agent>("behaviac::Agent");
        UnRegister<behaviac::EBTStatus>("behaviac::EBTStatus");
    }

    IInstanceMember* AgentMeta::ParseProperty(const char* value) {
        if (StringUtils::IsNullOrEmpty(value)) {
            return NULL;
        }

        behaviac::vector<behaviac::string> tokens = StringUtils::SplitTokens(value);
        behaviac::string typeName = "";

        if (tokens[0] == "const") {
            // const Int32 0
            //BEHAVIAC_ASSERT(tokens.Count == 3);
            BEHAVIAC_ASSERT(tokens.size() == 3);//const type bulabula

            const int kConstLength = 5;
            const char* strRemaining = value + (kConstLength + 1);
            //int p = StringUtils::FirstToken(strRemaining, ' ', behaviac::string(typeName));
            const char* p = StringUtils::FirstToken(strRemaining, ' ', typeName);
            const char* strVale = (p + 1);

            // const
            return AgentMeta::CreateInstanceConst(typeName.c_str(), strVale);
        } else {
            behaviac::string propStr = "";
            behaviac::string indexPropStr = "";

            if (tokens[0] == "static") {
                // static float Self.AgentNodeTest::s_float_type_0
                // static float Self.AgentNodeTest::s_float_type_0[int Self.AgentNodeTest::par_int_type_2]
                BEHAVIAC_ASSERT(tokens.size() == 3 || tokens.size() == 4);

                typeName = tokens[1];
                propStr = tokens[2];

                if (tokens.size() == 4) { // array index
                    indexPropStr = tokens[3];
                }
            } else {
                // float Self.AgentNodeTest::par_float_type_1
                // float Self.AgentNodeTest::par_float_type_1[int Self.AgentNodeTest::par_int_type_2]
                BEHAVIAC_ASSERT(tokens.size() == 2 || tokens.size() == 3);

                typeName = tokens[0];
                propStr = tokens[1];

                if (tokens.size() == 3) { // array index
                    indexPropStr = tokens[2];
                }
            }

            behaviac::string arrayItem = "";
            IInstanceMember* indexMember = NULL;

            //if (!StringUtils::IsNullOrEmpty(indexPropStr))
            if (indexPropStr.length() > 0) {
                arrayItem = "[]";
                indexMember = TParseProperty<int>(indexPropStr.c_str());
            }

			size_t pointIndex = propStr.find(".");
            BEHAVIAC_ASSERT(pointIndex > 0);

            behaviac::string instantceName = propStr.substr(0, pointIndex);

            propStr = propStr.substr(pointIndex + 1);

			size_t lastIndex = propStr.rfind("::");
            BEHAVIAC_ASSERT(lastIndex > 0);

            behaviac::string className = propStr.substr(0, lastIndex);
            behaviac::string propName = propStr.substr(lastIndex + 2);

            uint32_t propId = MakeVariableId((propName + arrayItem).c_str());
            uint32_t classId = behaviac::MakeVariableId(className.c_str());

            AgentMeta* meta = AgentMeta::GetMeta(classId);

            if (meta != NULL) {
                // property
                IProperty* p = meta->GetProperty(propId);

                if (p != NULL) {
                    return p->CreateInstance(instantceName.c_str(), indexMember);
                }
            }

            // local var
            return AgentMeta::CreateInstanceProperty(typeName.c_str(), instantceName.c_str(), indexMember, propId);
        }

        return NULL;
    }

    IInstanceMember* AgentMeta::ParseMethod(const char* valueStr, char* methodName) {
        //Self.test_ns::AgentActionTest::Action2(0)
        if (StringUtils::IsNullOrEmpty(valueStr) || (valueStr[0] == '\"' && valueStr[1] == '\"')) {
            return NULL;
        }

        char agentIntanceName[kNameLength] = { 0 };
        char agentClassName[kNameLength] = { 0 };
        const char* pBeginP = ParseMethodNames(valueStr, agentIntanceName, agentClassName, methodName);

        uint32_t agentClassId = MakeVariableId(agentClassName);
        uint32_t methodId = MakeVariableId(methodName);

        AgentMeta* meta = AgentMeta::GetMeta(agentClassId);
        BEHAVIAC_ASSERT(meta != NULL, "AgentMeta %s is not registered!", agentClassName);

        if (meta) {
            IInstanceMember* method = meta->GetMethod(methodId);

            if (method == NULL) {
                BEHAVIAC_ASSERT(false, "Method of %s::%s is not registered!\n", agentClassName, methodName);
            } else {
                method = (IInstanceMember*)(method->clone());

                const char* paramsStr = pBeginP;
                BEHAVIAC_ASSERT(paramsStr[0] == '(', "Method %s: '(' expected", methodName);

                behaviac::vector<behaviac::string> paramsTokens;
                size_t len = strlen(paramsStr);
                BEHAVIAC_ASSERT(paramsStr[len - 1] == ')', "Method %s: ')' expected", methodName);

                char text[1024] = { 0 };
                strncpy(text, paramsStr + 1, len - 2);
                paramsTokens = ParseForParams(text);

                method->load(agentIntanceName, paramsTokens);
            }

            return method;
        }

        return 0;
    }

    IInstanceMember* AgentMeta::ParseMethod(const char* valueStr) {
        char methodName[kNameLength] = { 0 };
        return ParseMethod(valueStr, methodName);
    }

    const char* AgentMeta::strrchr(const char* start, const char* end, char c) {
        while (end > start) {
            if (*end == c) {
                return end;
            }

            end--;
        }

        return 0;
    }

    const char* AgentMeta::ParseMethodNames(const char* fullName, char* agentIntanceName, char* agentClassName, char* methodName) {
        const char*  pClassBegin = strchr(fullName, '.');
        BEHAVIAC_ASSERT(pClassBegin);

        size_t posClass = pClassBegin - fullName;
        BEHAVIAC_ASSERT(posClass < kNameLength);
        string_ncpy(agentIntanceName, fullName, posClass);
        agentIntanceName[posClass] = '\0';

        const char* pBeginAgentClass = pClassBegin + 1;

        const char* pBeginP = strchr(pBeginAgentClass, '(');
        BEHAVIAC_ASSERT(pBeginP);

        //test_ns::AgentActionTest::Action2(0)
        const char* pBeginMethod = strrchr(pBeginAgentClass, pBeginP, ':');
        BEHAVIAC_ASSERT(pBeginMethod);
        //skip '::'
        BEHAVIAC_ASSERT(pBeginMethod[0] == ':' && pBeginMethod[-1] == ':');
        pBeginMethod += 1;

        size_t pos1 = pBeginP - pBeginMethod;
        BEHAVIAC_ASSERT(pos1 < kNameLength);

        string_ncpy(methodName, pBeginMethod, pos1);
        methodName[pos1] = '\0';

        size_t pos = pBeginMethod - 2 - pBeginAgentClass;
        BEHAVIAC_ASSERT(pos < kNameLength);

        string_ncpy(agentClassName, pBeginAgentClass, pos);
        agentClassName[pos] = '\0';

        return pBeginP;
    }

    behaviac::vector<behaviac::string> AgentMeta::ParseForParams(const char* tsrc) {
		size_t tsrcLen = strlen(tsrc);
		size_t startIndex = 0;
		size_t index = 0;
        int quoteDepth = 0;

        behaviac::vector<behaviac::string> params_;

        for (; index < tsrcLen; ++index) {
            if (tsrc[index] == '"') {
                quoteDepth++;

                if ((quoteDepth & 0x1) == 0) {
                    //closing quote
                    quoteDepth -= 2;
                    BEHAVIAC_ASSERT(quoteDepth >= 0);
                }
            } else if (quoteDepth == 0 && tsrc[index] == ',') {
                //skip ',' inside quotes, like "count, count"
				size_t lengthTemp = index - startIndex;
                //const char* strTemp = tsrc.Substring(startIndex, lengthTemp);
                char strTemp[1024] = { 0 };
                strncpy(strTemp, tsrc + startIndex, lengthTemp);
                params_.push_back(behaviac::string(strTemp));
                startIndex = index + 1;
            }
        }//end for

        // the last param
		size_t lengthTemp0 = index - startIndex;

        if (lengthTemp0 > 0) {
            //const char* strTemp = tsrc.Substring(startIndex, lengthTemp0)
            char strTemp[1024] = { 0 };
            strncpy(strTemp, tsrc + startIndex, lengthTemp0);
            strTemp[lengthTemp0] = '\0';
            params_.push_back(strTemp);
        }

        return params_;
    }

    void AgentMeta::LoadAllMetaFiles() {
        string metaFolder = StringUtils::CombineDir(Workspace::GetInstance()->GetFilePath(), "meta");

        if (Workspace::GetInstance()->GetMetaFile_()) {
			string filename = StringUtils::CombineDir(metaFolder.c_str(), Workspace::GetInstance()->GetMetaFile_());
			size_t index = filename.find(".meta");

			if (index == (size_t)-1) {
                filename += ".meta";
            }

            LoadMeta(filename);
        } else {
            const char* ext = (Workspace::GetInstance()->GetFileFormat() == Workspace::EFF_bson) ? ".bson.bytes" : ".xml";
            vector<string> allFiles;
            CFileSystem::ListFiles(allFiles, metaFolder.c_str(), false);

            for (unsigned int i = 0; i < allFiles.size(); ++i) {
                size_t index = allFiles[i].find(ext);

				if (index != (size_t)-1) {
                    index = allFiles[i].find(".meta");
                    BEHAVIAC_ASSERT(index > 0);
                    string filename = allFiles[i].substr(0, index + 5);

                    LoadMeta(filename);
                }
            }
        }
    }

    bool AgentMeta::LoadMeta(const string& metaFile) {
        //the workspace export path is provided by Workspace::GetFilePath
        //the file format(xml / bson) is provided by Workspace::GetFileFormat
        //generally, you need to derive Workspace and override GetFilePath and GetFileFormat,
        //then, instantiate your derived Workspace at the very beginning

        bool bLoadResult = false;
        Workspace::EFileFormat f = Workspace::GetInstance()->GetFileFormat();
        string ext = "";

        Workspace::GetInstance()->HandleFileFormat(metaFile, ext, f);

        switch (f) {
            case Workspace::EFF_bson: {
                uint32_t bufferSize = 0;
                char* pBuffer = Workspace::GetInstance()->ReadFileToBuffer(metaFile.c_str(), ext.c_str(), bufferSize);

                if (pBuffer != NULL) {
                    bLoadResult = load_bson(pBuffer);

                    Workspace::GetInstance()->PopFileFromBuffer(metaFile.c_str(), ext.c_str(), pBuffer, bufferSize);
                } else {
                    BEHAVIAC_ASSERT(false);
                }
            }
            break;

            //case Workspace::EFF_xml:
            default: {
                uint32_t bufferSize = 0;
                char* pBuffer = Workspace::GetInstance()->ReadFileToBuffer(metaFile.c_str(), ext.c_str(), bufferSize);

                if (pBuffer != NULL) {
                    bLoadResult = load_xml(pBuffer);

                    Workspace::GetInstance()->PopFileFromBuffer(metaFile.c_str(), ext.c_str(), pBuffer, bufferSize);
                } else {
                    BEHAVIAC_LOGERROR("'%s%s' doesn't exist! Please check the file name or override Workspace and its GetFilePath()\n", metaFile.c_str(), ext.c_str());
                    BEHAVIAC_ASSERT(false);
                }
            }
            break;
        }

        return bLoadResult;
    }

    void AgentMeta::registerCustomizedProperty(AgentMeta* meta, const char* propName, const char* typeName, const char* valueStr, bool isStatic) {
        uint32_t nameId = MakeVariableId(propName);
		IProperty* prop = meta->GetProperty(nameId);
		string typeNameStr = typeName;
		IProperty* newProp = AgentMeta::CreateCustomizedProperty(typeName, nameId, propName, valueStr);

		if (prop && newProp)
		{
			const char* newTypeName = newProp->GetClassNameString();
			const char* oldTypeName = prop->GetClassNameString();

			if (StringUtils::StringEqual(oldTypeName, newTypeName))
			{
				return;
			}

			BEHAVIAC_LOGWARNING("The type of '%s' has been modified to %s, which would bring the unpredictable consequences.\n", propName, typeName);
			BEHAVIAC_ASSERT(false);
		}

        if (isStatic) {
			meta->RegisterStaticCustomizedProperty(nameId, newProp);
        } else {
			meta->RegisterCustomizedProperty(nameId, newProp);
        }

		size_t index = typeNameStr.find("vector<");

        if (index == 0) { // array type
            // Get item type, i.e. vector<int>
            const size_t kVecLen = ::strlen("vector<");
            typeNameStr = typeNameStr.substr(kVecLen, typeNameStr.length() - kVecLen - 1); // item type
            IProperty* arrayItemProp = AgentMeta::CreateCustomizedArrayItemProperty(typeNameStr.c_str(), nameId, propName);
            string araryItemPropName = propName;
            araryItemPropName += "[]";
            nameId = MakeVariableId(araryItemPropName.c_str());

            if (isStatic) {
                meta->RegisterStaticCustomizedProperty(nameId, arrayItemProp);
            } else {
                meta->RegisterCustomizedProperty(nameId, arrayItemProp);
            }
        }
    }

    bool AgentMeta::checkSignature(const char* signatureStr) {
		long long signature = signatureStr ? ConvertStringToLongLong(signatureStr) : -1;

        if (signature != (long long)AgentMeta::GetTotalSignature()) {
            const char* errorInfo = "[meta] The types/internal/behaviac_agent_meta.cpp should be exported from the behaviac designer, and then integrated into your project!\n";

            BEHAVIAC_LOGWARNING(errorInfo);
			BEHAVIAC_ASSERT(false, errorInfo);

            return false;
        }

        return true;
    }

    bool AgentMeta::load_xml(char* pBuffer) {
        BEHAVIAC_ASSERT(pBuffer != NULL);
        rapidxml::xml_document<> doc;
        doc.parse<0>(pBuffer);
        rapidxml::xml_node<>* rootNode = doc.first_node("agents");

        if (rootNode == 0 || !StringUtils::StringEqual(rootNode->name(), "agents")) {
            return false;
        }

        // const char* versionStr = rootNode->first_attribute("version")->value();
        const char* signatureStr = rootNode->first_attribute("signature")->value();

        // don't use its return value, just let it report errors
        checkSignature(signatureStr);

        rapidxml::xml_node<>* children = rootNode->first_node();

        if (children == 0) {
            return false;
        }

        for (rapidxml::xml_node<>* bbNode = children; bbNode; bbNode = bbNode->next_sibling()) {
            if (StringUtils::StringEqual(bbNode->name(), "agent") && bbNode->first_node() != NULL) {
                const char* agentType = bbNode->first_attribute("type")->value();
                uint32_t agentClassId = MakeVariableId(agentType);
                AgentMeta* meta = AgentMeta::GetMeta(agentClassId);

                if (meta == NULL) {
                    meta = BEHAVIAC_NEW AgentMeta();
                    AgentMeta::GetAgentMetas()[agentClassId] = meta;
                }

				const char* agentSignature = bbNode->first_attribute("signature")->value();
				if (agentSignature)
				{
					long long signature = ConvertStringToLongLong(agentSignature);
					if (signature == (long long)meta->GetSignature())
					{
						continue;
					}
				}

                for (rapidxml::xml_node<>* propertiesNode = bbNode->first_node(); propertiesNode; propertiesNode = propertiesNode->next_sibling()) {
                    const char* propertiesNodeTag = propertiesNode->name();

                    if (StringUtils::StringEqual(propertiesNodeTag, "properties") && propertiesNode->first_node() != NULL) {
                        for (rapidxml::xml_node<>* propertyNode = propertiesNode->first_node(); propertyNode; propertyNode = propertyNode->next_sibling()) {
                            if (StringUtils::StringEqual(propertyNode->name(), "property")) {
                                const char* memberStr = propertyNode->first_attribute("member")->value();
                                bool bIsMember = (!StringUtils::IsNullOrEmpty(memberStr) && StringUtils::StringEqual(memberStr, "true"));

                                if (!bIsMember) {
                                    const char* propName = propertyNode->first_attribute("name")->value();
                                    const char* propType = propertyNode->first_attribute("type")->value();
                                    const char* valueStr = propertyNode->first_attribute("defaultvalue")->value();
                                    const char* isStatic = propertyNode->first_attribute("static")->value();
                                    bool bIsStatic = (!StringUtils::IsNullOrEmpty(isStatic) && StringUtils::StringEqual(isStatic, "true"));

                                    registerCustomizedProperty(meta, propName, propType, valueStr, bIsStatic);
                                }
                            }
                        }
                    }
                }//end of for propertiesNode
            }
        }//end of for bbNode

        return true;
    }

    bool AgentMeta::load_bson(const char* pBuffer) {
        BsonDeserizer* d = BEHAVIAC_NEW BsonDeserizer();

        if (d->Init(pBuffer)) {
            BsonDeserizer::BsonTypes type = d->ReadType();

            if (type == BsonDeserizer::BT_AgentsElement) {
                bool bOk = d->OpenDocument();
                BEHAVIAC_UNUSED_VAR(bOk);
                BEHAVIAC_ASSERT(bOk);

                const char* verStr = d->ReadString(); // version
                int version = atoi(verStr);

                const char* signatureStr = d->ReadString(); // signature
                checkSignature(signatureStr);

                {
                    type = d->ReadType();

                    while (type != BsonDeserizer::BT_None) {
                        if (type == BsonDeserizer::BT_AgentElement) {
                            load_agent(version, d);
                        }

                        type = d->ReadType();
                    }

                    BEHAVIAC_ASSERT(type == BsonDeserizer::BT_None);
                }

                d->CloseDocument(false);
                return true;
            }

            BEHAVIAC_DELETE d;
        }

        BEHAVIAC_ASSERT(false);
        return false;
    }

    bool AgentMeta::load_agent(int version, BsonDeserizer* d) {
        BEHAVIAC_UNUSED_VAR(version);
        d->OpenDocument();

        const char* agentType = d->ReadString();
        const char* pBaseName = d->ReadString();
        BEHAVIAC_UNUSED_VAR(pBaseName);

        uint32_t agentClassId = MakeVariableId(agentType);
        AgentMeta* meta = AgentMeta::GetMeta(agentClassId);

        if (meta == NULL) {
            meta = BEHAVIAC_NEW AgentMeta();
            AgentMeta::GetAgentMetas()[agentClassId] = meta;
        }

		bool signatrueChanged = false;
		const char* agentSignature = d->ReadString(); // signature
		if (agentSignature)
		{
			long long signature = ConvertStringToLongLong(agentSignature);
			if (signature != (long long)meta->GetSignature())
			{
				signatrueChanged = true;
			}
		}

        BsonDeserizer::BsonTypes type = d->ReadType();

        while (type != BsonDeserizer::BT_None) {
            if (type == BsonDeserizer::BT_PropertiesElement) {
                d->OpenDocument();

				BsonDeserizer::BsonTypes internalType = d->ReadType();

				while (internalType != BsonDeserizer::BT_None) {
					if (internalType == BsonDeserizer::BT_PropertyElement) {
                        d->OpenDocument();
                        const char* propName = d->ReadString();
                        const char* propType = d->ReadString();

                        const char* memberStr = d->ReadString();
                        bool bIsMember = (!StringUtils::IsNullOrEmpty(memberStr) && StringUtils::StringEqual(memberStr, "true"));

                        const char* isStatic = d->ReadString();
                        bool bIsStatic = (!StringUtils::IsNullOrEmpty(isStatic) && StringUtils::StringEqual(isStatic, "true"));

						if (!bIsMember)
						{
							const char* valueStr = d->ReadString();

							if (signatrueChanged)
							{
								registerCustomizedProperty(meta, propName, propType, valueStr, bIsStatic);
							}
						}
						else
						{
							d->ReadString(); // agentTypeMember
						}

                        d->CloseDocument(true);
                    } else {
                        BEHAVIAC_ASSERT(false);
                    }

					internalType = d->ReadType();
                }//end of while

                d->CloseDocument(false);

            } else if (type == BsonDeserizer::BT_MethodsElement) {
                load_methods(d, agentType, type);
            } else {
                BEHAVIAC_ASSERT(type == BsonDeserizer::BT_None);
            }

            type = d->ReadType();
        }

        d->CloseDocument(false);
        return true;
    }

    void AgentMeta::load_methods(BsonDeserizer* d, const char* agentType, BsonDeserizer::BsonTypes type) {
		BEHAVIAC_UNUSED_VAR(agentType);

		d->OpenDocument();

        type = d->ReadType();

        while (type == BsonDeserizer::BT_MethodElement) {
            d->OpenDocument();

            const char* methodName = d->ReadString();
            //string returnTypeStr = d->ReadString();
            //returnTypeStr = returnTypeStr.Replace("::", ".");
            //string isStatic = d->ReadString();
            //string eventStr = d->ReadString();
            //bool bEvent = (eventStr == "true");
            const char* agentStr = d->ReadString();

            BEHAVIAC_UNUSED_VAR(methodName);
            BEHAVIAC_UNUSED_VAR(agentStr);

            type = d->ReadType();

            while (type == BsonDeserizer::BT_ParameterElement) {
                d->OpenDocument();

                const char* paramName = d->ReadString();
                const char* paramType = d->ReadString();

                BEHAVIAC_UNUSED_VAR(paramName);
                BEHAVIAC_UNUSED_VAR(paramType);

                d->CloseDocument(true);

                type = d->ReadType();
            }

            d->CloseDocument(false);
            type = d->ReadType();
        }

        d->CloseDocument(false);
    }
}
