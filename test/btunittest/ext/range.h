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

#ifndef _CORE_RANGE_H_
#define _CORE_RANGE_H_

#include "behaviac/common/rttibase.h"

template <class T>
class TRange
{
public:
    BEHAVIAC_DECLARE_MEMORY_OPERATORS(TRange)
    T start;
    T end;

    TRange()
    {
        start = 0;
        end = 0;
    };
    TRange(const TRange& r)
    {
        start = r.start;
        end = r.end;
    };
    TRange(T s, T e)
    {
        start = s;
        end = e;
    };
};

typedef TRange<float> Range;

BEHAVIAC_OVERRIDE_TYPE_NAME(Range);

#endif // #ifndef _CORE_RANGE_H_
