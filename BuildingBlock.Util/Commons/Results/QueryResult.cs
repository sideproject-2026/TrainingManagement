namespace BuildingBlock.Util.Commons.Results;

public class QueryResult<T>
{
    public QueryResult(IEnumerable<T> data, int pageCount)
    {
        Data = data;
        PageCount = pageCount;
    }
    public IEnumerable<T> Data { get; private set; }
    public int PageCount { get; private set; }
}
