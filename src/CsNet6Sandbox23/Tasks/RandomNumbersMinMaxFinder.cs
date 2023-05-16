using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
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
        output.WriteLine("started");
        Random randomProvider = new();

        int numbersCount = 1000000;
        var numbers = new List<int>(numbersCount);
        for (int i = 0; i < numbersCount; i++)
            numbers.Add(randomProvider.Next());

        try
        {
            (int min, int max) = await FindMinMax(numbers);
        }
        catch (AggregateException ex)
        {
            output.WriteLine("AggregateException:");
            foreach (var innerException in ex.InnerExceptions)
                output.WriteLine(innerException.Message);
        }
        catch (Exception ex)
        {
            output.WriteLine("Exception:");
            output.WriteLine(ex.Message);
        }

        // true.Should().BeFalse();
    }

    private async Task<(int min, int max)> FindMinMax(IReadOnlyList<int> numbers)
    {
        IEnumerable<int[]> chunks = numbers.Chunk(numbers.Count / 10);

        FieldInfo? field = typeof(List<int>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
        int[]? items = field?.GetValue(numbers) as int[];

        int chunkSize = numbers.Count / 10;
        int from = 0;
        int to = 10;

        // TBC..
        (int min, int max) qwe = await FindMinMax(items, from, to);

        return new (1,1);
    }

    private async Task<(int min, int max)> FindMinMax(int[]? numbers, int from, int to)
    {
        return new(1, 1);
    }
}
