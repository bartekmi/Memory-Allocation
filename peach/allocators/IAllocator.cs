
namespace peach.allocators {
    internal interface IAllocator {
        void AddFreeChunk(FreeChunk chunk);
        FreeChunk FindSmallestFreeChunk(int minSize);
        void RemoveFreeChunk(FreeChunk chunk);

        // Return the free chunk adjecent on the left side, or null if none exists
        FreeChunk GetFreeChunkOnLeft(MemoryChunk allocated);

        // Return the free chunk adjecent on the right side, or null if none exists
        FreeChunk GetFreeChunkOnRight(MemoryChunk allocated);
    }

    internal interface IAllocatorUnitTests : IAllocator {
        int GetFreeChunkCount();
    }
}