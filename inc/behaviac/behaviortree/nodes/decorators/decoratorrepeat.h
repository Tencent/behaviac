#ifndef _BEHAVIAC_BEHAVIORTREE_DECORATORREPEAT_H_
#define _BEHAVIAC_BEHAVIORTREE_DECORATORREPEAT_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorcount.h"

namespace behaviac {
    class BEHAVIAC_API DecoratorRepeat : public DecoratorCount {
    public :
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorRepeat, DecoratorCount);
        DecoratorRepeat() {
        }

        virtual ~DecoratorRepeat() {
        }

    protected :
        virtual void load(int version, const char* agentType, const properties_t& properties);

    public :
        int Count(Agent* pAgent);

        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    protected:
        virtual BehaviorTask* createTask() const;

        ///Returns EBTStatus.BT_FAILURE for the specified number of iterations, then returns EBTStatus.BT_SUCCESS after that
    };
    class BEHAVIAC_API DecoratorRepeatTask : public  DecoratorCountTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorRepeatTask, DecoratorCountTask);
        DecoratorRepeatTask() {
        }

        ~DecoratorRepeatTask() {
        }

    public:
        virtual void copyto(BehaviorTask* target) const {
            super::copyto(target);
        }

        virtual void save(IIONode* node) const {
            super::save(node);
        }

        virtual void load(IIONode* node) {
            super::load(node);
        }

    protected:
        virtual EBTStatus decorate(EBTStatus status) {
            BEHAVIAC_UNUSED_VAR(status);
            BEHAVIAC_ASSERT(false, "unsurpported");

            return BT_INVALID;
        }

    protected:
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };

}
#endif