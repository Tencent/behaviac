using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace tutorial_5
{
    class Program
    {
        static FirstAgent g_FirstAgent;

        static bool InitBehavic()
        {
            Console.WriteLine("InitBehavic");

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

            Console.WriteLine("\nInput 1 : maintree    2 : maintree_task    Other Number : Exit\n");

            for (int input_key = Console.Read(); input_key > (int)'0' && input_key < (int)'3'; )
            {
                bool bInit = false;

                switch (input_key)
                {
                    case '1':
                        bInit = InitPlayer("maintree");
                        break;

                    case '2':
                        bInit = InitPlayer("maintree_task");
                        break;
                }

                if (bInit)
                {
                    UpdateLoop();

                    CleanupPlayer();
                }

                Console.Read(); // '\r'
                Console.Read(); // '\n'

                Console.WriteLine("\nInput 1 : LoopBT    2 : SequenceBT    Other Number : Exit\n");

                input_key = Console.Read();
            }

            CleanupBehaviac();

            Console.Read();
        }
    }
}
