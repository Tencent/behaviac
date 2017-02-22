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
using Behaviac.Design.Data;

namespace Behaviac.Design.Attributes
{
    public partial class CompositeEditorDialog : Form
    {
        public CompositeEditorDialog()
        {
            _initialized = false;
            _isModified = false;

            InitializeComponent();

            propertyGrid.PropertiesVisible(false, false);
            propertyGrid.ShowDescription(string.Empty, string.Empty);
            propertyGrid.ClearProperties();

            DesignerPropertyEditor.PropertyChanged += RefreshProperty;
        }

        private void CompositeEditorDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            DesignerPropertyEditor.PropertyChanged -= RefreshProperty;
        }

        private DesignerArrayPropertyInfo _arrayProperty = null;
        private DesignerStructPropertyInfo _structProperty = null;
        private object _object = null;
        private Nodes.Node _node = null;
        private Label _selectedLabel = null;

        private bool _initialized = false;
        private bool _isModified = false;

        public bool IsModified
        {
            get
            {
                return _isModified;
            }
        }

        public void SetArrayProperty(DesignerArrayPropertyInfo arrayProperty, object obj)
        {
            Debug.Check(arrayProperty != null);

            _arrayProperty = arrayProperty;
            setObject(obj);

            buildPropertyGrid();

            _initialized = true;
        }

        public void SetStructProperty(DesignerStructPropertyInfo structProperty, object obj)
        {
            Debug.Check(structProperty != null);

            _structProperty = structProperty;
            setObject(obj);

            buildPropertyGrid();

            _initialized = true;
        }

        private void setObject(object obj)
        {
            _object = obj;
            _node = _object as Nodes.Node;

            if (_node == null)
            {
                Attachments.Attachment attach = _object as Attachments.Attachment;

                if (attach != null)
                {
                    _node = attach.Node;
                }
            }
        }

        private void RefreshProperty()
        {
            buildPropertyGrid();
        }

        private void buildPropertyGrid(int selectedIndex = 0)
        {
            propertyGrid.PropertiesVisible(false, false);
            propertyGrid.ClearProperties();

            if (_arrayProperty != null)
            {
                _selectedLabel = null;

                if (_arrayProperty.ItemList.Count > 0)
                {
                    Type editorType = Plugin.InvokeEditorType(_arrayProperty.ItemType);
                    Debug.Check(editorType != null);

                    for (int index = 0; index < _arrayProperty.ItemList.Count; index++)
                    {
                        createArrayPropertyEditor(index, editorType, index == selectedIndex);
                    }

                    propertyGrid.UpdateSizes();
                    propertyGrid.PropertiesVisible(true, false);
                }

            }
            else if (_structProperty != null)
            {
                downButton.Visible = false;
                upButton.Visible = false;
                appendButton.Visible = false;
                insertButton.Visible = false;
                removeButton.Visible = false;
                //tableLayoutPanel.SetRowSpan(propertyGrid, tableLayoutPanel.RowCount - 1);
                //tableLayoutPanel.SetColumnSpan(propertyGrid, tableLayoutPanel.ColumnCount);

                Nodes.Action action = this._object as Nodes.Action;

                if (action == null)
                {
                    updateStructProperties(_structProperty.Owner);

                }
                else
                {
                    MethodDef method = action.Method;

                    if (method != null)
                    {
                        this.createParamEditor(_structProperty, action);
                    }
                }
            }
        }

        void createParamEditor(DesignerStructPropertyInfo structParam, Nodes.Action action)
        {
            MethodDef method = action.Method;

            if (method != null)
            {
                List<MethodDef.Param> parameters = method.GetParams(structParam);

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (MethodDef.Param p in parameters)
                    {
                        Type editorType = null;

                        if (structParam.ElmentIndexInArray != -1)
                        {
                            object member = p.Value;
                            editorType = p.Attribute.GetEditorType(member);

                        }
                        else
                        {
                            editorType = typeof(DesignerParameterComboEnumEditor);
                        }

                        string arugmentsName = "    " + p.DisplayName;
                        Label label = propertyGrid.AddProperty(arugmentsName, editorType,
                                                               p.Attribute != null ? p.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnly) : false);
                        label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
                        label.MouseEnter += label_MouseEnter;

                        DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;
                        editor.Enabled = true;
                        //editor.SetRootNode(this._node);
                        editor.SetParameter(p, action, false);
                        editor.MouseEnter += new EventHandler(editor_MouseEnter);
                        editor.ValueWasAssigned();
                        editor.ValueWasChanged += editor_ValueWasChanged;
                    }

                    {
                        propertyGrid.UpdateSizes();
                        propertyGrid.PropertiesVisible(true, true);
                    }
                }
            }
        }

        private Behaviac.Design.ObjectUI.ObjectUIPolicy uiPolicy = null;

        private void updateStructProperties(object owner)
        {
            IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(_structProperty.Type, DesignerProperty.SortByDisplayOrder);

            List<string> categories = new List<string>();

            foreach (DesignerPropertyInfo property in properties)
            {
                if (!categories.Contains(property.Attribute.CategoryResourceString))
                {
                    categories.Add(property.Attribute.CategoryResourceString);
                }
            }

            categories.Sort();

            UIObject uiObj = owner as UIObject;

            if (uiObj != null)
            {
                uiPolicy = uiObj.CreateUIPolicy();
                uiPolicy.Initialize(uiObj);
            }

            foreach (string category in categories)
            {
                if (categories.Count > 1)
                {
                    propertyGrid.AddCategory(Plugin.GetResourceString(category), true);
                }

                foreach (DesignerPropertyInfo property in properties)
                {
                    if (property.Attribute.CategoryResourceString == category)
                    {
                        if (uiPolicy != null && !uiPolicy.ShouldAddProperty(property))
                        {
                            continue;
                        }

                        object member = property.GetValue(owner);
                        Type type = property.Attribute.GetEditorType(member);
                        DesignerMethodEnum propertyMethod = property.Attribute as DesignerMethodEnum;

                        if (propertyMethod != null)
                        {
                            if ((propertyMethod.MethodType & MethodType.Task) == MethodType.Task)
                            {
                                type = typeof(DesignerMethodEnumEditor);
                            }
                        }

                        string displayName = property.Attribute.DisplayName;

                        if (uiPolicy != null)
                        {
                            displayName = uiPolicy.GetLabel(property);
                        }

                        Label label = propertyGrid.AddProperty(displayName, type, property.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnly));
                        label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
                        label.MouseEnter += new EventHandler(label_MouseEnter);

                        if (type != null)
                        {
                            DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;
                            editor.SetRootNode(this._node);
                            editor.SetProperty(property, owner);
                            editor.ValueWasAssigned();
                            editor.MouseEnter += editor_MouseEnter;
                            editor.ValueWasChanged += editor_ValueWasChanged;

                            if (uiPolicy != null)
                            {
                                uiPolicy.AddEditor(editor);
                            }
                        }

                        MethodDef method = null;

                        if (propertyMethod != null)
                        {
                            if (propertyMethod.MethodType != MethodType.Status)
                            {
                                method = member as MethodDef;
                            }
                        }
                        else
                        {
                            DesignerRightValueEnum propertyRV = property.Attribute as DesignerRightValueEnum;

                            if (propertyRV != null)
                            {
                                RightValueDef rv = member as RightValueDef;

                                if (rv != null && rv.IsMethod)
                                {
                                    method = rv.Method;
                                }
                            }
                        }

                        if (property.Attribute != null)
                        {
                            if (method != null)
                            {
                                if (property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoDisplayOnProperty))
                                {
                                    //don't dipslay on the property panel
                                }
                                else
                                {
                                    bool bReadonly = property.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnlyParams);

                                    createParamEditor(owner, method, true, bReadonly);
                                }
                            }
                            else
                            {
                                MethodDef.Param arrayIndexElement = null;

                                if (member is VariableDef)
                                {
                                    VariableDef var = member as VariableDef;
                                    arrayIndexElement = var.ArrayIndexElement;

                                }
                                else if (member is RightValueDef)
                                {
                                    RightValueDef varRV = member as RightValueDef;

                                    if (varRV.Var != null)
                                    {
                                        arrayIndexElement = varRV.Var.ArrayIndexElement;
                                    }
                                }

                                if (arrayIndexElement != null)
                                {
                                    createArrayIndexEditor(owner, "    ", arrayIndexElement);
                                }

                            }
                        }
                    }
                }
            }

            if (uiPolicy != null)
            {
                uiPolicy.Update(null, new DesignerPropertyInfo());
            }

            if (properties.Count > 0)
            {
                propertyGrid.UpdateSizes();
                propertyGrid.PropertiesVisible(true, true);
            }
        }

        MethodDef.Param lastListParam = null;
        void createParamEditor(object owner, MethodDef method, bool enable, bool bReadonlyParent)
        {
            List<MethodDef.Param> parameters = method.Params;

            foreach (MethodDef.Param p in parameters)
            {
                Type editorType = typeof(DesignerParameterComboEnumEditor);
                string arugmentsName = "    " + p.DisplayName;
                bool bReadonly = bReadonlyParent | p.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnly);
                Label label = propertyGrid.AddProperty(arugmentsName, editorType, bReadonly);

                label.MouseEnter += new EventHandler(label_MouseEnter);

                DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;

                if (p.Type.Name == "IList")
                {
                    lastListParam = p;
                }

                if (p.Type.Name == "System_Object" && lastListParam != null)
                {
                    p.ListParam = lastListParam;
                }

                editor.Enabled = enable;
                editor.SetParameter(p, owner, bReadonly);

                editor.ValueWasAssigned();
                editor.MouseEnter += editor_MouseEnter;
                editor.ValueWasChanged += editor_ValueWasChanged;
                //editor.ValueType = p.Attribute.ValueType;

                MethodDef.Param arrayIndexElement = null;

                if (p.Value is VariableDef)
                {
                    VariableDef var = p.Value as VariableDef;
                    arrayIndexElement = var.ArrayIndexElement;
                }
                else if (p.Value is RightValueDef)
                {
                    RightValueDef varRV = p.Value as RightValueDef;

                    if (varRV.Var != null)
                    {
                        arrayIndexElement = varRV.Var.ArrayIndexElement;
                    }
                }

                if (arrayIndexElement != null)
                {
                    createArrayIndexEditor(owner, "        ", arrayIndexElement);
                }
            }
        }

        void createArrayIndexEditor(object owner, string preBlank, MethodDef.Param arrayIndex)
        {
            Type editorType = typeof(DesignerParameterComboEnumEditor);
            string arugmentsName = preBlank + "Index";
            bool bReadonly = false;
            Label label = propertyGrid.AddProperty(arugmentsName, editorType, bReadonly);

            label.MouseEnter += new EventHandler(label_MouseEnter);

            DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;
            editor.Enabled = true;
            editor.SetParameter(arrayIndex, owner, bReadonly);
            editor.ValueWasAssigned();
            editor.MouseEnter += editor_MouseEnter;
            editor.ValueWasChanged += editor_ValueWasChanged;
        }

        private void label_MouseEnter(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;

            propertyGrid.ShowDescription(editor.DisplayName, editor.Description);
        }

        private void editor_MouseEnter(object sender, EventArgs e)
        {
            DesignerPropertyEditor editor = (DesignerPropertyEditor)sender;

            propertyGrid.ShowDescription(editor.DisplayName, editor.Description);
        }

        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            if (!_initialized)
            {
                return;
            }

            if (_node != null)
            {
                _node.OnPropertyValueChanged(true);

                UndoManager.Save(_node.Behavior);
            }

            //comment it as it disposed control and System.ObjectDisposedException might be raised
            //buildPropertyGrid();

            if (property.Attribute is DesignerNodeProperty ||
                uiPolicy != null && uiPolicy.ShouldUpdatePropertyGrids(property))
            {
                buildPropertyGrid();
            }

            _isModified = true;
        }

        private void selectLabel(Label label)
        {
            if (_selectedLabel != null)
            {
                _selectedLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
                _selectedLabel.ForeColor = Color.WhiteSmoke;
            }

            _selectedLabel = label;
            _selectedLabel.BackColor = Color.RoyalBlue;
            _selectedLabel.ForeColor = Color.White;
        }

        private int selectedIndex()
        {
            return (_selectedLabel != null) ? int.Parse(_selectedLabel.Text) : -1;
        }

        private void createArrayPropertyEditor(int itemIndex, Type editorType, bool isSelected)
        {
            Debug.Check(_arrayProperty != null);
            if (_arrayProperty != null)
            {
                string propertyName = string.Format("{0}", itemIndex);
                Label label = propertyGrid.AddProperty(propertyName, editorType, _arrayProperty.ReadOnly);
                label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
                label.MouseClick += new MouseEventHandler(label_MouseClick);

                DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;

                if (Plugin.IsCustomClassType(_arrayProperty.ItemType))
                {
                    editor.SetStructProperty(new DesignerStructPropertyInfo(
                                                 _arrayProperty.ItemType.Name, _arrayProperty.ItemType, _arrayProperty.ItemList[itemIndex], itemIndex),
                                             _object);

                }
                else
                {
                    DesignerArrayPropertyInfo arrayProperty = new DesignerArrayPropertyInfo(_arrayProperty);
                    arrayProperty.ItemIndex = itemIndex;

                    editor.SetArrayProperty(arrayProperty, _object);
                }

                editor.ValueWasAssigned();
                editor.ValueWasChanged += editor_ValueWasChanged;

                if (isSelected)
                {
                    selectLabel(label);
                }
            }
        }

        private void label_MouseClick(object sender, MouseEventArgs e)
        {
            selectLabel((Label)sender);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Debug.Check(_arrayProperty != null);

            if (_arrayProperty != null)
            {
                object item = Plugin.CreateInstance(_arrayProperty.ItemType);

                if (item != null)
                {
                    Type editorType = Plugin.InvokeEditorType(_arrayProperty.ItemType);
                    Debug.Check(editorType != null);

                    int index = _arrayProperty.ItemList.Add(item);
                    createArrayPropertyEditor(index, editorType, true);

                    propertyGrid.UpdateSizes();
                    propertyGrid.PropertiesVisible(true, false);

                    if (_node != null)
                    {
                        _node.OnPropertyValueChanged(true);
                    }
                }

                _isModified = true;
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Debug.Check(_arrayProperty != null);

            int index = selectedIndex();

            if (index < 0)
            {
                index = 0;
            }

            if (_arrayProperty != null)
            {
                object item = Plugin.CreateInstance(_arrayProperty.ItemType);

                if (item != null)
                {
                    _arrayProperty.ItemList.Insert(index, item);

                    buildPropertyGrid(index);

                    if (_node != null)
                    {
                        _node.OnPropertyValueChanged(true);
                    }
                }

                _isModified = true;
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Debug.Check(_arrayProperty != null);

            int index = selectedIndex();

            if (_arrayProperty != null && index > -1)
            {
                _arrayProperty.ItemList.RemoveAt(index);

                if (index == _arrayProperty.ItemList.Count)
                {
                    index--;
                }

                buildPropertyGrid(index);

                if (_node != null)
                {
                    _node.OnPropertyValueChanged(true);
                }
            }

            _isModified = true;
        }

        private void swapItems(int currentIndex, int otherIndex)
        {
            object item = _arrayProperty.ItemList[currentIndex];
            _arrayProperty.ItemList[currentIndex] = _arrayProperty.ItemList[otherIndex];
            _arrayProperty.ItemList[otherIndex] = item;

            buildPropertyGrid(otherIndex);

            if (_node != null)
            {
                _node.OnPropertyValueChanged(true);
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Debug.Check(_arrayProperty != null);

            int index = selectedIndex();

            if (index > 0)
            {
                swapItems(index, index - 1);
            }

            _isModified = true;
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            Debug.Check(_arrayProperty != null);

            if (_arrayProperty != null)
            {
                int index = selectedIndex();

                if (index > -1 && index < _arrayProperty.ItemList.Count - 1)
                {
                    swapItems(index, index + 1);
                }

                _isModified = true;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
