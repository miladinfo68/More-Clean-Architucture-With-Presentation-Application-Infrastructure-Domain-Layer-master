using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Text;
using CliApp.Models;
using CliApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace CliApp.IoC;

public static class AppDi
{
    private const string ServerBaseUrl = "http://localhost:5123";
    private const int BatchSize = 100;

    private static void MessageLogger(int affectedRows = 0, string message = "")
    {
        if (affectedRows > 0)
        {
            Console.WriteLine($"===>\t{affectedRows} {message}\r\n");
        }
    }

    private static async Task<Trade?> ProcessHttpCallTradeWithSemaphoreSlim(int id, SemaphoreSlim semaphore, CancellationToken cancellationToken)
    {
        try
        {
            return await HttpCallTrade(id, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }


    private static async Task<Trade?> HttpCallTrade(int id, CancellationToken cancellationToken)
    {
        Trade? trade = default;
        using var client = new HttpClient();
        var response = await client.GetAsync($"{ServerBaseUrl}/api/Trade/GetOne/{id}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!string.IsNullOrEmpty(content))
            {
                trade = JsonConvert.DeserializeObject<Trade>(content);
            }
        }
        return trade;

    }

    private static async Task<List<Trade>?> HttpCallRangeTrades(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        List<Trade> trades = new();
        using var client = new HttpClient();
        HttpContent body = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{ServerBaseUrl}/api/Trade/GetMany", body, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!string.IsNullOrEmpty(content))
            {
                trades.AddRange(JsonConvert.DeserializeObject<IEnumerable<Trade>>(content));
            }
        }
        return trades;
    }



    //public static (TService, IDbConnection, TUService) GetTargetServices<TService,TUService>(IHost host)
    public static (TService, IDbConnection) GetTargetServices<TService>(IHost host)
    {
        using var serviceScope = host.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var targetService = serviceProvider.GetRequiredService<TService>();
        var dbConnection = serviceProvider.GetRequiredService<IDbConnection>();
        //var otherService = serviceProvider.GetRequiredService<TUService>();

        return (targetService, dbConnection);
    }


    public static async ValueTask DirectBulkInsertTrades(IHost host, CancellationToken cancellationToken = default, int recordNumbers = 100000)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();

        var affectedRows = 0;
        if (recordNumbers > 0)
        {
            await using var serviceScope = host.Services.CreateAsyncScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var tradeService = serviceProvider.GetRequiredService<ITradeService<Trade>>();

            //var (tradeService , dbConnection) = GetTargetServices<ITradeService<Trade>>(host);
            affectedRows = await tradeService.BulkInsertTrades(cancellationToken, recordNumbers);
        }


        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(affectedRows, $"row('s) inserted [{timeTaken}] ms");
    }


    public static async ValueTask<IEnumerable<int>> DirectGetRangeTrades(IHost host, int top = 10_000, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IEnumerable<int> ids = Enumerable.Empty<int>();
        var timer = new Stopwatch();
        timer.Start();

        var rowCounts = 0;
        if (top > 0)
        {
            await using var serviceScope = host.Services.CreateAsyncScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var tradeService = serviceProvider.GetRequiredService<ITradeService<Trade>>();

            //var (tradeService , dbConnection) = GetTargetServices<ITradeService<Trade>>(host);

            ids = await tradeService.GetRange<int>(top, cancellationToken);
            rowCounts = ids?.Count() ?? 0;
        }


        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched in [{timeTaken}] ms");
        return ids;

    }

    public static async ValueTask DirectGetManyTrades(IHost host, IEnumerable<int>? ids = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();

        var rowCounts = 0;
        if (ids != null && ids.Any())
        {
            await using var serviceScope = host.Services.CreateAsyncScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var tradeService = serviceProvider.GetRequiredService<ITradeService<Trade>>();

            //var (tradeService , dbConnection) = GetTargetServices<ITradeService<Trade>>(host);

            var trades = await tradeService.GetMany(ids, cancellationToken);
            rowCounts = trades?.Count() ?? 0;
        }


        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched in [{timeTaken}] ms");
    }


    public static async ValueTask DirectGetTrade(IHost host, int id = 0, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();

        var rowCounts = 1;
        if (id > 0)
        {
            await using var serviceScope = host.Services.CreateAsyncScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var tradeService = serviceProvider.GetRequiredService<ITradeService<Trade>>();

            //var (tradeService , dbConnection) = GetTargetServices<ITradeService<Trade>>(host);
            var trade = await tradeService.Get(id, cancellationToken);
            rowCounts = trade is null ? 0 : 1;
        }


        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched in [{timeTaken}] ms");
    }


    //------------------------------- Rest Client

    public static async ValueTask HttpClientGetManyTrades(IEnumerable<int>? ids = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();

        List<Trade> trades = null;
        if (ids != null && ids.Any())
        {
            trades = await HttpCallRangeTrades(ids, cancellationToken);
        }
        var rowCounts = trades != null && trades.Any() ? trades.Count : 0;

        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched by http-client in [{timeTaken}] ms");
    }



    public static async ValueTask HttpClientGetTrade(int id = 0, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();

        var rowCounts = 0;
        if (id > 0)
        {
            rowCounts = await HttpCallTrade(id, cancellationToken) != null ? 1 : 0;
        }
        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched by http-client in [{timeTaken}] ms");
    }




    public static async Task VeryBadAsyncSequentialRequests(IEnumerable<int>? ids = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();

        var rowCounts = 0;

        List<Trade> trades = new();
        if (ids != null && ids.Any())
        {
            foreach (var id in ids)
            {
                Trade? trade = await HttpCallTrade(id, cancellationToken);
                if (trade != null)
                {
                    trades.Add(trade);
                    ++rowCounts;
                }
            }
        }
        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched by http-client as none-parallel in [{timeTaken}] ms");
    }

    public static async Task BetterParallelAsyncRequests(IEnumerable<int>? ids = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();
        var rowCounts = 0;
        if (ids != null && ids.Any())
        {
            var tasks = ids?.Select(id => HttpCallTrade(id, cancellationToken));
            var trades = await Task.WhenAll(tasks);
            rowCounts = trades?.Length ?? 0;
        }

        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched by http-client as parallel in [{timeTaken}] ms");
    }

    public static async Task MuchBetterParallelAsyncRequests(IEnumerable<int>? ids = default, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var timer = new Stopwatch();
        timer.Start();
        var rowCounts = 0;
        //var taskOfTrades = new List<Task<Trade?>>();
        if (ids != null && ids.Any())
        {
            int numberOfBatches = (int)Math.Ceiling((double)ids.Count() / BatchSize);
            var i = 0;
            while (i < numberOfBatches)
            {
                var batchIds = ids.Skip(i * BatchSize).Take(BatchSize);
                var tasks = batchIds?.Select(id => HttpCallTrade(id, cancellationToken));
                //taskOfTrades.AddRange(tasks);
                var trades = await Task.WhenAll(tasks);
                rowCounts += trades?.Length ?? 0;
                ++i;
            }
        }
        //var trades = await Task.WhenAll(taskOfTrades);
        //rowCounts = trades?.Length ?? 0;
        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched by http-client as parallel (used batch-size) in [{timeTaken}] ms");
    }


    public static async Task BestParallelAsyncRequestsBySemaphoreSlim(IEnumerable<int>? ids = default, CancellationToken cancellationToken = default)
    {

        var timer = new Stopwatch();
        timer.Start();
        var rowCounts = 0;

        if (ids != null && ids.Any())
        {

            // Limit the number of concurrent HTTP calls
            int maxConcurrentCalls = 100; // set batch-size
            var semaphore = new SemaphoreSlim(maxConcurrentCalls);

            var tasks = new List<Task<Trade?>>();

            foreach (var id in ids)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Wait for an available slot before starting a new HTTP call
                await semaphore.WaitAsync(cancellationToken);

                tasks.Add(ProcessHttpCallTradeWithSemaphoreSlim(id, semaphore, cancellationToken));

                //tasks.Add(
                //    HttpCallTrade(id, cancellationToken).ContinueWith(t =>
                //    {
                //        semaphore.Release();
                //        return t.Result;
                //    }, cancellationToken)
                //);
            }

            var trades = await Task.WhenAll(tasks);
            rowCounts += trades?.Length ?? 0;
        }

        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched by http-client as parallel (used semaphoreslim) in [{timeTaken}] ms");
    }

    public static async Task ParallelAsyncRequestsByAsyncForeEachWeakerThanSemaphoreSlime(IEnumerable<int>? ids = default, CancellationToken cancellationToken = default)
    {

        var timer = new Stopwatch();
        timer.Start();
        var rowCounts = 0;
        var trades = new ConcurrentBag<Trade>();
        if (ids != null && ids.Any())
        {
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            await Parallel.ForEachAsync(ids, parallelOptions, async (id, token) =>
            {
                var trade = await HttpCallTrade(id, token);
                if (trade != null)
                {
                    trades.Add(trade);
                }
            });
        }
        rowCounts = trades?.Count ?? 0;
        var timeTaken = timer.ElapsedMilliseconds;
        timer.Reset();

        MessageLogger(rowCounts, $"row('s) fetched by http-client as parallel (used from Parallel.ForEachAsync) in [{timeTaken}] ms");
    }




    //-------------------------------

    // Test Service Methods

    public static void SayHello(IHost host)
    {
        using var serviceScope = host.Services.CreateAsyncScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var testService = serviceProvider.GetRequiredService<ITestService>();

        testService.SayHello();
    }



}