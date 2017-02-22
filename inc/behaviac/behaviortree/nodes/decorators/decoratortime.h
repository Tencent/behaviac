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

#ifndef _BEHAVIAC_BEHAVIORTREE_DECORATORTIME_H_
#define _BEHAVIAC_BEHAVIORTREE_DECORATORTIME_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup DecoratorTime
    * @{ */

    /**
    It returns Running result until it reaches the time limit specified, no matter which
    value its child return. Or return the child's value.
    */
    class BEHAVIAC_API DecoratorTime : public DecoratorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorTime, DecoratorNode);

        DecoratorTime();
        virtual ~DecoratorTime();
        virtual void load(int version, const char* agentType, const properties_t& properties);

        virtual double GetTime(Agent* pAgent) const;
        virtual int GetIntTime(Agent* pAgent) const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        IInstanceMember* m_time;

        friend class DecoratorTimeTask;
    };

    class BEHAVIAC_API DecoratorTimeTask : public DecoratorTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorTimeTask, DecoratorTask);

        DecoratorTimeTask();

    protected:
        virtual ~DecoratorTimeTask();

        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual EBTStatus decorate(EBTStatus status);

        double		GetTime(Agent* pAgent) const;
        int			GetIntTime(Agent* pAgent) const;

    private:
        double		m_start;
        double		m_time;
        long long	m_intStart;
        int			m_intTime;
    };

    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_DECORATORTIME_H_
