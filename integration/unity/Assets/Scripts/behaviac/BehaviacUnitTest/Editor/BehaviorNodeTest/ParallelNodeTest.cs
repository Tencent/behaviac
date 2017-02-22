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
    [Category("ParallelNodeTest")]
    internal class ParallelNodeTest
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
        [Category("test_parallel_0")]
        public void test_parallel_0() {
            testAgent.btsetcurrent("node_test/parallel_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_parallel_1")]
        public void test_parallel_1() {
            testAgent.btsetcurrent("node_test/parallel_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(3, testAgent.testVar_0);
        }

        [Test]
        [Category("test_parallel_2")]
        public void test_parallel_2() {
            testAgent.btsetcurrent("node_test/parallel_ut_2");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_parallel_3")]
        public void test_parallel_3() {
            testAgent.btsetcurrent("node_test/parallel_ut_3");
            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(2, testAgent.testVar_0);

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_parallel_4")]
        public void test_parallel_4() {
            testAgent.btsetcurrent("node_test/parallel_ut_4");
            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(2, testAgent.testVar_0);

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(2, testAgent.testVar_0);
        }
    }
}

#endif
