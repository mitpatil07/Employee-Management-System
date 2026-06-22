using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            return await _context.Departments
                .Select(d => new DepartmentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Code = d.Code
                })
                .ToListAsync();
        }

        // GET: api/departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound(new { message = $"Department with ID {id} not found." });
            }

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code
            };
        }

        // POST: api/departments
        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(DepartmentCreateUpdateDto dto)
        {
            if (await _context.Departments.AnyAsync(d => d.Code.ToLower() == dto.Code.ToLower()))
            {
                return BadRequest(new { message = $"Department with code '{dto.Code}' already exists." });
            }

            var department = new Department
            {
                Name = dto.Name,
                Code = dto.Code.ToUpper()
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var result = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code
            };

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, result);
        }

        // PUT: api/departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, DepartmentCreateUpdateDto dto)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound(new { message = $"Department with ID {id} not found." });
            }

            // Check if code is being changed and is already taken
            if (department.Code.ToLower() != dto.Code.ToLower() && 
                await _context.Departments.AnyAsync(d => d.Code.ToLower() == dto.Code.ToLower()))
            {
                return BadRequest(new { message = $"Department with code '{dto.Code}' already exists." });
            }

            department.Name = dto.Name;
            department.Code = dto.Code.ToUpper();

            _context.Entry(department).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
            {
                return NotFound(new { message = $"Department with ID {id} not found." });
            }

            // Prevent deletion if department contains employees
            if (department.Employees.Any())
            {
                return BadRequest(new { message = "Cannot delete department because it contains active employees. Reassign or remove employees first." });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
