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
    public class WaitState : StateBase
    {
        public WaitState()
        : base(Resources.WaitState, Resources.WaitStateDesc)
        {
            this.Name = Resources.WaitState;
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
                return "WaitState";
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

        protected RightValueDef _time = new RightValueDef(new VariableDef(1000.0f));
        [DesignerRightValueEnum("Duration", "DurationDesc", "WaitState", DesignerProperty.DisplayMode.Parameter, 1, DesignerProperty.DesignerFlags.NoFlags, DesignerPropertyEnum.AllowStyles.ConstAttributesMethod, MethodType.Getter, "", "", ValueTypes.Float)]
        public RightValueDef Time
        {
            get
            {
                return _time;
            }
            set
            {
                this._time = value;
            }
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            WaitState dec = (WaitState)newnode;

            if (_time != null)
            {
                dec._time = (RightValueDef)_time.Clone();
            }
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<ErrorCheck> result)
        {
            Type valueType = (this._time != null) ? this._time.ValueType : null;

            if (valueType == null)
            {
                result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Time is not set!"));
            }
            else
            {
                if (!Plugin.IsIntergerType(valueType) && !Plugin.IsFloatType(valueType))
                {
                    result.Add(new Node.ErrorCheck(this, ErrorCheckLevel.Error, "Time should be a float number type!"));
                }
            }

            base.CheckForErrors(rootBehavior, result);
        }

        public override bool ResetMembers(MetaOperations metaOperation, AgentType agentType, BaseType baseType, MethodDef method, PropertyDef property)
        {
            bool bReset = false;

            if (this._time != null)
            {
                bReset |= this._time.ResetMembers(metaOperation, agentType, baseType, method, property);
            }

            bReset |= base.ResetMembers(metaOperation, agentType, baseType, method, property);

            return bReset;
        }

    }
}
