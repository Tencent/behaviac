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

using System;
using System.Collections.Generic;
using behaviac;
using TNS.NE.NAT;

[behaviac.TypeMetaInfo()]
public class EmployeeParTestAgent : ParTestAgent
{
    [behaviac.MemberMetaInfo("2 # TV_UINT_0", "A")]
    public uint TV_UINT_0 = 0;

    [behaviac.MemberMetaInfo("2 # TV_ULONG_0", "A")]
    public ulong TV_ULONG_0 = 0L;

    [behaviac.MemberMetaInfo("2 # TV_LL_0", "A")]
    public long TV_LL_0;

    [behaviac.MemberMetaInfo("2 # TV_ULL_0","A")]
    public ulong TV_ULL_0;

    [behaviac.MemberMetaInfo("2 # TV_F_0", "A")]
    public Single TV_F_0 = 0.0f;

    [behaviac.MemberMetaInfo("2 # STV_F_0", "A")]
    public static Single STV_F_0 = 0.0f;

    [behaviac.MemberMetaInfo("2 # TV_D_0", "A")]
    public Double TV_D_0 = 0.0;

    [behaviac.MemberMetaInfo("2 # TV_STR_0", "A")]
    public String TV_STR_0 = String.Empty;

    [behaviac.MemberMetaInfo("2 # TV_CSZSTR_0", "A")]
    public String TV_CSZSTR_0 = String.Empty;

    [behaviac.MemberMetaInfo("2 # STV_STR_0", "A")]
    public static String STV_STR_0 = String.Empty;

    // just temp variable to store the string that passed by char *
    public string TV_SzString_0;
    // just temp variable to store the string that passed by const char *
    public string TV_CSzString;


    [behaviac.MemberMetaInfo("2 # TV_AGENT_0", "A")]
    public Agent TV_AGENT_0 = null;

    [behaviac.MemberMetaInfo("2 # STV_AGENT_0", "A")]
    public static Agent STV_AGENT_0 = null;

    [behaviac.MemberMetaInfo("2 # TV_LIST_F_0", "A")]
    public List<Single> TV_LIST_F_0;

    [behaviac.MemberMetaInfo("2 # STV_LIST_F_0", "A")]
    public static List<Single> STV_LIST_F_0;

    [behaviac.MemberMetaInfo("2 # TV_LIST_STR_0", "A")]
    public List<String> TV_LIST_STR_0;

    [behaviac.MemberMetaInfo("2 # STV_LIST_STR_0", "A")]
    public static List<String> STV_LIST_STR_0;

    [behaviac.MemberMetaInfo("2 # TV_LIST_AGENT_0", "A")]
    public List<Agent> TV_LIST_AGENT_0;

    [behaviac.MemberMetaInfo("2 # STV_LIST_AGENT_0", "A")]
    public static List<Agent> STV_LIST_AGENT_0;

    public EmployeeParTestAgent() {
        TV_LIST_F_0 = new List<Single>();
        STV_LIST_F_0 = new List<Single>();
        TV_LIST_STR_0 = new List<String>();
        STV_LIST_STR_0 = new List<String>();
        TV_LIST_AGENT_0 = new List<Agent>();
        STV_LIST_AGENT_0 = new List<Agent>();
    }

    static public void clearAllStaticMemberVariables() {
        STV_F_0 = 0.0f;
        STV_STR_0 = "";
        STV_AGENT_0 = null;
        STV_LIST_F_0.Clear();
        STV_LIST_STR_0.Clear();
        STV_LIST_AGENT_0.Clear();

        ParTestAgent.STV_INT_0 = 0;
        ParTestAgent.STV_KEMPLOYEE_0.resetProperties();
        ParTestAgent.STV_LIST_INT_0.Clear();
        ParTestAgent.STV_LIST_KEMPLOYEE_0.Clear();

        ParTestAgentBase.STV_ECOLOR_0 = eColor.WHITE;
        ParTestAgentBase.STV_BOOL_0 = false;
        ParTestAgentBase.STV_CHAR_0 = '\0';
        ParTestAgentBase.STV_LIST_ECOLOR_0.Clear();
        ParTestAgentBase.STV_LIST_BOOL_0.Clear();
        ParTestAgentBase.STV_LIST_CHAR_0.Clear();
        ParTestAgentBase.STV_LIST_SBYTE_0.Clear();
    }

    public override void resetProperties() {
        base.resetProperties();

        TV_UINT_0 = 0;
        TV_ULONG_0 = 0L;
        TV_F_0 = 0.0f;
        STV_F_0 = 0.0f;
        TV_D_0 = 0.0;
        TV_STR_0 = String.Empty;
        STV_STR_0 = String.Empty;
        TV_AGENT_0 = null;
        STV_AGENT_0 = null;

        TV_LIST_F_0.Clear();
        STV_LIST_F_0.Clear();
        TV_LIST_STR_0.Clear();
        STV_LIST_STR_0.Clear();
        TV_LIST_AGENT_0.Clear();
        STV_LIST_AGENT_0.Clear();
    }

    public void init() {
        this.Init();
    }

    public void finl() {
    }

    #region Par Ref Test

    [behaviac.MethodMetaInfo("2 # UPR_UInt", "A")]
    public void Func_UIntRef(ref uint par) {
        par = 1;
    }

    [behaviac.MethodMetaInfo("2 # UPR_ULong", "A")]
    public void Func_ULongRef(ref ulong par) {
        par = 2;
    }

    [behaviac.MethodMetaInfo("2 # UPR_Single", "A")]
    public void Func_SingleRef(ref Single par) {
        par = 3.0f;
    }

    [behaviac.MethodMetaInfo("2 # UPR_Double", "A")]
    public void Func_DoubleRef(ref Double par) {
        par = 4.0;
    }

    [behaviac.MethodMetaInfo("2 # UPR_LongLongRef","A")]
    public void Func_LongLongRef(ref long par)
    {
        par -=200;
    }

    [behaviac.MethodMetaInfo("2 # UPR_ULongLongRef", "A")]
    public void Func_ULongLongRef(ref ulong par)
    {
        par +=200;
    }

    [behaviac.MethodMetaInfo("2 # UPR_String", "A")]
    public void Func_StringRef(ref String par) {
        par = "This is a string ref in test!";
    }

    [behaviac.MethodMetaInfo("2 # UPR_SzStringRef", "A")]
    public void Func_SzStringRef(ref String par) {
        TV_SzString_0 = par;
    }

    [behaviac.MethodMetaInfo("2 # UPR_CSzString", "A")]
    public void Func_CSzStringRef(ref String par) {
        TV_CSzString = par;
    }

    [behaviac.MethodMetaInfo("2 # UPR_Agent", "A")]
    public void Func_AgentRef(ref Agent par) {
        par = this;
    }

    [behaviac.MethodMetaInfo("2 # UPR_SingleList", "A")]
    public void Func_SingleListRef(ref List<Single> par) {
        par.Add(1.0f);
    }

    [behaviac.MethodMetaInfo("2 # UPR_StringList", "A")]
    public void Func_StringListRef(ref List<String> par) {
        par.Add("This is a string ref in test!");
    }

    [behaviac.MethodMetaInfo("2 # UPR_AgentList", "A")]
    public void Func_AgentListRef(ref List<Agent> par) {
        par.Add(this);
    }

    #endregion Par Ref Test

    #region Par IR Test

    [behaviac.MethodMetaInfo("2 # PIR_UInt", "A")]
    public uint Func_UIntIR(uint par) {
        uint tv = par + 54;
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_ULong", "A")]
    public ulong Func_ULongIR(ulong par) {
        ulong tv = par + 89;
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_Single", "A")]
    public Single Func_SingleIR(Single par) {
        Single tv = par + 72.3f;
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_Double", "A")]
    public Double Func_DoubleIR(Double par) {
        Double tv = par + 42.9;
        return tv;
    }
    [behaviac.MethodMetaInfo("2 # PIR_LongLongIR", "A")]
    public long  Func_LongLongIR(long par) {
        par -= 200;
        return par;
    }
    [behaviac.MethodMetaInfo("2 # PIR_ULongLongIR", "A")]
    public ulong Func_ULongLongIR(ulong par) {
        par += 200;
        return par;
    }

    [behaviac.MethodMetaInfo("2 # PIR_String", "A")]
    public String Func_StringIR(String par) {
        String tv = par + "extra";
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_ParTestAgentBase", "A")]
    public ParTestAgentBase Func_ParTestAgentBaseIR(ParTestAgentBase par) {
        ParTestAgentBase tv = null;

        if (par == null)
        { tv = this; }

        else
        { tv = null; }

        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_Agent", "A")]
    public Agent Func_AgentIR(Agent par) {
        Agent tv = null;

        if (par == null)
        { tv = this; }

        else
        { tv = null; }

        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_SingleList", "A")]
    public List<Single> Func_SingleListIR(List<Single> par) {
        List<Single> tv = new List<Single>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(93.7f);
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_StringList", "A")]
    public List<String> Func_StringListIR(List<String> par) {
        List<String> tv = new List<String>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add("extra");
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_AgentList", "A")]
    public List<Agent> Func_AgentListIR(List<Agent> par) {
        List<Agent> tv = new List<Agent>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(this);
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # PIR_ParTestAgentBaseList", "A")]
    public List<ParTestAgentBase> Func_ParTestAgentBaseListIR(List<ParTestAgentBase> par) {
        List<ParTestAgentBase> tv = new List<ParTestAgentBase>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(this);
        return tv;
    }

    #endregion Par IR Test

    #region Static Member Function Test

    [behaviac.MethodMetaInfo("2 # SMF_ULong", "A")]
    static public ulong Func_ULongSMF(ulong par) {
        ulong tv = par + 89;
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # SMF_String", "A")]
    static public String Func_StringSMF(String par) {
        String tv = par + "extra";
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # SMF_StringList", "A")]
    static public List<String> Func_StringListSMF(List<String> par) {
        List<String> tv = new List<String>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add("extra");
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # SMF_Agent", "A")]
    static public Agent Func_AgentSMF(Agent par) {
        Agent tv = null;
        //		if(par == null)
        //			tv = this;
        //		else
        //			tv = null;
        return tv;
    }

    [behaviac.MethodMetaInfo("2 # SMF_AgentList", "A")]
    static public List<Agent> Func_AgentListSMF(List<Agent> par) {
        List<Agent> tv = new List<Agent>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        //tv.Add(this);
        tv.Add(null);
        return tv;
    }

    #endregion Static Member Function Test
}
