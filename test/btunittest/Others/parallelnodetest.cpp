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
#include "behaviac/behaviortree/nodes/composites/parallel.h"

//using namespace behaviac;
//
//class ParallelNodeMock : public Parallel
//{
//public:
//	void SetPolicy(EFAILURE_POLICY failPolicy = FAIL_ON_ALL, ESUCCESS_POLICY successPolicy = SUCCEED_ON_ALL, EEXIT_POLICY exitPolicty = EXIT_NONE)
//	{
//		m_failPolicy = failPolicy;
//		m_succeedPolicy = successPolicy;
//		m_exitPolicy = exitPolicty;
//	}
//
//	virtual bool IsValid(behaviac::Agent* pAgent, BehaviorTask* pTask) const
//	{
//		BEHAVIAC_UNUSED_VAR(pAgent);
//		BEHAVIAC_UNUSED_VAR(pTask);
//
//		return true;
//	}
//};
//
//class ParallelMock : public ParallelTask
//{
//public:
//	ParallelMock(const ParallelNodeMock* node) : ParallelTask()
//	{
//		this->m_node = (BehaviorNode*)node;
//	}
//
//	~ParallelMock()
//	{
//		for (size_t i = 0; i < this->m_children.size(); ++i)
//		{
//			BEHAVIAC_DELETE(m_children[i]);
//		}
//
//		this->m_children.clear();
//	}
//
//	void SetPolicy(EFAILURE_POLICY failPolicy = FAIL_ON_ALL, ESUCCESS_POLICY successPolicy = SUCCEED_ON_ALL, EEXIT_POLICY exitPolicty = EXIT_NONE)
//	{
//		ParallelNodeMock* pNode = (ParallelNodeMock*)ParallelNodeMock::DynamicCast(this->GetNode());
//		pNode->SetPolicy(failPolicy, successPolicy, exitPolicty);
//	}
//
//	virtual void addChild(BehaviorTask* pBehavior)
//	{
//		super::addChild(pBehavior);
//	}
//};
//
//TEST(btunittest, simple1)
//{
//
//
//	ParallelNodeMock nodeMock;
//	ParallelMock* node = BEHAVIAC_NEW ParallelMock(&nodeMock);
//	node->SetPolicy(FAIL_ON_ALL);
//
//	DecoratorCountMock countMock1(1);
//	DecoratorCountMock countMock4(4);
//
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock4));
//	behaviac::Agent* pAgent = 0;
//
//	for (int i = 0; i < 2; i++)
//	{
//		node->reset(0);
//
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_FAILURE, node->exec(pAgent));
//	}
//
//	BEHAVIAC_DELETE(node);
//
//
//}
//
//TEST(btunittest, simple2)
//{
//
//	ParallelNodeMock nodeMock;
//	ParallelMock* node = BEHAVIAC_NEW ParallelMock(&nodeMock);
//
//	DecoratorCountMock countMock1(1);
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock1));
//
//	behaviac::Agent* pAgent = 0;
//
//	CHECK_EQUAL(BT_FAILURE, node->exec(pAgent));
//
//	BEHAVIAC_DELETE(node);
//
//}
//
//// as currently specified, the parallelnode will neither succeed nor fail in this situation
//// is this the best behavior?
//// seems like it should fail instead of being in limbo
//TEST(btunittest, simple3)
//{
//
//	ParallelNodeMock nodeMock;
//	ParallelMock* node = BEHAVIAC_NEW ParallelMock(&nodeMock);
//
//	node->SetPolicy(FAIL_ON_ALL);
//
//	DecoratorCountMock countMock1(1);
//	DecoratorCountMock countMock4(4);
//
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock4));
//
//	behaviac::Agent* pAgent = 0;
//
//	CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//	CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//	CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//	CHECK_EQUAL(BT_FAILURE, node->exec(pAgent));
//
//	BEHAVIAC_DELETE(node);
//
//}
//
//TEST(btunittest, simple4)
//{
//
//	ParallelNodeMock nodeMock;
//	ParallelMock* node = BEHAVIAC_NEW ParallelMock(&nodeMock);
//
//	DecoratorCountMock countMock1(1);
//	DecoratorCountMock countMock3(3);
//
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock3));
//	behaviac::Agent* pAgent = 0;
//
//	for (int i = 0; i < 2; i++)
//	{
//		node->reset(0);
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_SUCCESS, node->exec(pAgent));
//	}
//
//	BEHAVIAC_DELETE(node);
//
//}
//
//TEST(btunittest, simple5)
//{
//
//	ParallelNodeMock nodeMock;
//	ParallelMock* node = BEHAVIAC_NEW ParallelMock(&nodeMock);
//
//	DecoratorCountMock countMock1(1);
//	DecoratorCountMock countMock2(2);
//	DecoratorCountMock countMock4(4);
//
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock2));
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock4));
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock4));
//	behaviac::Agent* pAgent = 0;
//
//	for (int i = 0; i < 2; i++)
//	{
//		node->reset(0);
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_FAILURE, node->exec(pAgent));
//	}
//
//	BEHAVIAC_DELETE(node);
//
//}
//
//TEST(btunittest, simple6)
//{
//
//	ParallelNodeMock nodeMock;
//	ParallelMock* node = BEHAVIAC_NEW ParallelMock(&nodeMock);
//
//	node->SetPolicy(FAIL_ON_ALL, SUCCEED_ON_ONE);
//
//	DecoratorCountMock countMock1(1);
//	DecoratorCountMock countMock3(3);
//	DecoratorCountMock countMock4(4);
//
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock3));
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW FailureAfter(&countMock4));
//	behaviac::Agent* pAgent = 0;
//
//	for (int i = 0; i < 2; i++)
//	{
//		node->reset(0);
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_SUCCESS, node->exec(pAgent));
//	}
//
//	BEHAVIAC_DELETE(node);
//
//}
//
//TEST(btunittest, simple7)
//{
//
//	ParallelNodeMock nodeMock;
//	ParallelMock* node = BEHAVIAC_NEW ParallelMock(&nodeMock);
//
//	DecoratorCountMock countMock1(1);
//	DecoratorCountMock countMock2(2);
//	DecoratorCountMock countMock4(4);
//	DecoratorCountMock countMock8(8);
//
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock2));
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock1));
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock4));
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock8));
//	node->addChild(BEHAVIAC_NEW SuccessAfter(&countMock2));
//	behaviac::Agent* pAgent = 0;
//
//	for (int i = 0; i < 2; i++)
//	{
//		node->reset(0);
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_RUNNING, node->exec(pAgent));
//		CHECK_EQUAL(BT_SUCCESS, node->exec(pAgent));
//	}
//
//	BEHAVIAC_DELETE(node);
//
//}
