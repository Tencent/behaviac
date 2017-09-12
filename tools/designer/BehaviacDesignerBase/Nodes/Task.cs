/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR IfExistS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Nodes
{
    public class Task : Behaviac.Design.Nodes.Sequence
    {
        public const string LOCAL_TASK_PARAM_PRE = "_$local_task_param_$_";

        public Task(string label, string description)
        : base(label, description)
        {
            //don't do error check in the base's CheckForErrors as the error/warning messages in the Behaviac.Design.Nodes.Sequence is not properly.
            _do_sequence_error_check = false;
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/task/";
            }
        }

        protected override void CreateInterruptChild()
        {
        }

        public override string ExportClass
        {
            get
            {
                return "Task";
            }
        }

        public override string Description
        {
            get
            {
                string str = base.Description;

                if (_task != null)
                {
                    str += '\n' + _task.GetPrototype();
                }

                return str;
            }
        }

        private MethodDef _task = null;
        [DesignerMethodEnum("TaskPrototype", "TaskPrototypeDesc", "Task", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags | DesignerProperty.DesignerFlags.NoDisplayOnProperty, MethodType.Task)]
        public MethodDef Prototype
        {
            get
            {
                return _task;
            }
            set
            {
                this._task = value;
            }
        }

        public bool IsHTN
        {
            get
            {
                int count = 0;

                for (int i = 0; i < this.Children.Count; ++i)
                {
                    BaseNode c = this.Children[i];
                    bool isMethod = c is Method;

                    if (isMethod)
                    {
                        count++;
                    }
                }

                // all children are method
                if (count == this.Children.Count)
                {
                    return true;
                }

                return false;
            }
        }

        public void CollectTaskPars(ref List<ParInfo> pars)
        {
            if (this._task != null)
            {
                for (int i = 0; i < this._task.Params.Count; ++i)
                {
                    var param = this._task.Params[i];
                    string par_name = string.Format("{0}{1}", LOCAL_TASK_PARAM_PRE, i);

                    ParInfo par = new ParInfo(this, this.Behavior != null ? this.Behavior.AgentType : null);

                    par.IsAddedAutomatically = true;
                    par.Name = par_name;
                    par.DisplayName = param.DisplayName;
                    par.TypeName = param.Type.FullName;
                    par.Variable = new VariableDef(param.Value);
                    par.Description = param.Description;
                    par.Display = false;

                    pars.Add(par);
                }
            }
        }

        public override void OnPropertyValueChanged(DesignerPropertyInfo property)
        {
            if (property.Property.Name == "Prototype")
            {
                List<ParInfo> pars = ((Behavior)(this.Behavior)).LocalVars;

                bool bLoop = true;

                //remove old added local variables
                while (bLoop)
                {
                    int index = pars.FindIndex((p) => p.Name.IndexOf(LOCAL_TASK_PARAM_PRE) != -1);

                    if (index != -1)
                    {
                        pars.RemoveAt(index);
                    }
                    else
                    {
                        bLoop = false;
                    }
                }

                CollectTaskPars(ref pars);

                this.Behavior.AgentType.ResetPars(pars);

                if (Plugin.UpdateMetaStoreHandler != null)
                {
                    Plugin.UpdateMetaStoreHandler(null);
                }
            }
        }

        //can only added to the 'Behavior'
        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            return base.CanBeAdoptedBy(parent) && (parent is Behavior);
        }

        //can only accept multiple methods or only one other type of node
        public override bool CanAdopt(BaseNode child)
        {
            if (base.CanAdopt(child))
            {
                // 1. the 1st child, to accept it anyway
                if (this.Children.Count == 0)
                {
                    return true;
                }

                // 2. only accept it if the newly added one is a Method and the existing Children are all Method
                if (child is Nodes.Method)
                {
                    // insert a Method before a none-method node
                    if (this.Children.Count == 1)
                    {
                        return true;
                    }

                    int count = 0;

                    for (int i = 0; i < this.Children.Count; ++i)
                    {
                        BaseNode c = this.Children[i];
                        bool isMethod = c is Method;

                        if (isMethod)
                        {
                            count++;
                        }
                    }

                    // all children are method, accept any node, (a Method will be automatically added if it is not a method)
                    if (count == this.Children.Count)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override string GenerateNewLabel()
        {
            string newlabel = string.Empty;
            newlabel = string.Format("{0}({1})", this.Label, this._task != null ? this._task.PrototypeName : "");

            return newlabel;
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            Task right = (Task)newnode;

            if (_task != null)
            {
                right._task = (MethodDef)_task.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            if (_task == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "No method"));
            }

            if (this.Children.Count == 0)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.TaskNoMethod));

            }
            else if (this.Children.Count > 1)
            {
                // all children are method or only one child of other child
                int count = 0;

                for (int i = 0; i < this.Children.Count; ++i)
                {
                    BaseNode c = this.Children[i];
                    bool isMethod = c is Method;

                    if (isMethod)
                    {
                        count++;
                    }
                }

                if (count < this.Children.Count)
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, Resources.TaskMethodChildrenOrOne));
                }
            }

            base.CheckForErrors(rootBehavior, result);
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (this._task != null)
            {
                if (metaOperation == MetaOperations.ChangeAgentType || metaOperation == MetaOperations.RemoveAgentType)
                {
                    if (this._task.ShouldBeCleared(agentType))
                    {
                        this._task = null;

                        bReset = true;
                    }
                }
                else if (metaOperation == MetaOperations.RemoveMethod)
                {
                    if (method != null && method.OldName == this._task.Name)
                    {
                        this._task = null;

                        bReset = true;
                    }
                }
                else
                {
                    bReset |= this._task.ResetMembers(metaOperation, agentType, baseType, method, property);
                }
            }

            bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

            return bReset;
        }
    }
}
