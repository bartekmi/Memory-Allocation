using peach;
using System.Collections.Generic;
using System.Linq;

namespace peach.allocators {
    // Next step would be to create a more clever allocator which used a tree structure to allow binary-
    // searches for the sufficiently large chunk - O(N logN)
    internal class BruteForceAllocator : IAllocatorUnitTests {

        private List<FreeChunk> _chunks = new List<FreeChunk>();

        public void AddFreeChunk(FreeChunk chunk) {
            _chunks.Add(chunk);
        }

        public FreeChunk FindSmallestFreeChunk(int minLength) {
            FreeChunk smallest = null;

            foreach (FreeChunk chunk in _chunks)
                if (chunk.Length >= minLength &&                                // Must be big enough
                    (smallest == null || chunk.Length < smallest.Length))       // But we prefer smallest size available
                    smallest = chunk;

            return smallest;
        }

        // This operation is O(N) in this brute-force implementation
        public void RemoveFreeChunk(FreeChunk chunk) {
            _chunks.Remove(chunk);
        }

        // This operation is O(N) in this brute-force implementation
        public FreeChunk GetFreeChunkOnLeft(MemoryChunk allocated) {
            return _chunks.SingleOrDefault(x => x.IndexAfter == allocated._offset);
        }

        // This operation is O(N) in this brute-force implementation
        public FreeChunk GetFreeChunkOnRight(MemoryChunk allocated) {
            return _chunks.SingleOrDefault(x => x.Offset == allocated.IndexAfter);
        }

        public int GetFreeChunkCount() {
            return _chunks.Count;
        }
    }
}