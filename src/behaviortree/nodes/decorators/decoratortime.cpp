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
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/nodes/decorators/decoratortime.h"
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    DecoratorTime::DecoratorTime() : m_time(0) {
    }

    DecoratorTime::~DecoratorTime() {
        BEHAVIAC_DELETE(m_time);
    }

    void DecoratorTime::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Time")) {
                if (StringUtils::IsValidString(p.value)) {
                    const char* pParenthesis = StringUtils::StringFind(p.value, '(');

                    if (pParenthesis == 0) {
                        behaviac::string typeName;
                        this->m_time = AgentMeta::ParseProperty(p.value);
                    } else {
                        this->m_time = AgentMeta::ParseMethod(p.value);
                    }
                }
            }
        }
    }

    double DecoratorTime::GetTime(Agent* pAgent) const {
        double time = 0;

        if (this->m_time != NULL) {
            int typeNumberId = this->m_time->GetClassTypeNumberId();

            if (typeNumberId == GetClassTypeNumberId<int>()) {
                time = *(int*)this->m_time->GetValue(pAgent);
            } else if (typeNumberId == GetClassTypeNumberId<double>()) {
                time = *(double*)this->m_time->GetValue(pAgent);
            } else if (typeNumberId == GetClassTypeNumberId<float>()) {
                time = *(float*)this->m_time->GetValue(pAgent);
            } else {
                BEHAVIAC_ASSERT(false);
            }
        }

        return time;
    }

    int DecoratorTime::GetIntTime(Agent* pAgent) const {
        int time = 0;

		if (this->m_time) {
			int typeNumberId = this->m_time->GetClassTypeNumberId();

			if (typeNumberId == GetClassTypeNumberId<int>()) {
				time = *(int*)this->m_time->GetValue(pAgent);
			}
			else {
				BEHAVIAC_ASSERT(false);
			}
		}

        return time;
    }

    BehaviorTask* DecoratorTime::createTask() const {
        DecoratorTimeTask* pTask = BEHAVIAC_NEW DecoratorTimeTask();

        return pTask;
    }

    DecoratorTimeTask::DecoratorTimeTask() : DecoratorTask(), m_start(0), m_time(0), m_intStart(0), m_intTime(0) {
    }

    DecoratorTimeTask::~DecoratorTimeTask() {
    }

    double DecoratorTimeTask::GetTime(Agent* pAgent) const {
        BEHAVIAC_ASSERT(DecoratorTime::DynamicCast(this->GetNode()));
        const DecoratorTime* pNode = (const DecoratorTime*)(this->GetNode());

        return pNode ? pNode->GetTime(pAgent) : 0;
    }

    int DecoratorTimeTask::GetIntTime(Agent* pAgent) const {
        BEHAVIAC_ASSERT(DecoratorTime::DynamicCast(this->GetNode()));
        const DecoratorTime* pNode = (const DecoratorTime*)(this->GetNode());

        return pNode ? pNode->GetIntTime(pAgent) : 0;
    }

    void DecoratorTimeTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(DecoratorTimeTask::DynamicCast(target));
        DecoratorTimeTask* ttask = (DecoratorTimeTask*)target;

        ttask->m_start = this->m_start;
        ttask->m_time = this->m_time;

        ttask->m_intStart = this->m_intStart;
        ttask->m_intTime = this->m_intTime;
    }

    void DecoratorTimeTask::save(IIONode* node) const {
        super::save(node);

        if (this->m_status != BT_INVALID) {
            CIOID  startId("start");
            node->setAttr(startId, this->m_start);

            CIOID  timeId("time");
            node->setAttr(timeId, this->m_time);

            CIOID  intStartId("intstart");
            node->setAttr(intStartId, this->m_intStart);

            CIOID  intTimeId("inttime");
            node->setAttr(intTimeId, this->m_intTime);
        }
    }

    void DecoratorTimeTask::load(IIONode* node) {
        super::load(node);

        if (this->m_status != BT_INVALID) {
            behaviac::string attrStr;

            CIOID  startId("start");
            node->getAttr(startId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_start);

            CIOID  timeId("time");
            node->getAttr(timeId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_time);

            CIOID  intStartId("intstart");
            node->getAttr(intStartId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_intStart);

            CIOID  intTimeId("inttime");
            node->getAttr(intTimeId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_intTime);
        }
    }

    bool DecoratorTimeTask::onenter(Agent* pAgent) {
        super::onenter(pAgent);

        bool bUseIntValue = Workspace::GetInstance()->GetUseIntValue();

        if (bUseIntValue) {
            this->m_intStart = Workspace::GetInstance()->GetIntValueSinceStartup();
            this->m_intTime = this->GetIntTime(pAgent);

            if (this->m_intTime <= 0) {
                return false;
            }
        } else {
            this->m_start = Workspace::GetInstance()->GetDoubleValueSinceStartup();
            this->m_time = this->GetTime(pAgent);

            if (this->m_time <= 0) {
                return false;
            }
        }

        return true;
    }

    EBTStatus DecoratorTimeTask::decorate(EBTStatus status) {
        BEHAVIAC_UNUSED_VAR(status);

        bool bUseIntValue = Workspace::GetInstance()->GetUseIntValue();

        if (bUseIntValue) {
            long long time = Workspace::GetInstance()->GetIntValueSinceStartup();

            if (time - this->m_intStart >= this->m_intTime) {
                return BT_SUCCESS;
            }
        } else {
            double time = Workspace::GetInstance()->GetDoubleValueSinceStartup();

            if (time - this->m_start >= this->m_time) {
                return BT_SUCCESS;
            }
        }

        return BT_RUNNING;
    }
}
