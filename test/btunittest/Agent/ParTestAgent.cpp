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

#include "ParTestAgent.h"

signed int ParTestAgent::STV_INT_0 = 0;
kEmployee ParTestAgent::STV_KEMPLOYEE_0;
behaviac::vector<signed int> ParTestAgent::STV_LIST_INT_0;
behaviac::vector<kEmployee> ParTestAgent::STV_LIST_KEMPLOYEE_0;

ParTestAgent::ParTestAgent()
{
    TV_KEMPLOYEE_0.resetProperties();

    TV_SHORT_0 = 0;
    TV_INT_0 = 0;
    TV_LONG_0 = 0L;
    TV_USHORT_0 = 0;
}

ParTestAgent::~ParTestAgent()
{
}

void ParTestAgent::resetProperties()
{
    super::resetProperties();

    TV_SHORT_0 = 0;
    TV_INT_0 = 0;
    STV_INT_0 = 0;
    TV_LONG_0 = 0L;
    TV_USHORT_0 = 0;

    TV_KEMPLOYEE_0.resetProperties();
    STV_KEMPLOYEE_0.resetProperties();

    TV_LIST_INT_0.clear();
    STV_LIST_INT_0.clear();
    TV_LIST_KEMPLOYEE_0.clear();
    STV_LIST_KEMPLOYEE_0.clear();
}
