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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerPropertyEditor : UserControl
    {
        public DesignerPropertyEditor()
        {
            InitializeComponent();
        }

        protected object _object;
        public object SelectedObject
        {
            get
            {
                return _object;
            }
        }

        protected Nodes.Node _root;
        public void SetRootNode(Nodes.Node root)
        {
            _root = root;
        }

        protected Nodes.Behavior GetBehavior()
        {
            Attachments.Attach attach = _object as Attachments.Attach;
            Nodes.BaseNode baseNode = (attach != null) ? attach.Node : _object as Nodes.BaseNode;
            Nodes.Behavior behavior = (baseNode != null) ? baseNode.Behavior as Nodes.Behavior : null;

            if (behavior == null && _root != null)
            {
                behavior = _root.Behavior as Nodes.Behavior;
            }

            return behavior;
        }

        protected List<Plugin.InstanceName_t> InstanceNames
        {
            get
            {
                Nodes.Behavior behavior = GetBehavior();

                List<Plugin.InstanceName_t> instanceNames = new List<Plugin.InstanceName_t>();
                instanceNames.AddRange(Plugin.InstanceNames);

                if (behavior != null)
                {
                    instanceNames.AddRange(Plugin.GetLocalInstanceNames(behavior));
                }

                return instanceNames;
            }
        }

        protected bool CheckMethods(List<MethodDef> methods)
        {
            for (int i = 0; i < methods.Count - 1; ++i)
            {
                for (int k = i + 1; k < methods.Count; ++k)
                {
                    if (!string.IsNullOrEmpty(methods[i].DisplayName) && methods[i].DisplayName == methods[k].DisplayName)
                    {
                        string errorInfo = string.Format(Resources.MethodWithSameDisplayName, methods[i].BasicName, methods[k].BasicName, methods[i].DisplayName);
                        MessageBox.Show(errorInfo, Resources.Error);
                        return false;
                    }
                }
            }

            return true;
        }

        protected Type _filterType = null;
        public virtual Type FilterType
        {
            get
            {
                return _filterType;
            }
            set
            {
                _filterType = value;
            }
        }

        protected ValueTypes _valueType = ValueTypes.All;
        public ValueTypes ValueType
        {
            get
            {
                if (_valueType == ValueTypes.WaitType)
                {
                    return Workspace.Current.UseIntValue ? ValueTypes.Int : ValueTypes.Float;
                }

                return _valueType;
            }

            set
            {
                _valueType = value;
            }
        }

        public void SetupCastSettings(object obj)
        {
            if (obj != null && obj is Behaviac.Design.Nodes.Node)
            {
                Behaviac.Design.Nodes.Node assignNode = obj as Behaviac.Design.Nodes.Node;

                if (assignNode != null)
                {
                    bool bCasting = assignNode.IsCasting;

                    if (bCasting)
                    {
                        DesignerPropertyInfo leftPropInfo = DesignerProperty.GetDesignerProperty(assignNode.GetType(), "Opl");
                        VariableDef opl = (VariableDef)leftPropInfo.GetValue(assignNode);

                        Type leftType = opl.ValueType;

                        // if number
                        if (Plugin.IsIntergerNumberType(leftType) || Plugin.IsFloatType(leftType))
                        {
                            this.ValueType = ValueTypes.Int | ValueTypes.Float;

                            this.FilterType = null;
                        }
                        else if (Plugin.IsRefType(leftType))
                        {
                            //ref type/pointer type
                            this.ValueType = ValueTypes.RefType;

                            this.FilterType = leftType;
                        }
                        else
                        {
                            //
                        }
                    }
                }
            }
        }

        protected DesignerPropertyInfo _property;
        public virtual void SetProperty(DesignerPropertyInfo property, object obj)
        {
            _property = property;
            _object = obj;
        }
        public DesignerPropertyInfo GetProperty()
        {
            return _property;
        }

        protected DesignerArrayPropertyInfo _arrayProperty;
        public virtual void SetArrayProperty(DesignerArrayPropertyInfo arrayProperty, object obj)
        {
            _arrayProperty = arrayProperty;
            _object = obj;
        }

        protected DesignerStructPropertyInfo _structProperty;
        public virtual void SetStructProperty(DesignerStructPropertyInfo structProperty, object obj)
        {
            _structProperty = structProperty;
            _object = obj;
        }

        protected bool _bIsReadonly;
        protected MethodDef.Param _param;
        public virtual void SetParameter(MethodDef.Param param, object obj, bool bReadonly)
        {
            _param = param;
            _object = obj;
            _bIsReadonly = bReadonly;
        }

        protected VariableDef _variable;
        public virtual void SetVariable(VariableDef value, object obj)
        {
            _variable = value;
            _object = obj;
        }
        public VariableDef GetVariable()
        {
            return _variable;
        }

        protected ParInfo _par;
        public virtual void SetPar(ParInfo par, object obj)
        {
            _par = par;
            _object = obj;

            SetVariable(par.Variable, obj);
        }

        public Attributes.DesignerProperty Attribute
        {
            get
            {
                if (_param != null)
                {
                    return _param.Attribute;
                }

                return _property.Attribute;
            }
        }

        protected bool _valueWasAssigned = false;
        public void ValueWasAssigned()
        {
            _valueWasAssigned = true;
        }
        public void ValueWasnotAssigned()
        {
            _valueWasAssigned = false;
        }

        public virtual void ReadOnly()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void SetRange(double min, double max)
        {
        }

        public delegate void InvalidateProperty();
        public static event InvalidateProperty PropertyChanged;

        protected void RereshProperty(bool byForce, DesignerPropertyInfo property)
        {
            if (!byForce)
            {
                DesignerPropertyEnum enumAtt = property.Attribute as DesignerPropertyEnum;

                if (enumAtt != null && enumAtt.DependingProperty != "")
                {
                    byForce = true;
                }

                if (this._param != null && this._param.Type.FullName == "XMLPluginBehaviac.IList")
                {
                    byForce = true;
                }
            }

            if (byForce && DesignerPropertyEditor.PropertyChanged != null)
            {
                try
                {
                    //DesignerPropertyEditor.PropertyChanged();
                    this.BeginInvoke(new MethodInvoker(DesignerPropertyEditor.PropertyChanged));
                }
                catch
                {
                }
            }
        }

        public delegate void ValueChanged(object sender, DesignerPropertyInfo property);
        public event ValueChanged ValueWasChanged;
        protected void OnValueChanged(DesignerPropertyInfo property)
        {
            if (!_valueWasAssigned)
            {
                return;
            }

            Nodes.Node node = _object as Nodes.Node;

            if (node != null)
            {
                node.OnPropertyValueChanged(true);
                node.OnPropertyValueChanged(property);

            }
            else
            {
                Attachments.Attachment attach = _object as Attachments.Attachment;

                if (attach != null)
                {
                    attach.OnPropertyValueChanged(true);
                }
            }

            if (ValueWasChanged != null)
            {
                ValueWasChanged(this, property);
            }
        }

        public virtual string DisplayName
        {
            get
            {
                return (Attribute != null) ? Attribute.DisplayName : string.Empty;
            }
        }

        public virtual string Description
        {
            get
            {
                return (Attribute != null) ? Attribute.Description : string.Empty;
            }
        }

        public delegate void DescriptionChanged(string displayName, string description);
        public event DescriptionChanged DescriptionWasChanged;
        protected void OnDescriptionChanged(string displayName, string description)
        {
            if (DescriptionWasChanged != null)
            {
                DescriptionWasChanged(displayName, description);
            }
        }
    }
}
