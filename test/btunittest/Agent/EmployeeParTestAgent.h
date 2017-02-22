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
#include "Agent/ParTestAgent.h"

class EmployeeParTestAgent : public ParTestAgent
{
public:
    EmployeeParTestAgent();
    virtual ~EmployeeParTestAgent();

    static void clearAllStaticMemberVariables()
    {
        STV_F_0 = 0.0f;
        STV_STR_0 = "";
        STV_AGENT_0 = NULL;
        STV_LIST_F_0.clear();
        STV_LIST_STR_0.clear();
        STV_LIST_AGENT_0.clear();

        ParTestAgent::STV_INT_0 = 0;
        ParTestAgent::STV_KEMPLOYEE_0.resetProperties();
        ParTestAgent::STV_LIST_INT_0.clear();
        ParTestAgent::STV_LIST_KEMPLOYEE_0.clear();

        ParTestAgentBase::STV_ECOLOR_0 = WHITE;
        ParTestAgentBase::STV_BOOL_0 = false;
        ParTestAgentBase::STV_CHAR_0 = L'\0';
        ParTestAgentBase::STV_LIST_ECOLOR_0.clear();
        ParTestAgentBase::STV_LIST_BOOL_0.clear();
        ParTestAgentBase::STV_LIST_CHAR_0.clear();
        ParTestAgentBase::STV_LIST_SBYTE_0.clear();
    }

	BEHAVIAC_DECLARE_AGENTTYPE(EmployeeParTestAgent, ParTestAgent);

    //[behaviac.MemberMetaInfo("2 # TV_UINT_0", "A")]
    unsigned int TV_UINT_0;

    //[behaviac.MemberMetaInfo("2 # TV_ULONG_0", "A")]
    unsigned  long TV_ULONG_0;

    //[behaviac.MemberMetaInfo("2 # TV_LL_0", "A")]
    long long TV_LL_0;

    //[behaviac.MemberMetaInfo("2 # TV_ULL_0", "A")]
    unsigned long long TV_ULL_0;

    //[behaviac.MemberMetaInfo("2 # TV_F_0", "A")]
    float TV_F_0;

    //[behaviac.MemberMetaInfo("2 # STV_F_0", "A")]
    static float STV_F_0;

    //[behaviac.MemberMetaInfo("2 # TV_D_0", "A")]
    double TV_D_0;

    //[behaviac.MemberMetaInfo("2 # TV_STR_0", "A")]
    behaviac::string TV_STR_0;

    //[behaviac.MemberMetaInfo("2 # STV_STR_0", "A")]
    static behaviac::string STV_STR_0;

    //[behaviac.MemberMetaInfo("2 # STV_CSZSTR_0", "A")]
    const char* TV_CSZSTR_0;

    //[behaviac.MemberMetaInfo("2 # STV_SZSTR_0", "A")]
    char* TV_SZSTR_0;
    // just temp variable to store the string that passed by char *
    behaviac::string TV_SzString_0;
    // just temp variable to store the string that passed by const char *
    behaviac::string TV_CSzString;

    //[behaviac.MemberMetaInfo("2 # TV_AGENT_0", "A")]
    behaviac::Agent* TV_AGENT_0;

    //[behaviac.MemberMetaInfo("2 # STV_AGENT_0", "A")]
    static behaviac::Agent* STV_AGENT_0;

    //[behaviac.MemberMetaInfo("2 # TV_LIST_F_0", "A")]
    behaviac::vector<float> TV_LIST_F_0;

    //[behaviac.MemberMetaInfo("2 # STV_LIST_F_0", "A")]
    static behaviac::vector<float> STV_LIST_F_0;

    //[behaviac.MemberMetaInfo("2 # TV_LIST_STR_0", "A")]
    behaviac::vector<behaviac::string> TV_LIST_STR_0;

    //[behaviac.MemberMetaInfo("2 # STV_LIST_STR_0", "A")]
    static behaviac::vector<behaviac::string> STV_LIST_STR_0;

    //[behaviac.MemberMetaInfo("2 # TV_LIST_AGENT_0", "A")]
    behaviac::vector<behaviac::Agent*> TV_LIST_AGENT_0;

    //[behaviac.MemberMetaInfo("2 # STV_LIST_AGENT_0", "A")]
    static behaviac::vector<behaviac::Agent*> STV_LIST_AGENT_0;

public:
    virtual void resetProperties();

    void init()
    {
        //Init();
        resetProperties();
    }

    void finl()
    {
    }

    //[behaviac.MethodMetaInfo("2 # UPR_UInt", "A")]
    void Func_UIntRef(unsigned int& par)
    {
        par = 1;
    }

    //[behaviac.MethodMetaInfo("2 # UPR_ULong", "A")]
    void Func_ULongRef(unsigned long& par)
    {
        par = 2;
    }

    //[behaviac.MethodMetaInfo("2 # UPR_Single", "A")]
    void Func_SingleRef(float& par)
    {
        par = 3.0f;
    }

    //[behaviac.MethodMetaInfo("2 # UPR_Double", "A")]
    void Func_DoubleRef(double& par)
    {
        par = 4.0;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_LongLong", "A")]
    void Func_LongLongRef(long long& par)
    {
        par -= 200 ;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_ULongLong", "A")]
    void Func_ULongLongRef(unsigned long long& par)
    {
        par += 200;
    }

    //[behaviac.MethodMetaInfo("2 # UPR_String", "A")]
    void Func_StringRef(behaviac::string& par)
    {
        par = "This is a behaviac::string ref in test!";
    }
    //[behaviac.MethodMetaInfo("2 # UPR_SzString", "A")]
    void Func_SzStringRef(char*& par)
    {
        TV_SzString_0 = par;
    }

    //[behaviac.MethodMetaInfo("2 # UPR_CSzString", "A")]
    void Func_CSzStringRef(const char* par)
    {
        TV_CSzString = par;
    }
    //[behaviac.MethodMetaInfo("2 # UPR_Agent", "A")]
    void Func_AgentRef(behaviac::Agent*& par)
    {
        par = this;
    }

    //[behaviac.MethodMetaInfo("2 # UPR_SingleList", "A")]
    void Func_SingleListRef(behaviac::vector<float>& par)
    {
        par.push_back(1.0f);
    }

    //[behaviac.MethodMetaInfo("2 # UPR_StringList", "A")]
    void Func_StringListRef(behaviac::vector<behaviac::string>& par)
    {
        par.push_back("This is a behaviac::string ref in test!");
    }

    //[behaviac.MethodMetaInfo("2 # UPR_AgentList", "A")]
    void Func_AgentListRef(behaviac::vector<behaviac::Agent*>& par)
    {
        par.push_back(this);
    }

    //[behaviac.MethodMetaInfo("2 # PIR_UInt", "A")]
    unsigned int Func_UIntIR(unsigned int par)
    {
        unsigned int tv = par + 54;
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_ULong", "A")]
    unsigned long Func_ULongIR(unsigned long par)
    {
        unsigned long tv = par + 89;
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_Single", "A")]
    float Func_SingleIR(float par)
    {
        float tv = par + 72.3f;
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_Double", "A")]
    double Func_DoubleIR(double par)
    {
        double tv = par + 42.9;
        return tv;
    }
    //[behaviac.MethodMetaInfo("2 # PIR_LongLong", "A")]
    long long Func_LongLongIR(long long par)
    {
        par -= 200;
        return par;
    }
    //[behaviac.MethodMetaInfo("2 # PIR_ULongLong", "A")]
    unsigned long long Func_ULongLongIR(unsigned long long par)
    {
        par += 200;
        return par;
    }
    //[behaviac.MethodMetaInfo("2 # PIR_String", "A")]
    behaviac::string Func_StringIR(const behaviac::string& par)
    {
        behaviac::string tv = par + "extra";
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_String", "A")]
    char* Func_SzStringIR(char* par)
    {
        return par;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_String", "A")]
    const char*  Func_CSzStringIR(const char* par)
    {
        TV_CSzString = par;

        return TV_CSzString.c_str();
    }

    //[behaviac.MethodMetaInfo("2 # PIR_Agent", "A")]
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

    ParTestAgentBase* Func_ParTestAgentBaseIR(ParTestAgentBase* par)
    {
        ParTestAgentBase* tv = NULL;

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

    //[behaviac.MethodMetaInfo("2 # PIR_SingleList", "A")]
    behaviac::vector<float> Func_SingleListIR(behaviac::vector<float> par)
    {
        behaviac::vector<float> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(93.7f);
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_StringList", "A")]
    behaviac::vector<behaviac::string> Func_StringListIR(behaviac::vector<behaviac::string> par)
    {
        behaviac::vector<behaviac::string> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back("extra");
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # PIR_AgentList", "A")]
    behaviac::vector<behaviac::Agent*> Func_AgentListIR(behaviac::vector<behaviac::Agent*> par)
    {
        behaviac::vector<behaviac::Agent*> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(this);
        return tv;
    }

    behaviac::vector<ParTestAgentBase*> Func_ParTestAgentBaseListIR(behaviac::vector<ParTestAgentBase*> par)
    {
        behaviac::vector<ParTestAgentBase*> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back(this);
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # SMF_ULong", "A")]
    static unsigned  long Func_ULongSMF(unsigned long par)
    {
        unsigned  long tv = par + 89;
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # SMF_String", "A")]
    static behaviac::string Func_StringSMF(const behaviac::string& par)
    {
        behaviac::string tv = par + "extra";
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # SMF_StringList", "A")]
    static behaviac::vector<behaviac::string> Func_StringListSMF(behaviac::vector<behaviac::string> par)
    {
        behaviac::vector<behaviac::string> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        tv.push_back("extra");
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # SMF_Agent", "A")]
    static behaviac::Agent* Func_AgentSMF(behaviac::Agent* par)
    {
		BEHAVIAC_UNUSED_VAR(par);
        behaviac::Agent* tv = NULL;
        //		if(par == NULL)
        //			tv = this;
        //		else
        //			tv = NULL;
        return tv;
    }

    //[behaviac.MethodMetaInfo("2 # SMF_AgentList", "A")]
    static behaviac::vector<behaviac::Agent*> Func_AgentListSMF(behaviac::vector<behaviac::Agent*> par)
    {
        behaviac::vector<behaviac::Agent*> tv;

        for (size_t i = 0; i < par.size(); ++i)
        {
            tv.push_back(par[i]);
        }

        //tv.push_back(this);
        tv.push_back(NULL);
        return tv;
    }
};

