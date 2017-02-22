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

#ifndef _BEHAVIAC_BEHAVIORTREE_DECORATORRUNUNTIL_H_
#define _BEHAVIAC_BEHAVIORTREE_DECORATORRUNUNTIL_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorcount.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup DecoratorLoopUntil
    * @{ */

    /**
    DecoratorLoopUntil can be set two conditions, loop count 'C' and a return value 'R'.
    if current update count less equal than 'C' and child return value not equal to 'R',
    it returns Running. Or returns child value.
    */
    class BEHAVIAC_API DecoratorLoopUntil : public DecoratorCount {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorLoopUntil, DecoratorCount);

        DecoratorLoopUntil();
        virtual ~DecoratorLoopUntil();

        virtual void load(int version, const char* agentType, const properties_t& properties);

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        bool m_until;

        friend class DecoratorLoopUntilTask;
    };

    ///Returns BT_RUNNING until the child returns BT_SUCCESS. if the child returns BT_FAILURE, it still returns BT_RUNNING
    /**
    however, if m_until is false, the checking condition is inverted.
    i.e. it Returns BT_RUNNING until the child returns BT_FAILURE. if the child returns BT_SUCCESS, it still returns BT_RUNNING
    */
    class BEHAVIAC_API DecoratorLoopUntilTask : public DecoratorCountTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorLoopUntilTask, DecoratorCountTask);

        DecoratorLoopUntilTask() : DecoratorCountTask() {
        }

    protected:
        //virtual bool NeedRestart() const;
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual EBTStatus decorate(EBTStatus status);
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_DECORATORRUNUNTIL_H_
