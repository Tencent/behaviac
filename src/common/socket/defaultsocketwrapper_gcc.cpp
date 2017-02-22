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

#if !BEHAVIAC_CCDEFINE_MSVC
#include <pthread.h>		// beginthreadex
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include <fcntl.h>
#include <netdb.h>
#include <netinet/in.h>

typedef int SOCKET;

namespace {
    SOCKET AsSocket(behaviac::Socket::Handle h) {
#if BEHAVIAC_CCDEFINE_64BITS
        return (size_t)(h);
#else
        return (SOCKET)(h);
#endif
    }
}

namespace behaviac {
    namespace Socket {
        bool InitSockets() {
            return true;
        }

        void ShutdownSockets() {
        }

        Handle Create(bool blocking) {
            SOCKET s = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

            if (s < 0) {
                return Handle(0);
            }

#ifdef SO_NONBLOCK
#ifdef SO_NOSIGPIPE
            int noSigpipe = 1;
            setsockopt(s, SOL_SOCKET, SO_NOSIGPIPE, &noSigpipe, sizeof(noSigpipe));
#endif
            int inonBlocking = (blocking ? 0 : 1);
            return setsockopt(s, SOL_SOCKET, SO_NONBLOCK, &inonBlocking, sizeof(inonBlocking)) == 0 ? Handle(s) : Handle(0));
#else
            int v = fcntl(s, F_GETFL, 0);
            int cntl = !blocking ? (v | O_NONBLOCK) : (v & ~O_NONBLOCK);

#if BEHAVIAC_CCDEFINE_APPLE
            cntl |= SO_NOSIGPIPE;
#endif

            return fcntl(s, F_SETFL, cntl) != -1 ? Handle(s) : Handle(0);
#endif
        }

        void Close(Handle& h) {
            close(AsSocket(h));
            h = Handle(0);
        }

        bool Listen(Handle h, Port port, int maxConnections) {
            SOCKET winSocket = ::AsSocket(h);
            sockaddr_in addr;

            memset(&addr, 0, sizeof(addr));
            addr.sin_addr.s_addr = htonl(INADDR_ANY);
            addr.sin_family = AF_INET;
            addr.sin_port = htons(port);
            memset(addr.sin_zero, 0, sizeof(addr.sin_zero));

            int bReuseAddr = 1;
            ::setsockopt(winSocket, SOL_SOCKET, SO_REUSEADDR, (const char*)&bReuseAddr, sizeof(bReuseAddr));

            //int rcvtimeo = 1000;
            //::setsockopt(winSocket, SOL_SOCKET, SO_RCVTIMEO, (const char*)&rcvtimeo, sizeof(rcvtimeo));

            if (bind(winSocket, reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)) < 0) {
                Close(h);
                BEHAVIAC_LOGERROR("Listen failed at bind\n");
                return false;
            }

            if (listen(winSocket, maxConnections) < 0) {
                Close(h);
                BEHAVIAC_LOGERROR("Listen failed at listen\n");
                return false;
            }

            return true;
        }

        bool TestConnection(Handle h) {
            SOCKET winSocket = ::AsSocket(h);
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

            //printf("TestConnection %d\n", res);

            return false;
        }

        Handle Accept(Handle listeningSocket, size_t bufferSize) {
#if !BEHAVIAC_CCDEFINE_APPLE
#ifndef  __GNUC__
            typedef int socklen_t;
#endif
#endif//BEHAVIAC_CCDEFINE_APPLE
            sockaddr_in addr;
            socklen_t len = sizeof(sockaddr_in);
            memset(&addr, 0, sizeof(sockaddr_in));
            SOCKET outSocket = ::accept(::AsSocket(listeningSocket), (sockaddr*)&addr, &len);

            if (outSocket > 0) {
                int sizeOfBufSize = sizeof(bufferSize);
                ::setsockopt(outSocket, SOL_SOCKET, SO_RCVBUF, (const char*)&bufferSize, sizeOfBufSize);
                ::setsockopt(outSocket, SOL_SOCKET, SO_SNDBUF, (const char*)&bufferSize, sizeOfBufSize);
                return Handle(outSocket);
            }

            BEHAVIAC_LOGERROR("Accept failed\n");

            return Handle(0);
        }

        static size_t gs_packetsSent = 0;
        static size_t gs_packetsReceived = 0;

        bool Write(Handle& h, const void* buffer, size_t bytes, size_t& outBytesWritten) {
            outBytesWritten = 0;

            if (bytes == 0 || !h) {
                return bytes == 0;
            }

#if BEHAVIAC_CCDEFINE_APPLE
            const int flags = 0;
#else
            const int flags = MSG_NOSIGNAL;
#endif
            int res = ::send(::AsSocket(h), (const char*)buffer, (int)bytes, flags);

            if (res < 0) {
                //BEHAVIAC_ASSERT(0);
                // int err = WSAGetLastError();

                // if (err == WSAECONNRESET || err == WSAECONNABORTED)
                {
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
            FD_SET(::AsSocket(h), &readfds);
            int maxfd1 = h + 1;

            struct timeval tv;

            tv.tv_sec = 0;
            tv.tv_usec = 100000;//0.1s

            int rv = ::select(maxfd1, &readfds, 0, 0, &tv);

            if (rv < 0) {
                BEHAVIAC_ASSERT(0);
            } else if (rv == 0) {
                //timeout
            } else {
                int res = ::recv(::AsSocket(h), (char*)buffer, (int)bytesMax, 0);

                if (res < 0) {
                    // int err = WSAGetLastError();

                    // if (err == WSAECONNRESET || err == WSAECONNABORTED)
                    {
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
            BEHAVIAC_UNUSED_VAR(hThread);
            // uint32_t ret = ::WaitForSingleObject(hThread, 0);

            // if (ret == 0)
            // {
            // 	return true;
            // }

            return true;
        }

        ThreadHandle CreateAndStartThread(ThreadFunction* function, void* arg, size_t stackSize) {
            BEHAVIAC_UNUSED_VAR(stackSize);
            typedef void* ThreadFunction_t(void * arg);

            pthread_t tid;
            int rc = pthread_create(&tid, NULL, (ThreadFunction_t*)function, arg);

            if (rc) {
                return 0;
            }

            return (ThreadHandle)tid;
        }

        void StopThread(ThreadHandle h) {
            pthread_t tid = (pthread_t)h;
            pthread_join(tid, NULL);
        }

        void Sleep(long millis) {
            //sleep(millis);
            //printf("sleep %ld\n", millis);
            clock_t goal = millis + clock();

            while (goal > clock());
        }

    }//namespace thread
} // namespace behaviac
#endif//#if !BEHAVIAC_CCDEFINE_MSVC
