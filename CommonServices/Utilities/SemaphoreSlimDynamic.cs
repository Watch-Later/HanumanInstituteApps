﻿using System;
using System.Threading;

namespace HanumanInstitute.CommonServices
{
    /// <summary>
    /// An extension of SemaphoreSlim that allows changing the capacity dynamically.
    /// </summary>
    public class SemaphoreSlimDynamic : SemaphoreSlim
    {
        private readonly ReaderWriterLockSlim _lock;

        /// <summary>
        /// Gets the current capacity of the Semaphore.
        /// </summary>
        public int CurrentSize { get; private set; }

        /// <summary>
        /// Gets the absolute maximum capacity of the Semaphore.
        /// </summary>
        public int MaximumCapacity { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreSlimDynamic"/> class.
        /// </summary>
        /// <param name="initialCount">The initial number of slots.</param>
        /// <param name="maxCount">The maximum number of slots.</param>
        public SemaphoreSlimDynamic(int initialCount, int maxCount)
            : base(initialCount, maxCount * 2)
        {
            // Note: Reducing capacity is done by invoking Wait() which may only complete when a semaphore is released.
            // This means that increasing it again may cause an overflow. To avoid this, we set the hard maximum to twice maxCount.

            if (initialCount < 1 || initialCount > maxCount) { throw new ArgumentOutOfRangeException(nameof(initialCount)); }

            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            CurrentSize = initialCount;
            MaximumCapacity = maxCount;
        }

        /// <summary>
        /// Changes the capacity of the SemaphoreSlim to specified size.
        /// </summary>
        /// <param name="newSize">The new capacity of the SemaphoreSlim.</param>
        public void ChangeCapacity(int newSize)
        {
            if (newSize < 1 || newSize > MaximumCapacity) { throw new ArgumentOutOfRangeException(nameof(newSize)); }

            var adjust = 0;
            if (newSize != CurrentSize)
            {
                _lock.EnterWriteLock();
                adjust = newSize - CurrentSize;
                CurrentSize = newSize;
                _lock.ExitWriteLock();
            }

            if (adjust > 0)
            {
                Release(adjust);
            }
            else if (adjust < 0)
            {
                for (var i = 0; i < adjust; i++)
                {
                    _ = WaitAsync();
                }
            }
        }
    }
}
