using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peach {
    public abstract class BaseChunk {
        internal int Offset;
        internal int Length;

        // Both the Free and the Allocated Chunks are linked together in a doubly-linked list
        // to allow instant access to the predecessor and successor
        internal BaseChunk Next { get; set; }
        internal BaseChunk Previous { get; set; }

        // Derived Properties
        internal int IndexAfter { get { return Offset + Length; } }  // Index just after this chunk

        protected BaseChunk(int offset, int length) {
            Offset = offset;
            Length = length;
        }

        // Remove this chunk and join up the points on the left and right
        internal void DoublyLinkedListRemoveSelf(MemoryManager manager) {
            // Always maintain pointer to first chunk to enable "heap walking" to verify integrity
            if (Previous == null)
                manager.HeadForHeapWalking = this.Next;

            if (Previous != null)
                Previous.Next = Next;
            if (Next != null)
                Next.Previous = Previous;
        }

        // Insert this chunk between the two adjecent chunks specified in 'previous' and 'next'
        internal void DoublyLinkedListInsertSelfBetween(BaseChunk previous, BaseChunk next, MemoryManager manager) {
            // Always maintain pointer to first chunk to enable "heap walking" to verify integrity
            if (previous == null)
                manager.HeadForHeapWalking = this;

            if (previous != null)
                previous.Next = this;
            if (next != null)
                next.Previous = this;

            this.Previous = previous;
            this.Next = next;
        }

        // Replace this chunk with the new chunk, adjusting all pointers
        internal void DoublyLinkedListReplaceSelf(BaseChunk newChunk, MemoryManager manager) {
            // Always maintain pointer to first chunk to enable "heap walking" to verify integrity
            if (Previous == null)
                manager.HeadForHeapWalking = this;

            newChunk.Previous = Previous;
            newChunk.Next = Next;

            if (Previous != null)
                Previous.Next = newChunk;
            if (Next != null)
                Next.Previous = newChunk;
        }
    }
}
