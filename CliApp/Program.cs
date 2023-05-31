using CliApp.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Data.SqlClient;
using CliApp;
using CliApp.Models;
using CliApp.Services;

var ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TradeDB;User Id=sa;Password=123;Integrated Security=true; Encrypt=false;Connection Timeout=300";


using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

        services
            .AddTransient<IDbConnection>(sp => new SqlConnection(ConnectionString))
            .AddScoped(typeof(ITradeService<Trade>), typeof(TradeService))
            .AddScoped<ITestService, TestService>();
        //.AddHttpClient<ITradeService<Trade>>(client =>
        //  {
        //      client.BaseAddress = new Uri("http://localhost:5123");
        //  });
    })
    .Build();




Console.WriteLine("-----------------xxxxx---------------------\r\n");
Console.WriteLine("xxxxx Enter a number (or 'x' to exit) xxxxx\r\n");
Console.WriteLine("-------------------------------------------\r\n");

Guid requestId = Guid.NewGuid();

CancellationTokenSource cts = new();
CancellationToken token = cts.Token;

// Start a separate thread to listen for cancellation key press
ThreadPool.QueueUserWorkItem(_ =>
{
    if (Console.ReadKey().Key == ConsoleKey.Escape)
    {
        cts.Cancel(); // Cancel the token source if Escape key is pressed
        Console.WriteLine($"===>\tRequest [{requestId}] Cancled By User");
    }
});

while (!token.IsCancellationRequested)
{
    string input = Console.ReadLine();

    if (input.ToLower() == "x")
    {
        Console.WriteLine("===>\tGood By Press Key To Colse Application.\r\n");
        break;
    }

    if (!int.TryParse(input, out int number))
    {
        Console.WriteLine($"===>\tInvalid input");
        continue;
    }

    Console.WriteLine($"===>\tEntered Number Is : " + number);

    //await AppDi.DirectBulkInsertTrades(host, token, number);

    //var topTrades = await AppDi.DirectGetRangeTrades(host, number, token);
    //var ids = topTrades?.ToArray();

    //await AppDi.DirectGetTrade(host, ids[0], token);

    // await AppDi.DirectGetManyTrades(host, ids, token);

    //await AppDi.HttpClientGetTrade(ids[0], token);

    //await AppDi.HttpClientGetManyTrades(ids, token);




    //await AppDi.VeryBadAsyncSequentialRequests(ids, token);

    //await AppDi.BetterParallelAsyncRequests(ids, token);

    //await AppDi.MuchBetterParallelAsyncRequests(ids, token);

    //await AppDi.ParallelAsyncRequestsByAsyncForeEachWeakerThanSemaphoreSlime(ids, token);

    //await AppDi.BestParallelAsyncRequestsBySemaphoreSlim(ids, token);



    //Tricky Examples

    //await Examples.Use_IAsyncEnumerable_YieldReturn();

    //await Examples.Use_Foreach_Delay();

    //await Examples.Use_TaskYield_InLogRunningTasks();
    
    Examples.Use_DifferenceBetweenCreateTaskByRunOrFactory();



}

// Dispose the CancellationTokenSource
cts.Dispose();
Console.WriteLine("-----------------xxxxx---------------------\r\n");



await host.RunAsync();









async Task LongRunningOperationAsync()
{
    for (int i = 1; i <= 10; i++)
    {
        Console.WriteLine($"Long-running operation: {i}");
        // Yield to allow other tasks to run
        await Task.Yield();
    }
}

static void DoOtherWork() => Console.WriteLine($"Doing other work...");





