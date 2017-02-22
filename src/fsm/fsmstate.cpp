#include "behaviac/fsm/state.h"
#include "behaviac/behaviortree/attachments/effector.h"
#include "behaviac/fsm/startcondition.h"
#include "behaviac/fsm/transitioncondition.h"
#include "behaviac/common/meta.h"
#include "behaviac/common/member.h"

namespace behaviac {
    State::State() : m_bIsEndState(false), m_method(0) {
    }

    State::~State() {
        BEHAVIAC_DELETE(m_method);
    }

    void State::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Method")) {
                if (p.value[0] != '\0') {
                    this->m_method = AgentMeta::ParseMethod(p.value);
                }
            } else if (StringUtils::StringEqual(p.name, "IsEndState")) {
                if (p.value[0] != '\0') {
                    if (StringUtils::StringEqual(p.value, "true")) {
                        this->m_bIsEndState = true;
                    }
                }//if (p.value[0] != '\0')
            }
        }
    }

    void State::Attach(BehaviorNode* pAttachment, bool bIsPrecondition, bool bIsEffector, bool bIsTransition) {
        if (bIsTransition) {
            BEHAVIAC_ASSERT(!bIsEffector && !bIsPrecondition);

            Transition* pTransition = (Transition*)pAttachment;
            BEHAVIAC_ASSERT(pTransition != 0);
            this->m_transitions.push_back(pTransition);

            //time£º2015-07-24 15:49:05
            return;
        }

        BEHAVIAC_ASSERT(bIsTransition == false);
        super::Attach(pAttachment, bIsPrecondition, bIsEffector, bIsTransition);
    }

    bool State::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (State::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* State::createTask() const {
        StateTask* pTask = BEHAVIAC_NEW StateTask();

        return pTask;
    }

    bool State::IsEndState() const {
        return this->m_bIsEndState;
    }

    uint32_t SetNodeId(uint32_t nodeId);
    void ClearNodeId(uint32_t slot);

    EBTStatus State::update_impl(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);
        return BT_RUNNING;
    }

    EBTStatus State::Execute(Agent* pAgent) {
        EBTStatus result = BT_RUNNING;

        if (this->m_method) {
            //uint32_t nodeId = this->GetId();

            //uint32_t slot = SetNodeId(nodeId);
            //BEHAVIAC_ASSERT(slot != (uint32_t)-1, "no empty slot found!");

            //const Agent* pParent = this->m_method->GetParentAgent(pAgent);
            //this->m_method->run(pParent, pAgent);

            //ClearNodeId(slot);
            this->m_method->run(pAgent);
        } else {
            result = this->update_impl((Agent*)pAgent, BT_RUNNING);
        }

        return result;
    }

    //nextStateId holds the next state id if it returns running when a certain transition is satisfied
    //otherwise, it returns success or failure if it ends
    EBTStatus State::Update(Agent* pAgent, int& nextStateId) {
        nextStateId = -1;

        //when no method is specified(m_method == 0),
        //'update_impl' is used to return the configured result status for both xml/bson and c#
        EBTStatus result = this->Execute(pAgent);

        if (this->m_bIsEndState) {
            result = BT_SUCCESS;
        } else {
            bool bTransitioned = UpdateTransitions(pAgent, this, (const behaviac::vector<Transition*>*)&this->m_transitions, nextStateId, result);

            if (bTransitioned) {
                //it will transition to another state, set result as success so as it exits
                result = BT_SUCCESS;
            }
        }

        return result;
    }

    void CHECK_BREAKPOINT(Agent* pAgent, const BehaviorNode* b, const char* action, EActionResult actionResult);

    bool State::UpdateTransitions(Agent* pAgent, const BehaviorNode* node, const behaviac::vector<Transition*>* transitions, int& nextStateId, EBTStatus result) {
        BEHAVIAC_UNUSED_VAR(node);
        bool bTransitioned = false;

        if (transitions && transitions->size() > 0) {
            for (uint32_t i = 0; i < transitions->size(); ++i) {
                Transition* transition = (*transitions)[i];

                if (transition->Evaluate(pAgent, result)) {
                    nextStateId = transition->GetTargetStateId();
                    BEHAVIAC_ASSERT(nextStateId != -1);

                    //transition actions
                    transition->ApplyEffects(pAgent, Effector::E_BOTH);

#if !BEHAVIAC_RELEASE

                    if (Config::IsLoggingOrSocketing()) {
                        CHECK_BREAKPOINT(pAgent, node, "transition", EAR_none);
                    }

#endif
                    bTransitioned = true;

                    break;
                }
            }
        }

        return bTransitioned;
    }

    StateTask::StateTask() : m_nextStateId(-1) {
    }

    StateTask::~StateTask() {
    }

    void StateTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void StateTask::save(IIONode* node) const {
        super::save(node);
    }

    void StateTask::load(IIONode* node) {
        super::load(node);
    }

    int StateTask::GetNextStateId() const {
        return m_nextStateId;
    }

    bool StateTask::IsEndState() const {
        BEHAVIAC_ASSERT(State::DynamicCast(this->GetNode()) != 0, "node is not an State");

        State* pStateNode = (State*)(this->GetNode());

        return pStateNode->IsEndState();
    }

    bool StateTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        this->m_nextStateId = -1;
        return true;
    }

    void StateTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus StateTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(childStatus == BT_RUNNING);
        BEHAVIAC_UNUSED_VAR(childStatus);
        BEHAVIAC_ASSERT(State::DynamicCast(this->GetNode()) != 0, "node is not an State");

        State* pStateNode = (State*)(this->GetNode());

        EBTStatus result = pStateNode->Update(pAgent, this->m_nextStateId);

        return result;
    }
}


