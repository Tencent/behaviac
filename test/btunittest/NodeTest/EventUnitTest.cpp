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

LOAD_TEST(btunittest, event_ut_0)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/event_ut_0", format);

	ChildNodeTest* childTestAgent = ChildNodeTest::DynamicCast(behaviac::Agent::Create<ChildNodeTest>(100, "ChildNodeTest", 0, 0));
	CHECK_EQUAL(100, childTestAgent->testVar_0);

	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(0);
	myTestAgent->FireEvent("event_test_void");
	CHECK_EQUAL(true, myTestAgent->event_test_var_bool);

	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	behaviac::Workspace::GetInstance()->SetDoubleValueSinceStartup(5.1 * 1000);

	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	myTestAgent->resetProperties();
	myTestAgent->btsetcurrent("node_test/event_ut_0");
	myTestAgent->btexec();
	myTestAgent->FireEvent("event_test_int", 13);
	CHECK_EQUAL(13, myTestAgent->event_test_var_int);

	myTestAgent->FireEvent("event_test_float2", myTestAgent->TestFloat2);
	myTestAgent->FireEvent("event_test_float2_ref", myTestAgent->TestFloat2);
			
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	myTestAgent->resetProperties();
	myTestAgent->btsetcurrent("node_test/event_ut_0");
	myTestAgent->btexec();
	myTestAgent->FireEvent("event_test_int_bool", 15, true);
	CHECK_EQUAL(true, myTestAgent->event_test_var_bool);
	CHECK_EQUAL(15, myTestAgent->event_test_var_int);

	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	myTestAgent->resetProperties();
	myTestAgent->btexec();
	myTestAgent->FireEvent("event_test_int_bool_float", 15, true, 27.3f);
	CHECK_EQUAL(true, myTestAgent->event_test_var_bool);
	CHECK_EQUAL(15, myTestAgent->event_test_var_int);
	CHECK_FLOAT_EQUAL(27.3f, myTestAgent->event_test_var_float);

	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	myTestAgent->resetProperties();
	myTestAgent->btsetcurrent("node_test/event_ut_0");
	myTestAgent->btexec();
	myTestAgent->testVar_0 = 0;
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);
	CHECK_EQUAL(0, myTestAgent->testVar_1);

	myTestAgent->FireEvent("event_test_int_bool_float", 19, true, 11.9f);
	CHECK_EQUAL(false, myTestAgent->event_test_var_bool);
	CHECK_EQUAL(-1, myTestAgent->event_test_var_int);
	CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->event_test_var_float);

	myTestAgent->resetProperties();
	myTestAgent->btsetcurrent("node_test/event_ut_0");
	myTestAgent->btexec();

	CHECK_EQUAL(NULL, myTestAgent->event_test_var_agent);
	myTestAgent->FireEvent("event_test_agent", myTestAgent);
	CHECK_EQUAL(true, myTestAgent->event_test_var_agent != NULL);

	myTestAgent->resetProperties();
	myTestAgent->btsetcurrent("node_test/event_ut_0");
	myTestAgent->btexec();

	CHECK_EQUAL(NULL, myTestAgent->event_test_var_agent);
	myTestAgent->FireEvent("event_test_agent", childTestAgent);
	CHECK_EQUAL(true, myTestAgent->event_test_var_agent != NULL);

	BEHAVIAC_DELETE(childTestAgent);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, event_ut_1)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/event_ut_1", format);

	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	myTestAgent->FireEvent("event_test_int", 13);
	CHECK_EQUAL(13, myTestAgent->event_test_var_int);
			
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	
	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, event_ut_2)
{
	registerAllTypes();

	ChildNodeTest* childTestAgent = ChildNodeTest::DynamicCast(behaviac::Agent::Create<ChildNodeTest>(100, "ChildNodeTest", 0, 0));
	CHECK_EQUAL(100, childTestAgent->testVar_0);

	childTestAgent->resetProperties();

	const char* treePath = "node_test/event_ut_2";
	childTestAgent->btload(treePath);
    childTestAgent->btsetcurrent(treePath);

	childTestAgent->btexec();

	behaviac::EBTStatus status = childTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	childTestAgent->FireEvent("event_test_int_bool", 15, true);
	CHECK_EQUAL(true, childTestAgent->event_test_var_bool);
	CHECK_EQUAL(15, childTestAgent->event_test_var_int);

	status = childTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_SUCCESS, status);

	BEHAVIAC_DELETE(childTestAgent);

	unregisterAllTypes();
}

LOAD_TEST(btunittest, event_ut_3)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/event_ut_3", format);

	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);
	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);
	CHECK_EQUAL(1, myTestAgent->testVar_0);

	myTestAgent->FireEvent("event_test_void");

	CHECK_EQUAL(true, myTestAgent->event_test_var_bool);

	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, event_ut_4)
{
	AgentNodeTest* myTestAgent = initTestEnvNode("node_test/event_ut_4", format);

	myTestAgent->resetProperties();

	behaviac::EBTStatus status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	myTestAgent->FireEvent("event_test_a", 0);

	status = myTestAgent->btexec();
	CHECK_EQUAL(behaviac::BT_RUNNING, status);

	myTestAgent->FireEvent("event_test_b", 1);

	finlTestEnvNode(myTestAgent);
}
