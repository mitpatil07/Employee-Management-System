using System;

namespace EmployeeManagementApi.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfJoining { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; } = "Active"; // Active, Inactive, Terminated

        // Foreign keys
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
        public Designation? Designation { get; set; }
    }
}
