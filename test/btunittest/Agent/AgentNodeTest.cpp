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

#include "AgentNodeTest.h"


BEHAVIAC_BEGIN_ENUM(EnumTest, EnumTest)
{
	BEHAVIAC_ENUM_ITEM(EnumTest_One, "EnumTest_One");
	BEHAVIAC_ENUM_ITEM(EnumTest_OneAfterOne, "EnumTest_OneAfterOne");
}
BEHAVIAC_END_ENUM()

AgentNodeTest::AgentNodeTest()
{
}

AgentNodeTest::~AgentNodeTest()
{
}

void AgentNodeTest::resetProperties()
{
	par_child = 0;

    testVar_0 = -1;
    testVar_1 = -1;
    testVar_2 = -1.0f;
    testVar_3 = -1.0f;
    event_test_var_int = -1;
    event_test_var_bool = false;
    event_test_var_float = -1.0f;
	event_test_var_agent = NULL;
    waiting_timeout_interval = 0;

    action_0_enter_count = 0;
    action_0_exit_count = 0;
    action_1_enter_count = 0;
    action_1_exit_count = 0;
    action_2_enter_count = 0;
    action_2_exit_count = 0;

    testVar_str_0 = "";
	testColor = EnumTest_One;

	m_bCanSee = false;
	m_bTargetValid = false;

	TestFloat2.x = 2.0f;
	TestFloat2.y = 2.0f;

	testVar_Act.Var_B_Loop = false;
}

namespace UnityEngine
{
    BEHAVIAC_BEGIN_STRUCT(GameObject)
    {
        BEHAVIAC_REGISTER_STRUCT_PROPERTY(name);
    }
    BEHAVIAC_END_STRUCT()
}

BEHAVIAC_BEGIN_STRUCT(Act)
{
	BEHAVIAC_REGISTER_STRUCT_PROPERTY(Var_B_Loop);
	BEHAVIAC_REGISTER_STRUCT_PROPERTY(Var_List_EnumTest);
}
BEHAVIAC_END_STRUCT()

BEHAVIAC_BEGIN_STRUCT(BSASN::SpatialCoord)
{
	BEHAVIAC_REGISTER_STRUCT_PROPERTY(coordX);
	BEHAVIAC_REGISTER_STRUCT_PROPERTY(coordY);
}
BEHAVIAC_END_STRUCT()

BEHAVIAC_BEGIN_STRUCT(BSASN::TransitPlan)
{
	BEHAVIAC_REGISTER_STRUCT_PROPERTY(plan_ID);
	BEHAVIAC_REGISTER_STRUCT_PROPERTY(plan_selection_precedence);
	BEHAVIAC_REGISTER_STRUCT_PROPERTY(transit_points);
}
BEHAVIAC_END_STRUCT()

BEHAVIAC_BEGIN_STRUCT(TestClassA)
{
}
BEHAVIAC_END_STRUCT()


void AgentNodeTest::initChildAgentTest()
{
	ChildNodeTest* testChildAgent = this->getChildAgent<ChildNodeTest>(1, "par_child_agent_1");
	this->SetVariable("par_child_agent_1", testChildAgent);
}

void AgentNodeTest::initChildAgent()
{
    ChildNodeTest* test = this->GetVariable<ChildNodeTest*>("par_child_agent_1");

    test->resetProperties();
    test->testVar_1 = 888;
}

ChildNodeTest::ChildNodeTest(int var_0)
{
	testVar_0 = var_0;
}

ChildNodeTest::~ChildNodeTest()
{
}

float ChildNodeTest::GetConstFloatValue()
{
	return 1000.0f;
}

double ChildNodeTest::GetConstDoubleValue()
{
	return 1000.0;
}

