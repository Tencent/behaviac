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
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerCompositeEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerCompositeEditor()
        {
            InitializeComponent();
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            textBox.Enabled = false;
            button.Enabled = false;
        }

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            object value = property.GetValue(obj);

            if (value != null)
            {
                setProperty(value, property.Attribute.DisplayName, property.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnly), obj);
            }

            else
            {
                update();
            }
        }

        public override void SetVariable(VariableDef value, object obj)
        {
            base.SetVariable(value, obj);

            if (value != null)
            {
                setProperty(value.Value, "", false, obj);
            }

            else
            {
                update();
            }
        }

        public override void SetArrayProperty(DesignerArrayPropertyInfo arrayProperty, object obj)
        {
            base.SetArrayProperty(arrayProperty, obj);

            update();
        }

        public override void SetStructProperty(DesignerStructPropertyInfo structProperty, object obj)
        {
            base.SetStructProperty(structProperty, obj);

            update();
        }

        public override void SetParameter(MethodDef.Param param, object obj, bool bReadonly)
        {
            base.SetParameter(param, obj, bReadonly);

            if (param != null)
            {
                setProperty(param.Value, param.Name, false, obj);
            }

            else
            {
                update();
            }
        }

        public override void SetPar(ParInfo par, object obj)
        {
            if (par != null)
            {
                base.SetPar(par, obj);
            }

            if (par != null && par.Variable != null)
            {
                setProperty(par.Variable.Value, par.Name, false, obj);
            }
            else
            {
                update();
            }
        }

        private void setProperty(object value, string name, bool readOnly, object obj)
        {
            if (value != null)
            {
                Type type = value.GetType();

                if (Plugin.IsArrayType(type))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    System.Collections.IList itemList = (System.Collections.IList)(value);

                    SetArrayProperty(new DesignerArrayPropertyInfo(name, itemType, itemList, -1, readOnly), obj);

                }
                else if (Plugin.IsCustomClassType(type))
                {
                    SetStructProperty(new DesignerStructPropertyInfo(name, type, value), obj);

                }
                else
                {
                    // Can't support other types.
                    Debug.Check(false);
                }

            }
            else
            {
                update();
            }
        }

        private void update()
        {
            if (_arrayProperty != null)
            {
                textBox.Text = DesignerArray.RetrieveDisplayValue(_arrayProperty.ItemList);
                button.Enabled = true;

            }
            else if (_structProperty != null)
            {
                //textBox.Text = "(Multiple properties)";
                MethodDef method = null;
                Nodes.Action action = this._object as Nodes.Action;

                if (action != null)
                {
                    method = action.Method;
                }

                textBox.Text = DesignerPropertyUtility.RetrieveDisplayValue(_structProperty.Owner, method, _structProperty.Name, _structProperty.ElmentIndexInArray);
                button.Enabled = true;

            }
            else
            {
                textBox.Text = "null";
                //textBox.Text = DesignerPropertyUtility.RetrieveDisplayValue(_property, null, null);
                button.Enabled = false;
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            using(CompositeEditorDialog dialog = new CompositeEditorDialog())
            {
                if (_arrayProperty != null)
                {
                    dialog.Text = string.IsNullOrEmpty(_arrayProperty.Name) ? Resources.Properties : string.Format(Resources.PropertiesOf, _arrayProperty.Name);
                    dialog.SetArrayProperty(_arrayProperty, _object);

                }
                else if (_structProperty != null)
                {
                    dialog.Text = string.IsNullOrEmpty(_structProperty.Name) ? Resources.Properties : string.Format(Resources.PropertiesOf, _structProperty.Name);
                    dialog.SetStructProperty(_structProperty, _object);
                }

                dialog.ShowDialog();

                update();

                if (dialog.IsModified)
                {
                    OnValueChanged(new DesignerPropertyInfo());
                }
            }
        }

        private void DesignerArrayEditor_Resize(object sender, EventArgs e)
        {
            button.Size = new System.Drawing.Size(22, 22);
            button.Location = new System.Drawing.Point(Size.Width - button.Size.Width - 2, button.Location.Y);

            textBox.Size = new System.Drawing.Size(Size.Width - button.Size.Width - 6, textBox.Size.Height);
        }
    }
}
