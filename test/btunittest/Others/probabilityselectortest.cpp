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
#include "behaviac/behaviortree/nodes/composites/selectorprobability.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorweight.h"

using namespace behaviac;

class DecoratorWeightMock : public behaviac::DecoratorWeight
{
public:
    BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorWeightMock, behaviac::DecoratorWeight);

    DecoratorWeightMock(int w) : behaviac::DecoratorWeight()
    {
        m_weight = w;
    }

    virtual BehaviorTask* CreateTask() const
    {
        BehaviorTask* pTask = BEHAVIAC_NEW DecoratorWeightTask();
        pTask->Init(this);

        return pTask;
    }

    virtual bool IsValid(behaviac::Agent* pAgent, BehaviorTask* pTask) const
    {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(pTask);

        return true;
    }

    virtual int GetWeight(behaviac::Agent* pAgent) const
    {
        BEHAVIAC_UNUSED_VAR(pAgent);

        return m_weight;
    }

    int m_weight;
};

class SelectorProbabilityNode : public SelectorProbability
{
public:
    BEHAVIAC_DECLARE_DYNAMIC_TYPE(SelectorProbabilityNode, SelectorProbability);

    SelectorProbabilityNode() : SelectorProbability()
    {}

    virtual ~SelectorProbabilityNode()
    {}

    virtual BehaviorTask* CreateTask() const
    {
        BehaviorTask* pTask = BEHAVIAC_NEW SelectorProbabilityTask();
        pTask->Init(this);

        return pTask;
    }

    virtual bool IsValid(behaviac::Agent* pAgent, BehaviorTask* pTask) const
    {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(pTask);

        return true;
    }

    void AddChildWithWeight(BehaviorNode* pBehavior, int w)
    {
        DecoratorWeightMock* pDW = BEHAVIAC_NEW DecoratorWeightMock(w);
        pDW->AddChild(pBehavior);

        super::AddChild(pDW);
    }
};

//TEST(btunittest, trivial)
//{
//
//    SelectorProbabilityNode node;
//
//    DecoratorCountMock* countMock1 = BEHAVIAC_NEW DecoratorCountMock(1, true);
//
//    node.AddChildWithWeight(countMock1, 1);
//    BehaviorTask* task = node.CreateTask();
//
//    behaviac::Agent* pAgent = 0;
//
//    CHECK_EQUAL(BT_SUCCESS, task->exec(pAgent));
//    BEHAVIAC_DELETE(task);
//
//}
//
//TEST(btunittest, FiftyFifty)
//{
//
//    SelectorProbabilityNode node;
//
//    DecoratorCountMock* countMock1 = BEHAVIAC_NEW DecoratorCountMock(1, true);
//    DecoratorCountMock* countMock2 = BEHAVIAC_NEW DecoratorCountMock(1, false);
//
//    node.AddChildWithWeight(countMock1, 1);
//    node.AddChildWithWeight(countMock2, 1);
//
//    BehaviorTask* task = node.CreateTask();
//
//    behaviac::Agent* pAgent = 0;
//
//    int successes = 0, failures = 0;
//
//    for (int i = 0; i < 10000; i++)
//    {
//        task->reset(0);
//
//        switch (task->exec(pAgent))
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
//    if (behaviac::Max(failures, successes) > 5100)
//    {
//        BEHAVIAC_ASSERT(0, "questionable statistical distribution");
//    }
//
//    BEHAVIAC_DELETE(task);
//
//}
//
//TEST(btunittest, WithWeights1)
//{
//
//    SelectorProbabilityNode node;
//
//    DecoratorCountMock* countMock1 = BEHAVIAC_NEW DecoratorCountMock(1, true);
//    DecoratorCountMock* countMock2 = BEHAVIAC_NEW DecoratorCountMock(1, false);
//
//    node.AddChildWithWeight(countMock1, 9);
//    node.AddChildWithWeight(countMock2, 1);
//
//    BehaviorTask* task = node.CreateTask();
//
//    behaviac::Agent* pAgent = 0;
//
//    int successes = 0, failures = 0;
//
//    for (int i = 0; i < 10000; i++)
//    {
//        task->reset(0);
//
//        switch (task->exec(pAgent))
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
//    if (failures < 950 || failures > 1050 || successes < 8950 || successes > 9050)
//    {
//        BEHAVIAC_ASSERT(0, "questionable statistical distribution");
//    }
//
//    BEHAVIAC_DELETE(task);
//
//}
