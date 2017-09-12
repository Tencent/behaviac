using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;
using PluginBehaviac.Properties;
using PluginBehaviac.Nodes;

namespace PluginBehaviac.Events
{
    class WaitTransition : StartCondition
    {
        public WaitTransition(Node node)
        : base(node, Resources.WaitTransition, Resources.TransitionConditionDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/fsm/";
            }
        }

        public WaitTransition(Node node, string label, string desc)
        : base(node, label, desc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "WaitTransition";
            }
        }

        protected override string GeneratePropertiesLabel()
        {
            return "Transition";
        }

        public override bool CanBeDisabled()
        {
            return true;
        }

        public override bool IsStartCondition
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeDeleted
        {
            get
            {
                return false;
            }
        }
    }
}
