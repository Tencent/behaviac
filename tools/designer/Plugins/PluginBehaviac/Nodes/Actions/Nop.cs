using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Behaviac.Design;
using Behaviac.Design.Attributes;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Nodes
{
    public class Nop : Behaviac.Design.Nodes.Node
    {
        public Nop()
        : base(Resources.Nop, Resources.NopDesc)
        {
            _exportName = "Nop";
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(157, 75, 39));
        public override NodeViewData CreateNodeViewData(NodeViewData parent, Behaviac.Design.Nodes.BehaviorNode rootBehavior)
        {
            NodeViewDataStyled nvd = new NodeViewDataStyled(parent, rootBehavior, this, null, __defaultBackgroundBrush, _label, _description);

            nvd.ChangeShape(NodeShape.Rectangle);

            return nvd;
        }
    }
}
