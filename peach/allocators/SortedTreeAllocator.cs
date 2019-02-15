using peach.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peach.allocators {
    internal class SortedTreeAllocator : IAllocatorUnitTests {

        Tree<FreeChunk> _tree = new Tree<FreeChunk>();

        public void AddFreeChunk(FreeChunk chunk) {
            _tree.Insert(chunk);
        }

        public FreeChunk FindSmallestFreeChunk(int minSize) {
            return _tree.FindItemWithMinSize(minSize);
        }

        public void RemoveFreeChunk(FreeChunk chunk) {
            _tree.Delete(chunk);
        }

        public void AdjustChunkLength(FreeChunk chunk, int delta) {
            // When adjusting chunk length, it is necessary to first remove and then add the chunk to the tree.
            // The position of the chunk in the tree depends on the sort key, the Length.
            _tree.Delete(chunk);
            chunk.Length += delta;
            _tree.Insert(chunk);
        }

        public int GetFreeChunkCount() {
            return _tree.Count;
        }
    }
}
