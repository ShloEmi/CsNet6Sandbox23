using FluentAssertions;
using System.Diagnostics;

namespace CsNet6Sandbox23;

public class TasksUnitTest
{
    private int tId;
    private readonly SemaphoreSlim taskUnderTestSignal = new(0, 1);
    private readonly SemaphoreSlim taskUnderTestSignals = new(0, 1);
    TimeSpan dTwait;

    public TasksUnitTest()
    {
        tId = -1;

        dTwait = Debugger.IsAttached ?
            TimeSpan.FromHours(1) :
            TimeSpan.FromMilliseconds(100);
    }

    async Task<int> TaskUnderTest()
    {
        tId = Environment.CurrentManagedThreadId;
        await taskUnderTestSignal.WaitAsync();
        taskUnderTestSignals.Release();
        ++tId;
        await taskUnderTestSignal.WaitAsync();
        taskUnderTestSignals.Release();
        return ++tId;
    }


    [Fact]
    public void TEST__startTaskUnderTest__RunHalfTask__ShouldPartialyRunTheTaskInSameThread()
    {
        tId.Should().Be(-1);

        // act
        _ = TaskUnderTest();


        tId.Should().Be(Environment.CurrentManagedThreadId);
    }

    [Fact]
    public async void TEST__startTaskUnderTest__RunHalfTask__ShouldPartialyRunTheTaskInSameThread2()
    {
        tId.Should().Be(-1);
        int ctId = Environment.CurrentManagedThreadId;

        Task<int> task = TaskUnderTest();

        // act
        taskUnderTestSignal.Release();
        await taskUnderTestSignals.WaitAsync();


        tId.Should().Be(ctId + 1);
    }

    [Fact]
    public async void TEST__startTaskUnderTest__RunHalfTask__ShouldPartialyRunTheTaskInSameThread3()
    {
        tId.Should().Be(-1);
        int ctId = Environment.CurrentManagedThreadId;
        Task<int> task = TaskUnderTest();


        // act
        taskUnderTestSignal.Release();
        await taskUnderTestSignals.WaitAsync();
        taskUnderTestSignal.Release();
        task.Wait(dTwait).Should().BeTrue();

        tId.Should().Be(ctId + 2);
    }
}
