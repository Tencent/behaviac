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
#include "behaviac/behaviortree/nodes/decorators/decoratoralwayssuccess.h"

namespace behaviac {
    DecoratorAlwaysSuccess::DecoratorAlwaysSuccess()
    {}

    DecoratorAlwaysSuccess::~DecoratorAlwaysSuccess()
    {}

    void DecoratorAlwaysSuccess::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool DecoratorAlwaysSuccess::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!DecoratorAlwaysSuccess::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* DecoratorAlwaysSuccess::createTask() const {
        DecoratorAlwaysSuccessTask* pTask = BEHAVIAC_NEW DecoratorAlwaysSuccessTask();

        return pTask;
    }

    void DecoratorAlwaysSuccessTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void DecoratorAlwaysSuccessTask::save(IIONode* node) const {
        super::save(node);
    }

    void DecoratorAlwaysSuccessTask::load(IIONode* node) {
        super::load(node);
    }

    EBTStatus DecoratorAlwaysSuccessTask::decorate(EBTStatus status) {
        BEHAVIAC_UNUSED_VAR(status);

        return BT_SUCCESS;
    }
}//namespace behaviac
