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
#include "behaviac/behaviortree/nodes/actions/waitframes.h"
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    WaitFrames::WaitFrames() : m_frames(0) {
    }

    WaitFrames::~WaitFrames() {
        BEHAVIAC_DELETE(this->m_frames);
    }

    //Property* LoadRight(const char* value, const behaviac::string& propertyName, behaviac::string& typeName);
    //behaviac::CMethodBase* LoadMethod(const char* value);

    void WaitFrames::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

			if (StringUtils::StringEqual(p.name, "Frames")) {
                const char* pParenthesis = strchr(p.value, '(');

                if (pParenthesis == 0) {
                    behaviac::string typeName;
                    behaviac::string propertyName;
                    //this->m_frames_var = Condition::LoadRight(p.value, typeName);
                    this->m_frames = AgentMeta::ParseProperty(p.value);

                } else {
                    //method
                    //this->m_frames_method = Action::LoadMethod(p.value);
                    this->m_frames = AgentMeta::ParseMethod(p.value);

                }
            }
        }
    }

    int WaitFrames::GetFrames(Agent* pAgent) const {
        if (this->m_frames != NULL) {
            uint64_t frames = *(uint64_t*)this->m_frames->GetValue(pAgent);
			return (frames == ((uint64_t)-1) ? -1 : (int)(frames & 0x0000FFFF));
        }

        return 0;
    }

    BehaviorTask* WaitFrames::createTask() const {
        WaitFramesTask* pTask = BEHAVIAC_NEW WaitFramesTask();

        return pTask;
    }

    WaitFramesTask::WaitFramesTask() : LeafTask(), m_start(0), m_frames(0) {
    }

    void WaitFramesTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(WaitFramesTask::DynamicCast(target));
        WaitFramesTask* ttask = (WaitFramesTask*)target;
        ttask->m_start = this->m_start;
        ttask->m_frames = this->m_frames;
    }

    void WaitFramesTask::save(IIONode* node) const {
        super::save(node);

        if (this->m_status != BT_INVALID) {
            CIOID  startId("start");
            node->setAttr(startId, this->m_start);

            CIOID  framesId("frames");
            node->setAttr(framesId, this->m_frames);
        }
    }

    void WaitFramesTask::load(IIONode* node) {
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

    WaitFramesTask::~WaitFramesTask() {
    }

    int WaitFramesTask::GetFrames(Agent* pAgent) const {
        BEHAVIAC_ASSERT(WaitFrames::DynamicCast(this->GetNode()));
        const WaitFrames* pWaitNode = (const WaitFrames*)(this->GetNode());

        return pWaitNode ? pWaitNode->GetFrames(pAgent) : 0;
    }

    bool WaitFramesTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        this->m_start = Workspace::GetInstance()->GetFrameSinceStartup();
        this->m_frames = this->GetFrames(pAgent);

        if (this->m_frames <= 0) {
            return false;
        }

        return true;
    }

    void WaitFramesTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus WaitFramesTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);

        int frames = Workspace::GetInstance()->GetFrameSinceStartup();

        if (frames - this->m_start + 1 >= this->m_frames) {
            return BT_SUCCESS;
        }

        return BT_RUNNING;
    }
}
