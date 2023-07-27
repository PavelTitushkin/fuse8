namespace Fuse8_ByteMinds.SummerSchool.PublicApi.Models
{
    public class Settings
    {
        public string? DefaultCurrency { get; set; }
        public string? BaseCurrency { get; set; }
        public int RequestLimit { get; set; }
        public int RequestCount { get; set; }
        public int CurrencyRoundCount { get; set; }
    }
}
