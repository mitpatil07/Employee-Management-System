using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EmployeeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees(
            [FromQuery] string? search, 
            [FromQuery] int? departmentId, 
            [FromQuery] int? designationId)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .AsQueryable();

            // Search by Name or Email
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(e => e.FirstName.ToLower().Contains(s) || 
                                         e.LastName.ToLower().Contains(s) || 
                                         e.Email.ToLower().Contains(s));
            }

            // Filter by Department
            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            // Filter by Designation
            if (designationId.HasValue)
            {
                query = query.Where(e => e.DesignationId == designationId.Value);
            }

            return await query
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    DateOfBirth = e.DateOfBirth,
                    DateOfJoining = e.DateOfJoining,
                    Salary = e.Salary,
                    Status = e.Status,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department != null ? e.Department.Name : null,
                    DesignationId = e.DesignationId,
                    DesignationTitle = e.Designation != null ? e.Designation.Title : null
                })
                .ToListAsync();
        }

        // GET: api/employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound(new { message = $"Employee with ID {id} not found." });
            }

            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                DateOfBirth = employee.DateOfBirth,
                DateOfJoining = employee.DateOfJoining,
                Salary = employee.Salary,
                Status = employee.Status,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                DesignationId = employee.DesignationId,
                DesignationTitle = employee.Designation?.Title
            };
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeCreateUpdateDto dto)
        {
            // Validations
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest(new { message = "Selected Department does not exist." });
            }

            var designationExists = await _context.Designations.AnyAsync(d => d.Id == dto.DesignationId);
            if (!designationExists)
            {
                return BadRequest(new { message = "Selected Designation does not exist." });
            }

            // Check Email Uniqueness
            if (await _context.Employees.AnyAsync(e => e.Email.ToLower() == dto.Email.ToLower()))
            {
                return BadRequest(new { message = $"Employee with email '{dto.Email}' already exists." });
            }

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                DateOfJoining = dto.DateOfJoining,
                Salary = dto.Salary,
                Status = dto.Status,
                DepartmentId = dto.DepartmentId,
                DesignationId = dto.DesignationId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Load relations to return full DTO
            var loadedEmployee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstAsync(e => e.Id == employee.Id);

            var result = new EmployeeDto
            {
                Id = loadedEmployee.Id,
                FirstName = loadedEmployee.FirstName,
                LastName = loadedEmployee.LastName,
                Email = loadedEmployee.Email,
                Phone = loadedEmployee.Phone,
                DateOfBirth = loadedEmployee.DateOfBirth,
                DateOfJoining = loadedEmployee.DateOfJoining,
                Salary = loadedEmployee.Salary,
                Status = loadedEmployee.Status,
                DepartmentId = loadedEmployee.DepartmentId,
                DepartmentName = loadedEmployee.Department?.Name,
                DesignationId = loadedEmployee.DesignationId,
                DesignationTitle = loadedEmployee.Designation?.Title
            };

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, result);
        }

        // PUT: api/employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeCreateUpdateDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = $"Employee with ID {id} not found." });
            }

            // Validations
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest(new { message = "Selected Department does not exist." });
            }

            var designationExists = await _context.Designations.AnyAsync(d => d.Id == dto.DesignationId);
            if (!designationExists)
            {
                return BadRequest(new { message = "Selected Designation does not exist." });
            }

            // Check Email Uniqueness (excluding current employee)
            if (employee.Email.ToLower() != dto.Email.ToLower() && 
                await _context.Employees.AnyAsync(e => e.Email.ToLower() == dto.Email.ToLower()))
            {
                return BadRequest(new { message = $"Employee with email '{dto.Email}' already exists." });
            }

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.Phone = dto.Phone;
            employee.DateOfBirth = dto.DateOfBirth;
            employee.DateOfJoining = dto.DateOfJoining;
            employee.Salary = dto.Salary;
            employee.Status = dto.Status;
            employee.DepartmentId = dto.DepartmentId;
            employee.DesignationId = dto.DesignationId;

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = $"Employee with ID {id} not found." });
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
