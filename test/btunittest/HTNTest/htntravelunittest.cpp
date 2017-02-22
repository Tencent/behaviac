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
static HTNAgentTravel* initTestEnvHTNTravel(const char* treePath, behaviac::Workspace::EFileFormat format)
{
	behaviac::Profiler::CreateInstance();
	behaviac::Config::SetSocketing(false);
	//behaviac::Config::SetLogging(false);

	registerAllTypes();
	HTNAgentTravel* testAgent = HTNAgentTravel::DynamicCast(behaviac::Agent::Create<HTNAgentTravel>());
	behaviac::Agent::SetIdMask(1);
	testAgent->SetIdFlag(1);

	testAgent->btload(treePath);
	testAgent->btsetcurrent(treePath);
	return testAgent;
}

static void finlTestEnvHTNTravel(HTNAgentTravel* testAgent)
{
	BEHAVIAC_DELETE(testAgent);
	unregisterAllTypes();

	behaviac::Profiler::DestroyInstance();
}

/**
unit test for htn travel
*/
LOAD_TEST(btunittest, test_build_travel)
{
	behaviac::Config::SetLogging(true);

	HTNAgentTravel* testAgent = initTestEnvHTNTravel("node_test/htn/travel/root", format);
	testAgent->resetProperties();

	behaviac::Workspace::GetInstance()->DebugUpdate();
	behaviac::Workspace::GetInstance()->DebugUpdate();

	testAgent->SetStartFinish(testAgent->sh_td, testAgent->sz_td);
	testAgent->btexec();
	behaviac::Config::SetLogging(false);

	CHECK_EQUAL(3, testAgent->Path().size());
	CHECK_EQUAL("ride_taxi", testAgent->Path()[0].name);
	CHECK_EQUAL(testAgent->sh_td, testAgent->Path()[0].x);
	CHECK_EQUAL(testAgent->airport_sh_hongqiao, testAgent->Path()[0].y);

	CHECK_EQUAL("fly", testAgent->Path()[1].name);
	CHECK_EQUAL(testAgent->airport_sh_hongqiao, testAgent->Path()[1].x);
	CHECK_EQUAL(testAgent->airport_sz_baoan, testAgent->Path()[1].y);

	CHECK_EQUAL("ride_taxi", testAgent->Path()[2].name);
	CHECK_EQUAL(testAgent->airport_sz_baoan, testAgent->Path()[2].x);
	CHECK_EQUAL(testAgent->sz_td, testAgent->Path()[2].y);

	//
	testAgent->resetProperties();
	testAgent->SetStartFinish(testAgent->sh_td, testAgent->sh_home);
	testAgent->btexec();

	CHECK_EQUAL(1, testAgent->Path().size());
	CHECK_EQUAL("ride_taxi", testAgent->Path()[0].name);
	CHECK_EQUAL(testAgent->sh_td, testAgent->Path()[0].x);
	CHECK_EQUAL(testAgent->sh_home, testAgent->Path()[0].y);

	finlTestEnvHTNTravel(testAgent);
}
#endif