namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories
{
    public interface ICurrencyRepository
    {
        Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken);
        Task ChangeCurrencyRoundAsync(int round, CancellationToken cancellationToken);
    }
}
