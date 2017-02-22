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
#include "behaviac/behaviortree/nodes/composites/compositestochastic.h"
#include "behaviac/common/randomgenerator/randomgenerator.h"


#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/common/meta.h"
#include "behaviac/common/member.h"

namespace behaviac {
    CompositeStochastic::CompositeStochastic() : m_method(0)
    {}

    CompositeStochastic::~CompositeStochastic() {
        BEHAVIAC_DELETE(m_method);
    }

    //behaviac::CMethodBase* LoadMethod(const char* value);

    void CompositeStochastic::load(int version, const char* agentType, const properties_t& properties) {
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

    bool CompositeStochastic::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!CompositeStochastic::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    CompositeStochasticTask::CompositeStochasticTask() : CompositeTask() {
    }

    CompositeStochasticTask::~CompositeStochasticTask() {
    }

    //generate a random double value between 0.0 and 1.0
    double GetRandomValue(behaviac::IInstanceMember* method, Agent* pAgent) {
        double value = 0;

        if (method && method->GetClassTypeNumberId() == behaviac::GetClassTypeNumberId<float>()) {
            value = *(float*)method->GetValue(pAgent);
        } else {
            RandomGenerator* pRandomGenerator = RandomGenerator::GetInstance();

            value = (*pRandomGenerator)();
        }

        return value;
    }

    void CompositeStochasticTask::random_child(Agent* pAgent) {
        BEHAVIAC_ASSERT(!this->GetNode() || CompositeStochastic::DynamicCast(this->GetNode()));
        const CompositeStochastic* pNode = (const CompositeStochastic*)(this->GetNode());

        if (this->m_set.size() != this->m_children.size()) {
            this->m_set.resize(this->m_children.size());
        }

        uint32_t n = (uint32_t)this->m_set.size();

        for (uint32_t i = 0; i < n; ++i) {
            this->m_set[i] = i;
        }

        for (uint32_t i = 0; i < n; ++i) {
            uint32_t index1 = (uint32_t)(n * GetRandomValue(pNode ? pNode->m_method : 0, pAgent));
            BEHAVIAC_ASSERT(index1 < n);

            uint32_t index2 = (uint32_t)(n * GetRandomValue(pNode ? pNode->m_method : 0, pAgent));
            BEHAVIAC_ASSERT(index2 < n);

            //swap
            if (index1 != index2) {
                uint32_t old = this->m_set[index1];
                this->m_set[index1] = this->m_set[index2];
                this->m_set[index2] = old;
            }
        }
    }

    void CompositeStochasticTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(CompositeStochasticTask::DynamicCast(target));
        CompositeStochasticTask* ttask = (CompositeStochasticTask*)target;

        ttask->m_set = this->m_set;
    }

    void CompositeStochasticTask::save(IIONode* node) const {
        super::save(node);

        if (this->m_status != BT_INVALID) {
            CIOID  setId("set");
            node->setAttr(setId, this->m_set);
        }
    }

    void CompositeStochasticTask::load(IIONode* node) {
        super::load(node);

        if (this->m_status != BT_INVALID) {
            CIOID  setId("set");
            behaviac::string attrStr;
            node->getAttr(setId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_set);
        }
    }

    bool CompositeStochasticTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_ASSERT(this->m_children.size() > 0);

        this->random_child(pAgent);

        this->m_activeChildIndex = 0;
        return true;
    }

    EBTStatus CompositeStochasticTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        bool bFirst = true;

        BEHAVIAC_ASSERT(this->m_activeChildIndex != CompositeTask::InvalidChildIndex);

        // Keep going until a child behavior says its running.
        for (;;) {
            EBTStatus s = childStatus;

            if (!bFirst || s == BT_RUNNING) {
                uint32_t childIndex = this->m_set[this->m_activeChildIndex];
                BehaviorTask* pBehavior = this->m_children[childIndex];
                s = pBehavior->exec(pAgent);
            }

            bFirst = false;

            // If the child succeeds, or keeps running, do the same.
            if (s != BT_FAILURE) {
                return s;
            }

            // Hit the end of the array, job done!
            ++this->m_activeChildIndex;

            if (this->m_activeChildIndex >= (int)this->m_children.size()) {
                return BT_FAILURE;
            }
        }
    }

    void CompositeStochasticTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }
}
