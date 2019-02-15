using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peach {

    public class AllocatedChunk : BaseChunk {
        // Keeping a reference to the MemoryManager allows us to check for trying to free a chunk on the wrong MemoryManager
        private MemoryManager _manager;

        internal AllocatedChunk(MemoryManager manager, int offset, int length) : base(offset, length) { 
            _manager = manager;
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
            if (index < 0 || Offset + index >= _manager._buffer.Length)
                throw new IndexOutOfRangeException();
        }

        public override string ToString() {
            return string.Format("Offset = {0}, Length = {1}", Offset, Length);
        }
    }
}
