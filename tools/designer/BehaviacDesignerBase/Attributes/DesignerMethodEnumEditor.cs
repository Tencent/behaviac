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
using System.Reflection;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerMethodEnumEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerMethodEnumEditor()
        {
            InitializeComponent();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            comboBox.Enabled = false;
        }

        private List<MethodDef> _methods = new List<MethodDef>();
        private bool _resetMethods = false;

        public override void Clear()
        {
            if (_property.Property != null)
            {
                _property.Property.SetValue(_object, null, null);
            }

            _resetMethods = false;
            _methods.Clear();
            comboBox.Text = "";
            comboBox.Items.Clear();
        }

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            _resetMethods = false;

            DesignerRightValueEnum enumAttRV = _property.Attribute as DesignerRightValueEnum;

            this.FilterType = null;

            if (enumAttRV != null && enumAttRV.DependedProperty != "")
            {
                Type objType = _object.GetType();
                PropertyInfo pi = objType.GetProperty(enumAttRV.DependedProperty);
                object propMember = pi.GetValue(_object, null);
                VariableDef var = propMember as VariableDef;

                if (var != null)
                {
                    this.FilterType = var.ValueType;

                }
                else
                {
                    MethodDef method = propMember as MethodDef;

                    if (method != null)
                    {
                        this.FilterType = method.ReturnType;

                    }
                    else
                    {
                        RightValueDef varRVp = propMember as RightValueDef;

                        if (varRVp != null)
                        {
                            this.FilterType = varRVp.ValueType;
                        }
                    }
                }

            }
            else
            {
                this.FilterType = _property.Attribute.FilterType;
            }

            SetupCastSettings(obj);

            setComboBox();
        }

        private List<MethodDef> getMethods()
        {
            List<MethodDef> methods = new List<MethodDef>();

            Nodes.Behavior behavior = GetBehavior();
            AgentType agentType = (behavior != null) ? behavior.AgentType : null;

            object action = _property.Property.GetValue(_object, null);
            VariableDef var = action as VariableDef;
            Debug.Check(var == null);

            RightValueDef varRV = action as RightValueDef;

            if (varRV != null && Plugin.IsInstanceName(varRV.ValueClassReal, behavior))
            {
                agentType = Plugin.GetInstanceAgentType(varRV.ValueClassReal, behavior, agentType);
            }

            if (agentType != null)
            {
                DesignerRightValueEnum enumAttRV = _property.Attribute as DesignerRightValueEnum;
                DesignerMethodEnum attrMethod = _property.Attribute as DesignerMethodEnum;
                MethodType methodType = attrMethod != null ? attrMethod.MethodType : MethodType.Getter;

                if (enumAttRV != null)
                {
                    methodType = enumAttRV.MethodType;
                }

                IList<MethodDef> actions = agentType.GetMethods(true, methodType);

                foreach (MethodDef actionType in actions)
                {
                    if (Plugin.IsCompatibleType(this.ValueType, this.FilterType, actionType.ReturnType, false))
                    {
                        methods.Add(actionType);
                    }
                }
            }

            return methods;
        }

        private void setComboBox()
        {
            object action = _property.Property.GetValue(_object, null);
            MethodDef actionObj = action as MethodDef;
            RightValueDef varRV = action as RightValueDef;
            string selectionName = string.Empty;

            if (actionObj != null)
            {
                selectionName = actionObj.DisplayName;

            }
            else if (varRV != null && varRV.Method != null)
            {
                selectionName = varRV.Method.DisplayName;
            }

            _methods = getMethods();
            comboBox.Items.Clear();

            foreach (MethodDef md in _methods)
            {
                if (md.DisplayName == selectionName)
                {
                    _methods.Clear();
                    _methods.Add(md);
                    comboBox.Items.Add(md.DisplayName);

                    break;
                }
            }

            comboBox.Text = selectionName;
        }

        private void resetMethods()
        {
            if (!_resetMethods)
            {
                _resetMethods = true;

                this.SetupCastSettings(this._object);

                _methods = getMethods();

                if (_methods.Count > 0 && CheckMethods(_methods))
                {
                    if (string.IsNullOrEmpty(comboBox.Text))
                    {
                        foreach (MethodDef md in _methods)
                        {
                            if (!comboBox.Items.Contains(md.DisplayName))
                            {
                                comboBox.Items.Add(md.DisplayName);
                            }
                        }

                    }
                    else
                    {
                        int index = -1;

                        for (int i = 0; i < _methods.Count; ++i)
                        {
                            if (comboBox.Text == _methods[i].DisplayName)
                            {
                                index = i;
                                break;
                            }
                        }

                        if (index > -1)
                        {
                            for (int i = index - 1; i >= 0; --i)
                            {
                                if (!comboBox.Items.Contains(_methods[i].DisplayName))
                                {
                                    comboBox.Items.Insert(0, _methods[i].DisplayName);
                                }
                            }

                            for (int i = index + 1; i < _methods.Count; ++i)
                            {
                                if (!comboBox.Items.Contains(_methods[i].DisplayName))
                                {
                                    comboBox.Items.Add(_methods[i].DisplayName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void comboBox_DropDown(object sender, EventArgs e)
        {
            resetMethods();
        }

        private void comboBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            resetMethods();

            comboBox.DroppedDown = false;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_valueWasAssigned || comboBox.SelectedIndex < 0 || comboBox.SelectedIndex >= _methods.Count)
            {
                return;
            }

            DesignerRightValueEnum propertRV = _property.Attribute as DesignerRightValueEnum;
            MethodDef m_ = _methods[comboBox.SelectedIndex] as MethodDef;
            MethodDef m = new MethodDef(m_);

            m.Owner = VariableDef.kSelf;

            if (propertRV == null)
            {
                _property.Property.SetValue(_object, m, null);

            }
            else
            {
                object propertyMember = _property.Property.GetValue(_object, null);

                if (propertyMember != null)
                {
                    RightValueDef oldvarRV = propertyMember as RightValueDef;
                    RightValueDef varRV = new RightValueDef(m, oldvarRV.ValueClass);
                    _property.Property.SetValue(_object, varRV, null);
                }
            }

            this.RereshProperty(true, _property);

            OnValueChanged(_property);
        }

        private void comboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _methods.Count || e.Index >= comboBox.Items.Count)
            {
                return;
            }

            e.DrawBackground();
            e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), e.Font, System.Drawing.Brushes.LightGray, e.Bounds);
            e.DrawFocusRectangle();

            MethodDef m = _methods[e.Index];
            this.OnDescriptionChanged(this.DisplayName, m.Description);
        }

        private void DesignerMethodEnumEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }
        }

        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
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

        private void comboBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                resetMethods();

                string dragItem = getMethodName((string)e.Data.GetData(DataFormats.Text));

                if (!string.IsNullOrEmpty(dragItem) && comboBox.Items.Contains(dragItem))
                {
                    e.Effect = DragDropEffects.Move;
                    return;
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void comboBox_DragDrop(object sender, DragEventArgs e)
        {
            comboBox.Text = getMethodName((string)e.Data.GetData(DataFormats.Text));
        }
    }
}
