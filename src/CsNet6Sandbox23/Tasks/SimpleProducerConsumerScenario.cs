using Xunit.Abstractions;

namespace CsNet6Sandbox23.SimpleProducerConsumerScenario;

public class SimpleProducerConsumerScenarioUnitTest
{
    /*
    Write a C# program that uses a Monitor object to implement a simple producer-consumer scenario. 
    The program should use a thread-safe queue to hold the data, 
     with one thread adding items to the queue (the producer) and another thread removing items from the queue (the consumer). 
    The producer should wait when the queue is full, and the consumer should wait when the queue is empty. 
    Use the Monitor object to ensure that only one thread accesses the queue at a time.
    */

    static readonly int queueLimit = 100;
    Queue<int> queue = new(queueLimit);
    object queueLock = new();

    bool running = true;
    readonly AutoResetEvent queueFullSignal = new(false);
    readonly AutoResetEvent queueEmptySignal = new(false);
    readonly ManualResetEvent stopRunningSignal = new(false);

    Random randomProvider = new();

    private readonly ITestOutputHelper output;


    public SimpleProducerConsumerScenarioUnitTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    protected Task ProducerTask()
    {
        return new Task( () => {
            output.WriteLine("ProducerTask started");
            while (running) { 
                lock (queueLock)
                {
                    while (queue.Count < queueLimit)
                        queue.Enqueue(randomProvider.Next());
                }

                queueFullSignal.Set();
                WaitHandle.WaitAny(new WaitHandle[] { 
                    stopRunningSignal, 
                    queueEmptySignal 
                    });
            }
            output.WriteLine("ProducerTask ended");
        }, TaskCreationOptions.LongRunning);
    }

    protected Task ConsumerTask()
    {
        return new Task(() => {
            output.WriteLine("ConsumerTask started");
            while (running)
            {
                lock (queueLock)
                {
                    while (queue.Count > 0) 
                    {
                        int item = queue.Dequeue();
                    }
                }

                queueEmptySignal.Set();
                WaitHandle.WaitAny(new WaitHandle[] {
                    stopRunningSignal,
                    queueFullSignal
                    });
            }
            output.WriteLine("ConsumerTask ended");
        }, TaskCreationOptions.LongRunning);
    }


    [Fact]
    public async Task TEST__SimpleProducerConsumerScenario()
    {
        output.WriteLine("main start");

        var producerTask = ProducerTask();
        var consumerTask = ConsumerTask();

        producerTask.Start();
        consumerTask.Start();

        output.WriteLine("tasks started");

        await Task.Delay(250);

        running = false;
        stopRunningSignal.Set();
        output.WriteLine("running = false");

        Task.WaitAll(producerTask, consumerTask);
        output.WriteLine("main end");
    }
}
