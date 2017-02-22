using System.Collections.Generic;

namespace TestNS
{
    [behaviac.TypeMetaInfo("ArrayAccessTest_Agent", "ArrayAccessTest Agent Desc")]
    public class AgentArrayAccessTest : behaviac.Agent
    {
        public void resetProperties()
        {
            ListInts = new List<int> { 1, 2, 3, 4, 5 };
        }

        public void init()
        {
            base.Init();

            resetProperties();
        }

        public void finl()
        {
        }

        [behaviac.MemberMetaInfo()]
        public List<int> ListInts = new List<int> { 1, 2, 3, 4, 5 };

        [behaviac.MemberMetaInfo()]
        public int Int = 0;
    }
}