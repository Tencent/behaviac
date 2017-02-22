////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
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
    public partial class DesignerEnumVariableEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerEnumVariableEditor()
        {
            InitializeComponent();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            typeComboBox.Enabled = false;

            if (propertyEditor != null)
            {
                propertyEditor.ReadOnly();
            }
        }

        private Nodes.Node _node;
        private ParInfo _parameter;

        public void SetParameter(ParInfo parameter, Nodes.Node node)
        {
            _parameter = parameter;
            _node = node;

            typeComboBox.Width = flowLayoutPanel.Width * 2 / 5;

            typeComboBox.Items.Clear();

            foreach (Type key in Plugin.TypeHandlers.Keys)
            {
                typeComboBox.Items.Add(Plugin.GetNativeTypeName(key.Name));
            }

            typeComboBox.Text = parameter.TypeName;
        }

        private DesignerPropertyEditor CreateEditor()
        {
            if (flowLayoutPanel.Controls.Count > 1)
            {
                flowLayoutPanel.Controls.RemoveAt(1);
            }

            string currentType = typeComboBox.SelectedItem.ToString();
            Type type = null;

            foreach (Type key in Plugin.TypeHandlers.Keys)
            {
                if (Plugin.GetNativeTypeName(key.Name) == currentType)
                {
                    type = key;
                    break;
                }
            }

            Debug.Check(type != null);
            if (type != null)
            {
                Type editorType = Plugin.InvokeEditorType(type);
                Debug.Check(editorType != null);

                if (editorType != null)
                {
                    DesignerPropertyEditor editor = (DesignerPropertyEditor)editorType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);
                    editor.Location = new System.Drawing.Point(74, 1);
                    editor.Margin = new System.Windows.Forms.Padding(0);
                    editor.Size = new System.Drawing.Size(flowLayoutPanel.Width - typeComboBox.Width - 5, 20);
                    editor.TabIndex = 1;
                    editor.ValueWasChanged += new DesignerPropertyEditor.ValueChanged(editor_ValueWasChanged);

                    flowLayoutPanel.Controls.Add(editor);

                    return editor;
                }
            }

            return null;
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _parameter.TypeName = typeComboBox.SelectedItem.ToString();

            propertyEditor = CreateEditor();

            if (propertyEditor != null && _parameter.Variable != null)
            {
                propertyEditor.SetVariable(_parameter.Variable, _node);
                propertyEditor.ValueWasAssigned();
            }
        }

        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            _parameter.Variable = propertyEditor.GetVariable();
        }

        private void flowLayoutPanel_Resize(object sender, EventArgs e)
        {
            typeComboBox.Width = flowLayoutPanel.Width * 2 / 5;

            if (propertyEditor != null)
            {
                propertyEditor.Width = flowLayoutPanel.Width - typeComboBox.Width - 5;
            }
        }

        private void typeComboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }
    }
}
