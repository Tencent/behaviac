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
using System.IO;
using System.Windows.Forms;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Data;

namespace Behaviac.Design.Nodes
{
    /// <summary>
    /// This node represents a referenced behaviour which can be attached to the behaviour tree.
    /// </summary>
    [NodeDesc("Composites", NodeIcon.Behavior)]
    public class ReferencedBehavior : Node
    {
        protected Connector _genericChildren;

        protected RightValueDef _referencedBehavior = new RightValueDef(new VariableDef(""));
        [DesignerRightValueEnum("ReferencedBehaviorPath", "ReferencedBehaviorPathDesc", "ReferencedBehavior", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.String)]
        public RightValueDef ReferenceBehavior
        {
            get
            {
                return this._referencedBehavior;
            }
            set
            {
                this._referencedBehavior = value;

                if (this._referencedBehavior == null || this._referencedBehavior.Var == null || !this._referencedBehavior.Var.IsConst)
                {
                    this._task = null;
                }
                else if (this._task == null)
                {
                    this.SetTask(this.ReferenceBehaviorString);
                }
            }
        }

        public string ReferenceBehaviorString
        {
            get
            {
                string refTreeStr = this._referencedBehavior.GetExportValue();

                string[] tokens = refTreeStr.Split(' ');

                if (tokens != null)
                {
                    if (tokens[0] == "const")
                    {
                        string refTreePath = tokens[tokens.Length - 1];

                        if (refTreePath[0] == '\'' || refTreePath[0] == '\"')
                        {
                            refTreePath = refTreePath.Substring(1, refTreePath.Length - 2);
                        }

                        return refTreePath;
                    }
                }

                return null;
            }
        }

        public BehaviorNode ReferenceBehaviorNode
        {
            get
            {
                string refTreePath = this.ReferenceBehaviorString;

                if (!string.IsNullOrEmpty(refTreePath))
                {
                    string fullPath = FileManagers.FileManager.GetFullPath(refTreePath);

                    BehaviorNode referencedBehavior = BehaviorManager.Instance.GetBehavior(fullPath);
                    return referencedBehavior;
                }

                return null;
            }
        }

        public void SetReferenceBehavior(string referencedBehaviorFileName)
        {
            this._referencedBehavior = new RightValueDef(new VariableDef(referencedBehaviorFileName));

            SetTask(referencedBehaviorFileName);
        }

        private void SetTask(string referencedBehaviorFileName)
        {
            string fullPath = FileManagers.FileManager.GetFullPath(referencedBehaviorFileName);

            BehaviorNode referencedBehavior = BehaviorManager.Instance.GetBehavior(fullPath);

            if (referencedBehavior != null)
            {
                //when expand planning process, don't update _task, as it will be set wrongly
                if (Plugin.EditMode == EditModes.Design)
                {
                    Behavior refTree_ = referencedBehavior as Behavior;

                    if (refTree_.Children.Count > 0 && refTree_.Children[0] is Task)
                    {
                        Task rootTask = refTree_.Children[0] as Task;

                        if (rootTask.Prototype != null)
                        {
                            this._task = (MethodDef)rootTask.Prototype.Clone();
                        }
                    }
                }
            }
        }

        private void SetReferenceBehavior(BehaviorNode referencedBehavior_)
        {
            string referencedBehaviorFileName = FileManagers.FileManager.GetRelativePath(referencedBehavior_.Filename);

            this.SetReferenceBehavior(referencedBehaviorFileName);
        }

        // when this node is saved, the children won't as they belong to another behaviour
        public override bool SaveChildren
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new referenced behaviour.
        /// </summary>
        /// <param name="rootBehavior">The behaviour this node belongs not. NOT the one is references.</param>
        /// <param name="referencedBehavior">The behaviour you want to reference.</param>
        public ReferencedBehavior(BehaviorNode rootBehavior, BehaviorNode referencedBehavior)
        : base(Resources.ReferencedBehavior, Resources.ReferencedBehaviorDesc)
        {
            this.SetReferenceBehavior(referencedBehavior);
        }

        /// <summary>
        /// Creates a new referenced behaviour. The behaviour which will be referenced is read from the Reference attribute.
        /// </summary>
        public ReferencedBehavior()
        : base(Resources.ReferencedBehavior, Resources.ReferencedBehaviorDesc)
        {
            _genericChildren = new ConnectorMultiple(_children, string.Empty, Connector.kGeneric, 1, int.MaxValue);

            _referencedBehavior = null;
        }

        public override string ExportClass
        {
            get
            {
                return "ReferencedBehavior";
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            Type valueType = (this._referencedBehavior != null) ? this._referencedBehavior.ValueType : null;

            if (valueType == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "ReferencedBehaviorPath is not set!"));
            }
            else
            {
                if (!Plugin.IsStringType(valueType))
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "ReferencedBehaviorPath should be a string type!"));
                }
                else
                {
                    if (this._referencedBehavior != null && this._referencedBehavior.Var != null &&
                        this._referencedBehavior.Var.IsConst)
                    {
                        string treePath = this._referencedBehavior.Var.Value.ToString();

                        if (string.IsNullOrEmpty(treePath))
                        {
                            result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "ReferencedBehaviorPath should not be empty!"));
                        }
                        else
                        {
                            //treePath might have not been loaded yet.
                            //BehaviorNode referencedTree = this.ReferencedTree;
                            string fullPath = FileManagers.FileManager.GetFullPath(treePath);

                            //to load it forcefully
                            BehaviorNode referencedTree = BehaviorManager.Instance.GetBehavior(fullPath);

                            if (referencedTree == null)
                            {
                                referencedTree = BehaviorManager.Instance.LoadBehavior(fullPath, true);
                            }

                            if (referencedTree == null)
                            {
                                string errMsg = string.Format("'{0}' is not a valid relative tree path!", treePath);
                                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, errMsg));
                            }
                            else
                            {
                                checkErrorForSubTree(rootBehavior, referencedTree, result);
                            }
                        }
                    }
                }
            }

            base.CheckForErrors(rootBehavior, result);
        }

        private void checkErrorForSubTree(BehaviorNode rootBehavior, BehaviorNode referencedBehavior, List<ErrorCheck> result)
        {
            if (referencedBehavior != null)
            {
                // if our referenced behaviour could be loaded, check it as well for errors
                Behavior b = referencedBehavior as Behavior;

                if (b != null && b.AgentType != null)
                {
                    Behavior rootB = rootBehavior as Behavior;
                    Debug.Check(rootB != null);

                    if (rootB != null && rootB.AgentType != null)
                    {
                        string childBTAgent = b.AgentType.ToString();
                        string rootBTAgent = rootB.AgentType.ToString();

                        //the agent type specified at root bt should be derived from the agent type at child bt
                        if (!Plugin.IsAgentDerived(rootBTAgent, childBTAgent) && !Plugin.IsAgentDerived(childBTAgent, rootBTAgent))
                        {
                            result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.AgentTypeError));
                        }
                    }
                }

                if (Plugin.EditMode == EditModes.Design)
                {
                    // circular reference
                    if (referencedBehavior != rootBehavior)
                    {
                        List<Node.ErrorCheck> childResult = new List<Node.ErrorCheck>();

                        if (referencedBehavior != null)
                        {
                            ((Node)referencedBehavior).CheckForErrors(rootBehavior, childResult);
                        }

                        if (childResult.Count > 0)
                        {
                            int start = 0;
                            string errorMsg = Resources.BehaviorIsEmptyError;

                            if (childResult[0].Level == ErrorCheckLevel.Error && childResult[0].Description.IndexOf(errorMsg) > 0)
                            {
                                start = 1;
                            }

                            bool bErrorFound = false;

                            for (int i = start; i < childResult.Count; ++i)
                            {
                                Node.ErrorCheck c = childResult[i];

                                if (c.Level == ErrorCheckLevel.Error)
                                {
                                    bErrorFound = true;
                                    break;
                                }
                            }

                            if (bErrorFound)
                            {
                                result.Add(new Node.ErrorCheck(this, bErrorFound ? ErrorCheckLevel.Error : ErrorCheckLevel.Warning, bErrorFound ? Resources.ReferenceError : Resources.ReferenceWarning));
                            }
                        }
                    }
                }

                if (this.Task != null)
                {
                    bool isParamCompleted = true;

                    foreach (MethodDef.Param param in this.Task.Params)
                    {
                        if (param.Value == null)
                        {
                            isParamCompleted = false;
                            break;
                        }
                    }

                    if (!isParamCompleted)
                    {
                        result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.NoParam));
                    }
                }
            }
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/subtree/";
            }
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            // This function should be empty here, so don't remove it.
            return false;
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(140, 170, 80));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        /// <summary>
        /// Creates a view for this node. Allows you to return your own class and store additional data.
        /// </summary>
        /// <param name="rootBehavior">The root of the graph of the current view.</param>
        /// <param name="parent">The parent of the NodeViewData created.</param>
        /// <returns>Returns a new NodeViewData object for this node.</returns>
        public override NodeViewData CreateNodeViewData(NodeViewData parent, BehaviorNode rootBehavior)
        {
            NodeViewData nvd = base.CreateNodeViewData(parent, rootBehavior);
            nvd.ChangeShape(NodeShape.Rectangle);

            return nvd;
        }

        /// <summary>
        /// Searches a list for NodeViewData for this node. Internal use only.
        /// </summary>
        /// <param name="list">The list which is searched for the NodeViewData.</param>
        /// <returns>Returns null if no fitting NodeViewData could be found.</returns>
        public override NodeViewData FindNodeViewData(List<NodeViewData> list)
        {
            foreach (NodeViewData nvd in list)
            {
                if (nvd.Node is ReferencedBehavior)
                {
                    ReferencedBehavior refnode = (ReferencedBehavior)nvd.Node;

                    // if both nodes reference the same behaviour we copy the view related data
                    if (_referencedBehavior != null && refnode.ReferenceBehaviorNode == _referencedBehavior)
                    {
                        NodeViewData nvdrb = (NodeViewData)nvd;
                        NodeViewData newdata = (NodeViewData)CreateNodeViewData(nvd.Parent, nvd.RootBehavior);

                        // copy data
                        newdata.IsExpanded = nvdrb.IsExpanded;

                        // return new data
                        return newdata;
                    }
                }

                if (nvd.Node == this)
                {
                    return nvd;
                }
            }

            return null;
        }

        private MethodDef _task = null;
        [DesignerMethodEnum("TaskPrototype", "TaskPrototypeDesc", "Task", DesignerProperty.DisplayMode.List, 1, DesignerProperty.DesignerFlags.ReadOnly, MethodType.Task)]
        public MethodDef Task
        {
            get
            {
                return _task;
            }
            set
            {
                if (value != null)
                {
                    //don't clear it when _task has been set by SetReferenceBehavior
                    this._task = value;
                }
            }
        }

        //public override string GenerateNewLabel()
        //{
        //    string newlabel = string.Empty;

        //    if (this._task != null)
        //    {
        //        //newlabel =  this._task.PrototypeName;
        //        newlabel = this._task.GetDisplayValue();
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(this.ReferencedTreePath))
        //        {
        //            string szTreeInfo = this.ReferencedTreePath;
        //            newlabel = string.Format("{0}(\"{1}\")", this.Label, szTreeInfo);
        //        }
        //        else
        //        {
        //            string szTreeInfo = this.ReferenceBehavior.GetDisplayValue();
        //            newlabel = string.Format("{0}({1})", this.Label, szTreeInfo);
        //        }
        //    }

        //    return newlabel;
        //}

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            ReferencedBehavior refbehav = (ReferencedBehavior)newnode;
            refbehav.Label = Label;

            if (this._referencedBehavior != null)
            {
                refbehav._referencedBehavior = (RightValueDef)_referencedBehavior.Clone();
            }

            if (this._task != null)
            {
                refbehav._task = (MethodDef)this._task.Clone();
            }
        }

        public override bool CanAdopt(BaseNode child)
        {
            return false;
        }
    }
}
