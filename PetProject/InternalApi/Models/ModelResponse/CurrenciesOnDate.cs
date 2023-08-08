using Fuse8_ByteMinds.SummerSchool.InternalApi.Models.ModelResponse;

namespace InternalApi.Models.ModelResponse
{
    public class CurrenciesOnDate
    {
        public DateTime LastUpdatedAt { get; set; }
        public Currency[] Currencies { get; set; }
    }
}
