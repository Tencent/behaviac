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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Behaviac.Design.Attributes;
using Behaviac.Design.Data;
using Behaviac.Design.Network;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class MetaMethodPanel : UserControl
    {
        public delegate void MetaMethodNameDelegate();
        public event MetaMethodNameDelegate MetaMethodNameHandler;

        class RowControl
        {
            private MethodDef.Param _param;
            public MethodDef.Param Param
            {
                get
                {
                    return _param;
                }
                set
                {
                    _param = value;
                }
            }

            private System.Windows.Forms.CheckBox _selectCheckBox;
            public System.Windows.Forms.CheckBox SelectCheckBox
            {
                get
                {
                    return _selectCheckBox;
                }
                set
                {
                    _selectCheckBox = value;
                }
            }

            private System.Windows.Forms.TextBox _nameTextBox;
            public System.Windows.Forms.TextBox NameTextBox
            {
                get
                {
                    return _nameTextBox;
                }
                set
                {
                    _nameTextBox = value;
                }
            }

            private System.Windows.Forms.ComboBox _typeComboBox;
            public System.Windows.Forms.ComboBox TypeComboBox
            {
                get
                {
                    return _typeComboBox;
                }
                set
                {
                    _typeComboBox = value;
                }
            }

            private System.Windows.Forms.CheckBox _isArrayCheckBox;
            public System.Windows.Forms.CheckBox IsArrayCheckBox
            {
                get
                {
                    return _isArrayCheckBox;
                }
                set
                {
                    _isArrayCheckBox = value;
                }
            }

            private System.Windows.Forms.CheckBox _byReferrenceCheckBox;
            public System.Windows.Forms.CheckBox ByReferrenceCheckBox
            {
                get
                {
                    return _byReferrenceCheckBox;
                }
                set
                {
                    _byReferrenceCheckBox = value;
                }
            }

            private System.Windows.Forms.CheckBox _isConstCheckBox;
            public System.Windows.Forms.CheckBox IsConstCheckBox
            {
                get
                {
                    return _isConstCheckBox;
                }
                set
                {
                    _isConstCheckBox = value;
                }
            }

            private System.Windows.Forms.TextBox _displayNameTextBox;
            public System.Windows.Forms.TextBox DisplayNameTextBox
            {
                get
                {
                    return _displayNameTextBox;
                }
                set
                {
                    _displayNameTextBox = value;
                }
            }

            public void Cleanup()
            {
                if (this.SelectCheckBox != null)
                {
                    this.SelectCheckBox.Dispose();
                }

                if (this.NameTextBox != null)
                {
                    this.NameTextBox.Dispose();
                }

                if (this.TypeComboBox != null)
                {
                    this.TypeComboBox.Dispose();
                }

                if (this.IsArrayCheckBox != null)
                {
                    this.IsArrayCheckBox.Dispose();
                }

                if (this.ByReferrenceCheckBox != null)
                {
                    this.ByReferrenceCheckBox.Dispose();
                }

                if (this.IsConstCheckBox != null)
                {
                    this.IsConstCheckBox.Dispose();
                }

                if (this.DisplayNameTextBox != null)
                {
                    this.DisplayNameTextBox.Dispose();
                }
            }
        }

        private List<RowControl> _rowControls = new List<RowControl>();
        private AgentType _agent = null;
        private MethodDef _originalMethod = null;
        private MethodDef _method = null;

        private bool _initialized = false;
        private bool _isNew = false;
        private bool _isArray = false;

        public MetaMethodPanel()
        {
            InitializeComponent();
        }

        public MethodDef GetMethod()
        {
            Debug.Check(_method != null && !string.IsNullOrEmpty(_method.BasicName) && _method.ReturnType != null);

            if (_method != null)
            {
                if (string.IsNullOrEmpty(_method.DisplayName))
                {
                    _method.DisplayName = _method.BasicName;
                }

                if (string.IsNullOrEmpty(_method.BasicDescription))
                {
                    _method.BasicDescription = _method.BasicName;
                }

                return _method;
            }

            return null;
        }

        private bool _shouldCheckMembersInWorkspace = false;
        public bool ShouldCheckMembersInWorkspace()
        {
            return _shouldCheckMembersInWorkspace;
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

        public void Initialize(bool canBeEdit, AgentType agent, MethodDef method, MemberType memberType)
        {
            _initialized = false;

            _isModified = false;
            _shouldCheckMembersInWorkspace = false;
            _isNew = (method == null);
            _agent = agent;
            _originalMethod = method;

            setReturnTypes();

            if (memberType == MemberType.Task)
            {
                this.Text = _isNew ? Resources.AddTask : Resources.EditTask;
            }
            else if (memberType == MemberType.Method)
            {
                this.Text = _isNew ? Resources.AddMethod : Resources.EditMethod;
            }

            bool isCppLanguage = (Workspace.Current.Language == "cpp");

            if (_isNew)
            {
                string nativeReturnType = "void";
                Type returnType = typeof(void);
                _method = new MethodDef(agent, memberType, agent.Name, "", "", "", nativeReturnType, returnType);

                _method.IsPublic = !isCppLanguage;
            }
            else
            {
                _method = new MethodDef(method);
                _method.OldName = method.Name;

                this.dispTextBox.Text = _method.DisplayName;
                this.descTextBox.Text = _method.BasicDescription;

                this.tableLayoutPanel.Hide();

                deleteAllRowControls();

                for (int i = 0; i < _method.Params.Count; ++i)
                {
                    MethodDef.Param param = _method.Params[i];
                    addRowControl(param);
                }

                this.tableLayoutPanel.Show();
            }

            _isArray = Plugin.IsArrayType(_method.ReturnType);
            Type type = _isArray ? _method.ReturnType.GetGenericArguments()[0] : _method.ReturnType;

            this.nameTextBox.Text = _method.BasicName;
            this.returnTypeComboBox.Text = Plugin.GetMemberValueTypeName(type);
            this.arrayCheckBox.Checked = _isArray;
            this.isStaticCheckBox.Checked = _method.IsStatic;
            this.isPublicCheckBox.Checked = _method.IsPublic;

            this.nameTextBox.Enabled = canBeEdit;
            this.returnTypeComboBox.Enabled = (memberType != MemberType.Task) ? (canBeEdit || _method.IsChangeableType) : false;
            this.arrayCheckBox.Enabled = this.returnTypeComboBox.Enabled;
            this.isStaticCheckBox.Enabled = canBeEdit;
            this.isPublicCheckBox.Enabled = canBeEdit && isCppLanguage;
            this.dispTextBox.Enabled = canBeEdit;
            this.descTextBox.Enabled = canBeEdit;
            this.addParamButton.Enabled = canBeEdit;
            this.removeParamButton.Enabled = canBeEdit;
            this.tableLayoutPanel.Enabled = canBeEdit || _method.IsChangeableType; // parameters

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

        private void setReturnTypes()
        {
            this.returnTypeComboBox.Items.Clear();

            foreach (string typeName in Plugin.GetAllMemberValueTypeNames(true, true))
            {
                this.returnTypeComboBox.Items.Add(typeName);
            }
        }

        private int getRowIndex(CheckBox selectCheckBox, TextBox nameTextBox, ComboBox typeComboBox, CheckBox isArrayCheckBox, CheckBox byReferrenceCheckBox, CheckBox isConstCheckBox, TextBox displayNameTextBox)
        {
            for (int i = 0; i < _rowControls.Count; ++i)
            {
                if (selectCheckBox != null && _rowControls[i].SelectCheckBox == selectCheckBox ||
                    nameTextBox != null && _rowControls[i].NameTextBox == nameTextBox ||
                    typeComboBox != null && _rowControls[i].TypeComboBox == typeComboBox ||
                    isArrayCheckBox != null && _rowControls[i].IsArrayCheckBox == isArrayCheckBox ||
                    byReferrenceCheckBox != null && _rowControls[i].ByReferrenceCheckBox == byReferrenceCheckBox ||
                    isConstCheckBox != null && _rowControls[i].IsConstCheckBox == isConstCheckBox ||
                    displayNameTextBox != null && _rowControls[i].DisplayNameTextBox == displayNameTextBox)
                {
                    return i;
                }
            }

            return -1;
        }

        private void addParamButton_Click(object sender, EventArgs e)
        {
            Debug.Check(_method != null);

            if (_method != null)
            {
                if (_method.MemberType == MemberType.Task && _method.Params.Count >= 3)
                {
                    MessageBox.Show(Resources.EventParametersInfo, Resources.Warning, MessageBoxButtons.OK);
                    return;
                }

                if (_method.Params.Count >= 8)
                {
                    MessageBox.Show(Resources.MethodParametersInfo, Resources.Warning, MessageBoxButtons.OK);
                    return;
                }

                MethodDef.Param param = new MethodDef.Param("", null, "", "", "");
                _method.Params.Add(param);

                RowControl row = addRowControl(param);
                row.NameTextBox.Focus();

                this.IsModified = true;
            }
        }

        private void removeParamButton_Click(object sender, EventArgs e)
        {
            List<int> removeIndexes = new List<int>();

            for (int i = _rowControls.Count - 1; i >= 0; --i)
            {
                if (_rowControls[i].SelectCheckBox.Checked)
                {
                    removeIndexes.Add(i);
                }
            }

            if (removeIndexes.Count > 0 &&
                DialogResult.Yes == MessageBox.Show(Resources.RemoveParametersInfo, Resources.Warning, MessageBoxButtons.YesNo))
            {
                foreach (int index in removeIndexes)
                {
                    _method.Params.RemoveAt(index);

                    this.deleteRowControl(index);
                }

                this.IsModified = true;
            }
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            int rowIndex = getRowIndex(null, sender as TextBox, null, null, null, null, null);

            if (rowIndex > -1)
            {
                RowControl rowControl = _rowControls[rowIndex];

                if (rowControl.Param.Name != rowControl.NameTextBox.Text)
                {
                    rowControl.Param.Name = rowControl.NameTextBox.Text;
                    rowControl.Param.DisplayName = rowControl.Param.Name;

                    rowControl.DisplayNameTextBox.Text = rowControl.Param.DisplayName;

                    this.IsModified = true;
                }
            }
        }

        private void DisplayNameTextBox_TextChanged(object sender, EventArgs e)
        {
            int rowIndex = getRowIndex(null, null, null, null, null, null, sender as TextBox);

            if (rowIndex > -1)
            {
                RowControl rowControl = _rowControls[rowIndex];
                rowControl.Param.DisplayName = rowControl.DisplayNameTextBox.Text;

                this.IsModified = true;
            }
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int rowIndex = getRowIndex(null, null, sender as ComboBox, null, null, null, null);

            if (rowIndex > -1)
            {
                RowControl rowControl = _rowControls[rowIndex];
                rowControl.Param.Type = getParamType(rowIndex);

                rowControl.IsArrayCheckBox.Enabled = true;
                rowControl.ByReferrenceCheckBox.Enabled = true;
                rowControl.IsConstCheckBox.Enabled = true;

                this.IsModified = true;
            }
        }

        private void IsArrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int rowIndex = getRowIndex(null, null, null, sender as CheckBox, null, null, null);

            if (rowIndex > -1)
            {
                RowControl rowControl = _rowControls[rowIndex];
                rowControl.Param.Type = getParamType(rowIndex);

                this.IsModified = true;
            }
        }

        private void ByReferrenceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int rowIndex = getRowIndex(null, null, null, null, sender as CheckBox, null, null);

            if (rowIndex > -1)
            {
                RowControl rowControl = _rowControls[rowIndex];
                rowControl.Param.IsRef = rowControl.ByReferrenceCheckBox.Checked;

                this.IsModified = true;
            }
        }

        private void IsConstCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            int rowIndex = getRowIndex(null, null, null, null, null, sender as CheckBox, null);

            if (rowIndex > -1)
            {
                RowControl rowControl = _rowControls[rowIndex];
                rowControl.Param.IsConst = rowControl.IsConstCheckBox.Checked;

                this.IsModified = true;
            }
        }

        private void setParamTypes(ComboBox combobox)
        {
            combobox.Items.Clear();

            foreach (string typeName in Plugin.GetAllMemberValueTypeNames(false, true))
            {
                combobox.Items.Add(typeName);
            }
        }

        private Type getParamType(int rowIndex)
        {
            Type type = null;

            if (rowIndex > -1)
            {
                RowControl rowControl = _rowControls[rowIndex];

                if (rowControl.TypeComboBox.SelectedItem != null)
                {
                    type = Plugin.GetMemberValueType(rowControl.TypeComboBox.SelectedItem.ToString());
                    Debug.Check(type != null);

                    if (rowControl.IsArrayCheckBox.Checked)
                    {
                        type = typeof(List<>).MakeGenericType(type);
                    }
                }
            }

            return type;
        }

        private RowControl addRowControl(MethodDef.Param param)
        {
            Debug.Check(param != null);
            if (param != null)
            {
                RowControl rowControl = new RowControl();
                _rowControls.Add(rowControl);

                int rowIndex = _rowControls.Count;

                rowControl.Param = param;

                rowControl.SelectCheckBox = new System.Windows.Forms.CheckBox();
                rowControl.SelectCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
                rowControl.SelectCheckBox.ForeColor = System.Drawing.Color.LightGray;
                rowControl.SelectCheckBox.FlatAppearance.MouseDownBackColor = Color.DarkGray;
                rowControl.SelectCheckBox.FlatAppearance.MouseOverBackColor = Color.DarkGray;
                //rowControl.SelectCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
                rowControl.SelectCheckBox.CheckAlign = ContentAlignment.MiddleCenter;
                rowControl.SelectCheckBox.Margin = new System.Windows.Forms.Padding(0);
                this.tableLayoutPanel.Controls.Add(rowControl.SelectCheckBox, 0, rowIndex);

                rowControl.NameTextBox = new System.Windows.Forms.TextBox();
                rowControl.NameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
                rowControl.NameTextBox.ForeColor = System.Drawing.Color.LightGray;
                rowControl.NameTextBox.BorderStyle = BorderStyle.None;
                rowControl.NameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
                rowControl.NameTextBox.Margin = new System.Windows.Forms.Padding(3);
                rowControl.NameTextBox.Text = param.Name;
                rowControl.NameTextBox.TextChanged += new EventHandler(NameTextBox_TextChanged);
                this.tableLayoutPanel.Controls.Add(rowControl.NameTextBox, 1, rowIndex);

                bool isParamArray = Plugin.IsArrayType(param.Type) || param.Type == typeof(System.Collections.IList);
                Type paramType = isParamArray && param.Type.GetGenericArguments().Length > 0 ? param.Type.GetGenericArguments()[0] : param.Type;

                rowControl.TypeComboBox = new System.Windows.Forms.ComboBox();
                rowControl.TypeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
                rowControl.TypeComboBox.ForeColor = System.Drawing.Color.LightGray;
                rowControl.TypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                rowControl.TypeComboBox.FlatStyle = FlatStyle.Popup;
                rowControl.TypeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
                rowControl.TypeComboBox.Margin = new System.Windows.Forms.Padding(0);
                setParamTypes(rowControl.TypeComboBox);
                rowControl.TypeComboBox.Text = Plugin.GetMemberValueTypeName(paramType);
                rowControl.TypeComboBox.SelectedIndexChanged += new EventHandler(TypeComboBox_SelectedIndexChanged);
                this.tableLayoutPanel.Controls.Add(rowControl.TypeComboBox, 2, rowIndex);

                rowControl.IsArrayCheckBox = new System.Windows.Forms.CheckBox();
                rowControl.IsArrayCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
                rowControl.IsArrayCheckBox.ForeColor = System.Drawing.Color.LightGray;
                rowControl.IsArrayCheckBox.FlatAppearance.MouseDownBackColor = Color.DarkGray;
                rowControl.IsArrayCheckBox.FlatAppearance.MouseOverBackColor = Color.DarkGray;
                //rowControl.IsArrayCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
                rowControl.IsArrayCheckBox.CheckAlign = ContentAlignment.MiddleCenter;
                rowControl.IsArrayCheckBox.Margin = new System.Windows.Forms.Padding(0);
                rowControl.IsArrayCheckBox.Enabled = (paramType != null);
                rowControl.IsArrayCheckBox.Checked = isParamArray;
                rowControl.IsArrayCheckBox.CheckedChanged += new EventHandler(IsArrayCheckBox_CheckedChanged);
                this.tableLayoutPanel.Controls.Add(rowControl.IsArrayCheckBox, 3, rowIndex);

                rowControl.ByReferrenceCheckBox = new System.Windows.Forms.CheckBox();
                rowControl.ByReferrenceCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
                rowControl.ByReferrenceCheckBox.ForeColor = System.Drawing.Color.LightGray;
                rowControl.ByReferrenceCheckBox.FlatAppearance.MouseDownBackColor = Color.DarkGray;
                rowControl.ByReferrenceCheckBox.FlatAppearance.MouseOverBackColor = Color.DarkGray;
                //rowControl.ByReferrenceCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
                rowControl.ByReferrenceCheckBox.CheckAlign = ContentAlignment.MiddleCenter;
                rowControl.ByReferrenceCheckBox.Margin = new System.Windows.Forms.Padding(0);
                rowControl.ByReferrenceCheckBox.Enabled = (paramType != null);
                rowControl.ByReferrenceCheckBox.Checked = param.IsRef;
                rowControl.ByReferrenceCheckBox.CheckedChanged += new EventHandler(ByReferrenceCheckBox_CheckedChanged);
                this.tableLayoutPanel.Controls.Add(rowControl.ByReferrenceCheckBox, 4, rowIndex);

                rowControl.IsConstCheckBox = new System.Windows.Forms.CheckBox();
                rowControl.IsConstCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
                rowControl.IsConstCheckBox.ForeColor = System.Drawing.Color.LightGray;
                rowControl.IsConstCheckBox.FlatAppearance.MouseDownBackColor = Color.DarkGray;
                rowControl.IsConstCheckBox.FlatAppearance.MouseOverBackColor = Color.DarkGray;
                //rowControl.IsConstCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
                rowControl.IsConstCheckBox.CheckAlign = ContentAlignment.MiddleCenter;
                rowControl.IsConstCheckBox.Margin = new System.Windows.Forms.Padding(0);
                rowControl.IsConstCheckBox.Enabled = (paramType != null && Workspace.Current.Language == "cpp");
                rowControl.IsConstCheckBox.Checked = param.IsConst;
                rowControl.IsConstCheckBox.CheckedChanged += new EventHandler(IsConstCheckBox_CheckedChanged);
                this.tableLayoutPanel.Controls.Add(rowControl.IsConstCheckBox, 5, rowIndex);

                rowControl.DisplayNameTextBox = new System.Windows.Forms.TextBox();
                rowControl.DisplayNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
                rowControl.DisplayNameTextBox.ForeColor = System.Drawing.Color.LightGray;
                rowControl.DisplayNameTextBox.BorderStyle = BorderStyle.None;
                rowControl.DisplayNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
                rowControl.DisplayNameTextBox.Margin = new System.Windows.Forms.Padding(3);
                rowControl.DisplayNameTextBox.Text = param.DisplayName;
                rowControl.DisplayNameTextBox.TextChanged += new EventHandler(DisplayNameTextBox_TextChanged);
                this.tableLayoutPanel.Controls.Add(rowControl.DisplayNameTextBox, 6, rowIndex);

                this.tableLayoutPanel.RowCount++;
                this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));

                return rowControl;
            }

            return null;
        }

        private void deleteRowControl(int rowIndex)
        {
            RowControl rowControl = _rowControls[rowIndex];
            _rowControls.RemoveAt(rowIndex);

            rowIndex++;

            this.tableLayoutPanel.RowStyles.RemoveAt(rowIndex);

            this.tableLayoutPanel.Controls.Remove(rowControl.SelectCheckBox);
            this.tableLayoutPanel.Controls.Remove(rowControl.NameTextBox);
            this.tableLayoutPanel.Controls.Remove(rowControl.TypeComboBox);
            this.tableLayoutPanel.Controls.Remove(rowControl.IsArrayCheckBox);
            this.tableLayoutPanel.Controls.Remove(rowControl.ByReferrenceCheckBox);
            this.tableLayoutPanel.Controls.Remove(rowControl.IsConstCheckBox);
            this.tableLayoutPanel.Controls.Remove(rowControl.DisplayNameTextBox);

            for (int r = rowIndex + 1; r < this.tableLayoutPanel.RowCount; ++r)
            {
                for (int c = 0; c < this.tableLayoutPanel.ColumnCount; ++c)
                {
                    Control control = this.tableLayoutPanel.GetControlFromPosition(c, r);

                    if (control != null)
                    {
                        this.tableLayoutPanel.SetRow(control, r - 1);
                    }
                }
            }

            this.tableLayoutPanel.RowCount--;
        }

        private void deleteAllRowControls()
        {
            for (int rowIndex = _rowControls.Count - 1; rowIndex >= 0; --rowIndex)
            {
                RowControl rowControl = _rowControls[rowIndex];
                _rowControls.RemoveAt(rowIndex);

                this.tableLayoutPanel.RowStyles.RemoveAt(rowIndex);

                this.tableLayoutPanel.Controls.Remove(rowControl.SelectCheckBox);
                this.tableLayoutPanel.Controls.Remove(rowControl.NameTextBox);
                this.tableLayoutPanel.Controls.Remove(rowControl.TypeComboBox);
                this.tableLayoutPanel.Controls.Remove(rowControl.IsArrayCheckBox);
                this.tableLayoutPanel.Controls.Remove(rowControl.ByReferrenceCheckBox);
                this.tableLayoutPanel.Controls.Remove(rowControl.IsConstCheckBox);
                this.tableLayoutPanel.Controls.Remove(rowControl.DisplayNameTextBox);

                rowControl.Cleanup();
            }

            this.tableLayoutPanel.RowCount = 1;
        }

        private void resetType(Type type)
        {
            Debug.Check(_method != null);

            if (type == null)
            {
                if (this.returnTypeComboBox.SelectedItem != null)
                {
                    type = Plugin.GetMemberValueType(this.returnTypeComboBox.SelectedItem.ToString());
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

            if (type != null && _method != null)
            {
                _method.ReturnType = type;
                _method.NativeReturnType = Plugin.GetMemberValueTypeName(type);
            }
        }

        public bool Verify()
        {
            Debug.Check(_method != null);

            if (_method != null)
            {
                string methodName = this.nameTextBox.Text;

                bool isValid = !string.IsNullOrEmpty(methodName) && methodName.Length >= 1 &&
                               char.IsLetter(methodName[0]) && (_method.ReturnType != null);

                if (isValid)
                {
                    MethodDef method = _agent.GetMethodByName(methodName);

                    // check method name
                    if (_isNew)
                    {
                        isValid = (method == null);
                    }
                    else
                    {
                        isValid = (method == null || method == _originalMethod);
                    }

                    // check its parameters
                    if (isValid)
                    {
                        foreach (MethodDef.Param param in _method.Params)
                        {
                            if (string.IsNullOrEmpty(param.Name) || param.Type == null)
                            {
                                isValid = false;
                                break;
                            }
                        }
                    }
                }

                return isValid;
            }

            return false;
        }

        private bool _isNameModified = false;
        private void nameTextBox_TextChanged_1(object sender, EventArgs e)
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

                Debug.Check(_method != null);

                if (_method != null)
                {
                    if (!_isNew && !this.Verify())
                    {
                        MessageBox.Show(Resources.MethodVerifyWarning, Resources.Warning, MessageBoxButtons.OK);
                        this.nameTextBox.Text = _method.BasicName;
                    }
                    else if (_method.BasicName != this.nameTextBox.Text)
                    {
                        _method.Name = this.nameTextBox.Text;
                        _method.DisplayName = _method.Name;
                        this.dispTextBox.Text = _method.DisplayName;

                        this.IsModified = true;
                        _shouldCheckMembersInWorkspace = true;

                        if (MetaMethodNameHandler != null)
                        {
                            MetaMethodNameHandler();
                        }
                    }
                }
            }
        }

        private void dispTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized && _method.DisplayName != this.dispTextBox.Text)
            {
                _method.DisplayName = this.dispTextBox.Text;

                this.IsModified = true;
            }
        }

        private void descTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized && _method.BasicDescription != this.descTextBox.Text)
            {
                _method.BasicDescription = this.descTextBox.Text;

                this.IsModified = true;
            }
        }

        private void returnTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                resetType(null);

                this.IsModified = true;
                _shouldCheckMembersInWorkspace = true;
            }
        }

        private void arrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _isArray = this.arrayCheckBox.Checked;

                resetType(null);

                this.IsModified = true;
            }
        }

        private void isStaticCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _method.IsStatic = this.isStaticCheckBox.Checked;

                this.IsModified = true;
            }
        }

        private void isPublicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _method.IsPublic = this.isPublicCheckBox.Checked;

                this.IsModified = true;
            }
        }
    }
}
