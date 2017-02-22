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

#ifndef _BEHAVIAC_COMMON_TEXTNODE_H_
#define _BEHAVIAC_COMMON_TEXTNODE_H_

#include "behaviac/common/serialization/ionode.h"
#include "behaviac/common/xml/ixml.h"

#include <list>

namespace behaviac {
    class BEHAVIAC_API CTextNode : public IIONode {
    public:
        CTextNode()
            : IIONode(true),
              m_xmlNode(),
              m_constXmlNode()
        {}
        CTextNode(const behaviac::XmlNodeReference& xmlNode)
            : IIONode(true),
              m_xmlNode(xmlNode),
              m_constXmlNode(xmlNode) {
            RebuildChildrenList();
        }
        CTextNode(const XmlConstNodeRef& xmlNode)
            : IIONode(true),
              m_xmlNode(),
              m_constXmlNode(xmlNode) {
            RebuildChildrenList();
        }
        CTextNode(const CIOID& tag)
            : IIONode(true) {
            m_xmlNode = CreateXmlNode(tag.GetString());
            m_constXmlNode = m_xmlNode;
            RebuildChildrenList();
        }
        CTextNode(const CTextNode& other)
            : IIONode(true),
              m_xmlNode(other.m_xmlNode),
              m_constXmlNode(other.m_constXmlNode) {
            RebuildChildrenList();
        }

        CTextNode& operator=(IXmlNode* xmlNode) {
            m_xmlNode = xmlNode;
            m_constXmlNode = xmlNode;
            RebuildChildrenList();
            return *this;
        }
        CTextNode& operator=(const behaviac::XmlNodeReference& xmlNode) {
            m_xmlNode = xmlNode;
            m_constXmlNode = xmlNode;
            RebuildChildrenList();
            return *this;
        }
        CTextNode& operator=(const XmlConstNodeRef& xmlNode) {
            m_xmlNode = NULL;
            m_constXmlNode = xmlNode;
            RebuildChildrenList();
            return *this;
        }
        CTextNode& operator=(const CTextNode& other) {
            m_xmlNode = other.m_xmlNode;
            m_constXmlNode = other.m_constXmlNode;
            RebuildChildrenList();
            return *this;
        }

        //////////////////////////////////////////////////////////////////////////
        virtual IONodeRef clone() const;

        virtual int32_t getChildCount() const;
        virtual IIONode* getChild(int32_t childIndex);
        virtual const IIONode* getChild(int32_t childIndex) const;

        virtual IIONode* findNodeChild(const CIOID& childID);
        virtual const IIONode* findNodeChild(const CIOID& childID) const;

        virtual CTextNode* newNodeChild(const CIOID& childID);

        virtual void removeNodeChild(IIONode* child);

        virtual bool isTag(const CIOID& tagID) const {
            return CIOID(m_constXmlNode->getTag()) == tagID;
        }
        virtual CIOID getTag() const {
            return CIOID(m_constXmlNode->getTag());
        }

        virtual int32_t getAttributesCount() const;

        virtual const char* getAttrRaw(const CIOID& keyID, int typeId = 0, int length = 0) const {
            BEHAVIAC_UNUSED_VAR(typeId);
            BEHAVIAC_UNUSED_VAR(length);

            return m_constXmlNode->getAttr(keyID.GetString());
        }

        virtual void setAttrRaw(const CIOID& keyID, const char* valueStr, int typeId = 0, int length = 0) {
            BEHAVIAC_UNUSED_VAR(typeId);
            BEHAVIAC_UNUSED_VAR(length);

            m_xmlNode->setAttrText(keyID.GetString(), valueStr);
        }

        virtual void addChild(const CIOID& keyID, const IIONode* child);

        void addChild(XmlNodeReference xmlChild);

        virtual bool SaveToFile(const char* fileName) const;
        virtual bool LoadFromFile(const char* fileName);

        virtual bool SaveToFile(IFile* file) const;
        virtual bool LoadFromFile(IFile* file);

        virtual int32_t GetMemUsage() const;
    private:

        void RebuildChildrenList();

        typedef behaviac::list<CTextNode> ChildrenContainer;
        ChildrenContainer m_children;

        XmlNodeReference m_xmlNode;
        XmlConstNodeRef m_constXmlNode;
    };
}//namespace behaviac
#endif //_BEHAVIAC_COMMON_TEXTNODE_H_
