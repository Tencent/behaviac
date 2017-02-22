using System;
using System.Collections.Generic;
using System.Text;

[behaviac.TypeMetaInfo()]
class CBTPlayer : behaviac.Agent
{
    [behaviac.MemberMetaInfo()]
    public uint m_iBaseSpeed;

    [behaviac.MemberMetaInfo()]
    public int m_Frames;

    public CBTPlayer()
    {
        m_iBaseSpeed = 0;
        m_Frames = 0;
    }

    [behaviac.MethodMetaInfo()]
    public bool Condition()
    {
        Console.WriteLine("\tCondition");

        m_iBaseSpeed = 0;
        m_Frames = 0;
        return true;
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus Action1()
    {
        Console.WriteLine("\tAction1");

        return behaviac.EBTStatus.BT_SUCCESS;
    }

    [behaviac.MethodMetaInfo()]
    public behaviac.EBTStatus Action3()
    {
        Console.WriteLine("\tAction3");

        m_Frames++;

        if (m_Frames == 3)
        {
            return behaviac.EBTStatus.BT_SUCCESS;
        }

        return behaviac.EBTStatus.BT_RUNNING;
    }
}
