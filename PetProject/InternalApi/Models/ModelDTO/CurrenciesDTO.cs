using InternalApi.Models.ModelResponse;

namespace Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelDTO
{
    public class CurrenciesDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<Currency> CurrenciesList { get; set; }
    }
}
