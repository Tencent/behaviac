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
#include "behaviac/behaviortree/nodes/decorators/decoratornot.h"

namespace behaviac {
    DecoratorNot::DecoratorNot()
    {}

    DecoratorNot::~DecoratorNot()
    {}
    bool DecoratorNot::Evaluate(Agent* pAgent) {
        BEHAVIAC_ASSERT(this->m_children->size() == 1);
        bool ret = (*this->m_children)[0]->Evaluate(pAgent);
        return !ret;
    }

    void DecoratorNot::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool DecoratorNot::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!DecoratorNot::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* DecoratorNot::createTask() const {
        DecoratorNotTask* pTask = BEHAVIAC_NEW DecoratorNotTask();

        return pTask;
    }

    void DecoratorNotTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void DecoratorNotTask::save(IIONode* node) const {
        super::save(node);
    }

    void DecoratorNotTask::load(IIONode* node) {
        super::load(node);
    }

    EBTStatus DecoratorNotTask::decorate(EBTStatus status) {
        if (status == BT_FAILURE) {
            return BT_SUCCESS;
        }

        if (status == BT_SUCCESS) {
            return BT_FAILURE;
        }

        return status;
    }
}//namespace behaviac
