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
using System.Xml;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Attachments;

namespace Behaviac.Design.Exporters
{
    /// <summary>
    /// This exporter generates .cs files which generate a static variable which holds the behaviour tree.
    /// </summary>
    public class ExporterBson : Behaviac.Design.Exporters.Exporter
    {
        protected static string __usedNamespace = "Behaviac.Behaviors";

        /// <summary>
        /// The namespace the behaviours will be exported to.
        /// </summary>
        public static string UsedNamespace
        {
            get
            {
                return __usedNamespace;
            }
            set
            {
                __usedNamespace = value;
            }
        }

        public ExporterBson(BehaviorNode node, string outputFolder, string filename, List<string> includedFilenames = null)
        : base(node, outputFolder, filename + ".bson.bytes", includedFilenames)
        {
        }

        /// <summary>
        /// Exports a behaviour to the given file.
        /// </summary>
        /// <param name="file">The file we want to export to.</param>
        /// <param name="behavior">The behaviour we want to export.</param>
        protected void ExportBehavior(BsonSerializer file, BehaviorNode behavior)
        {
            if (behavior.FileManager == null)
            {
                return;
            }

            file.WriteComment("EXPORTED BY TOOL, DON'T MODIFY IT!");
            file.WriteComment("Source File: " + behavior.MakeRelative(behavior.FileManager.Filename));

            file.WriteStartElement("behavior");

            Behavior b = behavior as Behavior;
            Debug.Check(b != null);
            Debug.Check(b.Id == -1);

            //'\\' ->'/'
            string behaviorName = b.MakeRelative(b.Filename);
            behaviorName = behaviorName.Replace('\\', '/');
            int pos = behaviorName.IndexOf(".xml");

            if (pos != -1)
            {
                behaviorName = behaviorName.Remove(pos);
            }

            file.WriteString(behaviorName);
            file.WriteString(b.AgentType.Name);
            file.WriteBool(b.IsFSM);
            file.WriteString(b.Version.ToString());

            this.ExportProperties(file, b);

            this.ExportPars(file, b);

            if (!b.IsFSM)
            {
                this.ExportAttachments(file, b);
            }

            if (b.IsFSM)
            {
                file.WriteStartElement("node");

                file.WriteString("FSM");
                file.WriteString("-1");

                file.WriteStartElement("properties");
                file.WriteAttributeString("initialid", behavior.InitialStateId.ToString());
                file.WriteEndElement();

                foreach (Node child in((Node)behavior).FSMNodes)
                {
                    this.ExportNode(file, behavior, child);
                }

                file.WriteEndElement();
            }
            else
            {
                // export the children
                foreach (Node child in((Node)behavior).Children)
                {
                    this.ExportNode(file, behavior, child);
                }
            }

            file.WriteEndElement();
        }

        private void ExportPars(BsonSerializer file, Behavior behavior)
        {
            if (behavior.LocalVars.Count == 0)
            {
                return;
            }

            file.WriteStartElement("pars");

            for (int i = 0; i < behavior.LocalVars.Count; ++i)
            {
                ParInfo par = behavior.LocalVars[i];

                ExportPar(file, par, true);
            }

            file.WriteEndElement();
        }

        private void ExportPar(BsonSerializer file, ParInfo par, bool bExportValue)
        {
            file.WriteStartElement("par");

            file.WriteString(par.BasicName);
            file.WriteString(par.NativeType);

            if (bExportValue)
            {
                file.WriteString(par.DefaultValue);
            }
            else
            {
                file.WriteString("");
            }

            file.WriteEndElement();
        }

#if QUERY_EANBLED
        private void ExportDescritorRefs(BsonSerializer file, Behavior b)
        {
            //if (Plugin.IsQueryFiltered)
            //{
            //    return;
            //}

            if (b.DescriptorRefs.Count > 0)
            {
                string propValue = DesignerArray.RetrieveExportValue(b.DescriptorRefs);
                file.WriteAttributeString("DescriptorRefs", propValue);
            }

            string propValue2 = b.Domains;

            if (!string.IsNullOrEmpty(propValue2))
            {
                file.WriteAttributeString("Domains", propValue2);
            }
        }
#endif//#endif//#if QUERY_EANBLED

        /// <summary>
        /// Exports a node to the given file.
        /// </summary>
        /// <param name="file">The file we want to export to.</param>
        /// <param name="behavior">The behaviour we are currently exporting.</param>
        /// <param name="node">The node we want to export.</param>
        protected void ExportNode(BsonSerializer file, BehaviorNode behavior, Node node)
        {
            if (!node.Enable)
            {
                return;
            }

            file.WriteStartElement("node");

            file.WriteString(node.ExportClass);
            file.WriteString(node.Id.ToString());

            {
                // export the properties
                this.ExportProperties(file, node);

                this.ExportAttachments(file, node);

                if (!node.IsFSM && !(node is ReferencedBehavior))
                {
                    // export the child nodes
                    foreach (Node child in node.Children)
                    {
                        if (!node.GetConnector(child).IsAsChild)
                        {
                            file.WriteStartElement("custom");
                            this.ExportNode(file, behavior, child);
                            file.WriteEndElement();
                        }
                        else
                        {
                            this.ExportNode(file, behavior, child);
                        }
                    }
                }
            }

            file.WriteEndElement();
        }

        /// <summary>
        /// Exports all the properties of a ode and assigns them.
        /// </summary>
        /// <param name="file">The file we are exporting to.</param>
        /// <param name="nodeName">The name of the node we are setting the properties for.</param>
        /// <param name="node">The node whose properties we are exporting.</param>
        /// <param name="indent">The indent for the currently generated code.</param>
        private void ExportProperties(BsonSerializer file, Node n)
        {
            IList<DesignerPropertyInfo> properties = n.GetDesignerProperties();

            file.WriteStartElement("properties");

            foreach (DesignerPropertyInfo p in properties)
            {
                // we skip properties which are not marked to be exported
                if (p.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoExport))
                {
                    continue;
                }

                object v = p.Property.GetValue(n, null);
                bool bExport = !Plugin.IsExportArray(v); ;

                if (bExport)
                {

                    // create the code which assigns the value to the node's property
                    //file.Write(string.Format("{0}\t{1}.{2} = {3};\r\n", indent, nodeName, properties[p].Property.Name, properties[p].GetExportValue(node)));
                    string propValue = p.GetExportValue(n);

                    if (propValue != string.Empty && propValue != "\"\"")
                    {
                        WriteProperty(file, p, n);
                    }
                }
            }

            if (n is Task)
            {
                Task task = n as Task;

                file.WriteAttributeString("IsHTN", task.IsHTN ? "true" : "false");
            }

#if QUERY_EANBLED
            Behavior b = n as Behavior;

            if (b != null)
            {
                this.ExportDescritorRefs(file, b);
            }

#endif

            file.WriteEndElement();
        }

        private void ExportProperties(BsonSerializer file, Attachments.Attachment a)
        {
            DesignerPropertyInfo propertyEffector = new DesignerPropertyInfo();
            file.WriteStartElement("properties");
            IList<DesignerPropertyInfo> properties = a.GetDesignerProperties(true);

            foreach (DesignerPropertyInfo p in properties)
            {
                // we skip properties which are not marked to be exported
                if (p.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoExport))
                {
                    continue;
                }

                object v = p.Property.GetValue(a, null);
                bool bExport = !Plugin.IsExportArray(v);

                if (bExport)
                {
                    if (p.Property.Name == "Effectors")
                    {
                        propertyEffector = p;

                    }
                    else
                    {
                        WriteProperty(file, p, a);
                    }
                }
            }

            file.WriteEndElement();

            if (propertyEffector.Property != null)
            {
                List<TransitionEffector> listV = (List<TransitionEffector>)propertyEffector.Property.GetValue(a, null);

                if (listV != null)
                {
                    file.WriteStartElement("attachments");

                    foreach (TransitionEffector te in listV)
                    {
                        file.WriteStartElement("attachment");
                        file.WriteStartElement("properties");
                        IList<DesignerPropertyInfo> effectorProperties = te.GetDesignerProperties();

                        foreach (DesignerPropertyInfo p in effectorProperties)
                        {
                            // we skip properties which are not marked to be exported
                            if (p.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoExport))
                            {
                                continue;
                            }

                            WriteProperty(file, p, te);
                        }

                        file.WriteEndElement();
                        file.WriteEndElement();
                    }

                    file.WriteEndElement();
                }
            }
        }


        protected void ExportAttachments(BsonSerializer file, Node node)
        {
            //int localVars = ReferencedBehaviorLocalVars(node);
            int localVars = 0;

            if (node.Attachments.Count > 0 || localVars > 0)
            {
                file.WriteStartElement("attachments");

                //this.ExportReferencedBehaviorLocalVars(node, file);

                foreach (Attachments.Attachment a in node.Attachments)
                {
                    if (!a.Enable)
                    {
                        continue;
                    }

                    file.WriteStartElement("attachment");

                    Type type = a.GetType();

                    file.WriteString(a.ExportClass);
                    file.WriteString(a.Id.ToString());
                    file.WriteBool(a.IsPrecondition);
                    bool bIsEffector = a.IsEffector;

                    if (a.IsTransition)
                    {
                        bIsEffector = false;
                    }

                    file.WriteBool(bIsEffector);
                    file.WriteBool(a.IsTransition);

                    this.ExportProperties(file, a);

                    //this.ExportEventLocalVars(a, file);

                    file.WriteEndElement();
                }

                file.WriteEndElement();
            }
        }

        static private void WritePar(BsonSerializer file, string valueName, ParInfo par)
        {
            //WriteParValue(file, valueName, par);
            WriteParString(file, valueName, par);
        }

        static private void WriteParString(BsonSerializer file, string valueName, ParInfo par)
        {
            string parStr = par.DefaultValue;
            //file.WriteAttribute(valueName, parStr);
            file.WriteString(parStr);
        }

        static private void WriteProperty(BsonSerializer file, DesignerPropertyInfo property, object o)
        {
            //WritePropertyValue(file, property, o);
            WritePropertyString(file, property, o);
        }

        static private void WritePropertyString(BsonSerializer file, DesignerPropertyInfo property, object o)
        {
            string str = property.GetExportValue(o);

            file.WriteAttributeString(property.Property.Name, str);
        }

        static private void WritePropertyValue(BsonSerializer file, DesignerPropertyInfo property, object o)
        {
            string str = property.GetExportValue(o);
            string[] tokens = str.Split(' ');
            string valueString = null;

            if (tokens.Length == 3 && tokens[0] == "const")
            {
                valueString = tokens[2];

            }
            else if (tokens.Length == 1)
            {
                valueString = str;
            }

            bool bW = false;

            if (valueString != null)
            {
                object obj = property.Property.GetValue(o, null);

                object v = null;

                Type valueType = null;
                VariableDef varType = obj as VariableDef;

                if (varType != null)
                {
                    valueType = varType.ValueType;

                }
                else
                {
                    RightValueDef rvarType = obj as RightValueDef;

                    if (rvarType != null)
                    {
                        if (rvarType.Method == null)
                        {
                            valueType = rvarType.ValueType;
                        }

                    }
                    else
                    {
                        MethodDef mType = obj as MethodDef;

                        if (mType != null)
                        {
                            Debug.Check(true);

                        }
                        else
                        {
                            valueType = obj.GetType();
                        }
                    }
                }

                if (valueType != null && Plugin.InvokeTypeParser(null, valueType, valueString, (object value) => v = value, null))
                {
                    file.WriteAttribute(property.Property.Name, v);
                    bW = true;
                }
            }

            if (!bW)
            {
                file.WriteAttributeString(property.Property.Name, str);
            }
        }

        //public override FileManagers.SaveResult InitWriter()
        //{
        //    Debug.Check(Plugin.ConcurrentProcessBehaviors);

        //    string filename = Path.Combine(_outputFolder, _filename);
        //    FileManagers.SaveResult result = FileManagers.FileManager.MakeWritable(filename, Resources.ExportFileWarning);

        //    if (FileManagers.SaveResult.Succeeded != result)
        //    {
        //        return result;
        //    }

        //    // get the abolute folder of the file we want toexport
        //    string folder = Path.GetDirectoryName(filename);

        //    if (!Directory.Exists(folder))
        //    {
        //        Directory.CreateDirectory(folder);
        //    }

        //    try
        //    {
        //        Plugin.ConcurrentBsonFileStreams[filename] = File.Create(filename);
        //    }
        //    catch
        //    {
        //        Debug.Check(false, filename);

        //        return FileManagers.SaveResult.Failed;
        //    }

        //    return FileManagers.SaveResult.Succeeded;
        //}

        /// <summary>
        /// Export the assigned node to the assigned file.
        /// </summary>
        /// <returns>Returns the result when the behaviour is exported.</returns>
        public override FileManagers.SaveResult Export()
        {
            string filename = Path.Combine(_outputFolder, _filename);
            FileManagers.SaveResult result = FileManagers.FileManager.MakeWritable(filename, Resources.ExportFileWarning);

            if (FileManagers.SaveResult.Succeeded != result)
            {
                return result;
            }

            // get the abolute folder of the file we want toexport
            string folder = Path.GetDirectoryName(filename);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using(var ms = new MemoryStream())
            using(var writer = new BinaryWriter(ms))
            {
                //BsonSerializer.Serialize(writer, _node);
                BsonSerializer serializer = BsonSerializer.CreateSerialize(writer);
                serializer.WriteStartDocument();
                ExportBehavior(serializer, _node);
                serializer.WriteEndDocument();

                FileStream fs = null;
                //if (Plugin.ConcurrentProcessBehaviors)
                //{
                //    fs = Plugin.ConcurrentBsonFileStreams.GetValueUnsafe(filename);
                //}
                if (fs == null)
                {
                    fs = File.Create(filename);
                }

                // export to the file
                using (BinaryWriter w = new BinaryWriter(fs))
                {
                    byte[] d = ms.ToArray();

                    w.Write(d);

                    fs.Close();
                }
            }

            return FileManagers.SaveResult.Succeeded;
        }
    }
}
