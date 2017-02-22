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

#ifndef _BEHAVIAC_COMMON_XML_H_
#define _BEHAVIAC_COMMON_XML_H_

#include "behaviac/common/xml/ixml.h"

namespace behaviac {
    class IFile;
    class CXmlNode : public IXmlNode {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(CXmlNode)

        CXmlNode(const char* tag);
        ~CXmlNode();

        //////////////////////////////////////////////////////////////////////////
        void AddRef() const {
            m_refCount++;
        };

        void Release() const {
            if (--m_refCount <= 0) {
                BEHAVIAC_DELETE(const_cast<CXmlNode*>(this));
            }
        };

        const char* getTag() const {
            return m_tag.c_str();
        };

        void setTag(const char* tag) {
            m_tag = tag;
        }

        bool isTag(const char* tag) const;

        virtual void copyAttributes(XmlConstNodeRef fromNode);

        virtual int	getAttrCount() const;
        virtual const char* getAttr(int index) const;
        virtual const char* getAttrTag(int index) const;

        const char* getAttr(const char* key) const;
        bool haveAttr(const char* key) const;
        void addChild(XmlNodeReference node);

        XmlNodeReference newNodeChild(const char* tagName);
        void removeNodeChild(XmlNodeReference node);
        void removeAllChilds();

        void swapChilds(int child1, int child2);

        int	getChildCount() const {
            return (int)m_childs.size();
        };
        int	getChildCount(const char* tag) const;

        XmlNodeReference getChild(int i);
        XmlConstNodeRef getChild(int i) const;

        XmlNodeReference findNodeChild(const char* tag);
        XmlConstNodeRef findNodeChild(const char* tag) const;
        XmlNodeReference findChildSafe(const char* tag);
        XmlConstNodeRef findChildSafe(const char* tag) const;

        const char* getContent() const {
            return m_content.c_str();
        };
        void setContent(const char* str) {
            m_content = str;
        };
        void transferContent(behaviac::string& newContent) {
            m_content.swap(newContent);
        }

        XmlNodeReference	clone() const;

#ifdef _DEBUG
        int getLine() const {
            return m_line;
        };

        void setLine(int line) {
            m_line = line;
        };
#endif //_DEBUG

        void getXML(behaviac::string& xml, int level = 0) const;
        void getXML(behaviac::wstring& xml, int level = 0) const;
        bool saveToFile(const char* fileName) const;
        bool saveToFile(IFile* file) const;

        void ReserveAttr(int nCount);

        virtual void setAttrText(const char* key, const char* value);
        virtual void setAttrText(const char* key, const wchar_t* text);

        void delAttr(const char* key);
        void removeAllAttributes();

        const XmlConstNodeRef& getInvalidNode() const {
            return m_invalidNode;
        }

    private:

        static const XmlConstNodeRef m_invalidNode;

        typedef behaviac::vector<XmlNodeReference>	XmlNodes;
        XmlNodes m_childs;
        XmlNodeAttributes m_attributes;
        behaviac::string m_content;
        XmlStringItem m_tag;
        mutable int m_refCount;
#ifdef _DEBUG
        int m_line;
#endif //_DEBUG
    };

}//namespace behaviac

#endif // #ifndef _BEHAVIAC_COMMON_XML_H_
