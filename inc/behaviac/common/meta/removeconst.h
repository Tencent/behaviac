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

#ifndef _BEHAVIAC_COMMON_META_REMOVECONST_H_
#define _BEHAVIAC_COMMON_META_REMOVECONST_H_

namespace behaviac {
    namespace Meta {
        template< typename T >
        struct RemoveConst {
            typedef T Result;
        };

        template< typename T >
        struct RemoveConst< const T > {
            typedef T Result;
        };

        template< typename T >
        struct RemoveConst< const T* > {
            typedef T* Result;
        };

        template< typename T >
        struct RemoveConst< const T& > {
            typedef T& Result;
        };

        template <typename T>
        struct RemoveConst<volatile T> {
            typedef volatile typename RemoveConst<T>::Result Result;
        };

        template <typename T>
        struct RemoveConst<volatile const T> {
            typedef volatile typename RemoveConst<T>::Result Result;
        };

        template <typename T>
        struct RemoveConst<T&> {
            typedef typename RemoveConst<T>::Result& Result;
        };

        template <typename T>
        struct RemoveConst<T*> {
            typedef typename RemoveConst<T>::Result* Result;
        };

        template <typename T>
        struct RemoveConst<T* const> {
            typedef typename RemoveConst<T>::Result* Result;
        };

        template <typename T>
        struct RemoveConst<T* volatile const> {
            typedef typename RemoveConst<T>::Result* volatile Result;
        };
    }
}

#endif
