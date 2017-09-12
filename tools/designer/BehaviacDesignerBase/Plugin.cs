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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Behaviac.Design.Data;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;

namespace Behaviac.Design
{
    /// <summary>
    /// This enumeration decribes what type of node is used in the node explorer.
    /// It is only used for displaying the nodes in the explorer and to handle drag & drop actions.
    /// </summary>
    public enum NodeTagType { Behavior, BehaviorFolder, Prefab, PrefabFolder, Node, NodeFolder, Attachment };

    /// <summary>
    /// The NodeTag is used to identify nodes in the explorer. Each TreeViewItem.Tag is a NodeTag.
    /// It is only used for displaying the nodes in the explorer and to handle drag & drop actions.
    /// </summary>
    public class NodeTag
    {
        protected NodeTagType _type;

        /// <summary>
        /// The type of the node in the node explorer.
        /// </summary>
        public NodeTagType Type
        {
            get
            {
                return _type;
            }
        }

        protected Type _nodetype;

        /// <summary>
        /// The type of the node which will be created in the graph.
        /// </summary>
        public Type NodeType
        {
            get
            {
                return _nodetype;
            }
        }

        protected string _filename;

        /// <summary>
        /// The filename of the behaviour which will be loaded when we double-click it.
        /// </summary>
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;

                if (_defaults != null && _defaults.Behavior != null)
                {
                    _defaults.Behavior.Filename = value;
                }
            }
        }

        protected DefaultObject _defaults;

        /// <summary>
        /// A default instance of a node, used to get its description and things like these.
        /// The instance is automatically created for each node in the node explorer.
        /// </summary>
        public DefaultObject Defaults
        {
            get
            {
                return _defaults;
            }
        }

        /// <summary>
        /// Used to replace the default object with a behaviour once loaded.
        /// </summary>
        /// <param name="behavior">The behaviour we have loaded.</param>
        public void AssignLoadedBehavior(BehaviorNode behavior)
        {
            Debug.Check(_type == NodeTagType.Behavior || _type == NodeTagType.Prefab);
            Debug.Check(behavior.FileManager == null || _filename == behavior.FileManager.Filename);

            _defaults = (DefaultObject)behavior;
        }

        /// <summary>
        /// Creates a new NodeTag and an instance of the node for the defaults.
        /// </summary>
        /// <param name="type">The type of the node in the node explorer.</param>
        /// <param name="nodetype">The type of the node which will be added to the behaviour tree.</param>
        /// <param name="filename">The filename of the behaviour we want to load. Use string.Empty if the node is not a behaviour.</param>
        public NodeTag(NodeTagType type, Type nodetype, string filename)
        {
            if ((type == NodeTagType.BehaviorFolder || type == NodeTagType.PrefabFolder || type == NodeTagType.NodeFolder) && nodetype != null)
            {
                throw new Exception(Resources.ExceptionWrongNodeTagType);
            }

            _type = type;
            _nodetype = nodetype;

            if (nodetype == null)
            {
                _defaults = null;

            }
            else
            {
                //if(!nodetype.IsSubclassOf(typeof(DefaultObject)))
                //	throw new Exception(Resources.ExceptionNotImplementDefaultObject);

                if (nodetype.IsSubclassOf(typeof(Attachments.Attachment)) && type != NodeTagType.Attachment)
                {
                    throw new Exception(Resources.ExceptionWrongNodeTagType);
                }

                if (nodetype.IsSubclassOf(typeof(Nodes.Node)) && type != NodeTagType.Node && type != NodeTagType.Behavior && type != NodeTagType.Prefab)
                {
                    throw new Exception(Resources.ExceptionWrongNodeTagType);
                }

                switch (type)
                {
                    case NodeTagType.Attachment:
                        _defaults = (DefaultObject)Behaviac.Design.Attachments.Attachment.Create(nodetype, null);
                        break;

                    default:
                        _defaults = (DefaultObject)Nodes.Node.Create(nodetype);
                        break;
                }
            }

            Filename = filename;
        }
    }

    /// <summary>
    /// This enumeration represents the icons which are available for the nodes in the explorer. The order and number must be the same as for the ImageList in BehaviorTreeList [Design].
    /// </summary>
    public enum NodeIcon { FlagBlue, FlagGreen, FlagRed, Behavior, BehaviorLoaded, BehaviorModified, Condition, Time, Action, Decorator, Sequence, Selector, Parallel, FolderClosed, FolderOpen, Event, Override, PrimitiveTask, CompoundTask, Method, Precondition, Operator, Prefab, And, Or, Not, False, True, Assignment, Noop, Wait, Query, EventHandle, Loop, LoopUntil, Log, WaitFrame };

    public class NodeItem
    {
        private Type _type;
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        private NodeIcon _icon;
        public NodeIcon Icon
        {
            get
            {
                return _icon;
            }
        }

        public NodeItem(Type type, NodeIcon icon)
        {
            _type = type;
            _icon = icon;
        }
    }

    /// <summary>
    /// This class describes a group which will be shown in the node explorer.
    /// </summary>
    public class NodeGroup
    {
        protected string _name = string.Empty;

        /// <summary>
        /// The name of the group which will be displayed in the node explorer.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        protected NodeIcon _icon = NodeIcon.FolderClosed;

        /// <summary>
        /// The icon of the node and its children which will be displayed in the node explorer.
        /// Notice that for behaviours, other icons than the given one will be used.
        /// </summary>
        public NodeIcon Icon
        {
            get
            {
                return _icon;
            }
        }

        protected List<NodeGroup> _children = new List<NodeGroup>();

        /// <summary>
        /// Groups which will be shown below this one.
        /// </summary>
        public IList<NodeGroup> Children
        {
            get
            {
                return _children;
            }
        }

        protected List<NodeItem> _items = new List<NodeItem>();

        /// <summary>
        /// Nodes which will be shown in this group.
        /// </summary>
        public List<NodeItem> Items
        {
            get
            {
                return _items;
            }
        }

        public bool ContainType(Type type)
        {
            return null != Items.Find(delegate(NodeItem item)
            {
                return item.Type == type;
            });
        }

        /// <summary>
        /// Adds this NodeGroup to the TreeView of the node explorer.
        /// </summary>
        /// <param name="pool">The TreeNodeCollection the group and its sub-groups and childrens will be added.</param>
        public void Register(TreeNodeCollection pool)
        {
            // check if this NodeGroup already exists in the node explorer
            TreeNode tnode = null;

            foreach (TreeNode node in pool)
            {
                if (node.Text == _name)
                {
                    tnode = node;
                    break;
                }
            }

            // create a new group if it does not yet exist
            if (tnode == null)
            {
                tnode = new TreeNode(_name, (int)_icon, (int)_icon);
                tnode.Tag = new NodeTag(NodeTagType.NodeFolder, null, string.Empty);
                pool.Add(tnode);
            }

            // add the nodes which will be shown in this group
            foreach (NodeItem item in _items)
            {
                NodeTagType ntt = NodeTagType.Node;

                if (item.Type.IsSubclassOf(typeof(Attachments.Attachment)))
                {
                    ntt = NodeTagType.Attachment;
                }

                NodeTag nodetag = new NodeTag(ntt, item.Type, string.Empty);
                TreeNode inode = new TreeNode(nodetag.Defaults.Label, (int)item.Icon, (int)item.Icon);
                inode.Tag = nodetag;
                inode.ToolTipText = nodetag.Defaults.Description;

                tnode.Nodes.Add(inode);
            }

            // add any sub-group
            foreach (NodeGroup group in _children)
            {
                group.Register(tnode.Nodes);
            }

            tnode.ExpandAll();
        }

        /// <summary>
        /// Defines a new NodeGroup which will be shown in the node explorer.
        /// </summary>
        /// <param name="name">The displayed name of the group.</param>
        /// <param name="icon">The displayed icon of the group and its children.</param>
        /// <param name="parent">The parent of the group, can be null.</param>
        public NodeGroup(string name, NodeIcon icon, NodeGroup parent)
        {
            _name = name;
            _icon = icon;

            if (parent != null)
            {
                parent.Children.Add(this);
            }
        }
    }

    /// <summary>
    /// This class holds information about a file manager which can be used to load and save behaviours.
    /// </summary>
    public class FileManagerInfo
    {
        /// <summary>
        /// Returns a file manager which can be used to save or load a behaviour.
        /// </summary>
        /// <param name="filename">The name of the file we want to load or we want to save to.</param>
        /// <param name="node">The behaviour we want to load or save.</param>
        /// <returns>Returns the file manager which will be created.</returns>
        public FileManagers.FileManager Create(string filename, Nodes.BehaviorNode node)
        {
            object[] prms = new object[2] { filename, node };
            return (FileManagers.FileManager)_type.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, prms);
        }

        /// <summary>
        /// Defines a file manager which can be used to load and save behaviours.
        /// </summary>
        /// <param name="filemanager">The tpe of the file manager which will be created by this info.</param>
        /// <param name="filter">The text displayed in the save dialogue when selecting the file format.</param>
        /// <param name="fileextension">The file extension used to identify which file manager can handle the given file.</param>
        public FileManagerInfo(Type filemanager, string filter, string fileextension)
        {
            _filter = filter;
            _fileExtension = fileextension.ToLowerInvariant();
            _type = filemanager;

            // the file extension must always start with a dot
            if (_fileExtension[0] != '.')
            {
                _fileExtension = '.' + _fileExtension;
            }
        }

        protected Type _type;

        /// <summary>
        /// The type of the file manager which will be created.
        /// </summary>
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        protected string _filter;

        /// <summary>
        /// The displayed text in the save dialogue when selecting the file format.
        /// </summary>
        public string Filter
        {
            get
            {
                return _filter;
            }
        }

        protected string _fileExtension;

        /// <summary>
        /// The extension used to determine which file manager can handle the given file.
        /// </summary>
        public string FileExtension
        {
            get
            {
                return _fileExtension;
            }
        }
    }

    /// <summary>
    /// This class holds information about an exporter which can be used to export a behaviour into a format which can be used by the workflow of your game.
    /// </summary>
    public class ExporterInfo
    {
        /// <summary>
        /// Creates an instance of an exporter which will be used to export a behaviour.
        /// To export the behaviour, the Export() method must be called.
        /// </summary>
        /// <param name="node">The behaviour you want to export.</param>
        /// <param name="outputFolder">The folder you want to export the behaviour to.</param>
        /// <param name="filename">The relative filename you want to export the behaviour to.</param>
        /// <returns>Returns the created exporter.</returns>
        public Exporters.Exporter Create(Nodes.BehaviorNode node, string outputFolder, string filename, List<string> includedFilenames = null)
        {
            object[] prms = new object[] { node, outputFolder, filename, includedFilenames };
            return (Exporters.Exporter)_type.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, prms);
        }

        /// <summary>
        /// Defines an exporter which can be used to export a behaviour.
        /// </summary>
        /// <param name="exporter">The type of the exporter which will be created.</param>
        /// <param name="id">The id of the exporter which will be used by the Behaviac Exporter to identify which exporter to use.</param>
        /// <param name="description">The description which will be shown when the user must select the exporter (s)he wants to use.</param>
        /// <param name="mayExportAll">Determines if this exporter may be used to export multiple behaviours.
        /// This can be important if the output requires further user actions.</param>
        public ExporterInfo(Type exporter, string id, string description, bool mayExportAll, bool hasSettings = false)
        {
            _type = exporter;
            _id = id;
            _description = description;
            _mayExportAll = mayExportAll;
            _hasSettings = hasSettings;
        }

        protected Type _type;

        /// <summary>
        /// The type of the exporter which will be created.
        /// </summary>
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        private string _id;

        /// <summary>
        /// The id used to identify an exporter when being used with the Behaviac Exporter.
        /// </summary>
        public string ID
        {
            get
            {
                return _id;
            }
        }

        protected string _description;

        /// <summary>
        /// The displayed text when the user must select which exporter to use.
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// This is needed for the drop down list in the export dialogue to show the correct text.
        /// </summary>
        /// <returns>Returns the description of the exporter.</returns>
        public override string ToString()
        {
            return _description;
        }

        protected bool _mayExportAll;

        /// <summary>
        /// Determines if the exporter can be used when exporting multiple files. For example if further user actions are required.
        /// </summary>
        public bool MayExportAll
        {
            get
            {
                return _mayExportAll;
            }
        }

        /// <summary>
        /// Determines if the exported file format has settings
        /// </summary>
        protected bool _hasSettings = false;
        public bool HasSettings
        {
            get
            {
                return _hasSettings;
            }
        }
    }

    /// <summary>
    /// All editting modes of the editor.
    /// </summary>
    public enum EditModes
    {
        Design,
        Connect,
        Analyze
    }

    /// <summary>
    /// All updating modes of the timeline of the editor.
    /// </summary>
    public enum UpdateModes
    {
        Continue,
        Break
    }

    public enum MetaOperations
    {
        ChangeAgentType,
        RemoveAgentType,
        CheckAgentName,
        RenameAgentType,
        CheckEnumName,
        RenameEnumType,
        CheckStructName,
        RenameStructType,
        CheckProperty,
        ChangeProperty,
        RemoveProperty,
        CheckMethod,
        ChangeMethod,
        RemoveMethod,
    }

    public struct ObjectPair
    {
        public ObjectPair(Nodes.Node root, DefaultObject obj)
        {
            Root = root;
            Obj = obj;
        }

        public Nodes.Node Root;
        public DefaultObject Obj;
    }

    /// <summary>
    /// The base class for every plugin. The class name of your plugin must be the same as your library.
    /// </summary>
    public class Plugin
    {
        public delegate void UpdateMetaStoreDelegate(object dock);
        public static UpdateMetaStoreDelegate UpdateMetaStoreHandler;

        public delegate void PostSetWorkspaceDelegate();
        public static PostSetWorkspaceDelegate PostSetWorkspaceHandler;

        private static bool _useBasicDisplayName = true;
        public static bool UseBasicDisplayName
        {
            get
            {
                return _useBasicDisplayName;
            }
            set
            {
                _useBasicDisplayName = value;
            }
        }

        public delegate void EditModeDelegate(EditModes preEditMode, EditModes curEditMode);
        public static event EditModeDelegate EditModeHandler;
        private static EditModes _editMode = EditModes.Design;

        public static EditModes EditMode
        {
            get
            {
                return _editMode;
            }

            set
            {
                DebugAgentInstance = string.Empty;

                if (value != EditModes.Design)
                {
                    AgentInstancePool.Clear();
                    FrameStatePool.Clear();
                    AgentDataPool.Clear();

                    if (value == EditModes.Connect)
                    {
                        Plugin.UpdateMode = UpdateModes.Continue;
                    }

                    else
                    {
                        Plugin.UpdateMode = UpdateModes.Break;
                    }
                }

                if (_editMode != value)
                {
                    EditModes preEditMode = _editMode;
                    _editMode = value;

                    if (EditModeHandler != null)
                    {
                        EditModeHandler(preEditMode, value);
                    }
                }
            }
        }

        private static bool wrongWorksapceReported = false;
        public static bool WrongWorksapceReported
        {
            get
            {
                return wrongWorksapceReported;
            }
            set
            {
                wrongWorksapceReported = value;
            }
        }

        public delegate void UpdateModeDelegate(UpdateModes updateMode);
        public static event UpdateModeDelegate UpdateModeHandler;
        private static UpdateModes _updateMode = UpdateModes.Continue;

        public static UpdateModes UpdateMode
        {
            get
            {
                return _updateMode;
            }

            set
            {
                if (value == UpdateModes.Continue)
                {
                    HighlightBreakPoint.Instance = null;
                }

                //if (_updateMode != value)
                {
                    _updateMode = value;

                    if (UpdateModeHandler != null)
                    {
                        UpdateModeHandler(value);
                    }
                }
            }
        }

        public delegate void DebugAgentDelegate(string agentName);
        public static event DebugAgentDelegate DebugAgentHandler;
        private static string _debugAgentInstance = string.Empty;

        public static string DebugAgentInstance
        {
            get
            {
                return _debugAgentInstance;
            }

            set
            {
                //if (_debugAgentInstance != value)
                {
                    _debugAgentInstance = value;

                    if (DebugAgentHandler != null)
                    {
                        DebugAgentHandler(value);
                    }
                }
            }
        }

        //public class ConcurrentDictionary<TKey, TValue>
        //{
        //    private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
        //    private object _lockObject = new object();

        //    public void Add(TKey key, TValue value)
        //    {
        //        lock(_lockObject)
        //        {
        //            _dict.Add(key, value);
        //        }
        //    }

        //    public void Clear()
        //    {
        //        lock (_lockObject)
        //        {
        //            _dict.Clear();
        //        }
        //    }
            
        //    public bool ContainsKey(TKey key)
        //    {
        //        lock (_lockObject)
        //        {
        //            return _dict.ContainsKey(key);
        //        }
        //    }
            
        //    public bool Remove(TKey key)
        //    {
        //        lock (_lockObject)
        //        {
        //            return _dict.Remove(key);
        //        }
        //    }

        //    public TValue this[TKey key]
        //    {
        //        get
        //        {
        //            lock (_lockObject)
        //            {
        //                return _dict[key];
        //            }
        //        }

        //        set
        //        {
        //            lock (_lockObject)
        //            {
        //                _dict[key] = value;
        //            }
        //        }
        //    }

        //    public TValue GetValueUnsafe(TKey key)
        //    {
        //        if (_dict.ContainsKey(key))
        //        {
        //            return _dict[key];
        //        }

        //        return default(TValue);
        //    }
        //}

        //private static bool _concurrentProcessBehaviors = false;
        //public static bool ConcurrentProcessBehaviors
        //{
        //    get
        //    {
        //        return _concurrentProcessBehaviors;
        //    }
        //    set
        //    {
        //        _concurrentProcessBehaviors = value;
        //    }
        //}

        //// source
        //private static ConcurrentDictionary<string, FileStream> _allSourceFileStreams = new ConcurrentDictionary<string, FileStream>();
        //public static ConcurrentDictionary<string, FileStream> ConcurrentSourceFileStreams
        //{
        //    get
        //    {
        //        return _allSourceFileStreams;
        //    }
        //}

        //// xml
        //private static ConcurrentDictionary<string, StreamWriter> _allStreamWriters = new ConcurrentDictionary<string, StreamWriter>();
        //public static ConcurrentDictionary<string, StreamWriter> ConcurrentXmlStreamWriters
        //{
        //    get
        //    {
        //        return _allStreamWriters;
        //    }
        //}

        //// bson
        //private static ConcurrentDictionary<string, FileStream> _allFileStreams = new ConcurrentDictionary<string, FileStream>();
        //public static ConcurrentDictionary<string, FileStream> ConcurrentBsonFileStreams
        //{
        //    get
        //    {
        //        return _allFileStreams;
        //    }
        //}

        private static FrameStatePool.PlanningProcess _planningProcess;
        public static FrameStatePool.PlanningProcess PlanningProcess
        {
            get
            {
                return _planningProcess;
            }
            set
            {
                _planningProcess = value;
            }
        }

        public static bool IsMatchedStatusMethod(MethodDef method, MethodDef resultFunctor)
        {
            if (method != null &&
                resultFunctor != null &&
                resultFunctor.NativeReturnType == "behaviac::EBTStatus")
            {
                if (method.NativeReturnType == "void")
                {
                    return (resultFunctor.Params.Count == 0);
                }

                return (resultFunctor.Params.Count == 1) &&
                       (resultFunctor.Params[0].Type == method.ReturnType);
            }

            return false;
        }

        public static string Signature
        {
            get
            {
                string totalSignature = "";

                foreach (AgentType agent in Plugin.AgentTypes)
                {
                    totalSignature += agent.GetSignature();
                }

                foreach (EnumType e in TypeManager.Instance.Enums)
                {
                    if (e.IsCustomized)
                    {
                        totalSignature += e.Signature;
                    }
                }

                foreach (StructType s in TypeManager.Instance.Structs)
                {
                    if (s.IsCustomized)
                    {
                        totalSignature += s.Signature;
                    }
                }

                return totalSignature;
            }
        }

        public delegate bool WorkspaceDelegate(string workspacePath, bool bNew);
        public static WorkspaceDelegate WorkspaceDelegateHandler;

        private static Assembly _DesignerBaseDll = null;
        public Plugin()
        {
            if (_DesignerBaseDll == null)
            {
                _DesignerBaseDll = Assembly.GetAssembly(typeof(Behaviac.Design.TypeHandlerAttribute));

                Plugin.RegisterTypeHandlers(_DesignerBaseDll);
                Plugin.RegisterAgentTypes(_DesignerBaseDll);

                Plugin.InitNodeGroups();

                Plugin.RegisterNodeDesc(_DesignerBaseDll);
            }
        }

        /// <summary>
        /// A global list of all plugins which have been loaded. Mainly for internal use.
        /// </summary>
        private static List<Assembly> __loadedPlugins = new List<Assembly>();

        public static void UnLoadPlugins()
        {
            __loadedPlugins.Clear();
            _exporters.Clear();
        }

        public static IList<Assembly> GetLoadedPlugins()
        {
            return __loadedPlugins.AsReadOnly();
        }

        public delegate void RegisterPluginDelegate(Assembly assembly, Plugin plugin, ImageList imgList, bool bAddNodes);

        public static void LoadPlugins(RegisterPluginDelegate registerPlugin = null, ImageList imgList = null, bool bAddNodes = false)
        {
            // get all the DLL files
            string appDir = Path.GetDirectoryName(Application.ExecutablePath);
            string[] files = Directory.GetFiles(appDir, "*.dll", SearchOption.TopDirectoryOnly);

            // store if we are a 64-bit system or not
            bool is64 = (IntPtr.Size == 8);

            // load plugin DLLs
            foreach (string file in files)
            {
                //skip BehaviacDesignerBase.dll
                if (file.IndexOf("BehaviacDesignerBase") != -1)
                {
                    continue;
                }

                // store system information
                bool fileIs32 = file.EndsWith("32.dll", StringComparison.InvariantCultureIgnoreCase);
                bool fileIs64 = !fileIs32 && file.EndsWith("64.dll", StringComparison.InvariantCultureIgnoreCase);

                // skip plugins which are not made for our system
                if (fileIs32 && is64 || fileIs64 && !is64)
                {
                    continue;
                }

                // load file
                Assembly assembly = Assembly.LoadFile(Path.GetFullPath(file));

                // create an instance of the plugin class of the same name as the file
                string classname = Path.GetFileNameWithoutExtension(file);

                // remove system information
                if (fileIs32 || fileIs64)
                {
                    classname = classname.Substring(0, classname.Length - 2);
                }

                Plugin plugin = (Plugin)assembly.CreateInstance(classname + ".Plugin");

                if (plugin != null && registerPlugin != null)
                {
                    registerPlugin(assembly, plugin, imgList, bAddNodes);
                }

                if (imgList != null)
                {
                    ImageList nodeImageList = imgList as ImageList;

                    foreach (Image image in Plugin.ImagesList)
                    {
                        nodeImageList.Images.Add(image);
                    }
                }
            }
        }

        /// <summary>
        /// Add a plugin which has been loaded. Mainly for internal use.
        /// </summary>
        /// <param name="a"></param>
        public static void AddLoadedPlugin(Assembly a)
        {
            __loadedPlugins.Add(a);
        }

        public static string[] Split(string str, char delimiter)
        {
            List<string> result = new List<string>();

            string item = "";
            bool bInQuote = false;

            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '\"')
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        result.Add(item);
                        item = "";
                    }

                    if (bInQuote)
                    {
                        bInQuote = false;

                    }
                    else
                    {
                        bInQuote = true;
                    }
                }
                else if (!bInQuote && (str[i] == delimiter))
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        result.Add(item);
                        item = "";
                    }
                }
                else
                {
                    item += str[i];
                }
            }

            //the last token
            if (result.Count > 0 && !string.IsNullOrEmpty(item))
            {
                result.Add(item);
            }

            if (result.Count > 1)
            {
                return result.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Allows a plugin to register the types from an additional assembly.
        /// </summary>
        /// <param name="filename">The filename of the assembly we want to register.</param>
        protected static void RegisterAdditionalAssembly(string filename)
        {
            // load assembly
            Assembly assembly = Assembly.LoadFile(Path.GetFullPath(filename));

            if (assembly == null)
            {
                throw new Exception(Resources.ExceptionAdditionalAssemblyNotLoaded);
            }

            AddLoadedPlugin(assembly);

            // register resource manager if we can find one
            string resources = Path.GetFileNameWithoutExtension(filename) + ".Properties.Resources";
            ResourceManager resman = new ResourceManager(resources, assembly);

            AddResourceManager(resman);
        }

        /// <summary>
        /// Returns the type of a given class name. It searches all loaded plugins for this type.
        /// </summary>
        /// <param name="fullname">The name of the class we want to get the type for.</param>
        /// <returns>Returns the type if found in any loaded plugin. Retuns null if it could not be found.</returns>
        public static Type GetType(string fullname)
        {
            // search base class
            Type type = Type.GetType(fullname);

            if (type != null)
            {
                return type;
            }

            bool bHasNamespace = false;

            if (fullname.IndexOf('.') != -1)
            {
                bHasNamespace = true;
            }

            // search loaded plugins
            foreach (Assembly assembly in __loadedPlugins)
            {
                if (bHasNamespace)
                {
                    type = assembly.GetType(fullname);

                }
                else
                {
                    Type[] assemblyTypes = assembly.GetTypes();

                    for (int j = 0; j < assemblyTypes.Length; j++)
                    {
                        if (assemblyTypes[j].Name == fullname)
                        {
                            type = assemblyTypes[j];
                            break;
                        }
                    }
                }

                if (type != null)
                {
                    return type;
                }
            }

            // it could be a List<> type
            if (fullname.StartsWith("System.Collections.Generic.List"))
            {
                int startIndex = fullname.IndexOf("[[");

                if (startIndex > -1)
                {
                    int endIndex = fullname.IndexOf(",");

                    if (endIndex < 0)
                    {
                        endIndex = fullname.IndexOf("]]");
                    }

                    if (endIndex > startIndex)
                    {
                        string item = fullname.Substring(startIndex + 2, endIndex - startIndex - 2);
                        type = Plugin.GetType(item);

                        if (type != null)
                        {
                            type = typeof(List<>).MakeGenericType(type);
                            return type;
                        }
                    }
                }
            }

            return null;
        }

        public static Type FindType(string name)
        {
            // search base class
            Type type = Type.GetType(name);

            if (type != null)
            {
                return type;
            }

            // search loaded plugins
            foreach (Assembly assembly in __loadedPlugins)
            {
                string fullname = Path.GetFileNameWithoutExtension(assembly.Location) + name;
                type = assembly.GetType(fullname);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the types based of a given type. It searches all loaded plugins for this type.
        /// </summary>
        /// <param name="baseType">the base type.</param>
        /// <returns></returns>
        public static Type[] GetType(Type baseType)
        {
            List<Type> a = new List<Type>();

            // search base class
            if (baseType == null)
            {
                return a.ToArray();
            }

            // search loaded plugins
            foreach (Assembly assembly in __loadedPlugins)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type == baseType || type.IsSubclassOf(baseType))
                    {
                        a.Add(type);
                    }
                }
            }

            return a.ToArray();
        }

        public static bool IsASCII(string value)
        {
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        public static bool IsValidFilename(string value)
        {
            if (IsASCII(value))
            {
                return Regex.IsMatch(value, @"^[a-zA-Z0-9_]*$");
            }

            return false;
        }

        /// <summary>
        /// The type used to create behaviour nodes. Must implement the interface Nodes.BehaviorNode.
        /// </summary>
        private static Type __usedBehaviorNodeType = typeof(Nodes.Behavior);

        /// <summary>
        /// The type used to create behaviour nodes. Must implement the interface Nodes.BehaviorNode.
        /// </summary>
        public static Type BehaviorNodeType
        {
            get
            {
                return __usedBehaviorNodeType;
            }
        }

        /// <summary>
        /// Defines the type used when behaviour nodes are created.
        /// </summary>
        /// <param name="type">The type used to create behaviour nodes. Must implement Nodes.BehaviorNode.</param>
        protected void SetUsedBehaviorNodeType(Type type)
        {
            if (!typeof(Nodes.BehaviorNode).IsAssignableFrom(type) || type.IsAbstract)
            {
                throw new Exception(Resources.ExceptionBehaviorNodeTypeInvalid);
            }

            __usedBehaviorNodeType = type;
        }

        /// <summary>
        /// The type used to create referenced behaviour nodes. Must implement the interface Nodes.ReferencedBehavior.
        /// </summary>
        private static Type __usedReferencedBehaviorNodeType = typeof(Nodes.ReferencedBehavior);

        /// <summary>
        /// The type used to create referenced behaviour nodes. Must implement the interface Nodes.ReferencedBehavior.
        /// </summary>
        public static Type ReferencedBehaviorNodeType
        {
            get
            {
                return __usedReferencedBehaviorNodeType;
            }
        }

        private static Type __fsmReferencedBehaviorNodeType = null;
        public static Type FSMReferencedBehaviorNodeType
        {
            get
            {
                return __fsmReferencedBehaviorNodeType;
            }
            set
            {
                __fsmReferencedBehaviorNodeType = value;
            }
        }

        /// <summary>
        /// Defines the type used when referenced behaviour nodes are created.
        /// </summary>
        /// <param name="type">The type used to create referenced behaviour nodes. Must implement Nodes.ReferencedBehavior.</param>
        protected void SetUsedReferencedBehaviorNodeType(Type type)
        {
            if (!typeof(Nodes.ReferencedBehavior).IsAssignableFrom(type) || type.IsAbstract)
            {
                throw new Exception(Resources.ExceptionReferencedBehaviorNodeTypeInvalid);
            }

            __usedReferencedBehaviorNodeType = type;
        }

        private static bool __allowReferencedBehaviors = true;

        /// <summary>
        /// Defines if the user may drag behaviours into the view.
        /// </summary>
        public static bool AllowReferencedBehaviors
        {
            get
            {
                return __allowReferencedBehaviors;
            }

            protected set
            {
                __allowReferencedBehaviors = value;
            }
        }

        // key : instance name

        public struct InstanceName_t
        {
            private string _name;
            public string Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                }
            }

            private string _className;
            public string ClassName
            {
                get
                {
                    return (_agentType != null) ? _agentType.Name : _className;
                }
                set
                {
                    _className = value;
                }
            }

            private AgentType _agentType;
            public AgentType AgentType
            {
                get
                {
                    return _agentType;
                }
                set
                {
                    _agentType = value;
                }
            }

            private string _displayName;
            public string DisplayName
            {
                get
                {
                    return _displayName;
                }
                set
                {
                    _displayName = value;
                }
            }

            private string _desc;
            public string Desc
            {
                get
                {
                    return _desc;
                }
                set
                {
                    _desc = value;
                }
            }
        }

        private static Dictionary<string, InstanceName_t> _instanceNamesDict = new Dictionary<string, InstanceName_t>();

        public static void AddInstanceName(string instance, string className, string displayName, string desc, AgentType agent = null)
        {
            InstanceName_t instanceName = new InstanceName_t();

            Debug.Check(!string.IsNullOrEmpty(instance));
            Debug.Check(!string.IsNullOrEmpty(className));

            instanceName.Name = instance;
            instanceName.ClassName = className;
            instanceName.DisplayName = displayName;
            instanceName.Desc = desc;
            instanceName.AgentType = agent;

            _instanceNamesDict[instance] = instanceName;
        }

        public static bool RemoveInstance(string instance)
        {
            return _instanceNamesDict.Remove(instance);
        }

        public static AgentType GetInstanceAgentType(string instance, Behavior behavior, AgentType selfType)
        {
            if (!string.IsNullOrEmpty(instance))
            {
                // self
                if (instance == VariableDef.kSelf)
                {
                    return selfType;
                }

                // global instances
                if (_instanceNamesDict.ContainsKey(instance))
                {
                    return _instanceNamesDict[instance].AgentType;
                }

                // local instances
                List<InstanceName_t> instances = GetLocalInstanceNames(behavior);

                foreach (InstanceName_t instanceName in instances)
                {
                    if (instanceName.Name == instance)
                    {
                        return instanceName.AgentType;
                    }
                }
            }

            return GetAgentType(instance);
        }

        public static bool IsInstanceName(string instance, Behavior behavior)
        {
            if (!string.IsNullOrEmpty(instance))
            {
                // global instances
                if (_instanceNamesDict.ContainsKey(instance))
                {
                    return true;
                }

                // local instances
                List<InstanceName_t> instances = GetLocalInstanceNames(behavior);

                foreach (InstanceName_t instanceName in instances)
                {
                    if (instanceName.Name == instance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int InstanceNameIndex(string instance, Behavior behavior)
        {
            instance = GetInstanceNameFromClassName(instance);

            // global instances
            int index = InstanceNames.FindIndex(delegate(InstanceName_t instanceName_t)
            {
                return instanceName_t.Name == instance;
            });

            if (index >= 0)
            {
                return index;
            }

            // local instances
            index = InstanceNames.Count;

            List<InstanceName_t> instances = GetLocalInstanceNames(behavior);

            foreach (InstanceName_t instanceName in instances)
            {
                if (instanceName.Name == instance)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static List<InstanceName_t> GetLocalInstanceNames(Behavior behavior)
        {
            List<InstanceName_t> instanceNames = new List<InstanceName_t>();

            if (behavior != null && behavior.AgentType != null)
            {
                foreach (PropertyDef prop in behavior.AgentType.GetProperties())
                {
                    if (Plugin.IsDerived(prop.Type, typeof(Agent)))
                    {
                        InstanceName_t instanceName = new InstanceName_t();

                        instanceName.Name = prop.BasicName;
                        instanceName.ClassName = prop.AgentType.Name;
                        instanceName.AgentType = Plugin.GetAgentType(prop.Type);
                        instanceName.DisplayName = prop.DisplayName;
                        instanceName.Desc = prop.BasicDescription;

                        instanceNames.Add(instanceName);
                    }
                }
            }

            return instanceNames;
        }

        public static string GetInstanceDisplayName(string instance)
        {
            if (instance == VariableDef.kSelf)
            {
                return VariableDef.kSelf;
            }

            if (_instanceNamesDict.ContainsKey(instance))
            {
                return _instanceNamesDict[instance].DisplayName;
            }

            AgentType at = GetAgentType(instance);

            if (at != null)
            {
                return at.DisplayName;
            }

            return instance;
        }

        public static string GetInstanceNameFromClassName(string clsName)
        {
            foreach (KeyValuePair<string, InstanceName_t> pair in _instanceNamesDict)
            {
                if (pair.Value.ClassName == clsName)
                {
                    return pair.Value.Name;
                }
            }

            return clsName;
        }

        public static string GetInstanceDesc(string instance)
        {
            if (instance == VariableDef.kSelf)
            {
                return VariableDef.kSelf;
            }

            if (_instanceNamesDict.ContainsKey(instance))
            {
                return _instanceNamesDict[instance].Desc;
            }

            AgentType at = GetAgentType(instance);

            if (at != null)
            {
                return at.Description;
            }

            return string.Empty;
        }

        //called when xml dll is loaded and after agent types are registered
        public static void PrepareInstanceTypes()
        {
            List<string> keys = new List<string>(_instanceNamesDict.Keys);

            for (int i = 0; i < keys.Count; ++i)
            {
                string k = keys[i];
                AgentType at = Plugin.GetAgentType(_instanceNamesDict[k].ClassName);

                Debug.Check(at != null);

                InstanceName_t instanceName = _instanceNamesDict[k];
                instanceName.AgentType = at;

                _instanceNamesDict[k] = instanceName;
            }
        }

        public static List<InstanceName_t> InstanceNames
        {
            get
            {
                return new List<InstanceName_t>(_instanceNamesDict.Values);
            }
        }

        public static bool IsGlobalInstanceAgentType(AgentType type)
        {
            return _instanceNamesDict.ContainsKey(type.Name);
        }

        public static string GetInstanceName(string str)
        {
            int pointIndex = str.IndexOf('.');

            if (pointIndex > -1)
            {
                int parenthesesPos = str.IndexOf('(');

                if (parenthesesPos < 0 || pointIndex < parenthesesPos)
                {
                    return str.Substring(0, pointIndex);
                }
            }

            return string.Empty;
        }

        public static bool NativeFunctions(string methodName)
        {
            if (methodName == "LogMessage" || methodName == "VectorLength" || methodName == "VectorAdd" || methodName == "VectorRemove" || methodName == "VectorContains" || methodName == "VectorClear")
            {
                return true;
            }

            return false;
        }

        public static string GetClassName(string str)
        {
            string className = string.Empty;

            if (!str.StartsWith("const"))
            {
                //"WorldTest::action1(int AgentBase::Base_Property1)"
                int posP = str.IndexOf('(');

                if (posP != -1)
                {
                    int pos = str.LastIndexOf(':', posP);

                    if (pos != -1)
                    {
                        Debug.Check(str[pos - 1] == ':');
                        className = str.Substring(0, pos - 1);
                    }
                }
                else
                {
                    //Int32 Property1
                    //to skip the type
                    string[] tokens = str.Split(' ');

                    string propertyType, propertyName;

                    if (tokens[0] == "static")
                    {
                        Debug.Check(tokens.Length == 3);

                        //e.g. static int Property;
                        propertyType = tokens[1];
                        propertyName = tokens[2];
                    }
                    else if (tokens.Length == 1)
                    {
                        propertyName = tokens[0];
                    }
                    else
                    {
                        Debug.Check(tokens.Length == 2);

                        //e.g. int Property;
                        propertyType = tokens[0];
                        propertyName = tokens[1];
                    }

                    int pos = propertyName.LastIndexOf(':');

                    if (pos != -1)
                    {
                        Debug.Check(propertyName[pos - 1] == ':');
                        className = propertyName.Substring(0, pos - 1);
                    }
                }
            }

            int pointIndex = className.IndexOf('.');

            if (pointIndex > -1)
            {
                className = className.Substring(pointIndex + 1, className.Length - pointIndex - 1);
            }

            return className;
        }

        public static bool IsRefType(Type type)
        {
            if (type != null)
            {
                if (type == typeof(System.Object) || Plugin.GetAgentType(type.Name) != null)
                {
                    return true;
                }

                Attribute[] attributes = (Attribute[])type.GetCustomAttributes(typeof(Behaviac.Design.ClassDescAttribute), false);

                if (attributes.Length > 0)
                {
                    Behaviac.Design.ClassDescAttribute classDesc = (Behaviac.Design.ClassDescAttribute)attributes[0];

                    // All agents should be ref type.
                    Debug.Check(Plugin.GetAgentType(type.Name) == null || classDesc.IsRefType);

                    return classDesc.IsRefType;
                }

                if (type.IsInterface)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsClassDerived(Type type, Type baseType)
        {
            return (type != null) && (baseType != null) && (type == baseType || type.IsSubclassOf(baseType));
        }

        private static Dictionary<string, string> _typeRenames = new Dictionary<string, string>();
        public static Dictionary<string, string> TypeRenames
        {
            get
            {
                return _typeRenames;
            }
        }

        private static List<string> _filterNodes = new List<string>();
        public static List<string> FilterNodes
        {
            get
            {
                return _filterNodes;
            }
        }

        private static bool _onlyShowFrequentlyUsedNodes = false;
        public static bool OnlyShowFrequentlyUsedNodes
        {
            get
            {
                return _onlyShowFrequentlyUsedNodes;
            }
            set
            {
                _onlyShowFrequentlyUsedNodes = value;
            }
        }

        private static List<string> _frequentlyUsedNodes = new List<string>();
        public static List<string> FrequentlyUsedNodes
        {
            get
            {
                return _frequentlyUsedNodes;
            }
        }

        private static List<AgentType> _agentTypes = new List<AgentType>();
        public static List<AgentType> AgentTypes
        {
            get
            {
                if (_agentTypes.Count == 0)
                {
                    AgentType baseAgent = new AgentType(typeof(Agent), "behaviac::Agent", "", false, false, "behaviac::Agent", "", false, true, "");
                    _agentTypes.Add(baseAgent);
                }

                return _agentTypes;
            }
        }

        public static AgentType GetAgentType(string typeName)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                if (Plugin.NamesInNamespace.ContainsKey(typeName))
                {
                    typeName = Plugin.NamesInNamespace[typeName];
                    typeName = typeName.Replace("*","");
                    typeName = typeName.Replace("&", "");
                }

                foreach (AgentType at in _agentTypes)
                {
                    if (at.Name == typeName
#if BEHAVIAC_NAMESPACE_FIX
                        || at.Name.EndsWith(typeName)
#endif
                       )
                    {
                        return at;
                    }
                }
            }

            return null;
        }

        public static AgentType GetAgentType(Type type)
        {
            if (type != null)
            {
                foreach (AgentType at in _agentTypes)
                {
                    if (at.Type == type)
                    {
                        return at;
                    }
                }
            }

            return null;
        }

        private static Dictionary<string, string> _agentTypeHierarchy = new Dictionary<string, string>();

        public static bool IsAgentDerived(string childType, string parentType)
        {
            if (childType == parentType)
            {
                return true;
            }

            if (!_agentTypeHierarchy.ContainsKey(childType))
            {
                return false;
            }

            string parentTypeName = _agentTypeHierarchy[childType];

            if (parentTypeName == childType)
            {
                return false;
            }

            return IsAgentDerived(parentTypeName, parentType);
        }

        public static bool IsDerived(Type childType, Type baseType)
        {
            if (childType == null || baseType == null)
            {
                return false;
            }

            Type kObjectType = typeof(Object);

            if (baseType == kObjectType || childType == baseType)
            {
                return true;
            }

            while (childType != kObjectType)
            {
                if (childType == baseType)
                {
                    return true;
                }

                childType = childType.BaseType;
            }

            return false;
        }

        public static void UnRegisterAgentTypes()
        {
            _instanceNamesDict.Clear();
            _agentTypes.Clear();
            _agentTypeHierarchy.Clear();
        }

        public static void RegisterAgentTypes(Assembly a)
        {
            Type[] types = a.GetTypes();

            foreach (Type type in types)
            {
                if (type.IsSubclassOf(typeof(Behaviac.Design.Agent)))
                {
                    string fullname = type.Name;
                    string oldName = "";
                    bool isInherited = false;
                    bool isStatic = false;
                    string displayName = type.Name;
                    string description = displayName;
                    bool isCustomized = false;
                    bool isImplemented = false;
                    string exportLocation = "";

                    Attribute[] attributes = (Attribute[])type.GetCustomAttributes(typeof(Behaviac.Design.ClassDescAttribute), false);

                    if (attributes.Length > 0)
                    {
                        ClassDescAttribute cda = (ClassDescAttribute)attributes[0];

                        fullname = cda.Fullname;
                        oldName = cda.OldName;
                        isInherited = cda.IsInherited;
                        isStatic = cda.IsStatic;
                        displayName = cda.DisplayName;
                        description = cda.Description;
                        isCustomized = cda.IsCustomized;
                        isImplemented = cda.IsImplemented;
                        exportLocation = cda.ExportLocation;

                        _agentTypeHierarchy[fullname] = cda.BaseName;
                    }

                    AgentType at = new AgentType(type, fullname, oldName, isInherited, isStatic, displayName, description, isCustomized, isImplemented, exportLocation);
                    _agentTypes.Add(at);
                }
            }
        }

        protected static void RegisterAgentTypes()
        {
            Assembly a = Assembly.GetCallingAssembly();

            if (a != _DesignerBaseDll)
            {
                Plugin.RegisterAgentTypes(a);
            }
        }

        static private Dictionary<string, string> ms_namesInNamespace = new Dictionary<string, string>();
        static public Dictionary<string, string> NamesInNamespace
        {
            get
            {
                if (!ms_namesInNamespace.ContainsKey("behaviac_EBTStatus"))
                {
                    ms_namesInNamespace["behaviac_EBTStatus"] = "behaviac::EBTStatus";
                }

                if (!ms_namesInNamespace.ContainsKey("System_Object"))
                {
                    ms_namesInNamespace["System_Object"] = "System::Object";
                }

                if (!ms_namesInNamespace.ContainsKey("Object"))
                {
                    ms_namesInNamespace["Object"] = "System::Object";
                }

                return ms_namesInNamespace;
            }
        }

        public static string GetMemberValueTypeName(Type type)
        {
            if (type == typeof(void))
            {
                return "void";
            }

            if (type == typeof(object))
            {
                return "System::Object";
            }

            foreach (Type key in Plugin.TypeHandlers.Keys)
            {
                if (key == type)
                {
                    return Plugin.GetNativeTypeName(key);
                }
            }

            foreach (AgentType at in Plugin.AgentTypes)
            {
                if (at.Type == type)
                {
                    return at.DisplayName;
                }
            }

            return GetNativeTypeName(type);
        }

        public static Type GetMemberValueType(string typeName)
        {
            if (typeName == "void")
            {
                return typeof(void);
            }

            if (typeName == "System::Object" || typeName == "Object" || typeName == "object")
            {
                return typeof(object);
            }

            foreach (Type key in Plugin.TypeHandlers.Keys)
            {
                if (Plugin.GetNativeTypeName(key) == typeName)
                {
                    return key;
                }
            }

            foreach (AgentType at in Plugin.AgentTypes)
            {
                if (at.DisplayName == typeName)
                {
                    return at.Type;
                }
            }

            return null;
        }

        public static IList<string> GetAllMemberValueTypeNames(bool hasVoid, bool ignoreStatic)
        {
            List<string> allTypeNames = new List<string>();

            if (hasVoid)
            {
                allTypeNames.Add("void");
            }

            foreach (Type key in Plugin.TypeHandlers.Keys)
            {
                if (key != typeof(System.Collections.IList) && !Plugin.IsArrayType(key) && key != typeof(XMLPluginBehaviac.System_Object))
                {
                    allTypeNames.Add(Plugin.GetNativeTypeName(key.Name, false, true));
                }
            }

            foreach (AgentType at in Plugin.AgentTypes)
            {
                if (!ignoreStatic || !at.IsStatic)
                {
                    allTypeNames.Add(at.DisplayName);
                }
            }

            return allTypeNames.AsReadOnly();
        }

        public static string GetNativeTypeName(Type type, bool bForDisplay = false)
        {
            if (type == null)
            {
                return string.Empty;
            }

            if (Plugin.IsArrayType(type))
            {
                Type itemType = type.GetGenericArguments()[0];
                string itemTypeStr = Plugin.GetNativeTypeName(itemType, bForDisplay);

                //if (!itemTypeStr.Contains("*") && Plugin.IsRefType(itemType))
                //{
                //    itemTypeStr += "*";
                //}

                return string.Format("vector<{0}>", itemTypeStr);
            }

            string typeStr = GetNativeTypeName(type.Name, false, bForDisplay);

            //if (!typeStr.Contains("*") && Plugin.IsRefType(type))
            //{
            //    typeStr += "*";
            //}

            return typeStr;
        }

        static Dictionary<string, string> ms_type_mapping = new Dictionary<string, string>()
        {
            {"Boolean"          , "bool"},
            {"System.Boolean"   , "bool"},
            {"Int32"            , "int"},
            {"System.Int32"     , "int"},
            {"UInt32"           , "uint"},
            {"System.UInt32"    , "uint"},
            {"Int16"            , "short"},
            {"System.Int16"     , "short"},
            {"UInt16"           , "ushort"},
            {"System.UInt16"    , "ushort"},
            {"Int8"             , "sbyte"},
            {"System.Int8"      , "sbyte"},
            {"SByte"            , "sbyte"},
            {"System.SByte"     , "sbyte"},
            {"UInt8"            , "ubyte"},
            {"System.UInt8"     , "ubyte"},
            {"Byte"             , "ubyte"},
            {"System.Byte"      , "ubyte"},
            {"Char"             , "char"},
            {"Int64"            , "long"},
            {"System.Int64"     , "long"},
            {"UInt64"           , "ulong"},
            {"System.UInt64"    , "ulong"},
            {"Single"           , "float"},
            {"System.Single"    , "float"},
            {"Double"           , "double"},
            {"System.Double"    , "double"},
            {"String"           , "string"},
            {"System.String"    , "string"},
            {"Void"             , "void"},
            {"System.Void"      , "void"},
            {"Behaviac.Designer.llong",  "llong"},
            {"Behaviac.Designer.ullong", "ullong"}
        };

        public static string GetNativeTypeName(string typeName, bool withNamespace = false, bool bForDisplay = false)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return string.Empty;
            }

            foreach (KeyValuePair<string, string> pair in ms_type_mapping)
            {
                if (pair.Key == typeName)
                {
                    if (bForDisplay)
                    {
                        return pair.Value;
                    }

                    return pair.Value;
                }
                else
                {
                    string refType = pair.Key + "&";

                    if (refType == typeName)
                    {
                        string ret = pair.Value;

                        if (bForDisplay)
                        {
                            ret = pair.Value;
                        }

                        return ret + "&";
                    }
                }
            }

            typeName = typeName.Replace("const char*", "cszstring");
            typeName = typeName.Replace("char*", "szstring");
            typeName = typeName.Replace("const ", "");
            typeName = typeName.Replace("unsigned long long", "ullong");
            typeName = typeName.Replace("signed long long", "llong");
            typeName = typeName.Replace("long long", "llong");
            typeName = typeName.Replace("unsigned ", "u");

            if (!typeName.Contains("signed char"))
            {
                typeName = typeName.Replace("signed ", "");
            }

            typeName = TypeManager.Instance.GetCurrentName(typeName);

            if (NamesInNamespace.ContainsKey(typeName))
            {
                string nativeTypeName = NamesInNamespace[typeName];

                //Type type = Plugin.GetType(typeName);
                //if (type != null && type.IsSubclassOf(typeof(Behaviac.Design.Agent)))
                //{
                //    nativeTypeName += "*";
                //}

                return nativeTypeName;
            }

            if (withNamespace)
            {
                return typeName;
            }

            string[] types = typeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return types[types.Length - 1];
        }

        public static Type GetTypeFromName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            switch (typeName)
            {
                case "bool":
                    return typeof(bool);

                case "int":
                    return typeof(int);

                case "uint":
                    return typeof(uint);

                case "short":
                    return typeof(short);

                case "ushort":
                    return typeof(ushort);

                case "char":
                    return typeof(char);

                case "sbyte":
                    return typeof(sbyte);

                case "ubyte":
                    return typeof(byte);

                case "byte":
                    return typeof(byte);

                case "long":
                    return typeof(long);

                case "ulong":
                    return typeof(ulong);

                case "float":
                    return typeof(float);

                case "double":
                    return typeof(double);

                case "string":
                    return typeof(string);
            }

            Type type = null;

            bool isVec = typeName.StartsWith("vector<");
            bool isList = false;

            if (!isVec)
            {
                isList = typeName.StartsWith("List<");
            }

            if (isVec || isList)
            {
                try
                {
                    if (isVec)
                    {
                        typeName = typeName.Substring(7, typeName.Length - 8);
                    }
                    else
                    {
                        typeName = typeName.Substring(5, typeName.Length - 6);
                    }

                    typeName = typeName.Replace("::", "_");

                    type = GetTypeFromName(typeName);

                    if (type != null)
                    {
                        type = typeof(List<>).MakeGenericType(type);

                        return type;
                    }
                }
                catch
                {
                }
            }

            if (type == null)
            {
                type = Plugin.GetType(typeName);
            }

            if (type == null && (typeName.Contains("::") || typeName.Contains(".")))
            {
                typeName = typeName.Replace("::", "_");
                typeName = typeName.Replace(".", "_");
                type = Plugin.GetType(typeName);
            }

            if (type == null)
            {
                typeName = "XMLPluginBehaviac." + typeName;
                type = Plugin.GetType(typeName);
            }

            return type;
        }

        public static string GetFullTypeName(string typeName)
        {
            Type type = GetTypeFromName(typeName);
            return (type != null) ? type.FullName : string.Empty;
        }

        public static bool IsIntergerNumberType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            switch (typeName)
            {
                case "int":
                case "uint":
                case "short":
                case "ushort":
                case "sbyte":
                case "ubyte":
                case "long":
                case "ulong":
                    return true;
            }

            return false;
        }

        public static bool IsIntergerNumberType(Type type)
        {
            return type == typeof(int) || type == typeof(uint) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(sbyte) || type == typeof(byte) ||
                   type == typeof(long) || type == typeof(ulong);
        }

        public static ValueTypes GetValueType(Type type)
        {
            if (IsBooleanType(type))
            {
                return ValueTypes.Bool;
            }

            if (IsIntergerType32(type))
            {
                return ValueTypes.Int;
            }

            if (IsFloatType(type))
            {
                return ValueTypes.Float;
            }

            if (IsStringType(type))
            {
                return ValueTypes.String;
            }

            return ValueTypes.All;
        }

        public static Type GetTypeFromValue(ValueTypes value)
        {
            switch (value)
            {
                case ValueTypes.Bool:
                    return typeof(bool);

                case ValueTypes.Int:
                    return typeof(int);

                case ValueTypes.Float:
                    return typeof(float);

                case ValueTypes.String:
                    return typeof(String);
            }

            return null;
        }

        public static bool IsBooleanType(Type type)
        {
            return type == typeof(bool);
        }

        public static bool IsIntergerType(Type type)
        {
            return type != null ? IsIntergerNumberType(GetNativeTypeName(type.Name)) : false;
        }

        public static bool IsIntergerType32(Type type)
        {
            return type == typeof(int);
        }

        public static bool IsFloatType(Type type)
        {
            return type == typeof(float) || type == typeof(double);
        }

        public static bool IsStringType(Type type)
        {
            return type == typeof(string);
        }

        public static bool IsCharType(Type type)
        {
            return type == typeof(char);
        }

        public static bool IsEnumType(Type type)
        {
            return type != null && type.IsEnum;
        }

        public static bool IsArrayType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsExportArray(object v)
        {
            bool bExportArray = false;

            if (v != null && Plugin.IsArrayType(v.GetType()))
            {
                System.Collections.IList listV = (System.Collections.IList)(v);

                if (listV.Count == 0)
                {
                    bExportArray = true;
                }
            }

            return bExportArray;
        }

        public static bool IsCustomClassType(Type type)
        {
            return type != null && !type.IsByRef && (type.IsClass || type.IsValueType) &&
                type != typeof(void) && type != typeof(llong) && type != typeof(ullong) &&
                !type.IsEnum && !type.IsPrimitive && !IsStringType(type) && !IsArrayType(type);
        }

        public static bool IsCompatibleType(ValueTypes valueType, Type filterType, Type typeToFilter, bool bArrayType)
        {
            Debug.Check(typeToFilter != null);

            if (typeToFilter != null)
            {
                if (filterType != null && (filterType.Name == "System_Object" && !Plugin.IsArrayType(typeToFilter)))
                {
                    return true;
                }

                //if (valueType == ValueTypes.All)
                //{
                //    return true;
                //}

                if (filterType == null)
                {
                    if ((valueType & ValueTypes.Int) == ValueTypes.Int && Plugin.IsIntergerType(typeToFilter) ||
                        (valueType & ValueTypes.Float) == ValueTypes.Float && Plugin.IsFloatType(typeToFilter) ||
                        (valueType & ValueTypes.Bool) == ValueTypes.Float && Plugin.IsBooleanType(typeToFilter) ||
                        (valueType & ValueTypes.String) == ValueTypes.Float && Plugin.IsStringType(typeToFilter)
                       )
                    {
                        return true;
                    }
                }

                if ((valueType & ValueTypes.RefType) == ValueTypes.RefType && Plugin.IsRefType(typeToFilter))
                {
                    if (filterType == null || Plugin.IsDerived(typeToFilter, filterType))
                    {
                        return true;
                    }
                }

                bool bCompatible = false;

                if (bArrayType && Plugin.IsArrayType(typeToFilter))
                {
                    //list of the same type
                    Type typeToFilterElement = typeToFilter.GetGenericArguments()[0];

                    if (Plugin.IsCompatibleType(valueType, typeToFilterElement, filterType, false))
                    {
                        bCompatible = true;
                    }
                }
                else if (filterType == typeToFilter)
                {
                    bCompatible = true;
                }
                else if (filterType != null)
                {
                    bCompatible = Plugin.IsAgentDerived(typeToFilter.Name, filterType.Name);
                }
                else
                {
                    bCompatible = (valueType == ValueTypes.All);
                }

                return bCompatible;
            }

            return false;
        }

        public static object CloneValue(object value)
        {
            object clone = value;

            if (value != null)
            {
                if (value is VariableDef)
                {
                    clone = new VariableDef((VariableDef)value);
                }
                else if (value is RightValueDef)
                {
                    clone = ((RightValueDef)value).Clone();
                }
                else if (value is ParInfo)
                {
                    clone = new ParInfo((ParInfo)value);
                }
                else
                {
                    Type type = value.GetType();

                    if (Plugin.IsArrayType(type))
                    {
                        clone = Plugin.CreateInstance(type);
                        System.Collections.IList listValue = (System.Collections.IList)value;
                        System.Collections.IList listClone = (System.Collections.IList)clone;

                        foreach (object item in listValue)
                        {
                            object cloneValue = CloneValue(item);
                            listClone.Add(cloneValue);
                        }
                    }
                    else if (Plugin.IsCustomClassType(type))
                    {
                        clone = Plugin.CreateInstance(type);

                        try
                        {
                            foreach (PropertyInfo property in type.GetProperties())
                            {
                                object cloneValue = property.GetValue(value, null);

                                if (property.PropertyType != type)
                                {
                                    cloneValue = CloneValue(cloneValue);
                                }

                                if (property.CanWrite)
                                {
                                    property.SetValue(clone, cloneValue, null);

                                }
                                else
                                {
                                    DesignerProperty[] attributes = (DesignerProperty[])property.GetCustomAttributes(typeof(DesignerProperty), false);

                                    if (attributes.Length > 0)
                                    {
                                        Debug.Check(attributes[0].HasFlags(DesignerProperty.DesignerFlags.ReadOnly));
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            // Primitive type
            return clone;
        }

        public static int NewNodeId(Node root)
        {
            if (root == null)
            {
                return -1;
            }

            int id = 0;

            while (null != GetObjectById(root, id))
            {
                ++id;
            }

            return id;
        }

        public static DefaultObject GetObjectById(Node root, int id)
        {
            if (root == null || root.Id == id)
            {
                return root;
            }

            foreach (Attachments.Attachment attach in root.Attachments)
            {
                if (attach.Id == id)
                {
                    return attach;
                }
            }

            if (!(root is ReferencedBehavior))
            {
                foreach (Node child in root.Children)
                {
                    DefaultObject obj = GetObjectById(child, id);

                    if (null != obj)
                    {
                        return obj;
                    }
                }

                foreach (Node child in root.FSMNodes)
                {
                    DefaultObject obj = GetObjectById(child, id);

                    if (null != obj)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        public static DefaultObject GetPreviousObjectById(Node root, int id, DefaultObject currentObj)
        {
            if (root == null || root == currentObj)
            {
                return null;
            }

            if (root.Id == id)
            {
                return root;
            }

            foreach (Attachments.Attachment attach in root.Attachments)
            {
                if (attach == currentObj)
                {
                    return null;
                }

                if (attach.Id == id)
                {
                    return attach;
                }
            }

            if (!(root is ReferencedBehavior))
            {
                foreach (Node child in root.Children)
                {
                    DefaultObject obj = GetPreviousObjectById(child, id, currentObj);

                    if (null != obj)
                    {
                        return obj;
                    }
                }

                foreach (Node child in root.FSMNodes)
                {
                    DefaultObject obj = GetPreviousObjectById(child, id, currentObj);

                    if (null != obj)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        public static Node GetPrefabNode(Node instanceNode)
        {
            if (instanceNode != null && !string.IsNullOrEmpty(instanceNode.PrefabName))
            {
                string fullpath = Behaviac.Design.FileManagers.FileManager.GetFullPath(instanceNode.PrefabName);

                if (File.Exists(fullpath))
                {
                    BehaviorNode behavior = Behaviac.Design.BehaviorManager.Instance.LoadBehavior(fullpath);

                    if (behavior != null)
                    {
                        DefaultObject obj = Plugin.GetObjectById((Node)behavior, instanceNode.PrefabNodeId);

                        if (obj != null && obj.GetType() == instanceNode.GetType())
                        {
                            return (Node)obj;
                        }
                    }
                }
            }

            return null;
        }

        public static Attachments.Attachment GetPrefabAttachment(Attachments.Attachment instanceAttachment)
        {
            if (instanceAttachment != null && instanceAttachment.Node != null && !string.IsNullOrEmpty(instanceAttachment.Node.PrefabName))
            {
                string fullpath = Behaviac.Design.FileManagers.FileManager.GetFullPath(instanceAttachment.Node.PrefabName);
                BehaviorNode behavior = Behaviac.Design.BehaviorManager.Instance.LoadBehavior(fullpath);

                if (behavior != null)
                {
                    DefaultObject obj = Plugin.GetObjectById((Node)behavior, instanceAttachment.PrefabAttachmentId);

                    if (obj != null && obj.GetType() == instanceAttachment.GetType())
                    {
                        return (Attachments.Attachment)obj;
                    }
                }
            }

            return null;
        }

        public static List<Node.ErrorCheck> GetErrorChecks(List<Node.ErrorCheck> errorChecks, ErrorCheckLevel errorLevel = ErrorCheckLevel.Error)
        {
            List<Node.ErrorCheck> errors = new List<Node.ErrorCheck>();

            foreach (Node.ErrorCheck error in errorChecks)
            {
                if (errorLevel == error.Level)
                {
                    errors.Add(error);
                }
            }

            return errors;
        }

        private static Node getNode(DefaultObject obj)
        {
            Node node = obj as Node;

            if (node == null)
            {
                Attachments.Attachment attach = obj as Attachments.Attachment;

                if (attach != null)
                {
                    node = attach.Node;
                }
            }

            return node;
        }

        private static void checkPar(DefaultObject obj, ParInfo par, ref List<Node.ErrorCheck> result)
        {
            Node node = getNode(obj);

            if (node != null && par != null)
            {
                Type type = obj.GetType();

                foreach (PropertyInfo property in type.GetProperties())
                {
                    try
                    {
                        object value = property.GetValue(obj, null);

                        if (value != null)
                        {
                            if (property.PropertyType == typeof(MethodDef))
                            {
                                MethodDef method = value as MethodDef;
                                Debug.Check(method != null);

                                if (method.CheckPar(par))
                                {
                                    result.Add(new Node.ErrorCheck(node, ErrorCheckLevel.Error, "Par as a parameter of the method."));
                                }
                            }
                            else if (property.PropertyType == typeof(VariableDef))
                            {
                                VariableDef var = value as VariableDef;
                                Debug.Check(var != null);

                                if (var.CheckPar(par))
                                {
                                    result.Add(new Node.ErrorCheck(node, ErrorCheckLevel.Error, "Par as a value."));
                                }
                            }
                            else if (property.PropertyType == typeof(RightValueDef))
                            {
                                RightValueDef rv = value as RightValueDef;
                                Debug.Check(rv != null);

                                if (rv.CheckPar(par))
                                {
                                    result.Add(new Node.ErrorCheck(node, ErrorCheckLevel.Error, "Par as a right value."));
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void CheckPar(Node node, ParInfo par, ref List<Node.ErrorCheck> result)
        {
            try
            {
                // self
                checkPar(node, par, ref result);

                // attachment
                foreach (Attachments.Attachment attach in node.Attachments)
                {
                    checkPar(attach, par, ref result);
                }

                // children
                foreach (BaseNode child in node.GetChildNodes())
                {
                    if (child is Node && !(child is ReferencedBehavior))
                    {
                        Node childNode = child as Node;
                        CheckPar(childNode, par, ref result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.LoadError, MessageBoxButtons.OK);
            }
        }

        public static bool CheckTwoTypes(Type type1, Type type2)
        {
            if (type1 == type2)
            {
                return true;
            }

            if (type1 == typeof(llong))
            {
                return type2 == typeof(long);
            }

            if (type2 == typeof(llong))
            {
                return type1 == typeof(long);
            }

            if (type1 == typeof(ullong))
            {
                return type2 == typeof(ulong);
            }

            if (type2 == typeof(ullong))
            {
                return type1 == typeof(ulong);
            }

            return false;
        }

        public delegate void SetValue(object obj);

        // key : data type, value : handler type
        private static Dictionary<Type, Type> _typeHandlers = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> TypeHandlers
        {
            get
            {
                return _typeHandlers;
            }
        }

        public static object CreateInstance(Type type)
        {
            if (_typeHandlers.ContainsKey(type))
            {
                Type typeHandler = _typeHandlers[type];

                MethodInfo createMethod = typeHandler.GetMethod("Create");
                Debug.Check(createMethod != null);

                return createMethod.Invoke(null, new object[0]);
            }
            else if (type.IsEnum)
            {
                Array values = Enum.GetValues(type);

                foreach (object enumVal in values)
                {
                    return enumVal;
                }

                return null;
            }
            else if (IsCustomClassType(type) || IsArrayType(type))
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                string message = string.Format("Create for {0} is not registered!", type.Name);
                throw new Exception(message);
            }
        }

        public static bool InvokeTypeParser(List<Nodes.Node.ErrorCheck> result, Type type, string parStr, SetValue setter, DefaultObject node, string paramName = null)
        {
            if (type == null)
            {
                return false;
            }

            if (_typeHandlers.ContainsKey(type))
            {
                Type typeHandler = _typeHandlers[type];

                MethodInfo parserMethod = typeHandler.GetMethod("Parse");
                Debug.Check(parserMethod != null);

                if (parserMethod != null)
                {
                    if (string.IsNullOrEmpty(parStr) && type.IsPrimitive && type == typeof(string))
                    {
                        return false;
                    }

                    object[] pars = { node, paramName, parStr, setter };

                    return (bool)parserMethod.Invoke(null, pars);
                }
            }
            else if (type.IsEnum)
            {
                Array values = Enum.GetValues(type);

                foreach (object enumVal in values)
                {
                    string enumValueName = Enum.GetName(type, enumVal);

                    if (enumValueName == parStr)
                    {
                        setter(enumVal);
                        return true;
                    }
                }
            }
            else if (Plugin.IsArrayType(type))
            {
                object obj = DesignerArray.ParseStringValue(result, type, parStr, node);
                setter(obj);
                return true;
            }
            else if (Plugin.IsCustomClassType(type))
            {
                object obj = Behaviac.Design.Attributes.DesignerStruct.ParseStringValue(result, type, paramName, parStr, node);
                setter(obj);
                return true;
            }
            else
            {
                string message = string.Format("parser for {0} is not registered!", type.Name);
                throw new Exception(message);
            }

            return false;
        }

        public static Behaviac.Design.Attributes.DesignerProperty InvokeTypeCreateDesignerProperty(string category, string name, Type type, float rangeMin, float rangeMax)
        {
            if (_typeHandlers.ContainsKey(type))
            {
                Type typeHandler = _typeHandlers[type];

                MethodInfo method = typeHandler.GetMethod("CreateDesignerProperty");
                Debug.Check(method != null);

                if (method != null)
                {
                    object[] pars = { category, name, type, rangeMin, rangeMax };

                    return (Behaviac.Design.Attributes.DesignerProperty)method.Invoke(null, pars);
                }
            }
            else if (Plugin.IsArrayType(type))
            {
                Type itemType = type.GetGenericArguments()[0];

                if (Plugin.IsBooleanType(itemType))
                {
                    return new DesignerArrayBoolean(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
                }

                else if (Plugin.IsIntergerType(itemType))
                {
                    return new DesignerArrayInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
                }

                else if (Plugin.IsFloatType(itemType))
                {
                    return new DesignerArrayFloat(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
                }

                else if (Plugin.IsStringType(itemType) || Plugin.IsCharType(itemType))
                {
                    return new DesignerArrayString(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
                }

                else if (Plugin.IsEnumType(itemType))
                {
                    return new DesignerArrayEnum(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
                }

                else if (Plugin.IsCustomClassType(itemType))
                {
                    return new DesignerArrayStruct(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
                }
            }
            else
            {
                if (type == typeof(object) || type.IsSubclassOf(typeof(Agent)))
                {
                    return new DesignerArrayInteger(name, "", category, DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport);
                }
            }

            string message = string.Format("CreateDesignerProperty for {0} is not registered!", type.Name);
            throw new Exception(message);
        }

        public static Type InvokeEditorType(Type type)
        {
            if (_typeHandlers.ContainsKey(type))
            {
                Type typeHandler = _typeHandlers[type];

                MethodInfo method = typeHandler.GetMethod("GetEditorType");
                Debug.Check(method != null);

                if (method != null)
                {
                    object[] pars = { };

                    return (Type)method.Invoke(null, pars);
                }

                return null;
            }
            else if (type.IsEnum)
            {
                return typeof(Behaviac.Design.Attributes.DesignerEnumEditor);
            }
            else if (IsCustomClassType(type) || IsArrayType(type))
            {
                return typeof(Behaviac.Design.Attributes.DesignerCompositeEditor);
            }
            else
            {
                string message = string.Format("GetEditorType for {0} is not registered!", type.Name);
                throw new Exception(message);
            }
        }

        public static object DefaultValue(Type type, string defaultValue = "")
        {
            if (type == null)
            {
                return null;
            }

            if (_typeHandlers.ContainsKey(type))
            {
                Type typeHandler = _typeHandlers[type];

                MethodInfo method = typeHandler.GetMethod("DefaultValue");
                Debug.Check(method != null);

                if (method != null)
                {
                    object[] pars = { defaultValue };

                    return method.Invoke(null, pars);
                }
            }
            else if (type.IsEnum)
            {
                Array values = Enum.GetValues(type);

                foreach (object enumVal in values)
                {
                    string enumValueName = Enum.GetName(type, enumVal);

                    if (enumValueName == defaultValue)
                    {
                        return enumVal;
                    }
                }

                return null;

            }
            else if (IsArrayType(type))
            {
                return Activator.CreateInstance(type);

            }
            else
            {
                if (type == typeof(Agent) || type.IsSubclassOf(typeof(Agent)))
                {
                    return Activator.CreateInstance(type);
                }

                if (Plugin.IsArrayType(type) || type == typeof(System.Collections.IList))
                {
                    return null;
                }

                if (type == typeof(object) || Plugin.IsDerived(type, typeof(Agent)) || type.Name == "AgentType")
                {
                    return null;
                }

                string message = string.Format("DefaultValue for {0} is not registered!", type.Name);
                throw new Exception(message);
            }

            return null;
        }

        public static void UnRegisterTypeHandlers()
        {
            _typeHandlers.Clear();

            Debug.Check(_DesignerBaseDll != null);

            if (_DesignerBaseDll != null)
            {
                RegisterTypeHandlers(_DesignerBaseDll);
            }
        }

        public static void RegisterTypeHandlers(Assembly a)
        {
            Type[] types = a.GetTypes();

            foreach (Type typeHandler in types)
            {
                Attribute[] attributes = (Attribute[])typeHandler.GetCustomAttributes(typeof(Behaviac.Design.TypeHandlerAttribute), false);

                if (attributes.Length > 0)
                {
                    TypeHandlerAttribute parserTypeAttr = attributes[0] as TypeHandlerAttribute;

                    MethodInfo parserMethod = typeHandler.GetMethod("Parse");
                    Debug.Check(parserMethod != null);

                    MethodInfo createMethod = typeHandler.GetMethod("CreateDesignerProperty");
                    Debug.Check(createMethod != null);

                    MethodInfo defaultMethod = typeHandler.GetMethod("DefaultValue");
                    Debug.Check(defaultMethod != null);

                    _typeHandlers[parserTypeAttr.Type] = typeHandler;
                }
            }
        }

        protected static void RegisterTypeHandlers()
        {
            Assembly a = Assembly.GetCallingAssembly();

            if (a != _DesignerBaseDll)
            {
                Plugin.RegisterTypeHandlers(a);
            }
        }

        private static Dictionary<string, string> ms_typeNameMap = new Dictionary<string, string>();


        /// <summary>
        /// register the cs type name for the customized cpp type name
        /// you need to provide your cs type in the name space 'Behaviac.Design'
        /// </summary>
        /// <param name="cppTypeName"></param>
        /// <param name="csTypeName"></param>
        public static void RegisterTypeName(string cppTypeName, string csTypeName)
        {
            ms_typeNameMap[cppTypeName] = csTypeName;
        }

        public static string GetTypeName(string cppTypeName)
        {
            if (ms_typeNameMap.ContainsKey(cppTypeName))
            {
                return ms_typeNameMap[cppTypeName];
            }

            return cppTypeName;
        }

        public static bool IsRegisteredTypeName(string csTypeName)
        {
            if (ms_typeNameMap.ContainsValue(csTypeName))
            {
                return true;
            }

            return false;
        }

        protected static List<NodeGroup> _nodeGroups = new List<NodeGroup>();
        protected static List<NodeGroup> _frequentlyUsedNodeGroups = new List<NodeGroup>();

        public static void InitNodeGroups()
        {
            _nodeGroups.Clear();

            NodeGroup group = new NodeGroup(Resources.Attachments, NodeIcon.FolderClosed, null);
            _nodeGroups.Add(group);

            group = new NodeGroup(Resources.Conditions, NodeIcon.FolderClosed, null);
            _nodeGroups.Add(group);

            new NodeGroup(Resources.Leaf, NodeIcon.FolderClosed, group);

            group = new NodeGroup(Resources.Actions, NodeIcon.FolderClosed, null);
            _nodeGroups.Add(group);

            group = new NodeGroup(Resources.Composites, NodeIcon.FolderClosed, null);
            _nodeGroups.Add(group);

            new NodeGroup(Resources.Selectors, NodeIcon.FolderClosed, group);
            new NodeGroup(Resources.Sequences, NodeIcon.FolderClosed, group);
            new NodeGroup(Resources.EventHandling, NodeIcon.FolderClosed, group);

            group = new NodeGroup(Resources.Decorators, NodeIcon.FolderClosed, null);
            _nodeGroups.Add(group);
        }

        /// <summary>
        /// Holds a list of any root node group which will be automatically added to the node explorer.
        /// </summary>
        public static IList<NodeGroup> NodeGroups
        {
            get
            {
                return _nodeGroups.AsReadOnly();
            }
        }

        public static IList<NodeGroup> FrequentlyUsedNodeGroups
        {
            get
            {
                return _frequentlyUsedNodeGroups.AsReadOnly();
            }
        }

        public static bool IsQueryFiltered
        {
            get
            {
                //only filter query not in vc
                //return !System.Diagnostics.Debugger.IsAttached && FilterNodes.Contains("PluginBehaviac.Nodes.Query");

                return FilterNodes.Contains("PluginBehaviac.Nodes.Query");
            }
        }

        private static void initFrequentlyUsedNodeGroup()
        {
            _frequentlyUsedNodeGroups.Clear();

            NodeGroup group = new NodeGroup(Resources.Attachments, NodeIcon.FolderClosed, null);
            _frequentlyUsedNodeGroups.Add(group);

            group = new NodeGroup(Resources.Conditions, NodeIcon.FolderClosed, null);
            _frequentlyUsedNodeGroups.Add(group);

            group = new NodeGroup(Resources.Actions, NodeIcon.FolderClosed, null);
            _frequentlyUsedNodeGroups.Add(group);

            group = new NodeGroup(Resources.Composites, NodeIcon.FolderClosed, null);
            _frequentlyUsedNodeGroups.Add(group);

            group = new NodeGroup(Resources.Decorators, NodeIcon.FolderClosed, null);
            _frequentlyUsedNodeGroups.Add(group);
        }

        private static void copyNodeItems(NodeGroup source, NodeGroup target)
        {
            foreach (NodeItem nodeItem in source.Items)
            {
                if (nodeItem.Type != null && Plugin.FrequentlyUsedNodes.Contains(nodeItem.Type.FullName))
                {
                    target.Items.Add(nodeItem);
                }
            }

            foreach (NodeGroup nodeGroup in source.Children)
            {
                copyNodeItems(nodeGroup, target);
            }
        }

        public static void SetFrequentlyUsedNodeGroups()
        {
            initFrequentlyUsedNodeGroup();

            foreach (NodeGroup nodeGroup in _nodeGroups)
            {
                NodeGroup group = null;

                foreach (NodeGroup fuNodeGroup in _frequentlyUsedNodeGroups)
                {
                    if (fuNodeGroup.Name == nodeGroup.Name)
                    {
                        group = fuNodeGroup;
                        break;
                    }
                }

                if (group == null)
                {
                    group = new NodeGroup(nodeGroup.Name, nodeGroup.Icon, null);
                    _frequentlyUsedNodeGroups.Add(group);
                }

                copyNodeItems(nodeGroup, group);
            }

            // clear the empty group
            for (int g = 0; g < _frequentlyUsedNodeGroups.Count; g++)
            {
                if (_frequentlyUsedNodeGroups[g].Children.Count == 0 &&
                    _frequentlyUsedNodeGroups[g].Items.Count == 0)
                {
                    _frequentlyUsedNodeGroups.RemoveAt(g);
                    g--;
                }
            }
        }

        private static List<Image> _nodeImages = new List<Image>();
        public static List<Image> ImagesList
        {
            get
            {
                return _nodeImages;
            }
        }

        public static void RegisterNodeDesc(Assembly a, int iconCount = 0)
        {
            Type[] types = a.GetTypes();

            foreach (Type type in types)
            {
                // to skip it if confiured in the filter nodes
                if (FilterNodes.Contains(type.FullName))
                {
                    continue;
                }

                Attribute[] attributes = (Attribute[])type.GetCustomAttributes(typeof(Behaviac.Design.NodeDescAttribute), false);

                if (attributes.Length > 0)
                {
                    NodeDescAttribute nodeDescAttr = attributes[0] as NodeDescAttribute;
                    NodeGroup curGroup = null;
                    IList<NodeGroup> curNodeGroups = _nodeGroups;
                    string[] nameTree = nodeDescAttr.Group.Split(new Char[] { ':' });

                    for (int i = 0; i < nameTree.Length; ++i)
                    {
                        string name = Resources.ResourceManager.GetString(nameTree[i], Resources.Culture);

                        if (string.IsNullOrEmpty(name))
                        {
                            name = nameTree[i];
                        }

                        NodeGroup group = GetNodeGroup(curNodeGroups, name);

                        if (group == null)
                        {
                            group = new NodeGroup(name, NodeIcon.FolderClosed, curGroup);

                            if (curGroup == null)
                            {
                                curNodeGroups.Add(group);
                            }
                        }

                        curGroup = group;
                        curNodeGroups = group.Children;
                    }

                    if (curGroup != null && !curGroup.ContainType(type))
                    {
                        NodeIcon icon = nodeDescAttr.Icon;
                        Image image = GetResourceImage(nodeDescAttr.ImageName);

                        if (image != null)
                        {
                            _nodeImages.Add(image);
                            icon = (NodeIcon)(iconCount + _nodeImages.Count - 1);
                        }

                        curGroup.Items.Add(new NodeItem(type, icon));
                    }
                }
            }
        }

        //protected static void RegisterNodeDesc()
        //{
        //    Assembly a = Assembly.GetCallingAssembly();
        //    if (a != _DesignerBaseDll)
        //    {
        //        RegisterNodeDesc(a);
        //    }
        //}

        private static NodeGroup GetNodeGroup(IList<NodeGroup> nodeGroups, string groupName)
        {
            foreach (NodeGroup group in nodeGroups)
            {
                if (group.Name == groupName)
                {
                    return group;
                }
            }

            return null;
        }

        protected List<FileManagerInfo> _fileManagers = new List<FileManagerInfo>();

        /// <summary>
        /// Holds a listof file managers which will be automatically registered.
        /// </summary>
        public IList<FileManagerInfo> FileManagers
        {
            get
            {
                return _fileManagers.AsReadOnly();
            }
        }

        protected static List<ExporterInfo> _exporters = new List<ExporterInfo>();

        /// <summary>
        /// Holds a list of exporters which will be automatically registered.
        /// </summary>
        public static List<ExporterInfo> Exporters
        {
            get
            {
                return _exporters;
            }
        }

        public static int GetExporterIndex(string format, string desc = null)
        {
            for (int i = 0; i < Plugin.Exporters.Count; ++i)
            {
                if (Plugin.Exporters[i].ID == format || Plugin.Exporters[i].Description == desc)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Adds the resource manager to the list of available resource managers.
        /// </summary>
        /// <returns>List containing local resource manager.</returns>
        private static List<ResourceManager> AddLocalResources()
        {
            List<ResourceManager> list = new List<ResourceManager>();
            list.Add(Resources.ResourceManager);
            return list;
        }

        /// <summary>
        /// This list must contain any resource manager which is avilable.
        /// </summary>
        private static List<ResourceManager> __resources = AddLocalResources();

        /// <summary>
        /// Adds a resource manager to the list of all available resource managers.
        /// </summary>
        /// <param name="manager">The manager which will be added.</param>
        public static void AddResourceManager(ResourceManager manager)
        {
            if (!__resources.Contains(manager))
            {
                __resources.Add(manager);
            }
        }

        /// <summary>
        /// Retrieves a string from all available resource managers.
        /// </summary>
        /// <param name="name">The string's name we want to get.</param>
        /// <returns>Returns name if resource could not be found.</returns>
        public static string GetResourceString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            for (int i = __resources.Count - 1; i >= 0; --i)
            {
                try
                {
                    string val = __resources[i].GetString(name, Resources.Culture);

                    if (val != null)
                    {
                        return val;
                    }

                }
                catch (Exception)
                {
                }
            }

            return name;
        }

        public static Image GetResourceImage(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            for (int i = __resources.Count - 1; i >= 0; --i)
            {
                try
                {
                    object obj = __resources[i].GetObject(name, Resources.Culture);

                    if (obj != null && obj is Image)
                    {
                        return (Image)obj;
                    }

                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        public static void GetObjectsBySelfPropertyMethod(Nodes.Node root, DefaultObject obj, string propertyName, bool matchCase, bool matchWholeWord, ref List<ObjectPair> objects)
        {
            if (root == null || string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (!ContainObjectPair(objects, root, obj))
            {
                bool found = false;

                // search from its members
                Type type = obj.GetType();

                foreach (System.Reflection.PropertyInfo property in type.GetProperties())
                {
                    Attribute[] attributes = (Attribute[])property.GetCustomAttributes(typeof(Behaviac.Design.Attributes.DesignerProperty), false);

                    if (attributes == null || attributes.Length == 0)
                    {
                        continue;
                    }

                    try
                    {
                        object value = property.GetValue(obj, null);

                        if (value != null)
                        {
                            if (property.PropertyType == typeof(MethodDef))
                            {
                                MethodDef method = value as MethodDef;
                                Debug.Check(method != null);

                                if (CompareTwoTypes(method.Name, propertyName, matchCase, matchWholeWord) ||
                                    CompareTwoTypes(method.DisplayName, propertyName, matchCase, matchWholeWord) ||
                                    CompareTwoTypes(method.GetDisplayValue(), propertyName, matchCase, matchWholeWord) ||
                                    CompareTwoTypes(method.GetExportValue(), propertyName, matchCase, matchWholeWord))
                                {
                                    found = true;
                                    break;
                                }

                            }
                            else if (property.PropertyType == typeof(VariableDef))
                            {
                                VariableDef var = value as VariableDef;
                                Debug.Check(var != null);

                                if (CompareTwoTypes(var.GetDisplayValue(), propertyName, matchCase, matchWholeWord) ||
                                    CompareTwoTypes(var.GetExportValue(), propertyName, matchCase, matchWholeWord))
                                {
                                    found = true;
                                    break;
                                }

                            }
                            else if (property.PropertyType == typeof(RightValueDef))
                            {
                                RightValueDef rv = value as RightValueDef;
                                Debug.Check(rv != null);

                                if (CompareTwoTypes(rv.GetDisplayValue(), propertyName, matchCase, matchWholeWord) ||
                                    CompareTwoTypes(rv.GetExportValue(), propertyName, matchCase, matchWholeWord))
                                {
                                    found = true;
                                    break;
                                }

                            }
                            else if (CompareTwoTypes(value.ToString(), propertyName, matchCase, matchWholeWord))
                            {
                                found = true;
                                break;
                            }
                        }

                    }
                    catch (Exception)
                    {
                    }
                }

                // search from its pars
                if (!found)
                {
                    if (obj is Behavior)
                    {
                        foreach (ParInfo par in((Behavior)obj).LocalVars)
                        {
                            if (CompareTwoTypes(par.Name, propertyName, matchCase, matchWholeWord))
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (found)
                {
                    objects.Add(new ObjectPair(root, obj));
                }
            }
        }

        public static bool ContainObjectPair(List<ObjectPair> objects, Nodes.Node root, DefaultObject obj)
        {
            foreach (ObjectPair objPair in objects)
            {
                if (objPair.Root == root && objPair.Obj == obj)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CompareTwoObjectLists(List<ObjectPair> listA, List<ObjectPair> listB)
        {
            Debug.Check(listA != null && listB != null);

            if (listA != null && listB != null)
            {
                if (listA.Count != listB.Count)
                {
                    return false;
                }

                foreach (ObjectPair obj in listB)
                {
                    if (!ContainObjectPair(listA, obj.Root, obj.Obj))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool CompareTwoTypes(string strA, string strB, bool matchCase, bool matchWholeWord)
        {
            if (matchWholeWord)
            {
                if (matchCase)
                {
                    if (strA == strB || Plugin.GetResourceString(strA) == strB)
                    {
                        return true;
                    }
                }
                else
                {
                    strB = strB.ToLower();

                    if (strA.ToLower() == strB)
                    {
                        return true;
                    }

                    strA = Plugin.GetResourceString(strA);

                    if (strA.ToLower() == strB)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (matchCase)
                {
                    if (strA.Contains(strB) || Plugin.GetResourceString(strA).Contains(strB))
                    {
                        return true;
                    }
                }
                else
                {
                    strB = strB.ToLower();

                    if (strA.ToLower().Contains(strB))
                    {
                        return true;
                    }

                    strA = Plugin.GetResourceString(strA);

                    if (strA.ToLower().Contains(strB))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
