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
    public partial class DesignerNumberEditor : Behaviac.Design.Attributes.DesignerPropertyEditor
    {
        public DesignerNumberEditor()
        {
            InitializeComponent();
        }

        public override void SetRange(double min, double max)
        {
            if (min > double.MinValue)
            {
                numericUpDown.Minimum = (decimal)min;
            }

            if (max < double.MaxValue)
            {
                numericUpDown.Maximum = (decimal)max;
            }
        }

        public void SetRange(int precision, decimal min, decimal max, decimal increment, string unit = "")
        {
            numericUpDown.DecimalPlaces = precision;
            numericUpDown.Minimum = min;
            numericUpDown.Maximum = max;
            numericUpDown.Increment = increment;

            unitLabel.Text = unit;
            unitLabel.Visible = !string.IsNullOrEmpty(unitLabel.Text);
            DesignerNumberEditor_Resize(this, new EventArgs());
        }

        public override void SetProperty(DesignerPropertyInfo property, object obj)
        {
            base.SetProperty(property, obj);

            // check if there is an override for this paroperty
            Nodes.Node node = _object as Nodes.Node;

            if (node != null && node.HasOverrride(property.Property.Name))
            {
                numericUpDown.Enabled = false;

                return;
            }

            DesignerPropertyInfo restrictions = property;

            bool linkBroken;
            DesignerPropertyInfo linkedProperty = property.Attribute.GetLinkedProperty(obj, out linkBroken);

            // control cannot be used with a broken link
            if (linkBroken)
            {
                numericUpDown.Enabled = false;

                return;
            }

            // if we have a linked property this property will define the restrictions
            if (linkedProperty.Property != null)
            {
                restrictions = linkedProperty;
            }

            // extract resrictions for float property
            DesignerFloat restFloatAtt = restrictions.Attribute as DesignerFloat;

            if (restFloatAtt != null)
            {
                SetRange(restFloatAtt.Precision,
                         (decimal)restFloatAtt.Min,
                         (decimal)restFloatAtt.Max,
                         (decimal)restFloatAtt.Steps,
                         restFloatAtt.Units);
            }

            // extract restrictions for int property
            DesignerInteger restIntAtt = restrictions.Attribute as DesignerInteger;

            if (restIntAtt != null)
            {
                SetRange(0,
                         (decimal)restIntAtt.Min,
                         (decimal)restIntAtt.Max,
                         (decimal)restIntAtt.Steps,
                         restIntAtt.Units);
            }

            // extract the value
            decimal value = 0;

            DesignerFloat floatAtt = property.Attribute as DesignerFloat;

            if (floatAtt != null)
            {
                float val = (float)property.Property.GetValue(obj, null);

                value = (decimal)val;
            }

            DesignerInteger intAtt = property.Attribute as DesignerInteger;

            if (intAtt != null)
            {
                int val = 0;

                try
                {
                    val = Convert.ToInt32(property.Property.GetValue(obj, null));
                }
                catch
                {
                }

                value = (decimal)val;
            }

            // assign value within limits
            numericUpDown.Value = Math.Max(numericUpDown.Minimum, Math.Min(numericUpDown.Maximum, value));
        }

        public override void SetArrayProperty(DesignerArrayPropertyInfo arrayProperty, object obj)
        {
            base.SetArrayProperty(arrayProperty, obj);

            decimal value = 0;

            if (arrayProperty.ItemType == typeof(float))
            {
                const float maxValue = 1000000000000;
                SetRange(2,
                         (decimal)(-maxValue),
                         (decimal)maxValue,
                         (decimal)0.01);

                float val = (float)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(double))
            {
                const double maxValue = 1000000000000;
                SetRange(2,
                         (decimal)(-maxValue),
                         (decimal)maxValue,
                         (decimal)0.01);

                double val = (double)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(int))
            {
                SetRange(0,
                         (decimal)int.MinValue,
                         (decimal)int.MaxValue,
                         (decimal)1);

                int val = (int)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(uint))
            {
                SetRange(0,
                         (decimal)uint.MinValue,
                         (decimal)uint.MaxValue,
                         (decimal)1);

                int val = (int)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(short))
            {
                SetRange(0,
                         (decimal)short.MinValue,
                         (decimal)short.MaxValue,
                         (decimal)1);

                short val = (short)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(ushort))
            {
                SetRange(0,
                         (decimal)ushort.MinValue,
                         (decimal)ushort.MaxValue,
                         (decimal)1);

                ushort val = (ushort)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(sbyte))
            {
                SetRange(0,
                         (decimal)sbyte.MinValue,
                         (decimal)sbyte.MaxValue,
                         (decimal)1);

                sbyte val = (sbyte)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(byte))
            {
                SetRange(0,
                         (decimal)byte.MinValue,
                         (decimal)byte.MaxValue,
                         (decimal)1);

                byte val = (byte)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(long))
            {
                SetRange(0,
                         (decimal)long.MinValue,
                         (decimal)long.MaxValue,
                         (decimal)1);

                long val = (long)arrayProperty.Value;
                value = (decimal)val;

            }
            else if (arrayProperty.ItemType == typeof(ulong))
            {
                SetRange(0,
                         (decimal)ulong.MinValue,
                         (decimal)ulong.MaxValue,
                         (decimal)1);

                ulong val = (ulong)arrayProperty.Value;
                value = (decimal)val;

            }
            else
            {
                Debug.Check(false);
            }

            numericUpDown.Value = Math.Max(numericUpDown.Minimum, Math.Min(numericUpDown.Maximum, value));
        }

        public override void SetParameter(MethodDef.Param param, object obj, bool bReadonly)
        {
            base.SetParameter(param, obj, bReadonly);

            // extract resrictions for float property
            DesignerFloat restFloatAtt = param.Attribute as DesignerFloat;

            if (restFloatAtt != null)
            {
                SetRange(restFloatAtt.Precision,
                         (decimal)restFloatAtt.Min,
                         (decimal)restFloatAtt.Max,
                         (decimal)restFloatAtt.Steps,
                         restFloatAtt.Units);
            }

            // extract restrictions for int property
            DesignerInteger restIntAtt = param.Attribute as DesignerInteger;

            if (restIntAtt != null)
            {
                SetRange(0,
                         (decimal)restIntAtt.Min,
                         (decimal)restIntAtt.Max,
                         (decimal)restIntAtt.Steps,
                         restIntAtt.Units);
            }

            // extract the value
            decimal value = 0;

            DesignerFloat floatAtt = param.Attribute as DesignerFloat;

            if (floatAtt != null)
            {
                float val = (param.Value != null) ? float.Parse(param.Value.ToString()) : 0.0f;
                value = (decimal)val;
                numericUpDown.DecimalPlaces = 2;
            }

            DesignerInteger intAtt = param.Attribute as DesignerInteger;

            if (intAtt != null)
            {
                int val = (param.Value != null) ? int.Parse(param.Value.ToString()) : 0;
                value = (decimal)val;
            }

            // assign value within limits
            numericUpDown.Value = Math.Max(numericUpDown.Minimum, Math.Min(numericUpDown.Maximum, value));
        }

        public override void SetVariable(VariableDef variable, object obj)
        {
            base.SetVariable(variable, obj);

            if (variable != null)
            {
                // extract the value
                decimal value = 0;

                Type valueType = variable.ValueType;

                if (Plugin.IsFloatType(valueType))
                {
                    const float maxValue = 1000000000000;
                    SetRange(2,
                             (decimal)(-maxValue),
                             (decimal)maxValue,
                             (decimal)0.01);

                    float val = Convert.ToSingle(variable.Value);
                    value = (decimal)val;
                    numericUpDown.DecimalPlaces = 2;

                }
                else if (valueType == typeof(int))
                {
                    SetRange(0,
                             (decimal)int.MinValue,
                             (decimal)int.MaxValue,
                             (decimal)1);

                    int val = (int)variable.Value;
                    value = (decimal)val;

                }
                else if (valueType == typeof(uint))
                {
                    SetRange(0,
                             (decimal)int.MinValue,
                             (decimal)int.MaxValue,
                             (decimal)1);

                    uint val = (uint)variable.Value;
                    value = (decimal)val;

                }
                else if (valueType == typeof(short))
                {
                    SetRange(0,
                             (decimal)short.MinValue,
                             (decimal)short.MaxValue,
                             (decimal)1);

                    short val = (short)variable.Value;
                    value = (decimal)val;

                }
                else if (valueType == typeof(ushort))
                {
                    SetRange(0,
                             (decimal)ushort.MinValue,
                             (decimal)ushort.MaxValue,
                             (decimal)1);

                    ushort val = (ushort)variable.Value;
                    value = (decimal)val;

                }
                else if (valueType == typeof(sbyte))
                {
                    SetRange(0,
                             (decimal)sbyte.MinValue,
                             (decimal)sbyte.MaxValue,
                             (decimal)1);

                    sbyte val = (sbyte)variable.Value;
                    value = (decimal)val;

                }
                else if (valueType == typeof(byte))
                {
                    SetRange(0,
                             (decimal)byte.MinValue,
                             (decimal)byte.MaxValue,
                             (decimal)1);

                    byte val = (byte)variable.Value;
                    value = (decimal)val;

                }
                else if (valueType == typeof(long))
                {
                    SetRange(0,
                             (decimal)int.MinValue,
                             (decimal)int.MaxValue,
                             (decimal)1);

                    long val = (long)variable.Value;
                    value = (decimal)val;

                }
                else if (valueType == typeof(ulong))
                {
                    SetRange(0,
                             (decimal)uint.MinValue,
                             (decimal)uint.MaxValue,
                             (decimal)1);

                    ulong val = (ulong)variable.Value;
                    value = (decimal)val;
                }

                // assign value within limits
                numericUpDown.Value = Math.Max(numericUpDown.Minimum, Math.Min(numericUpDown.Maximum, value));
            }
        }

        public override void ReadOnly()
        {
            base.ReadOnly();

            numericUpDown.Enabled = false;
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_valueWasAssigned)
            {
                return;
            }

            try
            {
                if (this._property.Property != null)
                {
                    if (this._property.Attribute is DesignerFloat)
                    {
                        this._property.Property.SetValue(_object, (float)numericUpDown.Value, null);
                    }

                    else if (this._property.Attribute is DesignerInteger)
                    {
                        this._property.Property.SetValue(_object, (int)numericUpDown.Value, null);
                    }

                    else
                    {
                        Debug.Check(false);
                    }

                }
                else if (_arrayProperty != null)
                {
                    if (_arrayProperty.ItemType == typeof(float))
                    {
                        _arrayProperty.Value = (float)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(double))
                    {
                        _arrayProperty.Value = (double)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(int))
                    {
                        _arrayProperty.Value = (int)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(uint))
                    {
                        _arrayProperty.Value = (uint)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(short))
                    {
                        _arrayProperty.Value = (short)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(ushort))
                    {
                        _arrayProperty.Value = (ushort)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(sbyte))
                    {
                        _arrayProperty.Value = (sbyte)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(byte))
                    {
                        _arrayProperty.Value = (byte)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(long))
                    {
                        _arrayProperty.Value = (long)numericUpDown.Value;
                    }

                    else if (_arrayProperty.ItemType == typeof(ulong))
                    {
                        _arrayProperty.Value = (ulong)numericUpDown.Value;
                    }

                    else
                    {
                        Debug.Check(false);
                    }

                }
                else if (this._param != null)
                {
                    bool bFloat = false;

                    if (this._param.ListParam != null)
                    {
                        Type itemType = MethodDef.Param.GetListParamItemType(this._param);

                        if (Plugin.IsFloatType(itemType))
                        {
                            bFloat = true;
                        }
                    }

                    if (this._param.Attribute is DesignerFloat || bFloat)
                    {
                        float value = (float)numericUpDown.Value;

                        if (this._param.Value is float)
                        {
                            this._param.Value = (float)value;
                        }
                        else if (this._param.Value is double)
                        {
                            double d = (double)value;
                            this._param.Value = d;
                        }
                        else
                        {
                            this._param.Value = value;
                        }
                    }
                    else if (this._param.Attribute is DesignerInteger)
                    {
                        this._param.Value = (int)numericUpDown.Value;
                    }
                    else
                    {
                        Debug.Check(false);
                    }

                }
                else if (this._variable != null)
                {
                    if (this._variable.Value.GetType() == typeof(float))
                    {
                        this._variable.Value = (float)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(double))
                    {
                        this._variable.Value = (double)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(int))
                    {
                        this._variable.Value = (int)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(uint))
                    {
                        this._variable.Value = (uint)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(short))
                    {
                        this._variable.Value = (short)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(ushort))
                    {
                        this._variable.Value = (ushort)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(sbyte))
                    {
                        this._variable.Value = (sbyte)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(byte))
                    {
                        this._variable.Value = (byte)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(long))
                    {
                        this._variable.Value = (long)numericUpDown.Value;
                    }

                    else if (this._variable.Value.GetType() == typeof(ulong))
                    {
                        this._variable.Value = (ulong)numericUpDown.Value;
                    }

                    else
                    {
                        Debug.Check(false);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            OnValueChanged(_property);
        }

        private void DesignerNumberEditor_Resize(object sender, EventArgs e)
        {
            if (unitLabel.Visible)
            {
                numericUpDown.Size = new System.Drawing.Size(Size.Width - unitLabel.Width - 2, Size.Height);
            }

            else
            {
                numericUpDown.Size = new System.Drawing.Size(Size.Width - 7, Size.Height);
            }
        }

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }
    }
}
