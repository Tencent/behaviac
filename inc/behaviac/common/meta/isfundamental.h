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

#ifndef _BEHAVIAC_COMMON_META_ISFUNDAMENTAL_H_
#define _BEHAVIAC_COMMON_META_ISFUNDAMENTAL_H_

namespace behaviac {
    namespace Meta {
        template< typename Type >
        struct IsFundamental {
            enum {
                Result = 0
            };
        };

#define BEHAVIAC_IS_FUNDAMENTAL( Type )     \
    template<>                          \
    struct IsFundamental< Type >        \
    {                                   \
        enum                            \
        {                               \
            Result = 1                  \
        };                              \
    };

        BEHAVIAC_IS_FUNDAMENTAL(uint8_t)
        BEHAVIAC_IS_FUNDAMENTAL(uint16_t)
        BEHAVIAC_IS_FUNDAMENTAL(uint32_t)

        BEHAVIAC_IS_FUNDAMENTAL(int8_t)
        BEHAVIAC_IS_FUNDAMENTAL(int16_t)
        //int32_t is actually a typedef of int
        //BEHAVIAC_IS_FUNDAMENTAL(int32_t)

        BEHAVIAC_IS_FUNDAMENTAL(signed long)
        BEHAVIAC_IS_FUNDAMENTAL(unsigned long)

        BEHAVIAC_IS_FUNDAMENTAL(int)

        BEHAVIAC_IS_FUNDAMENTAL(char)
        BEHAVIAC_IS_FUNDAMENTAL(bool)

#if !BEHAVIAC_CCDEFINE_64BITS
        BEHAVIAC_IS_FUNDAMENTAL(int64_t)
        BEHAVIAC_IS_FUNDAMENTAL(uint64_t)
#else
        BEHAVIAC_IS_FUNDAMENTAL(long long)
        BEHAVIAC_IS_FUNDAMENTAL(unsigned long long)
#endif//BEHAVIAC_CCDEFINE_64BITS

        BEHAVIAC_IS_FUNDAMENTAL(float)
        BEHAVIAC_IS_FUNDAMENTAL(double)

        BEHAVIAC_IS_FUNDAMENTAL(void)
    }
}

#endif
