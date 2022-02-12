namespace System.IO.Compression
{
    public class GZipCompressionAlgorithm : ICompressionAlgorithm
    {
        public Stream CreateCompressStream(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Compress);
        }

        public Stream CreateDecompressStream(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }
    }
}
