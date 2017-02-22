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

#ifndef _BTPERFORMANCE_H_
#define _BTPERFORMANCE_H_

#include "behaviac/common/base.h"
#include "behaviac/common/profiler/profiler.h"
#include "behaviac/agent/agent.h"
#include "BehaviacWorkspace.h"

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#endif

class CPerformanceAgent : public behaviac::Agent
{
public:
	BEHAVIAC_DECLARE_AGENTTYPE(CPerformanceAgent, behaviac::Agent);

    CPerformanceAgent();
    virtual ~CPerformanceAgent()
    {}

    void Clear();
public:
    float DistanceToEnemy;
    float HP;

    float Hungry;
    float Food;

    float m_internal;

    behaviac::EBTStatus RunAway();
    void Fire();

    behaviac::EBTStatus SearchForFood();
    behaviac::EBTStatus Eat();

    behaviac::EBTStatus Wander();
    behaviac::EBTStatus Fidget();
};

#endif//_BTPERFORMANCE_H_