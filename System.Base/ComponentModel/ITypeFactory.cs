namespace System.ComponentModel
{
    public interface ITypeFactory
    {
        T Create<T>(params object[] args);
        
        object Create(string clsId, params object[] args);
    }
}
