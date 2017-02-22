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

#ifndef _BEHAVIAC_BEHAVIORTREE_NOOP_H_
#define _BEHAVIAC_BEHAVIORTREE_NOOP_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup Noop
    * @{ */

    /**
    Nothing to do, just return success.
    */
    class BEHAVIAC_API Noop : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Noop, BehaviorNode);

        Noop();
        virtual ~Noop();
        virtual void load(int version, const char* agentType, const properties_t& properties);
    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
    private:
        virtual BehaviorTask* createTask() const;
    };

    class BEHAVIAC_API NoopTask : public LeafTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(NoopTask, LeafTask);

        NoopTask() : LeafTask() {
        }

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    protected:
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_NOOP_H_
