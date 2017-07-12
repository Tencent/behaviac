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

#ifndef _BEHAVIAC_COMMON_SOCKETCONNECT_BASE_H_
#define _BEHAVIAC_COMMON_SOCKETCONNECT_BASE_H_

#include "behaviac/common/config.h"
#include "behaviac/common/staticassert.h"
#include "behaviac/common/thread/mutex_lock.h"
#include "behaviac/common/thread/thread.h"
#include "behaviac/common/thread/wrapper.h"
#include "behaviac/common/container/string.h"


#include <string>

#define USING_BEHAVIAC_SEQUENTIAL	1

#if USING_BEHAVIAC_SEQUENTIAL
#include <cstdlib>
#endif

namespace behaviac {
#if USING_BEHAVIAC_SEQUENTIAL
    struct Seq {
        Seq() : m_seq(0) {}
        behaviac::Atomic32 Next() {
            return behaviac::AtomicInc(m_seq) - 1;
        }
        volatile behaviac::Atomic32 m_seq;
    };
#else
    struct Seq {
        behaviac::Atomic32 Next() {
            return 0;
        }
    };
#endif // USING_BEHAVIAC_SEQUENTIAL
    Seq& GetNextSeq();

    //typedef const void*	Address;

    const size_t	kMaxPacketDataSize = 230;
    const size_t	kMaxPacketSize = 256;
    const size_t	kSocketBufferSize = 16384;
    const size_t	kGlobalQueueSize = (1024 * 32);
    const size_t	kLocalQueueSize = (1024 * 8);

    inline uint32_t ByteSwap32(uint32_t i) {
        return (0xFF & i) << 24 | (0xFF00 & i) << 8 | (0xFF0000 & i) >> 8 | (0xFF000000 & i) >> 24;
    }

    inline uint64_t ByteSwap64(uint64_t i) {
        //return (0xFF & i) << 24 | (0xFF00 & i) << 8 | (0xFF0000 & i) >> 8 | (0xFF000000 & i) >> 24;
        BEHAVIAC_ASSERT(0, "unimplemented");
        return i;
    }

    // For the time being only 32-bit pointers are supported.
    // Compile time error for other architectures.
    template<size_t N> struct PtrSizeHelper {};
    template<> struct PtrSizeHelper<4> {
        static uintptr_t ByteSwapAddress(uintptr_t a) {
            return ByteSwap32((uint32_t)a);
        }
    };

#if BEHAVIAC_CCDEFINE_64BITS
    template<> struct PtrSizeHelper<8> {
        static uintptr_t ByteSwapAddress(uintptr_t a) {
            return ByteSwap64((uint64_t)a);
        }
    };
#endif//#if BEHAVIAC_OS_WIN64

    inline Address ByteSwapAddress(Address a) {
        return (Address)PtrSizeHelper<sizeof(Address)>::ByteSwapAddress((uintptr_t)a);
    }

    // We assume that receiving application is little endian (PC).
#if BEHAVIAC_LITTLE_ENDIAN
    inline uint32_t ByteSwapToNet32(uint32_t i) {
        return i;
    }

    inline Address ByteSwapAddressToNet(Address a) {
        return a;
    }
#else
    inline uint32_t ByteSwapToNet32(uint32_t i) {
        return ByteSwap32(i);
    }

    inline Address ByteSwapAddressToNet(Address a) {
        return ByteSwapAddress(a);
    }
#endif

    struct ModuleInfo {
        enum { BEHAVIAC_MAX_PATH_LEN = 128 };

        unsigned long	moduleBase;
        unsigned long	moduleSize;
		char			debugInfoFile[BEHAVIAC_MAX_PATH_LEN];
    };

    namespace CommandId {
        enum Enum {
            CMDID_INITIAL_SETTINGS = 1,
            CMDID_TEXT
        };
    }

    const uint32_t kMaxTextLength = 228;

    struct Text {
        char	buffer[kMaxTextLength + 1];
    };

    BEHAVIAC_STATIC_ASSERT(sizeof(Text) < kMaxPacketDataSize);

    struct Packet {
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(Packet);

        explicit Packet(uint8_t commandId = 0xFF, long seq_ = 0) {
            this->Init(commandId, seq_);
        }

        void Init(uint8_t commandId, long seq_) {
            this->messageSize = 0;
            this->command = commandId;

#if USING_BEHAVIAC_SEQUENTIAL
            this->seq = seq_;
#else
            (void)sizeof(seq_);
#endif
        }

        uint32_t CalcPacketSize() const;

        size_t PrepareToSend() {
            const uint32_t packetSize = CalcPacketSize();
            BEHAVIAC_ASSERT(packetSize < kMaxPacketSize);
            messageSize = static_cast<uint8_t>(packetSize);
            return messageSize + 1;
        }

        uint8_t				messageSize;
        uint8_t				command;

        uint8_t				data[kMaxPacketDataSize];

        // has to be the last member variable, it's not being sent to tracer application.
#if USING_BEHAVIAC_SEQUENTIAL
        behaviac::Atomic32	seq;
#endif
    };

    BEHAVIAC_STATIC_ASSERT(sizeof(Packet) < kMaxPacketSize);

#if USING_BEHAVIAC_SEQUENTIAL
    //BEHAVIAC_STATIC_ASSERT((BEHAVIAC_OFFSETOF_POD(Packet, seq) == (sizeof(Packet) - sizeof(behaviac::Atomic32))));	// seq must be the last member

    inline int PacketCompare(const void* lhs, const void* rhs) {
        const Packet& pa = *(const Packet*)lhs;
        const Packet& pb = *(const Packet*)rhs;

        BEHAVIAC_ASSERT((BEHAVIAC_OFFSETOF_POD(Packet, seq) == (sizeof(Packet) - sizeof(behaviac::Atomic32))));	// seq must be the last member

        if (pa.seq < pb.seq) {
            return -1;
        }

        if (pa.seq > pb.seq) {
            return 1;
        }

        return 0;
    }
#else
    BEHAVIAC_STATIC_ASSERT(sizeof(Packet) == sizeof(AllocInfo) + 2);
#endif

    namespace Socket {
        typedef int	Handle;
        typedef unsigned short	Port;

        BEHAVIAC_API bool InitSockets();
        BEHAVIAC_API void ShutdownSockets();

        BEHAVIAC_API Handle Create(bool blocking);
        BEHAVIAC_API void Close(Handle&);

        BEHAVIAC_API bool Listen(Handle, Port port, int maxConnections = 5);
        BEHAVIAC_API bool TestConnection(Handle);
        BEHAVIAC_API Handle Accept(Handle listeningSocket, size_t bufferSize);

        BEHAVIAC_API bool Write(Handle& h, const void* buffer, size_t bytes, size_t& outBytesWritten);

        BEHAVIAC_API size_t Read(Handle& h, const void* buffer, size_t bytes);

        BEHAVIAC_API size_t GetPacketsSent();
        BEHAVIAC_API size_t GetPacketsReceived();
    } // namespace Socket

    namespace Platform {
        enum Enum {
            WINDOWS
        };
    }

    struct Packet;
    class PacketBuffer;
    class PacketCollection;
    class CustomObjectPool;

    class ConnectorInterface {
    public:
        ConnectorInterface();
        virtual ~ConnectorInterface();

        bool Init(int maxTracedThreads, unsigned short port, bool bBlocking);
        void Close();

        void CreateAndStartThread();

        bool IsConnected() const;
        bool IsDisconnected() const;
        bool IsConnectedFinished() const;
        bool IsInited() const;
        void SetConnectPort(unsigned short port);
        void AddPacket(const Packet& packet, bool bReserve);
        void RecordText(const char* text);
        virtual void OnConnection() = 0;
        virtual void OnRecieveMessages(const behaviac::string& msgs);
        void SendAllPackets();

        int GetBufferIndex(bool bReserve);

        behaviac::Socket::Handle GetWriteSocket() {
            return this->m_writeSocket;
        }

        //return true if 'msgCheck' is received
        bool ReceivePackets(const char* msgCheck = 0);
        void ThreadFunc();

        size_t GetMemoryOverhead() const;
        size_t GetNumTrackedThreads() const;
        int GetPacketsCount() const;

        void SendText(const char* text, uint8_t commandId = CommandId::CMDID_TEXT);
        bool ReadText(behaviac::string& text);
    protected:
        int ReserveThreadPacketBuffer();
        void Log(const char* msg);
        virtual void Clear();
        void SendExistingPackets();

        unsigned short				m_port;
        behaviac::Socket::Handle	m_writeSocket;

        PacketBuffer**				m_packetBuffers;
        PacketCollection*			m_packetCollection;
        CustomObjectPool*			m_packetPool;
        int							m_maxTracedThreads;
        volatile Atomic32			m_isInited;
        volatile Atomic32			m_isConnected;
        volatile Atomic32			m_isDisconnected;
        volatile Atomic32			m_isConnectedFinished;
        volatile Atomic32			m_terminating;
        volatile int				m_packetsCount;

        struct PacketsStats {
            int texts;
            int init;

            PacketsStats() : texts(0), init(0) {
            }
        };

        thread::ThreadHandle		s_tracerThread;
        Mutex						m_packetBuffersLock;
        behaviac::string			ms_texts;
        behaviac::Mutex				ms_cs;
        volatile bool				m_bHandleMessage;
    public:
        PacketsStats				gs_packetsStats;
    };

    extern behaviac::ThreadInt		gs_threadFlag;
}//namespace behaviac

#endif//_BEHAVIAC_COMMON_SOCKETCONNECT_BASE_H_
