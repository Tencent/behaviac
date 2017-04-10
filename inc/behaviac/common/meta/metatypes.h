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

#ifndef _BEHAVIAC_COMMON_META_METATYPES_H_
#define _BEHAVIAC_COMMON_META_METATYPES_H_

#include "behaviac/common/base.h"
#include "behaviac/common/assert.h"
#include "behaviac/common/staticassert.h"

#include "behaviac/common/meta/metabase.h"
#include "behaviac/common/meta/removeconst.h"
#include "behaviac/common/meta/removeref.h"
#include "behaviac/common/meta/removeptr.h"
#include "behaviac/common/meta/removeall.h"

#include "behaviac/common/meta/pointertype.h"
#include "behaviac/common/meta/reftype.h"

#include "behaviac/common/meta/isenum.h"
#include "behaviac/common/meta/isvector.h"
#include "behaviac/common/meta/ismap.h"
#include "behaviac/common/meta/isstruct.h"

#include "behaviac/common/meta/ifthenelse.h"
#include "behaviac/common/meta/hasfunction.h"

#define _BASETYPE_(T) typename behaviac::Meta::RemovePtr<typename behaviac::Meta::RemoveRef<T>::Result>::Result

#define REAL_BASETYPE(T) typename behaviac::Meta::RemoveConst<_BASETYPE_(T)>::Result

#define VALUE_TYPE(T) typename behaviac::Meta::RemoveConst<typename behaviac::Meta::RemoveRef<T>::Result>::Result
#define POINTER_TYPE(T) typename behaviac::Meta::PointerType<T>::Result

namespace behaviac {
    class Agent;

    namespace Meta {
        template <typename T, bool bPtr>
        class ParamTypeConverter {
        public:
            typedef REAL_BASETYPE(T)		BaseType;
            typedef POINTER_TYPE(T)			PointerType;
        };

        template <>
        class ParamTypeConverter<const char*, true> {
        public:
            typedef behaviac::string		BaseType;
            typedef const char**			PointerType;
        };

        template<typename T>
        struct ParamCalledType {
        private:
            //can't remove const
            typedef typename behaviac::Meta::RefType<T>::Result												RefType_t;
        public:
            typedef typename behaviac::Meta::IfThenElse<behaviac::Meta::IsPtr<T>::Result, RefType_t, T>::Result Result;
        };

        template <typename T>
        struct IsAgent {
            typedef REAL_BASETYPE(T)		TBaseType;

            enum {
                Result = behaviac::Meta::IsSame<behaviac::Agent, TBaseType>::Result || behaviac::Meta::IsDerived<behaviac::Agent, TBaseType>::Result
            };
        };

        template <typename T>
        struct TypeMapperTo {
            typedef T Type;
        };

        template <typename T>
        struct TIsRefType {
            enum {
                Result = false
            };
        };

        template <typename T, bool bHasFromString>
        struct IsRefTypeStruct {
            enum {
                Result = behaviac::Meta::TIsRefType<T>::Result
            };
        };

        //template <typename T>
        //class IsRefTypeStruct<T, true> {
        //public:
        //    enum {
        //        Result = T::ms_bIsRefType
        //    };
        //};

        template <typename T>
        struct IsRefType {
            typedef REAL_BASETYPE(T)						TBaseType;
            typedef typename TypeMapperTo<TBaseType>::Type	MappedType;
            enum {
                Result = behaviac::Meta::IsAgent<TBaseType>::Result || behaviac::Meta::IsRefTypeStruct<MappedType, behaviac::Meta::HasFromString<MappedType>::Result>::Result
            };
        };
    }//namespace Meta
}//namespace behaviac

#define PARAM_BASETYPE(T)		typename behaviac::Meta::ParamTypeConverter<T, behaviac::Meta::IsPtr<T>::Result>::BaseType
#define PARAM_POINTERTYPE(T)	typename behaviac::Meta::ParamTypeConverter<T, behaviac::Meta::IsPtr<T>::Result>::PointerType

#define PARAM_CALLEDTYPE(T)	typename behaviac::Meta::ParamCalledType<T>::Result
//////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////


#endif//_BEHAVIAC_COMMON_META_METATYPES_H_
