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
    public partial class DesignerParameterComboEnumEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerParameterComboEnumEditor()
        {
            InitializeComponent();

            SetTypes();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            this.typeComboBox.Enabled = false;

            if (this.propertyEditor != null)
            {
                this.propertyEditor.ReadOnly();
            }
        }

        public override string DisplayName
        {
            get
            {
                return (_param != null) ? _param.DisplayName : base.DisplayName;
            }
        }

        public override string Description
        {
            get
            {
                return (_param != null) ? _param.Description : base.Description;
            }
        }

        private List<string> _types = new List<string>();

        private void SetTypes()
        {
            _types.Clear();
            _types.Add(VariableDef.kConst);
            _types.Add(VariableDef.kSelf);

            foreach (Plugin.InstanceName_t instanceName in this.InstanceNames)
            {
                _types.Add(instanceName.DisplayName);
            }
        }

        public override void SetParameter(MethodDef.Param param, object obj, bool bReadonly)
        {
            base.SetParameter(param, obj, bReadonly);

            SetTypes();

            int typeIndex = -1;
            DesignerPropertyEditor editor = null;

            if (param.IsFromStruct)
            {
                string instance = string.Empty;
                string vt = VariableDef.kConst;

                if (_param.Value is VariableDef)
                {
                    VariableDef v = _param.Value as VariableDef;
                    vt = v.ValueClass;

                    instance = vt;

                    if (instance != VariableDef.kSelf)
                    {
                        instance = Plugin.GetInstanceNameFromClassName(instance);
                    }
                }

                typeIndex = getComboIndex(vt, instance, "");

                editor = createEditor(vt);

            }
            else
            {
                string valueType = "Self";
                string instance = "Self";
                string propertyName = "";

                if (param.Value != null)
                {
                    bool bSet = ClearValueIfChanged(param);

                    if (bSet)
                    {
                        string[] tokens = param.Value.ToString().Split(' ');
                        propertyName = tokens[tokens.Length - 1];
                        instance = Plugin.GetInstanceName(propertyName);
                        valueType = string.Empty;

                        if (!string.IsNullOrEmpty(instance))
                        {
                            propertyName = propertyName.Substring(instance.Length + 1, propertyName.Length - instance.Length - 1);
                            valueType = getValueType(param, instance, propertyName);

                        }
                        else
                        {
                            valueType = getValueType(param, propertyName);
                        }
                    }
                }

                typeIndex = getComboIndex(valueType, instance, propertyName);

                editor = createEditor(valueType);
            }

            if (editor != null)
            {
                setPropertyEditor(editor);
            }

            if (typeIndex > -1)
            {
                // Keep only one type for efficiency.
                this.typeComboBox.Items.Clear();
                this.typeComboBox.Items.Add(_types[typeIndex]);
                this.typeComboBox.SelectedIndex = 0;
            }
        }

        private bool ClearValueIfChanged(MethodDef.Param param)
        {
            bool bSet = true;

            if (param.ListParam != null)
            {
                Type itemType = MethodDef.Param.GetListParamItemType(param);

                if (param.Value is VariableDef)
                {
                    VariableDef var = param.Value as VariableDef;

                    if (var.ValueType != itemType)
                    {
                        //type changed, to clear the old one
                        bSet = false;
                        param.Value = null;
                    }
                }
                else if (param.Value is PropertyDef)
                {
                    PropertyDef var = param.Value as PropertyDef;

                    if (var.Type != itemType)
                    {
                        //type changed, to clear the old one
                        bSet = false;
                        param.Value = null;
                    }
                }
                else if (param.Value is ParInfo)
                {
                    ParInfo var = param.Value as ParInfo;

                    if (var.Type != itemType)
                    {
                        //type changed, to clear the old one
                        bSet = false;
                        param.Value = null;
                    }
                }
            }

            return bSet;
        }

        private void typeComboBox_DropDown(object sender, EventArgs e)
        {
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

        private int getComboIndex(string valueType, string instanceName, string propertyName)
        {
            if (valueType == VariableDef.kConst)
            {
                return 0;
            }

            if (valueType == VariableDef.kSelf)
            {
                return 1;
            }

            if (string.IsNullOrEmpty(instanceName))
            {
                instanceName = Plugin.GetClassName(propertyName);
            }

            Debug.Check(!string.IsNullOrEmpty(instanceName));
            Nodes.Behavior behavior = GetBehavior();
            int index = Plugin.InstanceNameIndex(instanceName, behavior);
            Debug.Check(index >= 0);

            return index + 2;
        }

        private string getValueType(MethodDef.Param param, string instanceName, string propertyName)
        {
            if (param.IsProperty)
            {
                if (instanceName == VariableDef.kSelf)
                {
                    return VariableDef.kSelf;
                }

                Nodes.Behavior behavior = GetBehavior();
                AgentType agent = (behavior != null) ? behavior.AgentType : null;
                agent = Plugin.GetInstanceAgentType(instanceName, behavior, agent);

                if (agent != null)
                {
                    IList<PropertyDef> properties = agent.GetProperties();

                    foreach (PropertyDef p in properties)
                    {
                        if (p.Name == propertyName
#if BEHAVIAC_NAMESPACE_FIX
                            || p.Name.EndsWith(propertyName)
#endif
                           )
                        {
                            return instanceName;
                        }
                    }
                }
            }

            return VariableDef.kConst;
        }

        private string getValueType(MethodDef.Param param, string propertyName)
        {
            if (param.IsLocalVar)
            {
                Nodes.Behavior behavior = GetBehavior();
                AgentType agent = (behavior != null) ? behavior.AgentType : null;

                // Try to find the Agent property with the name.
                if (agent != null)
                {
                    IList<PropertyDef> properties = agent.GetProperties();

                    foreach (PropertyDef p in properties)
                    {
                        if (p.Name == propertyName
#if BEHAVIAC_NAMESPACE_FIX
                            || p.Name.EndsWith(propertyName)
#endif
                           )
                        {
                            return VariableDef.kSelf;
                        }
                    }
                }

                // Try to find the global property with the name.
                string className = Plugin.GetClassName(propertyName);

                return getValueType(param, className, propertyName);
            }

            return VariableDef.kConst;
        }

        private Type getEditorType(string valueType)
        {
            if (valueType == VariableDef.kConst)
            {
                if (this._param != null && this._param.ListParam != null)
                {
                    // based on the List's item type
                    Type itemType = MethodDef.Param.GetListParamItemType(this._param);

                    if (!Plugin.IsCustomClassType(itemType))
                    {
                        if (Plugin.IsIntergerNumberType(itemType) || Plugin.IsFloatType(itemType))
                        {
                            return typeof(DesignerNumberEditor);
                        }
                        else if (Plugin.IsBooleanType(itemType))
                        {
                            return typeof(DesignerBooleanEditor);
                        }
                        else if (Plugin.IsStringType(itemType))
                        {
                            return typeof(DesignerStringEditor);
                        }
                    }
                }

                return (_param != null) ? _param.Attribute.GetEditorType(null) : null;
            }

            return typeof(DesignerPropertyEnumEditor);
        }

        private void setEditor(DesignerPropertyEditor editor, string valueType)
        {
            if (editor == null || _param == null)
            {
                return;
            }

            Type filterTypeCandidate = null;

            if (valueType == VariableDef.kConst)
            {
                if (_param.Value != null && (_param.Value is VariableDef || _param.Value is PropertyDef || _param.Value is ParInfo))
                {
                    if (!(_param.IsFromStruct))
                    {
                        _param.Value = Plugin.DefaultValue(_param.Type);
                    }
                    else
                    {
                        if (_param.Value is VariableDef)
                        {
                            VariableDef v = _param.Value as VariableDef;
                            _param.Value = Plugin.DefaultValue(v.ValueType);
                        }
                        else if (_param.Value is ParInfo)
                        {
                            ParInfo v = _param.Value as ParInfo;
                            _param.Value = Plugin.DefaultValue(v.Variable.ValueType);
                        }
                    }
                }
            }
            else
            {
                if (_param.Value is VariableDef)
                {
                    VariableDef v = _param.Value as VariableDef;

                    filterTypeCandidate = v.ValueType;

                    if (v.ValueClass != valueType)
                    {
                        Type t1 = v.ValueType != null ? v.ValueType : _param.Type;
                        object dv = Plugin.DefaultValue(t1);
                        _param.Value = new VariableDef(dv, valueType);
                    }
                }
                else if (_param.Value is ParInfo)
                {
                    ParInfo v = _param.Value as ParInfo;

                    filterTypeCandidate = v.Variable.ValueType;

                    if (v.Variable.ValueClass != valueType)
                    {
                        object dv = Plugin.DefaultValue(v.Variable.ValueType);
                        _param.Value = new VariableDef(dv, valueType);
                    }
                }
                else
                {
                    _param.Value = new VariableDef(_param.Value, valueType);
                    filterTypeCandidate = _param.Type;
                }
            }

            this.SetFilterType(editor, filterTypeCandidate);
            SetupCastSettings(_object);
            editor.SetParameter(_param, _object, false);

            editor.ValueWasAssigned();
            editor.ValueWasChanged += editor_ValueWasChanged;
        }

        private void SetFilterType(DesignerPropertyEditor propertyEnumEditor, Type valueType)
        {
            if (_param.ListParam != null && _param.ListParam.Value != null)
            {
                Type itemType = MethodDef.Param.GetListParamItemType(_param);
                Debug.Check(itemType != null);
                propertyEnumEditor.FilterType = itemType;
            }
            else if (valueType != null)
            {
                propertyEnumEditor.FilterType = valueType;
            }
            else
            {
                propertyEnumEditor.FilterType = _param.Type;
            }
        }


        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            OnValueChanged(_property);
        }

        private DesignerPropertyEditor createEditor(string valueType)
        {
            if (flowLayoutPanel.Controls.Count > 1)
            {
                flowLayoutPanel.Controls.RemoveAt(1);
            }

            Type editorType = getEditorType(valueType);

            if (editorType == null)
            {
                return null;
            }

            DesignerPropertyEditor editor = (DesignerPropertyEditor)editorType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);
            editor.Location = new System.Drawing.Point(74, 1);
            editor.Margin = new System.Windows.Forms.Padding(0);
            editor.Size = new System.Drawing.Size(flowLayoutPanel.Width - this.typeComboBox.Width - 5, 20);
            editor.TabIndex = 1;
            editor.ValueType = (this._param != null ? this._param.Attribute.ValueType : ValueTypes.All);

            if (this._param != null && this._param.IsArrayIndex)
            {
                editor.ValueType = ValueTypes.Int;
            }

            setEditor(editor, valueType);

            if (this._bIsReadonly)
            {
                editor.ReadOnly();
            }

            flowLayoutPanel.Controls.Add(editor);
            return editor;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.typeComboBox.SelectedItem != null)
            {
                setPropertyEditor(createEditor((string)this.typeComboBox.SelectedItem));

                OnValueChanged(_property);
            }
        }

        private void flowLayoutPanel_Resize(object sender, EventArgs e)
        {
            if (this.propertyEditor != null)
            {
                this.propertyEditor.Width = flowLayoutPanel.Width - this.typeComboBox.Width - 5;
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
