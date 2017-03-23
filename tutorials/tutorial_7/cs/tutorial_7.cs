using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace tutorial_7
{
    class Program
    {
        static FirstAgent g_FirstAgent;

        static bool InitBehavic()
        {
            Console.WriteLine("InitBehavic");

            behaviac.Config.IsSocketBlocking = true;
            //behaviac.Config.SocketPort = 60636;

            behaviac.Workspace.Instance.FilePath = "../../exported";
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

            return true;
        }

        static bool InitPlayer(string btName)
        {
            Console.WriteLine("InitPlayer : {0}", btName);

            g_FirstAgent = new FirstAgent();

            bool bRet = g_FirstAgent.btload(btName);
            Debug.Assert(bRet);

            g_FirstAgent.btsetcurrent(btName);

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

                behaviac.Workspace.Instance.DebugUpdate();

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

            bool bInit = InitPlayer("demo");

            if (bInit)
            {
                UpdateLoop();

                CleanupPlayer();
            }

            CleanupBehaviac();

            Console.Read();
        }
    }
}
