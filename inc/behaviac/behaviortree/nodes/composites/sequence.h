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

#ifndef _BEHAVIAC_BEHAVIORTREE_SEQUENCE_H_
#define _BEHAVIAC_BEHAVIORTREE_SEQUENCE_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup Sequence
    * @{ */

    ///Execute behaviors from first to last
    /**
    Sequences tick each of their children one at a time from top to bottom. If a child returns Failure,
    so does the Sequence. If it returns Success, the Sequence will move on to the next child in line
    and return Running.If a child returns Running, so does the Sequence and that same child will be
    ticked again next time the Sequence is ticked.Once the Sequence reaches the end of its child list,
    it returns Success and resets its child index meaning the first child in the line will be ticked
    on the next tick of the Sequence.
    */
    class BEHAVIAC_API Sequence : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Sequence, BehaviorNode);

        Sequence();
        virtual ~Sequence();
#if BEHAVIAC_USE_HTN
        virtual bool decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner);
#endif //BEHAVIAC_USE_HTN
        virtual void load(int version, const char* agentType, const properties_t& properties);
        EBTStatus SequenceUpdate(Agent* pAgent, EBTStatus childStatus, int& activeChildIndex, behaviac::vector<BehaviorTask*>& children);
        bool CheckIfInterrupted(Agent* pAgent);
        virtual bool Evaluate(Agent* pAgent);

    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    private:
        virtual BehaviorTask* createTask() const;
    };

    class BEHAVIAC_API SequenceTask : public CompositeTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SequenceTask, CompositeTask);
        SequenceTask();
        virtual ~SequenceTask();

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_SEQUENCE_H_
