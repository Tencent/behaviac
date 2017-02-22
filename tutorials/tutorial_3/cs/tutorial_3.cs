using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace tutorial_3
{
    class Program
    {
        static FirstAgent g_FirstAgent;
        static SecondAgent g_SecondAgent;
        static SecondAgent g_ThirdAgent;

        static bool InitBehavic()
        {
            Console.WriteLine("InitBehavic");

            behaviac.Workspace.Instance.FilePath = "../../exported";
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

            return true;
        }

        static bool InitPlayer()
        {
            Console.WriteLine("InitPlayer");

            g_FirstAgent = new FirstAgent();
            bool bRet = g_FirstAgent.btload("InstanceBT");
            Debug.Assert(bRet);
            g_FirstAgent.btsetcurrent("InstanceBT");

            g_SecondAgent = new SecondAgent();
            g_FirstAgent._set_pInstance(g_SecondAgent);

            g_ThirdAgent = new SecondAgent();
            behaviac.Agent.BindInstance(g_ThirdAgent, "SecondAgentInstance");

            return bRet;
        }

        static void UpdateLoop()
        {
            Console.WriteLine("UpdateLoop");

            int frames = 0;
            behaviac.EBTStatus status = behaviac.EBTStatus.BT_RUNNING;

            while (status == behaviac.EBTStatus.BT_RUNNING)
            {
                Console.WriteLine("frame {0}", ++frames);

                status = g_FirstAgent.btexec();
            }
        }

        static void CleanupPlayer()
        {
            Console.WriteLine("CleanupPlayer");

            g_FirstAgent = null;
        }

        static void CleanupBehaviac()
        {
            Console.WriteLine("CleanupBehaviac");

            behaviac.Workspace.Instance.Cleanup();
        }

        static void Main(string[] args)
        {
            InitBehavic();

            InitPlayer();

            UpdateLoop();

            CleanupPlayer();

            CleanupBehaviac();

            Console.Read();
        }
    }
}
