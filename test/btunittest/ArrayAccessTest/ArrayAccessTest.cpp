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

TestNS::AgentArrayAccessTest* initTestEnvArray(const char* treePath, behaviac::Workspace::EFileFormat format)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    registerAllTypes();
    TestNS::AgentArrayAccessTest* testAgent = TestNS::AgentArrayAccessTest::DynamicCast(behaviac::Agent::Create<TestNS::AgentArrayAccessTest>());
    behaviac::Agent::SetIdMask(1);
    testAgent->SetIdFlag(1);
    testAgent->btload(treePath);
    testAgent->btsetcurrent(treePath);
    return testAgent;
}

void finlTestEnvArray(TestNS::AgentArrayAccessTest* testAgent)
{
    BEHAVIAC_DELETE(testAgent);
    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

LOAD_TEST(btunittest, vector_test)
{
    TestNS::AgentArrayAccessTest* testAgent = initTestEnvArray("par_test/vector_test", format);
    testAgent->resetProperties();
    testAgent->btexec();

    int Int1 = testAgent->GetVariable<int>("Int");
    CHECK_EQUAL(1, Int1);

    int Int2 = testAgent->Int;
    CHECK_EQUAL(1, Int2);

    int c_Int = testAgent->GetVariable<int>("c_Int");
    CHECK_EQUAL(10, c_Int);

    int Int0 = testAgent->ListInts[0];
    CHECK_EQUAL(110, Int0);

    int c_Count = testAgent->GetVariable<int>("c_Count");
    CHECK_EQUAL(5, c_Count);

	behaviac::vector<double> c_douleVec = testAgent->GetVariable<behaviac::vector<double> >("c_douleVec");
	CHECK_EQUAL(103, c_douleVec.size());
	for (int i = 0; i < 100; ++i)
	{
		CHECK_FLOAT_EQUAL(c_douleVec[3 + i], 0.03);
	}

	behaviac::vector<double> c_douleVec2 = testAgent->GetVariable<behaviac::vector<double> >("c_doubleVec2");
	CHECK_EQUAL(103, c_douleVec2.size());
	for (int i = 0; i < 100; ++i)
	{
		CHECK_FLOAT_EQUAL(c_douleVec2[3 + i], 0.05);
	}

    finlTestEnvArray(testAgent);
}
