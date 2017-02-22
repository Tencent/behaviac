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
#include "UnitTestTypes.h"
#include "behaviac/common/container/string.h"

using TNS::NE::NAT::eColor;
using TNS::NE::NAT::WHITE;
using TNS::NE::NAT::RED;
using TNS::NE::NAT::GREEN;
using TNS::NE::NAT::YELLOW;
using TNS::NE::NAT::BLUE;
using TNS::ST::PER::WRK::kEmployee;

class ParTestAgentBase : public behaviac::Agent
{
public:
    ParTestAgentBase();
    virtual ~ParTestAgentBase();

	BEHAVIAC_DECLARE_AGENTTYPE(ParTestAgentBase, behaviac::Agent);

    eColor					TV_ECOLOR_0;
    static eColor			STV_ECOLOR_0;
    bool					TV_BOOL_0;
    static bool				STV_BOOL_0;
    char					TV_CHAR_0;
    static char				STV_CHAR_0;			//< char -> char
    unsigned char			TV_BYTE_0;			//< unsigned char -> unsigned char
    signed char				TV_SBYTE_0;			//< signed char -> signed char
    behaviac::vector<eColor>		TV_LIST_ECOLOR_0;	//< behaviac::vector -> behaviac::vector;
    static behaviac::vector<eColor> STV_LIST_ECOLOR_0;
    behaviac::vector<bool>			TV_LIST_BOOL_0;
    static behaviac::vector<bool>	STV_LIST_BOOL_0;
    behaviac::vector<char>			TV_LIST_CHAR_0;
    static behaviac::vector<char>	STV_LIST_CHAR_0;
    behaviac::vector<unsigned char>	TV_LIST_BYTE_0;
    behaviac::vector<signed char>	TV_LIST_SBYTE_0;
    static behaviac::vector<signed char> STV_LIST_SBYTE_0;

public:
    virtual void resetProperties();

    void Func_eColorRef(eColor& par)  	//< ref -> &
    {
        par = BLUE;
    }

    void Func_BooleanRef(bool& par)
    {
        par = true;
    }

    void Func_CharRef(char& par)
    {
        par = 'X';
    }

    void Func_ByteRef(unsigned char& par)
    {
        par = 2;
    }

    void Func_SByteRef(signed char& par)
    {
        par = -2;
    }

    void Func_eColorListRef(behaviac::vector<eColor>& par)
    {
        par.push_back(RED);
    }

    void Func_BooleanListRef(behaviac::vector<bool>& par)
    {
        par.push_back(true);
    }

    void Func_CharListRef(behaviac::vector<char>& par)
    {
        par.push_back('k');
    }

    void Func_ByteListRef(behaviac::vector<unsigned char>& par)
    {
        par.push_back(8);
    }

    void Func_SByteListRef(behaviac::vector<signed char>& par)
    {
        par.push_back(-4);
    }

    eColor Func_eColorIR(eColor par)
    {
        if (par == WHITE)
        {
            return BLUE;

        }
        else
        {
            return RED;
        }
    }

    bool Func_BooleanIR(bool par)
    {
        bool tv = !par;
        return tv;
    }

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

    unsigned char Func_ByteIR(unsigned char par)
    {
        unsigned char tv = (unsigned char)(par + 12);
        return tv;
    }

    signed char Func_SByteIR(signed char par)
    {
        signed char tv = (signed char)(par - 5);
        return tv;
    }

    behaviac::vector<eColor> Func_eColorListIR(behaviac::vector<eColor> par)
    {
        behaviac::vector<eColor> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(YELLOW);
        return tv;
    }

    behaviac::vector<bool> Func_BooleanListIR(behaviac::vector<bool> par)
    {
        behaviac::vector<bool> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(false);
        return tv;
    }

    behaviac::vector<char> Func_CharListIR(behaviac::vector<char> par)
    {
        behaviac::vector<char> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back('m');
        return tv;
    }

    behaviac::vector<unsigned char> Func_ByteListIR(behaviac::vector<unsigned char> par)
    {
        behaviac::vector<unsigned char> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(126);
        return tv;
    }

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

    static char Func_CharSMF(char par)
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

    static unsigned char Func_ByteSMF(unsigned char par)
    {
        unsigned char tv = (unsigned char)(par + 12);
        return tv;
    }

    static signed char Func_SByteSMF(signed char par)
    {
        signed char tv = (signed char)(par - 5);
        return tv;
    }

    static behaviac::vector<char> Func_CharListSMF(behaviac::vector<char> par)
    {
        behaviac::vector<char> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back('m');
        return tv;
    }

    static behaviac::vector<signed char> Func_SByteListSMF(behaviac::vector<signed char> par)
    {
        behaviac::vector<signed char> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(-126);
        return tv;
    }
};
