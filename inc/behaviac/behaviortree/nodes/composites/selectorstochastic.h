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

#ifndef _BEHAVIAC_BEHAVIORTREE_SELECTORSTOCHASTIC_H_
#define _BEHAVIAC_BEHAVIORTREE_SELECTORSTOCHASTIC_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/behaviortree/nodes/composites/compositestochastic.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup SelectorStochastic
    * @{ */

    /**
    the Selector runs the children from the first sequentially until the child which returns success.
    for SelectorStochastic, the children are not sequentially selected, instead it is selected stochasticly.

    for example: the children might be [0, 1, 2, 3, 4]
    Selector always select the child by the order of 0, 1, 2, 3, 4
    while SelectorStochastic, sometime, it is [4, 2, 0, 1, 3], sometime, it is [2, 3, 0, 4, 1], etc.
    */
    class BEHAVIAC_API SelectorStochastic : public CompositeStochastic {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SelectorStochastic, CompositeStochastic);

        SelectorStochastic();
        virtual ~SelectorStochastic();
        virtual void load(int version, const char* agentType, const properties_t& properties);
		bool CheckIfInterrupted(const Agent* pAgent);
    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
    private:
        virtual BehaviorTask* createTask() const;
    };

    class BEHAVIAC_API SelectorStochasticTask : public CompositeStochasticTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SelectorStochasticTask, CompositeStochasticTask);
        SelectorStochasticTask() : CompositeStochasticTask()
        {}

        virtual void addChild(BehaviorTask* pBehavior);
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

#endif//_BEHAVIAC_BEHAVIORTREE_SELECTORSTOCHASTIC_H_
