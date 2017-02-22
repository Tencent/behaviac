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

#ifndef BTUNITEST_PRECONEFFECTORAGENT_H_
#define BTUNITEST_PRECONEFFECTORAGENT_H_

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/agent/registermacros.h"

class PreconEffectorAgent : public behaviac::Agent
{
public:
    PreconEffectorAgent();
    virtual ~PreconEffectorAgent();

	BEHAVIAC_DECLARE_AGENTTYPE(PreconEffectorAgent, behaviac::Agent);

    void resetProperties()
    {
        this->count_success = 0;
        this->count_failure = 0;
        this->count_both = 0;
    }

    void init()
    {
        //base.Init();
        resetProperties();
    }

    void finl()
    {
    }

    int count_success ;

    int get_count_success()
    {
        return count_success;
    }

    int count_failure ;

    int count_both ;

    int ret ;

    int fn_return()
    {
        return 5;
    }

    void action()
    {
    }
};

#endif//BTUNITEST_PRECONEFFECTORAGENT_H_
