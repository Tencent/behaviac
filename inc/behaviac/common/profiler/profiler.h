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

#ifndef _BEHAVIAC_COMMON_PROFILER_H_
#define _BEHAVIAC_COMMON_PROFILER_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"
#include "behaviac/common/thread/thread.h"
#include "behaviac/common/container/string.h"
#include "behaviac/common/container/vector.h"
#include "behaviac/common/container/map.h"
#include "behaviac/common/string/stringutils.h"

#include "behaviac/common/workspace.h"

namespace behaviac {
    class Agent;
    class ProfileData;

    class BEHAVIAC_API Profiler {
    private:
        static Profiler* ms_instance;
    public:
        static Profiler* GetInstance();
        static Profiler* CreateInstance();
        static void DestroyInstance();

    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(Profiler);
        Profiler();
        virtual ~Profiler();

        void SetOutputDebugBlock(bool bOutput);

        void BeginBlock(const char* name, const behaviac::Agent* agent = 0, bool bDebugBlock = false);
        void EndBlock(bool bSend = false);

        void BeginFrame();
        void EndFrame();
        void BeginInterval();
        void SetHierarchy(bool bHierarchy);

        behaviac::string GetData(bool showUnused = false, bool showTotal = false, unsigned maxDepth = 10000) const;

        const ProfileData* GetCurrentBlock() const;
        const ProfileData* GetRootBlock() const;

    private:
        void GetData(const ProfileData* block, behaviac::string& output, unsigned depth, unsigned maxDepth, bool showUnused, bool showTotal) const;

        struct ThreadProfilerBlock_t {
            behaviac::THREAD_ID_TYPE		threadId;
            ProfileData*					block;
            ProfileData*					root;

            const static int kMaxBlockDepth = 100;
            ProfileData*					m_currentStack[kMaxBlockDepth];
            int								m_currentIndex;

            ThreadProfilerBlock_t() : threadId(0), block(0), root(0), m_currentIndex(0) {
            }

            void clear() {
                threadId = 0;
                block = 0;
                root = 0;
                m_currentIndex = 0;
            }
        };

        const static int kMaxThreads = 32;

        ThreadProfilerBlock_t				current_[kMaxThreads];
        int									threads_;

        int									frameStarted_;

        bool								outputDebugBlock_;
        bool								m_bHierarchy;

        unsigned intervalFrames_;
        unsigned totalFrames_;
    };

    class BEHAVIAC_API AutoProfileBlock {
    public:
        void setProfiler(Profiler* profiler) {
            this->profiler_ = profiler;
        }

        AutoProfileBlock(Profiler* profiler, const char* name, bool bDebugBlock);

        AutoProfileBlock(Profiler* profiler, const behaviac::string& name);

        ~AutoProfileBlock();

    private:
        Profiler* profiler_;
    };
}

//#define BEHAVIAC_PROFILE_V2(name, bDebugBlock) behaviac::AutoProfileBlock BEHAVIAC_UNIQUE_NAME(_profile) (behaviac::Profiler::GetInstance(), name, bDebugBlock)
//#define BEHAVIAC_PROFILE_V1(name) BEHAVIAC_PROFILE_V2(name, false)
//
//#define BEHAVIAC_PROFILE(...) ARGUMENT_SELECTOR3((__VA_ARGS__, BEHAVIAC_PROFILE_V2, BEHAVIAC_PROFILE_V1))(__VA_ARGS__)
//#define ARGUMENT_SELECTOR3(__args) GET_3TH_ARGUMENT __args
//#define GET_3TH_ARGUMENT(__p1,__p2,__n, ...) __n

#define BEHAVIAC_PROFILE_DEBUGBLOCK(name, bDebugBlock) behaviac::AutoProfileBlock BEHAVIAC_UNIQUE_NAME(_profile) (behaviac::Profiler::GetInstance(), name, bDebugBlock)

#define BEHAVIAC_PROFILE(name) BEHAVIAC_PROFILE_DEBUGBLOCK(name, false)

#endif//_BEHAVIAC_COMMON_PROFILER_H_
