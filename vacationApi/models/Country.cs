namespace EmployeeApi.Models
{
public class Country
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int NumDays { get; set; }
        public string? CountryCode { get; set; }
        public List<Holidays> Holidays { get; set; } = new List<Holidays>();
    }

        public class Holidays
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string? CountryCode { get; set; }
        public string? Name { get; set; }
        public Country? Country { get; set; }
    }
}