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
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;
using PluginBehaviac.Properties;
using PluginBehaviac.DataExporters;
using PluginBehaviac.NodeExporters;

namespace PluginBehaviac.Exporters
{
    public class ExporterCs : Behaviac.Design.Exporters.Exporter
    {
        public ExporterCs(BehaviorNode node, string outputFolder, string filename, List<string> includedFilenames = null)
            : base(node, outputFolder, filename + ".cs", includedFilenames)
        {
            _outputFolder = Path.Combine(Path.GetFullPath(_outputFolder), "behaviac_generated");
        }

        public override Behaviac.Design.FileManagers.SaveResult Export(List<BehaviorNode> behaviors, bool exportBehaviors, bool exportMeta, int exportFileCount)
        {
            string behaviorFilename = "behaviors/generated_behaviors.cs";
            string typesFolder = string.Empty;
            Behaviac.Design.FileManagers.SaveResult result = VerifyFilename(exportBehaviors, ref behaviorFilename, ref typesFolder);

            if (Behaviac.Design.FileManagers.SaveResult.Succeeded == result)
            {
                if (exportBehaviors)
                {
                    string behaviorFolder = Path.GetDirectoryName(behaviorFilename);

                    ExportBehaviors(behaviors, behaviorFilename, exportFileCount);
                }

                if (exportMeta)
                {
                    ExportMembers(typesFolder);
                    ExportAgents(typesFolder);
                    ExportCustomizedTypes(typesFolder);
                }
            }

            return result;
        }

        public override void PreviewAgentFile(AgentType agent)
        {
            string behaviacTypesDir = GetBehaviacTypesDir();
            string tmpFilename = Path.Combine(behaviacTypesDir, agent.BasicName + ".cs");

            ExportAgentCsFile(agent, tmpFilename, true);

            PreviewFile(tmpFilename);
        }

        public override void PreviewEnumFile(EnumType enumType)
        {
            string behaviacTypesDir = GetBehaviacTypesDir();
            string tmpFilename = Path.Combine(behaviacTypesDir, enumType.Name + ".cs");

            ExportEnumFile(null, enumType, tmpFilename);

            PreviewFile(tmpFilename);
        }

        public override void PreviewStructFile(StructType structType)
        {
            string behaviacTypesDir = GetBehaviacTypesDir();
            string tmpFilename = Path.Combine(behaviacTypesDir, structType.Name + ".cs");

            ExportStructFile(null, null, structType, tmpFilename);

            PreviewFile(tmpFilename);
        }

        private void ExportBehaviors(List<BehaviorNode> behaviors, string filename, int exportFileCount)
        {
            using (StringWriter file = new StringWriter())
            {
                ExportHead(file, filename);

                if (exportFileCount == 1)
                {
                    foreach (BehaviorNode behavior in behaviors)
                    {
                        ExportBody(file, behavior);
                    }
                }
                else
                {
                    foreach (BehaviorNode behavior in behaviors)
                    {
                        string behaviorFilename = behavior.RelativePath;
                        behaviorFilename = behaviorFilename.Replace("\\", "/");
                        behaviorFilename = Path.ChangeExtension(behaviorFilename, "cs");
                        behaviorFilename = Path.Combine("behaviors", behaviorFilename);

                        string agentFolder = string.Empty;

                        Behaviac.Design.FileManagers.SaveResult result = VerifyFilename(true, ref behaviorFilename, ref agentFolder);

                        if (Behaviac.Design.FileManagers.SaveResult.Succeeded == result)
                        {
                            using (StringWriter behaviorFile = new StringWriter())
                            {
                                ExportHead(behaviorFile, behaviorFilename);

                                ExportBody(behaviorFile, behavior);

                                ExportTail(behaviorFile);

                                UpdateFile(behaviorFile, behaviorFilename);
                            }
                        }
                    }
                }

                ExportTail(file);

                UpdateFile(file, filename);
            }
        }

        private Behaviac.Design.FileManagers.SaveResult VerifyFilename(bool exportBehaviors, ref string behaviorFilename, ref string agentFolder)
        {
            behaviorFilename = Path.Combine(_outputFolder, behaviorFilename);
            agentFolder = Path.Combine(_outputFolder, "types");

            // get the abolute folder of the file we want to export
            string folder = Path.GetDirectoryName(behaviorFilename);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (!Directory.Exists(agentFolder))
            {
                Directory.CreateDirectory(agentFolder);
            }

            if (exportBehaviors)
            {
                // verify it can be writable
                return Behaviac.Design.FileManagers.FileManager.MakeWritable(behaviorFilename, Resources.ExportFileWarning);
            }

            return Behaviac.Design.FileManagers.SaveResult.Succeeded;
        }

        private void ExportHead(StringWriter file, string exportFilename)
        {
            string wsfolder = Path.GetDirectoryName(Workspace.Current.FileName);
            exportFilename = Behaviac.Design.FileManagers.FileManager.MakeRelative(wsfolder, exportFilename);
            exportFilename = exportFilename.Replace("\\", "/");

            // write comments
            file.WriteLine("// ---------------------------------------------------------------------");
            file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
            file.WriteLine("// Export file: {0}", exportFilename);
            file.WriteLine("// ---------------------------------------------------------------------");
            file.WriteLine();

            // write the namespaces for the game agents
            file.WriteLine("using System;");
            file.WriteLine("using System.Collections;");
            file.WriteLine("using System.Collections.Generic;");
            file.WriteLine("using System.Reflection;");
            file.WriteLine();

            // write namespace
            file.WriteLine("namespace behaviac\r\n{");
        }

        private string getValidFilename(string filename)
        {
            filename = filename.Replace('/', '_');
            filename = filename.Replace('-', '_');

            return filename;
        }

        private void ExportBody(StringWriter file, BehaviorNode behavior)
        {
            behavior.PreExport();

            string filename = Path.ChangeExtension(behavior.RelativePath, "").Replace(".", "");
            filename = filename.Replace('\\', '/');

            // write comments
            file.WriteLine("\t// Source file: {0}\r\n", filename);

            string btClassName = string.Format("bt_{0}", getValidFilename(filename));
            string agentType = behavior.AgentType.Name;

            // create the class definition of its attachments
            ExportAttachmentClass(file, btClassName, (Node)behavior);

            // create the class definition of its children
            foreach (Node child in ((Node)behavior).GetChildNodes())
            {
                ExportNodeClass(file, btClassName, agentType, behavior, child);
            }

            // create the bt class
            file.WriteLine("\tpublic static class {0}\r\n\t{{", btClassName);

            // export the build function
            file.WriteLine("\t\tpublic static bool build_behavior_tree(BehaviorTree bt)\r\n\t\t{");
            file.WriteLine("\t\t\tbt.SetClassNameString(\"BehaviorTree\");");
            file.WriteLine("\t\t\tbt.SetId(-1);");
            file.WriteLine("\t\t\tbt.SetName(\"{0}\");", filename);
            file.WriteLine("\t\t\tbt.IsFSM = {0};", ((Node)behavior).IsFSM ? "true" : "false");
            file.WriteLine("#if !BEHAVIAC_RELEASE");
            file.WriteLine("\t\t\tbt.SetAgentType(\"{0}\");", agentType.Replace("::", "."));
            file.WriteLine("#endif");

            if (!string.IsNullOrEmpty(((Behavior)behavior).Domains))
            {
                file.WriteLine("\t\t\tbt.SetDomains(\"{0}\");", ((Behavior)behavior).Domains);
            }

            if (((Behavior)behavior).DescriptorRefs.Count > 0)
            {
                file.WriteLine("\t\t\tbt.SetDescriptors(\"{0}\");", DesignerPropertyUtility.RetrieveExportValue(((Behavior)behavior).DescriptorRefs));
            }

            ExportPars(file, agentType, "bt", (Node)behavior, "\t\t");

            // export its attachments
            ExportAttachment(file, btClassName, agentType, "bt", (Node)behavior, "\t\t\t");

            file.WriteLine("\t\t\t// children");

            // export its children
            if (((Node)behavior).IsFSM)
            {
                file.WriteLine("\t\t\t{");
                file.WriteLine("\t\t\t\tFSM fsm = new FSM();");
                file.WriteLine("\t\t\t\tfsm.SetClassNameString(\"FSM\");");
                file.WriteLine("\t\t\t\tfsm.SetId(-1);");
                file.WriteLine("\t\t\t\tfsm.InitialId = {0};", behavior.InitialStateId);
                file.WriteLine("#if !BEHAVIAC_RELEASE");
                file.WriteLine("\t\t\t\tfsm.SetAgentType(\"{0}\");", agentType.Replace("::", "."));
                file.WriteLine("#endif");

                foreach (Node child in ((Node)behavior).FSMNodes)
                {
                    ExportNode(file, btClassName, agentType, "fsm", child, 4);
                }

                file.WriteLine("\t\t\t\tbt.AddChild(fsm);");
                file.WriteLine("\t\t\t}");
            }
            else
            {
                foreach (Node child in ((Node)behavior).GetChildNodes())
                {
                    ExportNode(file, btClassName, agentType, "bt", child, 3);
                }
            }

            file.WriteLine("\t\t\treturn true;");

            // close the build function
            file.WriteLine("\t\t}");

            // close class
            file.WriteLine("\t}\r\n");

            behavior.PostExport();
        }

        private void ExportTail(StringWriter file)
        {
            // close namespace
            file.WriteLine("}");
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
                    string type = DataCsExporter.GetGeneratedParType(pars[i].Type);
                    string value = pars[i].DefaultValue.Replace("\"", "\\\"");

                    file.WriteLine("{0}\t{1}.AddLocal(\"{2}\", \"{3}\", \"{4}\", \"{5}\");", indent, nodeName, agentType, type, name, value);
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

                AttachmentCsExporter attachmentExporter = AttachmentCsExporter.CreateInstance(attach);
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
                    AttachmentCsExporter attachmentExporter = AttachmentCsExporter.CreateInstance(attach);
                    attachmentExporter.GenerateInstance(attach, file, indent, nodeName, agentType, btClassName);

                    string isPrecondition = attach.IsPrecondition && !attach.IsTransition ? "true" : "false";
                    string isEffector = attach.IsEffector && !attach.IsTransition ? "true" : "false";
                    string isTransition = attach.IsTransition ? "true" : "false";
                    file.WriteLine("{0}\t{1}.Attach({2}, {3}, {4}, {5});", indent, parentName, nodeName, isPrecondition, isEffector, isTransition);

                    if (attach is Behaviac.Design.Attachments.Event)
                    {
                        file.WriteLine("{0}\t{1}.SetHasEvents({1}.HasEvents() | ({2} is Event));", indent, parentName, nodeName);
                    }

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

            NodeCsExporter nodeExporter = NodeCsExporter.CreateInstance(node);
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
            NodeCsExporter nodeExporter = NodeCsExporter.CreateInstance(node);
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
                file.WriteLine("{0}\t{1}.AddChild({2});", indent, parentName, nodeName);
            }
            else
            {
                // add the node as its customized children
                file.WriteLine("{0}\t{1}.SetCustomCondition({2});", indent, parentName, nodeName);
            }

            // export the child nodes
            if (!node.IsFSM && !(node is ReferencedBehavior))
            {
                foreach (Node child in node.GetChildNodes())
                {
                    ExportNode(file, btClassName, agentType, nodeName, child, indentDepth + 1);
                }
            }

            file.WriteLine("{0}\t{1}.SetHasEvents({1}.HasEvents() | {2}.HasEvents());", indent, parentName, nodeName);

            // close the brackets for a better formatting in the generated code
            file.WriteLine("{0}}}", indent);
        }

        private void ExportAgents(string defaultAgentFolder)
        {
            foreach (AgentType agent in Plugin.AgentTypes)
            {
                if (agent.IsImplemented || agent.Name == "behaviac::Agent")
                {
                    continue;
                }

                string agentFolder = string.IsNullOrEmpty(agent.ExportLocation) ? defaultAgentFolder : Workspace.Current.MakeAbsolutePath(agent.ExportLocation);
                string filename = Path.Combine(agentFolder, agent.BasicName + ".cs");
                string oldFilename = "";

                if (!string.IsNullOrEmpty(agent.OldName) && agent.OldName != agent.Name)
                {
                    oldFilename = Path.Combine(agentFolder, agent.BasicOldName + ".cs");
                }

                Debug.Check(filename != oldFilename);
                agent.OldName = null;

                try
                {
                    if (!Directory.Exists(agentFolder))
                    {
                        Directory.CreateDirectory(agentFolder);
                    }

                    if (!File.Exists(filename))
                    {
                        ExportAgentCsFile(agent, filename, false);

                        if (File.Exists(oldFilename))
                        {
                            MergeFiles(oldFilename, filename, filename);
                        }
                    }
                    else
                    {
                        string behaviacAgentDir = GetBehaviacTypesDir();
                        string newFilename = Path.GetFileName(filename);
                        newFilename = Path.ChangeExtension(newFilename, ".new.cs");
                        newFilename = Path.Combine(behaviacAgentDir, newFilename);

                        ExportAgentCsFile(agent, newFilename, false);
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
                    Console.WriteLine("Merge Cs Files Error : {0} {1}", filename, e.Message);
                }
            }
        }

        private void ExportAgentCsFile(AgentType agent, string filename, bool preview)
        {
            using (StringWriter file = new StringWriter())
            {
                string indent = "";

                ExportFileWarningHeader(file);

                file.WriteLine("using System;");
                file.WriteLine("using System.Collections;");
                file.WriteLine("using System.Collections.Generic;");
                file.WriteLine();

                if (!preview)
                {
                    ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.file_init_part);
                    file.WriteLine();
                    ExportEndComment(file, indent);
                }

                file.WriteLine();

                if (!string.IsNullOrEmpty(agent.Namespace))
                {
                    indent = "\t";

                    file.WriteLine("namespace {0}", agent.Namespace.Replace("::", "."));
                    file.WriteLine("{");

                    if (!preview)
                    {
                        ExportBeginComment(file, "", Behaviac.Design.Exporters.Exporter.namespace_init_part);
                        file.WriteLine();
                        ExportEndComment(file, "");
                        file.WriteLine();
                    }
                }

                string agentDisplayName = string.IsNullOrEmpty(agent.DisplayName) ? agent.BasicName : agent.DisplayName;
                string agentDescription = string.IsNullOrEmpty(agent.Description) ? "" : agent.Description;
                //file.WriteLine("{0}[behaviac.TypeMetaInfo(\"{1}\", \"{2}\")]", indent, agentDisplayName, agentDescription);
                string baseClassStr = (agent.Base != null) ? string.Format(" : {0}", agent.Base.Name.Replace("::", ".")) : "";
                file.WriteLine("{0}public class {1}{2}", indent, agent.BasicName, baseClassStr);

                if (!preview)
                {
                    ExportBeginComment(file, indent, agent.BasicName);
                    ExportEndComment(file, indent);
                }

                file.WriteLine("{0}{{", indent);

                IList<PropertyDef> properties = agent.GetProperties(true);

                foreach (PropertyDef prop in properties)
                {
                    if ((preview || !agent.IsImplemented) && !prop.IsInherited && !prop.IsPar && !prop.IsArrayElement)
                    {
                        string staticStr = prop.IsStatic ? "static " : "";
                        string propType = DataCsExporter.GetGeneratedNativeType(prop.Type);
                        //string defaultValue = DataCsExporter.GetGeneratedPropertyDefaultValue(prop, propType);
                        string defaultValue = DataCsExporter.GetGeneratedPropertyDefaultValue(prop);

                        if (defaultValue != null)
                        {
                            defaultValue = " = " + defaultValue;
                        }

                        //file.WriteLine("{0}\t[behaviac.MemberMetaInfo(\"{1}\", \"{2}\")]", indent, prop.DisplayName, prop.BasicDescription);
                        if (prop.IsPublic)
                        {
                            file.WriteLine("{0}\tpublic {1}{2} {3}{4};", indent, staticStr, propType, prop.BasicName, defaultValue);
                        }
                        else
                        {
                            file.WriteLine("{0}\tprivate {1}{2} {3}{4};", indent, staticStr, propType, prop.BasicName, defaultValue);
                            file.WriteLine("{0}\tpublic {1}void _set_{2}({3} value)", indent, staticStr, prop.BasicName, propType);
                            file.WriteLine("{0}\t{{", indent);
                            file.WriteLine("{0}\t\t{1} = value;", indent, prop.BasicName);
                            file.WriteLine("{0}\t}}", indent);
                            file.WriteLine("{0}\tpublic {1}{2} _get_{3}()", indent, staticStr, propType, prop.BasicName);
                            file.WriteLine("{0}\t{{", indent);
                            file.WriteLine("{0}\t\treturn {1};", indent, prop.BasicName);
                            file.WriteLine("{0}\t}}", indent);
                        }

                        file.WriteLine();
                    }
                }

                IList<MethodDef> methods = agent.GetMethods(true);

                foreach (MethodDef method in methods)
                {
                    if ((preview || !agent.IsImplemented) && !method.IsInherited && !method.IsNamedEvent)
                    {
                        string publicStr = method.IsPublic ? "public " : "private ";
                        string staticStr = method.IsStatic ? "static " : "";

                        string allParams = "";

                        foreach (MethodDef.Param param in method.Params)
                        {
                            if (!string.IsNullOrEmpty(allParams))
                            {
                                allParams += ", ";
                            }

                            string paramType = DataCsExporter.GetGeneratedNativeType(param.NativeType);

                            if (param.IsRef)
                            {
                                paramType = "ref " + paramType;
                            }

                            allParams += paramType + " " + param.Name;
                        }

                        string returnType = DataCsExporter.GetGeneratedNativeType(method.ReturnType);
                        string returnValue = DataCsExporter.GetGeneratedDefaultValue(method.ReturnType, returnType);

                        //file.WriteLine("{0}\t[behaviac.MethodMetaInfo(\"{1}\", \"{2}\")]", indent, method.DisplayName, method.BasicDescription);
                        ExportMethodComment(file, "\t" + indent);

                        file.WriteLine("{0}\t{1}{2}{3} {4}({5})", indent, publicStr, staticStr, returnType, method.BasicName, allParams);
                        file.WriteLine("{0}\t{{", indent);

                        if (!preview)
                        {
                            ExportBeginComment(file, "\t\t" + indent, method.BasicName);
                        }

                        if (returnValue != null)
                        {
                            //file.WriteLine();
                            file.WriteLine("{0}\t\treturn {1};", indent, returnValue);
                        }

                        if (!preview)
                        {
                            ExportEndComment(file, "\t\t" + indent);
                        }

                        file.WriteLine("{0}\t}}", indent);
                        file.WriteLine();
                    }
                }

                if (!preview)
                {
                    ExportBeginComment(file, indent + "\t", Behaviac.Design.Exporters.Exporter.class_part);
                    file.WriteLine();
                    ExportEndComment(file, indent + "\t");
                    file.WriteLine();
                }

                file.WriteLine("{0}}}", indent);

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
                    file.WriteLine("}");
                }

                file.WriteLine();

                if (!preview)
                {
                    ExportBeginComment(file, indent, Behaviac.Design.Exporters.Exporter.file_uninit_part);
                    file.WriteLine();
                    ExportEndComment(file, indent);
                }

                file.WriteLine();

                UpdateFile(file, filename);
            }
        }

        private void ExportCustomizedTypes(string agentFolder)
        {
            if (TypeManager.Instance.HasNonImplementedTypes())
            {
                using (StringWriter file = new StringWriter())
                {
                    file.WriteLine("// ---------------------------------------------------------------------");
                    file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                    file.WriteLine("// ---------------------------------------------------------------------");
                    file.WriteLine();
                    file.WriteLine("using System;");
                    file.WriteLine("using System.Collections;");
                    file.WriteLine("using System.Collections.Generic;");
                    file.WriteLine();

                    if (TypeManager.Instance.HasNonImplementedEnums())
                    {
                        file.WriteLine("// -------------------");
                        file.WriteLine("// Customized enums");
                        file.WriteLine("// -------------------");

                        for (int e = 0; e < TypeManager.Instance.Enums.Count; ++e)
                        {
                            EnumType enumType = TypeManager.Instance.Enums[e];

                            if (enumType.IsImplemented)
                            {
                                continue;
                            }

                            ExportEnumFile(file, enumType, null);
                        }

                        file.WriteLine();
                    }

                    if (TypeManager.Instance.HasNonImplementedStructs())
                    {
                        file.WriteLine("// -------------------");
                        file.WriteLine("// Customized structs");
                        file.WriteLine("// -------------------");

                        Dictionary<string, bool> baseClasses = new Dictionary<string, bool>();

                        for (int s = 0; s < TypeManager.Instance.Structs.Count; s++)
                        {
                            StructType structType = TypeManager.Instance.Structs[s];

                            if (!string.IsNullOrEmpty(structType.BaseName))
                            {
                                baseClasses[structType.BaseName] = true;
                                baseClasses[structType.Name] = true;
                            }
                        }

                        for (int s = 0; s < TypeManager.Instance.Structs.Count; s++)
                        {
                            StructType structType = TypeManager.Instance.Structs[s];

                            if (structType.IsImplemented)
                            {
                                continue;
                            }

                            ExportStructFile(file, baseClasses, structType, null);
                        }
                    }

                    string filename = Path.Combine(agentFolder, "customizedtypes.cs");

                    UpdateFile(file, filename);
                }
            }
        }

        private void ExportEnumFile(StringWriter file, EnumType enumType, string filename)
        {
            StringWriter enumfile = file;
            bool hasSetExportLocation = (!string.IsNullOrEmpty(filename) || !string.IsNullOrEmpty(enumType.ExportLocation));

            if (hasSetExportLocation)
            {
                enumfile = new StringWriter();

                enumfile.WriteLine("// ---------------------------------------------------------------------");
                enumfile.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                enumfile.WriteLine("// ---------------------------------------------------------------------");
            }

            Debug.Check(enumfile != null);

            if (enumfile != null)
            {
                enumfile.WriteLine();

                string indent = "";

                if (!string.IsNullOrEmpty(enumType.Namespace))
                {
                    indent = "\t";

                    enumfile.WriteLine("namespace {0}", enumType.Namespace.Replace("::", "."));
                    enumfile.WriteLine("{");
                }

                //enumfile.WriteLine("{0}[behaviac.TypeMetaInfo(\"{1}\", \"{2}\")]", indent, enumType.DisplayName, enumType.Description);
                enumfile.WriteLine("{0}public enum {1}", indent, enumType.Name);
                enumfile.WriteLine("{0}{{", indent);

                for (int m = 0; m < enumType.Members.Count; ++m)
                {
                    EnumType.EnumMemberType member = enumType.Members[m];
                    //if (member.DisplayName != member.Name || !string.IsNullOrEmpty(member.Description))
                    //    enumfile.WriteLine("{0}\t[behaviac.MemberMetaInfo(\"{1}\", \"{2}\")]", indent, member.DisplayName, member.Description);

                    if (member.Value >= 0)
                    {
                        enumfile.WriteLine("{0}\t{1} = {2},", indent, member.Name, member.Value);
                    }
                    else
                    {
                        enumfile.WriteLine("{0}\t{1},", indent, member.Name);
                    }
                }

                enumfile.WriteLine("{0}}}", indent);

                if (!string.IsNullOrEmpty(enumType.Namespace))
                {
                    enumfile.WriteLine("}");
                }

                if (hasSetExportLocation)
                {
                    string enumFilename = filename;

                    if (string.IsNullOrEmpty(enumFilename))
                    {
                        string enumLocation = Workspace.Current.MakeAbsolutePath(enumType.ExportLocation);

                        if (!Directory.Exists(enumLocation))
                        {
                            Directory.CreateDirectory(enumLocation);
                        }

                        enumFilename = Path.Combine(enumLocation, enumType.Name + ".cs");
                    }

                    UpdateFile(enumfile, enumFilename);
                }
            }
        }

        private void ExportStructFile(StringWriter file, Dictionary<string, bool> baseClasses, StructType structType, string filename)
        {
            StringWriter structfile = file;
            bool hasSetExportLocation = (!string.IsNullOrEmpty(filename) || !string.IsNullOrEmpty(structType.ExportLocation));

            if (hasSetExportLocation)
            {
                structfile = new StringWriter();

                structfile.WriteLine("// ---------------------------------------------------------------------");
                structfile.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                structfile.WriteLine("// ---------------------------------------------------------------------");
                structfile.WriteLine();
                structfile.WriteLine("using System;");
                structfile.WriteLine("using System.Collections;");
                structfile.WriteLine("using System.Collections.Generic;");
            }

            structfile.WriteLine();

            string indent = "";

            if (!string.IsNullOrEmpty(structType.Namespace))
            {
                indent = "\t";

                structfile.WriteLine("namespace {0}", structType.Namespace.Replace("::", "."));
                structfile.WriteLine("{");
            }

            //structfile.WriteLine("{0}[behaviac.TypeMetaInfo(\"{1}\", \"{2}\")]", indent, structType.DisplayName, structType.Description);

            string structStr = "struct";

            if (baseClasses != null && baseClasses.ContainsKey(structType.Name))
            {
                structStr = "class";
            }

            if (string.IsNullOrEmpty(structType.BaseName))
            {
                structfile.WriteLine("{0}public {1} {2}", indent, structStr, structType.Name);
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
                        baseName = string.Format("{0}.{1}", baseStruct.Namespace.Replace("::", "."), baseName);
                    }
                }

                structfile.WriteLine("{0}public {1} {2} : {3}", indent, structStr, structType.Name, baseName);
            }

            structfile.WriteLine("{0}{{", indent);

            for (int m = 0; m < structType.Properties.Count; ++m)
            {
                PropertyDef member = structType.Properties[m];
                if (!member.IsInherited)
                {
                    //if (member.DisplayName != member.Name || !string.IsNullOrEmpty(member.BasicDescription))
                    //    structfile.WriteLine("{0}\t[behaviac.MemberMetaInfo(\"{1}\", \"{2}\")]", indent, member.DisplayName, member.BasicDescription);

                    structfile.WriteLine("{0}\tpublic {1} {2};", indent, DataCsExporter.GetGeneratedNativeType(member.NativeType), member.BasicName);
                }
            }

            structfile.WriteLine("{0}}}", indent);

            if (!string.IsNullOrEmpty(structType.Namespace))
            {
                structfile.WriteLine("}");
            }

            if (hasSetExportLocation)
            {
                string structFilename = filename;

                if (string.IsNullOrEmpty(structFilename))
                {
                    string structLocation = Workspace.Current.MakeAbsolutePath(structType.ExportLocation);

                    if (!Directory.Exists(structLocation))
                    {
                        Directory.CreateDirectory(structLocation);
                    }

                    structFilename = Path.Combine(structLocation, structType.Name + ".cs");
                }

                UpdateFile(structfile, structFilename);
            }
        }

        private void ExportMembers(string agentFolder)
        {
            using (StringWriter file = new StringWriter())
            {
                file.WriteLine("// ---------------------------------------------------------------------");
                file.WriteLine("// THIS FILE IS AUTO-GENERATED BY BEHAVIAC DESIGNER, SO PLEASE DON'T MODIFY IT BY YOURSELF!");
                file.WriteLine("// ---------------------------------------------------------------------\n");

                file.WriteLine("using System;");
                file.WriteLine("using System.Collections;");
                file.WriteLine("using System.Collections.Generic;");
                file.WriteLine();

                file.WriteLine("namespace behaviac");
                file.WriteLine("{");

                ExportTypeCompare(file);
                file.WriteLine();

                // AgentMeta class
                file.WriteLine("\tpublic class BehaviorLoaderImplement : BehaviorLoader");
                file.WriteLine("\t{");

                PreExportMeta(file);
                file.WriteLine();

                // Load method
                file.WriteLine("\t\tpublic override bool Load()");
                file.WriteLine("\t\t{");

                ExportMeta(file);

                file.WriteLine();

                foreach (AgentType agentType in Plugin.AgentTypes)
                {
                    if (agentType != null)
                    {
                        bool isStatic = agentType.IsStatic;

                        if (!isStatic)
                        {
                            file.WriteLine("\t\t\tAgentMeta.Register<{0}>(\"{0}\");", agentType.Name.Replace("::", "."));
                        }
                    }
                }

                foreach (EnumType enumType in TypeManager.Instance.Enums)
                {
                    string enumFullname = enumType.Fullname.Replace("::", ".");

                    file.WriteLine("\t\t\tAgentMeta.Register<{0}>(\"{0}\");", enumFullname);
                    file.WriteLine("\t\t\tComparerRegister.RegisterType<{0}, CompareValue_{1}>();", enumFullname, enumFullname.Replace(".", "_"));
                }

                foreach (StructType structType in TypeManager.Instance.Structs)
                {
                    string structFullname = structType.Fullname.Replace("::", ".");

                    if (structFullname != "System.Object")
                    {
                        file.WriteLine("\t\t\tAgentMeta.Register<{0}>(\"{0}\");", structFullname);
                        file.WriteLine("\t\t\tComparerRegister.RegisterType<{0}, CompareValue_{1}>();", structFullname, structFullname.Replace(".", "_"));
                    }
                }

                if (Plugin.InstanceNames.Count > 0)
                {
                    file.WriteLine();

                    foreach (Plugin.InstanceName_t instance in Plugin.InstanceNames)
                    {
                        AgentType agentType = Plugin.GetAgentType(instance.ClassName);
                        if (agentType != null)
                        {
                            bool isStatic = agentType.IsStatic;

                            if (!isStatic)
                            {
                                file.WriteLine("\t\t\tAgent.RegisterInstanceName<{0}>(\"{1}\");", instance.ClassName.Replace("::", "."), instance.Name);
                            }
                        }
                    }
                }

                if (Workspace.Current.UseIntValue)
                {
                    file.WriteLine();
                    file.WriteLine("\t\t\tbehaviac.Workspace.Instance.UseIntValue = true;");
                }

                file.WriteLine("\t\t\treturn true;");

                file.WriteLine("\t\t}");
                file.WriteLine();

                // UnLoad method
                file.WriteLine("\t\tpublic override bool UnLoad()");
                file.WriteLine("\t\t{");

                foreach (AgentType agentType in Plugin.AgentTypes)
                {
                    if (agentType != null)
                    {
                        bool isStatic = agentType.IsStatic;

                        if (!isStatic)
                        {
                            file.WriteLine("\t\t\tAgentMeta.UnRegister<{0}>(\"{0}\");", agentType.Name.Replace("::", "."));
                        }
                    }
                }

                foreach (EnumType enumType in TypeManager.Instance.Enums)
                {
                    string enumFullname = enumType.Fullname.Replace("::", ".");

                    file.WriteLine("\t\t\tAgentMeta.UnRegister<{0}>(\"{0}\");", enumFullname);
                }

                foreach (StructType structType in TypeManager.Instance.Structs)
                {
                    string structFullname = structType.Fullname.Replace("::", ".");

                    if (structFullname != "System.Object")
                    {
                        file.WriteLine("\t\t\tAgentMeta.UnRegister<{0}>(\"{0}\");", structFullname);
                    }
                }

                if (Plugin.InstanceNames.Count > 0)
                {
                    file.WriteLine();

                    foreach (Plugin.InstanceName_t instance in Plugin.InstanceNames)
                    {
                        AgentType agentType = Plugin.GetAgentType(instance.ClassName);
                        if (agentType != null)
                        {
                            bool isStatic = agentType.IsStatic;

                            if (!isStatic)
                            {
                                file.WriteLine("\t\t\tAgent.UnRegisterInstanceName<{0}>(\"{1}\");", instance.ClassName.Replace("::", "."), instance.Name);
                            }
                        }
                    }
                }

                file.WriteLine("\t\t\treturn true;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t}");
                file.WriteLine("}");

                string filename = Path.Combine(agentFolder, "AgentProperties.cs");

                UpdateFile(file, filename);
            }
        }

        private void ExportTypeCompare(StringWriter file)
        {
            foreach (EnumType enumType in TypeManager.Instance.Enums)
            {
                string enumFullname = enumType.Fullname.Replace("::", ".");

                file.WriteLine("\tpublic class CompareValue_{0} : ICompareValue<{1}>", enumFullname.Replace(".", "_"), enumFullname);
                file.WriteLine("\t{");
                file.WriteLine("\t\tpublic override bool Equal({0} lhs, {0} rhs)", enumFullname);
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn lhs == rhs;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t\tpublic override bool NotEqual({0} lhs, {0} rhs)", enumFullname);
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn lhs != rhs;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t\tpublic override bool Greater({0} lhs, {0} rhs)", enumFullname);
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn lhs > rhs;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t\tpublic override bool GreaterEqual({0} lhs, {0} rhs)", enumFullname);
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn lhs >= rhs;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t\tpublic override bool Less({0} lhs, {0} rhs)", enumFullname);
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn lhs < rhs;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t\tpublic override bool LessEqual({0} lhs, {0} rhs)", enumFullname);
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn lhs <= rhs;");
                file.WriteLine("\t\t}");
                file.WriteLine("\t}");
                file.WriteLine();
            }

            foreach (StructType structType in TypeManager.Instance.Structs)
            {
                string structFullname = structType.Fullname.Replace("::", ".");

                if (structFullname == "System.Object")
                {
                    continue;
                }

                file.WriteLine("\tpublic class CompareValue_{0} : ICompareValue<{1}>", structFullname.Replace(".", "_"), structFullname);
                file.WriteLine("\t{");
                file.WriteLine("\t\tpublic override bool Equal({0} lhs, {0} rhs)", structFullname);
                file.WriteLine("\t\t{");

                string compareStr = "";

                foreach (PropertyDef member in structType.Properties)
                {
                    if (member.IsStatic || member.IsReadonly)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(compareStr))
                    {
                        compareStr += " && ";
                    }

                    if (member.Type != null && (member.Type.IsValueType || Plugin.IsStringType(member.Type)))
                    {
                        compareStr += string.Format("(lhs.{0} == rhs.{0})", member.BasicName);
                    }
                    else
                    {
                        string nativeMemberType = CsExporter.GetGeneratedNativeType(member.NativeType);
                        compareStr += string.Format("OperationUtils.Compare<{0}>(lhs.{1}, rhs.{1}, EOperatorType.E_EQUAL)", nativeMemberType.Replace("::", "."), member.BasicName);
                    }
                }

                if (string.IsNullOrEmpty(compareStr))
                {
                    file.WriteLine("\t\t\treturn lhs == rhs;");
                }
                else
                {
                    file.WriteLine("\t\t\treturn {0};", compareStr);
                }

                file.WriteLine("\t\t}");
                file.WriteLine("\t\tpublic override bool NotEqual({0} lhs, {0} rhs)", structFullname);
                file.WriteLine("\t\t{");
                file.WriteLine("\t\t\treturn !Equal(lhs, rhs);");
                file.WriteLine("\t\t}");
                file.WriteLine("\t}");
                file.WriteLine();
            }
        }

        private bool IsStructType(MethodDef.Param param)
        {
            string paramType = DataCsExporter.GetGeneratedNativeType(param.NativeType);
            bool isStruct = Plugin.IsCustomClassType(param.Type);

            if (isStruct)
            {
                StructType structType = TypeManager.Instance.FindStruct(paramType);
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
                structTypeName = structTypeName.Replace(".", "_");

                // class
                file.WriteLine("\t\tclass CInstanceConst_{0} : CInstanceConst<{1}>", structTypeName, structType.Fullname.Replace("::", "."));
                file.WriteLine("\t\t{");

                foreach (PropertyDef prop in structType.Properties)
                {
                    if (!prop.IsReadonly)
                    {
                        file.WriteLine("\t\t\tIInstanceMember _{0};", prop.BasicName);
                    }
                }

                file.WriteLine();

                // Constructors
                file.WriteLine("\t\t\tpublic CInstanceConst_{0}(string typeName, string valueStr) : base(typeName, valueStr)", structTypeName);
                file.WriteLine("\t\t\t{");

                file.WriteLine("\t\t\t\tList<string> paramStrs = behaviac.StringUtils.SplitTokensForStruct(valueStr);");
                
                int validPropCount = 0;
                foreach (PropertyDef prop in structType.Properties)
                {
                    if (!prop.IsReadonly)
                    {
                        validPropCount++;
                    }
                }

                file.WriteLine("\t\t\t\tDebug.Check(paramStrs != null && paramStrs.Count == {0});", validPropCount);
                file.WriteLine();

                validPropCount = 0;
                foreach (PropertyDef prop in structType.Properties)
                {
                    if (!prop.IsReadonly)
                    {
                        string propType = DataCsExporter.GetGeneratedNativeType(prop.NativeType);

                        file.WriteLine("\t\t\t\t_{0} = (CInstanceMember<{1}>)AgentMeta.ParseProperty<{1}>(paramStrs[{2}]);", prop.BasicName, propType, validPropCount);

                        validPropCount++;
                    }
                }

                file.WriteLine("\t\t\t}");
                file.WriteLine();

                // Run()
                file.WriteLine("\t\t\tpublic override void Run(Agent self)");
                file.WriteLine("\t\t\t{");

                if (structType.Properties.Count > 0)
                {
                    foreach (PropertyDef prop in structType.Properties)
                    {
                        if (!prop.IsReadonly)
                        {
                            file.WriteLine("\t\t\t\tDebug.Check(_{0} != null);", prop.BasicName);
                        }
                    }

                    file.WriteLine();
                }

                foreach (PropertyDef prop in structType.Properties)
                {
                    if (!prop.IsReadonly)
                    {
                        string propType = DataCsExporter.GetGeneratedNativeType(prop.NativeType);

                        if (Plugin.IsRefType(prop.Type))
                        {
                            file.WriteLine("\t\t\t\t_value.{0} = ({1})_{0}.GetValueObject(self);", prop.BasicName, propType);
                        }
                        else
                        {
                            file.WriteLine("\t\t\t\t_value.{0} = ((CInstanceMember<{1}>)_{0}).GetValue(self);", prop.BasicName, propType);
                        }
                    }
                }

                file.WriteLine("\t\t\t}"); // Run()

                file.WriteLine("\t\t};"); // end of class
                file.WriteLine();
            }

            // all methods
            Dictionary<string, bool> allMethods = new Dictionary<string, bool>();

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                string agentTypeName = agent.Name.Replace("::", ".");

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

                            string methodReturnType = DataCsExporter.GetGeneratedNativeType(method.NativeReturnType);
                            string baseClass = (methodReturnType == "void") ? "CAgentMethodVoidBase" : string.Format("CAgentMethodBase<{0}>", methodReturnType);

                            // class
                            file.WriteLine("\t\tprivate class CMethod_{0} : {1}", methodFullname, baseClass);
                            file.WriteLine("\t\t{");

                            foreach (MethodDef.Param param in method.Params)
                            {
                                if (Plugin.IsRefType(param.Type))
                                {
                                    file.WriteLine("\t\t\tIInstanceMember _{0};", param.Name);
                                }
                                else
                                {
                                    string paramType = DataCsExporter.GetGeneratedNativeType(param.NativeType);
                                    file.WriteLine("\t\t\tCInstanceMember<{0}> _{1};", paramType, param.Name);
                                }
                            }

                            if (method.Params.Count > 0)
                            {
                                file.WriteLine();
                            }

                            // Constructors
                            file.WriteLine("\t\t\tpublic CMethod_{0}()", methodFullname);
                            file.WriteLine("\t\t\t{");
                            file.WriteLine("\t\t\t}");
                            file.WriteLine();

                            file.WriteLine("\t\t\tpublic CMethod_{0}(CMethod_{0} rhs) : base(rhs)", methodFullname);
                            file.WriteLine("\t\t\t{");
                            file.WriteLine("\t\t\t}");
                            file.WriteLine();

                            // Clone()
                            file.WriteLine("\t\t\tpublic override IMethod Clone()");
                            file.WriteLine("\t\t\t{");
                            file.WriteLine("\t\t\t\treturn new CMethod_{0}(this);", methodFullname);
                            file.WriteLine("\t\t\t}"); // Clone()
                            file.WriteLine();

                            // Load()
                            file.WriteLine("\t\t\tpublic override void Load(string instance, string[] paramStrs)");
                            file.WriteLine("\t\t\t{");

                            file.WriteLine("\t\t\t\tDebug.Check(paramStrs.Length == {0});", method.Params.Count);
                            file.WriteLine();
                            file.WriteLine("\t\t\t\t_instance = instance;");

                            for (int i = 0; i < method.Params.Count; ++i)
                            {
                                MethodDef.Param param = method.Params[i];
                                string paramType = DataCsExporter.GetGeneratedNativeType(param.NativeType);

                                if (IsStructType(param))
                                {
                                    file.WriteLine("\t\t\t\tif (paramStrs[{0}].StartsWith(\"{{\"))", i);
                                    file.WriteLine("\t\t\t\t{");
                                    file.WriteLine("\t\t\t\t\t_{0} = new CInstanceConst_{1}(\"{2}\", paramStrs[{3}]);", param.Name, paramType.Replace(".", "_"), paramType, i);
                                    file.WriteLine("\t\t\t\t}");
                                    file.WriteLine("\t\t\t\telse");
                                    file.WriteLine("\t\t\t\t{");
                                    file.WriteLine("\t\t\t\t\t_{0} = (CInstanceMember<{1}>)AgentMeta.ParseProperty<{1}>(paramStrs[{2}]);", param.Name, paramType, i);
                                    file.WriteLine("\t\t\t\t}");
                                }
                                else
                                {
                                    if (Plugin.IsRefType(param.Type))
                                    {
                                        file.WriteLine("\t\t\t\t_{0} = AgentMeta.ParseProperty<{1}>(paramStrs[{2}]);", param.Name, paramType, i);
                                    }
                                    else
                                    {
                                        file.WriteLine("\t\t\t\t_{0} = (CInstanceMember<{1}>)AgentMeta.ParseProperty<{1}>(paramStrs[{2}]);", param.Name, paramType, i);
                                    }
                                }
                            }

                            file.WriteLine("\t\t\t}"); // Load()
                            file.WriteLine();

                            // Run()
                            file.WriteLine("\t\t\tpublic override void Run(Agent self)");
                            file.WriteLine("\t\t\t{");

                            if (method.Params.Count > 0)
                            {
                                foreach (MethodDef.Param param in method.Params)
                                {
                                    file.WriteLine("\t\t\t\tDebug.Check(_{0} != null);", param.Name);
                                }

                                file.WriteLine();
                            }

                            string paramValues = "";

                            foreach (MethodDef.Param param in method.Params)
                            {
                                if (IsStructType(param))
                                {
                                    file.WriteLine("\t\t\t\t_{0}.Run(self);", param.Name);
                                }

                                if (!string.IsNullOrEmpty(paramValues))
                                {
                                    paramValues += ", ";
                                }

                                string paramType = DataCsExporter.GetGeneratedNativeType(param.NativeType);
                                string paramName = string.Format("((CInstanceMember<{0}>)_{1}).GetValue(self)", paramType, param.Name);

                                if (Plugin.IsRefType(param.Type))
                                {
                                    paramName = string.Format("({0})_{1}.GetValueObject(self)", paramType, param.Name);
                                }

                                if (param.IsRef || param.IsOut)
                                {
                                    file.WriteLine("\t\t\t\t{0} {1} = {2};", paramType, param.Name, paramName);

                                    if (method.IsPublic)
                                    {
                                        paramValues += param.IsRef ? "ref " : "out ";
                                    }

                                    paramValues += param.Name;
                                }
                                else
                                {
                                    paramValues += paramName;
                                }
                            }

                            if (!method.IsStatic)
                            {
                                file.WriteLine("\t\t\t\tAgent agent = Utils.GetParentAgent(self, _instance);");
                                file.WriteLine();
                            }

                            string instanceName = method.IsStatic ? agentTypeName : string.Format("(({0})agent)", agentTypeName);

                            if (methodReturnType == "void")
                            {
                                if (method.IsPublic)
                                {
                                    file.WriteLine("\t\t\t\t{0}.{1}({2});", instanceName, method.BasicName, paramValues);
                                }
                                else
                                {
                                    file.WriteLine("\t\t\t\tobject[] paramArray = new object[] {{ {0} }};", paramValues);
                                    file.WriteLine("\t\t\t\tAgentMetaVisitor.ExecuteMethod({0}, \"{1}\", paramArray);", instanceName, method.BasicName);
                                }
                            }
                            else
                            {
                                if (method.IsPublic)
                                {
                                    file.WriteLine("\t\t\t\t_returnValue.value = {0}.{1}({2});", instanceName, method.BasicName, paramValues);
                                }
                                else
                                {
                                    file.WriteLine("\t\t\t\tobject[] paramArray = new object[] {{ {0} }};", paramValues);
                                    file.WriteLine("\t\t\t\t_returnValue.value = ({0})AgentMetaVisitor.ExecuteMethod({1}, \"{2}\", paramArray);", methodReturnType, instanceName, method.BasicName);
                                }
                            }

                            for (int i = 0; i < method.Params.Count; ++i)
                            {
                                MethodDef.Param param = method.Params[i];

                                if (param.IsRef || param.IsOut)
                                {
                                    if (method.IsPublic)
                                    {
                                        file.WriteLine("\t\t\t\t_{0}.SetValue(self, {0});", param.Name);
                                    }
                                    else
                                    {
                                        string paramType = DataCsExporter.GetGeneratedNativeType(param.NativeType);
                                        file.WriteLine("\t\t\t\t_{0}.SetValue(self, ({1})paramArray[{2}]);", param.Name, paramType, i);
                                    }
                                }
                            }

                            file.WriteLine("\t\t\t}"); // Run()

                            if (method.Params.Count == 1 && methodReturnType == "behaviac.EBTStatus")
                            {
                                // GetIValue()
                                string paramType = DataCsExporter.GetGeneratedNativeType(method.Params[0].NativeType);

                                file.WriteLine();
                                file.WriteLine("\t\t\tpublic override IValue GetIValue(Agent self, IInstanceMember firstParam)");
                                file.WriteLine("\t\t\t{");
                                file.WriteLine("\t\t\t\tAgent agent = Utils.GetParentAgent(self, _instance);");
                                file.WriteLine();
                                file.WriteLine("\t\t\t\t{0} result = ((CInstanceMember<{0}>)firstParam).GetValue(self);", paramType);

                                MethodDef.Param param = method.Params[0];

                                if (method.IsPublic)
                                {
                                    string refStr = "";

                                    if (param.IsRef || param.IsOut)
                                    {
                                        refStr = param.IsRef ? "ref " : "out ";
                                    }

                                    file.WriteLine("\t\t\t\t_returnValue.value = {0}.{1}({2}result);", instanceName, method.BasicName, refStr);
                                }
                                else
                                {
                                    file.WriteLine("\t\t\t\tobject[] paramArray = new object[] { result };");
                                    file.WriteLine("\t\t\t\t_returnValue.value = ({0})AgentMetaVisitor.ExecuteMethod({1}, \"{2}\", paramArray);", methodReturnType, instanceName, method.BasicName);
                                }

                                if (param.IsRef || param.IsOut)
                                {
                                    if (method.IsPublic)
                                    {
                                        file.WriteLine("\t\t\t\tfirstParam.SetValue(self, result);");
                                    }
                                    else
                                    {
                                        string paramNativeType = DataCsExporter.GetGeneratedNativeType(param.NativeType);
                                        file.WriteLine("\t\t\t\tfirstParam.SetValue(self, ({0})paramArray[0]);", paramNativeType);
                                    }
                                }

                                file.WriteLine("\t\t\t\treturn _returnValue;");
                                file.WriteLine("\t\t\t}");
                            }

                            file.WriteLine("\t\t}"); // class
                            file.WriteLine();
                        }
                    }
                }
            }
        }

        private void ExportMeta(StringWriter file)
        {
            file.WriteLine("\t\t\tAgentMeta.TotalSignature = {0};", CRC32.CalcCRC(Plugin.Signature));
            file.WriteLine();
            file.WriteLine("\t\t\tAgentMeta meta;");

            foreach (AgentType agent in Plugin.AgentTypes)
            {
                string agentTypeName = agent.Name.Replace("::", ".");
                string signature = agent.GetSignature(true);

                file.WriteLine("\n\t\t\t// {0}", agentTypeName);
                file.WriteLine("\t\t\tmeta = new AgentMeta({0});", CRC32.CalcCRC(signature));
                file.WriteLine("\t\t\tAgentMeta._AgentMetas_[{0}] = meta;", CRC32.CalcCRC(agentTypeName));

                IList<PropertyDef> properties = agent.GetProperties(true);

                foreach (PropertyDef prop in properties)
                {
                    if (!prop.IsPar)
                    {
                        string bindingProperty = "";
                        string registerName = "RegisterMemberProperty";
                        string propType = DataCsExporter.GetGeneratedNativeType(prop.NativeItemType);

                        string propItemName = prop.BasicName;

                        if (prop.IsArrayElement)
                        {
                            propItemName = propItemName.Replace("[]", "[index]");
                        }

                        string instanceName = string.Format("(({0})self)", agentTypeName);

                        if (agent.IsStatic || prop.IsStatic)
                        {
                            instanceName = agentTypeName;
                        }

                        string setValue = "";
                        string getValue = "";

                        if (prop.IsPublic)
                        {
                            setValue = string.Format("{0}.{1} = value;", instanceName, propItemName);
                            getValue = string.Format("{0}.{1}", instanceName, propItemName);
                        }
                        else
                        {
                            if (prop.IsArrayElement)
                            {
                                setValue = string.Format("{0}._get_{1}()[index] = value;", instanceName, prop.BasicName.Replace("[]", ""));
                                getValue = string.Format("{0}._get_{1}()[index]", instanceName, prop.BasicName.Replace("[]", ""));
                            }
                            else
                            {
                                setValue = string.Format("{0}._set_{1}(value);", instanceName, propItemName);
                                getValue = string.Format("{0}._get_{1}()", instanceName, propItemName);
                            }
                        }

                        if (prop.IsReadonly)
                        {
                            setValue = "";
                        }

                        bool isMemberProp = prop.IsMember || agent.IsCustomized;

                        if (agent.IsStatic || isMemberProp && prop.IsStatic)
                        {
                            if (isMemberProp)
                            {
                                if (prop.IsArrayElement)
                                {
                                    bindingProperty = string.Format("new CStaticMemberArrayItemProperty<{0}>(\"{1}\", delegate({0} value, int index) {{ {2} }}, delegate(int index) {{ return {3}; }})",
                                                                    propType, prop.BasicName, setValue, getValue);
                                }
                                else
                                {
                                    bindingProperty = string.Format("new CStaticMemberProperty<{0}>(\"{1}\", delegate({0} value) {{ {2} }}, delegate() {{ return {3}; }})",
                                                                    propType, prop.BasicName, setValue, getValue);
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
                                    if (prop.IsArrayElement)
                                    {
                                        bindingProperty = string.Format("new CMemberArrayItemProperty<{0}>(\"{1}\", delegate(Agent self, {0} value, int index) {{ {2} }}, delegate(Agent self, int index) {{ return {3}; }})",
                                                                        propType, prop.BasicName, setValue, getValue);
                                    }
                                    else
                                    {
                                        bindingProperty = string.Format("new CMemberProperty<{0}>(\"{1}\", delegate(Agent self, {0} value) {{ {2} }}, delegate(Agent self) {{ return {3}; }})",
                                                                        propType, prop.BasicName, setValue, getValue);
                                    }
                                }
                            }
                            else
                            {
                                registerName = prop.IsStatic ? "RegisterStaticCustomizedProperty" : "RegisterCustomizedProperty";

                                if (prop.IsArrayElement)
                                {
                                    string propName = prop.BasicName.Replace("[]", "");
                                    bindingProperty = string.Format("new CCustomizedArrayItemProperty<{0}>({1}, \"{2}\")",
                                                                    propType, CRC32.CalcCRC(propName), propName);
                                }
                                else
                                {
                                    bindingProperty = string.Format("new CCustomizedProperty<{0}>({1}, \"{2}\", \"{3}\")",
                                                                    propType, CRC32.CalcCRC(prop.BasicName), prop.BasicName, prop.DefaultValue);
                                }
                            }
                        }

                        file.WriteLine("\t\t\tmeta.{0}({1}, {2});", registerName, CRC32.CalcCRC(prop.BasicName), bindingProperty);
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

                            string paramType = DataCsExporter.GetGeneratedNativeType(param.NativeType);
                            paramTypes += paramType;
                            paramTypeValues += ", " + paramType + " " + param.Name;
                            paramValues += param.Name;
                        }
                    }

                    string methodReturnType = DataCsExporter.GetGeneratedNativeType(method.NativeReturnType);

                    if (method.IsNamedEvent)
                    {
                        if (!string.IsNullOrEmpty(paramTypes))
                        {
                            paramTypes = string.Format("<{0}>", paramTypes);
                        }

                        agentMethod = string.Format("new CAgentMethodVoid{0}(delegate(Agent self{1}) {{ }}) /* {2} */", paramTypes, paramTypeValues, method.BasicName);

                        file.WriteLine("\t\t\tmeta.RegisterMethod({0}, {1});", CRC32.CalcCRC(method.BasicName), agentMethod);
                    }
                    else
                    {
                        if (hasRefParam)
                        {
                            string methodFullname = method.Name.Replace("::", "_");
                            agentMethod = string.Format("new CMethod_{0}()", methodFullname);
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

                                    agentMethod = string.Format("new CAgentStaticMethodVoid{0}(delegate({1}) {{ {2}.{3}({4}); }})",
                                                                paramTypes, paramTypeValues, agentTypeName, method.BasicName, paramValues);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = ", " + paramTypes;
                                    }

                                    agentMethod = string.Format("new CAgentStaticMethod<{0}{1}>(delegate({2}) {{ return {3}.{4}({5}); }})",
                                                                methodReturnType, paramTypes, paramTypeValues, agentTypeName, method.BasicName, paramValues);
                                }
                            }
                            else
                            {
                                string methodStr = "";

                                if (method.IsPublic)
                                {
                                    methodStr = string.Format("(({0})self).{1}({2})", agentTypeName, method.BasicName, paramValues);
                                }
                                else
                                {
                                    paramValues = string.IsNullOrEmpty(paramValues) ? "null" : string.Format("new object[]{{ {0} }}", paramValues);
                                    methodStr = string.Format("AgentMetaVisitor.ExecuteMethod(self, \"{0}\", {1})", method.BasicName, paramValues);
                                }

                                if (methodReturnType == "void")
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = string.Format("<{0}>", paramTypes);
                                    }

                                    agentMethod = string.Format("new CAgentMethodVoid{0}(delegate(Agent self{1}) {{ {2}; }})",
                                                                paramTypes, paramTypeValues, methodStr);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(paramTypes))
                                    {
                                        paramTypes = ", " + paramTypes;
                                    }

                                    if (!method.IsPublic)
                                    {
                                        methodStr = string.Format("({0}){1}", methodReturnType, methodStr);
                                    }

                                    agentMethod = string.Format("new CAgentMethod<{0}{1}>(delegate(Agent self{2}) {{ return {3}; }})",
                                                                methodReturnType, paramTypes, paramTypeValues, methodStr);
                                }
                            }
                        }

                        file.WriteLine("\t\t\tmeta.RegisterMethod({0}, {1});", CRC32.CalcCRC(method.BasicName), agentMethod);
                    }
                }
            }
        }
    }
}
