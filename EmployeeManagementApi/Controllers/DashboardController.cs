using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementApi.Data;
using EmployeeManagementApi.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            var employees = await _context.Employees.ToListAsync();
            var departments = await _context.Departments.ToListAsync();

            var totalEmployees = employees.Count;
            var activeEmployees = employees.Count(e => e.Status == "Active");
            var inactiveEmployees = employees.Count(e => e.Status == "Inactive" || e.Status == "Terminated");
            var totalDepartments = departments.Count;
            
            decimal averageSalary = 0;
            if (totalEmployees > 0)
            {
                averageSalary = employees.Average(e => e.Salary);
            }

            // Department statistics
            var deptStats = employees
                .GroupBy(e => e.DepartmentId)
                .Select(g => new DepartmentStatDto
                {
                    DepartmentName = departments.FirstOrDefault(d => d.Id == g.Key)?.Name ?? "Unknown",
                    EmployeeCount = g.Count()
                })
                .ToList();

            // Add departments that have 0 employees
            foreach (var dept in departments)
            {
                if (!deptStats.Any(ds => ds.DepartmentName == dept.Name))
                {
                    deptStats.Add(new DepartmentStatDto
                    {
                        DepartmentName = dept.Name,
                        EmployeeCount = 0
                    });
                }
            }

            // Recent 5 hires
            var recentHires = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .OrderByDescending(e => e.DateOfJoining)
                .Take(5)
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

            var stats = new DashboardStatsDto
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,
                InactiveEmployees = inactiveEmployees,
                TotalDepartments = totalDepartments,
                AverageSalary = Math.Round(averageSalary, 2),
                DepartmentStats = deptStats,
                RecentHires = recentHires
            };

            return Ok(stats);
        }
    }
}
