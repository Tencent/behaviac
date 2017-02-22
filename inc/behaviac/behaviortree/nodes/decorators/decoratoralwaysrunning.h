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

#ifndef _BEHAVIAC_BEHAVIORTREE_DECORATORALWAYSRUNNING_H_
#define _BEHAVIAC_BEHAVIORTREE_DECORATORALWAYSRUNNING_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup DecoratorAlwaysRunning
    * @{ */

    /**
    No matter what child return. DecoratorAlwaysRunning always return Running. it can only has one child node.
    */
    class BEHAVIAC_API DecoratorAlwaysRunning : public DecoratorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorAlwaysRunning, DecoratorNode);

        DecoratorAlwaysRunning();
        virtual ~DecoratorAlwaysRunning();
        virtual void load(int version, const char* agentType, const properties_t& properties);
    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
    private:
        virtual BehaviorTask* createTask() const;
    };

    class BEHAVIAC_API DecoratorAlwaysRunningTask : public DecoratorTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorAlwaysRunningTask, DecoratorTask);

        DecoratorAlwaysRunningTask() : DecoratorTask() {
        }

        virtual void addChild(BehaviorTask* pBehavior);
    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual EBTStatus decorate(EBTStatus status);
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_DECORATORALWAYSRUNNING_H_
