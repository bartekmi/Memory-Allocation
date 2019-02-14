

internal class FreeChunk {
    internal int Offset;
    internal int Length;

    // Derived Properties
    internal int IndexAfter { get { return Offset + Length; } }  // Index just after this chunk

    internal FreeChunk(int offset, int length) {
        Offset = offset;
        Length = length;
    }
}
