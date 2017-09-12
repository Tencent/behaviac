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
    public class FSMReferencedBehavior : ReferencedBehavior
    {
        public FSMReferencedBehavior(BehaviorNode rootBehavior, BehaviorNode referencedBehavior)
        : base(rootBehavior, referencedBehavior)
        {
        }

        public FSMReferencedBehavior()
        : base()
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/fsm/";
            }
        }

        public override bool IsFSM
        {
            get
            {
                return true;
            }
        }

        public override bool AcceptsAttachment(DefaultObject obj)
        {
            return (obj != null) && (obj is Attachment);
        }

        [DesignerFloat("LocationX", "LocationX", "State", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport)]
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

        [DesignerFloat("LocationY", "LocationY", "State", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoDisplay | DesignerProperty.DesignerFlags.NoExport)]
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

        public override Attachment CreateStartCondition(BaseNode owner)
        {
            return Attachment.Create(typeof(PluginBehaviac.Events.StartCondition), owner as Node);
        }
    }
}
