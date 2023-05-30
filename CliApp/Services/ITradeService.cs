namespace CliApp.Services;

public interface ITradeService<T>: IMarkerService where T : class
{
    Task<int> BulkInsertTrades(CancellationToken token=default ,int totalRecords = 10_000_000, int @chunkSize = 100_000);
    public Task<IEnumerable<T>?> GetMany<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<IEnumerable<int>?> GetRange<TKey>(int top=10_000, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<T?> Get<TKey>(TKey id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

}