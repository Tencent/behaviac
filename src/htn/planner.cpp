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
#include "behaviac/agent/agent.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"
#include "behaviac/behaviortree/attachments/effector.h"
#include "behaviac/behaviortree/nodes/composites/referencebehavior.h"
#include "behaviac/htn/task.h"
#include "behaviac/behaviortree/nodes/composites/selector.h"
#include "behaviac/behaviortree/nodes/composites/parallel.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorloop.h"
#include "behaviac/behaviortree/nodes/decorators/decoratoriterator.h"

namespace behaviac {
#if BEHAVIAC_USE_HTN

    void Planner::Init(Agent* pAgent, Task* rootTask) {
        this->agent = pAgent;
        this->m_rootTaskNode = rootTask;
    }

    void Planner::Uninit() {
        this->OnDisable();
    }

    void Planner::OnDisable() {
        if (this->m_rootTask != NULL) {
            if (this->m_rootTask->GetStatus() == BT_RUNNING) {
                this->m_rootTask->abort(this->agent);
                BehaviorTask::DestroyTask(this->m_rootTask);
            }

            this->m_rootTask = NULL;
        }
    }

    EBTStatus Planner::Update() {
        if (this->agent == NULL) {
            BEHAVIAC_ASSERT(false);
            //throw new InvalidOperationException("The Planner.Agent field has not been assigned");
        }

        doAutoPlanning();

        if (this->m_rootTask == NULL) {
            return BT_FAILURE;
        }

        // Need a local reference in case the this->m_rootTask is cleared by an event handler
        PlannerTask* rootTask = this->m_rootTask;

        EBTStatus taskStatus = rootTask->exec(this->agent);

        return taskStatus;
    }

    // Generate a new task for the <paramref name="agent"/> based on the current world state as
    // described by the <paramref name="agentState"/>.

    // <param name="agent">The agent for which the task is being generated. This object instance must be
    // of the same type as the type for which the Task was developed</param>
    // <param name="agentState">The current world state required by the planner</param>

    PlannerTask* Planner::GeneratePlan() {
        // If the planner is currently executing a task marked NotInterruptable, do not generate
        // any n ew plans.
        if (!canInterruptCurrentPlan()) {
            return NULL;
        }

        PlannerTask* newPlan = this->BuildPlan(this->m_rootTaskNode);

        if (newPlan == NULL) {
            return NULL;
        }

        if (!newPlan->IsHigherPriority(this->m_rootTask)) {
            return NULL;
        }

        return newPlan;
    }

    bool Planner::canInterruptCurrentPlan() {
        if (this->m_rootTask == NULL) {
            return true;
        }

        if (this->m_rootTask->GetStatus() != BT_RUNNING) {
            return true;
        }

        PlannerTask* task = this->m_rootTask;

        if (task == NULL || !task->NotInterruptable) {
            return true;
        }

        return task->GetStatus() == BT_FAILURE || task->GetStatus() == BT_SUCCESS;
    }

    void Planner::doAutoPlanning() {
        if (!this->AutoReplan) {
            return;
        }

        bool noPlan = this->m_rootTask == NULL || this->m_rootTask->GetStatus() != BT_RUNNING;

        if (noPlan) {
            PlannerTask* newPlan = this->GeneratePlan();

            if (newPlan != NULL) {
                if (this->m_rootTask != NULL) {
                    if (this->m_rootTask->GetStatus() == BT_RUNNING) {
                        this->m_rootTask->abort(this->agent);
                    }

                    BehaviorTask::DestroyTask(this->m_rootTask);
                }

                this->m_rootTask = newPlan;
            }
        }
    }

    void Planner::LogPlanBegin(Agent* a, Task* root) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(root);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string agentClassName(a->GetObjectTypeName());
            behaviac::string agentInstanceName(a->GetName());

            behaviac::string ni = BehaviorTask::GetTickInfo(a, root, "plan");
            int count = Workspace::GetInstance()->GetActionCount(ni.c_str()) + 1;
            char temp[1024];
            string_sprintf(temp, "[plan_begin]%s#%s %s %d\n", agentClassName.c_str(), agentInstanceName.c_str(), ni.c_str(), count);

            LogManager::GetInstance()->Log(temp);

            a->m_variables->Log(a, true);
        }

#endif
    }

    void Planner::LogPlanEnd(Agent* a, Task* root) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(root);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string agentClassName(a->GetObjectTypeName());
            behaviac::string agentInstanceName(a->GetName());

            behaviac::string ni = BehaviorTask::GetTickInfo(a, root, NULL);
            char temp[1024];
            string_sprintf(temp, "[plan_end]%s#%s %s\n", agentClassName.c_str(), agentInstanceName.c_str(), ni.c_str());

            LogManager::GetInstance()->Log(temp);
        }

#endif
    }

    void Planner::LogPlanNodeBegin(Agent* a, BehaviorNode* n) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(n);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, n, NULL);

            LogManager::GetInstance()->Log("[plan_node_begin]%s\n", ni.c_str());
            a->m_variables->Log(a, true);
        }

#endif
    }

    void Planner::LogPlanNodePreconditionFailed(Agent* a, BehaviorNode* n) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(n);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, n, NULL);

            LogManager::GetInstance()->Log("[plan_node_pre_failed]%s\n", ni.c_str());
        }

#endif
    }

    void Planner::LogPlanNodeEnd(Agent* a, BehaviorNode* n, const behaviac::string& result) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(n);
        BEHAVIAC_UNUSED_VAR(result);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, n, NULL);

            LogManager::GetInstance()->Log("[plan_node_end]%s %s\n", ni.c_str(), result.c_str());
        }

#endif
    }

    void Planner::LogPlanReferenceTreeEnter(Agent* a, ReferencedBehavior* referencedNode) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(referencedNode);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, referencedNode, NULL);
            const char* refTreeStr = referencedNode->GetReferencedTree(a);
            LogManager::GetInstance()->Log("[plan_referencetree_enter]%s %s.xml\n", ni.c_str(), refTreeStr);
        }

#endif
    }

    void Planner::LogPlanReferenceTreeExit(Agent* a, ReferencedBehavior* referencedNode) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(referencedNode);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, referencedNode, NULL);
            const char* refTreeStr = referencedNode->GetReferencedTree(a);

            LogManager::GetInstance()->Log("[plan_referencetree_exit]%s %s.xml\n", ni.c_str(), refTreeStr);
        }

#endif
    }

    void Planner::LogPlanMethodBegin(Agent* a, BehaviorNode* m) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(m);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, m, NULL);
            LogManager::GetInstance()->Log("[plan_method_begin]%s\n", ni.c_str());

            a->m_variables->Log(a, true);
        }

#endif
    }

    void Planner::LogPlanMethodEnd(Agent* a, BehaviorNode* m, const behaviac::string& result) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(m);
        BEHAVIAC_UNUSED_VAR(result);

#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, m, NULL);

            LogManager::GetInstance()->Log("[plan_method_end]%s %s\n", ni.c_str(), result.c_str());
        }

#endif
    }

    void Planner::LogPlanForEachBegin(Agent* a, DecoratorIterator* pForEach, int index, int count) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(pForEach);
        BEHAVIAC_UNUSED_VAR(index);
        BEHAVIAC_UNUSED_VAR(count);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, pForEach, NULL);
            LogManager::GetInstance()->Log("[plan_foreach_begin]%s %d %d\n", ni.c_str(), index, count);
            a->m_variables->Log(a, true);
        }

#endif
    }

    void Planner::LogPlanForEachEnd(Agent* a, DecoratorIterator* pForEach, int index, int count, const behaviac::string& result) {
        BEHAVIAC_UNUSED_VAR(a);
        BEHAVIAC_UNUSED_VAR(pForEach);
        BEHAVIAC_UNUSED_VAR(count);
        BEHAVIAC_UNUSED_VAR(index);
        BEHAVIAC_UNUSED_VAR(result);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            behaviac::string ni = BehaviorTask::GetTickInfo(a, pForEach, NULL);
            LogManager::GetInstance()->Log("[plan_foreach_end]%s %d %d %s\n", ni.c_str(), index, count, result.c_str());
        }

#endif
    }

    PlannerTask* Planner::BuildPlan(Task* root) {
        //LogManager.PLanningClearCache();

        int depth = this->agent->GetVariables()->Depth();
        BEHAVIAC_UNUSED_VAR(depth);

        PlannerTask* rootTask = NULL;

        {
            AgentState::AgentStateScope scopedState(this->agent->GetVariables()->Push(true));

            this->agent->m_planningTop = this->agent->GetVariables()->Top();
            BEHAVIAC_ASSERT(this->agent->m_planningTop >= 0);

            LogPlanBegin(this->agent, root);

            rootTask = this->decomposeNode((BehaviorNode*)root, 0);

            LogPlanEnd(this->agent, root);

#if !BEHAVIAC_RELEASE
            //BehaviorTask::CHECK_BREAKPOINT(this->agent, root, "plan", EActionResult.EAR_all);
#endif

            this->agent->m_planningTop = -1;
        }

        BEHAVIAC_ASSERT(this->agent->GetVariables()->Depth() == depth);

        return rootTask;
    }

    PlannerTask* Planner::decomposeNode(BehaviorNode* node, int depth) {
        // Ensure that the planner does not get stuck in an infinite loop
        if (depth >= 256) {
            //Debug.LogError("Exceeded task nesting depth. Does the graph contain an invalid cycle?");
            return NULL;
        }

        LogPlanNodeBegin(this->agent, node);

        int depth1 = this->agent->m_variables->Depth();
        BEHAVIAC_UNUSED_VAR(depth1);
        PlannerTask* taskAdded = NULL;

        bool isPreconditionOk = node->CheckPreconditions(this->agent, false);

        if (isPreconditionOk) {
            bool bOk = true;
            taskAdded = PlannerTask::Create(node, this->agent);

            if (Action::DynamicCast(node) != 0) {
                //nothing to do for action
                BEHAVIAC_ASSERT(true);
            } else {
                BEHAVIAC_ASSERT(PlannerTaskComplex::DynamicCast(taskAdded) != 0);
                PlannerTaskComplex* seqTask = (PlannerTaskComplex*)taskAdded;

                bOk = this->decomposeComplex(node, seqTask, depth);
            }

            if (bOk) {
                node->ApplyEffects(this->agent, BehaviorNode::E_SUCCESS);
            } else {
                BehaviorTask::DestroyTask(taskAdded);
                taskAdded = NULL;
            }
        } else {
            //precondition failed
            LogPlanNodePreconditionFailed(this->agent, node);
        }

        LogPlanNodeEnd(this->agent, node, taskAdded != NULL ? "success" : "failure");

        BEHAVIAC_ASSERT(this->agent->m_variables->Depth() == depth1);

        return taskAdded;
    }

    bool Planner::decomposeComplex(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth) {
        int depth1 = this->agent->m_variables->Depth();
        BEHAVIAC_UNUSED_VAR(depth1);
        bool bOk = false;

        bOk = node->decompose(node, seqTask, depth, this);

        BEHAVIAC_ASSERT(this->agent->m_variables->Depth() == depth1);
        return bOk;
    }

    PlannerTask* Planner::decomposeTask(Task* task, int depth) {
        uint32_t methodsCount = task->GetChildrenCount();

        if (methodsCount == 0) {
            return NULL;
        }

        int depth1 = this->agent->m_variables->Depth();
        BEHAVIAC_UNUSED_VAR(depth1);
        PlannerTask* methodTask = NULL;

        for (uint32_t i = 0; i < methodsCount; i++) {
            BehaviorNode* method = (BehaviorNode*)task->GetChild(i);
            BEHAVIAC_ASSERT(Method::DynamicCast(method) != 0);
            BEHAVIAC_UNUSED_VAR(method);

            int depth2 = this->agent->m_variables->Depth();
            BEHAVIAC_UNUSED_VAR(depth2);
            {
                AgentState::AgentStateScope scopedState(this->agent->m_variables->Push(false));

                LogPlanMethodBegin(this->agent, method);
                methodTask = this->decomposeNode(method, depth + 1);
                LogPlanMethodEnd(this->agent, method, methodTask != NULL ? "success" : "failure");

                if (methodTask != NULL) {
                    // succeeded
                    break;
                }
            }

            BEHAVIAC_ASSERT(this->agent->m_variables->Depth() == depth2);
        }

        BEHAVIAC_ASSERT(this->agent->m_variables->Depth() == depth1);
        return methodTask;
    }
#endif
}
