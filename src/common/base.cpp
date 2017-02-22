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
#include "behaviac/common/workspace.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/behaviortree/nodes/actions/compute.h"
#include "behaviac/htn/plannertask.h"
#include "behaviac/agent/agent.h"
#include "behaviac/common/file/filemanager.h"
#include "behaviac/common/file/file.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    namespace Socket {
        /**
        @param bBlocking
        if true, block the execution and wait for the connection from the designer
        if false, wait for the connection from the designer but doesn't block the game
        */
        bool SetupConnection(bool bBlocking, unsigned short port);
        void ShutdownConnection();
    }

    void CleanupTickingMutex();

    static int ms_nStarted = 0;

    bool IsStarted() {
        return ms_nStarted >= 1;
    }

    const char* GetVersionString() {
        //return BEHAVIAC_VERSION_STRING;
        return BEHAVIAC_BUILD_CONFIG_STR;
    }

    bool BaseStart() {
        //BEHAVIAC_ASSERT(ms_nStarted == 0, "behaviac::Stop was not invoked! or behaviac::Start had been invoked already!");
        if (ms_nStarted == 0) {
            ms_nStarted++;

            behaviac::SetMainThread();

            behaviac::Workspace::GetInstance()->RegisterBasicNodes();

            AgentMeta::Register();

            bool bSocketing = Config::IsSocketing();

            if (bSocketing) {
                bool bBlock = Config::IsSocketBlocking();
                unsigned short port = Config::GetSocketPort();

                Socket::SetupConnection(bBlock, port);
            }
        }

        return true;
    }

    void BaseStop() {
        //BEHAVIAC_ASSERT(ms_nStarted == 1, "behaviac::Start was not invoked! or behaviac::Stop had been invoked already!");
        if (ms_nStarted == 1) {
            ms_nStarted--;
            BEHAVIAC_ASSERT(ms_nStarted == 0);

            bool bSocketing = Config::IsSocketing();

            if (bSocketing) {
                Socket::ShutdownConnection();
            }

            BehaviorNode::Cleanup();

            CleanupTickingMutex();

            //Agent::Cleanup();
            //Variables::Cleanup();
            //Property::Cleanup();
            Condition::Cleanup();
            //Compute::Cleanup();
            CStringCRC::Cleanup();
            LogManager::Cleanup();

            CFileManager::Cleanup();

            //ComparerRegister::Cleanup();
            ComputerRegister::Cleanup();

            Context::Cleanup();

            //AgentProperties::Cleanup();
            AgentMeta::UnRegister();
#if BEHAVIAC_USE_HTN
            PlannerTask::Cleanup();
#endif
        }
    }

    bool TryStart() {
        if (!IsStarted()) {
            bool bOk = BaseStart();

            return bOk;
        }

        return true;
    }

    static behaviac::THREAD_ID_TYPE gs_mainTheadId;

    void SetMainThread() {
        gs_mainTheadId = behaviac::GetTID();
    }

    bool IsMainThread() {
        behaviac::THREAD_ID_TYPE currentThreadId = behaviac::GetTID();
        return currentThreadId == gs_mainTheadId;
    }

    static BreakpointPromptHandler_fn gs_BreakpointPromptHandler = 0;
    void SetBreakpointPromptHandler(BreakpointPromptHandler_fn fn) {
        gs_BreakpointPromptHandler = fn;
    }

    BreakpointPromptHandler_fn GetBreakpointPromptHandler() {
        return gs_BreakpointPromptHandler;
    }
}//namespace behaviac

