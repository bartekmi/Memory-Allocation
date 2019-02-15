using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using peach.allocators;

namespace peach {
    [TestClass]
    public class MemoryManagerTest {

        private MemoryManager _manager = new MemoryManager(new byte[10]);

        [TestMethod]
        public void TestAllocateTooMuch() {
            // All at once
            try {
                _manager.Alloc(11);
                Assert.Fail("Should have throw exception");
            } catch (OutOfMemoryException) { }

            // In chunks
            _manager.Alloc(3);
            _manager.WalkTheHeap();
            _manager.Alloc(3);
            _manager.WalkTheHeap();
            _manager.Alloc(3);
            _manager.WalkTheHeap();

            try {
                _manager.Alloc(2);
                Assert.Fail("Should have throw exception");
            } catch (OutOfMemoryException) { }
        }

        [TestMethod]
        public void TestFreeNoRecombination() {
            AllocatedChunk c1 = _manager.Alloc(3);
            AllocatedChunk c2 = _manager.Alloc(3);
            AllocatedChunk c3 = _manager.Alloc(3);

            IAllocatorUnitTests allocator = (IAllocatorUnitTests)_manager._allocator;
            Assert.AreEqual(1, allocator.GetFreeChunkCount());

            // Freeing the middle chunk should just create another a new free chunk
            _manager.Free(c2);
            _manager.WalkTheHeap();
            Assert.AreEqual(2, allocator.GetFreeChunkCount());

            // Now, when we allocate a chunk of the same size, it should exactly fill the gap left by c2
            AllocatedChunk c2_second = _manager.Alloc(3);
            _manager.WalkTheHeap();
            Assert.AreEqual(1, allocator.GetFreeChunkCount());
            Assert.AreEqual(3, c2_second.Offset);
            Assert.AreEqual(3, c2_second.Length);
        }

        [TestMethod]
        public void TestFreeCombineWithFreeChunkOnLeft() {
            AllocatedChunk c1 = _manager.Alloc(3);
            AllocatedChunk c2 = _manager.Alloc(3);
            AllocatedChunk c3 = _manager.Alloc(3);

            IAllocatorUnitTests allocator = (IAllocatorUnitTests)_manager._allocator;
            Assert.AreEqual(1, allocator.GetFreeChunkCount());

            // First, free the first of the three
            _manager.Free(c1);
            _manager.WalkTheHeap();
            Assert.AreEqual(2, allocator.GetFreeChunkCount());

            // Freeing the second should cause the free chunk with 0 offset to be extended right
            _manager.Free(c2);
            _manager.WalkTheHeap();

            Assert.AreEqual(2, allocator.GetFreeChunkCount());
            FreeChunk reclaimed = c3.Previous as FreeChunk;
            Assert.AreEqual(0, reclaimed.Offset);
            Assert.AreEqual(6, reclaimed.Length);
        }

        [TestMethod]
        public void TestFreeCombineWithFreeChunkOnRight() {
            AllocatedChunk c1 = _manager.Alloc(3);
            AllocatedChunk c2 = _manager.Alloc(3);
            AllocatedChunk c3 = _manager.Alloc(3);

            IAllocatorUnitTests allocator = (IAllocatorUnitTests)_manager._allocator;
            Assert.AreEqual(1, allocator.GetFreeChunkCount());

            // Freeing the third should cause the single free chunk to be extended left
            _manager.Free(c3);
            _manager.WalkTheHeap();

            Assert.AreEqual(1, allocator.GetFreeChunkCount());
            FreeChunk reclaimed = c2.Next as FreeChunk;
            Assert.AreEqual(6, reclaimed.Offset);
            Assert.AreEqual(4, reclaimed.Length);
        }

        [TestMethod]
        public void TestFreeCombineWithFreeChunkOnLeftAndRight() {
            AllocatedChunk c1 = _manager.Alloc(3);
            AllocatedChunk c2 = _manager.Alloc(3);
            AllocatedChunk c3 = _manager.Alloc(3);

            IAllocatorUnitTests allocator = (IAllocatorUnitTests)_manager._allocator;
            Assert.AreEqual(1, allocator.GetFreeChunkCount());

            // First, free the first and third 
            _manager.Free(c1);
            _manager.WalkTheHeap();
            Assert.AreEqual(2, allocator.GetFreeChunkCount());
            _manager.Free(c3);
            _manager.WalkTheHeap();

            // Freeing the second should cause there to be only one free chunk
            _manager.Free(c2);
            _manager.WalkTheHeap();

            Assert.AreEqual(1, allocator.GetFreeChunkCount());
            _manager.Alloc(10);     // This proves that all memory has been reclaimed
            _manager.WalkTheHeap();
        }
    }
}