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

#if !BEHAVIAC_NOT_USE_MONOBEHAVIOUR

using System;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace BehaviorOtherUnitTest
{
    [TestFixture]
    [Category("OtherUnitTest")]
    internal class OtherUnitTest
    {
        EmployeeParTestAgent parTestAgent = null;
        AgentNodeTest nodeTestAgent = null;

        //		AgentNodeTest nodeTestAgent_0 = null;
        //		ParTestAgent parTestAgent_0 = null;
        //		ParTestAgent parTestAgent_1 = null;
        //		ParTestAgent parTestAgent_2 = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@Agent_0";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            parTestAgent = testAgentObject.AddComponent<EmployeeParTestAgent>();
            parTestAgent.init();

            testAgentObject = new GameObject();
            testAgentObject.name = "@NodeTestAgent_1";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            nodeTestAgent = testAgentObject.AddComponent<AgentNodeTest>();
            nodeTestAgent.init();

            //			testAgentObject = new GameObject();
            //			testAgentObject.name = "@NodeTestAgent_0";
            //			testAgentObject.transform.localPosition = Vector3.zero;
            //			testAgentObject.transform.localRotation = Quaternion.identity;
            //			testAgentObject.transform.localScale = Vector3.one;
            //			nodeTestAgent_0 = testAgentObject.AddComponent<AgentNodeTest>();
            //			nodeTestAgent_0.init();
            //
            //			testAgentObject = new GameObject();
            //			testAgentObject.name = "@ParTestAgent_0";
            //			testAgentObject.transform.localPosition = Vector3.zero;
            //			testAgentObject.transform.localRotation = Quaternion.identity;
            //			testAgentObject.transform.localScale = Vector3.one;
            //			parTestAgent_0 = testAgentObject.AddComponent<ParTestAgent>();
            //			parTestAgent_0.initAgent();
            //
            //			testAgentObject = new GameObject();
            //			testAgentObject.name = "@ParTestAgent_1";
            //			testAgentObject.transform.localPosition = Vector3.zero;
            //			testAgentObject.transform.localRotation = Quaternion.identity;
            //			testAgentObject.transform.localScale = Vector3.one;
            //			parTestAgent_1 = testAgentObject.AddComponent<ParTestAgent>();
            //			parTestAgent_1.initAgent();
            //
            //			testAgentObject = new GameObject();
            //			testAgentObject.name = "@ParTestAgent_2";
            //			testAgentObject.transform.localPosition = Vector3.zero;
            //			testAgentObject.transform.localRotation = Quaternion.identity;
            //			testAgentObject.transform.localScale = Vector3.one;
            //			parTestAgent_2 = testAgentObject.AddComponent<ParTestAgent>();
            //			parTestAgent_2.initAgent();
        }

        [TestFixtureTearDown]
        public void finlGlobalTestEnv() {
            nodeTestAgent.finl();
            parTestAgent.finl();

            //			nodeTestAgent_0.finl();
            //
            //			parTestAgent_2.finlAgent();
            //			parTestAgent_1.finlAgent();
            //			parTestAgent_0.finlAgent();
            BehaviacSystem.Instance.Uninit();
        }

        [SetUp]
        public void initTestEnv() {
        }

        [TearDown]
        public void finlTestEnv() {
            behaviac.Workspace.Instance.UnLoadAll();
        }

        [Test]
        [Category("test_agent_instance")]
        public void test_agent_instance() {
            behaviac.Agent.RegisterInstanceName<AgentNodeTest>();
            behaviac.Agent.RegisterInstanceName<behaviac.Agent>("Name_Agent_0");
            behaviac.Agent.RegisterInstanceName<behaviac.Agent>("Name_Agent_1");
            behaviac.Agent.RegisterInstanceName<behaviac.Agent>("Name_Agent_2");

            behaviac.Agent.BindInstance(parTestAgent, "Name_Agent_0");
            behaviac.Agent.BindInstance(nodeTestAgent, "AgentNodeTest");

            behaviac.Agent testAgent_0 = behaviac.Agent.GetInstance<behaviac.Agent>("Name_Agent_0");
            AgentNodeTest testAgent_1 = behaviac.Agent.GetInstance<AgentNodeTest>();

//#if !BEHAVIAC_RELEASE
//            AgentNodeTest testAgent_3 = behaviac.Agent.GetAgent("AgentNodeTest") as AgentNodeTest;
//            Assert.AreEqual(testAgent_1, testAgent_3);
//#endif

            Assert.AreEqual(testAgent_0, parTestAgent);
            Assert.AreEqual(testAgent_1, nodeTestAgent);
            Assert.NotNull(testAgent_0);
            Assert.NotNull(testAgent_1);

            behaviac.Agent.UnbindInstance("Name_Agent_0");
            behaviac.Agent.UnbindInstance("AgentNodeTest");

//#if !BEHAVIAC_RELEASE
//            testAgent_3 = behaviac.Agent.GetAgent("AgentNodeTest#") as AgentNodeTest;
//            Assert.AreEqual(nodeTestAgent, testAgent_3);
//#endif

            behaviac.Agent testAgent_0_0 = behaviac.Agent.GetInstance<behaviac.Agent>("Name_Agent_0");
            AgentNodeTest testAgent_1_0 = behaviac.Agent.GetInstance<AgentNodeTest>();

            Assert.Null(testAgent_0_0);
            Assert.Null(testAgent_1_0);

            behaviac.Agent.BindInstance(testAgent_0, "Name_Agent_1");
            behaviac.Agent testAgent_0_1 = behaviac.Agent.GetInstance<behaviac.Agent>("Name_Agent_1");
            Assert.NotNull(testAgent_0_1);

            behaviac.Agent.BindInstance(testAgent_0, "Name_Agent_2");
            behaviac.Agent testAgent_0_2 = behaviac.Agent.GetInstance<behaviac.Agent>("Name_Agent_2");
            Assert.NotNull(testAgent_0_2);

            Assert.AreEqual(testAgent_0_1, testAgent_0_2);

            behaviac.Agent.UnbindInstance("Name_Agent_1");
            behaviac.Agent.UnbindInstance("Name_Agent_2");

            behaviac.Agent.UnRegisterInstanceName<behaviac.Agent>("Name_Agent_2");
            behaviac.Agent.UnRegisterInstanceName<behaviac.Agent>("Name_Agent_1");
            behaviac.Agent.UnRegisterInstanceName<behaviac.Agent>("Name_Agent_0");
            behaviac.Agent.UnRegisterInstanceName<AgentNodeTest>();
        }
    }
}

#endif
