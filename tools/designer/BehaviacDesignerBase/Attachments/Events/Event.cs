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
using System.Windows.Forms;
using System.Reflection;
using Behaviac.Design.Attributes;
using Behaviac.Design.Properties;
using Behaviac.Design.Nodes;

namespace Behaviac.Design.Attachments
{
    [Behaviac.Design.EnumDesc("Behaviac.Design.Attachments.TriggerMode", true, "TriggerMode", "TriggerModeDesc")]
    public enum TriggerMode
    {
        [Behaviac.Design.EnumMemberDesc("Transfer", true, "TriggerMode_Transfer", "TriggerMode_TransferDesc")]
        Transfer,

        [Behaviac.Design.EnumMemberDesc("Return", true, "TriggerMode_Return", "TriggerMode_ReturnDesc")]
        Return
    }

    /// <summary>
    /// This class represents an event which is attached to a node.
    /// </summary>
    public class Event : Attach
    {
        public Event(Behaviac.Design.Nodes.Node node)
        : base(node, Resources.Event, Resources.EventDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/attachment/#section-1";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "Event";
            }
        }

        protected bool _bTriggeredOnce = false;
        [DesignerBoolean("TriggeredOnce", "TriggeredOnceDesc", "Event", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags)]
        public bool TriggeredOnce
        {
            get
            {
                return _bTriggeredOnce;
            }
            set
            {
                _bTriggeredOnce = value;
            }
        }

        private TriggerMode _triggerMode = TriggerMode.Transfer;
        [DesignerEnum("TriggerMode", "TriggerModeDesc", "Event", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, "")]
        public TriggerMode TriggerMode
        {
            get
            {
                return _triggerMode;
            }
            set
            {
                _triggerMode = value;
            }
        }

        protected BehaviorNode _referencedBehavior;
        public BehaviorNode ReferencedBehavior
        {
            get
            {
                return _referencedBehavior;
            }
            set
            {
                _referencedBehavior = value;
                this.SetTask();
            }
        }

        [DesignerString("ReferencedBehaviorFilename", "ReferencedBehaviorFilenameDesc", "Event", DesignerProperty.DisplayMode.NoDisplay, 3, DesignerProperty.DesignerFlags.ReadOnly)]
        public string ReferenceFilename
        {
            get
            {
                if (_referencedBehavior == null)
                {
                    return string.Empty;
                }

                // make the path of the reference relative
                string relativeFilename = _referencedBehavior.MakeRelative(_referencedBehavior.FileManager.Filename);

                // make sure the behaviour filename is still correct
                Debug.Check(_referencedBehavior.MakeAbsolute(relativeFilename) == _referencedBehavior.FileManager.Filename);
                Debug.Check(!Path.IsPathRooted(relativeFilename));
                relativeFilename = relativeFilename.Replace('\\', '/');
                int pos = relativeFilename.IndexOf(".xml");

                if (pos != -1)
                {
                    relativeFilename = relativeFilename.Remove(pos);
                }

                return relativeFilename;
            }
            set
            {
                string absoluteFilename = this.Behavior.MakeAbsolute(value);

                // make sure the behaviour filename is still correct
                Debug.Check(Path.IsPathRooted(absoluteFilename));

                int pos = absoluteFilename.IndexOf(".xml");

                if (pos == -1)
                {
                    absoluteFilename += ".xml";
                }

                if (!File.Exists(absoluteFilename))
                {
                    string info = string.Format(Resources.ReferencedBehaviorError, Label);
                    MessageBox.Show(info, Resources.LoadWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // load the referenced behaviour
                _referencedBehavior = BehaviorManager.Instance.LoadBehavior(absoluteFilename);
                Debug.Check(_referencedBehavior != null);

                Behavior b = this.Behavior as Behavior;
                Debug.Check(b != null);

                b.AgentType.ResetPars(b.LocalVars);

                //((Node)_referencedBehavior).WasModified += new WasModifiedEventDelegate(referencedBehavior_WasModified);
                //_referencedBehavior.WasRenamed += new Behavior.WasRenamedEventDelegate(referencedBehavior_WasRenamed);
                this.SetTask();
            }
        }

        private void SetTask()
        {
            Behaviac.Design.Nodes.Behavior refB = ((Behaviac.Design.Nodes.Behavior)_referencedBehavior);

            if (refB.Children.Count > 0 && refB.Children[0] is Task)
            {
                Task rootTask = refB.Children[0] as Task;

                if (rootTask.Prototype != null)
                {
                    this._task = (MethodDef)rootTask.Prototype.Clone();
                }
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
        [DesignerMethodEnum("TaskPrototype", "TaskPrototypeDesc", "Task", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags | DesignerProperty.DesignerFlags.ReadOnly | DesignerProperty.DesignerFlags.NoDisplayOnProperty | DesignerProperty.DesignerFlags.ReadOnlyParams | DesignerProperty.DesignerFlags.NoSave, MethodType.Task)]
        public MethodDef Task
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

        protected override string GeneratePropertiesLabel()
        {
            string newlabel = string.Empty;

            if (this._task != null)
            {
                newlabel = this._task.PrototypeName;

                if (this._task.Params.Count == 0)
                {
                    newlabel += "()";
                }
            }
            else
            {
                Behavior b = this.ReferencedBehavior as Behavior;
                newlabel = b.Label;
            }

            return newlabel;
        }

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            Event prec = (Event)newattach;

            if (this._referencedBehavior != null)
            {
                prec._referencedBehavior = this._referencedBehavior;
            }

            prec._bTriggeredOnce = this._bTriggeredOnce;
            prec._triggerMode = this._triggerMode;

            if (this._task != null)
            {
                prec._task = (MethodDef)this._task.Clone();
            }
        }

        public override void CheckForErrors(Behaviac.Design.Nodes.BehaviorNode rootBehavior, List<Behaviac.Design.Nodes.Node.ErrorCheck> result)
        {
            if (this._referencedBehavior == null)
            {
                result.Add(new Behaviac.Design.Nodes.Node.ErrorCheck(this.Node, this.Id, this.Label, Behaviac.Design.Nodes.ErrorCheckLevel.Error, "Behavior is not specified!"));
            }

            base.CheckForErrors(rootBehavior, result);
        }

        public override void GetReferencedFiles(ref List<string> referencedFiles)
        {
            string file = this.ReferenceFilename;

            if (!string.IsNullOrEmpty(file) && !referencedFiles.Contains(file))
            {
                referencedFiles.Add(file);
            }
        }
    }
}
