namespace System.Geometries
{
    public class NotRepresentableException : ApplicationException
    {
        public NotRepresentableException()
            : base("Projective point not representable on the Cartesian plane.")
        {
        }
    }
}
