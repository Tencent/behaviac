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

#ifndef _BEHAVIAC_COMMON_SMART_PTR_H_
#define _BEHAVIAC_COMMON_SMART_PTR_H_

#include "behaviac/common/base.h"

class BEHAVIAC_API CRefCounted {
public:
    BEHAVIAC_DECLARE_MEMORY_OPERATORS(CRefCounted);

public:
    CRefCounted()
        : m_nRefCounter(0)
    {}
    CRefCounted(const CRefCounted& other)
        : m_nRefCounter(0) {
        BEHAVIAC_UNUSED_VAR(other);
    }

    virtual ~CRefCounted() {
        BEHAVIAC_ASSERT(!m_nRefCounter);
    }

    CRefCounted& operator=(const CRefCounted& other) {
        BEHAVIAC_UNUSED_VAR(other);
        return *this;
    }

    void AddRef() {
        ++m_nRefCounter;
    }

    void Release() {
        if (--m_nRefCounter <= 0) {
            DeleteRefCounted();
        }
    }

    long NumRefs() {
        return m_nRefCounter;
    }

protected:

    virtual void DeleteRefCounted() {
        BEHAVIAC_DELETE(this);
    }

    long m_nRefCounter;
};

#endif //_BEHAVIAC_COMMON_SMART_PTR_H_
