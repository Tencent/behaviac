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

#include "behaviac/common/xml/xml.h"
#include "behaviac/common/xml/xmlanalyzer.h"
#include "behaviac/common/file/file.h"
#include "behaviac/common/file/filemanager.h"
#include "behaviac/common/rapidxml/rapidxml.hpp"

namespace behaviac {
    class XmlAnalyzerImp {
    public:
        XmlAnalyzerImp();
        ~XmlAnalyzerImp();

        XmlNodeReference parse(char* buffer, int bufLen, const char* rootNodeName, behaviac::string& errorString, bool isFinal);

    protected:
        XmlNodeReference m_root;

        rapidxml::xml_document<>	m_parser;
        char*			m_buffer;
    };

    XmlAnalyzerImp::XmlAnalyzerImp() : m_root(0), m_buffer(0) {
    }

    XmlAnalyzerImp::~XmlAnalyzerImp() {
        BEHAVIAC_FREE(m_buffer);
    }

    XmlNodeReference cloneXmlNodeFrom(rapidxml::xml_node<>* xmlnode) {
        XmlNodeReference node = CreateXmlNode(xmlnode->name());

        for (rapidxml::xml_attribute<>* attr = xmlnode->first_attribute();
             attr; attr = attr->next_attribute()) {
            node->setAttrText(attr->name(), attr->value());
        }

        for (rapidxml::xml_node<>* subNode = xmlnode->first_node(); subNode; subNode = subNode->next_sibling()) {
            XmlNodeReference sub = cloneXmlNodeFrom(subNode);
            node->addChild(sub);
        }

        return node;
    }

    XmlNodeReference XmlAnalyzerImp::parse(char* buffer, int bufLen, const char* rootNodeName, behaviac::string& errorString, bool isFinal) {
        BEHAVIAC_UNUSED_VAR(bufLen);
        BEHAVIAC_UNUSED_VAR(errorString);
        BEHAVIAC_UNUSED_VAR(isFinal);

        m_parser.parse<0>(buffer);

        rapidxml::xml_node<>* xmlnode = m_parser.first_node(rootNodeName);

        if (xmlnode) {
            XmlNodeReference node = cloneXmlNodeFrom(xmlnode);

            return node;
        }

        BEHAVIAC_ASSERT(false);

        XmlNodeReference node = CreateXmlNode(rootNodeName);

        return node;
    }

    XmlNodeReference XmlAnalyzer::parse(const char* fileName, const char* rootNodeName, const char* suffix) {
        m_errorString.clear();
        behaviac::IFile* file = behaviac::CFileManager::GetInstance()->FileOpen(fileName, behaviac::CFileSystem::EOpenMode_Read);

        if (file) {
            XmlNodeReference xml = this->parse(file, rootNodeName, suffix, false);
            behaviac::CFileManager::GetInstance()->FileClose(file);

            if (!m_errorString.empty()) {
                BEHAVIAC_LOGWARNING("Error while parsing file : %s\n\n%s", fileName, m_errorString.c_str());
            }

            return xml;

        } else {
            BEHAVIAC_ASSERT(0, "Cannot open XML file : %s\n", fileName);
            return XmlNodeReference();
        }
    }

    XmlNodeReference XmlAnalyzer::parse(behaviac::IFile* file, const char* rootNodeName, const char* suffix, bool handleError) {
        BEHAVIAC_UNUSED_VAR(suffix);
        BEHAVIAC_UNUSED_VAR(handleError);

        m_errorString.clear();
        XmlAnalyzerImp xml;

        if (file) {
            int iSize = (int)file->GetSize() - (int)file->Seek(0, behaviac::CFileSystem::ESeekMode_Cur);

            if (iSize != 0) {
                static const int32_t ReadBlockSize = 64 * 1024;
                char* buf = (char*)BEHAVIAC_MALLOC_WITHTAG(ReadBlockSize, "XML");
                XmlNodeReference ref;

                for (int32_t i = 0; i <= iSize / (ReadBlockSize); ++i) {
                    int32_t bufSize = file->Read(buf, ReadBlockSize);
                    {
                        buf[bufSize] = '\0';
                        ref = xml.parse(buf, bufSize, rootNodeName, m_errorString, i == iSize / (ReadBlockSize));
                    }
                }

                BEHAVIAC_FREE(buf);

                if (handleError && !m_errorString.empty()) {
                    BEHAVIAC_LOGWARNING("Error while parsing file\n\n%s", m_errorString.c_str());
                }

                return ref;

            } else {
                return XmlNodeReference();
            }
        } else {
            BEHAVIAC_ASSERT(0, "XmlParse(behaviac::IFile*) - Invalid file\n");
            return XmlNodeReference();
        }
    }

    XmlNodeReference XmlAnalyzer::parseBuffer(const char* buffer, const char* rootNodeName) {
        size_t bufLen = strlen(buffer);
        char* temp = (char*)BEHAVIAC_MALLOC_WITHTAG(bufLen + 1, "XmlAnalyzerImp::parse");
        memcpy(temp, buffer, bufLen);
        BEHAVIAC_ASSERT(temp[bufLen] == '\0');
        XmlNodeReference result = this->parseBuffer(temp, (int)bufLen, rootNodeName);
        BEHAVIAC_FREE(temp);
        return result;
    }

    XmlNodeReference XmlAnalyzer::parseBuffer(char* buffer, int size, const char* rootNodeName) {
        m_errorString = "";
        XmlAnalyzerImp xml;
        XmlNodeReference ref = xml.parse(buffer, size, rootNodeName, m_errorString, true);

        if (!m_errorString.empty()) {
            BEHAVIAC_LOGWARNING("Error while parsing XML file : \n\n%s", m_errorString.c_str());
        }

        return ref;
    }
}//namespace behaviac