using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeApi.Models;

namespace vacationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(EmployeeContext context) : ControllerBase
    {
        private readonly EmployeeContext _context = context;

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
   {
            var employees = await _context.Employees
            .Include(e => e.VacationEntries) 
            .ToListAsync();

            return employees;
    }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(long id)
        {
            var employee = await _context.Employees
                .Include(e => e.VacationEntries) 
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        
       [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(long id, Employee updatedEmployee)
        {
            if (id != updatedEmployee.Id)
            {
                return BadRequest();
            }

            var existingEmployee = await _context.Employees
                                                .Include(e => e.VacationEntries)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Oppdater ansattens egenskaper
            existingEmployee.Name = updatedEmployee.Name;
            existingEmployee.Remaining = updatedEmployee.Remaining;
            existingEmployee.CountryCode = updatedEmployee.CountryCode;

            // Oppdater eller legg til ferieoppføringer
            foreach (var updatedEntry in updatedEmployee.VacationEntries)
            {
                var existingEntry = existingEmployee.VacationEntries.FirstOrDefault(e => e.Id == updatedEntry.Id);

                if (existingEntry != null)
                {
                    // Oppdater ferieoppføringens egenskaper
                    _context.Entry(existingEntry).CurrentValues.SetValues(updatedEntry);
                }
                else
                {
                    // Legg til ny ferieoppføring i den eksisterende ansatte
                    existingEmployee.VacationEntries.Add(updatedEntry);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                // Logg unntaket eller utfør annen håndtering her
                Console.WriteLine($"Feil oppsto ved lagring av Employee: {ex.Message}");
                return StatusCode(500, "En feil oppstod ved lagring av Employee-data.");
            }
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(long id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
