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

#ifndef BTUNITEST_HTNAGENTHOUSE_H_
#define BTUNITEST_HTNAGENTHOUSE_H_

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/agent/registermacros.h"
#include "HTNAgentHouseBase.h"

class HTNAgentHouse : public HTNAgentHouseBase
{
public:
    HTNAgentHouse();
    virtual ~HTNAgentHouse();

	BEHAVIAC_DECLARE_AGENTTYPE(HTNAgentHouse, HTNAgentHouseBase);

    void resetProperties()
    {
    }

    void init()
    {
        //super::Init();
        resetProperties();
    }

    void finl()
    {
    }

    void HireBuilder()
    {
    }

    void PayBuilder()
    {
    }

    void BuildFoundation()
    {
    }

    void BuildFrame()
    {
    }

    void BuildRoof()
    {
    }

    void BuildWalls()
    {
    }

    void BuildInterior()
    {
    }

    void CutLogs()
    {
    }

    void GetFriend()
    {
    }

    void BuyLand()
    {
    }

    void GetLoan()
    {
    }
};

#endif//BTUNITEST_HTNAGENTHOUSE_H_
