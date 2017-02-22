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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    internal partial class EditEnumMemberPanel : UserControl
    {
        private EnumType _enumType = null;
        private EnumType.EnumMemberType _enumMember = null;
        private EnumType.EnumMemberType _originalMember = null;
        private bool _initialized = false;
        private bool _isNew = false;
        private bool _updateDisplayNameByName = false;

        public EditEnumMemberPanel()
        {
            InitializeComponent();
        }

        public void Initialize(EnumType enumType, EnumType.EnumMemberType enumMember)
        {
            _initialized = false;
            _isModified = false;
            _enumType = enumType;
            _originalMember = enumMember;

            if (enumMember == null)
            {
                _isNew = true;
                _enumMember = new EnumType.EnumMemberType(enumMember);
            }
            else
            {
                _isNew = false;
                _enumMember = enumMember;
            }

            this.nameTextBox.Text = _enumMember.Name;
            this.valueNumericUpDown.Value = _enumMember.Value;
            this.dispTextBox.Text = _enumMember.DisplayName;
            this.descTextBox.Text = _enumMember.Description;

            this.nameTextBox.Focus();

            if (this.nameTextBox.TextLength > 0)
            {
                this.nameTextBox.SelectionStart = this.nameTextBox.TextLength;
            }
            else
            {
                this.nameTextBox.Select();
            }

            _initialized = true;
        }

        public EnumType.EnumMemberType GetEnumMember()
        {
            Debug.Check(_enumMember != null);
            if (_enumMember != null)
            {
                _enumMember.Name = this.nameTextBox.Text;
                _enumMember.Value = (int)this.valueNumericUpDown.Value;
                _enumMember.DisplayName = this.dispTextBox.Text;
                _enumMember.Description = this.descTextBox.Text;

                return _enumMember;
            }

            return null;
        }

        private bool _isModified = false;
        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            set
            {
                _isModified = true;
            }
        }

        public bool Verify()
        {
            Debug.Check(_enumType != null);
            Debug.Check(_enumMember != null);

            if (_enumType != null && _enumMember != null)
            {
                string memberName = this.nameTextBox.Text;
                bool isValid = !string.IsNullOrEmpty(memberName) && Char.IsLetter(memberName[0]);

                if (isValid && _enumMember != null)
                {
                    EnumType.EnumMemberType member = _enumType.GetMemberByName(memberName);

                    if (_isNew)
                    {
                        isValid = (member == null);
                    }
                    else
                    {
                        isValid = (member == null || member == _originalMember);
                    }
                }

                return isValid;
            }

            return false;
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized && Verify())
            {
                _enumMember.Name = this.nameTextBox.Text;

                _updateDisplayNameByName = true;
                this.dispTextBox.Text = _enumMember.DisplayName;
                _updateDisplayNameByName = false;

                this.IsModified = true;
            }
        }

        private void dispTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_initialized && !_updateDisplayNameByName && (_enumMember.DisplayName != this.dispTextBox.Text))
            {
                _enumMember.DisplayName = this.dispTextBox.Text;

                this.IsModified = true;
            }
        }
    }
}
