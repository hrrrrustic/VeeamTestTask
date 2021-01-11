namespace Core.Parsing
{
    public interface IParser<T>
    {
        T Parse(string[] values);
    }
}