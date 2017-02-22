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

#include "behaviac/common/base.h"
#include "test.h"
#include "../behaviortest.h"
//#include "behaviac/common/timer/timer.h"
#include "behaviac/behaviortree/nodes/composites/selectorloop.h"
#include "behaviac/behaviortree/nodes/composites/withprecondition.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorlog.h"
#include "behaviac/behaviortree/nodes/conditions/true.h"

using namespace behaviac;

//TEST(btunittest, selectorloop_basic1)
//{
//
//    SelectorLoopTask* node = BEHAVIAC_NEW SelectorLoopTask();
//
//    DecoratorCountMock countMock2(2);
//    WithPreconditionTask* pWithPrecondition1 = BEHAVIAC_NEW WithPreconditionTask();
//    pWithPrecondition1->addChild(BEHAVIAC_NEW FailureUntil(&countMock2));
//    pWithPrecondition1->addChild(BEHAVIAC_NEW TrueTask());
//    node->addChild(pWithPrecondition1);
//
//    WithPreconditionTask* pWithPrecondition2 = BEHAVIAC_NEW WithPreconditionTask();
//    pWithPrecondition2->addChild(BEHAVIAC_NEW TrueTask());
//
//    DecoratorAlwaysRunningTask* b = BEHAVIAC_NEW DecoratorAlwaysRunningTask();
//    BehaviorTask* c = BEHAVIAC_NEW NoopTask();
//    b->addChild(c);
//
//    pWithPrecondition2->addChild(b);
//    node->addChild(pWithPrecondition2);
//
//    behaviac::Agent* pAgent = 0;
//
//    CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//    CHECK_EQUAL(BT_SUCCESS, node->exec(pAgent));
//
//    BEHAVIAC_DELETE(node);
//
//}
