namespace TaoDatabaseService.Contracts
{
    public class CurrencyDto
    {
        public int Id { get; set; }
        public string Megnevezes { get; set; }
        public int? Egyseg { get; set; }
        public string ISO { get; set; }
    }
}