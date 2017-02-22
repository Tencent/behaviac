using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using PluginBehaviac.Properties;


namespace PluginBehaviac.Nodes
{
    public class CompoundAction : Node
    {
        private readonly static Brush __theBackgroundBrush = new SolidBrush(Color.FromArgb(39, 142, 157));

        protected ConnectorMultiple _preconditions;
        protected ConnectorMultiple _methods;

        public CompoundAction()
        : base(Resources.CompoundAction, Resources.CompoundActionDesc)
        {
            _preconditions = new ConnectorCondition(_children, "Precondition {0}", "Preconditions", 1, int.MaxValue);
            _methods = new ConnectorMultiple(_children, "Action {0}", "Actions", 1, int.MaxValue);
        }

        public override NodeViewData CreateNodeViewData(NodeViewData parent, BehaviorNode rootBehavior)
        {
            return new NodeViewDataCompoundTask(parent, rootBehavior, this, null, __theBackgroundBrush, _label, _description);
        }


        protected override void CloneProperties(Behaviac.Design.Nodes.Node newnode)
        {
            base.CloneProperties(newnode);
        }

        public override void OnPropertyValueChanged(bool wasModified)
        {
            base.OnPropertyValueChanged(wasModified);
        }
    }
}
