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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Attachments.Overrides;

namespace Behaviac.Design.Nodes
{
    /// <summary>
    /// This is the class for all nodes which are part of a behaviour tree and are not view data.
    /// </summary>
    public abstract partial class Node : BaseNode, DefaultObject, ICloneable
    {
        public enum ColorThemes
        {
            Default,
            Modern
        }

        private static ColorThemes _colorTheme = ColorThemes.Default;
        public static ColorThemes ColorTheme
        {
            get
            {
                return _colorTheme;
            }
            set
            {
                _colorTheme = value;
            }
        }

        public virtual string DocLink
        {
            get
            {
                return "http://www.behaviac.com/";
            }
        }

        public static Dictionary<string, Brush> BackgroundBrushes = new Dictionary<string, Brush>();

        protected readonly static Brush __backgroundBrush = new SolidBrush(Color.FromArgb(30, 99, 120));

        public Brush BackgroundBrush
        {
            get
            {
                if (ColorTheme == ColorThemes.Default)
                {
                    return DefaultBackgroundBrush;
                }

                string nodeName = this.GetType().FullName;

                if (BackgroundBrushes.ContainsKey(nodeName))
                {
                    return BackgroundBrushes[nodeName];
                }

                return __backgroundBrush;
            }
        }

        protected virtual Brush DefaultBackgroundBrush
        {
            get
            {
                return __backgroundBrush;
            }
        }

        public virtual Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.ObjectUIPolicy();
        }

        public virtual bool CanBeDragged()
        {
            return true;
        }

        public virtual bool CanBeDeleted()
        {
            return this.ParentCanAdoptChildren || this.IsFSM;
        }

        public bool CanBeDisabled()
        {
            return this.Enable ? (Parent != null && Parent.Children.Count > 1) : true;
        }

        public virtual bool AlwaysExpanded()
        {
            return false;
        }

        public virtual bool HasPrefixLabel
        {
            get
            {
                return true;
            }
        }

        public virtual string MiddleLabel
        {
            get
            {
                return null;
            }
        }

        public virtual bool HasFirstLabel
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsCasting
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Add a new child but the behaviour does not need to be saved.
        /// Used for collapsed referenced behaviours which show the behaviours they reference.
        /// </summary>
        /// <param name="connector">The connector the node will be added to. Use null for default connector.</param>
        /// <param name="node">The node you want to append.</param>
        /// <returns>Returns true if the child could be added.</returns>
        public virtual bool AddChildNotModified(Connector connector, Node node)
        {
            Debug.Check(connector != null && _children.HasConnector(connector));

            if (!connector.AcceptsChild(node))
            {
                //throw new Exception(Resources.ExceptionNodeHasTooManyChildren);
                return false;
            }

            if (!connector.AddChild(node))
            {
                return false;
            }

            node._parent = this;

            return true;
        }

        public void AddChild(string connectorStr, Node node)
        {
            Connector connector = this.GetConnector(connectorStr);
            this.AddChild(connector, node);
        }

        /// <summary>
        /// Add a new child node.
        /// </summary>
        /// <param name="connector">The connector the node will be added to. Use null for default connector.</param>
        /// <param name="node">The node you want to append.</param>
        /// <returns>Returns true if the child could be added.</returns>
        public virtual bool AddChild(Connector connector, Node node)
        {
            if (!AddChildNotModified(connector, node))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add a new child node.
        /// </summary>
        /// <param name="connector">The connector the node will be added to. Use null for default connector.</param>
        /// <param name="node">The node you want to append.</param>
        /// <param name="index">The index of the new node.</param>
        /// <returns>Returns true if the child could be added.</returns>
        public virtual bool AddChild(Connector connector, Node node, int index)
        {
            Debug.Check(connector != null && _children.HasConnector(connector));

            if (!connector.AcceptsChild(node))
            {
                throw new Exception(Resources.ExceptionNodeHasTooManyChildren);
            }

            if (!connector.AddChild(node, index))
            {
                return false;
            }

            node._parent = this;

            return true;
        }

        /// <summary>
        /// Removes a child node.
        /// </summary>
        /// <param name="connector">The connector the child is attached to.</param>
        /// <param name="node">The child node we want to remove.</param>
        public virtual void RemoveChild(Connector connector, Node node)
        {
            Debug.Check(connector != null && _children.HasConnector(connector));

            if (connector != null && !connector.RemoveChild(node))
            {
                throw new Exception(Resources.ExceptionNodeIsNoChild);
            }

            if (node != null)
            {
                node._parent = null;
            }
        }

        //a chance to modify the structure or data
        public virtual void PostCreate(List<Node.ErrorCheck> result, int version, System.Xml.XmlNode xmlNode)
        {
        }

        public virtual void PostCreatedByEditor()
        {
        }

        public new BehaviorNode Behavior
        {
            get
            {
                return ((BaseNode)this).Behavior;
            }
        }

        public virtual List<ParInfo> LocalVars
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if an attachment of a certain type is aceepted by this node or not.
        /// </summary>
        /// <param name="type">The type of the attachment we want to add.</param>
        /// <returns>Returns if the attachment may be added or not</returns>
        public virtual bool AcceptsAttachment(DefaultObject obj)
        {
            if (obj != null)
            {
                bool bCanBeAttached = obj.CanBeAttached;
                return !obj.IsFSM && bCanBeAttached;
            }

            return false;
        }

        protected List<Attachments.Attachment> _attachments;

        private void sortAttachments()
        {
            List<Attachments.Attachment> bottomAttachments = new List<Attachments.Attachment>();

            for (int i = _attachments.Count - 1; i >= 0; i--)
            {
                if (!_attachments[i].IsPrecondition)
                {
                    bottomAttachments.Add(_attachments[i]);
                    _attachments.RemoveAt(i);
                }
            }

            for (int i = bottomAttachments.Count - 1; i >= 0; i--)
            {
                _attachments.Add(bottomAttachments[i]);
            }
        }

        public IList<Attachments.Attachment> Attachments
        {
            get
            {
                return _attachments;
            }
        }

        public void AddAttachment(Attachments.Attachment attach)
        {
            Debug.Check(attach != null);

            if (attach != null)
            {
                _attachments.Add(attach);

                sortAttachments();

                attach.ResetId();
            }
        }

        public void AddAttachment(Attachments.Attachment attach, int index)
        {
            Debug.Check(attach != null);

            if (attach != null)
            {
                _attachments.Insert(index, attach);

                sortAttachments();

                attach.ResetId();
            }
        }

        public void RemoveAttachment(Attachments.Attachment attach)
        {
            _attachments.Remove(attach);
        }

        protected string _exportName = string.Empty;

        /// <summary>
        /// The name of the node used for the export process.
        /// </summary>
        public string ExportName
        {
            get
            {
                return _exportName;
            }
        }

        protected string _label;

        /// <summary>
        /// The label shown of the node.
        /// </summary>
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
            }
        }

        protected readonly string _description;

        /// <summary>
        /// The description of this node.
        /// </summary>
        public virtual string Description
        {
            get
            {
                string desc = Resources.PressF1 + "\n" + _description;
                return desc;
            }
        }

        public virtual object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            return null;
        }

        /// <summary>
        /// Creates a new behaviour node.
        /// </summary>
        /// <param name="label">The label of the behaviour node.</param>
        /// <returns>Returns the created behaviour node.</returns>
        public static BehaviorNode CreateBehaviorNode(string label)
        {
            BehaviorNode node = (BehaviorNode)Plugin.BehaviorNodeType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { label, true });

            if (node == null)
            {
                throw new Exception(Resources.ExceptionMissingNodeConstructor);
            }

            return node;
        }

        /// <summary>
        /// Creates a new referenced behaviour node.
        /// </summary>
        /// <param name="rootBehavior">The behaviour we are adding the reference to.</param>
        /// <param name="referencedBehavior">The behaviour we are referencing.</param>
        /// <returns>Returns the created referenced behaviour node.</returns>
        public static ReferencedBehavior CreateReferencedBehaviorNode(BehaviorNode rootBehavior, BehaviorNode referencedBehavior, bool isFSM = false)
        {
            Type type = isFSM ? Plugin.FSMReferencedBehaviorNodeType : Plugin.ReferencedBehaviorNodeType;
            Debug.Check(type != null);

            if (type != null)
            {
                ReferencedBehavior node = (ReferencedBehavior)type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { rootBehavior, referencedBehavior });

                if (node == null)
                {
                    throw new Exception(Resources.ExceptionMissingNodeConstructor);
                }

                return node;
            }

            return null;
        }

        /// <summary>
        /// Creates a node from a given type.
        /// </summary>
        /// <param name="type">The type we want to create a node of.</param>
        /// <returns>Returns the created node.</returns>
        public static Node Create(Type type)
        {
            Debug.Check(type != null);

            if (type != null)
            {
                // use the type overrides when set
                if (type == typeof(BehaviorNode))
                {
                    type = Plugin.BehaviorNodeType;
                }
                else if (type == typeof(ReferencedBehavior))
                {
                    type = Plugin.ReferencedBehaviorNodeType;
                }

                Debug.Check(type != null);
                if (type != null)
                {
                    Debug.Check(!type.IsAbstract);

                    Node node = (Node)type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);

                    if (node == null)
                    {
                        throw new Exception(Resources.ExceptionMissingNodeConstructor);
                    }

                    return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if the children of this node will be saved. Required for referenced behaviours.
        /// </summary>
        public virtual bool SaveChildren
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The name of the class we want to use for the exporter. This is usually the implemented node of the game.
        /// </summary>
        public virtual string ExportClass
        {
            get
            {
                return GetType().FullName;
            }
        }

        private Comment _comment = new Comment("");

        /// <summary>
        /// The comment object of the node.
        /// </summary>
        public Comment CommentObject
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        private bool _enable = true;
        [DesignerBoolean("Enable", "EnableDesc", "Debug", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport)]
        public bool Enable
        {
            get
            {
                return _enable;
            }
            set
            {
                _enable = value;
            }
        }

        private int _id = -1;
        [DesignerInteger("NodeId", "NodeIdDesc", "Debug", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.ReadOnly | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NotPrefabRelated, null, int.MinValue, int.MaxValue, 1, null)]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// The relative path of the prefab behavior
        /// </summary>
        private string _prefabName = string.Empty;
        [DesignerString("PrefabName", "PrefabNameDesc", "Prefab", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NotPrefabRelated)]
        public string PrefabName
        {
            get
            {
                return _prefabName;
            }
            set
            {
                _prefabName = value;

                if (!string.IsNullOrEmpty(_prefabName))
                {
                    _prefabName = _prefabName.Replace("预制文件集", "Prefabs");
                }
            }
        }

        /// <summary>
        /// The node id in the prefab behavior
        /// </summary>
        private int _prefabNodeId = -1;
        [DesignerInteger("PrefabNodeId", "PrefabNodeIdDesc", "Prefab", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NotPrefabRelated, null, int.MinValue, int.MaxValue, 1, null)]
        public int PrefabNodeId
        {
            get
            {
                return _prefabNodeId;
            }
            set
            {
                _prefabNodeId = value;
            }
        }

        /// <summary>
        /// If modifying the prefab data in the current node
        /// </summary>
        private bool _hasOwnPrefabData = false;
        [DesignerBoolean("HasOwnPrefabData", "HasOwnPrefabDataDesc", "Prefab", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NotPrefabRelated)]
        public bool HasOwnPrefabData
        {
            get
            {
                return _hasOwnPrefabData;
            }
            set
            {
                _hasOwnPrefabData = value;
            }
        }

        public bool IsPrefabDataDirty()
        {
            if (this.HasOwnPrefabData)
            {
                return true;
            }

            foreach (Node child in this.Children)
            {
                if (child.IsPrefabDataDirty())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The text of the comment shown for the node and its children.
        /// </summary>
        [DesignerString("NodeCommentText", "NodeCommentTextDesc", "CategoryComment", DesignerProperty.DisplayMode.NoDisplay, 10, DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NoSave | DesignerProperty.DesignerFlags.NotPrefabRelated)]
        public string CommentText
        {
            get
            {
                return _comment == null ? string.Empty : _comment.Text;
            }

            set
            {
                string str = value.Trim();

                if (str.Length < 1)
                {
                    _comment = null;

                }
                else
                {
                    if (_comment == null)
                    {
                        _comment = new Comment(str);
                    }

                    else
                    {
                        _comment.Text = str;
                    }

                    if (_comment.Background == CommentColor.NoColor && !string.IsNullOrEmpty(str))
                    {
                        _comment.Background = CommentColor.Gray;
                    }
                }
            }
        }

        /// <summary>
        /// The color of the comment shown for the node and its children.
        /// </summary>
        [DesignerEnum("NodeCommentBackground", "NodeCommentBackgroundDesc", "CategoryComment", DesignerProperty.DisplayMode.NoDisplay, 20, DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NoSave | DesignerProperty.DesignerFlags.NotPrefabRelated, "")]
        public CommentColor CommentBackground
        {
            get
            {
                return _comment == null ? CommentColor.NoColor : _comment.Background;
            }

            set
            {
                if (_comment != null)
                {
                    _comment.Background = value;
                }
            }
        }

        public void ResetId(bool setChildren)
        {
            Node root = (Node)this.Behavior;

            if (Id < 0 || null != Plugin.GetPreviousObjectById(root, Id, this))
            {
                Id = Plugin.NewNodeId(root);
            }

            foreach (Attachments.Attachment attach in this.Attachments)
            {
                if (attach.Id < 0 || null != Plugin.GetPreviousObjectById(root, attach.Id, attach))
                {
                    attach.Id = Plugin.NewNodeId(root);
                }
            }

            if (setChildren && !(this is ReferencedBehavior))
            {
                foreach (Node child in this.GetChildNodes())
                {
                    child.ResetId(setChildren);
                }
            }
        }

        private void checkId()
        {
            Node root = (Node)this.Behavior;

            // If its id has existed, reset it.
            if (null != Plugin.GetPreviousObjectById(root, Id, this))
            {
                ResetId(false);

                if (this.PrefabNodeId < 0)
                {
                    this.Behavior.TriggerWasModified(this);
                }
            }
        }

        /// <summary>
        /// Creates a new node and attaches the default attributes DebugName and ExportType.
        /// </summary>
        /// <param name="label">The default label of the node.</param>
        /// <param name="description">The description of the node shown to the designer.</param>
        protected Node(string label, string description)
        {
            _children = new ConnectedChildren(this);

            _label = label;
            _description = description;
            _attachments = new List<Attachments.Attachment>();
        }

        public virtual void OnPropertyValueChanged(DesignerPropertyInfo property)
        {
        }

        /// <summary>
        /// Is called when one of the node's properties were modified.
        /// </summary>
        /// <param name="wasModified">Holds if the event was modified.</param>
        public void OnPropertyValueChanged(bool wasModified)
        {
            if (wasModified)
            {
                BehaviorNode root = this.Behavior;

                if (root != null)
                {
                    root.TriggerWasModified(this);
                }
            }
        }

        public delegate void SubItemAddedEventDelegate(Node node, DesignerPropertyInfo property);
        public event SubItemAddedEventDelegate SubItemAdded;

        public void DoSubItemAdded(DesignerPropertyInfo property)
        {
            if (SubItemAdded != null)
            {
                SubItemAdded(this, property);
            }
        }

        /// <summary>
        /// Is called after the behaviour was loaded.
        /// </summary>
        /// <param name="behavior">The behaviour this node belongs to.</param>
        public virtual void PostLoad(BehaviorNode behavior)
        {
            checkId();
        }

        /// <summary>
        /// Is called before the behaviour is saved.
        /// </summary>
        /// <param name="behavior">The behaviour this node belongs to.</param>
        public void PreSave(BehaviorNode behavior)
        {
        }

        /// <summary>
        /// Returns the name of the node's type for the attribute ExportType.
        /// This is done as the class attribute can be quite long and bad to handle.
        /// </summary>
        /// <returns>Returns the value for ExportType</returns>
        protected virtual string GetExportType()
        {
            return GetType().Name;
        }

        public bool ReplaceNode(Node node)
        {
            if (node == null || !node.IsFSM && node.ParentConnector == null)
            {
                return false;
            }

            bool replaced = (node.Children.Count == 0);

            if (!replaced && this.CanAdoptChildren(node))
            {
                foreach (Node.Connector connector in node.Connectors)
                {
                    Node.Connector newConnector = this.GetConnector(connector.Identifier);

                    if (newConnector != null)
                    {
                        for (int i = 0; i < connector.ChildCount; ++i)
                        {
                            replaced |= this.AddChild(newConnector, (Node)connector.GetChild(i));
                        }

                        connector.ClearChildrenInternal();
                    }
                }
            }

            if (replaced)
            {
                this.Id = node.Id;
                this.PrefabName = node.PrefabName;
                this.PrefabNodeId = node.PrefabNodeId;
                this.HasOwnPrefabData = node.HasOwnPrefabData;
                this.CommentObject = node.CommentObject;

                Node parentNode = (Node)node.Parent;

                if (node.IsFSM)
                {
                    Debug.Check(this.IsFSM);

                    parentNode.RemoveFSMNode(node);
                    parentNode.AddFSMNode(this);

                    this.ScreenLocation = node.ScreenLocation;
                }
                else
                {
                    Node.Connector parentConnector = node.ParentConnector;
                    Debug.Check(parentConnector != null);

                    if (parentConnector != null)
                    {
                        int index = parentConnector.GetChildIndex(node);
                        Debug.Check(index >= 0);

                        parentNode.RemoveChild(parentConnector, node);
                        parentNode.AddChild(parentConnector, this, index);
                    }
                }

                foreach (Attachments.Attachment attach in node.Attachments)
                {
                    if (attach != null && this.AcceptsAttachment(attach))
                    {
                        this.AddAttachment(attach);
                    }
                }
            }

            return replaced;
        }

        public bool SetPrefab(string prefabName, bool prefabDirty = false, string oldPrefabName = "")
        {
            bool resetName = false;

            if (!string.IsNullOrEmpty(oldPrefabName))
            {
                if (this.PrefabName == oldPrefabName)
                {
                    this.PrefabName = prefabName;
                    resetName = true;
                }

            }
            else if (string.IsNullOrEmpty(this.PrefabName))
            {
                this.PrefabName = prefabName;
                this.PrefabNodeId = this.Id;
                this.HasOwnPrefabData = prefabDirty;
            }

            if (!(this is ReferencedBehavior))
            {
                foreach (Node child in this.Children)
                {
                    resetName |= child.SetPrefab(prefabName, prefabDirty, oldPrefabName);
                }
            }

            return resetName;
        }

        public bool ClearPrefab(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return false;
            }

            bool clear = false;

            if (this.PrefabName == prefabName)
            {
                clear = true;

                this.PrefabName = string.Empty;
                this.PrefabNodeId = -1;
                this.HasOwnPrefabData = false;
            }

            if (!(this is ReferencedBehavior))
            {
                foreach (Node child in this.Children)
                {
                    clear |= child.ClearPrefab(prefabName);
                }
            }

            return clear;
        }

        private void ClearPrefabDirty(string prefabName)
        {
            if (this.PrefabName == prefabName)
            {
                this.HasOwnPrefabData = false;
            }

            if (!(this is ReferencedBehavior))
            {
                foreach (Node child in this.Children)
                {
                    child.ClearPrefabDirty(prefabName);
                }
            }
        }

        public void RestorePrefab(string prefabName)
        {
            if (this.PrefabName == prefabName)
            {
                this.PrefabName = string.Empty;
                this.Id = this.PrefabNodeId;
                this.PrefabNodeId = -1;
                this.HasOwnPrefabData = false;
            }

            if (!(this is ReferencedBehavior))
            {
                foreach (Node child in this.Children)
                {
                    child.RestorePrefab(prefabName);
                }
            }
        }

        public bool ResetPrefabInstances(string prefabName, Node instanceRootNode)
        {
            bool reset = instanceRootNode.ResetByPrefab(prefabName, this);

            if (!(this is ReferencedBehavior))
            {
                foreach (Node child in this.Children)
                {
                    reset |= child.ResetPrefabInstances(prefabName, instanceRootNode);
                }
            }

            return reset;
        }

        private bool checkPrefab(string prefabName, Node prefabNode)
        {
            return !this.HasOwnPrefabData && this.PrefabName == prefabName && this.PrefabNodeId == prefabNode.Id;
        }

        public bool ResetByPrefab(string prefabName, Node prefabNode)
        {
            bool reset = false;

            if (this.checkPrefab(prefabName, prefabNode))
            {
                reset = true;

                int preId = this.Id;
                string prePrefabName = this.PrefabName;
                int prePrefabNodeId = this.PrefabNodeId;
                Comment preComment = (this.CommentObject != null) ? this.CommentObject.Clone() : null;

                if (prefabNode.GetType() == this.GetType())
                {
                    prefabNode.CloneProperties(this);
                }

                this.Id = preId;
                this.PrefabName = prePrefabName;
                this.PrefabNodeId = prePrefabNodeId;
                this.HasOwnPrefabData = false;
                this.CommentObject = preComment;

                // check the deleted children by the prefab node
                List<Node> deletedChildren = new List<Node>();

                foreach (Node child in this.Children)
                {
                    if (!child.HasOwnPrefabData && child.PrefabName == prefabName)
                    {
                        bool bFound = false;

                        foreach (Node prefabChild in prefabNode.Children)
                        {
                            if (child.PrefabNodeId == prefabChild.Id)
                            {
                                bFound = true;
                                break;
                            }
                        }

                        if (!bFound)
                        {
                            deletedChildren.Add(child);
                        }
                    }
                }

                foreach (Node child in deletedChildren)
                {
                    ((Node)child.Parent).RemoveChild(child.ParentConnector, child);
                }

                // check the added children by the prefab node
                List<Node> addedChildren = new List<Node>();
                List<int> indexes = new List<int>();

                for (int i = 0; i < prefabNode.Children.Count; ++i)
                {
                    Node prefabChild = (Node)prefabNode.Children[i];
                    bool bFound = false;

                    foreach (Node child in this.Children)
                    {
                        if (!string.IsNullOrEmpty(prefabChild.PrefabName) && child.PrefabName == prefabChild.PrefabName && child.PrefabNodeId == prefabChild.PrefabNodeId ||
                            child.PrefabName == prefabName && child.PrefabNodeId == prefabChild.Id)
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        addedChildren.Add(prefabChild);
                        indexes.Add(i);
                    }
                }

                for (int i = 0; i < addedChildren.Count; ++i)
                {
                    Node child = addedChildren[i].CloneBranch();

                    if (child != null)
                    {
                        child.SetPrefab(prefabName);

                        Node.Connector conn = addedChildren[i].ParentConnector;
                        Debug.Check(conn != null);
                        if (conn != null)
                        {
                            Node.Connector childconn = this.GetConnector(conn.Identifier);
                            Debug.Check(childconn != null);

                            if (childconn != null)
                            {
                                if (indexes[i] < this.Children.Count)
                                {
                                    this.AddChild(childconn, child, indexes[i]);
                                }

                                else
                                {
                                    this.AddChild(childconn, child);
                                }

                                child.ResetId(true);
                            }
                        }
                    }
                }
            }

            if (!(this is ReferencedBehavior))
            {
                for (int i = 0; i < this.Children.Count; ++i)
                {
                    Node child = this.Children[i] as Node;

                    if (child != null)
                    {
                        if (child.checkPrefab(prefabName, prefabNode))
                        {
                            if (child.GetType() != prefabNode.GetType())   // replace
                            {
                                Node newNode = prefabNode.Clone() as Node;
                                newNode.ReplaceNode(child);

                                child = newNode;
                            }
                        }

                        reset |= child.ResetByPrefab(prefabName, prefabNode);
                    }
                }
            }

            return reset;
        }

        public Node GetPrefabRoot()
        {
            string prefabName = this.PrefabName;

            if (!string.IsNullOrEmpty(prefabName))
            {
                Node node = this;
                Node root = this;

                while (node.Parent != null)
                {
                    string parentPrefabName = ((Node)node.Parent).PrefabName;

                    if (!string.IsNullOrEmpty(parentPrefabName))
                    {
                        if (parentPrefabName == prefabName)
                        {
                            root = (Node)node.Parent;
                        }

                        else
                        {
                            break;
                        }
                    }

                    node = (Node)node.Parent;
                }

                return root;
            }

            return null;
        }

        public BehaviorNode ApplyPrefabInstance()
        {
            string prefabName = this.PrefabName;

            if (!string.IsNullOrEmpty(prefabName))
            {
                Node root = this.GetPrefabRoot();

                if (root != null)
                {
                    string fullpath = FileManagers.FileManager.GetFullPath(prefabName);
                    BehaviorNode prefabBehavior = BehaviorManager.Instance.LoadBehavior(fullpath);

                    if (prefabBehavior != null)
                    {
                        Behavior b = this.Behavior as Behavior;
                        Debug.Check(b != null);

                        if (b != null)
                        {
                            b.AgentType.ResetPars(b.LocalVars);
                        }

                        root.ClearPrefabDirty(prefabName);

                        if (((Node)prefabBehavior).Children.Count > 0)
                        {
                            ((Node)prefabBehavior).RemoveChild(prefabBehavior.GenericChildren, (Node)((Node)prefabBehavior).Children[0]);
                        }

                        Node newnode = root.CloneBranch();
                        newnode.RestorePrefab(prefabName);
                        ((Node)prefabBehavior).AddChild(prefabBehavior.GenericChildren, newnode);

                        return prefabBehavior;
                    }
                }
            }

            return null;
        }

        public bool BreakPrefabInstance()
        {
            string prefabName = this.PrefabName;

            if (!string.IsNullOrEmpty(prefabName))
            {
                Node root = this.GetPrefabRoot();

                if (root != null)
                {
                    string fullpath = FileManagers.FileManager.GetFullPath(prefabName);
                    Nodes.BehaviorNode prefabBehavior = BehaviorManager.Instance.LoadBehavior(fullpath);

                    if (prefabBehavior != null)
                    {
                        Behavior b = this.Behavior as Behavior;
                        Debug.Check(b != null);

                        if (b != null)
                        {
                            b.AgentType.ResetPars(b.LocalVars);
                        }

                        root.ResetByPrefab(prefabName, (Node)prefabBehavior);
                    }

                    return root.ClearPrefab(prefabName);
                }
            }

            return false;
        }

        public void SetEnterExitSlot(List<Nodes.Node.ErrorCheck> result, string actionStr, bool bEnter)
        {
            Type t = null;

            if (bEnter)
            {
                t = Plugin.GetType("PluginBehaviac.Events.Precondition");
            }
            else
            {
                t = Plugin.GetType("PluginBehaviac.Events.Effector");
            }

            Behaviac.Design.Attachments.Attachment a = Behaviac.Design.Attachments.Attachment.Create(t, this);
            this.AddAttachment(a);

            IList<DesignerPropertyInfo> properties = a.GetDesignerProperties();

            foreach (DesignerPropertyInfo p in properties)
            {
                if (p.Property.Name == "Opl")
                {
                    p.SetValueFromString(result, a, actionStr);
                }
                else if (p.Property.Name == "Opr")
                {
                    p.SetValueFromString(result, a, "const bool true");
                }
                else if (!bEnter && p.Property.Name == "Phase")
                {
                    p.SetValueFromString(result, a, "Both");
                }
            }
        }

        /// <summary>
        /// Checks the current node and its children for errors.
        /// </summary>
        /// <param name="rootBehavior">The behaviour we are currently checking.</param>
        /// <param name="result">The list the errors are added to.</param>
        public virtual void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (Plugin.EditMode == EditModes.Design)
            {
                if (!Enable)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Warning, Resources.Disabled));
                }

                //don't check locals for ReferencedBehavior and Event
                if (this is Behavior)
                {
                    Behavior behavior = this as Behavior;
                    CheckPars(behavior.LocalVars, ref result);
                }

                foreach (Node node in _children)
                {
                    if (node.Enable)
                    {
                        node.CheckForErrors(rootBehavior, result);
                    }
                }

                foreach (Node node in this.FSMNodes)
                {
                    node.CheckForErrors(rootBehavior, result);
                }

                foreach (Attachments.Attachment attachment in _attachments)
                {
                    attachment.CheckForErrors(rootBehavior, result);
                }
            }
        }

        private void CheckPars(List<ParInfo> pars, ref List<ErrorCheck> result)
        {
            foreach (ParInfo par in pars)
            {
                if (par.Display)
                {
                    List<ErrorCheck> parResult = new List<ErrorCheck>();
                    Plugin.CheckPar(this, par, ref parResult);

                    if (parResult.Count == 0)
                    {
                        string info = string.Format(Resources.ParWarningInfo, par.Name);
                        result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Warning, info));
                    }
                }
            }
        }

        public virtual bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            foreach (Attachments.Attachment attach in this.Attachments)
            {
                bReset |= attach.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            foreach (Node child in this.GetChildNodes())
            {
                bReset |= child.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            return bReset;
        }

        public virtual void GetReferencedFiles(ref List<string> referencedFiles)
        {
            foreach (Attachments.Attachment attach in this.Attachments)
            {
                attach.GetReferencedFiles(ref referencedFiles);
            }

            foreach (Node child in this.GetChildNodes())
            {
                child.GetReferencedFiles(ref referencedFiles);
            }
        }



        public virtual void GetObjectsByType(Nodes.Node root, string nodeType, bool matchCase, bool matchWholeWord, ref List<ObjectPair> objects)
        {
            if (root == null || string.IsNullOrEmpty(nodeType))
            {
                return;
            }

            GetObjectsBySelfType(root, nodeType, matchCase, matchWholeWord, ref objects);

            foreach (Attachments.Attachment attach in this.Attachments)
            {
                if (!Plugin.ContainObjectPair(objects, root, attach) && Plugin.CompareTwoTypes(attach.GetType().Name, nodeType, matchCase, matchWholeWord))
                {
                    objects.Add(new ObjectPair(root, attach));
                }
            }

            foreach (Nodes.Node child in this.GetChildNodes())
            {
                child.GetObjectsByType(root, nodeType, matchCase, matchWholeWord, ref objects);
            }
        }

        protected void GetObjectsBySelfType(Nodes.Node root, string nodeType, bool matchCase, bool matchWholeWord, ref List<ObjectPair> objects)
        {
            if (root == null || string.IsNullOrEmpty(nodeType))
            {
                return;
            }

            if (!Plugin.ContainObjectPair(objects, root, this) && Plugin.CompareTwoTypes(this.GetType().Name, nodeType, matchCase, matchWholeWord))
            {
                objects.Add(new ObjectPair(root, this));
            }
        }

        public virtual void GetObjectsByPropertyMethod(Nodes.Node root, string propertyName, bool matchCase, bool matchWholeWord, ref List<ObjectPair> objects)
        {
            if (root == null || string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            Plugin.GetObjectsBySelfPropertyMethod(root, this, propertyName, matchCase, matchWholeWord, ref objects);

            foreach (Attachments.Attachment attach in this.Attachments)
            {
                Plugin.GetObjectsBySelfPropertyMethod(root, attach, propertyName, matchCase, matchWholeWord, ref objects);
            }

            foreach (Nodes.BaseNode child in this.GetChildNodes())
            {
                if (child is Nodes.Node)
                {
                    Nodes.Node childNode = child as Nodes.Node;
                    childNode.GetObjectsByPropertyMethod(root, propertyName, matchCase, matchWholeWord, ref objects);
                }
            }
        }

        /// <summary>
        /// Creates a view for this node. Allows you to return your own class and store additional data.
        /// </summary>
        /// <param name="rootBehavior">The root of the graph of the current view.</param>
        /// <param name="parent">The parent of the NodeViewData created.</param>
        /// <returns>Returns a new NodeViewData object for this node.</returns>
        public virtual NodeViewData CreateNodeViewData(NodeViewData parent, BehaviorNode rootBehavior)
        {
            return new NodeViewDataStyled(parent, rootBehavior, this, null, BackgroundBrush, _label, _description);
        }

        /// <summary>
        /// Searches a list for NodeViewData for this node. Internal use only.
        /// </summary>
        /// <param name="list">The list which is searched for the NodeViewData.</param>
        /// <returns>Returns null if no fitting NodeViewData could be found.</returns>
        public virtual NodeViewData FindNodeViewData(List<NodeViewData> list)
        {
            foreach (NodeViewData nvd in list)
            {
                if (nvd.Node == this)
                {
                    return nvd;
                }
            }

            return null;
        }

        /// <summary>
        /// Internally used by CloneBranch.
        /// </summary>
        /// <param name="newparent">The parent the clone children will be added to.</param>
        private void CloneChildNodes(Node newparent)
        {
            // we may not clone children of a referenced behavior
            if (newparent is ReferencedBehavior)
            {
                return;
            }

            // for each connector
            foreach (Connector connector in _children.Connectors)
            {
                // find the one from the new node...
                Connector localconn = newparent.GetConnector(connector.Identifier);
                //Debug.Check(localconn != null);

                // and duplicate its children into the new node's connector
                for (int i = 0; i < connector.ChildCount; ++i)
                {
                    Node child = (Node)connector.GetChild(i);

                    Node newchild = (Node)child.Clone();
                    newparent.AddChild(localconn, newchild);

                    // do this for the children as well
                    child.CloneChildNodes(newchild);
                }
            }

            // for each FSM node
            foreach (Node child in this.FSMNodes)
            {
                Node newchild = (Node)child.Clone();
                newparent.AddFSMNode(newchild);

                // do this for the children as well
                child.CloneChildNodes(newchild);
            }
        }

        public string GetFullId()
        {
            string fullId = this.Id.ToString();
            BaseNode n = this;

            while (n != null)
            {
                if (n is Behavior)
                {
                    Behavior b = n as Behavior;

                    if (b.ParentNode != null)
                    {
                        fullId = string.Format("{0}:{1}", b.ParentNode.Id, fullId);
                    }

                    n = b.ParentNode;

                }
                else
                {
                    n = n.Parent;
                }
            }

            return fullId;
        }

        /// <summary>
        /// Duplicates a node and all of its children.
        /// </summary>
        /// <returns>New node with new children.</returns>
        public Node CloneBranch()
        {
            Node newnode;

            if (this is ReferencedBehavior)
            {
                // if we want to clone the branch of a referenced behaviour we have to create a new behaviour node for that.
                // this should only be used to visualise stuff, never in the behaviour tree itself!
                newnode = Create(typeof(BehaviorNode));
                //newnode.Label= Label;

            }
            else
            {
                newnode = Create(GetType());
                CloneProperties(newnode);
            }

            CloneChildNodes(newnode);

            newnode.OnPropertyValueChanged(false);

            return newnode;
        }

        /// <summary>
        /// Duplicates this node. Parent and children are not copied.
        /// </summary>
        /// <returns>New node without parent and children.</returns>
        public virtual object Clone()
        {
            Node newnode = Create(GetType());

            CloneProperties(newnode);

            newnode.OnPropertyValueChanged(false);

            return newnode;
        }

        private void checkPrefabFile()
        {
            if (!string.IsNullOrEmpty(this.PrefabName))
            {
                string prefabName = FileManagers.FileManager.GetFullPath(this.PrefabName);

                if (!System.IO.File.Exists(prefabName))
                {
                    this.PrefabName = string.Empty;
                    this.PrefabNodeId = -1;
                    this.HasOwnPrefabData = false;
                }
            }
        }

        /// <summary>
        /// Used to duplicate all properties. Any property added must be duplicated here as well.
        /// </summary>
        /// <param name="newnode">The new node which is supposed to get a copy of the properties.</param>
        protected virtual void CloneProperties(Node newnode)
        {
            Debug.Check(newnode != null);

            if (newnode != null)
            {
                // clone properties
                newnode.ScreenLocation = this.ScreenLocation;
                newnode.Id = this.Id;
                newnode.Enable = this.Enable;

                if (this.CommentObject != null)
                {
                    newnode.CommentObject = this.CommentObject.Clone();
                }

                checkPrefabFile();
                newnode.PrefabName = this.PrefabName;
                newnode.PrefabNodeId = this.PrefabNodeId;
                newnode.HasOwnPrefabData = this.HasOwnPrefabData;

                // clone pars.
                if (this is Behavior)
                {
                    Behavior bnew = newnode as Behavior;
                    bnew.LocalVars.Clear();

                    foreach (ParInfo par in ((Behavior)this).LocalVars)
                    {
                        bnew.LocalVars.Add(par.Clone(bnew));
                    }
                }

                // clone attachements
                newnode.Attachments.Clear();

                foreach (Attachments.Attachment attach in _attachments)
                {
                    newnode.AddAttachment(attach.Clone(newnode));
                }
            }
        }

        /// <summary>
        /// This node will be removed from its parent and its children. The parent tries to adopt all children.
        /// </summary>
        /// <returns>Returns false if the parent cannot apobt the children and the operation fails.</returns>
        public bool ExtractNode()
        {
            // we cannot adopt children from a referenced behavior
            if (this is ReferencedBehavior && _parent != null)
            {
                ((Node)_parent).RemoveChild(_parentConnector, this);
                return true;
            }

            // check if the parent is allowed to adopt the children
            if (ParentCanAdoptChildren)
            {
                Connector conn = _parentConnector;
                Node parent = (Node)_parent;

                int n = conn.GetChildIndex(this);
                Debug.Check(n >= 0);

                parent.RemoveChild(conn, this);

                // let the node's parent adopt all the children
                foreach (Connector connector in _children.Connectors)
                {
                    for (int i = 0; i < connector.ChildCount; ++i, ++n)
                    {
                        parent.AddChild(conn, (Node)connector[i], n);
                    }

                    // remove the adopted children from the old connector. Do NOT clear the _connector member which already points to the new connector.
                    connector.ClearChildrenInternal();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a list of all properties which have a designer attribute attached.
        /// </summary>
        /// <returns>A list of all properties relevant to the designer.</returns>
        public IList<DesignerPropertyInfo> GetDesignerProperties()
        {
            return DesignerProperty.GetDesignerProperties(this.GetType());
        }

        /// <summary>
        /// Returns a list of all properties which have a designer attribute attached.
        /// </summary>
        /// <param name="comparison">The comparison used to sort the design properties.</param>
        /// <returns>A list of all properties relevant to the designer.</returns>
        public IList<DesignerPropertyInfo> GetDesignerProperties(Comparison<DesignerPropertyInfo> comparison)
        {
            return DesignerProperty.GetDesignerProperties(this.GetType(), comparison);
        }

        public bool AcceptDefaultPropertyByDragAndDrop()
        {
            return GetDefaultPropertyByDragAndDrop().Property != null;
        }

        public bool SetDefaultPropertyByDragAndDrop(string value)
        {
            return SetPropertyValue(GetDefaultPropertyNameByDragAndDrop(), value);
        }

        protected virtual string GetDefaultPropertyNameByDragAndDrop()
        {
            return "Method";
        }

        private DesignerPropertyInfo GetDefaultPropertyByDragAndDrop()
        {
            IList<DesignerPropertyInfo> properties = this.GetDesignerProperties();

            foreach (DesignerPropertyInfo property in properties)
            {
                if (property.Property != null && property.Property.Name == GetDefaultPropertyNameByDragAndDrop())
                {
                    return property;
                }
            }

            return new DesignerPropertyInfo();
        }

        private bool SetPropertyValue(string propName, string value)
        {
            DesignerPropertyInfo property = GetDefaultPropertyByDragAndDrop();

            if (property.Property != null)
            {
                property.SetValueFromString(null, this, value);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return _label;
        }

        /// <summary>
        /// Used when a DesignerNodeProperty property is exported to format the output.
        /// </summary>
        /// <returns>The format string used to write out the value.</returns>
        public virtual string GetNodePropertyExportString()
        {
            return "\"{0}\"";
        }

        /// <summary>
        /// Returns a list of properties that cannot be selected by a DesignerNodeProperty.
        /// </summary>
        /// <returns>Returns names of properties not allowed.</returns>
        public virtual string[] GetNodePropertyExcludedProperties()
        {
            return new string[] { "ClassVersion", "Version" };
        }

        /// <summary>
        /// Checks if this node has an override for a given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property we are checking.</param>
        /// <returns>Returns true if there is an attachement override.</returns>
        public bool HasOverrride(string propertyName)
        {
            foreach (Attachments.Attachment attach in _attachments)
            {
                Override overr = attach as Override;

                if (overr != null && overr.PropertyToOverride == propertyName)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual string GenerateNewLabel()
        {
            // generate the new label with the arguments
            string newlabel = string.Empty;

            // check all properties for one which must be shown as a parameter on the node
            IList<DesignerPropertyInfo> properties = this.GetDesignerProperties(DesignerProperty.SortByDisplayOrder);
            int paramCount = 0;

            for (int p = 0; p < properties.Count; ++p)
            {
                // property must be shown as a parameter on the node
                if (properties[p].Attribute.Display == DesignerProperty.DisplayMode.Parameter)
                {
                    string strProperty = properties[p].GetDisplayValue(this);

                    if (paramCount > 0)
                    {
                        newlabel += !string.IsNullOrEmpty(this.MiddleLabel) ? this.MiddleLabel : " ";
                    }

                    if (paramCount == 1 && this.HasFirstLabel)
                    {
                        newlabel += " = ";

                    }
                    else
                    {
                        if (paramCount > 0)
                        {
                            newlabel += " ";
                        }
                    }

                    newlabel += strProperty;
                    paramCount++;
                }
            }

            if (paramCount > 0 && this.HasPrefixLabel)
            {
                newlabel = this.Label + "(" + newlabel + ")";
            }

            return newlabel;
        }
    }
}
