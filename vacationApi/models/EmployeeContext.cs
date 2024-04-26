using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Models;

public class EmployeeContext : DbContext
{
    public EmployeeContext(DbContextOptions<EmployeeContext> options)
        : base(options)
    {
    }
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Country> Country { get; set; }
    public DbSet<Holidays> Holidays { get; set; }
    public List<VacationEntry> VacationEntries { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=mydatabase.db");
    }
}