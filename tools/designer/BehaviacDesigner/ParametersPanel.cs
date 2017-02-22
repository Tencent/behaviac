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

namespace Behaviac.Design
{
    public partial class ParametersPanel : UserControl
    {
        class RowControl
        {
            private string _name;
            public string Name
            {
                get
                {
                    return this._name;
                }
                set
                {
                    this._name = value;
                }
            }

            private System.Windows.Forms.Label _nameLabel;
            public System.Windows.Forms.Label NameLabel
            {
                get
                {
                    return _nameLabel;
                }
                set
                {
                    _nameLabel = value;
                }
            }

            private System.Windows.Forms.Label _typeLabel;
            public System.Windows.Forms.Label TypeLabel
            {
                get
                {
                    return _typeLabel;
                }
                set
                {
                    _typeLabel = value;
                }
            }

            private DesignerPropertyEditor _valueEditor;
            public DesignerPropertyEditor ValueEditor
            {
                get
                {
                    return _valueEditor;
                }
                set
                {
                    _valueEditor = value;
                }
            }
        }

        private List<RowControl> _rowControls = new List<RowControl>();
        private AgentType _agentType = null;
        private string _agentFullname = string.Empty;

        public ParametersPanel()
        {
            InitializeComponent();

            columnStyles = tableLayoutPanel.ColumnStyles;
        }

        public void InspectObject(AgentType agentType, string agentFullname)
        {
            _agentType = agentType;
            _agentFullname = agentFullname;

            preLayout();

            deleteAllRowControls();

            if (agentType != null)
            {
                IList<PropertyDef> properties = agentType.GetProperties();

                foreach (PropertyDef p in properties)
                {
                    if (!p.IsArrayElement)
                    {
                        addRowControl(p);
                    }
                }
            }

            postLayout();
        }

        public bool SetProperty(BehaviorNode behavior, string valueName, string valueStr)
        {
            DesignerPropertyEditor propertyEditor = getPropertyEditor(valueName);

            if (propertyEditor == null && behavior != null)
            {
                Node root = behavior as Node;

                foreach (PropertyDef p in root.LocalVars)
                {
                    if (!p.IsArrayElement && p.BasicName == valueName)
                    {
                        propertyEditor = addRowControl(p);
                        break;
                    }
                }
            }

            if (propertyEditor == null)
            {
                return false;
            }

            VariableDef var = propertyEditor.GetVariable();

            if (var.Value.ToString().ToLower() != valueStr.ToLower())
            {
                Plugin.InvokeTypeParser(null, var.ValueType, valueStr, (object value) => var.Value = value, null);

                propertyEditor.ValueWasnotAssigned();

                propertyEditor.SetVariable(var, null);

                propertyEditor.ValueWasAssigned();

                return true;
            }

            return false;
        }

        public void SetProperty(FrameStatePool.PlanningState nodeState, string agentFullName)
        {
            Debug.Check(nodeState != null);

            //iterate all the properties
            for (int i = 0; i < _rowControls.Count; ++i)
            {
                RowControl rc = _rowControls[i];

                object v = GetPropertyValue(nodeState, agentFullName, rc.Name);

                if (v != null)
                {
                    SetProperty(null, rc.Name, v.ToString());
                }
            }
        }

        private static object GetPropertyValue(FrameStatePool.PlanningState nodeState, string agentFullName, string propertyName)
        {
            FrameStatePool.PlanningState ns = nodeState;
            bool bOk = ns._agents.ContainsKey(agentFullName) && ns._agents[agentFullName].ContainsKey(propertyName);

            //if the current node has no value, to look for it in the parent's
            object v = null;

            while (!bOk)
            {
                ns = ns._parent;

                if (ns == null)
                {
                    break;
                }

                bOk = ns._agents.ContainsKey(agentFullName) && ns._agents[agentFullName].ContainsKey(propertyName);
            }

            if (bOk)
            {
                Debug.Check(ns._agents[agentFullName].ContainsKey(propertyName));
                v = ns._agents[agentFullName][propertyName];
            }

            return v;
        }


        private void preLayout()
        {
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
        }

        private void postLayout()
        {
            this.tableLayoutPanel.ResumeLayout();
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout();
            this.PerformLayout();

            columnStyles = tableLayoutPanel.ColumnStyles;
        }

        private DesignerPropertyEditor getPropertyEditor(string propertyName)
        {
            for (int i = 0; i < _rowControls.Count; ++i)
            {
                if (_rowControls[i].Name == propertyName)
                {
                    return _rowControls[i].ValueEditor;
                }
            }

            return null;
        }

        private int getRowIndex(DesignerPropertyEditor valueEditor)
        {
            for (int i = 0; i < _rowControls.Count; ++i)
            {
                if (_rowControls[i].ValueEditor == valueEditor)
                {
                    return i;
                }
            }

            return -1;
        }

        private DesignerPropertyEditor createPropertyEditor(PropertyDef property)
        {
            Type type = property.Type;
            Type editorType = Plugin.InvokeEditorType(type);
            Debug.Check(editorType != null);

            DesignerPropertyEditor editor = (DesignerPropertyEditor)editorType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);
            editor.TabStop = false;
            editor.Dock = System.Windows.Forms.DockStyle.Fill;
            editor.Margin = new System.Windows.Forms.Padding(0);
            editor.ValueWasChanged += new DesignerPropertyEditor.ValueChanged(editor_ValueWasChanged);

            VariableDef var = new VariableDef(Plugin.DefaultValue(type));
            editor.SetVariable(var, null);

            editor.ValueWasAssigned();

            return editor;
        }

        private DesignerPropertyEditor addRowControl(PropertyDef property)
        {
            this.tableLayoutPanel.RowCount++;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));

            int rowIndex = _rowControls.Count + 1;
            RowControl rowControl = new RowControl();
            _rowControls.Add(rowControl);

            rowControl.Name = property.BasicName;

            rowControl.NameLabel = new System.Windows.Forms.Label();
            rowControl.NameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            rowControl.NameLabel.Margin = new System.Windows.Forms.Padding(0);
            rowControl.NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            rowControl.NameLabel.Text = property.DisplayName;
            this.tableLayoutPanel.Controls.Add(rowControl.NameLabel, 0, rowIndex);

            rowControl.TypeLabel = new System.Windows.Forms.Label();
            rowControl.TypeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            rowControl.TypeLabel.Margin = new System.Windows.Forms.Padding(0);
            rowControl.TypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            rowControl.TypeLabel.Text = Plugin.GetNativeTypeName(property.Type);
            this.tableLayoutPanel.Controls.Add(rowControl.TypeLabel, 1, rowIndex);

            rowControl.ValueEditor = createPropertyEditor(property);
            rowControl.ValueEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            rowControl.ValueEditor.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Controls.Add(rowControl.ValueEditor, 2, rowIndex);

            return rowControl.ValueEditor;
        }

        private void deleteAllRowControls()
        {
            for (int rowIndex = _rowControls.Count - 1; rowIndex >= 0; --rowIndex)
            {
                RowControl rowControl = _rowControls[rowIndex];
                _rowControls.RemoveAt(rowIndex);

                this.tableLayoutPanel.RowStyles.RemoveAt(rowIndex);

                this.tableLayoutPanel.Controls.Remove(rowControl.NameLabel);
                this.tableLayoutPanel.Controls.Remove(rowControl.TypeLabel);

                if (rowControl.ValueEditor != null)
                {
                    this.tableLayoutPanel.Controls.Remove(rowControl.ValueEditor);
                }
            }

            this.tableLayoutPanel.RowCount = 1;
        }

        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            if (string.IsNullOrEmpty(_agentFullname))
            {
                return;
            }

            int index = getRowIndex(sender as DesignerPropertyEditor);

            if (index > -1)
            {
                RowControl row = _rowControls[index];

                VariableDef var = row.ValueEditor.GetVariable();

                if (var != null && var.Value != null)
                {
                    string value = var.Value.ToString();

                    if (!string.IsNullOrEmpty(value))
                    {
                        string valueType = row.TypeLabel.Text;
                        string valueName = row.Name;

                        if (AgentDataPool.CurrentFrame > -1)
                        {
                            AgentDataPool.AddValue(_agentFullname, valueName, AgentDataPool.CurrentFrame, value);
                        }

                        NetworkManager.Instance.SendProperty(_agentFullname, valueType, valueName, value);
                    }
                }
            }
        }

        private void lostAnyFocus()
        {
            this.Enabled = false;
            this.Enabled = true;
        }

        private void ParametersPanel_Click(object sender, EventArgs e)
        {
            lostAnyFocus();
        }

        private void tableLayoutPanel_Click(object sender, EventArgs e)
        {
            lostAnyFocus();
        }

        TableLayoutColumnStyleCollection columnStyles;
        bool resizing = false;
        int colindex = -1;

        private void tableLayoutPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                columnStyles = tableLayoutPanel.ColumnStyles;
                resizing = true;
            }
        }

        private void tableLayoutPanel_MouseMove(object sender, MouseEventArgs e)
        {
            List<float> widthes = new List<float>();

            if (columnStyles.Count > 0 && columnStyles[0].SizeType == SizeType.Absolute)
            {
                for (int i = 0; i < columnStyles.Count; i++)
                {
                    widthes.Add(columnStyles[i].Width);
                }

            }
            else
            {
                widthes.Add(nameLabel.Width);
                widthes.Add(typeLabel.Width);
                widthes.Add(valueLabel.Width);
            }

            if (!resizing)
            {
                float width = 0;

                for (int i = 0; i < widthes.Count; i++)
                {
                    width += widthes[i];

                    if (e.X > width - 5 && e.X < width + 5)
                    {
                        colindex = i;
                        tableLayoutPanel.Cursor = Cursors.VSplit;
                        break;

                    }
                    else
                    {
                        colindex = -1;
                        tableLayoutPanel.Cursor = Cursors.Default;
                    }
                }
            }

            if (resizing && (colindex > -1))
            {
                if (colindex > -1)
                {
                    float width = e.X;

                    for (int i = 0; i < colindex; i++)
                    {
                        width -= widthes[i];
                    }

                    if (width > 100)
                    {
                        tableLayoutPanel.SuspendLayout();
                        this.SuspendLayout();

                        columnStyles[colindex].SizeType = SizeType.Absolute;
                        columnStyles[colindex].Width = width;

                        tableLayoutPanel.ResumeLayout();
                        this.ResumeLayout();
                    }
                }
            }
        }

        private void tableLayoutPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                resizing = false;
                tableLayoutPanel.Cursor = Cursors.Default;
            }
        }

        private void tableLayoutPanel_MouseLeave(object sender, EventArgs e)
        {
            resizing = false;
            tableLayoutPanel.Cursor = Cursors.Default;
        }
    }
}
