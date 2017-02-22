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

using System.Collections;
using behaviac;
using TNS.ST.PER.WRK;
using System;
using System.Collections.Generic;
using TNS.NE.NAT;

[TypeMetaInfo()]
public static class StaticAgent
{
    [MemberMetaInfo()]
    public static int sInt = 1;

    [MethodMetaInfo()]
    public static void sAction() {
        sInt = 2;
    }
}

[TypeMetaInfo()]
public class ParTestRegNameAgent : behaviac.Agent
{
    [MemberMetaInfo("3 # TV_CHAR_0", "A")]
    public Char TV_CHAR_0 = '\0';

    [MemberMetaInfo("3 # TV_BYTE_0", "A")]
    public Byte TV_BYTE_0 = 0;

    [MemberMetaInfo("3 # TV_SBYTE_0", "A")]
    public SByte TV_SBYTE_0 = 0;

    [MemberMetaInfo("3 # TV_STR_0", "A")]
    public String TV_STR_0 = String.Empty;

    [MemberMetaInfo("3 # TV_AGENT_0", "A")]
    public Agent TV_AGENT_0 = null;

    [MemberMetaInfo("3 # TV_KEMPLOYEE_0", "A")]
    public kEmployee TV_KEMPLOYEE_0;

    [MemberMetaInfo("3 # STV_KEMPLOYEE_0", "A")]
    public static kEmployee STV_KEMPLOYEE_0;

    [MemberMetaInfo("3 # TV_LIST_KEMPLOYEE_0", "A")]
    public List<kEmployee> TV_LIST_KEMPLOYEE_0;

    [MemberMetaInfo("3 # STV_LIST_SBYTE_0", "A")]
    public static List<SByte> STV_LIST_SBYTE_0;

    [MemberMetaInfo("3 # STV_LIST_KEMPLOYEE_0", "A")]
    public static List<kEmployee> STV_LIST_KEMPLOYEE_0;

    public ParTestRegNameAgent() {
        STV_LIST_SBYTE_0		= new List<SByte>();
        TV_LIST_KEMPLOYEE_0		= new List<kEmployee>();
        STV_LIST_KEMPLOYEE_0	= new List<kEmployee>();
    }

    public static void clearAllStaticMemberVariables() {
        ParTestRegNameAgent.STV_KEMPLOYEE_0.resetProperties();

        ParTestRegNameAgent.STV_LIST_SBYTE_0.Clear();
        //behaviac::vector<signed char>().swap(ParTestRegNameAgent::STV_LIST_SBYTE_0);

        ParTestRegNameAgent.STV_LIST_KEMPLOYEE_0.Clear();
        //behaviac::vector<kEmployee>().swap(ParTestRegNameAgent::STV_LIST_KEMPLOYEE_0);
    }

    public virtual void resetProperties() {
        TV_CHAR_0 = '\0';
        TV_BYTE_0 = 0;
        TV_SBYTE_0 = 0;
        TV_STR_0 = String.Empty;
        TV_AGENT_0 = null;
        TV_KEMPLOYEE_0.resetProperties();
        STV_KEMPLOYEE_0.resetProperties();

        STV_LIST_SBYTE_0.Clear();
        TV_LIST_KEMPLOYEE_0.Clear();
        STV_LIST_KEMPLOYEE_0.Clear();
    }

    public void init() {
        Agent.RegisterInstanceName<ParTestRegNameAgent>();
        Agent.BindInstance(this);
    }

    public void finl() {
        Agent.UnbindInstance("ParTestRegNameAgent");
        Agent.UnRegisterInstanceName<ParTestRegNameAgent>(null);
    }

    #region Register Name IR Test
    [MethodMetaInfo("3 # PIR_Char", "A")]
    public Char Func_CharIR(Char par) {
        if (par == 'A')
        { return 'C'; }

        else
        { return 'D'; }
    }

    [MethodMetaInfo("3 # PIR_Byte", "A")]
    public Byte Func_ByteIR(Byte par) {
        Byte tv = (Byte)(par + 12);
        return tv;
    }

    [MethodMetaInfo("3 # PIR_SByte", "A")]
    public SByte Func_SByteIR(SByte par) {
        SByte tv = (SByte)(par - 5);
        return tv;
    }

    [MethodMetaInfo("3 # PIR_String", "A")]
    public String Func_StringIR(String par) {
        String tv = par + "extra";
        return tv;
    }

    [MethodMetaInfo("3 # PIR_Agent", "A")]
    public Agent Func_AgentIR(Agent par) {
        Agent tv = null;

        if (par == null)
        { tv = this; }

        else
        { tv = null; }

        return tv;
    }

    [MethodMetaInfo("3 # PIR_kEmployee", "A")]
    public kEmployee Func_kEmployeeIR(kEmployee par) {
        kEmployee tv;
        tv.id 		= par.id + 3;
        tv.name 	= par.name + "Jerry";

        if (par.code == 'C')
        { tv.code = 'Z'; }

        else
        { tv.code = 'V'; }

        tv.weight = par.weight + 20.2f;
        tv.isMale = !par.isMale;

        if (par.skinColor == eColor.WHITE)
        { tv.skinColor = eColor.RED; }

        else
        { tv.skinColor = eColor.BLUE; }

        if (par.boss == null)
        { tv.boss = this; }

        else
        { tv.boss = null; }

        tv.car.brand = par.car.brand + "Japan";

        if (par.car.color == eColor.WHITE)
        { tv.car.color = eColor.YELLOW; }

        else
        { tv.car.color = eColor.RED; }

        tv.car.price = par.car.price + 3000;
        return tv;
    }

    [MethodMetaInfo("3 # PIR_SByteList", "A")]
    public List<SByte> Func_SByteListIR(List<SByte> par) {
        List<SByte> tv = new List<SByte>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(-126);
        return tv;
    }

    [MethodMetaInfo("3 # PIR_kEmployeeList", "A")]
    public List<kEmployee> Func_kEmployeeListIR(List<kEmployee> par) {
        List<kEmployee> tv = new List<kEmployee>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        kEmployee jerry;
        jerry.id 			= 4;
        jerry.name		= "Jerry";
        jerry.code		= 'J';
        jerry.weight		= 60.0f;
        jerry.isMale		= false;
        jerry.skinColor	= eColor.WHITE;
        jerry.boss		= null;
        jerry.car.brand	= "Toyota";
        jerry.car.color	= eColor.YELLOW;
        jerry.car.price	= 43000;
        tv.Add(jerry);

        return tv;
    }
    #endregion
}
