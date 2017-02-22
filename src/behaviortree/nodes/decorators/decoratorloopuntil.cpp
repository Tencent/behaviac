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
#include "behaviac/behaviortree/nodes/decorators/decoratorloopuntil.h"

namespace behaviac {
    DecoratorLoopUntil::DecoratorLoopUntil() : m_until(true)
    {}

    DecoratorLoopUntil::~DecoratorLoopUntil()
    {}

    void DecoratorLoopUntil::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Until")) {
				if (StringUtils::StringEqual(p.value, "true")) {
                    this->m_until = true;

				}
				else if (StringUtils::StringEqual(p.value, "false")) {
                    this->m_until = false;
                }
            }
        }
    }

    BehaviorTask* DecoratorLoopUntil::createTask() const {
        DecoratorLoopUntilTask* pTask = BEHAVIAC_NEW DecoratorLoopUntilTask();

        return pTask;
    }

    void DecoratorLoopUntilTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void DecoratorLoopUntilTask::save(IIONode* node) const {
        super::save(node);
    }

    void DecoratorLoopUntilTask::load(IIONode* node) {
        super::load(node);
    }

    EBTStatus DecoratorLoopUntilTask::decorate(EBTStatus status) {
        if (this->m_n > 0) {
            this->m_n--;
        }

        if (this->m_n == 0) {
            return BT_SUCCESS;
        }

        BEHAVIAC_ASSERT(DecoratorLoopUntil::DynamicCast(this->GetNode()));
        DecoratorLoopUntil* pDecoratorLoopUntil = (DecoratorLoopUntil*)(this->GetNode());

        if (pDecoratorLoopUntil->m_until) {
            if (status == BT_SUCCESS) {
                return BT_SUCCESS;
            }
        } else {
            if (status == BT_FAILURE) {
                return BT_FAILURE;
            }
        }

        return BT_RUNNING;
    }
}//namespace behaviac
