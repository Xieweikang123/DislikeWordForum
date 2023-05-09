using System;

class Program
{
    static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        // 启动一个异步任务
        var task = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine("Task is running...");
                await Task.Delay(1000);
            }
            token.ThrowIfCancellationRequested();
        }, token);

        // 等待一段时间后取消任务
        await Task.Delay(5000);
        cts.Cancel();
        Console.WriteLine("Canceled task.");

        // 等待任务完成
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task canceled.");
        }


        // Wait for user input
        Console.Write("Press any key to exit...");
        Console.ReadKey();
    }
}
