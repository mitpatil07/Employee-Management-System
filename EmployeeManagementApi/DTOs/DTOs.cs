using System;

namespace EmployeeManagementApi.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
    }

    public class DepartmentCreateUpdateDto
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }

    public class DesignationDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }

    public class DesignationCreateUpdateDto
    {
        public required string Title { get; set; }
        public int DepartmentId { get; set; }
    }

    public class EmployeeDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoining { get; set; }
        public decimal Salary { get; set; }
        public required string Status { get; set; }
        
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        
        public int DesignationId { get; set; }
        public string? DesignationTitle { get; set; }
    }

    public class EmployeeCreateUpdateDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoining { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; } = "Active";
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public decimal AverageSalary { get; set; }
        public List<DepartmentStatDto> DepartmentStats { get; set; } = new();
        public List<EmployeeDto> RecentHires { get; set; } = new();
    }

    public class DepartmentStatDto
    {
        public required string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
    }
}
