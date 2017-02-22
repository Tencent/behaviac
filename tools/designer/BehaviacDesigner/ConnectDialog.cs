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
using System.Net;
using System.Net.Sockets;

namespace Behaviac.Design
{
    public partial class ConnectDialog : Form
    {
        public ConnectDialog(bool useLocalIp, string ip, int portNr)
        {
            InitializeComponent();

            localIPCheckBox.Checked = useLocalIp;
            tbServer.Text = !useLocalIp && Utilities.IPOnlyNumbersAndDots(ip) ? ip : Utilities.GetLocalIP();
            tbServer.Enabled = !useLocalIp;
            tbPort.Text = portNr.ToString();
        }

        public bool UseLocalIP()
        {
            return localIPCheckBox.Checked;
        }

        public String GetServer()
        {
            return Utilities.IPOnlyNumbersAndDots(tbServer.Text) ? tbServer.Text : Utilities.GetIP(tbServer.Text);
        }

        public int GetPort()
        {
            return Convert.ToInt32(tbPort.Text);
        }

        private void tbPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void localIPCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.UseLocalIP())
            {
                tbServer.Text = Utilities.GetLocalIP();
                tbServer.Enabled = false;
            }
            else
            {
                tbServer.Enabled = true;
            }
        }
    }
}
