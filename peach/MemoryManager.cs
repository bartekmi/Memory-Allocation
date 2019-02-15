using peach.allocators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peach {
    public class MemoryManager {

        internal byte[] _buffer;
        internal BaseChunk HeadForHeapWalking;
        internal IAllocator _allocator;         // Internal only for testing
        private int _memoryAllocated = 0;

        public double FractionAllocated {
            get {
                return (double)_memoryAllocated / (double)_buffer.Length;
            }
        }

        // buffer is a large chunk of contiguous memory.
        // It is not necessary to pass in the buffer size as in C# this information is embedded in the array object
        public MemoryManager(byte[] buffer) {
            _buffer = buffer;

            //_allocator = new BruteForceAllocator();
            _allocator = new SortedTreeAllocator();

            FreeChunk entireMemory = new FreeChunk(0, buffer.Length);
            HeadForHeapWalking = entireMemory;
            _allocator.AddFreeChunk(entireMemory);
        }

        // Allocate memory of size 'length'. Use malloc() like semantics.
        public AllocatedChunk Alloc(int length) {
            FreeChunk free = _allocator.FindSmallestFreeChunk(length);
            if (free == null)
                throw new OutOfMemoryException();

            AllocatedChunk chunk = new AllocatedChunk(this, free.Offset, length);

            // If the free memory chunk was just the right size, we remove it
            if (free.Length == length) {
                free.DoublyLinkedListReplaceSelf(chunk, this);
                _allocator.RemoveFreeChunk(free);
            } else {                               // Otherwise, we change its offset to make it smaller
                chunk.DoublyLinkedListInsertSelfBetween(free.Previous, free, this);

                // "Eat" the first part of the free chunk, making it smaller
                free.Offset += length;
                free.Length -= length;
            }

            _memoryAllocated += length;
            return chunk;
        }

        // Free up previously allocated memory.  Use free() like semantics.
        public void Free(AllocatedChunk chunk) {
            FreeChunk left = chunk.Previous as FreeChunk;
            FreeChunk right = chunk.Next as FreeChunk;

            chunk.DoublyLinkedListRemoveSelf(this);

            // There are four possible cases of the left/right free chunks being there.
            // We handle them each individualy
            if (left == null && right == null) {
                // Create new Free Chunk to replace the freed one
                FreeChunk free = new FreeChunk(chunk.Offset, chunk.Length);
                free.DoublyLinkedListInsertSelfBetween(chunk.Previous, chunk.Next, this);
                _allocator.AddFreeChunk(free);
            } else if (left != null && right == null) {
                // extend free chunk on left to reclaim space
                _allocator.AdjustChunkLength(left, chunk.Length);
            } else if (left == null && right != null) {
                // extend free chunk on right to reclaim space
                right.Offset -= chunk.Length;
                _allocator.AdjustChunkLength(right, chunk.Length);
            } else {
                // There are Free Chunks on both sides 
                // In this case, we extend the left Free Chunk to reclaim the space and also absorb the 
                // Free Chunk on the right, which is deleted
                _allocator.AdjustChunkLength(left, chunk.Length + right.Length);
                right.DoublyLinkedListRemoveSelf(this);
                _allocator.RemoveFreeChunk(right);
            }

            _memoryAllocated -= chunk.Length;
        }

        // Verify the internal integrity of the Memory Manager data structures.
        // An exception is thrown if any problems are detected
        internal void WalkTheHeap() {
            if (HeadForHeapWalking == null)
                throw new Exception("Head is null");

            // Iterate the linked list forward
            BaseChunk pointer = HeadForHeapWalking;
            while (pointer != null) {
                // Rules:
                // (1) Previous pointer of Next chunk must correctly point back to current chunk
                // (2) The consecutive chunks must have (a) no gaps and must (b) cover the entire buffer
                // (3) Free chunks should never be adjecent to each other - they should always have been merged
                // (4) No chunk length can ever be <= 0

                // Rule (1)
                if (pointer.Next != null)
                    if (pointer.Next.Previous != pointer)
                        throw new Exception("Doubly-Linked list is broken at " + pointer);

                // Rule (2)(a)
                if (pointer.Previous != null)
                    if (pointer.Previous.IndexAfter != pointer.Offset)
                        throw new Exception("Consecutive chunks do not touch: " + pointer.Previous + ", " + pointer);

                // Rule (2)(b)
                if (pointer.Next == null)       // This is the last chunk
                    if (pointer.IndexAfter != _buffer.Length)
                        throw new Exception("The last chunk does not align with the end of the buffer: " + pointer);

                // Rule (3)
                if (pointer.Previous != null)
                    if (pointer is FreeChunk && pointer.Previous is FreeChunk)
                        throw new Exception("There are two consecutive Free Chunks: " + pointer.Previous + " and " + pointer);

                // Rule (4)
                if (pointer.Length <= 0)
                    throw new Exception("Chunk Length has invalid value: " + pointer);

                pointer = pointer.Next;
            }
        }
    };
}
