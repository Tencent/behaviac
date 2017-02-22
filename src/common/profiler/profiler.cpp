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

#include "behaviac/common/profiler/profiler.h"
#include "behaviac/common/basictypes.h"
#include "behaviac/common/logger/logmanager.h"
#include "behaviac/agent/agent.h"

#include <cstdio>
#include <cstring>
#include <ctype.h>

#if BEHAVIAC_ENABLE_PROFILING

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>

#pragma comment(lib, "winmm.lib")
#else
#include <sys/time.h>
#endif//#if BEHAVIAC_CCDEFINE_MSVC

namespace behaviac {
    Profiler* Profiler::ms_instance = 0;

    Profiler* Profiler::GetInstance() {
        if (!ms_instance) {
            CreateInstance();
        }

        BEHAVIAC_ASSERT(ms_instance);
        return ms_instance;
    }

    Profiler* Profiler::CreateInstance() {
        if (!ms_instance) {
            Profiler* p = BEHAVIAC_NEW Profiler;
            ms_instance = p;
        }

        return ms_instance;
    }

    void Profiler::DestroyInstance() {
        BEHAVIAC_DELETE(ms_instance);
        ms_instance = 0;
    }

    static const unsigned int LINE_MAX_LENGTH = 1024;
    static const unsigned int NAME_MAX_LENGTH = 80;

    // High-resolution operating system timer used in profiling.
    class HiresTimer {
        friend class Profiler;

    public:
        // Construct. Get the starting high-resolution clock value.
        HiresTimer();

        // Return elapsed microseconds and optionally reset.
        long long GetUSec(bool reset);
        // Reset the timer.
        void Reset();

        // Return if high-resolution timer is supported.
        static bool IsSupported() {
            return supported;
        }
        // Return high-resolution timer frequency if supported.
        static long long GetFrequency() {
            return frequency;
        }

    private:
        // Starting clock value in CPU ticks.
        long long startTime_;

        // High-resolution timer support flag.
        static bool supported;
        // High-resolution timer frequency.
        static long long frequency;
    };

    // Profiling data for one block in the profiling tree.
    class ProfileData {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(ProfileData);

        // Construct with the specified parent block and name.
        ProfileData(ProfileData* pParent, const char* szName) :
            name_(szName), agent_(0),
            is_debug_block_(false),
            time_(0), maxTime_(0),
            count_(0), parent_(pParent),
            frameTime_(0), frameMaxTime_(0), frameCount_(0),
            intervalTime_(0), intervalMaxTime_(0), intervalCount_(0),
            totalTime_(0), totalMaxTime_(0), totalCount_(0) {
        }

        // Destruct. Free the child blocks.
        ~ProfileData() {
            for (behaviac::vector<ProfileData*>::iterator i = children_.begin(); i != children_.end(); ++i) {
                ProfileData* p = *i;

                BEHAVIAC_DELETE(p);
                *i = 0;
            }
        }

        // begin timing.
        void begin() {
            timer_.Reset();
            ++count_;
        }

        // end timing.
        void end() {
            long long time = timer_.GetUSec(false);

            if (time > maxTime_) {
                maxTime_ = time;
            }

            time_ = time;
        }

        void send() {
            LogManager::GetInstance()->Log(this->agent_, this->name_.c_str(), (long)this->time_);
        }

        // end profiling frame and update interval and total values.
        void EndFrame() {
            frameTime_ = time_;
            frameMaxTime_ = maxTime_;
            frameCount_ = count_;
            intervalTime_ += time_;

            if (maxTime_ > intervalMaxTime_) {
                intervalMaxTime_ = maxTime_;
            }

            intervalCount_ += count_;
            totalTime_ += time_;

            if (maxTime_ > totalMaxTime_) {
                totalMaxTime_ = maxTime_;
            }

            totalCount_ += count_;
            time_ = 0;
            maxTime_ = 0;
            count_ = 0;

            for (behaviac::vector<ProfileData*>::iterator i = children_.begin(); i != children_.end(); ++i) {
                ProfileData* p = *i;
                p->EndFrame();
            }
        }

        // begin new profiling interval.
        void BeginInterval() {
            intervalTime_ = 0;
            intervalMaxTime_ = 0;
            intervalCount_ = 0;

            for (behaviac::vector<ProfileData*>::iterator i = children_.begin(); i != children_.end(); ++i) {
                ProfileData* p = *i;

                p->BeginInterval();
            }
        }

        // Return child block with the specified name.
        ProfileData* GetChild(const char* name) {
            // First check using string pointers only, then resort to actual strcmp
            for (behaviac::vector<ProfileData*>::iterator i = children_.begin(); i != children_.end(); ++i) {
                ProfileData* p = *i;

                if (p->name_ == name) {
                    return p;
                }
            }

            ProfileData* newBlock = BEHAVIAC_NEW ProfileData(this, name);
            children_.push_back(newBlock);

            return newBlock;
        }

        // Block name.
        const behaviac::string name_;
        // High-resolution timer for measuring the block duration.
        HiresTimer timer_;

        const behaviac::Agent* agent_;

        bool is_debug_block_;

        // Time on current frame.
        long long time_;
        // Maximum time on current frame.
        long long maxTime_;
        // Calls on current frame.
        unsigned count_;
        // Parent block.
        ProfileData* parent_;
        // Child blocks.
        behaviac::vector<ProfileData*> children_;
        // Time on the previous frame.
        long long frameTime_;
        // Maximum time on the previous frame.
        long long frameMaxTime_;
        // Calls on the previous frame.
        unsigned frameCount_;
        // Time during current profiler interval.
        long long intervalTime_;
        // Maximum time during current profiler interval.
        long long intervalMaxTime_;
        // Calls during current profiler interval.
        unsigned intervalCount_;
        // Total accumulated time.
        long long totalTime_;
        // All-time maximum time.
        long long totalMaxTime_;
        // Total accumulated calls.
        unsigned totalCount_;
    private:
        ProfileData(const ProfileData& c);
        ProfileData& operator=(const ProfileData& c);
    };

    bool HiresTimer::supported(false);
    long long HiresTimer::frequency(1000LL);

    HiresTimer::HiresTimer() {
        Reset();
    }

    long long HiresTimer::GetUSec(bool reset) {
        long long currentTime;

#ifdef BEHAVIAC_CCDEFINE_MSVC

        if (supported) {
            LARGE_INTEGER counter;
            QueryPerformanceCounter(&counter);
            currentTime = counter.QuadPart;
        } else {
            currentTime = timeGetTime();
        }

#else
        struct timeval time;
        gettimeofday(&time, NULL);
        currentTime = time.tv_sec * 1000000LL + time.tv_usec;
#endif

        long long elapsedTime = currentTime - startTime_;

        // Correct for possible weirdness with changing internal frequency
        if (elapsedTime < 0) {
            elapsedTime = 0;
        }

        if (reset) {
            startTime_ = currentTime;
        }

        return (elapsedTime * 1000000LL) / frequency;
    }

    void HiresTimer::Reset() {
#ifdef BEHAVIAC_CCDEFINE_MSVC

        if (supported) {
            LARGE_INTEGER counter;
            QueryPerformanceCounter(&counter);
            startTime_ = counter.QuadPart;
        } else {
            startTime_ = timeGetTime();
        }

#else
        struct timeval time;
        gettimeofday(&time, NULL);
        startTime_ = time.tv_sec * 1000000LL + time.tv_usec;
#endif
    }

    Profiler::Profiler() : threads_(0), frameStarted_(0), outputDebugBlock_(false), m_bHierarchy(true), intervalFrames_(0), totalFrames_(0) {
#ifdef BEHAVIAC_CCDEFINE_MSVC
        LARGE_INTEGER frequency;

        if (QueryPerformanceFrequency(&frequency)) {
            HiresTimer::frequency = frequency.QuadPart;
            HiresTimer::supported = true;
        } else {
            //timeGetTime returns milliseconds
            HiresTimer::frequency = 1000LL;
        }

#else
        HiresTimer::frequency = 1000000;
        HiresTimer::supported = true;
#endif

        for (int i = 0; i < kMaxThreads; ++i) {
            current_[i].clear();
        }
    }

    Profiler::~Profiler() {
        for (int i = 0; i < this->threads_; ++i) {
            BEHAVIAC_DELETE(current_[i].root);

            current_[i].clear();
        }

        this->threads_ = 0;
    }

    const ProfileData* Profiler::GetCurrentBlock() const {
        ProfileData* current = 0;
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();

        for (int i = 0; i < this->threads_; ++i) {
            if (current_[i].threadId == threadId) {
                current = current_[i].block;
                break;
            }
        }

        return current;
    }

    // Return the root profiling block.
    const ProfileData* Profiler::GetRootBlock() const {
        ProfileData* root = 0;
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();

        for (int i = 0; i < this->threads_; ++i) {
            if (current_[i].threadId == threadId) {
                root = current_[i].root;
                break;
            }
        }

        return root;
    }

    void Profiler::BeginBlock(const char* name, const behaviac::Agent* agent, bool bDebugBlock) {
        ThreadProfilerBlock_t* pThread = 0;
        ProfileData* current = 0;
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();

        for (int i = 0; i < this->threads_; ++i) {
            if (current_[i].threadId == threadId) {
                pThread = &current_[i];

                if (this->m_bHierarchy) {
                    current = pThread->block;

                } else {
                    current = pThread->root;
                }

                break;
            }
        }

        if (!pThread) {
            current = BEHAVIAC_NEW ProfileData(0, "root");
            int found = this->threads_++;
            pThread = &current_[found];
            pThread->threadId = threadId;
            pThread->root = current;
            pThread->block = current;
        }

        BEHAVIAC_ASSERT(pThread);

        current = current->GetChild(name);
        current->agent_ = agent;
        current->is_debug_block_ = bDebugBlock;
        current->begin();

        if (!this->m_bHierarchy) {
            BEHAVIAC_ASSERT(pThread->m_currentIndex < ThreadProfilerBlock_t::kMaxBlockDepth);

            if (pThread->m_currentIndex < ThreadProfilerBlock_t::kMaxBlockDepth) {
                pThread->m_currentStack[pThread->m_currentIndex++] = pThread->block;
            }
        }

        pThread->block = current;
    }

    void Profiler::EndBlock(bool bSend) {
        ThreadProfilerBlock_t* pThread = 0;
        ProfileData* current = 0;
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();

        for (int i = 0; i < this->threads_; ++i) {
            if (current_[i].threadId == threadId) {
                pThread = &current_[i];
                current = pThread->block;
                break;
            }
        }

        BEHAVIAC_ASSERT(current && pThread);

        if (this->m_bHierarchy) {
            //not root
            if (current->parent_) {
                current->end();

                if (bSend) {
                    current->send();
                }

                current = current->parent_;

                pThread->block = current;
            }
        } else {
            current->end();

            if (bSend) {
                current->send();
            }

            BEHAVIAC_ASSERT(pThread->m_currentIndex > 0);

            if (pThread->m_currentIndex > 0) {
                current = pThread->m_currentStack[--pThread->m_currentIndex];
                pThread->block = current;
            }
        }
    }

    void Profiler::BeginFrame() {
        BEHAVIAC_ASSERT(this->frameStarted_ == 0, "EndFrame should be paired");
        this->frameStarted_ = 1;
        this->BeginBlock("RunFrame");
    }

    void Profiler::EndFrame() {
        BEHAVIAC_ASSERT(this->frameStarted_, "BeginFrame should be paired");

        ProfileData* root = 0;
        ProfileData* current = 0;
        behaviac::THREAD_ID_TYPE threadId = behaviac::GetTID();

        int found = -1;

        for (int i = 0; i < this->threads_; ++i) {
            if (current_[i].threadId == threadId) {
                current = current_[i].block;
                root = current_[i].root;
                found = i;
                break;
            }
        }

        if (current) {
            while (current->parent_) {
                EndBlock();

                current = current->parent_;
            }
        }

        ++intervalFrames_;
        ++totalFrames_;

        if (root) {
            root->EndFrame();

            BEHAVIAC_ASSERT(found != -1);
            current_[found].block = root;
        }

        this->frameStarted_ = 0;
    }

    void Profiler::BeginInterval() {
        ProfileData* root = (ProfileData*)GetRootBlock();

        if (root) {
            root->BeginInterval();
        }

        intervalFrames_ = 0;
    }

    behaviac::string Profiler::GetData(bool showUnused, bool showTotal, unsigned maxDepth) const {
        behaviac::string output;

        if (!showTotal) {
            //output += behaviac::string("Block                                                                              Cnt     Avg        Max     Frame       Total\n\n");
            output += behaviac::string("Block                                                                              Cnt     Avg        Max     Total\n\n");

        } else {
            output += behaviac::string("Block                                                                                         Last frame                       Whole execution time\n\n");
            output += behaviac::string("                                                                                   Cnt     Avg        Max        Total      Cnt      Avg         Max          Total\n\n");
        }

        if (!maxDepth) {
            maxDepth = 1;
        }

        for (int i = 0; i < this->threads_; ++i) {
            const ProfileData* root = current_[i].root;

            char temp[1024];
            string_sprintf(temp, "Thread %d:\n", current_[i].threadId);
            output += temp;
            this->GetData(root, output, 0, maxDepth, showUnused, showTotal);
        }

        return output;
    }

    void Profiler::GetData(const ProfileData* block, behaviac::string& output, unsigned depth, unsigned maxDepth, bool showUnused, bool showTotal) const {
        char line[LINE_MAX_LENGTH];
        char indentedName[LINE_MAX_LENGTH];

        uint32_t intervalFrames = behaviac::BinaryMax((uint32_t)intervalFrames_, (uint32_t)1);
        BEHAVIAC_ASSERT(intervalFrames >= 1);

        if (depth >= maxDepth) {
            return;
        }

        // Do not print the root block as it does not collect any actual data
        if (block->parent_) {
            if (block->is_debug_block_ && !this->outputDebugBlock_) {
                return;
            }

            if (showUnused || block->intervalCount_ || (showTotal && block->totalCount_)) {
                memset(indentedName, ' ', NAME_MAX_LENGTH);
                indentedName[depth] = 0;
                BEHAVIAC_ASSERT(block->name_.size() + depth < LINE_MAX_LENGTH);
                string_cat(indentedName, block->name_.c_str());
                indentedName[strlen(indentedName)] = ' ';
                indentedName[NAME_MAX_LENGTH] = 0;

                if (!showTotal) {
                    float avg = (block->intervalCount_ ? block->intervalTime_ / block->intervalCount_ : 0.0f) / 1000.0f;
                    float max = block->intervalMaxTime_ / 1000.0f;
                    float frame = block->intervalTime_ / intervalFrames / 1000.0f;
                    float all = block->intervalTime_ / 1000.0f;
                    BEHAVIAC_UNUSED_VAR(frame);

                    //string_sprintf(line, "%s %5u %10.4f %10.4f %11.4f %9.4f\n", indentedName, block->intervalCount_, avg, max, frame, all);
                    string_sprintf(line, "%s %5u %10.4f %10.4f %9.4f\n", indentedName, block->intervalCount_, avg, max, all);

                } else {
                    float avg = (block->frameCount_ ? block->frameTime_ / block->frameCount_ : 0.0f) / 1000.0f;
                    float max = block->frameMaxTime_ / 1000.0f;
                    float all = block->frameTime_ / 1000.0f;

                    float totalAvg = (block->totalCount_ ? block->totalTime_ / block->totalCount_ : 0.0f) / 1000.0f;
                    float totalMax = block->totalMaxTime_ / 1000.0f;
                    float totalAll = block->totalTime_ / 1000.0f;

                    //				Cnt, Avg, Max, Total, Cnt, Avg, Max, Total
                    string_sprintf(line, "%s %5u %10.4f %10.4f %11.4f  %7u %11.4f %11.4f %11.4f\n", indentedName, block->frameCount_, avg, max,
                                   all, block->totalCount_, totalAvg, totalMax, totalAll);
                }

                output += behaviac::string(line);
            }

            ++depth;
        }

        for (behaviac::vector<ProfileData*>::const_iterator i = block->children_.begin(); i != block->children_.end(); ++i) {
            ProfileData* p = *i;

            GetData(p, output, depth, maxDepth, showUnused, showTotal);
        }
    }
}
#else
namespace behaviac {
    Profiler::Profiler()
    {}

    Profiler::~Profiler() {
    }

    Profiler* Profiler::GetInstance() {
        return 0;
    }

    Profiler* Profiler::CreateInstance() {
        return 0;
    }
    void Profiler::DestroyInstance() {
    }

    void Profiler::BeginFrame() {
    }

    void Profiler::EndFrame() {
    }

    void Profiler::BeginInterval() {
    }

    // Return profiling data as text output.
    behaviac::string Profiler::GetData(bool showUnused, bool showTotal, unsigned maxDepth) const {
        BEHAVIAC_UNUSED_VAR(showUnused);
        BEHAVIAC_UNUSED_VAR(showTotal);
        BEHAVIAC_UNUSED_VAR(maxDepth);

        return "";
    }
}//namespace behaviac
#endif//#if BEHAVIAC_ENABLE_PROFILING

namespace behaviac {
    void Profiler::SetHierarchy(bool bHierarchy) {
        BEHAVIAC_UNUSED_VAR(bHierarchy);
#if BEHAVIAC_ENABLE_PROFILING
        m_bHierarchy = bHierarchy;
#endif
    }

    void Profiler::SetOutputDebugBlock(bool bOutput) {
        BEHAVIAC_UNUSED_VAR(bOutput);
#if BEHAVIAC_ENABLE_PROFILING
        outputDebugBlock_ = bOutput;
#endif
    }
}//namespace behaviac

namespace behaviac {
    // Construct. begin a profiling block with the specified name and optional call count.
    AutoProfileBlock::AutoProfileBlock(Profiler* profiler, const char* name, bool bDebugBlock) {
        BEHAVIAC_UNUSED_VAR(name);
        BEHAVIAC_UNUSED_VAR(bDebugBlock);

        this->setProfiler(profiler);
#if BEHAVIAC_ENABLE_PROFILING

        if (Config::IsProfiling()) {
            if (profiler_) {
                profiler_->BeginBlock(name, 0, bDebugBlock);
            }
        }

#endif
    }

    // Construct. begin a profiling block with the specified name and optional call count.
    AutoProfileBlock::AutoProfileBlock(Profiler* profiler, const behaviac::string& name) {
        BEHAVIAC_UNUSED_VAR(name);
        this->setProfiler(profiler);
#if BEHAVIAC_ENABLE_PROFILING

        if (Config::IsProfiling()) {
            if (profiler_) {
                profiler_->BeginBlock(name.c_str());
            }
        }

#endif
    }

    // Destruct. end the profiling block.
    AutoProfileBlock::~AutoProfileBlock() {
#if BEHAVIAC_ENABLE_PROFILING

        if (Config::IsProfiling()) {
            if (profiler_) {
                profiler_->EndBlock();
            }
        }

#endif
    }
}
