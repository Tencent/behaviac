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
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Nodes;

namespace Behaviac.Design
{
    public partial class ParSettingsDialog : Form
    {
        public ParSettingsDialog()
        {
            InitializeComponent();

            nameTextBox.SelectionStart = nameTextBox.TextLength;
        }

        private ParInfo _par = null;
        private ParInfo _parTemp = null;
        private Nodes.Node _rootNode = null;
        private bool _initialized = false;
        private bool _isNewPar = true;
        private bool _isArray = false;

        public void SetPar(ParInfo par, Nodes.Node rootNode, bool isNewPar)
        {
            Debug.Check(par != null && rootNode != null);

            _isNewPar = isNewPar;
            _initialized = false;

            this.Text = isNewPar ? Resources.NewPar : Resources.EditPar;

            _par = par;
            if (_par != null)
            {
                _parTemp = par.Clone();
                _rootNode = rootNode;

                setParTypes();

                if (par != null)
                {
                    _isArray = Plugin.IsArrayType(par.Type);
                    Type type = _isArray ? par.Type.GetGenericArguments()[0] : par.Type;

                    nameTextBox.Text = par.Name;
                    arrayCheckBox.Checked = _isArray;
                    typeComboBox.Text = Plugin.GetMemberValueTypeName(type);
                    descTextBox.Text = par.BasicDescription;

                    setValue(type);
                }

                enableOkButton();

                _initialized = true;
            }
        }

        public ParInfo GetPar()
        {
            return _parTemp;
        }

        private void enableOkButton()
        {
            okButton.Enabled = (_parTemp != null) && !string.IsNullOrEmpty(_parTemp.Name) && !string.IsNullOrEmpty(_parTemp.TypeName);
        }

        private void setParTypes()
        {
            this.typeComboBox.Items.Clear();

            foreach (string typeName in Plugin.GetAllMemberValueTypeNames(false, true))
            {
                this.typeComboBox.Items.Add(typeName);
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_parTemp != null);
                _parTemp.Name = nameTextBox.Text;

                enableOkButton();
            }
        }

        private void arrayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _isArray = arrayCheckBox.Checked;

                setValue(null);
            }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_parTemp != null && typeComboBox.SelectedItem != null);

                setValue(null);
            }
        }

        private void descTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                Debug.Check(_parTemp != null);

                _parTemp.Description = descTextBox.Text;
            }
        }

        private void setValue(Type type)
        {
            Debug.Check(_parTemp != null);

            if (type == null)
            {
                if (typeComboBox.SelectedItem != null)
                {
                    type = Plugin.GetMemberValueType(typeComboBox.SelectedItem.ToString());
                    Debug.Check(type != null);

                    if (_isArray)
                    {
                        type = typeof(List<>).MakeGenericType(type);
                        _parTemp.Variable = new VariableDef(Plugin.DefaultValue(type));

                    }
                    else
                    {
                        if (_parTemp.Variable == null)
                        {
                            _parTemp.Variable = new VariableDef(Plugin.DefaultValue(type));
                        }

                        else
                        {
                            _parTemp.Variable.Value = Plugin.DefaultValue(type);
                        }
                    }

                    _parTemp.TypeName = type.FullName;
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
                tableLayoutPanel.Controls.Remove(valueEditor);
                valueEditor = createValueEditor(type);
                tableLayoutPanel.Controls.Add(this.valueEditor, 1, 3);

                enableOkButton();
            }
        }

        private DesignerPropertyEditor createValueEditor(Type type)
        {
            Debug.Check(type != null);

            Type editorType = Plugin.InvokeEditorType(type);
            Debug.Check(editorType != null);

            DesignerPropertyEditor editor = (DesignerPropertyEditor)Plugin.CreateInstance(editorType);
            editor.AutoSize = true;
            editor.Dock = System.Windows.Forms.DockStyle.Fill;
            editor.Margin = new System.Windows.Forms.Padding(0);
            editor.Name = "propertyEditor";
            editor.TabIndex = 4;
            editor.ValueWasChanged += new DesignerPropertyEditor.ValueChanged(editor_ValueWasChanged);
            editor.SetPar(_parTemp, _rootNode);
            editor.ValueWasAssigned();

            return editor;
        }

        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            Debug.Check(_parTemp != null);

            DesignerPropertyEditor editor = sender as DesignerPropertyEditor;
            _parTemp.Variable = editor.GetVariable();
        }

        private bool _isParExisted = false;

        private void okButton_Click(object sender, EventArgs e)
        {
            Debug.Check(_parTemp != null && !string.IsNullOrEmpty(_parTemp.Name) && _rootNode != null);

            if (_rootNode is Behavior)
            {
                ParInfo p = ((Behavior)_rootNode).LocalVars.Find(delegate(ParInfo par)
                {
                    return par.Name == _parTemp.Name;
                });

                if (p == null || !_isNewPar && p == _par)
                {
                    _isParExisted = false;
                }

                else
                {
                    _isParExisted = true;
                }
            }
        }

        private void ParSettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _isParExisted;

            if (_isParExisted)
            {
                Debug.Check(_parTemp != null && !string.IsNullOrEmpty(_parTemp.Name));
                if (_parTemp != null)
                {
                    string info = string.Format(Resources.ParWarningInfo, _parTemp.Name);
                    MessageBox.Show(info, Resources.ParWarning, MessageBoxButtons.OK);
                }
            }

            _isParExisted = false;
        }
    }
}
