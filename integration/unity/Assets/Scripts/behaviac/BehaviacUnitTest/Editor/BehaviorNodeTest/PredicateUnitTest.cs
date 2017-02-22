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
    [Category("PredicateUnitTest")]
    internal class PredicateUnitTest
    {
        AgentNodeTest testAgent = null;

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
        }

        [TestFixtureTearDown]
        public void finlGlobalTestEnv() {
            testAgent.finl();
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
        [Category("test_predicate_selector_0")]
        public void test_predicate_selector_0() {
            testAgent.btsetcurrent("node_test/predicate_selector_ut_0");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_predicate_selector_1")]
        public void test_predicate_selector_1() {
            testAgent.btsetcurrent("node_test/predicate_selector_ut_1");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_predicate_selector_2")]
        public void test_predicate_selector_2() {
            testAgent.btsetcurrent("node_test/predicate_selector_ut_2");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(0, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_selector_3")]
        public void test_predicate_selector_3() {
            testAgent.btsetcurrent("node_test/predicate_selector_ut_3");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(-1, testAgent.testVar_1);
            Assert.AreEqual(-1.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_selector_4")]
        public void test_predicate_selector_4() {
            testAgent.btsetcurrent("node_test/predicate_selector_ut_4");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(-1.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_selector_5")]
        public void test_predicate_selector_5() {
            testAgent.btsetcurrent("node_test/predicate_selector_ut_5");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(0.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_sequence_0")]
        public void test_predicate_sequence_0() {
            testAgent.btsetcurrent("node_test/predicate_sequence_ut_0");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_predicate_sequence_1")]
        public void test_predicate_sequence_1() {
            testAgent.btsetcurrent("node_test/predicate_sequence_ut_1");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_predicate_sequence_2")]
        public void test_predicate_sequence_2() {
            testAgent.btsetcurrent("node_test/predicate_sequence_ut_2");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(0, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_sequence_3")]
        public void test_predicate_sequence_3() {
            testAgent.btsetcurrent("node_test/predicate_sequence_ut_3");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(-1, testAgent.testVar_1);
            Assert.AreEqual(-1.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_sequence_4")]
        public void test_predicate_sequence_4() {
            testAgent.btsetcurrent("node_test/predicate_sequence_ut_4");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(-1.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_sequence_5")]
        public void test_predicate_sequence_5() {
            testAgent.btsetcurrent("node_test/predicate_sequence_ut_5");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(0.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_predicate_action_0")]
        public void test_predicate_action_0() {
            testAgent.btsetcurrent("node_test/predicate_action_ut_0");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(-1, testAgent.testVar_1);
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
        }

        [Test]
        [Category("test_predicate_action_1")]
        public void test_predicate_action_1() {
            testAgent.btsetcurrent("node_test/predicate_action_ut_1");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
        }

        [Test]
        [Category("test_predicate_action_2")]
        public void test_predicate_action_2() {
            testAgent.btsetcurrent("node_test/predicate_action_ut_2");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);

            //testAgent.resetProperties();
            status = testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
        }

        [Test]
        [Category("test_predicate_action_3")]
        public void test_predicate_action_3() {
            testAgent.btsetcurrent("node_test/predicate_action_ut_3");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            status = testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
        }
    }
}

#endif
