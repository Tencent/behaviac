////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
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
    public class ExporterXml : Behaviac.Design.Exporters.Exporter
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

        public ExporterXml(BehaviorNode node, string outputFolder, string filename, List<string> includedFilenames = null)
        : base(node, outputFolder, filename + ".xml", includedFilenames)
        {
        }

        /// <summary>
        /// Exports a behaviour to the given file.
        /// </summary>
        /// <param name="file">The file we want to export to.</param>
        /// <param name="behavior">The behaviour we want to export.</param>
        protected void ExportBehavior(XmlWriter file, BehaviorNode behavior)
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

            file.WriteAttributeString("name", behaviorName);
            //file.WriteAttributeString("event", b.EventName);
            file.WriteAttributeString("agenttype", b.AgentType.Name);

            if (b.IsFSM)
            {
                file.WriteAttributeString("fsm", "true");
            }

            file.WriteAttributeString("version", b.Version.ToString());

            this.ExportProperties(file, b);

            this.ExportPars(file, b);

            if (!b.IsFSM)
            {
                this.ExportAttachments(file, b);
            }

#if QUERY_EANBLED
            //after ExportProperties as DescritorRefs are exported as property
            this.ExportDescritorRefs(file, b);
#endif//#if QUERY_EANBLED

            if (b.IsFSM)
            {
                file.WriteStartElement("node");

                file.WriteAttributeString("class", "FSM");
                file.WriteAttributeString("id", "-1");

                file.WriteStartElement("property");
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

        private void ExportPars(XmlWriter file, Behavior behavior)
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

        private void ExportPar(XmlWriter file, ParInfo par, bool bExportValue)
        {
            file.WriteStartElement("par");

            file.WriteAttributeString("name", par.BasicName);
            file.WriteAttributeString("type", par.NativeType);

            if (bExportValue)
            {
                file.WriteAttributeString("value", par.DefaultValue);
            }

            file.WriteEndElement();
        }

#if QUERY_EANBLED
        private void ExportDescritorRefs(XmlWriter file, Behavior b)
        {
            //if (Plugin.IsQueryFiltered)
            //{
            //    return;
            //}

            if (b.DescriptorRefs.Count > 0)
            {
                file.WriteStartElement("property");
                string propValue = DesignerArray.RetrieveExportValue(b.DescriptorRefs);
                file.WriteAttributeString("DescriptorRefs", propValue);
                file.WriteEndElement();
            }

            {
                string propValue = b.Domains;

                if (!string.IsNullOrEmpty(propValue))
                {
                    file.WriteStartElement("property");
                    file.WriteAttributeString("Domains", propValue);
                    file.WriteEndElement();
                }
            }
        }
#endif//#endif//#if QUERY_EANBLED

        /// <summary>
        /// Exports a node to the given file.
        /// </summary>
        /// <param name="file">The file we want to export to.</param>
        /// <param name="behavior">The behaviour we are currently exporting.</param>
        /// <param name="node">The node we want to export.</param>
        protected void ExportNode(XmlWriter file, BehaviorNode behavior, Node node)
        {
            if (!node.Enable)
            {
                return;
            }

            file.WriteStartElement("node");

            file.WriteAttributeString("class", node.ExportClass);
            file.WriteAttributeString("id", node.Id.ToString());

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
        private void ExportProperties(XmlWriter file, Node n)
        {
            IList<DesignerPropertyInfo> properties = n.GetDesignerProperties();

            foreach (DesignerPropertyInfo p in properties)
            {
                // we skip properties which are not marked to be exported
                if (p.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoExport))
                {
                    continue;
                }

                object v = p.Property.GetValue(n, null);
                bool bExport = !Plugin.IsExportArray(v);

                if (bExport)
                {

                    // create the code which assigns the value to the node's property
                    //file.Write(string.Format("{0}\t{1}.{2} = {3};\r\n", indent, nodeName, properties[p].Property.Name, properties[p].GetExportValue(node)));
                    string propValue = p.GetExportValue(n);

                    if (propValue != string.Empty && propValue != "\"\"")
                    {
                        file.WriteStartElement("property");
                        file.WriteAttributeString(p.Property.Name, propValue);
                        file.WriteEndElement();
                    }
                }
            }

            if (n is Task)
            {
                Task task = n as Task;
                file.WriteStartElement("property");
                file.WriteAttributeString("IsHTN", task.IsHTN ? "true" : "false");
                file.WriteEndElement();
            }
        }

        private void ExportProperties(XmlWriter file, Attachments.Attachment a)
        {
            DesignerPropertyInfo propertyEffector = new DesignerPropertyInfo();
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
                        // create the code which assigns the value to the node's property
                        //file.Write(string.Format("{0}\t{1}.{2} = {3};\r\n", indent, nodeName, properties[p].Property.Name, properties[p].GetExportValue(node)));
                        string propValue = p.GetExportValue(a);

                        if (propValue != string.Empty && propValue != "\"\"")
                        {
                            file.WriteStartElement("property");
                            file.WriteAttributeString(p.Property.Name, propValue);
                            file.WriteEndElement();
                        }
                    }
                }
            }

            if (propertyEffector.Property != null)
            {
                List<TransitionEffector> listV = (List<TransitionEffector>)propertyEffector.Property.GetValue(a, null);

                if (listV != null)
                {
                    foreach (TransitionEffector te in listV)
                    {
                        IList<DesignerPropertyInfo> effectorProperties = te.GetDesignerProperties();
                        file.WriteStartElement("attachment");

                        foreach (DesignerPropertyInfo p in effectorProperties)
                        {
                            // we skip properties which are not marked to be exported
                            if (p.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoExport))
                            {
                                continue;
                            }

                            string propValue = p.GetExportValue(te);

                            if (propValue != string.Empty && propValue != "\"\"")
                            {
                                file.WriteStartElement("property");
                                file.WriteAttributeString(p.Property.Name, propValue);
                                file.WriteEndElement();
                            }
                        }

                        file.WriteEndElement();
                    }
                }
            }
        }

        protected void ExportAttachments(XmlWriter file, Node node)
        {
            foreach (Attachments.Attachment a in node.Attachments)
            {
                if (!a.Enable)
                {
                    continue;
                }

                file.WriteStartElement("attachment");

                Type type = a.GetType();
                file.WriteAttributeString("class", a.ExportClass);
                file.WriteAttributeString("id", a.Id.ToString());

                string flagStr = "precondition";

                if (a.IsTransition)
                {
                    flagStr = "transition";

                }
                else if (a.IsPrecondition)
                {
                    Debug.Check(!a.IsEffector);
                    flagStr = "precondition";

                }
                else if (a.IsEffector)
                {
                    Debug.Check(!a.IsPrecondition);
                    flagStr = "effector";

                }
                else if (!a.IsPrecondition && !a.IsEffector)
                {
                    flagStr = "event";

                }
                else
                {
                    Debug.Check(false);
                }

                file.WriteAttributeString("flag", flagStr);

                this.ExportProperties(file, a);

                file.WriteEndElement();
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
        //        Plugin.ConcurrentXmlStreamWriters[filename] = new StreamWriter(filename);
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

            //StreamWriter file = null;
            //if (Plugin.ConcurrentProcessBehaviors)
            //{
            //    file = Plugin.ConcurrentXmlStreamWriters.GetValueUnsafe(filename);
            //}
            //if (file == null)
            //{
            //    file = new StreamWriter(filename);
            //}

            //XmlWriterSettings ws = new XmlWriterSettings();
            //ws.Indent = true;
            ////ws.OmitXmlDeclaration = true;

            //// export to the file
            //using (XmlWriter xmlWrtier = XmlWriter.Create(file, ws))
            //{
            //    xmlWrtier.WriteStartDocument();
            //    ExportBehavior(xmlWrtier, _node);
            //    xmlWrtier.WriteEndDocument();

            //    file.Close();
            //}

            // export to the file
            using (StreamWriter file = new StreamWriter(filename))
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;
                //ws.OmitXmlDeclaration = true;

                using (XmlWriter xmlWrtier = XmlWriter.Create(file, ws))
                {
                    xmlWrtier.WriteStartDocument();
                    ExportBehavior(xmlWrtier, _node);
                    xmlWrtier.WriteEndDocument();
                }

                file.Close();
            }

            return FileManagers.SaveResult.Succeeded;
        }
    }
}
