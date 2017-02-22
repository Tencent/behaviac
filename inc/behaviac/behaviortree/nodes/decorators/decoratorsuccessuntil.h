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

#ifndef _BEHAVIAC_BEHAVIORTREE_DECORATORSUCCESSUNTIL_H_
#define _BEHAVIAC_BEHAVIORTREE_DECORATORSUCCESSUNTIL_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorcount.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup DecoratorSuccessUntil
    * @{ */

    /**
    UntilFailureUntil node always return Success until it reaches a specified number of count.
    when reach time exceed the count specified return Failure. If the specified number of count
    is -1, then always return Success.
    */
    class BEHAVIAC_API DecoratorSuccessUntil : public DecoratorCount {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorSuccessUntil, DecoratorCount);

        DecoratorSuccessUntil();
        virtual ~DecoratorSuccessUntil();
        virtual void load(int version, const char* agentType, const properties_t& properties);
    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
    private:
        virtual BehaviorTask* createTask() const;
    };

    ///Returns BT_SUCCESS for the specified number of iterations, then returns BT_FAILURE after that
    class BEHAVIAC_API DecoratorSuccessUntilTask : public DecoratorCountTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorSuccessUntilTask, DecoratorCountTask);

        DecoratorSuccessUntilTask() : DecoratorCountTask() {
        }

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual void onreset(Agent* pAgent);
        virtual bool onenter(Agent* pAgent);
        virtual EBTStatus decorate(EBTStatus status);

        //virtual bool NeedRestart() const;
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_DECORATORSUCCESSUNTIL_H_
