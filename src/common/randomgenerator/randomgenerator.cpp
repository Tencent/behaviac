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

#include "behaviac/common/randomgenerator/randomgenerator.h"

namespace behaviac {
    RandomGenerator* RandomGenerator::ms_pInstance;

    void RandomGenerator::_SetInstance(RandomGenerator* pInstance) {
        RandomGenerator::ms_pInstance = pInstance;
    }

    RandomGenerator* RandomGenerator::_GetInstance() {
        RandomGenerator* pInstance = RandomGenerator::ms_pInstance;

        if (!pInstance) {
            pInstance = BEHAVIAC_NEW RandomGenerator;
        }

        return pInstance;
    }
}//namespace behaviac
