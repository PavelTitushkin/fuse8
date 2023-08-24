using DataStore.PublicApiDb;
using DataStore.PublicApiDb.Entities;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Contracts.IRepositories;
using Fuse8_ByteMinds.SummerSchool.PublicApi.Models.ModelsConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Data
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly PublicApiContext _publicApiContext;

        public AppSettings AppSettings { get; set; }
        public CurrencyRepository(PublicApiContext context, IOptions<AppSettings> options)
        {
            _publicApiContext = context;
            AppSettings = options.Value;
        }

        public async Task ChangeDefaultCurrencyAsync(string defaultCurrency, CancellationToken cancellationToken)
        {
            var entity = await _publicApiContext.PublicApiSettings.Where(e => e.Id == 1).FirstOrDefaultAsync(cancellationToken);
            if(entity != null)
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
    }
}
