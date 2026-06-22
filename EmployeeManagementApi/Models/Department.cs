using System.Text.Json.Serialization;

namespace EmployeeManagementApi.Models
{
    public class Department
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<Designation> Designations { get; set; } = new List<Designation>();
        
        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
