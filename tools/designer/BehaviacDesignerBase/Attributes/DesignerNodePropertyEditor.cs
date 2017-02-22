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
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerNodePropertyEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerNodePropertyEditor()
        {
            InitializeComponent();
        }

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            Nodes.Node node = null;

            if (obj is Nodes.Node)
            {
                node = (Nodes.Node)obj;

            }
            else if (obj is Attachments.Attachment)
            {
                node = ((Attachments.Attachment)obj).Node;

            }
            else
            {
                throw new Exception(string.Format(Resources.ExceptionDesignerAttributeCouldNotRetrieveNode, obj));
            }

            string[] excludedProperties = node.GetNodePropertyExcludedProperties();

            DesignerNodeProperty attribute = (DesignerNodeProperty)property.Attribute;

            comboBox.Items.Add(Resources.DesignerNodePropertyNone);

            IList<DesignerPropertyInfo> properties = node.GetDesignerProperties();

            foreach (DesignerPropertyInfo dpi in properties)
            {
                bool supported = false;

                // check if the property is allowed
                foreach (Type supportedType in attribute.SupportedTypes)
                {
                    if (dpi.Property.PropertyType == supportedType)
                    {
                        supported = true;

                        break;
                    }
                }

                // skip this property if it is not supported
                if (!supported)
                {
                    continue;
                }

                // check if the node denies using this property
                foreach (string excludedProperty in excludedProperties)
                {
                    if (dpi.Property.Name == excludedProperty)
                    {
                        supported = false;

                        break;
                    }
                }

                // skip this property if it is not supported
                if (!supported)
                {
                    continue;
                }

                // add the property to the list
                comboBox.Items.Add(dpi.Property.Name);
            }

            comboBox.Text = (string)property.Property.GetValue(obj, null);

            if (comboBox.Text.Length < 1)
            {
                comboBox.SelectedIndex = 0;
            }
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            comboBox.Enabled = false;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex < 0 || !_valueWasAssigned)
            {
                return;
            }

            _property.Property.SetValue(_object, comboBox.Text, null);

            OnValueChanged(_property);
        }

        private void comboBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void DesignerNodePropertyEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Alt)
            {
                e.Handled = true;
            }
        }
    }
}
