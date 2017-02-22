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

namespace Behaviac.Design
{
    public class CRC32
    {
        static public uint CalcCRC(string str)
        {
            uint crc = 0XFFFFFFFF;

            for (int i = 0; i < str.Length; ++i)
            {
                char ch = str[i];
                byte b = (byte)ch;

                UpdateB(ref crc, b);
            }

            UpdateB(ref crc, (byte)str.Length);
            return crc;
        }

        static public uint CalcCRCNoCase(string str)
        {
            uint crc = 0XFFFFFFFF;

            for (int i = 0; i < str.Length; ++i)
            {
                char ch = str[i];
                byte b = (byte)char.ToLower(ch);

                UpdateB(ref crc, b);
            }

            UpdateB(ref crc, (byte)str.Length);
            return crc;
        }

        #region Private
        static private void UpdateB(ref uint crc, byte b)
        {
            crc = s_table[(crc ^ b) & 0XFF] ^ (crc >> 8);
        }
        static private void Update(ref uint crc, uint d)
        {
            UpdateB(ref crc, (byte)(d & 0XFF));
            UpdateB(ref crc, (byte)((d >> 8) & 0XFF));
            UpdateB(ref crc, (byte)((d >> 16) & 0XFF));
            UpdateB(ref crc, (byte)((d >> 24) & 0XFF));
        }
        static private void Finish(ref uint crc)
        {
            crc ^= 0XFFFFFFFF;
        }

        private static uint[] s_table =
        {
            0X00000000, 0X77073096, 0XEE0E612C, 0X990951BA, 0X076DC419, 0X706AF48F, 0XE963A535, 0X9E6495A3, 0X0EDB8832, 0X79DCB8A4,
            0XE0D5E91E, 0X97D2D988, 0X09B64C2B, 0X7EB17CBD, 0XE7B82D07, 0X90BF1D91, 0X1DB71064, 0X6AB020F2, 0XF3B97148, 0X84BE41DE,
            0X1ADAD47D, 0X6DDDE4EB, 0XF4D4B551, 0X83D385C7, 0X136C9856, 0X646BA8C0, 0XFD62F97A, 0X8A65C9EC, 0X14015C4F, 0X63066CD9,
            0XFA0F3D63, 0X8D080DF5, 0X3B6E20C8, 0X4C69105E, 0XD56041E4, 0XA2677172, 0X3C03E4D1, 0X4B04D447, 0XD20D85FD, 0XA50AB56B,
            0X35B5A8FA, 0X42B2986C, 0XDBBBC9D6, 0XACBCF940, 0X32D86CE3, 0X45DF5C75, 0XDCD60DCF, 0XABD13D59, 0X26D930AC, 0X51DE003A,
            0XC8D75180, 0XBFD06116, 0X21B4F4B5, 0X56B3C423, 0XCFBA9599, 0XB8BDA50F, 0X2802B89E, 0X5F058808, 0XC60CD9B2, 0XB10BE924,
            0X2F6F7C87, 0X58684C11, 0XC1611DAB, 0XB6662D3D, 0X76DC4190, 0X01DB7106, 0X98D220BC, 0XEFD5102A, 0X71B18589, 0X06B6B51F,
            0X9FBFE4A5, 0XE8B8D433, 0X7807C9A2, 0X0F00F934, 0X9609A88E, 0XE10E9818, 0X7F6A0DBB, 0X086D3D2D, 0X91646C97, 0XE6635C01,
            0X6B6B51F4, 0X1C6C6162, 0X856530D8, 0XF262004E, 0X6C0695ED, 0X1B01A57B, 0X8208F4C1, 0XF50FC457, 0X65B0D9C6, 0X12B7E950,
            0X8BBEB8EA, 0XFCB9887C, 0X62DD1DDF, 0X15DA2D49, 0X8CD37CF3, 0XFBD44C65, 0X4DB26158, 0X3AB551CE, 0XA3BC0074, 0XD4BB30E2,
            0X4ADFA541, 0X3DD895D7, 0XA4D1C46D, 0XD3D6F4FB, 0X4369E96A, 0X346ED9FC, 0XAD678846, 0XDA60B8D0, 0X44042D73, 0X33031DE5,
            0XAA0A4C5F, 0XDD0D7CC9, 0X5005713C, 0X270241AA, 0XBE0B1010, 0XC90C2086, 0X5768B525, 0X206F85B3, 0XB966D409, 0XCE61E49F,
            0X5EDEF90E, 0X29D9C998, 0XB0D09822, 0XC7D7A8B4, 0X59B33D17, 0X2EB40D81, 0XB7BD5C3B, 0XC0BA6CAD, 0XEDB88320, 0X9ABFB3B6,
            0X03B6E20C, 0X74B1D29A, 0XEAD54739, 0X9DD277AF, 0X04DB2615, 0X73DC1683, 0XE3630B12, 0X94643B84, 0X0D6D6A3E, 0X7A6A5AA8,
            0XE40ECF0B, 0X9309FF9D, 0X0A00AE27, 0X7D079EB1, 0XF00F9344, 0X8708A3D2, 0X1E01F268, 0X6906C2FE, 0XF762575D, 0X806567CB,
            0X196C3671, 0X6E6B06E7, 0XFED41B76, 0X89D32BE0, 0X10DA7A5A, 0X67DD4ACC, 0XF9B9DF6F, 0X8EBEEFF9, 0X17B7BE43, 0X60B08ED5,
            0XD6D6A3E8, 0XA1D1937E, 0X38D8C2C4, 0X4FDFF252, 0XD1BB67F1, 0XA6BC5767, 0X3FB506DD, 0X48B2364B, 0XD80D2BDA, 0XAF0A1B4C,
            0X36034AF6, 0X41047A60, 0XDF60EFC3, 0XA867DF55, 0X316E8EEF, 0X4669BE79, 0XCB61B38C, 0XBC66831A, 0X256FD2A0, 0X5268E236,
            0XCC0C7795, 0XBB0B4703, 0X220216B9, 0X5505262F, 0XC5BA3BBE, 0XB2BD0B28, 0X2BB45A92, 0X5CB36A04, 0XC2D7FFA7, 0XB5D0CF31,
            0X2CD99E8B, 0X5BDEAE1D, 0X9B64C2B0, 0XEC63F226, 0X756AA39C, 0X026D930A, 0X9C0906A9, 0XEB0E363F, 0X72076785, 0X05005713,
            0X95BF4A82, 0XE2B87A14, 0X7BB12BAE, 0X0CB61B38, 0X92D28E9B, 0XE5D5BE0D, 0X7CDCEFB7, 0X0BDBDF21, 0X86D3D2D4, 0XF1D4E242,
            0X68DDB3F8, 0X1FDA836E, 0X81BE16CD, 0XF6B9265B, 0X6FB077E1, 0X18B74777, 0X88085AE6, 0XFF0F6A70, 0X66063BCA, 0X11010B5C,
            0X8F659EFF, 0XF862AE69, 0X616BFFD3, 0X166CCF45, 0XA00AE278, 0XD70DD2EE, 0X4E048354, 0X3903B3C2, 0XA7672661, 0XD06016F7,
            0X4969474D, 0X3E6E77DB, 0XAED16A4A, 0XD9D65ADC, 0X40DF0B66, 0X37D83BF0, 0XA9BCAE53, 0XDEBB9EC5, 0X47B2CF7F, 0X30B5FFE9,
            0XBDBDF21C, 0XCABAC28A, 0X53B39330, 0X24B4A3A6, 0XBAD03605, 0XCDD70693, 0X54DE5729, 0X23D967BF, 0XB3667A2E, 0XC4614AB8,
            0X5D681B02, 0X2A6F2B94, 0XB40BBE37, 0XC30C8EA1, 0X5A05DF1B, 0X2D02EF8D
        };
        #endregion
    }
}
