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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Reflection;
using Behaviac.Design.Nodes;
using Behaviac.Design.Data;
using Behaviac.Design.Importers;
using Behaviac.Design.Network;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    /// <summary>
    /// This control manages all the behaviors which are loaded and saved.
    /// </summary>
    internal partial class BehaviorTreeList : UserControl, BehaviorManagerInterface
    {
        public BehaviorTreeList()
        {
            InitializeComponent();

            BehaviorManager.Instance = this;

            if (Settings.Default.CheckLatestVersion)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(CheckVersionAsync));
                t.Start();
            }

            this.Disposed += new EventHandler(BehaviorTreeList_Disposed);
        }

        void BehaviorTreeList_Disposed(object sender, EventArgs e)
        {
            this.Disposed -= BehaviorTreeList_Disposed;

            if (this.toolStrip != null)
            {
                this.toolStrip.Dispose();
                this.toolStrip = null;
            }

            if (this.openFileDialog != null)
            {
                this.openFileDialog.Dispose();
                this.openFileDialog = null;
            }
        }

        internal void UpdateUIState(EditModes editMode)
        {
            bool enabled = (editMode == EditModes.Design);
            this.toolStripButton_workspace.Enabled = enabled;
            this.refreshButton.Enabled = enabled;
            this.newBehaviorButton.Enabled = enabled;
            this.createGroupButton.Enabled = enabled;
            this.deleteButton.Enabled = enabled;
            this.exportAllButton.Enabled = enabled;
            //this.treeView.Enabled = enabled;

            switch (editMode)
            {
                case EditModes.Design:
                    this.connectButton.Text = Resources.ConnectServer + " (Ctrl+L)";
                    this.connectButton.Enabled = true;

                    this.analyzeDumpButton.Text = Resources.AnalyzeDump;
                    this.analyzeDumpButton.Image = Resources.File_Open;
                    this.analyzeDumpButton.Enabled = true;
                    break;

                case EditModes.Connect:
                    this.connectButton.Text = Resources.DisconnectServer + " (Ctrl+L)";
                    this.connectButton.Image = Resources.disconnect;
                    this.connectButton.Enabled = true;

                    this.analyzeDumpButton.Image = Resources.File_Open;
                    this.analyzeDumpButton.Enabled = false;

                    break;

                case EditModes.Analyze:
                    this.connectButton.Enabled = false;

                    this.analyzeDumpButton.Text = Resources.StopAnalyzingDump;
                    this.analyzeDumpButton.Image = Resources.File_Delete;
                    this.analyzeDumpButton.Enabled = true;
                    break;
            }
        }

        /// <summary>
        /// All the file managers which were loaded.
        /// </summary>
        private List<FileManagerInfo> _fileManagers = new List<FileManagerInfo>();
        public IList<FileManagerInfo> GetFileManagers()
        {
            return _fileManagers;
        }

        internal void UnLoadPlugins(NodeTreeList nodeTreeList)
        {
            Plugin.UnLoadPlugins();

            _fileManagers.Clear();

            treeView.Nodes.Clear();

            if (nodeTreeList != null)
            {
                nodeTreeList.Clear();
            }
        }

        private void RegisterPlugin(Assembly assembly, Plugin plugin, ImageList imageList, bool bAddNodes)
        {
            // register the plugin
            Plugin.AddLoadedPlugin(assembly);
            Plugin.RegisterTypeHandlers(assembly);
            Plugin.RegisterAgentTypes(assembly);

            // register all the groups we need in the node explorer
            if (bAddNodes && imageList != null)
            {
                Plugin.RegisterNodeDesc(assembly, imageList.Images.Count);
            }
            else
            {
                Plugin.RegisterNodeDesc(assembly);
            }

            // add all file managers, exporters and AI types
            _fileManagers.AddRange(plugin.FileManagers);
        }

        /// <summary>
        /// Loads all plugins form a directory.
        /// </summary>
        /// <param name="path">The directory which is the root for the given list of plugins.</param>
        internal void LoadPlugins(NodeTreeList nodeTreeList, bool bAddNodes)
        {
            if (nodeTreeList != null)
            {
                Plugin.LoadPlugins(RegisterPlugin, nodeTreeList.ImageList, bAddNodes);

                if (bAddNodes)
                {
                    // create the tree nodes for all behavior nodes.
                    nodeTreeList.ToggleShowSelectedNodes(Plugin.OnlyShowFrequentlyUsedNodes, nodeTreeList.TreeView);
                }
            }

            // update labels
            newBehaviorButton.Text = Plugin.GetResourceString("NewBehavior") + " (Ctrl+N)";
            //_behaviorGroupName = Plugin.GetResourceString("BehaviorGroupName");
            //_prefabGroupName = Plugin.GetResourceString("PrefabGroupName");
        }

        internal void UnloadMeta()
        {
            Plugin.UnRegisterTypeHandlers();
            Plugin.UnRegisterAgentTypes();
        }

        internal void LoadMeta(string metaFile)
        {
            bool shouldSaveMeta = Workspace.PreLoadMeta(Workspace.Current);

            string dllFilename = ImporterXML.ImportXML(metaFile);
            bool bMerged = false;

            if (!string.IsNullOrEmpty(dllFilename))
            {
                Assembly assembly = Assembly.LoadFile(dllFilename);

                Plugin.AddLoadedPlugin(assembly);

                Plugin.RegisterTypeHandlers(assembly);
                Plugin.RegisterAgentTypes(assembly);

                Plugin.PrepareInstanceTypes();

                if (!string.IsNullOrEmpty(Workspace.Current.MetaFilename) && File.Exists(Workspace.Current.MetaFilename)
                    || Workspace.Current.Version == 0)
                {
                    try
                    {
                        System.Diagnostics.Process.Start("http://www.behaviac.com/updatenotes/#36x");
                    }
                    catch
                    {
                    }

                    //if (DialogResult.OK == MessageBox.Show(Resources.MetaWarning, Resources.LoadWarning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                    if (DialogResult.OK == MessageBox.Show(Resources.MetaWarning, Resources.LoadWarning, MessageBoxButtons.OK))
                    {
                        bMerged = true;

                        foreach (AgentType agent in Plugin.AgentTypes)
                        {
                            if (agent.Base != null)
                            {
                                Debug.Check(Plugin.AgentTypes.Count > 0 && agent != Plugin.AgentTypes[0]);
                                agent.IsImplemented = true;
                            }
                        }

                        foreach (EnumType enumType in TypeManager.Instance.Enums)
                        {
                            enumType.IsImplemented = true;
                        }

                        foreach (StructType structType in TypeManager.Instance.Structs)
                        {
                            structType.IsImplemented = true;
                        }

                        Workspace.LoadMeta(null);
                        Workspace.SaveMeta(Workspace.Current);
                        shouldSaveMeta = false;

                        Workspace.Current.MetaFilename = "";
                        Workspace.Current.Version = Workspace.CurrentVersion;
                        Workspace.SaveWorkspaceFile(Workspace.Current);

                        MetaStoreDock.Inspect(null);
                    }
                }
            }

            if (!bMerged)
            {
                Workspace.LoadMeta(null);

                if (shouldSaveMeta)
                {
                    Workspace.SaveMeta(Workspace.Current);
                }
            }
        }

        private void SetAllTypes()
        {
        }

        /// <summary>
        /// Return true when there are any file managers loaded.
        /// </summary>
        /// <returns></returns>
        internal bool HasFileManagers()
        {
            return _fileManagers.Count > 0;
        }

        /// <summary>
        /// Returns true if there are any exporters loaded.
        /// </summary>
        /// <returns></returns>
        internal bool HasExporters()
        {
            return Plugin.Exporters.Count > 0;
        }

        private List<BehaviorNode> _loadedBehaviors = new List<BehaviorNode>();
        public List<BehaviorNode> LoadedBehaviors
        {
            get
            {
                return _loadedBehaviors;
            }
        }

        private List<BehaviorNode> _newBehaviors = new List<BehaviorNode>();
        public List<BehaviorNode> NewBehaviors
        {
            get
            {
                return _newBehaviors;
            }
        }

        public IList<Nodes.BehaviorNode> GetAllOpenedBehaviors()
        {
            List<Nodes.BehaviorNode> behaviors = new List<Nodes.BehaviorNode>();
            behaviors.AddRange(_loadedBehaviors);
            behaviors.AddRange(_newBehaviors);

            return behaviors.AsReadOnly();
        }

        private string _behaviorFolder = string.Empty;

        /// <summary>
        /// The folder which is searched for behaviors.
        /// </summary>
        public string BehaviorFolder
        {
            get
            {
                return _behaviorFolder;
            }
            set
            {
                ChangeBehaviorFolder(value);
            }
        }

        private TreeNode getNodeGroup(TreeNodeCollection list, string name, string root, NodeTagType nodeTagType)
        {
            // search for an existing behavior group
            foreach (TreeNode node in list)
            {
                if (node.Text == name)
                {
                    return node;
                }
            }

            // create a new group
            TreeNode newnode = new TreeNode(name, (int)NodeIcon.FolderClosed, (int)NodeIcon.FolderClosed);
            newnode.Tag = new NodeTag(nodeTagType, null, root);
            list.Add(newnode);

            return newnode;
        }

        /// <summary>
        /// Returns a behavior group with a specific name. If none exists, it is created.
        /// </summary>
        /// <param name="list">The list which is earched for this behavior tree node.</param>
        /// <param name="name">The name of the node you want to find or create.</param>
        /// <param name="root">The path of the directory this behavior folder represents.</param>
        /// <returns></returns>
        private TreeNode GetBehaviorGroup(TreeNodeCollection list, string name, string root)
        {
            return getNodeGroup(list, name, root, NodeTagType.BehaviorFolder);
        }

        private TreeNode GetPrefabGroup(TreeNodeCollection list, string name, string root)
        {
            return getNodeGroup(list, name, root, NodeTagType.PrefabFolder);
        }

        /// <summary>
        /// The name of the root folder shown in the node explorer.
        /// </summary>
        private string _behaviorGroupName = "Behaviors";

        private string _prefabGroupName = "Prefabs";

        /// <summary>
        /// Creates a behavior group for a given folder or file.
        /// </summary>
        /// <param name="filename">The file or folder you want to create the behavior group for.</param>
        /// <param name="isFile">Determines if filename is a folder or a file.</param>
        /// <returns>Returns null if a tree node could not be created.</returns>
        private TreeNode GetBehaviorGroup(string filename, bool isFile)
        {
            // separate filename and behavior folder
            string name = filename.Substring(_behaviorFolder.Length + 1);
            string folder = _behaviorFolder;

            // get the different groups
            string[] groups = name.Split('\\');

            if (isFile && groups.Length < 2)
            {
                // if this is a file with no subfolder, no group needs to be created
                return null;
            }

            // get the group for the behaviors
            TreeNodeCollection list = GetBehaviorGroup(treeView.Nodes, _behaviorGroupName, _behaviorFolder).Nodes;

            TreeNode group = null;
            int count = isFile ? groups.Length - 1 : groups.Length;  // if this is a file, skip the filename

            // create a tree node for each folder
            for (int i = 0; i < count; ++i)
            {
                // update the directory
                folder += '\\' + groups[i];

                // create the behavior group
                group = GetBehaviorGroup(list, groups[i], folder);

                if (group == null)
                {
                    return null;
                }

                else
                {
                    list = group.Nodes;
                }
            }

            return group;
        }

        /// <summary>
        /// Returns the tree node of a behavior. For internal use only.
        /// </summary>
        /// <param name="node">The node you want to search for the node of a behavior.</param>
        /// <param name="identifier">The label or filename of the behavior you are looking for.</param>
        /// <param name="isFilename">Determines if the identifier is a filename or a label.</param>
        /// <returns>Returns null if no tree node for the behavior could be found.</returns>
        private TreeNode GetBehaviorNode(TreeNode node, string identifier, bool isFilename)
        {
            NodeTag nodetag = (NodeTag)node.Tag;

            if (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.Prefab)
            {
                if (isFilename)
                {
                    if (identifier == nodetag.Filename)
                    {
                        return node;
                    }

                }
                else
                {
                    if (identifier == node.Text)
                    {
                        return node;
                    }
                }
            }

            foreach (TreeNode child in node.Nodes)
            {
                TreeNode res = GetBehaviorNode(child, identifier, isFilename);

                if (res != null)
                {
                    return res;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the tree node for a behavior.
        /// </summary>
        /// <param name="identifier">The label or filename of the behavior you are looking for.</param>
        /// <param name="isFilename">Determines if the identifier is a filename or a label.</param>
        /// <returns>Returns null if no tree node for the behavior could be found.</returns>
        private TreeNode GetBehaviorNode(string identifier, bool isFilename)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return null;
            }

            // Try to get the behavior node.
            TreeNode behaviors = GetBehaviorGroup(treeView.Nodes, _behaviorGroupName, _behaviorFolder);
            TreeNode node = GetBehaviorNode(behaviors, identifier, isFilename);

            // If not existing, then try to get the prefab node.
            if (node == null)
            {
                string prefabFolder = GetPrefabFolder();
                if (Directory.Exists(prefabFolder))
                {
                    behaviors = GetBehaviorGroup(treeView.Nodes, _prefabGroupName, GetPrefabFolder());
                    node = GetBehaviorNode(behaviors, identifier, isFilename);
                }
            }

            return node;
        }

        private bool IsPrefabBehavior(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return false;
            }

            string prefabFolder = GetPrefabFolder();
            if (Directory.Exists(prefabFolder))
            {
                TreeNode behaviors = GetBehaviorGroup(treeView.Nodes, _prefabGroupName, prefabFolder);
                TreeNode node = GetBehaviorNode(behaviors, filename, true);

                return node != null;
            }

            return false;
        }

        private void BuildBehaviorList(TreeNodeCollection treeList, string folder, List<FileManagerInfo> fileManagers, bool isBehavior)
        {
            // search all subfolders of the current folder
            string[] subfolders = Directory.GetDirectories(folder);

            foreach (string subfolder in subfolders)
            {
                string lastFolder = Path.GetFileNameWithoutExtension(subfolder);

                // we skip hidden and system folders
                if (lastFolder == "behaviac_meta" || (File.GetAttributes(subfolder) & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                {
                    continue;
                }

                // create a group node for the the current folder
                string nodeLabel = new DirectoryInfo(subfolder).Name;
                TreeNode groupNode = new TreeNode(nodeLabel, (int)NodeIcon.FolderClosed, (int)NodeIcon.FolderClosed);
                groupNode.Tag = new NodeTag(isBehavior ? NodeTagType.BehaviorFolder : NodeTagType.PrefabFolder, null, subfolder);
                treeList.Add(groupNode);

                BuildBehaviorList(groupNode.Nodes, subfolder, fileManagers, isBehavior);
            }

            // search the files of the current folder
            string[] foundFiles = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);

            foreach (string file in foundFiles)
            {
                // we only add files which can be loaded by a file manager
                bool hasFileManger = false;

                foreach (FileManagerInfo fileman in fileManagers)
                {
                    if (file.ToLowerInvariant().EndsWith(fileman.FileExtension) && !file.ToLowerInvariant().EndsWith(".bb.xml"))
                    {
                        hasFileManger = true;
                        break;
                    }
                }

                // if there is no filemanager for this file, we skip it
                if (!hasFileManger)
                {
                    continue;
                }

                // we skip hidden and system files
                if ((File.GetAttributes(file) & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                {
                    continue;
                }

                // create a tree node for the file
                string nodeLabel = Path.GetFileNameWithoutExtension(file);
                int iconIndex = isBehavior ? ((GetBehavior(file) == null) ? (int)NodeIcon.Behavior : (int)NodeIcon.BehaviorLoaded) : (int)NodeIcon.Prefab;
                TreeNode behaviorNode = new TreeNode(nodeLabel, iconIndex, iconIndex);
                behaviorNode.Tag = new NodeTag(isBehavior ? NodeTagType.Behavior : NodeTagType.Prefab, Plugin.BehaviorNodeType, file);
                treeList.Add(behaviorNode);
            }
        }

        internal string GetPrefabFolder()
        {
            if (string.IsNullOrEmpty(_behaviorFolder))
            {
                return string.Empty;
            }

            DirectoryInfo parentDir = Directory.GetParent(_behaviorFolder);
            if (parentDir != null)
            {
                string folder = parentDir.FullName;
                string prefabFolder = Path.Combine(folder, _prefabGroupName);

                // This is a patch for the Chinese filename of the prefabs.
                if (!Directory.Exists(prefabFolder))
                {
                    string prefabGroup = Plugin.GetResourceString("PrefabGroupName");
                    prefabGroup = Path.Combine(folder, prefabGroup);

                    if (Directory.Exists(prefabGroup))
                    {
                        prefabFolder = prefabGroup;
                    }
                }

                return prefabFolder;
            }

            Debug.Check(false);

            return string.Empty;
        }

        /// <summary>
        /// Generates a new list of the behaviors.
        /// </summary>
        internal void RebuildBehaviorList()
        {
            try
            {
                // check if we have a valid folder
                if (string.IsNullOrEmpty(_behaviorFolder))
                {
                    return;
                }

                // create the folder if it does not exist
                if (!Directory.Exists(_behaviorFolder))
                {
                    Directory.CreateDirectory(_behaviorFolder);
                }

                // get the group for the behavior and clear it from the old ones.
                TreeNode behaviorTreeNode = GetBehaviorGroup(treeView.Nodes, _behaviorGroupName, _behaviorFolder);
                TreeNodeCollection behaviors = behaviorTreeNode.Nodes;
                behaviors.Clear();

                // build the behavior list
                BuildBehaviorList(behaviors, _behaviorFolder, _fileManagers, true);

                // get the group for the prefab and clear it from the old ones.
                string prefabFolder = GetPrefabFolder();

                if (Directory.Exists(prefabFolder))
                {
                    TreeNode prefabTreeNode = GetPrefabGroup(treeView.Nodes, _prefabGroupName, prefabFolder);
                    TreeNodeCollection prefabs = prefabTreeNode.Nodes;
                    prefabs.Clear();

                    // build the prefab list
                    BuildBehaviorList(prefabs, prefabFolder, _fileManagers, false);
                }

                // expand the behaviors
                //treeView.ExpandAll();

                foreach (BehaviorNode behavior in this.GetAllOpenedBehaviors())
                {
                    if (behavior.IsModified)
                    {
                        behavior.TriggerWasModified(behavior as Node);
                    }
                }

                treeView.SelectedNode = null;

                if (treeView.GetNodeCount(false) > 0)
                {
                    treeView.SelectedNode = treeView.Nodes[0];
                    treeView.SelectedNode.Expand();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Adds all tree nodes in the pool and its children to a list.
        /// </summary>
        /// <param name="pool">The pool you want to search.</param>
        /// <param name="list">The list the tree nodes are added to.</param>
        private void AddChildNodes(TreeNodeCollection pool, List<TreeNode> list)
        {
            foreach (TreeNode node in pool)
            {
                list.Add(node);

                AddChildNodes(node.Nodes, list);
            }
        }

        /// <summary>
        /// Adds all tree nodes in the pool and its children of a specific type to a list.
        /// </summary>
        /// <param name="pool">The pool you want to search.</param>
        /// <param name="list">The list the tree nodes are added to.</param>
        /// <param name="type">The type of nodes you want to get.</param>
        private void AddChildNodes(TreeNodeCollection pool, List<TreeNode> list, NodeTagType type)
        {
            foreach (TreeNode node in pool)
            {
                if (((NodeTag)node.Tag).Type == type)
                {
                    list.Add(node);
                }

                AddChildNodes(node.Nodes, list, type);
            }
        }

        /// <summary>
        /// Generates a label with a unique name.
        /// </summary>
        /// <param name="label">The name you want to generate a unique one from.</param>
        /// <param name="start">The first number you want to be added to the name.</param>
        /// <param name="used">Returns the number being added to the name.</param>
        /// <returns>Returns the unique name.</returns>
        private string GetUniqueLabel(string label, int start, out int used)
        {
            int i = start;

            // gather all tree nodes
            List<TreeNode> nodes = new List<TreeNode>();
            AddChildNodes(treeView.Nodes, nodes);

            // if there is no tree node, simply output the first available name
            if (nodes.Count < 1)
            {
                used = i;
                return string.Format("{0}_{1}", label, i);
            }

            do
            {
                // generate the new label
                string newlabel = string.Format("{0}_{1}", label, i);

                // check if there is any node with this name
                bool found = false;

                foreach (TreeNode node in nodes)
                {
                    if (node.Text == newlabel)
                    {
                        found = true;
                        break;
                    }
                }

                // if no node with the name exists, return it.
                if (!found)
                {
                    used = i;
                    return newlabel;
                }

                i++;
            }
            while (true);
        }

        public delegate BehaviorTreeViewDock ShowBehaviorEventDelegate(Nodes.BehaviorNode node);

        /// <summary>
        /// This event is triggered when double-clicking a behavior in the node explorer.
        /// </summary>
        public event ShowBehaviorEventDelegate ShowBehavior;

        /// <summary>
        /// Returns a behavior which has already been loaded.
        /// </summary>
        /// <param name="filename">Behaviour file to get the behavior for.</param>
        /// <returns>Returns null if the behavior is not loaded.</returns>
        public BehaviorNode GetBehavior(string filename)
        {
            foreach (BehaviorNode behavior in _loadedBehaviors)
            {
                if (behavior.Filename == filename)
                {
                    return behavior;
                }
            }

            foreach (BehaviorNode behavior in _newBehaviors)
            {
                if (behavior.Filename == filename)
                {
                    return behavior;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the behavior which is associated with a tree node.
        /// </summary>
        /// <param name="nodetag">The NodeTag of the tree node.</param>
        /// <param name="label">The label of the tree node.</param>
        /// <returns>Returns null if no matching behavior could be found or loaded.</returns>
        internal BehaviorNode GetBehavior(NodeTag nodetag, string label, List<Nodes.Node.ErrorCheck> result = null)
        {
            BehaviorNode behavior = LoadBehavior(nodetag.Filename, false, result);

            if (behavior != null)
            {
                return behavior;
            }

            MessageBox.Show(string.Format(Resources.NoBehaviorInfo, label, nodetag.Filename), Resources.FileError, MessageBoxButtons.OK);
            return null;
        }

        protected void RegisterLoadedBehaviorNode(BehaviorNode behavior)
        {
            Debug.Check(!_loadedBehaviors.Contains(behavior));

            _loadedBehaviors.Add(behavior);
        }

        private TreeNode GetTreeNode(BehaviorNode behavior)
        {
            if (behavior.FileManager == null)
            {
                return GetBehaviorNode(((Node)behavior).Label, false);
            }

            return GetBehaviorNode(behavior.Filename, true);
        }

        private void CheckNode(BehaviorNode behavior, TreeNode tnode)
        {
            if (Plugin.EditMode == EditModes.Design)
            {
                List<Node.ErrorCheck> result = new List<Node.ErrorCheck>();
                ((Behavior)behavior).CheckForErrors(behavior, result);

                if (tnode != null)
                {
                    tnode.ForeColor = (Plugin.GetErrorChecks(result).Count > 0) ? SystemColors.HotTrack : SystemColors.HighlightText;
                }
            }
        }

        private static List<string> metaCheckedWorkspaces = new List<string>();

        /// <summary>
        /// Loads the given behavior. If the behavior was already loaded, it is not loaded a second time.
        /// </summary>
        /// <param name="filename">Behaviour file to load.</param>
        /// <returns>Returns null if the behavior was not already loaded and could not be loaded.</returns>
        public BehaviorNode LoadBehavior(string filename, bool bForce = false, List<Nodes.Node.ErrorCheck> result = null)
        {
            // check if the behavior was already loaded.
            BehaviorNode behavior = GetBehavior(filename);

            if (behavior == null || bForce)
            {
                if (!metaCheckedWorkspaces.Contains(Workspace.Current.FileName))
                {
                    metaCheckedWorkspaces.Add(Workspace.Current.FileName);

                    if (Plugin.AgentTypes.Count <= 1)
                    {
                        MessageBox.Show(Resources.EmptyMetaWarning, Resources.Warning, MessageBoxButtons.OK);
                    }
                }

                FileManagers.FileManager filemanager = null;

                // search the file managers for the right one to load the given file
                foreach (FileManagerInfo info in _fileManagers)
                {
                    // check if the file manager could handle this file extension
                    if (filename.ToLowerInvariant().EndsWith(info.FileExtension))
                    {
                        try
                        {
                            // check if the file exists
                            if (!File.Exists(filename))
                            {
                                throw new Exception(string.Format(Resources.ExceptionNoSuchFile, filename));
                            }

                            // create a new file manager and load the behavior
                            filemanager = info.Create(filename, null);
                            filemanager.Load(result, RegisterLoadedBehaviorNode);
                            behavior = filemanager.Behavior;

                            if (behavior == null)
                            {
                                continue;
                            }

                            // register the WasSaved and WasModified events on the behavior
                            behavior.WasSaved += behavior_WasSaved;
                            behavior.WasModified += behavior_WasModified;

                            // assign the label of the behavior node
                            ((Node)behavior).Label = Path.GetFileNameWithoutExtension(filename);

                            // get the correct tree node
                            TreeNode tnode = (behavior.FileManager == null) ? GetBehaviorNode(((Node)behavior).Label, false) : GetBehaviorNode(behavior.Filename, true);

                            // change the behaviors icon to the loaded icon
                            if (tnode != null)
                            {
                                NodeTag nodetag = (NodeTag)tnode.Tag;

                                if (nodetag.Type == NodeTagType.Prefab)
                                {
                                    tnode.ImageIndex = (int)NodeIcon.Prefab;
                                    tnode.SelectedImageIndex = (int)NodeIcon.Prefab;

                                }
                                else
                                {
                                    tnode.ImageIndex = (int)NodeIcon.BehaviorLoaded;
                                    tnode.SelectedImageIndex = (int)NodeIcon.BehaviorLoaded;
                                }

                                nodetag.AssignLoadedBehavior(behavior);
                            }

                            //in behavior xml file, some node types might have been removed and IsModified is set to true in loading
                            if (behavior.IsModified)
                            {
                                behavior.TriggerWasModified(behavior as Node);
                            }

                            if (!bForce)
                            {
                                UndoManager.Save(behavior);
                            }
                        }
                        catch (Exception ex)
                        {
                            string errorInfo = string.Format("{0}\n{1}", filename, ex.Message);
                            MessageBox.Show(errorInfo, Resources.LoadError, MessageBoxButtons.OK);
                        }

                        break;
                    }
                }
            }

            // set the recent files
            if (MainWindow.Instance.RecentFilesMenu != null)
            {
                string file = FileManagers.FileManager.GetRelativePath(filename);
                MainWindow.Instance.RecentFilesMenu.AddEntry(file, false);

                int index = MainWindow.Instance.RecentFilesMenu.FindFilenameNumber(file);
                MainWindow.Instance.RecentFilesMenu.SetFirstFile(index);
            }

            if (behavior != null)
            {
                behavior.IsPrefab = IsPrefabBehavior(behavior.Filename);
            }

            return behavior;
        }

        public bool UnloadBehavior(string filename)
        {
            foreach (BehaviorNode behavior in _loadedBehaviors)
            {
                if (behavior.Filename == filename)
                {
                    UndoManager.Clear(behavior.Filename);

                    _loadedBehaviors.Remove(behavior);

                    return true;
                }
            }

            foreach (BehaviorNode behavior in _newBehaviors)
            {
                if (behavior.Filename == filename)
                {
                    UndoManager.Clear(behavior.Filename);

                    _newBehaviors.Remove(behavior);

                    return true;
                }
            }

            return false;
        }

        private BehaviorNode getBehaviorByTreeNode(TreeNode node, List<Nodes.Node.ErrorCheck> result = null)
        {
            BehaviorNode behavior = null;

            if (node != null)
            {
                NodeTag nodetag = (NodeTag)node.Tag;

                if (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.Prefab)
                {
                    behavior = GetBehavior(nodetag, node.Text, result);
                }
            }

            return behavior;
        }

        private static ErrorCheckDialog _errorDialog = null;

        public void OpenBehavior(TreeNode treenode, bool bShow)
        {
            if (treenode == null)
            {
                return;
            }

            List<Nodes.Node.ErrorCheck> result = new List<Node.ErrorCheck>();
            BehaviorNode behavior = getBehaviorByTreeNode(treenode, result);

            if (behavior != null)
            {
                if (bShow && ShowBehavior != null)
                {
                    ShowBehavior(behavior);
                }

                CheckNode(behavior, GetTreeNode(behavior));

                if (result.Count > 0 && BehaviorTreeViewDock.LastFocused != null)
                {
                    UIUtilities.ShowErrorDialog(ref _errorDialog, this, BehaviorTreeViewDock.LastFocused.BehaviorTreeView,
                                                this.ParentForm, Resources.LoadError, result);
                }

                Utilities.ReportLoadBehavior();
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenBehavior(e.Node, false);
        }

        /// <summary>
        /// Handles when a tree node in the node explorer is double-clicked.
        /// </summary>
        private void behaviorTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OpenBehavior(e.Node, true);
        }

        /// <summary>
        /// The number of the last newly created behavior.
        /// </summary>
        private int _lastNewBehavior = 1;

        public void NewBehavior()
        {
            if (Workspace.Current == null)
            {
                MessageBox.Show(Resources.NullWorkspace, Resources.Error, MessageBoxButtons.OK);
                return;
            }

            // create a new behavior node with a unique label
            string label = GetUniqueLabel("NewBehavior", _lastNewBehavior, out _lastNewBehavior);
            BehaviorNode behavior = Node.CreateBehaviorNode(label);
            behavior.TriggerWasModified(behavior as Node);

            // get updated when the behavior changes
            behavior.WasSaved += behavior_WasSaved;
            behavior.WasModified += behavior_WasModified;

            // mark behavior as being modified
            behavior.FileManager = null;

            // add the behavior to the list
            _newBehaviors.Add(behavior);

            // get the folder node for the new node.
            TreeNode folder = null;

            if (treeView.SelectedNode != null)
            {
                NodeTag nodetag = (NodeTag)treeView.SelectedNode.Tag;

                if (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.BehaviorFolder)
                {
                    folder = (nodetag.Type == NodeTagType.BehaviorFolder) ? treeView.SelectedNode : treeView.SelectedNode.Parent;
                }

                else if (nodetag.Type == NodeTagType.Prefab || nodetag.Type == NodeTagType.PrefabFolder)
                {
                    folder = (nodetag.Type == NodeTagType.PrefabFolder) ? treeView.SelectedNode : treeView.SelectedNode.Parent;
                }
            }

            // if the selected node can not be found, use the root folder defaultly.
            if (folder == null)
            {
                folder = GetBehaviorGroup(treeView.Nodes, _behaviorGroupName, _behaviorFolder);
            }

            // set the folder of the node.
            behavior.Folder = ((NodeTag)folder.Tag).Filename;

            Debug.Check(_fileManagers.Count > 0);

            // set the full name of the behavior.
            string filename = Path.Combine(behavior.Folder, label);
            filename = Path.ChangeExtension(filename, _fileManagers[0].FileExtension);

            behavior.Filename = filename;

            NodeTagType folderTagType = ((NodeTag)folder.Tag).Type;
            NodeTagType nodeTagType = (folderTagType == NodeTagType.BehaviorFolder) ? NodeTagType.Behavior : NodeTagType.Prefab;

            // create the tree node
            TreeNode newNode = new TreeNode(((Node)behavior).Label, (int)NodeIcon.BehaviorModified, (int)NodeIcon.BehaviorModified);
            newNode.Tag = new NodeTag(nodeTagType, behavior.GetType(), filename);

            ((NodeTag)(newNode.Tag)).AssignLoadedBehavior(behavior);

            // add the new node to the folder node, and select it.
            folder.Nodes.Add(newNode);
            treeView.SelectedNode = newNode;

            // trigger the ShowBehavior event
            if (ShowBehavior != null)
            {
                ShowBehavior(behavior);
            }

            Focus();

            //allow the user directly rename
            treeView.SelectedNode.BeginEdit();

            UndoManager.Save(behavior);
        }

        private TreeNode FindTreeNodeByName(TreeNodeCollection nodes, string filename)
        {
            foreach (TreeNode node in nodes)
            {
                NodeTag nodetag = (NodeTag)node.Tag;

                if (nodetag.Filename == filename)
                {
                    return node;
                }

                TreeNode findNode = FindTreeNodeByName(node.Nodes, filename);

                if (findNode != null)
                {
                    return findNode;
                }
            }

            return null;
        }

        public void ShowBehaviorTreeNode(string filename)
        {
            TreeNode node = FindTreeNodeByName(treeView.Nodes, filename);

            if (node != null)
            {
                TreeNode parent = node.Parent;

                while (parent != null)
                {
                    parent.Expand();
                    parent = parent.Parent;
                }

                treeView.SelectedNode = node;
                node.EnsureVisible();

                treeView.Focus();
                treeView.Select();
            }
        }

        /// <summary>
        /// Handles when the new behavior button is clicked.
        /// </summary>
        private void newBehaviorButton_Click(object sender, EventArgs e)
        {
            NewBehavior();
        }

        private void expandButton_Click(object sender, EventArgs e)
        {
            this.treeView.ExpandAll();
        }

        private void collapseButton_Click(object sender, EventArgs e)
        {
            this.treeView.CollapseAll();
        }

        /// <summary>
        /// Handles when the refresh list button is clicked.
        /// </summary>
        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (Workspace.Current != null)
            {
                // Save the debug info.
                DebugDataPool.Save(Workspace.Current.FileName);

                MainWindow.Instance.SetWorkspace(Workspace.Current.FileName, false);
            }
        }

        public delegate void BehaviorRenamedEventDelegate(BehaviorNode node);

        /// <summary>
        /// This event is triggered when a behavior was renamed.
        /// </summary>
        public event BehaviorRenamedEventDelegate BehaviorRenamed;

        public delegate void ClearBehaviorsEventDelegate(bool save, List<BehaviorNode> nodes, out bool[] result, out bool error);

        /// <summary>
        /// This event is triggered when all created and loaded behavior trees must be closed.
        /// </summary>
        public event ClearBehaviorsEventDelegate ClearBehaviors;

        /// <summary>
        /// Changes the currently selected behavior folder to another one.
        /// </summary>
        /// <param name="folder">The new behavior folder.</param>
        private void ChangeBehaviorFolder(string folder)
        {
            // assign the new folder
            _behaviorFolder = string.IsNullOrEmpty(folder) ? string.Empty : Path.GetFullPath(folder);

            // check if we can clear all behaviors
            if (ClearBehaviors != null)
            {
                // add all new behaviors to the list of behaviors which need to be saved
                List<BehaviorNode> behaviorsToSave = new List<BehaviorNode>();
                behaviorsToSave.AddRange(_newBehaviors);

                // add any modified behavior to that list as well
                foreach (BehaviorNode node in _loadedBehaviors)
                {
                    if (node.IsModified)
                    {
                        behaviorsToSave.Add(node);
                    }
                }

                // clear all of the behaviors
                bool error;
                bool[] result;
                ClearBehaviors(true, behaviorsToSave, out result, out error);

                // clear the new and loaded behaviors
                _newBehaviors.Clear();
                _loadedBehaviors.Clear();

                // rebuild the list for the new folder
                RebuildBehaviorList();
            }
        }

        /// <summary>
        /// Handles when a tree node is dragged from the node explorer
        /// </summary>
        private void treeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Right || Plugin.EditMode != EditModes.Design)
            {
                return;
            }

            TreeNode node = (TreeNode)e.Item;
            NodeTag nodetag = (NodeTag)node.Tag;

            // we can only move behaviors which are saved, behavior sub-folders and nodes
            if (nodetag.Type == NodeTagType.Behavior && !string.IsNullOrEmpty(nodetag.Filename) && Plugin.AllowReferencedBehaviors ||
                nodetag.Type == NodeTagType.BehaviorFolder && node.Parent != null ||
                nodetag.Type == NodeTagType.Prefab && !string.IsNullOrEmpty(nodetag.Filename) && Plugin.AllowReferencedBehaviors ||
                nodetag.Type == NodeTagType.PrefabFolder && node.Parent != null ||
                nodetag.Type == NodeTagType.Node ||
                nodetag.Type == NodeTagType.Attachment)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Handles when a tree node is dropped on the node explorer
        /// </summary>
        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            // check if a tree node was dropped
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                TreeView view = (TreeView)sender;

                // get the tree node the other node was dropped on
                Point pt = view.PointToClient(new Point(e.X, e.Y));
                TreeNode targetNode = view.GetNodeAt(pt);

                if (targetNode != null)
                {
                    NodeTag targetNodeTag = (NodeTag)targetNode.Tag;

                    TreeNode sourceNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                    NodeTag sourceNodeTag = (NodeTag)sourceNode.Tag;

                    // if the tree node was dragged in the same node explorer and
                    // we drop a behavior or a sub-folder
                    // on another behavior or folder, we continue
                    if (targetNode.TreeView == sourceNode.TreeView &&
                        ((sourceNodeTag.Type == NodeTagType.Behavior || (sourceNodeTag.Type == NodeTagType.BehaviorFolder && sourceNode.Parent != null)) &&
                         (targetNodeTag.Type == NodeTagType.Behavior || targetNodeTag.Type == NodeTagType.BehaviorFolder) ||
                         (sourceNodeTag.Type == NodeTagType.Prefab || (sourceNodeTag.Type == NodeTagType.PrefabFolder && sourceNode.Parent != null)) &&
                         (targetNodeTag.Type == NodeTagType.Prefab || targetNodeTag.Type == NodeTagType.PrefabFolder)))
                    {
                        // if we dropped on a behavior, we use its parent instead
                        if (targetNodeTag.Type == NodeTagType.Behavior || targetNodeTag.Type == NodeTagType.Prefab)
                        {
                            targetNode = targetNode.Parent;

                            if (targetNode != null)
                            {
                                targetNodeTag = (NodeTag)targetNode.Tag;
                            }
                        }

                        try
                        {
                            // generate the new filename
                            string targetfile = targetNodeTag.Filename + '\\' + Path.GetFileName(sourceNodeTag.Filename);

                            // check if the target file is different from the source file
                            if (sourceNodeTag.Filename != targetfile)
                            {
                                List<string> oldPrefabNames = new List<string>();
                                List<string> newPrefabNames = new List<string>();

                                if (sourceNodeTag.Type == NodeTagType.Prefab || sourceNodeTag.Type == NodeTagType.PrefabFolder)
                                {
                                    string msgInfo = sourceNodeTag.Type == NodeTagType.Prefab ? "The instances of the dragged prefab will be re-linked. Are you sure?"
                                                     : "The instances of the prefabs in the dragged folder will be re-linked. Are you sure?";
                                    DialogResult result = MessageBox.Show(msgInfo, Resources.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                                    if (result == DialogResult.Cancel)
                                    {
                                        return;
                                    }
                                }

                                // if the dropped item was a behavior
                                if (sourceNodeTag.Type == NodeTagType.Behavior || sourceNodeTag.Type == NodeTagType.Prefab)
                                {
                                    if (sourceNodeTag.Type == NodeTagType.Prefab)
                                    {
                                        oldPrefabNames.Add(sourceNodeTag.Filename);
                                        newPrefabNames.Add(targetfile);
                                    }

                                    // move the file
                                    File.Move(sourceNodeTag.Filename, targetfile);

                                    // and update its file manager
                                    BehaviorNode node = GetBehavior(sourceNodeTag.Filename);

                                    if (node != null)
                                    {
                                        node.Filename = targetfile;
                                    }

                                    // update the tree node's filename
                                    sourceNodeTag.Filename = targetfile;

                                    // move the tree node in the explorer
                                    sourceNode.Remove();
                                    if (targetNode != null)
                                    {
                                        targetNode.Nodes.Add(sourceNode);
                                        targetNode.Expand();

                                        // sort the tree
                                        targetNode.TreeView.Sort();
                                    }
                                }
                                else
                                {
                                    if (sourceNodeTag.Type == NodeTagType.PrefabFolder)
                                    {
                                        GetAllBehaviorNames(treeView.SelectedNode.Nodes, ref oldPrefabNames);

                                        foreach (string oldPrefabName in oldPrefabNames)
                                        {
                                            string prefabName = targetfile + oldPrefabName.Substring(sourceNodeTag.Filename.Length);
                                            newPrefabNames.Add(prefabName);
                                        }
                                    }

                                    // if it is a folder, move it
                                    Directory.Move(sourceNodeTag.Filename, targetfile);

                                    // update the filename of the already loaded behaviors
                                    foreach (BehaviorNode node in _loadedBehaviors)
                                    {
                                        if (node.Filename.StartsWith(sourceNodeTag.Filename + '\\'))
                                        {
                                            node.Filename = targetfile + node.Filename.Substring(sourceNodeTag.Filename.Length);
                                        }
                                    }

                                    // rebuild the behavior list to update the tree nodes
                                    RebuildBehaviorList();
                                }

                                Debug.Check(oldPrefabNames.Count == newPrefabNames.Count);

                                for (int i = 0; i < oldPrefabNames.Count; ++i)
                                {
                                    string oldPrefabName = FileManagers.FileManager.GetRelativePath(oldPrefabNames[i]);
                                    string newPrefabName = FileManagers.FileManager.GetRelativePath(newPrefabNames[i]);

                                    if (oldPrefabName != newPrefabName)
                                    {
                                        foreach (BehaviorNode behavior in this.GetAllBehaviors())
                                        {
                                            bool resetName = ((Node)behavior).SetPrefab(newPrefabName, false, oldPrefabName);

                                            if (resetName)
                                            {
                                                UndoManager.Save(behavior);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, Resources.FileError, MessageBoxButtons.OK);

                            RebuildBehaviorList();
                        }
                    }
                }//
            }
        }

        /// <summary>
        /// Handles when a tree node is dragged over the node explorer
        /// </summary>
        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;

            // if it is a tree node
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                TreeView view = (TreeView)sender;

                // get the tree node we are over
                Point pt = view.PointToClient(new Point(e.X, e.Y));
                TreeNode targetNode = view.GetNodeAt(pt);

                if (targetNode != null)
                {
                    NodeTag targetNodeTag = (NodeTag)targetNode.Tag;

                    if (targetNodeTag != null)
                    {
                        TreeNode sourceNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                        NodeTag sourceNodeTag = (NodeTag)sourceNode.Tag;

                        // if the tree node was dragged in the same node explorer and
                        // we drop a behavior or a sub-folder
                        // on another behavior or folder, we continue
                        if (targetNode.TreeView == sourceNode.TreeView &&
                            ((sourceNodeTag.Type == NodeTagType.Behavior || (sourceNodeTag.Type == NodeTagType.BehaviorFolder && sourceNode.Parent != null)) &&
                             (targetNodeTag.Type == NodeTagType.Behavior || targetNodeTag.Type == NodeTagType.BehaviorFolder) ||
                             (sourceNodeTag.Type == NodeTagType.Prefab || (sourceNodeTag.Type == NodeTagType.PrefabFolder && sourceNode.Parent != null)) &&
                             (targetNodeTag.Type == NodeTagType.Prefab || targetNodeTag.Type == NodeTagType.PrefabFolder)))
                        {
                            // if the target is a behavior, use its folder instead
                            if (targetNodeTag.Type == NodeTagType.Behavior || targetNodeTag.Type == NodeTagType.Prefab)
                            {
                                targetNode = targetNode.Parent;
                            }

                            if (targetNode != null)
                            {
                                // update the selected node and expand it so you can drag an item into a collapsed sub folder
                                //targetNode.TreeView.SelectedNode = targetNode;
                                targetNode.Expand();

                                e.Effect = DragDropEffects.Move;
                            }
                        }
                    }
                }
            }
        }

        public string GetUniqueFileName(string basename)
        {
            string filename = basename;
            string dir = Path.GetDirectoryName(basename);
            string file = Path.GetFileNameWithoutExtension(basename);
            string ext = Path.GetExtension(basename);
            int index = -1;

            do
            {
                filename = (index >= 0) ? Path.Combine(dir, file + "_" + index) : Path.Combine(dir, file);
                filename = Path.ChangeExtension(filename, ext);
                ++index;
            }
            while (File.Exists(filename));

            return filename;
        }

        /// <summary>
        /// Saves a given behavior under the filename which is stored in the behavior's file manager.
        /// If no file manager exists (new node), the user is asked to choose a name.
        /// </summary>
        /// <param name="node">The behavior node which will be saved.</param>
        /// <param name="saveas">If true, the user will always be asked for a filename, even when a file manager is already present.</param>
        /// <returns>Returns the result when the behaviour is saved.</returns>
        public FileManagers.SaveResult SaveBehavior(BehaviorNode node, bool saveas, bool showNode = true)
        {
            if (ShowBehavior == null)
            {
                throw new Exception("Missing event handler ShowBehavior");
            }

            try
            {
                Workspace.EnableFileWatcher(false);

                // store which behavior is currently shown
                BehaviorNode currNode = BehaviorTreeViewDock.LastFocused == null ? null : BehaviorTreeViewDock.LastFocused.BehaviorTreeView.RootNode;

                BehaviorTreeViewDock dock = null;
                BehaviorNode rootNode = node;

                if (showNode)
                {
                    // show the behavior we want to save
                    dock = ShowBehavior(node);

                    // check if we need to show the save dialogue
                    rootNode = dock.BehaviorTreeView.RootNode;
                }

                rootNode.PreSave();

                if (rootNode.FileManager == null || saveas)
                {
                    Debug.Check(_fileManagers.Count > 0);

                    string filename;

                    // set the filename
                    if (rootNode.FileManager != null && saveas)
                    {
                        filename = rootNode.Filename;

                    }
                    else
                    {
                        string dir = Directory.Exists(rootNode.Folder) ? rootNode.Folder : _behaviorFolder;
                        filename = Path.Combine(dir, ((Node)rootNode).Label);
                        filename = Path.ChangeExtension(filename, _fileManagers[0].FileExtension);
                    }

                    // the file is new if it has no file manager
                    bool isNew = (rootNode.FileManager == null);

                    if (isNew)
                    {
                        saveas = false;
                    }

                    bool isValidFile = Path.IsPathRooted(filename);

                    if (saveas || !isValidFile)
                    {
                        // Choose a valid file name.
                        using(SaveAsDialog saveAsDialog = new SaveAsDialog(true))
                        {
                            saveAsDialog.Text = Resources.SaveBehaviorAs;
                            saveAsDialog.FileName = GetUniqueFileName(filename);

                            // show the save dialogue
                            isValidFile = (saveAsDialog.ShowDialog() == DialogResult.OK);

                            if (isValidFile)
                            {
                                filename = saveAsDialog.FileName;
                            }
                        }
                    }

                    if (isValidFile)
                    {
                        // make sure we have the absolute filename
                        Debug.Check(Path.IsPathRooted(filename));

                        // create the selected file manager
                        FileManagers.FileManager fm = _fileManagers[0].Create(filename, saveas ? (BehaviorNode)rootNode.Clone() : rootNode);

                        if (fm == null)
                        {
                            throw new Exception("Could not create file manager");
                        }

                        if (isNew)
                        {
                            // assign the new file manager and the new name
                            rootNode.FileManager = fm;
                        }

                        // update the view so we get the new label
                        if (dock != null)
                        {
                            dock.BehaviorTreeView.Invalidate();
                        }

                        // save the behavior
                        fm.Save();

                        rootNode.PostSave();

                        // if the behavior was new, remove it from the list of new behaviors to the loaded ones
                        if (isNew)
                        {
                            if (_newBehaviors.Remove(rootNode))
                            {
                                _loadedBehaviors.Add(rootNode);
                            }
                        }

                        if (saveas)
                        {
                            // rebuild the behaviors in the node explorer
                            RebuildBehaviorList();

                            UIUtilities.ShowBehaviorTree(filename);

                            ShowBehaviorTreeNode(rootNode.Filename);
                        }
                    }
                    else
                    {
                        // the user aborted
                        return FileManagers.SaveResult.Cancelled;
                    }
                }
                else
                {
                    // simply save the behavior using the existing file manager
                    FileManagers.SaveResult saveResult = rootNode.FileManager.Save();

                    rootNode.PostSave();

                    if (FileManagers.SaveResult.Succeeded != saveResult)
                    {
                        return saveResult;
                    }
                }

                CheckNode(rootNode, GetTreeNode(rootNode));

                // if we were showing a different behavior before, return to it
                if (showNode && currNode != null)
                {
                    ShowBehavior(currNode);
                }
            }

            finally
            {
                Workspace.EnableFileWatcher(true);
            }

            return FileManagers.SaveResult.Succeeded;
        }

        /// <summary>
        /// Adds all found ebhaviours to the list.
        /// </summary>
        /// <param name="nodes">The tree nodes we are currently searching.</param>
        /// <param name="filenames">The list of filenames were we add all found behaviors to.</param>
        private void GetAllBehaviorNames(TreeNodeCollection nodes, ref List<string> filenames)
        {
            foreach (TreeNode node in nodes)
            {
                NodeTag nodetag = (NodeTag)node.Tag;

                if (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.Prefab)
                {
                    filenames.Add(nodetag.Filename);

                }
                else if (nodetag.Type == NodeTagType.BehaviorFolder || nodetag.Type == NodeTagType.PrefabFolder)
                {
                    GetAllBehaviorNames(node.Nodes, ref filenames);
                }
            }
        }

        public IList<string> GetAllBehaviorNames()
        {
            List<string> filenames = new List<string>();

            GetAllBehaviorNames(GetBehaviorGroup(treeView.Nodes, _behaviorGroupName, _behaviorFolder).Nodes, ref filenames);
            GetAllBehaviorNames(GetBehaviorGroup(treeView.Nodes, _prefabGroupName, GetPrefabFolder()).Nodes, ref filenames);

            return filenames.AsReadOnly();
        }

        public IList<Nodes.BehaviorNode> GetAllBehaviors()
        {
            IList<string> allBehaviorNames = this.GetAllBehaviorNames();
            List<Nodes.BehaviorNode> behaviors = new List<Nodes.BehaviorNode>();

            //Plugin.ConcurrentSourceFileStreams.Clear();
            //if (Plugin.ConcurrentProcessBehaviors)
            //{
            //    List<string> unloadedBehaviorNames = new List<string>();
            //    foreach (string filename in allBehaviorNames)
            //    {
            //        Nodes.BehaviorNode behavior = this.GetBehavior(filename);
            //        if (behavior == null)
            //        {
            //            unloadedBehaviorNames.Add(filename);
            //        }
            //    }

            //    ConcurrentCreateLoadFileStreams(unloadedBehaviorNames);
            //}

            foreach (string filename in allBehaviorNames)
            {
                Nodes.BehaviorNode behavior = this.LoadBehavior(filename);

                if (behavior != null)
                {
                    behaviors.Add(behavior);
                }
            }

            return behaviors.AsReadOnly();
        }

        //public class LoadThreadDataInfo
        //{
        //    public IList<string> AllBehaviorNames = null;
        //    public int BlockSize = 0;
        //    public int ThreadCount = 0;
        //    public int CurrentThreadIndex = 0;
        //}

        //private static int _loadThreadCount = 0;

        //private static void LoadThreadFunc(object param)
        //{
        //    try
        //    {
        //        System.Threading.Interlocked.Increment(ref _loadThreadCount);

        //        LoadThreadDataInfo tdi = (LoadThreadDataInfo)param;

        //        for (int s = 0; s < tdi.BlockSize; ++s)
        //        {
        //            int curIndex = tdi.CurrentThreadIndex + s * tdi.ThreadCount;
        //            if (curIndex >= tdi.AllBehaviorNames.Count)
        //            {
        //                break;
        //            }

        //            string filename = tdi.AllBehaviorNames[curIndex];
        //            Plugin.ConcurrentSourceFileStreams[filename] = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        }
        //    }
        //    finally
        //    {
        //        System.Threading.Interlocked.Decrement(ref _loadThreadCount);
        //    }
        //}

        //private void ConcurrentCreateLoadFileStreams(IList<string> allBehaviorNames)
        //{
        //    const int kThreadCount = 16;

        //    if (allBehaviorNames.Count < kThreadCount)
        //    {
        //        return;
        //    }

        //    int blockSize = allBehaviorNames.Count / kThreadCount;
        //    if (blockSize * kThreadCount < allBehaviorNames.Count)
        //    {
        //        blockSize++;
        //    }

        //    for (int i = 0; i < kThreadCount; ++i)
        //    {
        //        if (i >= allBehaviorNames.Count)
        //        {
        //            break;
        //        }

        //        LoadThreadDataInfo tdi = new LoadThreadDataInfo();
        //        tdi.AllBehaviorNames = allBehaviorNames;
        //        tdi.ThreadCount = kThreadCount;
        //        tdi.BlockSize = blockSize;
        //        tdi.CurrentThreadIndex = i;

        //        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadThreadFunc));
        //        thread.Start(tdi);
        //    }

        //    System.Threading.Thread.Sleep(1000);

        //    // wait all threads finished
        //    while (true)
        //    {
        //        int threadNumber = System.Threading.Interlocked.Exchange(ref _loadThreadCount, _loadThreadCount);
        //        if (threadNumber <= 0)
        //        {
        //            break;
        //        }
        //    }
        //}

        /// <summary>
        /// Handles when the user tries to rename a tree node.
        /// </summary>
        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            NodeTag nodetag = (NodeTag)e.Node.Tag;

            // we may not rename nodes, node folders and the root behavior folder
            if (nodetag.Type == NodeTagType.Node ||
                nodetag.Type == NodeTagType.NodeFolder ||
                nodetag.Type == NodeTagType.BehaviorFolder && e.Node.Parent == null ||
                nodetag.Type == NodeTagType.PrefabFolder && e.Node.Parent == null)
            {
                e.CancelEdit = true;

            }
            else
            {
                // we may not rename newly created behaviors as the label is used to identify them
                if (string.IsNullOrEmpty(nodetag.Filename) ||
                    (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.Prefab) &&
                    !Path.IsPathRooted(nodetag.Filename))
                {
                    e.CancelEdit = true;
                }
            }
        }

        /// <summary>
        /// Handles the rename process of a behavior folder or a behavior
        /// </summary>
        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // check if the new label is valid and did change
            if (e.Label == null || e.Label == e.Node.Text || e.Label.Length < 1)
            {
                e.CancelEdit = true;
                return;
            }

            NodeTag nodetag = (NodeTag)e.Node.Tag;

            // trim unrequired characters and check if the label is still valid
            string label = e.Label.Trim();

            if (label.Length < 1 || (nodetag.Type == NodeTagType.Behavior && !Plugin.IsValidFilename(label)))
            {
                if (label.Length > 0)
                {
                    MessageBox.Show(Resources.FilenameWarning, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                e.CancelEdit = true;
                return;
            }

            if (nodetag.Type == NodeTagType.Prefab || nodetag.Type == NodeTagType.PrefabFolder)
            {
                string msgInfo = nodetag.Type == NodeTagType.Prefab ? "The instances of the selected prefab will be re-linked. Are you sure?"
                                 : "The instances of the prefabs in the selected folder will be re-linked. Are you sure?";
                DialogResult result = MessageBox.Show(msgInfo, Resources.Warning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel)
                {
                    e.CancelEdit = true;
                    return;
                }
            }

            try
            {
                // create the new filename
                string targetfile = Path.Combine(Path.GetDirectoryName(nodetag.Filename), label);

                List<string> oldPrefabNames = new List<string>();
                List<string> newPrefabNames = new List<string>();

                // check if we are renaming a behavior, prefab or a folder
                if (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.Prefab)
                {
                    targetfile = Path.ChangeExtension(targetfile, Path.GetExtension(nodetag.Filename));

                    // check if the file already exists
                    if (GetBehavior(targetfile) != null || File.Exists(targetfile))
                    {
                        e.CancelEdit = true;
                        string msgInfo = string.Format(Resources.FilenameWarningInfo, e.Label);
                        MessageBox.Show(msgInfo, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // move the file
                    if (File.Exists(nodetag.Filename))
                    {
                        File.Move(nodetag.Filename, targetfile);
                    }

                    if (nodetag.Type == NodeTagType.Prefab)
                    {
                        oldPrefabNames.Add(nodetag.Filename);
                        newPrefabNames.Add(targetfile);
                    }

                    BehaviorNode node = GetBehavior(nodetag.Filename);

                    if (node != null)
                    {
                        // update the node's label and its file manager
                        ((Node)node).Label = label;

                        if (node.Filename != targetfile)
                        {
                            UndoManager.Clear(node.Filename);

                            node.Filename = targetfile;

                            UndoManager.Save(node);
                        }

                        // if the behavior is shown it needs to be updated because of the label
                        if (BehaviorTreeViewDock.LastFocused != null)
                        {
                            BehaviorTreeViewDock.LastFocused.Invalidate();
                        }

                        // triggered the behavior renamed events
                        node.TriggerWasRenamed();

                        if (BehaviorRenamed != null)
                        {
                            BehaviorRenamed(node);
                        }
                    }

                    // update the filename in the node tag
                    nodetag.Filename = targetfile;

                }
                else
                {
                    // check if the directory already exists
                    if (Directory.Exists(targetfile))
                    {
                        e.CancelEdit = true;
                        string msgInfo = string.Format(Resources.DirectoryWarningInfo, e.Label);
                        MessageBox.Show(msgInfo, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (nodetag.Type == NodeTagType.PrefabFolder)
                    {
                        GetAllBehaviorNames(treeView.SelectedNode.Nodes, ref oldPrefabNames);

                        foreach (string oldPrefabName in oldPrefabNames)
                        {
                            string prefabName = targetfile + oldPrefabName.Substring(nodetag.Filename.Length);
                            newPrefabNames.Add(prefabName);
                        }
                    }

                    // move the folder
                    FileManagers.FileManager.MergeFolders(nodetag.Filename, targetfile);

                    // update the filename of all loaded behaviors from this folder
                    foreach (BehaviorNode node in _loadedBehaviors)
                    {
                        if (node.Filename.StartsWith(nodetag.Filename + '\\'))
                        {
                            node.Filename = targetfile + node.Filename.Substring(nodetag.Filename.Length);

                            // triggered the behavior renamed events
                            node.TriggerWasRenamed();

                            if (BehaviorRenamed != null)
                            {
                                BehaviorRenamed(node);
                            }
                        }
                    }

                    // update the filename in the node tag
                    nodetag.Filename = targetfile;

                    RebuildBehaviorList();
                }

                Debug.Check(oldPrefabNames.Count == newPrefabNames.Count);

                for (int i = 0; i < oldPrefabNames.Count; ++i)
                {
                    string oldPrefabName = FileManagers.FileManager.GetRelativePath(oldPrefabNames[i]);
                    string newPrefabName = FileManagers.FileManager.GetRelativePath(newPrefabNames[i]);

                    if (oldPrefabName != newPrefabName)
                    {
                        foreach (BehaviorNode behavior in this.GetAllBehaviors())
                        {
                            bool resetName = ((Node)behavior).SetPrefab(newPrefabName, false, oldPrefabName);

                            if (resetName)
                            {
                                UndoManager.Save(behavior);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                e.CancelEdit = true;
                MessageBox.Show(ex.Message, Resources.FileError, MessageBoxButtons.OK);

                RebuildBehaviorList();
            }
        }

        private int _lastNewGroup = 1;

        internal void CreateGroup()
        {
            // if no tree node is selected we do not know where to create the new folder
            if (treeView.SelectedNode == null)
            {
                return;
            }

            // we can only create folders for behaviors and folders
            NodeTag nodetag = (NodeTag)treeView.SelectedNode.Tag;

            if (nodetag != null)
            {
                if (nodetag.Type != NodeTagType.Behavior && nodetag.Type != NodeTagType.BehaviorFolder &&
                    nodetag.Type != NodeTagType.Prefab && nodetag.Type != NodeTagType.PrefabFolder)
                {
                    return;
                }

                try
                {
                    // if the selected tree node is a behavior we use its folder
                    TreeNode folder = (nodetag.Type == NodeTagType.BehaviorFolder || nodetag.Type == NodeTagType.PrefabFolder) ? treeView.SelectedNode : treeView.SelectedNode.Parent;

                    if (folder != null)
                    {
                        nodetag = (NodeTag)folder.Tag;

                        if (nodetag != null)
                        {
                            // create a unique node name the the name of the folder
                            string label = GetUniqueLabel("New Folder", _lastNewGroup, out _lastNewGroup);
                            string dir = nodetag.Filename + '\\' + label;

                            // create the new folder
                            Directory.CreateDirectory(dir);

                            // create the tree node for the folder
                            TreeNode newnode = new TreeNode(label, (int)NodeIcon.FolderClosed, (int)NodeIcon.FolderClosed);
                            newnode.Tag = new NodeTag(nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.BehaviorFolder ? NodeTagType.BehaviorFolder : NodeTagType.PrefabFolder, null, dir);

                            // add the folder and expand its parent
                            folder.Nodes.Add(newnode);
                            folder.Expand();

                            // selected the new node
                            treeView.SelectedNode = newnode;

                            // allow the user to define a custom name
                            newnode.BeginEdit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.FileError, MessageBoxButtons.OK);

                    // if there was an error we have to rebuild the list of available behaviors and folders
                    RebuildBehaviorList();
                }
            }
        }

        /// <summary>
        /// Handles when the new group button is clicked
        /// </summary>
        private void createGroupButton_Click(object sender, EventArgs e)
        {
            CreateGroup();
        }

        /// <summary>
        /// Handles when the key is pressed
        /// </summary>
        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            // if the F2 key is pressed and a node is selected which is not currently edited, edit the nodes label
            if (e.KeyCode == Keys.F2 && treeView.SelectedNode != null && !treeView.SelectedNode.IsEditing)
            {
                treeView.SelectedNode.BeginEdit();
            }

            // delete the current tree node
            else if (e.KeyCode == Keys.Delete)
            {
                if (Plugin.EditMode == EditModes.Design)
                {
                    deleteButton_Click(sender, null);
                }
            }
        }

        /// <summary>
        /// Handles when the delete button is clicked.
        /// </summary>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // if no tree node is selected we have nothing to delete
            if (treeView.SelectedNode == null)
            {
                return;
            }

            // we may only delete behaviors, prefabs and folders.
            NodeTag nodetag = (NodeTag)treeView.SelectedNode.Tag;

            if (nodetag.Type != NodeTagType.Behavior && nodetag.Type != NodeTagType.BehaviorFolder &&
                nodetag.Type != NodeTagType.Prefab && nodetag.Type != NodeTagType.PrefabFolder)
            {
                return;
            }

            bool isFolder = nodetag.Type == NodeTagType.BehaviorFolder || nodetag.Type == NodeTagType.PrefabFolder;
            string warningInfo = string.Format(isFolder ? Resources.DeleteFolderWarningInfo : Resources.DeleteWarningInfo, treeView.SelectedNode.Text);

            if (nodetag.Type == NodeTagType.Prefab || nodetag.Type == NodeTagType.PrefabFolder)
            {
                warningInfo = isFolder ? "All prefabs in the selected folder will be deleted, and the link with their instances will be broken, which is not undoable. Are you sure?"
                              : "The selected prefab will be deleted, and the link with its instances will be broken, which is not undoable. Are you sure?";
            }

            DialogResult dr = MessageBox.Show(warningInfo, Resources.DeleteWarning, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

            if (dr == DialogResult.Cancel)
            {
                return;
            }

            // the list of the behaviors deleted
            List<BehaviorNode> behaviors = new List<BehaviorNode>();

            try
            {
                // check if we delete a behavior
                List<string> prefabFiles = new List<string>();

                if (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.Prefab)
                {
                    BehaviorNode node = GetBehavior(nodetag, treeView.SelectedNode.Text);

                    if (node != null)
                    {
                        behaviors.Add(node);
                    }

                    if (nodetag.Type == NodeTagType.Prefab)
                    {
                        prefabFiles.Add(nodetag.Filename);
                    }

                }
                else
                {
                    // add all behaviors which are loaded and in the folder we want to delete to the behavior list
                    foreach (BehaviorNode node in _loadedBehaviors)
                    {
                        if (node.Filename.StartsWith(nodetag.Filename + '\\'))
                        {
                            behaviors.Add(node);
                        }
                    }

                    if (nodetag.Type == NodeTagType.PrefabFolder)
                    {
                        GetAllBehaviorNames(treeView.SelectedNode.Nodes, ref prefabFiles);
                    }
                }

                foreach (string prefabFile in prefabFiles)
                {
                    Nodes.BehaviorNode prefabBehavior = this.LoadBehavior(prefabFile);

                    if (prefabBehavior != null)
                    {
                        string prefabName = FileManagers.FileManager.GetRelativePath(prefabFile);

                        foreach (BehaviorNode behavior in this.GetAllBehaviors())
                        {
                            ((Node)behavior).ResetByPrefab(prefabName, (Node)prefabBehavior);
                            ((Node)behavior).ClearPrefab(prefabName);
                        }
                    }
                }

                // close all behaviors which we want to delete
                bool error = false;

                if (ClearBehaviors != null)
                {
                    bool[] result;
                    ClearBehaviors(false, behaviors, out result, out error);

                    // check if there was no problem
                    for (int i = 0; i < behaviors.Count; ++i)
                    {
                        // if the behavior could be closed
                        if (result[i])
                        {
                            // remove the behavior from the correct list
                            if (behaviors[i].FileManager == null || behaviors[i].Filename == string.Empty)
                            {
                                _newBehaviors.Remove(behaviors[i]);
                            }

                            else
                            {
                                _loadedBehaviors.Remove(behaviors[i]);
                            }

                            UndoManager.Clear(behaviors[i].Filename);
                        }
                    }
                }

                // if there was no error remove the tree node and delete the selected path
                if (!error)
                {
                    treeView.SelectedNode.Remove();

                    if (!string.IsNullOrEmpty(nodetag.Filename))
                    {
                        if (File.Exists(nodetag.Filename))
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(nodetag.Filename, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                        }

                        else if (Directory.Exists(nodetag.Filename))
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(nodetag.Filename, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.FileError, MessageBoxButtons.OK);

                // if there was an error rebuild the list of available behaviors and folders
                RebuildBehaviorList();
            }
        }

        /// <summary>
        /// Handles when a behavior was modified.
        /// </summary>
        /// <param name="node">The node that was modified.</param>
        private void behavior_WasModified(BehaviorNode behavior, Node node)
        {
            // get the correct tree node
            TreeNode tnode = null;

            if (behavior.FileManager == null)
            {
                tnode = GetBehaviorNode(((Node)behavior).Label, false);

            }
            else
            {
                tnode = GetBehaviorNode(behavior.Filename, true);
            }

            // change the behaviors icon to the modified icon
            if (tnode != null)
            {
                tnode.ImageIndex = (int)NodeIcon.BehaviorModified;
                tnode.SelectedImageIndex = (int)NodeIcon.BehaviorModified;
            }
        }

        /// <summary>
        /// Handles when a behavior was saved.
        /// </summary>
        /// <param name="node">The node that was modified.</param>
        private void behavior_WasSaved(BehaviorNode node)
        {
            // if the file manager is null the file could not be saved
            if (node.FileManager == null || string.IsNullOrEmpty(node.Filename))
            {
                return;
            }

            // update the behaviors icon to the not-modified one
            TreeNode tnode = GetBehaviorNode(node.Filename, true);

            if (tnode != null)
            {
                NodeTag nodetag = (NodeTag)tnode.Tag;

                if (nodetag.Type == NodeTagType.Prefab)
                {
                    tnode.ImageIndex = (int)NodeIcon.Prefab;
                    tnode.SelectedImageIndex = (int)NodeIcon.Prefab;

                }
                else
                {
                    tnode.ImageIndex = (int)NodeIcon.BehaviorLoaded;
                    tnode.SelectedImageIndex = (int)NodeIcon.BehaviorLoaded;
                }
            }

            UndoManager.OnBehaviorSaved(node);
        }

        /// <summary>
        /// Exports behaviors from the export dialogue. Internal use only.
        /// </summary>
        /// <param name="pool">The pool of tree nodes whose behaviors you want to export.</param>
        /// <param name="folder">The folder the behaviors are exported to.</param>
        /// <param name="exportNoGroups">Defines if the groups are exported as well.</param>
        /// <param name="exporter">The exporter which is used.</param>
        private void ExportBehavior(bool onlyCollectFilename, TreeNodeCollection pool, string folder, bool exportNoGroups, ExporterInfo exporter, ref bool aborted)
        {
            if (aborted)
            {
                return;
            }

            // Export the behavior for each tree node.
            foreach (TreeNode tnode in pool)
            {
                if (aborted)
                {
                    break;
                }

                NodeTag nodetag = (NodeTag)tnode.Tag;

                // If the tree node is selected and a behavior.
                if (nodetag != null && nodetag.Type == NodeTagType.Behavior && tnode.Checked)
                {
                    // Get or load the behavior we want to export.
                    BehaviorNode node = GetBehavior(nodetag, tnode.Text);
                    Debug.Check(node != null);
                    if (node != null)
                    {
                        if (!onlyCollectFilename)
                        {
                            node.PreExport();
                        }

                        FileManagers.SaveResult saveResult = FileManagers.SaveResult.Succeeded;

                        if (FileManagers.SaveResult.Succeeded == saveResult)
                        {
                            string fullPath = tnode.FullPath;

                            if (fullPath.StartsWith("Behaviors\\") || fullPath.StartsWith("Behaviors/"))
                            {
                                fullPath = fullPath.Substring(10);
                            }

                            // Generate the new filename and the exporter.
                            Exporters.Exporter exp = exporter.Create(node, folder, exportNoGroups ? tnode.Text : fullPath);

                            //if (onlyCollectFilename)
                            //{
                            //    _allExporters.Add(exp);
                            //}
                            //else
                            {
                                // Export behavior.
                                saveResult = exp.Export();
                            }
                        }

                        if (!onlyCollectFilename)
                        {
                            node.PostExport();
                        }

                        if (FileManagers.SaveResult.Cancelled == saveResult)
                        {
                            // Abort the exporting all files process.
                            aborted = true;
                            return;
                        }
                    }
                }

                // Export the child tree nodes.
                ExportBehavior(onlyCollectFilename, tnode.Nodes, folder, exportNoGroups, exporter, ref aborted);
            }
        }

        private void GetExportBehaviors(TreeNodeCollection pool, bool exportNoGroups, ExporterInfo exporter, ref bool aborted, ref List<BehaviorNode> exportBehaviors)
        {
            if (aborted || pool == null)
            {
                return;
            }

            // Export the behavior for each tree node.
            foreach (TreeNode tnode in pool)
            {
                NodeTag nodetag = (NodeTag)tnode.Tag;

                // If the tree node is selected and a behavior.
                if (nodetag != null && nodetag.Type == NodeTagType.Behavior && tnode.Checked)
                {
                    // Get or load the behavior we want to export.
                    BehaviorNode node = GetBehavior(nodetag, tnode.Text);
                    Debug.Check(node != null);

                    FileManagers.SaveResult saveResult = FileManagers.SaveResult.Succeeded;

                    if (node != null)
                    {
                        // Before exporting, we try to save this behavior if being modified.
                        if (node.IsModified)
                        {
                            saveResult = SaveBehavior(node, false, false);
                        }

                        if (FileManagers.SaveResult.Cancelled == saveResult)
                        {
                            // Abort the exporting all files process.
                            aborted = true;
                            return;
                        }

                        exportBehaviors.Add(node);
                    }
                }

                // Export the child tree nodes.
                GetExportBehaviors(tnode.Nodes, exportNoGroups, exporter, ref aborted, ref exportBehaviors);
            }
        }

        //private bool UpdateExportedSettting(string exportedPath) {
        //    string exportedDbgXml = Path.Combine(exportedPath, "behaviors.dbg.xml");
        //    FileManagers.SaveResult result = FileManagers.FileManager.MakeWritable(exportedDbgXml, Resources.FileWarning);

        //    if (FileManagers.SaveResult.Succeeded == result) {
        //        using(StreamWriter file = new StreamWriter(exportedDbgXml)) {
        //            XmlWriterSettings ws = new XmlWriterSettings();
        //            ws.Indent = true;
        //            using(XmlWriter xmlWrtier = XmlWriter.Create(file, ws)) {
        //                xmlWrtier.WriteStartDocument();

        //                xmlWrtier.WriteComment("EXPORTED BY TOOL, DON'T MODIFY IT!");
        //                xmlWrtier.WriteComment("DON'T REMOVE IT AS IT IS USED FOR DEBUGGING!");

        //                xmlWrtier.WriteStartElement("workspace");

        //                string fullPath = Path.GetFullPath(exportedDbgXml);
        //                string fullPath1 = Path.GetFullPath(Workspace.Current.Folder);
        //                string relativePath1 = Workspace.MakeRelative(fullPath1, fullPath, true);
        //                string fullPath2 = Path.GetFullPath(Workspace.Current.XMLFolder);
        //                string relativePath2 = Workspace.MakeRelative(fullPath2, fullPath, true);

        //                string workspacePath = Workspace.MakeRelative(Workspace.Current.FileName, fullPath, true);

        //                if (Path.IsPathRooted(workspacePath)) {
        //                    MessageBox.Show("WorkspacePath should be a relative path!", "Update Export Setting", MessageBoxButtons.OK);
        //                    Debug.Check(true);
        //                }

        //                //string workspacePath = Workspace.Current.FileName;

        //                xmlWrtier.WriteAttributeString("name", Workspace.Current.Name);
        //                xmlWrtier.WriteAttributeString("path", workspacePath);
        //                xmlWrtier.WriteAttributeString("source", relativePath1);

        //                xmlWrtier.WriteAttributeString("xmlmeta", relativePath2);

        //                xmlWrtier.WriteEndElement();

        //                xmlWrtier.WriteEndDocument();
        //            }

        //            file.Close();
        //        }

        //    } else if (FileManagers.SaveResult.Cancelled == result) {
        //        return false;
        //    }

        //    return true;
        //}

        internal TreeNode GetBehaviorGroup()
        {
            return GetBehaviorGroup(treeView.Nodes, _behaviorGroupName, _behaviorFolder);
        }

        /// <summary>
        /// Shows the export dialogue for the behaviors to be exported.
        /// </summary>
        /// <param name="node">The behavior you want to export. Use null to export all behaviors.</param>
        /// <returns>Returns true if the user did not abort and all behaviors could be exported.</returns>
        internal bool ExportBehavior(BehaviorNode node, string format = "", bool ignoreErrors = false, TreeNode selectedTreeRoot = null)
        {
            if (Workspace.Current == null)
            {
                MessageBox.Show(Resources.NullWorkspace, Resources.Error, MessageBoxButtons.OK);
                return false;
            }

            DateTime preTime = DateTime.Now;

            // save modified behaviors
            if (node != null)
            {
                this.SaveBehavior(node, false, false);
            }
            else
            {
                this.SaveAll();
            }

            // get the exporter index
            int formatIndex = Plugin.GetExporterIndex(format);

            // create export dialogue
            using(ExportDialog dialog = new ExportDialog(this, node, ignoreErrors, selectedTreeRoot, formatIndex))
            {
                //when format is not empty, it is for the command line exporting, don't show the gui
                if (string.IsNullOrEmpty(format))
                {
                    // show the dialogue
                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        return false;
                    }

                    if (!Workspace.Current.IsSetExportFolder(Workspace.Current.Language))
                    {
                        if (DialogResult.OK == MessageBox.Show(Resources.InvalidExportedTypePath, Resources.ExportError, MessageBoxButtons.OK))
                        {
                            using(EditWorkspaceDialog wksDialog = new EditWorkspaceDialog())
                            {
                                wksDialog.EditWorkspace(Workspace.Current);

                                if (wksDialog.ShowDialog() == DialogResult.Cancel)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }

                try
                {
                    string exportedPath = Workspace.Current.DefaultExportFolder;

                    if (!Directory.Exists(exportedPath))
                    {
                        Directory.CreateDirectory(exportedPath);
                    }

                    if (exportedPath.StartsWith(_behaviorFolder, StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new Exception("Behaviors cannot be exported into the behaviors source folder.");
                    }

                    bool aborted = false;
                    bool exportXML = false;
                    bool exportBson = false;

                    //if (this.UpdateExportedSettting(exportedPath))
                    {
                        Debug.Check(Workspace.Current != null);

                        for (int i = 0; i < Plugin.Exporters.Count; ++i)
                        {
                            ExporterInfo info = Plugin.Exporters[i];

                            if ((string.IsNullOrEmpty(format) && (Workspace.Current.ShouldBeExported(info.ID) || (info.ID == Workspace.Current.Language))) || (info.ID == format))
                            {
                                exportXML |= (info.ID == "xml");
                                exportBson |= (info.ID == "bson");

                                bool bExportBehaviors = dialog.ExportBehaviors;
                                if (info.ID == Workspace.Current.Language && !Workspace.Current.ShouldBeExported(info.ID))
                                {
                                    bExportBehaviors = false;
                                }

                                exportBehavior(bExportBehaviors, dialog.ExportMeta, i, exportedPath, dialog.treeView.Nodes, ref aborted);

                                if (aborted)
                                {
                                    break;
                                }
                            }
                        }

                        if (!aborted)
                        {
                            Workspace.ExportMeta(Workspace.Current, exportXML, exportBson);

                            Utilities.ReportExportBehavior();

                            DateTime curTime = DateTime.Now;
                            TimeSpan ts = curTime.Subtract(preTime);

                            string msg = string.Format(Resources.MessageBTExport, dialog.GetExportedInfo() + "\t" + ts.TotalSeconds + "s");
                            ErrorInfoDock.Inspect();
                            ErrorInfoDock.WriteLineWithTime(msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.ExportError, MessageBoxButtons.OK);
                    return false;
                }
            }

            return true;
        }

        public void ExportTypes(int exporterIndex)
        {
            Debug.Check(exporterIndex >= 0 && exporterIndex < Plugin.Exporters.Count);

            // retrieve the correct exporter info
            ExporterInfo exporter = Plugin.Exporters[exporterIndex];
            Debug.Check(exporter.HasSettings);

            string exportedPath = Workspace.Current.GetExportAbsoluteFolder(exporter.ID);
            List<string> includedFilenames = Workspace.Current.GetExportIncludedFilenames(exporter.ID);

            Exporters.Exporter exp = exporter.Create(null, exportedPath, "", includedFilenames);

            int fileCount = Workspace.Current.GetExportFileCount(exporter.ID);

            exp.Export(null, false, true, fileCount);
        }

        private static List<Exporters.Exporter> _allExporters = new List<Exporters.Exporter>();

        public class ExportThreadDataInfo
        {
            public int BlockSize = 0;
            public int ThreadCount = 0;
            public int CurrentThreadIndex = 0;
        }

        private static int _threadCount = 0;

        private static void ExportThreadFunc(object param)
        {
            try
            {
                System.Threading.Interlocked.Increment(ref _threadCount);

                ExportThreadDataInfo tdi = (ExportThreadDataInfo)param;

                for (int s = 0; s < tdi.BlockSize; ++s)
                {
                    int curIndex = tdi.CurrentThreadIndex + s * tdi.ThreadCount;
                    if (curIndex >= _allExporters.Count)
                    {
                        break;
                    }

                    _allExporters[curIndex].InitWriter();
                }
            }
            finally
            {
                System.Threading.Interlocked.Decrement(ref _threadCount);
            }
        }  

        private void ConcurrentExportFileStreams()
        {
            const int kThreadCount = 16;

            if (_allExporters.Count < kThreadCount)
            {
                return;
            }

            int blockSize = _allExporters.Count / kThreadCount;
            if (blockSize * kThreadCount < _allExporters.Count)
            {
                blockSize++;
            }

            for (int i = 0; i < kThreadCount; ++i)
            {
                if (i >= _allExporters.Count)
                {
                    break;
                }

                ExportThreadDataInfo tdi = new ExportThreadDataInfo();
                tdi.ThreadCount = kThreadCount;
                tdi.BlockSize = blockSize;
                tdi.CurrentThreadIndex = i;

                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ExportThreadFunc));
                thread.Start(tdi);
            }

            System.Threading.Thread.Sleep(1000);

            // wait all threads finished
            while (true)
            {
                int threadNumber = System.Threading.Interlocked.Exchange(ref _threadCount, _threadCount);
                if (threadNumber <= 0)
                {
                    break;
                }
            }
        }

        private void exportBehavior(bool isExportingBehaviors, bool isExportingMeta, int exporterIndex, string exportedPath, TreeNodeCollection nodes, ref bool aborted)
        {
            // retieve the correct exporter info
            ExporterInfo exporter = Plugin.Exporters[exporterIndex];

            if (!exporter.HasSettings)
            {
                //if (Plugin.ConcurrentProcessBehaviors)
                //{
                //    _allExporters.Clear();
                //    Plugin.ConcurrentXmlStreamWriters.Clear();
                //    Plugin.ConcurrentBsonFileStreams.Clear();

                //    ExportBehavior(true, nodes, exportedPath, false, exporter, ref aborted);

                //    ConcurrentExportFileStreams();
                //}

                // export the selected behaviors
                ExportBehavior(false, nodes, exportedPath, false, exporter, ref aborted);
            }
            else
            {
                // export all behaviors
                List<BehaviorNode> exportBehaviors = new List<BehaviorNode>();
                GetExportBehaviors(nodes, false, exporter, ref aborted, ref exportBehaviors);

                exportedPath = Workspace.Current.GetExportAbsoluteFolder(exporter.ID);

                List<string> exportIncludedFilenames = Workspace.Current.GetExportIncludedFilenames(exporter.ID);
                Exporters.Exporter exp = exporter.Create(null, exportedPath, "", exportIncludedFilenames);

                // Export behavior.
                exp.Export(exportBehaviors, isExportingBehaviors, isExportingMeta, Workspace.Current.GetExportFileCount(exporter.ID));

                ErrorInfoDock.WriteExportTypeInfo();
            }
        }

        internal void OpenWorkspace(string wksFile = "")
        {
            if (string.IsNullOrEmpty(wksFile))
            {
                openFileDialog.CheckFileExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    wksFile = openFileDialog.FileName;
                }
            }

            if (!string.IsNullOrEmpty(wksFile))
            {
                if (Plugin.WorkspaceDelegateHandler != null && Plugin.WorkspaceDelegateHandler(wksFile, false))
                {
                    MainWindow.Instance.RecentWorkspacesMenu.AddEntry(wksFile);

                    int index = MainWindow.Instance.RecentWorkspacesMenu.FindFilenameNumber(wksFile);
                    MainWindow.Instance.RecentWorkspacesMenu.SetFirstFile(index);
                }
                else
                {
                    string errorInfo = string.Format(Resources.WorkspaceLoadError, wksFile);
                    MessageBox.Show(errorInfo, Resources.LoadError, MessageBoxButtons.OK);
                }
            }
        }

        public void HandleConnect()
        {
            try
            {
                Plugin.WrongWorksapceReported = false;

                if (Plugin.EditMode == EditModes.Connect || NetworkManager.Instance.IsConnected())
                {
                    if (Settings.Default.DumpConnectData)
                    {
                        this.DumpLogFile();
                    }

                    NetworkManager.Instance.Disconnect();
                    Plugin.DebugAgentInstance = string.Empty;

                    Plugin.EditMode = EditModes.Design;
                }
                else
                {
                    FileManagers.SaveResult saveResult = MainWindow.Instance.CheckSavingBehaviors();

                    if (saveResult != FileManagers.SaveResult.Cancelled)
                    {
                        using(ConnectDialog cd = new ConnectDialog(NetworkManager.UseLocalIP, NetworkManager.ServerIP, NetworkManager.ServerPort))
                        {
                            if (cd.ShowDialog(this) == DialogResult.OK)
                            {
                                NetworkManager.UseLocalIP = cd.UseLocalIP();
                                NetworkManager.ServerIP = cd.GetServer();
                                NetworkManager.ServerPort = cd.GetPort();

                                if (NetworkManager.Instance.Connect(cd.GetServer(), cd.GetPort()))
                                {
                                    Plugin.EditMode = EditModes.Connect;

                                    Utilities.ReportConnectGame();
                                }
                            }
                        }
                    }
                }
            }
            catch     //(Exception ex)
            {
                //Debug.Check(false, ex.Message);
            }
        }

        internal void LoadDump()
        {
            if (Plugin.EditMode == EditModes.Design)
            {
                using(OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = Resources.OpenDumpFile;
                    openFileDialog.Filter = "*.*|*.*|*.log|*.log|*.dump|*.dump";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //set the EditMode here as it will be used when processing log
                        Plugin.EditMode = EditModes.Analyze;

                        string ext = Path.GetExtension(openFileDialog.FileName);
                        ext = ext.ToLower();

                        if (ext == ".dump")
                        {
                            AgentDataPool.LoadDump(openFileDialog.FileName, Workspace.Current.Name);
                        }
                        else if (ext == ".log")
                        {
                            AgentDataPool.LoadLog(openFileDialog.FileName);
                        }
                        else
                        {
                            string errMsg = string.Format("'{0}' is not supported. only .log or .dump are suported.", ext);
                            MessageBox.Show(errMsg, Resources.LoadError, MessageBoxButtons.OK);
                            Plugin.EditMode = EditModes.Design;
                            return;
                        }

                        BehaviorTreeViewDock.ClearHighlights();

                        int firstFrame = MessageQueue.MessageFirstFrame();
                        TimelineDock.SetTotalFrame(AgentDataPool.TotalFrames, firstFrame);
                        //TimelineDock.SetCurrentFrame(firstFrame);
                        TimelineDock.SetCurrentFrame(AgentDataPool.TotalFrames);

                        TimelineDock.UpdateUIState(Plugin.EditMode);
                    }
                }

            }
            else if (Plugin.EditMode == EditModes.Analyze)
            {
                Plugin.EditMode = EditModes.Design;
            }
        }

        private void toolStripButton_dump_Click(object sender, EventArgs e)
        {
            this.LoadDump();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            this.HandleConnect();
        }

        private void toolStripButton_workspace_Click(object sender, EventArgs e)
        {
            OpenWorkspace();
        }

        private BehaviorTreeView getFocusedView()
        {
            return (BehaviorTreeViewDock.LastFocused != null) ? BehaviorTreeViewDock.LastFocused.BehaviorTreeView : null;
        }

        /// <summary>
        /// Handles when double-clicking an error in the check for errors dialogue.
        /// </summary>
        /// <param name="node">The node you want to show.</param>
        /// <returns>Returns the NodeViewData which will be shown.</returns>
        internal NodeViewData ShowNode(Node node, Node root = null)
        {
            try
            {
                if (node != null)
                {
                    if (ShowBehavior != null)
                    {
                        if (root != null && root is BehaviorNode)
                        {
                            ShowBehavior((BehaviorNode)root);
                        }
                        else
                        {
                            ShowBehavior(node.Behavior);
                        }
                    }

                    BehaviorTreeView behaviorTreeView = getFocusedView();

                    if (behaviorTreeView != null && behaviorTreeView.RootNodeView != null)
                    {
                        return behaviorTreeView.RootNodeView.FindNodeViewData(node);
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        #region Auto updater

        private int ConvertVertionToInt32(string verStr)
        {
            int ver = 0;

            string[] digits = verStr.Split('.');

            //System.Diagnostics.Debug.Assert(digits.Length == 4);

            if (digits.Length == 4)
            {
                int shift = 0;

                for (int i = digits.Length - 1; i >= 0; --i)
                {
                    int verI = Convert.ToInt32(digits[i]);
                    ver += (verI << shift);
                    shift += 8;
                }
            }

            return ver;
        }

        private const string url_server = "\\\\tencent.com\\tfs\\部门目录\\IEG互动娱乐事业群\\互动娱乐研发部\\引擎组\\Tag\\Behaviac\\";
        private const string urlVersionFile = url_server + "version.txt";

        private string TrimVersionString(string verStr)
        {
            string ret0 = verStr.Trim();
            string ret = ret0.TrimEnd('\r', '\n');

            return ret;
        }

        internal void CheckVersionSync()
        {
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();

                byte[] data = wc.DownloadData(urlVersionFile);
                String[] datas = new System.Text.ASCIIEncoding().GetString(data).Split('|');
                string verCheck = TrimVersionString(datas[0]);
                int newestVersion = ConvertVertionToInt32(verCheck);

                string verStr = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                int Version = ConvertVertionToInt32(verStr);

                if (Version < newestVersion)
                {
                    string newerFile = "BehaviacSetup_" + verCheck + ".exe";
                    string urlNewVersionFile = url_server + newerFile;

                    string questionStr = string.Format(Resources.NewerVersionInfo, newerFile, urlNewVersionFile);
                    DialogResult dr = MessageBox.Show(questionStr, Resources.NewerVersionFound, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        if (!url_server.StartsWith("http"))
                        {
                            urlNewVersionFile = "file:" + urlNewVersionFile;
                            System.Diagnostics.Process.Start(url_server, newerFile);

                        }
                        else
                        {
                            System.Diagnostics.Process.Start(urlNewVersionFile);
                        }
                    }

                }
                else
                {
                    string message = string.Format(Resources.LatestVersionInfo, verStr);
                    MessageBox.Show(message, Resources.BehaviacDesigner, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void CheckVersionAsync()
        {
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();

                System.Threading.AutoResetEvent waiter = new System.Threading.AutoResetEvent(false);
                wc.DownloadDataCompleted += new System.Net.DownloadDataCompletedEventHandler(DownloadDataCompletedCallback);

                System.Uri uri = new System.Uri(urlVersionFile);

                wc.DownloadDataAsync(uri, waiter);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void DownloadDataCompletedCallback(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            System.Threading.AutoResetEvent waiter = (System.Threading.AutoResetEvent)e.UserState;

            try
            {
                // If the request was not canceled and did not throw
                // an exception, display the resource.
                if (!e.Cancelled && e.Error == null)
                {
                    byte[] data = (byte[])e.Result;

                    String[] datas = new System.Text.ASCIIEncoding().GetString(data).Split('|');

                    if (datas[0].Length < 4 * 4)
                    {
                        string verCheck = TrimVersionString(datas[0]);
                        int newestVersion = ConvertVertionToInt32(verCheck);

                        string verStr = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                        int Version = ConvertVertionToInt32(verStr);

                        if (Version < newestVersion)
                        {
                            string newerFile = "BehaviacSetup_" + verCheck + ".exe";
                            string urlNewVersionFile = url_server + newerFile;

                            string questionStr = string.Format(Resources.NewerVersionInfo, newerFile, urlNewVersionFile);
                            DialogResult dr = MessageBox.Show(questionStr, Resources.NewerVersionFound, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (dr == DialogResult.Yes)
                            {
                                if (!url_server.StartsWith("http"))
                                {
                                    urlNewVersionFile = "file:" + urlNewVersionFile;
                                    System.Diagnostics.Process.Start(url_server, newerFile);

                                }
                                else
                                {
                                    System.Diagnostics.Process.Start(urlNewVersionFile);
                                }
                            }
                        }
                    }
                }
            }

            finally
            {
                // Let the main application thread resume.
                waiter.Set();
            }
        }

        #endregion

        public delegate void TickDelegate();
        public event TickDelegate TickDelegateHandler;

        internal Timer Timer
        {
            get
            {
                return timer;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (TickDelegateHandler != null)
            {
                TickDelegateHandler();
            }
        }

        private void expandMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView.SelectedNode != null)
            {
                this.treeView.SelectedNode.ExpandAll();
            }

            else
            {
                this.treeView.ExpandAll();
            }
        }

        private void collapseMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView.SelectedNode != null)
            {
                this.treeView.SelectedNode.Collapse();
            }

            else
            {
                this.treeView.CollapseAll();
            }
        }

        private void exportMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView.SelectedNode != null)
            {
                NodeTag nodetag = (NodeTag)this.treeView.SelectedNode.Tag;

                if (nodetag.Type == NodeTagType.Behavior)
                {
                    BehaviorNode behavior = LoadBehavior(nodetag.Filename);

                    if (behavior != null)
                    {
                        ExportBehavior(behavior);
                    }

                }
                else if (nodetag.Type == NodeTagType.BehaviorFolder)
                {
                    ExportBehavior(null, "", false, this.treeView.SelectedNode);
                }

            }
            else
            {
                ExportBehavior(null);
            }
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            deleteButton_Click(sender, null);
        }

        internal void DumpLogFile()
        {
            using(SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save Dump File";
                saveFileDialog.Filter = "*.dump|*.dump";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    AgentDataPool.Save(saveFileDialog.FileName, Workspace.Current.FileName);
                }
            }
        }

        internal void SaveAll()
        {
            // store the behavior which is currently shown
            BehaviorTreeView behaviorTreeView = getFocusedView();
            BehaviorNode currentNode = (behaviorTreeView == null) ? null : behaviorTreeView.RootNode;

            // save all the newly created behaviors
            if (_newBehaviors.Count > 0)
            {
                BehaviorNode[] newBehaviors = new BehaviorNode[_newBehaviors.Count];
                int i = 0;
                foreach (BehaviorNode node in _newBehaviors)
                {
                    newBehaviors[i] = node;
                    i++;
                }

                foreach (BehaviorNode node in newBehaviors)
                {
                    try
                    {
                        SaveBehavior(node, false, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.SaveError, MessageBoxButtons.OK);
                    }
                }
            }

            // save all the modified behaviors
            foreach (BehaviorNode node in _loadedBehaviors)
            {
                if (node.IsModified)
                {
                    try
                    {
                        SaveBehavior(node, false, false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Resources.SaveError, MessageBoxButtons.OK);
                    }
                }
            }

            // restore the previously shown behavior
            if (currentNode != null && ShowBehavior != null)
            {
                ShowBehavior(currentNode);
            }
        }

        internal void ForceSaveAll()
        {
            // save all the newly created behaviors
            foreach (BehaviorNode node in _newBehaviors)
            {
                Workspace.EnableFileWatcher(false);

                try
                {
                    //SaveBehavior(node, false);
                    if (node.FileManager != null)
                    {
                        node.FileManager.Save();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.SaveError, MessageBoxButtons.OK);
                }

                finally
                {
                    Workspace.EnableFileWatcher(true);
                }
            }

            // save all the modified behaviors
            foreach (BehaviorNode node in _loadedBehaviors)
            {
                try
                {
                    Workspace.EnableFileWatcher(false);

                    //SaveBehavior(node, false);
                    if (node.FileManager != null)
                    {
                        node.FileManager.Save();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Resources.SaveError, MessageBoxButtons.OK);
                }

                finally
                {
                    Workspace.EnableFileWatcher(true);
                }
            }
        }

        private void forceSaveAllMenuItem_Click(object sender, EventArgs e)
        {
            ForceSaveAll();
        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {
            MainWindow.Instance.ExportBehavior(true);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (this.treeView.SelectedNode != null)
            {
                bool enabled = (Plugin.EditMode == EditModes.Design);
                NodeTag nodetag = (NodeTag)treeView.SelectedNode.Tag;
                enabled &= (nodetag.Type == NodeTagType.Behavior || nodetag.Type == NodeTagType.BehaviorFolder);

                this.exportMenuItem.Enabled = enabled;

                this.expandMenuItem.Enabled = true;
                this.collapseMenuItem.Enabled = true;
                this.deleteMenuItem.Enabled = true;
                this.saveBehaviorContextMenuItem.Enabled = true;
                this.saveAsBehaviorContextMenuItem.Enabled = true;

            }
            else
            {
                this.expandMenuItem.Enabled = false;
                this.collapseMenuItem.Enabled = false;
                this.deleteMenuItem.Enabled = false;
                this.saveBehaviorContextMenuItem.Enabled = false;
                this.saveAsBehaviorContextMenuItem.Enabled = false;
                this.exportMenuItem.Enabled = false;
            }
        }

        private void findFileButton_Click(object sender, EventArgs e)
        {
            FindFileDialog.Inspect();
        }

        private void openBehaviorMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenBehavior(treeView.SelectedNode, true);
        }

        private void newBehaviorMenuItem_Click(object sender, EventArgs e)
        {
            this.NewBehavior();
        }

        private void createGroupMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateGroup();
        }

        private void renameMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView.SelectedNode != null)
            {
                this.treeView.SelectedNode.BeginEdit();
            }
        }

        private void saveBehaviorContextMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                BehaviorNode behavior = getBehaviorByTreeNode(treeView.SelectedNode);

                MainWindow.Instance.SaveBehavior(behavior, false);
            }
        }

        private void saveAsBehaviorContextMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                BehaviorNode behavior = getBehaviorByTreeNode(treeView.SelectedNode);

                MainWindow.Instance.SaveBehavior(behavior, true);
            }
        }
    }
}
