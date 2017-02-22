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
#include "behaviac/behaviortree/nodes/decorators/decoratorloop.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"

namespace behaviac {
    DecoratorLoop::DecoratorLoop() : m_bDoneWithinFrame(false) {
    }

    DecoratorLoop::~DecoratorLoop() {
    }

    void DecoratorLoop::load(int version, const char* agentType, const properties_t& properties) {
        DecoratorCount::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "DoneWithinFrame")) {
                if (p.value[0] != '\0') {
                    if (StringUtils::StringEqual(p.value, "true")) {
                        this->m_bDoneWithinFrame = true;
                    }
                }//if (p.value[0] != '\0')
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }

    }

#if BEHAVIAC_USE_HTN

    bool DecoratorLoop::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        bool bOk = false;
        DecoratorLoop* loop = (DecoratorLoop*)node;
        int childCount = loop->GetChildrenCount();
        BEHAVIAC_UNUSED_VAR(childCount);
        BEHAVIAC_ASSERT(childCount == 1);
        BehaviorNode* childNode = (BehaviorNode*)loop->GetChild(0);
        PlannerTask* childTask = planner->decomposeNode(childNode, depth);

        if (childTask != NULL) {
            seqTask->AddChild(childTask);
            bOk = true;
        }

        return bOk;
    }
#endif

    bool DecoratorLoop::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!DecoratorLoop::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* DecoratorLoop::createTask() const {
        DecoratorLoopTask* pTask = BEHAVIAC_NEW DecoratorLoopTask();

        return pTask;
    }

    DecoratorLoopTask::DecoratorLoopTask() : DecoratorCountTask() {
    }

    DecoratorLoopTask::~DecoratorLoopTask() {
    }

    void DecoratorLoopTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void DecoratorLoopTask::save(IIONode* node) const {
        super::save(node);
    }

    void DecoratorLoopTask::load(IIONode* node) {
        super::load(node);
    }

    EBTStatus DecoratorLoopTask::decorate(EBTStatus status) {
        BEHAVIAC_UNUSED_VAR(status);

        if (this->m_n > 0) {
            this->m_n--;

            if (this->m_n == 0) {
                return BT_SUCCESS;
            }

            return BT_RUNNING;
        }

        if (this->m_n == -1) {
            return BT_RUNNING;
        }

        BEHAVIAC_ASSERT(this->m_n == 0);

        return BT_SUCCESS;
    }

    EBTStatus DecoratorLoopTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(DecoratorLoop::DynamicCast(this->m_node));
        DecoratorLoop* node = (DecoratorLoop*)this->m_node;

        if (node->m_bDoneWithinFrame) {
            BEHAVIAC_ASSERT(this->m_n >= 0);
            BEHAVIAC_ASSERT(this->m_root != NULL);

            EBTStatus status = BT_INVALID;

            for (int i = 0; i < this->m_n; ++i) {
                status = this->m_root->exec(pAgent, childStatus);

                if (node->m_bDecorateWhenChildEnds) {
                    while (status == BT_RUNNING) {
                        status = super::update(pAgent, childStatus);
                    }
                }

                if (status == BT_FAILURE) {
                    return BT_FAILURE;
                }
            }

            return BT_SUCCESS;
        }

        return super::update(pAgent, childStatus);
    }

}//namespace behaviac
