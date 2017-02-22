using System;
using System.Collections;
using System.Collections.Generic;

[behaviac.TypeMetaInfo()]
public class PreconEffectorAgent : behaviac.Agent
{
    public void resetProperties() {
        this.count_success = 0;
        this.count_failure = 0;
        this.count_both = 0;
        this.ret = 0;
    }

    public void init() {
        base.Init();

        resetProperties();
    }

    public void finl() {
    }

    [behaviac.MemberMetaInfo("", "")]
    public int count_success = 0;

    public int get_count_success() {
        return count_success;
    }

    [behaviac.MemberMetaInfo("", "")]
    public int count_failure = 0;

    [behaviac.MemberMetaInfo("", "")]
    public int count_both = 0;

    [behaviac.MemberMetaInfo("", "")]
    public int ret = 0;

    [behaviac.MethodMetaInfo()]
    public int fn_return() {
        return 5;
    }

    [behaviac.MethodMetaInfo()]
    public void action() {
    }
}
