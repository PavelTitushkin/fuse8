namespace API_DataBase.Entities
{
    public class Currencies
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<CurrencyEntity> CurrenciesList { get; set; }
    }
}
