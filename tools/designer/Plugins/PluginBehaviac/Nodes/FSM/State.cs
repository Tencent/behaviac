using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Behaviac.Design;
using PluginBehaviac.Properties;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("FSM:State", "state_icon")]
    public class State : StateBase
    {
        public State()
        : base(Resources.State, Resources.StateDesc)
        {
        }

        public State(string label, string description)
        : base(label, description)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/fsm/";
            }
        }

        [DesignerString("Name", "NameDesc", "State", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoExport)]
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    if (this.Method != null)
                    {
                        string methodName = this.Method.DisplayName;

                        if (string.IsNullOrEmpty(methodName))
                        {
                            methodName = this.Method.BasicName;
                        }

                        return this.Label + "(" + methodName + ")";
                    }

                    return this.Label;
                }

                return _name;
            }

            set
            {
                _name = value;
            }
        }

        protected MethodDef _method = null;
        [DesignerMethodEnum("AgentMethod", "AgentMethodDesc", "State", DesignerProperty.DisplayMode.Parameter, 2, DesignerProperty.DesignerFlags.NoFlags, MethodType.Method | MethodType.AllowNullMethod)]
        public MethodDef Method
        {
            get
            {
                return _method;
            }
            set
            {
                this._method = value;
            }
        }

        private readonly static Brush __defaultBackgroundBrush = new SolidBrush(Color.FromArgb(79, 129, 189));
        protected override Brush DefaultBackgroundBrush
        {
            get
            {
                return __defaultBackgroundBrush;
            }
        }

        public override string ExportClass
        {
            get
            {
                return "State";
            }
        }

        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            return base.CanBeAdoptedBy(parent) && (parent is Behavior) && (parent.IsFSM || (parent.Children.Count == 0)) && (parent.FSMNodes.Count == 0);
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            State state = (State)newnode;

            state._method = this._method;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            base.CheckForErrors(rootBehavior, result);
        }
    }
}
