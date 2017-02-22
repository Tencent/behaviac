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
#include "behaviac/behaviortree/nodes/conditions/and.h"

namespace behaviac {
    And::And()
    {}

    And::~And()
    {}

    void And::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool And::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!And::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    bool And::Evaluate(Agent* pAgent) {
        bool ret = true;

        for (behaviac::vector<BehaviorNode*>::iterator it = this->m_children->begin(); it != this->m_children->end(); ++it) {
            ret = (*it)->Evaluate(pAgent);

            if (!ret) {
                break;
            }
        }

        return ret;
    }

    BehaviorTask* And::createTask() const {
        AndTask* pTask = BEHAVIAC_NEW AndTask();

        return pTask;
    }

    void AndTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void AndTask::save(IIONode* node) const {
        super::save(node);
    }

    void AndTask::load(IIONode* node) {
        super::load(node);
    }

    EBTStatus AndTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);

        //BEHAVIAC_ASSERT(this->m_children.size() == 2);

        for (BehaviorTasks_t::iterator it = this->m_children.begin(); it != this->m_children.end(); ++it) {
            BehaviorTask* pBehavior = *it;
            EBTStatus s = pBehavior->exec(pAgent);

            // If the child fails, fails
            if (s == BT_FAILURE) {
                return s;
            }

            BEHAVIAC_ASSERT(s == BT_SUCCESS);
        }

        return BT_SUCCESS;
    }
}
