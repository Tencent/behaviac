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
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;

namespace Behaviac.Design.Attachments
{
    /// <summary>
    /// This class represents objects that can be attached to nodes like events or overrides.
    /// </summary>
    public abstract class Attachment : DefaultObject
    {
        /// <summary>
        /// Creates an attachment from a given type.
        /// </summary>
        /// <param name="type">The type we want to create an attachment of.</param>
        /// <param name="node">The node this will be added to.</param>
        /// <returns>Returns the created event.</returns>
        public static Attachment Create(Type type, Nodes.Node node)
        {
            Debug.Check(type != null);

            if (type != null)
            {
                Attachment atta = (Attachment)type.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[] { node });

                if (atta == null)
                {
                    throw new Exception(Resources.ExceptionMissingEventConstructor);
                }

                return atta;
            }

            return null;
        }

        public virtual string DocLink
        {
            get
            {
                return "http://www.behaviac.com/attachment/";
            }
        }

        public virtual bool CanBeDisabled()
        {
            return this.Enable ? false : true;
        }

        public bool CanBeAttached
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsFSM
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsStartCondition
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanBeDraggedToTarget
        {
            get
            {
                return false;
            }
        }

        private int _targetFSMNodeId = int.MinValue;
        public virtual int TargetFSMNodeId
        {
            get
            {
                return _targetFSMNodeId;
            }
            set
            {
                _targetFSMNodeId = value;
            }
        }

        public virtual bool IsPrecondition
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsEffector
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsTransition
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanBeDeleted
        {
            get
            {
                return true;
            }
        }

        protected Nodes.Node _node;

        /// <summary>
        /// The node we are attached to.
        /// </summary>
        public Nodes.Node Node
        {
            get
            {
                return _node;
            }
        }

        private string _label;
        private string _baselabel;
        public string Label
        {
            get
            {
                return _label;
            }
        }

        protected string _description;

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

        public Behaviac.Design.Nodes.BehaviorNode Behavior
        {
            get
            {
                return (Node != null) ? Node.Behavior : null;
            }
        }

        protected Attachment(Nodes.Node node, string label, string description)
        {
            _node = node;
            _label = label;
            _baselabel = label;
            _description = description;
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

        private int _id;
        [DesignerInteger("AttachmentId", "AttachmentIdDesc", "Debug", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.ReadOnly | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NotPrefabRelated, null, int.MinValue, int.MaxValue, 1, null)]
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
        /// The attachment id in the prefab behavior
        /// </summary>
        private int _prefabAttachmentId = -1;
        [DesignerInteger("PrefabAttachmentId", "PrefabAttachmentIdDesc", "Prefab", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.ReadOnly | DesignerProperty.DesignerFlags.NoExport | DesignerProperty.DesignerFlags.NotPrefabRelated | DesignerProperty.DesignerFlags.NoDisplay, null, int.MinValue, int.MaxValue, 1, null)]
        public int PrefabAttachmentId
        {
            get
            {
                return _prefabAttachmentId;
            }
            set
            {
                _prefabAttachmentId = value;
            }
        }

        public virtual List<ParInfo> LocalVars
        {
            get
            {
                return null;
            }
        }

        public void ResetId()
        {
            if (this.Id < 0 || null != Plugin.GetPreviousObjectById((Nodes.Node)Node.Behavior, this.Id, this))
            {
                this.Id = Plugin.NewNodeId((Nodes.Node)Node.Behavior);
            }
        }

        private void setLabel(string value, bool wasModified)
        {
            _label = value; //Resources.ResourceManager.GetString(value, Resources.Culture);

            // store the original label so we can automatically generate a new label when an ttribute changes.
            if (_baselabel == string.Empty)
            {
                _baselabel = _label;
            }

            // when the label changes the size of the node might change as well
            if (_node != null && wasModified && _label != _baselabel)
            {
                _node.Behavior.TriggerWasModified(_node);
            }
        }

        //a chance to modify the structure or data
        public virtual void PostCreate(List<Nodes.Node.ErrorCheck> result, int version, Nodes.Node node, System.Xml.XmlNode xmlNode)
        {
        }

        /// <summary>
        /// Is called when one of the event's proterties were modified.
        /// </summary>
        /// <param name="wasModified">Holds if the event was modified.</param>
        public void OnPropertyValueChanged(bool wasModified)
        {
            if (_node != null)
            {
                _node.OnPropertyValueChanged(wasModified);
            }

            setLabel(GenerateNewLabel(), wasModified);
        }

        public override string ToString()
        {
            return _label;
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

        public virtual Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.ObjectUIPolicy();
        }

        public virtual object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            return null;
        }

        public Attachment Clone(Nodes.Node newnode)
        {
            Attachment atta = Create(GetType(), newnode);

            CloneProperties(atta);

            atta.OnPropertyValueChanged(false);

            return atta;
        }

        protected virtual void CloneProperties(Attachment newattach)
        {
            newattach._id = this._id;
            newattach._enable = this._enable;
            newattach._prefabAttachmentId = this._prefabAttachmentId;
            //newattach._node = this._node; // The node should be the new one.
            newattach._label = this._label;
            newattach._baselabel = this._baselabel;
            newattach._description = this._description;
        }

        public virtual void CheckForErrors(Behaviac.Design.Nodes.BehaviorNode rootBehavior, List<Behaviac.Design.Nodes.Node.ErrorCheck> result)
        {
            if (!this.IsPrecondition && !this.IsEffector)
            {
                //flagStr = "event";
            }
            else if (this.IsPrecondition)
            {
                //flagStr = "precondition";
                if (this.IsEffector)
                {
                    result.Add(new Nodes.Node.ErrorCheck(this.Node, this.Id, this.Label, Nodes.ErrorCheckLevel.Error, "the attachment can only be a Precondition or an Effector!"));
                }

            }
            else if (this.IsEffector)
            {
                //flagStr = "effector";
                if (this.IsPrecondition)
                {
                    result.Add(new Nodes.Node.ErrorCheck(this.Node, this.Id, this.Label, Nodes.ErrorCheckLevel.Error, "the attachment can only be a Precondition or an Effector!"));
                }

            }
            else
            {
                Debug.Check(false);
            }
        }

        public virtual void GetReferencedFiles(ref List<string> referencedFiles)
        {
        }

        public virtual bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            return false;
        }

        /// <summary>
        /// Returns a list of all properties which have a designer attribute attached.
        /// </summary>
        /// <returns>A list of all properties relevant to the designer.</returns>
        public virtual IList<DesignerPropertyInfo> GetDesignerProperties(bool bCustom = false)
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

        /// <summary>
        /// Generates a new label by adding the attributes to the label as arguments
        /// </summary>
        /// <returns>Returns the label with a list of arguments.</returns>
        protected string GenerateNewLabel()
        {
            // generate the new label with the arguments
            return /*_baselabel + ": " +*/ this.GeneratePropertiesLabel();
        }

        protected virtual string GeneratePropertiesLabel()
        {
            string propertiesLabel = string.Empty;
            int paramCount = 0;

            // check all properties for one which must be shown as a parameter on the node
            IList<DesignerPropertyInfo> properties = GetDesignerProperties(DesignerProperty.SortByDisplayOrder);

            for (int p = 0; p < properties.Count; ++p)
            {
                // property must be shown as a parameter on the node
                if (properties[p].Attribute.Display == DesignerProperty.DisplayMode.Parameter)
                {
                    if (paramCount == 0)
                    {
                        propertiesLabel += "(";
                    }

                    else
                    {
                        propertiesLabel += ", ";
                    }

                    propertiesLabel += properties[p].GetDisplayValue(this);
                    paramCount++;
                }
            }

            if (paramCount > 0)
            {
                propertiesLabel += ")";
            }

            return propertiesLabel;
        }

        public abstract NodeViewData.SubItemAttachment CreateSubItem();
    }
}
