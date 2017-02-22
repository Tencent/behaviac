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

#if BEHAVIAC_USE_HTN
namespace BehaviorNodeUnitTest
{
    [TestFixture]
    [Category("HTNTest")]
    internal class HTNTravelUnitTest : HTNAgentTravel_0
    {
        [Test]
        [Category("test_travel")]
        public void test_travel() {
            testAgent.btsetcurrent("node_test/htn/travel/root");
            testAgent.resetProperties();
            testAgent.SetStartFinish(HTNAgentTravel.sh_td, HTNAgentTravel.sz_td);
            testAgent.btexec();

            Assert.AreEqual(3, testAgent.Path.Count);
            Assert.AreEqual("ride_taxi", testAgent.Path[0].name);
            Assert.AreEqual(HTNAgentTravel.sh_td, testAgent.Path[0].x);
            Assert.AreEqual(HTNAgentTravel.airport_sh_hongqiao, testAgent.Path[0].y);

            Assert.AreEqual("fly", testAgent.Path[1].name);
            Assert.AreEqual(HTNAgentTravel.airport_sh_hongqiao, testAgent.Path[1].x);
            Assert.AreEqual(HTNAgentTravel.airport_sz_baoan, testAgent.Path[1].y);

            Assert.AreEqual("ride_taxi", testAgent.Path[2].name);
            Assert.AreEqual(HTNAgentTravel.airport_sz_baoan, testAgent.Path[2].x);
            Assert.AreEqual(HTNAgentTravel.sz_td, testAgent.Path[2].y);

            testAgent.resetProperties();
            testAgent.SetStartFinish(HTNAgentTravel.sh_td, HTNAgentTravel.sh_home);
            testAgent.btexec();

            Assert.AreEqual(1, testAgent.Path.Count);
            Assert.AreEqual("ride_taxi", testAgent.Path[0].name);
            Assert.AreEqual(HTNAgentTravel.sh_td, testAgent.Path[0].x);
            Assert.AreEqual(HTNAgentTravel.sh_home, testAgent.Path[0].y);
        }
    }
}
#endif//BEHAVIAC_USE_HTN

#endif
