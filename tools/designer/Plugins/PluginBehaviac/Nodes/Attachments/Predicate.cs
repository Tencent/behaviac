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
using System.Drawing;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Events
{
    class Predicate : Behaviac.Design.Attachments.Predicate
    {
        public Predicate(Node node)
        : base(node, Resources.Predicate, Resources.PredicateDesc)
        {
        }

        public Predicate(Node node, string displayName, string desc)
        : base(node, displayName, desc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "Predicate";
            }
        }

        private BinaryOperator _binary = BinaryOperator.And;
        [DesignerEnum("BinaryOperator", "BinaryOperatorDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, "")]
        public BinaryOperator BinaryOperator
        {
            get
            {
                return _binary;
            }
            set
            {
                _binary = value;
            }
        }

        private PreconditionPhase _phase = PreconditionPhase.Enter;
        [DesignerEnum("PreconditionPhase", "PreconditionPhaseDesc", "Condition", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, "EffectorOperaptor")]
        public PreconditionPhase Phase
        {
            get
            {
                return _phase;
            }
            set
            {
                _phase = value;
            }
        }

        private RightValueDef _opl;
        [DesignerRightValueEnum("OperandLeft", "OperandLeftDesc", "Predicate", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.AttributesMethod, MethodType.Getter, "", "Opr")]
        public RightValueDef Opl
        {
            get
            {
                return _opl;
            }
            set
            {
                this._opl = value;
            }
        }

        private OperatorType _operator = OperatorType.Equal;
        [DesignerEnum("Operator", "OperatorDesc", "Predicate", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, "Opl")]
        public OperatorType Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
            }
        }

        private RightValueDef _opr;
        [DesignerRightValueEnum("OperandRight", "OperandRightDesc", "Predicate", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "Opl", "")]
        public RightValueDef Opr
        {
            get
            {
                return _opr;
            }
            set
            {
                this._opr = value;
            }
        }

        public override string Description
        {
            get
            {
                //not ideal, for the left node list
                if (_opl == null && _opr == null)
                {
                    return base.Description;
                }

                string str = base.Description;

                if (_opl != null)
                {
                    str += "\n" + _opl.GetExportValue();
                }

                str += "\n" + _operator.ToString();

                if (_opr != null)
                {
                    str += "\n" + _opr.GetExportValue();
                }

                return str;
            }
        }

        public override object[] GetExcludedEnums(DesignerEnum enumAttr)
        {
            //List<object> excludedOperatorsResult = new List<object>() { OperatorType.Assignment, OperatorType.In };
            List<object> excludedOperatorsResult = new List<object>();

            if (enumAttr != null && enumAttr.ExcludeTag == "Opl")
            {
                if (this.Opl != null)
                {
                    bool bNoAssignment = (this.Opl.Method != null);

                    if (this.Opl.ValueType != typeof(bool))
                    {
                        //and and or are only valid for bool, so to exclude and and or when the type is not bool
                        object[] excludedOperators = new object[] { OperatorType.And, OperatorType.Or };
                        excludedOperatorsResult.AddRange(excludedOperators);
                    }
                    else if (this.Opl.ValueType == typeof(bool))
                    {
                        object[] excludedOperators = new object[] { OperatorType.Greater, OperatorType.GreaterEqual, OperatorType.Less, OperatorType.LessEqual };
                        excludedOperatorsResult.AddRange(excludedOperators);
                    }

                    if (bNoAssignment)
                    {
                        excludedOperatorsResult.Add(OperatorType.Assignment);
                    }
                }
            }

            return excludedOperatorsResult.ToArray();
        }

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            Predicate prec = (Predicate)newattach;

            prec._binary = _binary;
            prec._phase = _phase;

            //prec._negate = _negate;
            if (this._opl != null)
            {
                prec._opl = (RightValueDef)this._opl.Clone();
            }

            if (this._opr != null)
            {
                prec._opr = (RightValueDef)this._opr.Clone();
            }

            prec._operator = this._operator;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<Node.ErrorCheck> result)
        {
            if (this.Opl == null || this.Opr == null)
            {
                result.Add(new Node.ErrorCheck(this.Node, this.Id, this.Label, ErrorCheckLevel.Error, "Precondition Operand is not complete!"));
            }
            else if (this.Opl.ValueType != typeof(bool) &&
                     (this.Operator == OperatorType.And || this.Operator == OperatorType.Or))
            {
                //and and or are only valid for bool
                result.Add(new Node.ErrorCheck(this.Node, this.Id, this.Label, ErrorCheckLevel.Error, "And and Or are only valid for bool!"));
            }

            base.CheckForErrors(rootBehavior, result);
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (this.Opl != null)
            {
                bReset |= this.Opl.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            if (this.Opr != null)
            {
                bReset |= this.Opr.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

            if (bReset && metaOperation != MetaOperations.CheckProperty && metaOperation != MetaOperations.CheckMethod)
            {
                OnPropertyValueChanged(false);
            }

            return bReset;
        }

        public override void PostCreate(List<Node.ErrorCheck> result, int version, Behaviac.Design.Nodes.Node node, System.Xml.XmlNode xmlNode)
        {
            AutoRestruct(result, version, this, node);
        }

        //if there is a 'Predicate' attachment, convert it to a Condition node and attach it to the '_custom_condition' connector.
        private void AutoRestruct(List<Node.ErrorCheck> result, int version, Behaviac.Design.Attachments.Attachment a, Node node)
        {
            if (version <= 1)
            {
                string attachClass = a.GetType().FullName;

                if (attachClass.IndexOf("PluginBehaviac.Events.Predicate") >= 0)
                {
                    DesignerPropertyInfo propInfo = DesignerProperty.GetDesignerProperty(a.GetType(), "Opl");
                    RightValueDef opl = propInfo.GetValue(a) as RightValueDef;
                    propInfo = DesignerProperty.GetDesignerProperty(a.GetType(), "Opr");
                    RightValueDef opr = propInfo.GetValue(a) as RightValueDef;
                    propInfo = DesignerProperty.GetDesignerProperty(a.GetType(), "Operator");
                    OperatorType oprr = (OperatorType)propInfo.GetValue(a);
                    OperatorTypes oprType = (OperatorTypes)((int)OperatorTypes.Equal - (int)OperatorType.Equal + (int)oprr);
                    propInfo = DesignerProperty.GetDesignerProperty(a.GetType(), "BinaryOperator");
                    Behaviac.Design.Attachments.BinaryOperator binaryOpr = (Behaviac.Design.Attachments.BinaryOperator)propInfo.GetValue(a);

                    string clss = node.GetType().FullName;
                    bool bIsSeqSel = (node.GetType().IsSubclassOf(typeof(Sequence)) ||
                                      node.GetType().IsSubclassOf(typeof(Selector)));

                    bool bCare = (bIsSeqSel ||
                                  node.GetType().IsSubclassOf(typeof(Impulse))
                                 );

                    if (bCare ||
                        clss == "PluginBehaviac.Nodes.Query" ||
                        clss == "PluginBehaviac.Nodes.DecoratorCountLimit")
                    {
                        node.RemoveAttachment(a);
                        node.Behavior.TriggerWasModified(node);

                        Type newType = Plugin.GetType("PluginBehaviac.Nodes.Condition");

                        Behaviac.Design.Nodes.Node newNode = Behaviac.Design.Nodes.Node.Create(newType);
                        Behaviac.Design.Nodes.Node.Connector connector = node.GetConnector(Node.Connector.kInterupt);

                        if (connector != null && connector.Identifier == Node.Connector.kInterupt && connector.ChildCount > 0)
                        {
                            //it has multiple Predicates, so insert all of them to a newly created Sequence
                            Node oldOne = (Node)connector.GetChild(0);

                            if (oldOne.GetType().IsSubclassOf(typeof(Condition)))
                            {
                                AddAfterConditions(node, binaryOpr, newNode, connector, oldOne);
                            }
                            else
                            {
                                if (bIsSeqSel)
                                {
                                    Debug.Check(oldOne.GetType().IsSubclassOf(typeof(Decorator)));
                                    Decorator d = oldOne as Decorator;
                                    node = oldOne;
                                    connector = node.GetConnector(BaseNode.Connector.kGeneric);
                                    oldOne = (Node)d.Children[0];
                                }

                                if (oldOne.GetType() == typeof(PluginBehaviac.Nodes.And))
                                {
                                    if (binaryOpr == Behaviac.Design.Attachments.BinaryOperator.Or)
                                    {
                                        node.RemoveChild(connector, oldOne);
                                        Type selType1 = Plugin.GetType("PluginBehaviac.Nodes.Or");

                                        Behaviac.Design.Nodes.Node sel = Behaviac.Design.Nodes.Node.Create(selType1);
                                        sel.AddChild(BaseNode.Connector.kGeneric, oldOne);
                                        sel.AddChild(BaseNode.Connector.kGeneric, newNode);

                                        node.AddChild(BaseNode.Connector.kInterupt, sel);
                                    }
                                    else
                                    {
                                        oldOne.AddChild(BaseNode.Connector.kGeneric, newNode);
                                    }
                                }
                                else if (oldOne.GetType() == typeof(PluginBehaviac.Nodes.Or))
                                {
                                    if (binaryOpr == Behaviac.Design.Attachments.BinaryOperator.And)
                                    {
                                        node.RemoveChild(connector, oldOne);
                                        Type selType1 = Plugin.GetType("PluginBehaviac.Nodes.And");

                                        Behaviac.Design.Nodes.Node sel = Behaviac.Design.Nodes.Node.Create(selType1);
                                        sel.AddChild(BaseNode.Connector.kGeneric, oldOne);
                                        sel.AddChild(BaseNode.Connector.kGeneric, newNode);

                                        node.AddChild(BaseNode.Connector.kInterupt, sel);
                                    }
                                    else
                                    {
                                        oldOne.AddChild(BaseNode.Connector.kGeneric, newNode);
                                    }
                                }
                                else if (oldOne.GetType().IsSubclassOf(typeof(Condition)))
                                {
                                    AddAfterConditions(node, binaryOpr, newNode, connector, oldOne);
                                }
                                else
                                {
                                    Debug.Check(false);
                                }
                            }
                        }
                        else
                        {
                            //the first condition
                            Behaviac.Design.Nodes.Node notNode = null;

                            if (bIsSeqSel)
                            {
                                //for sequence/selector, it is reverted
                                Type notType = Plugin.GetType("PluginBehaviac.Nodes.DecoratorNot");

                                notNode = Behaviac.Design.Nodes.Node.Create(notType);
                                node.AddChild(BaseNode.Connector.kInterupt, notNode);
                                notNode.AddChild(BaseNode.Connector.kGeneric, newNode);
                            }
                            else
                            {
                                node.AddChild(BaseNode.Connector.kInterupt, newNode);
                            }
                        }

                        // initialise the attachments properties
                        IList<DesignerPropertyInfo> lp = newNode.GetDesignerProperties();

                        for (int p = 0; p < lp.Count; ++p)
                        {
                            if (lp[p].Property.Name == "Opl")
                            {
                                lp[p].Property.SetValue(newNode, opl, null);
                            }
                            else if (lp[p].Property.Name == "Opr")
                            {
                                lp[p].Property.SetValue(newNode, opr, null);
                            }
                            else if (lp[p].Property.Name == "Operator")
                            {
                                lp[p].Property.SetValue(newNode, oprr, null);
                            }
                        }

                        // update attacheent with attributes
                        newNode.OnPropertyValueChanged(false);
                    }
                    else if (clss == "PluginBehaviac.Nodes.Action")
                    {
                        Type newType = Plugin.GetType("PluginBehaviac.Events.Precondition");

                        Behaviac.Design.Attachments.Attachment newNode = Behaviac.Design.Attachments.Attachment.Create(newType, node);
                        node.AddAttachment(newNode);
                        node.RemoveAttachment(a);
                        node.Behavior.TriggerWasModified(node);

                        // initialise the attachments properties
                        IList<DesignerPropertyInfo> lp = newNode.GetDesignerProperties();

                        for (int p = 0; p < lp.Count; ++p)
                        {
                            if (lp[p].Property.Name == "BinaryOperator")
                            {
                                lp[p].Property.SetValue(newNode, binaryOpr, null);
                            }
                            else if (lp[p].Property.Name == "Opl")
                            {
                                lp[p].Property.SetValue(newNode, opl, null);
                            }
                            else if (lp[p].Property.Name == "Opr2")
                            {
                                lp[p].Property.SetValue(newNode, opr, null);
                            }
                            else if (lp[p].Property.Name == "Operator")
                            {
                                lp[p].Property.SetValue(newNode, oprType, null);
                            }
                            else if (lp[p].Property.Name == "IsAlive")
                            {
                                lp[p].SetValueFromString(result, newNode, "true");
                            }
                        }

                        // update attacheent with attributes
                        newNode.OnPropertyValueChanged(false);
                    }
                    else
                    {
                        Debug.Check(false);
                    }
                }
            } // if (version <= 1)
        }

        private static void AddAfterConditions(Node node, Behaviac.Design.Attachments.BinaryOperator binaryOpr, Behaviac.Design.Nodes.Node newNode, Behaviac.Design.Nodes.Node.Connector connector, Node oldOne)
        {
            node.RemoveChild(connector, oldOne);

            Type seqType = Plugin.GetType("PluginBehaviac.Nodes.And");

            if (binaryOpr == Behaviac.Design.Attachments.BinaryOperator.Or)
            {
                seqType = Plugin.GetType("PluginBehaviac.Nodes.Or");
            }

            Behaviac.Design.Nodes.Node seq = Behaviac.Design.Nodes.Node.Create(seqType);
            seq.AddChild(BaseNode.Connector.kGeneric, newNode);
            seq.AddChild(BaseNode.Connector.kGeneric, oldOne);

            node.AddChild(BaseNode.Connector.kInterupt, seq);
        }
    }
}
