namespace System.IO.Compression
{
    public class DeflateCompressionAlgorithm : ICompressionAlgorithm
    {
        public Stream CreateCompressStream(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Compress, true);
        }

        public Stream CreateDecompressStream(Stream stream)
        {
            return new DeflateStream(stream, CompressionMode.Decompress, true);
        }
    }
}
