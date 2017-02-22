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
FSMAgentTest* initTestEnvFSM(const char* treePath, behaviac::Workspace::EFileFormat format)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);


    registerAllTypes();
    FSMAgentTest* testAgent = FSMAgentTest::DynamicCast(behaviac::Agent::Create<FSMAgentTest>());
    behaviac::Agent::SetIdMask(1);
    testAgent->SetIdFlag(1);

    testAgent->btload(treePath);
    testAgent->btsetcurrent(treePath);
    return testAgent;
}

void finlTestEnvFSM(FSMAgentTest* testAgent)
{
    BEHAVIAC_DELETE(testAgent);
    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

/**
unit test for effector
*/
LOAD_TEST(btunittest, fsm_ut_1)
{
	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(0);

    FSMAgentTest* testAgent = initTestEnvFSM("node_test/fsm/fsm_ut_1", format);
    testAgent->resetProperties();

    behaviac::EBTStatus status = behaviac::BT_INVALID;

    testAgent->Message = FSMAgentTest::Invalid;
    status = testAgent->btexec();
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    int InactiveCount = testAgent->GetVariable<int>("InactiveCount");
    CHECK_EQUAL(0, InactiveCount);
    CHECK_EQUAL(0, testAgent->TestVar);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

    //switch to Active
    testAgent->Message = FSMAgentTest::Begin;
    status = testAgent->btexec();
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    InactiveCount = testAgent->GetVariable<int>("InactiveCount");
    CHECK_EQUAL(1, InactiveCount);
    uint32_t ActiveCount = testAgent->GetVariable<uint32_t>("ActiveCount");
    CHECK_EQUAL(0, ActiveCount);
    CHECK_EQUAL(2, testAgent->TestVar);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

    //switch to Pause
    testAgent->Message = FSMAgentTest::Pause;
    status = testAgent->btexec();
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    ActiveCount = testAgent->GetVariable<uint32_t>("ActiveCount");
    CHECK_EQUAL(1, ActiveCount);
    short PauseCount = testAgent->GetVariable<short>("PauseCount");
    CHECK_EQUAL(0, PauseCount);
    CHECK_EQUAL(4, testAgent->TestVar);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

    //switch to Inactive
    testAgent->Message = FSMAgentTest::End;
    status = testAgent->btexec();
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    PauseCount = testAgent->GetVariable<short>("PauseCount");
    CHECK_EQUAL(1, PauseCount);
    long ExitCount = testAgent->GetVariable<long>("ExitCount");
    CHECK_EQUAL(0, ExitCount);
    CHECK_EQUAL(6, testAgent->TestVar);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

    //switch to Exit
    testAgent->Message = FSMAgentTest::Exit;
    status = testAgent->btexec();

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

	CHECK_EQUAL(behaviac::BT_RUNNING, status);
    status = testAgent->btexec();

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

	CHECK_EQUAL(behaviac::BT_RUNNING, status);
    status = testAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);

    InactiveCount = testAgent->GetVariable<int>("InactiveCount");
    CHECK_EQUAL(1, InactiveCount);
    ExitCount = testAgent->GetVariable<long>("ExitCount");
    CHECK_EQUAL(1, ExitCount);
    CHECK_EQUAL(7, testAgent->TestVar);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

	//reenter again
	testAgent->Message = FSMAgentTest::Invalid;
	status = testAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	InactiveCount = testAgent->GetVariable<int>("InactiveCount");
	CHECK_EQUAL(0, InactiveCount);
	CHECK_EQUAL(8, testAgent->TestVar);

    finlTestEnvFSM(testAgent);
}

LOAD_TEST(btunittest, fsm_ref_bt)
{
    AgentNodeTest* testBtAgent = initTestEnvNode("node_test/fsm/fsm_ref_bt_ut", format);

    testBtAgent->resetProperties();

    behaviac::EBTStatus status = behaviac::BT_INVALID;
    CHECK_EQUAL(-1, testBtAgent->testVar_0);

    status = testBtAgent->btexec();
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    CHECK_EQUAL(2, testBtAgent->testVar_0);

    CHECK_FLOAT_EQUAL(1.8f, testBtAgent->testVar_2);
    CHECK_FLOAT_EQUAL(4.5f, testBtAgent->testVar_3);
    CHECK_EQUAL(true, "HC" == testBtAgent->testVar_str_0);

    status = testBtAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
    CHECK_EQUAL(4, testBtAgent->testVar_0);

    CHECK_FLOAT_EQUAL(1.8f, testBtAgent->testVar_2);
    CHECK_FLOAT_EQUAL(4.5f, testBtAgent->testVar_3);
    CHECK_EQUAL(true, "HC" == testBtAgent->testVar_str_0);

    finlTestEnvNode(testBtAgent);
}

LOAD_TEST(btunittest, fsm_ref_fsm_ut)
{
    FSMAgentTest* testAgent = initTestEnvFSM("node_test/fsm/fsm_ref_fsm_ut", format);
    testAgent->resetProperties();

    behaviac::EBTStatus status = behaviac::BT_INVALID;

    {
        testAgent->Message = FSMAgentTest::Invalid;
        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        int InactiveCount = testAgent->GetVariable<int>("InactiveCount");
        CHECK_EQUAL(0, InactiveCount);
        CHECK_EQUAL(0, testAgent->TestVar);
    }

    {
        //switch to Active
        testAgent->Message = FSMAgentTest::Begin;
        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        int InactiveCount = testAgent->GetVariable<int>("InactiveCount");
        CHECK_EQUAL(1, InactiveCount);
        unsigned int ActiveCount = testAgent->GetVariable<unsigned int>("ActiveCount");
        CHECK_EQUAL(0, ActiveCount);
        CHECK_EQUAL(2, testAgent->TestVar);

        int FoodCount = testAgent->GetVariable<int>("FoodCount");
        CHECK_EQUAL(1, FoodCount);

        for (int i = 1; i < 9; ++i)
        {
            status = testAgent->btexec();
            CHECK_EQUAL(behaviac::BT_RUNNING, status);

            FoodCount = testAgent->GetVariable<int>("FoodCount");
            CHECK_EQUAL((i + 1), FoodCount);
        }

        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        FoodCount = testAgent->GetVariable<int>("FoodCount");
        CHECK_EQUAL(8, FoodCount);

        int EnergyCount = testAgent->GetVariable<int>("EnergyCount");
        CHECK_EQUAL(1, EnergyCount);

    }

    {
        //switch to Pause
        testAgent->Message = FSMAgentTest::Pause;
        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        unsigned int ActiveCount = testAgent->GetVariable<unsigned int>("ActiveCount");
        CHECK_EQUAL(1, ActiveCount);
        short PauseCount = testAgent->GetVariable<short>("PauseCount");
        CHECK_EQUAL(0, PauseCount);
        CHECK_EQUAL(14, testAgent->TestVar);
    }

    {
        //switch to Inactive
        testAgent->Message = FSMAgentTest::End;
        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        short PauseCount = testAgent->GetVariable<short>("PauseCount");
        CHECK_EQUAL(1, PauseCount);
        long ExitCount = testAgent->GetVariable<long>("ExitCount");
        CHECK_EQUAL(0, ExitCount);
        CHECK_EQUAL(16, testAgent->TestVar);
    }

    {
        //switch to Exit
        testAgent->Message = FSMAgentTest::Exit;
        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_SUCCESS, status);
        int InactiveCount = testAgent->GetVariable<int>("InactiveCount");
        CHECK_EQUAL(1, InactiveCount);
        long ExitCount = testAgent->GetVariable<long>("ExitCount");
        CHECK_EQUAL(1, ExitCount);
        CHECK_EQUAL(17, testAgent->TestVar);
    }

    finlTestEnvFSM(testAgent);
}

LOAD_TEST(btunittest, bt_ref_fsm)
{
    FSMAgentTest* testAgent = initTestEnvFSM("node_test/fsm/bt_ref_fsm", format);
    testAgent->resetProperties();

    behaviac::EBTStatus status = behaviac::BT_INVALID;

    {
        //switch to Active
        testAgent->Message = FSMAgentTest::Begin;
        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);

        int FoodCount = testAgent->GetVariable<int>("FoodCount");
        CHECK_EQUAL(1, FoodCount);

        for (int i = 1; i < 9; ++i)
        {
            status = testAgent->btexec();
            CHECK_EQUAL(behaviac::BT_RUNNING, status);

            FoodCount = testAgent->GetVariable<int>("FoodCount");
            CHECK_EQUAL((i + 1), FoodCount);
        }

        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        FoodCount = testAgent->GetVariable<int>("FoodCount");
        CHECK_EQUAL(8, FoodCount);

        int EnergyCount = testAgent->GetVariable<int>("EnergyCount");
        CHECK_EQUAL(1, EnergyCount);
    }

    finlTestEnvFSM(testAgent);
}
