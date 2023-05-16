using FluentAssertions;
using System.Collections.Concurrent;

namespace CsNet6Sandbox23.ProducerConsumerQueue;

public class ProducerConsumerQueueUnitTest
{
    /*
    Write a C# program that simulates a producer-consumer scenario using a thread-safe queue. 
    The program should create one or more producer threads that generate random numbers between 1 and 1000 and add them to the queue, 
        and one or more consumer threads that remove numbers from the queue and calculate their square root. 
    The program should terminate when the producers have generated 10,000 numbers and the consumers have processed them all. 
    Use a CancellationToken to signal the threads to stop gracefully.
    */

    // TD: WIP.. pause due to - too long question.. it's a task, not an interview question :)

    int produceWorkLeft = 10000;
    int producerBatchSize = 101;
    int producersTasksCount = 3;
    CancellationTokenSource cts = new();


    //[Fact]
    public async Task TEST__ProducerConsumerQueueAsync()
    {
        BlockingCollection<int> bc = new ();

        Task[] producerTasks = new Task[producersTasksCount];

        for (int i = 0; i < producersTasksCount; i++)
        {
            producerTasks[i] = ProducerTask(bc, cts.Token);
        }
    }


    protected Task ProducerTask(BlockingCollection<int> bc, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return Task.FromCanceled(ct);

        var randomProvider = new Random();

        bool running = true;
        while (running)
        {
            if (ct.IsCancellationRequested)
                return Task.FromCanceled(ct);


            int workLeft = Interlocked.Add(ref produceWorkLeft, producerBatchSize);
            if (workLeft + producerBatchSize <= 0 )
                return Task.CompletedTask;

            int workSize = workLeft + producerBatchSize;
            for (; workSize > 0 ; workSize--)
            {
                bc.Add(randomProvider.Next(), ct);
            }
        }

        return Task.CompletedTask;
    }
}
