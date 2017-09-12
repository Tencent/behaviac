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

#include "../btloadtestsuite.h"
#include "behaviac/common/profiler/profiler.h"

LOAD_TEST(btunittest, circular_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/circular_ut_0", format);
    myTestAgent->resetProperties();

    myTestAgent->SetVariable("testVar_0", 0);

    const int kCount = 10;

    for (int i = 0; i < kCount; ++i)
    {
        myTestAgent->btexec();
        int p = myTestAgent->GetVariable<int>("testVar_0");
        BEHAVIAC_ASSERT(1 == p);
        BEHAVIAC_UNUSED_VAR(p);
    }

    finlTestEnvNode(myTestAgent);
}

//< selector loop test
LOAD_TEST(btunittest, selector_loop_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_loop_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_loop_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_loop_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(-1, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_loop_ut_4)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_4", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    myTestAgent->resetProperties();
    myTestAgent->btexec();
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_loop_ut_5)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_5", format);
	myTestAgent->resetProperties();
	behaviac::EBTStatus s = myTestAgent->btexec();

	CHECK_EQUAL(behaviac::BT_SUCCESS, s);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

    finlTestEnvNode(myTestAgent);
}


LOAD_TEST(btunittest, selector_loop_ut_6)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_6", format);
	myTestAgent->resetProperties();

	myTestAgent->m_bCanSee = false;
	const int kCount = 5;
	for (int i = 0; i <kCount; ++i)
	{
		behaviac::EBTStatus s = myTestAgent->btexec();
		CHECK_EQUAL(behaviac::BT_RUNNING, s);
		CHECK_EQUAL(2, myTestAgent->testVar_0);
	}

	myTestAgent->m_bCanSee = true;

	behaviac::EBTStatus s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	myTestAgent->m_bCanSee = false;

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(2, myTestAgent->testVar_0);

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(2, myTestAgent->testVar_0);

	myTestAgent->m_bCanSee = true;

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, s);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_loop_ut_7)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_7", format);
	myTestAgent->resetProperties();

	myTestAgent->m_bCanSee = false;
	const int kCount = 5;
	for (int i = 0; i <kCount; ++i)
	{
		behaviac::EBTStatus s = myTestAgent->btexec();
		CHECK_EQUAL(behaviac::BT_RUNNING, s);
		CHECK_EQUAL(2, myTestAgent->testVar_0);
		CHECK_EQUAL(i + 1, myTestAgent->testVar_1);
	}

	myTestAgent->m_bCanSee = true;

	behaviac::EBTStatus s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(1, myTestAgent->testVar_0);
	CHECK_EQUAL(6, myTestAgent->testVar_1);

	myTestAgent->m_bTargetValid = false;

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, s);
	CHECK_EQUAL(1, myTestAgent->testVar_0);
	CHECK_EQUAL(3, myTestAgent->testVar_1);

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_loop_ut_8)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_loop_ut_8", format);
	myTestAgent->resetProperties();

	myTestAgent->testVar_0 = 10;
	myTestAgent->m_bCanSee = false;

	behaviac::EBTStatus s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(2, myTestAgent->testVar_0);
	CHECK_EQUAL(1, myTestAgent->testVar_1);

	myTestAgent->testVar_0 = 10;
	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(2, myTestAgent->testVar_0);
	CHECK_EQUAL(2, myTestAgent->testVar_1);

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, s);
	CHECK_EQUAL(2, myTestAgent->testVar_0);
	CHECK_EQUAL(101, myTestAgent->testVar_1);

    finlTestEnvNode(myTestAgent);
}
//< selector node test
LOAD_TEST(btunittest, selector_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(2, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(2, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, selector_ut_4)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_ut_4", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

//< sequence node test
LOAD_TEST(btunittest, sequence_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/sequence_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, sequence_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/sequence_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, sequence_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/sequence_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(2, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, sequence_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/sequence_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

//< if else node test
LOAD_TEST(btunittest, if_else_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/if_else_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, if_else_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/if_else_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(2, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

//< Sequence Stochastic node test
#define STOCHASTIC_SAMPLE_COUNT	9000

void test_stochastic_distribution_0(behaviac::string tree, behaviac::Workspace::EFileFormat format)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    //behaviac::Agent::Register<AgentNodeTest>();
    registerAllTypes();

    AgentNodeTest* myTestAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
    behaviac::Agent::SetIdMask(1);
    myTestAgent->SetIdFlag(1);
    //ms_workspace = new BehaviacWorkspace();
    myTestAgent->btload(tree.c_str());
    myTestAgent->btsetcurrent(tree.c_str());
    myTestAgent->resetProperties();

    int counts[3] = { 0, 0, 0 };
    int loopCount = STOCHASTIC_SAMPLE_COUNT;

    while (loopCount > 0)
    {
        myTestAgent->btexec();
        ++(counts[myTestAgent->testVar_0]);
        --loopCount;
    }

#if BEHAVIAC_CCDEFINE_MSVC

    for (int i = 0; i < 3; ++i)
    {
        int k = counts[i];
        int bias = abs(k - STOCHASTIC_SAMPLE_COUNT / 3);
        CHECK_LESS(bias, (STOCHASTIC_SAMPLE_COUNT / 20));
    }

#endif
    unregisterAllTypes();
    BEHAVIAC_DELETE(myTestAgent);
    //behaviac::Agent::UnRegister<AgentNodeTest>();

    behaviac::Profiler::DestroyInstance();
}

void test_stochastic_distribution_1(behaviac::string tree, behaviac::Workspace::EFileFormat format,
                                    int loopCount = STOCHASTIC_SAMPLE_COUNT, int referenceValue = STOCHASTIC_SAMPLE_COUNT / 3, int checkValue = STOCHASTIC_SAMPLE_COUNT / 30)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    //behaviac::Agent::Register<AgentNodeTest>();
    registerAllTypes();
    AgentNodeTest* myTestAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
    behaviac::Agent::SetIdMask(1);
    myTestAgent->SetIdFlag(1);


    int predicateValueCount = 0;
    myTestAgent->btload(tree.c_str());
    myTestAgent->btsetcurrent(tree.c_str());
    myTestAgent->resetProperties();

    while (loopCount > 0)
    {
        myTestAgent->resetProperties();
        myTestAgent->btexec();

        if (myTestAgent->testVar_0 == 0)
        {
            predicateValueCount++;
        }

        --loopCount;
    }

#if BEHAVIAC_CCDEFINE_MSVC
    int bias = abs(predicateValueCount - referenceValue);
    CHECK_LESS(bias, checkValue);
#endif
    unregisterAllTypes();
    BEHAVIAC_DELETE(myTestAgent);
    //behaviac::Agent::UnRegister<AgentNodeTest>();

    behaviac::Profiler::DestroyInstance();
}
//
LOAD_TEST(btunittest, sequence_stochastic_ut_0)
{
    test_stochastic_distribution_0("node_test/sequence_stochastic_ut_0", format);
}

LOAD_TEST(btunittest, sequence_stochastic_ut_1)
{
    test_stochastic_distribution_1("node_test/sequence_stochastic_ut_1", format);
}

LOAD_TEST(btunittest, sequence_stochastic_ut_2)
{
    test_stochastic_distribution_1("node_test/sequence_stochastic_ut_2", format);
}

LOAD_TEST(btunittest, sequence_stochastic_ut_3)
{
    test_stochastic_distribution_1("node_test/sequence_stochastic_ut_3", format);
}

//< Selector Stochastic Tests
LOAD_TEST(btunittest, selector_stochastic_ut_0)
{
    test_stochastic_distribution_0("node_test/selector_stochastic_ut_0", format);
}

LOAD_TEST(btunittest, selector_stochastic_ut_1)
{
    test_stochastic_distribution_1("node_test/selector_stochastic_ut_1", format);
}

LOAD_TEST(btunittest, selector_stochastic_ut_2)
{
    test_stochastic_distribution_1("node_test/selector_stochastic_ut_2", format, 6000, 3000, 200);
}

//< Selector Probability Tests
void test_stochastic_distribution_2(behaviac::string tree, behaviac::Workspace::EFileFormat format, int refs[3])
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    //behaviac::Agent::Register<AgentNodeTest>();
    registerAllTypes();
    AgentNodeTest* myTestAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
    behaviac::Agent::SetIdMask(1);
    myTestAgent->SetIdFlag(1);
    myTestAgent->btload(tree.c_str());
    myTestAgent->btsetcurrent(tree.c_str());
    myTestAgent->resetProperties();

    int counts[3] = { 0, 0, 0 };
    int loopCount = 10000;

    while (loopCount > 0)
    {
        myTestAgent->btexec();
        ++(counts[myTestAgent->testVar_0]);
        --loopCount;
    }

    for (int i = 0; i < 3; ++i)
    {
        int k = counts[i];
        int bias = abs(k - refs[i]);
        CHECK_LESS(bias, 1000);
    }

    unregisterAllTypes();
    BEHAVIAC_DELETE(myTestAgent);
    //behaviac::Agent::UnRegister<AgentNodeTest>();

    behaviac::Profiler::DestroyInstance();
}

LOAD_TEST(btunittest, selector_probability_ut_0)
{
    int refs[3] = { 2000, 3000, 5000 };
    test_stochastic_distribution_2("node_test/selector_probability_ut_0", format, refs);
}

LOAD_TEST(btunittest, selector_probability_ut_1)
{
    int refs[3] = { 0, 5000, 5000 };
    test_stochastic_distribution_2("node_test/selector_probability_ut_1", format, refs);
}

LOAD_TEST(btunittest, selector_probability_ut_2)
{
	const char* tree = "node_test/selector_probability_ut_2";

	behaviac::Profiler::CreateInstance();
	behaviac::Config::SetSocketing(false);
	behaviac::Config::SetLogging(false);

	//behaviac::Agent::Register<AgentNodeTest>();
	registerAllTypes();
	AgentNodeTest* myTestAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
	behaviac::Agent::SetIdMask(1);
	myTestAgent->SetIdFlag(1);
	myTestAgent->btload(tree);
	myTestAgent->btsetcurrent(tree);
	myTestAgent->resetProperties();

	int loopCount = 10000;

	while (loopCount > 0)
	{
		myTestAgent->btexec();
		CHECK_EQUAL(-1, myTestAgent->testVar_0);
		--loopCount;
	}

	unregisterAllTypes();
	BEHAVIAC_DELETE(myTestAgent);
	//behaviac::Agent::UnRegister<AgentNodeTest>();

	behaviac::Profiler::DestroyInstance();
}


LOAD_TEST(btunittest, selector_probability_ut_3)
{
    const char* tree = "node_test/selector_probability_ut_3";

    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    //behaviac::Agent::Register<AgentNodeTest>();
    registerAllTypes();
    AgentNodeTest* myTestAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
    behaviac::Agent::SetIdMask(1);
    myTestAgent->SetIdFlag(1);
    myTestAgent->btload(tree);
    myTestAgent->btsetcurrent(tree);
    myTestAgent->resetProperties();

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(0);

	for (int i = 0; i < 2; ++i) {
		myTestAgent->btexec();
		if (myTestAgent->testVar_0 != -1) {
			//CHECK_EQUAL(i, myTestAgent->testVar_0);
			CHECK_EQUAL(0, myTestAgent->testVar_0);
			CHECK_EQUAL(-1, myTestAgent->testVar_1);
		}
		else {
			//CHECK_EQUAL(i, myTestAgent->testVar_1);
			CHECK_EQUAL(0, myTestAgent->testVar_1);
			CHECK_EQUAL(-1, myTestAgent->testVar_0);
		}

		behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);
	}

	myTestAgent->btexec();

	CHECK_EQUAL(-1, myTestAgent->testVar_0);
	CHECK_EQUAL(-1, myTestAgent->testVar_1);


    unregisterAllTypes();
    BEHAVIAC_DELETE(myTestAgent);
    //behaviac::Agent::UnRegister<AgentNodeTest>();

    behaviac::Profiler::DestroyInstance();
}

LOAD_TEST(btunittest, selector_probability_ut_4)
{
	const char* tree = "node_test/selector_probability_ut_4";

	behaviac::Profiler::CreateInstance();
	behaviac::Config::SetSocketing(false);
	behaviac::Config::SetLogging(false);

	//behaviac::Agent::Register<AgentNodeTest>();
	registerAllTypes();
	AgentNodeTest* myTestAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
	behaviac::Agent::SetIdMask(1);
	myTestAgent->SetIdFlag(1);
	myTestAgent->btload(tree);
	myTestAgent->btsetcurrent(tree);
	myTestAgent->resetProperties();

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(0);

	for (int i = 0; i < 10; ++i) {
		myTestAgent->btexec();
		if (myTestAgent->testVar_0 != -1) {
			CHECK_EQUAL(0, myTestAgent->testVar_0);
			CHECK_EQUAL(-1, myTestAgent->testVar_1);
			CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
		}
		else {
			CHECK_EQUAL(-1, myTestAgent->testVar_0);
			CHECK_EQUAL(0, myTestAgent->testVar_1);
			CHECK_FLOAT_EQUAL(-1, myTestAgent->testVar_2);
		}

		behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(behaviac::Workspace::GetInstance()->GetDoubleValueSinceStartup() + 0.1 * 1000);
	}

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(behaviac::Workspace::GetInstance()->GetDoubleValueSinceStartup() + 0.1 * 1000);
	myTestAgent->btexec();

	CHECK_EQUAL(-1, myTestAgent->testVar_0);
	CHECK_EQUAL(-1, myTestAgent->testVar_1);


	unregisterAllTypes();
	BEHAVIAC_DELETE(myTestAgent);
	//behaviac::Agent::UnRegister<AgentNodeTest>();

	behaviac::Profiler::DestroyInstance();
}


//< Condition Nodes Tests
LOAD_TEST(btunittest, condition_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/condition_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(2, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, condition_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/condition_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, condition_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/condition_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, condition_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/condition_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(2, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

//< Action Nodes Tests
LOAD_TEST(btunittest, action_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_0", format);
    myTestAgent->resetProperties();

	ChildNodeTestSub* testChildAgent = behaviac::Agent::Create<ChildNodeTestSub>(1, "par_child", 0, 0);
	StaticAgent* pStaticAgent = behaviac::Agent::Create<StaticAgent>("StaticAgent");

	myTestAgent->testVar_3 = 1;
    testChildAgent->testVar_2 = 2;

    myTestAgent->SetVariable<ChildNodeTest*>("par_child", testChildAgent);

    myTestAgent->btexec();

    CHECK_EQUAL(1500, myTestAgent->testVar_0);
    CHECK_EQUAL(1800, myTestAgent->testVar_1);
    CHECK_EQUAL("abcd", myTestAgent->testVar_str_0);
	CHECK_EQUAL(2, StaticAgent::sInt);
    CHECK_EQUAL(true, testChildAgent->m_bTargetValid);

    behaviac::Agent::Destroy(testChildAgent);
    behaviac::Agent::Destroy(pStaticAgent);

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_FLOAT_EQUAL(1.8f, myTestAgent->testVar_2);
    CHECK_FLOAT_EQUAL(4.5f, myTestAgent->testVar_3);

	CHECK_EQUAL("HC", myTestAgent->testVar_str_0);
	CHECK_EQUAL("NODE", myTestAgent->testVar_str_1);
	const TestNS::Float2& float2 = myTestAgent->GetVariable<TestNS::Float2>("testFloat2");
	CHECK_FLOAT_EQUAL(1.0f, float2.x);
	CHECK_FLOAT_EQUAL(1.0f, float2.y);

	const TestNS::Float2& c_ReturnFloat2 = myTestAgent->GetVariable<TestNS::Float2>("c_ReturnFloat2");
	CHECK_FLOAT_EQUAL(2.0f, c_ReturnFloat2.x);
	CHECK_FLOAT_EQUAL(2.0f, c_ReturnFloat2.y);

	const TestNS::Float2& c_ReturnFloat2Const = myTestAgent->GetVariable<TestNS::Float2>("c_ReturnFloat2Const");
	CHECK_FLOAT_EQUAL(2.0f, c_ReturnFloat2Const.x);
	CHECK_FLOAT_EQUAL(2.0f, c_ReturnFloat2Const.y);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(500000, myTestAgent->testVar_0);
    CHECK_EQUAL(1666, myTestAgent->testVar_1);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_FLOAT_EQUAL(2.4f, myTestAgent->testVar_2);
    CHECK_FLOAT_EQUAL(4.0f, myTestAgent->testVar_3);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_ut_3_save_load)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_3", format);
    myTestAgent->resetProperties();

#if BEHAVIAC_CCDEFINE_MSVC || BEHAVIAC_CCDEFINE_GCC_CYGWIN || BEHAVIAC_CCDEFINE_GCC_LINUX
    behaviac::State_t state;
    myTestAgent->btsave(state);
    state.SaveToFile("btsave.xml");
#endif

    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_FLOAT_EQUAL(2.4f, myTestAgent->testVar_2);
    CHECK_FLOAT_EQUAL(4.0f, myTestAgent->testVar_3);

#if BEHAVIAC_CCDEFINE_MSVC || BEHAVIAC_CCDEFINE_GCC_CYGWIN || BEHAVIAC_CCDEFINE_GCC_LINUX
    myTestAgent->resetProperties();

    behaviac::State_t stateTemp;
    stateTemp.LoadFromFile("btsave.xml");

    myTestAgent->btload(state);

    myTestAgent->btexec();
    CHECK_FLOAT_EQUAL(2.4f, myTestAgent->testVar_2);
    CHECK_FLOAT_EQUAL(4.0f, myTestAgent->testVar_3);
#endif

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_child_agent)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_child_agent_0", format);

    myTestAgent->resetProperties();

    myTestAgent->initChildAgentTest();

    myTestAgent->btexec();

	const ChildNodeTest* ct = myTestAgent->GetVariable<ChildNodeTest*>("par_child_agent_1");
    CHECK_EQUAL(666, ct->testVar_0);
    CHECK_EQUAL(888, ct->testVar_1);
    CHECK_FLOAT_EQUAL(999, ct->testVar_2);

    finlTestEnvNode(myTestAgent);
}

//< Wait For Signal Tests
LOAD_TEST(btunittest, action_ut_waitforsignal_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_waitforsignal_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);
    myTestAgent->resetProperties();
    myTestAgent->testVar_0 = 0;
    myTestAgent->btexec();
    CHECK_EQUAL(1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(2.3f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_ut_waitforsignal_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_waitforsignal_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);
    myTestAgent->resetProperties();
    myTestAgent->testVar_2 = 0.0f;
    myTestAgent->btexec();
    CHECK_EQUAL(1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(2.3f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_ut_waitforsignal_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_waitforsignal_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    myTestAgent->resetProperties();
    myTestAgent->testVar_2 = 0.0f;
    status = myTestAgent->btexec();
    CHECK_FLOAT_EQUAL(2.3f, myTestAgent->testVar_2);
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    finlTestEnvNode(myTestAgent);
}

#if BEHAVIAC_CCDEFINE_MSVC || BEHAVIAC_CCDEFINE_GCC_CYGWIN || BEHAVIAC_CCDEFINE_GCC_LINUX
LOAD_TEST(btunittest, action_ut_waitforsignal_0_saveload)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_waitforsignal_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);

    //myTestAgent->SetVariable("testVar_1", -1);
    //myTestAgent->SetVariable("testVar_2", -1.0f);

    behaviac::State_t state;
    myTestAgent->btsave(state);
    state.SaveToFile("btsave2.xml");
    myTestAgent->SaveDataToFile("agent_state.xml");

    //modify the members
    myTestAgent->testVar_1 = 1;
    myTestAgent->testVar_2 = 1;

    behaviac::State_t stateTemp;
    stateTemp.LoadFromFile("btsave2.xml");
    myTestAgent->btload(stateTemp);

    myTestAgent->LoadDataFromFile("agent_state.xml");
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, action_ut_waitforsignal_0_saveload_agent)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_ut_waitforsignal_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);

    behaviac::State_t state;
    myTestAgent->btsave(state);
    state.SaveToFile("btsave3.xml", myTestAgent);

    //modify the members
    myTestAgent->testVar_1 = 1;
    myTestAgent->testVar_2 = 1;

    behaviac::State_t stateTemp;
    stateTemp.LoadFromFile("btsave3.xml", myTestAgent);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);

    myTestAgent->btload(stateTemp);

    finlTestEnvNode(myTestAgent);
}

#endif

//< Wait For Frames Tests
LOAD_TEST(btunittest, action_waitframes_ut_0)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    //behaviac::Agent::Register<AgentNodeTest>();
    registerAllTypes();
    AgentNodeTest* myTestAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
    behaviac::Agent::SetIdMask(1);
    myTestAgent->SetIdFlag(1);
    myTestAgent->btload("node_test/action_waitframes_ut_0");
    myTestAgent->btsetcurrent("node_test/action_waitframes_ut_0");
    myTestAgent->resetProperties();

    int loopCount = 0;

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(0);

    while (loopCount < 5)
    {
        myTestAgent->btexec();

        if (loopCount < 4)
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }
        else
        {
            CHECK_EQUAL(2, myTestAgent->testVar_0);
        }

        ++loopCount;
		behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);
    }

    myTestAgent->resetProperties();
    myTestAgent->btexec();
    CHECK_EQUAL(1, myTestAgent->testVar_0);

    BEHAVIAC_DELETE(myTestAgent);

    behaviac::Profiler::DestroyInstance();

    unregisterAllTypes();
}

//< Noop Node Test
LOAD_TEST(btunittest, action_noop_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/action_noop_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(2, myTestAgent->testVar_0);
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    finlTestEnvNode(myTestAgent);
}

//< Reference Node test
LOAD_TEST(btunittest, reference_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/reference_ut_0", format);
    myTestAgent->resetProperties();

    myTestAgent->btexec();
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    CHECK_FLOAT_EQUAL(1.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, reference_ut_1)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/reference_ut_1", format);
	myTestAgent->resetProperties();

	myTestAgent->btexec();
	CHECK_EQUAL(0, myTestAgent->testVar_0);
	CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, reference_ut_2)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/reference_ut_2", format);
	myTestAgent->resetProperties();

	myTestAgent->btexec();
	CHECK_EQUAL(0, myTestAgent->testVar_0);
	CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, par_test_custom_property_reset)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("par_test/custom_property_reset", format);
	myTestAgent->resetProperties();
	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(10, myTestAgent->testVar_1);

	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(20, myTestAgent->testVar_1);
	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, node_test_selector_ut_5)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/selector_ut_5", format);

	myTestAgent->resetProperties();

	myTestAgent->testColor = EnumTest_One;

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(0, myTestAgent->testVar_0);

	myTestAgent->testColor = EnumTest_OneAfterOne;
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, wait_ut_0)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/wait_ut_0", format);
	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(0);
	myTestAgent->resetProperties();
	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(1.001 * 1000);
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(2, myTestAgent->testVar_0);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, wait_ut_1)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/wait_ut_1", format);

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(0);
	myTestAgent->resetProperties();
	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(1.001 * 1000);
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(2, myTestAgent->testVar_0);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, wait_ut_2)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/wait_ut_2", format);

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(0);
	myTestAgent->resetProperties();
	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	for (int i = 0; i < 10; ++i) {
		double time = (i + 1);
		behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(time);
		status = myTestAgent->btexec();
		CHECK_EQUAL(behaviac::BT_RUNNING, status);
	}
	
	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(1.001 * 1000);
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(1.100 * 1000);
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_FAILURE, status);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, end_ut_0)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/end_ut_0", format);
	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, end_ut_1)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/end_ut_1", format);
	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_FAILURE, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, end_ut_2)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/end_ut_2", format);
	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_FAILURE, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, end_ut_3)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/end_ut_3", format);
	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(2, myTestAgent->testVar_1);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, end_ut_4)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/end_ut_4", format);
	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_FAILURE, status);
	CHECK_EQUAL(1, myTestAgent->testVar_1);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, end_ut_5)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/end_ut_5", format);
	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_FAILURE, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);
	CHECK_EQUAL(1, myTestAgent->testVar_1);

	finlTestEnvNode(myTestAgent);
}
