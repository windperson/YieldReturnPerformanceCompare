using BenchmarkDotNet.Attributes;

namespace LoopYieldReturnBenchmarks;

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "RatioSD")]
public class LoopWithYieldReturn
{
    [Params(10, 100, 1_000, 5_000, 10_000)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int Count { get; set; }

    [Benchmark(Description = "Normal for Loop", Baseline = true)]
    public List<int> ProduceDecreaseNumbers()
    {
        var result = new List<int>(Count);
        for (var i = Count; i > 0; i--)
        {
            result.Add(i);
        }

        return result;
    }

    [Benchmark(Description = "Loop on Yield Return")]
    public List<int> YieldReturnProduceDecreaseNumbers()
    {
        var result = new List<int>(Count);
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var i in DecreaseNumberCoRoutine(Count))
        {
            result.Add(i);
        }

        return result;
    }

    [Benchmark(Description = "LINQ Range + Yield Return")]
    public List<int> YieldReturnProduceDecreaseNumbersWithLinqRang()
    {
        var result = new List<int>(Count);
        result.AddRange(DecreaseNumberCoRoutine(Count));

        return result;
    }

    [Benchmark(Description = "await foreach()")]
    public async Task<List<int>> YieldReturnProduceDecreaseNumbersAsync()
    {
        var result = new List<int>(Count);
        await foreach (var i in DecreaseNumberCoRoutineAsync(Count))
        {
            result.Add(i);
        }

        return result;
    }

    private static IEnumerable<int> DecreaseNumberCoRoutine(int count)
    {
        for (var i = count; i > 0; i--)
        {
            yield return i;
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CA1822
    private async IAsyncEnumerable<int> DecreaseNumberCoRoutineAsync(int count)
#pragma warning restore CA1822
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        for (var i = count; i > 0; i--)
        {
            yield return i;
        }
    }
}