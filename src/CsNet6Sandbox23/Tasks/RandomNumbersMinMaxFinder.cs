using FluentAssertions;
using Xunit.Abstractions;

namespace CsNet6Sandbox23.RandomNumbersMinMaxFinder;

public class RandomNumbersMinMaxFinderUnitTest
{
    /*
    Write a program that generates a list of random numbers and finds the maximum and minimum values in parallel using tasks. 
    The program should divide the list into multiple partitions, and each partition should be processed by a separate task. 
    Once all tasks have completed, the program should determine the overall maximum and minimum values from the results.
    */


    ITestOutputHelper output;


    public RandomNumbersMinMaxFinderUnitTest(ITestOutputHelper output)
    {
        this.output = output;
    }


    [Fact]
    public async Task TestRun__RandomNumbersMinMaxFinder()
    {
        output.WriteLine("main start");

        true.Should().BeFalse();
    }
}
