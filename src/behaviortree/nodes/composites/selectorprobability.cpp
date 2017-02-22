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

#include "behaviac/behaviortree/nodes/composites/selectorprobability.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorweight.h"
#include "behaviac/common/randomgenerator/randomgenerator.h"

#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/common/meta.h"
#include "behaviac/common/member.h"

namespace behaviac {
    SelectorProbability::SelectorProbability() : m_method(0)
    {}

    SelectorProbability::~SelectorProbability() {
        BEHAVIAC_DELETE(m_method);
    }

    //behaviac::CMethodBase* LoadMethod(const char* value);

    void SelectorProbability::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

			if (StringUtils::StringEqual(p.name, "RandomGenerator")) {
                if (p.value[0] != '\0') {
                    this->m_method = AgentMeta::ParseMethod(p.value);
                }//if (p.value[0] != '\0')

            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }
    }

    bool SelectorProbability::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!SelectorProbability::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* SelectorProbability::createTask() const {
        SelectorProbabilityTask* pTask = BEHAVIAC_NEW SelectorProbabilityTask();

        return pTask;
    }

    void SelectorProbability::AddChild(BehaviorNode* pBehavior) {
        BEHAVIAC_ASSERT(DecoratorWeight::DynamicCast(pBehavior));
        DecoratorWeight* pDW = (DecoratorWeight*)(pBehavior);

        if (pDW) {
            super::AddChild(pBehavior);

        } else {
            BEHAVIAC_ASSERT(false, "only DecoratorWeightTask can be children");
        }
    }

    SelectorProbabilityTask::SelectorProbabilityTask() : CompositeTask(), m_totalSum(0)
    {}

    SelectorProbabilityTask::~SelectorProbabilityTask()
    {}

    void SelectorProbabilityTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void SelectorProbabilityTask::save(IIONode* node) const {
        super::save(node);
    }

    void SelectorProbabilityTask::load(IIONode* node) {
        super::load(node);
    }

    bool SelectorProbabilityTask::onenter(Agent* pAgent) {
        BEHAVIAC_ASSERT(this->m_children.size() > 0);
        //BEHAVIAC_ASSERT(this->m_activeChildIndex == CompositeTask::InvalidChildIndex);
		this->m_activeChildIndex = CompositeTask::InvalidChildIndex;

        //const SelectorProbability* pSelectorProbabilityNode = SelectorProbability::DynamicCast(this->GetNode());

        this->m_weightingMap.clear();
        this->m_totalSum = 0;

        for (BehaviorTasks_t::iterator it = this->m_children.begin(); it != this->m_children.end(); ++it) {
            BehaviorTask* task = *it;
            BEHAVIAC_ASSERT(DecoratorWeightTask::DynamicCast(task));
            DecoratorWeightTask* pWT = (DecoratorWeightTask*)task;

            int weight = pWT->GetWeight(pAgent);
            this->m_weightingMap.push_back(weight);
            this->m_totalSum += weight;
        }

        BEHAVIAC_ASSERT(this->m_weightingMap.size() == this->m_children.size());

        return true;
    }

    void SelectorProbabilityTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
        this->m_activeChildIndex = CompositeTask::InvalidChildIndex;
    }

    double GetRandomValue(behaviac::IInstanceMember* method, Agent* pAgent);

    EBTStatus SelectorProbabilityTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(SelectorProbability::DynamicCast(this->GetNode()));
        const SelectorProbability* pSelectorProbabilityNode = (const SelectorProbability*)(this->GetNode());

        if (childStatus != BT_RUNNING) {
            return childStatus;
        }

        //check if we've already chosen a node to run
        if (this->m_activeChildIndex != CompositeTask::InvalidChildIndex) {
            BehaviorTask* pNode = this->m_children[this->m_activeChildIndex];

            EBTStatus status = pNode->exec(pAgent);

            return status;
        }

        BEHAVIAC_ASSERT(this->m_weightingMap.size() == this->m_children.size());

        //generate a number between 0 and the sum of the weights
        double chosen = this->m_totalSum * GetRandomValue(pSelectorProbabilityNode->m_method, pAgent);

        double sum = 0;

        for (uint32_t i = 0; i < this->m_children.size(); ++i) {
            int w = this->m_weightingMap[i];

            sum += w;

            if (w > 0 && sum >= chosen) { //execute this node
                BehaviorTask* pChild = this->m_children[i];

                EBTStatus status = pChild->exec(pAgent);

                if (status == BT_RUNNING) {
                    this->m_activeChildIndex = i;

                } else {
                    this->m_activeChildIndex = CompositeTask::InvalidChildIndex;
                }

                return status;
            }
        }

        return BT_FAILURE;
    }
}//namespace namespace behaviac
