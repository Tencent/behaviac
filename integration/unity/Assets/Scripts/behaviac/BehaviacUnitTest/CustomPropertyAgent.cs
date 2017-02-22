using System.Collections;
using System.Collections.Generic;

namespace TestNamespace
{
    [behaviac.TypeMetaInfo("Float2", "Float2 structure")]
    public struct Float2
    {
        [behaviac.MemberMetaInfo("x", "x Axis")]
        public float x;
        [behaviac.MemberMetaInfo("y", "y Axis")]
        public float y;
    }

    [behaviac.TypeMetaInfo(behaviac.ERefType.ERT_ValueType)]
    public class ClassAsValueType
    {
        public float x;
        public float y;
    }

}


[behaviac.TypeMetaInfo()]
public class CustomPropertyAgent : behaviac.Agent
{
    public void resetProperties() {
        base.Init();
    }

    public void init() {
        resetProperties();
    }

    public void finl() {
    }

    public int _int_member = 0;

    [behaviac.MemberMetaInfo("", "")]
    public int IntProperty {
        get
        {
            return _int_member;
        }
        set
        {
            _int_member = value;
        }
    }

    public float _float_member = 0;

    [behaviac.MemberMetaInfo()]
    public float FloatPropertyReadonly {
        get
        {
            return _float_member;
        }
    }

    [behaviac.MemberMetaInfo()]
    public readonly bool BoolMemberReadonly = false;

    [behaviac.MemberMetaInfo()]
    public const int IntMemberConst = 0;

    [behaviac.MethodMetaInfo()]
    public void TestFn1(TestNamespace.Float2 v)
    {
    }

    [behaviac.MethodMetaInfo()]
    public void TestFn2(TestNamespace.ClassAsValueType v)
    {
    }

    public List<int> PIR_IntArray(List<int> arr)
    {
        return null;
    }

    [behaviac.MemberMetaInfo(true)]
    public string StringMemberReadonly = "read only sting";

    [behaviac.MemberMetaInfo()]
    public UnityEngine.Vector3 Location = new UnityEngine.Vector3();

    [behaviac.MethodMetaInfo()]
    public void FnWithOutParam(out int param) {
        param = 1;
    }
}
