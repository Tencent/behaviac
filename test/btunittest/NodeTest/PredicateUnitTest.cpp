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

LOAD_TEST(btunittest, predicate_selector_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_selector_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_selector_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_selector_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_selector_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_selector_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_selector_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_selector_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_selector_ut_4)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_selector_ut_4", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_selector_ut_5)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_selector_ut_5", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_sequence_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_sequence_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_sequence_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_sequence_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_sequence_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_sequence_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_sequence_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_sequence_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_sequence_ut_4)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_sequence_ut_4", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(-1.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_sequence_ut_5)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_sequence_ut_5", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_FLOAT_EQUAL(0.0f, myTestAgent->testVar_2);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_action_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_action_ut_0", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(-1, myTestAgent->testVar_1);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_action_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_action_ut_1", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_action_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_action_ut_2", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    status = myTestAgent->btexec();
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(0, myTestAgent->testVar_1);
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, predicate_action_ut_3)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/predicate_action_ut_3", format);
    myTestAgent->resetProperties();
    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    status = myTestAgent->btexec();
    CHECK_EQUAL(0, myTestAgent->testVar_0);
    CHECK_EQUAL(behaviac::BT_FAILURE, status);
    finlTestEnvNode(myTestAgent);
}
