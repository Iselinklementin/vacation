using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeApi.Models
{
    public class Employee
    {
   
     public long Id { get; set; }
        public string? Name { get; set; }
        public int Remaining { get; set; }
        public string? CountryCode { get; set; }
        public List<VacationEntry> VacationEntries { get; set; } = new List<VacationEntry>();
      
        public DateTime[]? Timestamp { get; set; }
        
    }

    public class VacationEntry
    {
        public long Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool Approved { get; set; }
    }
}
