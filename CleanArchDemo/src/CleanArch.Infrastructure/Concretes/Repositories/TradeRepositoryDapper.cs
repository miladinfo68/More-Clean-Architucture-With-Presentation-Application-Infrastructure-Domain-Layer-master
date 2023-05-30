using System.Data;
using CleanArch.Common.Mapping;
using CleanArch.Domain.Abstractions.Interfaces.Repositories;
using CleanArch.Domain.DomainModels;
using Dapper;

namespace CleanArch.Infrastructure.Concretes.Repositories;


public interface ITradeRepositoryDapper : IBaseRepository<Trade>
{
}



public class TradeRepositoryDapper : BaseRepository<Trade>, ITradeRepositoryDapper
{
    public TradeRepositoryDapper(IDbConnection dbConnection) : base(dbConnection)
    {
    }

    public override async Task<IReadOnlyCollection<Trade>?> Get(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var conn = DbConnection;
        var lastTrades = (await conn.QueryAsync<Trade>("SP_GetLastTrades")).ToList();
        return lastTrades;
    }

    public async Task<IReadOnlyCollection<Trade>?> GetMany<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var conn = DbConnection;
        var tradIds = ids.ToList().ToDataTable().AsTableValuedParameter("Tvp_Trade");
        var lastTrades = (await conn.QueryAsync<Trade>("SP_GetLastTrades"
            , new { Id = 0, Tvp_Trade = tradIds }
            , null
            , null
            , CommandType.StoredProcedure
            )).ToList();
        return lastTrades;
    }

    public async Task<Trade?> Get<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var conn = DbConnection;
        var trade = await conn.QueryFirstOrDefaultAsync<Trade>("SP_GetLastTrades", new { Id = id });
        return trade;
    }








}


