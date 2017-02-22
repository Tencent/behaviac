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

#ifndef _BEHAVIAC_COMMON_CONFIG_H_
#define _BEHAVIAC_COMMON_CONFIG_H_

/**
BEHAVIAC_RELEASE	0	// development mode
BEHAVIAC_RELEASE	1	// release/retail mode

BEHAVIAC_RELEASE MUST be defined the same in lib(behaviac) and app(your game).

Please don't define BEHAVIAC_RELEASE unless you know what you are doing.
*/
#include "behaviac/common/_config.h"
#include "behaviac/common/staticassert.h"
#include "behaviac/common/defines.h"
//#include "behaviac/common/basictypes.h"

//if BEHAVIAC_RELEASE is not defined in _config.h, BEHAVIAC_RELEASE will be defined
// as 0 for Debug and 1 for Release
//if BEHAVIAC_RELEASE is defined in _config.h, then, it will be used and will not be defined in the following.
#if (defined(_DEBUG) || defined(DEBUG))
#ifndef BEHAVIAC_RELEASE
#define BEHAVIAC_RELEASE				0
#endif
#else
#ifndef BEHAVIAC_RELEASE
#define BEHAVIAC_RELEASE				1
#endif
#endif//

#if _MSC_VER
#define BEHAVIAC_STRING_CONTAT(A, B)	BEHAVIAC_JOIN_TOKENS(A, B)
#else
#define BEHAVIAC_STRING_CONTAT(A, B)	A B
#endif

#if BEHAVIAC_CCDEFINE_64BITS
#define _BEHAVIAC_BUILD_BITS_STR_ "_64"
#else
#define _BEHAVIAC_BUILD_BITS_STR_ "_32"
#endif

#define _BEHAVIAC_VERSION_STRING_ BEHAVIAC_STRING_CONTAT("_", BEHAVIAC_VERSION_STRING)
//ex: vs2013_3.4.3
#define _BEHAVIAC_TEMP_COMPILER_VERSION_STR_ BEHAVIAC_STRING_CONTAT(BEHAVIAC_CCDEFINE_NAME, _BEHAVIAC_VERSION_STRING_)
//ex: vs2013_3.4.3_32
#define _BEHAVIAC_TEMP_BITS_COMPILER_VERSION_STR_ BEHAVIAC_STRING_CONTAT(_BEHAVIAC_TEMP_COMPILER_VERSION_STR_, _BEHAVIAC_BUILD_BITS_STR_)
//ex: behaviac_vs2013_3.4.3_32
#define _BEHAVIAC_CCDEFINE_VERSION_STR_ BEHAVIAC_STRING_CONTAT("behaviac_", _BEHAVIAC_TEMP_BITS_COMPILER_VERSION_STR_)

#if (defined(_DEBUG) || defined(DEBUG))
#define _BEHAVIAC_BUILD_CONFIG_STR_ "_debug"
#else
#define _BEHAVIAC_BUILD_CONFIG_STR_ "_ndebug"
#endif//

//ex: behaviac_vs2013_3.4.3_32_debug
#define _BEHAVIAC_BUILD_INFO_STR_ BEHAVIAC_STRING_CONTAT(_BEHAVIAC_CCDEFINE_VERSION_STR_, _BEHAVIAC_BUILD_CONFIG_STR_)

//ex: behaviac_vs2013_3.4.3_32_debug_NRELEASE
#if BEHAVIAC_RELEASE
#define BEHAVIAC_BUILD_CONFIG_STR BEHAVIAC_STRING_CONTAT(_BEHAVIAC_BUILD_INFO_STR_, "_RELEASE")
#else
#define BEHAVIAC_BUILD_CONFIG_STR BEHAVIAC_STRING_CONTAT(_BEHAVIAC_BUILD_INFO_STR_, "_NRELEASE")
#endif

#if !BEHAVIAC_RELEASE
#define BEHAVIAC_ENABLE_HOTRELOAD	1
#define BEHAVIAC_ENABLE_PROFILING	1
#endif//BEHAVIAC_RELEASE

//please don't use the following macros in the public header files unless you know what you are doing.
//otherwise, it might be causing uncompatible layout for types
#ifndef BEHAVIAC_ENABLE_NETWORKD
#define BEHAVIAC_ENABLE_NETWORKD	0
#endif//BEHAVIAC_ENABLE_NETWORKD

#ifndef BEHAVIAC_USE_HTN
// by default, disable HTN
#define BEHAVIAC_USE_HTN          0
#endif//BEHAVIAC_USE_HTN

#ifndef BEHAVIAC_USE_CUSTOMSTRING
#define BEHAVIAC_USE_CUSTOMSTRING		0
#endif

#endif//_BEHAVIAC_COMMON_CONFIG_H_
