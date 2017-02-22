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

#ifndef _BEHAVIAC_COMMON_IONODE_H_
#define _BEHAVIAC_COMMON_IONODE_H_

#include "behaviac/common/base.h"
#include "behaviac/common/xml/ixml.h"
#include "behaviac/common/swapbyte.h"
#include "behaviac/common/smartptr.h"

template <typename T>
bool FromBinary(const uint8_t* binaryData, T& value) {
    //BEHAVIAC_ASSERT(0, "please provide your specification!");
    value = *(T*)binaryData;

    return true;
}

template <typename T>
uint8_t* ToBinary(const T& value) {
    //BEHAVIAC_ASSERT(0, "please provide your specification!");
    //return (uint8_t*)&s_binary_temp;
    return (uint8_t*)&value;
}

namespace behaviac {
    class IIONode;

    class CIOID {
    public:
        CIOID() : m_idString(""), m_id() {
        }

        explicit CIOID(const char* idString)
            : m_idString(idString),
              m_id(idString)
        {}

        explicit CIOID(behaviac::CStringCRC::IDType crc, const char* idString)
            : m_idString(idString),
              m_id(crc)
        {}

        bool operator==(const CIOID& other) const {
            return m_id == other.m_id;
        }

        bool operator<(const CIOID& other) const {
            return m_id < other.m_id;
        }

        bool operator==(const behaviac::CStringCRC& other) const {
            return m_id == other;
        }

        CIOID& operator=(const behaviac::CStringCRC& other) {
            m_idString = NULL;
            m_id = other;
            return *this;
        }

        const char* GetString() const {
            return m_idString;
        }
        const behaviac::CStringCRC& GetID() const {
            return m_id;
        }

    private:
        const char* m_idString;
        behaviac::CStringCRC	m_id;
    };

    class IONodeRef {
    public:
        IONodeRef()
            : m_node(NULL)
        {}
        ~IONodeRef();

        IONodeRef(const IONodeRef& node);
        IONodeRef& operator=(const IONodeRef& node);

        template <class TNodeClass, class TParamType>
        TNodeClass* CreateNode(TParamType param) {
            TNodeClass* node = BEHAVIAC_NEW TNodeClass(param);
            *this = node;
            return node;
        }

        template <class TNodeClass>
        TNodeClass* CreateNode() {
            TNodeClass* node = BEHAVIAC_NEW TNodeClass();
            *this = node;
            return node;
        }

        template <class TNodeClass, class TParamType1, class TParamType2>
        TNodeClass* CreateNode(TParamType1 param1, TParamType2 param2) {
            TNodeClass* node = BEHAVIAC_NEW TNodeClass(param1, param2);
            *this = node;
            return node;
        }

        template <class TNodeClass, class TParamType1, class TParamType2, class TParamType3>
        TNodeClass* CreateNode(TParamType1 param1, TParamType2 param2, TParamType3 param3) {
            TNodeClass* node = BEHAVIAC_NEW TNodeClass(param1, param2, param3);
            *this = node;
            return node;
        }

        IIONode* GetNode() const {
            return m_node;
        }

        IIONode* operator->() const {
            return m_node;
        }

        void ReleaseNode() {
            *this = NULL;
        }

    private:
        friend class IIONode;
        IONodeRef(IIONode* node);
        IONodeRef& operator=(IIONode* node);

        IIONode* m_node;
    };

    class BEHAVIAC_API IIONode : private CRefCounted {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(IIONode);

        IIONode(bool bText, bool bSwap = true) : m_bText(bText), m_bSwap(bSwap)
        {}

        bool IsText() const {
            return this->m_bText;
        }

        virtual ~IIONode() {};

        virtual IONodeRef clone() const = 0;

        virtual int32_t getChildCount() const = 0;
        virtual IIONode* getChild(int32_t childIndex) = 0;
        virtual const IIONode* getChild(int32_t childIndex) const = 0;

        virtual IIONode* findNodeChild(const CIOID& childID) = 0;
        virtual const IIONode* findNodeChild(const CIOID& childID) const = 0;

        virtual IIONode* newNodeChild(const CIOID& childID) = 0;

        virtual void removeNodeChild(IIONode* child) = 0;

        virtual bool isTag(const CIOID& tagID) const = 0;
        virtual CIOID getTag() const = 0;

        virtual int32_t getAttributesCount() const = 0;

        virtual bool SaveToFile(const char* fileName) const = 0;
        virtual bool LoadFromFile(const char* fileName) = 0;

        virtual bool SaveToFile(IFile* file) const = 0;
        virtual bool LoadFromFile(IFile* file) = 0;

        virtual int32_t GetMemUsage() const = 0;

        virtual const char* getAttrRaw(const CIOID& keyID, int typeId = 0, int length = 0) const {
            BEHAVIAC_UNUSED_VAR(keyID);
            BEHAVIAC_UNUSED_VAR(typeId);
            BEHAVIAC_UNUSED_VAR(length);

            BEHAVIAC_ASSERT(0);

            return 0;
        }

        virtual void setAttrRaw(const CIOID& keyID, const char* valueStr, int typeId = 0, int length = 0) {
            BEHAVIAC_UNUSED_VAR(keyID);
            BEHAVIAC_UNUSED_VAR(valueStr);
            BEHAVIAC_UNUSED_VAR(typeId);
            BEHAVIAC_UNUSED_VAR(length);

            BEHAVIAC_ASSERT(0);
        }

        template <class T>
        void setAttr(const CIOID& keyID, const T& value) {
            int typeId = GetClassTypeNumberId<T>();

            if (this->m_bText) {
                behaviac::string str = behaviac::StringUtils::ToString(value);
                this->setAttrRaw(keyID, str.c_str(), typeId, sizeof(T));
            } else {
                uint8_t* binaryData = ToBinary(value);

                if (binaryData) {
                    if (this->m_bSwap) {
                        behaviacSwapByte(*(T*)binaryData);
                    }

                    this->setAttrRaw(keyID, (const char*)binaryData, typeId, sizeof(T));
                }
            }
        }

        template <class T>
        bool getAttr(const CIOID& keyID, T& value) const {
            int typeId = GetClassTypeNumberId<T>();

            if (this->m_bText) {
                const char* valueStr = this->getAttrRaw(keyID, typeId, sizeof(T));

                if (valueStr) {
                    if (behaviac::StringUtils::ParseString(valueStr, value)) {
                        return true;

                    } else {
                        BEHAVIAC_LOGWARNING("Fail to read a (%s) from the value (%s) in the xml attribute (%s) in xml node (%s)\n",
                                            GetClassTypeName((T*)0),
                                            (strlen(valueStr) < 64 ? valueStr : behaviac::StringUtils::printf("__too_long_[%u]_to_display__", strlen(valueStr)).c_str()),
                                            keyID.GetString(), getTag().GetString());
                        return false;
                    }
                }
            } else {
                const char* p = this->getAttrRaw(keyID, typeId, sizeof(T));

                if (p) {
                    bool bOk = FromBinary((uint8_t*)p, value);

                    if (bOk && this->m_bSwap) {
                        behaviacSwapByte(value);
                    }
                }
            }

            return false;
        }

        // Node
        virtual void addChild(const CIOID& keyID, const IIONode* child) {
            BEHAVIAC_UNUSED_VAR(keyID);
            BEHAVIAC_UNUSED_VAR(child);
        }

    protected:
        friend class IONodeRef;

        static IONodeRef GetNodeRef(IIONode* node) {
            return IONodeRef(node);
        }

        bool	m_bText;
        bool	m_bSwap;
    };

    class IONodeIt {
        int32_t m_index;
        IIONode* m_node;

    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(IONodeIt);

        IONodeIt(IIONode* node)
            : m_index(0),
              m_node(node)
        {}
        void reset(IIONode* node) {
            m_index = 0;
            m_node = node;
        }
        IIONode* first() {
            m_index = 0;
            return next();
        }
        IIONode* first(const CIOID& tag) {
            m_index = 0;
            return next(tag);
        }

        IIONode* next() {
            if (m_node) {
                if (m_index < m_node->getChildCount()) {
                    return m_node->getChild(m_index++);
                }
            }

            return NULL;
        }
        IIONode* next(const CIOID& tag) {
            if (m_node) {
                while (m_index < m_node->getChildCount()) {
                    IIONode* node = m_node->getChild(m_index++);

                    if (node->isTag(tag)) {
                        return node;
                    }
                }
            }

            return NULL;
        }
    };

    class ConstIONodeIt {
        int32_t m_index;
        const IIONode* m_node;

    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(ConstIONodeIt);

        ConstIONodeIt(const IIONode* node)
            : m_index(0),
              m_node(node)
        {}
        void reset(const IIONode* node) {
            m_index = 0;
            m_node = node;
        }
        const IIONode* first() {
            m_index = 0;
            return next();
        }
        const IIONode* first(const CIOID& tag) {
            m_index = 0;
            return next(tag);
        }

        const IIONode* next() {
            if (m_node) {
                if (m_index < m_node->getChildCount()) {
                    return m_node->getChild(m_index++);
                }
            }

            return NULL;
        }
        const IIONode* next(const CIOID& tag) {
            if (m_node) {
                while (m_index < m_node->getChildCount()) {
                    const IIONode* node = m_node->getChild(m_index++);

                    if (node->isTag(tag)) {
                        return node;
                    }
                }
            }

            return NULL;
        }
    };

    inline IONodeRef::~IONodeRef() {
        if (m_node) {
            m_node->Release();
        }
    }

    inline IONodeRef::IONodeRef(const IONodeRef& node)
        : m_node(node.m_node) {
        if (m_node) {
            m_node->AddRef();
        }
    }

    inline IONodeRef& IONodeRef::operator=(const IONodeRef& node) {
        return *this = node.GetNode();
    }

    inline IONodeRef::IONodeRef(IIONode* node)
        : m_node(node) {
        if (m_node) {
            m_node->AddRef();
        }
    }

    inline IONodeRef& IONodeRef::operator=(IIONode* node) {
        if (node) {
            node->AddRef();
        }

        if (m_node) {
            m_node->Release();
        }

        m_node = node;
        return *this;
    }
}//namespace behaviac
#endif //_BEHAVIAC_COMMON_IONODE_H_
