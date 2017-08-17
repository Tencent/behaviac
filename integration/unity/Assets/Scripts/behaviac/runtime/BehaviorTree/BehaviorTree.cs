/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions and limitations under
// the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define LITTLE_ENDIAN_ONLY
#define USE_STRING_COUNT_HEAD

using System;
using System.Collections.Generic;
using System.IO;

#if BEHAVIAC_USE_SYSTEM_XML
using System.Xml;
#else
using System.Security;
using MiniXml;
#endif

namespace behaviac
{
    public struct property_t
    {
        public string name;
        public string value;

        public property_t(string n, string v)
        {
            name = n;
            value = v;
        }
    }

    //bson deserizer
    public class BsonDeserizer
    {
        public enum BsonTypes
        {
            BT_None = 0,
            BT_Double = 1,
            BT_String = 2,
            BT_Object = 3,
            BT_Array = 4,
            BT_Binary = 5,
            BT_Undefined = 6,
            BT_ObjectId = 7,
            BT_Boolean = 8,
            BT_DateTime = 9,
            BT_Null = 10,
            BT_Regex = 11,
            BT_Reference = 12,
            BT_Code = 13,
            BT_Symbol = 14,
            BT_ScopedCode = 15,
            BT_Int32 = 16,
            BT_Timestamp = 17,
            BT_Int64 = 18,
            BT_Float = 19,
            BT_Element = 20,
            BT_Set = 21,
            BT_BehaviorElement = 22,
            BT_PropertiesElement = 23,
            BT_ParsElement = 24,
            BT_ParElement = 25,
            BT_NodeElement = 26,
            BT_AttachmentsElement = 27,
            BT_AttachmentElement = 28,
            BT_AgentsElement = 29,
            BT_AgentElement = 30,
            BT_PropertyElement = 31,
            BT_MethodsElement = 32,
            BT_MethodElement = 33,
            BT_Custom = 34,
            BT_ParameterElement = 35
        }

        public bool Init(byte[] pBuffer)
        {
            try
            {
                m_pBuffer = pBuffer;

                if (m_pBuffer != null && m_pBuffer.Length > 0)
                {
                    m_BinaryReader = new BinaryReader(new MemoryStream(m_pBuffer));

                    if (this.OpenDocument())
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Check(false, e.Message);
            }

            Debug.Check(false);
            return false;
        }

        private int GetCurrentIndex()
        {
            Debug.Check(this.m_BinaryReader != null);
            return (int)this.m_BinaryReader.BaseStream.Position;
        }

        public bool OpenDocument()
        {
            int head = this.GetCurrentIndex();
            int size = this.ReadInt32();
            int end = head + size - 1;

            if (this.m_pBuffer[end] == 0)
            {
                return true;
            }
            else
            {
                Debug.Check(false);
                return false;
            }
        }

        //if ReadType has been called as a 'peek', use CloseDocumente(false)
        //usually, after a loop, use CloseDocumente(false) as that loop usually terminates with e peek ReadType
        public void CloseDocument(bool bEatEod /*= false*/)
        {
            int endLast = this.GetCurrentIndex();

            if (bEatEod)
            {
                this.m_BinaryReader.BaseStream.Position++;
            }
            else
            {
                endLast--;
            }

            Debug.Check(this.m_pBuffer[endLast] == 0);
        }

        public BsonTypes ReadType()
        {
            byte b = m_BinaryReader.ReadByte();

            return (BsonTypes)b;
        }

        public int ReadInt32()
        {
            int i = m_BinaryReader.ReadInt32();

#if LITTLE_ENDIAN_ONLY
            Debug.Check(BitConverter.IsLittleEndian);
            return i;
#else

            if (BitConverter.IsLittleEndian)
            {
                return i;
            }
            else
            {
                byte[] bytes = BitConverter.GetBytes(i);
                i = (bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3]);

                return i;
            }

#endif//LITTLE_ENDIAN_ONLY
        }

        public UInt16 ReadUInt16()
        {
            ushort us = m_BinaryReader.ReadUInt16();

#if LITTLE_ENDIAN_ONLY
            Debug.Check(BitConverter.IsLittleEndian);
            return us;
#else

            if (BitConverter.IsLittleEndian)
            {
                return us;
            }
            else
            {
                byte[] bytes = BitConverter.GetBytes(us);
                us = (ushort)(bytes[0] << 8 | bytes[1]);

                return us;
            }

#endif//LITTLE_ENDIAN_ONLY
        }

        public float ReadFloat()
        {
            float f = m_BinaryReader.ReadSingle();

#if LITTLE_ENDIAN_ONLY
            Debug.Check(BitConverter.IsLittleEndian);
            return f;
#else

            if (BitConverter.IsLittleEndian)
            {
                return f;
            }
            else
            {
                byte[] bytes = BitConverter.GetBytes(f);
                Array.Reverse(bytes);
                f = BitConverter.ToSingle(bytes, 0);

                return f;
            }

#endif//LITTLE_ENDIAN_ONLY
        }

        public bool ReadBool()
        {
            byte b = m_BinaryReader.ReadByte();

            return (b != 0) ? true : false;
        }

        public string ReadString()
        {
#if USE_STRING_COUNT_HEAD
            UInt16 count = ReadUInt16();
            byte[] bytes = m_BinaryReader.ReadBytes(count);

            // The exporter uses UTF8 to export strings, so the same encoding type is used here.
            string str = System.Text.Encoding.UTF8.GetString(bytes, 0, count - 1);

            Debug.Check(this.m_pBuffer[this.GetCurrentIndex() - 1] == 0);
            return str;
#else
            List<byte> bytes = new List<byte>();

            while (true)
            {
                byte b = m_BinaryReader.ReadByte();

                if (b == 0)
                {
                    break;
                }

                bytes.Add(b);
            }

            // The exporter uses UTF8 to export strings, so the same encoding type is used here.
            string str = System.Text.Encoding.UTF8.GetString(bytes.ToArray());

            return str;
#endif
        }

        public bool eod()
        {
            byte c = this.m_pBuffer[this.GetCurrentIndex()];
            return (c == 0);
        }

        private byte[] m_pBuffer = null;
        private BinaryReader m_BinaryReader = null;
    }

    /**
    * Base class for BehaviorTree Nodes. This is the static part
    */

    public abstract class BehaviorNode
    {
#if BEHAVIAC_USE_HTN
        public virtual bool decompose(BehaviorNode node, PlannerTaskComplex seqTask, int depth, Planner planner)
        {
            Debug.Check(false, "Can't step into this line");
            return false;
        }
#endif//

        public BehaviorTask CreateAndInitTask()
        {
            BehaviorTask pTask = this.createTask();

            Debug.Check(pTask != null);
            pTask.Init(this);

            return pTask;
        }

        public bool HasEvents()
        {
            return this.m_bHasEvents;
        }

        public void SetHasEvents(bool hasEvents)
        {
            this.m_bHasEvents = hasEvents;
        }

        public int GetChildrenCount()
        {
            if (this.m_children != null)
            {
                return this.m_children.Count;
            }

            return 0;
        }

        public BehaviorNode GetChild(int index)
        {
            if (this.m_children != null && index < this.m_children.Count)
            {
                return this.m_children[index];
            }

            return null;
        }

        public BehaviorNode GetChildById(int nodeId)
        {
            if (this.m_children != null && this.m_children.Count > 0)
            {
                for (int i = 0; i < this.m_children.Count; ++i)
                {
                    BehaviorNode c = this.m_children[i];

                    if (c.GetId() == nodeId)
                    {
                        return c;
                    }
                }
            }

            return null;
        }

        protected BehaviorNode()
        {
        }

        //~BehaviorNode()
        //{
        //    this.Clear();
        //}

        public void Clear()
        {
            if (this.m_events != null)
            {
                this.m_events.Clear();
                this.m_events = null;
            }

            if (this.m_preconditions != null)
            {
                this.m_preconditions.Clear();
                this.m_preconditions = null;
            }

            if (this.m_effectors != null)
            {
                this.m_effectors.Clear();
                this.m_effectors = null;
            }

            if (this.m_children != null)
            {
                this.m_children.Clear();
                this.m_children = null;
            }
        }

        public virtual bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
#if !BEHAVIAC_RELEASE
            Debug.Check(!string.IsNullOrEmpty(this.m_agentType));

            return Agent.IsDerived(pAgent, this.m_agentType);
#else
            return true;
#endif//#if !BEHAVIAC_RELEASE
        }

        //return true for Parallel, SelectorLoop, etc., which is responsible to update all its children just like sub trees
        //so that they are treated as a return-running node and the next update will continue them.
        public virtual bool IsManagingChildrenAsSubTrees()
        {
            return false;
        }

        #region Load

        protected static BehaviorNode Create(string className)
        {
            return Workspace.Instance.CreateBehaviorNode(className);
        }

        protected virtual void load(int version, string agentType, List<property_t> properties)
        {
            string nodeType = this.GetClassNameString().Replace(".", "::");
            Workspace.Instance.OnBehaviorNodeLoaded(nodeType, properties);
        }

#if BEHAVIAC_USE_SYSTEM_XML
        protected virtual void load_local(int version, string agentType, XmlNode node)
        {
            Debug.Check(false);
        }

        protected void load_properties_pars_attachments_children(bool bNode, int version, string agentType, XmlNode node)
        {
#if !BEHAVIAC_RELEASE
            SetAgentType(agentType);
#endif//#ifdef _DEBUG

            bool bHasEvents = this.HasEvents();
            List<property_t> properties = new List<property_t>();

            foreach (XmlNode c in node.ChildNodes)
            {
                if (!load_property_pars(ref properties, c, version, agentType))
                {
                    if (bNode)
                    {
                        if (c.Name == "attachment")
                        {
                            bHasEvents = this.load_attachment(version, agentType, bHasEvents, c);
                        }
                        else if (c.Name == "custom")
                        {
                            Debug.Check(c.ChildNodes.Count == 1);
                            XmlNode customNode = c.ChildNodes[0];
                            BehaviorNode pChildNode = BehaviorNode.load(agentType, customNode, version);
                            this.m_customCondition = pChildNode;
                        }
                        else if (c.Name == "node")
                        {
                            BehaviorNode pChildNode = BehaviorNode.load(agentType, c, version);
                            bHasEvents |= pChildNode.m_bHasEvents;

                            this.AddChild(pChildNode);
                        }
                    }
                    else
                    {
                        if (c.Name == "attachment")
                        {
                            bHasEvents = this.load_attachment(version, agentType, bHasEvents, c);
                        }
                    }
                }
            }

            if (properties.Count > 0)
            {
                this.load(version, agentType, properties);
            }

            this.m_bHasEvents |= bHasEvents;
        }

        private void load_attachment_transition_effectors(int version, string agentType, bool bHasEvents, XmlNode node)
        {
            this.m_loadAttachment = true;

            this.load_properties_pars_attachments_children(false, version, agentType, node);

            this.m_loadAttachment = false;
        }

        private bool load_attachment(int version, string agentType, bool bHasEvents, XmlNode node)
        {
            try
            {
                string pAttachClassName = (node.Attributes["class"] != null) ? node.Attributes["class"].Value : null;

                if (pAttachClassName == null)
                {
                    this.load_attachment_transition_effectors(version, agentType, bHasEvents, node);
                    return true;
                }

                BehaviorNode pAttachment = BehaviorNode.Create(pAttachClassName);

                Debug.Check(pAttachment != null);

                if (pAttachment != null)
                {
                    pAttachment.SetClassNameString(pAttachClassName);
                    string idStr = node.Attributes["id"].Value;
                    pAttachment.SetId(Convert.ToInt32(idStr));

                    bool bIsPrecondition = false;
                    bool bIsEffector = false;
                    bool bIsTransition = false;
                    string flagStr = node.Attributes["flag"].Value;

                    if (flagStr == "precondition")
                    {
                        bIsPrecondition = true;
                    }
                    else if (flagStr == "effector")
                    {
                        bIsEffector = true;
                    }
                    else if (flagStr == "transition")
                    {
                        bIsTransition = true;
                    }

                    pAttachment.load_properties_pars_attachments_children(false, version, agentType, node);

                    this.Attach(pAttachment, bIsPrecondition, bIsEffector, bIsTransition);

                    bHasEvents |= (pAttachment is Event);
                }

                return bHasEvents;
            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return bHasEvents;
        }

        private bool load_property_pars(ref List<property_t> properties, XmlNode node, int version, string agentType)
        {
            try
            {
                if (node.Name == "property")
                {
                    Debug.Check(node.Attributes.Count == 1);

                    if (node.Attributes.Count == 1)
                    {
                        XmlAttribute attr = node.Attributes[0];

                        property_t p = new property_t(attr.Name, attr.Value);
                        properties.Add(p);
                    }

                    return true;
                }
                else if (node.Name == "pars")
                {
                    foreach (XmlNode parNode in node.ChildNodes)
                    {
                        if (parNode.Name == "par")
                        {
                            this.load_local(version, agentType, parNode);
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return false;
        }

        protected static BehaviorNode load(string agentType, XmlNode node, int version)
        {
            Debug.Check(node.Name == "node");

            string pClassName = node.Attributes["class"].Value;
            BehaviorNode pNode = BehaviorNode.Create(pClassName);

            Debug.Check(pNode != null, "unsupported class {0}", pClassName);

            if (pNode != null)
            {
                pNode.SetClassNameString(pClassName);
                string idStr = node.Attributes["id"].Value;
                pNode.SetId(Convert.ToInt32(idStr));

                pNode.load_properties_pars_attachments_children(true, version, agentType, node);
            }

            return pNode;
        }
#else

        protected virtual void load_local(int version, string agentType, SecurityElement node)
        {
            Debug.Check(false);
        }

        protected void load_properties_pars_attachments_children(bool bNode, int version, string agentType, SecurityElement node)
        {
#if !BEHAVIAC_RELEASE
            SetAgentType(agentType);
#endif//#ifdef _DEBUG

            bool bHasEvents = this.HasEvents();

            if (node.Children != null)
            {
                List<property_t> properties = new List<property_t>();

                foreach (SecurityElement c in node.Children)
                {
                    if (!load_property_pars(ref properties, c, version, agentType))
                    {
                        if (bNode)
                        {
                            if (c.Tag == "attachment")
                            {
                                bHasEvents = this.load_attachment(version, agentType, bHasEvents, c);
                            }
                            else if (c.Tag == "custom")
                            {
                                Debug.Check(c.Children.Count == 1);
                                SecurityElement customNode = (SecurityElement)c.Children[0];
                                BehaviorNode pChildNode = BehaviorNode.load(agentType, customNode, version);
                                this.m_customCondition = pChildNode;
                            }
                            else if (c.Tag == "node")
                            {
                                BehaviorNode pChildNode = BehaviorNode.load(agentType, c, version);
                                bHasEvents |= pChildNode.m_bHasEvents;

                                this.AddChild(pChildNode);
                            }
                            else
                            {
                                Debug.Check(false);
                            }
                        }
                        else
                        {
                            if (c.Tag == "attachment")
                            {
                                bHasEvents = this.load_attachment(version, agentType, bHasEvents, c);
                            }
                        }
                    }
                }

                if (properties.Count > 0)
                {
                    this.load(version, agentType, properties);
                }
            }

            this.m_bHasEvents |= bHasEvents;
        }

        private void load_attachment_transition_effectors(int version, string agentType, bool bHasEvents, SecurityElement c)
        {
            this.m_loadAttachment = true;

            this.load_properties_pars_attachments_children(false, version, agentType, c);

            this.m_loadAttachment = false;
        }

        private bool load_attachment(int version, string agentType, bool bHasEvents, SecurityElement c)
        {
            try
            {
                string pAttachClassName = c.Attribute("class");

                if (pAttachClassName == null)
                {
                    this.load_attachment_transition_effectors(version, agentType, bHasEvents, c);
                    return true;
                }

                BehaviorNode pAttachment = BehaviorNode.Create(pAttachClassName);

                Debug.Check(pAttachment != null);

                if (pAttachment != null)
                {
                    pAttachment.SetClassNameString(pAttachClassName);
                    string idStr = c.Attribute("id");
                    pAttachment.SetId(Convert.ToInt32(idStr));

                    bool bIsPrecondition = false;
                    bool bIsEffector = false;
                    bool bIsTransition = false;
                    string flagStr = c.Attribute("flag");

                    if (flagStr == "precondition")
                    {
                        bIsPrecondition = true;
                    }
                    else if (flagStr == "effector")
                    {
                        bIsEffector = true;
                    }
                    else if (flagStr == "transition")
                    {
                        bIsTransition = true;
                    }

                    pAttachment.load_properties_pars_attachments_children(false, version, agentType, c);

                    this.Attach(pAttachment, bIsPrecondition, bIsEffector, bIsTransition);

                    bHasEvents |= (pAttachment is Event);
                }

                return bHasEvents;
            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return bHasEvents;
        }

        private bool load_property_pars(ref List<property_t> properties, SecurityElement c, int version, string agentType)
        {
            try
            {
                if (c.Tag == "property")
                {
                    Debug.Check(c.Attributes.Count == 1);

                    foreach (string propName in c.Attributes.Keys)
                    {
                        string propValue = (string)c.Attributes[propName];
                        property_t p = new property_t(propName, propValue);
                        properties.Add(p);
                        break;
                    }

                    return true;
                }
                else if (c.Tag == "pars")
                {
                    if (c.Children != null)
                    {
                        foreach (SecurityElement parNode in c.Children)
                        {
                            if (parNode.Tag == "par")
                            {
                                this.load_local(version, agentType, parNode);
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return false;
        }

        protected static BehaviorNode load(string agentType, SecurityElement node, int version)
        {
            Debug.Check(node.Tag == "node");

            string pClassName = node.Attribute("class");
            BehaviorNode pNode = BehaviorNode.Create(pClassName);

            Debug.Check(pNode != null, "unsupported class {0}", pClassName);

            if (pNode != null)
            {
                pNode.SetClassNameString(pClassName);
                string idStr = node.Attribute("id");
                pNode.SetId(Convert.ToInt32(idStr));

                pNode.load_properties_pars_attachments_children(true, version, agentType, node);
            }

            return pNode;
        }

#endif

        #region Bson Load

        protected void load_properties(int version, string agentType, BsonDeserizer d)
        {
#if !BEHAVIAC_RELEASE
            SetAgentType(agentType);
#endif

            d.OpenDocument();

            //load property after loading par as property might reference par
            List<property_t> properties = new List<property_t>();

            BsonDeserizer.BsonTypes type = d.ReadType();

            while (type == BsonDeserizer.BsonTypes.BT_String)
            {
                string propertyName = d.ReadString();
                string propertyValue = d.ReadString();

                properties.Add(new property_t(propertyName, propertyValue));

                type = d.ReadType();
            }

            if (properties.Count > 0)
            {
                this.load(version, agentType, properties);
            }

            Debug.Check(type == BsonDeserizer.BsonTypes.BT_None);
            d.CloseDocument(false);
        }

        protected void load_locals(int version, string agentType, BsonDeserizer d)
        {
            d.OpenDocument();

            BsonDeserizer.BsonTypes type = d.ReadType();

            while (type == BsonDeserizer.BsonTypes.BT_ParElement)
            {
                this.load_local(version, agentType, d);

                type = d.ReadType();
            }

            Debug.Check(type == BsonDeserizer.BsonTypes.BT_None);
            d.CloseDocument(false);
        }

        protected void load_children(int version, string agentType, BsonDeserizer d)
        {
            d.OpenDocument();

            BehaviorNode pChildNode = this.load(agentType, d, version);
            bool bHasEvents = pChildNode.m_bHasEvents;

            this.AddChild(pChildNode);

            this.m_bHasEvents |= bHasEvents;

            d.CloseDocument(false);
        }

        protected void load_custom(int version, string agentType, BsonDeserizer d)
        {
            d.OpenDocument();

            BsonDeserizer.BsonTypes type = d.ReadType();
            Debug.Check(type == BsonDeserizer.BsonTypes.BT_NodeElement);

            d.OpenDocument();

            BehaviorNode pChildNode = this.load(agentType, d, version);
            this.m_customCondition = pChildNode;

            d.CloseDocument(false);

            d.CloseDocument(false);

            type = d.ReadType();
            Debug.Check(type == BsonDeserizer.BsonTypes.BT_None);
        }

        protected void load_properties_pars_attachments_children(int version, string agentType, BsonDeserizer d, bool bIsTransition)
        {
            BsonDeserizer.BsonTypes type = d.ReadType();

            while (type != BsonDeserizer.BsonTypes.BT_None)
            {
                if (type == BsonDeserizer.BsonTypes.BT_PropertiesElement)
                {
                    try
                    {
                        this.load_properties(version, agentType, d);
                    }
                    catch (Exception e)
                    {
                        Debug.Check(false, e.Message);
                    }
                }
                else if (type == BsonDeserizer.BsonTypes.BT_ParsElement)
                {
                    this.load_locals(version, agentType, d);
                }
                else if (type == BsonDeserizer.BsonTypes.BT_AttachmentsElement)
                {
                    this.load_attachments(version, agentType, d, bIsTransition);

                    this.m_bHasEvents |= this.HasEvents();
                }
                else if (type == BsonDeserizer.BsonTypes.BT_Custom)
                {
                    this.load_custom(version, agentType, d);
                }
                else if (type == BsonDeserizer.BsonTypes.BT_NodeElement)
                {
                    this.load_children(version, agentType, d);
                }
                else
                {
                    Debug.Check(false);
                }

                type = d.ReadType();
            }
        }

        protected BehaviorNode load(string agentType, BsonDeserizer d, int version)
        {
            string pClassName = d.ReadString();
            BehaviorNode pNode = BehaviorNode.Create(pClassName);
            Debug.Check(pNode != null, pClassName);

            if (pNode != null)
            {
                pNode.SetClassNameString(pClassName);
                string idString = d.ReadString();
                pNode.SetId(Convert.ToInt32(idString));

                pNode.load_properties_pars_attachments_children(version, agentType, d, false);
            }

            return pNode;
        }

        protected virtual void load_local(int version, string agentType, BsonDeserizer d)
        {
            Debug.Check(false);
        }

        protected void load_attachments(int version, string agentType, BsonDeserizer d, bool bIsTransition)
        {
            d.OpenDocument();

            BsonDeserizer.BsonTypes type = d.ReadType();

            while (type == BsonDeserizer.BsonTypes.BT_AttachmentElement)
            {
                d.OpenDocument();

                if (bIsTransition)
                {
                    this.m_loadAttachment = true;
                    this.load_properties_pars_attachments_children(version, agentType, d, false);
                    this.m_loadAttachment = false;
                }
                else
                {
                    string attachClassName = d.ReadString();

                    BehaviorNode pAttachment = BehaviorNode.Create(attachClassName);
                    Debug.Check(pAttachment != null, attachClassName);

                    if (pAttachment != null)
                    {
                        pAttachment.SetClassNameString(attachClassName);

                        string idString = d.ReadString();
                        pAttachment.SetId(Convert.ToInt32(idString));

                        bool bIsPrecondition = d.ReadBool();
                        bool bIsEffector = d.ReadBool();
                        bool bAttachmentIsTransition = d.ReadBool();

                        pAttachment.load_properties_pars_attachments_children(version, agentType, d, bAttachmentIsTransition);

                        this.Attach(pAttachment, bIsPrecondition, bIsEffector, bAttachmentIsTransition);

                        this.m_bHasEvents |= (pAttachment is Event);
                    }
                }

                d.CloseDocument(false);

                type = d.ReadType();
            }

            if (type != BsonDeserizer.BsonTypes.BT_None)
            {
                if (type == BsonDeserizer.BsonTypes.BT_ParsElement)
                {
                    this.load_locals(version, agentType, d);
                }
                else if (type == BsonDeserizer.BsonTypes.BT_AttachmentsElement)
                {
                    this.load_attachments(version, agentType, d, bIsTransition);

                    this.m_bHasEvents |= this.HasEvents();
                }
                else
                {
                    Debug.Check(false);
                }

                type = d.ReadType();
            }

            Debug.Check(type == BsonDeserizer.BsonTypes.BT_None);
            d.CloseDocument(false);
        }

        protected BehaviorNode load_node(int version, string agentType, BsonDeserizer d)
        {
            d.OpenDocument();

            BsonDeserizer.BsonTypes type = d.ReadType();
            Debug.Check(type == BsonDeserizer.BsonTypes.BT_NodeElement);

            d.OpenDocument();
            BehaviorNode node = this.load(agentType, d, version);
            d.CloseDocument(false);

            type = d.ReadType();
            Debug.Check(type == BsonDeserizer.BsonTypes.BT_None);
            d.CloseDocument(false);

            return node;
        }

        #endregion Bson Load

        #endregion Load

        public void Attach(BehaviorNode pAttachment, bool bIsPrecondition, bool bIsEffector)
        {
            this.Attach(pAttachment, bIsPrecondition, bIsEffector, false);
        }

        public virtual void Attach(BehaviorNode pAttachment, bool bIsPrecondition, bool bIsEffector, bool bIsTransition)
        {
            Debug.Check(bIsTransition == false);

            if (bIsPrecondition)
            {
                Debug.Check(!bIsEffector);

                if (this.m_preconditions == null)
                {
                    this.m_preconditions = new List<Precondition>();
                }

                Precondition predicate = pAttachment as Precondition;
                Debug.Check(predicate != null);
                this.m_preconditions.Add(predicate);

                Precondition.EPhase phase = predicate.Phase;

                if (phase == Precondition.EPhase.E_ENTER)
                {
                    this.m_enter_precond++;
                }
                else if (phase == Precondition.EPhase.E_UPDATE)
                {
                    this.m_update_precond++;
                }
                else if (phase == Precondition.EPhase.E_BOTH)
                {
                    this.m_both_precond++;
                }
                else
                {
                    Debug.Check(false);
                }
            }
            else if (bIsEffector)
            {
                Debug.Check(!bIsPrecondition);

                if (this.m_effectors == null)
                {
                    this.m_effectors = new List<Effector>();
                }

                Effector effector = pAttachment as Effector;
                Debug.Check(effector != null);
                this.m_effectors.Add(effector);

                Effector.EPhase phase = effector.Phase;

                if (phase == Effector.EPhase.E_SUCCESS)
                {
                    this.m_success_effectors++;
                }
                else if (phase == Effector.EPhase.E_FAILURE)
                {
                    this.m_failure_effectors++;
                }
                else if (phase == Effector.EPhase.E_BOTH)
                {
                    this.m_both_effectors++;
                }
                else
                {
                    Debug.Check(false);
                }
            }
            else
            {
                if (this.m_events == null)
                {
                    this.m_events = new List<BehaviorNode>();
                }

                this.m_events.Add(pAttachment);
            }
        }

        public virtual void AddChild(BehaviorNode pChild)
        {
            pChild.m_parent = this;

            if (this.m_children == null)
            {
                this.m_children = new List<BehaviorNode>();
            }

            this.m_children.Add(pChild);
        }

        protected virtual EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return EBTStatus.BT_FAILURE;
        }

        public void SetClassNameString(string className)
        {
            this.m_className = className;
        }

        public string GetClassNameString()
        {
            return this.m_className;
        }

        public int GetId()
        {
            return this.m_id;
        }

        public void SetId(int id)
        {
            this.m_id = id;
        }

        public string GetPath()
        {
            return "";
        }

        public BehaviorNode Parent
        {
            get
            {
                return this.m_parent;
            }
        }

        public int PreconditionsCount
        {
            get
            {
                if (this.m_preconditions != null)
                {
                    return this.m_preconditions.Count;
                }

                return 0;
            }
        }

        public bool CheckPreconditions(Agent pAgent, bool bIsAlive)
        {
            Precondition.EPhase phase = bIsAlive ? Precondition.EPhase.E_UPDATE : Precondition.EPhase.E_ENTER;

            //satisfied if there is no preconditions
            if (this.m_preconditions == null || this.m_preconditions.Count == 0)
            {
                return true;
            }

            if (this.m_both_precond == 0)
            {
                if (phase == Precondition.EPhase.E_ENTER && this.m_enter_precond == 0)
                {
                    return true;
                }

                if (phase == Precondition.EPhase.E_UPDATE && this.m_update_precond == 0)
                {
                    return true;
                }
            }

            bool firstValidPrecond = true;
            bool lastCombineValue = false;

            for (int i = 0; i < this.m_preconditions.Count; ++i)
            {
                Precondition pPrecond = this.m_preconditions[i];

                if (pPrecond != null)
                {
                    Precondition.EPhase ph = pPrecond.Phase;

                    if (phase == Precondition.EPhase.E_BOTH || ph == Precondition.EPhase.E_BOTH || ph == phase)
                    {
                        bool taskBoolean = pPrecond.Evaluate(pAgent);

                        CombineResults(ref firstValidPrecond, ref lastCombineValue, pPrecond, taskBoolean);
                    }
                }
            }

            return lastCombineValue;
        }

        private static void CombineResults(ref bool firstValidPrecond, ref bool lastCombineValue, Precondition pPrecond, bool taskBoolean)
        {
            if (firstValidPrecond)
            {
                firstValidPrecond = false;
                lastCombineValue = taskBoolean;
            }
            else
            {
                bool andOp = pPrecond.IsAnd;

                if (andOp)
                {
                    lastCombineValue = lastCombineValue && taskBoolean;
                }

                else
                {
                    lastCombineValue = lastCombineValue || taskBoolean;
                }
            }
        }

        public virtual void ApplyEffects(Agent pAgent, Effector.EPhase phase)
        {
            if (this.m_effectors == null || this.m_effectors.Count == 0)
            {
                return;
            }

            if (this.m_both_effectors == 0)
            {
                if (phase == Effector.EPhase.E_SUCCESS && this.m_success_effectors == 0)
                {
                    return;
                }

                if (phase == Effector.EPhase.E_FAILURE && this.m_failure_effectors == 0)
                {
                    return;
                }
            }

            for (int i = 0; i < this.m_effectors.Count; ++i)
            {
                Effector pEffector = this.m_effectors[i];

                if (pEffector != null)
                {
                    Effector.EPhase ph = pEffector.Phase;

                    if (phase == Effector.EPhase.E_BOTH || ph == Effector.EPhase.E_BOTH || ph == phase)
                    {
                        pEffector.Evaluate(pAgent);
                    }
                }
            }

            return;
        }

        public bool CheckEvents(string eventName, Agent pAgent, Dictionary<uint, IInstantiatedVariable> eventParams)
        {
            if (this.m_events != null)
            {
                //bool bTriggered = false;
                for (int i = 0; i < this.m_events.Count; ++i)
                {
                    BehaviorNode pA = this.m_events[i];
                    Event pE = pA as Event;

                    //check events only
                    if (pE != null && !string.IsNullOrEmpty(eventName))
                    {
                        string pEventName = pE.GetEventName();

                        if (!string.IsNullOrEmpty(pEventName) && pEventName == eventName)
                        {
                            pE.switchTo(pAgent, eventParams);

                            if (pE.TriggeredOnce())
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public virtual bool Evaluate(Agent pAgent)
        {
            Debug.Check(false, "only Condition/Sequence/And/Or allowed");
            return false;
        }



        protected bool EvaluteCustomCondition(Agent pAgent)
        {
            if (this.m_customCondition != null)
            {
                return m_customCondition.Evaluate(pAgent);
            }

            return false;
        }

        public void SetCustomCondition(BehaviorNode node)
        {
            this.m_customCondition = node;
        }

#if !BEHAVIAC_RELEASE
        private string m_agentType;

        public void SetAgentType(string agentType)
        {
            Debug.Check(agentType.IndexOf("::") == -1);

            this.m_agentType = agentType;
        }

        public string GetAgentType()
        {
            return this.m_agentType;
        }

#endif

        protected abstract BehaviorTask createTask();

        public virtual bool enteraction_impl(Agent pAgent)
        {
            return false;
        }

        public virtual bool exitaction_impl(Agent pAgent)
        {
            return false;
        }

        private string m_className;
        private int m_id;

        protected List<BehaviorNode> m_events;
        private List<Precondition> m_preconditions;
        private List<Effector> m_effectors;
        protected bool m_loadAttachment = false;
        private byte m_enter_precond;
        private byte m_update_precond;
        private byte m_both_precond;
        private byte m_success_effectors;
        private byte m_failure_effectors;
        private byte m_both_effectors;

        protected BehaviorNode m_parent;
        protected List<BehaviorNode> m_children;
        protected BehaviorNode m_customCondition;

        protected bool m_bHasEvents;
    }

    public abstract class DecoratorNode : BehaviorNode
    {
        public DecoratorNode()
        {
            m_bDecorateWhenChildEnds = false;
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);

            for (int i = 0; i < properties.Count; ++i)
            {
                property_t p = properties[i];

                if (p.name == "DecorateWhenChildEnds")
                {
                    if (p.value == "true")
                    {
                        this.m_bDecorateWhenChildEnds = true;
                    }
                }
            }
        }

        public override bool IsManagingChildrenAsSubTrees()
        {
            //if it needs to evaluate something even the child is running, this needs to return true.
            //return !this.m_bDecorateWhenChildEnds;
            return true;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            if (!(pTask.GetNode() is DecoratorNode))
            {
                return false;
            }

            return base.IsValid(pAgent, pTask);
        }

        public bool m_bDecorateWhenChildEnds;
    }

    // ============================================================================
    public class BehaviorTree : BehaviorNode
    {
        //keep this version equal to designers' NewVersion
        private const int SupportedVersion = 5;

        private Dictionary<uint, ICustomizedProperty> m_localProps;
        public Dictionary<uint, ICustomizedProperty> LocalProps
        {
            get
            {
                return m_localProps;
            }
        }

        // deprecated, to use AddLocal
        public void AddPar(string agentType, string typeName, string name, string valueStr)
        {
            this.AddLocal(agentType, typeName, name, valueStr);
        }

        public void AddLocal(string agentType, string typeName, string name, string valueStr)
        {
            if (this.m_localProps == null)
            {
                this.m_localProps = new Dictionary<uint, ICustomizedProperty>();
            }

            uint varId = Utils.MakeVariableId(name);
            ICustomizedProperty prop = AgentMeta.CreateProperty(typeName, varId, name, valueStr);
            this.m_localProps[varId] = prop;

            Type type = Utils.GetElementTypeFromName(typeName);

            if (type != null)
            {
                typeName = Utils.GetNativeTypeName(type);
                prop = AgentMeta.CreateArrayItemProperty(typeName, varId, name);
                varId = Utils.MakeVariableId(name + "[]");
                this.m_localProps[varId] = prop;
            }
        }

        public void InstantiatePars(Dictionary<uint, IInstantiatedVariable> vars)
        {
            if (this.m_localProps != null)
            {
                var e = this.m_localProps.Keys.GetEnumerator();

                while (e.MoveNext())
                {
                    uint varId = e.Current;
                    vars[varId] = this.m_localProps[varId].Instantiate();
                }
            }
        }

        public void UnInstantiatePars(Dictionary<uint, IInstantiatedVariable> vars)
        {
            if (this.m_localProps != null)
            {
                var e = this.m_localProps.Keys.GetEnumerator();

                while (e.MoveNext())
                {
                    uint varId = e.Current;
                    vars.Remove(varId);
                }
            }
        }

#if BEHAVIAC_USE_SYSTEM_XML
        protected override void load_local(int version, string agentType, XmlNode node)
        {
            if (node.Name != "par")
            {
                Debug.Check(false);
                return;
            }

            string name = node.Attributes["name"].Value;
            string type = node.Attributes["type"].Value.Replace("::", ".");
            string value = node.Attributes["value"].Value;

            this.AddLocal(agentType, type, name, value);
        }
#else
        protected override void load_local(int version, string agentType, SecurityElement node)
        {
            if (node.Tag != "par")
            {
                Debug.Check(false);
                return;
            }

            string name = node.Attribute("name");
            string type = node.Attribute("type").Replace("::", ".");
            string value = node.Attribute("value");

            this.AddLocal(agentType, type, name, value);
        }
#endif

        protected override void load_local(int version, string agentType, BsonDeserizer d)
        {
            d.OpenDocument();

            string name = d.ReadString();
            string type = d.ReadString().Replace("::", ".");
            string value = d.ReadString();
            this.AddLocal(agentType, type, name, value);

            d.CloseDocument(true);
        }

        public bool load_xml(byte[] pBuffer)
        {
            try
            {
                Debug.Check(pBuffer != null);
                string xml = System.Text.Encoding.UTF8.GetString(pBuffer);

#if BEHAVIAC_USE_SYSTEM_XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                XmlNode behaviorNode = xmlDoc.DocumentElement;

                if (behaviorNode.Name != "behavior" && behaviorNode.ChildNodes.Count != 1)
                {
                    return false;
                }

                this.m_name = behaviorNode.Attributes["name"].Value;
                string agentType = behaviorNode.Attributes["agenttype"].Value;
                string fsm = (behaviorNode.Attributes["fsm"] != null) ? behaviorNode.Attributes["fsm"].Value : null;
                string versionStr = behaviorNode.Attributes["version"].Value;
#else
                SecurityParser xmlDoc = new SecurityParser();
                xmlDoc.LoadXml(xml);

                SecurityElement behaviorNode = xmlDoc.ToXml();

                if (behaviorNode.Tag != "behavior" && (behaviorNode.Children == null || behaviorNode.Children.Count != 1))
                {
                    return false;
                }

                this.m_name = behaviorNode.Attribute("name");
                string agentType = behaviorNode.Attribute("agenttype").Replace("::", ".");
                string fsm = behaviorNode.Attribute("fsm");
                string versionStr = behaviorNode.Attribute("version");
#endif
                int version = int.Parse(versionStr);

                if (version != SupportedVersion)
                {
                    Debug.LogError(string.Format("'{0}' Version({1}), while Version({2}) is supported, please update runtime or rexport data using the latest designer", this.m_name, version, SupportedVersion));
                }

                this.SetClassNameString("BehaviorTree");
                this.SetId(-1);

                if (!string.IsNullOrEmpty(fsm) && fsm == "true")
                {
                    this.m_bIsFSM = true;
                }

                this.load_properties_pars_attachments_children(true, version, agentType, behaviorNode);

                return true;
            }
            catch (Exception e)
            {
                Debug.Check(false, e.Message);
            }

            Debug.Check(false);
            return false;
        }

        public bool load_bson(byte[] pBuffer)
        {
            try
            {
                BsonDeserizer d = new BsonDeserizer();

                if (d.Init(pBuffer))
                {
                    BsonDeserizer.BsonTypes type = d.ReadType();

                    if (type == BsonDeserizer.BsonTypes.BT_BehaviorElement)
                    {
                        bool bOk = d.OpenDocument();
                        Debug.Check(bOk);

                        this.m_name = d.ReadString();
                        string agentTypeTmp = d.ReadString();
                        string agentType = agentTypeTmp.Replace("::", ".");
                        bool bFsm = d.ReadBool();
                        string versionStr = d.ReadString();
                        int version = Convert.ToInt32(versionStr);

                        if (version != SupportedVersion)
                        {
                            Debug.LogError(string.Format("'{0}' Version({1}), while Version({2}) is supported, please update runtime or rexport data using the latest designer", this.m_name, version, SupportedVersion));
                        }

                        this.SetClassNameString("BehaviorTree");
                        this.SetId(-1);

                        this.m_bIsFSM = bFsm;

                        this.load_properties_pars_attachments_children(version, agentType, d, false);

                        d.CloseDocument(false);

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("load_bson failed: {0} {1}", e.Message, pBuffer.Length));
                Debug.Check(false, e.Message);
            }

            Debug.Check(false);
            return false;
        }

        //return the path relative to the workspace path
        protected string m_name;

        public string GetName()
        {
            return this.m_name;
        }

        public void SetName(string name)
        {
            this.m_name = name;
        }

        #region FSM
        private bool m_bIsFSM = false;

        public bool IsFSM
        {
            get
            {
                return this.m_bIsFSM;
            }
            set
            {
                this.m_bIsFSM = value;
            }
        }
        #endregion

        protected override BehaviorTask createTask()
        {
            BehaviorTreeTask pTask = new BehaviorTreeTask();
            return pTask;
        }
    }
}
