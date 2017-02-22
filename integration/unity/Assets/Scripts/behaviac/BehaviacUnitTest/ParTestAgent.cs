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
using System;
using System.Collections.Generic;
using TNS.ST.PER.WRK;
using TNS.NE.NAT;

[behaviac.TypeMetaInfo()]
public class ParTestAgent : ParTestAgentBase
{
    [behaviac.MemberMetaInfo("1 # TV_SHORT_0", "A")]
    public short TV_SHORT_0		= 0;

    [behaviac.MemberMetaInfo("1 # TV_INT_0", "A")]
    public int TV_INT_0		= 0;

    [behaviac.MemberMetaInfo("1 # STV_INT_0", "A")]
    public static int STV_INT_0 = 0;

    [behaviac.MemberMetaInfo("1 # TV_LONG_0", "A")]
    public long TV_LONG_0		= 0L;
    
    [behaviac.MemberMetaInfo("1 # TV_USHORT_0", "A")]
    public ushort TV_USHORT_0		= 0;

    [behaviac.MemberMetaInfo("1 # TV_KEMPLOYEE_0", "A")]
    public kEmployee TV_KEMPLOYEE_0;

    [behaviac.MemberMetaInfo("1 # STV_KEMPLOYEE_0", "A")]
    public static kEmployee STV_KEMPLOYEE_0;

    [behaviac.MemberMetaInfo("1 # TV_LIST_INT_0", "A")]
    public List<int> TV_LIST_INT_0;

    [behaviac.MemberMetaInfo("1 # STV_LIST_INT_0", "A")]
    public static List<int> STV_LIST_INT_0;

    [behaviac.MemberMetaInfo("1 # TV_LIST_KEMPLOYEE_0", "A")]
    public List<kEmployee> TV_LIST_KEMPLOYEE_0;

    [behaviac.MemberMetaInfo("1 # STV_LIST_KEMPLOYEE_0", "A")]
    public static List<kEmployee> STV_LIST_KEMPLOYEE_0;

    public ParTestAgent() {
        TV_KEMPLOYEE_0.resetProperties();

        TV_LIST_INT_0 			= new List<int>();
        STV_LIST_INT_0 			= new List<int>();
        TV_LIST_KEMPLOYEE_0 	= new List<kEmployee>();
        STV_LIST_KEMPLOYEE_0 	= new List<kEmployee>();
    }

    public override void resetProperties() {
        base.resetProperties();

        TV_SHORT_0	= 0;
        TV_INT_0	= 0;
        STV_INT_0	= 0;
        TV_LONG_0	= 0L;
        TV_USHORT_0	= 0;

        TV_KEMPLOYEE_0.resetProperties();
        STV_KEMPLOYEE_0.resetProperties();

        TV_LIST_INT_0.Clear();
        STV_LIST_INT_0.Clear();
        TV_LIST_KEMPLOYEE_0.Clear();
        STV_LIST_KEMPLOYEE_0.Clear();
    }

    public void initAgent() {
        this.Init();

        resetProperties();
    }

    public void finlAgent() {
    }

    #region Par Ref Test
    [behaviac.MethodMetaInfo("1 # UPR_Short", "A")]
    public void Func_ShortRef(ref short par) {
        par = 1;
    }

    [behaviac.MethodMetaInfo("1 # UPR_Int", "A")]
    public void Func_IntRef(ref int par) {
        par = 2;
    }

    [behaviac.MethodMetaInfo("1 # UPR_Long", "A")]
    public void Func_LongRef(ref long par) {
        par = 3L;
    }

    [behaviac.MethodMetaInfo("1 # UPR_UShort", "A")]
    public void Func_UShortRef(ref ushort par) {
        par = 4;
    }

    [behaviac.MethodMetaInfo("1 # UPR_kEmployee", "A")]
    public void Func_kEmployeeRef(ref kEmployee par) {
        par.id 			= 3;
        par.name		= "Tom";
        par.code		= 'X';
        par.weight		= 58.7f;
        par.isMale		= true;
        par.skinColor	= eColor.GREEN;
        par.boss		= this;
        par.car.brand	= "Honda";
        par.car.color	= eColor.RED;
        par.car.price	= 23000;
    }

    [behaviac.MethodMetaInfo("1 # UPR_IntList", "A")]
    public void Func_IntListRef(ref List<int> par) {
        par.Add(5);
    }

    [behaviac.MethodMetaInfo("1 # UPR_kEmployeeList", "A")]
    public void Func_kEmployeeListRef(ref List<kEmployee> par) {
        kEmployee tom;
        tom.id 			= 3;
        tom.name		= "Tom";
        tom.code		= 'X';
        tom.weight		= 58.7f;
        tom.isMale		= true;
        tom.skinColor	= eColor.GREEN;
        tom.boss		= this;
        tom.car.brand	= "Honda";
        tom.car.color	= eColor.RED;
        tom.car.price	= 23000;
        par.Add(tom);
    }
    #endregion

    #region Par IR Test
    [behaviac.MethodMetaInfo("1 # PIR_Short", "A")]
    public short Func_ShortIR(short par) {
        short tv = (short)(par + 250);
        return tv;
    }

    [behaviac.MethodMetaInfo("1 # PIR_Int", "A")]
    public int Func_IntIR(int par) {
        int tv = par + 350;
        return tv;
    }

    [behaviac.MethodMetaInfo("1 # PIR_Long", "A")]
    public long Func_LongIR(long par) {
        long tv = par + 450L;
        return tv;
    }

    [behaviac.MethodMetaInfo("1 # PIR_UShort", "A")]
    public ushort Func_UShortIR(ushort par) {
        ushort tv = (ushort)(par + 550);
        return tv;
    }

    [behaviac.MethodMetaInfo("1 # PIR_kEmployee", "A")]
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

    [behaviac.MethodMetaInfo("1 # PIR_IntList", "A")]
    public List<int> Func_IntListIR(List<int> par) {
        List<int> tv = new List<int>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(235);
        return tv;
    }

    [behaviac.MethodMetaInfo("1 # PIR_kEmployeeList", "A")]
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

    #region Static Member Function Test
    [behaviac.MethodMetaInfo("1 # SMF_kEmployee", "A")]
    static public kEmployee Func_kEmployeeSMF(kEmployee par) {
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

        //		if(par.boss == null)
        //			tv.boss = this;
        //		else
        //			tv.boss = null;
        tv.boss = null;

        tv.car.brand = par.car.brand + "Japan";

        if (par.car.color == eColor.WHITE)
        { tv.car.color = eColor.YELLOW; }

        else
        { tv.car.color = eColor.RED; }

        tv.car.price = par.car.price + 3000;
        return tv;
    }

    [behaviac.MethodMetaInfo("1 # SMF_kEmployeeList", "A")]
    static public List<kEmployee> Func_kEmployeeListSMF(List<kEmployee> par) {
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