using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace tutorial_1_1
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

        static void ExecuteBT()
        {
            behaviac.EBTStatus status = g_FirstAgent.btexec();

            Console.WriteLine("ExecuteBT : {0}", status);
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

            Console.WriteLine("\nInput 1 : subtree1    2 : subtree2    3 : maintree1    4 : maintree2    Other Number : Exit\n");

            for (int input_key = Console.Read(); input_key > (int)'0' && input_key < (int)'5'; )
            {
                bool bInit = false;
                behaviac.EBTStatus status;

                switch (input_key)
                {
                    case '1':
                        bInit = InitPlayer("subtree1");
                        break;

                    case '2':
                        bInit = InitPlayer("subtree2");
                        break;

                    case '3':
                        bInit = InitPlayer("maintree1");
                        break;

                    case '4':
                        bInit = InitPlayer("maintree2");
                        break;
                }

                if (bInit)
                {
                    ExecuteBT();

                    CleanupPlayer();
                }

                Console.Read(); // '\r'
                Console.Read(); // '\n'

                Console.WriteLine("\nInput 1 : subtree1    2 : subtree2    3 : maintree1    4 : maintree2    Other Number : Exit\n");

                input_key = Console.Read();
            }

            CleanupBehaviac();

            Console.Read();
        }
    }
}
