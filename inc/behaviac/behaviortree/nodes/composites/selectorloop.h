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

#ifndef _BEHAVIAC_BEHAVIORTREE_SELECTORLOOP_H_
#define _BEHAVIAC_BEHAVIORTREE_SELECTORLOOP_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup SelectorLoop
    * @{ */

    /**
    Behavives similarly to SelectorTask, i.e. executing chidren until the first successful one.
    however, in the following ticks, it constantly monitors the higher priority nodes.if any
    one's precondtion node returns success, it picks it and execute it, and before executing,
    it first cleans up the original executing one. all its children are WithPreconditionTask
    or its derivatives.
    */
    class BEHAVIAC_API SelectorLoop : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SelectorLoop, BehaviorNode);

        SelectorLoop();
        virtual ~SelectorLoop();
        virtual void load(int version, const char* agentType, const properties_t& properties);
        virtual bool IsManagingChildrenAsSubTrees() const;

    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
		bool    m_bResetChildren;

		friend class SelectorLoopTask;
    };

    class BEHAVIAC_API SelectorLoopTask : public CompositeTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SelectorLoopTask, CompositeTask);
        SelectorLoopTask();
        virtual ~SelectorLoopTask();

        virtual void Init(const BehaviorNode* node);
        virtual void addChild(BehaviorTask* pBehavior);
    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_SELECTORLOOP_H_
