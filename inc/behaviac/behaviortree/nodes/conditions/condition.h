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

#ifndef _BEHAVIAC_BEHAVIORTREE_CONDITION_H_
#define _BEHAVIAC_BEHAVIORTREE_CONDITION_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/property/property.h"

#include "behaviac/common/member.h"

#include "behaviac/behaviortree/nodes/conditions/conditionbase.h"

namespace behaviac {
    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup Condition
    * @{ */

    /**
    Condition node can compare the value of right and left. return Failure or Success
    */

    class BEHAVIAC_API Condition : public ConditionBase {
    private:
        template<typename T>
        static bool Register(const char* typeName) {
            return true;
        }

        template<typename T>
        static void UnRegister(const char* typeName) {
        }
    public:
        static void Cleanup();

    private:
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Condition, ConditionBase);

        Condition();
        virtual ~Condition();
        virtual void load(int version, const char* agentType, const properties_t& properties);
        virtual bool Evaluate(Agent* pAgent);

    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        IInstanceMember*			m_opl;
        IInstanceMember*			m_opr;
        EOperatorType               m_operator;
        friend class ConditionTask;
    };

    class BEHAVIAC_API ConditionTask : public ConditionBaseTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(ConditionTask, ConditionBaseTask);

        ConditionTask() : ConditionBaseTask() {
        }

        virtual ~ConditionTask() {
        }

    protected:
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
    };
    /*! @} */
    /*! @} */
}

#endif//_BEHAVIAC_BEHAVIORTREE_CONDITION_H_
