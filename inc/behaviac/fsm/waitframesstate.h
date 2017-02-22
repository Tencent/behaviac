#ifndef _BEHAVIAC_FSM_WAITFRAMESSTATE_H_
#define _BEHAVIAC_FSM_WAITFRAMESSTATE_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"
#include "behaviac/fsm/state.h"

namespace behaviac {
    class Transition;
    // ============================================================================
    class BEHAVIAC_API WaitFramesState : public State {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(WaitFramesState, State);

        WaitFramesState();
        virtual ~WaitFramesState();
    protected:
        virtual void load(int version, const char* agentType, const properties_t& properties);
        virtual BehaviorTask* createTask() const;
    public:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

        virtual int GetFrames(Agent* pAgent) const;
    protected:
        IInstanceMember*		m_frames;

        friend class WaitFramesStateTask;
    };

    class WaitFramesStateTask : public StateTask {
    public:
        WaitFramesStateTask();

        virtual ~WaitFramesStateTask();
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);
    protected:
        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        int GetFrames(Agent* pAgent) const;

        int		m_start;
        int		m_frames;
    };

}
#endif//_BEHAVIAC_FSM_WAITFRAMESSTATE_H_
