using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace demo_running
{
    class Program
    {
        static CBTPlayer g_player;

        static bool InitBehavic()
        {
            Console.WriteLine("InitBehavic");

            behaviac.Agent.RegisterInstanceName<CBTPlayer>();

            behaviac.Config.IsLogging = false;
            behaviac.Config.IsSocketing = false;

            behaviac.Workspace.Instance.ExportMetas("../../meta/demo_running.xml");

            behaviac.Workspace.Instance.FilePath = "../../exported";
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

            return true;
        }

        static bool InitPlayer(string pszTreeName)
        {
            Console.WriteLine("InitPlayer");

            g_player = new CBTPlayer();

            bool bRet = g_player.btload(pszTreeName);
            Debug.Assert(bRet);

            g_player.btsetcurrent(pszTreeName);

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

                status = g_player.btexec();
            }
        }

        static void CleanupPlayer()
        {
            Console.WriteLine("CleanupPlayer");

            g_player = null;
        }

        static void CleanupBehaviac()
        {
            Console.WriteLine("CleanupBehaviac");

            behaviac.Workspace.Instance.Cleanup();
        }

        static void Main(string[] args)
        {
            InitBehavic();

            InitPlayer("demo_running");

            UpdateLoop();

            CleanupPlayer();

            CleanupBehaviac();

            Console.Read();
        }
    }
}
