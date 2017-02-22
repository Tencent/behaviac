////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    /// <summary>
    /// A class for parsing commands inside a tool. Based on Novell Options class (http://www.ndesk.org/Options).
    /// </summary>
    public class CommandOptions
    {
        public bool help = false;
        public bool export = false;
        public string format = "";
        public string workspace = "";
        public string bt = "";

        public bool Parse(string[] args)
        {
            if (args.Length == 0)
            {
                help = true;
            }

            this.format = "";

            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];

                if (arg.StartsWith("/help", StringComparison.OrdinalIgnoreCase))
                {
                    this.help = true;
                }
                else if (arg.StartsWith("/export", StringComparison.OrdinalIgnoreCase))
                {
                    this.export = true;

                    int pos = arg.IndexOf('=');

                    if (pos != -1)
                    {
                        this.format = arg.Substring(pos + 1);

                        bool isValidFormat = false;

                        foreach (ExporterInfo info in Plugin.Exporters)
                        {
                            if (info.ID == this.format)
                            {
                                isValidFormat = true;
                                break;
                            }
                        }

                        if (!isValidFormat)
                        {
                            System.Diagnostics.Debug.Assert(false);
                        }
                    }
                }
                else if (arg.StartsWith("/bt", StringComparison.OrdinalIgnoreCase))
                {
                    int pos = arg.IndexOf('=');

                    if (pos != -1)
                    {
                        this.bt = arg.Substring(pos + 1);
                    }

                }
                else
                {
                    this.workspace = arg;
                }
            }

            return !string.IsNullOrEmpty(this.workspace) && !string.IsNullOrEmpty(this.format);
        }
    }

    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(UInt32 dwProcessId);
        [DllImport("kernel32.dll")]
        private static extern bool GetFileInformationByHandle(SafeFileHandle hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);
        [DllImport("kernel32.dll")]
        private static extern SafeFileHandle GetStdHandle(UInt32 nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern bool SetStdHandle(UInt32 nStdHandle, SafeFileHandle hHandle);
        [DllImport("kernel32.dll")]
        private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, SafeFileHandle hSourceHandle, IntPtr hTargetProcessHandle, out SafeFileHandle lpTargetHandle, UInt32 dwDesiredAccess, Boolean bInheritHandle, UInt32 dwOptions);

        private const UInt32 ATTACH_PARENT_PROCESS = 0xFFFFFFFF;
        private const UInt32 STD_OUTPUT_HANDLE = 0xFFFFFFF5;
        private const UInt32 STD_ERROR_HANDLE = 0xFFFFFFF4;
        private const UInt32 DUPLICATE_SAME_ACCESS = 2;
        struct BY_HANDLE_FILE_INFORMATION
        {
            public UInt32 FileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
            public UInt32 VolumeSerialNumber;
            public UInt32 FileSizeHigh;
            public UInt32 FileSizeLow;
            public UInt32 NumberOfLinks;
            public UInt32 FileIndexHigh;
            public UInt32 FileIndexLow;
        }
        public static void InitConsoleHandles()
        {
            SafeFileHandle hStdOut, hStdErr, hStdOutDup, hStdErrDup;
            BY_HANDLE_FILE_INFORMATION bhfi;
            hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            hStdErr = GetStdHandle(STD_ERROR_HANDLE);
            // Get current process handle
            IntPtr hProcess = Process.GetCurrentProcess().Handle;
            // Duplicate Stdout handle to save initial value
            DuplicateHandle(hProcess, hStdOut, hProcess, out hStdOutDup,
                            0, true, DUPLICATE_SAME_ACCESS);
            // Duplicate Stderr handle to save initial value
            DuplicateHandle(hProcess, hStdErr, hProcess, out hStdErrDup,
                            0, true, DUPLICATE_SAME_ACCESS);
            // Attach to console window ¨C this may modify the standard handles
            AttachConsole(ATTACH_PARENT_PROCESS);

            // Adjust the standard handles
            if (GetFileInformationByHandle(GetStdHandle(STD_OUTPUT_HANDLE), out bhfi))
            {
                SetStdHandle(STD_OUTPUT_HANDLE, hStdOutDup);

            }
            else
            {
                SetStdHandle(STD_OUTPUT_HANDLE, hStdOut);
            }

            if (GetFileInformationByHandle(GetStdHandle(STD_ERROR_HANDLE), out bhfi))
            {
                SetStdHandle(STD_ERROR_HANDLE, hStdErrDup);

            }
            else
            {
                SetStdHandle(STD_ERROR_HANDLE, hStdErr);
            }
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (Settings.Default.UpdateRequired)
                {
                    Settings.Default.Upgrade();
                    Settings.Default.UpdateRequired = false;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                MainWindow.SetDefaultSettings();

                Plugin.LoadPlugins();

                //only do it when there are args
                if (args.Length > 0)
                {
                    NativeMethods.InitConsoleHandles();
                }

                CommandOptions options = new CommandOptions();

                bool bOk = options.Parse(args);

                if (!bOk || options.help)
                {
                    string allFormats = string.Empty;

                    foreach (ExporterInfo info in Plugin.Exporters)
                    {
                        if (!string.IsNullOrEmpty(allFormats))
                        {
                            allFormats += "|";
                        }

                        allFormats += info.ID;
                    }

                    string usage = string.Format
                                   (@"
                                    Usage:
                                    BehaviacDesigner.exe <workspaceFile> <options>
                                    options:
                                    /bt=btFile
                                    /export=<{0}>
                                    /help", allFormats);

                    System.Console.WriteLine(usage);
                }

                if (options.export)
                {
                    if (bOk)
                    {
                        string msg = string.Format("Exporting: format '{0}' workspace '{1}' ...", options.format, options.workspace);
                        System.Console.WriteLine(msg);

                        if (!System.IO.File.Exists(options.workspace))
                        {
                            msg = string.Format("Workspace '{0}' does not exist! check the workspace path and name.", options.workspace);
                            System.Console.WriteLine(msg);

                            return;
                        }

                        try
                        {
                            new MainWindow(false);

                            MainWindow.Instance.Hide();

                            if (MainWindow.Instance.SetWorkspace(options.workspace, false))
                            {
                                MainWindow.Instance.ExportBehavior(true, options.format);
                            }
                            else
                            {
                                //msg = string.Format("Workspace '{0}' is not a valid workspace file!", options.workspace);
                                //System.Console.WriteLine(msg);
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(options.workspace))
                        {
                            System.Console.WriteLine("No workspace is specified!");
                        }

                        if (string.IsNullOrEmpty(options.format))
                        {
                            System.Console.WriteLine("No format is specified!");
                        }
                    }

                    return;
                }

                using (MainWindow mainWindow = new MainWindow(true))
                {
                    if (!string.IsNullOrEmpty(options.workspace))
                    {
                        mainWindow.SetWorkspace(options.workspace, false);

                        if (!string.IsNullOrEmpty(options.bt))
                        {
                            UIUtilities.ShowBehaviorTree(options.bt);
                        }
                    }

                    Application.Run(mainWindow);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception is {0}", e);
            }
        }
    }
}