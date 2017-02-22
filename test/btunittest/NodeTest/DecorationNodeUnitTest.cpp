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

LOAD_TEST(btunittest, save_meta_file)
{
    registerAllTypes();

    unregisterAllTypes();
}

//< Decoration Loop Tests
LOAD_TEST(btunittest, decoration_loop_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_loop_ut_0", format);
    int loopCount = 1000;

    while (loopCount > 0)
    {
        myTestAgent->resetProperties();

		behaviac::EBTStatus status = myTestAgent->btexec();

		CHECK_EQUAL(behaviac::BT_RUNNING, status);
		CHECK_EQUAL(0, myTestAgent->testVar_0);

		behaviac::BehaviorTreeTask* btTask = myTestAgent->btgetcurrent();
		CHECK_EQUAL(true, btTask != NULL);

		behaviac::vector<behaviac::BehaviorTask*> nodes = btTask->GetRunningNodes(false);
		CHECK_EQUAL(3, nodes.size());

		behaviac::vector<behaviac::BehaviorTask*> leaves = btTask->GetRunningNodes(true);
		CHECK_EQUAL(0, leaves.size());

        --loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_loop_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_loop_ut_1", format);
    int loopCount = 0;

    while (loopCount < 500)
    {
        myTestAgent->resetProperties();
        myTestAgent->btexec();

        if (loopCount < 499)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

//< Decoration Not Tests
LOAD_TEST(btunittest, decoration_not_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_not_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_not_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_not_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_not_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_not_ut_2", format);
    int loopCount = 0;

    while (loopCount < 500)
    {
        myTestAgent->resetProperties();
        myTestAgent->btexec();
        CHECK_EQUAL(0, myTestAgent->testVar_0);
        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

//< Decoration Frames Tests
LOAD_TEST(btunittest, decoration_frames_ut_0)
{
	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(0);

    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_frames_ut_0", format);
    int loopCount = 0;

    while (loopCount < 100)
    {
        myTestAgent->resetProperties();
        myTestAgent->btexec();

        if (loopCount < 99)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);
        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
		behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);
    }

    finlTestEnvNode(myTestAgent);
}

void once_(AgentNodeTest* myTestAgent)
{
	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(0);

	behaviac::EBTStatus s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(0, myTestAgent->testVar_0);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(0, myTestAgent->testVar_0);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

	myTestAgent->testVar_0 = 1;

	//Move ends, testVar_0 = 2
	//Frames(5) is still running
	//testVar_0 = 0 again
	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(0, myTestAgent->testVar_0);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, s);
	CHECK_EQUAL(0, myTestAgent->testVar_0);

	behaviac::Workspace::GetInstance()->SetFrameSinceStartup(behaviac::Workspace::GetInstance()->GetFrameSinceStartup() + 1);

	s = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, s);
	CHECK_EQUAL(3, myTestAgent->testVar_0);
}

LOAD_TEST(btunittest, frames_ut_0)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/frames_ut_0", format);
	myTestAgent->resetProperties();

	once_(myTestAgent);

	//rerun again
	once_(myTestAgent);

	finlTestEnvNode(myTestAgent);
}

//LOAD_TEST(btunittest, decoration_frames_ut_0)
//{
//	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_frames_ut_0", format);
//int loopCount = 0;
//behaviac::Workspace::SetDeltaFrames(5);
//while (loopCount < 20)
//{
//	myTestAgent->resetProperties();
//	myTestAgent->btexec();
//	if (loopCount < 19)
//	{
//		CHECK_EQUAL(0, myTestAgent->testVar_0);
//	}
//	else
//	{
//		CHECK_EQUAL(1, myTestAgent->testVar_0);
//	}
//	++loopCount;
//}
//behaviac::Workspace::SetDeltaFrames(1);
//finlTestEnvNode(myTestAgent);
//}

//< Decoration Loop Until Tests
LOAD_TEST(btunittest, decoration_loopuntil_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_loopuntil_ut_0", format);
    int loopCount = 0;
    myTestAgent->resetProperties();

    while (loopCount < 100)
    {
        myTestAgent->btexec();

        if (loopCount < 99)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_loopuntil_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_loopuntil_ut_1", format);
    int loopCount = 0;
    myTestAgent->resetProperties();

    while (loopCount < 50)
    {
        myTestAgent->btexec();

        if (loopCount < 49)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_loopuntil_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_loopuntil_ut_2", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 50)
    {
        myTestAgent->btexec();

        if (loopCount < 49)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_repeat_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/repeat/repeat_ut_0", format);
    myTestAgent->resetProperties();

    myTestAgent->btexec();

    CHECK_EQUAL(5, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_repeat_1)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/repeat/repeat_ut_1", format);
	myTestAgent->resetProperties();

	myTestAgent->btexec();

	CHECK_EQUAL(5, myTestAgent->testVar_0);
	finlTestEnvNode(myTestAgent);
}

//< Decoration Count Limit Tests
LOAD_TEST(btunittest, decoration_countlimit_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_countlimit_ut_0", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 6)
    {
        myTestAgent->btexec();

        if (loopCount < 5)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_countlimit_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_countlimit_ut_1", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 11)
    {
        if (loopCount == 5)
        {
            myTestAgent->testVar_1 = 0;
        }

        myTestAgent->btexec();
        myTestAgent->testVar_1 = -1;

        if (loopCount < 10)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_countlimit_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_countlimit_ut_2", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 11)
    {
        if (loopCount == 5)
        {
            myTestAgent->testVar_1 = 0;
            myTestAgent->testVar_2 = 0.0f;
        }

        myTestAgent->btexec();
        myTestAgent->testVar_1 = -1;

        if (loopCount < 10)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_countlimit_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_countlimit_ut_3", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 11)
    {
        if (loopCount == 5)
        {
            myTestAgent->testVar_3 = 0.0f;
        }

        myTestAgent->btexec();
        myTestAgent->testVar_3 = -1.0f;

        if (loopCount < 10)
        {
            CHECK_EQUAL(0, myTestAgent->testVar_0);

        }
        else
        {
            CHECK_EQUAL(1, myTestAgent->testVar_0);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

//< Decoration Failure Until Tests
LOAD_TEST(btunittest, decoration_failureuntil_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_failureuntil_ut_0", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 1000)
    {
        behaviac::EBTStatus status = myTestAgent->btexec();
        CHECK_EQUAL(behaviac::BT_FAILURE, status);
        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_failureuntil_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_failureuntil_ut_1", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 5)
    {
        behaviac::EBTStatus status = myTestAgent->btexec();

        if (loopCount < 4)
        {
            CHECK_EQUAL(behaviac::BT_FAILURE, status);
        }
        else
        {
            CHECK_EQUAL(behaviac::BT_SUCCESS, status);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

//< Decoration Success Until Tests
LOAD_TEST(btunittest, decoration_successuntil_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_successuntil_ut_0", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 1000)
    {
        behaviac::EBTStatus status = myTestAgent->btexec();
        CHECK_EQUAL(behaviac::BT_SUCCESS, status);
        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, decoration_successuntil_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_successuntil_ut_1", format);
    myTestAgent->resetProperties();
    int loopCount = 0;

    while (loopCount < 5)
    {
        behaviac::EBTStatus status = myTestAgent->btexec();

        if (loopCount < 4)
        {
            CHECK_EQUAL(behaviac::BT_SUCCESS, status);

        }
        else
        {
            CHECK_EQUAL(behaviac::BT_FAILURE, status);
        }

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

//< Decoration Always Failure Tests
LOAD_TEST(btunittest, decoration_alwaysfailure_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_alwaysfailure_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(1, myTestAgent->testVar_0);
    CHECK_EQUAL(1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(1.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

//< Decoration Always Success Tests
LOAD_TEST(btunittest, decoration_alwayssuccess_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_alwayssuccess_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    BEHAVIAC_UNUSED_VAR(status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

//< Decoration Always Running Tests
LOAD_TEST(btunittest, decoration_alwaysrunning_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_alwaysrunning_ut_0", format);
    int loopCount = 0;

    while (loopCount < 1000)
    {
        myTestAgent->resetProperties();
        behaviac::EBTStatus status = myTestAgent->btexec();
        BEHAVIAC_UNUSED_VAR(status);
        CHECK_EQUAL(0, myTestAgent->testVar_0);
        CHECK_EQUAL(0, myTestAgent->testVar_1);
        CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);

        ++loopCount;
    }

    finlTestEnvNode(myTestAgent);
}

//< Decoration Log Tests
LOAD_TEST(btunittest, decoration_log_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/decoration_log_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    finlTestEnvNode(myTestAgent);
}
