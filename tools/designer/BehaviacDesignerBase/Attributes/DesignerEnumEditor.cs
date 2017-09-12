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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerEnumEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerEnumEditor()
        {
            InitializeComponent();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            comboBox.Enabled = false;
        }

        private List<object> _values = new List<object>();
        private List<object> _allValues = new List<object>();

        private void clear()
        {
            _allValues.Clear();
            _values.Clear();
            comboBox.Items.Clear();
        }

        private void filterEnums(DesignerEnum enumAtt, string enumName, Type enumtype)
        {
            UIObject uiObject = _object as UIObject;

            object[] excludedElements = null;

            if (uiObject != null)
            {
                excludedElements = uiObject.GetExcludedEnums(enumAtt);
            }

            Array list = Enum.GetValues(enumtype);

            foreach (object enumVal in list)
            {
                bool excluded = false;

                if (excludedElements != null)
                {
                    for (int i = 0; i < excludedElements.Length; ++i)
                    {
                        if (excludedElements[i].Equals(enumVal))
                        {
                            excluded = true;
                            break;
                        }
                    }
                }

                if (!excluded)
                {
                    _allValues.Add(enumVal);

                    if (DesignerEnum.GetDisplayName(enumVal) == enumName)
                    {
                        _values.Add(enumVal);
                        comboBox.Items.Add(enumName);
                    }
                }
            }

            comboBox.Text = enumName;
        }

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            string enumName = string.Empty;
            Type enumtype = null;

            DesignerEnum enumAtt = property.Attribute as DesignerEnum;

            if (enumAtt != null)
            {
                enumName = DesignerEnum.GetDisplayName(property.Property.GetValue(obj, null));
                enumtype = property.Property.PropertyType;
            }

            if (enumtype == null)
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, property.Property.Name));
            }

            clear();

            filterEnums(enumAtt, enumName, enumtype);
        }

        public override void SetArrayProperty(DesignerArrayPropertyInfo arrayProperty, object obj)
        {
            base.SetArrayProperty(arrayProperty, obj);

            clear();

            Array list = Enum.GetValues(arrayProperty.ItemType);
            string enumName = DesignerEnum.GetDisplayName(arrayProperty.Value);

            foreach (object enumVal in list)
            {
                _allValues.Add(enumVal);

                if (DesignerEnum.GetDisplayName(enumVal) == enumName)
                {
                    _values.Add(enumVal);
                    comboBox.Items.Add(enumName);
                }
            }

            comboBox.Text = enumName;
        }

        public override void SetParameter(MethodDef.Param param, object obj, bool bReadonly)
        {
            base.SetParameter(param, obj, bReadonly);

            string enumName = string.Empty;
            Type enumtype = null;

            DesignerEnum enumAtt = param.Attribute as DesignerEnum;

            if (enumAtt != null)
            {
                enumName = DesignerEnum.GetDisplayName(param.Value);
                enumtype = param.Value.GetType();
            }

            if (enumtype == null)
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, param.Attribute.DisplayName));
            }

            clear();

            filterEnums(enumAtt, enumName, enumtype);
        }

        public override void SetVariable(VariableDef variable, object obj)
        {
            base.SetVariable(variable, obj);

            _valueWasAssigned = false;

            clear();

            if (variable != null)
            {
                Type enumtype = variable.ValueType;

                if (enumtype.IsEnum)
                {
                    string enumName = DesignerEnum.GetDisplayName(variable.Value);

                    filterEnums(null, enumName, enumtype);
                }
            }

            _valueWasAssigned = true;
        }

        private void comboBox_DropDown(object sender, EventArgs e)
        {
            if (_allValues.Count > 0 && _values.Count != _allValues.Count)
            {
                _values = _allValues;

                if (string.IsNullOrEmpty(comboBox.Text))
                {
                    foreach (object v in _values)
                    {
                        string enumName = DesignerEnum.GetDisplayName(v);

                        if (!comboBox.Items.Contains(enumName))
                        {
                            comboBox.Items.Add(enumName);
                        }
                    }

                }
                else
                {
                    int index = -1;

                    for (int i = 0; i < _values.Count; ++i)
                    {
                        string enumName = DesignerEnum.GetDisplayName(_values[i]);

                        if (comboBox.Text == enumName)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index > -1)
                    {
                        for (int i = index - 1; i >= 0; --i)
                        {
                            string enumName = DesignerEnum.GetDisplayName(_values[i]);

                            if (!comboBox.Items.Contains(enumName))
                            {
                                comboBox.Items.Insert(0, enumName);
                            }
                        }

                        for (int i = index + 1; i < _values.Count; ++i)
                        {
                            string enumName = DesignerEnum.GetDisplayName(_values[i]);

                            if (!comboBox.Items.Contains(enumName))
                            {
                                comboBox.Items.Add(enumName);
                            }
                        }
                    }
                }
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex < 0 || !_valueWasAssigned)
            {
                return;
            }

            if (_property.Property != null)
            {
                _property.Property.SetValue(_object, _values[comboBox.SelectedIndex], null);

            }
            else if (_arrayProperty != null)
            {
                _arrayProperty.Value = _values[comboBox.SelectedIndex];

            }
            else if (_param != null)
            {
                Debug.Check(_param.Attribute is DesignerEnum);
                _param.Value = _values[comboBox.SelectedIndex];

            }
            else if (_variable != null)
            {
                _variable.Value = _values[comboBox.SelectedIndex];

            }
            else
            {
                Debug.Check(false);
            }

            OnValueChanged(_property);
        }

        private void comboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            e.DrawBackground();
            e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), e.Font, System.Drawing.Brushes.LightGray, e.Bounds);
            e.DrawFocusRectangle();

            this.OnDescriptionChanged(this.DisplayName, DesignerEnum.GetDescription(_values[e.Index]));
        }

        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }
        }
    }
}
