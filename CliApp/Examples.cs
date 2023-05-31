namespace CliApp;
public static class Examples
{


    public static void Use_DifferenceBetweenCreateTaskByRunOrFactory()
    {
        //create task by factory
        //Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current

        // create task by run method (behind the sceen)
        //Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

        CreateTaskFactoryStartNew();
        Console.WriteLine("----------------------------\rn");
        CreateTaskByRunMethod();
    }



    public static async Task Use_TaskYield_InLogRunningTasks()
    {
        var longRunningTask = LongRunningOperationAsync();
        DoOtherWork();
        // Wait for the long-running task to complete
        await longRunningTask;
    }

    public static async Task Use_IAsyncEnumerable_YieldReturn()
    {
        await foreach (var item in FetchIAsyncEnumerable())
            Console.WriteLine(item);
    }

    public static async Task Use_Foreach_Delay()
    {
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(1000);
            Console.WriteLine(i);
        }
    }






    private static void CreateTaskFactoryStartNew()
    {
        Task? innerTask = null;
        var outerTask = Task.Factory.StartNew(() =>
        {
            innerTask = new Task(() =>
            {
                //Thread.Sleep(3000);
                Console.WriteLine($"\tTaskId : {Task.CurrentId ?? -1} Inner Task Point A");

            }, TaskCreationOptions.AttachedToParent);

            Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task in Point B");
            Console.WriteLine($"\tTaskId : {Task.CurrentId ?? -1} Outer Task in Point AA");
            
            innerTask?.Start(TaskScheduler.Default);
            //Thread.Sleep(5000);
            Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task in Point C");
        });

        Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task Point D");
        Console.WriteLine($"\tTaskId : {outerTask?.Id ?? -1} Outer Task Point BB");

        outerTask?.Wait();

        Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task Point E IsCompleted: {innerTask?.IsCompleted ?? false}");
        Console.WriteLine($"\tTaskId : {outerTask?.Id ?? -1} Outer Task Point BBB IsCompleted: {outerTask?.IsCompleted ?? false}");

    }

    private static void CreateTaskByRunMethod()
    {
        Task? innerTask = null;
        var outerTask = Task.Run(() =>
        {
            innerTask = new Task(() =>
            {
                //Thread.Sleep(3000);
                Console.WriteLine($"\tTaskId : {Task.CurrentId ?? -1} Inner Task Point A");

            }, TaskCreationOptions.AttachedToParent);

            Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task in Point B");
            Console.WriteLine($"\tTaskId : {Task.CurrentId ?? -1} Outer Task in Point AA");

            innerTask?.Start(TaskScheduler.Default);
            //Thread.Sleep(5000);
            Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task in Point C");
        });

        Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task Point D");
        Console.WriteLine($"\tTaskId : {outerTask?.Id ?? -1} Outer Task Point BB");

        outerTask?.Wait();

        Console.WriteLine($"\tTaskId : {innerTask?.Id ?? -1} Inner Task Point E IsCompleted: {innerTask?.IsCompleted ?? false}");
        Console.WriteLine($"\tTaskId : {outerTask?.Id ?? -1} Outer Task Point BBB IsCompleted: {outerTask?.IsCompleted ?? false}");

    }

    private static void DoOtherWork() => Console.WriteLine($"Doing other work...");
    private static async Task LongRunningOperationAsync()
    {
        for (int i = 1; i <= 10; i++)
        {
            Console.WriteLine($"Long-running operation: {i}");
            // Yield to allow other tasks to run
            await Task.Yield();
        }
    }


    private static async IAsyncEnumerable<int> FetchIAsyncEnumerable()
    {
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(1000);
            yield return i;
        }
    }
}
