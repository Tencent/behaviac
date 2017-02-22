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
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerTypeEnumEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerTypeEnumEditor()
        {
            InitializeComponent();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            comboBox.Enabled = false;
        }

        private List<AgentType> _agentTypes = new List<AgentType>();

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            string enumName = string.Empty;
            Type enumtype = null;

            DesignerTypeEnum enumAtt = property.Attribute as DesignerTypeEnum;

            if (enumAtt != null)
            {
                object agentType = property.Property.GetValue(obj, null);
                AgentType t = agentType as AgentType;

                if (t != null)
                {
                    enumName = t.DisplayName;
                }

                enumtype = property.Property.PropertyType;
            }

            if (enumtype == null)
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeExpectedEnum, property.Property.Name));
            }

            _agentTypes = getAgentTypes();
            comboBox.Items.Clear();

            foreach (AgentType t in _agentTypes)
            {
                if (t.DisplayName == enumName)
                {
                    _agentTypes.Clear();
                    _agentTypes.Add(t);
                    comboBox.Items.Add(t.DisplayName);
                    break;
                }
            }

            comboBox.Text = enumName;
        }

        private List<AgentType> getAgentTypes()
        {
            List<AgentType> agentTypes = new List<AgentType>();

            foreach (AgentType t in Plugin.AgentTypes)
            {
                agentTypes.Add(t);
            }

            return agentTypes;
        }

        private void comboBox_DropDown(object sender, EventArgs e)
        {
            _agentTypes = getAgentTypes();

            if (_agentTypes.Count > 0)
            {
                if (string.IsNullOrEmpty(comboBox.Text))
                {
                    foreach (AgentType t in _agentTypes)
                    {
                        if (!comboBox.Items.Contains(t.DisplayName))
                        {
                            comboBox.Items.Add(t.DisplayName);
                        }
                    }

                }
                else
                {
                    int index = -1;

                    for (int i = 0; i < _agentTypes.Count; ++i)
                    {
                        if (comboBox.Text == _agentTypes[i].DisplayName)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index > -1)
                    {
                        for (int i = index - 1; i >= 0; --i)
                        {
                            if (!comboBox.Items.Contains(_agentTypes[i].DisplayName))
                            {
                                comboBox.Items.Insert(0, _agentTypes[i].DisplayName);
                            }
                        }

                        for (int i = index + 1; i < _agentTypes.Count; ++i)
                        {
                            if (!comboBox.Items.Contains(_agentTypes[i].DisplayName))
                            {
                                comboBox.Items.Add(_agentTypes[i].DisplayName);
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

            object agentType = _property.Property.GetValue(_object, null);
            AgentType preType = agentType as AgentType;
            AgentType curType = _agentTypes[comboBox.SelectedIndex];
            DialogResult result = DialogResult.OK;

            if (preType != null && preType != curType && _root != null && _root.Children.Count > 0)
            {
                result = MessageBox.Show(Resources.AgentTypeChangedWarning, Resources.Warning, MessageBoxButtons.OKCancel);
            }

            if (result == DialogResult.OK)
            {
                // reset the properties and methods of the root
                if (_root != null)
                {
                    _root.ResetMembers(MetaOperations.ChangeAgentType, curType, null, null, null);
                }

                _property.Property.SetValue(_object, curType, null);

                OnValueChanged(_property);
            }
            else if (preType != null)
            {
                comboBox.Text = preType.DisplayName;
            }
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

            this.OnDescriptionChanged(this.DisplayName, _agentTypes[e.Index].Description);
        }

        private void DesignerTypeEnumEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }
        }
    }
}
