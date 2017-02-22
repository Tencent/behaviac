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
using System.Reflection;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerPropertyEnumEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerPropertyEnumEditor()
        {
            InitializeComponent();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();
            comboBox.Enabled = false;
        }

        public override void Clear()
        {
            if (_property.Property != null)
            {
                _property.Property.SetValue(_object, null, null);
            }

            _resetProperties = false;
            _properties.Clear();
            comboBox.Text = "";
            comboBox.Items.Clear();
        }

        AgentType _agentType = null;
        string _valueOwner = VariableDef.kSelf;
        private List<PropertyDef> _properties = new List<PropertyDef>();
        private bool _resetProperties = false;

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            _resetProperties = false;

            Type enumtype = null;
            DesignerPropertyEnum enumAtt = property.Attribute as DesignerPropertyEnum;

            if (enumAtt != null)
            {
                enumtype = property.Property.PropertyType;
            }

            if (enumtype == null)
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, property.Property.Name));
            }

            Nodes.Behavior behavior = GetBehavior();
            _agentType = (behavior != null) ? behavior.AgentType : null;

            object propertyMember = property.Property.GetValue(obj, null);
            VariableDef variable = propertyMember as VariableDef;
            RightValueDef variableRV = propertyMember as RightValueDef;

            if (variable != null && variable.ValueClass != VariableDef.kSelf)
            {
                _valueOwner = variable.ValueClass;
                _agentType = Plugin.GetInstanceAgentType(_valueOwner, behavior, _agentType);
            }

            if (variableRV != null && variableRV.ValueClassReal != VariableDef.kSelf)
            {
                _valueOwner = variableRV.ValueClassReal;
                _agentType = Plugin.GetInstanceAgentType(_valueOwner, behavior, _agentType);
            }

            string selectionName = string.Empty;

            if (variable != null && variable.Property != null)
            {
                selectionName = variable.Property.DisplayName;

            }
            else if (variableRV != null && variableRV.Var != null && variableRV.Var.Property != null)
            {
                selectionName = variableRV.Var.Property.DisplayName;
            }

            this.FilterType = null;

            if (enumAtt != null)
            {
                if (enumAtt.DependedProperty != "")
                {
                    Type objType = _object.GetType();
                    PropertyInfo pi = objType.GetProperty(enumAtt.DependedProperty);
                    object propMember = pi.GetValue(obj, null);
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
                            RightValueDef varRV = propMember as RightValueDef;

                            if (varRV != null)
                            {
                                this.FilterType = varRV.ValueType;
                            }
                        }
                    }

                }
                else
                {
                    this.FilterType = enumAtt.FilterType;
                }
            }


            setComboBox(selectionName);

            //after the left is changed, the right might need to be invalidated
            if (this.comboBox.Text != selectionName)
            {
                property.Property.SetValue(_object, null, null);
            }
        }


        public override void SetParameter(MethodDef.Param param, object obj, bool bReadonly)
        {
            base.SetParameter(param, obj, bReadonly);

            _resetProperties = false;

            string selectionName = string.Empty;
            VariableDef variable = param.Value as VariableDef;

            if (variable != null)
            {
                _valueOwner = variable.ValueClass;
                selectionName = (variable.Property != null) ? variable.Property.DisplayName : variable.DisplayName;

            }
            else
            {
                RightValueDef variableRV = param.Value as RightValueDef;

                if (variableRV != null)
                {
                    _valueOwner = variableRV.ValueClassReal;
                    selectionName = variableRV.DisplayName;
                }
            }

            Nodes.Behavior behavior = GetBehavior();
            _agentType = (behavior != null) ? behavior.AgentType : null;

            if (_valueOwner != VariableDef.kSelf)
            {
                _agentType = Plugin.GetInstanceAgentType(_valueOwner, behavior, _agentType);
            }

            setComboBox(selectionName);
        }

        private List<PropertyDef> getProperties()
        {

            this.SetupCastSettings(this._object);

            List<PropertyDef> properties = new List<PropertyDef>();

            if (_agentType != null)
            {
                bool bNoRealyOnly = false;

                if (this._property.Attribute != null)
                {
                    bNoRealyOnly = this._property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoReadonly);
                }

                bool bIsArrayIndex = false;

                if (this._param != null)
                {
                    bNoRealyOnly = this._param.IsOut || this._param.IsRef;
                    bIsArrayIndex = this._param.IsArrayIndex;
                }

                bool bArrayOnly = ((this.ValueType & ValueTypes.Array) == ValueTypes.Array);

                foreach (PropertyDef p in _agentType.GetProperties(true))
                {
                    if (bIsArrayIndex && p.IsArrayElement)
                    {
                        //array index can only be a const or another property
                        continue;
                    }

                    if (bNoRealyOnly && p.IsReadonly)
                    {
                        continue;
                    }

                    if (_valueOwner != VariableDef.kSelf && p.IsPar)
                    {
                        continue;
                    }

                    Type listType = Plugin.GetType("XMLPluginBehaviac.IList");
                    bool isArrayType = Plugin.IsArrayType(this.FilterType) || this.FilterType == typeof(System.Collections.IList) || (listType != null && this.FilterType == listType);

                    if (Plugin.IsCompatibleType(this.ValueType, this.FilterType, p.Type, bArrayOnly) ||
                        (bArrayOnly && isArrayType && Plugin.IsArrayType(p.Type)))
                    {
                        bool isInt = Plugin.IsIntergerType(p.Type);
                        bool isFloat = Plugin.IsFloatType(p.Type);
                        bool isBool = Plugin.IsBooleanType(p.Type);
                        bool isString = Plugin.IsStringType(p.Type);
                        bool isRefType = Plugin.IsRefType(p.Type);
                        bool bOk = false;

                        if (bArrayOnly)
                        {
                            //bool isArray = Plugin.IsArrayType(p.Type) && !Plugin.IsArrayType(this.FilterType);
                            bool isArray = Plugin.IsArrayType(p.Type);
                            bOk = isArray;
                        }
                        else
                        {
                            bOk = (this.ValueType == ValueTypes.All) ||
                                  ((isBool && ((this.ValueType & ValueTypes.Bool) == ValueTypes.Bool))) ||
                                  ((isInt && ((this.ValueType & ValueTypes.Int) == ValueTypes.Int))) ||
                                  (isFloat && ((this.ValueType & ValueTypes.Float) == ValueTypes.Float)) ||
                                  (isString && ((this.ValueType & ValueTypes.String) == ValueTypes.String)) ||
                                  (isRefType && ((this.ValueType & ValueTypes.RefType) == ValueTypes.RefType));
                        }

                        if (bOk)
                        {
                            properties.Add(p);
                        }
                    }
                }
            }

            return properties;
        }

        private void setComboBox(string selectionName)
        {
            _properties = getProperties();
            comboBox.Items.Clear();

            foreach (PropertyDef p in _properties)
            {
                if (p.DisplayName == selectionName)
                {
                    _properties.Clear();
                    _properties.Add(p);
                    comboBox.Items.Add(p.DisplayName);

                    break;
                }
            }

            comboBox.Text = selectionName;
        }

        private void resetProperties()
        {
            if (!_resetProperties)
            {
                _resetProperties = true;

                this.SetupCastSettings(this._object);

                _properties = getProperties();

                if (_properties.Count > 0)
                {
                    if (string.IsNullOrEmpty(comboBox.Text))
                    {
                        foreach (PropertyDef p in _properties)
                        {
                            if (!comboBox.Items.Contains(p.DisplayName))
                            {
                                comboBox.Items.Add(p.DisplayName);
                            }
                        }

                    }
                    else
                    {
                        int index = -1;

                        for (int i = 0; i < _properties.Count; ++i)
                        {
                            if (comboBox.Text == _properties[i].DisplayName)
                            {
                                index = i;
                                break;
                            }
                        }

                        if (index > -1)
                        {
                            for (int i = index - 1; i >= 0; --i)
                            {
                                if (!comboBox.Items.Contains(_properties[i].DisplayName))
                                {
                                    comboBox.Items.Insert(0, _properties[i].DisplayName);
                                }
                            }

                            for (int i = index + 1; i < _properties.Count; ++i)
                            {
                                if (!comboBox.Items.Contains(_properties[i].DisplayName))
                                {
                                    comboBox.Items.Add(_properties[i].DisplayName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void comboBox_DropDown(object sender, EventArgs e)
        {
            resetProperties();
        }

        private void comboBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            resetProperties();

            comboBox.DroppedDown = false;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_valueWasAssigned || comboBox.SelectedIndex < 0 || comboBox.SelectedIndex >= _properties.Count)
            {
                return;
            }

            PropertyDef selectedProperty = _properties[comboBox.SelectedIndex];
            selectedProperty = selectedProperty.Clone();
            selectedProperty.Owner = _valueOwner;

            bool isArrayElement = selectedProperty.IsArrayElement;
            bool bForceFresh = false;

            if (_property.Property != null)
            {
                object propertyMember = _property.Property.GetValue(_object, null);
                VariableDef var = propertyMember as VariableDef;
                DesignerRightValueEnum rvPropertyEnum = _property.Attribute as DesignerRightValueEnum;

                if (rvPropertyEnum == null)
                {
                    if (var == null)
                    {
                        var = new VariableDef(selectedProperty, VariableDef.kSelf);
                    }

                    else
                    {
                        var.SetProperty(selectedProperty, var.ValueClass);
                    }

                    if (isArrayElement && var.ArrayIndexElement == null)
                    {
                        var.ArrayIndexElement = new MethodDef.Param("ArrayIndex", typeof(int), "int", "ArrayIndex", "ArrayIndex");
                        var.ArrayIndexElement.IsArrayIndex = true;
                        bForceFresh = true;

                    }
                    else if (!isArrayElement && var.ArrayIndexElement != null)
                    {
                        var.ArrayIndexElement = null;
                        bForceFresh = true;
                    }

                    _property.Property.SetValue(_object, var, null);

                }
                else if (propertyMember != null)
                {
                    RightValueDef varRV = propertyMember as RightValueDef;

                    if (varRV == null)
                    {
                        Debug.Check(false);
                        //varRV = new RightValueDef(selectedProperty, VariableDef.kSelf);

                    }
                    else
                    {
                        if (varRV.IsMethod)
                        {
                            Debug.Check(false);

                        }
                        else
                        {
                            if (varRV.Var != null)
                            {
                                varRV.Var.SetProperty(selectedProperty, varRV.ValueClassReal);

                            }
                            else
                            {
                                var = new VariableDef(selectedProperty, varRV.ValueClassReal);
                                varRV = new RightValueDef(var);
                            }
                        }
                    }

                    if (varRV != null && varRV.Var != null)
                    {
                        if (isArrayElement && varRV.Var.ArrayIndexElement == null)
                        {
                            varRV.Var.ArrayIndexElement = new MethodDef.Param("ArrayIndex", typeof(int), "int", "ArrayIndex", "ArrayIndex");
                            varRV.Var.ArrayIndexElement.IsArrayIndex = true;

                            bForceFresh = true;

                        }
                        else if (!isArrayElement && varRV.Var.ArrayIndexElement != null)
                        {
                            varRV.Var.ArrayIndexElement = null;
                            bForceFresh = true;
                        }
                    }

                    _property.Property.SetValue(_object, varRV, null);
                }

            }
            else if (_param != null)
            {
                string valueType = _valueOwner;
                bool bOldArrayElment = false;

                if (_param.Value != null && _param.Value is VariableDef)
                {
                    VariableDef paramV = _param.Value as VariableDef;

                    if (paramV.ArrayIndexElement != null)
                    {
                        bOldArrayElment = true;
                    }
                }

                VariableDef paramValue = new VariableDef(selectedProperty, valueType);
                _param.Value = paramValue;

                if (isArrayElement && paramValue.ArrayIndexElement == null)
                {
                    paramValue.ArrayIndexElement = new MethodDef.Param("ArrayIndex", typeof(int), "int", "ArrayIndex", "ArrayIndex");
                    paramValue.ArrayIndexElement.IsArrayIndex = true;

                    bForceFresh = true;

                }
                else if (!isArrayElement && bOldArrayElment)
                {
                    paramValue.ArrayIndexElement = null;
                    bForceFresh = true;
                }
            }

            this.RereshProperty(bForceFresh, _property);

            OnValueChanged(_property);
        }

        private void comboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _properties.Count || e.Index >= comboBox.Items.Count)
            {
                return;
            }

            e.DrawBackground();
            e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), e.Font, System.Drawing.Brushes.LightGray, e.Bounds);
            e.DrawFocusRectangle();

            PropertyDef p = _properties[e.Index];
            this.OnDescriptionChanged(this.DisplayName, p.Description);
        }

        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }
        }

        private string getPropertyName(string fullname)
        {
            if (!string.IsNullOrEmpty(fullname))
            {
                string[] names = fullname.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                return names[names.Length - 1];
            }

            return string.Empty;
        }

        private void comboBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                resetProperties();

                string dragItem = getPropertyName((string)e.Data.GetData(DataFormats.Text));

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
            comboBox.Text = getPropertyName((string)e.Data.GetData(DataFormats.Text));
        }
    }
}
