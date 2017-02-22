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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace BehaviorNodeUnitTest
{
    [TestFixture]
    [Category("ArrayAccessTest")]
    internal class ArrayAccessTest
    {
        public TestNS.AgentArrayAccessTest testAgent = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testAgent = testAgentObject.AddComponent<TestNS.AgentArrayAccessTest>();
            testAgent.init();

            //Debug.Log("InitTestFixture");
        }

        [TestFixtureTearDown]
        public void finlGlobalTestEnv() {
            testAgent.finl();

            BehaviacSystem.Instance.Uninit();
            //Debug.Log("FinlTestFixture");
        }

        [SetUp]
        public void initTestEnv() {
        }

        [TearDown]
        public void finlTestEnv() {
            testAgent.btunloadall();
        }

        [Test]
        [Category("test_vector_accessor")]
        public void test_vector_accessor() {
            testAgent.btsetcurrent("par_test/vector_test");
            testAgent.resetProperties();
            testAgent.btexec();

            int Int1 = testAgent.GetVariable<int>("Int");
            Assert.AreEqual(1, Int1);

            int Int2 = testAgent.Int;
            Assert.AreEqual(1, Int2);

            int c_Int = testAgent.GetVariable<int>("c_Int");
            Assert.AreEqual(10, c_Int);

            int Int0 = testAgent.ListInts[0];
            Assert.AreEqual(110, Int0);

            int c_Count = testAgent.GetVariable<int>("c_Count");
            Assert.AreEqual(5, c_Count);

            List<int> c_ListInts = testAgent.GetVariable<List<int>>("c_ListInts");
            Assert.AreEqual(5, c_ListInts.Count);
            Assert.AreEqual(20, c_ListInts[0]);

            List<double> c_douleVec = testAgent.GetVariable<List<double>>("c_douleVec");
            Assert.AreEqual(103, c_douleVec.Count);
            for (int i = 0; i < 100; ++i)
            {
                Assert.AreEqual(c_douleVec[3 + i], 0.03);
            }

            List<double> c_douleVec2 = testAgent.GetVariable<List<double>>("c_doubleVec2");
            Assert.AreEqual(103, c_douleVec2.Count);
            for (int i = 0; i < 100; ++i)
            {
                Assert.AreEqual(c_douleVec2[3 + i], 0.05);
            }
        }
    }
}

#endif
