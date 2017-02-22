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

using UnityEngine;
using System.Collections;
using NUnit.Framework;

namespace BehaviorNodeUnitTest
{
    [TestFixture]
    [Category("EventUnitTest")]
    internal class EventUnitTest
    {
        AgentNodeTest testAgent = null;
        ChildNodeTest testChild = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testAgent = testAgentObject.AddComponent<AgentNodeTest>();
            testAgent.init();

            testChild = testAgentObject.AddComponent<ChildNodeTest>();
            testChild.init();
        }

        [TestFixtureTearDown]
        public void finlGlobalTestEnv() {
            testAgent.finl();
            testChild.finl();
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
        [Category("test_event_0")]
        public void test_event_0() {
            testAgent.btsetcurrent("node_test/event_ut_0");
            testAgent.resetProperties();

            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.FireEvent("event_test_void");
            Assert.AreEqual(true, testAgent.event_test_var_bool);

            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/event_ut_0");
            testAgent.btexec();

            behaviac.Workspace.Instance.DoubleValueSinceStartup = 0;
            testAgent.FireEvent("event_test_int", 13);
            Assert.AreEqual(13, testAgent.event_test_var_int);

            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            behaviac.Workspace.Instance.DoubleValueSinceStartup = 5.1 * 1000;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/event_ut_0");
            testAgent.btexec();
            testAgent.FireEvent("event_test_int_bool", 15, true);
            Assert.AreEqual(true, testAgent.event_test_var_bool);
            Assert.AreEqual(15, testAgent.event_test_var_int);

            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/event_ut_0");
            testAgent.btexec();
            testAgent.FireEvent("event_test_int_bool_float", 15, true, 27.3f);
            Assert.AreEqual(true, testAgent.event_test_var_bool);
            Assert.AreEqual(15, testAgent.event_test_var_int);
            Assert.AreEqual(27.3f, testAgent.event_test_var_float);

            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/event_ut_0");
            testAgent.btexec();
            testAgent.testVar_0 = 0;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(0, testAgent.testVar_1);

            testAgent.FireEvent("event_test_int_bool_float", 19, true, 11.9f);
            Assert.AreEqual(false, testAgent.event_test_var_bool);
            Assert.AreEqual(-1, testAgent.event_test_var_int);
            Assert.AreEqual(-1.0f, testAgent.event_test_var_float);

            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/event_ut_0");
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            Assert.AreEqual(null, testAgent.event_test_var_agent);
            testAgent.FireEvent("event_test_agent", testAgent);
            Assert.AreNotEqual(null, testAgent.event_test_var_agent);

            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/event_ut_0");
            testAgent.btresetcurrrent();
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            Assert.AreEqual(null, testAgent.event_test_var_agent);
            testAgent.FireEvent("event_test_agent", testChild);
            Assert.AreNotEqual(null, testAgent.event_test_var_agent);
        }

        [Test]
        [Category("test_event_1")]
        public void test_event_1()
        {
            testAgent.btsetcurrent("node_test/event_ut_1");
            testAgent.resetProperties();

            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.FireEvent("event_test_int", 13);
            Assert.AreEqual(13, testAgent.event_test_var_int);

            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
        }

        [Test]
        [Category("test_event_2")]
        public void test_event_2()
        {
            testChild.resetProperties();

            const string treePath = "node_test/event_ut_2";
            testChild.btload(treePath);
            testChild.btsetcurrent(treePath);

            behaviac.EBTStatus status = testChild.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testChild.FireEvent("event_test_int_bool", 15, true);
            Assert.AreEqual(true, testChild.event_test_var_bool);
            Assert.AreEqual(15, testChild.event_test_var_int);

            status = testChild.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
        }

        [Test]
        [Category("test_event_3")]
        public void test_event_3()
        {
            testAgent.btsetcurrent("node_test/event_ut_3");
            testAgent.resetProperties();

            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(1, testAgent.testVar_0);
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(1, testAgent.testVar_0);
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(1, testAgent.testVar_0);

            testAgent.FireEvent("event_test_void");

            Assert.AreEqual(true, testAgent.event_test_var_bool);

            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
        }


        [Test]
        [Category("test_local_vars")]
        public void test_local_vars() {
            testAgent.btsetcurrent("node_test/event_ut_1");
            testAgent.resetProperties();

            testAgent.btexec();
            testAgent.FireEvent("event_test_int_bool", 15, true);
            Assert.AreEqual(true, testAgent.event_test_var_bool);
            Assert.AreEqual(15, testAgent.event_test_var_int);
        }
    }
}

#endif
