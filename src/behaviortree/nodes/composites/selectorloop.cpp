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
#include "behaviac/behaviortree/nodes/composites/selectorloop.h"
#include "behaviac/behaviortree/nodes/composites/withprecondition.h"

namespace behaviac {
    SelectorLoop::SelectorLoop()
		: m_bResetChildren(false)
    {
	}

    SelectorLoop::~SelectorLoop()
    {
	}

    bool SelectorLoop::IsManagingChildrenAsSubTrees() const {
        return true;
    }

    void SelectorLoop::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

		for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
			const property_t& p = (*it);

			if (StringUtils::StringEqual(p.name, "ResetChildren")) {
				this->m_bResetChildren = StringUtils::StringEqualNoCase(p.value, "true");
				break;
			}
		}
    }

    bool SelectorLoop::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!SelectorLoop::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* SelectorLoop::createTask() const {
        SelectorLoopTask* pTask = BEHAVIAC_NEW SelectorLoopTask();

        return pTask;
    }

    SelectorLoopTask::SelectorLoopTask() : CompositeTask() {
    }

    void SelectorLoopTask::Init(const BehaviorNode* node) {
        super::Init(node);
    }

    void SelectorLoopTask::copyto(BehaviorTask* target) const {
        CompositeTask::copyto(target);

        BEHAVIAC_ASSERT(SelectorLoopTask::DynamicCast(target));
        SelectorLoopTask* ttask = (SelectorLoopTask*)target;

        ttask->m_activeChildIndex = this->m_activeChildIndex;
    }

    void SelectorLoopTask::save(IIONode* node) const {
        super::save(node);
    }

    void SelectorLoopTask::load(IIONode* node) {
        super::load(node);
    }

    SelectorLoopTask::~SelectorLoopTask() {
    }

    void SelectorLoopTask::addChild(BehaviorTask* pBehavior) {
        super::addChild(pBehavior);

        BEHAVIAC_ASSERT(WithPreconditionTask::DynamicCast(pBehavior));
    }

    bool SelectorLoopTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        //reset the action child as it will be checked in the update
        this->m_activeChildIndex = CompositeTask::InvalidChildIndex;
        BEHAVIAC_ASSERT(this->m_activeChildIndex == CompositeTask::InvalidChildIndex);

        return super::onenter(pAgent);
    }

    void SelectorLoopTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        super::onexit(pAgent, s);
    }

    EBTStatus SelectorLoopTask::update_current(Agent* pAgent, EBTStatus childStatus) {
        EBTStatus s = this->update(pAgent, childStatus);

        return s;
    }

    EBTStatus SelectorLoopTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);
        int idx = -1;

        if (childStatus != BT_RUNNING) {
            BEHAVIAC_ASSERT(this->m_activeChildIndex != CompositeTask::InvalidChildIndex);

            if (childStatus == BT_SUCCESS) {
                return BT_SUCCESS;
            } else if (childStatus == BT_FAILURE) {
                //the next for starts from (idx + 1), so that it starts from next one after this failed one
                idx = this->m_activeChildIndex;
            } else {
                BEHAVIAC_ASSERT(false);
            }
        }

        //checking the preconditions and take the first action tree
        uint32_t index = (uint32_t) - 1;

        for (uint32_t i = (idx + 1); i < this->m_children.size(); ++i) {
            WithPreconditionTask* pSubTree = (WithPreconditionTask*)this->m_children[i];
            BEHAVIAC_ASSERT(WithPreconditionTask::DynamicCast(pSubTree));
            BehaviorTask* pre = pSubTree->PreconditionNode();

            EBTStatus status = pre->exec(pAgent);

            if (status == BT_SUCCESS) {
                index = i;
                break;
            }
        }

        //clean up the current ticking action tree
        if (index != (uint32_t) - 1) {
            if (this->m_activeChildIndex != CompositeTask::InvalidChildIndex)
			{
				bool abortChild = (this->m_activeChildIndex != (int)index);
				if (!abortChild)
				{
					const SelectorLoop* pSelectorLoop = SelectorLoop::DynamicCast(this->GetNode());
					BEHAVIAC_ASSERT(pSelectorLoop);

					if (pSelectorLoop)
					{
						abortChild = pSelectorLoop->m_bResetChildren;
					}
				}

				if (abortChild)
				{
					WithPreconditionTask* pCurrentSubTree = (WithPreconditionTask*)this->m_children[this->m_activeChildIndex];
					BEHAVIAC_ASSERT(WithPreconditionTask::DynamicCast(pCurrentSubTree));
					//BehaviorTask* action = pCurrentSubTree->ActionNode();
					//BEHAVIAC_UNUSED_VAR(action);
					pCurrentSubTree->abort(pAgent);
				}
            }

            for (uint32_t i = index; i < this->m_children.size(); ++i) {
                WithPreconditionTask* pSubTree = (WithPreconditionTask*)this->m_children[i];
                BEHAVIAC_ASSERT(WithPreconditionTask::DynamicCast(pSubTree));

                if (i > index) {
                    BehaviorTask* pre = pSubTree->PreconditionNode();
                    EBTStatus status = pre->exec(pAgent);

                    //to search for the first one whose precondition is success
                    if (status != BT_SUCCESS) {
                        continue;
                    }
                }

                BehaviorTask* action = pSubTree->ActionNode();
                EBTStatus s = action->exec(pAgent);

                if (s == BT_RUNNING) {
                    this->m_activeChildIndex = i;
                    pSubTree->m_status = BT_RUNNING;
                } else {
                    //pActionTree->reset(pAgent);
                    pSubTree->m_status = s;

                    if (s == BT_FAILURE) {
                        //THE ACTION failed, to try the next one
                        continue;
                    }
                }

                BEHAVIAC_ASSERT(s == BT_RUNNING || s == BT_SUCCESS);

                return s;
            }
        }

        return BT_FAILURE;
    }
}
