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

#include "../behaviortest.h"
#include "test.h"
#include "behaviac/behaviortree/nodes/composites/selectorstochastic.h"

using namespace behaviac;

//TEST(btunittest, selectorstochastic_trivial)
//{
//
//    SelectorStochasticTask* node = BEHAVIAC_NEW SelectorStochasticTask();
//
//    DecoratorCountMock countMock1(1);
//    node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock1));
//    behaviac::Agent* pAgent = 0;
//    CHECK_EQUAL(BT_SUCCESS, node->exec(pAgent));
//    BEHAVIAC_DELETE(node);
//
//}
//
//TEST(btunittest, selectorstochastic_FiftyFifty)
//{
//
//    SelectorStochasticTask* node = BEHAVIAC_NEW SelectorStochasticTask();
//
//    DecoratorCountMock countMock1(1);
//    node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock1));
//    node->addChild(BEHAVIAC_NEW FailureAfter(&countMock1));
//    behaviac::Agent* pAgent = 0;
//    int successes = 0, failures = 0;
//
//    for (int i = 0; i < 10000; i++)
//    {
//        node->reset(0);
//
//        switch (node->exec(pAgent))
//        {
//            case BT_FAILURE:
//                failures++;
//                break;
//
//            case BT_SUCCESS:
//                successes++;
//                break;
//
//            default:
//                break;
//        }
//    }
//
//    CHECK_EQUAL(10000, successes);
//    BEHAVIAC_DELETE(node);
//
//}
