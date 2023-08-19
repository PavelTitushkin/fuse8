namespace API_DataBase.Entities
{
    public class Currencies
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public List<Currency> CurrenciesList { get; set; }
    }
}
