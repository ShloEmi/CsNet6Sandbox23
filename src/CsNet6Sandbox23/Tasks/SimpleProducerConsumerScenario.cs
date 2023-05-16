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

    private readonly AutoResetEvent queueFullSignal = new(false);
    private readonly AutoResetEvent queueEmptySignal = new(false);
    private readonly Random randomProvider = new();
    
    private readonly ITestOutputHelper output;
    private readonly CancellationTokenSource cts = new();


    public SimpleProducerConsumerScenarioUnitTest(ITestOutputHelper output)
    {
        this.output = output;
    }


    protected void ProducerJob(CancellationToken ct)
    {
        output.WriteLine("ProducerJob started");
        while (ct.IsCancellationRequested == false) { 
            lock (queueLock)
            {
                while (queue.Count < queueLimit)
                    queue.Enqueue(randomProvider.Next());
            }

            queueFullSignal.Set();

            WaitHandle.WaitAny(new WaitHandle[] {
                ct.WaitHandle,
                queueEmptySignal 
                });
        }
        output.WriteLine("ProducerJob ended");
    }

    protected void ConsumerJob(CancellationToken ct)
    {
        output.WriteLine("ConsumerJob started");
        while (ct.IsCancellationRequested == false)
        {
            lock (queueLock)
            {
                while (queue.Count > 0) 
                {
                    int item = queue.Dequeue();
                    // do item work here.. :)
                }
            }

            queueEmptySignal.Set();
            
            WaitHandle.WaitAny(new WaitHandle[] {
                ct.WaitHandle,
                queueFullSignal
                });
        }
        output.WriteLine("ConsumerJob ended");
    }


    [Fact]
    public async Task TEST__SimpleProducerConsumerScenario()
    {
        output.WriteLine("main start");

        var producerTask = Task.Factory.StartNew(() => ProducerJob(cts.Token), TaskCreationOptions.LongRunning);
        var consumerTask = Task.Factory.StartNew(() => ConsumerJob(cts.Token), TaskCreationOptions.LongRunning);

        output.WriteLine("tasks started");

        await Task.Delay(250);

        cts.Cancel();

        output.WriteLine("running = false");

        Task.WaitAll(producerTask, consumerTask);
        output.WriteLine("main end");
    }
}
