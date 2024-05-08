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

            existingEmployee.Name = updatedEmployee.Name;
            existingEmployee.Remaining = updatedEmployee.Remaining;
            existingEmployee.CountryCode = updatedEmployee.CountryCode;

            foreach (var updatedEntry in updatedEmployee.VacationEntries)
            {
                var existingEntry = existingEmployee.VacationEntries.FirstOrDefault(e => e.Id == updatedEntry.Id);

                if (existingEntry != null)
                {
                    _context.Entry(existingEntry).CurrentValues.SetValues(updatedEntry);
                }
                else
                {
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

            return Ok(existingEmployee);
        }
        [HttpPut("{id}/vacation/{vid}")]
        public async Task<IActionResult> PutVacationEntry(long id, long vid, VacationEntry updatedEntry)
        {
            var existingEmployee = await _context.Employees
                                                .Include(e => e.VacationEntries)
                                                .FirstOrDefaultAsync(e => e.Id == id);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            VacationEntry? existingVacationEntry = null;
            foreach (VacationEntry entry in existingEmployee.VacationEntries)
            {
                if (entry.Id.Equals(vid))
                {
                    existingVacationEntry = entry;
                    VacationEntry mergedEntry = existingVacationEntry;
                    mergedEntry.Status = updatedEntry.Status;
                    _context.Entry(existingVacationEntry).CurrentValues.SetValues(mergedEntry);
                }
            }

            if (existingVacationEntry == null)
            {
                return NotFound();
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

            var employees = await _context.Employees
            .Include(e => e.VacationEntries) 
            .ToListAsync();

            return Ok(employees);
        }
        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostEmployee(Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is required.");
            }

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

        [HttpGet("{id}/vacation/{vid}")]
        public async Task<ActionResult<VacationEntry>> GetVacationEntry(long id, long vid)
        {
            var employee = await _context.Employees
                .Include(e => e.VacationEntries) 
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            foreach (VacationEntry entry in employee.VacationEntries) 
            {
                if (entry.Id.Equals(vid))
                {
                    return entry;
                }
            }

            return NotFound();
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

        [HttpDelete("{id}/vacation/{vid}")]
        public async Task<ActionResult<VacationEntry>> DeleteVacationEntry(long id, long vid)
        {
            var employee = await _context.Employees
                .Include(e => e.VacationEntries)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var entryToRemove = employee.VacationEntries.FirstOrDefault(entry => entry.Id == vid);

            if (entryToRemove == null)
            {
                return NotFound();
            }

            _context.Remove(entryToRemove); 
            await _context.SaveChangesAsync(); 

            return entryToRemove;
        }
    }
}
