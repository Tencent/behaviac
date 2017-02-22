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

#ifndef _BEHAVIAC_BEHAVIORTREE_GETTER_H_
#define _BEHAVIAC_BEHAVIORTREE_GETTER_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/agent/agent.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup Assignment
    * @{ */

    ///Assignment
    /**
    Assign a right value to left par or agent property. a right value can be a par or agent property.
    */
    class BEHAVIAC_API Assignment : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Assignment, BehaviorNode);

        Assignment();
        virtual ~Assignment();
        virtual void load(int version, const char* agentType, const properties_t& properties);

        //static bool EvaluteAssignment(const Agent* pAgent, IInstanceMember* opl, IInstanceMember* opr, behaviac::CMethodBase* opr_m);

    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        IInstanceMember*			m_opl;
        IInstanceMember*			m_opr;
        bool				        m_bCast;

        friend class AssignmentTask;
    };

    class BEHAVIAC_API AssignmentTask : public LeafTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(AssignmentTask, LeafTask);

        AssignmentTask();
        virtual ~AssignmentTask();

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_GETTER_H_
