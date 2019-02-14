using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace peach {
    [TestClass]
    public class MemoryChunkTest {

        private MemoryManager _manager = new MemoryManager(new byte[10]);

        [TestMethod]
        public void TestSetGet() {
            MemoryChunk chunk = new MemoryChunk(_manager, 1, 3);

            // First element
            chunk.Set(0, 77);
            Assert.AreEqual(77, chunk.Get(0));

            // Last element
            chunk.Set(2, 78);
            Assert.AreEqual(78, chunk.Get(2));
        }

        [TestMethod]
        public void TestSetGetOutOfRange() {
            MemoryChunk chunk = new MemoryChunk(_manager, 1, 3);

            // Set
            try {
                chunk.Set(-1, 77);
                Assert.Fail("Should have thrown exception");
            } catch { }

            try {
                chunk.Set(3, 77);
                Assert.Fail("Should have thrown exception");
            } catch { }

            // SGet
            try {
                chunk.Get(-1);
                Assert.Fail("Should have thrown exception");
            } catch { }

            try {
                chunk.Get(3);
                Assert.Fail("Should have thrown exception");
            } catch { }
        }
    }
}
