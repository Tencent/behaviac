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
    [Category("CustomPropertyUnitTest")]
    internal class CustomPropertyUnitTest
    {
        public CustomPropertyAgent testAgent = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testAgent = testAgentObject.AddComponent<CustomPropertyAgent>();
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
        [Category("test_custom_property")]
        public void test_custom_property() {
            testAgent.btsetcurrent("par_test/custom_property_as_left_value_and_param");
            testAgent.resetProperties();
            testAgent.btexec();

            bool c_Bool = testAgent.GetVariable<bool>("c_Bool");
            Assert.AreEqual(true, c_Bool);
            List<int> intArray = testAgent.GetVariable<List<int>>("c_IntArray");
            Assert.AreEqual(2, intArray.Count);
            Assert.AreEqual(1, intArray[0]);
            Assert.AreEqual(2, intArray[1]);

            string c_StaticString = testAgent.GetVariable<string>("c_StaticString");
            Assert.AreEqual("test string", c_StaticString);
            TNS.NE.NAT.eColor color = testAgent.GetVariable<TNS.NE.NAT.eColor>("c_Enum");
            Assert.AreEqual(TNS.NE.NAT.eColor.GREEN, color);

            int l_Int = testAgent.GetVariable<int>("l_Int");
            Assert.AreEqual(2, l_Int);

            //although c_Location = Location, it is different copies
            Vector3 Location = testAgent.Location;
            Assert.AreEqual(2.0, Location.x, 0.001f);
            Assert.AreEqual(2.0, Location.y, 0.001f);
            Assert.AreEqual(2.0, Location.z, 0.001f);

            Vector3 c_Location = testAgent.GetVariable<Vector3>("c_Location");
            Assert.AreEqual(1.0, c_Location.x, 0.001f);
            Assert.AreEqual(1.0, c_Location.y, 0.001f);
            Assert.AreEqual(1.0, c_Location.z, 0.001f);
        }

        [Test]
        [Category("test_vector3_unique")]
        public void test_vector3_unique() {
            testAgent.btsetcurrent("par_test/custom_property_as_left_value_and_param");
            testAgent.resetProperties();
            testAgent.btexec();

            //although c_Location = Location, it is different copies
            Vector3 Location = testAgent.Location;
            Assert.AreEqual(2.0, Location.x, 0.001f);
            Assert.AreEqual(2.0, Location.y, 0.001f);
            Assert.AreEqual(2.0, Location.z, 0.001f);

            Vector3 c_Location = testAgent.GetVariable<Vector3>("c_Location");
            Assert.AreEqual(1.0, c_Location.x, 0.001f);
            Assert.AreEqual(1.0, c_Location.y, 0.001f);
            Assert.AreEqual(1.0, c_Location.z, 0.001f);

            GameObject testAgentObject2 = new GameObject();
            testAgentObject2.name = "@UnitTestAgent2";
            testAgentObject2.transform.localPosition = Vector3.zero;
            testAgentObject2.transform.localRotation = Quaternion.identity;
            testAgentObject2.transform.localScale = Vector3.one;
            CustomPropertyAgent testAgent2 = testAgentObject2.AddComponent<CustomPropertyAgent>();
            testAgent2.init();

            testAgent2.btsetcurrent("par_test/custom_property_as_left_value_and_param");
            testAgent2.resetProperties();
            Assert.AreEqual(0.0, testAgent2.Location.x, 0.001f);
            Assert.AreEqual(0.0, testAgent2.Location.y, 0.001f);
            Assert.AreEqual(0.0, testAgent2.Location.z, 0.001f);

            Vector3 c_Location20 = testAgent2.GetVariable<Vector3>("c_Location");
            Assert.AreEqual(0.0, c_Location20.x, 0.001f);
            Assert.AreEqual(0.0, c_Location20.y, 0.001f);
            Assert.AreEqual(0.0, c_Location20.z, 0.001f);

            testAgent2.btexec();

            Assert.AreEqual(2.0, testAgent2.Location.x, 0.001f);
            Assert.AreEqual(2.0, testAgent2.Location.y, 0.001f);
            Assert.AreEqual(2.0, testAgent2.Location.z, 0.001f);

            //although c_Location = Location, it is different copies
            Vector3 c_Location21 = testAgent2.GetVariable<Vector3>("c_Location");
            Assert.AreEqual(1.0, c_Location21.x, 0.001f);
            Assert.AreEqual(1.0, c_Location21.y, 0.001f);
            Assert.AreEqual(1.0, c_Location21.z, 0.001f);
        }

        [Test]
        [Category("local_out_scope")]
        public void local_out_scope() {
            testAgent.btsetcurrent("par_test/local_out_scope");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            bool c_Bool = testAgent.GetVariable<bool>("c_Bool");
            Assert.AreEqual(true, c_Bool);
            List<int> intArray = testAgent.GetVariable<List<int>>("c_IntArray") as List<int>;
            Assert.AreEqual(2, intArray.Count);
            Assert.AreEqual(1, intArray[0]);
            Assert.AreEqual(2, intArray[1]);

            string c_StaticString = testAgent.GetVariable<string>("c_StaticString");
            Assert.AreEqual("test string", c_StaticString);
            TNS.NE.NAT.eColor color = testAgent.GetVariable<TNS.NE.NAT.eColor>("c_Enum");
            Assert.AreEqual(TNS.NE.NAT.eColor.GREEN, color);

            //l_Int defined as local in custom_property_as_left_value_and_param, not defined in local_out_scope
            Assert.AreEqual(false, testAgent.IsValidVariable("l_Int"));

            List<int> l_IntArray = testAgent.GetVariable<List<int>>("l_IntArray") ;
            Assert.AreEqual(2, l_IntArray.Count);
            Assert.AreEqual(2, l_IntArray[0]);
            Assert.AreEqual(3, l_IntArray[1]);

            status = testAgent.btexec();

            //bt succeeded, so the l_IntArray is out of scope
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(false, testAgent.IsValidVariable("l_IntArray"));
        }
    }
}

#endif
