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

#ifndef _BEHAVIAC_COMMON_OBJECT_MEMBER_H_
#define _BEHAVIAC_COMMON_OBJECT_MEMBER_H_
#include "behaviac/property/property.h"
#include "behaviac/network/network.h"

namespace behaviac {
    class CTagObject;
    class Agent;
}

namespace behaviac {
    class IInstanceMember;

    class BEHAVIAC_API IMemberBase {
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(IMemberBase);

    public:
        IMemberBase(const char* propertyName, const char* classFullName)
            : m_bStatic(false), m_bIsConst(false), m_netRole(NET_ROLE_DEFAULT),
              m_classFullName(classFullName), m_instaceName(classFullName), m_propertyID(propertyName), m_range(1.0f) {
            BEHAVIAC_ASSERT(m_classFullName);
        }

        IMemberBase(const IMemberBase& copy) :
            m_bStatic(copy.m_bStatic), m_bIsConst(copy.m_bIsConst), m_netRole(copy.m_netRole),
            m_classFullName(copy.m_classFullName), m_instaceName(copy.m_instaceName), m_propertyID(copy.m_propertyID),
            m_displayName(copy.m_displayName), m_desc(copy.m_desc), m_range(copy.m_range)
        {}

        virtual ~IMemberBase() {
        }

        virtual void        Load(CTagObject* parent, const IIONode* node) = 0;
        virtual void        Save(const CTagObject* parent, IIONode* node) = 0;
        virtual void        LoadState(CTagObject* parent, const IIONode* node) = 0;
        virtual void        SaveState(const CTagObject* parent, IIONode* node) = 0;
        virtual IMemberBase* clone() const = 0;
        virtual CIOID GetID() const {
            return m_propertyID;
        }

        virtual void        Set(const CTagObject* parent, const void* value, int typeId) const {
            BEHAVIAC_UNUSED_VAR(parent);
            BEHAVIAC_UNUSED_VAR(value);
            BEHAVIAC_UNUSED_VAR(typeId);

            BEHAVIAC_ASSERT(!"Only works with TGenericMembers");
        };
        virtual const void*       Get(const CTagObject* parent, int typeId) const {
            BEHAVIAC_UNUSED_VAR(parent);
            BEHAVIAC_UNUSED_VAR(typeId);

            BEHAVIAC_ASSERT(!"Only works with TGenericMembers");
            return NULL;
        }

        virtual void        SetVariable(const CTagObject* parent, const void* value, int typeId) const {
            BEHAVIAC_UNUSED_VAR(parent);
            BEHAVIAC_UNUSED_VAR(value);
            BEHAVIAC_UNUSED_VAR(typeId);

            BEHAVIAC_ASSERT(!"Only works with TGenericMembers");
        }

        virtual void*       GetVariable(const CTagObject* parent, int typeId) const {
            BEHAVIAC_UNUSED_VAR(parent);
            BEHAVIAC_UNUSED_VAR(typeId);

            BEHAVIAC_ASSERT(!"Only works with TGenericMembers");
            return NULL;
        }

        virtual int GetTypeId() const {
            BEHAVIAC_ASSERT(!"Only works with TGenericMembers");

            return 0;
        }

        virtual bool Equal(const CTagObject* lhs, const CTagObject* rhs) const {
            BEHAVIAC_UNUSED_VAR(lhs);
            BEHAVIAC_UNUSED_VAR(rhs);

            BEHAVIAC_ASSERT(!"Only works with TGenericMembers");

            return false;
        }

        virtual IInstanceMember* CreateProperty(const char* defaultValue, bool bConst) const {
            BEHAVIAC_UNUSED_VAR(defaultValue);
            BEHAVIAC_UNUSED_VAR(bConst);

            return 0;
        }

        const char* GetClassNameString() const {
            return m_classFullName;
        }

        const char* GetInstanceNameString() const {
            return m_instaceName.c_str();
        }

        void SetInstanceNameString(const char* name) {
            m_instaceName = name;
        }

        const char* GetName() const {
            return this->m_propertyID.GetString();
        }

        const wstring& GetDisplayName() const {
            return this->m_displayName;
        }

        const wstring& GetDesc() const {
            return this->m_desc;
        }

        bool ISSTATIC() const {
            return this->m_bStatic;
        }

        IMemberBase& RANGE(float range) {
            this->m_range = range;

            return *this;
        }

        /// deparated, to use DISPLAY_INFO
        IMemberBase& DISPLAYNAME(const wchar_t* displayName) {
            BEHAVIAC_UNUSED_VAR(displayName);

            if (displayName) {
                m_displayName = displayName;
            }

            return *this;
        }

        /// deparated, to use DISPLAY_INFO
        IMemberBase& DESC(const wchar_t* desc) {
            BEHAVIAC_UNUSED_VAR(desc);

            if (desc) {
                m_desc = desc;
            }

            return *this;
        }

        IMemberBase& DISPLAY_INFO(const wchar_t* displayName, const wchar_t* desc) {
            BEHAVIAC_UNUSED_VAR(displayName);
            BEHAVIAC_UNUSED_VAR(desc);

            if (displayName) {
                m_displayName = displayName;
            }

            if (desc) {
                m_desc = desc;

            } else {
                m_desc = m_displayName;
            }

            return *this;
        }

        IMemberBase& SETREADONLY() {
            this->m_bIsConst = true;

            return *this;
        }

        virtual int READONLYFLAG() const {
            return this->m_bIsConst ? 0x1 : 0;
        }

#if BEHAVIAC_ENABLE_NETWORKD
        IMemberBase& NETROLE(NetworkRole netRole) {
            m_netRole = netRole;

            if (this->m_netRole != NET_ROLE_DEFAULT) {
                Network* pNw = Network::GetInstance();

                if (pNw && !pNw->IsSinglePlayer()) {
                    string nameTemp;

                    if (this->m_className) {
                        nameTemp = FormatString("%s::%s", this->m_className, m_propertyID.GetString());

                    } else {
                        nameTemp = m_propertyID.GetString();
                    }

                    pNw->RegisterReplicatedProperty(this->m_netRole, nameTemp.c_str());
                }
            }

            return *this;
        }

        NetworkRole NETROLE() const {
            return m_netRole;
        }

        virtual void ReplicateProperty(Agent* pAgent) {
            BEHAVIAC_UNUSED_VAR(pAgent);
        }
#endif//#if BEHAVIAC_ENABLE_NETWORKD
    protected:
        bool						m_bStatic;
        bool						m_bIsConst;
        NetworkRole		            m_netRole;
        const char*					m_classFullName;
        string			            m_instaceName;
        CIOID						m_propertyID;

        wstring			            m_displayName;
        wstring			            m_desc;

        float						m_range;
    };

}//

#endif // #ifndef _BEHAVIAC_COMMON_OBJECT_MEMBER_H_
