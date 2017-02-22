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
#include "behaviac/behaviortree/nodes/decorators/decoratorframes.h"
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    DecoratorFrames::DecoratorFrames() : m_frames(0) {
    }

    DecoratorFrames::~DecoratorFrames() {
        BEHAVIAC_DELETE(this->m_frames);
    }

    void DecoratorFrames::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Frames")) {
                const char* pParenthesis = strchr(p.value, '(');

                if (pParenthesis == 0) {
                    behaviac::string typeName;
                    behaviac::string propertyName;
                    //this->m_frames = Condition::LoadRight(p.value, typeName);
                    this->m_frames = AgentMeta::ParseProperty(p.value);
                } else {
                    //method
                    //this->m_frames = Action::LoadMethod(p.value);
                    this->m_frames = AgentMeta::ParseMethod(p.value);
                }
            }
        }
    }

    int DecoratorFrames::GetFrames(Agent* pAgent) const {
        if (this->m_frames) {
            uint64_t frames = *(uint64_t*)this->m_frames->GetValue(pAgent);
			return (frames == ((uint64_t)-1) ? -1 : (int)(frames & 0x0000FFFF));
        }

        return 0;
    }

    BehaviorTask* DecoratorFrames::createTask() const {
        DecoratorFramesTask* pTask = BEHAVIAC_NEW DecoratorFramesTask();

        return pTask;
    }

    DecoratorFramesTask::DecoratorFramesTask() : DecoratorTask(), m_start(0), m_frames(0) {
    }

    DecoratorFramesTask::~DecoratorFramesTask() {
    }

    int DecoratorFramesTask::GetFrames(Agent* pAgent) const {
        BEHAVIAC_ASSERT(DecoratorFrames::DynamicCast(this->GetNode()));
        const DecoratorFrames* pNode = (const DecoratorFrames*)(this->GetNode());

        return pNode ? pNode->GetFrames(pAgent) : 0;
    }

    void DecoratorFramesTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(DecoratorFramesTask::DynamicCast(target));
        DecoratorFramesTask* ttask = (DecoratorFramesTask*)target;

        ttask->m_start = this->m_start;
        ttask->m_frames = this->m_frames;
    }

    void DecoratorFramesTask::save(IIONode* node) const {
        super::save(node);

        if (this->m_status != BT_INVALID) {
            CIOID  startId("start");
            node->setAttr(startId, this->m_start);

            CIOID  framesId("frames");
            node->setAttr(framesId, this->m_frames);
        }
    }

    void DecoratorFramesTask::load(IIONode* node) {
        super::load(node);

        if (this->m_status != BT_INVALID) {
            CIOID  startId("start");
            behaviac::string attrStr;
            node->getAttr(startId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_start);

            CIOID  framesId("frames");
            node->getAttr(framesId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_frames);
        }
    }

    bool DecoratorFramesTask::onenter(Agent* pAgent) {
        super::onenter(pAgent);

        this->m_start = Workspace::GetInstance()->GetFrameSinceStartup();
        this->m_frames = this->GetFrames(pAgent);

        return (this->m_frames > 0);
    }

    EBTStatus DecoratorFramesTask::decorate(EBTStatus status) {
        BEHAVIAC_UNUSED_VAR(status);

        if (Workspace::GetInstance()->GetFrameSinceStartup() - this->m_start + 1 >= this->m_frames) {
            return BT_SUCCESS;
        }

        return BT_RUNNING;
    }
}//namespace behaviac
