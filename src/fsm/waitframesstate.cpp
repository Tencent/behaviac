#include "behaviac/fsm/waitframesstate.h"
#include "behaviac/behaviortree/attachments/effector.h"
#include "behaviac/fsm/startcondition.h"
#include "behaviac/fsm/transitioncondition.h"
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    WaitFramesState::WaitFramesState() : m_frames(0) {
    }

    WaitFramesState::~WaitFramesState() {
        BEHAVIAC_DELETE(this->m_frames);
    }

    void WaitFramesState::load(int version, const char* agentType, const properties_t& properties) {
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
                    this->m_frames = AgentMeta::ParseMethod(p.value);
                    //this->m_frames_method = Action::LoadMethod(p.value);
                }
            }
        }
    }

    int WaitFramesState::GetFrames(Agent* pAgent) const {
        if (this->m_frames != NULL) {
            uint64_t frames = *(uint64_t*)this->m_frames->GetValue(pAgent);
			return (frames == ((uint64_t)-1) ? -1 : (int)(frames & 0x0000FFFF));
        }

        return 0;
    }

    bool WaitFramesState::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (WaitFramesState::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* WaitFramesState::createTask() const {
        WaitFramesStateTask* pTask = BEHAVIAC_NEW WaitFramesStateTask();

        return pTask;
    }

    WaitFramesStateTask::WaitFramesStateTask() : StateTask(), m_start(0), m_frames(0) {
    }

    WaitFramesStateTask::~WaitFramesStateTask() {
    }

    void WaitFramesStateTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(WaitFramesStateTask::DynamicCast(target));
        WaitFramesStateTask* ttask = (WaitFramesStateTask*)target;
        ttask->m_start = this->m_start;
        ttask->m_frames = this->m_frames;
    }

    void WaitFramesStateTask::save(IIONode* node) const {
        super::save(node);

        if (this->m_status != BT_INVALID) {
            CIOID  startId("start");
            node->setAttr(startId, this->m_start);

            CIOID  framesId("frames");
            node->setAttr(framesId, this->m_frames);
        }
    }

    void WaitFramesStateTask::load(IIONode* node) {
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

    int WaitFramesStateTask::GetFrames(Agent* pAgent) const {
        BEHAVIAC_ASSERT(WaitFramesState::DynamicCast(this->GetNode()));
        const WaitFramesState* pWaitNode = (const WaitFramesState*)(this->GetNode());

        return pWaitNode ? pWaitNode->GetFrames(pAgent) : 0;
    }

    bool WaitFramesStateTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);

        this->m_nextStateId = -1;

        this->m_start = Workspace::GetInstance()->GetFrameSinceStartup();
        this->m_frames = this->GetFrames(pAgent);

        if (this->m_frames <= 0) {
            return false;
        }

        return true;
    }

    void WaitFramesStateTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus WaitFramesStateTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);

        if (Workspace::GetInstance()->GetFrameSinceStartup() - this->m_start + 1 >= this->m_frames) {
            BEHAVIAC_ASSERT(WaitFramesState::DynamicCast(this->GetNode()) != 0, "node is not an WaitFramesState");
            WaitFramesState* pStateNode = (WaitFramesState*)(this->GetNode());

            pStateNode->Update(pAgent, this->m_nextStateId);

            return BT_SUCCESS;
        }

        return BT_RUNNING;
    }
}
