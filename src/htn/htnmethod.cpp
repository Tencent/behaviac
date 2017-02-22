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

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"

#include "behaviac/htn/method.h"

#include "behaviac/htn/planner.h"
#include "behaviac/htn/plannertask.h"

namespace behaviac {
    Method::Method() {

    }

    Method::~Method() {

    }

    void Method::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);
    }

#if BEHAVIAC_USE_HTN

    bool Method::decompose(BehaviorNode* node, PlannerTaskComplex* seqTask, int depth, Planner* planner) {
        bool bOk = false;
        Method* branch = (Method*)node;
        int childCount = branch->GetChildrenCount();
        BEHAVIAC_UNUSED_VAR(childCount);
        BEHAVIAC_ASSERT(childCount == 1);
        BehaviorNode* childNode = (BehaviorNode*)branch->GetChild(0);
        PlannerTask* childTask = planner->decomposeNode(childNode, depth);

        if (childTask != NULL) {
            seqTask->AddChild(childTask);
            bOk = true;
        }

        return bOk;
    }
#endif //BEHAVIAC_USE_HTN

    bool Method::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (Method::DynamicCast(pTask->GetNode()) == 0) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }
    BehaviorTask* Method::createTask() const {
        BEHAVIAC_ASSERT(false);
        return 0;
    }
}
