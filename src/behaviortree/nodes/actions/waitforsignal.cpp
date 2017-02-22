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
#include "behaviac/behaviortree/nodes/actions/waitforsignal.h"

namespace behaviac {
    WaitforSignal::WaitforSignal()
    {}

    WaitforSignal::~WaitforSignal()
    {}

    void WaitforSignal::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

    bool WaitforSignal::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!WaitforSignal::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    bool WaitforSignal::CheckIfSignaled(const Agent* pAgent) {
        bool ret = this->EvaluteCustomCondition(pAgent);
        return ret;
    }
    BehaviorTask* WaitforSignal::createTask() const {
        WaitforSignalTask* pTask = BEHAVIAC_NEW WaitforSignalTask();

        return pTask;
    }

    void WaitforSignalTask::Init(const BehaviorNode* node) {
        super::Init(node);
    }

    WaitforSignalTask::~WaitforSignalTask() {
    }

    void WaitforSignalTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(WaitforSignalTask::DynamicCast(target));
        WaitforSignalTask* ttask = (WaitforSignalTask*)target;

        ttask->m_bTriggered = this->m_bTriggered;
    }

    void WaitforSignalTask::save(IIONode* node) const {
        super::save(node);

        if (this->m_status != BT_INVALID) {
            CIOID  triggeredId("triggered");
            node->setAttr(triggeredId, this->m_bTriggered);
        }
    }

    void WaitforSignalTask::load(IIONode* node) {
        super::load(node);

        if (this->m_status != BT_INVALID) {
            CIOID  triggeredId("triggered");
            behaviac::string attrStr;
            node->getAttr(triggeredId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_bTriggered);
        }
    }

    bool WaitforSignalTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        this->m_bTriggered = false;

        return true;
    }

    void WaitforSignalTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus WaitforSignalTask::update(Agent* pAgent, EBTStatus childStatus) {
        if (childStatus != BT_RUNNING) {
            return childStatus;
        }

        if (!this->m_bTriggered) {
            WaitforSignal* node = (WaitforSignal*)this->m_node;
            this->m_bTriggered = node->CheckIfSignaled(pAgent);
        }

        if (this->m_bTriggered) {
            if (!this->m_root) {
                return BT_SUCCESS;
            }

            EBTStatus status = super::update(pAgent, childStatus);

            return status;
        }

        return BT_RUNNING;
    }
}
