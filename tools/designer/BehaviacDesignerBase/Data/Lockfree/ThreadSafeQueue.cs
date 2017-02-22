using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Concurrent
{
    /// <summary>
    /// A thread-safe reader-writer-locked wrapper to the stock .NET Queue container.
    /// </summary>
    /// <typeparam name="TValue">The type of the classes that will be queued in this container.</typeparam>
    /// <remarks>All operations are exception checked, so it will return default values rather than crash when used improperly.</remarks>
    public class ThreadSafeQueue<TValue>
    {
        /// <summary>The key to protecting the contained queue properly</summary>
        private ReaderWriterLock AccessLock = new ReaderWriterLock();

        /// <summary>The protected queue</summary>
        private Queue<TValue> ProtectedQueue = new Queue<TValue>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ThreadSafeQueue()
        {
        }

        /// <summary>
        /// Completely empties the queue.
        /// </summary>
        public void Clear()
        {
            // Modifies the collection, use a writer lock
            AccessLock.AcquireWriterLock(Timeout.Infinite);

            try
            {
                ProtectedQueue.Clear();
            }

            finally
            {
                AccessLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Queue up a new element.
        /// </summary>
        /// <param name="V">A new element to queue.</param>
        public void Enqueue(TValue V)
        {
            // Modifies the collection, use a writer lock
            AccessLock.AcquireWriterLock(Timeout.Infinite);

            try
            {
                ProtectedQueue.Enqueue(V);
            }

            finally
            {
                AccessLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Dequeue an existing element.
        /// </summary>
        /// <returns>A previously queued element.</returns>
        public TValue Dequeue()
        {
            // Modifies the collection, use a writer lock
            AccessLock.AcquireWriterLock(Timeout.Infinite);
            TValue V = default(TValue);

            try
            {
                V = ProtectedQueue.Dequeue();
            }

            finally
            {
                AccessLock.ReleaseWriterLock();
            }

            return V;
        }

        /// <summary>
        /// Return all elements of the queue as an array.
        /// </summary>
        /// <returns>An array of all elements in the queue.</returns>
        public TValue[] ToArray()
        {
            // Does not modify the collection, use a reader lock
            AccessLock.AcquireReaderLock(Timeout.Infinite);
            TValue[] ReturnValues;

            try
            {
                ReturnValues = ProtectedQueue.ToArray();
            }

            finally
            {
                AccessLock.ReleaseReaderLock();
            }

            return ReturnValues;
        }

        /// <summary>
        /// Return the number of elements in the queue.
        /// </summary>
        /// <returns>A count of elements in the queue.</returns>
        public int Count
        {
            get
            {
                // Does not modify the collection, use a reader lock
                AccessLock.AcquireReaderLock(Timeout.Infinite);
                int ReturnValue = 0;

                try
                {
                    ReturnValue = ProtectedQueue.Count;
                }
                finally
                {
                    AccessLock.ReleaseReaderLock();
                }

                return ReturnValue;
            }
        }
    }
}