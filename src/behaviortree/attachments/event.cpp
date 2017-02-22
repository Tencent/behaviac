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
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"

#include "behaviac/behaviortree/attachments/event.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    Event::Event() : m_event(0), m_triggerMode(TM_Transfer), m_bTriggeredOnce(false) {
        m_eventName[0] = 0;
    }

    Event::~Event() {
        BEHAVIAC_DELETE(m_event);
    }

    //behaviac::CMethodBase* LoadMethod(const char* value);

    bool Event::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Event::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    void Event::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        behaviac::string typeName;
        behaviac::string propertyName;
        behaviac::string comparatorName;

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Task")) {
                //method
                //this->m_event = Action::LoadMethod(p.value);
                this->m_event = AgentMeta::ParseMethod(p.value, this->m_eventName);
            } else if (StringUtils::StringEqual(p.name, "ReferenceFilename")) {
                this->m_referencedBehaviorPath = p.value;
				if (Config::PreloadBehaviors()) {
					BehaviorTree* behaviorTree = Workspace::GetInstance()->LoadBehaviorTree(p.value);
					BEHAVIAC_ASSERT(behaviorTree);
					BEHAVIAC_UNUSED_VAR(behaviorTree);
				}
            } else if (StringUtils::StringEqual(p.name, "TriggeredOnce")) {
				if (StringUtils::StringEqual(p.value, "true")) {
                    this->m_bTriggeredOnce = true;
                }
            } else if (StringUtils::StringEqual(p.name, "TriggerMode")) {
				if (StringUtils::StringEqual(p.value, "Transfer")) {
                    this->m_triggerMode = TM_Transfer;
				}
				else if (StringUtils::StringEqual(p.value, "Return")) {
                    this->m_triggerMode = TM_Return;
                } else {
                    BEHAVIAC_ASSERT(0, "unrecognised trigger mode %s", p.value);
                }
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }
    }

    const char* Event::GetEventName() {
        return this->m_eventName;
    }

    bool Event::TriggeredOnce() {
        return this->m_bTriggeredOnce;
    }

    TriggerMode Event::GetTriggerMode() {
        return m_triggerMode;
    }

    void Event::switchTo(Agent* pAgent, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams) {
        if (!StringUtils::IsNullOrEmpty(this->m_referencedBehaviorPath.c_str())) {
            if (pAgent != NULL) {
                TriggerMode tm = this->GetTriggerMode();

                pAgent->bteventtree(this->m_referencedBehaviorPath.c_str(), tm);
                BehaviorTreeTask* pCurrentTree = pAgent->btgetcurrent();
                BEHAVIAC_ASSERT(pCurrentTree);
                pCurrentTree->AddVariables(eventParams);
                pAgent->btexec();
            }
        }
    }

    BehaviorTask* Event::createTask() const {
        EventetTask* pTask = BEHAVIAC_NEW EventetTask();

        return pTask;
    }

    EventetTask::EventetTask() : AttachmentTask() {
    }

    EventetTask::~EventetTask() {
    }

    //bool EventetTask::NeedRestart() const
    //{
    //	return true;
    //}

    void EventetTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void EventetTask::save(IIONode* node) const {
        super::save(node);
    }

    void EventetTask::load(IIONode* node) {
        super::load(node);
    }

    bool EventetTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        return true;
    }

    void EventetTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    bool EventetTask::TriggeredOnce() const {
        const Event* pEventNode = Event::DynamicCast(this->GetNode());
        return pEventNode->m_bTriggeredOnce;
    }

    TriggerMode EventetTask::GetTriggerMode() const {
        const Event* pEventNode = Event::DynamicCast(this->GetNode());
        return pEventNode->m_triggerMode;
    }

    const char* EventetTask::GetEventName() const {
        const Event* pEventNode = Event::DynamicCast(this->GetNode());
        return pEventNode->m_eventName;
    }

    EBTStatus EventetTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);

        BEHAVIAC_ASSERT(Event::DynamicCast(this->GetNode()));
        const Event* pEventNode = (const Event*)(this->GetNode());

        if (!pEventNode->m_referencedBehaviorPath.empty()) {
            if (pAgent) {
                TriggerMode tm = this->GetTriggerMode();

                pAgent->bteventtree(pEventNode->m_referencedBehaviorPath.c_str(), tm);
                EBTStatus s = pAgent->btexec();
                BEHAVIAC_UNUSED_VAR(s);
            }
        }

        return BT_SUCCESS;
    }
}
