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
using System.Collections.Generic;
using NUnit.Framework;

namespace BehaviorNodeUnitTest
{
    [TestFixture]
    [Category("EnterExitActionUnitTest")]
    internal class EnterExitActionUnitTest
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
        [Category("test_enter_exit_action_0")]
        public void test_enter_exit_action_0() {
            testAgent.btsetcurrent("node_test/enter_exit_action_ut_0");
            testAgent.resetProperties();

            behaviac.EBTStatus status = testAgent.btexec();
            //Assert.AreEqual(1, testAgent.action_0_enter_count);
            Assert.AreEqual(1, testAgent.action_1_enter_count);
            Assert.AreEqual(1, testAgent.action_2_enter_count);
            Assert.AreEqual("enter_test", testAgent.testVar_str_0);

            //Assert.AreEqual(0, testAgent.action_0_exit_count);
            Assert.AreEqual(0, testAgent.action_1_exit_count);
            Assert.AreEqual(0, testAgent.action_2_exit_count);

            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            int loopCount = 1000;

            while (loopCount > 0) {
                status = testAgent.btexec();
                //Assert.AreEqual(1, testAgent.action_0_enter_count);
                Assert.AreEqual(1, testAgent.action_1_enter_count);
                Assert.AreEqual(1, testAgent.action_2_enter_count);

                //Assert.AreEqual(0, testAgent.action_0_exit_count);
                Assert.AreEqual(0, testAgent.action_1_exit_count);
                Assert.AreEqual(0, testAgent.action_2_exit_count);

                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                --loopCount;
            }

            //
            testAgent.testVar_0 = 0;
            status = testAgent.btexec();
            //Assert.AreEqual(1, testAgent.action_0_enter_count);
            Assert.AreEqual(1, testAgent.action_1_enter_count);
            Assert.AreEqual(1, testAgent.action_2_enter_count);
            Assert.AreEqual("exit_test", testAgent.testVar_str_0);

            //Assert.AreEqual(1, testAgent.action_0_exit_count);
            Assert.AreEqual(1, testAgent.action_1_exit_count);
            Assert.AreEqual(1, testAgent.action_2_exit_count);

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);

            loopCount = 100;

            while (loopCount > 0) {
                status = testAgent.btexec();
                --loopCount;
            }

            //Assert.AreEqual(101, testAgent.action_0_enter_count);
            Assert.AreEqual(101, testAgent.action_1_enter_count);
            Assert.AreEqual(101, testAgent.action_2_enter_count);

            //Assert.AreEqual(101, testAgent.action_0_exit_count);
            Assert.AreEqual(101, testAgent.action_1_exit_count);
            Assert.AreEqual(101, testAgent.action_2_exit_count);

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
        }

        [Test]
        [Category("test_enter_exit_action_1")]
        public void test_enter_exit_action_1() {
            testAgent.btsetcurrent("node_test/enter_exit_action_ut_1");
            testAgent.resetProperties();

            behaviac.EBTStatus status = testAgent.btexec();
            //Assert.AreEqual(1, testAgent.action_0_enter_count);
            Assert.AreEqual(1, testAgent.action_1_enter_count);
            Assert.AreEqual(1, testAgent.action_2_enter_count);

            //Assert.AreEqual(0, testAgent.action_0_exit_count);
            Assert.AreEqual(0, testAgent.action_1_exit_count);
            Assert.AreEqual(0, testAgent.action_2_exit_count);

            Assert.AreEqual(3, testAgent.testVar_1);
            Assert.AreEqual("hello", testAgent.testVar_str_0);

            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.testVar_0 = 0;
            status = testAgent.btexec();
            //Assert.AreEqual(1, testAgent.action_0_enter_count);
            Assert.AreEqual(1, testAgent.action_1_enter_count);
            Assert.AreEqual(1, testAgent.action_2_enter_count);

            //Assert.AreEqual(1, testAgent.action_0_exit_count);
            Assert.AreEqual(1, testAgent.action_1_exit_count);
            Assert.AreEqual(1, testAgent.action_2_exit_count);

            Assert.AreEqual(5, testAgent.testVar_1);
            Assert.AreEqual("world", testAgent.testVar_str_0);

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
        }

        [Test]
        [Category("test_enter_exit_action_2")]
        public void test_enter_exit_action_2() {
            testAgent.btsetcurrent("node_test/enter_exit_action_ut_2");
            testAgent.resetProperties();

            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(1, testAgent.action_0_enter_count);
            Assert.AreEqual(1, testAgent.action_1_enter_count);
            Assert.AreEqual(1, testAgent.action_2_enter_count);

            Assert.AreEqual(1, testAgent.action_0_exit_count);
            Assert.AreEqual(0, testAgent.action_1_exit_count);
            Assert.AreEqual(0, testAgent.action_2_exit_count);

            Assert.AreEqual(3, testAgent.testVar_1);
            Assert.AreEqual("hello", testAgent.testVar_str_0);

            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.testVar_0 = 0;
            status = testAgent.btexec();
            Assert.AreEqual(2, testAgent.action_0_enter_count);
            Assert.AreEqual(1, testAgent.action_1_enter_count);
            Assert.AreEqual(1, testAgent.action_2_enter_count);

            Assert.AreEqual(2, testAgent.action_0_exit_count);
            Assert.AreEqual(1, testAgent.action_1_exit_count);
            Assert.AreEqual(1, testAgent.action_2_exit_count);

            Assert.AreEqual(5, testAgent.testVar_1);
            Assert.AreEqual("world", testAgent.testVar_str_0);

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
        }

        [Test]
        [Category("test_cstringid")]
        public void test_cstringid()
        {
            var list = new List<object>();
            list.Add(new behaviac.CStringID("aaa"));

            if (list.Contains("?"))
            {
                System.Console.WriteLine("");
            }
        }
    }
}

#endif
