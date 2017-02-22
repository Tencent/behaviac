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
#include "behaviac/common/operation.h"
#include "behaviac/common/rttibase.h"


namespace behaviac {

    behaviac::map<behaviac::string, IIComputeValue*> ComputerRegister::ms_valueComputers;

    void ComputerRegister::Register() {
        ms_valueComputers[GetTypeDescString<int>()] = BEHAVIAC_NEW ComputeValueInt();
        ms_valueComputers[GetTypeDescString<long>()] = BEHAVIAC_NEW ComputeValueLong();
        ms_valueComputers[GetTypeDescString<short>()] = BEHAVIAC_NEW ComputeValueShort();
        ms_valueComputers[GetTypeDescString<char>()] = BEHAVIAC_NEW ComputeValueByte();
        ms_valueComputers[GetTypeDescString<uint32_t>()] = BEHAVIAC_NEW ComputeValueuint32_t();
        ms_valueComputers[GetTypeDescString<unsigned long>()] = BEHAVIAC_NEW ComputeValueULong();
        ms_valueComputers[GetTypeDescString<unsigned short>()] = BEHAVIAC_NEW ComputeValueUShort();
        ms_valueComputers[GetTypeDescString<unsigned char>()] = BEHAVIAC_NEW ComputeValueUByte();
        ms_valueComputers[GetTypeDescString<float>()] = BEHAVIAC_NEW ComputeValueFloat();
        ms_valueComputers[GetTypeDescString<double>()] = BEHAVIAC_NEW ComputeValueDouble();

    }
    void ComputerRegister::Init() {
        if (ms_valueComputers.size() == 0) {
            Register();
        }
    }
    void ComputerRegister::Cleanup() {
        for (behaviac::map<behaviac::string, IIComputeValue*>::iterator it = ms_valueComputers.begin(); it != ms_valueComputers.end(); ++it) {
            IIComputeValue* p = it->second;
            BEHAVIAC_DELETE p;
        }

        ms_valueComputers.clear();
    }
    IIComputeValue* ComputerRegister::Get(behaviac::string type) {
        if (ms_valueComputers.find(type) != ms_valueComputers.end()) {
            IIComputeValue* pComparer = ms_valueComputers[type];
            return pComparer;
        }

        BEHAVIAC_ASSERT(false);

        return NULL;
    }

}
