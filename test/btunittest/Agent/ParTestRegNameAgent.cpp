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

#include "ParTestRegNameAgent.h"

kEmployee ParTestRegNameAgent::STV_KEMPLOYEE_0;
behaviac::vector<signed char> ParTestRegNameAgent::STV_LIST_SBYTE_0;
behaviac::vector<kEmployee> ParTestRegNameAgent::STV_LIST_KEMPLOYEE_0;

int StaticAgent::sInt = 1;

ParTestRegNameAgent::ParTestRegNameAgent()
{
    TV_CHAR_0 = '\0';
    TV_BYTE_0 = 0;
    TV_SBYTE_0 = 0;
    TV_STR_0 = "";
    TV_AGENT_0 = NULL;
}

ParTestRegNameAgent::~ParTestRegNameAgent()
{
}

void ParTestRegNameAgent::resetProperties()
{
    TV_CHAR_0 = '\0';
    TV_BYTE_0 = 0;
    TV_SBYTE_0 = 0;
    TV_STR_0 = "";
    TV_AGENT_0 = NULL;
    TV_KEMPLOYEE_0.resetProperties();
    STV_KEMPLOYEE_0.resetProperties();

    TV_LIST_KEMPLOYEE_0.clear();
    STV_LIST_SBYTE_0.clear();
    STV_LIST_KEMPLOYEE_0.clear();
}

