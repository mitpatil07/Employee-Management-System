using Microsoft.EntityFrameworkCore;
using EmployeeManagementApi.Models;
using System;

namespace EmployeeManagementApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Designation> Designations => Set<Designation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Designation)
                .WithMany(des => des.Employees)
                .HasForeignKey(e => e.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Designation>()
                .HasOne(des => des.Department)
                .WithMany(d => d.Designations)
                .HasForeignKey(des => des.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Information Technology", Code = "IT" },
                new Department { Id = 2, Name = "Human Resources", Code = "HR" },
                new Department { Id = 3, Name = "Finance & Accounts", Code = "FIN" },
                new Department { Id = 4, Name = "Marketing", Code = "MKT" }
            );

            // Seed Designations
            modelBuilder.Entity<Designation>().HasData(
                new Designation { Id = 1, Title = "Software Engineer", DepartmentId = 1 },
                new Designation { Id = 2, Title = "Senior Software Engineer", DepartmentId = 1 },
                new Designation { Id = 3, Title = "IT Project Manager", DepartmentId = 1 },
                new Designation { Id = 4, Title = "HR Executive", DepartmentId = 2 },
                new Designation { Id = 5, Title = "HR Manager", DepartmentId = 2 },
                new Designation { Id = 6, Title = "Accountant", DepartmentId = 3 },
                new Designation { Id = 7, Title = "Finance Manager", DepartmentId = 3 },
                new Designation { Id = 8, Title = "Marketing Specialist", DepartmentId = 4 }
            );

            // Seed Employees
            modelBuilder.Entity<Employee>().HasData(
                new Employee 
                { 
                    Id = 1, 
                    FirstName = "Alice", 
                    LastName = "Johnson", 
                    Email = "alice.j@example.com", 
                    Phone = "+1-555-0100", 
                    DateOfBirth = new DateTime(1992, 4, 15), 
                    DateOfJoining = new DateTime(2021, 6, 1), 
                    Salary = 85000, 
                    Status = "Active", 
                    DepartmentId = 1, 
                    DesignationId = 2 
                },
                new Employee 
                { 
                    Id = 2, 
                    FirstName = "Bob", 
                    LastName = "Smith", 
                    Email = "bob.s@example.com", 
                    Phone = "+1-555-0101", 
                    DateOfBirth = new DateTime(1995, 8, 23), 
                    DateOfJoining = new DateTime(2023, 1, 15), 
                    Salary = 62000, 
                    Status = "Active", 
                    DepartmentId = 1, 
                    DesignationId = 1 
                },
                new Employee 
                { 
                    Id = 3, 
                    FirstName = "Charlie", 
                    LastName = "Brown", 
                    Email = "charlie.b@example.com", 
                    Phone = "+1-555-0102", 
                    DateOfBirth = new DateTime(1989, 11, 30), 
                    DateOfJoining = new DateTime(2019, 3, 10), 
                    Salary = 75000, 
                    Status = "Active", 
                    DepartmentId = 2, 
                    DesignationId = 5 
                },
                new Employee 
                { 
                    Id = 4, 
                    FirstName = "Diana", 
                    LastName = "Prince", 
                    Email = "diana.p@example.com", 
                    Phone = "+1-555-0103", 
                    DateOfBirth = new DateTime(1993, 2, 14), 
                    DateOfJoining = new DateTime(2022, 10, 1), 
                    Salary = 58000, 
                    Status = "Active", 
                    DepartmentId = 3, 
                    DesignationId = 6 
                },
                new Employee 
                { 
                    Id = 5, 
                    FirstName = "Evan", 
                    LastName = "Wright", 
                    Email = "evan.w@example.com", 
                    Phone = "+1-555-0104", 
                    DateOfBirth = new DateTime(1996, 7, 7), 
                    DateOfJoining = new DateTime(2024, 2, 1), 
                    Salary = 50000, 
                    Status = "Inactive", 
                    DepartmentId = 4, 
                    DesignationId = 8 
                },
                new Employee 
                { 
                    Id = 6, 
                    FirstName = "Fiona", 
                    LastName = "Gallagher", 
                    Email = "fiona.g@example.com", 
                    Phone = "+1-555-0105", 
                    DateOfBirth = new DateTime(1994, 12, 5), 
                    DateOfJoining = new DateTime(2023, 7, 19), 
                    Salary = 64000, 
                    Status = "Active", 
                    DepartmentId = 1, 
                    DesignationId = 1 
                }
            );
        }
    }
}
