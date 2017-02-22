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

#ifndef _BEHAVIAC_BEHAVIORTREE_DECORATORWEIGHT_H_
#define _BEHAVIAC_BEHAVIORTREE_DECORATORWEIGHT_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    class IInstanceMember;
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup DecoratorWeight
    * @{ */

    /**
    Specified the weight value of SelectorProbability child node.
    */
    class BEHAVIAC_API DecoratorWeight : public DecoratorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorWeight, DecoratorNode);

        DecoratorWeight();
        virtual ~DecoratorWeight();
        virtual void load(int version, const char* agentType, const properties_t& properties);

        virtual int GetWeight(behaviac::Agent* pAgent) const;
        virtual bool IsManagingChildrenAsSubTrees() const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        IInstanceMember*	m_weight;

        friend class DecoratorWeightTask;
    };

    class BEHAVIAC_API DecoratorWeightTask : public DecoratorTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorWeightTask, DecoratorTask);

        DecoratorWeightTask() : DecoratorTask() {
        }

        int GetWeight(Agent* pAgent) const;

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual EBTStatus decorate(EBTStatus status);
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_DECORATORWEIGHT_H_
