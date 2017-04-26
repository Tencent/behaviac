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

#ifndef _BEHAVIAC_COMMON_RANDOMGENERATOR_H_
#define _BEHAVIAC_COMMON_RANDOMGENERATOR_H_
#include "behaviac/common/base.h"

//#define _SYS_RANDOM_	1

#if _SYS_RANDOM_
#include <stdlib.h>
#endif//#if _SYS_RANDOM_

namespace behaviac {
	class BEHAVIAC_API RandomGenerator {
    public:
        static RandomGenerator* GetInstance() {
            RandomGenerator* pRandomGenerator = RandomGenerator::_GetInstance();

            return pRandomGenerator;
        }

        //[0, 1)
        double operator()() {
            return this->random();
        }

        //[low, high)
        template <typename T>
        double InRange(T low, T high) {
            double r = (*this)();
            double ret = r * (high - low) + low;
            return ret;
        }

        void setSeed(unsigned int seed) {
            this->m_seed = seed;
#if _SYS_RANDOM_
            srand(seed);
#endif//#if _SYS_RANDOM_
        }
    protected:
        RandomGenerator(unsigned int seed = 0) : m_seed(seed) {
            RandomGenerator::_SetInstance(this);
        }

        virtual ~RandomGenerator()
        {}

    private:
        static RandomGenerator* ms_pInstance;
        static void _SetInstance(RandomGenerator* pInstance);
        static RandomGenerator* _GetInstance();

        //[0, 1)
        virtual double random() {
#if _SYS_RANDOM_
            int v = rand();
            double r = v / (double)RAND_MAX;
#else
            m_seed = 214013 * m_seed + 2531011;
            double r = (m_seed * (1.0 / 4294967296.0));
#endif//_SYS_RANDOM_

            BEHAVIAC_ASSERT(r >= 0.0 && r < 1.0);
            return r;
        }

        unsigned int m_seed;
    };
}//namespace behaviac

#endif//_BEHAVIAC_COMMON_RANDOMGENERATOR_H_
