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
#include "behaviac/common/memory/memory.h"
#include "behaviac/common/factory.h"
#include "behaviac/common/logger/logger.h"

namespace behaviac {
    bool FactoryUnregister_(FactoryContainer_t* creators, const behaviac::CStringCRC& typeID) {
        creators->Lock();
        SFactoryBag_t bucket(typeID, NULL);
        IteratorCreator itEnd(creators->end());
        IteratorCreator itFound(std::find(creators->begin(), itEnd, bucket));

        if (itFound != itEnd) {
            SFactoryBag_t& item = *itFound;
            BEHAVIAC_FREE(item.m_typeConstructor);
            creators->erase(itFound);
            creators->Unlock();
            return true;
        }

        BEHAVIAC_ASSERT("Cannot find the specified factory entry\n");
        creators->Unlock();
        return false;
    }

    bool FactoryRegister_(FactoryContainer_t* creators, const behaviac::CStringCRC& typeID, void* typeConstructor) {
        SFactoryBag_t bucket(typeID, typeConstructor);
        creators->Lock();
        IteratorCreator itEnd(creators->end());
        IteratorCreator itFound(std::find(creators->begin(), itEnd, bucket));
        bool wasThere = (itFound != itEnd);

        //Add it only once
        if (!wasThere) {
            creators->push_back(bucket);

        } else {
            BEHAVIAC_ASSERT(0, "Trying to register an already registered type %d", typeID.GetUniqueID());
        }

        creators->Unlock();
        return !wasThere;
    }
}//
