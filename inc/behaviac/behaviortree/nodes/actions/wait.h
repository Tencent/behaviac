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

#ifndef _BEHAVIAC_BEHAVIORTREE_WAIT_H_
#define _BEHAVIAC_BEHAVIORTREE_WAIT_H_

#include "behaviac/common/base.h"
#include "behaviac/common/member.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/agent/agent.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup Wait
    * @{ */

    /**
    Wait for the specified milliseconds. always return Running until time over.
    */
    class BEHAVIAC_API Wait : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Wait, BehaviorNode);

        Wait();
        virtual ~Wait();
        virtual void load(int version, const char* agentType, const properties_t& properties);

        virtual double GetTime(Agent* pAgent) const;
        virtual int GetIntTime(Agent* pAgent) const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        IInstanceMember* m_time;

        friend class WaitTask;
    };

    class BEHAVIAC_API WaitTask : public LeafTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(WaitTask, LeafTask);

        WaitTask();

    protected:
        virtual ~WaitTask();

        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        double		GetTime(Agent* pAgent) const;
        int			GetIntTime(Agent* pAgent) const;

        double		m_start;
        double		m_time;
        long long	m_intStart;
        int			m_intTime;
    };

    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_WAIT_H_
