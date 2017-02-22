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

#include "behaviac/common/config.h"
#include "behaviac/common/string/stringcrc.h"
#include "behaviac/common/crc.h"


////////////////////////////////////////

namespace behaviac {
    const static CStringCRC::IDType InvalidID = 0xFFFFFFFF;

    CStringCRC::CStringCRC() : m_value(static_cast<IDType>(InvalidID)) {

    }

    CStringCRC::CStringCRC(const char* str) : m_value(CRC32::CalcCRC(str)) {

    }

    void CStringCRC::ParseString(const char* content) {
        this->m_value = CRC32::CalcCRC(content);
    }

    bool CStringCRC::IsValid() const {
        return m_value != static_cast<IDType>(InvalidID);
    }

    void CStringCRC::Cleanup() {

    }

}//namespace behaviac