using peach;
using System.Collections.Generic;
using System.Linq;

namespace peach.allocators {

    // The Brute-Force implementation has O(n) operations, where n is the number of Free Chunks.
    internal class BruteForceAllocator : IAllocatorUnitTests {

        private List<FreeChunk> _chunks = new List<FreeChunk>();

        public void AddFreeChunk(FreeChunk chunk) {
            _chunks.Add(chunk);
        }

        // This operation is O(N) in this brute-force implementation
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

        public void AdjustChunkLength(FreeChunk chunk, int delta) {
            chunk.Length += delta;
        }

        public int GetFreeChunkCount() {
            return _chunks.Count;
        }
    }
}