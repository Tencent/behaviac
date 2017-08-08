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

#include "behaviac/common/object/tagobject.h"
#include "behaviac/common/object/member.h"
#include "behaviac/common/logger/logmanager.h"
#include "behaviac/common/member.h"

namespace behaviac {
    static EnumClassMap_t* s_enumClasses;
    EnumClassMap_t& GetEnumValueNameMaps() {
        if (!s_enumClasses) {
            s_enumClasses = BEHAVIAC_NEW EnumClassMap_t;
        }

        BEHAVIAC_ASSERT(s_enumClasses);
        return *s_enumClasses;
    }

    void CleanupEnumValueNameMaps() {
        if (s_enumClasses) {
            EnumClassMap_t* enumClasses = s_enumClasses;

            for (EnumClassMap_t::iterator it = enumClasses->begin(); it != enumClasses->end(); ++it) {
                EnumClassDescriptionAuto_t* pEnumClassD = (EnumClassDescriptionAuto_t*)it->second;
                pEnumClassD->descriptor->valueMaps.clear();
                BEHAVIAC_DELETE(pEnumClassD->descriptor);
                pEnumClassD->descriptor = 0;
            }

            s_enumClasses->clear();
            BEHAVIAC_DELETE(s_enumClasses);
            s_enumClasses = 0;
        }
    }

	void CTagObject::Load(const void* pObject, const behaviac::IIONode* node, const char* szClassName) {
		CStringCRC agentId(szClassName);
        AgentMeta* pAgentMeta = AgentMeta::GetMeta(agentId.GetUniqueID());

		if (pAgentMeta) {
			const behaviac::map<uint32_t, IProperty*>& members = pAgentMeta->GetMemberProperties();

			for (behaviac::map<uint32_t, IProperty*>::const_iterator it = members.begin(); it != members.end(); ++it) {
				// uint32_t memberId = it->first;
				const IProperty* pProperty = it->second;

				const char* szPropertyName = pProperty->Name();
				CIOID ioid(szPropertyName);

				const char* valueStr = node->getAttrRaw(ioid);

				if (valueStr) {
					pProperty->SetValueFromString((behaviac::Agent*)pObject, valueStr);
				}
				else {
					const behaviac::IIONode* childNode = node->findNodeChild(ioid);

					if (childNode) {
						CTagObject* pChildObject = (CTagObject*)pProperty->GetValue((behaviac::Agent*)pObject);
						const char* szPropertyClassName = pProperty->GetClassNameString();
						CTagObject::Load(pChildObject, childNode, szPropertyClassName);
					}
					else {
						BEHAVIAC_ASSERT(false);
					}
				}
			}
		}
    }

	void CTagObject::Save(const void* pObject, behaviac::IIONode* node, const char* szClassName) {
		CStringCRC agentId(szClassName);
        AgentMeta* pAgentMeta = AgentMeta::GetMeta(agentId.GetUniqueID());

		if (pAgentMeta) {
			const behaviac::map<uint32_t, IProperty*>& members = pAgentMeta->GetMemberProperties();

			for (behaviac::map<uint32_t, IProperty*>::const_iterator it = members.begin(); it != members.end(); ++it) {
				//uint32_t memberId = it->first;
				const IProperty* pProperty = it->second;
				if (!pProperty->IsArrayItem()) {
					behaviac::string valueStr = pProperty->GetValueToString((behaviac::Agent*)pObject);
					CIOID  attrId(pProperty->Name());
					node->setAttr(attrId, valueStr.c_str());
				}
			}
		}
    }

	bool Equal_Struct(void* lhs, void* rhs, const char* szClassName) {
		BEHAVIAC_UNUSED_VAR(lhs);
		BEHAVIAC_UNUSED_VAR(rhs);
		BEHAVIAC_UNUSED_VAR(szClassName);

		CStringCRC type_id(szClassName);
		AgentMeta* pAgentMeta = AgentMeta::GetMeta(type_id.GetUniqueID());

		if (pAgentMeta) {
			const behaviac::map<uint32_t, IProperty*>& members = pAgentMeta->GetMemberProperties();

			for (behaviac::map<uint32_t, IProperty*>::const_iterator it = members.begin(); it != members.end(); ++it) {
				const IProperty* pProperty = it->second;

				bool bEqual = pProperty->Equal((behaviac::Agent*)lhs, (behaviac::Agent*)rhs);

				if (!bEqual) {
					return false;
				}
			}
		}

		return true;
	}
}

namespace behaviac {
    namespace StringUtils {
        XmlNodeReference MakeXmlNodeStruct(const char* str, const behaviac::string& typeNameT) {
            behaviac::string src = str;

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            //the first char is '{'
            //the last char is '}'
            behaviac::string::size_type posCloseBrackets = behaviac::StringUtils::SkipPairedBrackets(src);
            BEHAVIAC_ASSERT(posCloseBrackets != behaviac::string::npos);

            bool bIsStructMember = false;
            XmlNodeReference xmlNode = CreateXmlNode(typeNameT.c_str());

            //{color=0;id=;type={bLive=false;name=0;weight=0;};}
            //{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
            behaviac::string::size_type posBegin = 1;
            behaviac::string::size_type posEnd = src.find_first_of(';', posBegin);

            while (posEnd != behaviac::string::npos) {
                BEHAVIAC_ASSERT(src[posEnd] == ';');

                //the last one might be empty
                if (posEnd > posBegin) {
                    behaviac::string::size_type posEqual = src.find_first_of('=', posBegin);
                    BEHAVIAC_ASSERT(posEqual > posBegin);

                    size_t length = posEqual - posBegin;
                    behaviac::string memmberName = src.substr(posBegin, length);
                    behaviac::string memmberValue;
                    char c = src[posEqual + 1];

                    if (c != '{') {
                        //to check if it is an array
						IsArrayString(src, posEqual + 1, posEnd);
						length = posEnd - posEqual - 1;
						memmberValue = src.substr(posEqual + 1, length);
                    } else {
                        bIsStructMember = true;

                        const char* pStructBegin = src.c_str();
                        pStructBegin += posEqual + 1;
                        const char* posCloseBrackets_ = behaviac::StringUtils::SkipPairedBrackets(pStructBegin);
                        length = posCloseBrackets_ - pStructBegin + 1;

                        memmberValue = src.substr(posEqual + 1, length);

                        posEnd = posEqual + 1 + length;
                    }

                    if (bIsStructMember) {
                        XmlNodeReference memberNode = MakeXmlNodeStruct(memmberValue.c_str(), memmberName);

                        xmlNode->addChild(memberNode);
                    } else {
                        //behaviac::string memmberNameFull = typeNameT + "::" + memmberName;
                        //xmlNode->setAttr(memmberNameFull.c_str(), memmberValue.c_str());
                        xmlNode->setAttr(memmberName.c_str(), memmberValue.c_str());
                    }
                }

                bIsStructMember = false;

                //skip ';'
                posBegin = posEnd + 1;

                //{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
                posEnd = src.find_first_of(';', posBegin);

                if (posEnd > posCloseBrackets) {
                    break;
                }
            }

            return xmlNode;
        }

        bool MakeStringFromXmlNodeStruct(behaviac::XmlConstNodeRef xmlNode, behaviac::string& result) {
            //xmlNode->getXML(result);
            result = "{";

            for (int a = 0; a < xmlNode->getAttrCount(); ++a) {
                const char* tag = xmlNode->getAttrTag(a);
                const char* value = xmlNode->getAttr(a);

                char temp[1024];
                string_sprintf(temp, "%s=%s;", tag, value);
                result += temp;
            }

            for (int c = 0; c < xmlNode->getChildCount(); ++c) {
                behaviac::XmlConstNodeRef childNode = xmlNode->getChild(c);

                behaviac::string childString;

                if (MakeStringFromXmlNodeStruct(childNode, childString)) {
                    result += childString;
                    result += ";";
                }
            }

            result += "}";

            return true;
        }

        //bool ParseForStruct(const char* str, behaviac::string& strT, behaviac::map<behaviac::CStringCRC, behaviac::IInstanceMember*>&  props) {
        //    const char* pB = str;

        //    while (*str) {
        //        char c = *str;

        //        if (c == ';' || c == '{' || c == '}') {
        //            const char* p = pB;

        //            while (p <= str) {
        //                strT += *p++;
        //            }

        //            pB = str + 1;

        //        } else if (c == ' ') {
        //            //par or property
        //            behaviac::string propName;
        //            const char* p = pB;

        //            while (*p != '=') {
        //                propName += *p++;
        //            }

        //            //skip '='
        //            BEHAVIAC_ASSERT(*p == '=');
        //            p++;

        //            behaviac::string typeStr;

        //            while (*p != ' ') {
        //                typeStr += *p++;
        //            }

        //            bool bStatic = false;

        //            if (typeStr == "static") {
        //                //skip ' '
        //                BEHAVIAC_ASSERT(*p == ' ');
        //                p++;

        //                while (*p != ' ') {
        //                    typeStr += *p++;
        //                }

        //                bStatic = true;
        //            }

        //            BEHAVIAC_UNUSED_VAR(bStatic);


        //            behaviac::string parName;

        //            //skip ' '
        //            BEHAVIAC_ASSERT(*str == ' ');
        //            str++;

        //            while (*str != ';') {
        //                parName += *str++;
        //            }

        //            behaviac::CStringCRC propertyId(propName.c_str());
        //            //props[propertyId] = behaviac::Property::Create(typeStr.c_str(), parName.c_str(), bStatic, 0);
        //            BEHAVIAC_ASSERT(false);

        //            //skip ';'
        //            BEHAVIAC_ASSERT(*str == ';');

        //            pB = str + 1;
        //        }

        //        str++;
        //    }

        //    return true;
        //}
    }//namespace StringUtils
}
