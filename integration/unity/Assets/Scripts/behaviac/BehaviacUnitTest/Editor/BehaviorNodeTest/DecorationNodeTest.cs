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
    [Category("BehaviorNodeTest")]
    internal class DecorationNodeTest
    {
        AgentNodeTest testAgent = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv()
        {
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
        public void finlGlobalTestEnv()
        {
            testAgent.finl();
            BehaviacSystem.Instance.Uninit();
        }

        [SetUp]
        public void initTestEnv()
        {
        }

        [TearDown]
        public void finlTestEnv()
        {
            behaviac.Workspace.Instance.UnLoadAll();
        }

        [Test]
        [Category("test_decoration_loop_0")]
        public void test_decoration_loop_0()
        {
            testAgent.btsetcurrent("node_test/decoration_loop_ut_0");

            int loopCount = 1000;

            while (loopCount > 0)
            {
                testAgent.resetProperties();

                behaviac.EBTStatus status = testAgent.btexec();

		        Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                Assert.AreEqual(0, testAgent.testVar_0);

                behaviac.BehaviorTreeTask btTask = testAgent.CurrentTreeTask;
                Assert.AreNotEqual(null, btTask);

                List<behaviac.BehaviorTask> nodes = btTask.GetRunningNodes(false);
                Assert.AreEqual(3, nodes.Count);

                List<behaviac.BehaviorTask> leaves = btTask.GetRunningNodes(true);
                Assert.AreEqual(0, leaves.Count);

                --loopCount;
            }
        }

        [Test]
        [Category("test_decoration_loop_1")]
        public void test_decoration_loop_1()
        {
            testAgent.btsetcurrent("node_test/decoration_loop_ut_1");

            int loopCount = 0;

            while (loopCount < 500)
            {
                testAgent.resetProperties();
                testAgent.btexec();

                if (loopCount < 499)
                {
                    Assert.AreEqual(0, testAgent.testVar_0);

                }
                else
                {
                    Assert.AreEqual(1, testAgent.testVar_0);
                }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_repeat_0")]
        public void test_decoration_repeat_0()
        {
            testAgent.btsetcurrent("node_test/repeat/repeat_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(5, testAgent.testVar_0);
        }

        [Test]
        [Category("test_decoration_repeat_1")]
        public void test_decoration_repeat_1()
        {
            testAgent.btsetcurrent("node_test/repeat/repeat_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(5, testAgent.testVar_0);
        }

        [Test]
        [Category("test_decoration_not_0")]
        public void test_decoration_not_0()
        {
            testAgent.btsetcurrent("node_test/decoration_not_ut_0");

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_decoration_not_1")]
        public void test_decoration_not_1()
        {
            testAgent.btsetcurrent("node_test/decoration_not_ut_1");

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_decoration_not_2")]
        public void test_decoration_not_2()
        {
            testAgent.btsetcurrent("node_test/decoration_not_ut_2");
            int loopCount = 0;

            while (loopCount < 500)
            {
                testAgent.resetProperties();
                testAgent.btexec();
                Assert.AreEqual(0, testAgent.testVar_0);
                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_frames_0")]
        public void test_decoration_frames_0()
        {
            behaviac.Workspace.Instance.FrameSinceStartup = 0;

            testAgent.btsetcurrent("node_test/decoration_frames_ut_0");
            int loopCount = 0;

            while (loopCount < 100)
            {
                testAgent.resetProperties();
                testAgent.btexec();

                if (loopCount < 99)
                {
                    Assert.AreEqual(0, testAgent.testVar_0);
                }
                else
                {
                    Assert.AreEqual(1, testAgent.testVar_0);
                }

                ++loopCount;
                behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;
            }
        }

        [Test]
        [Category("test_decoration_frames_1")]
        public void test_decoration_frames_1()
        {
            behaviac.Workspace.Instance.FrameSinceStartup = 0;

            testAgent.btsetcurrent("node_test/decoration_frames_ut_0");
            int loopCount = 0;

            while (loopCount < 21)
            {

                testAgent.resetProperties();
                testAgent.btexec();

                if (loopCount < 20)
                {
                    Assert.AreEqual(0, testAgent.testVar_0);
                }
                else
                {
                    Assert.AreEqual(1, testAgent.testVar_0);
                }

                ++loopCount;
                behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 5;
            }
        }

        void once_(AgentNodeTest myTestAgent)
        {
            behaviac.Workspace.Instance.FrameSinceStartup = 0;

            behaviac.EBTStatus s = myTestAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(0, myTestAgent.testVar_0);

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            s = myTestAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(0, myTestAgent.testVar_0);

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            myTestAgent.testVar_0 = 1;

            //Move ends, testVar_0 = 2
            //Frames(5) is still running
            //testVar_0 = 0 again
            s = myTestAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(0, myTestAgent.testVar_0);

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            s = myTestAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(0, myTestAgent.testVar_0);

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            s = myTestAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, s);
            Assert.AreEqual(3, myTestAgent.testVar_0);
        }

        [Test]
        [Category("frames_ut_0")]
        public void frames_ut_0()
        {
            testAgent.btsetcurrent("node_test/frames_ut_0");

            once_(testAgent);

            //rerun again
            once_(testAgent);
        }

        [Test]
        [Category("test_decoration_loopuntil_0")]
        public void test_decoration_loopuntil_0()
        {
            testAgent.btsetcurrent("node_test/decoration_loopuntil_ut_0");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 100)
            {
                testAgent.btexec();

                if (loopCount < 99)
                { Assert.AreEqual(0, testAgent.testVar_0); }

                else
                { Assert.AreEqual(1, testAgent.testVar_0); }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_loopuntil_1")]
        public void test_decoration_loopuntil_1()
        {
            testAgent.btsetcurrent("node_test/decoration_loopuntil_ut_1");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 50)
            {
                testAgent.btexec();

                if (loopCount < 49)
                { Assert.AreEqual(0, testAgent.testVar_0); }

                else
                { Assert.AreEqual(1, testAgent.testVar_0); }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_loopuntil_2")]
        public void test_decoration_loopuntil_2()
        {
            testAgent.btsetcurrent("node_test/decoration_loopuntil_ut_2");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 50)
            {
                testAgent.btexec();

                if (loopCount < 49)
                { Assert.AreEqual(0, testAgent.testVar_0); }

                else
                { Assert.AreEqual(1, testAgent.testVar_0); }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_countlimit_0")]
        public void test_decoration_countlimit_0()
        {
            testAgent.btsetcurrent("node_test/decoration_countlimit_ut_0");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 6)
            {
                testAgent.btexec();

                if (loopCount < 5)
                { 
                    Assert.AreEqual(0, testAgent.testVar_0); 
                }
                else
                { 
                    Assert.AreEqual(1, testAgent.testVar_0); 
                }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_countlimit_1")]
        public void test_decoration_countlimit_1()
        {
            testAgent.btsetcurrent("node_test/decoration_countlimit_ut_1");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 11)
            {
                if (loopCount == 5)
                { testAgent.testVar_1 = 0; }

                testAgent.btexec();
                testAgent.testVar_1 = -1;

                if (loopCount < 10)
                { Assert.AreEqual(0, testAgent.testVar_0); }

                else
                { Assert.AreEqual(1, testAgent.testVar_0); }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_countlimit_2")]
        public void test_decoration_countlimit_2()
        {
            testAgent.btsetcurrent("node_test/decoration_countlimit_ut_2");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 11)
            {
                if (loopCount == 5)
                {
                    testAgent.testVar_1 = 0;
                    testAgent.testVar_2 = 0.0f;
                }

                testAgent.btexec();
                testAgent.testVar_1 = -1;

                if (loopCount < 10)
                { Assert.AreEqual(0, testAgent.testVar_0); }

                else
                { Assert.AreEqual(1, testAgent.testVar_0); }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_countlimit_3")]
        public void test_decoration_countlimit_3()
        {
            testAgent.btsetcurrent("node_test/decoration_countlimit_ut_3");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 11)
            {
                if (loopCount == 5)
                {
                    testAgent.testVar_3 = 0.0f;
                }

                testAgent.btexec();
                testAgent.testVar_3 = -1.0f;

                if (loopCount < 10)
                { Assert.AreEqual(0, testAgent.testVar_0); }

                else
                { Assert.AreEqual(1, testAgent.testVar_0); }

                ++loopCount;
            }
        }


        [Test]
        [Category("test_decoration_failureuntil_0")]
        public void test_decoration_failureuntil_0()
        {
            testAgent.btsetcurrent("node_test/decoration_failureuntil_ut_0");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 1000)
            {
                behaviac.EBTStatus status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_failureuntil_1")]
        public void test_decoration_failureuntil_1()
        {
            testAgent.btsetcurrent("node_test/decoration_failureuntil_ut_1");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 5)
            {
                behaviac.EBTStatus status = testAgent.btexec();

                if (loopCount < 4)
                { Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status); }

                else
                { Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status); }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_successuntil_0")]
        public void test_decoration_successuntil_0()
        {
            testAgent.btsetcurrent("node_test/decoration_successuntil_ut_0");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 1000)
            {
                behaviac.EBTStatus status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_successuntil_1")]
        public void test_decoration_successuntil_1()
        {
            testAgent.btsetcurrent("node_test/decoration_successuntil_ut_1");
            testAgent.resetProperties();
            int loopCount = 0;

            while (loopCount < 5)
            {
                behaviac.EBTStatus status = testAgent.btexec();

                if (loopCount < 4)
                { Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status); }

                else
                { Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status); }

                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_alwaysfailure_0")]
        public void test_decoration_alwaysfailure_0()
        {
            testAgent.btsetcurrent("node_test/decoration_alwaysfailure_ut_0");

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(1, testAgent.testVar_1);
            Assert.AreEqual(1.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_decoration_alwayssuccess_0")]
        public void test_decoration_alwayssuccess_0()
        {
            testAgent.btsetcurrent("node_test/decoration_alwayssuccess_ut_0");

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
            Assert.AreEqual(0.0f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_decoration_alwaysrunning_0")]
        public void test_decoration_alwaysrunning_0()
        {
            testAgent.btsetcurrent("node_test/decoration_alwaysrunning_ut_0");

            int loopCount = 0;

            while (loopCount < 1000)
            {
                testAgent.resetProperties();
                testAgent.btexec();
                Assert.AreEqual(0, testAgent.testVar_0);
                Assert.AreEqual(0, testAgent.testVar_1);
                Assert.AreEqual(0.0f, testAgent.testVar_2);
                ++loopCount;
            }
        }

        [Test]
        [Category("test_decoration_log_0")]
        public void test_decoration_log_0()
        {
            testAgent.btsetcurrent("node_test/decoration_log_ut_0");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(1, testAgent.testVar_1);
            Assert.AreEqual(0.0f, testAgent.testVar_2);
        }
    }
}

#endif
