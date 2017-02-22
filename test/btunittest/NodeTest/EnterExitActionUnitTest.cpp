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

LOAD_TEST(btunittest, enter_exit_action_ut_0)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/enter_exit_action_ut_0", format);
    myTestAgent->resetProperties();

    behaviac::EBTStatus status = myTestAgent->btexec();
    //CHECK_EQUAL(1, myTestAgent->action_0_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_1_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_2_enter_count);

    //CHECK_EQUAL(0, myTestAgent->action_0_exit_count);
    CHECK_EQUAL(0, myTestAgent->action_1_exit_count);
    CHECK_EQUAL(0, myTestAgent->action_2_exit_count);

    CHECK_EQUAL(behaviac::BT_RUNNING, status);
    int loopCount = 1000;

    while (loopCount > 0)
    {
        status = myTestAgent->btexec();
        //CHECK_EQUAL(1, myTestAgent->action_0_enter_count);
        CHECK_EQUAL(1, myTestAgent->action_1_enter_count);
        CHECK_EQUAL(1, myTestAgent->action_2_enter_count);

        //CHECK_EQUAL(0, myTestAgent->action_0_exit_count);
        CHECK_EQUAL(0, myTestAgent->action_1_exit_count);
        CHECK_EQUAL(0, myTestAgent->action_2_exit_count);

        CHECK_EQUAL(behaviac::BT_RUNNING, status);
        --loopCount;
    }

    //
    myTestAgent->testVar_0 = 0;
    status = myTestAgent->btexec();
    //CHECK_EQUAL(1, myTestAgent->action_0_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_1_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_2_enter_count);

    //CHECK_EQUAL(1, myTestAgent->action_0_exit_count);
    CHECK_EQUAL(1, myTestAgent->action_1_exit_count);
    CHECK_EQUAL(1, myTestAgent->action_2_exit_count);

    CHECK_EQUAL(behaviac::BT_SUCCESS, status);

    loopCount = 100;

    while (loopCount > 0)
    {
        status = myTestAgent->btexec();
        --loopCount;
    }

    //CHECK_EQUAL(101, myTestAgent->action_0_enter_count);
    CHECK_EQUAL(101, myTestAgent->action_1_enter_count);
    CHECK_EQUAL(101, myTestAgent->action_2_enter_count);

    //CHECK_EQUAL(101, myTestAgent->action_0_exit_count);
    CHECK_EQUAL(101, myTestAgent->action_1_exit_count);
    CHECK_EQUAL(101, myTestAgent->action_2_exit_count);

    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, enter_exit_action_ut_1)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/enter_exit_action_ut_1", format);
    myTestAgent->resetProperties();

    behaviac::EBTStatus status = myTestAgent->btexec();
    //CHECK_EQUAL(1, myTestAgent->action_0_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_1_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_2_enter_count);

    //CHECK_EQUAL(0, myTestAgent->action_0_exit_count);
    CHECK_EQUAL(0, myTestAgent->action_1_exit_count);
    CHECK_EQUAL(0, myTestAgent->action_2_exit_count);

    CHECK_EQUAL(3, myTestAgent->testVar_1);
    CHECK_STR_EQUAL("hello", myTestAgent->testVar_str_0.c_str());

    CHECK_EQUAL(behaviac::BT_RUNNING, status);

    myTestAgent->testVar_0 = 0;
    status = myTestAgent->btexec();
    //CHECK_EQUAL(1, myTestAgent->action_0_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_1_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_2_enter_count);

    //CHECK_EQUAL(1, myTestAgent->action_0_exit_count);
    CHECK_EQUAL(1, myTestAgent->action_1_exit_count);
    CHECK_EQUAL(1, myTestAgent->action_2_exit_count);

    CHECK_EQUAL(5, myTestAgent->testVar_1);
    CHECK_STR_EQUAL("world", myTestAgent->testVar_str_0.c_str());
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    finlTestEnvNode(myTestAgent);
}

LOAD_TEST(btunittest, enter_exit_action_ut_2)
{
    AgentNodeTest* myTestAgent = initTestEnvNode("node_test/enter_exit_action_ut_2", format);
    myTestAgent->resetProperties();

    behaviac::EBTStatus status = myTestAgent->btexec();
    CHECK_EQUAL(1, myTestAgent->action_0_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_1_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_2_enter_count);

    CHECK_EQUAL(1, myTestAgent->action_0_exit_count);
    CHECK_EQUAL(0, myTestAgent->action_1_exit_count);
    CHECK_EQUAL(0, myTestAgent->action_2_exit_count);

    CHECK_EQUAL(3, myTestAgent->testVar_1);
    CHECK_STR_EQUAL("hello", myTestAgent->testVar_str_0.c_str());
    CHECK_EQUAL(behaviac::BT_RUNNING, status);

    myTestAgent->testVar_0 = 0;
    status = myTestAgent->btexec();
    CHECK_EQUAL(2, myTestAgent->action_0_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_1_enter_count);
    CHECK_EQUAL(1, myTestAgent->action_2_enter_count);

    CHECK_EQUAL(2, myTestAgent->action_0_exit_count);
    CHECK_EQUAL(1, myTestAgent->action_1_exit_count);
    CHECK_EQUAL(1, myTestAgent->action_2_exit_count);

    CHECK_EQUAL(5, myTestAgent->testVar_1);
    CHECK_STR_EQUAL("world", myTestAgent->testVar_str_0.c_str());
    CHECK_EQUAL(behaviac::BT_SUCCESS, status);
    finlTestEnvNode(myTestAgent);
}
