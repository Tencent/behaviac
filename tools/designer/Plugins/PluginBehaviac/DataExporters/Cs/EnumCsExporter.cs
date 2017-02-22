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
using System.Text;
using System.IO;
using Behaviac.Design;

namespace PluginBehaviac.DataExporters
{
    public class EnumCsExporter
    {
        public static string GeneratedCode(object obj)
        {
            Debug.Check(obj != null);

            if (obj != null)
            {
                Type type = obj.GetType();
                Debug.Check(type.IsEnum);

                string enumName = type.Name;
                string memberName = Enum.GetName(type, obj);

                Attribute[] enumAttributes = (Attribute[])obj.GetType().GetCustomAttributes(typeof(EnumDescAttribute), false);

                if (enumAttributes.Length > 0)
                {
                    enumName = ((EnumDescAttribute)enumAttributes[0]).Fullname;
                    enumName = enumName.Replace("::", ".");
                }

                return enumName + "." + memberName;
            }

            return string.Empty;
        }
    }
}
