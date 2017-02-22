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

#include "./propertynode.h"

#include "behaviac/common/member.h"

namespace behaviac {
    CPropertyNode::~CPropertyNode() {
    }

    IONodeRef CPropertyNode::clone() const {
        return GetNodeRef(BEHAVIAC_NEW CPropertyNode(this->m_pAgent, this->m_tag.c_str()));
    }

    int32_t CPropertyNode::getChildCount() const {
        return (int32_t)m_children.size();
    }

    IIONode* CPropertyNode::getChild(int32_t childIndex) {
        BEHAVIAC_ASSERT(childIndex < getChildCount());
        ChildrenContainer::iterator iter = m_children.begin();
        std::advance(iter, childIndex);
        return &*iter;
    }

    const IIONode* CPropertyNode::getChild(int32_t childIndex) const {
        BEHAVIAC_ASSERT(childIndex < getChildCount());
        ChildrenContainer::const_iterator iter = m_children.begin();
        std::advance(iter, childIndex);
        return &*iter;
    }

    IIONode* CPropertyNode::findNodeChild(const CIOID& childID) {
        {
            ChildrenContainer::iterator iter, end = m_children.end();

            for (iter = m_children.begin(); iter != end; ++iter) {
                IIONode* currentChild = &*iter;

                if (currentChild->isTag(childID)) {
                    return currentChild;
                }
            }
        }

        return NULL;
    }

    const IIONode* CPropertyNode::findNodeChild(const CIOID& childID) const {
        {
            ChildrenContainer::const_iterator iter, end = m_children.end();

            for (iter = m_children.begin(); iter != end; ++iter) {
                const IIONode* currentChild = &*iter;

                if (currentChild->isTag(childID)) {
                    return currentChild;
                }
            }
        }

        return NULL;
    }

    CPropertyNode* CPropertyNode::newNodeChild(const CIOID& childID) {
        behaviac::string tag(this->m_tag);
        tag += "::";
        tag += childID.GetString();

        CPropertyNode newXmlChild(this->m_pAgent, tag.c_str());

        m_children.push_back(newXmlChild);
        return &m_children.back();
    }

    void CPropertyNode::removeNodeChild(IIONode* child) {
        {
            ChildrenContainer::iterator iter, end = m_children.end();

            for (iter = m_children.begin(); iter != end; ++iter) {
                if (&*iter == child) {
                    m_children.erase(iter);
                    return;
                }
            }
        }
    }

    int32_t CPropertyNode::getAttributesCount() const {
        return 0;
    }

    void CPropertyNode::RebuildChildrenList() {
    }

    ////////////////////////////////////////////////////////////////////////////////
    // set/getAttr type specializations
    ////////////////////////////////////////////////////////////////////////////////
    void CPropertyNode::addChild(const CIOID& keyID, const IIONode* child) {
        BEHAVIAC_UNUSED_VAR(keyID);
        BEHAVIAC_UNUSED_VAR(child);
    }

    void CPropertyNode::addChild(XmlNodeReference xmlChild) {
        BEHAVIAC_UNUSED_VAR(xmlChild);
    }

    bool CPropertyNode::LoadFromFile(const char* fileName) {
        BEHAVIAC_UNUSED_VAR(fileName);
        return (false);
    }

    bool CPropertyNode::SaveToFile(const char* fileName) const {
        BEHAVIAC_UNUSED_VAR(fileName);
        return false;
    }

    bool CPropertyNode::LoadFromFile(IFile* file) {
        BEHAVIAC_UNUSED_VAR(file);
        return (false);
    }

    bool CPropertyNode::SaveToFile(IFile* file) const {
        BEHAVIAC_UNUSED_VAR(file);
        return false;
    }

    // This is a very unprecise approximation...
    int32_t CPropertyNode::GetMemUsage() const {
        int32_t memUsage = sizeof(CPropertyNode);

        {
            ChildrenContainer::const_iterator iter, end = m_children.end();

            for (iter = m_children.begin(); iter != end; ++iter) {
                memUsage += iter->GetMemUsage();
            }
        }

        return memUsage;
    }

    const char* CPropertyNode::getAttrRaw(const CIOID& keyID, int typeId, int length) const {
		BEHAVIAC_UNUSED_VAR(typeId);
		BEHAVIAC_UNUSED_VAR(length);

        behaviac::IInstantiatedVariable* pInstantiatedVariable = this->m_pAgent->GetInstantiatedVariable(keyID.GetID().GetUniqueID());

        if (pInstantiatedVariable) {
            const void* p = pInstantiatedVariable->GetValueObject(this->m_pAgent);

            return (const char*)p;
        }

        return 0;
    }

    void CPropertyNode::setAttrRaw(const CIOID& keyID, const char* valueData, int typeId, int length) {
		BEHAVIAC_UNUSED_VAR(typeId);
		BEHAVIAC_UNUSED_VAR(length);

        behaviac::IInstantiatedVariable* pInstantiatedVariable = this->m_pAgent->GetInstantiatedVariable(keyID.GetID().GetUniqueID());

        if (pInstantiatedVariable) {
            pInstantiatedVariable->SetValue(this->m_pAgent, valueData);
        }
    }
}//namespace behaviac