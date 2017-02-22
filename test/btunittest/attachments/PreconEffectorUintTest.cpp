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
PreconEffectorAgent* initTestEnvPreEff(const char* treePath, behaviac::Workspace::EFileFormat format)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);


    registerAllTypes();
    PreconEffectorAgent* testAgent = PreconEffectorAgent::DynamicCast(behaviac::Agent::Create<PreconEffectorAgent>());
    behaviac::Agent::SetIdMask(1);
    testAgent->SetIdFlag(1);

    testAgent->btload(treePath);
    testAgent->btsetcurrent(treePath);
    return testAgent;
}
void finlTestEnvPreEff(PreconEffectorAgent* testAgent)
{
    BEHAVIAC_DELETE(testAgent);
    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

LOAD_TEST(btunittest, preconditioneffectortest_0)
{
    PreconEffectorAgent* testAgent = initTestEnvPreEff("node_test/PreconditionEffectorTest/PreconditionEffectorTest_0", format);
    testAgent->resetProperties();
    testAgent->count_both = 1;
    testAgent->btexec();

    //precondition failed
    CHECK_EQUAL(0, testAgent->get_count_success());
    CHECK_EQUAL(0, testAgent->count_failure);
    CHECK_EQUAL(1, testAgent->count_both);
    finlTestEnvPreEff(testAgent);
}

LOAD_TEST(btunittest, test_precondition_alive)
{
    PreconEffectorAgent* testAgent = initTestEnvPreEff("node_test/PreconditionEffectorTest/PreconditionEffectorTest_1", format);

    testAgent->resetProperties();
    behaviac::EBTStatus status = behaviac::BT_INVALID;

    for (int i = 0; i < 10; ++i)
    {
        status = testAgent->btexec();
        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        CHECK_EQUAL(0, testAgent->get_count_success());
        CHECK_EQUAL(0, testAgent->count_failure);
        CHECK_EQUAL(0, testAgent->count_both);
    }

    testAgent->count_both = 1;
    status = testAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);

    CHECK_EQUAL(0, testAgent->get_count_success());
    CHECK_EQUAL(1, testAgent->count_failure);
    CHECK_EQUAL(2, testAgent->count_both);
    CHECK_EQUAL(5, testAgent->ret);

    finlTestEnvPreEff(testAgent);
}

LOAD_TEST(btunittest, test_effector)
{
    PreconEffectorAgent* testAgent = initTestEnvPreEff("node_test/PreconditionEffectorTest/PreconditionEffectorTest_0", format);
    testAgent->resetProperties();
    testAgent->btexec();

    //success/failure/both effectors
    CHECK_EQUAL(1, testAgent->get_count_success());
    CHECK_EQUAL(1, testAgent->count_failure);
    CHECK_EQUAL(2, testAgent->count_both);
    finlTestEnvPreEff(testAgent);
}


LOAD_TEST(btunittest, preconditioneffectortest_2)
{
	PreconEffectorAgent* testAgent = initTestEnvPreEff("node_test/PreconditionEffectorTest/PreconditionEffectorTest_2", format);
	testAgent->resetProperties();

	testAgent->count_both = 1;
	testAgent->btexec();
	CHECK_EQUAL(2, testAgent->count_success);
	CHECK_EQUAL(0, testAgent->count_failure);
	CHECK_EQUAL(2, testAgent->ret);

	testAgent->count_both = 0;
	testAgent->btexec();
	CHECK_EQUAL(4, testAgent->count_success);
	CHECK_EQUAL(0, testAgent->count_failure);
	CHECK_EQUAL(2, testAgent->ret);

	testAgent->count_both = 1;
	testAgent->btexec();
	CHECK_EQUAL(6, testAgent->count_success);
	CHECK_EQUAL(0, testAgent->count_failure);
	CHECK_EQUAL(4, testAgent->ret);

	testAgent->count_both = 0;
	testAgent->btexec();
	CHECK_EQUAL(8, testAgent->count_success);
	CHECK_EQUAL(0, testAgent->count_failure);
	CHECK_EQUAL(4, testAgent->ret);

	finlTestEnvPreEff(testAgent);
}

LOAD_TEST(btunittest, preconditioneffectortest_3)
{
	PreconEffectorAgent* testAgent = initTestEnvPreEff("node_test/PreconditionEffectorTest/PreconditionEffectorTest_3", format);
	testAgent->resetProperties();

	testAgent->count_both = 1;
	testAgent->btexec();
	CHECK_EQUAL(0, testAgent->count_success);
	CHECK_EQUAL(0, testAgent->count_failure);
	CHECK_EQUAL(0, testAgent->ret);

	testAgent->count_both = 0;
	testAgent->btexec();

	CHECK_EQUAL(1, testAgent->count_success);
	CHECK_EQUAL(1, testAgent->count_failure);
	CHECK_EQUAL(2, testAgent->ret);

	testAgent->count_both = 1;
	testAgent->btexec();
	CHECK_EQUAL(1, testAgent->count_success);
	CHECK_EQUAL(2, testAgent->count_failure);
	CHECK_EQUAL(3, testAgent->ret);

	testAgent->count_both = 0;
	testAgent->btexec();

	CHECK_EQUAL(2, testAgent->count_success);
	CHECK_EQUAL(3, testAgent->count_failure);
	CHECK_EQUAL(5, testAgent->ret);

	finlTestEnvPreEff(testAgent);
}

LOAD_TEST(btunittest, preconditioneffectortest_4)
{
	PreconEffectorAgent* testAgent = initTestEnvPreEff("node_test/PreconditionEffectorTest/PreconditionEffectorTest_3", format);
	testAgent->resetProperties();

	testAgent->count_both = 1;
	testAgent->btexec();
	CHECK_EQUAL(0, testAgent->count_success);
	CHECK_EQUAL(0, testAgent->count_failure);
	CHECK_EQUAL(0, testAgent->ret);

	testAgent->count_both = 0;
	testAgent->btexec();

	CHECK_EQUAL(1, testAgent->count_success);
	CHECK_EQUAL(1, testAgent->count_failure);
	CHECK_EQUAL(2, testAgent->ret);

	testAgent->btsetcurrent("node_test/PreconditionEffectorTest/PreconditionEffectorTest_0");
	CHECK_EQUAL(1, testAgent->count_success);
	CHECK_EQUAL(2, testAgent->count_failure);
	CHECK_EQUAL(3, testAgent->ret);

	testAgent->btexec();

	CHECK_EQUAL(1, testAgent->count_success);
	CHECK_EQUAL(2, testAgent->count_failure);
	CHECK_EQUAL(3, testAgent->ret);


	finlTestEnvPreEff(testAgent);
}
