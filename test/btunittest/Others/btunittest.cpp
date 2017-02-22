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
#include "behaviac/behaviac.h"
#include "test.h"

#include "behaviac/behaviortree/nodes/composites/sequence.h"
#include "behaviac/behaviortree/nodes/composites/selector.h"
#include "behaviac/behaviortree/nodes/actions/noop.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/nodes/actions/wait.h"

#include "behaviac/behaviortree/generator.h"

#if BEHAVIAC_CCDEFINE_MSVC
TEST(btunittest, coroutine)
{
    $generator(descent)
    {
        // place for all variables used in the generator
        int i; // our counter

        // place the constructor of our generator, e.g.
        // descent(int minv, int maxv) {...}

        // from $emit to $stop is a body of our generator:

        $emit(int) // will emit int values. Start of body of the generator.

        for (i = 10; i > 0; --i)
        {
            CHECK_EQUAL(1, 1);
            $yield(i); // a.k.a. yield in Python,
        }

        // returns next number in [1..10], reversed.
        $stop; // stop, end of sequence. End of body of the generator.
    };

    descent gen;

    for (int n = 0; gen(n);)   // "get next" generator invocation
    {
    }
}
#endif


TEST(btunittest, mbstowcs)
{
    behaviac::string str = "A中B国C";
    size_t mbs_len = str.size();
    BEHAVIAC_UNUSED_VAR(mbs_len);
    //CHECK_EQUAL(7, mbs_len);
    BEHAVIAC_ASSERT(mbs_len == 7 || mbs_len == 9);

    behaviac::wstring wstr;

    bool bOk = behaviac::StringUtils::MBSToWCS(wstr, str, "chs");

    //bOk is true only when BEHAVIAC_CCDEFINE_MSVC
    if (bOk)
    {
        CHECK_EQUAL(true, bOk);

		size_t wcs_len = wstr.size();
        CHECK_EQUAL(5, wcs_len);
    }
}

TEST(btunittest, wcstombs)
{
    behaviac::wstring wstr = L"A中B国C";
	size_t wcs_len = wstr.size();
    CHECK_EQUAL(5, wcs_len);

    behaviac::string str;

    bool bOk = behaviac::StringUtils::WCSToMBS(str, wstr, "chs");

    //bOk is true only when BEHAVIAC_CCDEFINE_MSVC
    if (bOk)
    {
        CHECK_EQUAL(true, bOk);

		size_t mbs_len = str.size();
        BEHAVIAC_UNUSED_VAR(mbs_len);
        //CHECK_EQUAL(7, mbs_len);
        BEHAVIAC_ASSERT(mbs_len == 7 || mbs_len == 9);
    }
}

TEST(btunittest, stringconvert)
{
    behaviac::wstring wstr = L"A中B国C";

    behaviac::string s = behaviac::StringUtils::WCSToMBS(wstr);
    behaviac::wstring ws = behaviac::StringUtils::MBSToWCS(s);

#if !BEHAVIAC_CCDEFINE_APPLE
    CHECK_EQUAL(0, wcscmp(wstr.c_str(), ws.c_str()));
#endif 
}
