#if !BEHAVIAC_NOT_USE_MONOBEHAVIOUR

using UnityEngine;
using System.Collections;
using NUnit.Framework;

namespace BehaviorNodeUnitTest
{
    internal class FSMAgentTest_0
    {
        public FSMAgentTest testAgent = null;
        public AgentNodeTest testBtAgent = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testAgent = testAgentObject.AddComponent<FSMAgentTest>();
            testAgent.init();

            testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestBtAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testBtAgent = testAgentObject.AddComponent<AgentNodeTest>();
            testBtAgent.init();

            //Debug.Log("InitTestFixture");
        }

        [TestFixtureTearDown]
        public void finlGlobalTestEnv() {
            testAgent.finl();
            testBtAgent.finl();

            BehaviacSystem.Instance.Uninit();
            //Debug.Log("FinlTestFixture");
        }

        [SetUp]
        public void initTestEnv() {
        }

        [TearDown]
        public void finlTestEnv() {
            testAgent.btunloadall();
            testBtAgent.btunloadall();
        }
    }
}

#endif
