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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Behaviac.Design.Data;
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;

namespace Behaviac.Design
{
    internal partial class MetaStoreDock : Form
    {
        public class MemberItem
        {
            public string DisplayName = "";

            public PropertyDef Property = null;
            public MethodDef Method = null;

            public MemberItem(string displayName, PropertyDef property)
            {
                DisplayName = displayName;
                Property = property;
            }

            public MemberItem(string displayName, MethodDef method)
            {
                DisplayName = displayName;
                Method = method;
            }
        }

        private const string Par_Str = "-  ";
        private const string Exported_Str = ">  ";
        private const string Customized_Str = "*  ";
        private const string Member_Str = "   ";
        private const string Inherited_Str = "~  ";
        private const string Empty_Type_Str = "?  ";
        private const string Changeable_Type_Str = "$  ";

        private static MetaStoreDock _metaStoreDock = null;
        private static AgentType _lastAgent = null;
        private static string _lastSelectedType = "";
        private static int _lastMemberTypeIndex = (int)MemberType.Property;
        private static Nodes.BehaviorNode _isParDirtyBehavior = null;

        private int _previousSelectedTypeIndex = -1;
        private int _previousSelectedMemberIndex = -1;

        internal static void Inspect(BehaviorTreeViewDock dock)
        {
            if (_metaStoreDock == null)
            {
                _metaStoreDock = new MetaStoreDock();
                _metaStoreDock.Owner = MainWindow.Instance;
            }

            _metaStoreDock.initialize(dock);
            _metaStoreDock.Show();
        }

        internal static bool IsVisible()
        {
            return _metaStoreDock != null && _metaStoreDock.Visible;
        }

        internal static void CheckSave()
        {
            if (_metaStoreDock != null && Workspace.Current.IsBlackboardDirty &&
                DialogResult.Yes == MessageBox.Show(Resources.MetaSaveInfo, Resources.Warning, MessageBoxButtons.YesNo))
            {
                _metaStoreDock.save();
            }
        }

        internal static void Reset()
        {
            if (_metaStoreDock != null)
            {
                _metaStoreDock.Clear();
            }
        }

        private BehaviorTreeViewDock _dock = null;
        private ToolTip _toolTip = new ToolTip();

        public MetaStoreDock()
        {
            InitializeComponent();

            _metaStoreDock = this;

            _previousSelectedTypeIndex = -1;
            _previousSelectedMemberIndex = -1;

            Plugin.UpdateMetaStoreHandler += Plugin_UpdateMetaStoreHandler;
            Plugin.PostSetWorkspaceHandler += Plugin_PostSetWorkspaceHandler;
        }

        private void MetaStoreDock_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Hide();
            //e.Cancel = true;

            CheckTypeModified();
            CheckMemberModified();

            CheckSave();

            this.metaTypePanel.MetaTypeHandler -= metaTypePanel_MetaTypeHandler;

            Plugin.UpdateMetaStoreHandler -= Plugin_UpdateMetaStoreHandler;
            Plugin.PostSetWorkspaceHandler -= Plugin_PostSetWorkspaceHandler;

            _lastSelectedType = "";
            _metaStoreDock = null;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Plugin_UpdateMetaStoreHandler(object dock)
        {
            if (MetaStoreDock.IsVisible())
            {
                MetaStoreDock.Inspect(dock as BehaviorTreeViewDock);
            }
        }

        private void Plugin_PostSetWorkspaceHandler()
        {
            if (MetaStoreDock.IsVisible())
            {
                MetaStoreDock.Reset();
            }
        }

        private void Clear()
        {
            //_lastSelectedType = "";
        }

        private void apply(bool query, int typeSelectedIndex, int memberSelectedIndex, bool resetSelectedIndex)
        {
            if (this.metaTypePanel.IsModified)
            {
                if (this.metaTypePanel.Verify())
                {
                    this.metaTypePanel.IsModified = false;
                }
                else
                {
                    if (query)
                    {
                        MessageBox.Show(Resources.TypeVerifyWarning, Resources.Warning, MessageBoxButtons.OK);
                    }

                    return;
                }

                editType(typeSelectedIndex);
            }

            if (memberSelectedIndex < 0)
            {
                return;
            }

            if (this.metaPropertyPanel != null && this.metaPropertyPanel.IsModified)
            {
                if (this.metaPropertyPanel.Verify())
                {
                    this.metaPropertyPanel.IsModified = false;
                }
                else
                {
                    if (query)
                    {
                        MessageBox.Show(Resources.PropertyVerifyWarning, Resources.Warning, MessageBoxButtons.OK);
                    }

                    return;
                }
            }
            else if (this.metaMethodPanel != null && this.metaMethodPanel.IsModified)
            {
                if (this.metaMethodPanel.Verify())
                {
                    this.metaMethodPanel.IsModified = false;
                }
                else
                {
                    if (query)
                    {
                        MessageBox.Show(Resources.MethodVerifyWarning, Resources.Warning, MessageBoxButtons.OK);
                    }

                    return;
                }
            }
            else if (this._editEnumMemberPanel != null && this._editEnumMemberPanel.IsModified)
            {
                if (this._editEnumMemberPanel.Verify())
                {
                    this._editEnumMemberPanel.IsModified = false;
                }
                else
                {
                    if (query)
                    {
                        MessageBox.Show(Resources.EnumMemberVerifyWarning, Resources.Warning, MessageBoxButtons.OK);
                    }

                    return;
                }
            }
            else
            {
                return;
            }

            editMember(memberSelectedIndex, resetSelectedIndex);
        }

        private void save(bool refreshWorkspace = false)
        {
            if (!refreshWorkspace)
            {
                apply(true, this.typeListBox.SelectedIndex, this.memberListBox.SelectedIndex, true);
            }

            bool isBlackboardDirty = Workspace.Current.IsBlackboardDirty;

            Workspace.SaveMeta(Workspace.Current);

            if (_isParDirtyBehavior != null)
            {
                Nodes.BehaviorNode root = getCurrentRootNode();

                if (root != null && _isParDirtyBehavior == root && root.IsModified)
                {
                    BehaviorTreeViewDock dock = getCurrentBehaviorTreeDock();

                    if (dock.BehaviorTreeView.Save(false))
                    {
                        _isParDirtyBehavior = null;
                    }
                }
            }

            if (isBlackboardDirty)
            {
                if (refreshWorkspace)
                {
                    // refresh the meta
                    Debug.Check(Workspace.Current != null);
                    MainWindow.Instance.SetWorkspace(Workspace.Current.FileName, false, true);
                }
                else
                {
                    BehaviorTreeViewDock.RefreshAll();
                    PropertiesDock.InspectObject(null, null);
                }
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            save();

            bool exportXML = Workspace.Current.ShouldBeExported("xml");
            bool exportBson = Workspace.Current.ShouldBeExported("bson");
            Workspace.ExportMeta(Workspace.Current, exportXML, exportBson);

            int index = Plugin.GetExporterIndex(Workspace.Current.Language);
            MainWindow.Instance.BehaviorTreeList.ExportTypes(index);

            ErrorInfoDock.WriteExportTypeInfo();
        }

        private bool _initialized = false;

        private void initialize(BehaviorTreeViewDock dock)
        {
            _initialized = false;
            _dock = dock;

            if (Workspace.Current.IsBlackboardDirty)
            {
                MainWindow.Instance.SetWorkspace(Workspace.Current.FileName, false);

                Workspace.Current.IsBlackboardDirty = false;
            }

            this.Text = Resources.MetaStore;

            if (getCurrentBehaviorTreeDock() != null)
            {
                this.Text += " - " + getCurrentBehaviorTreeDock().Text;
            }

            this.typeListBox.Items.Clear();
            this.instanceComboBox.Items.Clear();
            this.memberTypeComboBox.Items.Clear();

            string selectedType = _lastSelectedType;

            if (string.IsNullOrEmpty(_lastSelectedType))
            {
                AgentType agentType = getCurrentViewAgentType();

                if (agentType == null)
                {
                    agentType = _lastAgent;
                }

                if (agentType != null)
                {
                    selectedType = agentType.Name;
                }
            }

            resetAllTypes(selectedType);

            // set all member types
            foreach (string mt in Enum.GetNames(typeof(MemberType)))
            {
                this.memberTypeComboBox.Items.Add(mt);
            }

            this.memberTypeComboBox.SelectedIndex = _lastMemberTypeIndex;

            this.headerFileButton.Visible = (Workspace.Current.Language == "cpp" || Workspace.Current.ShouldBeExported("cpp"));
            this.closeButton.Select();

            this.metaTypePanel.MetaTypeHandler -= metaTypePanel_MetaTypeHandler;
            this.metaTypePanel.MetaTypeHandler += metaTypePanel_MetaTypeHandler;

            _initialized = true;
        }

        private void CheckTypeName()
        {
            object typeObject = this.getSelectedType();

            if (typeObject != null)
            {
                if (typeObject is AgentType)
                {
                    AgentType agentType = (AgentType)typeObject;

                    if (!string.IsNullOrEmpty(agentType.OldName) && agentType.OldName != agentType.Name)
                    {
                        if (this.checkMembersInWorkspace(MetaOperations.CheckAgentName, agentType, null, null, null))
                        {
                            this.resetMembersInWorkspace(MetaOperations.RenameAgentType, agentType, null, null, null);

                            //save();
                        }
                        else
                        {
                            agentType.Name = agentType.OldName;
                            _lastSelectedType = agentType.Name;

                            resetAllTypes(_lastSelectedType);
                        }
                    }
                }
                else if (typeObject is EnumType)
                {
                    EnumType enumType = (EnumType)typeObject;

                    if (!string.IsNullOrEmpty(enumType.OldName) && enumType.OldName != enumType.Name)
                    {
                        if (this.checkMembersInWorkspace(MetaOperations.CheckEnumName, null, enumType, null, null))
                        {
                            this.resetMembersInWorkspace(MetaOperations.RenameEnumType, null, enumType, null, null);

                            //save();
                        }
                        else
                        {
                            enumType.Name = enumType.OldName;
                            _lastSelectedType = enumType.Fullname;

                            resetAllTypes(_lastSelectedType);
                        }
                    }
                }
                else if (typeObject is StructType)
                {
                    StructType structType = (StructType)typeObject;

                    if (!string.IsNullOrEmpty(structType.OldName) && structType.OldName != structType.Name)
                    {
                        if (this.checkMembersInWorkspace(MetaOperations.CheckStructName, null, structType, null, null))
                        {
                            this.resetMembersInWorkspace(MetaOperations.RenameStructType, null, structType, null, null);

                            //save();
                        }
                        else
                        {
                            structType.Name = structType.OldName;
                            _lastSelectedType = structType.Fullname;

                            resetAllTypes(_lastSelectedType);
                        }
                    }
                }
            }
        }

        private void metaTypePanel_MetaTypeHandler()
        {
            CheckTypeModified();
        }

        private void metaPropertyPanel_MetaPropertyNameHandler()
        {
            CheckMemberModified();
        }

        private void metaPropertyPanel_MetaPropertyHandler()
        {
            CheckMemberModified();
        }

        private void metaMethodPanel_MetaMethodNameHandler()
        {
            CheckMemberModified();
        }

        private void resetAllTypes(string selectedType)
        {
            this.typeListBox.Items.Clear();

            int index = 0;

            for (int i = 0; i < Plugin.AgentTypes.Count; ++i)
            {
                AgentType agentType = Plugin.AgentTypes[i];
                string preStr = agentType.IsImplemented ? Member_Str : Exported_Str;

                this.typeListBox.Items.Add(preStr + agentType.Name);

                if (agentType.Name == selectedType)
                {
                    index = i;
                }
            }

            for (int i = 0; i < TypeManager.Instance.Enums.Count; ++i)
            {
                EnumType enumType = TypeManager.Instance.Enums[i];
                string preStr = enumType.IsImplemented ? Member_Str : Exported_Str;

                this.typeListBox.Items.Add(preStr + enumType.Fullname);

                if (enumType.Fullname == selectedType)
                {
                    index = i + Plugin.AgentTypes.Count;
                }
            }

            for (int i = 0; i < TypeManager.Instance.Structs.Count; ++i)
            {
                StructType structType = TypeManager.Instance.Structs[i];
                string preStr = structType.IsImplemented ? Member_Str : Exported_Str;

                this.typeListBox.Items.Add(preStr + structType.Fullname);

                if (structType.Fullname == selectedType)
                {
                    index = i + Plugin.AgentTypes.Count + TypeManager.Instance.Enums.Count;
                }
            }

            if (this.typeListBox.Items.Count > 0)
            {
                this.typeListBox.SelectedIndex = index;
            }
        }

        private BehaviorTreeViewDock getCurrentBehaviorTreeDock()
        {
            return (_initialized || _dock == null) ? BehaviorTreeViewDock.LastFocused : _dock;
        }

        private Nodes.BehaviorNode getCurrentRootNode()
        {
            BehaviorTreeViewDock dock = getCurrentBehaviorTreeDock();
            BehaviorTreeView focusedView = (dock != null) ? dock.BehaviorTreeView : null;
            return (focusedView != null) ? focusedView.RootNode : null;
        }

        private AgentType getCurrentViewAgentType()
        {
            BehaviorTreeViewDock dock = getCurrentBehaviorTreeDock();
            BehaviorTreeView focusedView = (dock != null) ? dock.BehaviorTreeView : null;
            return (focusedView != null && focusedView.RootNode != null) ? focusedView.RootNode.AgentType : null;
        }

        private string getPrefixString(PropertyDef p)
        {
            if (p != null)
            {
                if (p.IsChangeableType)
                {
                    return (p.Type == typeof(object)) ? Empty_Type_Str : Changeable_Type_Str;
                }

                if (p.IsInherited)
                {
                    return Inherited_Str;
                }

                if (p.IsPar)
                {
                    return Par_Str;
                }

                if (p.IsCustomized)
                {
                    return Customized_Str;
                }
            }

            return Member_Str;
        }

        private string getPrefixString(MethodDef m)
        {
            if (m != null)
            {
                if (m.IsChangeableType)
                {
                    return (m.ReturnType == typeof(object)) ? Empty_Type_Str : Changeable_Type_Str;
                }

                if (m.IsInherited)
                {
                    return Inherited_Str;
                }
            }

            return Member_Str;
        }

        private void setMembers()
        {
            this.addMemberButton.Enabled = false;
            this.removeMemberButton.Enabled = false;
            this.upMemberButton.Enabled = false;
            this.downMemberButton.Enabled = false;

            this.memberListBox.Items.Clear();

            int index = this.typeListBox.SelectedIndex;

            if (index > -1 && this.memberTypeComboBox.SelectedIndex > -1)
            {
                string filter = !string.IsNullOrEmpty(memberFilterTextBox.Text) ? memberFilterTextBox.Text.ToLowerInvariant() : string.Empty;

                // agent
                if (index < Plugin.AgentTypes.Count)
                {
                    if (index > 0)
                    {
                        this.addMemberButton.Enabled = true;
                    }

                    AgentType agentType = Plugin.AgentTypes[index];

                    if (agentType != null)
                    {
                        if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                        {
                            Nodes.BehaviorNode root = this.getCurrentRootNode();

                            if (root == null || root.AgentType != agentType)
                            {
                                agentType.ClearPars();
                            }

                            IList<PropertyDef> properties = agentType.GetProperties();

                            foreach (PropertyDef p in properties)
                            {
                                if (p.IsArrayElement)
                                {
                                    continue;
                                }

                                string propName = p.DisplayName.ToLowerInvariant();

                                if (memberFilterCheckBox.Checked && propName.StartsWith(filter) ||
                                    !memberFilterCheckBox.Checked && propName.Contains(filter))
                                {
                                    string disp = getPrefixString(p) + p.DisplayName;
                                    this.memberListBox.Items.Add(new MemberItem(disp, p));
                                }
                            }

                        }
                        else if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Method ||
                                 this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
                        {
                            MethodType methodType = MethodType.Method;

                            if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
                            {
                                methodType = MethodType.Task;
                            }

                            IList<MethodDef> methods = agentType.GetMethods(false, methodType);

                            foreach (MethodDef m in methods)
                            {
                                string methodName = m.DisplayName.ToLowerInvariant();

                                if (memberFilterCheckBox.Checked && methodName.StartsWith(filter) ||
                                    !memberFilterCheckBox.Checked && methodName.Contains(filter))
                                {
                                    string disp = getPrefixString(m) + m.DisplayName;
                                    this.memberListBox.Items.Add(new MemberItem(disp, m));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        // enum
                        if (index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)
                        {
                            EnumType enumType = TypeManager.Instance.Enums[index - Plugin.AgentTypes.Count];

                            this.addMemberButton.Enabled = true;

                            foreach (EnumType.EnumMemberType member in enumType.Members)
                            {
                                string memberName = member.DisplayName.ToLowerInvariant();

                                if (memberFilterCheckBox.Checked && memberName.StartsWith(filter) ||
                                    !memberFilterCheckBox.Checked && memberName.Contains(filter))
                                {
                                    string preStr = enumType.IsCustomized ? Customized_Str : Member_Str;
                                    this.memberListBox.Items.Add(preStr + member.DisplayName);
                                }
                            }
                        }

                        // struct
                        else
                        {
                            StructType structType = TypeManager.Instance.Structs[index - Plugin.AgentTypes.Count - TypeManager.Instance.Enums.Count];

                            this.addMemberButton.Enabled = true;

                            foreach (PropertyDef member in structType.Properties)
                            {
                                string memberName = member.DisplayName.ToLowerInvariant();

                                if (memberFilterCheckBox.Checked && memberName.StartsWith(filter) ||
                                    !memberFilterCheckBox.Checked && memberName.Contains(filter))
                                {
                                    string preStr = member.IsInherited ? Inherited_Str : (structType.IsCustomized ? Customized_Str : Member_Str);
                                    this.memberListBox.Items.Add(preStr + member.DisplayName);
                                }
                            }
                        }
                    }
                }
            }

            this.memberCountLabel.Text = this.memberListBox.Items.Count.ToString();
        }

        private MetaPropertyPanel metaPropertyPanel = null;
        private MetaMethodPanel metaMethodPanel = null;
        private EditEnumMemberPanel _editEnumMemberPanel = null;

        private void initMetaPanel()
        {
            if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
            {
                if (this.metaMethodPanel != null)
                {
                    this.Controls.Remove(this.metaMethodPanel);
                    this.metaMethodPanel = null;
                }

                int index = this.typeListBox.SelectedIndex;

                if (index >= Plugin.AgentTypes.Count && index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)   // enum
                {
                    if (this.metaPropertyPanel != null)
                    {
                        this.Controls.Remove(this.metaPropertyPanel);
                        this.metaPropertyPanel = null;
                    }

                    if (this._editEnumMemberPanel == null)
                    {
                        this._editEnumMemberPanel = new EditEnumMemberPanel();
                        this._editEnumMemberPanel.Hide();
                        this.Controls.Add(this._editEnumMemberPanel);

                        setMetaPanelLocation();
                    }
                }
                else     // agent or struct
                {
                    if (this._editEnumMemberPanel != null)
                    {
                        this.Controls.Remove(this._editEnumMemberPanel);
                        this._editEnumMemberPanel = null;
                    }

                    if (this.metaPropertyPanel == null)
                    {
                        this.metaPropertyPanel = new MetaPropertyPanel();
                        this.metaPropertyPanel.Hide();
                        this.Controls.Add(this.metaPropertyPanel);

                        setMetaPanelLocation();
                    }
                }
            }
            else if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Method ||
                     this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
            {
                if (this.metaPropertyPanel != null)
                {
                    this.Controls.Remove(this.metaPropertyPanel);
                    this.metaPropertyPanel = null;
                }

                if (this._editEnumMemberPanel != null)
                {
                    this.Controls.Remove(this._editEnumMemberPanel);
                    this._editEnumMemberPanel = null;
                }

                if (this.metaMethodPanel == null)
                {
                    this.metaMethodPanel = new MetaMethodPanel();
                    this.metaMethodPanel.Hide();
                    this.Controls.Add(this.metaMethodPanel);

                    setMetaPanelLocation();
                }
            }
            else
            {
                if (this.metaPropertyPanel != null)
                {
                    this.Controls.Remove(this.metaPropertyPanel);
                    this.metaPropertyPanel = null;
                }

                if (this.metaMethodPanel != null)
                {
                    this.Controls.Remove(this.metaMethodPanel);
                    this.metaMethodPanel = null;
                }

                if (this._editEnumMemberPanel != null)
                {
                    this.Controls.Remove(this._editEnumMemberPanel);
                    this._editEnumMemberPanel = null;
                }
            }

            if (this.metaPropertyPanel != null)
            {
                this.metaPropertyPanel.MetaPropertyNameHandler -= metaPropertyPanel_MetaPropertyNameHandler;
                this.metaPropertyPanel.MetaPropertyNameHandler += metaPropertyPanel_MetaPropertyNameHandler;

                this.metaPropertyPanel.MetaPropertyHandler -= metaPropertyPanel_MetaPropertyHandler;
                this.metaPropertyPanel.MetaPropertyHandler += metaPropertyPanel_MetaPropertyHandler;
            }

            if (this.metaMethodPanel != null)
            {
                this.metaMethodPanel.MetaMethodNameHandler -= metaMethodPanel_MetaMethodNameHandler;
                this.metaMethodPanel.MetaMethodNameHandler += metaMethodPanel_MetaMethodNameHandler;
            }
        }

        private void setMetaPanelLocation()
        {
            if (this.metaPropertyPanel != null)
            {
                this.metaPropertyPanel.Left = this.memberListBox.Left;
                this.metaPropertyPanel.Width = this.downMemberButton.Right - this.metaPropertyPanel.Left;
                this.metaPropertyPanel.Top = this.memberListBox.Bottom + 10;
            }
            else if (this.metaMethodPanel != null)
            {
                this.metaMethodPanel.Left = this.memberListBox.Left;
                this.metaMethodPanel.Width = this.downMemberButton.Right - this.metaMethodPanel.Left;
                this.metaMethodPanel.Top = this.memberListBox.Bottom + 10;
            }
            else if (this._editEnumMemberPanel != null)
            {
                this._editEnumMemberPanel.Left = this.memberListBox.Left;
                this._editEnumMemberPanel.Width = this.downMemberButton.Right - this._editEnumMemberPanel.Left;
                this._editEnumMemberPanel.Top = this.memberListBox.Bottom + 10;
            }
        }

        private void typeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.addMemberButton.Enabled = true;
            this.removeTypeButton.Enabled = false;

            int typeIndex = this.typeListBox.SelectedIndex;

            if (typeIndex >= 0)
            {
                _previousSelectedMemberIndex = -1;

                if (_previousSelectedTypeIndex != typeIndex)
                {
                    apply(false, _previousSelectedTypeIndex, -1, false);

                    _previousSelectedTypeIndex = typeIndex;
                }

                this.removeTypeButton.Enabled = true;
                this.instanceComboBox.Enabled = false;
                this.addInstanceButton.Enabled = false;
                this.removeInstanceButton.Enabled = false;

                this.instanceComboBox.Items.Clear();

                if (typeIndex < Plugin.AgentTypes.Count)
                {
                    _lastAgent = Plugin.AgentTypes[typeIndex];

                    AgentType currentViewAgentType = getCurrentViewAgentType();

                    // set all instance names
                    bool hasSelf = false;
                    if (currentViewAgentType != null && currentViewAgentType == _lastAgent)
                    {
                        hasSelf = true;
                        this.instanceComboBox.Items.Add(VariableDef.kSelf);
                    }

                    foreach (Plugin.InstanceName_t instanceName in Plugin.InstanceNames)
                    {
                        if (instanceName.AgentType == _lastAgent && (!hasSelf || instanceName.Name != _lastAgent.Name))
                        {
                            this.instanceComboBox.Items.Add(instanceName.DisplayName);
                        }
                    }

                    if (this.instanceComboBox.Items.Count > 0)
                    {
                        this.instanceComboBox.SelectedIndex = 0;
                    }

                    this.instanceComboBox.Enabled = true;
                    this.addInstanceButton.Enabled = true;
                    this.instanceLabel.Visible = true;
                    this.memberTypeComboBox.Enabled = true;
                }
                else
                {
                    // select property
                    if (this.memberTypeComboBox.Items.Count > 0)
                    {
                        this.memberTypeComboBox.SelectedIndex = 0;
                    }

                    this.memberTypeComboBox.Enabled = false;
                }

                this.metaTypePanel.Initialize(this.getSelectedType());

                setMembers();

                if (this.typeListBox.SelectedIndex == 0)
                {
                    this.removeTypeButton.Enabled = false;
                    this.addMemberButton.Enabled = false;
                }
            }

            if (this.metaPropertyPanel != null)
            {
                this.metaPropertyPanel.Hide();
            }

            if (this.metaMethodPanel != null)
            {
                this.metaMethodPanel.Hide();
            }

            if (this._editEnumMemberPanel != null)
            {
                this._editEnumMemberPanel.Hide();
            }

            initMetaPanel();
        }

        private void memberTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _lastMemberTypeIndex = this.memberTypeComboBox.SelectedIndex;

            setMembers();

            initMetaPanel();
        }

        private void memberFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            setMembers();
        }

        private void memberFilterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            setMembers();
        }

        private PropertyDef getPropertyByIndex(int index)
        {
            if (index > -1 && index < this.memberListBox.Items.Count)
            {
                MemberItem item = this.memberListBox.Items[index] as MemberItem;

                if (item != null)
                {
                    return item.Property;
                }
            }

            return null;
        }

        private MethodDef getMethodByIndex(int index)
        {
            if (index > -1 && index < this.memberListBox.Items.Count)
            {
                MemberItem item = this.memberListBox.Items[index] as MemberItem;

                if (item != null)
                {
                    return item.Method;
                }
            }

            return null;
        }

        private void setDescription()
        {
            this.removeMemberButton.Enabled = false;
            this.upMemberButton.Enabled = false;
            this.downMemberButton.Enabled = false;

            int index = this.typeListBox.SelectedIndex;
            int memberIndex = this.memberListBox.SelectedIndex;

            if (index > -1 && memberIndex > -1)
            {
                // agent
                if (index < Plugin.AgentTypes.Count)
                {
                    AgentType agent = Plugin.AgentTypes[index];

                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        PropertyDef prop = getPropertyByIndex(memberIndex);

                        if (index > 0)
                        {
                            //don't allow to remove automatically added "_$local_task_param_$_0"
                            bool bEnabled = (prop != null && prop.CanBeRemoved());
                            this.removeMemberButton.Enabled = bEnabled;
                            this.upMemberButton.Enabled = bEnabled;
                            this.downMemberButton.Enabled = bEnabled;
                        }

                        if (prop != null)
                        {
                            bool canBeEdit = !prop.IsInherited;

                            if (prop.IsPar)
                            {
                                //don't allow to edit automatically added "_$local_task_param_$_0"
                                if (prop.IsAddedAutomatically)
                                {
                                    canBeEdit = false;
                                }
                            }

                            Nodes.Behavior root = getCurrentRootNode() as Nodes.Behavior;

                            if (this.metaPropertyPanel != null)
                            {
                                this.metaPropertyPanel.Initialize(canBeEdit, agent, null, prop, root != null);
                                this.metaPropertyPanel.Show();
                            }
                        }
                    }
                    else if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Method ||
                             this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
                    {
                        MethodDef method = getMethodByIndex(memberIndex);

                        if (index > 0)
                        {
                            bool bEnabled = (method != null && method.CanBeRemoved());
                            this.removeMemberButton.Enabled = bEnabled;
                            this.upMemberButton.Enabled = bEnabled;
                            this.downMemberButton.Enabled = bEnabled;
                        }

                        if (method != null && this.metaMethodPanel != null)
                        {
                            this.metaMethodPanel.Initialize(index > 0 && !method.IsInherited, agent, method, (MemberType)this.memberTypeComboBox.SelectedIndex);
                            this.metaMethodPanel.Show();
                        }
                    }
                }
                else
                {
                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        // enum
                        if (index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)
                        {
                            EnumType enumType = TypeManager.Instance.Enums[index - Plugin.AgentTypes.Count];
                            EnumType.EnumMemberType enumMember = enumType.Members[memberIndex];

                            if (this._editEnumMemberPanel != null)
                            {
                                this._editEnumMemberPanel.Initialize(enumType, enumMember);
                                this._editEnumMemberPanel.Show();
                            }

                            bool bEnabled = enumType.CanBeRemoved();
                            this.removeMemberButton.Enabled = bEnabled;
                            this.upMemberButton.Enabled = bEnabled;
                            this.downMemberButton.Enabled = bEnabled;
                        }
                        // struct
                        else
                        {
                            StructType structType = TypeManager.Instance.Structs[index - Plugin.AgentTypes.Count - TypeManager.Instance.Enums.Count];
                            PropertyDef structProp = structType.Properties[memberIndex];
                            bool bEnabled = structProp.CanBeRemoved();

                            if (this.metaPropertyPanel != null)
                            {
                                this.metaPropertyPanel.Initialize(bEnabled, null, structType, structProp, false);
                                this.metaPropertyPanel.Show();
                            }

                            this.removeMemberButton.Enabled = bEnabled;
                            this.upMemberButton.Enabled = bEnabled;
                            this.downMemberButton.Enabled = bEnabled;
                        }
                    }
                }
            }
        }

        private bool isMemberModified()
        {
            if (this.metaPropertyPanel != null)
            {
                return this.metaPropertyPanel.IsModified;
            }
            else if (this.metaMethodPanel != null)
            {
                return this.metaMethodPanel.IsModified;
            }
            else if (this._editEnumMemberPanel != null)
            {
                return this._editEnumMemberPanel.IsModified;
            }

            return false;
        }

        private delegate void preSelectMemberDelegate();

        private void CheckTypeModified()
        {
            if (_previousSelectedTypeIndex >= 0 && this.metaTypePanel.IsModified)
            {
                apply(false, _previousSelectedTypeIndex, -1, false);
            }
        }

        private void CheckMemberModified()
        {
            if (_previousSelectedMemberIndex >= 0 && this.isMemberModified())
            {
                apply(false, -1, _previousSelectedMemberIndex, false);
            }
        }

        private void postSelectMember()
        {
            int currentMemberIndex = this.memberListBox.SelectedIndex;

            setDescription();

            _previousSelectedMemberIndex = currentMemberIndex;

            if (this.memberListBox.SelectedIndex != currentMemberIndex)
            {
                this.memberListBox.SelectedIndex = currentMemberIndex;
            }
        }

        private void memberListBox_MouseDown(object sender, MouseEventArgs e)
        {
            //CheckMemberModified();

            postSelectMember();

            if (this.memberListBox.SelectedItem != null)
            {
                string instanceName = this.instanceComboBox.Text;

                if (!string.IsNullOrEmpty(instanceName))
                {
                    instanceName = instanceName.Replace(Inherited_Str, "");
                    instanceName = instanceName.Replace(Par_Str, "");
                    instanceName = instanceName.Replace(Customized_Str, "");
                    instanceName = instanceName.Replace(Member_Str, "");
                    instanceName = instanceName.Replace(Empty_Type_Str, "");
                    instanceName = instanceName.Replace(Changeable_Type_Str, "");
                    instanceName = instanceName.Trim();

                    MemberItem item = this.memberListBox.SelectedItem as MemberItem;
                    Debug.Check(item.Property != null || item.Method != null);
                    if (item.Property != null || item.Method != null)
                    {
                        string memberName = (item.Property != null) ? item.Property.Name : item.Method.Name;

                        memberName = instanceName + "." + memberName;

                        // method
                        if (this.memberTypeComboBox.SelectedIndex != (int)MemberType.Property)
                        {
                            memberName += "()";
                        }

                        memberListBox.DoDragDrop(memberName, DragDropEffects.Move);
                    }
                }
            }
        }

        private void memberListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                removeSelectedMember();
            }
            else
            {
                CheckMemberModified();
            }
        }

        private void memberListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            postSelectMember();
        }

        private object getSelectedType()
        {
            int index = this.typeListBox.SelectedIndex;

            if (index < 0)
            {
                return null;
            }

            if (index < Plugin.AgentTypes.Count)
            {
                return Plugin.AgentTypes[index];
            }

            index -= Plugin.AgentTypes.Count;

            if (index < TypeManager.Instance.Enums.Count)
            {
                return TypeManager.Instance.Enums[index];
            }

            index -= TypeManager.Instance.Enums.Count;

            return TypeManager.Instance.Structs[index];
        }

        private void addTypeButton_Click(object sender, EventArgs e)
        {
            MetaTypeDialog typeDialog = new MetaTypeDialog(null);

            if (DialogResult.OK == typeDialog.ShowDialog())
            {
                Workspace.Current.IsBlackboardDirty = true;

                MetaTypePanel.MetaTypes metaType = typeDialog.GetMetaType();

                if (metaType == MetaTypePanel.MetaTypes.Agent)
                {
                    AgentType agentType = typeDialog.GetCustomizedAgent();
                    Debug.Check(agentType != null);
                    if (agentType != null)
                    {
                        Plugin.AgentTypes.Add(agentType);

                        resetAllTypes(agentType.Name);

                        _lastSelectedType = agentType.Name;
                    }
                }
                else if (metaType == MetaTypePanel.MetaTypes.Enum)
                {
                    EnumType enumType = typeDialog.GetCustomizedEnum();
                    Debug.Check(enumType != null);
                    if (enumType != null)
                    {
                        TypeManager.Instance.Enums.Add(enumType);

                        resetAllTypes(enumType.Fullname);

                        _lastSelectedType = enumType.Fullname;
                    }
                }
                else if (metaType == MetaTypePanel.MetaTypes.Struct)
                {
                    StructType structType = typeDialog.GetCustomizedStruct();
                    Debug.Check(structType != null);
                    if (structType != null)
                    {
                        TypeManager.Instance.Structs.Add(structType);

                        resetAllTypes(structType.Fullname);

                        _lastSelectedType = structType.Fullname;
                    }
                }

                // refresh the workspace to load the type
                save(true);
            }
        }

        private void editType(int typeSelectedIndex)
        {
            if (typeSelectedIndex > -1)
            {
                Workspace.Current.IsBlackboardDirty = true;

                MetaTypePanel.MetaTypes metaType = this.metaTypePanel.GetMetaType();

                if (metaType == MetaTypePanel.MetaTypes.Agent)
                {
                    AgentType agentType = this.metaTypePanel.GetCustomizedAgent();
                    Debug.Check(agentType != null);

                    if (agentType != null)
                    {
                        string preStr = agentType.IsImplemented ? Member_Str : Exported_Str;

                        this.typeListBox.Items[typeSelectedIndex] = preStr + agentType.Name;

                        _lastSelectedType = agentType.Name;
                    }
                }
                else if (metaType == MetaTypePanel.MetaTypes.Enum)
                {
                    EnumType enumType = this.metaTypePanel.GetCustomizedEnum();
                    Debug.Check(enumType != null);

                    if (enumType != null)
                    {
                        string preStr = enumType.IsImplemented ? Member_Str : Exported_Str;

                        this.typeListBox.Items[typeSelectedIndex] = preStr + enumType.Fullname;

                        _lastSelectedType = enumType.Fullname;
                    }
                }
                else if (metaType == MetaTypePanel.MetaTypes.Struct)
                {
                    StructType structType = this.metaTypePanel.GetCustomizedStruct();
                    Debug.Check(structType != null);

                    if (structType != null)
                    {
                        string preStr = structType.IsImplemented ? Member_Str : Exported_Str;

                        this.typeListBox.Items[typeSelectedIndex] = preStr + structType.Fullname;

                        _lastSelectedType = structType.Fullname;
                    }
                }

                CheckTypeName();

                // refresh the workspace to load the type
                save(true);
            }
        }

        private void removeTypeButton_Click(object sender, EventArgs e)
        {
            int index = this.typeListBox.SelectedIndex;

            if (index > -1 &&
                DialogResult.OK == MessageBox.Show(Resources.TypeRemoveWarning, Resources.Warning, MessageBoxButtons.OKCancel))
            {
                if (index < Plugin.AgentTypes.Count)
                {
                    AgentType agent = Plugin.AgentTypes[index];

                    if (agent != null)
                    {
                        Workspace.Current.IsBlackboardDirty = true;

                        Plugin.AgentTypes.RemoveAt(index);

                        this.typeListBox.Items.RemoveAt(index);
                    }
                }
                else
                {
                    if (index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)
                    {
                        TypeManager.Instance.Enums.RemoveAt(index - Plugin.AgentTypes.Count);

                        this.typeListBox.Items.RemoveAt(index);
                    }
                    else
                    {
                        TypeManager.Instance.Structs.RemoveAt(index - Plugin.AgentTypes.Count - TypeManager.Instance.Enums.Count);

                        this.typeListBox.Items.RemoveAt(index);
                    }
                }

                if (index >= this.typeListBox.Items.Count)
                {
                    index = this.typeListBox.Items.Count - 1;
                }

                this.typeListBox.SelectedIndex = index;

                _lastSelectedType = "";

                // refresh the workspace to load the type
                //save(true);
            }
        }

        private int getLastParIndex()
        {
            int index = 0;

            for (int i = 0; i < this.memberListBox.Items.Count; ++i)
            {
                MemberItem item = this.memberListBox.Items[i] as MemberItem;

                if (item.Property != null && item.Property.IsPar)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

        private void addMember()
        {
            int index = this.typeListBox.SelectedIndex;

            if (index > -1)
            {
                bool bAdded = false;
                bool bAddAgent = false;

                // agent
                if (index < Plugin.AgentTypes.Count)
                {
                    AgentType agent = Plugin.AgentTypes[index];

                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        Nodes.Behavior root = getCurrentRootNode() as Nodes.Behavior;
                        MetaPropertyDialog propertyDialog = new MetaPropertyDialog(true, agent, null, null, root != null);

                        if (DialogResult.OK == propertyDialog.ShowDialog())
                        {
                            PropertyDef prop = propertyDialog.GetProperty();
                            Debug.Check(prop != null);

                            if (prop != null && agent.AddProperty(prop) >= 0)
                            {
                                bAdded = true;
                                bAddAgent = true;

                                if (prop.IsPar)
                                {
                                    if (root != null)
                                    {
                                        root.LocalVars.Add(prop as ParInfo);

                                        UndoManager.Save(root);

                                        _isParDirtyBehavior = root;
                                    }

                                    int lastParIndex = getLastParIndex();
                                    this.memberListBox.Items.Insert(lastParIndex, new MemberItem(Par_Str + prop.DisplayName, prop));
                                    this.memberListBox.SelectedIndex = lastParIndex;
                                }
                                else
                                {
                                    this.memberListBox.Items.Add(new MemberItem(Member_Str + prop.DisplayName, prop));
                                    this.memberListBox.SelectedIndex = this.memberListBox.Items.Count - 1;
                                }

                                _lastSelectedType = "";
                            }
                        }
                    }
                    else if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Method ||
                             this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
                    {
                        MetaMethodDialog methodDialog = new MetaMethodDialog(agent, null, (MemberType)this.memberTypeComboBox.SelectedIndex);

                        if (DialogResult.OK == methodDialog.ShowDialog())
                        {
                            MethodDef method = methodDialog.GetMethod();
                            Debug.Check(method != null);

                            if (agent.AddMethod(method))
                            {
                                bAdded = true;

                                this.memberListBox.Items.Add(new MemberItem(Member_Str + method.DisplayName, method));
                                this.memberListBox.SelectedIndex = this.memberListBox.Items.Count - 1;

                                _lastSelectedType = "";
                            }
                        }
                    }
                }
                else
                {
                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        // enum
                        if (index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)
                        {
                            EnumType enumType = TypeManager.Instance.Enums[index - Plugin.AgentTypes.Count];
                            EditEnumMemberDialog enumMemberDialog = new EditEnumMemberDialog(enumType, null);

                            if (DialogResult.OK == enumMemberDialog.ShowDialog())
                            {
                                bAdded = true;

                                EnumType.EnumMemberType enumMember = enumMemberDialog.GetEnumMember();
                                Debug.Check(enumMember != null);

                                if (enumMember != null)
                                {
                                    enumType.Members.Add(enumMember);

                                    this.memberListBox.Items.Add(Member_Str + enumMember.DisplayName);
                                    this.memberListBox.SelectedIndex = this.memberListBox.Items.Count - 1;

                                    _lastSelectedType = enumType.Fullname;
                                }
                            }
                        }
                        // struct
                        else
                        {
                            StructType structType = TypeManager.Instance.Structs[index - Plugin.AgentTypes.Count - TypeManager.Instance.Enums.Count];
                            MetaPropertyDialog propertyDialog = new MetaPropertyDialog(true, null, structType, null, false);

                            if (DialogResult.OK == propertyDialog.ShowDialog())
                            {
                                bAdded = true;

                                PropertyDef prop = propertyDialog.GetProperty();
                                Debug.Check(prop != null);

                                if (prop != null)
                                {
                                    structType.AddProperty(prop);

                                    this.memberListBox.Items.Add(Member_Str + prop.DisplayName);
                                    this.memberListBox.SelectedIndex = this.memberListBox.Items.Count - 1;

                                    _lastSelectedType = structType.Fullname;
                                }
                            }
                        }
                    }
                }

                if (bAdded)
                {
                    Workspace.Current.IsBlackboardDirty = true;

                    PropertiesDock.UpdatePropertyGrids();

                    if (!bAddAgent)
                    {
                        // refresh the workspace to load the type
                        save(true);
                    }
                }
            }
        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            addMember();
        }

        IList<Nodes.BehaviorNode> getAllBehaviors(PropertyDef property)
        {
            List<Nodes.BehaviorNode> allBehaviors = new List<Nodes.BehaviorNode>();

            if (property != null && property.IsPar)
            {
                allBehaviors.Add(getCurrentRootNode());
            }
            else
            {
                BehaviorTreeList behaviorTreeList = BehaviorManager.Instance as BehaviorTreeList;

                if (behaviorTreeList != null)
                {
                    foreach (Nodes.BehaviorNode behavior in behaviorTreeList.GetAllOpenedBehaviors())
                    {
                        if (!allBehaviors.Contains(behavior))
                        {
                            allBehaviors.Add(behavior);
                        }
                    }

                    foreach (Nodes.BehaviorNode behavior in behaviorTreeList.GetAllBehaviors())
                    {
                        if (!allBehaviors.Contains(behavior))
                        {
                            allBehaviors.Add(behavior);
                        }
                    }
                }
            }

            return allBehaviors;
        }

        private bool checkTypesInWorkspace(MetaOperations metaOperation, AgentType agentType, BaseType baseType)
        {
            Debug.Check(metaOperation == MetaOperations.CheckAgentName || metaOperation == MetaOperations.CheckEnumName || metaOperation == MetaOperations.CheckStructName);

            string oldTypeName = "";
            if (agentType != null)
            {
                oldTypeName = agentType.OldName;
            }
            else if (baseType != null)
            {
                oldTypeName = baseType.OldName;
            }
            else
            {
                Debug.Check(false);
            }

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                foreach (PropertyDef prop in agent.GetProperties())
                {
                    if (oldTypeName == prop.NativeType)
                    {
                        return true;
                    }
                }

                foreach (MethodDef method in agent.GetMethods())
                {
                    if (oldTypeName == method.NativeReturnType)
                    {
                        return true;
                    }

                    foreach (MethodDef.Param param in method.Params)
                    {
                        if (oldTypeName == param.NativeType)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void resetTypesInWorkspace(MetaOperations metaOperation, AgentType agentType, BaseType baseType)
        {
            Debug.Check(metaOperation == MetaOperations.RenameAgentType || metaOperation == MetaOperations.RenameEnumType || metaOperation == MetaOperations.RenameStructType);

            string oldTypeName = "";
            string curTypeName = "";
            if (agentType != null)
            {
                oldTypeName = agentType.OldName;
                curTypeName = agentType.Name;
            }
            else if (baseType != null)
            {
                oldTypeName = baseType.OldName;
                curTypeName = baseType.Name;
            }
            else
            {
                Debug.Check(false);
            }

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                foreach (PropertyDef prop in agent.GetProperties())
                {
                    if (oldTypeName == prop.NativeType)
                    {
                        prop.NativeType = curTypeName;
                    }
                }

                foreach (MethodDef method in agent.GetMethods())
                {
                    if (oldTypeName == method.NativeReturnType)
                    {
                        method.NativeReturnType = curTypeName;
                    }

                    foreach (MethodDef.Param param in method.Params)
                    {
                        if (oldTypeName == param.NativeType)
                        {
                            param.NativeType = curTypeName;
                        }
                    }
                }
            }
        }

        private bool checkMembersInWorkspace(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            Debug.Check(metaOperation == MetaOperations.CheckAgentName || metaOperation == MetaOperations.CheckEnumName || metaOperation == MetaOperations.CheckStructName ||
                metaOperation == MetaOperations.CheckProperty || metaOperation == MetaOperations.CheckMethod);

            string warningInfo = Resources.ModifyMemberWarning;
            if (metaOperation == MetaOperations.CheckAgentName || metaOperation == MetaOperations.CheckEnumName || metaOperation == MetaOperations.CheckStructName)
            {
                warningInfo = Resources.ModifyTypeWarning;
            }

            bool bRet = false;
            if (metaOperation == MetaOperations.CheckAgentName || metaOperation == MetaOperations.CheckEnumName || metaOperation == MetaOperations.CheckStructName)
            {
                bRet = checkTypesInWorkspace(metaOperation, agentType, baseType);

                if (bRet)
                {
                    return DialogResult.Yes == MessageBox.Show(warningInfo, Resources.Warning, MessageBoxButtons.YesNo);
                }
            }

            foreach (Nodes.BehaviorNode behavior in getAllBehaviors(property))
            {
                if (behavior != null && behavior is Nodes.Node)
                {
                    bRet = ((Nodes.Node)behavior).ResetMembers(metaOperation, agentType, baseType, method, property);

                    if (bRet)
                    {
                        return DialogResult.Yes == MessageBox.Show(warningInfo, Resources.Warning, MessageBoxButtons.YesNo);
                    }
                }
            }

            return true;
        }

        private void resetMembersInWorkspace(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            Debug.Check(metaOperation == MetaOperations.RenameAgentType || metaOperation == MetaOperations.RenameEnumType || metaOperation == MetaOperations.RenameStructType ||
                metaOperation == MetaOperations.ChangeProperty || metaOperation == MetaOperations.ChangeMethod ||
                metaOperation == MetaOperations.RemoveProperty || metaOperation == MetaOperations.RemoveMethod);

            if (metaOperation == MetaOperations.RenameAgentType || metaOperation == MetaOperations.RenameEnumType || metaOperation == MetaOperations.RenameStructType)
            {
                resetTypesInWorkspace(metaOperation, agentType, baseType);

                if (agentType != null)
                {
                    agentType.OldName = "";
                }
                else if (baseType != null)
                {
                    baseType.OldName = "";
                }
            }

            foreach (Nodes.BehaviorNode behavior in getAllBehaviors(property))
            {
                if (behavior != null && behavior is Nodes.Node)
                {
                    bool bReset = ((Nodes.Node)behavior).ResetMembers(metaOperation, agentType, baseType, method, property);

                    if (bReset)
                    {
                        UndoManager.Save(behavior);
                    }
                }
            }
        }

        private void editMember(int selectedMemberIndex, bool resetMemberSelectedIndex)
        {
            int index = this.typeListBox.SelectedIndex;
            int memberIndex = selectedMemberIndex;

            if (index > -1 && memberIndex > -1)
            {
                // agent
                if (index < Plugin.AgentTypes.Count)
                {
                    AgentType agent = Plugin.AgentTypes[index];
                    MemberItem item = this.memberListBox.Items[memberIndex] as MemberItem;
                    bool bEdit = false;

                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        PropertyDef prop = getPropertyByIndex(memberIndex);

                        if (prop != null && this.metaPropertyPanel != null)
                        {
                            bool canBeEdit = true;

                            if (prop.IsPar)
                            {
                                //don't allow to edit automatically added "_$local_task_param_$_0"
                                if (prop.IsAddedAutomatically)
                                {
                                    canBeEdit = false;
                                }
                            }

                            Nodes.Behavior root = getCurrentRootNode() as Nodes.Behavior;

                            PropertyDef curProp = this.metaPropertyPanel.GetProperty();
                            Debug.Check(curProp != null);

                            if (curProp != null && (prop.IsChangeableType || canBeEdit) && (curProp.Name == prop.Name || this.metaPropertyPanel.ShouldCheckMembersInWorkspace() && checkMembersInWorkspace(MetaOperations.CheckProperty, agent, null, null, curProp)))
                            {
                                if (curProp.IsPar || curProp.IsPar != prop.IsPar)
                                {
                                    _isParDirtyBehavior = root;
                                }

                                if (curProp.Type != prop.Type)
                                {
                                    resetMembersInWorkspace(MetaOperations.RemoveProperty, agent, null, null, curProp);
                                }
                                else if (curProp.Name != prop.Name)
                                {
                                    resetMembersInWorkspace(MetaOperations.ChangeProperty, agent, null, null, curProp);
                                }

                                if (Plugin.IsArrayType(prop.Type))
                                {
                                    PropertyDef elementProperty = agent.GetPropertyByName(prop.BasicName + "[]");

                                    if (elementProperty != null && elementProperty.IsArrayElement)
                                    {
                                        elementProperty.SetArrayElement(curProp);
                                    }
                                }

                                string oldPropName = prop.Name;
                                prop.CopyFrom(curProp);

                                if (string.IsNullOrEmpty(prop.OldName) && oldPropName != prop.Name)
                                {
                                    prop.OldName = oldPropName;
                                }

                                item.DisplayName = getPrefixString(prop) + prop.DisplayName;
                                bEdit = true;

                                if (curProp.IsPar != prop.IsPar)
                                {
                                    bEdit = false;
                                    Workspace.Current.IsBlackboardDirty = true;

                                    this.memberListBox.Items.RemoveAt(memberIndex);

                                    if (curProp.IsPar)
                                    {
                                        if (agent.RemoveProperty(prop))
                                        {
                                            if (root != null)
                                            {
                                                root.LocalVars.Add(curProp as ParInfo);

                                                UndoManager.Save(root);
                                            }

                                            int lastParIndex = getLastParIndex();
                                            this.memberListBox.Items.Insert(lastParIndex, new MemberItem(Par_Str + curProp.DisplayName, curProp));

                                            if (resetMemberSelectedIndex)
                                            {
                                                this.memberListBox.SelectedIndex = lastParIndex;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (root != null && root.LocalVars.Remove(prop as ParInfo))
                                        {
                                            UndoManager.Save(root);
                                        }

                                        agent.RemoveProperty(prop);
                                        agent.AddProperty(curProp);

                                        this.memberListBox.Items.Add(new MemberItem(Member_Str + curProp.DisplayName, curProp));

                                        if (resetMemberSelectedIndex)
                                        {
                                            this.memberListBox.SelectedIndex = this.memberListBox.Items.Count - 1;
                                        }
                                    }

                                    BehaviorTreeViewDock.RefreshAll();
                                    PropertiesDock.UpdatePropertyGrids();
                                }
                                else
                                {
                                    if (curProp.IsPar)
                                    {
                                        if (root != null)
                                        {
                                            UndoManager.Save(root);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Method ||
                             this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
                    {
                        MethodDef method = getMethodByIndex(memberIndex);

                        if (method != null && this.metaMethodPanel != null)
                        {
                            MethodDef curMethod = this.metaMethodPanel.GetMethod();
                            Debug.Check(curMethod != null);

                            if (curMethod != null && (curMethod.Name == method.Name ||
                                this.metaMethodPanel.ShouldCheckMembersInWorkspace() && checkMembersInWorkspace(MetaOperations.CheckMethod, agent, null, curMethod, null)))
                            {
                                resetMembersInWorkspace(MetaOperations.ChangeMethod, agent, null, curMethod, null);

                                string oldMethodName = method.Name;
                                method.CopyFrom(curMethod);

                                if (string.IsNullOrEmpty(method.OldName) && oldMethodName != method.Name)
                                {
                                    method.OldName = oldMethodName;
                                }

                                item.DisplayName = getPrefixString(method) + method.DisplayName;
                                bEdit = true;
                            }
                        }
                    }

                    if (bEdit)
                    {
                        _lastSelectedType = "";

                        Workspace.Current.IsBlackboardDirty = true;

                        // update the memberListBox
                        this.memberListBox.Items.RemoveAt(memberIndex);
                        this.memberListBox.Items.Insert(memberIndex, item);
                        this.memberListBox.SelectedIndex = memberIndex;

                        BehaviorTreeViewDock.RefreshAll();
                        PropertiesDock.UpdatePropertyGrids();
                    }

                }
                else
                {
                    bool bEdit = false;

                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        // enum
                        if (index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)
                        {
                            if (this._editEnumMemberPanel != null)
                            {
                                EnumType.EnumMemberType enumMember = this._editEnumMemberPanel.GetEnumMember();
                                Debug.Check(enumMember != null);

                                if (enumMember != null)
                                {
                                    EnumType enumType = TypeManager.Instance.Enums[index - Plugin.AgentTypes.Count];
                                    enumType.Members[memberIndex] = enumMember;

                                    string preStr = enumType.IsCustomized ? Customized_Str : Member_Str;
                                    this.memberListBox.Items[memberIndex] = preStr + enumMember.DisplayName;

                                    _lastSelectedType = enumType.Name;

                                    if (enumType.IsCustomized)
                                    {
                                        bEdit = true;

                                        // refresh the workspace to load the type
                                        save(true);
                                    }
                                }
                            }
                        }
                        // struct
                        else
                        {
                            if (this.metaPropertyPanel != null)
                            {
                                Workspace.Current.IsBlackboardDirty = true;

                                PropertyDef prop = this.metaPropertyPanel.GetProperty();
                                Debug.Check(prop != null);

                                if (prop != null)
                                {
                                    StructType structType = TypeManager.Instance.Structs[index - Plugin.AgentTypes.Count - TypeManager.Instance.Enums.Count];
                                    if (structType != null)
                                    {
                                        structType.Properties[memberIndex].CopyFrom(prop);

                                        string preStr = structType.IsCustomized ? Customized_Str : Member_Str;
                                        this.memberListBox.Items[memberIndex] = preStr + prop.DisplayName;

                                        _lastSelectedType = structType.Name;

                                        if (structType.IsCustomized)
                                        {
                                            bEdit = true;

                                            // refresh the workspace to load the type
                                            save(true);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (bEdit)
                    {
                        Workspace.Current.IsBlackboardDirty = true;

                        BehaviorTreeViewDock.RefreshAll();
                        PropertiesDock.UpdatePropertyGrids();
                    }
                }
            }
        }

        private void removeSelectedMember()
        {
            if (!this.removeMemberButton.Enabled)
                return;

            int index = this.typeListBox.SelectedIndex;
            int memberIndex = this.memberListBox.SelectedIndex;

            if (index > -1 && memberIndex > -1)
            {
                // agent
                if (index < Plugin.AgentTypes.Count)
                {
                    AgentType agent = Plugin.AgentTypes[index];
                    bool bRemoved = false;

                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        PropertyDef prop = getPropertyByIndex(memberIndex);

                        if (prop != null)
                        {
                            prop.OldName = prop.Name;
                        }

                        if (prop != null && prop.CanBeRemoved() &&
                            checkMembersInWorkspace(MetaOperations.CheckProperty, agent, null, null, prop) &&
                            agent.RemoveProperty(prop))
                        {
                            bRemoved = true;

                            this.memberListBox.Items.RemoveAt(memberIndex);

                            if (prop.IsPar)
                            {
                                Nodes.BehaviorNode root = getCurrentRootNode();

                                if (root != null && ((Nodes.Node)root).LocalVars.Remove(prop as ParInfo))
                                {
                                    UndoManager.Save(root);

                                    _isParDirtyBehavior = root;
                                }
                            }

                            if (memberIndex >= this.memberListBox.Items.Count)
                            {
                                memberIndex = this.memberListBox.Items.Count - 1;
                            }

                            this.memberListBox.SelectedIndex = memberIndex;

                            resetMembersInWorkspace(MetaOperations.RemoveProperty, agent, null, null, prop);
                        }

                    }
                    else if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Method ||
                             this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
                    {
                        MethodDef method = getMethodByIndex(memberIndex);

                        if (method != null)
                        {
                            method.OldName = method.Name;
                        }

                        if (method != null && method.CanBeRemoved() &&
                            checkMembersInWorkspace(MetaOperations.CheckMethod, agent, null, method, null) &&
                            agent.RemoveMethod(method))
                        {
                            bRemoved = true;

                            this.memberListBox.Items.RemoveAt(memberIndex);

                            if (memberIndex >= this.memberListBox.Items.Count)
                            {
                                memberIndex = this.memberListBox.Items.Count - 1;
                            }

                            this.memberListBox.SelectedIndex = memberIndex;

                            resetMembersInWorkspace(MetaOperations.RemoveProperty, agent, null, method, null);
                        }
                    }

                    if (bRemoved)
                    {
                        setDescription();

                        _lastSelectedType = "";

                        Workspace.Current.IsBlackboardDirty = true;

                        BehaviorTreeViewDock.RefreshAll();
                        PropertiesDock.UpdatePropertyGrids();
                    }

                }
                else
                {
                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        // enum
                        if (index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)
                        {
                            EnumType enumType = TypeManager.Instance.Enums[index - Plugin.AgentTypes.Count];

                            if (enumType.CanBeRemoved())
                            {
                                enumType.Members.RemoveAt(memberIndex);

                                _lastSelectedType = enumType.Name;
                            }
                        }

                        // struct
                        else
                        {
                            StructType structType = TypeManager.Instance.Structs[index - Plugin.AgentTypes.Count - TypeManager.Instance.Enums.Count];

                            if (structType.CanBeRemoved())
                            {
                                structType.RemoveProperty(structType.Properties[memberIndex].BasicName);

                                _lastSelectedType = structType.Name;
                            }
                        }

                        this.memberListBox.Items.RemoveAt(memberIndex);

                        if (memberIndex >= this.memberListBox.Items.Count)
                        {
                            memberIndex = this.memberListBox.Items.Count - 1;
                        }

                        this.memberListBox.SelectedIndex = memberIndex;

                        setDescription();

                        Workspace.Current.IsBlackboardDirty = true;

                        BehaviorTreeViewDock.RefreshAll();
                        PropertiesDock.UpdatePropertyGrids();

                        // refresh the workspace to load the type
                        save(true);
                    }
                }

                this.memberCountLabel.Text = this.memberListBox.Items.Count.ToString();
            }
        }

        private void removeMemberButton_Click(object sender, EventArgs e)
        {
            removeSelectedMember();
        }

        private void swapTwoProperties(int index1, int index2)
        {
            int index = this.typeListBox.SelectedIndex;

            if (index > -1 && index1 > -1 && index1 < this.memberListBox.Items.Count &&
                index2 > -1 && index2 < this.memberListBox.Items.Count)
            {
                AgentType agent = Plugin.AgentTypes[index];
                bool swapSucceeded = false;

                if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                {
                    PropertyDef prop1 = getPropertyByIndex(index1);
                    PropertyDef prop2 = getPropertyByIndex(index2);

                    if (prop1 != null && prop2 != null)
                    {
                        swapSucceeded = agent.SwapTwoProperties(prop1, prop2);

                        if (swapSucceeded && prop1.IsPar)
                        {
                            if (prop1.IsPar && prop2.IsPar)
                            {
                                Nodes.BehaviorNode root = getCurrentRootNode();

                                if (root != null)
                                {
                                    Nodes.Node node = (Nodes.Node)root;

                                    ParInfo prePar = node.LocalVars[index2];
                                    node.LocalVars[index2] = node.LocalVars[index1];
                                    node.LocalVars[index1] = prePar;

                                    UndoManager.Save(root);

                                    _isParDirtyBehavior = root;
                                }
                            }
                        }
                    }

                }
                else if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Method ||
                         this.memberTypeComboBox.SelectedIndex == (int)MemberType.Task)
                {
                    MethodDef method1 = getMethodByIndex(index1);
                    MethodDef method2 = getMethodByIndex(index2);

                    if (method1 != null && method2 != null)
                    {
                        swapSucceeded = agent.SwapTwoMethods(method1, method2);
                    }
                }

                if (swapSucceeded)
                {
                    object preItem = this.memberListBox.Items[index2];
                    this.memberListBox.Items[index2] = this.memberListBox.Items[index1];
                    this.memberListBox.Items[index1] = preItem;

                    this.memberListBox.SelectedIndex = index2;

                    Workspace.Current.IsBlackboardDirty = true;
                }
            }
        }

        private void moveCustomizedType(EnumType enumType, StructType structType, int sourceIndex, int targetIndex)
        {
            if (sourceIndex >= 0 && sourceIndex < this.memberListBox.Items.Count &&
                targetIndex >= 0 && targetIndex < this.memberListBox.Items.Count)
            {
                if (enumType != null)
                {
                    EnumType.EnumMemberType enumMember = enumType.Members[sourceIndex];
                    enumType.Members.RemoveAt(sourceIndex);
                    enumType.Members.Insert(targetIndex, enumMember);

                    Workspace.Current.IsBlackboardDirty = true;

                }
                else if (structType != null)
                {
                    PropertyDef structMember = structType.Properties[sourceIndex];
                    structType.Properties.RemoveAt(sourceIndex);
                    structType.Properties.Insert(targetIndex, structMember);

                    Workspace.Current.IsBlackboardDirty = true;
                }

                object item = this.memberListBox.Items[sourceIndex];
                this.memberListBox.Items.RemoveAt(sourceIndex);
                this.memberListBox.Items.Insert(targetIndex, item);

                this.memberListBox.SelectedIndex = targetIndex;
            }
        }

        private void moveMember(int sourceIndex, int targetIndex)
        {
            int index = this.typeListBox.SelectedIndex;
            int memberIndex = this.memberListBox.SelectedIndex;

            if (index > -1 && memberIndex > -1)
            {
                // agent
                if (index < Plugin.AgentTypes.Count)
                {
                    swapTwoProperties(sourceIndex, targetIndex);

                }
                else
                {
                    if (this.memberTypeComboBox.SelectedIndex == (int)MemberType.Property)
                    {
                        // enum
                        if (index - Plugin.AgentTypes.Count < TypeManager.Instance.Enums.Count)
                        {
                            EnumType enumType = TypeManager.Instance.Enums[index - Plugin.AgentTypes.Count];

                            moveCustomizedType(enumType, null, sourceIndex, targetIndex);
                        }

                        // struct
                        else
                        {
                            StructType structType = TypeManager.Instance.Structs[index - Plugin.AgentTypes.Count - TypeManager.Instance.Enums.Count];

                            moveCustomizedType(null, structType, sourceIndex, targetIndex);
                        }
                    }
                }
            }
        }

        private void upMemberButton_Click(object sender, EventArgs e)
        {
            int memberIndex = this.memberListBox.SelectedIndex;

            moveMember(memberIndex, memberIndex - 1);
        }

        private void downMemberButton_Click(object sender, EventArgs e)
        {
            int memberIndex = this.memberListBox.SelectedIndex;

            moveMember(memberIndex, memberIndex + 1);
        }

        private void memberListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            if (e.ListItem is MemberItem)
            {
                MemberItem item = e.ListItem as MemberItem;
                e.Value = item.DisplayName;
            }
        }

        private void MetaStoreDock_Resize(object sender, EventArgs e)
        {
            setMetaPanelLocation();
        }

        private void addInstanceButton_Click(object sender, EventArgs e)
        {
            int typeIndex = this.typeListBox.SelectedIndex;

            if (typeIndex >= 0 && typeIndex < Plugin.AgentTypes.Count)
            {
                AgentType agent = Plugin.AgentTypes[typeIndex];
                using (MetaInstanceDialog instanceDialog = new MetaInstanceDialog(agent.Name))
                {
                    if (instanceDialog.ShowDialog() == DialogResult.OK)
                    {
                        string instanceName = instanceDialog.InstanceName;
                        Plugin.AddInstanceName(instanceName, agent.Name, instanceName, instanceName, agent);

                        int index = this.instanceComboBox.Items.Add(instanceName);
                        this.instanceComboBox.SelectedIndex = index;
                    }
                }
            }
        }

        private void removeInstanceButton_Click(object sender, EventArgs e)
        {
            int typeIndex = this.typeListBox.SelectedIndex;

            if (typeIndex >= 0 && typeIndex < Plugin.AgentTypes.Count)
            {
                int instanceIndex = this.instanceComboBox.SelectedIndex;

                if (instanceIndex >= 0)
                {
                    Plugin.RemoveInstance(this.instanceComboBox.SelectedItem as string);

                    this.instanceComboBox.Items.RemoveAt(instanceIndex);

                    if (instanceIndex < this.instanceComboBox.Items.Count)
                    {
                        this.instanceComboBox.SelectedIndex = instanceIndex;
                    }
                    else
                    {
                        this.instanceComboBox.SelectedIndex = this.instanceComboBox.Items.Count - 1;
                    }
                }
            }
        }

        private void instanceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (instanceComboBox.SelectedIndex >= 0)
            {
                this.removeInstanceButton.Enabled = true;
            }
            else
            {
                this.removeInstanceButton.Enabled = false;
            }
        }

        private void previewCodeButton_Click(object sender, EventArgs e)
        {
            int exporterIndex = Plugin.GetExporterIndex(Workspace.Current.Language);
            Debug.Check(exporterIndex >= 0 && exporterIndex < Plugin.Exporters.Count);

            ExporterInfo exporter = Plugin.Exporters[exporterIndex];
            Debug.Check(exporter.HasSettings);

            string exportedPath = Workspace.Current.GetExportAbsoluteFolder(exporter.ID);
            List<string> includedFilenames = Workspace.Current.GetExportIncludedFilenames(exporter.ID);

            Exporters.Exporter exp = exporter.Create(null, exportedPath, "", includedFilenames);

            object typeObject = this.getSelectedType();

            if (typeObject != null)
            {
                if (typeObject is AgentType)
                {
                    exp.PreviewAgentFile((AgentType)typeObject);
                }
                else if (typeObject is EnumType)
                {
                    exp.PreviewEnumFile((EnumType)typeObject);
                }
                else if (typeObject is StructType)
                {
                    exp.PreviewStructFile((StructType)typeObject);
                }
            }
        }

        private void headerFileButton_Click(object sender, EventArgs e)
        {
            int exportIndex = Plugin.GetExporterIndex("cpp");
            ExporterInfo info = Plugin.Exporters[exportIndex];

            using(ExportSettingDialog dialog = new ExportSettingDialog(info))
            {
                dialog.ShowDialog();
            }
        }

        private void openPathButton_Click(object sender, EventArgs e)
        {
            try
            {
                string exportPath = "";
                object typeObject = this.getSelectedType();

                if (typeObject != null)
                {
                    if (typeObject is AgentType)
                    {
                        if (!((AgentType)typeObject).IsImplemented)
                        {
                            exportPath = ((AgentType)typeObject).ExportLocation;
                        }
                    }
                    else if (typeObject is EnumType)
                    {
                        if (!((EnumType)typeObject).IsImplemented)
                        {
                            exportPath = ((EnumType)typeObject).ExportLocation;
                        }
                    }
                    else if (typeObject is StructType)
                    {
                        if (!((StructType)typeObject).IsImplemented)
                        {
                            exportPath = ((StructType)typeObject).ExportLocation;
                        }
                    }
                }

                if (string.IsNullOrEmpty(exportPath))
                {
                    exportPath = Workspace.Current.GetExportAbsoluteFolder(Workspace.Current.Language);
                    exportPath = Path.Combine(exportPath, "behaviac_generated/types");
                }
                else
                {
                    exportPath = Workspace.Current.MakeAbsolutePath(exportPath);
                }

                System.Diagnostics.Process.Start(exportPath);
            }
            catch
            {
            }
        }

        private void clearPathButton_Click(object sender, EventArgs e)
        {
            try
            {
                string exportPath = Workspace.Current.GetExportAbsoluteFolder(Workspace.Current.Language);
                exportPath = Path.Combine(exportPath, "behaviac_generated");
                exportPath = Path.GetFullPath(exportPath);

                string info = string.Format(Resources.ClearGeneratedCodesInfo, exportPath);

                if (DialogResult.Yes == MessageBox.Show(info, Resources.Warning, MessageBoxButtons.YesNo))
                {
                    Directory.Delete(exportPath, true);
                }
            }
            catch
            {
            }
        }

        private void previewCodeButton_MouseEnter(object sender, EventArgs e)
        {
            _toolTip.AutoPopDelay = 5000;
            _toolTip.InitialDelay = 500;
            _toolTip.ReshowDelay = 5000;
            _toolTip.ShowAlways = true;

            _toolTip.Show(Resources.PreviewCode, this.previewCodeButton);
        }

        private void previewCodeButton_MouseLeave(object sender, EventArgs e)
        {
            _toolTip.Hide(this.previewCodeButton);
        }

        private void headerFileButton_MouseEnter(object sender, EventArgs e)
        {
            _toolTip.AutoPopDelay = 5000;
            _toolTip.InitialDelay = 500;
            _toolTip.ReshowDelay = 5000;
            _toolTip.ShowAlways = true;

            _toolTip.Show(Resources.SetHeaderFiles, this.headerFileButton);
        }

        private void headerFileButton_MouseLeave(object sender, EventArgs e)
        {
            _toolTip.Hide(this.headerFileButton);

        }
    }
}
