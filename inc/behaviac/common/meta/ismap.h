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

#ifndef _BEHAVIAC_COMMON_META_ISMAP_H_
#define _BEHAVIAC_COMMON_META_ISMAP_H_

#include "behaviac/common/meta/metabase.h"

namespace behaviac {
    namespace Meta {
        template<typename TTYPE>
        struct IsMap {
            enum { Result = 0 };
            typedef TTYPE KeyType;
            typedef TTYPE ValueType;
        };

        template<typename KType, typename VType>
        struct IsMap< behaviac::map<KType, VType> > {
            enum { Result = 1 };
            typedef KType KeyType;
            typedef VType ValueType;
        };
    }
}

#endif//_BEHAVIAC_COMMON_META_ISMAP_H_
