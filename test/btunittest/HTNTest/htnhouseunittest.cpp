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

#if BEHAVIAC_USE_HTN

static HTNAgentHouse* initTestEnvHTNHouse(const char* treePath, behaviac::Workspace::EFileFormat format)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);


    registerAllTypes();
    HTNAgentHouse* testAgent = HTNAgentHouse::DynamicCast(behaviac::Agent::Create<HTNAgentHouse>());
    behaviac::Agent::SetIdMask(1);
    testAgent->SetIdFlag(1);

    testAgent->btload(treePath);
    testAgent->btsetcurrent(treePath);
    return testAgent;
}


static void finlTestEnvHTNHouse(HTNAgentHouse* testAgent)
{
    BEHAVIAC_DELETE(testAgent);
    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

/**
unit test for effector
*/
LOAD_TEST(btunittest, test_build_house)
{
    HTNAgentHouse* testAgent = initTestEnvHTNHouse("node_test/htn/house/root", format);
    testAgent->resetProperties();

    testAgent->SetVariable("Money", 200);

    testAgent->btexec();

    int money = testAgent->GetVariable<int>("Money");
    CHECK_EQUAL(100, money);

    bool land = testAgent->GetVariable<bool>("Land");
    CHECK_EQUAL(true, land);

    bool goodCredit = testAgent->GetVariable<bool>("GoodCredit");
    CHECK_EQUAL(true, goodCredit);

    bool mortgage = testAgent->GetVariable<bool>("Mortgage");
    CHECK_EQUAL(true, mortgage);

    bool permit = testAgent->GetVariable<bool>("Permit");
    CHECK_EQUAL(true, permit);

    bool contract = testAgent->GetVariable<bool>("Contract");
    CHECK_EQUAL(false, contract);

    bool house = testAgent->GetVariable<bool>("House");
    CHECK_EQUAL(true, house);

    finlTestEnvHTNHouse(testAgent);
}

#endif