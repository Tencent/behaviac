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

#ifndef _BEHAVIAC_BEHAVIORTREE_WAITFRAMES_H_
#define _BEHAVIAC_BEHAVIORTREE_WAITFRAMES_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup WaitFrames
    * @{ */

    /**
    Wait for the specified frames. always return Running until exceeds count.
    */
    class IInstanceMember;
    class BEHAVIAC_API WaitFrames : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(WaitFrames, BehaviorNode);

        WaitFrames();
        virtual ~WaitFrames();
        virtual void load(int version, const char* agentType, const properties_t& properties);

    private:
        virtual BehaviorTask* createTask() const;
		virtual int GetFrames(Agent* pAgent) const;

    protected:
        IInstanceMember* m_frames;

        friend class WaitFramesTask;
    };

    class BEHAVIAC_API WaitFramesTask : public LeafTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(WaitFramesTask, LeafTask);

        WaitFramesTask();

    protected:
        virtual ~WaitFramesTask();

        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        int GetFrames(Agent* pAgent) const;

    private:
        int		m_start;
        int		m_frames;
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_WAITFRAMES_H_
