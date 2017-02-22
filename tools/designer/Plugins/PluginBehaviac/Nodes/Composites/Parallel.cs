/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Behaviac.Design;
using PluginBehaviac.Properties;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;

namespace PluginBehaviac.Nodes
{
    [Behaviac.Design.EnumDesc("PluginBehaviac.Nodes.FailurePolicy", "FailurePolicy", "FailurePolicy����ָ����ʲô����½ڵ�ʧ��")]
    public enum FailurePolicy
    {
        [Behaviac.Design.EnumMemberDesc("FAIL_ON_ONE", "FAIL_ON_ONE", "����һ���ڵ�ʧ����ʧ��")]
        FAIL_ON_ONE,

        [Behaviac.Design.EnumMemberDesc("FAIL_ON_ALL", "FAIL_ON_ALL", "���нڵ�ʧ�ܲ�ʧ��")]
        FAIL_ON_ALL
    };

    [Behaviac.Design.EnumDesc("PluginBehaviac.Nodes.SuccessPolicy", "SuccessPolicy", "SuccessPolicy����ָ����ʲô����½ڵ�ɹ�")]
    public enum SuccessPolicy
    {
        [Behaviac.Design.EnumMemberDesc("SUCCEED_ON_ONE", "SUCCEED_ON_ONE", "����һ���ڵ�ɹ���ɹ�")]
        SUCCEED_ON_ONE,

        [Behaviac.Design.EnumMemberDesc("SUCCEED_ON_ALL", "SUCCEED_ON_ALL", "���нڵ�ɹ��ųɹ�")]
        SUCCEED_ON_ALL
    };

    [Behaviac.Design.EnumDesc("PluginBehaviac.Nodes.ExitPolicy", "ExitPolicy", "ExitPolicy����ָ���ڵ��˳���ʱ����ʲô")]
    public enum ExitPolicy
    {
        [Behaviac.Design.EnumMemberDesc("EXIT_NONE", "EXIT_NONE", "�ڵ��˳�ʱʲô������")]
        EXIT_NONE,

        [Behaviac.Design.EnumMemberDesc("EXIT_ABORT_RUNNINGSIBLINGS", "EXIT_ABORT_RUNNINGSIBLINGS", "�ڵ��˳�ʱ�����ӽڵ㶼ǿ���˳�")]
        EXIT_ABORT_RUNNINGSIBLINGS
    };

    [Behaviac.Design.EnumDesc("PluginBehaviac.Nodes.ChildFinishPolicy", "ChildFinishPolicy", "ChildFinishPolicy����ָ���ӽڵ��˳�������ýڵ㻹��ִ�����ӽڵ���μ���")]
    public enum ChildFinishPolicy
    {
        [Behaviac.Design.EnumMemberDesc("CHILDFINISH_ONCE", "CHILDFINISH_ONCE", "�ӽڵ��˳��󣬼�ʹ�ýڵ㻹��ִ���ӽڵ�Ҳ����ִ��")]
        CHILDFINISH_ONCE,

        [Behaviac.Design.EnumMemberDesc("CHILDFINISH_LOOP", "CHILDFINISH_LOOP", "�ӽڵ��˳�������ýڵ㻹��ִ�����ӽڵ��ѭ��ִ��")]
        CHILDFINISH_LOOP
    };

    [NodeDesc("Composites", NodeIcon.Parallel)]
    public class Parallel : Behaviac.Design.Nodes.Sequence
    {
        public Parallel()
        : base(Resources.Parallel, Resources.ParallelDesc)
        {
        }

        public override string DocLink
        {
            get
            {
                return "http://www.behaviac.com/language/zh/parallel/";
            }
        }

        protected override void CreateInterruptChild()
        {
        }

        public override string ExportClass
        {
            get
            {
                return "Parallel";
            }
        }

        protected FailurePolicy _failure_policy = FailurePolicy.FAIL_ON_ONE;
        [DesignerEnum("FailurePolicy", "FailurePolicyDesc", "Parallel", DesignerProperty.DisplayMode.NoDisplay, 0, DesignerProperty.DesignerFlags.NoFlags, "")]
        public FailurePolicy FailurePolicy
        {
            get
            {
                return _failure_policy;
            }
            set
            {
                _failure_policy = value;
            }
        }

        protected SuccessPolicy _success_policy = SuccessPolicy.SUCCEED_ON_ALL;
        [DesignerEnum("SuccessPolicy", "SuccessPolicyDesc", "Parallel", DesignerProperty.DisplayMode.NoDisplay, 1, DesignerProperty.DesignerFlags.NoFlags, "")]
        public SuccessPolicy SuccessPolicy
        {
            get
            {
                return _success_policy;
            }
            set
            {
                _success_policy = value;
            }
        }

        protected ExitPolicy _exit_policy = ExitPolicy.EXIT_ABORT_RUNNINGSIBLINGS;
        [DesignerEnum("ExitPolicy", "ExitPolicyDesc", "Parallel", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags, "")]
        public ExitPolicy ExitPolicy
        {
            get
            {
                return _exit_policy;
            }
            set
            {
                _exit_policy = value;
            }
        }

        protected ChildFinishPolicy _childfinishpolicy = ChildFinishPolicy.CHILDFINISH_LOOP;
        [DesignerEnum("ChildFinishPolicy", "ChildFinishPolicyDesc", "Parallel", DesignerProperty.DisplayMode.NoDisplay, 2, DesignerProperty.DesignerFlags.NoFlags, "")]
        public ChildFinishPolicy ChildFinishPolicy
        {
            get
            {
                return _childfinishpolicy;
            }
            set
            {
                _childfinishpolicy = value;
            }
        }

        public override Behaviac.Design.ObjectUI.ObjectUIPolicy CreateUIPolicy()
        {
            return new Behaviac.Design.ObjectUI.ParallelUIPolicy();
        }

        protected override void CloneProperties(Node newnode)
        {
            base.CloneProperties(newnode);

            Parallel dec = (Parallel)newnode;
            dec._failure_policy = _failure_policy;
            dec._success_policy = _success_policy;
            dec._exit_policy = _exit_policy;
        }
    }
}
