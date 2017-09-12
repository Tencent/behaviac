using System;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;
using PluginBehaviac.Nodes;

namespace PluginBehaviac.Events
{
    class StartCondition : Behaviac.Design.Attachments.Attach
    {
        public StartCondition(Node node)
        : base(node, Resources.StartCondition, Resources.StartConditionDesc)
        {
        }

        public StartCondition(Node node, string label, string description)
        : base(node, label, description)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/fsm/";
            }
        }

        public override string ExportClass
        {
            get
            {
                return "StartCondition";
            }
        }

        public override bool IsFSM
        {
            get
            {
                return true;
            }
        }

        public override bool IsStartCondition
        {
            get
            {
                return true;
            }
        }

        public override bool IsTransition
        {
            get
            {
                return true;
            }
        }

        public override bool IsEffector
        {
            get
            {
                return true;
            }
        }

        public override bool CanBeDeleted
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeDraggedToTarget
        {
            get
            {
                return true;
            }
        }

        [DesignerInteger("TargetId", "TargetIdDesc", "Transition", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.ReadOnly | DesignerProperty.DesignerFlags.NotPrefabRelated, null, int.MinValue, int.MaxValue, 1, null)]
        public override int TargetFSMNodeId
        {
            get
            {
                return base.TargetFSMNodeId;
            }
            set
            {
                base.TargetFSMNodeId = value;
            }
        }

        protected override string GeneratePropertiesLabel()
        {
            return "Start";
        }

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            StartCondition con = (StartCondition)newattach;

            con.TargetFSMNodeId = this.TargetFSMNodeId;
        }
    }
}
