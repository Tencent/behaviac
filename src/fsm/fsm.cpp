#include "behaviac/fsm/fsm.h"
#include "behaviac/fsm/state.h"

namespace behaviac {
    FSM::FSM() : m_initialId(-1) {
    }

    FSM::~FSM() {
    }

    void FSM::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

			if (StringUtils::StringEqual(p.name, "initialid")) {
                this->m_initialId = atoi(p.value);
            }
        }
    }

    bool FSM::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!FSM::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* FSM::createTask() const {
        FSMTask* pTask = BEHAVIAC_NEW FSMTask();

        return pTask;
    }

    void FSM::SetInitialId(int initialId) {
        this->m_initialId = initialId;
    }

    int FSM::GetInitialId() const {
        return this->m_initialId;
    }

#if BEHAVIAC_USE_HTN

    bool FSM::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        BEHAVIAC_UNUSED_VAR(node);
        BEHAVIAC_UNUSED_VAR(seqTask);
        BEHAVIAC_UNUSED_VAR(depth);
        BEHAVIAC_UNUSED_VAR(planner);

        BEHAVIAC_ASSERT(false, "Can't step into this line");
        return false;
    }
#endif //BEHAVIAC_USE_HTN

    FSMTask::FSMTask() : CompositeTask() {
    }

    FSMTask::~FSMTask()
    {}

    void FSMTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void FSMTask::save(IIONode* node) const {
        super::save(node);
    }

    void FSMTask::load(IIONode* node) {
        super::load(node);
    }

    bool FSMTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        FSM* fsm = (FSM*)this->m_node;
        this->m_activeChildIndex = 0;

        this->m_currentNodeId = fsm->GetInitialId();

        return true;
    }

    void FSMTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
        this->m_currentNodeId = -1;

        super::onexit(pAgent, s);
    }

#if !BEHAVIAC_RELEASE
    const behaviac::string GetParentTreeName(const Agent* pAgent, const BehaviorNode* n);
#endif

    EBTStatus FSMTask::UpdateFSM(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(this->m_node != 0);
        BEHAVIAC_ASSERT(this->m_currentNodeId != -1);

        EBTStatus status = childStatus;
        bool bLoop = true;

#if !BEHAVIAC_RELEASE
        const int kMaxCount = 10;
        behaviac::map<int, int> state_update_count;
#endif//#if !BEHAVIAC_RELEASE

        while (bLoop) {
            BehaviorTask* currentState = this->GetChildById(this->m_currentNodeId);
            //BEHAVIAC_ASSERT(currentState->GetNextStateId() == -1, "m_nextStateId is not reset to -1 in onenter");
            currentState->exec(pAgent);

            if (StateTask::DynamicCast(currentState) != 0) {
                StateTask* pStateTask = (StateTask*)currentState;

                if (pStateTask->IsEndState()) {
                    return BT_SUCCESS;
                }
            }

            int nextStateId = currentState->GetNextStateId();

            if (nextStateId < 0) {
                //if not transitioned, don't go on next state, to exit
                bLoop = false;
            } else {
#if !BEHAVIAC_RELEASE
                state_update_count[this->m_currentNodeId]++;

                if (state_update_count[this->m_currentNodeId] > kMaxCount) {
                    behaviac::string treeName = GetParentTreeName(pAgent, this->GetNode());
                    BEHAVIAC_LOGERROR("%s might be updating an FSM('%s') endlessly, possibly a dead loop, please redesign it!", pAgent->GetName().c_str(), treeName.c_str());
                    BEHAVIAC_ASSERT(false);
                }

#endif

                //if transitioned, go on next state
                this->m_currentNodeId = nextStateId;
            }
        }

        return status;
    }

    EBTStatus FSMTask::update_current(Agent* pAgent, EBTStatus childStatus) {
        EBTStatus status = this->update(pAgent, childStatus);

        return status;
    }

    EBTStatus FSMTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(childStatus);

        BEHAVIAC_ASSERT(this->m_activeChildIndex < (int)this->m_children.size());
        // FSM* node = (FSM*)this->m_node;

        EBTStatus s = this->UpdateFSM(pAgent, childStatus);

        return s;
    }
}
