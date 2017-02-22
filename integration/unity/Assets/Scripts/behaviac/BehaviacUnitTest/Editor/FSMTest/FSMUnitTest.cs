#if !BEHAVIAC_NOT_USE_MONOBEHAVIOUR

using System;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace BehaviorNodeUnitTest
{
    [TestFixture]
    [Category("HTNTest")]
    internal class FSMUnitTest : FSMAgentTest_0
    {
        [Test]
        [Category("fsm_ut_1")]
        public void fsm_ut_1() {
            behaviac.Workspace.Instance.FrameSinceStartup = 0;

            testAgent.btsetcurrent("node_test/fsm/fsm_ut_1");
            testAgent.resetProperties();

            behaviac.EBTStatus status = behaviac.EBTStatus.BT_INVALID;

            {
                testAgent.Message = FSMAgentTest.EMessage.Invalid;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                int InactiveCount = testAgent.GetVariable<int>("InactiveCount");
                Assert.AreEqual(0, InactiveCount);
                Assert.AreEqual(0, testAgent.TestVar);
            }

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            {
                //switch to Active
                testAgent.Message = FSMAgentTest.EMessage.Begin;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                int InactiveCount = testAgent.GetVariable<int>("InactiveCount");
                Assert.AreEqual(1, InactiveCount);
                uint ActiveCount = testAgent.GetVariable<uint>("ActiveCount");
                Assert.AreEqual(0, ActiveCount);
                Assert.AreEqual(2, testAgent.TestVar);
            }

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            {
                //switch to Pause
                testAgent.Message = FSMAgentTest.EMessage.Pause;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                uint ActiveCount = testAgent.GetVariable<uint>("ActiveCount");
                Assert.AreEqual(1, ActiveCount);
                short PauseCount = testAgent.GetVariable<short>("PauseCount");
                Assert.AreEqual(0, PauseCount);
                Assert.AreEqual(4, testAgent.TestVar);
            }

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            {
                //switch to Inactive
                testAgent.Message = FSMAgentTest.EMessage.End;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                short PauseCount = testAgent.GetVariable<short>("PauseCount");
                Assert.AreEqual(1, PauseCount);
                long ExitCount = testAgent.GetVariable<long>("ExitCount");
                Assert.AreEqual(0, ExitCount);
                Assert.AreEqual(6, testAgent.TestVar);
            }

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            {
                //switch to Exit
                testAgent.Message = FSMAgentTest.EMessage.Exit;

                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

                behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

                behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);

                int InactiveCount = testAgent.GetVariable<int>("InactiveCount");
                Assert.AreEqual(1, InactiveCount);
                long ExitCount = testAgent.GetVariable<long>("ExitCount");
                Assert.AreEqual(1, ExitCount);
                Assert.AreEqual(7, testAgent.TestVar);
            }

            behaviac.Workspace.Instance.FrameSinceStartup = behaviac.Workspace.Instance.FrameSinceStartup + 1;

            {
                //reenter again
                testAgent.Message = FSMAgentTest.EMessage.Invalid;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                int InactiveCount = testAgent.GetVariable<int>("InactiveCount");
                Assert.AreEqual(0, InactiveCount);
                Assert.AreEqual(8, testAgent.TestVar);
            }
        }

        [Test]
        [Category("fsm_ref_bt_ut")]
        public void fsm_ref_bt()
        {
            testBtAgent.btsetcurrent("node_test/fsm/fsm_ref_bt_ut");
            testBtAgent.resetProperties();

            behaviac.EBTStatus status = behaviac.EBTStatus.BT_INVALID;
            Assert.AreEqual(-1, testBtAgent.testVar_0);

            status = testBtAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(2, testBtAgent.testVar_0);

            Assert.AreEqual(1.8f, testBtAgent.testVar_2);
            Assert.AreEqual(4.5f, testBtAgent.testVar_3);
            Assert.AreEqual(true, "HC" == testBtAgent.testVar_str_0);

            status = testBtAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
            Assert.AreEqual(4, testBtAgent.testVar_0);

            Assert.AreEqual(1.8f, testBtAgent.testVar_2);
            Assert.AreEqual(4.5f, testBtAgent.testVar_3);
            Assert.AreEqual(true, "HC" == testBtAgent.testVar_str_0);
        }

        [Test]
        [Category("fsm_ref_fsm_ut")]
        public void fsm_ref_fsm_ut()
        {
            testAgent.btsetcurrent("node_test/fsm/fsm_ref_fsm_ut");
            testAgent.resetProperties();

            behaviac.EBTStatus status = behaviac.EBTStatus.BT_INVALID;

            {
                testAgent.Message = FSMAgentTest.EMessage.Invalid;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                int InactiveCount = testAgent.GetVariable<int>("InactiveCount");
                Assert.AreEqual(0, InactiveCount);
                Assert.AreEqual(0, testAgent.TestVar);
            }

            {
                //switch to Active
                testAgent.Message = FSMAgentTest.EMessage.Begin;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                int InactiveCount = testAgent.GetVariable<int>("InactiveCount");
                Assert.AreEqual(1, InactiveCount);
                uint ActiveCount = testAgent.GetVariable<uint>("ActiveCount");
                Assert.AreEqual(0, ActiveCount);
                Assert.AreEqual(2, testAgent.TestVar);

                int FoodCount = testAgent.GetVariable<int>("FoodCount");
                Assert.AreEqual(1, FoodCount);

                for (int i = 1; i < 9; ++i)
                {
                    status = testAgent.btexec();
                    Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

                    FoodCount = testAgent.GetVariable<int>("FoodCount");
                    Assert.AreEqual((i + 1), FoodCount);
                }

                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                FoodCount = testAgent.GetVariable<int>("FoodCount");
                Assert.AreEqual(8, FoodCount);

                int EnergyCount = testAgent.GetVariable<int>("EnergyCount");
                Assert.AreEqual(1, EnergyCount);
            }

            {
                //switch to Pause
                testAgent.Message = FSMAgentTest.EMessage.Pause;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                uint ActiveCount = testAgent.GetVariable<uint>("ActiveCount");
                Assert.AreEqual(1, ActiveCount);
                short PauseCount = testAgent.GetVariable<short>("PauseCount");
                Assert.AreEqual(0, PauseCount);
                Assert.AreEqual(14, testAgent.TestVar);
            }

            {
                //switch to Inactive
                testAgent.Message = FSMAgentTest.EMessage.End;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                short PauseCount = testAgent.GetVariable<short>("PauseCount");
                Assert.AreEqual(1, PauseCount);
                long ExitCount = testAgent.GetVariable<long>("ExitCount");
                Assert.AreEqual(0, ExitCount);
                Assert.AreEqual(16, testAgent.TestVar);
            }

            {
                //switch to Exit
                testAgent.Message = FSMAgentTest.EMessage.Exit;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_SUCCESS, status);
                int InactiveCount = testAgent.GetVariable<int>("InactiveCount");
                Assert.AreEqual(1, InactiveCount);
                long ExitCount = testAgent.GetVariable<long>("ExitCount");
                Assert.AreEqual(1, ExitCount);
                Assert.AreEqual(17, testAgent.TestVar);
            }
        }

        [Test]
        [Category("bt_ref_fsm_ut")]
        public void bt_ref_fsm_ut()
        {
            testAgent.btsetcurrent("node_test/fsm/bt_ref_fsm");
            testAgent.resetProperties();

            behaviac.EBTStatus status = behaviac.EBTStatus.BT_INVALID;

            {
                //switch to Active
                testAgent.Message = FSMAgentTest.EMessage.Begin;
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

                int FoodCount = testAgent.GetVariable<int>("FoodCount");
                Assert.AreEqual(1, FoodCount);

                for (int i = 1; i < 9; ++i)
                {
                    status = testAgent.btexec();
                    Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

                    FoodCount = testAgent.GetVariable<int>("FoodCount");
                    Assert.AreEqual((i + 1), FoodCount);
                }

                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);
                FoodCount = testAgent.GetVariable<int>("FoodCount");
                Assert.AreEqual(8, FoodCount);

                int EnergyCount = testAgent.GetVariable<int>("EnergyCount");
                Assert.AreEqual(1, EnergyCount);
            }
        }
    }
}

#endif
