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
#include "behaviac/behaviortree/nodes/decorators/decoratorlog.h"

namespace behaviac {
    DecoratorLog::DecoratorLog()
    {}

    DecoratorLog::~DecoratorLog()
    {}

    void DecoratorLog::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Log")) {
                this->m_message = p.value;
            }
        }
    }

    bool DecoratorLog::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!DecoratorLog::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* DecoratorLog::createTask() const {
        DecoratorLogTask* pTask = BEHAVIAC_NEW DecoratorLogTask();

        return pTask;
    }

    void DecoratorLogTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void DecoratorLogTask::save(IIONode* node) const {
        super::save(node);
    }

    void DecoratorLogTask::load(IIONode* node) {
        super::load(node);
    }

    EBTStatus DecoratorLogTask::decorate(EBTStatus status) {
        BEHAVIAC_ASSERT(DecoratorLog::DynamicCast(this->GetNode()));
        const DecoratorLog* pDecoratorLogNode = (const DecoratorLog*)(this->GetNode());
        BEHAVIAC_LOGINFO("DecoratorLogTask:%s\n", pDecoratorLogNode->m_message.c_str());

        return status;
    }
}//namespace behaviac
