using peach.allocators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peach {
    public class MemoryManager {

        internal byte[] _buffer;
        internal IAllocator _allocator;         // Internal only for testing

        // buffer is a large chunk of contiguous memory.
        // It is not necessary to pass in the buffer size as in C# this information is embedded in the array object
        public MemoryManager(byte[] buffer) {
            _buffer = buffer;

            _allocator = new BruteForceAllocator();

            _allocator.AddFreeChunk(new FreeChunk(0, buffer.Length));
        }

        // Allocate memory of size 'size'. Use malloc() like semantics.
        public MemoryChunk Alloc(int length) {
            FreeChunk free = _allocator.FindSmallestFreeChunk(length);
            if (free == null)
                throw new OutOfMemoryException();

            MemoryChunk chunk = new MemoryChunk(this, free.Offset, length);

            // If the free memory chunk was just the right size, we remove it
            if (free.Length == length)
                _allocator.RemoveFreeChunk(free);
            else {                               // Otherwise, we changs its offset to make it smaller
                // "Eat" the first part of the chunk, making it smaller
                free.Offset += length;
                free.Length -= length;
            }

            return chunk;
        }

        // Free up previously allocated memory.  Use free() like semantics.
        public void Free(MemoryChunk chunk) {
            FreeChunk left = _allocator.GetFreeChunkOnLeft(chunk);
            FreeChunk right = _allocator.GetFreeChunkOnRight(chunk);

            // There are four possible cases of the left/right free chunks being there.
            // We handle them each individualy
            if (left == null && right == null) {
                // Just create new free chunk - no need to combine
                _allocator.AddFreeChunk(new FreeChunk(chunk._offset, chunk.Length));
            } else if (left != null && right == null) {
                // extend free chunk on left to reclaim space
                left.Length += chunk.Length;
            } else if (left == null && right != null) {
                // extend free chunk on right to reclaim space
                right.Offset -= chunk.Length;
                right.Length += chunk.Length;
            } else {
                // Free chunks on both sides 
                // In this case, we extend the left free chunk to reclaim the space and also absorb the 
                // free chunk on the right, which is deleted

                left.Length += (chunk.Length + right.Length);
                _allocator.RemoveFreeChunk(right);
            }
        }
    };
}
