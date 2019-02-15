using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using peach.allocators;

namespace peach {
    [TestClass]
    public class MemoryManagerTortureTest {

        private MemoryManager _manager = new MemoryManager(new byte[1000]);
        private Random _random = new Random(0);     // Set a seed for reproducability
        private int[] ALLOC_SIZES = new int[] { 10, 20, 50 };   

        [TestMethod]
        public void Torture() {
            HashSet<AllocatedChunk> allocated = new HashSet<AllocatedChunk>();

            // Allocate then de-allocate memory
            for (int ii = 0; ii < 10; ii++) {
                Allocate(0.9, allocated, ii);
                Deallocate(0.2, allocated, ii);
            }

            // Finally, de-allocate everything
            Deallocate(0.0, allocated, 10);

            // Verify that all memory has consolidated
            IAllocatorUnitTests allocator = (IAllocatorUnitTests)_manager._allocator;
            Assert.AreEqual(1, allocator.GetFreeChunkCount());
        }

        // Keep allocating memory until we reach the fraction desired
        private void Allocate(double fraction, ICollection<AllocatedChunk> allocated, int iteration) {
            while (_manager.FractionAllocated < fraction) {
                int length = ALLOC_SIZES[_random.Next(ALLOC_SIZES.Length)];
                allocated.Add(_manager.Alloc(length));
                _manager.WalkTheHeap();
            }
        }

        private void Deallocate(double fraction, ICollection<AllocatedChunk> allocated, int iteration) {
            while (_manager.FractionAllocated > fraction) {
                AllocatedChunk randomChunk = allocated.First();
                allocated.Remove(randomChunk);
                _manager.Free(randomChunk);
                _manager.WalkTheHeap();
            }
        }
    }
}