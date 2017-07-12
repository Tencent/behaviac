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
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using Behaviac.Design.Attributes;
using Behaviac.Design.Data;
using Behaviac.Design.Network;
using Behaviac.Design.Nodes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class MetaTypePanel : UserControl
    {
        public delegate void MetaTypeDelegate();
        public event MetaTypeDelegate MetaTypeHandler;

        public enum MetaTypes
        {
            Agent,
            Enum,
            Struct
        }

        private bool _initialized = false;
        private ToolTip _toolTip = new ToolTip();

        public MetaTypePanel()
        {
            InitializeComponent();
        }

        public MetaTypes GetMetaType()
        {
            return (MetaTypes)this.typeComboBox.SelectedIndex;
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
                _isModified = value;

                if (_isModified && MetaTypeHandler != null)
                {
                    MetaTypeHandler();
                }
            }
        }

        public string GetNamespace()
        {
            return this.namespaceTextBox.Text;
        }

        private string GetFullname()
        {
            string name = this.nameTextBox.Text;

            if (!string.IsNullOrEmpty(this.namespaceTextBox.Text))
            {
                name = this.namespaceTextBox.Text + "::" + name;
            }

            return name;
        }

        private AgentType _customizedAgent = null;
        public AgentType GetCustomizedAgent()
        {
            if (this.GetMetaType() == MetaTypes.Agent)
            {
                if (this.baseComboBox.SelectedIndex > -1)
                {
                    Debug.Check(_customizedAgent != null);
                    if (_customizedAgent != null)
                    {
                        AgentType baseAgent = Plugin.AgentTypes[this.baseComboBox.SelectedIndex];
                        string exportLocation = Workspace.Current.MakeRelativePath(this.locationTextBox.Text);
                        exportLocation = exportLocation.Replace("\\\\", "/");
                        exportLocation = exportLocation.Replace("\\", "/");

                        _customizedAgent.Reset(!this.exportCodeCheckBox.Checked, GetFullname(), _customizedAgent.OldName, baseAgent, exportLocation, this.dispTextBox.Text, this.descTextBox.Text);
                    }
                }
            }

            return _customizedAgent;
        }

        private EnumType _enumType = null;
        public EnumType GetCustomizedEnum()
        {
            if (this.GetMetaType() == MetaTypes.Enum)
            {
                Debug.Check(_enumType != null);
                string exportLocation = Workspace.Current.MakeRelativePath(this.locationTextBox.Text);
                exportLocation = exportLocation.Replace("\\\\", "/");
                exportLocation = exportLocation.Replace("\\", "/");

                if (_enumType != null)
                {
                    _enumType.Reset(true, !this.exportCodeCheckBox.Checked, this.nameTextBox.Text, this.namespaceTextBox.Text, exportLocation, this.dispTextBox.Text, this.descTextBox.Text);
                }
            }

            return _enumType;
        }

        private StructType _structType = null;
        public StructType GetCustomizedStruct()
        {
            if (this.GetMetaType() == MetaTypes.Struct)
            {
                Debug.Check(_structType != null);
                string exportLocation = Workspace.Current.MakeRelativePath(this.locationTextBox.Text);
                exportLocation = exportLocation.Replace("\\\\", "/");
                exportLocation = exportLocation.Replace("\\", "/");

                if (_structType != null)
                {
                    _structType.Reset(this.isRefCheckBox.Checked, true, !this.exportCodeCheckBox.Checked, this.nameTextBox.Text, this.namespaceTextBox.Text, this.baseComboBox.Text, exportLocation, this.dispTextBox.Text, this.descTextBox.Text);
                }
            }

            return _structType;
        }

        private bool _isNew = false;

        public void Initialize(object typeObject)
        {
            _initialized = false;
            _isModified = false;
            _isNew = (typeObject == null);
            this.Text = _isNew ? Resources.AddType : Resources.EditType;

            MetaTypes metaType = MetaTypes.Agent;

            if (typeObject != null)
            {
                if (typeObject is AgentType)
                {
                    metaType = MetaTypes.Agent;
                    _customizedAgent = typeObject as AgentType;
                }
                else if (typeObject is EnumType)
                {
                    metaType = MetaTypes.Enum;
                    _enumType = typeObject as EnumType;
                }
                else if (typeObject is StructType)
                {
                    metaType = MetaTypes.Struct;
                    _structType = typeObject as StructType;
                }
            }

            this.typeComboBox.Enabled = _isNew;
            this.typeComboBox.Items.Clear();

            foreach (string type in Enum.GetNames(typeof(MetaTypes)))
            {
                this.typeComboBox.Items.Add(type);
            }

            this.typeComboBox.SelectedIndex = (int)metaType;

            if (this.GetMetaType() == MetaTypes.Agent)
            {
                Debug.Check(_customizedAgent != null);

                if (_customizedAgent != null)
                {
                    if (_customizedAgent.Base != null)
                    {
                        this.baseComboBox.SelectedText = _customizedAgent.Base.Name;
                    }

                    this.exportCodeCheckBox.Checked = !_customizedAgent.IsImplemented;
                    this.nameTextBox.Text = _customizedAgent.BasicName;
                    this.namespaceTextBox.Text = _customizedAgent.Namespace;
                    this.isRefCheckBox.Checked = true;
                    this.locationTextBox.Text = Workspace.Current.MakeAbsolutePath(_customizedAgent.ExportLocation);
                    this.dispTextBox.Text = _customizedAgent.DisplayName;
                    this.descTextBox.Text = _customizedAgent.Description;

                    this.exportCodeCheckBox.Enabled = (_customizedAgent.Name != "behaviac::Agent");
                    this.baseComboBox.Enabled = true;
                    this.isRefCheckBox.Enabled = false;
                }
            }
            else
            {
                if (this.GetMetaType() == MetaTypes.Enum)
                {
                    Debug.Check(_enumType != null);

                    if (_enumType != null)
                    {
                        this.exportCodeCheckBox.Checked = !_enumType.IsImplemented;
                        this.nameTextBox.Text = _enumType.Name;
                        this.namespaceTextBox.Text = _enumType.Namespace;
                        this.isRefCheckBox.Checked = false;
                        this.locationTextBox.Text = Workspace.Current.MakeAbsolutePath(_enumType.ExportLocation);
                        this.dispTextBox.Text = _enumType.DisplayName;
                        this.descTextBox.Text = _enumType.Description;

                        this.exportCodeCheckBox.Enabled = true;
                        this.baseComboBox.Enabled = false;
                        this.isRefCheckBox.Enabled = false;
                    }
                }
                else if (this.GetMetaType() == MetaTypes.Struct)
                {
                    Debug.Check(_structType != null);

                    if (_structType != null)
                    {
                        this.exportCodeCheckBox.Checked = !_structType.IsImplemented;
                        this.nameTextBox.Text = _structType.Name;
                        this.namespaceTextBox.Text = _structType.Namespace;
                        this.baseComboBox.SelectedText = _structType.BaseName;
                        this.isRefCheckBox.Checked = _structType.IsRef;
                        this.locationTextBox.Text = Workspace.Current.MakeAbsolutePath(_structType.ExportLocation);
                        this.dispTextBox.Text = _structType.DisplayName;
                        this.descTextBox.Text = _structType.Description;

                        this.exportCodeCheckBox.Enabled = true;
                        this.baseComboBox.Enabled = true;
                        this.isRefCheckBox.Enabled = true;
                    }
                }
            }

            if (_isNew && string.IsNullOrEmpty(this.namespaceTextBox.Text))
            {
                this.namespaceTextBox.Text = Settings.Default.DefaultNamespace;
            }

            setControlsByExportCode();

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

        private void resetBaseTypes()
        {
            this.baseComboBox.Items.Clear();
            this.baseComboBox.Enabled = false;

            this.isRefCheckBox.Checked = false;
            this.isRefCheckBox.Enabled = false;

            if (this.GetMetaType() == MetaTypes.Agent || this.GetMetaType() == MetaTypes.Struct)
            {
                this.baseComboBox.Enabled = true;

                int baseIndex = -1;

                if (this.GetMetaType() == MetaTypes.Agent) // agent
                {
                    this.isRefCheckBox.Checked = true;

                    for (int i = 0; i < Plugin.AgentTypes.Count; ++i)
                    {
                        AgentType agentType = Plugin.AgentTypes[i];
                        this.baseComboBox.Items.Add(agentType.Name);

                        if (this._customizedAgent != null && this._customizedAgent.Base != null && this._customizedAgent.Base.Name == agentType.Name)
                        {
                            baseIndex = i;
                        }
                    }

                    if (baseIndex < 0 && Plugin.AgentTypes.Count > 0)
                    {
                        baseIndex = 0;
                    }
                }
                else // struct
                {
                    if (this._structType != null)
                    {
                        this.isRefCheckBox.Checked = this._structType.IsRef;
                    }
                    this.isRefCheckBox.Enabled = true;

                    this.baseComboBox.Items.Add("");

                    for (int i = 0; i < TypeManager.Instance.Structs.Count; ++i)
                    {
                        StructType structType = TypeManager.Instance.Structs[i];

                        if (this._structType == null ||
                            string.IsNullOrEmpty(this._structType.Name) ||
                            this._structType.Fullname != structType.Fullname &&
                            this._structType.Fullname != structType.BaseName)
                        {
                            this.baseComboBox.Items.Add(structType.Fullname);

                            if (this._structType != null && this._structType.BaseName == structType.Fullname)
                            {
                                baseIndex = i + 1;
                            }
                        }
                    }
                }

                this.baseComboBox.SelectedIndex = baseIndex;
            }
        }

        public bool Verify()
        {
            if (string.IsNullOrEmpty(this.nameTextBox.Text) || this.nameTextBox.Text.Length < 1 || !char.IsLetter(this.nameTextBox.Text[0]))
            {
                return false;
            }

            string fullname = string.IsNullOrEmpty(this.namespaceTextBox.Text) ? this.nameTextBox.Text : (this.namespaceTextBox.Text + "::" + this.nameTextBox.Text);

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                if (this.GetMetaType() == MetaTypes.Agent && _customizedAgent == agent)
                {
                    return true;
                }

                if (agent.Name == fullname)
                {
                    return false;
                }
            }

            foreach (EnumType customizedEnum in TypeManager.Instance.Enums)
            {
                if (this.GetMetaType() == MetaTypes.Enum && _enumType == customizedEnum)
                {
                    return true;
                }

                if (customizedEnum.Fullname == fullname)
                {
                    return false;
                }
            }

            foreach (StructType structType in TypeManager.Instance.Structs)
            {
                if (this.GetMetaType() == MetaTypes.Struct && _structType == structType)
                {
                    return true;
                }

                if (structType.Fullname == fullname)
                {
                    return false;
                }
            }

            return this.GetMetaType() != MetaTypes.Agent || this.baseComboBox.SelectedIndex > -1;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.GetMetaType() == MetaTypes.Agent)
            {
                if (_customizedAgent == null)
                {
                    string agentName = this.nameTextBox.Text;

                    if (!string.IsNullOrEmpty(this.namespaceTextBox.Text))
                    {
                        agentName = this.namespaceTextBox.Text + "::" + agentName;
                    }

                    _customizedAgent = new AgentType(true, !this.exportCodeCheckBox.Checked, agentName, "", null, this.locationTextBox.Text, this.dispTextBox.Text, this.descTextBox.Text);
                }
            }
            else if (this.GetMetaType() == MetaTypes.Enum)
            {
                if (_enumType == null)
                {
                    _enumType = new EnumType(true, !this.exportCodeCheckBox.Checked, this.nameTextBox.Text, this.namespaceTextBox.Text, this.locationTextBox.Text, this.dispTextBox.Text, this.descTextBox.Text);
                }
            }
            else if (this.GetMetaType() == MetaTypes.Struct)
            {
                if (_structType == null)
                {
                    _structType = new StructType(this.isRefCheckBox.Checked, true, !this.exportCodeCheckBox.Checked, this.nameTextBox.Text, this.namespaceTextBox.Text, this.baseComboBox.Text, this.locationTextBox.Text, this.dispTextBox.Text, this.descTextBox.Text);
                }
            }

            resetBaseTypes();

            if (_initialized)
            {
                this.IsModified = true;
            }
        }

        private bool isCustomizedType()
        {
            if (this.GetMetaType() == MetaTypes.Agent)
            {
                if (_customizedAgent != null)
                {
                    return _customizedAgent.IsCustomized;
                }
            }
            else if (this.GetMetaType() == MetaTypes.Enum)
            {
                if (_enumType != null)
                {
                    return _enumType.IsCustomized;
                }
            }
            else if (this.GetMetaType() == MetaTypes.Struct)
            {
                if (_structType != null)
                {
                    return _structType.IsCustomized;
                }
            }

            return false;
        }

        private void setControlsByExportCode()
        {
            this.baseComboBox.Enabled = true;

            if (this.GetMetaType() == MetaTypes.Enum)
            {
                this.baseComboBox.Enabled = false;

                if (this.baseComboBox.Items.Count > 0)
                {
                    this.baseComboBox.SelectedIndex = 0;
                }
            }
        }

        private void exportCodeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                this.IsModified = true;

                setControlsByExportCode();
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
            string curName = this.nameTextBox.Text;

            if (_initialized && _isNameModified && !string.IsNullOrEmpty(curName))
            {
                _isNameModified = false;

                if (this.GetMetaType() == MetaTypes.Agent)
                {
                    Debug.Check(_customizedAgent != null);

                    if (_customizedAgent != null && curName != _customizedAgent.Name)
                    {
                        _customizedAgent.OldName = _customizedAgent.Name;
                        _customizedAgent.Name = curName;

                        this.IsModified = true;
                    }
                }
                else if (this.GetMetaType() == MetaTypes.Enum)
                {
                    Debug.Check(_enumType != null);

                    if (_enumType != null && curName != _enumType.Name)
                    {
                        _enumType.OldName = _enumType.Fullname;
                        _enumType.Name = curName;

                        TypeManager.Instance.MapName(_enumType.OldName, _enumType.Fullname);

                        this.IsModified = true;
                    }
                }
                else if (this.GetMetaType() == MetaTypes.Struct)
                {
                    Debug.Check(_structType != null);

                    if (_structType != null && curName != _structType.Name)
                    {
                        _structType.OldName = _structType.Fullname;
                        _structType.Name = curName;

                        TypeManager.Instance.MapName(_structType.OldName, _structType.Fullname);

                        this.IsModified = true;
                    }
                }
            }
        }

        private bool _isNamespaceModified = false;

        private void namespaceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _isNamespaceModified = true;
            }
        }

        private void namespaceTextBox_Leave(object sender, EventArgs e)
        {
            if (_initialized && _isNamespaceModified)
            {
                _isNamespaceModified = false;

                this.namespaceTextBox.Text = this.namespaceTextBox.Text.Replace(".", "::");

                if (this.GetMetaType() == MetaTypes.Agent)
                {
                    Debug.Check(_customizedAgent != null);

                    if (_customizedAgent != null)
                    {
                        _customizedAgent.OldName = _customizedAgent.Name;

                        this.IsModified = true;
                    }
                }
                else if (this.GetMetaType() == MetaTypes.Enum)
                {
                    Debug.Check(_enumType != null);

                    if (_enumType != null)
                    {
                        _enumType.OldName = _enumType.Fullname;

                        TypeManager.Instance.MapName(_enumType.OldName, _enumType.Fullname);

                        this.IsModified = true;
                    }
                }
                else if (this.GetMetaType() == MetaTypes.Struct)
                {
                    Debug.Check(_structType != null);

                    if (_structType != null)
                    {
                        _structType.OldName = _structType.Fullname;

                        TypeManager.Instance.MapName(_structType.OldName, _structType.Fullname);

                        this.IsModified = true;
                    }
                }
            }
        }

        private void baseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                this.IsModified = true;
            }
        }

        private void isRefCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                this.IsModified = true;
            }
        }

        private bool _isDispModified = false;

        private void dispTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _isDispModified = true;
            }
        }

        private void dispTextBox_Leave(object sender, EventArgs e)
        {
            if (_initialized && _isDispModified)
            {
                _isDispModified = false;

                this.IsModified = true;
            }
        }

        private bool _isDescModified = false;

        private void descTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _isDescModified = true;
            }
        }

        private void descTextBox_Leave(object sender, EventArgs e)
        {
            if (_initialized && _isDescModified)
            {
                _isDescModified = false;

                this.IsModified = true;
            }
        }

        private void locationButton_Click(object sender, EventArgs e)
        {
            if (_initialized)
            {
                folderBrowserDialog.ShowNewFolderButton = false;

                if (!string.IsNullOrEmpty(this.locationTextBox.Text))
                {
                    folderBrowserDialog.SelectedPath = Path.GetFullPath(this.locationTextBox.Text);
                }
                else
                {
                    folderBrowserDialog.SelectedPath = Workspace.Current.DefaultExportFolder;
                }

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string driveStr0 = Path.GetPathRoot(Workspace.Current.FileName);
                    string driveStr1 = Path.GetPathRoot(folderBrowserDialog.SelectedPath);

                    if (driveStr1 != driveStr0)
                    {
                        MessageBox.Show(Resources.WorkspaceExportRootWarning, Resources.Warning, MessageBoxButtons.OK);
                        return;
                    }

                    this.locationTextBox.Text = folderBrowserDialog.SelectedPath;
                }

                this.IsModified = true;
            }
        }

        private void exportCodeCheckBox_MouseEnter(object sender, EventArgs e)
        {
            _toolTip.AutoPopDelay = 5000;
            _toolTip.InitialDelay = 500;
            _toolTip.ReshowDelay = 5000;
            _toolTip.ShowAlways = true;

            _toolTip.Show(Resources.IsTypeImplemented, this.exportCodeCheckBox);
        }

        private void exportCodeCheckBox_MouseLeave(object sender, EventArgs e)
        {
            _toolTip.Hide(this.exportCodeCheckBox);
        }
    }
}
