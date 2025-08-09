namespace Common.Libraries.USSD.XML
{
    public interface IxmlEntity { }
    public interface IXmlReader<T> where T : class, IxmlEntity
    {
        T? DeserializeXml(string xml);
        string SerializeXml(T obj);
    }
}