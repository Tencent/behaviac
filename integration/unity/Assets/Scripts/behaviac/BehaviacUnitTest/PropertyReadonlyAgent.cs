using System;
using System.Collections;

[behaviac.TypeMetaInfo()]
public class PropertyReadonlyAgent : behaviac.Agent
{
    public void resetProperties() {
    }

    public void init() {
        base.Init();

        resetProperties();
    }

    public void finl() {
    }

    private int _int_member = 0;

    [behaviac.MemberMetaInfo("", "")]
    public int PropertyGetterSetter {
        get
        {
            return _int_member;
        }
        set
        {
            _int_member = value;
        }
    }

    private int _int_property_getteronly = 1;

    [behaviac.MemberMetaInfo()]
    public int PropertyGetterOnly {
        get
        {
            return _int_property_getteronly;
        }
    }

    [behaviac.MemberMetaInfo()]
    public readonly int MemberReadonly = 2;

    [behaviac.MemberMetaInfo(true)]
    public int MemberReadonlyAs = 3;

    [behaviac.MethodMetaInfo()]
    public void FnWithOutParam(out int param) {
        param = 4;
    }

    [behaviac.MethodMetaInfo()]
    public void PassInProperty(int param) {
        MemberReadonlyAs = param;
    }

}
