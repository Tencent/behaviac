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
    internal class UnitTestBase_0
    {
        //protected const int kLoopCount = 30000;
        protected const int kLoopCount = 3000;
        protected const int kLoopMed = kLoopCount / 3;
        protected const int kLoopMed2 = kLoopMed * 3 / 2;
        protected const int kLoopBias = kLoopMed / 10;

        public AgentNodeTest testAgent = null;
        public ChildNodeTest testChildAgent = null;
        public GameObject testAgentObject = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;

            testAgent = testAgentObject.AddComponent<AgentNodeTest>();
            testAgent.init();


            testChildAgent = testAgentObject.AddComponent<ChildNodeTestSub>();
            testChildAgent.init();

            //Debug.Log("InitTestFixture");
        }
       
        [TestFixtureTearDown]
        public void finlGlobalTestEnv() {
            testAgent.finl();
            testChildAgent.finl();

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
    }
}

#endif
