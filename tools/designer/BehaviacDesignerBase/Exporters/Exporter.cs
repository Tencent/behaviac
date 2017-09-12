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
using System.Text;
using System.IO;

namespace Behaviac.Design.Exporters
{
    /// <summary>
    /// This is the base class for an exporter. It allows you to export behaviours into your workflow to be added to your game.
    /// </summary>
    public abstract class Exporter
    {
        protected Nodes.BehaviorNode _node;

        /// <summary>
        /// The behaviour which will be exported.
        /// </summary>
        public Nodes.BehaviorNode Node
        {
            get
            {
                return _node;
            }
        }

        protected string _outputFolder;

        /// <summary>
        /// The folder we want to export the behaviour to.
        /// </summary>
        public string OutputFolder
        {
            get
            {
                return _outputFolder;
            }
        }

        protected string _filename;

        /// <summary>
        /// The relative filename the behaviour will be exported to.
        /// </summary>
        public string Filename
        {
            get
            {
                return _filename;
            }
        }

        protected List<string> _includedFilenames;
        public List<string> IncludedFilenames
        {
            get
            {
                return _includedFilenames;
            }
        }

        /// <summary>
        /// Creates a new exporter.
        /// </summary>
        /// <param name="node">The behaviour hich will be exported.</param>
        /// <param name="outputFolder">The folder we want to export the behaviour to.</param>
        /// <param name="filename">The relative filename we want to export to. You have to add your file extension.</param>
        public Exporter(Nodes.BehaviorNode node, string outputFolder, string filename, List<string> includedFilenames = null)
        {
            _node = node;
            _outputFolder = outputFolder;
            _filename = filename;
            _includedFilenames = includedFilenames;
        }

        public virtual FileManagers.SaveResult InitWriter()
        {
            return FileManagers.SaveResult.Succeeded;
        }

        /// <summary>
        /// Exportes the node to the given filename.
        /// </summary>
        /// <returns>Returns the result when the behaviour is exported.</returns>
        public virtual FileManagers.SaveResult Export()
        {
            return FileManagers.SaveResult.Succeeded;
        }

        public virtual FileManagers.SaveResult Export(List<Nodes.BehaviorNode> behaviors, bool exportBehaviors, bool exportMeta, int exportFileCount)
        {
            return FileManagers.SaveResult.Succeeded;
        }

        public virtual void PreviewAgentFile(AgentType agent)
        {
        }

        public virtual void PreviewEnumFile(EnumType enumType)
        {
        }

        public virtual void PreviewStructFile(StructType structType)
        {
        }

        protected void PreviewFile(string filename)
        {
            try
            {
                System.Diagnostics.Process.Start(filename);
            }
            catch
            {
            }
        }

        private bool CheckFileModified(string filename, string newContent)
        {
            if (!File.Exists(filename))
            {
                return true;
            }

            try
            {
                Encoding utf8WithBom = new UTF8Encoding(true);
                using (StreamReader file = new StreamReader(filename, utf8WithBom))
                {
                    return file.ReadToEnd() != newContent;
                }
            }
            catch
            {
                return true;
            }
        }

        protected void UpdateFile(StringWriter strWriter, string filename)
        {
            string fileContent = strWriter.ToString();
            if (CheckFileModified(filename, fileContent))
            {
                Encoding utf8WithBom = new UTF8Encoding(true);
                using (StreamWriter fileWriter = new StreamWriter(filename, false, utf8WithBom))
                {
                    fileWriter.Write(fileContent);
                    fileWriter.Close();
                }
            }
        }

        protected static string GetBehaviacTypesDir()
        {
            string behaviacTypesDir = Path.Combine(Path.GetTempPath(), "Behaviac_Types");
            behaviacTypesDir = Path.Combine(behaviacTypesDir, Workspace.Current.Name);

            if (!Directory.Exists(behaviacTypesDir))
            {
                Directory.CreateDirectory(behaviacTypesDir);
            }

            return behaviacTypesDir;
        }

        private static char[] spaces = { ' ', '\t' };
        private static string item_head = "///<<< THE METHOD HEAD";
        private static string method_head_old_name = "///<<< LASTNAME:";
        private static string item_begin = "///<<< BEGIN WRITING YOUR CODE";
        private static string item_end = "///<<< END WRITING YOUR CODE";

        protected static string file_init_part = "FILE_INIT";
        protected static string file_uninit_part = "FILE_UNINIT";
        protected static string namespace_init_part = "NAMESPACE_INIT";
        protected static string namespace_uninit_part = "NAMESPACE_UNINIT";
        protected static string class_part = "CLASS_PART";
        protected static string constructor_part = "CONSTRUCTOR";
        protected static string desctructor_part = "DESTRUCTOR";


        protected static void ExportFileWarningHeader(StringWriter file)
        {
            file.WriteLine("// -------------------------------------------------------------------------------");
            file.WriteLine("// THIS FILE IS ORIGINALLY GENERATED BY THE DESIGNER.");
            file.WriteLine("// YOU ARE ONLY ALLOWED TO MODIFY CODE BETWEEN '///<<< BEGIN' AND '///<<< END'.");
            file.WriteLine("// PLEASE MODIFY AND REGENERETE IT IN THE DESIGNER FOR CLASS/MEMBERS/METHODS, ETC.");
            file.WriteLine("// -------------------------------------------------------------------------------");
            file.WriteLine();
        }

        protected static void ExportMethodComment(StringWriter file, string indent, string oldName = null)
        {
            //file.WriteLine("{0}{1}", indent, item_head);
            if (!string.IsNullOrEmpty(oldName))
            {
                //remove class type name
                int pos = oldName.IndexOf("::");

                if (pos != -1)
                {
                    oldName = oldName.Substring(pos + 2);
                }
                else
                {
                    pos = oldName.IndexOf('.');

                    if (pos != -1)
                    {
                        oldName = oldName.Substring(pos + 1);
                    }
                }

                file.WriteLine("{0}///<<< LASTNAME:{1}", indent, oldName);
            }
        }

        protected static void ExportBeginComment(StringWriter file, string indent, string name)
        {
            //file.WriteLine("{0}{1} {2}", indent, item_begin, name);
            file.WriteLine("{0} {1}", item_begin, name);
        }

        protected static void ExportEndComment(StringWriter file, string indent)
        {
            //file.WriteLine("{0}{1}", indent, item_end);
            file.WriteLine("{0}", item_end);
        }

        private static bool StartsWith(string token, string line)
        {
            string lineNoEmpty = line.TrimStart(spaces);

            if (lineNoEmpty.StartsWith(token))
            {
                return true;
            }

            return false;
        }

        private static bool StartsWith(string token, string line, out string name)
        {
            string lineNoEmpty = line.TrimStart(spaces);

            if (lineNoEmpty.StartsWith(token))
            {
                name = lineNoEmpty.Substring(token.Length + 1);
                return true;
            }

            name = "";

            return false;
        }

        private static string ReadMethodOldName(string line)
        {
            string lineNoEmpty = line.TrimStart(spaces);

            if (lineNoEmpty.StartsWith(method_head_old_name))
            {
                string old_name = lineNoEmpty.Substring(method_head_old_name.Length);
                return old_name;
            }

            return string.Empty;
        }


        private static string ReadMethodName(string line)
        {
            string lineNoEmpty = line.TrimStart(spaces);
            {
                //bool AgentTest::Method(int param0)
                //public float GetConstFloatValue()
                int len_skip = 0;
                int end_pos = lineNoEmpty.IndexOf('(');
                int pos = lineNoEmpty.IndexOf("::");

                if (pos != -1)
                {
                    //c++
                    len_skip = 2;
                }
                else
                {
                    //c#
                    pos = lineNoEmpty.LastIndexOf(' ', end_pos);
                    len_skip = 1;
                }

                int len = end_pos - (pos + len_skip);
                string name = lineNoEmpty.Substring(pos + len_skip, len);

                return name;
            }
        }

        class MethodContent
        {
            public string name;
            public string old_name;
            public List<string> pre = null;
            public List<string> codes = new List<string>();
            //public List<string> post = new List<string>();

            public MethodContent(string n, string old_n)
            {
                this.name = n;
                this.old_name = old_n;
            }

            public void Add(string code)
            {
                this.codes.Add(code);
            }

            public static void Write(StringWriter file, List<string> lines, string methodName = null)
            {
                foreach (string s in lines)
                {
                    string item_name;
                    if (!string.IsNullOrEmpty(methodName) && StartsWith(item_begin, s, out item_name))
                    {
                        file.WriteLine("{0} {1}", item_begin, methodName);
                    }
                    else
                    {
                        file.WriteLine(s);
                    }
                }
            }
        }

        private static MethodContent FindMethod(List<MethodContent> ms, string name)
        {
            foreach (MethodContent m in ms)
            {
                if (m.name == name)
                {
                    return m;
                }
            }

            return null;
        }


        private static string Readline(StreamReader file, ref List<string> lines)
        {
            string line = file.ReadLine();

            if (line != null)
            {
                lines.Add(line);
            }

            return line;
        }

        private static MethodContent ReadItem(StreamReader file)
        {
            List<string> lines = new List<string>();

            string line = Readline(file, ref lines);

            if (line == null)
            {
                return null;
            }

            int lastIndex = -1;
            string name = "";
            string old_name = "";

            while (line != null && !StartsWith(item_begin, line))
            {
                line = Readline(file, ref lines);

                if (line == null)
                {
                    break;
                }

                // old format
                bool bHasItemHead = false;

                // method header
                if (StartsWith(item_head, line))
                {
                    line = Readline(file, ref lines);
                    bHasItemHead = true;
                }

                // last name
                if (StartsWith(method_head_old_name, line))
                {
                    old_name = ReadMethodOldName(line);

                    //don't include last name
                    lastIndex = lines.Count - 1;
                    lines.RemoveAt(lastIndex);

                    // skip last name
                    line = Readline(file, ref lines);
                }

                if (bHasItemHead)
                {
                    // old format
                    name = ReadMethodName(line);

                    if (string.IsNullOrEmpty(old_name))
                    {
                        old_name = name;
                    }
                }
            }

            if (line != null)
            {
                string item_name = "";

                if (StartsWith(item_begin, line, out item_name))
                {
                    name = item_name;

                    if (string.IsNullOrEmpty(old_name))
                    {
                        old_name = name;
                    }
                }

                //don't include item_begin
                lastIndex = lines.Count - 1;
                lines.RemoveAt(lastIndex);

                MethodContent method = new MethodContent(name, old_name);

                method.pre = lines;

                // add item_begin
                method.Add(line);
                line = Readline(file, ref method.codes);

                while (!StartsWith(item_end, line))
                {
                    line = Readline(file, ref method.codes);
                }

                // item_end added

                return method;
            }

            name = "last_part";
            old_name = name;
            MethodContent method2 = new MethodContent(name, old_name);
            method2.pre = lines;

            return method2;
        }

        private static List<MethodContent> ReadFileMethods(string filename)
        {
            List<MethodContent> methods = new List<MethodContent>();

            using(StreamReader file = new StreamReader(filename))
            {
                while (true)
                {
                    MethodContent method = ReadItem(file);

                    if (method != null)
                    {
                        methods.Add(method);

                        if (methods.Count == 1 && string.IsNullOrEmpty(method.name))
                        {
                            method.name = "init_part";
                            method.old_name = method.name;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (methods.Count > 1)
                {
                    MethodContent lastMethod = methods[methods.Count - 1];

                    if (string.IsNullOrEmpty(lastMethod.name))
                    {
                        lastMethod.name = "uninit_part";
                        lastMethod.old_name = lastMethod.name;
                    }
                }

                return methods;
            }
        }

        public void MergeFiles(string filename, string newFilename, string out_filename)
        {
            try
            {
                List<MethodContent> methods = ReadFileMethods(filename);
                List<MethodContent> new_methods = ReadFileMethods(newFilename);

                using (StringWriter file = new StringWriter())
                {
                    foreach (MethodContent method in new_methods)
                    {
                        MethodContent.Write(file, method.pre);

                        MethodContent old_method = FindMethod(methods, method.old_name);

                        if (old_method == null)
                        {
                            old_method = method;
                        }

                        MethodContent.Write(file, old_method.codes, method.name);
                    }

                    UpdateFile(file, out_filename);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Diff Error : {0} {1}", filename, e.Message);
            }
        }

    }
}
