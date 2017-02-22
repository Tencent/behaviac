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

#ifndef _BEHAVIAC_PROPERTYNODE_H_
#define _BEHAVIAC_PROPERTYNODE_H_

#include "behaviac/common/serialization/ionode.h"
#include "behaviac/agent/agent.h"
#include "behaviac/common/object/member.h"

namespace behaviac
{
    class BEHAVIAC_API CPropertyNode : public behaviac::IIONode
    {
    public:
        CPropertyNode() : behaviac::IIONode(false, false), m_pAgent(0), m_bParseString(false)
        {}

        CPropertyNode(behaviac::Agent* pAgent, const char* tag, const char* valueStr = 0)
            : behaviac::IIONode(false, false), m_pAgent(pAgent), m_tag(tag), m_value(valueStr), m_bParseString(valueStr != 0)
        {}

        ~CPropertyNode();

        CPropertyNode& operator=(const CPropertyNode&);

        //////////////////////////////////////////////////////////////////////////
        // behaviac::IIONode interface
        virtual behaviac::IONodeRef clone() const;

        virtual int32_t getChildCount() const;
        virtual behaviac::IIONode* getChild(int32_t childIndex);
        virtual const behaviac::IIONode* getChild(int32_t childIndex) const;

        virtual behaviac::IIONode* findNodeChild(const behaviac::CIOID& childID);
        virtual const behaviac::IIONode* findNodeChild(const behaviac::CIOID& childID) const;

        virtual CPropertyNode* newNodeChild(const behaviac::CIOID& childID);

        virtual void removeNodeChild(behaviac::IIONode* child);

        virtual bool isTag(const behaviac::CIOID& tagID) const {
            BEHAVIAC_UNUSED_VAR(tagID);

            return false;
        }
        virtual behaviac::CIOID getTag() const {
            return behaviac::CIOID("");
        }

        virtual int32_t getAttributesCount() const;

        virtual const char* getAttr(const behaviac::CIOID& keyID) const {
            BEHAVIAC_UNUSED_VAR(keyID);

            return 0;
        }

        virtual const char* getAttrRaw(const behaviac::CIOID& keyID, int typeId = 0, int length = 0) const;
        virtual void setAttrRaw(const behaviac::CIOID& keyID, const char* valueData, int typeId = 0, int length = 0);

        virtual void addChild(const behaviac::CIOID& keyID, const behaviac::IIONode* child);

        void addChild(behaviac::XmlNodeReference xmlChild);

        virtual bool SaveToFile(const char* fileName) const;
        virtual bool LoadFromFile(const char* fileName);

        virtual bool SaveToFile(IFile* file) const;
        virtual bool LoadFromFile(IFile* file);

        virtual int32_t GetMemUsage() const;

    private:
        void RebuildChildrenList();

        typedef behaviac::list<CPropertyNode> ChildrenContainer;
        ChildrenContainer m_children;

        behaviac::Agent*		m_pAgent;
        const behaviac::string		m_tag;
        //const behaviac::string		m_value;
        const char*				m_value;
        const bool				m_bParseString;
    };
}//namespace behaviac

#endif //_BEHAVIAC_PROPERTYNODE_H_
