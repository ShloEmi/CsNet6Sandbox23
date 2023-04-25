using FluentAssertions;

namespace CsNet6Sandbox23;

public class TasksUnitTest
{
    private int tId;
    private readonly SemaphoreSlim taskUnderTestSignal = new(0, 1);

    public TasksUnitTest() => 
        tId = -1;


    async Task<int> TaskUnderTest()
    {
        tId = Environment.CurrentManagedThreadId;
        await taskUnderTestSignal.WaitAsync();
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
        await task;


        tId.Should().Be(ctId + 1);
    }
}
