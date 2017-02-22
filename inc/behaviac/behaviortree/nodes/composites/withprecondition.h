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

#ifndef _BEHAVIAC_BEHAVIORTREE_WITHPRECONDITION_H_
#define _BEHAVIAC_BEHAVIORTREE_WITHPRECONDITION_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/behaviortree/nodes/composites/sequence.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup SelectorStochastic
    * @{ */

    /**
    WithPrecondition is the precondition of SelectorLoop child. must be used in conjunction with SelectorLoop.
    WithPrecondition can return SUCCESS or FAILURE. child would execute when it returns SUCCESS, or not.
    */
    class BEHAVIAC_API WithPrecondition : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(WithPrecondition, BehaviorNode);

        WithPrecondition();
        virtual ~WithPrecondition();
        virtual void load(int version, const char* agentType, const properties_t& properties);
    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
    private:
        virtual BehaviorTask* createTask() const;
    };

    class BEHAVIAC_API WithPreconditionTask : public SequenceTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(WithPreconditionTask, SequenceTask);
        WithPreconditionTask() : SequenceTask() {
        }
        virtual void addChild(BehaviorTask* pBehavior);

        BehaviorTask* PreconditionNode() const;
        BehaviorTask* ActionNode() const;
    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    private:
        friend class SelectorLoopTask;
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_WITHPRECONDITION_H_
