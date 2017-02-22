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

#ifndef _BEHAVIAC_COMMON_IXML_H_
#define _BEHAVIAC_COMMON_IXML_H_

#include "behaviac/common/string/stringcrc.h"
#include "behaviac/common/thread/mutex_lock.h"
#include "behaviac/common/base.h"
#include "behaviac/common/swapbyte.h"
#include "behaviac/common/string/stringutils.h"
#include "behaviac/common/string/tostring.h"
#include "behaviac/common/string/fromstring.h"
#include "behaviac/common/rttibase.h"

#include <vector>
#include <map>

namespace behaviac {
    class StringHashCompare {
    public:
        StringHashCompare() {
        }

        size_t operator()(const char*  keyVal) const {
            uint32_t crc = CRC32::CalcCRC(keyVal);

            ldiv_t qrem = ldiv((long)crc, 127773);
            qrem.rem = 16807 * qrem.rem - 2836 * qrem.quot;

            if (qrem.rem < 0) {
                qrem.rem += 2147483647;
            }

            return ((size_t)qrem.rem);
        }

        bool operator()(const char* keyVal1, const char* keyVal2) const {
            return strcmp(keyVal1, keyVal2) < 0;
        }
    };

    class BEHAVIAC_API XmlStringItem {
    public:
        XmlStringItem() : m_str(0)
        {}

        XmlStringItem(XmlStringItem const& string);
        XmlStringItem(char const* string);
        ~XmlStringItem();

        void operator=(XmlStringItem const& string);

        char const* c_str() const {
            return m_str;
        }

    private:
        void Insert(char const* string);
        void Remove();

        typedef behaviac::map<char const*, int32_t, StringHashCompare> TableType;
        static behaviac::Mutex ms_critSection;
        static TableType m_table;

        char const* m_str;
    };

    class XmlNodeAttr {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(XmlNodeAttr);

        XmlNodeAttr() : m_wide(false) {}
        XmlNodeAttr(const char* k, const char* v) : m_key(k), m_value(v), m_wide(false) {}
        explicit XmlNodeAttr(const char* k) : m_key(k), m_wide(false) {}

        bool operator<(const XmlNodeAttr& attr) const {
            return string_icmp(m_key.c_str(), attr.m_key.c_str()) < 0;
        }

        bool operator==(const XmlNodeAttr& attr) const {
            return string_icmp(m_key.c_str(), attr.m_key.c_str()) == 0;
        }

        bool operator==(const char* sKey) const {
            return string_icmp(m_key.c_str(), sKey) == 0;
        }

        inline void SetKey(const char* key) {
            m_key = key;
        }
        inline void SetValue(const char* value) {
            m_value = value;
        }
        inline const char* GetKey() const {
            return m_key.c_str();
        }
        inline const char* GetValue() const {
            return m_value.c_str();
        }

        inline bool IsWide() const {
            return m_wide;
        }
        inline void SetValue(const wchar_t* value) {
            m_wide = true;
            m_value_w = value;
        }
        inline const wchar_t* GetValueWide() const {
            return m_value_w.c_str();
        }
    protected:
        XmlStringItem m_key;
        XmlStringItem m_value;

        bool		m_wide;
        behaviac::wstring m_value_w;
    };

    class IXmlNode;

    typedef behaviac::vector<XmlNodeAttr>	XmlNodeAttributes;

    class XmlNodeReference {
    private:
        IXmlNode* p;
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(XmlNodeReference);

        XmlNodeReference() : p(NULL) {}
        XmlNodeReference(IXmlNode* p_);
        XmlNodeReference(const behaviac::XmlNodeReference& p_);
        ~XmlNodeReference();

        operator IXmlNode* () const {
            return p;
        }
        IXmlNode& operator*() const {
            return *p;
        }
        IXmlNode* operator->(void) const {
            return p;
        }

        XmlNodeReference&  operator=(IXmlNode* newp);
        XmlNodeReference&  operator=(const behaviac::XmlNodeReference& newp);

        friend class XmlConstNodeRef;
    };

    class XmlConstNodeRef {
    private:
        const IXmlNode* p;
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(XmlConstNodeRef);

        XmlConstNodeRef() : p(NULL) {}
        XmlConstNodeRef(const IXmlNode* p_);
        XmlConstNodeRef(const XmlConstNodeRef& p_);
        XmlConstNodeRef(const behaviac::XmlNodeReference& p_);
        ~XmlConstNodeRef();

        operator const IXmlNode* () const {
            return p;
        }
        const IXmlNode& operator*() const {
            return *p;
        }
        const IXmlNode* operator->(void) const {
            return p;
        }

        XmlConstNodeRef&  operator=(const IXmlNode* newp);
        XmlConstNodeRef&  operator=(const XmlConstNodeRef& newp);
        XmlConstNodeRef&  operator=(const behaviac::XmlNodeReference& newp);
    };

    class IFile;
    class IXmlNode {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(IXmlNode);

        virtual ~IXmlNode() {};

        virtual void AddRef() const = 0;
        virtual void Release() const = 0;

        virtual const char* getTag() const = 0;
        virtual void setTag(const char* tag) = 0;
        virtual bool isTag(const char* tag) const = 0;

        virtual void copyAttributes(XmlConstNodeRef fromNode) = 0;

        virtual const char* getAttr(const char* key) const = 0;
        virtual int	getAttrCount() const = 0;
        virtual const char* getAttr(int index) const = 0;
        virtual const char* getAttrTag(int index) const = 0;
        virtual bool haveAttr(const char* key) const = 0;

        virtual void addChild(XmlNodeReference node) = 0;
        virtual XmlNodeReference newNodeChild(const char* tagName) = 0;
        virtual void removeNodeChild(XmlNodeReference node) = 0;
        virtual void removeAllChilds() = 0;

        virtual void swapChilds(int child1, int child2) = 0;
        virtual int	getChildCount() const = 0;
        virtual int	getChildCount(const char* tag) const = 0;

        virtual XmlNodeReference getChild(int i) = 0;
        virtual XmlConstNodeRef getChild(int i) const = 0;

        virtual XmlNodeReference findNodeChild(const char* tag) = 0;
        virtual XmlConstNodeRef findNodeChild(const char* tag) const = 0;

        virtual XmlNodeReference findChildSafe(const char* tag) = 0;
        virtual XmlConstNodeRef findChildSafe(const char* tag) const = 0;

        virtual const char* getContent() const = 0;

        virtual void setContent(const char* str) = 0;

        virtual void transferContent(behaviac::string& newContent) = 0;

        virtual XmlNodeReference clone() const = 0;

#ifdef _DEBUG
        virtual int getLine() const = 0;
        virtual void setLine(int line) = 0;
#endif

        virtual void getXML(behaviac::string& xml, int level = 0) const = 0;
        virtual void getXML(behaviac::wstring& xml, int level = 0) const = 0;

        virtual bool saveToFile(const char* fileName) const = 0;
        virtual bool saveToFile(IFile* file) const = 0;

        virtual void ReserveAttr(int nCount) = 0;

        virtual void setAttrText(const char* key, const char* text) = 0;
        virtual void setAttrText(const char* key, const wchar_t* text) = 0;
        void setAttr(const char* key, const char* value) {
            setAttrText(key, value);
        }
        void setAttr(const char* key, char* value) {
            setAttrText(key, value);
        }
        void setAttr(const char* key, const behaviac::wstring& value) {
            setAttrText(key, value.c_str());
        }
        template <typename T> void setAttr(const char* key, const T& value) {
            behaviac::string str = behaviac::StringUtils::ToString(value);
            setAttrText(key, str.c_str());
        }
        template <class T> void setAttrStringIDKind(const char* key, const T& value) {
            char crcValue[11];
            string_sprintf(crcValue, "%%$%08X", value.GetUniqueID());
            setAttr(key, crcValue);
        }

        inline void setAttr_Hex(const char* key, int32_t& value) {
            char str[11];
            string_sprintf(str, "0x%08x", value);
            setAttrText(key, str);
        }
#if !BEHAVIAC_CCDEFINE_64BITS
        inline void setAttr_Hex(const char* key, int64_t& value) {
            char str[19];
            string_sprintf(str, "0x%16llx", value);
            setAttrText(key, str);
        }

        inline void setAttr_Hex(const char* key, uint64_t& value) {
            char str[19];
            string_sprintf(str, "0x%16llx", value);
            setAttrText(key, str);
        }
#else
        inline void setAttr_Hex(const char* key, long long& value) {
            char str[19];
            string_sprintf(str, "0x%16llx", value);
            setAttrText(key, str);
        }
#endif

        template <class T>
        void setAttrGeneric(const char* key, T& value) {
            setAttr(key, (int)value);
        }
        //////////////////////////////////////////////////////////////////////////

        virtual void delAttr(const char* key) = 0;
        virtual void removeAllAttributes() = 0;

        template <typename T> bool getAttr(const char* key, T& value) const {
            const char* str = getAttr(key);

            if (str) {
                if (behaviac::StringUtils::ParseString(str, value)) {
                    return true;

                } else {
                    BEHAVIAC_LOGWARNING("Fail to read a (%s) from the value (%s) in the xml attribute (%s) in xml node (%s)\n",
                                        GetClassTypeName((T*)0),
                                        strlen(str) < 64 ? str : behaviac::StringUtils::printf("__too_long_[%u]_to_display__", strlen(str)).c_str(),
                                        key, getTag());
                    return false;
                }
            }

            return false;
        }

        template <bool> bool getAttr(const char* key, bool& value) const {
            const char* str = getAttr(key);

            if (str) {
                if (behaviac::StringUtils::ParseString(str, value)) {
                    return true;

                } else {
                    BEHAVIAC_LOGWARNING("Fail to read a (%s) from the value (%s) in the xml attribute (%s) in xml node (%s)\n",
                                        GetClassTypeName((bool*)0),
                                        strlen(str) < 64 ? str : behaviac::StringUtils::printf("__too_long_[%u]_to_display__", strlen(str)).c_str(),
                                        key, getTag());
                    return false;
                }
            }

            return false;
        }

        template <class T> bool getAttrStringIDKind(const char* key, T& value) const {
            const char* textVal = getAttr(key);

            if (textVal) {
                if (textVal[0] == '%' && textVal[1] == '$') {
                    int32_t crcValue;

                    if (sscanf(textVal + 2, "%08X", &crcValue) == 1) {
                        value = T(crcValue);

                    } else {
                        return false;
                    }
                } else {
                    value.ParseString(textVal);
                }

                return true;
            }

            return false;
        }

        template <class T>
        bool getAttrGeneric(const char* key, T& value) const {
            int	val;

            if (!getAttr(key, val)) {
                return	false;
            }

            value = (T)val;
            return	true;
        }

        inline bool getAttr_Hex(const char* key, int32_t& value) const {
            const char* str = getAttr(key);

            if (str && *str) {
                if (strncmp(str, "0x", 2) == 0 && sscanf(str, "0x%08x", &value) == 1) {
                    return true;

                } else {
                    BEHAVIAC_LOGWARNING("Fail to read a (hexa U32) from the value (%s) in the xml attribute (%s) in xml node (%s)\n", str, key, getTag());
                }
            }

            return false;
        }
#if !BEHAVIAC_CCDEFINE_64BITS
        inline bool getAttr_Hex(const char* key, int64_t& value) const {
            const char* str = getAttr(key);

            if (str && *str) {
                if (strncmp(str, "0x", 2) == 0 && sscanf(str, "0x%16llx", &value) == 1) {
                    return true;

                } else {
                    BEHAVIAC_LOGWARNING("Fail to read a (hexa U64) from the value (%s) in the xml attribute (%s) in xml node (%s)\n", str, key, getTag());
                }
            }

            return false;
        }

        inline bool getAttr_Hex(const char* key, uint64_t& value) const {
            const char* str = getAttr(key);

            if (str && *str) {
                if (strncmp(str, "0x", 2) == 0 && sscanf(str, "0x%16llx", &value) == 1) {
                    return true;

                } else {
                    BEHAVIAC_LOGWARNING("Fail to read a (hexa U64) from the value (%s) in the xml attribute (%s) in xml node (%s)\n", str, key, getTag());
                }
            }

            return false;
        }
#else
        inline bool getAttr_Hex(const char* key, long long& value) const {
            const char* str = getAttr(key);

            if (str && *str) {
                if (strncmp(str, "0x", 2) == 0 && sscanf(str, "0x%16llx", &value) == 1) {
                    return true;

                } else {
                    BEHAVIAC_LOGWARNING("Fail to read a (hexa U64) from the value (%s) in the xml attribute (%s) in xml node (%s)\n", str, key, getTag());
                }
            }

            return false;
        }

        inline bool getAttr_Hex(const char* key, unsigned long long& value) const {
            const char* str = getAttr(key);

            if (str && *str) {
                if (strncmp(str, "0x", 2) == 0 && sscanf(str, "0x%16llx", &value) == 1) {
                    return true;

                } else {
                    BEHAVIAC_LOGWARNING("Fail to read a (hexa U64) from the value (%s) in the xml attribute (%s) in xml node (%s)\n", str, key, getTag());
                }
            }

            return false;
        }

#endif
        template<typename T>
        bool getAttr(const char* key, T& value, const T& defaultValue) const {
            if (!getAttr(key, value)) {
                value = defaultValue;
                return false;
            }

            return true;
        }

        inline bool getAttrPrecise(const char* key, float& value) const {
            union {
                float f;
                int32_t p;
            } temp;

            if (getAttr(key, temp.p)) {
                value = temp.f;
                return true;
            }

            return false;
        }

        friend class XmlNodeReference;
        friend class XmlConstNodeRef;
    };

    class XmlNodeIt {
        int m_Idx;
        XmlNodeReference m_node;

    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(XmlNodeIt);

        XmlNodeIt(const behaviac::XmlNodeReference& node) {
            reset(node);
        }
        void reset(const behaviac::XmlNodeReference& node) {
            m_Idx = 0;
            m_node = node;
        }
        XmlNodeReference first() {
            m_Idx = 0;
            return next();
        }
        XmlNodeReference first(const char* tag) {
            m_Idx = 0;
            return next(tag);
        }

        XmlNodeReference next() {
            if (!m_node) {
                return XmlNodeReference(0);
            }

            if (m_Idx < m_node->getChildCount()) {
                return m_node->getChild(m_Idx++);
            }

            return XmlNodeReference(0);
        }
        XmlNodeReference next(const char* tag) {
            if (!m_node) {
                return XmlNodeReference(0);
            }

            while (m_Idx < m_node->getChildCount()) {
                XmlNodeReference node = m_node->getChild(m_Idx++);

                if (node->isTag(tag)) {
                    return node;
                }
            }

            return XmlNodeReference(0);
        }
    };

    class XmlConstNodeIt {
        int m_Idx;
        XmlConstNodeRef m_node;

    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(XmlConstNodeIt);

        XmlConstNodeIt(const XmlConstNodeRef& node) {
            reset(node);
        }
        void reset(const XmlConstNodeRef& node) {
            m_Idx = 0;
            m_node = node;
        }
        XmlConstNodeRef first() {
            m_Idx = 0;
            return next();
        }
        XmlConstNodeRef first(const char* tag) {
            m_Idx = 0;
            return next(tag);
        }

        XmlConstNodeRef next() {
            if (!m_node) {
                return XmlConstNodeRef(0);
            }

            if (m_Idx < m_node->getChildCount()) {
                return m_node->getChild(m_Idx++);
            }

            return XmlConstNodeRef(0);
        }
        XmlConstNodeRef next(const char* tag) {
            if (!m_node) {
                return XmlConstNodeRef(0);
            }

            while (m_Idx < m_node->getChildCount()) {
                XmlConstNodeRef node = m_node->getChild(m_Idx++);

                if (node->isTag(tag)) {
                    return node;
                }
            }

            return XmlConstNodeRef(0);
        }
    };

    //////////////////////////////////////////////////////////////////////////

    inline XmlNodeReference::XmlNodeReference(IXmlNode* p_) : p(p_) {
        if (p) {
            p->AddRef();
        }
    }

    inline XmlNodeReference::XmlNodeReference(const behaviac::XmlNodeReference& p_) : p(p_.p) {
        if (p) {
            p->AddRef();
        }
    }

    inline XmlNodeReference::~XmlNodeReference() {
        if (p) {
            p->Release();
        }
    }

    inline behaviac::XmlNodeReference&  XmlNodeReference::operator=(IXmlNode* newp) {
        if (newp) {
            newp->AddRef();
        }

        if (p) {
            p->Release();
        }

        p = newp;
        return *this;
    }

    inline behaviac::XmlNodeReference&  XmlNodeReference::operator=(const behaviac::XmlNodeReference& newp) {
        if (newp.p) {
            newp.p->AddRef();
        }

        if (p) {
            p->Release();
        }

        p = newp.p;
        return *this;
    }

    inline XmlConstNodeRef::XmlConstNodeRef(const IXmlNode* p_) : p(p_) {
        if (p) {
            p->AddRef();
        }
    }

    inline XmlConstNodeRef::XmlConstNodeRef(const XmlConstNodeRef& p_) : p(p_.p) {
        if (p) {
            p->AddRef();
        }
    }

    inline XmlConstNodeRef::XmlConstNodeRef(const behaviac::XmlNodeReference& p_) : p(p_.p) {
        if (p) {
            p->AddRef();
        }
    }

    inline XmlConstNodeRef::~XmlConstNodeRef() {
        if (p) {
            p->Release();
        }
    }

    inline XmlConstNodeRef&  XmlConstNodeRef::operator=(const IXmlNode* newp) {
        if (newp) {
            newp->AddRef();
        }

        if (p) {
            p->Release();
        }

        p = newp;
        return *this;
    }

    inline XmlConstNodeRef&  XmlConstNodeRef::operator=(const XmlConstNodeRef& newp) {
        if (newp.p) {
            newp.p->AddRef();
        }

        if (p) {
            p->Release();
        }

        p = newp.p;
        return *this;
    }

    inline XmlConstNodeRef&  XmlConstNodeRef::operator=(const behaviac::XmlNodeReference& newp) {
        if (newp.p) {
            newp.p->AddRef();
        }

        if (p) {
            p->Release();
        }

        p = newp.p;
        return *this;
    }

    BEHAVIAC_API IXmlNode* CreateXmlNode(const char* tag);

    template <> inline void IXmlNode::setAttr(const char* key, const behaviac::string& value) {
        setAttrText(key, value.c_str());
    }
    template <> inline void IXmlNode::setAttr(const char* key, const behaviac::CStringCRC& value) {
        setAttrStringIDKind(key, value);
    }
    template <> inline bool IXmlNode::getAttr(const char* key, const char*& value) const {
        const char* attrValue = getAttr(key);

        if (attrValue) {
            value = attrValue;
        }

        return attrValue != NULL;
    }
    template <> inline bool IXmlNode::getAttr(const char* key, behaviac::string& value) const {
        const char* attrValue = getAttr(key);

        if (attrValue) {
            value = attrValue;
        }

        return attrValue != NULL;
    }
    template <> inline bool IXmlNode::getAttr(const char* key, behaviac::CStringCRC& value) const {
        return getAttrStringIDKind(key, value);
    }
}//namespace behaviac

BEHAVIAC_OVERRIDE_TYPE_NAME(behaviac::XmlNodeReference);
BEHAVIAC_OVERRIDE_TYPE_NAME(behaviac::XmlConstNodeRef);

#endif // #ifndef _BEHAVIAC_COMMON_IXML_H_
