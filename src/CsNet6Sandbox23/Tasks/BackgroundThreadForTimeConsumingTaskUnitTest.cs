// #define PRODUCTION

using FluentAssertions;
using NSubstitute;

namespace CsNet6Sandbox23.BackgroundThreadForTimeConsumingTask;

public class BackgroundThreadForTimeConsumingTaskUnitTest
{
    /*
    Question 1 - Background thread for time-consuming task:

    Write a C# application that creates and starts a new thread to perform a time-consuming task in the background, 
     while the main thread continues executing. The time-consuming task should buffer numbers from 1 to 1000 with a delay of 5 
     milliseconds between each number. The main thread should buffer a message before and after when ending the time-consuming task.

    Please provide your solution to the problem.
    */

    public interface IDelayProvider
    {
        Task Delay(TimeSpan delay, CancellationToken cancellationToken);
    }

#if PRODUCTION
    private class DelayProvider : IDelayProvider
    {
        public async Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        {
            await Task.Delay(delay, cancellationToken);
        }
    }
#endif

    static readonly string startMsg = "starting";
    static readonly string endMsg = "ending";
    static readonly int count = 1000;




    public BackgroundThreadForTimeConsumingTaskUnitTest()
    {
    }


    [Fact]
    public async Task TestRun__BackgroundThreadForTimeConsumingTask()
    {
        CancellationTokenSource cts = new();
        List<string> output;

#if !PRODUCTION
        IDelayProvider delayProvider = Substitute.For<IDelayProvider>();

        delayProvider
            .Delay(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
#else
        IDelayProvider delayProvider = new DelayProvider();
#endif



        try
        {
            output = new List<string>();
        }
        catch (Exception)
        {
            // log ex.Message
            return;
        };


        try
        {
            output.Add(startMsg);

            using (cts)
                await DoWorkAsync(output, delayProvider, cts.Token);

            output.Add(endMsg);
        }
        catch (AggregateException ex)
        {
            // TODO: test task Cancellation
            foreach (var innerException in ex.InnerExceptions)
                output.Add(innerException.Message);

            return;
        }
        catch (Exception ex)
        {
            output.Add(ex.Message);
        };




        // assert
        output[0].Should().Be(startMsg);

        for (int i = 1; i <= count; i++)
            output[i].Should().Be(i.ToString());

        output[^1].Should().Be(endMsg);
    }

    private static async Task DoWorkAsync(List<string> output, IDelayProvider delayProvider, CancellationToken token)
    {
        TimeSpan delay = TimeSpan.FromMilliseconds(5);

        for (int i = 1; i <= count; i++)
        {
            output.Add(i.ToString());

            token.ThrowIfCancellationRequested();
            await delayProvider.Delay(delay, token);
        }
    }
}
