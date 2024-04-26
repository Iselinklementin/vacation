using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class Employee
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int Remaining { get; set; }
        public List<VacationEntry> VacationEntries { get; set; } = new List<VacationEntry>();
        public Country Country { get; set; } = new Country();
    }

    public class Country
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public int NumDays { get; set; }
        public string? CountryCode { get; set; }
        public List<Holidays> Holidays { get; set; } = new List<Holidays>();
    }

    public class VacationEntry
    {
        public long Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool Approved { get; set; }
    }

    public class Holidays
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string? CountryCode { get; set; }
        public string? Name { get; set; }
    }
}
