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

#ifndef _BEHAVIAC_BEHAVIORTREE_COMPUTE_H_
#define _BEHAVIAC_BEHAVIORTREE_COMPUTE_H_

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/agent/agent.h"


namespace behaviac {

    /*! \addtogroup treeNodes Behavior Tree
    * @{
    * \addtogroup Compute
    * @{ */

    ///Compute
    /**
    Compute the result of Operand1 and Operand2 and assign it to the Left Operand.
    Compute node can perform Add, Sub, Mul and Div operations. a left and right Operand
    can be a agent property or a par value.
    */
    class BEHAVIAC_API Compute : public BehaviorNode {
    public:
        static void RegisterBasicTypes();
        static void UnRegisterBasicTypes();

        static void Cleanup();
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(Compute, BehaviorNode);

        Compute();
        virtual ~Compute();
        virtual void load(int version, const char* agentType, const properties_t& properties);
        //static bool EvaluteCompute(Agent* pAgent, const behaviac::string& typeName, IInstanceMember* opl, IInstanceMember* opr1, behaviac::CMethodBase* opr1_m, EComputeOperator computeOperator, IInstanceMember* opr2, behaviac::CMethodBase* opr2_m);
    protected:
        virtual bool IsValid(Agent* pAgent, BehaviorTask* pTask) const;

    private:
        virtual BehaviorTask* createTask() const;

    protected:
        IInstanceMember*			m_opl;
        IInstanceMember*			m_opr1;
        IInstanceMember*			m_opr2;

        EOperatorType	            m_operator;

        behaviac::string	m_typeName;

        friend class ComputeTask;
    };

    class BEHAVIAC_API ComputeTask : public LeafTask {
    public:
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(ComputeTask, LeafTask);

        ComputeTask();
        virtual ~ComputeTask();

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

#endif//_BEHAVIAC_BEHAVIORTREE_COMPUTE_H_
