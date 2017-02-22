#ifndef _BEHAVIAC_FSM_H_
#define _BEHAVIAC_FSM_H_
#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"

namespace behaviac {
    // ============================================================================
    class BEHAVIAC_API FSM : public BehaviorNode {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(FSM, BehaviorNode);

    public:
        FSM();
        virtual ~FSM();

        void SetInitialId(int initialId);
        int GetInitialId() const;
#if BEHAVIAC_USE_HTN
        virtual bool decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner);
#endif //BEHAVIAC_USE_HTN
    protected:
        virtual void load(int version, const char* agentType, const properties_t& properties);
        virtual BehaviorTask* createTask() const;
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;
    private:
        int		m_initialId;
    };

    class BEHAVIAC_API FSMTask : public CompositeTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(FSMTask, CompositeTask);

        FSMTask();
        virtual ~FSMTask();

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);

        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        EBTStatus UpdateFSM(Agent* pAgent, EBTStatus childStatus);
    };

}
#endif
