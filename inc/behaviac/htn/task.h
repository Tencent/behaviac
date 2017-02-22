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

#ifndef _BEHAVIAC_HTN_TASK_H_
#define _BEHAVIAC_HTN_TASK_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"
#include "behaviac/htn/method.h"
namespace behaviac {
    class BEHAVIAC_API Task : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Task, BehaviorNode);
        Task();
        virtual ~Task();

        virtual bool isSync() const {
            return false;
        }
#if BEHAVIAC_USE_HTN
        virtual bool decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner);
#endif //BEHAVIAC_USE_HTN
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
        virtual BehaviorTask* createTask() const;
        int FindMethodIndex(Method* method);
        bool			IsHTN();
    protected:
        behaviac::IInstanceMember* m_task;
        bool			m_bHTN;

        virtual void load(int version, const char* agentType, const properties_t& properties);
        friend class TaskTask;
    };

    class BEHAVIAC_API TaskTask : public SequenceTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(TaskTask, SequenceTask)
        TaskTask();
        virtual ~TaskTask();

    protected:
        void addChild(BehaviorTask* pBehavior);
    public:
        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;

        virtual void save(IIONode* node) const;

        virtual void load(IIONode* node);
    protected:
        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
    private:

#if BEHAVIAC_USE_HTN
        Planner* _planner;
#endif
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };
}
#endif
