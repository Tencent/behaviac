using System;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design.Nodes;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Nodes
{
    public class DecoratorTrue : Decorator
    {
        public DecoratorTrue() : base(Resources.DecoratorTrue, Resources.DecoratorTrueDesc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "DecoratorAlwaysTrue";
            }
        }
    }
}
