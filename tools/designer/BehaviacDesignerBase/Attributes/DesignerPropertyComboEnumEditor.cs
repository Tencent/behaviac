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
    public partial class DesignerPropertyComboEnumEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerPropertyComboEnumEditor()
        {
            InitializeComponent();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            typeComboBox.Enabled = false;

            if (this.propertyEditor != null)
            {
                this.propertyEditor.ReadOnly();
            }
        }

        public override void Clear()
        {
            if (this.propertyEditor != null)
            {
                this.propertyEditor.Clear();
            }
        }

        public override Type FilterType
        {
            set
            {
                this._filterType = value;

                if (this.propertyEditor != null)
                {
                    this.propertyEditor.FilterType = this._filterType;
                }
            }
        }

        private bool _methodOnly = false;
        private bool _allowConst = true;
        private List<string> _names = new List<string>();
        private List<string> _types = new List<string>();
        private List<string> _currentNames = new List<string>();
        private bool _isReady = false;

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            _isReady = false;

            DesignerPropertyEnum enumAtt = property.Attribute as DesignerPropertyEnum;
            DesignerRightValueEnum enumAttRV = property.Attribute as DesignerRightValueEnum;

            _methodOnly = true;

            _names.Clear();
            _types.Clear();

            int defaultSelect = 0;

            if (enumAtt != null && enumAtt.HasStyles(DesignerPropertyEnum.AllowStyles.Const))
            {
                _methodOnly = false;
                _allowConst = true;

                _names.Add(VariableDef.kConst);
                _types.Add(VariableDef.kConst);

            }
            else
            {
                _allowConst = false;
            }

            if (enumAtt != null && enumAtt.HasStyles(DesignerPropertyEnum.AllowStyles.Self))
            {
                _methodOnly = false;

                _names.Add(VariableDef.kSelf);
                _types.Add(VariableDef.kSelf);
                defaultSelect = _types.Count - 1;
            }

            List<Plugin.InstanceName_t> instanceNames = this.InstanceNames;

            if (enumAtt != null && enumAtt.HasStyles(DesignerPropertyEnum.AllowStyles.Instance))
            {
                _methodOnly = false;

                foreach (Plugin.InstanceName_t instanceName in instanceNames)
                {
                    _names.Add(instanceName.Name);
                    _types.Add(instanceName.DisplayName);
                }
            }

            if (enumAtt != null && enumAtt.HasStyles(DesignerPropertyEnum.AllowStyles.SelfMethod))
            {
                _names.Add(VariableDef.kSelfMethod);
                _types.Add(VariableDef.kSelfMethod);

                if (enumAtt.HasStyles(DesignerPropertyEnum.AllowStyles.Instance))
                {
                    foreach (Plugin.InstanceName_t instanceName in instanceNames)
                    {
                        _names.Add(instanceName.Name + VariableDef.kMethod);
                        _types.Add(instanceName.DisplayName + VariableDef.kMethod);
                    }
                }
            }

            typeComboBox.Enabled = (_types.Count > 1);

            if (property.Property.PropertyType == null)
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, property.Property.Name));
            }

            object propertyMember = property.Property.GetValue(obj, null);
            VariableDef variable = propertyMember as VariableDef;
            RightValueDef variableRV = propertyMember as RightValueDef;

            //right value's default should be const
            if (enumAtt != null && enumAtt.DependedProperty != null && enumAtt.HasStyles(DesignerPropertyEnum.AllowStyles.Const))
            {
                defaultSelect = 0;
            }

            int typeIndex = -1;

            if (variableRV == null)
            {
                if (variable != null)
                {
                    typeIndex = getComboIndex(variable.ValueClass);

                }
                else
                {
                    typeIndex = defaultSelect;
                }

            }
            else
            {
                if (!string.IsNullOrEmpty(variableRV.ValueClass))
                {
                    typeIndex = getComboIndex(variableRV.ValueClass);

                }
                else
                {
                    typeIndex = 0;
                }
            }

            if (_types.Count > 0 && typeIndex > -1)
            {
                // Keep only one type for efficiency.
                _currentNames.Clear();
                _currentNames.Add(_names[typeIndex]);

                this.typeComboBox.Items.Clear();
                this.typeComboBox.Items.Add(_types[typeIndex]);
                this.typeComboBox.SelectedIndex = 0;
            }

            _isReady = true;

            string selectedText = ((string)typeComboBox.SelectedItem);
            setPropertyEditor(CreateEditor(selectedText, _property, _object,
                                           _allowConst && selectedText == VariableDef.kConst,
                                           variableRV != null ? variableRV.IsMethod : false));

            this.ValueWasAssigned();
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

        private void setPropertyEditor(DesignerPropertyEditor editor)
        {
            this.propertyEditor = editor;

            if (this.propertyEditor != null)
            {
                this.propertyEditor.MouseEnter += typeComboBox_MouseEnter;
                this.propertyEditor.DescriptionWasChanged += propertyEditor_DescriptionWasChanged;
            }
        }

        private int getComboIndex(string valueType)
        {
            Nodes.Behavior behavior = GetBehavior();

            if (_methodOnly)
            {
                if (valueType == VariableDef.kSelfMethod)
                {
                    //self::method
                    return 0;

                }
                else
                {
                    int pos = valueType.IndexOf(VariableDef.kMethod);

                    if (pos != -1)
                    {
                        //self::method world::method player::method
                        string classType = valueType.Substring(0, pos);
                        return Plugin.InstanceNameIndex(classType, behavior) + 1;
                    }
                }
            }

            List<Plugin.InstanceName_t> instanceNames = this.InstanceNames;

            int indexBegin = _allowConst ? 1 : 0;
            int indexOffset = 1;

            if (valueType == VariableDef.kConst)
            {
                return 0;

            }
            else if (valueType == VariableDef.kSelf)
            {
                //[const] [par] self world player self::method
                return indexBegin;

            }
            else if (valueType == VariableDef.kSelfMethod)
            {
                //[const] [par] self world player self::method
                return indexBegin + indexOffset + instanceNames.Count;

            }
            else
            {
                int pos = valueType.IndexOf(VariableDef.kMethod);

                if (pos != -1)
                {
                    //[const] [par] self world player self::method world::method player::method
                    string instanceName = valueType.Substring(0, pos);
                    int index = Plugin.InstanceNameIndex(instanceName, behavior);
                    return index + indexBegin + indexOffset + instanceNames.Count + 1;

                }
                else
                {
                    //[const] [par] self world player
                    int index = Plugin.InstanceNameIndex(valueType, behavior);
                    return index + indexBegin + indexOffset;
                }
            }
        }

        private PropertyInfo getDependedProperty(DesignerPropertyInfo prop, object obj)
        {
            Debug.Check(obj != null);

            if (obj != null)
            {
                DesignerPropertyEnum propertyAttr = prop.Attribute as DesignerPropertyEnum;

                if (propertyAttr != null && !string.IsNullOrEmpty(propertyAttr.DependedProperty))
                {
                    return obj.GetType().GetProperty(propertyAttr.DependedProperty);
                }
            }

            return null;
        }

        private Type getDependedPropertyType(DesignerPropertyInfo prop, object obj)
        {
            Debug.Check(obj != null);

            if (obj != null)
            {
                PropertyInfo pi = getDependedProperty(prop, obj);

                if (pi != null)
                {
                    object propertyMember = pi.GetValue(obj, null);

                    VariableDef variable = propertyMember as VariableDef;

                    if (variable != null)
                    {
                        return variable.ValueType;
                    }

                    RightValueDef varRV = propertyMember as RightValueDef;

                    if (varRV != null)
                    {
                        return varRV.ValueType;
                    }

                    MethodDef method = propertyMember as MethodDef;

                    if (method != null)
                    {
                        return method.ReturnType;
                    }
                }
            }

            return null;
        }

        private Type getPropertyType(DesignerPropertyInfo prop, object obj, string valueClass)
        {
            if (prop.Property != null && obj != null)
            {
                object propertyMember = prop.Property.GetValue(obj, null);

                if (propertyMember != null)
                {
                    VariableDef variable = propertyMember as VariableDef;

                    if (variable != null)
                    {
                        variable.ValueClass = valueClass;
                        return variable.ValueType;
                    }

                    RightValueDef varRV = propertyMember as RightValueDef;

                    if (varRV != null)
                    {
                        return varRV.ValueType;
                    }
                }
            }

            if (prop.Attribute != null)
            {
                DesignerPropertyEnum enumAtt = prop.Attribute as DesignerPropertyEnum;

                if (enumAtt != null)
                {
                    return enumAtt.FilterType;
                }
            }

            return null;
        }

        private Type GetEditorType(DesignerPropertyInfo prop, object obj, bool isConst, bool isFunction)
        {
            if (isConst)
            {
                Type type = getDependedPropertyType(prop, obj);

                if (type == null)
                {
                    type = getPropertyType(prop, obj, VariableDef.kConst);
                }

                if (type != null)
                {
                    return Plugin.InvokeEditorType(type);
                }

            }
            else if (isFunction)
            {
                return typeof(DesignerMethodEnumEditor);
            }
            else
            {
                return typeof(DesignerPropertyEnumEditor);
            }

            return null;
        }

        private void setEditor(string valueType, DesignerPropertyEditor editor, DesignerPropertyInfo prop, object obj, bool isConst, bool isFunction)
        {
            if (editor != null)
            {
                editor.SetRootNode(this._root);

                object propertyMember = prop.Property.GetValue(obj, null);
                VariableDef var = propertyMember as VariableDef;
                RightValueDef varRV = propertyMember as RightValueDef;

                DesignerPropertyEnum enumAtt = prop.Attribute as DesignerPropertyEnum;
                DesignerRightValueEnum enumAttRV = prop.Attribute as DesignerRightValueEnum;

                if (isConst)
                {
                    bool bHasDepend = false;
                    Type dependVarType = getDependedPropertyType(prop, obj);

                    if (dependVarType == null)
                    {
                        dependVarType = getPropertyType(prop, obj, VariableDef.kConst);

                    }
                    else
                    {
                        bHasDepend = true;
                    }

                    Debug.Check(dependVarType != null);

                    object defaultValue = Plugin.DefaultValue(dependVarType);
                    Debug.Check(defaultValue != null);

                    if (defaultValue != null)
                    {
                        //for a const bool, to use true as the default when it is the right operand
                        if (bHasDepend && (defaultValue is bool))
                        {
                            defaultValue = true;
                        }

                        if (var == null)
                        {
                            var = new VariableDef(defaultValue);
                        }

                        if (var.Value == null || var.Value.GetType() != defaultValue.GetType())
                        {
                            var.Value = defaultValue;
                        }
                    }

                    var.ValueClass = VariableDef.kConst;

                    if (enumAttRV == null)
                    {
                        prop.Property.SetValue(obj, var, null);
                        editor.SetVariable(var, obj);

                        if (enumAtt != null)
                        {
                            editor.SetRange(enumAtt.MinValue, enumAtt.MaxValue);
                        }

                    }
                    else
                    {
                        if (varRV == null || varRV.Var == null || varRV.ValueClass != var.ValueClass || varRV.ValueType != var.ValueType)
                        {
                            varRV = new RightValueDef(var);
                        }

                        prop.Property.SetValue(obj, varRV, null);
                        editor.SetVariable(varRV.Var, obj);
                    }
                }
                else
                {
                    // VariableDef
                    if (enumAttRV == null)
                    {
                        if (var == null)
                        {
                            var = new VariableDef(null);
                            prop.Property.SetValue(obj, var, null);
                        }

                        var.ValueClass = valueType;
                    }

                    // RightValueDef
                    else
                    {
                        if (varRV == null)
                        {
                            varRV = new RightValueDef(var);
                            prop.Property.SetValue(obj, varRV, null);
                        }

                        varRV.ValueClass = valueType;
                    }

                    editor.ValueType = prop.Attribute.ValueType;
                    editor.SetProperty(prop, obj);
                }

                editor.ValueWasAssigned();
                OnValueChanged(_property);
            }
        }

        private DesignerPropertyEditor CreateEditor(string valueType, DesignerPropertyInfo prop, object obj, bool isConst, bool isFunction)
        {
            if (flowLayoutPanel.Controls.Count > 1)
            {
                flowLayoutPanel.Controls.RemoveAt(1);
            }

            Type editorType = GetEditorType(prop, obj, isConst, isFunction);

            if (editorType == null)
            {
                return null;
            }

            DesignerPropertyEditor editor = (DesignerPropertyEditor)editorType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);
            editor.Location = new System.Drawing.Point(74, 1);
            editor.Margin = new System.Windows.Forms.Padding(0);
            editor.Size = new System.Drawing.Size(flowLayoutPanel.Width - typeComboBox.Width - 5, 20);
            editor.TabIndex = 1;
            editor.ValueWasChanged += new DesignerPropertyEditor.ValueChanged(editor_ValueWasChanged);

            setEditor(valueType, editor, prop, obj, isConst, isFunction);

            flowLayoutPanel.Controls.Add(editor);
            return editor;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isReady && typeComboBox.SelectedItem != null)
            {
                _property.Property.SetValue(_object, null, null);

                string selectedText = ((string)typeComboBox.SelectedItem);
                int pos = selectedText.IndexOf("::");
                setPropertyEditor(CreateEditor(selectedText,
                                               _property, _object,
                                               _allowConst && selectedText == VariableDef.kConst,
                                               pos != -1));

                this.RereshProperty(true, _property);

                OnValueChanged(_property);
            }
        }

        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            // Set the owner for the method.
            if (_property.Property != null)
            {
                object propertyMember = _property.Property.GetValue(_object, null);
                RightValueDef varRV = propertyMember as RightValueDef;

                if (varRV != null && varRV.IsMethod)
                {
                    int methodIndex = -1;
                    int offset = 0;

                    for (int i = 0; i < typeComboBox.Items.Count; ++i)
                    {
                        string item = typeComboBox.Items[i].ToString();
                        int pos = item.IndexOf(VariableDef.kMethod);

                        if (pos != -1)
                        {
                            methodIndex = i;

                            if (item == VariableDef.kSelfMethod)
                            {
                                offset = 1;
                            }

                            break;
                        }
                    }

                    if (methodIndex != -1)
                    {
                        Debug.Check(varRV.Method != null);

                        if (typeComboBox.SelectedIndex - methodIndex - offset >= 0)
                        {
                            string owner = _currentNames[typeComboBox.SelectedIndex - methodIndex - offset];
                            int pos = owner.IndexOf(VariableDef.kMethod);

                            if (pos >= 0)
                            {
                                owner = owner.Substring(0, pos);
                            }

                            varRV.Method.Owner = owner;

                        }
                        else
                        {
                            varRV.Method.Owner = VariableDef.kSelf;
                        }
                    }
                }
            }

            OnValueChanged(_property);
        }

        private void flowLayoutPanel_Resize(object sender, EventArgs e)
        {
            if (this.propertyEditor != null)
            {
                this.propertyEditor.Width = flowLayoutPanel.Width - typeComboBox.Width - 5;
            }
        }

        private void typeComboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void propertyEditor_DescriptionWasChanged(string displayName, string description)
        {
            this.OnDescriptionChanged(displayName, description);
        }
    }
}
