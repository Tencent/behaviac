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

namespace Behaviac.Design
{
    public partial class EditEnumMemberDialog : Form
    {
        public EditEnumMemberDialog(EnumType enumType, EnumType.EnumMemberType enumMember)
        {
            InitializeComponent();

            this.Owner = MainWindow.Instance;
            this.Text = (enumMember == null) ? Resources.AddEnumMember : Resources.EditEnumMember;

            this.editEnumMemberPanel.Initialize(enumType, enumMember);
        }

        public EnumType.EnumMemberType GetEnumMember()
        {
            return this.editEnumMemberPanel.GetEnumMember();
        }

        private void EditEnumMemberDialog_Resize(object sender, EventArgs e)
        {
            this.editEnumMemberPanel.Width = this.Width - 22;
        }

        private bool _isSetting = false;

        private void okButton_Click(object sender, EventArgs e)
        {
            _isSetting = true;
        }

        private void EditEnumMemberDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isSetting && !this.editEnumMemberPanel.Verify())
            {
                e.Cancel = true;

                MessageBox.Show(Resources.EnumMemberVerifyWarning, Resources.Warning, MessageBoxButtons.OK);
            }

            _isSetting = false;
        }
    }
}
