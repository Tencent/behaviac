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
#include "behaviac/common/socket/socketconnect.h"
#include "behaviac/common/socket/socketconnect_base.h"
#include "behaviac/common/file/filesystem.h"

#include "behaviac/common/logger/logger.h"
#include "behaviac/common/container/spscqueue.h"
#include "behaviac/common/container/hash_exmemory.h"
#include "behaviac/common/thread/mutex_lock.h"

#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/common/logger/logmanager.h"
#include "behaviac/agent/context.h"
#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#endif//BEHAVIAC_CCDEFINE_MSVC

namespace behaviac {
    uint32_t Packet::CalcPacketSize() const {
        size_t packetSize(0);

        if (command == CommandId::CMDID_TEXT) {
            //packetSize = sizeof(Text);
            packetSize = strlen((char*)this->data);
            BEHAVIAC_ASSERT(packetSize <= kMaxTextLength);
        } else {
            BEHAVIAC_ASSERT(false, "Unknown command");
        }

        packetSize += sizeof(command);
        return (uint32_t)packetSize;
    }
}

namespace behaviac {
    namespace Socket {
        void SendWorkspaceSettings();
    }

#pragma pack(push, 1)
    struct InitialSettingsPacket {
        InitialSettingsPacket()
            : messageSize(0),
              command(behaviac::CommandId::CMDID_INITIAL_SETTINGS),
              platform(Platform::WINDOWS) {
#if BEHAVIAC_CCDEFINE_MSVC
            HANDLE processHandle = GetCurrentProcess();
            this->processId = GetProcessId(processHandle);
#else
            this->processId = 0;
#endif
        }

        size_t PrepareToSend() {
            messageSize = sizeof(InitialSettingsPacket) - 1;
            return messageSize + 1;
        }

        uint8_t	messageSize;
        uint8_t	command;
        uint8_t	platform;
        uint32_t processId;
    };
#pragma pack(pop)

    class ConnectorImpl : public ConnectorInterface {
    public:
        ConnectorImpl();
        virtual ~ConnectorImpl();

        virtual void OnConnection();

        bool IsWorkspaceSent() const {
            return m_workspaceSent;
        }

        void SetWorkspaceSent(bool bSent) {
            m_workspaceSent = bSent;
        }
    private:
        volatile bool				m_workspaceSent;

        void SendInitialSettings();
        void SendInitialProperties();
        virtual void Clear() {
            ConnectorInterface::Clear();

            m_workspaceSent = false;
        }
    };

    ConnectorImpl					s_tracer;

    ConnectorImpl::ConnectorImpl() : m_workspaceSent(false) {
        //don't handle message automatically
        m_bHandleMessage = false;
    }

    ConnectorImpl::~ConnectorImpl() {
    }

    void ConnectorImpl::SendInitialSettings() {
        InitialSettingsPacket initialPacket;
        const size_t bytesToSend = initialPacket.PrepareToSend();
        size_t bytesWritten(0);

        if (!behaviac::Socket::Write(m_writeSocket, &initialPacket, bytesToSend, bytesWritten) ||
            bytesWritten != bytesToSend) {
            Log("behaviac: Couldn't send initial settings.\n");
        }

        gs_packetsStats.init++;
    }

    void ConnectorImpl::OnConnection() {
        Log("behaviac: sending initial settings.\n");

        this->SendInitialSettings();

        Socket::SendWorkspaceSettings();

        this->SendInitialProperties();

        {
            ScopedInt_t scopedInt(&gs_threadFlag);
            Log("behaviac: sending packets before connecting.\n");

            this->SendExistingPackets();
        }

        //Log("[connected]precached message done\n");
        behaviac::Socket::SendText("[connected]precached message done");

        //when '[connected]' is handled in the designer, it will send back all the breakpoints if any and '[breakcpp]' and '[start]'
        //here we block until all those messages have been received, otherwise, if we don't block here to wait for all those messages
        //the breakpoints checking might be wrong.
        bool bLoop = true;

        while (bLoop && !m_isDisconnected && this->m_writeSocket) {
            //sending packets if any
            if (m_packetsCount > 0) {
                SendAllPackets();
            }

            const char* kStartMsg = "[start]";
            bool bFound = this->ReceivePackets(kStartMsg);

            if (bFound) {
                bLoop = false;
            } else {
                behaviac::thread::Sleep(1);
            }
        }

        Log("behaviac: OnConnection done.\n");
        //this->m_bHandleMessage = false;
    }

    void ConnectorImpl::SendInitialProperties() {
        Workspace::GetInstance()->LogCurrentStates();
    }
}

namespace behaviac {
    namespace Socket {
        bool SetupConnection(bool bBlocking, unsigned short port) {
            if (Config::IsSocketing()) {
                if (!s_tracer.IsInited()) {
                    const int		kMaxThreads = 128;

                    if (!s_tracer.Init(kMaxThreads, port, bBlocking)) {
                        return false;
                    }
                }

                BEHAVIAC_LOGINFO("behaviac: SetupConnection successful\n");

                return true;
            }

            return false;
        }

        bool IsConnected() {
            if (Config::IsSocketing()) {
                return s_tracer.IsConnected();
            }

            return false;
        }

        void ShutdownConnection() {
            if (Config::IsSocketing()) {
                s_tracer.Close();

                BEHAVIAC_LOGINFO("behaviac: ShutdownConnection\n");
            }
        }

        void SendText(const char* text) {
            if (Config::IsSocketing()) {
                s_tracer.SendText(text);
            }
        }

        void SendWorkspace(const char* text) {
            if (Config::IsSocketing()) {
                s_tracer.SendText(text, CommandId::CMDID_TEXT);
            }
        }

        bool ReadText(behaviac::string& text) {
            if (Config::IsSocketing()) {
                return s_tracer.ReadText(text);
            }

            return false;
        }

        void Flush() {
            if (Config::IsSocketing()) {
                while (s_tracer.GetPacketsCount()) {
                    behaviac::thread::Sleep(1);
                }
            }
        }

        void SendWorkspaceSettings() {
            if (Config::IsSocketing()) {
                if (!s_tracer.IsWorkspaceSent() && s_tracer.IsConnected()) {
#if BEHAVIAC_CCDEFINE_MSVC
                    const char* platform = "Windows";
#elif BEHAVIAC_CCDEFINE_APPLE
                    const char* platform = "iOS";
#elif BEHAVIAC_CCDEFINE_ANDROID
                    const char* platform = "Android";
#elif BEHAVIAC_CCDEFINE_GCC_LINUX
                    const char* platform = "Linux";
#elif BEHAVIAC_CCDEFINE_GCC_CYGWIN
                    const char* platform = "Cygwin";
#else
                    const char* platform = "Unknown Platform";
#endif

                    char msg[1024];
                    string_sprintf(msg, "[platform] %s\n", platform);
                    LogManager::GetInstance()->LogWorkspace(true, msg);

                    Workspace::EFileFormat format = Workspace::GetInstance()->GetFileFormat();
					const char* formatString = (format == Workspace::EFF_bson ? "bson.bytes" : (format == Workspace::EFF_cpp ? "cpp" : "xml"));

                    string_sprintf(msg, "[workspace] %s \"%s\"\n", formatString, "");
                    LogManager::GetInstance()->LogWorkspace(true, msg);

                    s_tracer.SetWorkspaceSent(true);
                }
            }
        }

        size_t GetMemoryOverhead() {
            if (Config::IsSocketing()) {
                return s_tracer.GetMemoryOverhead();
            }

            return 0;
        }

        size_t GetNumTrackedThreads() {
            if (Config::IsSocketing()) {
                return s_tracer.GetNumTrackedThreads();
            }

            return false;
        }

        void UpdatePacketsStats() {
            if (Config::IsSocketing()) {
                //size_t overhead = (behaviac::GetMemoryOverhead());
                //BEHAVIAC_SETTRACEDVAR("Stats::Vars", gs_packetsStats.vars);
            }
        }
    }
} // behaviac
