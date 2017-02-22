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

#ifndef _BEHAVIAC_BEHAVIORTREE_SELECTORPROBABILITY_H_
#define _BEHAVIAC_BEHAVIORTREE_SELECTORPROBABILITY_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup SelectorProbability
    * @{ */

    ///Pick a child to Execute
    /**
    Choose a child to execute based on the probability have set. then return the child execute result.
    */
    class BEHAVIAC_API SelectorProbability : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SelectorProbability, BehaviorNode);

        SelectorProbability();
        virtual ~SelectorProbability();
        virtual void load(int version, const char* agentType, const properties_t& properties);
        virtual void AddChild(BehaviorNode* pBehavior);

    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        behaviac::IInstanceMember*		m_method;

        friend class SelectorProbabilityTask;
    };

    ///Executes behaviors randomly, based on a given set of weights.
    /** The weights are not percentages, but rather simple ratios.
    For example, if there were two children with a weight of one, each would have a 50% chance of being executed.
    If another child with a weight of eight were added, the previous children would have a 10% chance of being executed, and the new child would have an 80% chance of being executed.
    This weight system is intended to facilitate the fine-tuning of behaviors.
    */
    class BEHAVIAC_API SelectorProbabilityTask : public CompositeTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SelectorProbabilityTask, CompositeTask);

        SelectorProbabilityTask();
        ~SelectorProbabilityTask();

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        behaviac::vector<int>	m_weightingMap;
        int					m_totalSum;
    };
    /*! @} */
    /*! @} */
}//namespace behaviac

#endif//_BEHAVIAC_BEHAVIORTREE_SELECTORPROBABILITY_H_
