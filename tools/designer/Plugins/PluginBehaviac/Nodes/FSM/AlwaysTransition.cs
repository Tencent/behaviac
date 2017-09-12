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
    [Behaviac.Design.EnumDesc("PluginBehaviac.Events.ETransitionPhase", "TransitionPhase", "跳转时机")]
    public enum ETransitionPhase
    {
        [Behaviac.Design.EnumMemberDesc("Always", "总跳转")]
        ETP_Always,

        [Behaviac.Design.EnumMemberDesc("Success", "成功时")]
        ETP_Success,

        [Behaviac.Design.EnumMemberDesc("Failure", "失败时")]
        ETP_Failure,

        [Behaviac.Design.EnumMemberDesc("Exit", "结束时")]
        ETP_Exit,

    }

    [NodeDesc("FSM:Transition", "transition_icon")]
    class AlwaysTransition : StartCondition
    {
        public AlwaysTransition(Node node)
        : base(node, Resources.StatusTransition, Resources.StatusTransitionDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/fsm/";
            }
        }

        public AlwaysTransition(Node node, string label, string desc)
        : base(node, label, desc)
        {
        }

        public override string ExportClass
        {
            get
            {
                return "AlwaysTransition";
            }
        }

        private ETransitionPhase _statusPhase = ETransitionPhase.ETP_Always;
        [DesignerEnum("TransitionPhase", "TransitionPhaseDesc", "StatusTransition", DesignerProperty.DisplayMode.Parameter, 3, DesignerProperty.DesignerFlags.NoFlags, "AlwaysTransition")]
        public ETransitionPhase TransitionPhase
        {
            get
            {
                return _statusPhase;
            }
            set
            {
                _statusPhase = value;
            }
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.AlwaysTransitionUIPolicy();
        }


        protected override string GeneratePropertiesLabel()
        {
            string statusStr = this._statusPhase.ToString();
            System.Reflection.FieldInfo fi = this._statusPhase.GetType().GetField(statusStr);
            Attribute[] attributes = (Attribute[])fi.GetCustomAttributes(typeof(EnumMemberDescAttribute), false);

            if (attributes.Length > 0)
            {
                statusStr = ((EnumMemberDescAttribute)attributes[0]).DisplayName;
            }

            return statusStr;
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
                return true;
            }
        }

        protected override void CloneProperties(Behaviac.Design.Attachments.Attachment newattach)
        {
            base.CloneProperties(newattach);

            AlwaysTransition con = (AlwaysTransition)newattach;

            con.TargetFSMNodeId = this.TargetFSMNodeId;

            con._statusPhase = this._statusPhase;
        }

        public override void CheckForErrors(BehaviorNode rootBehavior, List<Node.ErrorCheck> result)
        {
            bool bIsSubTree = this.Node is PluginBehaviac.Nodes.FSMReferencedBehavior;

            if (!bIsSubTree && this._statusPhase != ETransitionPhase.ETP_Always)
            {
                result.Add(new Node.ErrorCheck(this.Node, ErrorCheckLevel.Error, "only always is supported for non subtree nodes"));
            }

            base.CheckForErrors(rootBehavior, result);
        }

    }
}
