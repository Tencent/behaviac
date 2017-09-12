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
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;
using Behaviac.Design.Attributes;
using PluginBehaviac.Properties;
using PluginBehaviac.NodeExporters;
using PluginBehaviac.DataExporters;

namespace PluginBehaviac.Exporters
{
    public class ExporterCpp : Behaviac.Design.Exporters.Exporter
    {
        class BehaviorCreator
        {
            public BehaviorCreator(string filename, string classname)
            {
                Filename = filename;
                Classname = classname;
            }

            public string Filename;
            public string Classname;
        }

        List<BehaviorCreator> _behaviorCreators = new List<BehaviorCreator>();

        public ExporterCpp(BehaviorNode node, string outputFolder, string filename, List<string> includedFilenames = null)
            : base(node, outputFolder, filename, includedFilenames)
        {
            //automatically create an extra level of path
            _outputFolder = Path.Combine(Path.GetFullPath(_outputFolder), "behaviac_generated");
            _filename = "behaviors/behaviac_generated_behaviors.h";
        }

        public override Behaviac.Design.FileManagers.SaveResult Export(List<BehaviorNode> behaviors, bool exportBehaviors, bool exportMeta, int exportFileCount)
        {
            string behaviorFilename = "behaviors/behaviac_generated_behaviors.h";
            string typesFolder = string.Empty;
            Behaviac.Design.FileManagers.SaveResult result = VerifyFilename(exportBehaviors, ref behaviorFilename, ref typesFolder);

            if (Behaviac.Design.FileManagers.SaveResult.Succeeded == result)
            {
                // meta
                if (exportMeta)
                {
                    ExportTypesHeader(typesFolder);

                    string internalFolder = Path.Combine(typesFolder, "internal");

                    if (!Directory.Exists(internalFolder))
                    {
                        Directory.CreateDirectory(internalFolder);
                    }

                    ExportAgentHeader(internalFolder);
                    ExportAgentMeta(internalFolder);
                    ExportAgentMemberVisitor(internalFolder);

                    ExportAgentsDefinition(internalFolder);
                    ExportAgentsImplemention(internalFolder);

                    ExportCustomizedTypesDefinition(internalFolder);
                    ExportCustomizedTypesImplemention(internalFolder);
                }

                // behaviors
                if (exportBehaviors)
                {
                    string behaviorFolder = Path.GetDirectoryName(behaviorFilename);

                    ExportBehaviors(behaviors, behaviorFilename, exportFileCount);
                }
            }

            return result;
        }

        public override void PreviewAgentFile(AgentType agent)
        {
            string behaviacAgentDir = GetBehaviacTypesDir();
            string tmpFilename = Path.Combine(behaviacAgentDir, agent.BasicName + ".h");

            ExportAgentHeaderFile(agent, tmpFilename, true);

            PreviewFile(tmpFilename);
        }

        public override void PreviewEnumFile(EnumType enumType)
        {
            string behaviacAgentDir = GetBehaviacTypesDir();
            string tmpFilename = Path.Combine(behaviacAgentDir, enumType.Name + ".h");

            ExportEnumFile(null, enumType, tmpFilename);

            PreviewFile(tmpFilename);
        }

        public override void PreviewStructFile(StructType structType)
        {
            string behaviacAgentDir = GetBehaviacTypesDir();
            string tmpFilename = Path.Combine(behaviacAgentDir, structType.Name + ".h");

            ExportStructFile(null, structType, tmpFilename);

            PreviewFile(tmpFilename);
        }

        private void ExportBehaviors(List<BehaviorNode> behaviors, string filename, int exportFileCount)
        {
            using (StringWriter file = new StringWriter())  // behaviac_generated_behaviors.h
            {
                _behaviorCreators.Clear();

                using (StringWriter baseCppFile = new StringWriter())  // behaviac_generated_behaviors.cpp
                {
                    baseCppFile.WriteLine("// ---------------------------------------------------------------------");
                    baseCppFile.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                    baseCppFile.WriteLine("// ---------------------------------------------------------------------");
                    baseCppFile.WriteLine();

                    baseCppFile.WriteLine("#include \"behaviac/behaviac.h\"");
                    baseCppFile.WriteLine();
                    baseCppFile.WriteLine("#include \"behaviac_generated_behaviors.h\"");

                    if (exportFileCount <= 1)
                    {
                        file.WriteLine("// ---------------------------------------------------------------------");
                        file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                        file.WriteLine("// ---------------------------------------------------------------------");
                        file.WriteLine();

                        string headerFileMacro = "_BEHAVIAC_GENERATED_BEHAVIORS_H_";

                        file.WriteLine("#ifndef {0}", headerFileMacro);
                        file.WriteLine("#define {0}", headerFileMacro);
                        file.WriteLine();

                        file.WriteLine("#include \"../types/behaviac_types.h\"");
                        file.WriteLine();

                        file.WriteLine("namespace behaviac");
                        file.WriteLine("{");

                        foreach (BehaviorNode behavior in behaviors)
                        {
                            behavior.PreExport();

                            _behaviorCreators.Add(ExportBody(file, behavior, false));

                            behavior.PostExport();
                        }

                        file.WriteLine("}");
                        file.WriteLine("#endif // {0}", headerFileMacro);

                        UpdateFile(file, filename);
                    }
                    else
                    {
                        string ext = Path.GetExtension(filename);
                        int unitNum = 0;
                        int behaviorsCount = behaviors.Count;
                        int behaviorUnitSize = behaviorsCount / exportFileCount;

                        if (behaviorUnitSize < 1)
                        {
                            behaviorUnitSize = 1;
                        }

                        for (int i = 0; i < behaviorsCount; i += behaviorUnitSize)
                        {
                            Debug.Check(unitNum < 10000);

                            string unitHeaderFileName = filename.Replace(ext, "_" + unitNum + ".h");
                            string unitCppFileName = filename.Replace(ext, "_" + unitNum + ".cpp");

                            StringWriter headerSW = new StringWriter();
                            StringWriter cppSW = new StringWriter();

                            // header
                            headerSW.WriteLine("// ---------------------------------------------------------------------");
                            headerSW.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                            headerSW.WriteLine("// ---------------------------------------------------------------------");
                            headerSW.WriteLine();
                            headerSW.WriteLine("#include \"../types/behaviac_types.h\"");
                            headerSW.WriteLine();

                            headerSW.WriteLine("namespace behaviac");
                            headerSW.WriteLine("{");

                            // cpp
                            cppSW.WriteLine("// ---------------------------------------------------------------------");
                            cppSW.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                            cppSW.WriteLine("// ---------------------------------------------------------------------");
                            cppSW.WriteLine();

                            string unitHeaderFile = Path.GetFileName(unitHeaderFileName);
                            baseCppFile.WriteLine("#include \"{0}\"", unitHeaderFile);

                            cppSW.WriteLine("#include \"{0}\"", unitHeaderFile);
                            cppSW.WriteLine();
                            cppSW.WriteLine("namespace behaviac");
                            cppSW.WriteLine("{");

                            for (int k = 0; k < behaviorUnitSize && i + k < behaviorsCount; ++k)
                            {
                                BehaviorNode behavior = behaviors[i + k];

                                behavior.PreExport();

                                // cpp
                                BehaviorCreator creator = ExportBody(cppSW, behavior, true);
                                _behaviorCreators.Add(creator);

                                // header
                                headerSW.WriteLine("\tclass {0}", creator.Classname);
                                headerSW.WriteLine("\t{");
                                headerSW.WriteLine("\tpublic:");
                                headerSW.WriteLine("\t\tstatic bool Create(BehaviorTree* pBT);");
                                headerSW.WriteLine("\t};");
                                headerSW.WriteLine();

                                behavior.PostExport();
                            }

                            // header
                            headerSW.WriteLine("}");

                            // cpp
                            cppSW.WriteLine("}");

                            UpdateFile(headerSW, unitHeaderFileName);
                            UpdateFile(cppSW, unitCppFileName);

                            unitNum++;
                        }
                    }

                    baseCppFile.WriteLine();
                    baseCppFile.WriteLine("namespace behaviac");
                    baseCppFile.WriteLine("{");

                    ExportTail(baseCppFile);

                    baseCppFile.WriteLine("}");
                    baseCppFile.Close();

                    string cppFileName = Path.ChangeExtension(filename, ".cpp");

                    UpdateFile(baseCppFile, cppFileName);
                }

                UpdateFile(file, filename);
            }
        }

        private Behaviac.Design.FileManagers.SaveResult VerifyFilename(bool exportBehaviors, ref string behaviorFilename, ref string typesFolder)
        {
            behaviorFilename = Path.Combine(_outputFolder, behaviorFilename);
            typesFolder = Path.Combine(_outputFolder, "types");

            // get the abolute folder of the file we want to export
            string folder = Path.GetDirectoryName(behaviorFilename);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (!Directory.Exists(typesFolder))
            {
                Directory.CreateDirectory(typesFolder);
            }

            if (exportBehaviors)
            {
                // verify it can be writable
                return Behaviac.Design.FileManagers.FileManager.MakeWritable(behaviorFilename, Resources.ExportFileWarning);
            }

            return Behaviac.Design.FileManagers.SaveResult.Succeeded;
        }

        private List<string> GetNamespaces(string ns)
        {
            List<string> namespaces = new List<string>();
            if (!string.IsNullOrEmpty(ns))
            {
                int startIndex = 0;

                for (int i = 0; i < ns.Length; ++i)
                {
                    if (ns[i] == ':')
                    {
                        Debug.Check(ns[i + 1] == ':');

                        namespaces.Add(ns.Substring(startIndex, i - startIndex));
                        startIndex = i + 2;
                        ++i;
                    }
                }

                ns = ns.Substring(startIndex, ns.Length - startIndex);

                if (!string.IsNullOrEmpty(ns))
                {
                    namespaces.Add(ns);
                }
            }

            return namespaces;
        }

        private string WriteNamespacesHead(StringWriter file, List<string> namespaces)
        {
            string indent = string.Empty;

            for (int i = 0; i < namespaces.Count; ++i)
            {
                file.WriteLine("{0}namespace {1}", indent, namespaces[i]);
                file.WriteLine("{0}{{", indent);
                indent += '\t';
            }

            return indent;
        }

        private void WriteNamespacesTail(StringWriter file, List<string> namespaces)
        {
            for (int i = 0; i < namespaces.Count; ++i)
            {
                string indent = string.Empty;

                for (int k = i + 1; k < namespaces.Count; ++k)
                {
                    indent += '\t';
                }

                file.WriteLine("{0}}}", indent);
            }
        }

        private void ExportBehaviacHeader(StringWriter file)
        {
            file.WriteLine("#include \"behaviac/behaviac.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/behaviortree.h\"");
            file.WriteLine("#include \"behaviac/agent/agent.h\"");
            file.WriteLine("#include \"behaviac/common/meta.h\"");
            file.WriteLine();

            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/action.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/assignment.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/compute.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/end.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/noop.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/wait.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/waitforsignal.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/actions/waitframes.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/compositestochastic.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/ifelse.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/parallel.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/referencebehavior.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/selector.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/selectorloop.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/selectorprobability.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/selectorstochastic.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/sequence.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/sequencestochastic.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/composites/withprecondition.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/conditions/and.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/conditions/conditionbase.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/conditions/condition.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/conditions/false.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/conditions/or.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/conditions/true.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratoralwaysfailure.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratoralwaysrunning.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratoralwayssuccess.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorcount.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorcountlimit.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorfailureuntil.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorframes.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratoriterator.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorlog.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorloop.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorloopuntil.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratornot.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorrepeat.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorsuccessuntil.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratortime.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/nodes/decorators/decoratorweight.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/attachments/event.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/attachments/attachaction.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/attachments/precondition.h\"");
            file.WriteLine("#include \"behaviac/behaviortree/attachments/effector.h\"");
            file.WriteLine("#include \"behaviac/htn/task.h\"");
            file.WriteLine("#include \"behaviac/fsm/fsm.h\"");
            file.WriteLine("#include \"behaviac/fsm/state.h\"");
            file.WriteLine("#include \"behaviac/fsm/startcondition.h\"");
            file.WriteLine("#include \"behaviac/fsm/transitioncondition.h\"");
            file.WriteLine("#include \"behaviac/fsm/waitstate.h\"");
            file.WriteLine("#include \"behaviac/fsm/waitframesstate.h\"");
            file.WriteLine("#include \"behaviac/fsm/alwaystransition.h\"");
            file.WriteLine("#include \"behaviac/fsm/waittransition.h\"");
        }

        private void ExportTypesHeader(string agentFolder)
        {
            using (StringWriter file = new StringWriter())
            {
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine();

                string headerFileMacro = "_BEHAVIAC_TYPES_H_";

                file.WriteLine("#ifndef {0}", headerFileMacro);
                file.WriteLine("#define {0}", headerFileMacro);
                file.WriteLine();

                ExportBehaviacHeader(file);
                file.WriteLine();

                file.WriteLine("#include \"internal/behaviac_agent_headers.h\"");
                file.WriteLine("#include \"internal/behaviac_agent_member_visitor.h\"");

                if (TypeManager.Instance.Enums.Count > 0 || TypeManager.Instance.Structs.Count > 0)
                {
                    file.WriteLine("#include \"internal/behaviac_customized_types.h\"");
                }

                file.WriteLine();
                file.WriteLine("#endif // {0}", headerFileMacro);

                const string kTypesFilename = "behaviac_types.h";
                string filename = Path.Combine(agentFolder, kTypesFilename);

                UpdateFile(file, filename);
            }
        }

        private void ExportAgentHeader(string agentFolder)
        {
            using (StringWriter file = new StringWriter())
            {
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine();

                string headerFileMacro = "_BEHAVIAC_HEADERS_H_";

                file.WriteLine("#ifndef {0}", headerFileMacro);
                file.WriteLine("#define {0}", headerFileMacro);
                file.WriteLine();

                file.WriteLine("#include \"behaviac/behaviac.h\"");
                file.WriteLine();

                file.WriteLine("// YOU SHOULD SET THE HEADER FILES OF YOUR GAME WHEN EXPORTING CPP FILES ON THE BEHAVIAC EDITOR:");

                // write included files for the game agents
                if (this.IncludedFilenames != null)
                {
                    string exportFullPath = Workspace.Current.GetExportAbsoluteFolder(Workspace.Current.Language);
                    string relativePath = "";

                    if (Workspace.Current.UseRelativePath)
                    {
                        relativePath = agentFolder.Replace('/', '\\');
                        relativePath = Workspace.MakeRelative(exportFullPath, relativePath, true, true);
                        relativePath = relativePath.Replace('\\', '/');
                        relativePath += "/";
                    }

                    foreach (string headerFilename in this.IncludedFilenames)
                    {
                        file.WriteLine("#include \"{0}{1}\"", relativePath, headerFilename);
                    }
                }

                file.WriteLine();

                if (TypeManager.Instance.Enums.Count > 0 || TypeManager.Instance.Structs.Count > 0)
                {
                    file.WriteLine("#include \"behaviac_customized_types.h\"");
                }

                file.WriteLine();
                file.WriteLine("#endif // {0}", headerFileMacro);

                string filename = Path.Combine(agentFolder, "behaviac_headers.h");

                UpdateFile(file, filename);
            }

            using (StringWriter file = new StringWriter())
            {
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine();

                string headerFileMacro = "_BEHAVIAC_AGENT_HEADERS_H_";

                file.WriteLine("#ifndef {0}", headerFileMacro);
                file.WriteLine("#define {0}", headerFileMacro);
                file.WriteLine();

                file.WriteLine("#include \"behaviac_headers.h\"");
                file.WriteLine();
                file.WriteLine("// THE FOLLOWING AGENT HEADER FILES IS GENERATED AUTOMATICALLY:");

                foreach (AgentType agent in Plugin.AgentTypes)
                {
                    if (!agent.IsImplemented)
                    {
                        file.WriteLine("#include \"{0}.h\"", agent.BasicName);
                    }
                }

                file.WriteLine();
                file.WriteLine("#endif // {0}", headerFileMacro);

                string filename = Path.Combine(agentFolder, "behaviac_agent_headers.h");

                UpdateFile(file, filename);
            }
        }

        private void ExportAgentMemberVisitor(string folder)
        {
            using (StringWriter file = new StringWriter())
            {
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine();

                string headerFileMacro = "_BEHAVIAC_MEMBER_VISITOR_H_";

                file.WriteLine("#ifndef {0}", headerFileMacro);
                file.WriteLine("#define {0}", headerFileMacro);
                file.WriteLine();

                file.WriteLine("#include \"behaviac_agent_headers.h\"");
                file.WriteLine();

                GenerateMemberHandler(file);

                file.WriteLine("#endif // {0}", headerFileMacro);

                string filename = Path.Combine(folder, "behaviac_agent_member_visitor.h");

                UpdateFile(file, filename);
            }
        }

        private void GenerateMemberHandler(StringWriter file)
        {
            // write property and method handlers
            file.WriteLine("// Agent property and method handlers\r\n");
            string allParamTypes = string.Empty;

            foreach (AgentType agenType in Plugin.AgentTypes)
            {
                List<string> namespaces = new List<string>();
                string ns = agenType.Namespace;

                if (!string.IsNullOrEmpty(ns))
                {
                    foreach (PropertyDef prop in agenType.GetProperties(true))
                    {
                        if (!prop.IsPublic && !prop.IsInherited && !prop.IsArrayElement)
                        {
                            namespaces = GetNamespaces(ns);
                            break;
                        }
                    }
                }

                if (namespaces.Count == 0 && !string.IsNullOrEmpty(ns))
                {
                    foreach (MethodDef method in agenType.GetMethods(true))
                    {
                        if (!method.IsPublic && !method.IsNamedEvent && method.ClassName == agenType.Name)
                        {
                            namespaces = GetNamespaces(ns);
                            break;
                        }
                    }
                }

                string indent = WriteNamespacesHead(file, namespaces);

                foreach (PropertyDef prop in agenType.GetProperties(true))
                {
                    if (!prop.IsPublic && !prop.IsInherited && !prop.IsArrayElement)
                    {
                        string propName = prop.Name.Replace("::", "_").Replace("[]", "");
                        string nativeType = DataCppExporter.GetBasicGeneratedNativeType(prop.NativeType);

                        if (Plugin.IsRefType(prop.Type))
                        {
                            if (!nativeType.Contains("*"))
                            {
                                nativeType += "*";
                            }
                        }
                        else if (nativeType != "char*" && !Plugin.IsArrayType(prop.Type))
                        {
                            nativeType = nativeType.Replace("*", "");
                        }

                        file.WriteLine("{0}struct PROPERTY_TYPE_{1} {{ }};", indent, propName);
                        file.WriteLine("{0}template<> inline {1}& {2}::_Get_Property_<PROPERTY_TYPE_{3}>()", indent, nativeType, agenType.BasicName, propName);
                        file.WriteLine("{0}{{", indent);

                        if (prop.IsProperty)
                        {
                            file.WriteLine("{0}\treturn *({1}*)&this->GetVariable<{1}>(\"{2}\");", indent, nativeType, prop.BasicName);
                        }
                        else // field
                        {
                            string typeConvert = "";
                            if (prop.IsReadonly || prop.NativeType == "char*" || prop.NativeType == "const char*")
                            {
                                typeConvert = string.Format("({0}&)", nativeType);
                            }

                            if (prop.IsStatic)
                            {
                                file.WriteLine("{0}\treturn {1}{2};", indent, typeConvert, prop.Name);
                            }
                            else
                            {
                                file.WriteLine("{0}\treturn {1}this->{2};", indent, typeConvert, prop.BasicName);
                            }
                        }

                        file.WriteLine("{0}}}\r\n", indent);
                    }
                }

                foreach (MethodDef method in agenType.GetMethods(true))
                {
                    if (!method.IsPublic && !method.IsNamedEvent && method.ClassName == agenType.Name)
                    {
                        string paramStrDef = string.Empty;
                        string paramStr = string.Empty;

                        for (int i = 0; i < method.Params.Count; ++i)
                        {
                            if (i > 0)
                            {
                                paramStrDef += ", ";
                                paramStr += ", ";
                            }

                            string basicNativeType = DataCppExporter.GetGeneratedNativeType(method.Params[i].NativeType);
                            if (method.Params[i].IsConst)
                            {
                                basicNativeType = "const " + basicNativeType;
                            }
                            paramStrDef += string.Format("{0} p{1}", basicNativeType, i);
                            paramStr += string.Format("p{0}", i);
                        }

                        string methodName = agenType.Name.Replace("::", "_") + "_" + method.BasicName.Replace("::", "_");
                        string nativeReturnType = DataCppExporter.GetGeneratedNativeType(method.NativeReturnType);
                        if (Plugin.IsRefType(method.ReturnType) && !nativeReturnType.Contains("*"))
                        {
                            nativeReturnType += "*";
                        }

                        if (method.NativeReturnType.StartsWith("const "))
                        {
                            nativeReturnType = "const " + nativeReturnType;
                        }

                        file.WriteLine("{0}struct METHOD_TYPE_{1} {{ }};", indent, methodName);
                        file.WriteLine("{0}template<> inline {1} {2}::_Execute_Method_<METHOD_TYPE_{3}>({4})", indent, nativeReturnType, agenType.BasicName, methodName, paramStrDef);
                        file.WriteLine("{0}{{", indent);

                        string ret = (method.NativeReturnType == "void") ? string.Empty : "return ";
                        file.WriteLine("{0}\t{1}this->{2}({3});", indent, ret, method.Name, paramStr);

                        file.WriteLine("{0}}}", indent);
                        file.WriteLine();
                    }
                }

                WriteNamespacesTail(file, namespaces);
                file.WriteLine();
            }

            //foreach (StructType s in TypeManager.Instance.Structs)
            //{
            //    if (s.Properties.Count == 0)
            //    {
            //        continue;
            //    }

            //    List<string> namespaces = GetNamespaces(s.Namespace);

            //    string indent = WriteNamespacesHead(file, namespaces);

            //    foreach (PropertyDef prop in s.Properties)
            //    {
            //        //if (!prop.IsPublic)
            //        {
            //            string propName = prop.Name.Replace("::", "_").Replace("[]", "");
            //            string nativeType = DataCppExporter.GetBasicGeneratedNativeType(prop.NativeType);
            //            file.WriteLine("{0}struct PROPERTY_TYPE_{1} {{ }};", indent, propName);
            //            file.WriteLine("{0}template<> inline {1}& {2}::_Get_Property_<PROPERTY_TYPE_{3}>()", indent, nativeType, s.Name, propName);
            //            file.WriteLine("{0}{{", indent);

            //            if (prop.IsStatic)
            //            {
            //                file.WriteLine("{0}\tunsigned char* pc = (unsigned char*)(&{1});", indent, prop.Name);
            //            }
            //            else
            //            {
            //                file.WriteLine("{0}\tunsigned char* pc = (unsigned char*)this;", indent);
            //                file.WriteLine("{0}\tpc += (int)BEHAVIAC_OFFSETOF({1}, {2});", indent, prop.ClassName, prop.Name);
            //            }
            //            file.WriteLine("{0}\treturn *(reinterpret_cast<{1}*>(pc));", indent, nativeType);

            //            file.WriteLine("{0}}}", indent);
            //            file.WriteLine();
            //        }
            //    }

            //    WriteNamespacesTail(file, namespaces);
            //    file.WriteLine();
            //}
        }

        private BehaviorCreator ExportBody(StringWriter file, BehaviorNode behavior, bool onlyImplement)
        {
            string filename = Path.ChangeExtension(behavior.RelativePath, "").Replace(".", "");
            filename = filename.Replace('\\', '/');

            // write comments
            file.WriteLine("\t// Source file: {0}\r\n", filename);

            string btClassName = string.Format("bt_{0}", filename.Replace('/', '_'));
            string agentType = behavior.AgentType.Name;

            // create the class definition of its attachments
            ExportAttachmentClass(file, btClassName, (Node)behavior);

            // create the class definition of its children
            foreach (Node child in ((Node)behavior).GetChildNodes())
            {
                ExportNodeClass(file, btClassName, agentType, behavior, child);
            }

            // export the create function
            if (onlyImplement)
            {
                file.WriteLine("\t\tbool {0}::Create(BehaviorTree* pBT)", btClassName);
            }
            else
            {
                file.WriteLine("\tclass {0}", btClassName);
                file.WriteLine("\t{");
                file.WriteLine("\tpublic:");
                file.WriteLine("\t\tstatic bool Create(BehaviorTree* pBT)");
            }

            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tpBT->SetClassNameString(\"BehaviorTree\");");
            file.WriteLine("\t\t\tpBT->SetId((uint16_t)-1);");
            file.WriteLine("\t\t\tpBT->SetName(\"{0}\");", filename);
            file.WriteLine("\t\t\tpBT->SetIsFSM({0});", ((Node)behavior).IsFSM ? "true" : "false");
            file.WriteLine("#if !BEHAVIAC_RELEASE");
            file.WriteLine("\t\t\tpBT->SetAgentType(\"{0}\");", agentType);
            file.WriteLine("#endif");

            if (!string.IsNullOrEmpty(((Behavior)behavior).Domains))
            {
                file.WriteLine("\t\t\tpBT->SetDomains(\"{0}\");", ((Behavior)behavior).Domains);
            }

            if (((Behavior)behavior).DescriptorRefs.Count > 0)
            {
                file.WriteLine("\t\t\tpBT->SetDescriptors(\"{0}\");", DesignerPropertyUtility.RetrieveExportValue(((Behavior)behavior).DescriptorRefs));
            }

            ExportPars(file, agentType, "pBT", (Node)behavior, "\t\t");

            // export its attachments
            ExportAttachment(file, btClassName, agentType, "pBT", (Node)behavior, "\t\t\t");

            file.WriteLine("\t\t\t// children");

            // export its children
            if (((Node)behavior).IsFSM)
            {
                file.WriteLine("\t\t\t{");
                file.WriteLine("\t\t\t\tFSM* fsm = BEHAVIAC_NEW FSM();");
                file.WriteLine("\t\t\t\tfsm->SetClassNameString(\"FSM\");");
                file.WriteLine("\t\t\t\tfsm->SetId((uint16_t)-1);");
                file.WriteLine("\t\t\t\tfsm->SetInitialId({0});", behavior.InitialStateId);
                file.WriteLine("#if !BEHAVIAC_RELEASE");
                file.WriteLine("\t\t\t\tfsm->SetAgentType(\"{0}\");", agentType);
                file.WriteLine("#endif");

                foreach (Node child in ((Node)behavior).FSMNodes)
                {
                    ExportNode(file, btClassName, agentType, "fsm", child, 4);
                }

                file.WriteLine("\t\t\t\tpBT->AddChild(fsm);");
                file.WriteLine("\t\t\t}");
            }
            else
            {
                foreach (Node child in ((Node)behavior).GetChildNodes())
                {
                    ExportNode(file, btClassName, agentType, "pBT", child, 3);
                }
            }

            file.WriteLine("\t\t\treturn true;");
            file.WriteLine("\t\t}");

            if (!onlyImplement)
            {
                file.WriteLine("\t};");
            }

            file.WriteLine();

            return new BehaviorCreator(filename, btClassName);
        }

        private void ExportTail(StringWriter file)
        {
            file.WriteLine("\tclass CppGenerationManager : GenerationManager");
            file.WriteLine("\t{");
            file.WriteLine("\tpublic:");
            file.WriteLine("\t\tCppGenerationManager()");
            file.WriteLine("\t\t{");
            file.WriteLine("\t\t\tSetInstance(this);");
            file.WriteLine("\t\t}\n");

            file.WriteLine("\t\tvirtual void RegisterBehaviorsImplement()");
            file.WriteLine("\t\t{");

            for (int i = 0; i < _behaviorCreators.Count; ++i)
            {
                string filename = _behaviorCreators[i].Filename;
                string btClassName = _behaviorCreators[i].Classname;
                file.WriteLine("\t\t\tWorkspace::GetInstance()->RegisterBehaviorTreeCreator(\"{0}\", {1}::Create);", filename, btClassName);
            }

            file.WriteLine("\t\t}");
            file.WriteLine("\t};\n");

            file.WriteLine("\tCppGenerationManager _cppGenerationManager_;");

            // close namespace
            //file.WriteLine("}");
        }

        private void ExportPars(StringWriter file, string agentType, string nodeName, Node node, string indent)
        {
            if (node is Behavior)
            {
                ExportPars(file, agentType, nodeName, ((Behavior)node).LocalVars, indent);
            }
        }

        private void ExportPars(StringWriter file, string agentType, string nodeName, List<Behaviac.Design.ParInfo> pars, string indent)
        {
            if (pars.Count > 0)
            {
                file.WriteLine("{0}\t// locals", indent);

                for (int i = 0; i < pars.Count; ++i)
                {
                    string name = pars[i].BasicName;
                    string type = pars[i].NativeType;
                    string value = pars[i].DefaultValue.Replace("\"", "\\\"");

                    file.WriteLine("{0}\t{1}->AddLocal(\"{2}\", \"{3}\", \"{4}\", \"{5}\");", indent, nodeName, agentType, type, name, value);
                }
            }
        }

        private void ExportAttachmentClass(StringWriter file, string btClassName, Node node)
        {
            foreach (Behaviac.Design.Attachments.Attachment attach in node.Attachments)
            {
                if (!attach.Enable)
                {
                    continue;
                }

                string nodeName = string.Format("attach{0}", attach.Id);

                AttachmentCppExporter attachmentExporter = AttachmentCppExporter.CreateInstance(attach);
                attachmentExporter.GenerateClass(attach, file, "", nodeName, btClassName);
            }
        }

        private void ExportAttachment(StringWriter file, string btClassName, string agentType, string parentName, Node node, string indent)
        {
            if (node.Attachments.Count > 0)
            {
                file.WriteLine("{0}// attachments", indent);

                foreach (Behaviac.Design.Attachments.Attachment attach in node.Attachments)
                {
                    if (!attach.Enable || attach.IsStartCondition)
                    {
                        continue;
                    }

                    file.WriteLine("{0}{{", indent);

                    string nodeName = string.Format("attach{0}", attach.Id);

                    // export its instance and the properties
                    AttachmentCppExporter attachmentExporter = AttachmentCppExporter.CreateInstance(attach);
                    attachmentExporter.GenerateInstance(attach, file, indent, nodeName, agentType, btClassName);

                    string isPrecondition = attach.IsPrecondition && !attach.IsTransition ? "true" : "false";
                    string isEffector = attach.IsEffector && !attach.IsTransition ? "true" : "false";
                    string isTransition = attach.IsTransition ? "true" : "false";
                    file.WriteLine("{0}\t{1}->Attach({2}, {3}, {4}, {5});", indent, parentName, nodeName, isPrecondition, isEffector, isTransition);
                    file.WriteLine("{0}\t{1}->SetHasEvents({1}->HasEvents() | (Event::DynamicCast({2}) != 0));", indent, parentName, nodeName);
                    file.WriteLine("{0}}}", indent);
                }
            }
        }

        private void ExportNodeClass(StringWriter file, string btClassName, string agentType, BehaviorNode behavior, Node node)
        {
            if (!node.Enable)
            {
                return;
            }

            string nodeName = string.Format("node{0}", node.Id);

            NodeCppExporter nodeExporter = NodeCppExporter.CreateInstance(node);
            nodeExporter.GenerateClass(node, file, "", nodeName, agentType, btClassName);

            ExportAttachmentClass(file, btClassName, node);

            if (!(node is ReferencedBehavior))
            {
                foreach (Node child in node.GetChildNodes())
                {
                    ExportNodeClass(file, btClassName, agentType, behavior, child);
                }
            }
        }

        private void ExportNode(StringWriter file, string btClassName, string agentType, string parentName, Node node, int indentDepth)
        {
            if (!node.Enable)
            {
                return;
            }

            // generate the indent string
            string indent = string.Empty;

            for (int i = 0; i < indentDepth; ++i)
            {
                indent += '\t';
            }

            string nodeName = string.Format("node{0}", node.Id);

            // open some brackets for a better formatting in the generated code
            file.WriteLine("{0}{{", indent);

            // export its instance and the properties
            NodeCppExporter nodeExporter = NodeCppExporter.CreateInstance(node);
            nodeExporter.GenerateInstance(node, file, indent, nodeName, agentType, btClassName);

            ExportPars(file, agentType, nodeName, node, indent);

            ExportAttachment(file, btClassName, agentType, nodeName, node, indent + "\t");

            bool isAsChild = true;

            if (node.Parent != null)
            {
                BaseNode.Connector connector = node.Parent.GetConnector(node);

                if (connector != null && !connector.IsAsChild)
                {
                    isAsChild = false;
                }
            }

            if (isAsChild)
            {
                // add the node to its parent
                file.WriteLine("{0}\t{1}->AddChild({2});", indent, parentName, nodeName);
            }
            else
            {
                // add the node as its customized children
                file.WriteLine("{0}\t{1}->SetCustomCondition({2});", indent, parentName, nodeName);
            }

            // export the child nodes
            if (!node.IsFSM && !(node is ReferencedBehavior))
            {
                foreach (Node child in node.GetChildNodes())
                {
                    ExportNode(file, btClassName, agentType, nodeName, child, indentDepth + 1);
                }
            }

            file.WriteLine("{0}\t{1}->SetHasEvents({1}->HasEvents() | {2}->HasEvents());", indent, parentName, nodeName);

            // close the brackets for a better formatting in the generated code
            file.WriteLine("{0}}}", indent);
        }

        private bool IsStructType(MethodDef.Param param)
        {
            string paramType = DataCppExporter.GetGeneratedNativeType(param.NativeType);
            bool isStruct = Plugin.IsCustomClassType(param.Type);

            if (isStruct)
            {
                string tmpParamType = paramType.Replace("&", "");
                tmpParamType = tmpParamType.Replace("*", "");

                StructType structType = TypeManager.Instance.FindStruct(tmpParamType);
                if (structType == null || structType.IsRef || structType.Properties.Count == 0)
                {
                    isStruct = false;
                }
            }

            return isStruct;
        }

        private void PreExportMeta(StringWriter file)
        {
            // all structs
            Dictionary<string, bool> allStructs = new Dictionary<string, bool>();

            foreach (StructType structType in TypeManager.Instance.Structs)
            {
                if (structType.IsRef || structType.Properties.Count == 0)
                {
                    continue;
                }

                string structTypeName = structType.Fullname.Replace("::", "_");

                // class
                file.WriteLine("\tclass CInstanceConst_{0} : public CInstanceConstBase<{1}>", structTypeName, structType.Fullname);
                file.WriteLine("\t{");

                foreach (PropertyDef prop in structType.Properties)
                {
                    file.WriteLine("\t\tIInstanceMember* _{0};", prop.BasicName);
                }

                file.WriteLine();

                // Constructors
                file.WriteLine("\tpublic: ");
                file.WriteLine("\t\tCInstanceConst_{0}(const char* valueStr) : CInstanceConstBase<{1}>(valueStr)", structTypeName, structType.Fullname);
                file.WriteLine("\t\t{");

                file.WriteLine("\t\t\tbehaviac::vector<behaviac::string> paramStrs = behaviac::StringUtils::SplitTokensForStruct(valueStr);");
                file.WriteLine("\t\t\tBEHAVIAC_ASSERT(paramStrs.size() == {0});", structType.Properties.Count);
                file.WriteLine();

                for (int i = 0; i < structType.Properties.Count; ++i)
                {
                    PropertyDef prop = structType.Properties[i];
                    string propType = DataCppExporter.GetGeneratedNativeType(prop.NativeType);

                    if (propType.EndsWith("&") || Plugin.IsRefType(prop.Type) && propType.EndsWith("*"))
                    {
                        propType = propType.Substring(0, propType.Length - 1);
                    }

                    file.WriteLine("\t\t\t_{0} = AgentMeta::TParseProperty<{1} >(paramStrs[{2}].c_str());", prop.BasicName, propType, i);
                }

                file.WriteLine("\t\t}");
                file.WriteLine();

                // Destructor
                file.WriteLine("\t\t~CInstanceConst_{0}()", structTypeName);
                file.WriteLine("\t\t{");

                foreach (PropertyDef prop in structType.Properties)
                {
                    file.WriteLine("\t\t\tBEHAVIAC_DELETE _{0};", prop.BasicName);
                }

                file.WriteLine("\t\t}");
                file.WriteLine();

                // Run()
                file.WriteLine("\t\tvirtual void run(Agent* self)");
                file.WriteLine("\t\t{");

                if (structType.Properties.Count > 0)
                {
                    foreach (PropertyDef prop in structType.Properties)
                    {
                        file.WriteLine("\t\t\tBEHAVIAC_ASSERT(_{0} != NULL);", prop.BasicName);
                    }

                    file.WriteLine();
                }

                foreach (PropertyDef prop in structType.Properties)
                {
                    string propType = DataCppExporter.GetGeneratedNativeType(prop.NativeType);
                    string tempPropType = propType;
                    string pointStr = "*";

                    if (propType.EndsWith("&"))
                    {
                        tempPropType = propType.Substring(0, propType.Length - 1);
                    }
                    else if (propType.EndsWith("*"))
                    {
                        tempPropType = propType.Substring(0, propType.Length - 1);
                        pointStr = "";
                    }

                    if (tempPropType == "char*")
                    {
                        file.WriteLine("\t\t\t_value.{0} = ({1})_{0}->GetValue(self, behaviac::Meta::IsVector<{1} >::Result, behaviac::GetClassTypeNumberId<{1} >());", prop.BasicName, tempPropType);
                    }
                    else
                    {
                        file.WriteLine("\t\t\t_value.{0} = {1}({2}*)_{0}->GetValue(self, behaviac::Meta::IsVector<{2} >::Result, behaviac::GetClassTypeNumberId<{2} >());", prop.BasicName, pointStr, tempPropType);
                    }
                }

                file.WriteLine("\t\t}"); // Run()

                file.WriteLine("\t};"); // end of class
                file.WriteLine();
            }

            // all methods
            Dictionary<string, bool> allMethods = new Dictionary<string, bool>();

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                string agentTypeName = agent.Name;

                IList<MethodDef> methods = agent.GetMethods(true);

                foreach (MethodDef method in methods)
                {
                    if (!method.IsNamedEvent)
                    {
                        bool hasRefParam = false;

                        foreach (MethodDef.Param param in method.Params)
                        {
                            if (param.IsRef || param.IsOut || Plugin.IsRefType(param.Type) || IsStructType(param))
                            {
                                hasRefParam = true;
                                break;
                            }
                        }

                        if (hasRefParam)
                        {
                            string methodFullname = method.Name.Replace("::", "_");

                            if (allMethods.ContainsKey(methodFullname))
                            {
                                continue;
                            }
                            else
                            {
                                allMethods[methodFullname] = true;
                            }

                            string methodReturnType = DataCppExporter.GetGeneratedNativeType(method.NativeReturnType);
                            if (Plugin.IsRefType(method.ReturnType) && !methodReturnType.Contains("*"))
                            {
                                methodReturnType += "*";
                            }

                            string baseClass = (methodReturnType == "void") ? "CAgentMethodVoidBase" : string.Format("CAgentMethodBase<{0}>", methodReturnType);

                            // class
                            file.WriteLine("\tclass CMethod_{0} : public {1}", methodFullname, baseClass);
                            file.WriteLine("\t{");
                            string initVarsList = "";
                            bool isFirstTime = true;

                            foreach (MethodDef.Param param in method.Params)
                            {
                                if (Plugin.IsRefType(param.Type))
                                {
                                    file.WriteLine("\t\tIInstanceMember* _{0};", param.Name);

                                    if (isFirstTime)
                                    {
                                        isFirstTime = false;
                                        initVarsList += string.Format("_{0}(0) ", param.Name);
                                    }
                                    else
                                    {
                                        initVarsList += string.Format(", _{0}(0) ", param.Name);
                                    }
                                }
                                else
                                {
                                    string paramType = DataCppExporter.GetGeneratedNativeType(param.NativeType);
                                    file.WriteLine("\t\tIInstanceMember* _{0};", param.Name);

                                    if (isFirstTime)
                                    {
                                        isFirstTime = false;
                                        initVarsList += string.Format("_{0}(0) ", param.Name);
                                    }
                                    else
                                    {
                                        initVarsList += string.Format(", _{0}(0) ", param.Name);
                                    }
                                }
                            }

                            if (method.Params.Count > 0)
                            {
                                file.WriteLine();
                            }

                            // Constructors
                            file.WriteLine("\tpublic: ");
                            file.WriteLine("\t\tCMethod_{0}() : {1}", methodFullname, initVarsList);
                            file.WriteLine("\t\t{");
                            file.WriteLine("\t\t}");
                            file.WriteLine();

                            file.WriteLine("\t\tCMethod_{0}(CMethod_{0} &rhs) : {1}(rhs) , {2}", methodFullname, baseClass, initVarsList);
                            file.WriteLine("\t\t{");
                            file.WriteLine("\t\t}");
                            file.WriteLine();

                            // Destructor
                            file.WriteLine("\t\t~CMethod_{0}()", methodFullname);
                            file.WriteLine("\t\t{");

                            foreach (MethodDef.Param param in method.Params)
                            {
                                if (Plugin.IsRefType(param.Type))
                                {
                                    file.WriteLine("\t\t\tBEHAVIAC_DELETE _{0};", param.Name);
                                }
                                else
                                {
                                    string paramType = DataCppExporter.GetGeneratedNativeType(param.NativeType);
                                    file.WriteLine("\t\t\tBEHAVIAC_DELETE _{0};", param.Name);
                                }
                            }

                            file.WriteLine("\t\t}");
                            file.WriteLine();

                            // Clone()
                            file.WriteLine("\t\tvirtual IInstanceMember* clone()");
                            file.WriteLine("\t\t{");
                            file.WriteLine("\t\t\treturn BEHAVIAC_NEW CMethod_{0}(*this);", methodFullname);
                            file.WriteLine("\t\t}"); // Clone()
                            file.WriteLine();

                            // Load()
                            file.WriteLine("\t\tvirtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs)");
                            file.WriteLine("\t\t{");

                            file.WriteLine("\t\t\tBEHAVIAC_ASSERT(paramStrs.size() == {0});", method.Params.Count);
                            file.WriteLine();
                            file.WriteLine("\t\t\tbehaviac::StringUtils::StringCopySafe(kInstanceNameMax, _instance, instance);");

                            for (int i = 0; i < method.Params.Count; ++i)
                            {
                                MethodDef.Param param = method.Params[i];
                                string paramType = DataCppExporter.GetGeneratedNativeType(param.NativeType);

                                string tmpParamType = paramType.Replace("&", "");
                                tmpParamType = tmpParamType.Replace("*", "");
                                tmpParamType = tmpParamType.Replace("::", "_");

                                if (paramType.EndsWith("&") || Plugin.IsRefType(param.Type) && paramType.EndsWith("*"))
                                {
                                    paramType = paramType.Substring(0, paramType.Length - 1);
                                }

                                if (IsStructType(param))
                                {
                                    file.WriteLine("\t\t\tif (behaviac::StringUtils::StartsWith(paramStrs[{0}].c_str(), \"{{\"))", i);
                                    file.WriteLine("\t\t\t{");
                                    file.WriteLine("\t\t\t\t_{0} = BEHAVIAC_NEW CInstanceConst_{1}(paramStrs[{2}].c_str());", param.Name, tmpParamType, i);
                                    file.WriteLine("\t\t\t}");
                                    file.WriteLine("\t\t\telse");
                                    file.WriteLine("\t\t\t{");
                                    file.WriteLine("\t\t\t\t_{0} = AgentMeta::TParseProperty<{1} >(paramStrs[{2}].c_str());", param.Name, paramType, i);
                                    file.WriteLine("\t\t\t}");
                                }
                                else
                                {
                                    file.WriteLine("\t\t\t_{0} = AgentMeta::TParseProperty<{1} >(paramStrs[{2}].c_str());", param.Name, paramType, i);
                                }
                            }

                            file.WriteLine("\t\t}"); // Load()
                            file.WriteLine();

                            // Run()
                            file.WriteLine("\t\tvirtual void run(Agent* self)");
                            file.WriteLine("\t\t{");

                            if (method.Params.Count > 0)
                            {
                                foreach (MethodDef.Param param in method.Params)
                                {
                                    file.WriteLine("\t\t\tBEHAVIAC_ASSERT(_{0} != NULL);", param.Name);
                                }

                                file.WriteLine();
                            }

                            string paramValues = "";
                            string executeMethodParamValues = "";
                            string allParamTypes = "";

                            foreach (MethodDef.Param param in method.Params)
                            {
                                if (!string.IsNullOrEmpty(paramValues))
                                {
                                    paramValues += ", ";
                                }

                                if (!string.IsNullOrEmpty(executeMethodParamValues))
                                {
                                    executeMethodParamValues += ", ";
                                }

                                string paramType = DataCppExporter.GetGeneratedNativeType(param.NativeType);
                                if (param.IsConst)
                                {
                                    paramType = "const " + paramType;
                                }
                                string tempParamType = paramType;
                                string tempexecuteMethodParamType = tempParamType;

                                if (paramType.EndsWith("&"))
                                {
                                    tempParamType = paramType.Substring(0, paramType.Length - 1);
                                    tempexecuteMethodParamType = tempParamType;
                                }
                                else if (paramType.EndsWith("*"))
                                {
                                    tempParamType = paramType.Substring(0, paramType.Length - 1);
                                }

                                string formatStr = "\t\t\t{0}& pValue_{1} = *({0}*)_{1}->GetValue(self, behaviac::Meta::IsVector<{0} >::Result, behaviac::GetClassTypeNumberId<{0} >());";

                                if (tempParamType == "char*")
                                {
                                    formatStr = "\t\t\t{0} pValue_{1} = ({0})_{1}->GetValue(self, behaviac::Meta::IsVector<{0} >::Result, behaviac::GetClassTypeNumberId<{0} >());";
                                }

                                if (IsStructType(param))
                                {
                                    file.WriteLine("\t\t\t_{0}->run(self);", param.Name);
                                }

                                if (method.IsPublic)
                                {
                                    file.WriteLine(formatStr, tempParamType, param.Name);
                                }
                                else
                                {
                                    file.WriteLine(formatStr, tempexecuteMethodParamType, param.Name);
                                }

                                string paramName = string.Format("pValue_{0}", param.Name);
                                string executeMethodParamName = paramName;

                                allParamTypes += ", " + paramType;
                                paramValues += paramName;
                                executeMethodParamValues += executeMethodParamName;
                            }

                            if (!method.IsStatic)
                            {
                                file.WriteLine("\t\t\tself = Agent::GetParentAgent(self, _instance);");
                                file.WriteLine();
                            }

                            if (methodReturnType == "void")
                            {
                                if (method.IsPublic)
                                {
                                    if (method.IsStatic)
                                    {
                                        file.WriteLine("\t\t\t{0}::{1}({2});", agentTypeName, method.BasicName, paramValues);
                                    }
                                    else
                                    {
                                        file.WriteLine("\t\t\t(({0}*)self)->{1}({2});", agentTypeName, method.BasicName, paramValues);
                                    }
                                }
                                else
                                {
                                    string methodType = "";

                                    if (method.NativeReturnType.Contains("const "))
                                    {
                                        methodType = "const " + methodReturnType;
                                    }
                                    else
                                    {
                                        methodType = methodReturnType;
                                    }

                                    string retStr = string.Format("\t\t\t(({0}*)self)->_Execute_Method_<{1}METHOD_TYPE_{2}, {3}{4} >({5});", method.ClassName, getNamespace(method.ClassName), method.Name.Replace("::", "_"), methodType, allParamTypes, executeMethodParamValues);
                                    file.WriteLine(retStr);
                                }
                            }
                            else
                            {
                                if (method.IsPublic)
                                {
                                    if (method.IsStatic)
                                    {
                                        file.WriteLine("\t\t\t_returnValue->value = {0}::{1}({2});", agentTypeName, method.BasicName, paramValues);
                                    }
                                    else
                                    {
                                        file.WriteLine("\t\t\t_returnValue->value = (({0}*)self)->{1}({2});", agentTypeName, method.BasicName, paramValues);
                                    }
                                }
                                else
                                {
                                    string methodName = agentTypeName.Replace("::", "_") + "_" + method.BasicName.Replace("::", "_");
                                    string methodType = "";

                                    if (method.NativeReturnType.Contains("const "))
                                    {
                                        methodType = "const " + methodReturnType;
                                    }
                                    else
                                    {
                                        methodType = methodReturnType;
                                    }

                                    string retStr = string.Format("\t\t\t_returnValue->value = (({0}*)self)->_Execute_Method_<{1}METHOD_TYPE_{2}, {3}{4} >({5});", method.ClassName, getNamespace(method.ClassName), method.Name.Replace("::", "_"), methodType, allParamTypes, executeMethodParamValues);
                                    file.WriteLine(retStr);
                                }
                            }

                            file.WriteLine("\t\t}"); // Run()

                            file.WriteLine("\t};"); // end of class
                            file.WriteLine();
                        }
                    }
                }
            }
        }

        private void ExportDelegateMethod(StringWriter file)
        {
            file.WriteLine("\t// ---------------------------------------------------------------------");
            file.WriteLine("\t// Delegate methods");
            file.WriteLine("\t// ---------------------------------------------------------------------");
            file.WriteLine();

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                bool hasProperties = false;

                IList<PropertyDef> properties = agent.GetProperties(true);

                foreach (PropertyDef prop in properties)
                {
                    bool isMemberProp = prop.IsMember || agent.IsCustomized;

                    if (!prop.IsInherited && isMemberProp && (!prop.IsAddedAutomatically || prop.IsArrayElement))
                    {
                        string bindingProperty = "";
                        string propName = prop.Name.Replace("::", "_").Replace("[]", "");
                        string propFullType = DataCppExporter.GetGeneratedNativeType(prop.NativeType);
                        string propType = DataCppExporter.GetGeneratedNativeType(prop.NativeItemType);
                        string propBasicName = prop.BasicName.Replace("[]", "");
                        string propItemName = prop.BasicName;

                        if (Plugin.IsRefType(prop.Type))
                        {
                            if (!prop.IsArrayElement && !propFullType.Contains("*"))
                            {
                                propFullType += "*";
                            }

                            if (!propType.Contains("*"))
                            {
                                propType += "*";
                            }
                        }
                        else if (propType != "char*" && !Plugin.IsArrayType(prop.Type))
                        {
                            if (!prop.IsArrayElement)
                            {
                                propFullType = propFullType.Replace("*", "");
                            }

                            propType = propType.Replace("*", "");
                        }

                        if (prop.IsArrayElement)
                        {
                            propItemName = propItemName.Replace("[]", "[index]");
                        }

                        string agentTypeName = agent.Name;

                        if (agent.IsStatic || isMemberProp && prop.IsStatic)
                        {
                            if (isMemberProp)
                            {
                                string setValue = "";
                                string getValue = "";

                                if (prop.IsPublic)
                                {
                                    setValue = string.Format("{0}::{1} = value;", agentTypeName, propItemName);
                                    getValue = string.Format("{0}::{1}", agentTypeName, propItemName);
                                }
                                else
                                {
                                    string propValue = string.Format("(({0}*)0)->_Get_Property_<{1}PROPERTY_TYPE_{2}, {3} >()", agentTypeName, getNamespace(agentTypeName).Replace("::", "_"), propName, propFullType);

                                    if (prop.IsArrayElement)
                                    {
                                        setValue = string.Format("{0}[index] = value;", propValue);
                                        getValue = string.Format("{0}[index]", propValue);
                                    }
                                    else
                                    {
                                        setValue = string.Format("{0} = value;", propValue);
                                        getValue = propValue;
                                    }
                                }

                                if (prop.IsReadonly)
                                {
                                    setValue = "";
                                }

                                string str_setter = "";
                                string str_getter = "";

                                if (prop.IsArrayElement)
                                {
                                    str_setter = string.Format("\n\tinline void Set_{0}({1} value, int index) {{ {2} }}", propName, propType, setValue);

                                    if (String.Compare(prop.NativeItemType, "bool", true) == 0)
                                    {
                                        str_getter = string.Format("\n\tinline const void* Get_{0}(int index)\n\t{{\n#if _MSC_VER\n\t\treturn {1}._Getptr();\n#else\n\t\tstatic ThreadBool buffer;\n\t\tbool b = {1};\n\t\tbuffer.set(b);\n\t\treturn buffer.value();\n#endif\n\t}}", propName, getValue);
                                    }
                                    else
                                    {
                                        str_getter = string.Format("\n\tinline const void* Get_{0}(int index) {{ return &{1}; }}", propName, getValue);
                                    }
                                }
                                else
                                {
                                    str_setter = string.Format("\n\tinline void Set_{0}({1} value) {{ {2} }}", propName, propType, setValue);
                                    str_getter = string.Format("\n\tinline const void* Get_{0}() {{ return &{1}; }}", propName, getValue);
                                }

                                bindingProperty = str_setter + str_getter;
                            }
                        }
                        else
                        {
                            if (isMemberProp && !prop.IsStatic)
                            {
                                string setValue = "";
                                string getValue = "";

                                if (prop.IsPublic)
                                {
                                    setValue = string.Format("(({0}*)self)->{1} = value;", agentTypeName, propItemName);
                                    getValue = string.Format("(({0}*)self)->{1}", agentTypeName, propItemName);
                                }
                                else
                                {
                                    string propValue = string.Format("(({0}*)self)->_Get_Property_<{1}PROPERTY_TYPE_{2}, {3} >()", agentTypeName, getNamespace(agentTypeName), propName, propFullType);

                                    if (prop.IsArrayElement)
                                    {
                                        setValue = string.Format("{0}[index] = value;", propValue);
                                        getValue = string.Format("{0}[index]", propValue);
                                    }
                                    else
                                    {
                                        setValue = string.Format("{0} = value;", propValue);
                                        getValue = propValue;
                                    }
                                }

                                if (prop.IsReadonly)
                                {
                                    setValue = "";
                                }

                                string str_setter = "";
                                string str_getter = "";

                                if (prop.IsArrayElement)
                                {
                                    if (prop.IsPar)
                                    {
                                        //str_setter = string.Format("\n\tinline void Set_{0}(Agent* self, {1} value, int index) {{ self->SetVariable(\"{2}\",{3}u,value); }};", propName, propType, propBasicName, CRC32.CalcCRC(propBasicName));
                                        //str_getter = string.Format("\n\tinline const void* Get_{1}(Agent* self, int index ){{ return &self->GetVariable<{0}>({2}u); }};", propType, propName, CRC32.CalcCRC(propBasicName));
                                    }
                                    else
                                    {
                                        str_setter = string.Format("\n\tinline void Set_{0}(Agent* self, {1} value, int index) {{ {2} }};", propName, propType, setValue);

                                        if (String.Compare(prop.NativeItemType, "bool", true) == 0)
                                        {
                                            str_getter = string.Format("\n\tinline const void* Get_{0}(Agent* self, int index)\n\t{{\n#if _MSC_VER\n\t\treturn {1}._Getptr();\n#else\n\t\tstatic ThreadBool buffer;\n\t\tbool b = {1};\n\t\tbuffer.set(b);\n\t\treturn buffer.value();\n#endif\n\t}}", propName, getValue);
                                        }
                                        else
                                        {
                                            str_getter = string.Format("\n\tinline const void* Get_{0}(Agent* self, int index) {{ return &{1}; }};", propName, getValue);
                                        }
                                    }
                                }
                                else
                                {
                                    if (prop.IsPar)
                                    {
                                        //str_setter = string.Format("\n\tinline void Set_{0}(Agent* self, {1} value) {{ self->SetVariable(\"{2}\",{3}u,value); }};", propName, propType, prop.BasicName, CRC32.CalcCRC(propBasicName));
                                        //str_getter = string.Format("\n\tinline const void* Get_{1}(Agent* self){{ return &self->GetVariable<{0}>({2}u); }};", propType, propName, CRC32.CalcCRC(propBasicName));
                                    }
                                    else
                                    {
                                        str_setter = string.Format("\n\tinline void Set_{0}(Agent* self, {1} value) {{ {2} }};", propName, propType, setValue);
                                        str_getter = string.Format("\n\tinline const void* Get_{0}(Agent* self) {{ return &{1}; }};", propName, getValue);
                                    }
                                }

                                bindingProperty = str_setter + str_getter;
                            }
                        }

                        if (!string.IsNullOrEmpty(bindingProperty))
                        {
                            file.WriteLine(bindingProperty);

                            hasProperties = true;
                        }
                    }
                }

                if (hasProperties)
                {
                    file.WriteLine();
                }

                IList<MethodDef> methods = agent.GetMethods(true);

                foreach (MethodDef method in methods)
                {
                    bool hasRefParam = false;

                    foreach (MethodDef.Param param in method.Params)
                    {
                        if (param.IsRef || param.IsOut || Plugin.IsRefType(param.Type) || IsStructType(param))
                        {
                            hasRefParam = true;
                            break;
                        }
                    }

                    string agentMethod = "";
                    string paramTypes = "";
                    string paramTypeValues = "";
                    string funcParamTypeValues = "";
                    string paramValues = "";
                    string allParamTypes = "";

                    if (method.IsNamedEvent || !hasRefParam)
                    {
                        foreach (MethodDef.Param param in method.Params)
                        {
                            if (!string.IsNullOrEmpty(paramTypes))
                            {
                                paramTypes += ", ";
                            }

                            if (!string.IsNullOrEmpty(paramValues))
                            {
                                paramValues += ", ";
                            }

                            if (!string.IsNullOrEmpty(funcParamTypeValues))
                            {
                                funcParamTypeValues += ", ";
                            }

                            string paramType = DataCppExporter.GetGeneratedNativeType(param.NativeType);
                            paramTypes += paramType;
                            paramTypeValues += ", " + paramType + " " + param.Name;
                            funcParamTypeValues += DataCppExporter.GetGeneratedNativeType(param.Type, param.NativeType) + " " + param.Name;
                            paramValues += param.Name;
                            allParamTypes += ", " + paramType;
                        }
                    }

                    string methodReturnType = DataCppExporter.GetGeneratedNativeType(method.NativeReturnType);
                    if (Plugin.IsRefType(method.ReturnType) && !methodReturnType.Contains("*"))
                    {
                        methodReturnType += "*";
                    }

                    string agentTypeName = agent.Name;
                    string methodName = agentTypeName.Replace("::", "_") + "_" + method.BasicName.Replace("::", "_");

                    if (method.IsNamedEvent)
                    {
                        if (!string.IsNullOrEmpty(paramTypes))
                        {
                            paramTypes = string.Format("<{0}>", paramTypes);
                        }

                        if (!string.IsNullOrEmpty(funcParamTypeValues))
                        {
                            funcParamTypeValues = ", " + funcParamTypeValues;
                        }

                        string str_setter = string.Format("\tinline void FunctionPointer_{0}(Agent* self{1}) {{ }} /* {2} */",
                                                          methodName, funcParamTypeValues, method.BasicName);

                        file.WriteLine(str_setter);
                    }
                    else
                    {
                        if (hasRefParam)
                        {
                            string methodFullname = method.Name.Replace("::", "_");
                            agentMethod = "";
                        }
                        else
                        {
                            if (method.IsStatic)
                            {
                                if (paramTypeValues.StartsWith(", "))
                                {
                                    paramTypeValues = paramTypeValues.Substring(2);
                                }

                                if (methodReturnType == "void")
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = string.Format("<{0}>", paramTypes);
                                    }

                                    agentMethod = string.Format("\tinline void FunctionPointer_{0}({1}) {{ {2}::{3}({4}); }}",
                                                                methodName, funcParamTypeValues, agentTypeName, method.BasicName, paramValues);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = ", " + paramTypes;
                                    }

                                    agentMethod = string.Format("\tinline {0} FunctionPointer_{1}({2}) {{ return {3}::{4}({5}); }}",
                                                                methodReturnType, methodName, funcParamTypeValues, agentTypeName, method.BasicName, paramValues);
                                }
                            }
                            else
                            {
                                string methodStr = "";

                                if (method.IsPublic)
                                {
                                    methodStr = string.Format("(({0}*)self)->{1}({2})", agentTypeName, method.BasicName, paramValues);
                                }
                                else
                                {
                                    string methodType = "";

                                    if (method.NativeReturnType.Contains("const "))
                                    {
                                        methodType = "const " + methodReturnType;
                                    }
                                    else
                                    {
                                        methodType = methodReturnType;
                                    }

                                    methodStr = string.Format("(({0}*){1})->_Execute_Method_<{2}METHOD_TYPE_{3}, {4}{5} >({6})", method.ClassName, "self", getNamespace(method.ClassName), method.Name.Replace("::", "_"), methodType, allParamTypes, paramValues);
                                }

                                if (!string.IsNullOrEmpty(funcParamTypeValues))
                                {
                                    funcParamTypeValues = ", " + funcParamTypeValues;
                                }

                                if (methodReturnType == "void")
                                {
                                    agentMethod = string.Format("\tinline void FunctionPointer_{0}(Agent* self{1}) {{ {2}; }}",
                                                                methodName, funcParamTypeValues, methodStr);
                                }
                                else
                                {
                                    if (!method.IsPublic)
                                    {
                                        methodStr = string.Format("({0}){1}", methodReturnType, methodStr);
                                    }

                                    agentMethod = string.Format("\tinline {0} FunctionPointer_{1}(Agent* self{2}) {{ return {3}; }}",
                                                                methodReturnType, methodName, funcParamTypeValues, methodStr);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(agentMethod))
                        {
                            file.WriteLine(agentMethod);
                        }
                    }
                }
            }

            foreach (StructType s in TypeManager.Instance.Structs)
            {
                foreach (PropertyDef prop in s.Properties)
                {
                    if (!prop.IsInherited)
                    {
                        string propName = prop.Name.Replace("::", "_").Replace("[]", "");
                        string propFullname = propName;

                        if (!string.IsNullOrEmpty(s.Namespace))
                        {
                            propFullname = s.Namespace + "::" + propFullname;
                        }

                        propFullname = propFullname.Replace("::", "_");
                        string propType = DataCppExporter.GetGeneratedNativeType(prop.NativeItemType);
                        string propFullType = DataCppExporter.GetGeneratedNativeType(prop.NativeType);
                        string structName = s.Fullname;
                        string setValue = "";
                        string getValue = "";

                        if (prop.IsPublic)
                        {
                            string propBasicName = prop.BasicName.Replace("::", "_").Replace("[]", "");

                            setValue = prop.IsReadonly ? "" : string.Format("(({0}*)self)->{1} = value;", structName, propBasicName);
                            getValue = string.Format("(({0}*)self)->{1}", structName, propBasicName);
                        }

                        //else
                        //{
                        //    string ns = string.IsNullOrEmpty(s.Namespace) ? "" : (s.Namespace + "::");
                        //    string propValue = string.Format("(({0}*)self)->_Get_Property_<{1}PROPERTY_TYPE_{2}, {3} >()", structName, ns, propName, propFullType);

                        //    setValue = string.Format("{0} = value;", propValue);
                        //    getValue = propValue;
                        //}

                        file.WriteLine("\tinline void Set_{0}(Agent* self, {1} value) {{ {2} }};", propFullname, propType, setValue);
                        file.WriteLine("\tinline const void* Get_{0}(Agent* self) {{ return &{1}; }};", propFullname, getValue);
                        file.WriteLine();
                    }
                }
            }
        }

        private void ExportAgentsDefinition(string defaultAgentFolder)
        {
            foreach (AgentType agent in Plugin.AgentTypes)
            {
                if (agent.IsImplemented || agent.Name == "behaviac::Agent")
                {
                    continue;
                }

                string agentFolder = string.IsNullOrEmpty(agent.ExportLocation) ? defaultAgentFolder : Workspace.Current.MakeAbsolutePath(agent.ExportLocation);
                string filename = Path.Combine(agentFolder, agent.BasicName + ".h");
                string oldFilename = "";

                if (!string.IsNullOrEmpty(agent.OldName) && agent.OldName != agent.Name)
                {
                    oldFilename = Path.Combine(agentFolder, agent.BasicOldName + ".h");
                }

                Debug.Check(filename != oldFilename);

                try
                {
                    if (!Directory.Exists(agentFolder))
                    {
                        Directory.CreateDirectory(agentFolder);
                    }

                    if (!File.Exists(filename))
                    {
                        ExportAgentHeaderFile(agent, filename, false);

                        if (File.Exists(oldFilename))
                        {
                            MergeFiles(oldFilename, filename, filename);
                        }
                    }
                    else
                    {
                        string behaviacAgentDir = GetBehaviacTypesDir();
                        string newFilename = Path.GetFileName(filename);
                        newFilename = Path.ChangeExtension(newFilename, ".new.h");
                        newFilename = Path.Combine(behaviacAgentDir, newFilename);

                        ExportAgentHeaderFile(agent, newFilename, false);
                        Debug.Check(File.Exists(newFilename));

                        MergeFiles(filename, newFilename, filename);
                    }

                    if (File.Exists(oldFilename))
                    {
                        File.Delete(oldFilename);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Merge Headers Error : {0} {1}", filename, e.Message);
                }
            }
        }

        private void ExportAgentHeaderFile(AgentType agent, string filename, bool preview)
        {
            using (StringWriter file = new StringWriter())
            {
                ExportFileWarningHeader(file);

                string headerFileMacro = string.Format("_BEHAVIAC_{0}_H_", agent.Name.Replace("::", "_").ToUpperInvariant());

                file.WriteLine("#ifndef {0}", headerFileMacro);
                file.WriteLine("#define {0}", headerFileMacro);
                file.WriteLine();

                file.WriteLine("#include \"behaviac_headers.h\"");
                file.WriteLine();

                if (agent.Base != null && !agent.Base.IsImplemented)
                {
                    string baseHeader = agent.Base.BasicName;
                    if (!string.IsNullOrEmpty(agent.Base.ExportLocation))
                    {
                        baseHeader = Path.Combine(agent.Base.ExportLocation, baseHeader);
                    }

                    file.WriteLine("#include \"{0}.h\"", baseHeader);
                    file.WriteLine();
                }

                if (!preview)
                {
                    ExportBeginComment(file, "", Behaviac.Design.Exporters.Exporter.file_init_part);
                    file.WriteLine();
                    ExportEndComment(file, "");
                    file.WriteLine();
                }

                string indent = "";
                List<string> namespaces = new List<string>();

                if (!string.IsNullOrEmpty(agent.Namespace))
                {
                    namespaces = GetNamespaces(agent.Namespace);
                    indent = WriteNamespacesHead(file, namespaces);

                    if (!preview)
                    {
                        ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.namespace_init_part);
                        file.WriteLine();
                        ExportEndComment(file, indent);
                        file.WriteLine();
                    }
                }

                string baseClassStr = (agent.Base != null) ? string.Format(" : public {0}", agent.Base.Name) : "";
                file.WriteLine("{0}class {1}{2}", indent, agent.BasicName, baseClassStr);

                if (!preview)
                {
                    ExportBeginComment(file, indent, agent.BasicName);
                    ExportEndComment(file, indent);
                }

                file.WriteLine("{0}{{", indent);
                file.WriteLine("{0}public:", indent);
                file.WriteLine("{0}\t{1}();", indent, agent.BasicName);
                file.WriteLine("{0}\tvirtual ~{1}();", indent, agent.BasicName);
                file.WriteLine();

                if (agent.Base != null)
                {
                    file.WriteLine("{0}\tBEHAVIAC_DECLARE_AGENTTYPE({1}, {2})", indent, agent.Name, agent.Base.Name);
                    file.WriteLine();
                }

                IList<PropertyDef> properties = agent.GetProperties(true);

                foreach (PropertyDef prop in properties)
                {
                    if ((preview || !agent.IsImplemented) && !prop.IsInherited && !prop.IsPar && !prop.IsArrayElement)
                    {
                        string publicStr = prop.IsPublic ? "public:" : "private:";
                        string staticStr = prop.IsStatic ? "static " : "";
                        string propType = DataCppExporter.GetGeneratedNativeType(prop.Type);

                        file.WriteLine("{0}\t{1} {2}{3} {4};", indent, publicStr, staticStr, propType, prop.BasicName);
                        file.WriteLine();
                    }
                }

                IList<MethodDef> methods = agent.GetMethods(true);

                foreach (MethodDef method in methods)
                {
                    if ((preview || !agent.IsImplemented) && !method.IsInherited && !method.IsNamedEvent)
                    {
                        string publicStr = method.IsPublic ? "public:" : "private:";
                        string staticStr = method.IsStatic ? "static " : "";

                        string allParams = "";

                        foreach (MethodDef.Param param in method.Params)
                        {
                            if (!string.IsNullOrEmpty(allParams))
                            {
                                allParams += ", ";
                            }

                            string constStr = param.IsConst ? "const " : "";

                            allParams += constStr + DataCppExporter.GetGeneratedNativeType(param.NativeType) + " " + param.Name;
                        }

                        file.WriteLine("{0}\t{1} {2}{3} {4}({5});", indent, publicStr, staticStr, DataCppExporter.GetGeneratedNativeType(method.ReturnType), method.BasicName, allParams);
                        file.WriteLine();
                    }
                }

                if (!preview)
                {
                    ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.class_part);
                    file.WriteLine();
                    ExportEndComment(file, indent);
                }

                //end of class
                file.WriteLine("{0}}};", indent);

                if (!string.IsNullOrEmpty(agent.Namespace))
                {
                    if (!preview)
                    {
                        file.WriteLine();
                        ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.namespace_uninit_part);
                        file.WriteLine();
                        ExportEndComment(file, indent);
                    }

                    //end of namespace
                    WriteNamespacesTail(file, namespaces);
                }

                file.WriteLine();

                if (!preview)
                {
                    ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.file_uninit_part);
                    file.WriteLine();
                    ExportEndComment(file, indent);
                }

                file.WriteLine();
                //file.WriteLine("BEHAVIAC_DECLARE_TYPE_VECTOR_HANDLER({0}*);", agent.Name);
                //file.WriteLine();

                file.WriteLine("#endif");

                UpdateFile(file, filename);
            }
        }

        private void ExportAgentsImplemention(string defaultAgentFolder)
        {
            foreach (AgentType agent in Plugin.AgentTypes)
            {
                if (agent.IsImplemented || agent.Base == null)
                {
                    continue;
                }

                string agentFolder = string.IsNullOrEmpty(agent.ExportLocation) ? defaultAgentFolder : Workspace.Current.MakeAbsolutePath(agent.ExportLocation);
                string filename = Path.Combine(agentFolder, agent.BasicName + ".cpp");
                string oldFilename = "";

                if (!string.IsNullOrEmpty(agent.OldName) && agent.OldName != agent.Name)
                {
                    oldFilename = Path.Combine(agentFolder, agent.BasicOldName + ".cpp");
                }

                Debug.Check(filename != oldFilename);

                try
                {
                    if (!Directory.Exists(agentFolder))
                    {
                        Directory.CreateDirectory(agentFolder);
                    }

                    if (!File.Exists(filename))
                    {
                        ExportAgentCppFile(agent, filename);

                        if (File.Exists(oldFilename))
                        {
                            MergeFiles(oldFilename, filename, filename);
                        }
                    }
                    else
                    {
                        string behaviacAgentDir = GetBehaviacTypesDir();
                        string newFilename = Path.GetFileName(filename);
                        newFilename = Path.ChangeExtension(newFilename, ".new.cpp");
                        newFilename = Path.Combine(behaviacAgentDir, newFilename);

                        ExportAgentCppFile(agent, newFilename);
                        Debug.Check(File.Exists(newFilename));

                        MergeFiles(filename, newFilename, filename);
                    }

                    if (File.Exists(oldFilename))
                    {
                        File.Delete(oldFilename);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Merge Cpp Files Error : {0} {1}", filename, e.Message);
                }

                // after rename
                agent.OldName = null;
            }
        }

        private void ExportAgentCppFile(AgentType agent, string filename)
        {
            Debug.Check(agent != null && agent.Base != null);
            if (agent != null && agent.Base != null)
            {
                using (StringWriter file = new StringWriter())
                {
                    ExportFileWarningHeader(file);

                    file.WriteLine("#include \"{0}.h\"", agent.BasicName);
                    file.WriteLine();

                    ExportBeginComment(file, "", Behaviac.Design.Exporters.Exporter.file_init_part);
                    file.WriteLine();
                    ExportEndComment(file, "");
                    file.WriteLine();

                    string indent = "";
                    List<string> namespaces = new List<string>();

                    if (!string.IsNullOrEmpty(agent.Namespace))
                    {
                        namespaces = GetNamespaces(agent.Namespace);
                        indent = WriteNamespacesHead(file, namespaces);

                        ExportBeginComment(file, "", Behaviac.Design.Exporters.Exporter.namespace_init_part);
                        file.WriteLine();
                        ExportEndComment(file, "");
                        file.WriteLine();
                    }

                    bool hasStaticProperties = false;

                    foreach (PropertyDef prop in agent.GetProperties(true))
                    {
                        if (prop.IsStatic && (prop.IsCustomized || !agent.IsInherited) && !prop.IsInherited && !prop.IsPar && !prop.IsArrayElement)
                        {
                            hasStaticProperties = true;

                            string propType = DataCppExporter.GetGeneratedNativeType(prop.Type);
                            string defaultValue = DataCppExporter.GetGeneratedPropertyDefaultValue(prop, propType);

                            if (defaultValue != null)
                            {
                                defaultValue = " = " + defaultValue;
                            }
                            else
                            {
                                defaultValue = "";
                            }

                            file.WriteLine("{0}{1} {2}::{3}{4};", indent, propType, agent.BasicName, prop.BasicName, defaultValue);
                        }
                    }

                    if (hasStaticProperties)
                    {
                        file.WriteLine();
                    }

                    ExportMethodComment(file, indent, agent.OldName);

                    file.WriteLine("{0}{1}::{1}()", indent, agent.BasicName);
                    file.WriteLine("{0}{{", indent);

                    foreach (PropertyDef prop in agent.GetProperties(true))
                    {
                        if (!prop.IsStatic && (prop.IsCustomized || !agent.IsImplemented) && !prop.IsInherited && !prop.IsPar && !prop.IsArrayElement)
                        {
                            DataCppExporter.GeneratedPropertyDefaultValue(file, indent + "\t", prop);
                        }
                    }

                    ExportBeginComment(file, "\t" + indent, Behaviac.Design.Exporters.Exporter.constructor_part);
                    file.WriteLine();
                    ExportEndComment(file, "\t" + indent);

                    file.WriteLine("{0}}}", indent);
                    file.WriteLine();

                    ExportMethodComment(file, indent, agent.OldName);

                    file.WriteLine("{0}{1}::~{1}()", indent, agent.BasicName);
                    file.WriteLine("{0}{{", indent);
                    ExportBeginComment(file, "\t" + indent, Behaviac.Design.Exporters.Exporter.desctructor_part);
                    file.WriteLine();
                    ExportEndComment(file, "\t" + indent);
                    file.WriteLine("{0}}}", indent);
                    file.WriteLine();

                    foreach (MethodDef method in agent.GetMethods(true))
                    {
                        if (!agent.IsImplemented && !method.IsInherited && !method.IsNamedEvent)
                        {
                            string allParams = "";

                            foreach (MethodDef.Param param in method.Params)
                            {
                                if (!string.IsNullOrEmpty(allParams))
                                {
                                    allParams += ", ";
                                }

                                string constStr = param.IsConst ? "const " : "";

                                allParams += constStr + DataCppExporter.GetGeneratedNativeType(param.NativeType) + " " + param.Name;
                            }

                            string returnValue = DataCppExporter.GetGeneratedDefaultValue(method.ReturnType, method.NativeReturnType);

                            ExportMethodComment(file, indent, method.OldName);
                            method.OldName = null;

                            string methodReturnType = DataCppExporter.GetGeneratedNativeType(method.NativeReturnType);
                            if (Plugin.IsRefType(method.ReturnType) && !methodReturnType.Contains("*"))
                            {
                                methodReturnType += "*";
                            }

                            file.WriteLine("{0}{1} {2}::{3}({4})", indent, methodReturnType, agent.BasicName, method.BasicName, allParams);
                            file.WriteLine("{0}{{", indent);
                            ExportBeginComment(file, "\t" + indent, method.BasicName);

                            if (returnValue != null)
                            {
                                file.WriteLine("{0}\treturn {1};", indent, returnValue);
                            }
                            else
                            {
                                if (Plugin.IsCustomClassType(method.ReturnType))
                                {
                                    string rtnTypename = DataCppExporter.GetBasicGeneratedNativeType(method.NativeReturnType);
                                    rtnTypename = rtnTypename.Replace("*", "");

                                    file.WriteLine("{0}\t{1} tmp;", indent, rtnTypename);
                                    file.WriteLine("{0}\treturn tmp;", indent);
                                }
                            }

                            ExportEndComment(file, "\t" + indent);
                            file.WriteLine("{0}}}", indent);
                            file.WriteLine();
                        }
                    }

                    if (!string.IsNullOrEmpty(agent.Namespace))
                    {
                        ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.namespace_uninit_part);
                        file.WriteLine();
                        ExportEndComment(file, indent);

                        //end of namespace
                        WriteNamespacesTail(file, namespaces);
                    }

                    file.WriteLine();
                    ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.file_uninit_part);
                    file.WriteLine();
                    ExportEndComment(file, indent);

                    UpdateFile(file, filename);
                }
            }
        }

        private void ExportCustomizedTypesDefinition(string agentFolder)
        {
            if (TypeManager.Instance.Enums.Count > 0 || TypeManager.Instance.Structs.Count > 0)
            {
                using (StringWriter file = new StringWriter())
                {
                    file.WriteLine("// ---------------------------------------------------------------------");
                    file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                    file.WriteLine("// ---------------------------------------------------------------------");
                    file.WriteLine();

                    string headerFileMacro = "_BEHAVIAC_CUSTOMIZED_TYPES_H_";

                    file.WriteLine("#ifndef {0}", headerFileMacro);
                    file.WriteLine("#define {0}", headerFileMacro);

                    file.WriteLine();
                    file.WriteLine("#include \"behaviac/agent/agent.h\"");

                    if (TypeManager.Instance.Enums.Count > 0)
                    {
                        file.WriteLine();
                        file.WriteLine("// -------------------");
                        file.WriteLine("// Customized enums");
                        file.WriteLine("// -------------------");

                        for (int e = 0; e < TypeManager.Instance.Enums.Count; ++e)
                        {
                            EnumType enumType = TypeManager.Instance.Enums[e];

                            ExportEnumFile(file, enumType, null);
                        }
                    }

                    if (TypeManager.Instance.Structs.Count > 0)
                    {
                        file.WriteLine();
                        file.WriteLine("// -------------------");
                        file.WriteLine("// Customized structs");
                        file.WriteLine("// -------------------");

                        for (int s = 0; s < TypeManager.Instance.Structs.Count; s++)
                        {
                            StructType structType = TypeManager.Instance.Structs[s];

                            ExportStructFile(file, structType, null);
                        }
                    }

                    file.WriteLine();
                    file.WriteLine("#endif // {0}", headerFileMacro);

                    string filename = Path.Combine(agentFolder, "behaviac_customized_types.h");

                    UpdateFile(file, filename);
                }
            }
        }

        private void ExportEnumFile(StringWriter file, EnumType enumType, string filename)
        {
            StringWriter enumfile = file;
            bool shouldExportEnum = (!enumType.IsImplemented || !string.IsNullOrEmpty(filename));
            bool hasSetExportLocation = (!string.IsNullOrEmpty(filename) || !string.IsNullOrEmpty(enumType.ExportLocation));
            string enumHeaderFileMacro = string.Format("_BEHAVIAC_ENUM_{0}_H_", enumType.Name.ToUpperInvariant());

            if (shouldExportEnum)
            {
                if (hasSetExportLocation)
                {
                    enumfile = new StringWriter();

                    enumfile.WriteLine("// ---------------------------------------------------------------------");
                    enumfile.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                    enumfile.WriteLine("// ---------------------------------------------------------------------");
                    enumfile.WriteLine();
                    enumfile.WriteLine("#ifndef {0}", enumHeaderFileMacro);
                    enumfile.WriteLine("#define {0}", enumHeaderFileMacro);
                    enumfile.WriteLine();
                    enumfile.WriteLine("#include \"behaviac/agent/agent.h\"\n");
                }

                Debug.Check(enumfile != null);

                if (enumfile != null)
                {
                    enumfile.WriteLine();

                    string indent = "";

                    if (!string.IsNullOrEmpty(enumType.Namespace))
                    {
                        indent = "\t";

                        enumfile.WriteLine("namespace {0}", enumType.Namespace);
                        enumfile.WriteLine("{");
                    }

                    enumfile.WriteLine("{0}enum {1}", indent, enumType.Name);
                    enumfile.WriteLine("{0}{{", indent);

                    for (int m = 0; m < enumType.Members.Count; ++m)
                    {
                        EnumType.EnumMemberType member = enumType.Members[m];

                        if (member.Value >= 0)
                        {
                            enumfile.WriteLine("{0}\t{1} = {2},", indent, member.Name, member.Value);
                        }
                        else
                        {
                            enumfile.WriteLine("{0}\t{1},", indent, member.Name);
                        }
                    }

                    enumfile.WriteLine("{0}}};", indent);

                    if (!string.IsNullOrEmpty(enumType.Namespace))
                    {
                        enumfile.WriteLine("}");
                    }
                }
            }

            if (enumfile != null)
            {
                if (enumType.Fullname != "behaviac::EBTStatus")
                {
                    enumfile.WriteLine();
                    enumfile.WriteLine("DECLARE_BEHAVIAC_ENUM_EX({0}, {1});", enumType.Fullname, enumType.Name);
                    enumfile.WriteLine("BEHAVIAC_DECLARE_TYPE_VECTOR_HANDLER({0});", enumType.Fullname);
                    enumfile.WriteLine();
                }

                if (shouldExportEnum && hasSetExportLocation)
                {
                    enumfile.WriteLine("#endif // {0}", enumHeaderFileMacro);

                    string enumFilename = filename;

                    if (string.IsNullOrEmpty(enumFilename))
                    {
                        string enumLocation = Workspace.Current.MakeAbsolutePath(enumType.ExportLocation);
                        if (!Directory.Exists(enumLocation))
                        {
                            Directory.CreateDirectory(enumLocation);
                        }

                        enumFilename = Path.Combine(enumLocation, enumType.Name + ".h");
                    }

                    UpdateFile(enumfile, enumFilename);
                }
            }
        }

        private void ExportStructFile(StringWriter file, StructType structType, string filename)
        {
            StringWriter structfile = file;
            bool shouldExportStruct = (!structType.IsImplemented || !string.IsNullOrEmpty(filename));
            bool hasSetExportLocation = (!string.IsNullOrEmpty(filename) || !string.IsNullOrEmpty(structType.ExportLocation));
            string structHeaderFileMacro = string.Format("_BEHAVIAC_STRUCT_{0}_H_", structType.Name.ToUpperInvariant());

            if (shouldExportStruct)
            {
                if (hasSetExportLocation)
                {
                    structfile = new StringWriter();

                    structfile.WriteLine("// ---------------------------------------------------------------------");
                    structfile.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                    structfile.WriteLine("// ---------------------------------------------------------------------");
                    structfile.WriteLine();
                    structfile.WriteLine("#ifndef {0}", structHeaderFileMacro);
                    structfile.WriteLine("#define {0}", structHeaderFileMacro);
                    structfile.WriteLine();
                    structfile.WriteLine("#include \"behaviac/agent/agent.h\"\n");
                }

                structfile.WriteLine();

                string indent = "";

                if (!string.IsNullOrEmpty(structType.Namespace))
                {
                    indent = "\t";

                    structfile.WriteLine("namespace {0}", structType.Namespace);
                    structfile.WriteLine("{");
                }

                if (string.IsNullOrEmpty(structType.BaseName))
                {
                    structfile.WriteLine("{0}struct {1}", indent, structType.Name);
                }
                else
                {
                    StructType baseStruct = TypeManager.Instance.FindStruct(structType.BaseName);
                    Debug.Check(baseStruct != null);

                    string baseName = structType.BaseName;

                    if (baseStruct != null)
                    {
                        baseName = baseStruct.Name;

                        if (!string.IsNullOrEmpty(baseStruct.Namespace) && baseStruct.Namespace != structType.Namespace)
                        {
                            baseName = string.Format("{0}::{1}", baseStruct.Namespace, baseName);
                        }
                    }

                    structfile.WriteLine("{0}struct {1} : public {2}", indent, structType.Name, baseName);
                }

                structfile.WriteLine("{0}{{", indent);

                if (structType.Properties.Count > 0)
                {
                    for (int m = 0; m < structType.Properties.Count; ++m)
                    {
                        PropertyDef member = structType.Properties[m];
                        if (!member.IsInherited)
                        {
                            structfile.WriteLine("{0}\t{1} {2};", indent, DataCppExporter.GetGeneratedNativeType(member.NativeType), member.BasicName);
                        }
                    }
                }

                //structfile.WriteLine();
                //structfile.WriteLine("DECLARE_BEHAVIAC_STRUCT({0});", structType.Fullname);

                structfile.WriteLine("{0}}};", indent);

                if (!string.IsNullOrEmpty(structType.Namespace))
                {
                    structfile.WriteLine("}");
                }
            }

            structfile.WriteLine();
            structfile.WriteLine("BEHAVIAC_EXTEND_EXISTING_TYPE_EX({0}, {1});", structType.Fullname, structType.IsRef ? "true" : "false");
            structfile.WriteLine("BEHAVIAC_DECLARE_TYPE_VECTOR_HANDLER({0});", structType.Fullname);
            structfile.WriteLine();

            structfile.WriteLine("template< typename SWAPPER >");
            structfile.WriteLine("inline void SwapByteImplement({0}& v)", structType.Fullname);
            structfile.WriteLine("{");
            for (int m = 0; m < structType.Properties.Count; ++m)
            {
                PropertyDef member = structType.Properties[m];
                structfile.WriteLine("\tSwapByteImplement< SWAPPER >(v.{0});", member.BasicName);
            }
            structfile.WriteLine("}");
            structfile.WriteLine();

            structfile.WriteLine("namespace behaviac");
            structfile.WriteLine("{");
            structfile.WriteLine("\tnamespace PrivateDetails");
            structfile.WriteLine("\t{");
            structfile.WriteLine("\t\ttemplate<>");
            structfile.WriteLine("\t\tinline bool Equal(const {0}& lhs, const {0}& rhs)", structType.Fullname);
            structfile.WriteLine("\t\t{");
            if (structType.IsRef || structType.Properties.Count == 0)
            {
                structfile.WriteLine("\t\t\treturn &lhs == &rhs;");
            }
            else
            {
                structfile.Write("\t\t\treturn ");
                for (int m = 0; m < structType.Properties.Count; ++m)
                {
                    PropertyDef member = structType.Properties[m];
                    string preStr = "";
                    if (m > 0)
                    {
                        preStr = "\t\t\t\t&& ";
                    }

                    string postStr = "";
                    if (m == structType.Properties.Count - 1)
                    {
                        postStr = ";";
                    }

                    structfile.WriteLine("{0}Equal(lhs.{1}, rhs.{1}){2}", preStr, member.BasicName, postStr);
                }
            }
            structfile.WriteLine("\t\t}");
            structfile.WriteLine("\t}");
            structfile.WriteLine("}");

            if (shouldExportStruct && hasSetExportLocation)
            {
                structfile.WriteLine();
                structfile.WriteLine("#endif // {0}", structHeaderFileMacro);

                string structFilename = filename;

                if (string.IsNullOrEmpty(structFilename))
                {
                    string structLocation = Workspace.Current.MakeAbsolutePath(structType.ExportLocation);

                    if (!Directory.Exists(structLocation))
                    {
                        Directory.CreateDirectory(structLocation);
                    }

                    structFilename = Path.Combine(structLocation, structType.Name + ".h");
                }

                UpdateFile(structfile, structFilename);
            }
        }

        private void ExportCustomizedTypesImplemention(string agentFolder)
        {
            if (TypeManager.Instance.Enums.Count > 0)
            {
                using (StringWriter file = new StringWriter())
                {
                    file.WriteLine("// ---------------------------------------------------------------------");
                    file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                    file.WriteLine("// ---------------------------------------------------------------------");
                    file.WriteLine();
                    //file.WriteLine("#include \"behaviac/agent/registermacros.h\"");
                    //file.WriteLine("#include \"behaviac_customized_types.h\"");
                    file.WriteLine("#include \"../behaviac_types.h\"");
                    file.WriteLine();

                    file.WriteLine("// -------------------");
                    file.WriteLine("// Customized enums");
                    file.WriteLine("// -------------------");

                    for (int e = 0; e < TypeManager.Instance.Enums.Count; ++e)
                    {
                        EnumType enumType = TypeManager.Instance.Enums[e];

                        if (enumType.Fullname == "behaviac::EBTStatus")
                        {
                            continue;
                        }

                        StringWriter enumfile = file;

                        if (!string.IsNullOrEmpty(enumType.ExportLocation))
                        {
                            enumfile = new StringWriter();

                            enumfile.WriteLine("// ---------------------------------------------------------------------");
                            enumfile.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                            enumfile.WriteLine("// ---------------------------------------------------------------------");
                            enumfile.WriteLine();
                            enumfile.WriteLine("#include \"behaviac/agent/registermacros.h\"");
                            enumfile.WriteLine("#include \"{0}.h\"", enumType.Name);
                            enumfile.WriteLine();
                        }

                        enumfile.WriteLine();

                        string fullName = enumType.Name;

                        if (!string.IsNullOrEmpty(enumType.Namespace))
                        {
                            fullName = enumType.Namespace + "::" + enumType.Name;
                        }

                        enumfile.WriteLine("BEHAVIAC_BEGIN_ENUM_EX({0}, {1})", fullName, enumType.Name);
                        enumfile.WriteLine("{");
                        enumfile.WriteLine("\tBEHAVIAC_ENUMCLASS_DISPLAY_INFO_EX(L\"{0}\", L\"{1}\");", enumType.DisplayName, enumType.Description);
                        enumfile.WriteLine();

                        for (int m = 0; m < enumType.Members.Count; ++m)
                        {
                            EnumType.EnumMemberType member = enumType.Members[m];

                            string fullMemberName = member.Name;

                            if (!string.IsNullOrEmpty(enumType.Namespace))
                            {
                                fullMemberName = enumType.Namespace + "::" + member.Name;
                            }

                            if (member.DisplayName != member.Name || !string.IsNullOrEmpty(member.Description))
                            {
                                enumfile.WriteLine("\tBEHAVIAC_ENUM_ITEM_EX({0}, \"{1}\").DISPLAY_INFO(L\"{2}\", L\"{3}\");", fullMemberName, member.Name, member.DisplayName, member.Description);
                            }
                            else
                            {
                                enumfile.WriteLine("\tBEHAVIAC_ENUM_ITEM_EX({0}, \"{1}\");", fullMemberName, member.Name);
                            }
                        }

                        enumfile.WriteLine("}");
                        enumfile.WriteLine("BEHAVIAC_END_ENUM_EX()");

                        if (!string.IsNullOrEmpty(enumType.ExportLocation))
                        {
                            string enumLocation = Workspace.Current.MakeAbsolutePath(enumType.ExportLocation);
                            string enumFilename = Path.Combine(enumLocation, enumType.Name + ".cpp");

                            UpdateFile(enumfile, enumFilename);
                        }
                    }

                    file.WriteLine();

                    string filename = Path.Combine(agentFolder, "behaviac_customized_types.cpp");

                    UpdateFile(file, filename);
                }
            }
        }

        private void ExportAgentMeta(string agentFolder)
        {
            using (StringWriter file = new StringWriter())
            {
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine();

                string headerFileMacro = "_BEHAVIAC_AGENT_PROPERTIES_H_";

                file.WriteLine("#ifndef {0}", headerFileMacro);
                file.WriteLine("#define {0}", headerFileMacro);
                file.WriteLine();

                file.WriteLine("#include \"behaviac_agent_headers.h\"");
                file.WriteLine("#include \"behaviac_agent_member_visitor.h\"");

                if (TypeManager.Instance.Enums.Count > 0 || TypeManager.Instance.Structs.Count > 0)
                {
                    file.WriteLine("#include \"behaviac_customized_types.h\"");
                }

                file.WriteLine();
                file.WriteLine("namespace behaviac");
                file.WriteLine("{");

                ExportDelegateMethod(file);

                file.WriteLine("}");
                file.WriteLine("#endif // {0}", headerFileMacro);

                string filename = Path.Combine(agentFolder, "behaviac_agent_meta.h");

                UpdateFile(file, filename);
            }

            using (StringWriter file = new StringWriter())
            {
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine();

                file.WriteLine("#include \"behaviac/common/meta.h\"");
                file.WriteLine("#include \"behaviac/common/member.h\"");
                file.WriteLine("#include \"behaviac_agent_meta.h\"");
                file.WriteLine();
                file.WriteLine("namespace behaviac");
                file.WriteLine("{");

                PreExportMeta(file);

                file.WriteLine("\tclass BehaviorLoaderImplement : BehaviorLoader");
                file.WriteLine("\t{");

                file.WriteLine("\tpublic:");
                file.WriteLine("\t\tBehaviorLoaderImplement()");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tAgentMeta::SetBehaviorLoader(this);");
                file.WriteLine("\t\t}\n");

                // destructor
                file.WriteLine("\t\tvirtual ~BehaviorLoaderImplement()");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t}\n");

                // load
                file.WriteLine("\t\tvirtual bool load()");
                file.WriteLine("\t\t{");

                ExportMembers(file);

                file.WriteLine();

                foreach (AgentType agentType in Plugin.AgentTypes)
                {
                    if (agentType != null)
                    {
                        bool isStatic = agentType.IsStatic;

                        if (!isStatic)
                        {
                            file.WriteLine("\t\t\tAgentMeta::Register<{0}>(\"{0}\");", agentType.Name);
                        }
                    }
                }

                foreach (EnumType enumType in TypeManager.Instance.Enums)
                {
                    string enumFullname = enumType.Fullname;
                    file.WriteLine("\t\t\tAgentMeta::Register<{0}>(\"{0}\");", enumFullname);
                }

                foreach (StructType structType in TypeManager.Instance.Structs)
                {
                    string structFullname = structType.Fullname;
                    file.WriteLine("\t\t\tAgentMeta::Register<{0}>(\"{0}\");", structFullname);
                }

                foreach (string newTypes in Plugin.TypeRenames.Values)
                {
                    file.WriteLine("\t\t\tAgentMeta::Register<{0}>(\"{0}\");", newTypes);
                }

                if (Plugin.InstanceNames.Count > 0)
                {
                    file.WriteLine();

                    foreach (Plugin.InstanceName_t instance in Plugin.InstanceNames)
                    {
                        file.WriteLine("\t\t\tAgent::RegisterInstanceName<{0}>(\"{1}\");", instance.ClassName, instance.Name);
                    }
                }

                if (Workspace.Current.UseIntValue)
                {
                    file.WriteLine();
                    file.WriteLine("\t\t\tbehaviac::Workspace::GetInstance()->SetUseIntValue(true);");
                }

                file.WriteLine("\n\t\t\treturn true;");
                file.WriteLine("\t\t}\n");

                // unLoad
                file.WriteLine("\t\tvirtual bool unLoad()");
                file.WriteLine("\t\t{");

                foreach (AgentType agentType in Plugin.AgentTypes)
                {
                    if (agentType != null)
                    {
                        bool isStatic = agentType.IsStatic;

                        if (!isStatic)
                        {
                            file.WriteLine("\t\t\tAgentMeta::UnRegister<{0}>(\"{0}\");", agentType.Name);
                        }
                    }
                }

                foreach (EnumType enumType in TypeManager.Instance.Enums)
                {
                    string enumFullname = enumType.Fullname;

                    file.WriteLine("\t\t\tAgentMeta::UnRegister<{0}>(\"{0}\");", enumFullname);
                }

                foreach (StructType structType in TypeManager.Instance.Structs)
                {
                    string structFullname = structType.Fullname;

                    file.WriteLine("\t\t\tAgentMeta::UnRegister<{0}>(\"{0}\");", structFullname);
                }

                if (Plugin.InstanceNames.Count > 0)
                {
                    file.WriteLine();

                    foreach (Plugin.InstanceName_t instance in Plugin.InstanceNames)
                    {
                        file.WriteLine("\t\t\tAgent::UnRegisterInstanceName<{0}>(\"{1}\");", instance.ClassName, instance.Name);
                    }
                }

                file.WriteLine("\n\t\t\treturn true;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t};");
                file.WriteLine();

                file.WriteLine("\tstatic BehaviorLoaderImplement _behaviorLoaderImplement_;");
                file.WriteLine();
                file.WriteLine("\tstatic BehaviorLoaderImplement* _pBehaviorLoader_ = NULL;");
                file.WriteLine();
                file.WriteLine("\tvoid InitBehaviorLoader()");
                file.WriteLine("\t{");
                file.WriteLine("\t\t_pBehaviorLoader_ = BEHAVIAC_NEW BehaviorLoaderImplement();");
                file.WriteLine("\t}");
                file.WriteLine();
                file.WriteLine("\tvoid DestroyBehaviorLoader()");
                file.WriteLine("\t{");
                file.WriteLine("\t\tif (_pBehaviorLoader_)");
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\tBEHAVIAC_DELETE _pBehaviorLoader_;");
                file.WriteLine("\t\t\t_pBehaviorLoader_ = NULL;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t}");

                file.WriteLine("}");

                string cppFilename = Path.Combine(agentFolder, "behaviac_agent_meta.cpp");

                UpdateFile(file, cppFilename);
            }
        }

        private void ExportMembers(StringWriter file)
        {
            file.WriteLine("\t\t\tAgentMeta::SetTotalSignature({0}u);", CRC32.CalcCRC(Plugin.Signature));
            file.WriteLine();
            file.WriteLine("\t\t\tAgentMeta* meta = NULL;");
            file.WriteLine("\t\t\tBEHAVIAC_UNUSED_VAR(meta);");

            foreach (StructType s in TypeManager.Instance.Structs)
            {
                if (s.Properties.Count == 0)
                {
                    continue;
                }

                string structName = s.Fullname;
                string signature = s.Signature;

                file.WriteLine("\n\t\t\t// {0}", structName);
                file.WriteLine("\t\t\tmeta = BEHAVIAC_NEW AgentMeta({0}u);", CRC32.CalcCRC(signature));
                file.WriteLine("\t\t\tAgentMeta::GetAgentMetas()[{0}u] = meta;", CRC32.CalcCRC(structName));

                IList<PropertyDef> properties = s.Properties;

                foreach (PropertyDef prop in properties)
                {
                    string propType = DataCppExporter.GetGeneratedNativeType(prop.NativeItemType);

                    if (Plugin.IsRefType(prop.Type))
                    {
                        if (!propType.Contains("*"))
                        {
                            propType += "*";
                        }
                    }
                    else if (propType != "char*" && !Plugin.IsArrayType(prop.Type))
                    {
                        propType = propType.Replace("*", "");
                    }

                    string propName = prop.Name.Replace("::", "_").Replace("[]", "");
                    string propFullname = propName;

                    if (!string.IsNullOrEmpty(s.Namespace))
                    {
                        propFullname = s.Namespace + "::" + propFullname;
                    }

                    propFullname = propFullname.Replace("::", "_");

                    string bindingProperty = string.Format("BEHAVIAC_NEW CMemberProperty< {0} >(\"{1}\", Set_{2}, Get_{2})", propType, prop.BasicName, propFullname);
                    file.WriteLine("\t\t\tmeta->RegisterMemberProperty({0}u, {1});", CRC32.CalcCRC(prop.BasicName), bindingProperty);
                }
            }

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                string agentTypeName = agent.Name;
                string signature = agent.GetSignature(true);

                file.WriteLine("\n\t\t\t// {0}", agentTypeName);
                file.WriteLine("\t\t\tmeta = BEHAVIAC_NEW AgentMeta({0}u);", CRC32.CalcCRC(signature));
                file.WriteLine("\t\t\tAgentMeta::GetAgentMetas()[{0}u] = meta;", CRC32.CalcCRC(agentTypeName));

                IList<PropertyDef> properties = agent.GetProperties(true);

                foreach (PropertyDef prop in properties)
                {
                    if (!prop.IsPar)
                    {
                        string bindingProperty = "";
                        string registerName = "RegisterMemberProperty";
                        string propType = DataCppExporter.GetGeneratedNativeType(prop.NativeItemType);
                        string propItemName = prop.BasicName;
                        string propName = prop.Name.Replace("::", "_").Replace("[]", "");

                        if (Plugin.IsRefType(prop.Type))
                        {
                            if (!propType.Contains("*"))
                            {
                                propType += "*";
                            }
                        }
                        else if (propType != "char*" && !Plugin.IsArrayType(prop.Type))
                        {
                            propType = propType.Replace("*", "");
                        }

                        if (prop.IsArrayElement)
                        {
                            propItemName = propItemName.Replace("[]", "[index]");
                        }

                        bool isMemberProp = prop.IsMember || agent.IsCustomized;

                        if (agent.IsStatic || isMemberProp && prop.IsStatic)
                        {
                            if (isMemberProp)
                            {
                                if (prop.IsArrayElement)
                                {
                                    bindingProperty = string.Format("BEHAVIAC_NEW CStaticMemberArrayItemProperty< {0} >(\"{1}\", Set_{2}, Get_{2})",
                                                                    propType, prop.BasicName, propName);
                                }
                                else
                                {
                                    bindingProperty = string.Format("BEHAVIAC_NEW CStaticMemberProperty< {0} >(\"{1}\", Set_{2}, Get_{2})",
                                                                    propType, prop.BasicName, propName);
                                }
                            }
                            else
                            {
                                Debug.Check(false);
                            }
                        }
                        else
                        {
                            if (isMemberProp)
                            {
                                if (!prop.IsStatic)
                                {
                                    string setValue = "";

                                    if (!prop.IsReadonly)
                                    {
                                        setValue = string.Format("(({0})self).{1} = value;", agentTypeName, propItemName);
                                    }

                                    if (prop.IsArrayElement)
                                    {
                                        bindingProperty = string.Format("BEHAVIAC_NEW CMemberArrayItemProperty< {0} >(\"{1}\", Set_{2}, Get_{2})",
                                                                        propType, prop.BasicName, propName);
                                    }
                                    else
                                    {
                                        bindingProperty = string.Format("BEHAVIAC_NEW CMemberProperty< {0} >(\"{1}\", Set_{2}, Get_{2})",
                                                                        propType, prop.BasicName, propName);
                                    }
                                }
                            }
                            else
                            {
                                registerName = prop.IsStatic ? "RegisterStaticCustomizedProperty" : "RegisterCustomizedProperty";

                                if (prop.IsArrayElement)
                                {
                                    string propBasicName = prop.BasicName.Replace("[]", "");
                                    bindingProperty = string.Format("BEHAVIAC_NEW CCustomizedArrayItemProperty< {0} >({1}u, \"{2}\")",
                                                                    propType, CRC32.CalcCRC(propBasicName), propBasicName);
                                }
                                else
                                {
                                    bindingProperty = string.Format("BEHAVIAC_NEW CCustomizedProperty< {0} >({1}u, \"{2}\", \"{3}\")",
                                                                    propType, CRC32.CalcCRC(prop.BasicName), prop.BasicName, prop.DefaultValue);
                                }
                            }
                        }

                        file.WriteLine("\t\t\tmeta->{0}({1}u, {2});", registerName, CRC32.CalcCRC(prop.BasicName), bindingProperty);
                    }
                }

                IList<MethodDef> methods = agent.GetMethods(true);

                foreach (MethodDef method in methods)
                {
                    bool hasRefParam = false;

                    foreach (MethodDef.Param param in method.Params)
                    {
                        if (param.IsRef || param.IsOut || Plugin.IsRefType(param.Type) || IsStructType(param))
                        {
                            hasRefParam = true;
                            break;
                        }
                    }

                    string agentMethod = "";
                    string paramTypes = "";
                    string paramTypeValues = "";
                    string paramValues = "";
                    string template_suffix = "";

                    if (method.IsNamedEvent || !hasRefParam)
                    {
                        foreach (MethodDef.Param param in method.Params)
                        {
                            if (!string.IsNullOrEmpty(paramTypes))
                            {
                                paramTypes += ", ";
                            }

                            if (!string.IsNullOrEmpty(paramValues))
                            {
                                paramValues += ", ";
                            }

                            string paramType = DataCppExporter.GetGeneratedNativeType(param.Type, param.NativeType);
                            paramTypes += paramType;
                            paramTypeValues += ", " + paramType + " " + param.Name;
                            paramValues += param.Name;
                        }
                    }

                    if (method.Params.Count > 0)
                    {
                        template_suffix = string.Format("_{0}", method.Params.Count);
                    }

                    string methodReturnType = DataCppExporter.GetGeneratedNativeType(method.NativeReturnType);
                    if (Plugin.IsRefType(method.ReturnType) && !methodReturnType.Contains("*"))
                    {
                        methodReturnType += "*";
                    }

                    string methodName = agentTypeName.Replace("::", "_") + "_" + method.BasicName.Replace("::", "_");

                    if (method.IsNamedEvent)
                    {
                        if (!string.IsNullOrEmpty(paramTypes))
                        {
                            paramTypes = string.Format("<{0}>", paramTypes);
                        }

                        agentMethod = string.Format("BEHAVIAC_NEW CAgentMethodVoid{3}{0}(FunctionPointer_{1}) /* {2} */", paramTypes, methodName, method.BasicName, template_suffix);

                        file.WriteLine("\t\t\tmeta->RegisterMethod({0}u, {1});", CRC32.CalcCRC(method.BasicName), agentMethod);
                    }
                    else
                    {
                        if (hasRefParam)
                        {
                            string methodFullname = method.Name.Replace("::", "_");
                            agentMethod = string.Format("BEHAVIAC_NEW CMethod_{0}()", methodFullname);
                        }
                        else
                        {
                            if (method.IsStatic)
                            {
                                if (paramTypeValues.StartsWith(", "))
                                {
                                    paramTypeValues = paramTypeValues.Substring(2);
                                }

                                if (methodReturnType == "void")
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = string.Format("<{0}>", paramTypes);
                                    }

                                    agentMethod = string.Format("BEHAVIAC_NEW CAgentStaticMethodVoid{2}{0}(FunctionPointer_{1})",
                                                                paramTypes, methodName, template_suffix);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = ", " + paramTypes;
                                    }

                                    agentMethod = string.Format("BEHAVIAC_NEW CAgentStaticMethod{3}< {0}{1} >(FunctionPointer_{2})",
                                                                methodReturnType, paramTypes, methodName, template_suffix);
                                }
                            }
                            else
                            {
                                if (methodReturnType == "void")
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = string.Format("<{0}>", paramTypes);
                                    }

                                    agentMethod = string.Format("BEHAVIAC_NEW CAgentMethodVoid{2}{0}(FunctionPointer_{1})",
                                                                paramTypes, methodName, template_suffix);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = ", " + paramTypes;
                                    }

                                    agentMethod = string.Format("BEHAVIAC_NEW CAgentMethod{3}< {0}{1} >(FunctionPointer_{2})",
                                                                methodReturnType, paramTypes, methodName, template_suffix);
                                }
                            }
                        }

                        file.WriteLine("\t\t\tmeta->RegisterMethod({0}u, {1});", CRC32.CalcCRC(method.BasicName), agentMethod);
                    }
                }
            }
        }

        private static string getNamespace(string className)
        {
            if (!string.IsNullOrEmpty(className))
            {
                int index = className.LastIndexOf(":");

                if (index > 1)
                {
                    return className.Substring(0, index - 1) + "::";
                }
            }

            return string.Empty;
        }
    }
}
