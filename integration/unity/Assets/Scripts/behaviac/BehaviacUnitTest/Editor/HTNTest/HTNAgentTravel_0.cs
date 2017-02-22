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
    internal class HTNAgentTravel_0
    {
        public HTNAgentTravel testAgent = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testAgent = testAgentObject.AddComponent<HTNAgentTravel>();
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
    }
}

#endif
