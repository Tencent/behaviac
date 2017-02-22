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

#ifndef _BEHAVIAC_COMMON_CONTAINER_VECTOR_H_
#define _BEHAVIAC_COMMON_CONTAINER_VECTOR_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"

#include <vector>
#include "behaviac/common/memory/stl_allocator.h"

namespace behaviac {
    template<class _Ty, class _Alloc = behaviac::stl_allocator<_Ty> >
    class vector : public std::vector<_Ty, _Alloc> {
    public:
        typedef typename std::vector<_Ty, _Alloc>::value_type value_type;
        typedef typename std::vector<_Ty, _Alloc>::size_type size_type;
        typedef typename behaviac::vector<_Ty, _Alloc> _Myt;

        vector() : std::vector<_Ty, _Alloc>() {
        }

        explicit vector(const _Alloc& _Al)
            : std::vector<_Ty, _Alloc>(_Al) {
        }

        explicit vector(size_type _Count)
            : std::vector<_Ty, _Alloc>::vector(_Count)
        {}

        vector(size_type _Count, const value_type& _Val)
            : std::vector<_Ty, _Alloc>::vector(_Count, _Val)
        {}

        vector(size_type _Count, const value_type& _Val, const _Alloc& _Al)
            : std::vector<_Ty, _Alloc>::vector(_Count, _Val, _Al) {
        }

        vector(const _Myt& _Right)
            : std::vector<_Ty, _Alloc>::vector(_Right) {
        }

        vector(const _Myt& _Right, const _Alloc& _Al) :
            std::vector<_Ty, _Alloc>::vector(_Right, _Al)
        {}
    };
}//namespace behaviac

#endif //#ifndef _BEHAVIAC_COMMON_CONTAINER_VECTOR_H_
