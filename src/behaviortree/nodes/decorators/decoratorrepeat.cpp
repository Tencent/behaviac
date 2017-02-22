#include "behaviac/common/base.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorrepeat.h"
namespace behaviac {
    void DecoratorRepeat::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }
    int DecoratorRepeat::Count(Agent* pAgent) {
        return super::GetCount(pAgent);
    }
    bool DecoratorRepeat::IsValid(Agent* pAgent, BehaviorTask* pTask) const {

        if (!DecoratorRepeat::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    BehaviorTask* DecoratorRepeat::createTask() const {
        DecoratorRepeatTask* pTask = BEHAVIAC_NEW DecoratorRepeatTask();

        return pTask;
    }
    EBTStatus DecoratorRepeatTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_ASSERT(DecoratorNode::DynamicCast(this->m_node));
        DecoratorNode* node = (DecoratorNode*)this->m_node;

        BEHAVIAC_ASSERT(this->m_n >= 0);
        BEHAVIAC_ASSERT(this->m_root != NULL);

        EBTStatus status = BT_INVALID;

        for (int i = 0; i < this->m_n; ++i) {
            status = this->m_root->exec(pAgent, childStatus);

            if (node->m_bDecorateWhenChildEnds) {
                while (status == BT_RUNNING) {
                    status = super::update(pAgent, childStatus);
                }
            }

            if (status == BT_FAILURE) {
                return BT_FAILURE;
            }
        }

        return BT_SUCCESS;
    }
}
