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

namespace behaviac
{
    internal class RandomGenerator
    {
        private static RandomGenerator Instance = null;

        public static RandomGenerator GetInstance()
        {
            if (Instance == null)
            {
                Instance = new RandomGenerator(0);
            }

            return RandomGenerator.Instance;
        }

        //[0, 1)
        public float GetRandom()
        {
            m_seed = 214013 * m_seed + 2531011;
            float r = (m_seed * (1.0f / 4294967296.0f));

            Debug.Check(r >= 0.0f && r < 1.0f);
            return r;
        }

        //[low, high)
        public float InRange(float low, float high)
        {
            float r = this.GetRandom();
            float ret = r * (high - low) + low;
            return ret;
        }

        public void SetSeed(uint seed)
        {
            this.m_seed = seed;
        }

        protected RandomGenerator(uint seed)
        {
            m_seed = seed;
        }

        //~RandomGenerator()
        //{ }

        private uint m_seed;
    };
}
