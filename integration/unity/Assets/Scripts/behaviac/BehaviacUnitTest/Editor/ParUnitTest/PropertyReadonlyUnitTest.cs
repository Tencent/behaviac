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
    [Category("PropertyReadonlyUnitTest")]
    internal class PropertyReadonlyUnitTest
    {
        public PropertyReadonlyAgent testAgent = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testAgent = testAgentObject.AddComponent<PropertyReadonlyAgent>();
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
        [Category("readonly_default")]
        public void readonly_default() {
            testAgent.btsetcurrent("par_test/readonly_default");
            testAgent.resetProperties();

            Assert.AreEqual(1, testAgent.PropertyGetterOnly);
            Assert.AreEqual(2, testAgent.MemberReadonly);
            Assert.AreEqual(3, testAgent.MemberReadonlyAs);

            testAgent.btexec();

            int c_IntReadonly = testAgent.GetVariable<int>("c_IntReadonly");
            Assert.AreEqual(10, c_IntReadonly);

            // PropertyGetterSetter = MemberReadonly, while MemberReadonly = 2
            Assert.AreEqual(2, testAgent.PropertyGetterSetter);

            // PropertyGetterOnly is passed in as the param of PassInProperty
            Assert.AreEqual(1, testAgent.PropertyGetterOnly);

            // MemberReadonly is readonly, not changed
            Assert.AreEqual(2, testAgent.MemberReadonly);

            // MemberReadonlyAs is modified in PassInProperty and assigned to be PropertyGetterOnly
            Assert.AreEqual(1, testAgent.MemberReadonlyAs);

            // m_Int is as the ref param of FnWithOutParam and set to 4
            int c_Int = testAgent.GetVariable<int>("c_Int");
            Assert.AreEqual(4, c_Int);

            // c_ResultStatic = MemberReadonly + PropertyGetterOnly, 2 + 1 = 3
            int c_ResultStatic = testAgent.GetVariable<int>("c_ResultStatic");
            Assert.AreEqual(3, c_ResultStatic);
        }
    }
}

#endif
