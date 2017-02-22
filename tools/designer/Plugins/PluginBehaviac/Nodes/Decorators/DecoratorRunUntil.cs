using System;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("Decorators", NodeIcon.Decorator)]
    public class DecoratorRunUntil : Decorator
    {
        public DecoratorRunUntil()
        : base(Resources.DecoratorRunUntil, Resources.DecoratorRunUntilDesc)
        {
        }


        public override string ExportClass
        {
            get
            {
                return "DecoratorRunUntil";
            }
        }


        protected bool _until_false = false;
        [DesignerBoolean("DecoratorUntilFalse", "DecoratorUntilFalseDesc", "CategoryBasic", DesignerProperty.DisplayMode.List, 0, DesignerProperty.DesignerFlags.NoFlags)]
        public bool UntilFalse
        {
            get
            {
                return _until_false;
            }
            set
            {
                _until_false = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);
        }

    }
}
