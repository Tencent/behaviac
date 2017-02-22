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
#include "behaviac/behaviortree/nodes/conditions/or.h"

namespace behaviac {
    Or::Or()
    {}

    Or::~Or()
    {}

    void Or::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool Or::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Or::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    bool Or::Evaluate(Agent* pAgent) {
        bool ret = true;

        for (behaviac::vector<BehaviorNode*>::iterator it = this->m_children->begin(); it != this->m_children->end(); ++it) {
            ret = (*it)->Evaluate(pAgent);

            if (ret) {
                break;
            }
        }

        return ret;
    }
    BehaviorTask* Or::createTask() const {
        OrTask* pTask = BEHAVIAC_NEW OrTask();

        return pTask;
    }

    void OrTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void OrTask::save(IIONode* node) const {
        super::save(node);
    }

    void OrTask::load(IIONode* node) {
        super::load(node);
    }

    EBTStatus OrTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);
        //BEHAVIAC_ASSERT(this->m_children.size() == 2);

        for (BehaviorTasks_t::iterator it = this->m_children.begin(); it != this->m_children.end(); ++it) {
            BehaviorTask* pBehavior = *it;
            EBTStatus s = pBehavior->exec(pAgent);

            // If the child succeeds, succeeds
            if (s == BT_SUCCESS) {
                return s;
            }

            BEHAVIAC_ASSERT(s == BT_FAILURE);
        }

        return BT_FAILURE;
    }
}
