using FluentAssertions;

namespace CsNet6Sandbox23.SharedVariableIncrementation;

public class SharedVariableIncrementationUnitTest
{
    /*
        Write a C# program that creates two threads and uses a shared variable to increment it in both threads simultaneously. 
        Use the Interlocked class to perform atomic increments of the shared variable. 
        Print the final value of the shared variable after both threads have finished executing.     
    */

    int sharedCount;


    [Fact]
    public void TestRun__SharedVariableIncrementation()
    {
        CancellationTokenSource tcs = new();
        Task[] tasks = new Task[2] ;

        for (int i = 0; i < 2; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                await Task.Delay(100);
                Interlocked.Increment(ref sharedCount);
            }, tcs.Token);
        }

        Task.WaitAll(tasks, tcs.Token);

        sharedCount.Should().Be(2);
    }
}
