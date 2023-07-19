using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Fuse8_ByteMinds.SummerSchool.Domain;

//BenchmarkRunner.Run<StringInternBenchmark>();

BenchmarkRunner.Run<AccountProcessorBenchmark>();

[MemoryDiagnoser(displayGenColumns: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class StringInternBenchmark
{
    private readonly List<string> _words = new();
    public StringInternBenchmark()
    {
       foreach (var word in File.ReadLines(@".\SpellingDictionaries\ru_RU.dic"))
           _words.Add(string.Intern(word));
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(SampleData))]
    public bool WordIsExists(string word)
        => _words.Any(item => word.Equals(item, StringComparison.Ordinal));

    [Benchmark]
    [ArgumentsSource(nameof(SampleData))]
    public bool WordIsExistsIntern(string word)
    {
        var internedWord = string.Intern(word);
        return _words.Any(item => ReferenceEquals(internedWord, item));
    }

    public IEnumerable<string> SampleData()
    {
        yield return new StringBuilder().Append("зайц").Append("ем").ToString();
        yield return new StringBuilder().Append("первопроход").Append("цев").ToString();
        yield return new StringBuilder().Append("бронзирую").Append("тся").ToString();

        yield return new StringBuilder().Append("катава").Append("сия").ToString(); ;

        yield return "абвгд";
    }
    #region
    /*
     * |             Method |           word |       Mean |    Error |    StdDev |     Median | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
|------------------- |--------------- |-----------:|---------:|----------:|-----------:|------:|--------:|-----:|----------:|------------:|
| WordIsExistsIntern |          абвгд | 2,587.2 us | 50.89 us | 114.87 us | 2,553.4 us |  0.80 |    0.04 |    1 |     130 B |        1.00 |
|       WordIsExists |          абвгд | 3,327.3 us | 63.17 us |  56.00 us | 3,311.9 us |  1.00 |    0.00 |    2 |     130 B |        1.00 |
|                    |                |            |          |           |            |       |         |      |           |             |
| WordIsExistsIntern |   бронзируются | 2,493.4 us | 49.80 us | 124.01 us | 2,455.1 us |  0.72 |    0.06 |    1 |     130 B |        1.00 |
|       WordIsExists |   бронзируются | 3,465.7 us | 68.72 us | 161.97 us | 3,443.5 us |  1.00 |    0.00 |    2 |     130 B |        1.00 |
|                    |                |            |          |           |            |       |         |      |           |             |
| WordIsExistsIntern |         зайцем |   250.7 us |  3.03 us |   2.53 us |   250.2 us |  0.77 |    0.05 |    1 |     128 B |        1.00 |
|       WordIsExists |         зайцем |   320.6 us |  5.25 us |  12.88 us |   315.8 us |  1.00 |    0.00 |    2 |     128 B |        1.00 |
|                    |                |            |          |           |            |       |         |      |           |             |
| WordIsExistsIntern |      катавасия | 2,585.8 us | 38.18 us |  35.71 us | 2,580.0 us |  0.74 |    0.01 |    1 |     130 B |        0.98 |
|       WordIsExists |      катавасия | 3,481.7 us | 57.03 us |  50.56 us | 3,480.8 us |  1.00 |    0.00 |    2 |     132 B |        1.00 |
|                    |                |            |          |           |            |       |         |      |           |             |
| WordIsExistsIntern | первопроходцев | 1,433.0 us | 25.19 us |  22.33 us | 1,427.4 us |  0.72 |    0.03 |    1 |     129 B |        1.00 |
|       WordIsExists | первопроходцев | 2,033.1 us | 40.18 us |  67.13 us | 2,037.2 us |  1.00 |    0.00 |    2 |     129 B |        1.00 |
     */
    #endregion
}


[MemoryDiagnoser(displayGenColumns: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class AccountProcessorBenchmark
{
    AccountProcessor accountProcessor = new AccountProcessor();
    BankAccount bankAccount = new BankAccount();
    BankOperation bankAccountLastOperation = new BankOperation();
    BankOperation bankAccountPreviousOperation = new BankOperation();

    public AccountProcessorBenchmark()
    {
        bankAccountLastOperation.TotalAmount = 10;
        bankAccountLastOperation.OperationInfo0 = 10;
        bankAccountLastOperation.OperationInfo1 = 10;
        bankAccountLastOperation.OperationInfo2 = 10;

        bankAccountPreviousOperation.TotalAmount = 20;
        bankAccountPreviousOperation.OperationInfo0 = 20;
        bankAccountPreviousOperation.OperationInfo1 = 20;
        bankAccountPreviousOperation.OperationInfo2 = 20;

        bankAccount.TotalAmount = 0;
        bankAccount.LastOperation = bankAccountLastOperation;
        bankAccount.PreviousOperation = bankAccountPreviousOperation;
    }

    [Benchmark(Baseline = true)]
    public void CalculatePerformed()
    {
        accountProcessor.CalculatePerformed(ref bankAccount);
    }

    [Benchmark]
    public void Calculate()
    {
        accountProcessor.Calculate(bankAccount);
    }

    #region
    /*
     * |             Method |     Mean |     Error |    StdDev |   Median | Ratio | RatioSD | Rank |   Gen0 | Allocated | Alloc Ratio |
|------------------- |---------:|----------:|----------:|---------:|------:|--------:|-----:|-------:|----------:|------------:|
| CalculatePerformed | 1.487 us | 0.0298 us | 0.0799 us | 1.463 us |  1.00 |    0.00 |    1 | 4.2820 |   6.56 KB |        1.00 |
|          Calculate | 1.722 us | 0.0343 us | 0.0709 us | 1.701 us |  1.15 |    0.08 |    2 | 4.2820 |   6.56 KB |        1.00 |
     */
    #endregion
}