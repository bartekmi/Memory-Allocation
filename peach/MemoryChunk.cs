using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peach {

    // By making MemoryChunk a struct instead of a class, we ensure that (in most cases) they are not allocated on the heap
    public struct MemoryChunk {
        // Keeping a reference to the MemoryManager allows us to check for trying to free a chunk on the wrong MemoryManager
        private MemoryManager _manager;

        internal int _offset;
        public int Length { get; private set; }

        // Derived Properties
        internal int IndexAfter { get { return _offset + Length; } }  // Index just after this chunk

        internal MemoryChunk(MemoryManager manager, int offset, int length) {
            _manager = manager;
            _offset = offset;
            Length = length;
        }

        public void Set(int index, byte b) {
            ValidateIndex(index);
            _manager._buffer[index] = b;
        }

        public byte Get(int index) {
            ValidateIndex(index);
            return _manager._buffer[index];
        }

        private void ValidateIndex(int index) {
            if (index < 0 || _offset + index >= _manager._buffer.Length)
                throw new IndexOutOfRangeException();
        }

        public override string ToString() {
            return string.Format("Offset = {0}, Length = {1}", _offset, Length);
        }
    }
}
