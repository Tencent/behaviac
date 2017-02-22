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
    public class StateBase : Behaviac.Design.Nodes.Node
    {
        public StateBase(string label, string description)
        : base(label, description)
        {
        }

        public override bool IsFSM
        {
            get
            {
                return true;
            }
        }

        protected string _name = "";

        [DesignerString("Name", "NameDesc", "State", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoExport)]
        public virtual string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        protected bool _bEndState = false;
        [DesignerBoolean("IsEndState", "IsEndStateDesc", "State", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags)]
        public bool IsEndState
        {
            get
            {
                return _bEndState;
            }
            set
            {
                _bEndState = value;
            }
        }

        public override Attachment CreateStartCondition(BaseNode owner)
        {
            return Attachment.Create(typeof(PluginBehaviac.Events.StartCondition), owner as Node);
        }

        [DesignerFloat("LocationX", "LocationX", "State", DesignerProperty.DisplayMode.NoDisplay, 20, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport)]
        public float ScreenLocationX
        {
            get
            {
                return _screenLocation.X;
            }
            set
            {
                _screenLocation.X = value;
            }
        }

        [DesignerFloat("LocationY", "LocationY", "State", DesignerProperty.DisplayMode.NoDisplay, 21, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport)]
        public float ScreenLocationY
        {
            get
            {
                return _screenLocation.Y;
            }
            set
            {
                _screenLocation.Y = value;
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

        public override string GenerateNewLabel()
        {
            return this.Name;
        }

        public override NodeViewData CreateNodeViewData(NodeViewData parent, BehaviorNode rootBehavior)
        {
            NodeViewData nvd = base.CreateNodeViewData(parent, rootBehavior);
            nvd.ChangeShape(this.IsEndState ? NodeShape.RoundedRectangle : NodeShape.Rectangle);

            return nvd;
        }

        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            return base.CanBeAdoptedBy(parent) && (parent is Behavior) && (parent.IsFSM || (parent.Children.Count == 0)) && (parent.FSMNodes.Count == 0);
        }

        public override bool AcceptsAttachment(DefaultObject obj)
        {
            return (obj != null) && (obj is Attachment);
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            StateBase state = (StateBase)newnode;

            state.Name = this.Name;
            state.ScreenLocationX = this.ScreenLocationX;
            state.ScreenLocationY = this.ScreenLocationY;
        }

        private bool isPointedByOther()
        {
            if (this.Parent.FSMNodes.Count > 0)
            {
                if (this.Id == this.Behavior.InitialStateId)
                {
                    return true;
                }

                foreach (Node node in this.Parent.FSMNodes)
                {
                    foreach (Attachment attachment in node.Attachments)
                    {
                        if (attachment.IsFSM && attachment.TargetFSMNodeId == this.Id)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            base.CheckForErrors(rootBehavior, result);

            if (string.IsNullOrEmpty(this.Name))
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "No name!"));
            }

            if (!this.isPointedByOther())
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Warning, "This state is not transited by anyone!"));
            }
        }
    }
}
