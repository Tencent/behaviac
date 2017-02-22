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

#include "EmployeeParTestAgent.h"

float EmployeeParTestAgent::STV_F_0 = 0.0f;
behaviac::string EmployeeParTestAgent::STV_STR_0 = "";
behaviac::Agent* EmployeeParTestAgent::STV_AGENT_0 = NULL;
behaviac::vector<float> EmployeeParTestAgent::STV_LIST_F_0;
behaviac::vector<behaviac::string> EmployeeParTestAgent::STV_LIST_STR_0;
behaviac::vector<behaviac::Agent*> EmployeeParTestAgent::STV_LIST_AGENT_0;

EmployeeParTestAgent::EmployeeParTestAgent()
{
    TV_UINT_0 = 0;
    TV_ULONG_0 = 0L;
    TV_LL_0 = 0L;
    TV_ULL_0 = 0L;
    TV_F_0 = 0.0f;
    TV_D_0 = 0.0;
    TV_STR_0 = "";
    TV_AGENT_0 = NULL;
	TV_CSZSTR_0 = 0;
	TV_SZSTR_0 = 0;
}

EmployeeParTestAgent::~EmployeeParTestAgent()
{
}

void EmployeeParTestAgent::resetProperties()
{
    super::resetProperties();

    TV_UINT_0 = 0;
    TV_ULONG_0 = 0L;
    TV_F_0 = 0.0f;
    STV_F_0 = 0.0f;
    TV_D_0 = 0.0;
    TV_LL_0 = 0L;
    TV_ULL_0 = 0L;
    TV_STR_0 = "";
    TV_SZSTR_0 = NULL;
    TV_CSZSTR_0 = "TV_CSZSTR_0";
    STV_STR_0 = "";
    TV_AGENT_0 = NULL;
    STV_AGENT_0 = NULL;

    TV_LIST_F_0.clear();
    STV_LIST_F_0.clear();
    TV_LIST_STR_0.clear();
    STV_LIST_STR_0.clear();
    TV_LIST_AGENT_0.clear();
    STV_LIST_AGENT_0.clear();
}

