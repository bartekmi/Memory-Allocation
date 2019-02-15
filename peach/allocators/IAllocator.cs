
namespace peach.allocators {
    internal interface IAllocator {
        void AddFreeChunk(FreeChunk chunk);
        FreeChunk FindSmallestFreeChunk(int minSize);
        void RemoveFreeChunk(FreeChunk chunk);
        void AdjustChunkLength(FreeChunk chunk, int delta);
    }

    internal interface IAllocatorUnitTests : IAllocator {
        int GetFreeChunkCount();
    }
}