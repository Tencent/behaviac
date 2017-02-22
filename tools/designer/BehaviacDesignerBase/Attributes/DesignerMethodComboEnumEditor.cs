/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerMethodComboEnumEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        private AgentType _agentType; // Self agent
        private List<string> _types = new List<string>();
        private List<string> _names = new List<string>();
        private List<string> _currentNames = new List<string>();
        private List<MethodDef> _methods = new List<MethodDef>();
        private bool _resetMethods = false;

        public DesignerMethodComboEnumEditor()
        {
            InitializeComponent();

            SetTypes();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            this.typeComboBox.Enabled = false;
            this.valueComboBox.Enabled = false;
        }

        public override void Clear()
        {
            this.valueComboBox.SelectedIndex = -1;
        }

        private void SetTypes()
        {
            _names.Clear();
            _types.Clear();

            _names.Add(VariableDef.kSelf);
            _types.Add(VariableDef.kSelf + "::Method");

            foreach (Plugin.InstanceName_t instanceName in this.InstanceNames)
            {
                _names.Add(instanceName.Name);
                _types.Add(instanceName.DisplayName + "::Method");
            }
        }

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            _resetMethods = false;

            DesignerMethodEnum enumAtt = property.Attribute as DesignerMethodEnum;

            if (enumAtt != null && property.Property.PropertyType == null)
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, property.Property.Name));
            }

            Nodes.Behavior behavior = GetBehavior();
            _agentType = (behavior != null) ? behavior.AgentType : null;

            SetTypes();

            object action = property.Property.GetValue(obj, null);
            MethodDef method = action as MethodDef;
            int typeIndex = -1;

            if (method != null)
            {
                typeIndex = getTypeIndex(method.Owner);
            }

            if (typeIndex < 0)
            {
                typeIndex = 0;
            }

            // Keep only one type for efficiency.
            _currentNames.Clear();
            _currentNames.Add(_names[typeIndex]);

            this.typeComboBox.Items.Clear();
            this.typeComboBox.Items.Add(_types[typeIndex]);
            this.typeComboBox.SelectedIndex = 0;
        }

        private int getTypeIndex(string owner)
        {
            for (int i = 0; i < _names.Count; ++i)
            {
                if (owner == _names[i])
                {
                    return i;
                }
            }

            return -1;
        }

        private void typeComboBox_DropDown(object sender, EventArgs e)
        {
            _currentNames = _names;

            if (string.IsNullOrEmpty(typeComboBox.Text))
            {
                foreach (string t in _types)
                {
                    if (!typeComboBox.Items.Contains(t))
                    {
                        typeComboBox.Items.Add(t);
                    }
                }

            }
            else
            {
                int index = -1;

                for (int i = 0; i < _types.Count; ++i)
                {
                    if (typeComboBox.Text == _types[i])
                    {
                        index = i;
                        break;
                    }
                }

                if (index > -1)
                {
                    for (int i = index - 1; i >= 0; --i)
                    {
                        if (!typeComboBox.Items.Contains(_types[i]))
                        {
                            typeComboBox.Items.Insert(0, _types[i]);
                        }
                    }

                    for (int i = index + 1; i < _types.Count; ++i)
                    {
                        if (!typeComboBox.Items.Contains(_types[i]))
                        {
                            typeComboBox.Items.Add(_types[i]);
                        }
                    }
                }
            }
        }

        private List<MethodDef> getMethods()
        {
            List<MethodDef> methods = new List<MethodDef>();

            if (typeComboBox.SelectedIndex > -1)
            {
                Nodes.Behavior behavior = GetBehavior();
                AgentType agentType = Plugin.GetInstanceAgentType(_currentNames[typeComboBox.SelectedIndex], behavior, _agentType);

                if (agentType != null)
                {
                    // get the linked method to filter
                    MethodDef linkedMethod = null;
                    bool linkBroken;
                    DesignerPropertyInfo linkedProp = _property.Attribute.GetLinkedProperty(_object, out linkBroken);
                    object prop = linkedProp.GetValue(_object);

                    if (prop != null && prop is MethodDef)
                    {
                        linkedMethod = prop as MethodDef;
                    }

                    DesignerMethodEnum attrMethod = _property.Attribute as DesignerMethodEnum;
#if USE_NOOP
                    IList<MethodDef> methods = new List<MethodDef>();
                    methods.Add(MethodDef.Noop);

                    IList<MethodDef> agentMethods = agentType.GetMethods(attrMethod.MethodType, ValueTypes.All, linkedMethod);

                    foreach (MethodDef m in agentMethods)
                    {
                        methods.Add(m);
                    }

#else

                    if (attrMethod != null)
                    {
                        methods.AddRange(agentType.GetMethods(true, attrMethod.MethodType, attrMethod.MethodReturnType, linkedMethod));

                    }
                    else
                    {
                        DesignerRightValueEnum attrMethodRV = _property.Attribute as DesignerRightValueEnum;

                        if (attrMethodRV != null)
                        {
                            methods.AddRange(agentType.GetMethods(true, attrMethodRV.MethodType, ValueTypes.All, linkedMethod));
                        }
                    }

#endif//#if USE_NOOP
                }
            }

            return methods;
        }

        private void setValueComboBox()
        {
            if (_methods.Count == 0 || !CheckMethods(_methods))
            {
                return;
            }

            if (string.IsNullOrEmpty(valueComboBox.Text))
            {
                foreach (MethodDef md in _methods)
                {
                    if (!valueComboBox.Items.Contains(md.DisplayName))
                    {
                        valueComboBox.Items.Add(md.DisplayName);
                    }
                }

            }
            else
            {
                int index = -1;

                for (int i = 0; i < _methods.Count; ++i)
                {
                    if (valueComboBox.Text == _methods[i].DisplayName)
                    {
                        index = i;
                        break;
                    }
                }

                if (index > -1)
                {
                    for (int i = index - 1; i >= 0; --i)
                    {
                        if (!valueComboBox.Items.Contains(_methods[i].DisplayName))
                        {
                            valueComboBox.Items.Insert(0, _methods[i].DisplayName);
                        }
                    }

                    for (int i = index + 1; i < _methods.Count; ++i)
                    {
                        if (!valueComboBox.Items.Contains(_methods[i].DisplayName))
                        {
                            valueComboBox.Items.Add(_methods[i].DisplayName);
                        }
                    }
                }
            }

            object action = _property.Property.GetValue(_object, null);
            DesignerMethodEnum attrMethod = _property.Attribute as DesignerMethodEnum;

            if (attrMethod != null)
            {
                MethodDef method = action as MethodDef;

                if (method != null && this.valueComboBox.Text != method.DisplayName)
                {
                    this.valueComboBox.Text = method.DisplayName;
                }

            }
            else
            {
                DesignerRightValueEnum attrMethodRV = _property.Attribute as DesignerRightValueEnum;

                if (attrMethodRV != null)
                {
                    RightValueDef method = action as RightValueDef;

                    if (method != null && this.valueComboBox.Text != method.Method.DisplayName)
                    {
                        Debug.Check(method.IsMethod);
                        this.valueComboBox.Text = method.Method.DisplayName;
                    }
                }
            }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            object action = _property.Property.GetValue(_object, null);
            MethodDef method = action as MethodDef;

            if (method != null)
            {
                _methods = getMethods();

                foreach (MethodDef md in _methods)
                {
                    if (md.Name == method.Name)
                    {
                        // Keep only one method for efficiency.
                        _methods.Clear();
                        _methods.Add(md);
                        break;
                    }
                }

            }
            else
            {
                _methods.Clear();
            }

            valueComboBox.Text = "";
            valueComboBox.Items.Clear();

            setValueComboBox();
        }

        private void resetMethods()
        {
            if (!_resetMethods)
            {
                _resetMethods = true;

                _methods = getMethods();

                setValueComboBox();
            }
        }

        private void valueComboBox_DropDown(object sender, EventArgs e)
        {
            resetMethods();
        }

        private void valueComboBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            resetMethods();

            valueComboBox.DroppedDown = false;
        }

        private void valueComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_valueWasAssigned ||
                typeComboBox.SelectedIndex < 0 || typeComboBox.SelectedIndex >= _currentNames.Count ||
                valueComboBox.SelectedIndex < 0 || valueComboBox.SelectedIndex >= _methods.Count)
            {
                return;
            }

            MethodDef m_ = _methods[valueComboBox.SelectedIndex] as MethodDef;
            MethodDef m = new MethodDef(m_);
            m.Owner = _currentNames[typeComboBox.SelectedIndex];

            DesignerMethodEnum attrMethod = _property.Attribute as DesignerMethodEnum;

            if (attrMethod != null)
            {
                if ((attrMethod.MethodType & MethodType.AllowNullMethod) == MethodType.AllowNullMethod &&
                    attrMethod.Display == DesignerProperty.DisplayMode.List)
                {
                    object oldValue = _property.Property.GetValue(_object, null);

                    if (oldValue == null)
                    {
                        Nodes.Node n = this._object as Nodes.Node;

                        if (n != null)
                        {
                            //n.AddSubItem(new NodeViewData.SubItemProperty(n, _property, attrMethod));
                            n.DoSubItemAdded(_property);
                        }
                    }
                }

                _property.Property.SetValue(_object, m, null);

            }
            else
            {
                object propertyMember = _property.Property.GetValue(_object, null);
                RightValueDef oldvarRV = propertyMember as RightValueDef;

                RightValueDef v = new RightValueDef(m, oldvarRV.ValueClass);

                _property.Property.SetValue(_object, m, null);
            }

            this.RereshProperty(true, _property);

            OnValueChanged(_property);
        }

        private void flowLayoutPanel_Resize(object sender, EventArgs e)
        {
            valueComboBox.Width = flowLayoutPanel.Width - typeComboBox.Width - 6;
        }

        private void typeComboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);

            this.OnDescriptionChanged(this.DisplayName, typeComboBox.SelectedItem != null ? typeComboBox.SelectedItem.ToString() : string.Empty);
        }

        private void valueComboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);

            if (valueComboBox.SelectedIndex >= 0 && valueComboBox.SelectedIndex < _methods.Count)
            {
                MethodDef m = _methods[valueComboBox.SelectedIndex];
                this.OnDescriptionChanged(this.DisplayName, m.Description);
            }
        }

        private void valueComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _methods.Count || e.Index >= valueComboBox.Items.Count)
            {
                return;
            }

            e.DrawBackground();
            e.Graphics.DrawString(valueComboBox.Items[e.Index].ToString(), e.Font, System.Drawing.Brushes.LightGray, e.Bounds);
            e.DrawFocusRectangle();

            MethodDef m = _methods[e.Index];
            this.OnDescriptionChanged(this.DisplayName, m.Description);
        }

        private void valueComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.Tab)
            {
                e.Handled = true;
            }
        }

        private string getMethodName(string fullname)
        {
            string methodName = string.Empty;

            if (!string.IsNullOrEmpty(fullname))
            {
                string[] names = fullname.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                methodName = names[names.Length - 1];
                methodName = methodName.Replace("()", "");
                methodName = methodName.Trim();
            }

            return methodName;
        }

        private void valueComboBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                resetMethods();

                string dragItem = getMethodName((string)e.Data.GetData(DataFormats.Text));

                if (!string.IsNullOrEmpty(dragItem) && valueComboBox.Items.Contains(dragItem))
                {
                    e.Effect = DragDropEffects.Move;
                    return;
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void valueComboBox_DragDrop(object sender, DragEventArgs e)
        {
            valueComboBox.Text = getMethodName((string)e.Data.GetData(DataFormats.Text));
        }
    }
}
