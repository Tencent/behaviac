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

#ifndef _BEHAVIAC_COMMON_BASICTYPES_H_
#define _BEHAVIAC_COMMON_BASICTYPES_H_

#include "behaviac/common/config.h"
#include "behaviac/common/staticassert.h"

#if defined(_MSC_VER) && _MSC_VER < 1600 /* MSVC2010 */
#include "behaviac/common/msc_stdint.h"
#else
#include <stdint.h>
#endif

#include <stddef.h>
#include <float.h>
#include <math.h>

namespace behaviac {
    typedef char				Char;
    typedef float				Float32;
    typedef double				Float64;
#if BEHAVIAC_CCDEFINE_64BITS
    typedef uint64_t            Address;
#else
    typedef unsigned long       Address;
#endif
    const Float32	Float32_Epsilon = 0.000001f;

    template <typename DST, typename SRC>
    inline DST unaliased_cast(SRC source) {
        union UnaliasedConverter {
            SRC source;
            DST destination;
        };
        BEHAVIAC_STATIC_ASSERT(sizeof(DST) == sizeof(SRC));
        UnaliasedConverter converter;
        converter.source = source;
        return converter.destination;
    }

    inline bool  IsEqualWithEpsilon(Float64 x, Float64 y, Float64 Epsilon = (Float32_Epsilon * 2)) {
        return fabs(x - y) <= Epsilon;
    }

    template<typename T>
    inline bool InRange(const T& Value, const T& Min, const T& Max) {
        return (Min <= Value) && (Value <= Max);
    }

    template<typename T>
    inline T ClampValue(const T& Value, const T& Min, const T& Max) {
        T returnVal = Value;

        if (Value < Min) {
            returnVal = Min;

        } else if (Value > Max) {
            returnVal = Max;
        }

        return returnVal;
    }

    template<typename T>
    inline void SwapValues(T& a, T& b) {
        T Temp = a;
        a = b;
        b = Temp;
    }

    template<typename _Type>
    _Type BinaryMin(_Type a, _Type b) {
        return a < b ? a : b;
    }

    template <typename _Type>
    _Type BinaryMax(_Type a, _Type b) {
        return a > b ? a : b;
    }
    template <typename _Type>
    inline _Type TripleMin(_Type a, _Type b, _Type c) {
        return a < b ? (a < c ? a : c) : (b < c ? b : c);
    }

    template <typename _Type>
    inline _Type TripleMax(_Type a, _Type b, _Type c) {
        return a > b ? (a > c ? a : c) : (b > c ? b : c);
    }

    template <typename _T>
    inline unsigned CountBits(_T Value) {
        unsigned Count = 0;

        while (Value != 0) {
            Value = Value & (Value - 1);
            Count++;
        }

        return Count;
    }

    template <typename _T>
    inline bool IsPowerOf2(_T Value) {
        return (!Value || !((Value - 1) & Value));
    }

#define BEHAVIAC_MAKE64(Low, High)   ((uint64_t) (((uint32_t) (Low)) | ((uint64_t) ((uint32_t) (High))) << 32))

#define BEHAVIAC_ROUND(Val, Align)                  ((((size_t)Val) + ((Align)-1)) & ~((Align)-1))

#define BEHAVIAC_FLOOR(Val, Align)                  (((size_t)Val) & ~((Align) - 1))
#define BEHAVIAC_ALIGNED(Val, Align)                ((((size_t)Val) & ((Align) - 1)) == 0)
#define BEHAVIAC_PTR_TO_ADDR(ptr)   ((behaviac::Address)((size_t)(ptr)))
#define BEHAVIAC_ADDR_TO_PTR(addr)  ((void*)((size_t)(addr)))
#define BEHAVIAC_PTR_ALIGNED(Ptr, Align)            BEHAVIAC_ALIGNED(BEHAVIAC_PTR_TO_ADDR(Ptr), (Align))
#define BEHAVIAC_ALIGN_PTR(type, Ptr, Ofs)          ((type) BEHAVIAC_FLOOR((size_t)(Ptr), Ofs))
#define BEHAVIAC_DIFF_PTR(First, Second)            ((size_t) (((size_t)(First)) - (size_t)(Second)))
#define BEHAVIAC_ARRAY_NELEMENT(a)					(sizeof(a) / sizeof(a[0]))

#if _MSC_VER >= 1400
    template<typename T_> struct alignment_of {
        static T_& t;
        enum { value = __alignof(t) };
    };

#define BEHAVIAC_ALIGNOF(T)     behaviac::alignment_of<T>::value
#else
#define BEHAVIAC_ALIGNOF(T)		__alignof(T)
#endif//#elif _MSC_VER >= 1400

    template <typename T>
    inline uint32_t AlignOf() {
        return BEHAVIAC_ALIGNOF(T);
    }

    template <>
    inline uint32_t AlignOf<void>() {
        return 1;
    }

}//end of ns

#endif//_BEHAVIAC_COMMON_BASICTYPES_H_
