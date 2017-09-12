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

#ifndef _BEHAVIAC_BEHAVIORTREE_END_H_
#define _BEHAVIAC_BEHAVIORTREE_END_H_

#include "behaviac/common/base.h"
#include "behaviac/common/member.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/agent/agent.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup End
    * @{ */

    /**
    The behavior tree return success or failure.
    */
    class BEHAVIAC_API End : public BehaviorNode {
    public:
		BEHAVIAC_DECLARE_DYNAMIC_TYPE(End, BehaviorNode);

		End();
		virtual ~End();
        virtual void load(int version, const char* agentType, const properties_t& properties);

    private:
        virtual BehaviorTask* createTask() const;

		virtual EBTStatus GetStatus(Agent* pAgent) const;
		bool GetEndOutside() const;

    protected:
		IInstanceMember* m_endStatus;
		bool             m_endOutside;

		friend class EndTask;
    };

	class BEHAVIAC_API EndTask : public LeafTask {
    public:
		BEHAVIAC_DECLARE_DYNAMIC_TYPE(EndTask, LeafTask);

    protected:
        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

		EBTStatus GetStatus(Agent* pAgent) const;
		bool      GetEndOutside() const;
	};

    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_END_H_
