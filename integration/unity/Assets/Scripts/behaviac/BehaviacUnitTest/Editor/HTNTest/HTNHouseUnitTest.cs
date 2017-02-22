#if !BEHAVIAC_NOT_USE_MONOBEHAVIOUR

using System;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

#if BEHAVIAC_USE_HTN
namespace BehaviorNodeUnitTest
{
    [TestFixture]
    [Category("HTNTest")]
    internal class HTNHouseUnitTest : HTNAgentBase_0
    {
        [Test]
        [Category("test_build_house")]
        public void test_build_house()
        {
            testAgent.btsetcurrent("node_test/htn/house/root");
            testAgent.resetProperties();

            //testAgent.Money = 200;
            testAgent.SetVariable("Money", 200);

            testAgent.btexec();

            int money = testAgent.GetVariable<int>("Money");
            Assert.AreEqual(100, money);

            bool land = testAgent.GetVariable<bool>("Land");
            Assert.AreEqual(true, land);

            bool goodCredit = testAgent.GetVariable<bool>("GoodCredit");
            Assert.AreEqual(true, goodCredit);

            bool mortgage = testAgent.GetVariable<bool>("Mortgage");
            Assert.AreEqual(true, mortgage);

            bool permit = testAgent.GetVariable<bool>("Permit");
            Assert.AreEqual(true, permit);

            bool contract = testAgent.GetVariable<bool>("Contract");
            Assert.AreEqual(false, contract);

            bool house = testAgent.GetVariable<bool>("House");
            Assert.AreEqual(true, house);
        }
    }
}
#endif//BEHAVIAC_USE_HTN

#endif
