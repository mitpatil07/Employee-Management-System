using System.Text.Json.Serialization;

namespace EmployeeManagementApi.Models
{
    public class Designation
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        
        public int DepartmentId { get; set; }
        
        // Navigation properties
        public Department? Department { get; set; }

        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
