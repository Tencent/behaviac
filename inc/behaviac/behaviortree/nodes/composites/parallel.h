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

#ifndef _BEHAVIAC_BEHAVIORTREE_PARALLEL_H_
#define _BEHAVIAC_BEHAVIORTREE_PARALLEL_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

#include <vector>

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup Parallel
    * @{ */

    // options when a parallel node is considered to be failed.
    // FAIL_ON_ONE: the node fails as soon as one of its children fails.
    // FAIL_ON_ALL: the node failes when all of the node's children must fail
    // If FAIL_ON_ONE and SUCEED_ON_ONE are both active and are both trigerred, it fails
    enum EFAILURE_POLICY {
        FAIL_ON_ONE,
        FAIL_ON_ALL
    };

    // options when a parallel node is considered to be succeeded.
    // SUCCEED_ON_ONE: the node will return success as soon as one of its children succeeds.
    // SUCCEED_ON_ALL: the node will return success when all the node's children must succeed.
    enum ESUCCESS_POLICY {
        SUCCEED_ON_ONE,
        SUCCEED_ON_ALL
    };

    // options when a parallel node is exited
    // EXIT_NONE: the parallel node just exit.
    // EXIT_ABORT_RUNNINGSIBLINGS: the parallel node abort all other running siblings.
    enum EEXIT_POLICY {
        EXIT_NONE,
        EXIT_ABORT_RUNNINGSIBLINGS
    };

    // the options of what to do when a child finishes
    // CHILDFINISH_ONCE: the child node just executes once.
    // CHILDFINISH_LOOP: the child node runs again and again.
    enum ECHILDFINISH_POLICY {
        CHILDFINISH_ONCE,
        CHILDFINISH_LOOP
    };

    class BEHAVIAC_API Parallel : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Parallel, BehaviorNode);

        Parallel();
        virtual ~Parallel();
#if BEHAVIAC_USE_HTN
        virtual bool decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner);
#endif //BEHAVIAC_USE_HTN
        virtual void load(int version, const char* agentType, const properties_t& properties);
        EBTStatus ParallelUpdate(Agent* pAgent, behaviac::vector<BehaviorTask*>children);
        virtual bool IsManagingChildrenAsSubTrees() const;
    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

        void SetPolicy(EFAILURE_POLICY failPolicy = FAIL_ON_ALL, ESUCCESS_POLICY successPolicy = SUCCEED_ON_ALL, EEXIT_POLICY exitPolicty = EXIT_NONE) {
            m_failPolicy = failPolicy;
            m_succeedPolicy = successPolicy;
            m_exitPolicy = exitPolicty;
        }

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        EFAILURE_POLICY		m_failPolicy;
        ESUCCESS_POLICY		m_succeedPolicy;
        EEXIT_POLICY			m_exitPolicy;
        ECHILDFINISH_POLICY	m_childFinishPolicy;

        friend class ParallelTask;
    };

    ///Execute behaviors in parallel
    //There are two policies that control the flow of execution:
    //the policy for failure, and the policy for success.
    class BEHAVIAC_API ParallelTask : public CompositeTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(ParallelTask, CompositeTask);
        ParallelTask();
        virtual ~ParallelTask();

    protected:
        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);
    };
    /*! @} */
    /*! @} */
}

#endif//BEHAVIAC_BEHAVIORTREE_PARALLEL_Hrap
