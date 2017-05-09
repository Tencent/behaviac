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

//#define QUERY_EANBLED

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;
using Behaviac.Design.Properties;
using Behaviac.Design.Data;

namespace Behaviac.Design.Nodes
{
    public delegate void AgentTypeEventDelegate(AgentType agentType);
    public delegate void WasModifiedEventDelegate(BehaviorNode root, Node node);
    public delegate void WasSavedEventDelegate(BehaviorNode root);
    public delegate void WasRenamedEventDelegate(BehaviorNode root);

    public interface BehaviorNode : ICloneable
    {
        bool HasNoError
        {
            get;
            set;
        }

        FileManagers.FileManager FileManager
        {
            get;
            set;
        }
        string Folder
        {
            get;
            set;
        }
        string Filename
        {
            get;
            set;
        }
        string RelativePath
        {
            get;
        }
        bool IsPrefab
        {
            get;
            set;
        }
        int Version
        {
            get;
            set;
        }

        void Restore(BehaviorNode source);

        int InitialStateId
        {
            get;
        }

        AgentType AgentType
        {
            get;
            set;
        }
        int ModificationID
        {
            get;
        }
        bool IsModified
        {
            get;
        }
        void TriggerWasModified(Node node);
        void TriggerWasSaved();
        void TriggerWasRenamed();

        Node.Connector GenericChildren
        {
            get;
        }

        string MakeAbsolute(string filename);
        string MakeRelative(string filename);
        string GetPathLabel(string behaviorFolder);

        void PostLoadPars();

        void PreSave();
        void PostSave();

        void PreExport();
        void PostExport();

        event AgentTypeEventDelegate AgentTypeChanged;
        event WasModifiedEventDelegate WasModified;
        event WasSavedEventDelegate WasSaved;
        event WasRenamedEventDelegate WasRenamed;
    }

    /// <summary>
    /// This node represents the behaviour tree's root node.
    /// </summary>
    public class Behavior : Node, BehaviorNode
    {
        private bool hasNoError = false;
        public bool HasNoError
        {
            get
            {
                return hasNoError;
            }
            set
            {
                hasNoError = value;
            }
        }

        public override string ExportClass
        {
            get
            {
                return "Behavior";
            }
        }

        public override bool AlwaysExpanded()
        {
            return true;
        }

        public override bool CanBeDragged()
        {
            return false;
        }

        public override bool IsFSM
        {
            get
            {
                return this.FSMNodes.Count > 0;
            }
        }

        public int InitialStateId
        {
            get
            {
                int initialid = -1;

                foreach (Attachment attach in this.Attachments)
                {
                    if (attach.IsStartCondition && attach.TargetFSMNodeId > -1)
                    {
                        initialid = attach.TargetFSMNodeId;
                        break;
                    }
                }

                return initialid;
            }
        }

        public override bool CanBeAttached
        {
            get
            {
                return (this.Children.Count == 1 && this.Children[0] is Task);
            }
        }

        public void Restore(BehaviorNode source)
        {
            Debug.Check(source is Nodes.Node);

            Behavior sourceBehavior = (Behavior)source;
            Behavior targetBehavior = (Behavior)this;

            // Restore the agent type.
            targetBehavior.AgentType = sourceBehavior.AgentType;

            // Restore the child connector.
            targetBehavior._genericChildren = sourceBehavior._genericChildren;

            Node sourceNode = (Node)source;
            Node targetNode = (Node)this;

            // Restore the hierarchy.
            targetNode.ReplaceChildren(sourceNode);

            sourceBehavior.CloneProperties(this);
        }

        public override object Clone()
        {
            // Clone the hierarchy.
            Nodes.BehaviorNode behavior = (Nodes.BehaviorNode)this.CloneBranch();

            // Set the agent type.
            behavior.AgentType = this.AgentType;

            // Clone the file manager.
            if (this.FileManager != null)
            {
                behavior.FileManager = (FileManagers.FileManager)this.FileManager.Clone();
                behavior.FileManager.Behavior = behavior;

            }
            else
            {
                behavior.Filename = this.Filename;
            }

            return behavior;
        }

        public override bool AcceptsAttachment(DefaultObject obj)
        {
            Type type = (obj != null) ? obj.GetType() : null;
            return type != null && ((this.IsFSM && Plugin.IsClassDerived(type, typeof(Behaviac.Design.Attachments.AttachAction))) ||  //if fsm, only accept effectors
                                    (!Plugin.IsClassDerived(type, typeof(Behaviac.Design.Attachments.AttachAction)) &&
                                     !Plugin.IsClassDerived(type, typeof(Behaviac.Design.Attachments.Event)) &&
                                     !Plugin.IsClassDerived(type, typeof(Behaviac.Design.Nodes.Behavior))));
        }

        private bool _isVisiting = false;

        private string _folder = "";
        public string Folder
        {
            get
            {
                return _folder;
            }
            set
            {
                _folder = value;
            }
        }

        private bool _isPrefab = false;
        public bool IsPrefab
        {
            get
            {
                return _isPrefab;
            }
            set
            {
                _isPrefab = value;
            }
        }

        protected Connector _genericChildren;
        public Connector GenericChildren
        {
            get
            {
                return _genericChildren;
            }
        }

        //please update BehaviorTree.SupportedVersion when you update the NewVersion
        public static int NewVersion = 5;

        private int _version = NewVersion;
        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                //if (value > _version)
                {
                    _version = value;
                }
            }
        }

        protected FileManagers.FileManager _fileManager = null;
        public FileManagers.FileManager FileManager
        {
            get
            {
                return _fileManager;
            }
            set
            {
                _fileManager = value;
            }
        }

        /// <summary>
        /// The filename of the behaviour.
        /// </summary>
        //[DesignerString("BehaviorFilename", "BehaviorFilenameDesc", "CategoryBasic", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.ReadOnly|DesignerProperty.DesignerFlags.NoExport|DesignerProperty.DesignerFlags.NoSave)]
        private string _filename = string.Empty;
        public string Filename
        {
            get
            {
                return (_fileManager == null) ? _filename : _fileManager.Filename;
            }

            set
            {
                _filename = value;

                if (_fileManager != null)
                {
                    _fileManager.Filename = _filename;
                }
            }
        }

        private FrameStatePool.PlanningProcess _planningProcess = null;
        public FrameStatePool.PlanningProcess PlanningProcess
        {
            get
            {
                return this._planningProcess;
            }
            set
            {
                this._planningProcess = value;
            }
        }

        public static readonly int kPlanIsCollapseFailedBranch = 2;
        private int _planIsCollapseFailedBranch = 0;
        public int PlanIsCollapseFailedBranch
        {
            get
            {
                return this._planIsCollapseFailedBranch;
            }
            set
            {
                this._planIsCollapseFailedBranch = value;
            }
        }

#if QUERY_EANBLED
        private string _domains = "";
        [DesignerString("BehaviorDomains", "BehaviorDomainsDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.QueryRelated)]
        public string Domains
        {
            get
            {
                return _domains;
            }
            set
            {
                _domains = value;
            }
        }

        public class DescriptorRef
        {
            private VariableDef _descriptor;
            [DesignerPropertyEnum("Descriptor", "DescriptorDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.Self | DesignerPropertyEnum.AllowStyles.Instance, "", "Reference")]
            public VariableDef Descriptor
            {
                get
                {
                    return _descriptor;
                }
                set
                {
                    _descriptor = value;
                }
            }

            private VariableDef _reference;

            [DesignerPropertyEnum("Reference", "ReferenceDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.Const, "Descriptor", "")]
            public VariableDef Reference
            {
                get
                {
                    return _reference;
                }
                set
                {
                    _reference = value;
                }
            }
        }

        private List<DescriptorRef> _descriptorRefs = new List<DescriptorRef>();

        [DesignerArrayStruct("DescriptorRefs", "DescriptorRefsDesc", "Query", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoSave | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.QueryRelated)]
        public List<DescriptorRef> DescriptorRefs
        {
            get
            {
                return _descriptorRefs;
            }
            set
            {
                this._descriptorRefs = value;
            }
        }
#endif//#if QUERY_EANBLED

        private AgentType _agentType;
        public event AgentTypeEventDelegate AgentTypeChanged;

        /// <summary>
        /// The AgentType of the behaviour.
        /// </summary>
        [DesignerTypeEnum("BehaviorAgentType", "BehaviorAgentTypeDesc", "CategoryBasic", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoExport)]
        public AgentType AgentType
        {
            get
            {
                return _agentType;
            }
            set
            {
                if (this._agentType != value)
                {
                    bool bEmpty = (this._agentType == null);
                    this._agentType = value;

                    if (!bEmpty && this.Children.Count > 0 && AgentTypeChanged != null)
                    {
                        AgentTypeChanged(value);
                    }
                }
            }
        }

        //used for the referenced behavior
        private Node _parentNode;
        public Node ParentNode
        {
            get
            {
                return this._parentNode;
            }
            set
            {
                this._parentNode = value;
            }
        }

        public Behavior GetTopBehavior()
        {
            Behavior b = this;

            while (b.ParentNode != null)
            {
                b = b.ParentNode.Behavior as Behavior;
                Debug.Check(b != null);
            }

            return b;
        }

        public void PostLoadPars()
        {
            if (this.AgentType != null)
            {
                this.AgentType.ResetPars(this.LocalVars);
            }
        }

        public override void PostLoad(BehaviorNode behavior)
        {
            if (this.LocalVars != null && this.Children.Count > 0)
            {
                BaseNode child = this.Children[0];
                if (child is Task)
                {
                    Task task = child as Task;
                    List<ParInfo> pars = new List<ParInfo>();

                    task.CollectTaskPars(ref pars);

                    foreach (ParInfo par in pars)
                    {
                        if (this.LocalVars.Find((p) => p.Name == par.Name) == null)
                        {
                            this.LocalVars.Add(par);
                        }
                    }
                }
            }

            base.PostLoad(behavior);
        }

        public void PreSave()
        {
            PreExport();

            List<Node.ErrorCheck> result = new List<Node.ErrorCheck>();
            this.CheckForErrors(this, result);

            if (Plugin.GetErrorChecks(result).Count > 0)
            {
                this.HasNoError = false;
            }
            else
            {
                this.HasNoError = true;
            }
        }

        public void PostSave()
        {
            PostExport();
        }

        public void PreExport()
        {
            PostLoadPars();
        }

        public void PostExport()
        {
            //if (this.AgentType != null)
            //    this.AgentType.ClearPars();
        }

        public Behavior(string label, bool createConnector = true)
        : base(label, "BehaviorDesc")
        {
            if (createConnector)
            {
                _genericChildren = new ConnectorSingle(_children, string.Empty, Connector.kGeneric);
            }
        }

        public Behavior()
        : this(string.Empty)
        {
        }

        public Behavior(Behavior other)
        : this(string.Empty)
        {
            // Clone the hierarchy.
            //Nodes.BehaviorNode behavior = (Nodes.BehaviorNode)this.CloneBranch();

            // Set the agent type.
            this.AgentType = other.AgentType;

            // Clone the file manager.
            if (other.FileManager != null)
            {
                this.FileManager = (FileManagers.FileManager)other.FileManager.Clone();
                this.FileManager.Behavior = this;

            }
            else
            {
                this.Filename = other.Filename;
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(100, 120, 80));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.BehaviorUIPolicy();
        }

        private static string _behaviorPath = "";
        public static string BehaviorPath
        {
            get
            {
                return _behaviorPath;
            }
            set
            {
                _behaviorPath = value;
            }
        }

        /// <summary>
        /// Makes a filename relative to the filename of this behaviour. Used for referenced behaviours.
        /// </summary>
        /// <param name="filename">The filename which will become relative to this behaviour.</param>
        /// <returns>Returns the relative filename of the filename parameter.</returns>
        public string MakeRelative(string filename)
        {
            //return FileManagers.FileManager.MakeRelative(Path.GetDirectoryName(_fileManager.Filename), filename);
            return FileManagers.FileManager.MakeRelative(_behaviorPath, filename);
        }

        public string RelativePath
        {
            get
            {
                return string.IsNullOrEmpty(Filename) ? string.Empty : MakeRelative(Filename);
            }
        }

        /// <summary>
        /// Makes a filename absolute which is relative to the filename of this behaviour. Used for referenced behaviours.
        /// </summary>
        /// <param name="filename">The filename which is relative and will become absolute.</param>
        /// <returns>Returns the sbolute filename of the filename parameter.</returns>
        public string MakeAbsolute(string filename)
        {
            //return FileManagers.FileManager.MakeAbsolute(Path.GetDirectoryName(_fileManager.Filename), filename);
            return FileManagers.FileManager.MakeAbsolute(_behaviorPath, filename);
        }

        public event WasModifiedEventDelegate WasModified;
        public event WasSavedEventDelegate WasSaved;
        public event WasRenamedEventDelegate WasRenamed;

        private bool _isModified = false;
        public bool IsModified
        {
            get
            {
                return _isModified;
            }
        }

        protected int _modificationID = 0;
        public int ModificationID
        {
            get
            {
                return _modificationID;
            }
        }

        public void TriggerWasModified(Node node)
        {
            _isModified = true;
            _modificationID++;

            if (WasModified != null)
            {
                WasModified(this, node);
            }
        }

        public void TriggerWasSaved()
        {
            _isModified = false;

            if (_fileManager != null)
            {
                this.Label = Path.GetFileNameWithoutExtension(_fileManager.Filename);
            }

            if (WasSaved != null)
            {
                WasSaved(this);
            }
        }

        public void TriggerWasRenamed()
        {
            if (WasRenamed != null)
            {
                WasRenamed(this);
            }
        }

        protected List<ParInfo> _localVars = new List<ParInfo>();
        public override List<ParInfo> LocalVars
        {
            get
            {
                return _localVars;
            }
        }

        /// <summary>
        /// Returns the relative filename and path of the behaviour.
        /// Used to show a unique behaviour name in the CheckForErrors dialogue.
        /// </summary>
        /// <param name="behaviorFolder">The root folder of the behaviour.</param>
        /// <returns>Returns a string with the relative path of this behaviour.</returns>
        public string GetPathLabel(string behaviorFolder)
        {
            string label = Label;

            if (FileManager != null && FileManager.Filename != string.Empty)
            {
                // cut away the behaviour folder
                label = FileManager.Filename.Substring(behaviorFolder.Length + 1);

                // remove the extension
                string ext = System.IO.Path.GetExtension(label);
                label = label.Substring(0, label.Length - ext.Length);

                // replace ugly back-slashes with nicer forward-slashes
                label = label.Replace('\\', '/');
            }

            return label;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (!this._isVisiting)
            {
                this._isVisiting = true;

                if (this.FSMNodes.Count == 0 && this.InitialStateId >= 0)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "There can not be Start condition on the empty FSM!"));
                }

                // check if the node has any children
                if (!this.IsFSM && _genericChildren.ChildCount < 1)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.BehaviorIsEmptyError));
                }

                if (this._agentType == null)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.NoAgent));
                }

                //else if (this._agentType.IsCustomized)
                //{ result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.CustomizedAgentError)); }

                base.CheckForErrors(rootBehavior, result);

                this._isVisiting = false;
            }
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (!this._isVisiting)
            {
                this._isVisiting = true;

                if (metaOperation == MetaOperations.CheckAgentName || metaOperation == MetaOperations.RenameAgentType)
                {
                    Debug.Check(agentType != null);
                    if (agentType != null)
                    {
                        if (agentType == this._agentType)
                        {
                            bReset = true;
                        }
                    }
                }

                bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

                this._isVisiting = false;
            }

            return bReset;
        }

        public override void GetReferencedFiles(ref List<string> referencedFiles)
        {
            if (!this._isVisiting)
            {
                this._isVisiting = true;

                base.GetReferencedFiles(ref referencedFiles);

                this._isVisiting = false;
            }
        }

        public override void GetObjectsByType(Nodes.Node root, string nodeType, bool matchCase, bool matchWholeWord, ref List<ObjectPair> objects)
        {
            if (!this._isVisiting)
            {
                this._isVisiting = true;

                base.GetObjectsByType(root, nodeType, matchCase, matchWholeWord, ref objects);

                this._isVisiting = false;
            }
        }

        public override void GetObjectsByPropertyMethod(Nodes.Node root, string propertyName, bool matchCase, bool matchWholeWord, ref List<ObjectPair> objects)
        {
            if (!this._isVisiting)
            {
                this._isVisiting = true;

                base.GetObjectsByPropertyMethod(root, propertyName, matchCase, matchWholeWord, ref objects);

                this._isVisiting = false;
            }
        }
    }
}
