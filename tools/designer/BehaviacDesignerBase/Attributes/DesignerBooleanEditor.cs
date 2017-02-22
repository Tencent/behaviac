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

namespace Behaviac.Design.Attributes
{
    public partial class DesignerBooleanEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerBooleanEditor()
        {
            InitializeComponent();
        }

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            DesignerBoolean boolAtt = property.Attribute as DesignerBoolean;

            if (boolAtt != null)
            {
                checkBox.Checked = (bool)property.Property.GetValue(obj, null);
            }
        }

        public override void SetArrayProperty(DesignerArrayPropertyInfo arrayProperty, object obj)
        {
            base.SetArrayProperty(arrayProperty, obj);

            checkBox.Checked = (arrayProperty.Value != null) ? (bool)arrayProperty.Value : false;
        }

        public override void SetParameter(MethodDef.Param param, object obj, bool bReadonly)
        {
            base.SetParameter(param, obj, bReadonly);

            DesignerBoolean boolAtt = param.Attribute as DesignerBoolean;

            if (boolAtt != null)
            {
                checkBox.Checked = (bool)param.Value;
            }
        }

        public override void SetVariable(VariableDef variable, object obj)
        {
            base.SetVariable(variable, obj);

            if (variable != null)
            {
                Type valueType = variable.ValueType;

                if (valueType == typeof(bool))
                {
                    checkBox.Checked = (bool)variable.Value;
                }
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_valueWasAssigned)
            {
                return;
            }

            if (_property.Property != null)
            {
                _property.Property.SetValue(_object, checkBox.Checked, null);

            }
            else if (_arrayProperty != null)
            {
                _arrayProperty.Value = checkBox.Checked;

            }
            else if (_param != null)
            {
                Debug.Check(_param.Attribute is DesignerBoolean);
                _param.Value = checkBox.Checked;

            }
            else
            {
                if (_variable != null)
                {
                    _variable.Value = checkBox.Checked;

                }
                else
                {
                    Debug.Check(false);
                }
            }

            OnValueChanged(_property);
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            checkBox.Enabled = false;
        }

        private void checkBox_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }
    }
}
