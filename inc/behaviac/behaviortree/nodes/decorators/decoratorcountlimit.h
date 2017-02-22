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

#ifndef _BEHAVIAC_BEHAVIORTREE_DECORATORCOUNTLIMIT_H_
#define _BEHAVIAC_BEHAVIORTREE_DECORATORCOUNTLIMIT_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorcount.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup DecoratorCountLimit
    * @{ */

    /**
    DecoratorCountLimit can be set a integer Count limit value. DecoratorCountLimit node tick its child until
    inner count less equal than count limit value. Whether node increase inner count depend on
    the return value of its child when it updates. if DecorateChildEnds flag is true, node increase count
    only when its child node return value is Success or Failure. The inner count will never reset until
    attachment on the node evaluate true.
    */
    class BEHAVIAC_API DecoratorCountLimit : public DecoratorCount {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorCountLimit, DecoratorCount);

        DecoratorCountLimit();
        virtual ~DecoratorCountLimit();
        virtual void load(int version, const char* agentType, const properties_t& properties);
        bool CheckIfReInit(Agent* pAgent);

    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    private:
        virtual BehaviorTask* createTask() const;
    };

    ///enter and tick the child for the specified number of iterations, then it will not enter and tick the child after that
    class BEHAVIAC_API DecoratorCountLimitTask : public DecoratorCountTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorCountLimitTask, DecoratorCountTask);

        DecoratorCountLimitTask();

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual EBTStatus decorate(EBTStatus status);
    private:
        bool m_bInited;
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_DECORATORCOUNTLIMIT_H_
