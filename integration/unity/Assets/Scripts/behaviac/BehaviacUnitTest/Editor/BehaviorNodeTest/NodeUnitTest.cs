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
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace BehaviorNodeUnitTest
{
    [TestFixture]
    [Category("SelectorLoopTest")]
    internal class SelectorLoopTest : UnitTestBase_0
    {
        [Test]
        [Category("test_selector_loop_0")]
        public void test_selector_loop_0()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_loop_1")]
        public void test_selector_loop_1()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_loop_2")]
        public void test_selector_loop_2()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_2");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_loop_3")]
        public void test_selector_loop_3()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_3");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(-1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_loop_4")]
        public void test_selector_loop_4()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_4");
            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
        }

        [Test]
        [Category("test_selector_loop_5")]
        public void test_selector_loop_5()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_5");
            testAgent.resetProperties();
            testAgent.btexec();

            behaviac.EBTStatus s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, s);
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_loop_6")]
        public void test_selector_loop_6()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_6");
            testAgent.resetProperties();

            testAgent.m_bCanSee = false;
            const int kCount = 5;
            for (int i = 0; i < kCount; ++i)
            {
                behaviac.EBTStatus status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                Assert.AreEqual(2, testAgent.testVar_0);
            }

            testAgent.m_bCanSee = true;

            behaviac.EBTStatus s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(1, testAgent.testVar_0);

            s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(1, testAgent.testVar_0);

            testAgent.m_bCanSee = false;

            s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(2, testAgent.testVar_0);

            s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(2, testAgent.testVar_0);

            testAgent.m_bCanSee = true;

            s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, s);
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_loop_7")]
        public void test_selector_loop_7()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_7");
            testAgent.resetProperties();

            testAgent.m_bCanSee = false;
            const int kCount = 5;
            for (int i = 0; i < kCount; ++i)
            {
                behaviac.EBTStatus status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                Assert.AreEqual(2, testAgent.testVar_0);

                Assert.AreEqual(i + 1, testAgent.testVar_1);
            }

            testAgent.m_bCanSee = true;

            behaviac.EBTStatus s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(6, testAgent.testVar_1);

            testAgent.m_bTargetValid = false;

            s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, s);
            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(3, testAgent.testVar_1);
        }

        [Test]
        [Category("test_selector_loop_8")]
        public void test_selector_loop_8()
        {
            testAgent.btsetcurrent("node_test/selector_loop_ut_8");
            testAgent.resetProperties();

            testAgent.testVar_0 = 10;
            testAgent.m_bCanSee = false;

            behaviac.EBTStatus s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(2, testAgent.testVar_0);
            Assert.AreEqual(1, testAgent.testVar_1);

            testAgent.testVar_0 = 10;
            s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, s);
            Assert.AreEqual(2, testAgent.testVar_0);
            Assert.AreEqual(2, testAgent.testVar_1);

            s = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, s);
            Assert.AreEqual(2, testAgent.testVar_0);
            Assert.AreEqual(101, testAgent.testVar_1);
        }

    }

    [TestFixture]
    [Category("SelectorTests")]
    internal class SelectorTest : UnitTestBase_0
    {
        [Test]
        [Category("test_selector_0")]
        public void test_selector_0()
        {
            testAgent.btsetcurrent("node_test/selector_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_1")]
        public void test_selector_1()
        {
            testAgent.btsetcurrent("node_test/selector_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_2")]
        public void test_selector_2()
        {
            testAgent.btsetcurrent("node_test/selector_ut_2");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_3")]
        public void test_selector_3()
        {
            testAgent.btsetcurrent("node_test/selector_ut_3");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_selector_4")]
        public void test_selector_4()
        {
            testAgent.btsetcurrent("node_test/selector_ut_4");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(0, testAgent.testVar_0);
        }
    }

    [TestFixture]
    [Category("SequenceTests")]
    internal class SequenceTests : UnitTestBase_0
    {
        [Test]
        [Category("test_sequence_0")]
        public void test_sequence_0()
        {
            testAgent.btsetcurrent("node_test/sequence_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_sequence_1")]
        public void test_sequence_1()
        {
            testAgent.btsetcurrent("node_test/sequence_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_sequence_2")]
        public void test_sequence_2()
        {
            testAgent.btsetcurrent("node_test/sequence_ut_2");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_sequence_3")]
        public void test_sequence_3()
        {
            testAgent.btsetcurrent("node_test/sequence_ut_3");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(0, testAgent.testVar_0);
        }
    }

    // ---------------- IfElseTests ------------------
    [TestFixture]
    [Category("IfElseTests")]
    internal class IfElseTests : UnitTestBase_0
    {
        [Test]
        [Category("test_true")]
        public void test_true()
        {
            testAgent.btsetcurrent("node_test/if_else_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_false")]
        public void test_false()
        {
            testAgent.btsetcurrent("node_test/if_else_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            //< check int value
            Assert.AreEqual(2, testAgent.testVar_0);
        }
    }

    [TestFixture]
    [Category("SequenceStochasticTests")]
    internal class SequenceStochasticTests : UnitTestBase_0
    {
        [Test]
        [Category("test_sequence_stochastic_0")]
        public void test_sequence_stochastic_0()
        {
            testAgent.resetProperties();

            int[] counts = new int[] { 0, 0, 0 };
            int loopCount = kLoopCount;

            while (loopCount > 0)
            {
                testAgent.btsetcurrent("node_test/sequence_stochastic_ut_0");
                testAgent.btexec();
                ++(counts[testAgent.testVar_0]);
                --loopCount;
            }

            //< check int value
            for (int i = 0; i < counts.Length; ++i)
            {
                int k = counts[i];
                int bias = Mathf.Abs(k - kLoopMed);
                Assert.Less(bias, kLoopBias);
            }
        }

        void test_sequence_stochastic_distribution(string bt)
        {
            int predicateValueCount = 0;
            int loopCount = kLoopCount;

            while (loopCount > 0)
            {
                testAgent.btsetcurrent(bt);
                testAgent.resetProperties();
                testAgent.btexec();

                if (testAgent.testVar_0 == 0)
                { predicateValueCount++; }

                --loopCount;
            }

            int bias = Mathf.Abs(predicateValueCount - kLoopMed);
            Assert.Less(bias, kLoopBias);
        }

        [Test]
        [Category("test_sequence_stochastic_1")]
        public void test_sequence_stochastic_1()
        {
            test_sequence_stochastic_distribution("node_test/sequence_stochastic_ut_1");
        }

        [Test]
        [Category("test_sequence_stochastic_2")]
        public void test_sequence_stochastic_2()
        {
            test_sequence_stochastic_distribution("node_test/sequence_stochastic_ut_2");
        }

        [Test]
        [Category("test_sequence_stochastic_3")]
        public void test_sequence_stochastic_3()
        {
            test_sequence_stochastic_distribution("node_test/sequence_stochastic_ut_3");
        }
    }

    [TestFixture]
    [Category("SelectorStochasticTests")]
    internal class SelectorStochasticTests : UnitTestBase_0
    {
        [Test]
        [Category("test_selector_stochastic_0")]
        public void test_selector_stochastic_0()
        {
            testAgent.resetProperties();

            int[] counts = new int[] { 0, 0, 0 };
            int loopCount = kLoopCount;

            while (loopCount > 0)
            {
                testAgent.btsetcurrent("node_test/selector_stochastic_ut_0");
                testAgent.btexec();
                ++(counts[testAgent.testVar_0]);
                --loopCount;
            }

            //< check int value
            for (int i = 0; i < counts.Length; ++i)
            {
                int k = counts[i];
                int bias = Mathf.Abs(k - kLoopMed);
                Assert.Less(bias, kLoopBias);
            }
        }

        [Test]
        [Category("test_selector_stochastic_1")]
        public void test_selector_stochastic_1()
        {
            int predicateValueCount = 0;
            int loopCount = kLoopCount;

            while (loopCount > 0)
            {
                testAgent.btsetcurrent("node_test/selector_stochastic_ut_1");
                testAgent.resetProperties();
                testAgent.btexec();

                if (testAgent.testVar_0 == 0)
                { predicateValueCount++; }

                --loopCount;
            }

            int bias = Mathf.Abs(predicateValueCount - kLoopMed);
            Assert.Less(bias, kLoopBias);
        }

        [Test]
        [Category("test_selector_stochastic_2")]
        public void test_selector_stochastic_2()
        {
            int predicateValueCount = 0;
            int loopCount = kLoopCount;

            while (loopCount > 0)
            {
                testAgent.btsetcurrent("node_test/selector_stochastic_ut_2");
                testAgent.resetProperties();
                testAgent.btexec();

                if (testAgent.testVar_0 == 0)
                { predicateValueCount++; }

                --loopCount;
            }

            int bias = Mathf.Abs(predicateValueCount - kLoopMed2);
            Assert.Less(bias, kLoopBias);
        }
    }

    [TestFixture]
    [Category("SelectorProbabilityTests")]
    internal class SelectorProbabilityTests : UnitTestBase_0
    {
        int[] test_selector_probability_distribution(string bt)
        {
            testAgent.resetProperties();

            int[] counts = new int[] { 0, 0, 0 };
            int loopCount = 10000;

            while (loopCount > 0)
            {
                testAgent.btsetcurrent(bt);
                testAgent.btexec();
                ++(counts[testAgent.testVar_0]);
                --loopCount;
            }

            return counts;
        }

        [Test]
        [Category("test_selector_probability_0")]
        public void test_selector_probability_0()
        {
            int[] counts = test_selector_probability_distribution("node_test/selector_probability_ut_0");
            int[] targetCounts = new int[] { 2000, 3000, 5000 };

            for (int i = 0; i < counts.Length; ++i)
            {
                int k = counts[i];
                int bias = Mathf.Abs(k - targetCounts[i]);
                Assert.Less(bias, 1000);
            }
        }

        [Test]
        [Category("test_selector_probability_1")]
        public void test_selector_probability_1()
        {
            int[] counts = test_selector_probability_distribution("node_test/selector_probability_ut_1");
            int[] targetCounts = new int[] { 0, 5000, 5000 };

            for (int i = 0; i < counts.Length; ++i)
            {
                int k = counts[i];
                int bias = Mathf.Abs(k - targetCounts[i]);
                Assert.Less(bias, 1000);
            }
        }

        [Test]
        [Category("test_selector_probability_2")]
        public void test_selector_probability_2()
        {
            testAgent.resetProperties();

            int loopCount = 10000;

            while (loopCount > 0)
            {
                testAgent.btsetcurrent("node_test/selector_probability_ut_2");
                testAgent.btexec();
                Assert.AreEqual(testAgent.testVar_0, -1);
                --loopCount;
            }
        }

        [Test]
        [Category("test_selector_probability_3")]
        public void test_selector_probability_3()
        {
            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/selector_probability_ut_3");
            behaviac.Workspace.Instance.FrameSinceStartup = 0;

            for (int i = 0; i < 2; ++i)
            {
                testAgent.btexec();
                if (testAgent.testVar_0 != -1)
                {
                    //Assert.AreEqual(i, testAgent.testVar_0);
                    Assert.AreEqual(0, testAgent.testVar_0);
                    Assert.AreEqual(-1, testAgent.testVar_1);
                }
                else
                {
                    //Assert.AreEqual(i, testAgent.testVar_1);
                    Assert.AreEqual(0, testAgent.testVar_1);
                    Assert.AreEqual(-1, testAgent.testVar_0);
                }

                behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;
            }

            testAgent.btexec();

            Assert.AreEqual(-1, testAgent.testVar_0);
            Assert.AreEqual(-1, testAgent.testVar_1);
        }

        [Test]
        [Category("test_selector_probability_4")]
        public void test_selector_probability_4()
        {
            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/selector_probability_ut_4");

            behaviac.Workspace.Instance.DoubleValueSinceStartup = 0;

            for (int i = 0; i < 10; ++i)
            {
                testAgent.btexec();
                if (testAgent.testVar_0 != -1)
                {
                    Assert.AreEqual(0, testAgent.testVar_0);
                    Assert.AreEqual(-1, testAgent.testVar_1);
                    Assert.AreEqual(0.0, testAgent.testVar_2);
                }
                else
                {
                    Assert.AreEqual(-1, testAgent.testVar_0);
                    Assert.AreEqual(0, testAgent.testVar_1);
                    Assert.AreEqual(-1, testAgent.testVar_2);
                }

                behaviac.Workspace.Instance.DoubleValueSinceStartup = behaviac.Workspace.Instance.DoubleValueSinceStartup + 0.1 * 1000;
            }

            behaviac.Workspace.Instance.DoubleValueSinceStartup = behaviac.Workspace.Instance.DoubleValueSinceStartup + 0.1 * 1000;
            testAgent.btexec();

            Assert.AreEqual(-1, testAgent.testVar_0);
            Assert.AreEqual(-1, testAgent.testVar_1);
        }



    }

    [TestFixture]
    [Category("ConditionNodesTests")]
    internal class ConditionNodesTests : UnitTestBase_0
    {
        [Test]
        [Category("test_condition_0")]
        public void test_condition_0()
        {
            testAgent.btsetcurrent("node_test/condition_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_condition_1")]
        public void test_condition_1()
        {
            testAgent.btsetcurrent("node_test/condition_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_condition_2")]
        public void test_condition_2()
        {
            testAgent.btsetcurrent("node_test/condition_ut_2");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
        }

        [Test]
        [Category("test_condition_3")]
        public void test_condition_3()
        {
            testAgent.btsetcurrent("node_test/condition_ut_3");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(2, testAgent.testVar_0);
        }
    }

    [TestFixture]
    [Category("ActionNodesTests")]
    internal class ActionNodesTests : UnitTestBase_0
    {
        [Test]
        [Category("test_action_0")]
        public void test_action_0()
        {
            testAgent.btsetcurrent("node_test/action_ut_0");
            testAgent.resetProperties();

            testAgent.testVar_3 = 1;
            testChildAgent.testVar_2 = 2;

            testAgent.SetChildAgent(testChildAgent);

            testAgent.btexec();

            Assert.AreEqual(1500, testAgent.testVar_0);
            Assert.AreEqual(1800, testAgent.testVar_1);
            Assert.AreEqual("abcd", testAgent.testVar_str_0);
            Assert.AreEqual(2, StaticAgent.sInt);
            Assert.AreEqual(true, testChildAgent.m_bTargetValid);
        }

        [Test]
        [Category("test_action_1")]
        public void test_action_1()
        {
            testAgent.btsetcurrent("node_test/action_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(1.8f, testAgent.testVar_2);
            Assert.AreEqual(4.5f, testAgent.testVar_3);
            Assert.AreEqual("HC", testAgent.testVar_str_0);
            Assert.AreEqual("NODE", testAgent.testVar_str_1);

            TestNS.Float2 float2 = testAgent.GetVariable<TestNS.Float2>("testFloat2");
            Assert.AreEqual(1.0f, float2.x);
            Assert.AreEqual(1.0f, float2.y);

            TestNS.Float2 c_ReturnFloat2 = testAgent.GetVariable<TestNS.Float2>("c_ReturnFloat2");
            Assert.AreEqual(2.0f, c_ReturnFloat2.x);
            Assert.AreEqual(2.0f, c_ReturnFloat2.y);

            TestNS.Float2 c_ReturnFloat2Const = testAgent.GetVariable<TestNS.Float2>("c_ReturnFloat2Const");
            Assert.AreEqual(2.0f, c_ReturnFloat2Const.x);
            Assert.AreEqual(2.0f, c_ReturnFloat2Const.y);
        }

        [Test]
        [Category("test_action_2")]
        public void test_action_2()
        {
            testAgent.resetProperties();
            testAgent.btsetcurrent("node_test/action_ut_2");
            testAgent.btexec();

            Assert.AreEqual(500000, testAgent.testVar_0);
            Assert.AreEqual(1666, testAgent.testVar_1);
        }

        [Test]
        [Category("test_action_3")]
        public void test_action_3()
        {
            testAgent.btsetcurrent("node_test/action_ut_3");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(2.4f, testAgent.testVar_2);
            Assert.AreEqual(4.0f, testAgent.testVar_3);
        }

        [Test]
        [Category("test_action_waitforsignal_0")]
        public void test_action_waitforsignal_0()
        {
            testAgent.btsetcurrent("node_test/action_ut_waitforsignal_0");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(-1, testAgent.testVar_1);
            Assert.AreEqual(-1.0f, testAgent.testVar_2);

            testAgent.resetProperties();
            testAgent.testVar_0 = 0;
            testAgent.btexec();
            Assert.AreEqual(1, testAgent.testVar_1);
            Assert.AreEqual(2.3f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_action_waitforsignal_1")]
        public void test_action_waitforsignal_1()
        {
            testAgent.btsetcurrent("node_test/action_ut_waitforsignal_1");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(-1, testAgent.testVar_1);
            Assert.AreEqual(-1.0f, testAgent.testVar_2);

            testAgent.resetProperties();
            testAgent.testVar_2 = 0.0f;
            testAgent.btexec();
            Assert.AreEqual(1, testAgent.testVar_1);
            Assert.AreEqual(2.3f, testAgent.testVar_2);
        }

        [Test]
        [Category("test_action_waitforsignal_2")]
        public void test_action_waitforsignal_2()
        {
            testAgent.btsetcurrent("node_test/action_ut_waitforsignal_2");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(-1.0f, testAgent.testVar_2);
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            testAgent.resetProperties();
            testAgent.testVar_2 = 0.0f;
            status = testAgent.btexec();
            Assert.AreEqual(2.3f, testAgent.testVar_2);
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
        }

        [Test]
        [Category("test_action_waitframes_0")]
        public void test_action_waitframes_0()
        {
            behaviac.Workspace.Instance.FrameSinceStartup = 0;

            testAgent.btsetcurrent("node_test/action_waitframes_ut_0");
            testAgent.resetProperties();

            int loopCount = 0;

            while (loopCount < 5)
            {
                testAgent.btexec();

                if (loopCount < 4)
                { Assert.AreEqual(1, testAgent.testVar_0); }

                else
                { Assert.AreEqual(2, testAgent.testVar_0); }

                ++loopCount;
                behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;
            }

            testAgent.resetProperties();
            testAgent.btexec();
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_action_noop_0")]
        public void test_action_noop_0()
        {
            testAgent.btsetcurrent("node_test/action_noop_ut_0");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_action_child_agent_0")]
        public void test_action_child_agent_0()
        {
            testAgent.btsetcurrent("node_test/action_child_agent_0");
            testAgent.resetProperties();

            testAgent.initChildAgentTest();
            testAgent.btexec();

            ChildNodeTest ct = testAgent.GetVariable<ChildNodeTest>("par_child_agent_1");
            Assert.AreEqual(666, ct.testVar_0);
            Assert.AreEqual(888, ct.testVar_1);
            Assert.AreEqual(999, ct.testVar_2);
        }

        [Test]
        [Category("custom_property_reset")]
        public void custom_property_reset()
        {
            testAgent.btsetcurrent("par_test/custom_property_reset");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();

            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(10, testAgent.testVar_1);

            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(20, testAgent.testVar_1);
        }

        [Test]
        [Category("node_test_selector_ut_5")]
        public void node_test_selector_ut_5()
        {
            testAgent.btsetcurrent("node_test/selector_ut_5");
            testAgent.resetProperties();

            testAgent.testColor = EnumTest.EnumTest_One;
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(0, testAgent.testVar_0);

            testAgent.testColor = EnumTest.EnumTest_OneAfterOne;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(1, testAgent.testVar_0);
        }
    }

    [TestFixture]
    [Category("WaitNodesTests")]
    internal class WaitNodesTests : UnitTestBase_0
    {
        [Test]
        [Category("test_wait_0")]
        public void test_wait_0()
        {
            behaviac.Workspace.Instance.DoubleValueSinceStartup = 0;
            testAgent.btsetcurrent("node_test/wait_ut_0");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(1, testAgent.testVar_0);

            behaviac.Workspace.Instance.DoubleValueSinceStartup = 1001 * 1000;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_wait_1")]
        public void test_wait_1()
        {
            behaviac.Workspace.Instance.DoubleValueSinceStartup = 0;
            testAgent.btsetcurrent("node_test/wait_ut_1");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(1, testAgent.testVar_0);

            behaviac.Workspace.Instance.DoubleValueSinceStartup = 1001 * 1000;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(2, testAgent.testVar_0);
        }

        [Test]
        [Category("test_wait_2")]
        public void test_wait_2()
        {
            behaviac.Workspace.Instance.DoubleValueSinceStartup = 0;
            testAgent.btsetcurrent("node_test/wait_ut_2");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(1, testAgent.testVar_0);

            for (int i = 0; i < 10; ++i)
            {
                double time = (i + 1);
                behaviac.Workspace.Instance.DoubleValueSinceStartup = time;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            }

            behaviac.Workspace.Instance.DoubleValueSinceStartup = 1.001 * 1000;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

            behaviac.Workspace.Instance.DoubleValueSinceStartup = 1.100 * 1000;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
        }
    }

    [TestFixture]
    [Category("ReferencedBehaviorTests")]
    internal class ReferencedBehaviorTests : UnitTestBase_0
    {
        [Test]
        [Category("circular_ut_0")]
        public void circular_ut_0()
        {
            testAgent.btsetcurrent("node_test/circular_ut_0");
            testAgent.resetProperties();
            testAgent.testVar_0 = 0;
            testAgent.btexec();

            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(0, testAgent.testVar_1);
        }

        [Test]
        [Category("reference_ut_0")]
        public void reference_ut_0()
        {
            testAgent.btsetcurrent("node_test/reference_ut_0");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(1.0, testAgent.testVar_2);
        }

        [Test]
        [Category("reference_ut_1")]
        public void reference_ut_1()
        {
            testAgent.btsetcurrent("node_test/reference_ut_1");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0.0, testAgent.testVar_2);
        }

        [Test]
        [Category("reference_ut_2")]
        public void reference_ut_2()
        {
            testAgent.btsetcurrent("node_test/reference_ut_2");
            testAgent.resetProperties();
            testAgent.btexec();

            Assert.AreEqual(0, testAgent.testVar_0);
            Assert.AreEqual(0.0, testAgent.testVar_2);
        }
    }

    [TestFixture]
    [Category("EndNodesTests")]
    internal class EndNodesTests : UnitTestBase_0
    {
        [Test]
        [Category("test_end_0")]
        public void test_end_0()
        {
            testAgent.btsetcurrent("node_test/end_ut_0");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_end_1")]
        public void test_end_1()
        {
            testAgent.btsetcurrent("node_test/end_ut_1");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_end_2")]
        public void test_end_2()
        {
            testAgent.btsetcurrent("node_test/end_ut_2");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(1, testAgent.testVar_0);
        }

        [Test]
        [Category("test_end_3")]
        public void test_end_3()
        {
            testAgent.btsetcurrent("node_test/end_ut_3");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
            Assert.AreEqual(2, testAgent.testVar_1);
        }

        [Test]
        [Category("test_end_4")]
        public void test_end_4()
        {
            testAgent.btsetcurrent("node_test/end_ut_4");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(1, testAgent.testVar_1);
        }

        [Test]
        [Category("test_end_5")]
        public void test_end_5()
        {
            testAgent.btsetcurrent("node_test/end_ut_5");
            testAgent.resetProperties();
            behaviac.EBTStatus status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);
            Assert.AreEqual(1, testAgent.testVar_0);
            Assert.AreEqual(1, testAgent.testVar_1);
        }
    }
}

#endif
