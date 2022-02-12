namespace System.IO.Compression
{
    public interface ICompressionAlgorithm
    {
        Stream CreateCompressStream(Stream stream);
        Stream CreateDecompressStream(Stream stream);
    }
}
