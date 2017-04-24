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
using System.Windows.Forms;
using System.Xml;
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;
using Behaviac.Design.Exporters;

namespace Behaviac.Design
{
    /// <summary>
    /// This class represents a workspace available to the user.
    /// </summary>
    public sealed class Workspace
    {
        public delegate void WorkspaceChangedDelegate();
        public static WorkspaceChangedDelegate WorkspaceChangedHandler;

        private static Workspace _current = null;
        public static Workspace Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (_current != value)
                {
                    _current = value;

                    if (WorkspaceChangedHandler != null)
                    {
                        WorkspaceChangedHandler();
                    }
                }
            }
        }

        public static bool DebugWorkspace = true;

        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private string _language = "cpp";
        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _language = value;

                    Debug.Check(value == "cpp" || value == "cs", "Only cpp or cs are supported now!");
                }
            }
        }

        private bool _useIntValue = false;
        public bool UseIntValue
        {
            get
            {
                return _useIntValue;
            }
            set
            {
                _useIntValue = value ;
            }
        }

        private bool _promptMergingMetaFiles = false;
        public bool PromptMergingMetaFiles
        {
            get
            {
                return _promptMergingMetaFiles;
            }
            set
            {
                _promptMergingMetaFiles = value;
            }
        }

        public static int CurrentVersion = 3;

        private int _version = 0;
        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        private string _filename;
        public string FileName
        {
            get
            {
                //return Path.Combine(_folder, _name) + ".xml";
                return _filename;
            }
        }

        private string _sourceFolder;
        public string SourceFolder
        {
            get
            {
                _sourceFolder = Path.GetFullPath(_sourceFolder);
                return _sourceFolder;
            }
        }

        public string RelativeSourceFolder
        {
            get
            {
                return MakeRelativePath(this.SourceFolder);
            }
        }

        private string _defaultExportFolder;
        public string DefaultExportFolder
        {
            get
            {
                return _defaultExportFolder;
            }
        }

        public string RelativeDefaultExportFolder
        {
            get
            {
                return MakeRelativePath(_defaultExportFolder);
            }
        }

        private string _metaFilename;
        public string MetaFilename
        {
            get
            {
                return _metaFilename;
            }
            set
            {
                _metaFilename = value;
            }
        }

        public string RelativeMetaFilename
        {
            get
            {
                return string.IsNullOrEmpty(_metaFilename) ? "" : MakeRelativePath(_metaFilename);
            }
        }

        private bool _useRelativePath = true;
        public bool UseRelativePath
        {
            get
            {
                return _useRelativePath;
            }
            set
            {
                _useRelativePath = value;
            }
        }

        public class ExportData
        {
            private bool _isExported = true;
            public bool IsExported
            {
                get
                {
                    return _isExported;
                }
                set
                {
                    _isExported = value;
                }
            }

            private int _exportFileCount = 1;
            public int ExportFileCount
            {
                get
                {
                    return _exportFileCount;
                }
                set
                {
                    _exportFileCount = value;
                }
            }

            /// <summary>
            /// ExportFolder should be saved as relative path, but used as absoluted path.
            /// </summary>
            private string _exportFolder = "";
            public string ExportFolder
            {
                get
                {
                    return _exportFolder;
                }
                set
                {
                    _exportFolder = value;
                }
            }

            /// <summary>
            /// ExportIncludedFilenames should be saved and used as relative path.
            /// </summary>
            private List<string> _exportIncludedFilenames = new List<string>();
            public List<string> ExportIncludedFilenames
            {
                get
                {
                    return _exportIncludedFilenames;
                }
            }
        }

        private Dictionary<string, ExportData> _exportDatas = new Dictionary<string, ExportData>();
        public Dictionary<string, ExportData> ExportDatas
        {
            get
            {
                return _exportDatas;
            }
        }

        public void SetExportInfo(string format, bool isExported, int exportFileCount, string folder = null, List<string> includedFilenames = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                return;
            }

            if (!_exportDatas.ContainsKey(format))
            {
                _exportDatas[format] = new ExportData();
            }

            ExportData data = _exportDatas[format];
            data.IsExported = isExported;
            data.ExportFileCount = exportFileCount;

            if (folder != null)
            {
                data.ExportFolder = folder;
            }

            if (includedFilenames != null)
            {
                data.ExportIncludedFilenames.Clear();
                data.ExportIncludedFilenames.AddRange(includedFilenames);
            }
        }

        public void SetIncludedFilenames(string format, List<string> includedFilenames)
        {
            if (string.IsNullOrEmpty(format))
            {
                return;
            }

            if (includedFilenames != null)
            {
                if (!_exportDatas.ContainsKey(format))
                {
                    _exportDatas[format] = new ExportData();
                }

                ExportData data = _exportDatas[format];
                data.ExportIncludedFilenames.Clear();
                data.ExportIncludedFilenames.AddRange(includedFilenames);
            }
        }

        public bool ShouldBeExported(string format)
        {
            if (_exportDatas.ContainsKey(format))
            {
                ExportData data = _exportDatas[format];
                return data.IsExported;
            }

            // export xml only by default
            if (format == "xml")
            {
                return true;
            }

            return false;
        }

        public int GetExportFileCount(string format)
        {
            if (format == "xml" || format == "bson")
            {
                return 1;
            }

            if (_exportDatas.ContainsKey(format))
            {
                ExportData data = _exportDatas[format];
                return data.ExportFileCount;
            }

            return 1;
        }

        public void SetExportFileCount(string format, int fileCount)
        {
            if (format == "xml" || format == "bson")
            {
                return;
            }

            if (!_exportDatas.ContainsKey(format))
            {
                _exportDatas[format] = new ExportData();
            }

            ExportData data = _exportDatas[format];
            data.ExportFileCount = fileCount;
        }

        public bool IsSetExportFolder(string format)
        {
            if (_exportDatas.ContainsKey(format))
            {
                ExportData data = _exportDatas[format];
                return !string.IsNullOrEmpty(data.ExportFolder);
            }

            return false;
        }

        public string GetExportFolder(string format)
        {
            string exportFolder = "";

            if (_exportDatas.ContainsKey(format))
            {
                ExportData data = _exportDatas[format];
                exportFolder = data.ExportFolder;
            }

            if (string.IsNullOrEmpty(exportFolder))
            {
                string wsFilename = this.FileName.Replace('/', '\\');
                exportFolder = this.DefaultExportFolder.Replace('/', '\\');
                exportFolder = Workspace.MakeRelative(exportFolder, wsFilename, true, true);
                exportFolder = exportFolder.Replace('\\', '/');
            }

            return exportFolder;
        }

        public string GetExportAbsoluteFolder(string format)
        {
            string wsFilename = Workspace.Current.FileName.Replace('/', '\\');

            string exportFolder = Workspace.Current.GetExportFolder(format);
            exportFolder = exportFolder.Replace('/', '\\');

            string exportAbsoluteFolder = FileManagers.FileManager.MakeAbsolute(wsFilename, exportFolder);
            return exportAbsoluteFolder;
        }

        public List<string> GetExportIncludedFilenames(string format)
        {
            if (_exportDatas.ContainsKey(format))
            {
                ExportData data = _exportDatas[format];
                return data.ExportIncludedFilenames;
            }

            return new List<string>();
        }

        public string MakeAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return "";
            }

            string absolute = relativePath;

            absolute = absolute.Replace("$DESKTOP", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            absolute = absolute.Replace("$PERSONAL", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            absolute = absolute.Replace("$DOCUMENTS", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            absolute = absolute.Replace("$APPDATA", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            absolute = absolute.Replace("$LOCALAPPDATA", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            absolute = absolute.Replace("$COMMONAPPDATA", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            absolute = absolute.Replace("$PICTURES", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

            if (Path.IsPathRooted(absolute))
            {
                return absolute;
            }

            string path = Path.GetDirectoryName(_filename);
            string result = Path.Combine(path, absolute);
            result = Path.GetFullPath(result);
            return result;
        }

        public string MakeRelativePath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
            {
                return "";
            }

            string path = Path.GetDirectoryName(_filename);
            string relative = MakeRelative(absolutePath, path, true, true);

            return relative;
        }

        /// <summary>
        /// Creates a new workspace.
        /// </summary>
        /// <param name="name">The name of the workspace.</param>
        /// <param name="folder">The folder the behaviors will be loaded from.</param>
        /// <param name="defaultExportFolder">The folder behaviours are exported to by default.</param>
        public Workspace(bool useIntValue, string language, string path, string name, string xmlFile, string folder, string defaultExportFolder, Dictionary<string, ExportData> exportDatas = null)
        {
            _version = Workspace.CurrentVersion;
            _useIntValue = useIntValue;
            _filename = Path.GetFullPath(path);
            _name = name;
            this.Language = language;

            Debug.Check(_filename != null);
            _metaFilename = string.IsNullOrEmpty(xmlFile) ? "" : MakeAbsolutePath(xmlFile);
            _sourceFolder = MakeAbsolutePath(folder);
            _defaultExportFolder = MakeAbsolutePath(defaultExportFolder);

            if (exportDatas != null)
            {
                _exportDatas = exportDatas;
            }
        }

        /// <summary>
        /// This is required for the combobox used in the workspace selection dialogue.
        /// </summary>
        /// <returns>Returns the name of the workspace.</returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Creates a relative path from one file
        /// or folder to another.
        /// </summary>
        /// <param name="fromDirectory">
        /// Contains the directory that defines the
        /// start of the relative path.
        /// </param>
        /// <param name="toPath">
        /// Contains the path that defines the
        /// endpoint of the relative path.
        /// </param>
        /// <returns>
        /// The relative path from the start
        /// directory to the end path.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string MakeRelative(string toPath, string fromDirectory_, bool bDifferent, bool from_is_path = false)
        {
            if (fromDirectory_ == null)
            {
                throw new ArgumentNullException("fromDirectory");
            }

            string fromDirectory = fromDirectory_;

            if (!from_is_path)
            {
                fromDirectory = Path.GetDirectoryName(fromDirectory_);
            }

            if (toPath == null)
            {
                throw new ArgumentNullException("toPath");
            }

            bool isRooted = (Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(toPath));

            if (isRooted)
            {
                bool isDifferentRoot = (string.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), true) != 0);

                if (isDifferentRoot)
                {
                    return toPath;
                }
            }

            List<string> relativePath = new List<string>();
            string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);

            string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);

            int length = Math.Min(fromDirectories.Length, toDirectories.Length);

            int lastCommonRoot = -1;

            // find common root
            for (int x = 0; x < length; x++)
            {
                if (string.Compare(fromDirectories[x], toDirectories[x], true) != 0)
                {
                    break;
                }

                lastCommonRoot = x;
            }

            if (lastCommonRoot == -1)
            {
                return toPath;
            }

            // add relative folders in from path
            int higherLevel = bDifferent ? 1 : 0;

            for (int x = lastCommonRoot + higherLevel; x < fromDirectories.Length; x++)
            {
                if (fromDirectories[x].Length > 0)
                {
                    relativePath.Add("..");
                }
            }

            // add to folders to path
            for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
            {
                relativePath.Add(toDirectories[x]);
            }

            // create relative path
            string[] relativeParts = new string[relativePath.Count];
            relativePath.CopyTo(relativeParts, 0);

            string newPath = string.Join(Path.DirectorySeparatorChar.ToString(), relativeParts);

            if (string.IsNullOrEmpty(newPath))
            {
                newPath = ".";
            }

            return newPath;
        }

        /// <summary>
        /// Retrieves an attribute from a XML node. If the attribute does not exist an exception is thrown.
        /// </summary>
        /// <param name="node">The XML node we want to get the attribute from.</param>
        /// <param name="att">The name of the attribute we want.</param>
        /// <returns>Returns the attributes value. Is always valid.</returns>
        private static string GetAttribute(XmlNode node, string att)
        {
            XmlNode value = node.Attributes.GetNamedItem(att);

            if (value != null && value.NodeType == XmlNodeType.Attribute)
            {
                return value.Value;
            }

            return string.Empty;
        }

        public static Workspace LoadWorkspaceFile(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    return null;
                }

                XmlDocument xml = new XmlDocument();
                xml.Load(filename);

                XmlNode root = xml.ChildNodes[1];

                if (root.Name == "workspace")
                {
                    string language = GetAttribute(root, "language");
                    string name = GetAttribute(root, "name");
                    string folder = GetAttribute(root, "folder");
                    string defaultExportFolder = GetAttribute(root, "export");
                    string version = GetAttribute(root, "version");
                    string metaFile = GetAttribute(root, "xmlmetafolder");

                    if (string.IsNullOrEmpty(metaFile))
                    {
                        metaFile = GetAttribute(root, "metafilename");
                    }

                    string useIntValue = GetAttribute(root, "useintvalue");
                    if (string.IsNullOrEmpty(useIntValue))
                    {
                        useIntValue = GetAttribute(root, "useinttime");
                    }

                    string promptMergingMetaFiles = GetAttribute(root, "promptmergingmeta");
                    string useRelativePath = GetAttribute(root, "userelativepath");

                    if (string.IsNullOrEmpty(name) ||
                        string.IsNullOrEmpty(folder) ||
                        string.IsNullOrEmpty(defaultExportFolder))
                    {
                        throw new Exception(Resources.LoadWorkspaceError);
                    }

                    Workspace ws = new Workspace(useIntValue == "true", language, filename, name, metaFile, folder, defaultExportFolder);
                    ws.PromptMergingMetaFiles = (promptMergingMetaFiles == "true");
                    ws.Version = 0;
                    ws.UseRelativePath = (useRelativePath != "false");

                    if (!string.IsNullOrEmpty(version))
                    {
                        int v = 0;

                        if (int.TryParse(version, out v))
                        {
                            ws.Version = v;
                        }
                    }

                    foreach (XmlNode subnode in root)
                    {
                        if (subnode.NodeType == XmlNodeType.Element)
                        {
                            switch (subnode.Name)
                            {
                                    // Load all XMLs.
                                case "xmlmeta":
                                    string nodeName = subnode.InnerText.Trim();

                                    if (!string.IsNullOrEmpty(nodeName))
                                    {
                                        ws.MetaFilename = Path.Combine(ws.MetaFilename, nodeName);
                                    }

                                    break;

                                    // Load export nodes.
                                case "export":
                                    foreach (XmlNode exportNode in subnode)
                                    {
                                        if (exportNode.NodeType == XmlNodeType.Element)
                                        {
                                            if (!ws._exportDatas.ContainsKey(exportNode.Name))
                                            {
                                                ws._exportDatas[exportNode.Name] = new ExportData();
                                            }

                                            ExportData data = ws._exportDatas[exportNode.Name];

                                            foreach (XmlNode exportInfoNode in exportNode)
                                            {
                                                switch (exportInfoNode.Name)
                                                {
                                                    case "isexported":
                                                        data.IsExported = Boolean.Parse(exportInfoNode.InnerText.Trim());
                                                        break;

                                                    case "exportunifiedfile":
                                                        bool exportUnifiedFile = Boolean.Parse(exportInfoNode.InnerText.Trim());
                                                        data.ExportFileCount = exportUnifiedFile ? 1 : -1;
                                                        break;

                                                    case "exportfilecount":
                                                        data.ExportFileCount = Int32.Parse(exportInfoNode.InnerText.Trim());
                                                        break;

                                                    case "folder":
                                                        data.ExportFolder = exportInfoNode.InnerText.Trim();
                                                        break;

                                                    case "includedfilenames":
                                                        foreach (XmlNode sn in exportInfoNode)
                                                        {
                                                            if (sn.NodeType == XmlNodeType.Element && sn.Name == "includedfilename")
                                                            {
                                                                string includeFilename = sn.InnerText.Trim();

                                                                if (!data.ExportIncludedFilenames.Contains(includeFilename))
                                                                {
                                                                    data.ExportIncludedFilenames.Add(includeFilename);
                                                                }
                                                            }
                                                        }

                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    break;
                            }
                        }
                    }

                    return ws;
                }

            }
            catch (Exception)
            {
                string msgError = string.Format(Resources.LoadWorkspaceError, filename);
                MessageBox.Show(msgError, Resources.LoadError, MessageBoxButtons.OK);
            }

            return null;
        }

        public static void SaveWorkspaceFile(Workspace ws)
        {
            // check if we have a valid result
            if (ws != null && !string.IsNullOrEmpty(ws.FileName))
            {
                try
                {
                    XmlDocument xml = new XmlDocument();

                    // Create the xml declaration.
                    XmlDeclaration declaration = xml.CreateXmlDeclaration("1.0", "utf-8", null);
                    xml.AppendChild(declaration);

                    {
                        // Create workspace node.
                        XmlElement workspace = xml.CreateElement("workspace");
                        workspace.SetAttribute("name", ws.Name);

                        if (!string.IsNullOrEmpty(ws.RelativeMetaFilename))
                        {
                            workspace.SetAttribute("metafilename", ws.RelativeMetaFilename);
                        }

                        workspace.SetAttribute("folder", ws.RelativeSourceFolder);
                        workspace.SetAttribute("export", ws.RelativeDefaultExportFolder);
                        workspace.SetAttribute("language", ws.Language);

                        if (ws.UseIntValue)
                        {
                            workspace.SetAttribute("useintvalue", "true");
                        }

                        if (ws.PromptMergingMetaFiles)
                        {
                            workspace.SetAttribute("promptmergingmeta", "true");
                        }

                        if (!ws.UseRelativePath)
                        {
                            workspace.SetAttribute("userelativepath", "false");
                        }

                        workspace.SetAttribute("version", Workspace.CurrentVersion.ToString());
                        xml.AppendChild(workspace);

                        // Create export nodes.
                        XmlElement export = xml.CreateElement("export");
                        workspace.AppendChild(export);

                        foreach (string format in ws._exportDatas.Keys)
                        {
                            ExportData data = ws._exportDatas[format];

                            // Create exporter nodes.
                            XmlElement exporter = xml.CreateElement(format);
                            export.AppendChild(exporter);

                            // Create isExported node.
                            XmlElement isExported = xml.CreateElement("isexported");
                            isExported.InnerText = data.IsExported.ToString();
                            exporter.AppendChild(isExported);

                            // Create exportfilecount node.
                            if (format != "xml" && format != "bson")
                            {
                                XmlElement exportUnifiedFile = xml.CreateElement("exportfilecount");
                                exportUnifiedFile.InnerText = data.ExportFileCount.ToString();
                                exporter.AppendChild(exportUnifiedFile);
                            }

                            // Create folder node.
                            if (!string.IsNullOrEmpty(data.ExportFolder))
                            {
                                XmlElement folder = xml.CreateElement("folder");
                                folder.InnerText = data.ExportFolder;
                                exporter.AppendChild(folder);
                            }

                            // Create includedfilenames nodes.
                            if (data.ExportIncludedFilenames.Count > 0)
                            {
                                XmlElement exportincludedfilenames = xml.CreateElement("includedfilenames");
                                exporter.AppendChild(exportincludedfilenames);

                                foreach (string includedFilename in data.ExportIncludedFilenames)
                                {
                                    XmlElement exportincludedfilename = xml.CreateElement("includedfilename");
                                    exportincludedfilename.InnerText = includedFilename;
                                    exportincludedfilenames.AppendChild(exportincludedfilename);
                                }
                            }
                        }
                    }

                    //FileManagers.FileManager.MakeWritable(filename);

                    //string dir = Path.GetDirectoryName(filename);
                    //if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    //{
                    //    Directory.CreateDirectory(dir);
                    //}

                    // save workspaces
                    xml.Save(ws.FileName);

                }
                catch (Exception)
                {
                    string msgError = string.Format(Resources.SaveWorkspaceError, ws.FileName);
                    MessageBox.Show(msgError, Resources.SaveError, MessageBoxButtons.OK);
                }
            }
        }

        private string getSourceMetaPath(bool isSaving = false)
        {
            string metaFile = "";

            try
            {
                metaFile = this.FileName.Replace(".workspace.xml", ".meta.xml");

                string metaDir = Path.Combine(this.SourceFolder, "behaviac_meta");
                if (!File.Exists(metaFile) && !Directory.Exists(metaDir))
                {
                    Directory.CreateDirectory(metaDir);
                }

                string tmpMetaFile = Path.GetFileName(metaFile);
                tmpMetaFile = Path.Combine(metaDir, tmpMetaFile);

                string[] files = Directory.GetFiles(metaDir, "*.meta.xml", SearchOption.TopDirectoryOnly);

                if (files.Length >= 1)
                {
                    if (!File.Exists(tmpMetaFile))
                    {
                        tmpMetaFile = files[0];
                    }
                }

                if (isSaving || File.Exists(tmpMetaFile))
                {
                    metaFile = tmpMetaFile;
                }
                else if (!File.Exists(metaFile))
                {
                    string bbFile = Path.Combine(this.SourceFolder, "behaviac.bb.xml");
                    if (File.Exists(bbFile))
                    {
                        metaFile = bbFile;
                    }
                    else
                    {
                        metaFile = tmpMetaFile;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return metaFile;
        }

        private string getExportMetaXMLPath()
        {
            string metaFile = "";

            try
            {
                string metaFolder = Path.Combine(this.DefaultExportFolder, "meta");
                if (!Directory.Exists(metaFolder))
                {
                    Directory.CreateDirectory(metaFolder);
                }

                metaFile = Path.GetFileNameWithoutExtension(this.FileName);
                metaFile = Path.Combine(metaFolder, metaFile);
                metaFile = Path.ChangeExtension(metaFile, ".meta.xml");
            }
            catch (Exception e)
            {
                Debug.Check(false);
                Console.WriteLine(e.Message);
            }

            return metaFile;
        }

        private string getExportMetaBsonPath()
        {
            string metaFile = getExportMetaXMLPath();
            metaFile = Path.ChangeExtension(metaFile, ".bson.bytes");

            return metaFile;
        }

        private static List<XmlNode> _agentsXMLNodes = new List<XmlNode>();
        private static List<XmlNode> _typesXMLNodes = new List<XmlNode>();
        private static List<XmlNode> _instancesXMLNodes = new List<XmlNode>();

        public static List<XmlNode> CustomizedTypesXMLNodes
        {
            get { return _typesXMLNodes; }
        }

        public static List<XmlNode> CustomizedAgentsXMLNodes
        {
            get { return _agentsXMLNodes; }
        }

        public static List<XmlNode> CustomizedInstancesXMLNodes
        {
            get { return _instancesXMLNodes; }
        }

        public static bool PreLoadMeta(Workspace ws)
        {
            bool shouldSaveMeta = false;

            _agentsXMLNodes.Clear();
            _typesXMLNodes.Clear();
            _instancesXMLNodes.Clear();

            TypeManager.Instance.Clear();

            try
            {
                string metaDir = Path.Combine(ws.SourceFolder, "behaviac_meta");
                if (Directory.Exists(metaDir))
                {
                    string[] files = Directory.GetFiles(metaDir, "*.meta.xml", SearchOption.TopDirectoryOnly);

                    if (!ws.PromptMergingMetaFiles && files.Length > 1)
                    {
                        ws.PromptMergingMetaFiles = true;
                        Workspace.SaveWorkspaceFile(ws);

                        if (DialogResult.Yes == MessageBox.Show(Resources.AutoMergeMetaFilesInfo, Resources.LoadWarning, MessageBoxButtons.YesNo))
                        {
                            shouldSaveMeta = true;

                            foreach (string metaPath in files)
                            {
                                _preLoadMeta(ws, metaPath);
                            }

                            Directory.Delete(metaDir, true);
                            if (!Directory.Exists(metaDir))
                            {
                                Directory.CreateDirectory(metaDir);
                            }
                        }
                    }
                }

                if (!shouldSaveMeta)
                {
                    string metaPath = ws.getSourceMetaPath();
                    _preLoadMeta(ws, metaPath);
                }
            }
            catch (Exception e)
            {
                Debug.Check(false);
                Console.WriteLine(e.Message);
            }

            return shouldSaveMeta;
        }

        private static void _preLoadMeta(Workspace ws, string metaPath)
        {
            if (string.IsNullOrEmpty(metaPath) || !File.Exists(metaPath))
            {
                return;
            }

            try
            {
                XmlDocument metaFile = new XmlDocument();
                using (FileStream fs = new FileStream(metaPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    metaFile.Load(fs);
                    fs.Close();

                    for (int i = 1; i < metaFile.ChildNodes.Count; ++i)
                    {
                        XmlNode root = metaFile.ChildNodes[i];

                        if (root.Name == "meta")
                        {
                            foreach (XmlNode xmlNode in root.ChildNodes)
                            {
                                if (xmlNode.Name == "agents")
                                {
                                    _agentsXMLNodes.Add(xmlNode);
                                }
                                else if (xmlNode.Name == "types")
                                {
                                    _typesXMLNodes.Add(xmlNode);
                                }
                                else if (xmlNode.Name == "instances")
                                {
                                    _instancesXMLNodes.Add(xmlNode);
                                }
                            }
                        }
                        else if (root.Name == "agents")
                        {
                            _agentsXMLNodes.Add(root);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string errorInfo = string.Format("{0}\n{1}", metaPath, e.Message);
                MessageBox.Show(errorInfo, Resources.LoadError, MessageBoxButtons.OK);
            }
        }

        public static void LoadMeta(List<Nodes.Node.ErrorCheck> result)
        {
            foreach (XmlNode typesXmlNode in _typesXMLNodes)
            {
                LoadTypes(typesXmlNode);
            }

            foreach (XmlNode agentsXmlNode in _agentsXMLNodes)
            {
                LoadAgents(result, agentsXmlNode);
            }

            foreach (XmlNode instancesXmlNode in _instancesXMLNodes)
            {
                LoadInstances(result, instancesXmlNode);
            }

            _agentsXMLNodes.Clear();
            _typesXMLNodes.Clear();
            _instancesXMLNodes.Clear();
        }

        private static void loadCustomProperties(List<Nodes.Node.ErrorCheck> result, XmlNode propNode, AgentType agent)
        {
            Debug.Check(propNode.Name == "property" || propNode.Name == "Member");

            string propName = GetAttribute(propNode, "name");

            if (string.IsNullOrEmpty(propName))
            {
                propName = GetAttribute(propNode, "Name");
            }

            try
            {
                string isStatic = GetAttribute(propNode, "static");

                if (string.IsNullOrEmpty(isStatic))
                {
                    isStatic = GetAttribute(propNode, "Static");
                }

                bool bStatic = (!string.IsNullOrEmpty(isStatic) && isStatic == "true");

                string isPublic = GetAttribute(propNode, "public");

                if (string.IsNullOrEmpty(isPublic))
                {
                    isPublic = GetAttribute(propNode, "Public");
                }

                bool bPublic = (string.IsNullOrEmpty(isPublic) || isPublic == "true");

                string isReadonly = GetAttribute(propNode, "readonly");

                if (string.IsNullOrEmpty(isReadonly))
                {
                    isReadonly = GetAttribute(propNode, "Readonly");
                }

                bool bReadonly = (!string.IsNullOrEmpty(isReadonly) && isReadonly == "true");

                string propType = GetAttribute(propNode, "type");

                if (string.IsNullOrEmpty(propType))
                {
                    propType = GetAttribute(propNode, "TypeFullName");
                }

                Type type = Plugin.GetTypeFromName(propType);

                string classname = GetAttribute(propNode, "classname");

                if (string.IsNullOrEmpty(classname))
                {
                    classname = GetAttribute(propNode, "Class");
                }

                if (string.IsNullOrEmpty(classname))
                {
                    classname = agent.Name;
                }

                string propDisp = GetAttribute(propNode, "disp");

                if (string.IsNullOrEmpty(propDisp))
                {
                    propDisp = GetAttribute(propNode, "DisplayName");
                }

                if (string.IsNullOrEmpty(propDisp))
                {
                    propDisp = propName;
                }

                string propDesc = GetAttribute(propNode, "desc");

                if (string.IsNullOrEmpty(propDesc))
                {
                    propDesc = GetAttribute(propNode, "Desc");
                }

                PropertyDef prop = new PropertyDef(agent, type, classname, propName, propDisp, propDesc);
                prop.IsStatic = bStatic;
                prop.IsPublic = bPublic;
                prop.IsReadonly = bReadonly;

                string defaultValue = GetAttribute(propNode, "defaultvalue");

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    prop.Variable = new VariableDef(null);
                    Plugin.InvokeTypeParser(result, type, defaultValue, (object value) => prop.Variable.Value = value, null);
                }

                agent.AddProperty(prop);
            }
            catch (Exception)
            {
                string errorInfo = string.Format("error when loading Agent '{0}' Member '{1}'", agent.Name, propName);
                MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
            }
        }

        private static void loadCustomMethods(List<Nodes.Node.ErrorCheck> result, XmlNode methodNode, AgentType agent)
        {
            Debug.Check(methodNode.Name == "method" || methodNode.Name == "Method");

            string methodName = GetAttribute(methodNode, "name");

            if (string.IsNullOrEmpty(methodName))
            {
                methodName = GetAttribute(methodNode, "Name");
            }

            try
            {
                string returnTypename = GetAttribute(methodNode, "returntype");

                if (string.IsNullOrEmpty(returnTypename))
                {
                    returnTypename = GetAttribute(methodNode, "ReturnTypeFullName");
                }

                Type returnType = Plugin.GetTypeFromName(returnTypename);

                string isStatic = GetAttribute(methodNode, "static");

                if (string.IsNullOrEmpty(isStatic))
                {
                    isStatic = GetAttribute(methodNode, "Static");
                }

                bool bStatic = (!string.IsNullOrEmpty(isStatic) && isStatic == "true");

                string isPublic = GetAttribute(methodNode, "public");

                if (string.IsNullOrEmpty(isPublic))
                {
                    isPublic = GetAttribute(methodNode, "Public");
                }

                bool bPublic = (string.IsNullOrEmpty(isPublic) || isPublic == "true");

                string classname = GetAttribute(methodNode, "classname");

                if (string.IsNullOrEmpty(classname))
                {
                    classname = GetAttribute(methodNode, "Class");
                }

                if (string.IsNullOrEmpty(classname))
                {
                    classname = agent.Name;
                }

                string methodDisp = GetAttribute(methodNode, "disp");

                if (string.IsNullOrEmpty(methodDisp))
                {
                    methodDisp = GetAttribute(methodNode, "DisplayName");
                }

                if (string.IsNullOrEmpty(methodDisp))
                {
                    methodDisp = methodName;
                }

                string methodDesc = GetAttribute(methodNode, "desc");

                if (string.IsNullOrEmpty(methodDesc))
                {
                    methodDesc = GetAttribute(methodNode, "Desc");
                }

                bool istask = (GetAttribute(methodNode, "istask") == "true");
                //bool isevent = (GetAttribute(methodNode, "isevent") == "true");

                MemberType memberType = MemberType.Method;

                if (istask)
                {
                    memberType = MemberType.Task;
                }

                methodName = string.Format("{0}::{1}", agent.Name, methodName);

                MethodDef method = new MethodDef(agent, memberType, classname, methodName, methodDisp, methodDesc, "", returnType);
                method.IsStatic = bStatic;
                method.IsPublic = bPublic;

                agent.AddMethod(method);

                foreach (XmlNode paramNode in methodNode)
                {
                    string paramName = GetAttribute(paramNode, "name");

                    if (string.IsNullOrEmpty(paramName))
                    {
                        paramName = GetAttribute(paramNode, "Name");
                    }

                    string paramTypename = GetAttribute(paramNode, "type");

                    if (string.IsNullOrEmpty(paramTypename))
                    {
                        paramTypename = GetAttribute(paramNode, "TypeFullName");
                    }

                    Type paramType = Plugin.GetTypeFromName(paramTypename);

                    string isOutStr = GetAttribute(paramNode, "isout");

                    if (string.IsNullOrEmpty(isOutStr))
                    {
                        isOutStr = GetAttribute(paramNode, "IsOut");
                    }

                    string isRefStr = GetAttribute(paramNode, "isref");

                    if (string.IsNullOrEmpty(isRefStr))
                    {
                        isRefStr = GetAttribute(paramNode, "IsRef");
                    }

                    string isConstStr = GetAttribute(paramNode, "IsConst");

                    string nativeType = Plugin.GetNativeTypeName(paramType);

                    string paramDisp = GetAttribute(paramNode, "disp");

                    if (string.IsNullOrEmpty(paramDisp))
                    {
                        paramDisp = GetAttribute(paramNode, "DisplayName");
                    }

                    if (string.IsNullOrEmpty(paramDisp))
                    {
                        paramDisp = paramName;
                    }

                    string paramDesc = GetAttribute(paramNode, "desc");

                    if (string.IsNullOrEmpty(paramDesc))
                    {
                        paramDesc = GetAttribute(paramNode, "Desc");
                    }

                    MethodDef.Param param = new MethodDef.Param(paramName, paramType, nativeType, paramDisp, paramDesc);
                    param.IsOut = (isOutStr == "true");
                    param.IsRef = (isRefStr == "true");
                    param.IsConst = (isConstStr == "true");

                    method.Params.Add(param);
                }
            }
            catch (Exception)
            {
                string errorInfo = string.Format("error when loading Agent '{0}' Method '{1}'", agent.Name, methodName);
                MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
            }
        }

        private static void LoadInstances(List<Nodes.Node.ErrorCheck> result, XmlNode rootNode)
        {
            try
            {
                if (rootNode == null)
                {
                    return;
                }

                foreach (XmlNode xmlNode in rootNode.ChildNodes)
                {
                    if (xmlNode.Name == "instance")
                    {
                        string name = GetAttribute(xmlNode, "name");
                        string className = GetAttribute(xmlNode, "class");
                        string disp = GetAttribute(xmlNode, "DisplayName");
                        string desc = GetAttribute(xmlNode, "Desc");

                        AgentType agent = Plugin.GetAgentType(className);
                        Debug.Check(agent != null);

                        Plugin.AddInstanceName(name, className, disp, desc, agent);
                    }
                }
            }
            catch (Exception)
            {
                string errorInfo = string.Format("error when loading custom members");

                MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
            }
        }

        private static void LoadAgents(List<Nodes.Node.ErrorCheck> result, XmlNode rootNode)
        {
            try
            {
                if (rootNode == null)
                {
                    return;
                }

                // Set the default base agent.
                if (Plugin.AgentTypes.Count == 0)
                {
                    AgentType agent = new AgentType(typeof(Agent), "Agent", "", false, false, "Agent", "", false, true, "");
                    Plugin.AgentTypes.Add(agent);
                }

                string agentName;

                // first pass, to load all the agents as it might be used as a property of another agent
                foreach (XmlNode xmlNode in rootNode.ChildNodes)
                {
                    if (xmlNode.Name == "agent")
                    {
                        LoadAgentType(xmlNode, out agentName);
                    }
                }

                foreach (XmlNode xmlNode in rootNode.ChildNodes)
                {
                    if (xmlNode.Name == "agent")
                    {
                        AgentType agent = LoadAgentType(xmlNode, out agentName);

                        foreach (XmlNode bbNode in xmlNode)
                        {
                            if (bbNode.Name == "properties")
                            {
                                foreach (XmlNode propNode in bbNode)
                                {
                                    loadCustomProperties(result, propNode, agent);
                                }
                            }
                            else if (bbNode.Name == "methods")
                            {
                                foreach (XmlNode methodNode in bbNode)
                                {
                                    loadCustomMethods(result, methodNode, agent);
                                }
                            }
                            else if (bbNode.Name == "Member")
                            {
                                loadCustomProperties(result, bbNode, agent);
                            }
                            else if (bbNode.Name == "Method")
                            {
                                loadCustomMethods(result, bbNode, agent);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                string errorInfo = string.Format("error when loading custom members");

                MessageBox.Show(errorInfo, "Loading Custom Meta", MessageBoxButtons.OK);
            }
        }

        private static AgentType LoadAgentType(XmlNode xmlNode, out string agentName)
        {
            agentName = GetAttribute(xmlNode, "type");

            if (string.IsNullOrEmpty(agentName))
            {
                agentName = GetAttribute(xmlNode, "classfullname");
            }

            AgentType agent = Plugin.GetAgentType(agentName);

            if (agent == null)
            {
                string agentBase = GetAttribute(xmlNode, "base");
                int baseIndex = -1;

                for (int i = 0; i < Plugin.AgentTypes.Count; ++i)
                {
                    if (Plugin.AgentTypes[i].Name == agentBase)
                    {
                        baseIndex = i;
                        break;
                    }
                }

                string oldName = GetAttribute(xmlNode, "OldName");

                string agentDisp = GetAttribute(xmlNode, "disp");

                if (string.IsNullOrEmpty(agentDisp))
                {
                    agentDisp = GetAttribute(xmlNode, "DisplayName");
                }

                string agentDesc = GetAttribute(xmlNode, "desc");

                if (string.IsNullOrEmpty(agentDesc))
                {
                    agentDesc = GetAttribute(xmlNode, "Desc");
                }

                if (string.IsNullOrEmpty(agentDisp))
                {
                    agentDisp = agentName;
                }

                string isCustomized = GetAttribute(xmlNode, "IsCustomized");
                string isImplemented = GetAttribute(xmlNode, "IsImplemented");
                string exportLocation = GetAttribute(xmlNode, "ExportLocation");
                exportLocation = exportLocation.Replace("\\\\", "/");
                exportLocation = exportLocation.Replace("\\", "/");

                agent = new AgentType(isCustomized == "true", isImplemented == "true", agentName, oldName, (baseIndex > -1) ? Plugin.AgentTypes[baseIndex] : null, exportLocation, agentDisp, agentDesc);
                Plugin.AgentTypes.Add(agent);
            }

            return agent;
        }

        private static void LoadTypes(XmlNode rootNode)
        {
            try
            {
                if (rootNode == null)
                {
                    return;
                }

                foreach (XmlNode xmlNode in rootNode.ChildNodes)
                {
                    if (xmlNode.Name == "enumtype")
                    {
                        string enumName = GetAttribute(xmlNode, "Type");
                        EnumType enumType = TypeManager.Instance.FindEnum(enumName);

                        if (enumType != null)
                        {
                            TypeManager.Instance.Enums.Remove(enumType);
                        }

                        string[] enumNames = enumName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        enumName = enumNames[enumNames.Length - 1];

                        string isCustomized = GetAttribute(xmlNode, "IsCustomized");
                        string isImplemented = GetAttribute(xmlNode, "IsImplemented");
                        string ns = GetAttribute(xmlNode, "Namespace");
                        string exportLocation = GetAttribute(xmlNode, "ExportLocation");
                        exportLocation = exportLocation.Replace("\\\\", "/");
                        exportLocation = exportLocation.Replace("\\", "/");
                        string displayName = GetAttribute(xmlNode, "DisplayName");
                        string desc = GetAttribute(xmlNode, "Desc");

                        enumType = new EnumType(isCustomized == "true", isImplemented == "true", enumName, ns, exportLocation, displayName, desc);

                        foreach (XmlNode memberNode in xmlNode.ChildNodes)
                        {
                            if (memberNode.Name == "enum")
                            {
                                string memberNativeValue = GetAttribute(memberNode, "NativeValue");
                                string memberName = GetAttribute(memberNode, "Value");
                                string[] memberNames = memberName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                memberName = memberNames[memberNames.Length - 1];

                                string memberDisplayName = GetAttribute(memberNode, "DisplayName");
                                string memberDesc = GetAttribute(memberNode, "Desc");
                                string memberValue = GetAttribute(memberNode, "MemberValue");

                                EnumType.EnumMemberType enumMember = new EnumType.EnumMemberType(null);
                                enumMember.NativeValue = memberNativeValue;
                                enumMember.Name = memberName;
                                enumMember.DisplayName = memberDisplayName;
                                enumMember.Description = memberDesc;

                                try
                                {
                                    enumMember.Value = int.Parse(memberValue);
                                }
                                catch
                                {
                                    enumMember.Value = -1;
                                }

                                enumType.Members.Add(enumMember);
                            }
                        }

                        TypeManager.Instance.Enums.Add(enumType);
                    }
                    else if (xmlNode.Name == "struct")
                    {
                        string structName = GetAttribute(xmlNode, "Type");
                        StructType structType = TypeManager.Instance.FindStruct(structName);

                        if (structType != null)
                        {
                            TypeManager.Instance.Structs.Remove(structType);
                        }

                        string[] structNames = structName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        structName = structNames[structNames.Length - 1];

                        string isRef = GetAttribute(xmlNode, "IsRefType");
                        string isCustomized = GetAttribute(xmlNode, "IsCustomized");
                        string isImplemented = GetAttribute(xmlNode, "IsImplemented");
                        string ns = GetAttribute(xmlNode, "Namespace");
                        string baseName = GetAttribute(xmlNode, "Base");
                        string exportLocation = GetAttribute(xmlNode, "ExportLocation");
                        exportLocation = exportLocation.Replace("\\\\", "/");
                        exportLocation = exportLocation.Replace("\\", "/");
                        string displayName = GetAttribute(xmlNode, "DisplayName");
                        string desc = GetAttribute(xmlNode, "Desc");
                        string isStatic = GetAttribute(xmlNode, "Static");

                        structType = new StructType(isRef == "true", isCustomized == "true", isImplemented == "true", structName, ns, baseName, exportLocation, displayName, desc);

                        foreach (XmlNode memberNode in xmlNode.ChildNodes)
                        {
                            if (memberNode.Name == "Member")
                            {
                                string memberName = GetAttribute(memberNode, "Name");
                                string[] memberNames = memberName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                memberName = memberNames[memberNames.Length - 1];

                                string memberType = GetAttribute(memberNode, "TypeFullName");

                                if (string.IsNullOrEmpty(memberType))
                                {
                                    memberType = GetAttribute(memberNode, "Type");
                                }

                                Type type = Plugin.GetTypeFromName(memberType);

                                string memberDisplayName = GetAttribute(memberNode, "DisplayName");
                                string memberDesc = GetAttribute(memberNode, "Desc");

                                string memberReadonly = GetAttribute(memberNode, "Readonly");

                                PropertyDef structProp = new PropertyDef(null, type, structName, memberName, memberDisplayName, memberDesc);
                                structProp.IsStatic = (isStatic == "true");
                                structProp.IsReadonly = (memberReadonly == "true");

                                if (string.IsNullOrEmpty(structProp.NativeType))
                                {
                                    structProp.NativeType = memberType;
                                }

                                structType.AddProperty(structProp);
                            }
                        }

                        TypeManager.Instance.Structs.Add(structType);
                    }
                }
            }
            catch (Exception)
            {
                string errorInfo = string.Format("errors when loading custom types");
                MessageBox.Show(errorInfo, "Loading Meta", MessageBoxButtons.OK);
            }
        }

        private bool _isBlackboardDirty = false;
        public bool IsBlackboardDirty
        {
            get
            {
                return _isBlackboardDirty;
            }
            set
            {
                _isBlackboardDirty = value;
            }
        }

        private static System.IO.FileSystemWatcher _fileWatcher = new FileSystemWatcher();

        public static void EnableFileWatcher(bool bEnable)
        {
            _fileWatcher.EnableRaisingEvents = bEnable;
        }

        public static void InitFileWatcher(string folder, FileSystemEventHandler eventHandler)
        {
            _fileWatcher.Path = folder;
            _fileWatcher.Filter = "*.xml";
            _fileWatcher.IncludeSubdirectories = true;
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            //_fileWatcher.SynchronizingObject = behaviorTreeList;

            _fileWatcher.Changed -= eventHandler;
            _fileWatcher.Changed += eventHandler;

            EnableFileWatcher(true);
        }

        public static bool SaveMeta(Workspace ws)
        {
            string metaPath = ws.getSourceMetaPath(true);

            try
            {
                string metaDir = Path.GetDirectoryName(metaPath);
                if (!Directory.Exists(metaDir))
                {
                    Directory.CreateDirectory(metaDir);
                }

                XmlDocument metaFile = new XmlDocument();
                FileManagers.SaveResult result = FileManagers.FileManager.MakeWritable(metaPath, Resources.SaveFileWarning);

                if (FileManagers.SaveResult.Succeeded != result)
                {
                    return false;
                }

                XmlDeclaration declaration = metaFile.CreateXmlDeclaration("1.0", "utf-8", null);
                metaFile.AppendChild(declaration);

                XmlElement meta = metaFile.CreateElement("meta");
                metaFile.AppendChild(meta);

                SaveTypes(metaFile, meta);
                SaveAgents(metaFile, meta);

                EnableFileWatcher(false);

                metaFile.Save(metaPath);

                EnableFileWatcher(true);

                ws.IsBlackboardDirty = false;

                return true;
            }
            catch (Exception ex)
            {
                string msgError = string.Format(Resources.SaveFileError, metaPath, ex.Message);
                MessageBox.Show(msgError, Resources.SaveError, MessageBoxButtons.OK);
            }

            EnableFileWatcher(true);

            return false;
        }

        private static void SaveAgents(XmlDocument bbfile, XmlNode meta)
        {
            XmlElement root = bbfile.CreateElement("agents");
            meta.AppendChild(root);

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                XmlElement bbEle = bbfile.CreateElement("agent");
                bbEle.SetAttribute("classfullname", agent.Name);

                if (agent.Base != null)
                {
                    bbEle.SetAttribute("base", agent.Base.Name);
                }

                if (!string.IsNullOrEmpty(agent.OldName) && agent.OldName != agent.Name)
                {
                    bbEle.SetAttribute("OldName", agent.OldName);
                }

                bbEle.SetAttribute("DisplayName", agent.DisplayName);
                bbEle.SetAttribute("Desc", agent.Description);
                bbEle.SetAttribute("IsRefType", "true");

                if (agent.IsStatic)
                {
                    bbEle.SetAttribute("IsStatic", "true");
                }

                if (agent.IsCustomized)
                {
                    bbEle.SetAttribute("IsCustomized", "true");
                }

                if (agent.IsImplemented)
                {
                    bbEle.SetAttribute("IsImplemented", "true");
                }

                if (!string.IsNullOrEmpty(agent.ExportLocation))
                {
                    bbEle.SetAttribute("ExportLocation", agent.ExportLocation);
                }

                foreach (PropertyDef prop in agent.GetProperties())
                {
                    if (prop.IsArrayElement || prop.IsPar || prop.IsInherited)
                    {
                        continue;
                    }

                    XmlElement propEle = bbfile.CreateElement("Member");

                    propEle.SetAttribute("Name", prop.BasicName);
                    propEle.SetAttribute("DisplayName", prop.DisplayName);
                    propEle.SetAttribute("Desc", prop.BasicDescription);
                    propEle.SetAttribute("Class", prop.ClassName);
                    propEle.SetAttribute("Type", prop.NativeType);
                    propEle.SetAttribute("TypeFullName", (prop.Type != null) ? prop.Type.FullName : prop.NativeType);

                    if (prop.IsCustomized)
                    {
                        propEle.SetAttribute("IsCustomized", "true");
                    }

                    propEle.SetAttribute("defaultvalue", prop.DefaultValue);
                    propEle.SetAttribute("Static", prop.IsStatic ? "true" : "false");
                    propEle.SetAttribute("Public", prop.IsPublic ? "true" : "false");
                    propEle.SetAttribute("Readonly", prop.IsReadonly ? "true" : "false");

                    bbEle.AppendChild(propEle);
                }

                foreach (MethodDef method in agent.GetMethods())
                {
                    if (method.IsInherited)
                    {
                        continue;
                    }

                    XmlElement methodEle = bbfile.CreateElement("Method");

                    methodEle.SetAttribute("Name", method.BasicName);
                    methodEle.SetAttribute("DisplayName", method.DisplayName);
                    methodEle.SetAttribute("Desc", method.BasicDescription);
                    methodEle.SetAttribute("Class", method.ClassName);
                    methodEle.SetAttribute("ReturnType", method.NativeReturnType);
                    methodEle.SetAttribute("ReturnTypeFullName", method.ReturnType.FullName);
                    methodEle.SetAttribute("Static", method.IsStatic ? "true" : "false");
                    methodEle.SetAttribute("Public", method.IsPublic ? "true" : "false");
                    methodEle.SetAttribute("istask", (method.MemberType == MemberType.Task) ? "true" : "false");
                    //methodEle.SetAttribute("isevent", (method.IsNamedEvent || method.MemberType == MemberType.Task) ? "true" : "false");

                    foreach (MethodDef.Param param in method.Params)
                    {
                        XmlElement paramEle = bbfile.CreateElement("Param");

                        paramEle.SetAttribute("Name", param.Name);
                        paramEle.SetAttribute("Type", param.NativeType);
                        paramEle.SetAttribute("TypeFullName", (param.Type != null) ? param.Type.FullName : param.NativeType);

                        if (param.IsOut)
                        {
                            paramEle.SetAttribute("IsOut", "true");
                        }

                        if (param.IsRef)
                        {
                            paramEle.SetAttribute("IsRef", "true");
                        }

                        if (param.IsConst)
                        {
                            paramEle.SetAttribute("IsConst", "true");
                        }

                        paramEle.SetAttribute("DisplayName", param.DisplayName);
                        paramEle.SetAttribute("Desc", param.Description);

                        methodEle.AppendChild(paramEle);
                    }

                    bbEle.AppendChild(methodEle);
                }

                root.AppendChild(bbEle);
            }

            XmlElement instanceRoot = bbfile.CreateElement("instances");
            meta.AppendChild(instanceRoot);

            foreach (Plugin.InstanceName_t instance in Plugin.InstanceNames)
            {
                XmlElement instanceEle = bbfile.CreateElement("instance");
                instanceEle.SetAttribute("name", instance.Name);
                instanceEle.SetAttribute("class", instance.ClassName);
                instanceEle.SetAttribute("DisplayName", instance.DisplayName);
                instanceEle.SetAttribute("Desc", instance.Desc);

                instanceRoot.AppendChild(instanceEle);
            }
        }

        private static void SaveTypes(XmlDocument bbfile, XmlNode meta)
        {
            XmlElement root = bbfile.CreateElement("types");
            meta.AppendChild(root);

            foreach (EnumType enumType in TypeManager.Instance.Enums)
            {
                string enumFullname = enumType.Fullname;

                XmlElement enumEle = bbfile.CreateElement("enumtype");
                enumEle.SetAttribute("Type", enumFullname);
                enumEle.SetAttribute("Namespace", enumType.Namespace);

                if (enumType.IsCustomized)
                {
                    enumEle.SetAttribute("IsCustomized", "true");
                }

                if (enumType.IsImplemented)
                {
                    enumEle.SetAttribute("IsImplemented", "true");
                }

                if (!string.IsNullOrEmpty(enumType.ExportLocation))
                {
                    enumEle.SetAttribute("ExportLocation", enumType.ExportLocation);
                }

                enumEle.SetAttribute("DisplayName", enumType.DisplayName);
                enumEle.SetAttribute("Desc", enumType.Description);

                foreach (EnumType.EnumMemberType member in enumType.Members)
                {
                    XmlElement memberEle = bbfile.CreateElement("enum");

                    member.Namespace = enumType.Namespace;
                    if (!string.IsNullOrEmpty(enumType.Namespace) && !member.NativeValue.Contains("::"))
                    {
                        member.NativeValue = enumType.Namespace + "::" + member.NativeValue;
                    }

                    memberEle.SetAttribute("NativeValue", member.NativeValue);
                    memberEle.SetAttribute("Value", member.Name);
                    memberEle.SetAttribute("DisplayName", member.DisplayName);
                    memberEle.SetAttribute("Desc", member.Description);
                    memberEle.SetAttribute("MemberValue", member.Value.ToString());

                    enumEle.AppendChild(memberEle);
                }

                root.AppendChild(enumEle);
            }

            foreach (StructType structType in TypeManager.Instance.Structs)
            {
                string structFullname = structType.Fullname;

                XmlElement structEle = bbfile.CreateElement("struct");

                structEle.SetAttribute("Type", structFullname);
                structEle.SetAttribute("Namespace", structType.Namespace);
                structEle.SetAttribute("Base", structType.BaseName);

                if (structType.IsRef)
                {
                    structEle.SetAttribute("IsRefType", "true");
                }

                if (structType.IsCustomized)
                {
                    structEle.SetAttribute("IsCustomized", "true");
                }

                if (structType.IsImplemented)
                {
                    structEle.SetAttribute("IsImplemented", "true");
                }

                if (!string.IsNullOrEmpty(structType.ExportLocation))
                {
                    structEle.SetAttribute("ExportLocation", structType.ExportLocation);
                }

                structEle.SetAttribute("DisplayName", structType.DisplayName);
                structEle.SetAttribute("Desc", structType.Description);

                foreach (PropertyDef member in structType.Properties)
                {
                    XmlElement memberEle = bbfile.CreateElement("Member");

                    memberEle.SetAttribute("Name", member.BasicName);
                    memberEle.SetAttribute("DisplayName", member.DisplayName);
                    memberEle.SetAttribute("Desc", member.BasicDescription);

                    Debug.Check(member.Type != null);
                    memberEle.SetAttribute("Type", member.NativeType);
                    memberEle.SetAttribute("TypeFullName", member.Type.FullName);

                    memberEle.SetAttribute("Class", structFullname);
                    memberEle.SetAttribute("Public", "true");

                    if (member.IsReadonly)
                    {
                        memberEle.SetAttribute("Readonly", "true");
                    }

                    structEle.AppendChild(memberEle);
                }

                root.AppendChild(structEle);
            }
        }

        public static void ExportMeta(Workspace ws, bool exportXML, bool exportBson)
        {
            SaveMeta(ws);

            if (exportXML || !exportBson)
            {
                ExportXmlMeta(ws);
            }

            if (exportBson)
            {
                ExportBsonMeta(ws);
            }
        }

        private static bool ExportXmlMeta(Workspace ws)
        {
            string metaFilename = ws.getExportMetaXMLPath();

            try
            {
                FileManagers.SaveResult result = FileManagers.FileManager.MakeWritable(metaFilename, Resources.SaveFileWarning);

                if (FileManagers.SaveResult.Succeeded != result)
                {
                    return false;
                }

                using(StreamWriter file = new StreamWriter(metaFilename))
                {
                    XmlWriterSettings xmlWs = new XmlWriterSettings();
                    xmlWs.Indent = true;
                    //xmlWs.OmitXmlDeclaration = true;

                    using(XmlWriter xmlWrtier = XmlWriter.Create(file, xmlWs))
                    {
                        xmlWrtier.WriteStartDocument();

                        xmlWrtier.WriteComment("EXPORTED BY TOOL, DON'T MODIFY IT!");
                        //bbfile.WriteComment("Source File: " + behavior.MakeRelative(behavior.FileManager.Filename));

                        xmlWrtier.WriteStartElement("agents");
                        xmlWrtier.WriteAttributeString("version", "1");
                        xmlWrtier.WriteAttributeString("signature", CRC32.CalcCRC(Plugin.Signature).ToString());

                        foreach (AgentType agent in Plugin.AgentTypes)
                        {
                            bool hasProperty = false;

                            foreach (PropertyDef prop in agent.GetProperties(true))
                            {
                                // skip array element
                                if (prop.IsArrayElement || prop.IsPar)
                                {
                                    continue;
                                }

                                hasProperty = true;
                                break;
                            }

                            if (hasProperty)
                            {
                                xmlWrtier.WriteStartElement("agent");

                                xmlWrtier.WriteAttributeString("type", agent.Name);

                                if (agent.Base != null)
                                {
                                    xmlWrtier.WriteAttributeString("base", agent.Base.Name);
                                }

                                xmlWrtier.WriteAttributeString("signature", CRC32.CalcCRC(agent.GetSignature(true)).ToString());

                                {
                                    xmlWrtier.WriteStartElement("properties");

                                    foreach (PropertyDef prop in agent.GetProperties(true))
                                    {
                                        //skip array element
                                        if (prop.IsArrayElement || prop.IsPar)
                                        {
                                            continue;
                                        }

                                        xmlWrtier.WriteStartElement("property");

                                        xmlWrtier.WriteAttributeString("name", prop.BasicName);
                                        xmlWrtier.WriteAttributeString("type", Plugin.GetNativeTypeName(prop.NativeType));
                                        xmlWrtier.WriteAttributeString("member", prop.IsMember ? "true" : "false");
                                        xmlWrtier.WriteAttributeString("static", prop.IsStatic ? "true" : "false");
                                        //xmlWrtier.WriteAttributeString("public", prop.IsPublic ? "true" : "false");

                                        if (prop.IsMember)
                                        {
                                            xmlWrtier.WriteAttributeString("agent", prop.ClassName);
                                        }
                                        else
                                        {
                                            xmlWrtier.WriteAttributeString("defaultvalue", prop.DefaultValue);
                                        }

                                        xmlWrtier.WriteEndElement();
                                    }

                                    xmlWrtier.WriteEndElement();
                                }

                                //end of agent
                                xmlWrtier.WriteEndElement();
                            }
                        }

                        //end of agents
                        xmlWrtier.WriteEndElement();

                        xmlWrtier.WriteEndDocument();
                    }

                    file.Close();
                }

                return true;

            }
            catch (Exception ex)
            {
                string msgError = string.Format(Resources.SaveFileError, metaFilename, ex.Message);
                MessageBox.Show(msgError, Resources.SaveError, MessageBoxButtons.OK);
            }

            return false;
        }

        private static bool ExportBsonMeta(Workspace ws)
        {
            string metaFilename = ws.getExportMetaBsonPath();

            try
            {
                FileManagers.SaveResult result = FileManagers.FileManager.MakeWritable(metaFilename, Resources.SaveFileWarning);

                if (FileManagers.SaveResult.Succeeded != result)
                {
                    return false;
                }

                using(var ms = new MemoryStream())
                using(var writer = new BinaryWriter(ms))
                {
                    BsonSerializer serializer = BsonSerializer.CreateSerialize(writer);
                    serializer.WriteStartDocument();

                    serializer.WriteComment("EXPORTED BY TOOL, DON'T MODIFY IT!");

                    serializer.WriteStartElement("agents");
                    serializer.WriteString("1"); // version
                    serializer.WriteString(CRC32.CalcCRC(Plugin.Signature).ToString()); // signature

                    foreach (AgentType agent in Plugin.AgentTypes)
                    {
                        bool hasProperty = false;

                        foreach (PropertyDef prop in agent.GetProperties(true))
                        {
                            // skip array element
                            if (prop.IsArrayElement || prop.IsPar)
                            {
                                continue;
                            }

                            hasProperty = true;
                            break;
                        }

                        if (hasProperty)
                        {
                            serializer.WriteStartElement("agent");

                            serializer.WriteString(agent.Name);

                            // base
                            if (agent.Base != null)
                            {
                                serializer.WriteString(agent.Base.Name);
                            }
                            else
                            {
                                serializer.WriteString("none");
                            }

                            // signature
                            serializer.WriteString(CRC32.CalcCRC(agent.GetSignature(true)).ToString());

                            {
                                serializer.WriteStartElement("properties");

                                foreach (PropertyDef prop in agent.GetProperties(true))
                                {
                                    // skip array element
                                    if (prop.IsArrayElement || prop.IsPar)
                                    {
                                        continue;
                                    }

                                    serializer.WriteStartElement("property");

                                    serializer.WriteString(prop.BasicName);
                                    serializer.WriteString(Plugin.GetNativeTypeName(prop.NativeType));
                                    serializer.WriteString(prop.IsMember ? "true" : "false");
                                    serializer.WriteString(prop.IsStatic ? "true" : "false");
                                    //serializer.WriteString(prop.IsPublic ? "true" : "false");

                                    if (prop.IsMember)
                                    {
                                        serializer.WriteString(prop.ClassName);
                                    }
                                    else
                                    {
                                        serializer.WriteString(prop.DefaultValue);
                                    }

                                    serializer.WriteEndElement();
                                }

                                serializer.WriteEndElement();
                            }

                            //end of agent
                            serializer.WriteEndElement();
                        }
                        else
                        {
                            //serializer.WriteTypeNone();
                        }
                    }

                    //end of 'agents'
                    serializer.WriteEndElement();

                    serializer.WriteEndDocument();

                    using(FileStream fs = File.Create(metaFilename))
                    {
                        using(BinaryWriter w = new BinaryWriter(fs))
                        {
                            byte[] d = ms.ToArray();

                            w.Write(d);
                            fs.Close();
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string msgError = string.Format(Resources.SaveFileError, metaFilename, ex.Message);
                MessageBox.Show(msgError, Resources.SaveError, MessageBoxButtons.OK);
            }

            return false;
        }
    }
}
