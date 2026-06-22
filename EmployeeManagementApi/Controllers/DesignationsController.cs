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
    public class DesignationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DesignationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/designations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DesignationDto>>> GetDesignations()
        {
            return await _context.Designations
                .Include(d => d.Department)
                .Select(d => new DesignationDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Department != null ? d.Department.Name : null
                })
                .ToListAsync();
        }

        // GET: api/designations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DesignationDto>> GetDesignation(int id)
        {
            var designation = await _context.Designations
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (designation == null)
            {
                return NotFound(new { message = $"Designation with ID {id} not found." });
            }

            return new DesignationDto
            {
                Id = designation.Id,
                Title = designation.Title,
                DepartmentId = designation.DepartmentId,
                DepartmentName = designation.Department != null ? designation.Department.Name : null
            };
        }

        // GET: api/designations/department/3
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<DesignationDto>>> GetDesignationsByDepartment(int departmentId)
        {
            return await _context.Designations
                .Where(d => d.DepartmentId == departmentId)
                .Select(d => new DesignationDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    DepartmentId = d.DepartmentId
                })
                .ToListAsync();
        }

        // POST: api/designations
        [HttpPost]
        public async Task<ActionResult<DesignationDto>> CreateDesignation(DesignationCreateUpdateDto dto)
        {
            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest(new { message = $"Department with ID {dto.DepartmentId} does not exist." });
            }

            // Check duplicate designation title in the same department
            if (await _context.Designations.AnyAsync(d => d.DepartmentId == dto.DepartmentId && d.Title.ToLower() == dto.Title.ToLower()))
            {
                return BadRequest(new { message = $"Designation '{dto.Title}' already exists in this department." });
            }

            var designation = new Designation
            {
                Title = dto.Title,
                DepartmentId = dto.DepartmentId
            };

            _context.Designations.Add(designation);
            await _context.SaveChangesAsync();

            // Reload designation with department to construct response
            var loadedDesignation = await _context.Designations
                .Include(d => d.Department)
                .FirstAsync(d => d.Id == designation.Id);

            var result = new DesignationDto
            {
                Id = loadedDesignation.Id,
                Title = loadedDesignation.Title,
                DepartmentId = loadedDesignation.DepartmentId,
                DepartmentName = loadedDesignation.Department?.Name
            };

            return CreatedAtAction(nameof(GetDesignation), new { id = designation.Id }, result);
        }

        // PUT: api/designations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDesignation(int id, DesignationCreateUpdateDto dto)
        {
            var designation = await _context.Designations.FindAsync(id);
            if (designation == null)
            {
                return NotFound(new { message = $"Designation with ID {id} not found." });
            }

            var departmentExists = await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId);
            if (!departmentExists)
            {
                return BadRequest(new { message = $"Department with ID {dto.DepartmentId} does not exist." });
            }

            // Check duplicate designation title in the same department
            if ((designation.Title.ToLower() != dto.Title.ToLower() || designation.DepartmentId != dto.DepartmentId) &&
                await _context.Designations.AnyAsync(d => d.DepartmentId == dto.DepartmentId && d.Title.ToLower() == dto.Title.ToLower()))
            {
                return BadRequest(new { message = $"Designation '{dto.Title}' already exists in this department." });
            }

            designation.Title = dto.Title;
            designation.DepartmentId = dto.DepartmentId;

            _context.Entry(designation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/designations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            var designation = await _context.Designations
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (designation == null)
            {
                return NotFound(new { message = $"Designation with ID {id} not found." });
            }

            // Prevent deletion if linked to employees
            if (designation.Employees.Any())
            {
                return BadRequest(new { message = "Cannot delete designation because it is assigned to employees. Reassign employees first." });
            }

            _context.Designations.Remove(designation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
