using FluentAssertions;

namespace CsNet6Sandbox23.TaskEnterWait;

public class TaskEnterWaitUnitTest
{
    /*
    Write a function, that each task that enters it, waits until a fourth task enters and releases them all.
    */

    int waiting = 0;
    ManualResetEvent gate = new(false);

    /* CR REMARKS: 
     *  this code is not thread safe! 
     *  see _semaphore.WaitAsync code as good solution reference
     */

    private void TaskEnterWaitAsync()
    {
        Task.Delay(50).Wait();

        int currentWaiting = Interlocked.Increment(ref waiting);
        if (currentWaiting % 4 == 0)
        {
            gate.Set();
            return;
        }

        gate.WaitOne();
    }


    [Fact]
    public void TEST__TaskEnterWait()
    {
        List<Task> tasks = new();

        for (int i = 0; i < 8; i++)
            tasks.Add(Task.Run(TaskEnterWaitAsync));

        Task.WaitAll(tasks.ToArray());
        waiting.Should().Be(8);
    }
}
