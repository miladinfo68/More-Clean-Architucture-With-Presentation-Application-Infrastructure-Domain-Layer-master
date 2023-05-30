using CliApp.Models;
using Dapper;
using System.Data;

namespace CliApp.Services;

public class TradeService : ITradeService<Trade>
{
    private readonly IDbConnection _dbConnection;

    public TradeService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<int> BulkInsertTrades(CancellationToken cancellationToken, int totalRecords = 10_000_000, int chunkSize = 100_000)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rowAffected = -1;

        using var conn = _dbConnection;
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            rowAffected = await conn.ExecuteScalarAsync<int>("SP_BulkInsertTrades"
                , new { TotalRecords = totalRecords, ChunkSize = chunkSize }
                , commandType: CommandType.StoredProcedure
                , transaction: transaction
                , commandTimeout: 300 // Increase the command timeout value to 5 minutes (in seconds)
                );

            // Commit the transaction
            transaction.Commit();

            return rowAffected;

        }
        catch (Exception)
        {
            transaction.Rollback();
            //throw;
        }

        return rowAffected;
    }

    public async Task<IEnumerable<Trade>?> GetMany<TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<Trade> trades = Enumerable.Empty<Trade>();

        using var conn = _dbConnection;
        conn.Open();
        try
        {
            //var allParams = new DynamicParameters();
            //allParams.Add("Id",0);
            //allParams.Add("Tvp_Trade", ids.ToList().ToDataTable().AsTableValuedParameter("dbo.Tvp_Trade"));

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            ids.ToList().ForEach(x => dt.Rows.Add(int.Parse(x.ToString())));
            var tvp = dt.AsTableValuedParameter("Tvp_Trade");


            trades =(await conn.QueryAsync<Trade>("SP_GetLastTrades", new { Id = 0, Tvp_Trade = tvp },null,null,CommandType.StoredProcedure)).ToList();
        }
        catch (Exception)
        {
            // ignored
        }

        return trades;
    }


    public async Task<IEnumerable<int>?> GetRange<TKey>(int top = 10_000, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<int> trades = Enumerable.Empty<int>();

        using var conn = _dbConnection;
        conn.Open();
        try
        {
            trades = (await conn.QueryAsync<int>("SP_GetRangeTrades", new { top},null,null ,CommandType.StoredProcedure)).ToList();
        }
        catch (Exception)
        {
            // ignored
        }

        return trades;
    }




    public async Task<Trade?> Get<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Trade? trade = null;
        using var conn = _dbConnection;
        conn.Open();
        try
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            var tvp = dt.AsTableValuedParameter("Tvp_Trade");
            trade = await conn.QueryFirstOrDefaultAsync<Trade>("SP_GetLastTrades", new { Id = id });
        }
        catch (Exception)
        {
            // ignored
        }

        return trade;
    }



}
