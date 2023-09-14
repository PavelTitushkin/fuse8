using Fuse8_ByteMinds.SummerSchool.PublicApi.Abstractions;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Microsoft.Extensions.Options;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Services
{
    /// <summary>
    /// Сервис для работы с PublicApi
    /// </summary>
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly ICurrencyRepository _currencyRepository;
        public AppSettings AppSettings { get; }

        /// <summary>
        /// Сервис для работы с PublicApi
        /// </summary>
        /// <param name="options">Кофигурации приложения</param>
        /// <param name="httpCurrencyRepository"><inheritdoc cref="IHttpCurrencyRepository"/></param>
        /// <param name="currencyRepository"><inheritdoc cref="ICurrencyRepository"/></param>
        public CurrencyRateService(IOptions<AppSettings> options, ICurrencyRepository currencyRepository)
        {
            AppSettings = options.Value;
            _currencyRepository = currencyRepository;
        }

        public async Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken)
        {
            await _currencyRepository.ChangeDefaultCurrencyAsync(defaultCurrency.ToUpper(), cancellationToken);
        }

        public async Task ChangeCurrencyRoundAsync(int round, CancellationToken cancellationToken)
        {
            await _currencyRepository.ChangeCurrencyRoundAsync(round, cancellationToken);
        }

        public async Task<FavoriteCurrencyDTO> GetFavoriteCurrencyAsync(string currencyName, CancellationToken cancellationToken)
        {
            var dto = await _currencyRepository.GetFavoriteCurrencyByNameAsync(currencyName, cancellationToken);

            return dto != null ? dto : throw new ArgumentNullException(nameof(dto));
        }

        public async Task<List<FavoriteCurrencyDTO>> GetAllFavoritesCurrenciesAsync(CancellationToken cancellationToken)
        {
            var dto = await _currencyRepository.GetAllFavoritesCurrenciesAsync(cancellationToken);

            return dto != null ? dto : throw new ArgumentNullException(nameof(dto));
        }

        public async Task AddNewFavoriteCurrencyAsync(string currencyName, string currency, string currencyBase, CancellationToken cancellationToken)
        {
            var dto = new FavoriteCurrencyDTO
            {
                Name = currencyName,
                Currency = currency,
                BaseCurrency = currencyBase
            };

            await _currencyRepository.AddNewFavoriteCurrencyAsync(dto, cancellationToken);
        }

        public async Task ChangeFavoriteCurrencyByNameAsync(string currencyName, string changedCurrencyName, string changedCurrency, string changedCurrencyBase, CancellationToken cancellationToken)
        {
            var entity = await _currencyRepository.GetFavoriteCurrencyByNameAsync(currencyName, cancellationToken);
            if (entity != null)
            {
                entity.Name = changedCurrencyName;
                entity.Currency = changedCurrency;
                entity.BaseCurrency = changedCurrencyBase;

                await _currencyRepository.ChangeFavoriteCurrencyByNameAsync(currencyName, entity, cancellationToken);
            }
        }

        public async Task DeleteFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken)
        {
            await _currencyRepository.DeleteFavoriteCurrencyByNameAsync(currencyName, cancellationToken);
        }
    }
}
