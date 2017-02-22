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

#ifndef _BEHAVIAC_PROPERTY_H_
#define _BEHAVIAC_PROPERTY_H_

#include "behaviac/common/rttibase.h"
#include "behaviac/common/factory.h"
#include "behaviac/common/base.h"
#include "behaviac/common/object/tagobject.h"
#include "behaviac/common/string/fromstring.h"
#include "behaviac/property/vector_ext.h"

namespace behaviac {
    class IMemberBase;
    //------------------------------------------------------------------------
    BEHAVIAC_API uint32_t MakeVariableId(const char* idString);
    BEHAVIAC_API const char* GetNameWithoutClassName(const char* variableName);
    enum EComputeOperator {
        ECO_INVALID,
        ECO_ADD,
        ECO_SUB,
        ECO_MUL,
        ECO_DIV
    };

}

#endif//_BEHAVIAC_PROPERTY_H_
