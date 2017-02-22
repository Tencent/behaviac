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

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"
#include "behaviac/common/logger/logger.h"

#include "behaviac/common/socket/socketconnect_base.h"

#if BEHAVIAC_CCDEFINE_MSVC
#include <process.h>		// beginthreadex
#include <windows.h>

#pragma comment(lib, "Ws2_32.lib")

namespace {
    SOCKET AsWinSocket(behaviac::Socket::Handle h) {
        return (SOCKET)(h);
    }
}

namespace behaviac {
    namespace Socket {
        bool InitSockets() {
            WSADATA wsaData;
            int ret = WSAStartup(MAKEWORD(2, 2), &wsaData);
            return (ret == 0);
        }

        void ShutdownSockets() {
            WSACleanup();
        }

        Handle Create(bool blocking) {
            SOCKET winSocket = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

            if (winSocket == INVALID_SOCKET) {
                return Handle(0);
            }

            Handle r = Handle(winSocket);

            unsigned long inonBlocking = (blocking ? 0 : 1);

            if (ioctlsocket(winSocket, FIONBIO, &inonBlocking) == 0) {
                return r;
            }

            Close(r);

            return Handle(0);
        }

        void Close(Handle& h) {
            ::closesocket(::AsWinSocket(h));
            h = Handle(0);
        }

        bool Listen(Handle h, Port port, int maxConnections) {
            SOCKET winSocket = ::AsWinSocket(h);
            sockaddr_in addr = { 0 };
            addr.sin_addr.s_addr = INADDR_ANY;
            addr.sin_family = AF_INET;
            addr.sin_port = htons(port);
            memset(addr.sin_zero, 0, sizeof(addr.sin_zero));

            int bReuseAddr = 1;
            ::setsockopt(winSocket, SOL_SOCKET, SO_REUSEADDR, (const char*)&bReuseAddr, sizeof(bReuseAddr));

            //int rcvtimeo = 1000;
            //::setsockopt(winSocket, SOL_SOCKET, SO_RCVTIMEO, (const char*)&rcvtimeo, sizeof(rcvtimeo));

            if (::bind(winSocket, reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)) == SOCKET_ERROR) {
                Close(h);
                return false;
            }

            if (::listen(winSocket, maxConnections) == SOCKET_ERROR) {
                Close(h);
                return false;
            }

            return true;
        }

        bool TestConnection(Handle h) {
            SOCKET winSocket = ::AsWinSocket(h);
            fd_set readSet;
            FD_ZERO(&readSet);
            FD_SET(winSocket, &readSet);
            timeval timeout = { 0, 17000 };
            int res = ::select(0, &readSet, 0, 0, &timeout);

            if (res > 0) {
                if (FD_ISSET(winSocket, &readSet)) {
                    return true;
                }
            }

            return false;
        }

        Handle Accept(Handle listeningSocket, size_t bufferSize) {
            typedef int socklen_t;
            sockaddr_in addr;
            socklen_t len = sizeof(sockaddr_in);
            memset(&addr, 0, sizeof(sockaddr_in));
            SOCKET outSocket = ::accept(::AsWinSocket(listeningSocket), (sockaddr*)&addr, &len);

            if (outSocket != SOCKET_ERROR) {
                int sizeOfBufSize = sizeof(bufferSize);
                ::setsockopt(outSocket, SOL_SOCKET, SO_RCVBUF, (const char*)&bufferSize, sizeOfBufSize);
                ::setsockopt(outSocket, SOL_SOCKET, SO_SNDBUF, (const char*)&bufferSize, sizeOfBufSize);
                return Handle(outSocket);
            }

            return Handle(0);
        }

        static size_t gs_packetsSent = 0;
        static size_t gs_packetsReceived = 0;

        bool Write(Handle& h, const void* buffer, size_t bytes, size_t& outBytesWritten) {
            outBytesWritten = 0;

            if (bytes == 0 || !h) {
                return bytes == 0;
            }

            int res = ::send(::AsWinSocket(h), (const char*)buffer, (int)bytes, 0);

            if (res == SOCKET_ERROR) {
                int err = WSAGetLastError();

                if (err == WSAECONNRESET || err == WSAECONNABORTED) {
                    Close(h);
                }
            } else {
				outBytesWritten = (size_t)res;
                gs_packetsSent++;
            }

            return outBytesWritten != 0;
        }

        size_t Read(Handle& h, const void* buffer, size_t bytesMax) {
            size_t bytesRead = 0;

            if (bytesMax == 0 || !h) {
                return bytesRead;
            }

            fd_set readfds;
            FD_ZERO(&readfds);
            FD_SET(::AsWinSocket(h), &readfds);

            struct timeval tv;

            tv.tv_sec = 0;
            tv.tv_usec = 100000;//0.1s

            int rv = ::select(1, &readfds, 0, 0, &tv);

            if (rv == -1) {
                BEHAVIAC_ASSERT(0);
            } else if (rv == 0) {
                //timeout
            } else {
                int res = ::recv(::AsWinSocket(h), (char*)buffer, (int)bytesMax, 0);

                if (res == SOCKET_ERROR) {
                    int err = WSAGetLastError();

                    if (err == WSAECONNRESET || err == WSAECONNABORTED) {
                        Close(h);
                    }
                } else {
					bytesRead = (size_t)res;
                    gs_packetsReceived++;
                }

                return bytesRead;
            }

            return 0;
        }

        size_t GetPacketsSent() {
            return gs_packetsSent;
        }

        size_t GetPacketsReceived() {
            return gs_packetsReceived;
        }
    }//namespace Socket

    namespace thread {
        bool IsThreadTerminated(ThreadHandle hThread) {
            uint32_t ret = ::WaitForSingleObject(hThread, 0);

            if (ret == 0) {
                return true;
            }

            return false;
        }

        ThreadHandle CreateAndStartThread(ThreadFunction* function, void* arg, size_t stackSize) {
            const uint32_t creationFlags = 0x0;
            uintptr_t hThread = ::_beginthreadex(NULL, (unsigned)stackSize, function, arg, creationFlags, NULL);
            return (ThreadHandle)hThread;
        }

        void StopThread(ThreadHandle h) {
            uint32_t wait_duation = 1000 * 1;
            uint32_t waitRc = WaitForSingleObject(h, wait_duation);

            if (waitRc == WAIT_OBJECT_0) {
                CloseHandle(h);
            }
        }

        void Sleep(long ms) {
            ::Sleep(ms);
        }

    }//namespace thread
} // namespace behaviac
#endif//#if BEHAVIAC_CCDEFINE_MSVC
