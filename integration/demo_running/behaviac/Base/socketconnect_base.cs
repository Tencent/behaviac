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

#if !BEHAVIAC_RELEASE
// Setting this to true makes sure that messages sent in one batch
// are ordered same as they were generated (when they come from different threads).
// It has a slight memory/performance overhead.
#define USING_BEHAVIAC_SEQUENTIAL

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace behaviac
{
    public struct Atomic32
    {
        private ulong m_value;

        public Atomic32(ulong v)
        {
            m_value = v;
        }

        public void Set(ulong v)
        {
            m_value = v;
        }

        public ulong Get()
        {
            return m_value;
        }

        public ulong AtomicInc()
        {
            m_value++;
            return m_value;
        }

        public ulong AtomicDec()
        {
            m_value--;
            return m_value;
        }
    }

    public static class SocketConnection
    {
#if USING_BEHAVIAC_SEQUENTIAL

        public class Seq
        {
            public Seq()
            {
                //m_seq = 0;
            }

            public ulong Next()
            {
                ulong v = m_seq.AtomicInc();
                return v - 1;
            }

            public Atomic32 m_seq;
        };

#else
        struct Seq
        {
            behaviac.Atomic32 Next()
            {
                return 0;
            }
        };
#endif // USING_BEHAVIAC_SEQUENTIAL
        private static Seq s_seq = new Seq();

        public static Seq GetNextSeq()
        {
            return s_seq;
        }

        public const int kMaxPacketDataSize = 230;
        public const int kMaxPacketSize = 256;
        public const int kSocketBufferSize = 16384 * 10;
        public const int kGlobalQueueSize = (1024 * 32);
        public const int kLocalQueueSize = (1024 * 8);

        public static uint ByteSwap32(uint i)
        {
            return (0xFF & i) << 24 | (0xFF00 & i) << 8 | (0xFF0000 & i) >> 8 | (0xFF000000 & i) >> 24;
        }

        public static ulong ByteSwap64(ulong i)
        {
            //return (0xFF & i) << 24 | (0xFF00 & i) << 8 | (0xFF0000 & i) >> 8 | (0xFF000000 & i) >> 24;
            Debug.Check(false, "unimplemented");
            return i;
        }

        // For the time being only 32-bit pointers are supported.
        // Compile time error for other architectures.
        public static UIntPtr ByteSwapAddress(UIntPtr a)
        {
            return (UIntPtr)ByteSwap32((uint)a);
        }

#if BEHAVIAC_COMPILER_64BITS
        static UIntPtr ByteSwapAddress(UIntPtr a)
        {
            return (UIntPtr)ByteSwap64((ulong)a);
        }
#endif//#if BEHAVIAC_OS_WIN64
    }

    internal enum CommandId
    {
        CMDID_INITIAL_SETTINGS = 1,
        CMDID_TEXT
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct Text
    {
        private const int kMaxTextLength = 228;

        //public string buffer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = kMaxTextLength + 1)]
        private byte[] buffer;
    };

    [StructLayout(LayoutKind.Sequential)]
    public class Packet
    {
        public Packet()
        {
        }

        public Packet(byte commandId, ulong seq_)
        {
            this.Init(commandId, seq_);
        }

        public void Init(byte commandId, ulong seq_)
        {
            this.messageSize = 0;
            this.command = commandId;

#if USING_BEHAVIAC_SEQUENTIAL
            this.seq.Set(seq_);
#else
            (void)sizeof(seq_);
#endif
        }

        public int CalcPacketSize()
        {
            int packetSize = (0);

            if (command == (byte)CommandId.CMDID_TEXT)
            {
                if (this.data != null)
                {
                    packetSize = this.data.Length;
                }
                else
                {
                    packetSize = Marshal.SizeOf(typeof(Text));
                }
            }
            else
            {
                Debug.Check(false, "Unknown command");
            }

            packetSize += Marshal.SizeOf(command);
            return packetSize;
        }

        public void SetData(string text)
        {
            byte[] ascII = System.Text.Encoding.ASCII.GetBytes(text);

            Debug.Check(ascII.Length < SocketConnection.kMaxPacketDataSize);
            this.data = new byte[ascII.Length];

            System.Buffer.BlockCopy(ascII, 0, this.data, 0, ascII.Length);
        }

        public byte[] GetData()
        {
            int len = this.PrepareToSend();
            byte[] da = new byte[len];

            da[0] = messageSize;
            da[1] = command;

            Array.Copy(this.data, 0, da, 2, this.data.Length);

            return da;
        }

        public int PrepareToSend()
        {
            int packetSize = CalcPacketSize();
            Debug.Check(packetSize < SocketConnection.kMaxPacketSize);
            messageSize = (byte)packetSize;
            return messageSize + 1;
        }

        private byte messageSize;
        private byte command;

        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = SocketConnection.kMaxPacketDataSize)]
        private byte[] data;

        // IMPORTANT: has to be the last member variable, it's not being sent
        // to tracer application.
#if USING_BEHAVIAC_SEQUENTIAL
        public Atomic32 seq;
#endif
    };

    //public static class StructExtentsion
    //{
    //    private static T ReadStruct<T>(this BinaryReader reader) where T : struct
    //    {
    //        Byte[] buffer = new Byte[Marshal.SizeOf(typeof(T))];
    //        reader.Read(buffer, 0, buffer.Length);
    //        GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
    //        T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
    //        handle.Free();
    //        return result;
    //    }
    //}

    // Minimal subset of functionality that we need.
    public static class SocketBase
    {
        public static bool InitSockets()
        {
            return true;
        }

        public static void ShutdownSockets()
        {
        }

        public static Socket Create(bool blocking)
        {
            Socket h = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (h == null)
            {
                return null;
            }

            h.Blocking = blocking;
            return h;
        }

        public static void Close(ref Socket h)
        {
            if (h != null)
            {
                h.Close();
                h = null;
            }
        }

        public static bool Listen(Socket h, ushort port, int maxConnections)
        {
            try
            {
                System.Net.IPEndPoint ipe = new System.Net.IPEndPoint(System.Net.IPAddress.Any, port);

                h.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //h.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);

                h.Bind(ipe);

                h.Listen(maxConnections);

                return true;
            }
            catch
            {
            }

            return false;
        }

        public static bool TestConnection(Socket h)
        {
            bool hasRead = h.Poll(17000, SelectMode.SelectRead);

            if (hasRead)
            {
                return true;
            }

            return false;
        }

        public static Socket Accept(Socket listeningSocket, int bufferSize)
        {
            Socket outSocket = listeningSocket.Accept();

            if (outSocket != null)
            {
                outSocket.ReceiveBufferSize = bufferSize;
                outSocket.SendBufferSize = bufferSize;
                return outSocket;
            }

            return null;
        }

        private static uint gs_packetsSent = 0;
        private static uint gs_packetsReceived = 0;

        public static bool Write(Socket h, byte[] buffer, ref int outBytesWritten)
        {
            int bytes = buffer != null ? buffer.Length : 0;

            if (bytes == 0 || h == null)
            {
                return bytes == 0;
            }

            try
            {
                int res = h.Send(buffer);

                if (res == -1)
                {
                    outBytesWritten = 0;

                    if (!h.Connected)
                    {
                        Close(ref h);
                    }
                }
                else
                {
                    outBytesWritten = res;
                    gs_packetsSent++;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            return outBytesWritten != 0;
        }

        public static int Read(ref Socket h, byte[] buffer, int bytesMax)
        {
            int bytesRead = 0;

            if (bytesMax == 0 || h == null)
            {
                return bytesRead;
            }
            else if (!h.Connected)
            {
                Close(ref h);
                return 0;
            }

            bool hasData = h.Poll(1000, SelectMode.SelectRead);

            if (hasData)
            {
                if (h.Available == 0)
                {
                    //disconnected
                    Close(ref h);
                    return 0;
                }

                int res = h.Receive(buffer, bytesMax, SocketFlags.None);

                if (res == -1)
                {
                    Close(ref h);
                }
                else
                {
                    bytesRead = res;
                    gs_packetsReceived++;
                }

                return bytesRead;
            }

            return 0;
        }

        public static uint GetPacketsSent()
        {
            return gs_packetsSent;
        }

        public static uint GetPacketsReceived()
        {
            return gs_packetsReceived;
        }
    }// namespace Socket

    internal enum Platform
    {
        WINDOWS
    };

#if USING_BEHAVIAC_SEQUENTIAL

    public class PacketCollection
    {
        public PacketCollection()
        {
            m_packets = null;
            m_packetsEnd = (0);
            m_packetsCapacity = (0);
        }

        //~PacketCollection(
        //{
        //    Close();
        //}

        public void Init(int capacity)
        {
            Debug.Check(m_packets == null);
            m_packets = new Packet[capacity];
            m_packetsEnd = 0;
            m_packetsCapacity = capacity;
        }

        public void Close()
        {
            m_packets = null;
        }

        public Packet[] GetPackets(ref int end)
        {
            end = this.m_packetsEnd;
            return m_packets;
        }

        public int GetMemoryOverhead()
        {
            return (m_packetsCapacity) * Marshal.SizeOf(typeof(Packet));
        }

        // False if not enough space, packet not added.
        public bool Add(Packet packet)
        {
            if (m_packetsEnd == m_packetsCapacity)
            {
                behaviac.Debug.LogWarning("buffer overflow...\n");
                return false;
            }

            m_packets[m_packetsEnd++] = packet;
            return true;
        }

        public void Reset()
        {
            m_packetsEnd = 0;
        }

        public void Sort()
        {
            PacketComparer c = new PacketComparer();
            Array.Sort(m_packets, 0, this.m_packetsEnd, c);
        }

        private class PacketComparer : IComparer<Packet>
        {
#if USING_BEHAVIAC_SEQUENTIAL

            public int Compare(Packet pa, Packet pb)
            {
                if (pa.seq.Get() < pb.seq.Get())
                {
                    return -1;
                }

                if (pa.seq.Get() > pb.seq.Get())
                {
                    return 1;
                }

                return 0;
            }

#else
            int Compare(Packet pa, Packet pb)
            {
                return 0;
            }
#endif
        }

        private Packet[] m_packets;
        private int m_packetsEnd;
        private int m_packetsCapacity;
    };

#else
    class PacketCollection
    {
        void Init(uint) {}
        void Close() {}
        uint GetMemoryOverhead()
        {
            return 0;
        }
    };
#endif // #if USING_BEHAVIAC_SEQUENTIAL

    public class PacketBuffer
    {
        private ConnectorInterface _conector;

        public PacketBuffer(ConnectorInterface c)
        {
            _conector = c;
            m_free = true;
        }

        public void AddPacket(Packet packet)
        {
            if (packet != null)
            {
                // Spin loop until there is a place for new packet.
                // If this happens to often, it means we are producing packets
                // quicker than consuming them, increasing max # of packets in buffer
                // could help.
                while (m_packetQueue.Count >= SocketConnection.kLocalQueueSize)
                {
                    //BEHAVIAC_LOGINFO("packet buffer is full... buffer size: %d\n", MAX_PACKETS_IN_BUFFER);
                    System.Threading.Thread.Sleep(1);

                    if (this._conector.WriteSocket == null || !this._conector.WriteSocket.Connected)
                    {
                        break;
                    }
                }

                lock (m_packetQueue)
                {
                    m_packetQueue.Enqueue(packet);
                }
            }
        }

#if USING_BEHAVIAC_SEQUENTIAL

        public bool CollectPackets(PacketCollection coll)
        {
            if (m_packetQueue.Count == 0)
            {
                return true;
            }

            lock (m_packetQueue)
            {
                Packet packet = m_packetQueue.Peek();

                while (packet == null)
                {
                    m_packetQueue.Dequeue();

                    if (m_packetQueue.Count == 0)
                    {
                        break;
                    }

                    packet = m_packetQueue.Peek();
                }

                while (packet != null)
                {
                    if (coll.Add(packet))
                    {
                        m_packetQueue.Dequeue();

                        if (m_packetQueue.Count == 0)
                        {
                            break;
                        }

                        packet = m_packetQueue.Peek();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

#endif

        private void SendPackets(Socket h)
        {
            lock (m_packetQueue)
            {
                Packet packet = m_packetQueue.Peek();

                while (packet != null)
                {
                    int bytesWritten = (0);
                    bool success = SocketBase.Write(h, packet.GetData(), ref bytesWritten);

                    // Failed to send data. Most probably sending too much, break and
                    // hope for the best next time
                    if (!success)
                    {
                        Debug.Check(false);
                        behaviac.Debug.LogWarning("A packet is not correctly sent...\n");
                        break;
                    }

                    m_packetQueue.Dequeue();	// 'Commit' pop if data sent.
                    packet = m_packetQueue.Peek();
                }
            }
        }

        public void Clear()
        {
            m_packetQueue.Clear();
        }

        public bool m_free;

        private Queue<Packet> m_packetQueue = new Queue<Packet>();
    };

    /// <summary>
    /// Represents a pool of objects with a size limit.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public class ObjectPool<T> : IDisposable
        where T : new()
    {
        private readonly int size;
        private readonly object locker;
        private readonly Queue<T> queue;
        private int count;

        /// <summary>
        /// Initializes a new instance of the ObjectPool class.
        /// </summary>
        /// <param name="size">The size of the object pool.</param>
        public ObjectPool(int size)
        {
            if (size <= 0)
            {
                const string message = "The size of the pool must be greater than zero.";
                throw new ArgumentOutOfRangeException("size", size, message);
            }

            this.size = size;
            locker = new object();
            queue = new Queue<T>();
        }

        /// <summary>
        /// Retrieves an item from the pool.
        /// </summary>
        /// <returns>The item retrieved from the pool.</returns>
        public T Get()
        {
            lock (locker)
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }

                count++;
                return new T();
            }
        }

        /// <summary>
        /// Places an item in the pool.
        /// </summary>
        /// <param name="item">The item to place to the pool.</param>
        public void Put(T item)
        {
            lock (locker)
            {
                if (count < size)
                {
                    queue.Enqueue(item);
                }
                else
                {
                    using(item as IDisposable)
                    {
                        count--;
                    }
                }
            }
        }

        /// <summary>
        /// Disposes of items in the pool that implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            lock (locker)
            {
                count = 0;

                while (queue.Count > 0)
                {
                    using(queue.Dequeue() as IDisposable)
                    {
                    }
                }
            }
        }
    }

    public class CustomObjectPool : IEnumerable
    {
        private readonly int m_capacity;

        private class PacketSegment
        {
            public int m_count;
            public Packet[] m_buffer;

            public PacketSegment m_next;
        }

        private PacketSegment m_segments;

        public CustomObjectPool(int objectCountPerSegment)
        {
            m_capacity = objectCountPerSegment;
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            PacketSegment n = m_segments;

            while (n != null)
            {
                for (int i = 0; i < n.m_count; ++i)
                {
                    yield return n.m_buffer[i];
                }

                n = n.m_next;
            }
        }

        public Packet Allocate()
        {
            if (m_segments == null || m_segments.m_count >= m_capacity)
            {
                AllocSegment();
            }

            Packet p = m_segments.m_buffer[m_segments.m_count++];

            return p;
        }

        private void AllocSegment()
        {
            PacketSegment n = new PacketSegment();
            n.m_count = 0;
            n.m_buffer = new Packet[m_capacity];

            for (int i = 0; i < m_capacity; ++i)
            {
                n.m_buffer[i] = new Packet();
            }

            if (m_segments != null)
            {
                m_segments.m_next = n;
            }

            m_segments = n;
        }

        public void Clear()
        {
            PacketSegment n = m_segments;

            while (n != null)
            {
                n.m_buffer = null;

                n = n.m_next;
            }

            m_segments = null;
        }

        public int GetMemoryUsage()
        {
            return 0;
        }
    };

    public abstract class ConnectorInterface
    {
        private const int kMaxTextLength = 228;

        public ConnectorInterface()
        {
            Debug.Check(Marshal.SizeOf(typeof(Text)) < SocketConnection.kMaxPacketDataSize);
            Debug.Check(Marshal.SizeOf(typeof(Packet)) < SocketConnection.kMaxPacketSize);

#if !USING_BEHAVIAC_SEQUENTIAL
            Debug.Check(sizeof(Packet) == sizeof(AllocInfo) + 2);
#endif

#if USING_BEHAVIAC_SEQUENTIAL
            //Debug.Check(sizeof(Packet) == sizeof(Text) + 2 + 4);
            Debug.Check((int)Marshal.OffsetOf(typeof(Packet), "seq") == Marshal.SizeOf(typeof(Packet)) - Marshal.SizeOf(typeof(Atomic32)));	// seq must be the last member
#endif
            // Local queue size must be power of two.
            Debug.Check((SocketConnection.kLocalQueueSize & (SocketConnection.kLocalQueueSize - 1)) == 0);

            m_port = (0);
            m_writeSocket = null;
            m_packetBuffers = null;
            m_maxTracedThreads = 0;
            m_isConnected.Set(0);
            m_isDisconnected.Set(0);
            m_isConnectedFinished.Set(0);
            m_isInited.Set(0);
            m_terminating.Set(0);
            m_packetPool = null;
            m_packetCollection = null;
            m_packetsCount = 0;
            s_tracerThread = null;
            m_bHandleMessage = true;
        }

        ~ConnectorInterface()
        {
            this.Close();
        }

        private void RecordText(string text)
        {
            if (this.m_packetPool != null)
            {
                //if it is out of memory here, please check 'SetupConnection'
                Packet pP = this.m_packetPool.Allocate();

                if (pP != null)
                {
                    pP.Init((byte)CommandId.CMDID_TEXT, SocketConnection.GetNextSeq().Next());

                    pP.SetData(text);
                }
            }
        }

        private void CreateAndStartThread()
        {
            s_tracerThread = new System.Threading.Thread(MemTracer_ThreadFunc);
            s_tracerThread.Start();
        }

        public bool IsConnected()
        {
            return m_isConnected.Get() != 0;
        }

        private bool IsDisconnected()
        {
            return m_isDisconnected.Get() != 0;
        }

        private bool IsConnectedFinished()
        {
            return m_isConnectedFinished.Get() != 0;
        }

        public bool IsInited()
        {
            return m_isInited.Get() != 0;
        }

        private void SetConnectPort(ushort port)
        {
            this.m_port = port;
        }

        private void AddPacket(Packet packet, bool bReserve)
        {
            if (this.IsConnected() && this.m_writeSocket != null && this.m_writeSocket.Connected)
            {
                int bufferIndex = this.GetBufferIndex(bReserve);

                if (bufferIndex > 0)
                {
                    m_packetBuffers[bufferIndex].AddPacket(packet);

                    this.m_packetsCount++;
                }
                else
                {
                    //Debug.Check(false);
                    Debug.LogError("invalid bufferIndex");
                }
            }
        }

        private int GetBufferIndex(bool bReserve)
        {
            //Debug.Check(t_packetBufferIndex != -1);
            int bufferIndex = (int)t_packetBufferIndex;
            //WHEN bReserve is false, it is unsafe to allocate memory as other threads might be allocating
            //you can avoid the following assert to malloc a block of memory in your thread at the very beginning
            Debug.Check(bufferIndex > 0 || bReserve);

            //bufferIndex initially is 0
            if (bufferIndex <= 0 && bReserve)
            {
                bufferIndex = ReserveThreadPacketBuffer();
            }

            return bufferIndex;
        }

        protected abstract void OnConnection();

        protected virtual void OnRecieveMessages(string msgs)
        {
        }

        protected void SendAllPackets()
        {
            if (this.m_writeSocket != null && this.m_writeSocket.Connected)
            {
                for (int i = 0; i < m_maxTracedThreads; ++i)
                {
                    if (m_packetBuffers[i] != null && !m_packetBuffers[i].m_free)
                    {
#if USING_BEHAVIAC_SEQUENTIAL

                        if (!m_packetBuffers[i].CollectPackets(m_packetCollection))
                        {
                            break;
                        }

#else
                        m_packetBuffers[i].SendPackets(m_writeSocket);
#endif
                    }
                }

#if USING_BEHAVIAC_SEQUENTIAL
                // TODO: Deal with Socket.Write failures.
                // (right now packet is lost).
                m_packetCollection.Sort();

                int endIndex = 0;
                Packet[] packets = m_packetCollection.GetPackets(ref endIndex);

                for (int i = 0; i < endIndex; ++i)
                {
                    Packet p = packets[i];

                    int bytesWritten = (0);
                    SocketBase.Write(this.m_writeSocket, p.GetData(), ref bytesWritten);

                    if (this.m_writeSocket == null || !this.m_writeSocket.Connected || bytesWritten <= 0)
                    {
                        break;
                    }
                }

                m_packetCollection.Reset();
#endif
                this.m_packetsCount = 0;
            }
        }

        private static string GetStringFromBuffer(byte[] data, int dataIdx, int maxLen, bool isAsc)
        {
            System.Text.Encoding ecode;

            if (isAsc)
            {
                ecode = new System.Text.ASCIIEncoding();
            }
            else
            {
                ecode = new System.Text.UTF8Encoding();
            }

            string ret = ecode.GetString(data, dataIdx, maxLen);
            char[] zeroChars = { '\0', '?' };

            return ret.TrimEnd(zeroChars);
        }

        /**
        return true if 'msgCheck' is received
        */

        protected bool ReceivePackets(string msgCheck)
        {
            if (this.m_writeSocket != null && this.m_writeSocket.Connected)
            {
                int kBufferLen = 2048;
                byte[] buffer = new byte[kBufferLen];

                bool found = false;
                int reads = SocketBase.Read(ref m_writeSocket, buffer, kBufferLen);

                while (reads > 0 && this.m_writeSocket != null && this.m_writeSocket.Connected)
                {
                    //BEHAVIAC_LOG(MEMDIC_LOG_INFO, buffer);

                    lock (this)
                    {
                        ms_texts += GetStringFromBuffer(buffer, 0, reads, true);
                    }

                    if (!string.IsNullOrEmpty(msgCheck) && ms_texts.IndexOf(msgCheck) != -1)
                    {
                        found = true;
                    }

                    reads = SocketBase.Read(ref m_writeSocket, buffer, kBufferLen);
                }

                if (this.m_bHandleMessage && this.m_writeSocket != null && this.m_writeSocket.Connected)
                {
                    string msgs = "";

                    if (this.ReadText(ref msgs))
                    {
                        this.OnRecieveMessages(msgs);

                        return true;
                    }
                }

                return found;
            }

            return false;
        }

        private void ThreadFunc()
        {
            Log("behaviac: Socket Thread Starting\n");

            try
            {
                this.ReserveThreadPacketBuffer();
                int bufferIndex = t_packetBufferIndex;
                Debug.Check(bufferIndex > 0);

                bool blockingSocket = true;
                Socket serverSocket = null;

                try
                {
                    serverSocket = SocketBase.Create(blockingSocket);

                    if (serverSocket == null)
                    {
                        Log("behaviac: Couldn't create server socket.\n");
                        return;
                    }

                    string bufferTemp = string.Format("behaviac: Listening at port {0}...\n", m_port);
                    Log(bufferTemp);

                    // max connections: 1, don't allow multiple clients?
                    if (!SocketBase.Listen(serverSocket, m_port, 1))
                    {
                        Log("behaviac: Couldn't configure server socket.\n");
                        SocketBase.Close(ref serverSocket);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }

                while (m_terminating.Get() == 0)
                {
                    //wait for connecting
                    while (m_terminating.Get() == 0)
                    {
                        if (SocketBase.TestConnection(serverSocket))
                        {
                            break;
                        }

                        System.Threading.Thread.Sleep(100);
                    }

                    if (m_terminating.Get() == 0)
                    {
                        Log("behaviac: accepting...\n");

                        try
                        {
                            m_writeSocket = SocketBase.Accept(serverSocket, SocketConnection.kSocketBufferSize);

                            if (m_writeSocket == null)
                            {
                                Log("behaviac: Couldn't create write socket.\n");
                                SocketBase.Close(ref serverSocket);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }

                        try
                        {
                            m_isConnected.AtomicInc();
                            System.Threading.Thread.Sleep(1);

                            OnConnection();

                            m_isConnectedFinished.AtomicInc();
                            System.Threading.Thread.Sleep(1);

                            //this.OnConnectionFinished();

                            Log("behaviac: Connected. accepted\n");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }

                        try
                        {
                            while (m_terminating.Get() == 0 && this.m_writeSocket != null)
                            {
                                System.Threading.Thread.Sleep(1);

                                this.SendAllPackets();
                                this.ReceivePackets("");
                            }

                            if (this.m_writeSocket != null && this.m_writeSocket.Connected)
                            {
                                // One last time, to send any outstanding packets out there.
                                this.SendAllPackets();
                            }

                            SocketBase.Close(ref this.m_writeSocket);
                            this.Clear();

                            Log("behaviac: disconnected. \n");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.Message + ex.StackTrace);
                        }
                    }
                }//while (!m_terminating)

                SocketBase.Close(ref serverSocket);

                this.Clear();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            Log("behaviac: ThreadFunc exited. \n");
        }

        public int GetMemoryOverhead()
        {
            int threads = GetNumTrackedThreads();
            int bufferSize = Marshal.SizeOf(typeof(PacketBuffer)) * threads;
            int packetCollectionSize = m_packetCollection != null ? m_packetCollection.GetMemoryOverhead() : 0;
            int packetPoolSize = m_packetPool != null ? m_packetPool.GetMemoryUsage() : 0;
            return bufferSize + packetCollectionSize + packetPoolSize;
        }

        public int GetNumTrackedThreads()
        {
            int numTrackedThreads = (0);

            if (m_packetBuffers != null)
            {
                for (int i = 0; i < m_maxTracedThreads; ++i)
                {
                    if (m_packetBuffers[i] != null && !m_packetBuffers[i].m_free)
                    {
                        ++numTrackedThreads;
                    }
                }
            }

            return numTrackedThreads;
        }

        public int GetPacketsCount()
        {
            //not thread safe
            if (this.IsConnected())
            {
                return m_packetsCount;
            }

            return 0;
        }

        public void SendText(string text, byte commandId /*= (byte)CommandId.CMDID_TEXT*/)
        {
            if (this.IsConnected())
            {
                Packet packet = new Packet(commandId, SocketConnection.GetNextSeq().Next());

                packet.SetData(text);
                this.AddPacket(packet, true);
                gs_packetsStats.texts++;
            }

            //else
            //{
            //    RecordText(text);
            //}
        }

        public bool ReadText(ref string text)
        {
            if (this.IsConnected())
            {
                lock (this)
                {
                    text = this.ms_texts;
                    this.ms_texts = string.Empty;

                    return !string.IsNullOrEmpty(text);
                }
            }

            return false;
        }

        protected int ReserveThreadPacketBuffer()
        {
            int bufferIndex = t_packetBufferIndex;
            //THREAD_ID_TYPE id = behaviac.GetTID();
            //BEHAVIAC_LOGINFO("ReserveThreadPacketBuffer:%d thread %d\n", bufferIndex, id);

            //bufferIndex initially is -1
            if (bufferIndex <= 0)
            {
                int retIndex = (-2);

                lock (this)
                {
                    // NOTE: This is quite naive attempt to make sure that main thread queue
                    // is the last one (rely on the fact that it's most likely to be the first
                    // one trying to send message). This means EndFrame event should be sent after
                    // memory operations from that frame.
                    // (doesn't matter in SEQUENTIAL mode).
                    for (int i = m_maxTracedThreads - 1; i >= 0; --i)
                    {
                        if (m_packetBuffers[i] == null)
                        {
                            m_packetBuffers[i] = new PacketBuffer(this);
                        }

                        if (m_packetBuffers[i] != null)
                        {
                            if (m_packetBuffers[i].m_free)
                            {
                                m_packetBuffers[i].m_free = false;
                                retIndex = i;
                                break;
                            }
                        }
                    }

                    if (retIndex > 0)
                    {
                        t_packetBufferIndex = retIndex;
                    }
                    else
                    {
                        Log("behaviac: Couldn't reserve packet buffer, too many active threads.\n");
                        Debug.Check(false);
                    }

                    bufferIndex = retIndex;
                }

                //BEHAVIAC_LOGINFO("ReserveThreadPacketBuffer:%d thread %d\n", bufferIndex, id);
            }

            return bufferIndex;
        }

        protected void Log(string msg)
        {
            behaviac.Debug.Log(msg);
        }

        protected virtual void Clear()
        {
            this.m_isConnected.Set(0);
            this.m_isDisconnected.Set(0);
            this.m_isConnectedFinished.Set(0);
            this.m_terminating.Set(0);

            if (this.m_packetBuffers != null)
            {
                int bufferIndex = this.GetBufferIndex(false);

                if (bufferIndex > 0)
                {
                    this.m_packetBuffers[bufferIndex].Clear();
                }
            }

            if (this.m_packetPool != null)
            {
                this.m_packetPool.Clear();
            }

            if (this.m_packetBuffers != null)
            {
                this.m_packetCollection.Reset();
            }

            this.m_packetsCount = 0;
        }

        protected void SendExistingPackets()
        {
            int packetsCount = 0;

            foreach (Packet p in this.m_packetPool)
            {
                int bytesWritten = (0);
                SocketBase.Write(m_writeSocket, p.GetData(), ref bytesWritten);
                packetsCount++;
            }

            //wait for the finish
            System.Threading.Thread.Sleep(1000);

            this.m_packetPool.Clear();
        }

        protected ushort m_port;
        protected Socket m_writeSocket;

        protected PacketBuffer[] m_packetBuffers;
        protected PacketCollection m_packetCollection;
        protected CustomObjectPool m_packetPool;
        protected int m_maxTracedThreads;
        protected Atomic32 m_isInited;
        protected Atomic32 m_isConnected;
        protected Atomic32 m_isDisconnected;
        protected Atomic32 m_isConnectedFinished;
        protected Atomic32 m_terminating;
        protected volatile int m_packetsCount;

        protected struct PacketsStats
        {
            public int texts;
            public int init;
        };

        private System.Threading.Thread s_tracerThread;
        protected string ms_texts;
        protected volatile bool m_bHandleMessage;
        protected PacketsStats gs_packetsStats;

        [ThreadStatic]
        protected static int t_packetBufferIndex = -1;

        public bool Init(int maxTracedThreads, ushort port, bool bBlocking)
        {
            this.Clear();

            m_port = ushort.MaxValue;

            m_packetPool = new CustomObjectPool(4096);
            m_packetCollection = new PacketCollection();
            m_packetBuffers = new PacketBuffer[maxTracedThreads];
            m_maxTracedThreads = maxTracedThreads;
            m_packetCollection.Init(SocketConnection.kGlobalQueueSize);

            if (!SocketBase.InitSockets())
            {
                this.Log("behaviac: Failed to initialize sockets.\n");
                return false;
            }

            {
                behaviac.Debug.Log("behaviac: ConnectorInterface.Init Enter\n");
                string portMsg = string.Format("behaviac: listing at port {0}\n", port);
                behaviac.Debug.Log(portMsg);

                this.ReserveThreadPacketBuffer();
                this.SetConnectPort(port);

                {
                    this.CreateAndStartThread();
                }

                if (bBlocking)
                {
                    Debug.LogWarning("behaviac: SetupConnection is blocked, please Choose 'Connect' in the Designer to continue");

                    while (!this.IsConnected() || !this.IsConnectedFinished())
                    {
                        // Wait for connection
                        System.Threading.Thread.Sleep(100);
                    }

                    System.Threading.Thread.Sleep(1);

                    Debug.Check(this.IsConnected() && this.IsConnectedFinished());
                }

                behaviac.Debug.Log("behaviac: ConnectorInterface.Init Connected\n");

                //wait for the OnConnection ends
                System.Threading.Thread.Sleep(200);

                behaviac.Debug.Log("behaviac: ConnectorInterface.Init successful\n");
            }

            m_isInited.AtomicInc();

            return m_packetBuffers != null;
        }

        public void Close()
        {
            m_terminating.AtomicInc();
            m_isConnectedFinished.AtomicDec();

            m_isDisconnected.AtomicInc();

            if (s_tracerThread != null)
            {
                if (s_tracerThread.IsAlive)
                {
                    while (IsConnected() && s_tracerThread.IsAlive)
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                }

                lock (this)
                {
                    m_packetBuffers = null;
                }

                if (s_tracerThread.IsAlive)
                {
                    s_tracerThread.Abort();
                }

                s_tracerThread = null;
            }

            if (m_packetCollection != null)
            {
                m_packetCollection.Close();
                m_packetCollection = null;
            }

            m_packetPool = null;
            t_packetBufferIndex = -1;
            SocketBase.ShutdownSockets();

            m_isInited.AtomicDec();
        }

        public Socket WriteSocket
        {
            get
            {
                return m_writeSocket;
            }
        }

        private void MemTracer_ThreadFunc(object tracer_)
        {
            //ConnectorInterface tracer = (ConnectorInterface)tracer_;
            this.ThreadFunc();
        }
    }
}

#endif
