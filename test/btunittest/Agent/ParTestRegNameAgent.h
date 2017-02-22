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

#pragma once

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/agent/registermacros.h"
#include "Agent/UnitTestTypes.h"

using TNS::NE::NAT::eColor;
using TNS::NE::NAT::WHITE;
using TNS::NE::NAT::RED;
using TNS::NE::NAT::GREEN;
using TNS::NE::NAT::YELLOW;
using TNS::NE::NAT::BLUE;
using TNS::ST::PER::WRK::kEmployee;

//[TypeMetaInfo()]
class StaticAgent : public behaviac::Agent
{
public:
	BEHAVIAC_DECLARE_AGENTTYPE(StaticAgent, behaviac::Agent);

    //[MemberMetaInfo()]
    static int sInt;

    //[MethodMetaInfo()]
    static void sAction()
    {
        sInt = 2;
    }
};

class ParTestRegNameAgent : public behaviac::Agent
{
public:
    ParTestRegNameAgent();
    virtual ~ParTestRegNameAgent();

	BEHAVIAC_DECLARE_AGENTTYPE(ParTestRegNameAgent, behaviac::Agent);

    static void clearAllStaticMemberVariables()
    {
        ParTestRegNameAgent::STV_KEMPLOYEE_0.resetProperties();

        ParTestRegNameAgent::STV_LIST_SBYTE_0.clear();
        behaviac::vector<signed char>().swap(ParTestRegNameAgent::STV_LIST_SBYTE_0);

        ParTestRegNameAgent::STV_LIST_KEMPLOYEE_0.clear();
        behaviac::vector<kEmployee>().swap(ParTestRegNameAgent::STV_LIST_KEMPLOYEE_0);
    }

    //[MemberMetaInfo("3 # TV_CHAR_0", "A")]
    char TV_CHAR_0;

    //[MemberMetaInfo("3 # TV_BYTE_0", "A")]
    unsigned char TV_BYTE_0;

    //[MemberMetaInfo("3 # TV_SBYTE_0", "A")]
    signed char TV_SBYTE_0;

    //[MemberMetaInfo("3 # TV_STR_0", "A")]
    behaviac::string TV_STR_0;

    //[MemberMetaInfo("3 # TV_AGENT_0", "A")]
    behaviac::Agent* TV_AGENT_0;

    //[MemberMetaInfo("3 # TV_KEMPLOYEE_0", "A")]
    kEmployee TV_KEMPLOYEE_0;

    //[MemberMetaInfo("3 # STV_KEMPLOYEE_0", "A")]
    static kEmployee STV_KEMPLOYEE_0;

    //[MemberMetaInfo("3 # TV_LIST_KEMPLOYEE_0", "A")]
    behaviac::vector<kEmployee> TV_LIST_KEMPLOYEE_0;

    //[MemberMetaInfo("3 # STV_LIST_SBYTE_0", "A")]
    static behaviac::vector<signed char> STV_LIST_SBYTE_0;

    //[MemberMetaInfo("3 # STV_LIST_KEMPLOYEE_0", "A")]
    static behaviac::vector<kEmployee> STV_LIST_KEMPLOYEE_0;

public:
    virtual void resetProperties();

    void init()
    {
        //base.Init();

        //behaviac::Agent*.RegisterInstanceName<ParTestRegNameAgent>();
        //behaviac::Agent*.BindInstance(this);
    }

    void finl()
    {
        //behaviac::Agent*.UnbindInstance("ParTestRegNameAgent");
        //behaviac::Agent*.UnRegisterInstanceName<ParTestRegNameAgent>(NULL);
    }

    //[MethodMetaInfo("3 # PIR_Char", "A")]
    char Func_CharIR(char par)
    {
        if (par == 'A')
        {
            return 'C';

        }
        else
        {
            return 'D';
        }
    }

    //[MethodMetaInfo("3 # PIR_Byte", "A")]
    unsigned char Func_ByteIR(unsigned char par)
    {
        unsigned char tv = (unsigned char)(par + 12);
        return tv;
    }

    //[MethodMetaInfo("3 # PIR_SByte", "A")]
    signed char Func_SByteIR(signed char par)
    {
        signed char tv = (signed char)(par - 5);
        return tv;
    }

    //[MethodMetaInfo("3 # PIR_String", "A")]
    behaviac::string Func_StringIR(behaviac::string par)
    {
        behaviac::string tv = par + "extra";
        return tv;
    }

    //[MethodMetaInfo("3 # PIR_Agent", "A")]
    behaviac::Agent* Func_AgentIR(behaviac::Agent* par)
    {
        behaviac::Agent* tv = NULL;

        if (par == NULL)
        {
            tv = this;

        }
        else
        {
            tv = NULL;
        }

        return tv;
    }

    //[MethodMetaInfo("3 # PIR_kEmployee", "A")]
    kEmployee Func_kEmployeeIR(kEmployee par)
    {
        kEmployee tv;
        tv.id = par.id + 3;
        tv.name = par.name + "Jerry";

        if (par.code == 'C')
        {
            tv.code = 'Z';

        }
        else
        {
            tv.code = 'V';
        }

        tv.weight = par.weight + 20.2f;
        tv.isMale = !par.isMale;

        if (par.skinColor == WHITE)
        {
            tv.skinColor = RED;

        }
        else
        {
            tv.skinColor = BLUE;
        }

        if (par.boss == NULL)
        {
            tv.boss = this;

        }
        else
        {
            tv.boss = NULL;
        }

        tv.car.brand = par.car.brand + "Japan";

        if (par.car.color == WHITE)
        {
            tv.car.color = YELLOW;

        }
        else
        {
            tv.car.color = RED;
        }

        tv.car.price = par.car.price + 3000;
        return tv;
    }

    //[MethodMetaInfo("3 # PIR_SByteList", "A")]
    behaviac::vector<signed char> Func_SByteListIR(behaviac::vector<signed char> par)
    {
        behaviac::vector<signed char> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(-126);
        return tv;
    }

    //[MethodMetaInfo("3 # PIR_kEmployeeList", "A")]
    behaviac::vector<kEmployee> Func_kEmployeeListIR(behaviac::vector<kEmployee> par)
    {
        behaviac::vector<kEmployee> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        kEmployee jerry;
        jerry.id = 4;
        jerry.name = "Jerry";
        jerry.code = 'J';
        jerry.weight = 60.0f;
        jerry.isMale = false;
        jerry.skinColor = WHITE;
        jerry.boss = NULL;
        jerry.car.brand = "Toyota";
        jerry.car.color = YELLOW;
        jerry.car.price = 43000;
        tv.push_back(jerry);

        return tv;
    }
};
