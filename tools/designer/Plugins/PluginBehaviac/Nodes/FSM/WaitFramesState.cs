using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;
using Behaviac.Design.Attachments;
using PluginBehaviac.Events;
using PluginBehaviac.Properties;

namespace PluginBehaviac.Nodes
{
    [NodeDesc("FSM:State", NodeIcon.Wait)]
    public class WaitFramesState : StateBase
    {
        public WaitFramesState()
        : base(Resources.WaitFramesState, Resources.WaitFramesStateDesc)
        {
            this.Name = Resources.WaitFramesState;
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/fsm/";
            }
        }

        public override void PostCreatedByEditor()
        {
            Attachment attach = Attachment.Create(typeof(WaitTransition), this);
            attach.ResetId();
            this.AddAttachment(attach);
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
                return "WaitFramesState";
            }
        }

        protected override bool CanBeAdoptedBy(BaseNode parent)
        {
            return base.CanBeAdoptedBy(parent) && (parent is Behavior) && (parent.IsFSM || (parent.Children.Count == 0)) && (parent.FSMNodes.Count == 0);
        }

        public override bool AcceptsAttachment(DefaultObject obj)
        {
            return (obj != null) && (obj is WaitTransition);
        }

        private RightValueDef _frames;
        [DesignerRightValueEnum("Frames", "FramesDesc", "WaitFramesState", DesignerProperty.DisplayMode.Parameter, 0, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Int)]
        public RightValueDef Frames
        {
            get
            {
                return _frames;
            }
            set
            {
                _frames = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            WaitFramesState dec = (WaitFramesState)newnode;

            if (_frames != null)
            {
                dec._frames = (RightValueDef)_frames.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            Type valueType = this._frames.ValueType;

            string typeName = Plugin.GetNativeTypeName(valueType.FullName);

            if (Plugin.IsIntergerNumberType(typeName))
            { }
            else
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Frames should be an integer number type!"));
            }

            base.CheckForErrors(rootBehavior, result);
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (this._frames != null)
            {
                bReset |= this._frames.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

            return bReset;
        }

    }
}
