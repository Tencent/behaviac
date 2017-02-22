using System;
using System.Threading;
using System.Collections.Generic;

namespace Concurrent
{
    // SPSCQueue{T} is an efficient queue which allows one thread to use the Enqueue
    // and another one use the Dequeue without locking
    public class SPSCQueue<T>
    {
        class Chunk
        {
            public long dist_;
            public volatile int headPos_;
            public volatile Chunk nextChunk_;
            public volatile int tailPos_;
            public T[] values_;

            #region .ctor
            public Chunk(int capacity)
            {
                values_ = new T[capacity];
                headPos_ = 0;
                tailPos_ = 0;
                nextChunk_ = null;
                dist_ = 0;
            }
            #endregion
        }

        const int kDefaultCapacity = 1000;
        volatile Chunk divider_;
        readonly int granularity_;
        volatile Chunk tail_chunk_;

        #region .ctor
        public SPSCQueue()
        : this(kDefaultCapacity)
        {
        }

        public SPSCQueue(int granularity)
        {
            granularity_ = granularity;
            divider_ = new Chunk(granularity);

            divider_.tailPos_ = granularity - 1;
            divider_.headPos_ = granularity;

            tail_chunk_ = divider_;
        }
        #endregion

        public void Enqueue(T element)
        {
            int tail_pos = tail_chunk_.tailPos_;

            // If either the queue is not empty or the tail chunk is not full, adds
            // the specified element to the back end of the current tail chunk.
            if (tail_chunk_ != divider_ && ++tail_pos < granularity_)
            {
                tail_chunk_.values_[tail_pos] = element;

                // Prevents any kind of instruction reorderring or caching.
                Thread.MemoryBarrier();

                // "Commit" the newly added item and "publish" it atomically
                // to the consumer thread.
                tail_chunk_.tailPos_ = tail_pos;
                return;
            }

            // Create a new chunk if a cached one does not exists and links it
            // to the current last node.
            Chunk chunk = new Chunk(granularity_);
            tail_chunk_.nextChunk_ = chunk;

            // Reset the chunk and append the specified element to the first slot.
            chunk.tailPos_ = 0; // An unconsumed element is added to the first slot.
            chunk.headPos_ = 0;
            chunk.nextChunk_ = null;
            chunk.values_[0] = element;
            chunk.dist_ = tail_chunk_.dist_ + 1;

            // Make sure that the new chunk is fully initialized before it is
            // assigned to the tail chunk.
            Thread.MemoryBarrier();

            // At this point the newly created chunk(or the last cached chunk) is
            // not yet shared, but still private to the producer; the consumer will
            // not follow the linked chunk unless the value of |tail_chunk_| says
            // it may follow. The line above "commit" the update and publish it
            // atomically to the consumer thread.
            tail_chunk_ = tail_chunk_.nextChunk_;
        }

        public void Clear()
        {
            // Save the current tail chunk to ensure that the future elements are
            // not cleared.
            Chunk current_tail_chunk = tail_chunk_;

            while (divider_ != current_tail_chunk)
            {
                divider_ = divider_.nextChunk_;
            }
        }

        public T Dequeue()
        {
            T t;
            bool ok = Dequeue(out t);

            if (!ok)
            {
                throw new InvalidOperationException("invalid operation");
            }

            return t;
        }

        public bool Dequeue(out T t)
        {
            // checks if the queue is empty
            while (divider_ != tail_chunk_)
            {
                // The chunks that sits between the |divider_| and the |tail_chunk_|,
                // excluding the |divider_| and including the |tail_chunk_|, are
                // unconsumed.
                Chunk current_chunk = divider_.nextChunk_;

                // We need to compare the current chunk |tail_pos| with the |head_pos|
                // and |granularity|. Since, the |tail_pos| can be modified by the
                // producer thread we need to cache it instantaneous value.
                int tail_pos;
                tail_pos = current_chunk.tailPos_;

                if (current_chunk.headPos_ > tail_pos)
                {
                    if (tail_pos == granularity_ - 1)
                    {
                        // we have reached the end of the chunk, go to the next chunk and
                        // frees the unused chunk.
                        divider_ = current_chunk;
                        //head_chunk_ = head_chunk_.next;

                    }
                    else
                    {
                        // we already consume all the available itens.
                        t = default(T);
                        return false;
                    }

                }
                else
                {
                    // Ensure that we are reading the freshness value from the chunk
                    // values array.
                    Thread.MemoryBarrier();

                    // Here the |head_pos| is less than or equals to |tail_pos|, get
                    // the first unconsumed element and increments |head_pos| to publish
                    // the queue item removal.
                    t = current_chunk.values_[current_chunk.headPos_];

                    // keep the order between assignment and publish operations.
                    Thread.MemoryBarrier();

                    current_chunk.headPos_++;
                    return true;
                }
            }

            t = default(T);
            return false;
        }

        public bool IsEmpty
        {
            get
            {
                Chunk divider = divider_;
                Chunk tail = tail_chunk_;

                return (divider.nextChunk_ == tail || divider == tail) &&
                       tail.headPos_ > tail.tailPos_;
            }
        }
    }
}