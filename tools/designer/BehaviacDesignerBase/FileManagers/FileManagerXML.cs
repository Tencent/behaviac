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
using System.Xml;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using System.Reflection;
using Behaviac.Design.Properties;

namespace Behaviac.Design.FileManagers
{
    /// <summary>
    /// This is the default file manager which saves and loads XML files.
    /// </summary>
    public class FileManagerXML : FileManager
    {
        protected XmlDocument _xmlfile = new XmlDocument();

        /// <summary>
        /// Creates a new XML file manager to load and save behaviours.
        /// </summary>
        /// <param name="filename">The file we want to load from or save to.</param>
        /// <param name="node">The node we want to save. When loading, use null.</param>
        public FileManagerXML(string filename, BehaviorNode node)
        : base(filename, node)
        {
        }

        public override object Clone()
        {
            return new FileManagerXML(this.Filename, this.Behavior);
        }

        /// <summary>
        /// Retrieves an attribute from a XML node. If the attribute does not exist an exception is thrown.
        /// </summary>
        /// <param name="node">The XML node we want to get the attribute from.</param>
        /// <param name="att">The name of the attribute we want.</param>
        /// <returns>Returns the attributes value. Is always valid.</returns>
        protected string GetAttribute(XmlNode node, string att)
        {
            XmlNode value = node.Attributes.GetNamedItem(att);

            // maintain compatibility to version 1
            if (value == null)
            {
                value = node.Attributes.GetNamedItem(att.ToLowerInvariant());
            }

            if (value != null && value.NodeType == XmlNodeType.Attribute)
            {
                return value.Value;
            }

            throw new Exception(string.Format(Resources.ExceptionFileManagerXMLMissingAttribute, att));
        }

        /// <summary>
        /// Retrieves an attribute from a XML node. Attribute does not need to exist.
        /// </summary>
        /// <param name="node">The XML node we want to get the attribute from.</param>
        /// <param name="att">The name of the attribute we want.</param>
        /// <param name="result">The value of the attribute found. Is string.Empty if the attribute does not exist.</param>
        /// <returns>Returns true if the attribute exists.</returns>
        protected bool GetAttribute(XmlNode node, string att, out string result)
        {
            XmlNode value = node.Attributes.GetNamedItem(att);

            if (value != null && value.NodeType == XmlNodeType.Attribute)
            {
                result = value.Value;
                return true;
            }

            result = string.Empty;
            return false;
        }

        /// <summary>
        /// Initialises a property on a given node.
        /// </summary>
        /// <param name="xml">The XML element containing the attribute we want to get.</param>
        /// <param name="node">The node whose property we want to set.</param>
        /// <param name="property">The property on the node we want to set.</param>
        protected void InitProperty(List<Nodes.Node.ErrorCheck> result, XmlNode xml, Node node, DesignerPropertyInfo property)
        {
            string value;

            if (GetAttribute(xml, property.Property.Name, out value))
            {
                property.SetValueFromString(result, node, value);
            }
        }

        /// <summary>
        /// Initialises a property on a given attachment.
        /// </summary>
        /// <param name="xml">The XML element containing the attribute we want to get.</param>
        /// <param name="node">The attachment whose property we want to set.</param>
        /// <param name="property">The property on the attachment we want to set.</param>
        protected void InitProperty(List<Nodes.Node.ErrorCheck> result, XmlNode xml, Attachments.Attachment attach, DesignerPropertyInfo property)
        {
            string value;

            if (GetAttribute(xml, property.Property.Name, out value))
            {
                property.SetValueFromString(result, attach, value);
            }
        }

        /// <summary>
        /// Loads an attachment which is attached to a node.
        /// </summary>
        /// <param name="node">The node the attachment is created for.</param>
        /// <param name="xml">The XML node the attachment retrieves its name and attributes from.</param>
        /// <returns>Returns the created Attachment.</returns>
        protected Attachments.Attachment CreateAttachment(List<Nodes.Node.ErrorCheck> result, Node node, XmlNode xml)
        {
            try
            {
                // get the type of the attachment and create it

                // maintain compatibility with version 1
                //string clss= GetAttribute(xml, "Class");
                string clss;

                if (!GetAttribute(xml, "Class", out clss))
                {
                    string find = ".Events." + GetAttribute(xml, "name");
                    Type found = Plugin.FindType(find);

                    if (found != null)
                    {
                        clss = found.FullName;
                    }
                }

                Type t = Plugin.GetType(clss);

                if (t == null)
                {
                    throw new Exception(string.Format(Resources.ExceptionUnknownEventType, clss));
                }

                Attachments.Attachment attach = Attachments.Attachment.Create(t, node);

                // initialise the attachments properties
                IList<DesignerPropertyInfo> properties = attach.GetDesignerProperties();

                for (int p = 0; p < properties.Count; ++p)
                {
                    if (properties[p].Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                    {
                        continue;
                    }

                    InitProperty(result, xml, attach, properties[p]);
                }

                // update attacheent with attributes
                attach.OnPropertyValueChanged(false);

                return attach;

            }
            catch (Exception ex)
            {
                string msgError = string.Format("{0}\n{1}:\n{2} Of Node {3}", ex.Message, this.Filename, "Attachment", node.Id);

                //throw new Exception(msg);
                MessageBox.Show(msgError, Resources.LoadError, MessageBoxButtons.OK);
            }

            return null;
        }

        /// <summary>
        /// Loads a node from a given XML node.
        /// </summary>
        /// <param name="getBehaviorNode">The callback used to return the behavior.</param>
        /// <param name="xml">The XML node we want to create the node from.</param>
        /// <param name="parent">The parent this node will be added to.</param>
        /// <param name="connector">The connector used to add this node to the parent.</param>
        /// <returns>Returns the created node.</returns>
        protected Node CreateNodeAndAdd(List<Nodes.Node.ErrorCheck> result, GetBehaviorNode getBehaviorNode, XmlNode xml, Node parent, Node.Connector connector)
        {
            try
            {
                // get the type of the node and create it
                string clss = GetAttribute(xml, "Class");
                Type t = Plugin.GetType(clss);

                if (t == null)
                {
                    string msg = string.Format(Resources.ExceptionUnknownNodeType, clss);

                    string msgError = string.Format("{0}:\n{1}", this.Filename, msg);

                    //throw new Exception(msg);
                    MessageBox.Show(msgError, Resources.LoadError, MessageBoxButtons.OK);

                    parent.Behavior.TriggerWasModified(parent);

                    return null;
                }

                Node node = Nodes.Node.Create(t);

                if (parent == null)
                {
                    //if this._version == 0, it means there is no Version attribute in the file
                    node.Behavior.Version = this._version;
                }

                // update the loaded behaviour member
                if (node is BehaviorNode)
                {
                    ((BehaviorNode)node).FileManager = this;
                }

                // add the node to the parent
                if (parent != null)
                {
                    if (connector != null)
                    {
                        parent.AddChildNotModified(connector, node);
                    }

                    else
                    {
                        parent.AddFSMNode(node);
                    }
                }

                int version = 0;

                if (node.Behavior != null)
                {
                    version = node.Behavior.Version;

                }
                else
                {
                    Debug.Check(true);
                }

                SetEnterExitSlot(result, xml, node, version);

                // initialise the properties
                IList<DesignerPropertyInfo> properties = node.GetDesignerProperties();

                foreach (DesignerPropertyInfo property in properties)
                {
                    if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                    {
                        InitProperty(result, xml, node, property);
                    }
                }

                // return the created behaviour node
                if (node is BehaviorNode)
                {
                    getBehaviorNode((BehaviorNode)node);
                }

                // maintain compatibility with version 1
                if (node is ReferencedBehavior)
                {
                    ReferencedBehavior refbehavior = (ReferencedBehavior)node;

                    if (refbehavior.Behavior.Version <= 3)
                    {
                        string refTree = null;

                        if (GetAttribute(xml, "ReferenceFilename", out refTree) && !string.IsNullOrEmpty(refTree))
                        {
                            refbehavior.SetReferenceBehavior(refTree);
                        }
                    }
                }

                // update node with properties
                node.OnPropertyValueChanged(false);

                // load child objects
                foreach (XmlNode xnode in xml.ChildNodes)
                {
                    if (xnode.NodeType == XmlNodeType.Element)
                    {
                        switch (xnode.Name)
                        {
                                // load parameters
                            case ("Parameters"):
                                if (node is Behavior || node is ReferencedBehavior)
                                {
                                    LoadParameters(result, xnode, node, node.LocalVars);
                                    node.Behavior.PostLoadPars();
                                }

                                break;

                            case ("DescriptorRefs"):
#if QUERY_EANBLED
                                LoadDescriptorRefs(result, xnode, node.Behavior as Behavior);
#endif//#if QUERY_EANBLED
                                break;

                                // maintain compatibility with version 1
                            case ("Node"):
                                CreateNodeAndAdd(result, getBehaviorNode, xnode, node, node.GetConnector(BaseNode.Connector.kGeneric));
                                break;

                                // maintain compatibility with version 2.1
                            case ("Event"):
                            case ("Attachment"):
                            {
                                Attachments.Attachment a = CreateAttachment(result, node, xnode);

                                if (a != null)
                                {
                                    node.AddAttachment(a);

                                    a.PostCreate(result, version, node, xnode);
                                }
                            }
                            break;

                            case ("Comment"):
                                // create a comment object
                                node.CommentText = "temp";

                                // initialise the attributes
                                properties = node.CommentObject.GetDesignerProperties();

                                foreach (DesignerPropertyInfo property in properties)
                                {
                                    if (property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                                    {
                                        continue;
                                    }

                                    string value;

                                    if (GetAttribute(xnode, property.Property.Name, out value))
                                    {
                                        property.SetValueFromString(result, node.CommentObject, value);
                                    }
                                }

                                break;

                            case ("Connector"):
                                string identifier = GetAttribute(xnode, "Identifier");
                                Nodes.Node.Connector conn = node.GetConnector(identifier);

                                foreach (XmlNode connected in xnode.ChildNodes)
                                {
                                    if (connected.NodeType == XmlNodeType.Element && connected.Name == "Node")
                                    {
                                        CreateNodeAndAdd(result, getBehaviorNode, connected, node, conn);
                                    }
                                }

                                break;

                            case ("FSMNodes"):
                                string locationX = GetAttribute(xnode, "ScreenLocationX");
                                string locationY = GetAttribute(xnode, "ScreenLocationY");

                                if (!string.IsNullOrEmpty(locationX) && !string.IsNullOrEmpty(locationY))
                                {
                                    try
                                    {
                                        float x = float.Parse(locationX);
                                        float y = float.Parse(locationY);
                                        node.ScreenLocation = new System.Drawing.PointF(x, y);

                                    }
                                    catch
                                    {
                                    }
                                }

                                foreach (XmlNode fsmNode in xnode.ChildNodes)
                                {
                                    if (fsmNode.NodeType == XmlNodeType.Element && fsmNode.Name == "Node")
                                    {
                                        CreateNodeAndAdd(result, getBehaviorNode, fsmNode, node, null);
                                    }
                                }

                                break;
                        }
                    }
                }

                // update attachments with attributes
                foreach (Attachments.Attachment attach in node.Attachments)
                {
                    attach.OnPropertyValueChanged(false);
                }

                // set the properties from its prefab
                bool isPrefabInstance = !string.IsNullOrEmpty(node.PrefabName) && !node.HasOwnPrefabData;

                if (isPrefabInstance)
                {
                    Node prefabNode = Plugin.GetPrefabNode(node);

                    if (prefabNode != null)
                    {
                        node.ResetByPrefab(node.PrefabName, prefabNode);

                        Behavior b = node.Behavior as Behavior;

                        if (b.AgentType != null)
                        {
                            b.AgentType.ResetPars(b.LocalVars);
                        }
                    }
                }

                node.PostCreate(result, version, xml);

                return node;

            }
            catch (Exception e)
            {
                string idNode = xml.Attributes["Id"].Value;
                string msgError = string.Format("{0}:\r\nNode Id:{1}\r\n{2}", this.Filename, idNode, e.Message);

                //throw new Exception(msg);
                MessageBox.Show(msgError, Resources.LoadError, MessageBoxButtons.OK);
            }

            return null;
        }

        private void SetEnterExitSlot(List<Nodes.Node.ErrorCheck> result, XmlNode xml, Node node, int version)
        {
            if (version <= 1)
            {
                string enterActionStr = null;
                GetAttribute(xml, "EnterAction", out enterActionStr);

                if (!string.IsNullOrEmpty(enterActionStr))
                {
                    node.SetEnterExitSlot(result, enterActionStr, true);
                    node.Behavior.TriggerWasModified(node);
                }

                string exitActionStr = null;
                GetAttribute(xml, "ExitAction", out exitActionStr);

                if (!string.IsNullOrEmpty(exitActionStr))
                {
                    node.SetEnterExitSlot(result, exitActionStr, false);
                    node.Behavior.TriggerWasModified(node);
                }
            }
        }


        /// <summary>
        /// This method allows nodes to process the loaded attributes.
        /// </summary>
        /// <param name="processedBehaviors">The behaviours which have already been processed to avoid circular references.</param>
        /// <param name="node">The node which is processed.</param>
        protected void DoPostLoad(ProcessedBehaviors processedBehaviors, Node node)
        {
            if (processedBehaviors.MayProcess(node))
            {
                node.PostLoad(_behavior);

                foreach (Node child in node.Children)
                {
                    DoPostLoad(processedBehaviors.Branch(child), child);
                }
            }
        }

        private void LoadParameters(List<Nodes.Node.ErrorCheck> result, XmlNode xmlNode, Nodes.Node node, List<ParInfo> pars)
        {
            if (pars != null)
            {
                pars.Clear();
            }

            if (xmlNode != null)
            {
                foreach (XmlNode child in xmlNode.ChildNodes)
                {
                    try
                    {
                        if (child.Attributes["Name"] != null && child.Attributes["Type"] != null)
                        {
                            ParInfo par = new ParInfo(node, node.Behavior.AgentType);

                            par.Name = child.Attributes["Name"].Value;

                            if (string.IsNullOrEmpty(par.Name))
                            {
                                continue;
                            }

                            par.TypeName = Plugin.GetFullTypeName(child.Attributes["Type"].Value);

                            if (string.IsNullOrEmpty(par.TypeName))
                            {
                                continue;
                            }

                            if (par.TypeName == "Behaviac.Design.llong")
                            {
                                par.NativeType = "llong";
                            }
                            else if (par.TypeName == "Behaviac.Design.ullong")
                            {
                                par.NativeType = "ullong";
                            }

                            if (child.Attributes["EventParam"] != null)
                            {
                                par.EventParam = child.Attributes["EventParam"].Value;
                            }

                            if (child.Attributes["DisplayName"] != null)
                            {
                                par.DisplayName = child.Attributes["DisplayName"].Value;
                            }

                            if (child.Attributes["Desc"] != null)
                            {
                                par.BasicDescription = child.Attributes["Desc"].Value;
                            }

                            Type valueType = Plugin.GetType(par.TypeName);
                            string defaultValue = child.Attributes["DefaultValue"].Value;
                            par.Display = child.Attributes["Display"] != null ? child.Attributes["Display"].Value.ToLower() == "true" : true;

                            par.Variable = new VariableDef(null);
                            Plugin.InvokeTypeParser(result, valueType, defaultValue, (object value) => par.Variable.Value = value, node);

                            if (pars != null)
                            {
                                pars.Add(par);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Resources.LoadError, MessageBoxButtons.OK);
                    }
                }
            }
        }

#if QUERY_EANBLED
        private void LoadDescriptorRefs(List<Nodes.Node.ErrorCheck> result, XmlNode xmlNode, Behavior b)
        {
            b.DescriptorRefs.Clear();

            if (xmlNode != null)
            {
                Type type = b.DescriptorRefs.GetType();
                string valueString = xmlNode.Attributes["value"].Value;

                b.DescriptorRefs = (List<Behavior.DescriptorRef>)DesignerPropertyUtility.ParseStringValue(result, type, valueString, b);
            }
        }
#endif//#if QUERY_EANBLED

        private int _version = 0;
        /// <summary>
        /// Loads a behaviour from the given filename
        /// </summary>
        /// <param name="getBehaviorNode">The callback used to return the behavior.</param>
        public override void Load(List<Nodes.Node.ErrorCheck> result, GetBehaviorNode getBehaviorNode)
        {
            try
            {
                //FileStream fs = Plugin.ConcurrentSourceFileStreams.GetValueUnsafe(_filename);
                //if (fs == null)
                //{
                //    fs = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                //}
                FileStream fs = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read);

                _xmlfile.Load(fs);
                fs.Close();

                XmlNode rootNode = _xmlfile.ChildNodes[1];

                int versionLoaded = (rootNode.Attributes != null) ? int.Parse(rootNode.Attributes["Version"] != null ? rootNode.Attributes["Version"].Value : "0") : 0;
                this._version = versionLoaded;

                if (versionLoaded > Nodes.Behavior.NewVersion)
                {
                    MessageBox.Show(Resources.FileVersionWarning, Resources.LoadWarning, MessageBoxButtons.OK);
                }

                foreach (XmlNode xmlNode in rootNode.ChildNodes)
                {
                    if (xmlNode.Name == "Node")
                    {
                        _behavior = (BehaviorNode)CreateNodeAndAdd(result, getBehaviorNode, xmlNode, null, null);

                        ProcessedBehaviors processedBehaviors = new ProcessedBehaviors();
                        DoPostLoad(processedBehaviors, (Node)_behavior);

                        break;
                    }
                }

                if (_behavior != null)
                {
                    if (versionLoaded != Nodes.Behavior.NewVersion)
                    {
                        _behavior.Version = Nodes.Behavior.NewVersion;
                        _behavior.TriggerWasModified((Node)_behavior);
                    }

                    string noError = rootNode.Attributes["NoError"] != null ? rootNode.Attributes["NoError"].Value : "false";
                    _behavior.HasNoError = (noError == "true");
                }
            }
            catch (Exception e)
            {
                string errorInfo = string.Format("{0}\n{1}", _filename, e.Message);
                MessageBox.Show(errorInfo, Resources.LoadError, MessageBoxButtons.OK);

                _xmlfile.RemoveAll();

                throw;
            }
        }

        /// <summary>
        /// Saves a node to the XML file.
        /// </summary>
        /// <param name="root">The XML node we want to attach the node to.</param>
        /// <param name="node">The node we want to save.</param>
        protected void SaveNode(XmlElement root, Node node)
        {
            try
            {
                // allow the node to process its attributes in preparation of the save
                node.PreSave(_behavior);

                // store the class we have to create when loading
                XmlElement elem = _xmlfile.CreateElement("Node");
                elem.SetAttribute("Class", node.GetType().FullName);

                bool isPrefabInstance = !string.IsNullOrEmpty(node.PrefabName) && !node.IsPrefabDataDirty();

                // save attributes
                IList<DesignerPropertyInfo> properties = node.GetDesignerProperties();

                for (int p = 0; p < properties.Count; ++p)
                {
                    if (!properties[p].Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                    {
                        bool bDo = !isPrefabInstance || properties[p].Attribute.HasFlags(DesignerProperty.DesignerFlags.NotPrefabRelated);

                        if (bDo)
                        {
                            if (bDo)
                            {
                                elem.SetAttribute(properties[p].Property.Name, properties[p].GetSaveValue(node));
                            }
                        }
                    }
                }

                // append node to root
                root.AppendChild(elem);

                // save comment
                if (node.CommentObject != null)
                {
                    XmlElement comment = _xmlfile.CreateElement("Comment");

                    properties = node.CommentObject.GetDesignerProperties();

                    for (int p = 0; p < properties.Count; ++p)
                    {
                        if (!properties[p].Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                        {
                            comment.SetAttribute(properties[p].Property.Name, properties[p].GetSaveValue(node.CommentObject));
                        }
                    }

                    elem.AppendChild(comment);
                }

                if (!isPrefabInstance)
                {
                    // save parameters
                    if (node.LocalVars != null)
                    {
                        Debug.Check(node is Behavior || node is ReferencedBehavior) ;
                        SaveParameters(elem, node.LocalVars);
                    }

                    // save DescriptorRefs
                    if (node is Behavior)
                    {
#if QUERY_EANBLED
                        Behavior b = node as Behavior;
                        SaveDescriptorRefs(elem, b);
#endif//#if QUERY_EANBLED
                    }

                    // save attachments
                    foreach (Attachments.Attachment attach in node.Attachments)
                    {
                        XmlElement attelem = _xmlfile.CreateElement("Attachment");
                        attelem.SetAttribute("Class", attach.GetType().FullName);

                        // save attributes
                        properties = attach.GetDesignerProperties();

                        for (int p = 0; p < properties.Count; ++p)
                        {
                            if (!properties[p].Attribute.HasFlags(DesignerProperty.DesignerFlags.NoSave))
                            {
                                attelem.SetAttribute(properties[p].Property.Name, properties[p].GetSaveValue(attach));
                            }
                        }

                        if (attach.LocalVars != null && attach.LocalVars.Count > 0)
                        {
                            Debug.Check(attach is Attachments.Event);
                            SaveParameters(attelem, attach.LocalVars);
                        }

                        elem.AppendChild(attelem);
                    }

                    // save children if allowed. Disallowed for referenced behaviours.
                    if (node.SaveChildren)
                    {
                        // save connectors
                        foreach (Nodes.Node.Connector connector in node.Connectors)
                        {
                            // if we have no children to store we can skip the connector
                            if (connector.ChildCount < 1)
                            {
                                continue;
                            }

                            XmlElement conn = _xmlfile.CreateElement("Connector");
                            conn.SetAttribute("Identifier", connector.Identifier);
                            elem.AppendChild(conn);

                            // save their children
                            for (int i = 0; i < connector.ChildCount; ++i)
                            {
                                SaveNode(conn, (Node)connector.GetChild(i));
                            }
                        }

                        // save fsm nodes
                        if (node.FSMNodes.Count > 0)
                        {
                            XmlElement fsmNodesElement = _xmlfile.CreateElement("FSMNodes");
                            fsmNodesElement.SetAttribute("ScreenLocationX", node.ScreenLocation.X.ToString());
                            fsmNodesElement.SetAttribute("ScreenLocationY", node.ScreenLocation.Y.ToString());
                            elem.AppendChild(fsmNodesElement);

                            foreach (Nodes.Node fsmNode in node.FSMNodes)
                            {
                                SaveNode(fsmNodesElement, fsmNode);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.SaveError, MessageBoxButtons.OK);

                throw;
            }
        }

        private void SaveParameters(XmlElement root, List<ParInfo> pars)
        {
            if (pars.Count == 0)
            {
                return;
            }

            XmlElement elem = _xmlfile.CreateElement("Parameters");

            foreach (ParInfo p in pars)
            {
                XmlElement par = _xmlfile.CreateElement("Parameter");

                par.SetAttribute("Name", p.BasicName);
                par.SetAttribute("Type", p.TypeName);
                par.SetAttribute("DefaultValue", p.DefaultValue);

                if (!string.IsNullOrEmpty(p.EventParam))
                {
                    par.SetAttribute("EventParam", p.EventParam);
                }

                par.SetAttribute("DisplayName", p.DisplayName);
                par.SetAttribute("Desc", p.BasicDescription);
                par.SetAttribute("Display", p.Display ? "true" : "false");

                elem.AppendChild(par);
            }

            root.AppendChild(elem);
        }

#if QUERY_EANBLED
        private void SaveDescriptorRefs(XmlElement root, Behavior b)
        {
            XmlElement elem = _xmlfile.CreateElement("DescriptorRefs");

            string value = DesignerArray.RetrieveExportValue(b.DescriptorRefs);

            elem.SetAttribute("value", value);

            root.AppendChild(elem);
        }
#endif//#if QUERY_EANBLED

        /// <summary>
        /// Save the given behaviour to the given file.
        /// </summary>
        /// <returns>Returns the result when the behaviour is saved.</returns>
        public override SaveResult Save()
        {
            SaveResult result = MakeWritable(_filename, Resources.SaveFileWarning);

            if (SaveResult.Succeeded != result)
            {
                return result;
            }

            _xmlfile.RemoveAll();

            XmlDeclaration declaration = _xmlfile.CreateXmlDeclaration("1.0", "utf-8", null);
            _xmlfile.AppendChild(declaration);

            XmlElement root = _xmlfile.CreateElement("Behavior");
            _xmlfile.AppendChild(root);

            ///force to use new version when saved
            _behavior.Version = Nodes.Behavior.NewVersion;
            int verison = _behavior.Version;
            root.SetAttribute("Version", verison.ToString());

            if (_behavior.HasNoError)
            {
                root.SetAttribute("NoError", "true");
            }

            SaveNode(root, (Node)_behavior);

            try
            {
                _xmlfile.Save(_filename);

                // update modified status
                this.Behavior.TriggerWasSaved();

                return SaveResult.Succeeded;

            }
            catch (Exception ex)
            {
                _xmlfile.RemoveAll();

                string msgError = string.Format(Resources.SaveFileError, _filename, ex.Message);
                MessageBox.Show(msgError, Resources.SaveError, MessageBoxButtons.OK);
            }

            return SaveResult.Failed;
        }
    }
}
