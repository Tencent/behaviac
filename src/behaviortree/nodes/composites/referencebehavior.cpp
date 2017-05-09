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

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/nodes/composites/referencebehavior.h"
#include "behaviac/agent/agent.h"

#include "behaviac/behaviortree/nodes/actions/action.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"
#include "behaviac/htn/task.h"
#include "behaviac/fsm/state.h"
#include "behaviac/fsm/transitioncondition.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    ReferencedBehavior::ReferencedBehavior() : m_taskNode(0), m_referencedBehaviorPath(0), m_taskMethod(0), m_transitions(0)
    {}

    ReferencedBehavior::~ReferencedBehavior() {
        BEHAVIAC_DELETE(m_referencedBehaviorPath);
        BEHAVIAC_DELETE(m_taskMethod);
        BEHAVIAC_DELETE this->m_transitions;
    }

    void ReferencedBehavior::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "ReferenceBehavior")) {
                if (StringUtils::IsValidString(p.value)) {
                    const char* pParenthesis = strchr(p.value, '(');

                    if (pParenthesis == 0) {
                        behaviac::string typeName;
                        this->m_referencedBehaviorPath = AgentMeta::ParseProperty(p.value);
                    } else {
                        this->m_referencedBehaviorPath = AgentMeta::ParseMethod(p.value);
                    }

                    const char* szTreePath = this->GetReferencedTree(0);

                    //conservatively make it true
                    bool bHasEvents = true;

                    if (!StringUtils::IsNullOrEmpty(szTreePath)) {
						if (Config::PreloadBehaviors()) {
							//it has a const tree path, so as to load the tree and check if that tree has events
							BehaviorTree* behaviorTree = Workspace::GetInstance()->LoadBehaviorTree(szTreePath);
							BEHAVIAC_ASSERT(behaviorTree);

							if (behaviorTree) {
								bHasEvents = behaviorTree->HasEvents();
							}
						}

						this->m_bHasEvents |= bHasEvents;
					}
                }
			}
			else if (StringUtils::StringEqual(p.name, "Task")) {
                BEHAVIAC_ASSERT(!StringUtils::IsNullOrEmpty(p.value));
                //BEHAVIAC_ASSERT(m is CTaskMethod);

                this->m_taskMethod = AgentMeta::ParseMethod(p.value);
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }
    }

#if BEHAVIAC_USE_HTN

    bool ReferencedBehavior::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        ReferencedBehavior* taskSubTree = (ReferencedBehavior*)node;// as ReferencedBehavior;
        bool bOk = false;
        BEHAVIAC_ASSERT(taskSubTree != 0);
        int depth2 = planner->GetAgent()->GetVariables()->Depth();
        BEHAVIAC_UNUSED_VAR(depth2);
        {
            AgentState::AgentStateScope scopedState(planner->GetAgent()->GetVariables()->Push(false));

            Agent* pAgent = planner->GetAgent();
            const char* szTreePath = taskSubTree->GetReferencedTree(pAgent);
            BehaviorTreeTask* subTreeTask = Workspace::GetInstance()->CreateBehaviorTreeTask(szTreePath);

            //planner.agent.Variables.Log(planner.agent, true);
            taskSubTree->SetTaskParams(planner->GetAgent(), subTreeTask);

            Task* task = taskSubTree->RootTaskNode(planner->GetAgent());

            if (task != 0) {
                planner->LogPlanReferenceTreeEnter(planner->GetAgent(), taskSubTree);

                //const BehaviorNode* tree = task->GetParent();
                //tree->InstantiatePars(planner->GetAgent());
                BehaviorTreeTask* oldCurrentTreeTask = pAgent->m_excutingTreeTask;
                pAgent->m_excutingTreeTask = subTreeTask;

                PlannerTask* childTask = planner->decomposeNode(task, depth);
                pAgent->m_excutingTreeTask = oldCurrentTreeTask;

                if (childTask != 0) {
                    PlannerTaskReference* subTreeRef = (PlannerTaskReference*)seqTask;
                    subTreeRef->SetSubTreeTask(subTreeTask);

                    seqTask->AddChild(childTask);
                    bOk = true;
                }

                planner->LogPlanReferenceTreeExit(planner->GetAgent(), taskSubTree);
                BEHAVIAC_ASSERT(true);
            }
        }

        BEHAVIAC_ASSERT(planner->GetAgent()->GetVariables()->Depth() == depth2);
        return bOk;
    }
#endif

    void ReferencedBehavior::Attach(BehaviorNode* pAttachment, bool bIsPrecondition, bool bIsEffector, bool bIsTransition) {
        if (bIsTransition) {
            BEHAVIAC_ASSERT(!bIsEffector && !bIsPrecondition);

            if (this->m_transitions == 0) {
                this->m_transitions = BEHAVIAC_NEW behaviac::vector<Transition*>();
            }

            BEHAVIAC_ASSERT(Transition::DynamicCast(pAttachment) != 0);
            Transition* pTransition = (Transition*)pAttachment;
            this->m_transitions->push_back(pTransition);

            return;
        }

        BEHAVIAC_ASSERT(bIsTransition == false);
        super::Attach(pAttachment, bIsPrecondition, bIsEffector, bIsTransition);
    }

    bool ReferencedBehavior::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!ReferencedBehavior::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* ReferencedBehavior::createTask() const {
        ReferencedBehaviorTask* pTask = BEHAVIAC_NEW ReferencedBehaviorTask();

        return pTask;
    }

    void ReferencedBehavior::SetTaskParams(Agent* pAgent, BehaviorTreeTask* treeTask) {
        if (this->m_taskMethod != 0) {
            this->m_taskMethod->SetTaskParams(pAgent, treeTask);
        }
    }

    Task* ReferencedBehavior::RootTaskNode(Agent* pAgent) {
        if (this->m_taskNode == 0) {
            BehaviorTree* bt = Workspace::GetInstance()->LoadBehaviorTree(this->GetReferencedTree(pAgent));

            if (bt != 0 && bt->GetChildrenCount() == 1) {
                BehaviorNode* root = (BehaviorNode*)bt->GetChild(0);
                this->m_taskNode = (Task*)root;
            }
        }

        return this->m_taskNode;
    }

    const char* ReferencedBehavior::GetReferencedTree(const Agent* pAgent) const {
        BEHAVIAC_ASSERT(this->m_referencedBehaviorPath);

        if (this->m_referencedBehaviorPath) {
            const char* str = (const char*)m_referencedBehaviorPath->GetValue(pAgent, false, behaviac::GetClassTypeNumberId<const char*>());

            if (str == NULL) {
                return NULL;
            } else {
                return str;// ->c_str();
            }
        }

        return NULL;
    }

	ReferencedBehaviorTask::ReferencedBehaviorTask() : SingeChildTask(), m_nextStateId(-1), m_subTree(NULL) {
    }

    ReferencedBehaviorTask::~ReferencedBehaviorTask() {
		if (this->m_subTree)
		{
			behaviac::Workspace::GetInstance()->DestroyBehaviorTreeTask(this->m_subTree, NULL);
			this->m_subTree = NULL;
		}
    }

    void ReferencedBehaviorTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        // BEHAVIAC_ASSERT(ReferencedBehaviorTask::DynamicCast(target));
        // ReferencedBehaviorTask* ttask = (ReferencedBehaviorTask*)target;
    }

    void ReferencedBehaviorTask::save(IIONode* node) const {
        super::save(node);
    }

    void ReferencedBehaviorTask::load(IIONode* node) {
        super::load(node);
    }

    void ReferencedBehaviorTask::Init(const BehaviorNode* node) {
        super::Init(node);
    }

    bool ReferencedBehaviorTask::onevent(Agent* pAgent, const char* eventName, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams) {
        if (this->m_status == BT_RUNNING && this->m_node->HasEvents()) {
            BEHAVIAC_ASSERT(this->m_subTree);

            if (!this->m_subTree->onevent(pAgent, eventName, eventParams)) {
                return false;
            }
        }

        return true;
    }

    bool ReferencedBehaviorTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_ASSERT(ReferencedBehavior::DynamicCast(this->m_node) != 0);
        ReferencedBehavior* pNode = (ReferencedBehavior*)this->m_node;

        BEHAVIAC_ASSERT(pNode != 0);

        this->m_nextStateId = -1;

        const char* szTreePath = pNode->GetReferencedTree(pAgent);

		//to create the task on demand
		if (szTreePath && (this->m_subTree == NULL || !StringUtils::Compare(szTreePath, this->m_subTree->GetName().c_str())))
		{
			if (this->m_subTree)
			{
				behaviac::Workspace::GetInstance()->DestroyBehaviorTreeTask(this->m_subTree, pAgent);
			}

			this->m_subTree = Workspace::GetInstance()->CreateBehaviorTreeTask(szTreePath);
			pNode->SetTaskParams(pAgent, this->m_subTree);
		}
		else if (this->m_subTree)
		{
			this->m_subTree->reset(pAgent);
		}

        return true;
    }

    void ReferencedBehaviorTask::onexit(Agent* pAgent, EBTStatus s) {
#if BEHAVIAC_USE_HTN
        BEHAVIAC_ASSERT(this->m_currentState != NULL);
        this->m_currentState->Pop();
#endif
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    int ReferencedBehaviorTask::GetNextStateId() const {
        return this->m_nextStateId;
    }

    bool ReferencedBehaviorTask::CheckPreconditions(const Agent* pAgent, bool bIsAlive) const {
#if BEHAVIAC_USE_HTN
        ReferencedBehaviorTask* pThis = const_cast<ReferencedBehaviorTask*>(this);
        pThis->m_currentState = const_cast<Agent*>(pAgent)->GetVariables()->Push(false);
        BEHAVIAC_ASSERT(m_currentState != NULL);
#endif//

        bool bOk = BehaviorTask::CheckPreconditions(pAgent, bIsAlive);
#if BEHAVIAC_USE_HTN

        if (!bOk) {
            this->m_currentState->Pop();
            pThis->m_currentState = NULL;
        }

#endif//
        return bOk;
    }

    EBTStatus ReferencedBehaviorTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(childStatus);
        BEHAVIAC_ASSERT(ReferencedBehavior::DynamicCast(this->GetNode()));
        const ReferencedBehavior* pNode = (const ReferencedBehavior*)this->m_node;
        BEHAVIAC_ASSERT(pNode);

#if !BEHAVIAC_RELEASE
        pAgent->m_debug_count++;

        if (pAgent->m_debug_count > 20) {
            BEHAVIAC_LOGWARNING("%s might be in a recurrsive inter calling of trees\n", pAgent->GetName().c_str());
            BEHAVIAC_ASSERT(false);
        }

#endif//#if !BEHAVIAC_RELEASE

        EBTStatus result = this->m_subTree->exec(pAgent);

        bool bTransitioned = State::UpdateTransitions(pAgent, pNode, pNode->m_transitions, this->m_nextStateId, result);

        if (bTransitioned) {
            if (result == BT_RUNNING) {
                //subtree not exited, but it will transition to other states
                this->m_subTree->abort(pAgent);
            }

            result = BT_SUCCESS;
        }

        return result;
    }
}//namespace behaviac
