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

//------------------------------------------------------------------------
//------------------------------------------------------------------------

#include "behaviac/common/meta/meta.h"

#include "test.h"

SUITE(behaviac)
{
    SUITE(Type)
    {
        struct TrueCheck
        {
            enum { Result = 1 };
        };
        struct FalseCheck
        {
            enum { Result = 2 };
        };
        TEST(IfThenElse, Test4)
        {
            const int32_t trueCheck = behaviac::Meta::IfThenElse< true, TrueCheck, FalseCheck >::Result::Result;
            const int32_t falseCheck = behaviac::Meta::IfThenElse< false, TrueCheck, FalseCheck >::Result::Result;
            CHECK_EQUAL(1, trueCheck);
            CHECK_EQUAL(2, falseCheck);
        }
    }
}
