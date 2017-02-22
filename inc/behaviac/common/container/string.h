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

#ifndef _BEHAVIAC_COMMON_CONTAINER_STRING_H_
#define _BEHAVIAC_COMMON_CONTAINER_STRING_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"

#include <string>
#include <algorithm>
#include "behaviac/common/memory/stl_allocator.h"

namespace behaviac {
#if BEHAVIAC_USE_CUSTOMSTRING
    typedef std::basic_string<char, std::char_traits<char>, behaviac::stl_allocator<char> > string;
    typedef std::basic_string<wchar_t, std::char_traits<wchar_t>, behaviac::stl_allocator<wchar_t> > wstring;
#else
	typedef std::string string;
	typedef std::wstring wstring;
	//template<class _Elem>
	//class basic_string_t : public std::basic_string<_Elem, std::char_traits<_Elem>, std::allocator<_Elem> >
	//{
	//};

	//typedef basic_string_t<char, std::char_traits<char>, std::allocator<char> > string;
	//typedef basic_string_t<wchar_t, std::char_traits<wchar_t>, std::allocator<wchar_t> > wstring;
#endif//BEHAVIAC_USE_CUSTOMSTRING

    inline behaviac::string make_lower(const behaviac::string& src) {
        behaviac::string dst = src;
        std::transform(dst.begin(), dst.end(), dst.begin(), ::tolower);

        return dst;
    }

    inline behaviac::string make_upper(const behaviac::string& src) {
        behaviac::string dst = src;
        std::transform(dst.begin(), dst.end(), dst.begin(), ::toupper);

        return dst;
    }

}

#endif //#ifndef _BEHAVIAC_COMMON_CONTAINER_STRING_H_
