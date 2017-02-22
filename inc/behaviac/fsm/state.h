#ifndef _BEHAVIAC_FSM_STATE_H_
#define _BEHAVIAC_FSM_STATE_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"
#include "behaviac/behaviortree/nodes/actions/action.h"

namespace behaviac {
    class Transition;
    // ============================================================================
    class BEHAVIAC_API State : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(State, BehaviorNode);

        State();
        virtual ~State();
    protected:
        virtual void load(int version, const char* agentType, const properties_t& properties);
        virtual EBTStatus update_impl(Agent* pAgent, EBTStatus childStatus);
        virtual BehaviorTask* createTask() const;
        EBTStatus Execute(Agent* pAgent);
    public:
        virtual void Attach(BehaviorNode* pAttachment, bool bIsPrecondition, bool bIsEffector, bool bIsTransition);
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
        bool IsEndState() const;

        EBTStatus Update(Agent* pAgent, int& nextStateId);
        static bool UpdateTransitions(Agent* pAgent, const BehaviorNode* node, const behaviac::vector<Transition*>* transitions, int& nextStateId, EBTStatus result);
    protected:
        bool							m_bIsEndState;
        behaviac::IInstanceMember*					m_method;
        behaviac::vector<Transition*>	m_transitions;
    };

    class StateTask : public LeafTask {
    public:
        StateTask();

        virtual ~StateTask();
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual  int GetNextStateId() const;
        bool IsEndState() const;
    protected:
        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    protected:
        int m_nextStateId;
    };

}
#endif
