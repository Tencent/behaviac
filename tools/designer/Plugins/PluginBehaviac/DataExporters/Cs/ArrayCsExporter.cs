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
    public class ArrayCsExporter
    {
        public static void GenerateCode(object obj, DefaultObject defaultObj, StringWriter stream, string indent, string itemTypename, string var)
        {
            if (obj != null)
            {
                Type type = obj.GetType();

                if (Plugin.IsArrayType(type))
                {
                    System.Collections.IList list = (System.Collections.IList)obj;

                    if (list.Count > 0)
                    {
                        stream.WriteLine("{0}{1} = new {2}();", indent, var, DataCsExporter.GetGeneratedNativeType(type));
                        stream.WriteLine("{0}{1}.Capacity = {2};", indent, var, list.Count);

                        for (int i = 0; i < list.Count; ++i)
                        {
                            string itemName = string.Format("{0}_item{1}", var.Replace(".", "_"), i);

                            DataCsExporter.GenerateCode(list[i], defaultObj, stream, indent, itemTypename, itemName, string.Empty);

                            stream.WriteLine("{0}{1}.Add({2});", indent, var, itemName);
                        }
                    }
                }
            }
        }
    }
}
