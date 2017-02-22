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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class MetaPropertyPanel : UserControl
    {
        public delegate void MetaPropertyNameDelegate();
        public event MetaPropertyNameDelegate MetaPropertyNameHandler;

        public delegate void MetaPropertyDelegate();
        public event MetaPropertyDelegate MetaPropertyHandler;

        private bool _initialized = false;
        private bool _isNew = true;
        private bool _isArray = false;

        private AgentType _agent = null;
        StructType _structType = null;
        private PropertyDef _originalProperty = null;
        private PropertyDef _property = null;

        DesignerPropertyEditor _valueEditor;

        public MetaPropertyPanel()
        {
            InitializeComponent();
        }

        private bool _shouldCheckMembersInWorkspace = false;
        public bool ShouldCheckMembersInWorkspace()
        {
            return _shouldCheckMembersInWorkspace;
        }

        public void Initialize(bool canBeEdit, AgentType agent, StructType structType, PropertyDef prop, bool canBePar)
        {
            Debug.Check(agent != null || structType != null);

            _initialized = false;

            _isModified = false;
            _shouldCheckMembersInWorkspace = false;
            _isNew = (prop == null);
            _agent = agent;
            _structType = structType;
            _originalProperty = prop;

            setTypes();

            if (_isNew)
            {
                this.Text = canBeEdit ? Resources.AddProperty : Resources.ViewProperty;

                if (_structType == null)
                {
                    if (agent != null)
                    {
                        _property = new PropertyDef(agent, null, agent.Name, "", "", "");
                    }
                }
                else
                {
                    _property = new PropertyDef(null, null, _structType.Name, "", "", "");
                }

                _property.IsPublic = false;

                resetProperty(_property, _property.IsPar);
            }
            else
            {
                this.Text = canBeEdit ? Resources.EditProperty : Resources.ViewProperty;

                resetProperty(prop, prop.IsPar);
            }

            //this.customizedCheckBox.Visible = canBeEdit && !_property.IsInherited && agent != null;
            this.customizedCheckBox.Visible = false;
            this.isLocalCheckBox.Checked = _structType == null && _property.IsPar;
            this.isLocalCheckBox.Visible = canBePar && _structType == null && !_property.IsMember;
            this.isLocalCheckBox.Enabled = canBeEdit;
            this.nameTextBox.Enabled = canBeEdit;
            this.arrayCheckBox.Enabled = canBeEdit || (_structType == null || _structType.IsCustomized) && _property.IsChangeableType;
            this.typeComboBox.Enabled = canBeEdit || (_structType == null || _structType.IsCustomized) && _property.IsChangeableType;
            this.isStaticCheckBox.Enabled = canBeEdit;
            this.isPublicCheckBox.Enabled = canBeEdit;
            this.isConstCheckBox.Enabled = canBeEdit;
            this.dispTextBox.Enabled = canBeEdit;
            this.descTextBox.Enabled = canBeEdit;

            this.nameTextBox.Focus();

            if (this.nameTextBox.TextLength > 0)
            {
                this.nameTextBox.SelectionStart = this.nameTextBox.TextLength;
            }
            else
            {
                this.nameTextBox.Select();
            }

            _initialized = true;
        }

        public PropertyDef GetProperty()
        {
            Debug.Check(_property != null && !string.IsNullOrEmpty(_property.BasicName));

            if (_property != null)
            {
                if (string.IsNullOrEmpty(_property.DisplayName))
                {
                    _property.DisplayName = _property.BasicName;
                }

                if (string.IsNullOrEmpty(_property.BasicDescription))
                {
                    _property.BasicDescription = _property.BasicName;
                }

                return _property;
            }

            return null;
        }

        private bool _isModified = false;
        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            set
            {
                _isModified = true;
            }
        }

        private void resetProperty(PropertyDef prop, bool isPar)
        {
            if (prop != null && (_property != prop || _property != null && _property.IsPar != isPar))
            {
                if (isPar)
                {
                    _property = new ParInfo(prop);
                }
                else
                {
                    _property = new PropertyDef(prop);
                }
            }

            if (_property != null)
            {
                if (prop != null)
                {
                    _property.OldName = prop.Name;
                }

                _isArray = Plugin.IsArrayType(_property.Type);
                Type type = _isArray ? _property.Type.GetGenericArguments()[0] : _property.Type;

                this.nameTextBox.Text = _property.BasicName;
                this.arrayCheckBox.Checked = _isArray;
                this.typeComboBox.Text = Plugin.GetMemberValueTypeName(type);
                this.isStaticCheckBox.Checked = _property.IsStatic;
                this.isPublicCheckBox.Checked = _property.IsPublic;
                this.isConstCheckBox.Checked = _property.IsReadonly;
                this.customizedCheckBox.Checked = _property.IsCustomized;
                this.dispTextBox.Text = _property.DisplayName;
                this.descTextBox.Text = _property.BasicDescription;

                resetType(type, false);
            }
        }

        private void createValueEditor(Type type)
        {
            Type editorType = Plugin.InvokeEditorType(type);
            Debug.Check(editorType != null);

            if (editorType != null)
            {
                ParInfo par = this._property as ParInfo;

                if (_property.Variable == null || _property.Variable.ValueType != type)
                {
                    _property.Variable = new VariableDef(Plugin.DefaultValue(type));
                }

                _valueEditor = (DesignerPropertyEditor)editorType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
                if (_valueEditor != null)
                {
                    _valueEditor.Margin = new System.Windows.Forms.Padding(0);
                    _valueEditor.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    _valueEditor.Width = this.dispTextBox.Width;
                    _valueEditor.Location = this.dispTextBox.Location;
                    _valueEditor.Location = new Point(_valueEditor.Location.X, _valueEditor.Location.Y - _valueEditor.Height - 6);
                    _valueEditor.ValueWasChanged += new DesignerPropertyEditor.ValueChanged(editor_ValueWasChanged);

                    if (par != null)
                    {
                        _valueEditor.SetPar(par, null);
                    }
                    else
                    {
                        _valueEditor.SetVariable(this._property.Variable, null);
                    }

                    _valueEditor.ValueWasAssigned();

                    this.Controls.Add(_valueEditor);
                    _valueEditor.BringToFront();
                }
            }
        }

        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            if (_initialized)
            {
                DesignerPropertyEditor editor = sender as DesignerPropertyEditor;

                _property.Variable = editor.GetVariable();

                this.IsModified = true;
            }
        }

        public bool Verify()
        {
            Debug.Check(_property != null);

            string propertyName = this.nameTextBox.Text;

            bool isValid = false;
            if (_property != null)
            {
                isValid = !string.IsNullOrEmpty(propertyName) && propertyName.Length >= 1 &&
                               char.IsLetter(propertyName[0]) && (_property.Type != null);
            }

            if (isValid && _agent != null)
            {
                PropertyDef prop = _agent.GetPropertyByName(propertyName);

                if (_isNew)
                {
                    isValid = (prop == null);
                }
                else
                {
                    isValid = (prop == null || prop == _originalProperty);
                }
            }

            return isValid;
        }

        private void setTypes()
        {
            this.typeComboBox.Items.Clear();

            foreach (string typeName in Plugin.GetAllMemberValueTypeNames(false, true))
            {
                this.typeComboBox.Items.Add(typeName);
            }
        }

        private void resetType(Type type, bool reset)
        {
            Debug.Check(_property != null);

            if (type == null)
            {
                if (this.typeComboBox.SelectedItem != null)
                {
                    type = Plugin.GetMemberValueType(this.typeComboBox.SelectedItem.ToString());
                    Debug.Check(type != null);

                    if (_isArray)
                    {
                        type = typeof(List<>).MakeGenericType(type);
                    }
                }
            }
            else
            {
                if (_isArray)
                {
                    type = typeof(List<>).MakeGenericType(type);
                }
            }

            if (type != null)
            {
                if (reset && _property != null)
                {
                    _property.Type = type;
                    _property.NativeType = Plugin.GetMemberValueTypeName(type);
                }

                if (_valueEditor != null)
                {
                    this.Controls.Remove(_valueEditor);
                    _valueEditor = null;
                }

                if (_agent != null)
                {
                    createValueEditor(type);

                    this.valueLabel.Visible = true;
                }
                else
                {
                    this.valueLabel.Visible = false;
                }
            }
        }

        private bool _isNameModified = false;
        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _isNameModified = true;
            }
        }

        private void nameTextBox_Leave(object sender, EventArgs e)
        {
            ModifyName();
        }

        private void nameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ModifyName();
            }
        }

        private void ModifyName()
        {
            if (_initialized && _isNameModified)
            {
                _isNameModified = false;

                Debug.Check(_property != null);
                if (_property != null)
                {
                    if (!_isNew && !this.Verify())
                    {
                        MessageBox.Show(Resources.PropertyVerifyWarning, Resources.Warning, MessageBoxButtons.OK);
                        this.nameTextBox.Text = _property.BasicName;
                    }
                    else if (_property.BasicName != this.nameTextBox.Text)
                    {
                        _property.Name = this.nameTextBox.Text;
                        _property.DisplayName = _property.Name;
                        this.dispTextBox.Text = _property.DisplayName;

                        this.IsModified = true;
                        _shouldCheckMembersInWorkspace = true;

                        if (MetaPropertyNameHandler != null)
                        {
                            MetaPropertyNameHandler();
                        }
                    }
                }
            }
        }

        private void arrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _isArray = this.arrayCheckBox.Checked;

                resetType(null, true);

                this.IsModified = true;
            }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                resetType(null, true);

                this.IsModified = true;
                _shouldCheckMembersInWorkspace = true;
            }
        }

        private void isStaticCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_property != null);
                if (_property != null)
                {
                    _property.IsStatic = this.isStaticCheckBox.Checked;
                }

                this.IsModified = true;
            }
        }

        private void isPublicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_property != null);
                if (_property != null)
                {
                    _property.IsPublic = this.isPublicCheckBox.Checked;
                }

                this.IsModified = true;
            }
        }

        private void dispTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized && _property != null && _property.DisplayName != this.dispTextBox.Text)
            {
                _property.DisplayName = this.dispTextBox.Text;

                this.IsModified = true;
            }
        }

        private void descTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized && _property != null && _property.BasicDescription != this.descTextBox.Text)
            {
                _property.BasicDescription = this.descTextBox.Text;

                this.IsModified = true;
            }
        }

        private void isConstcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_property != null);
                if (_property != null)
                {
                    _property.IsReadonly = this.isConstCheckBox.Checked;
                }

                this.IsModified = true;
            }
        }

        private void customizedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_property != null);
                if (_property != null)
                {
                    _property.IsCustomized = this.customizedCheckBox.Checked;
                }

                this.IsModified = true;

                if (MetaPropertyHandler != null)
                {
                    MetaPropertyHandler();
                }
            }
        }

        private void isLocalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_property != null);
                if (_property != null)
                {
                    resetProperty(_property, this.isLocalCheckBox.Checked);
                }

                this.IsModified = true;

                if (MetaPropertyHandler != null)
                {
                    MetaPropertyHandler();
                }
            }
        }
    }
}
