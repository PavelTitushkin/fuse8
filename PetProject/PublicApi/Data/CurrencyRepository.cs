using AutoMapper;
using DataStore.PublicApiDb;
using DataStore.PublicApiDb.Entities;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.DTO;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Data
{
    /// <summary>
    /// Класс для работы с данными
    /// </summary>
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly PublicApiContext _publicApiContext;
        private readonly IMapper _mapper;

        public AppSettings AppSettings { get; set; }

        /// <summary>
        /// <inheritdoc cref="CurrencyRepository"/>
        /// </summary>
        /// <param name="context">Контекст БД</param>
        /// <param name="options">Кофигурации приложения</param>
        /// <param name="mapper">Маппер</param>
        public CurrencyRepository(PublicApiContext context, IOptions<AppSettings> options, IMapper mapper)
        {
            _publicApiContext = context;
            AppSettings = options.Value;
            _mapper = mapper;
        }

        public async Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken)
        {
            var entity = await _publicApiContext.PublicApiSettings.Where(e => e.Id == 1).FirstOrDefaultAsync(cancellationToken);
            if (entity != null)
            {
                entity.DefaultCurrency = defaultCurrency;
                _publicApiContext.Update(entity);
                await _publicApiContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                await AddDefaultCurrencyAsync(defaultCurrency, cancellationToken);
            }
        }

        public async Task ChangeCurrencyRoundAsync(int round, CancellationToken cancellationToken)
        {
            var entity = await _publicApiContext.PublicApiSettings.Where(e => e.Id == 1).FirstOrDefaultAsync(cancellationToken);
            if (entity != null)
            {
                entity.CurrencyRoundCount = round;
                _publicApiContext.Update(entity);
                await _publicApiContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                await AddCurrencyRoundAsync(round, cancellationToken);
            }
        }

        public async Task<FavoriteCurrencyDTO?> GetFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken)
        {
            return await _publicApiContext.FavoritesCurrencies
                .AsNoTracking()
                .Where(e => e.Name == currencyName)
                .Select(entity => _mapper.Map<FavoriteCurrencyDTO>(entity))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<FavoriteCurrencyDTO>> GetAllFavoritesCurrenciesAsync(CancellationToken cancellationToken)
        {
            return await _publicApiContext.FavoritesCurrencies
                .AsNoTracking()
                .Select(entity => _mapper.Map<FavoriteCurrencyDTO>(entity))
                .ToListAsync(cancellationToken);
        }

        public async Task AddNewFavoriteCurrencyAsync(FavoriteCurrencyDTO currenciesDTO, CancellationToken cancellationToken)
        {
            if (!await IsUniqueValuesAsync(currenciesDTO, cancellationToken))
            {
                throw new ArgumentException("Значения не уникальны.");
            }
            var entity = _mapper.Map<FavoriteCurrency>(currenciesDTO);
            entity.Id = 0;
            await _publicApiContext.AddAsync(entity, cancellationToken);
            await _publicApiContext.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeFavoriteCurrencyByNameAsync(string currencyName, FavoriteCurrencyDTO currenciesDTO, CancellationToken cancellationToken)
        {
            if (!await IsUniqueValuesAsync(currenciesDTO, cancellationToken))
            {
                throw new ArgumentException("Значения не уникальны.");
            }
            var entity = await _publicApiContext.FavoritesCurrencies
                .Where(e => e.Name == currencyName)
                .FirstOrDefaultAsync(cancellationToken);
            if (entity != null)
            {
                entity.Name = currenciesDTO.Name;
                entity.Currency = currenciesDTO.Currency;
                entity.BaseCurrency = currenciesDTO.BaseCurrency;

                _publicApiContext.Update(entity);
                await _publicApiContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteFavoriteCurrencyByNameAsync(string currencyName, CancellationToken cancellationToken)
        {
            var entity = await _publicApiContext.FavoritesCurrencies
                .Where(e => e.Name == currencyName)
                .FirstOrDefaultAsync(cancellationToken);
            if (entity != null)
            {
                _publicApiContext.Remove(entity);
                await _publicApiContext.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Добавляет <paramref name="defaultCurrency"/> в БД
        /// </summary>
        /// <param name="defaultCurrency">код валюты по умолчанию</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат добавления</returns>
        private async Task AddDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken)
        {
            var entity = new PublicApiSettings
            {
                Id = 1,
                DefaultCurrency = defaultCurrency,
                CurrencyRoundCount = AppSettings.Round
            };

            await _publicApiContext.AddAsync(entity, cancellationToken);
            await _publicApiContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Добавляет <paramref name="round"/>
        /// </summary>
        /// <param name="round">Значение округления</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат добавления</returns>
        private async Task AddCurrencyRoundAsync(int round, CancellationToken cancellationToken)
        {
            var entity = new PublicApiSettings
            {
                Id = 1,
                DefaultCurrency = AppSettings.Default,
                CurrencyRoundCount = round
            };

            await _publicApiContext.AddAsync(entity, cancellationToken);
            await _publicApiContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Проверка на уникальность значений Избранной валюты 
        /// </summary>
        /// <param name="currenciesDTO"><inheritdoc cref="FavoriteCurrencyDTO"/></param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат сравнения на унивальность</returns>
        private async Task<bool> IsUniqueValuesAsync(FavoriteCurrencyDTO currenciesDTO, CancellationToken cancellationToken)
        {
            var isNotUniqueName = await _publicApiContext.FavoritesCurrencies.AnyAsync(e => e.Name == currenciesDTO.Name, cancellationToken);
            var isNotUniqueCurrencies = await _publicApiContext.FavoritesCurrencies.AnyAsync(e => e.Currency == currenciesDTO.Currency && e.BaseCurrency == currenciesDTO.BaseCurrency, cancellationToken);

            return isNotUniqueName != true && isNotUniqueCurrencies != true;
        }
    }
}
