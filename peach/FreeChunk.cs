
using peach.helpers;

namespace peach {
    internal class FreeChunk : BaseChunk, ISortOrder {
        internal FreeChunk(int offset, int length) : base(offset, length) {
            // Do nothing
        }

        // Implements the ISortOrder interface and is needed by our Tree<T> implementation
        public int OrderBy { get { return Length; } }
    }
}