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
using System.Collections.Generic;
using System;
using TNS.NE.NAT;

//TV_***, Test Variable
//STV_***, Static Test Variable
//PIR_***, Par, Input, Return
//UPR_***, Unittest Property Reference
 
[behaviac.TypeMetaInfo("ParTestAgentBase", "测试Agent的最底层基类")]
public class ParTestAgentBase : behaviac.Agent
{
    [behaviac.MemberMetaInfo("0 # TV_ECOLOR_0", "A")]
    public eColor TV_ECOLOR_0 	= eColor.WHITE;

    [behaviac.MemberMetaInfo("0 # STV_ECOLOR_0", "A")]
    public static eColor STV_ECOLOR_0 	= eColor.WHITE;

    [behaviac.MemberMetaInfo("0 # TV_BOOL_0", "A")]
    public Boolean TV_BOOL_0	= false;

    [behaviac.MemberMetaInfo("0 # STV_BOOL_0", "A")]
    public static Boolean STV_BOOL_0 = false;

    [behaviac.MemberMetaInfo("0 # TV_CHAR_0", "A")]
    public Char TV_CHAR_0		= '\0';

    [behaviac.MemberMetaInfo("0 # STV_CHAR_0", "A")]
    public static Char STV_CHAR_0		= '\0';

    [behaviac.MemberMetaInfo("0 # TV_BYTE_0", "A")]
    public Byte TV_BYTE_0		= 0;

    [behaviac.MemberMetaInfo("0 # TV_SBYTE_0", "A")]
    public SByte TV_SBYTE_0		= 0;

    [behaviac.MemberMetaInfo("0 # TV_LIST_ECOLOR_0", "A")]
    public List<eColor> TV_LIST_ECOLOR_0;

    [behaviac.MemberMetaInfo("0 # STV_LIST_ECOLOR_0", "A")]
    public static List<eColor> STV_LIST_ECOLOR_0;

    [behaviac.MemberMetaInfo("0 # TV_LIST_BOOL_0", "A")]
    public List<Boolean> TV_LIST_BOOL_0;

    [behaviac.MemberMetaInfo("0 # STV_LIST_BOOL_0", "A")]
    public static List<Boolean> STV_LIST_BOOL_0;

    [behaviac.MemberMetaInfo("0 # TV_LIST_CHAR_0", "A")]
    public List<Char> TV_LIST_CHAR_0;

    [behaviac.MemberMetaInfo("0 # STV_LIST_CHAR_0", "A")]
    public static List<Char> STV_LIST_CHAR_0;

    [behaviac.MemberMetaInfo("0 # TV_LIST_BYTE_0", "A")]
    public List<Byte> TV_LIST_BYTE_0;

    [behaviac.MemberMetaInfo("0 # TV_LIST_SBYTE_0", "A")]
    public List<SByte> TV_LIST_SBYTE_0;

    [behaviac.MemberMetaInfo("0 # STV_LIST_SBYTE_0", "A")]
    public static List<SByte> STV_LIST_SBYTE_0;

    public ParTestAgentBase() {
        TV_LIST_ECOLOR_0	= new List<eColor>();
        STV_LIST_ECOLOR_0	= new List<eColor>();
        TV_LIST_BOOL_0 		= new List<Boolean>();
        STV_LIST_BOOL_0 	= new List<Boolean>();
        TV_LIST_CHAR_0 		= new List<Char>();
        STV_LIST_CHAR_0 	= new List<Char>();
        TV_LIST_BYTE_0 		= new List<Byte>();
        TV_LIST_SBYTE_0 	= new List<SByte>();
        STV_LIST_SBYTE_0 	= new List<SByte>();
    }

    public virtual void resetProperties() {
        TV_ECOLOR_0			= eColor.WHITE;
        STV_ECOLOR_0		= eColor.WHITE;
        TV_BOOL_0 			= false;
        STV_BOOL_0 			= false;
        TV_CHAR_0			= '\0';
        STV_CHAR_0			= '\0';
        TV_BYTE_0			= 0;
        TV_SBYTE_0			= 0;

        TV_LIST_ECOLOR_0.Clear();
        STV_LIST_ECOLOR_0.Clear();
        TV_LIST_BOOL_0.Clear();
        STV_LIST_BOOL_0.Clear();
        TV_LIST_CHAR_0.Clear();
        STV_LIST_CHAR_0.Clear();
        TV_LIST_BYTE_0.Clear();
        TV_LIST_SBYTE_0.Clear();
        STV_LIST_SBYTE_0.Clear();
    }

    #region Par Ref Test
    [behaviac.MethodMetaInfo("0 # UPR_eColor", "A")]
    public void Func_eColorRef(ref eColor par) {
        par = eColor.BLUE;
    }

    [behaviac.MethodMetaInfo("0 # UPR_Boolean", "A")]
    public void Func_BooleanRef(ref Boolean par) {
        par = true;
    }

    [behaviac.MethodMetaInfo("0 # UPR_Char", "A")]
    public void Func_CharRef(ref Char par) {
        par = 'X';
    }

    [behaviac.MethodMetaInfo("0 # UPR_Byte", "A")]
    public void Func_ByteRef(ref Byte par) {
        par = 2;
    }

    [behaviac.MethodMetaInfo("0 # UPR_SByte", "A")]
    public void Func_SByteRef(ref SByte par) {
        par = -2;
    }

    [behaviac.MethodMetaInfo("0 # UPR_eColorList", "A")]
    public void Func_eColorListRef(ref List<eColor> par) {
        par.Add(eColor.RED);
    }

    [behaviac.MethodMetaInfo("0 # UPR_BooleanList", "A")]
    public void Func_BooleanListRef(ref List<Boolean> par) {
        par.Add(true);
    }

    [behaviac.MethodMetaInfo("0 # UPR_CharList", "A")]
    public void Func_CharListRef(ref List<Char> par) {
        par.Add('k');
    }

    [behaviac.MethodMetaInfo("0 # UPR_ByteList", "A")]
    public void Func_ByteListRef(ref List<Byte> par) {
        par.Add(8);
    }

    [behaviac.MethodMetaInfo("0 # UPR_SByteList", "A")]
    public void Func_SByteListRef(ref List<SByte> par) {
        par.Add(-4);
    }
    #endregion

    #region Par IR Test
    [behaviac.MethodMetaInfo("0 # PIR_eColor", "A")]
    public eColor Func_eColorIR(eColor par) {
        if (par == eColor.WHITE)
        { return eColor.BLUE; }

        else
        { return eColor.RED; }
    }

    [behaviac.MethodMetaInfo("0 # PIR_Boolean", "A")]
    public Boolean Func_BooleanIR(Boolean par) {
        Boolean tv = !par;
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # PIR_Char", "A")]
    public Char Func_CharIR(Char par) {
        if (par == 'A')
        { return 'C'; }

        else
        { return 'D'; }
    }

    [behaviac.MethodMetaInfo("0 # PIR_Byte", "A")]
    public Byte Func_ByteIR(Byte par) {
        Byte tv = (Byte)(par + 12);
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # PIR_SByte", "A")]
    public SByte Func_SByteIR(SByte par) {
        SByte tv = (SByte)(par - 5);
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # PIR_eColorList", "A")]
    public List<eColor> Func_eColorListIR(List<eColor> par) {
        List<eColor> tv = new List<eColor>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(eColor.YELLOW);
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # PIR_BooleanList", "A")]
    public List<Boolean> Func_BooleanListIR(List<Boolean> par) {
        List<Boolean> tv = new List<Boolean>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(false);
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # PIR_CharList", "A")]
    public List<Char> Func_CharListIR(List<Char> par) {
        List<Char> tv = new List<Char>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add('m');
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # PIR_ByteList", "A")]
    public List<Byte> Func_ByteListIR(List<Byte> par) {
        List<Byte> tv = new List<Byte>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(126);
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # PIR_SByteList", "A")]
    public List<SByte> Func_SByteListIR(List<SByte> par) {
        List<SByte> tv = new List<SByte>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(-126);
        return tv;
    }
    #endregion

    #region Static Member Function Test
    [behaviac.MethodMetaInfo("0 # SMF_Char", "A")]
    static public Char Func_CharSMF(Char par) {
        if (par == 'A')
        { return 'C'; }

        else
        { return 'D'; }
    }

    [behaviac.MethodMetaInfo("0 # SMF_Byte", "A")]
    static public Byte Func_ByteSMF(Byte par) {
        Byte tv = (Byte)(par + 12);
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # SMF_SByte", "A")]
    static public SByte Func_SByteSMF(SByte par) {
        SByte tv = (SByte)(par - 5);
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # SMF_CharList", "A")]
    static public List<Char> Func_CharListSMF(List<Char> par) {
        List<Char> tv = new List<Char>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add('m');
        return tv;
    }

    [behaviac.MethodMetaInfo("0 # SMF_SByteList", "A")]
    static public List<SByte> Func_SByteListSMF(List<SByte> par) {
        List<SByte> tv = new List<SByte>();

        for (int i = 0; i < par.Count; ++i) {
            tv.Add(par[i]);
        }

        tv.Add(-126);
        return tv;
    }
    #endregion
}