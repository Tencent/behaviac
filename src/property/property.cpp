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

#include "behaviac/common/assert.h"
#include "behaviac/common/crc.h"
#include "behaviac/property/property.h"
#include "behaviac/agent/agent.h"


#include "behaviac/common/logger/logger.h"

behaviac::vector<IList::IListPool**>* IList::ms_pools;

behaviac::vector<IList::IListPool**>& IList::GetPools() {
    if (ms_pools == 0) {
        ms_pools = BEHAVIAC_NEW behaviac::vector<IListPool**>;
    }

    return *ms_pools;
}

void IList::Cleanup() {
    if (ms_pools) {
        for (behaviac::vector<IListPool**>::iterator it = ms_pools->begin(); it != ms_pools->end(); ++it) {
            IListPool** ppListPool = *it;
            IListPool* pListPool = *ppListPool;
            pListPool->cleanup();
            BEHAVIAC_DELETE pListPool;
            (*ppListPool) = 0;
        }

        ms_pools->clear();
        BEHAVIAC_DELETE ms_pools;
        ms_pools = 0;
    }
}

namespace behaviac {
    uint32_t MakeVariableId(const char* idString) {
        return CRC32::CalcCRC(idString);
    }

}//namespace behaviac
