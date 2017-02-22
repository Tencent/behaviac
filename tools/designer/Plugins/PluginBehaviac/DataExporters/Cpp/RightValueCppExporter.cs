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
using Behaviac.Design.Attributes;

namespace PluginBehaviac.DataExporters
{
    public class RightValueCppExporter
    {
        public static void GenerateClassConstructor(DefaultObject defaultObj, Behaviac.Design.RightValueDef rightValue, StringWriter stream, string indent, string var)
        {
            if (rightValue.IsMethod)
            {
                MethodCppExporter.GenerateClassConstructor(defaultObj, rightValue.Method, stream, indent, var);
            }
            else
            {
                VariableCppExporter.GenerateClassConstructor(defaultObj, rightValue.Var, stream, indent, var);
            }
        }

        public static void GenerateClassMember(Behaviac.Design.RightValueDef rightValue, StringWriter stream, string indent, string var)
        {
            if (rightValue.IsMethod)
            {
                MethodCppExporter.GenerateClassMember(rightValue.Method, stream, indent, var);
            }
            else
            {
                VariableCppExporter.GenerateClassMember(rightValue.Var, stream, indent, var);
            }
        }

        public static string GenerateCode(DefaultObject defaultObj, Behaviac.Design.RightValueDef rightValue, StringWriter stream, string indent, string typename, string var, string caller)
        {
            string retStr = string.Empty;

            if (rightValue.IsMethod)
            {
                retStr = MethodCppExporter.GenerateCode(defaultObj, rightValue.Method, stream, indent, rightValue.Method.NativeReturnType, var, caller);
            }
            else
            {
                retStr = VariableCppExporter.GenerateCode(defaultObj, rightValue.Var, false, stream, indent, typename, var, caller);
            }

            return retStr;
        }

        public static void PostGenerateCode(Behaviac.Design.RightValueDef rightValue, StringWriter stream, string indent, string typename, string var, string caller, object parent = null, string paramName = "")
        {
            if (rightValue.IsMethod)
            {
                MethodCppExporter.PostGenerateCode(rightValue.Method, stream, indent, rightValue.Method.NativeReturnType, var, string.Format("(({0}*)pAgent_{1})", rightValue.Method.ClassName, var));
            }
            else
            {
                VariableCppExporter.PostGenerateCode(rightValue.Var, stream, indent, typename, var, caller, parent, paramName);
            }
        }
    }
}
