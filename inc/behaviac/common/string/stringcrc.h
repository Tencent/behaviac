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

#ifndef _BEHAVIAC_COMMON_STRINGCRC_H_
#define _BEHAVIAC_COMMON_STRINGCRC_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"
#include "behaviac/common/basictypes.h"

#if !BEHAVIAC_RELEASE
#define		BEHAVIAC_STRINGID_DEBUG 1
#endif

namespace behaviac {
    class BEHAVIAC_API CStringCRC {
    public:
        typedef uint32_t IDType;
    public:

        CStringCRC();
        virtual ~CStringCRC() {

        }

        CStringCRC(const CStringCRC& other) : m_value(other.m_value) {

        }

        explicit CStringCRC(IDType crc) : m_value(crc) {

        }

        explicit CStringCRC(const char* str);

        void ParseString(const char* content);

        const IDType& GetUniqueID() const {
            return this->m_value;
        }

        void SetUniqueID(IDType crc) {
            this->m_value = crc;
        }

        bool IsValid() const;

        static void Cleanup();
    private:

        IDType m_value;

    public:
        const char* LogStr() const;
    };

#if !BEHAVIAC_CCDEFINE_MSVC
    //#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif//
    inline bool operator==(const CStringCRC& r, const CStringCRC& l) {
        const CStringCRC* pR = (const CStringCRC*)&r;
        const CStringCRC* pL = (const CStringCRC*)&l;

        return pR->GetUniqueID() == pL->GetUniqueID();
    }
#if !BEHAVIAC_CCDEFINE_MSVC
    //#pragma GCC diagnostic pop
#endif

    inline bool operator==(const CStringCRC& r, const CStringCRC::IDType l) {
        return r.GetUniqueID() == l;
    }

    inline bool operator==(const CStringCRC::IDType r, const CStringCRC& l) {
        return r == l.GetUniqueID();
    }

    inline bool operator!=(const CStringCRC& r, const CStringCRC& l) {
        return r.GetUniqueID() != l.GetUniqueID();
    }

    inline bool operator!=(const CStringCRC& r, const CStringCRC::IDType l) {
        return r.GetUniqueID() != l;
    }

    inline bool operator!=(const CStringCRC::IDType r, const CStringCRC& l) {
        return r != l.GetUniqueID();
    }

    inline bool operator<(const CStringCRC& r, const CStringCRC& l) {
        return r.GetUniqueID() < l.GetUniqueID();
    }

}//namespace behaviac

#endif //#ifndef _BEHAVIAC_COMMON_STRINGCRC_H_
