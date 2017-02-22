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

#ifndef BTUNITEST_FSMAGENTTEST_H_
#define BTUNITEST_FSMAGENTTEST_H_

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/agent/registermacros.h"

class FSMAgentTest : public behaviac::Agent
{
public:
    FSMAgentTest();
    virtual ~FSMAgentTest();

	BEHAVIAC_DECLARE_AGENTTYPE(FSMAgentTest, behaviac::Agent);

    enum EMessage
    {
        Invalid,
        Begin,
        End,
        Pause,
        Resume,
        Exit
    };

    void resetProperties()
    {
        TestVar = -1;
        Message = Invalid;
    }

    void init()
    {
        //base.Init();
        resetProperties();
    }

    void finl()
    {
    }

    EMessage Message ;

    int TestVar ;

    void inactive_update()
    {
        TestVar++;
    }

    void active_update()
    {
        TestVar++;
    }

    void pause_update()
    {
        TestVar++;
    }

    void exit_update()
    {
        TestVar++;
    }
};

DECLARE_BEHAVIAC_ENUM(FSMAgentTest::EMessage, EMessage)

#endif//BTUNITEST_FSMAGENTTEST_H_
