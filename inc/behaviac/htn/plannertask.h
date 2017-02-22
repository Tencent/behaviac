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

#ifndef _BEHAVIAC_HTN_PLANNERTASK_H_
#define _BEHAVIAC_HTN_PLANNERTASK_H_
#include <typeinfo>
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/properties.h"

#if BEHAVIAC_USE_HTN
namespace behaviac {
    class AgentState;
    class PlannerTask : public BehaviorTask {
        typedef PlannerTask* (*TaskCreator)(BehaviorNode* node, Agent* pAgent);

    private:
        typedef behaviac::map<CStringCRC, TaskCreator> PlannerTaskFactory_t;
        static PlannerTaskFactory_t* ms_factory;

        template<typename T, typename TT>
        static void Register();
    public:
        PlannerTask* Parent;
        bool NotInterruptable;

        PlannerTask();

        virtual ~PlannerTask();

        static PlannerTask* Create(BehaviorNode* node, Agent* pAgent);

        PlannerTask(BehaviorNode* node, Agent* pAgent);

        static void Cleanup();

        bool IsHigherPriority(PlannerTask* other);

        virtual void traverse(bool childFirst, NodeHandler_t handler, Agent* pAgent, void* user_data);
    protected:
        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskAction : public PlannerTask {
        //private object[] ParamsValue { get; set; }

    public:
        PlannerTaskAction(BehaviorNode* node, Agent* pAgent);

    protected:
        virtual bool onenter(Agent* pAgent);

        virtual void onexit(Agent* pAgent, EBTStatus s);

    protected:
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskComplex : public PlannerTask {
    protected:
        int								m_activeChildIndex;
        behaviac::vector<BehaviorTask*> m_children;

    public:
        void AddChild(PlannerTask* task);

        void RemoveChild(PlannerTask* childTask);

        PlannerTaskComplex(BehaviorNode* node, Agent* pAgent);

    protected:
        virtual bool onenter(Agent* pAgent);

        virtual void onexit(Agent* pAgent, EBTStatus s);

        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskSequence : public PlannerTaskComplex {
    public:
        PlannerTaskSequence(BehaviorNode* node, Agent* pAgent);

    protected:
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskSelector : public PlannerTaskComplex {
    public:
        PlannerTaskSelector(BehaviorNode* node, Agent* pAgent);

    protected:
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskParallel : public PlannerTaskComplex {
    public:
        PlannerTaskParallel(BehaviorNode* node, Agent* pAgent);
    protected:
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskLoop : public PlannerTaskComplex {
    public:
        PlannerTaskLoop(BehaviorNode* node, Agent* pAgent);

    protected:
        int m_n;

        virtual bool onenter(Agent* pAgent);

    public:
        int GetCount(Agent* pAgent);

    protected:
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskIterator : public PlannerTaskComplex {
    public:
        PlannerTaskIterator(BehaviorNode* node, Agent* pAgent);

        void SetIndex(int index);
    protected:
        virtual bool onenter(Agent* pAgent);

        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
        int m_index;
    };

    class PlannerTaskReference : public PlannerTaskComplex {
    public:
        PlannerTaskReference(BehaviorNode* node, Agent* pAgent);

        void SetSubTreeTask(BehaviorTreeTask* treeTask) {
            this->m_subTree = treeTask;
        }

        AgentState* currentState;

    protected:
        virtual bool CheckPreconditions(const Agent* pAgent, bool bIsAlive) const;

#if !BEHAVIAC_RELEASE
        bool _logged;
#endif
        BehaviorTreeTask* m_subTree;
        BehaviorTreeTask* m_oldTreeTask;
    protected:
        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus status);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskTask : public PlannerTaskComplex {
    public:
        PlannerTaskTask(BehaviorNode* node, Agent* pAgent);

    protected:
        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);

        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

    class PlannerTaskMethod : public  PlannerTaskComplex {
    public:
        PlannerTaskMethod(BehaviorNode* node, Agent* pAgent);

    protected:
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };
}
#endif//#if BEHAVIAC_USE_HTN

#endif

