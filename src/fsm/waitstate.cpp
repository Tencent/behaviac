#include "behaviac/fsm/waitstate.h"
#include "behaviac/behaviortree/attachments/effector.h"
#include "behaviac/fsm/startcondition.h"
#include "behaviac/fsm/transitioncondition.h"
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    WaitState::WaitState() : m_time(0) {
    }

    WaitState::~WaitState() {
        BEHAVIAC_DELETE(this->m_time);
    }

    void WaitState::load(int version, const char* agentType, const properties_t& properties) {
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

    double WaitState::GetTime(Agent* pAgent) const {
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

    int WaitState::GetIntTime(Agent* pAgent) const {
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

    bool WaitState::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (WaitState::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* WaitState::createTask() const {
        WaitStateTask* pTask = BEHAVIAC_NEW WaitStateTask();

        return pTask;
    }

    WaitStateTask::WaitStateTask() : StateTask(), m_start(0), m_time(0), m_intStart(0), m_intTime(0) {
    }

    WaitStateTask::~WaitStateTask() {
    }

    void WaitStateTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(WaitStateTask::DynamicCast(target));
        WaitStateTask* ttask = (WaitStateTask*)target;

        ttask->m_start = this->m_start;
        ttask->m_time = this->m_time;

        ttask->m_intStart = this->m_intStart;
        ttask->m_intTime = this->m_intTime;
    }

    void WaitStateTask::save(IIONode* node) const {
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

    void WaitStateTask::load(IIONode* node) {
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

    double WaitStateTask::GetTime(Agent* pAgent) const {
        const WaitState* pWaitNode = WaitState::DynamicCast(this->GetNode());

        return pWaitNode ? pWaitNode->GetTime(pAgent) : 0;
    }

    int WaitStateTask::GetIntTime(Agent* pAgent) const {
        const WaitState* pWaitNode = WaitState::DynamicCast(this->GetNode());

        return pWaitNode ? pWaitNode->GetIntTime(pAgent) : 0;
    }

    bool WaitStateTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        this->m_nextStateId = -1;

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

    void WaitStateTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus WaitStateTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(childStatus == BT_RUNNING);
        BEHAVIAC_UNUSED_VAR(childStatus);
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_ASSERT(WaitState::DynamicCast(this->GetNode()) != 0, "node is not an WaitState");

        WaitState* pStateNode = (WaitState*)(this->GetNode());

		bool bUseIntValue = Workspace::GetInstance()->GetUseIntValue();

		if (bUseIntValue) {
			long long time = Workspace::GetInstance()->GetIntValueSinceStartup();

			if (time - this->m_intStart >= this->m_intTime) {
				pStateNode->Update(pAgent, this->m_nextStateId);
				return BT_SUCCESS;
			}
		}
		else {
			double time = Workspace::GetInstance()->GetDoubleValueSinceStartup();

			if (time - this->m_start >= this->m_time) {
				pStateNode->Update(pAgent, this->m_nextStateId);
				return BT_SUCCESS;
			}
		}

        return BT_RUNNING;
    }
}
