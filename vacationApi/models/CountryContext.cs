using Microsoft.EntityFrameworkCore;
using EmployeeApi.Models;
using System;

namespace EmployeeApi.Models
{
    public class CountryContext(DbContextOptions<CountryContext> options) : DbContext(options)
    {
        public DbSet<Country> Country { get; set; }
        public DbSet<Holidays> Holidays { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=mydatabase.db");
    }
    }
}