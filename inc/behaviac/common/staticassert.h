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

#ifndef _BEHAVIAC_COMMON_STATICASSERT_H_
#define _BEHAVIAC_COMMON_STATICASSERT_H_

#define BEHAVIAC_JOIN_TOKENS(a, b)			BEHAVIAC_JOIN_TOKENS_IMPL(a, b)
#define BEHAVIAC_JOIN_TOKENS_IMPL(a,	b)	BEHAVIAC_JOIN_TOKENS_IMPL2(a, b)
#define BEHAVIAC_JOIN_TOKENS_IMPL2(a, b)	a ## b

/**
usage:
int BEHAVIAC_UNIQUE_NAME(intVar) = 0;
*/
#define BEHAVIAC_UNIQUE_NAME(name) BEHAVIAC_JOIN_TOKENS(name, __LINE__)

//#define VA_NARGS(...) VA_NARGS_II((VA_NARGS_PREFIX_ ## __VA_ARGS__ ## _VA_NARGS_POSTFIX,32,31,30,29,28,27,26,25,24,23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,2,1,0))
//#define VA_NARGS_II(__args) VA_NARGS_I __args
//#define VA_NARGS_PREFIX__VA_NARGS_POSTFIX ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,0
//#define VA_NARGS_I(__p0,__p1,__p2,__p3,__p4,__p5,__p6,__p7,__p8,__p9,__p10,__p11,__p12,__p13,__p14,__p15,__p16,__p17,__p18,__p19,__p20,__p21,__p22,__p23,__p24,__p25,__p26,__p27,__p28,__p29,__p30,__p31,__n,...) __n

namespace behaviac {
    namespace internal {
        template <bool> struct STATIC_ASSERT_FAILURE;
        template <> struct STATIC_ASSERT_FAILURE<true> {
            enum { value = 1 };
        };

        template<int x> struct static_assert_test {};
    }//namespae internal
}//namespace behaviac

#define BEHAVIAC_STATIC_ASSERT(x)    typedef behaviac::internal::static_assert_test<sizeof(behaviac::internal::STATIC_ASSERT_FAILURE< (bool)( (x) ) >)> BEHAVIAC_UNIQUE_NAME(_behaviac_static_assert_typedef_) BEHAVIAC_UNUSED

#endif//_BEHAVIAC_COMMON_STATICASSERT_H_
