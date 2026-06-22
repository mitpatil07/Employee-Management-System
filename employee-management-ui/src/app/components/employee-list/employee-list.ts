import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EmployeeService, Employee } from '../../services/employee.service';
import { DepartmentService, Department, Designation } from '../../services/department.service';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.css'
})
export class EmployeeListComponent implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly departmentService = inject(DepartmentService);

  // States
  protected readonly employees = signal<Employee[]>([]);
  protected readonly departments = signal<Department[]>([]);
  protected readonly designations = signal<Designation[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);

  // Filter States
  protected search = '';
  protected selectedDept: number | null = null;
  protected selectedDesig: number | null = null;

  // Pagination states
  protected currentPage = 1;
  protected itemsPerPage = 5;

  ngOnInit(): void {
    this.loadFilters();
    this.loadEmployees();
  }

  private loadFilters(): void {
    this.departmentService.getDepartments().subscribe({
      next: (data) => this.departments.set(data),
      error: (err) => console.error('Error loading departments', err)
    });
  }

  protected onDepartmentChange(): void {
    this.selectedDesig = null; // Reset designation selection
    this.designations.set([]);
    
    if (this.selectedDept) {
      this.departmentService.getDesignationsByDepartment(this.selectedDept).subscribe({
        next: (data) => this.designations.set(data),
        error: (err) => console.error('Error loading designations', err)
      });
    }
    this.currentPage = 1; // Reset pagination
    this.loadEmployees();
  }

  protected loadEmployees(): void {
    this.loading.set(true);
    this.error.set(null);

    const deptId = this.selectedDept || undefined;
    const desigId = this.selectedDesig || undefined;

    this.employeeService.getEmployees(this.search, deptId, desigId).subscribe({
      next: (data) => {
        this.employees.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading employees', err);
        this.error.set('Failed to fetch employee list.');
        this.loading.set(false);
      }
    });
  }

  protected onSearchChange(): void {
    this.currentPage = 1;
    this.loadEmployees();
  }

  protected clearFilters(): void {
    this.search = '';
    this.selectedDept = null;
    this.selectedDesig = null;
    this.designations.set([]);
    this.currentPage = 1;
    this.loadEmployees();
  }

  protected deleteEmployee(id: number, name: string): void {
    if (confirm(`Are you sure you want to delete employee: ${name}?`)) {
      this.employeeService.deleteEmployee(id).subscribe({
        next: () => {
          // Remove from local signal list
          this.employees.update(prev => prev.filter(e => e.id !== id));
          // Refresh list to make sure dashboard sync is correct
          alert('Employee deleted successfully.');
        },
        error: (err) => {
          console.error('Error deleting employee', err);
          alert('Failed to delete employee. Please try again.');
        }
      });
    }
  }

  // Pagination helper methods
  protected get totalPages(): number {
    return Math.ceil(this.employees().length / this.itemsPerPage) || 1;
  }

  protected get paginatedEmployees(): Employee[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return this.employees().slice(startIndex, startIndex + this.itemsPerPage);
  }

  protected get showingTo(): number {
    return Math.min(this.currentPage * this.itemsPerPage, this.employees().length);
  }

  protected prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  protected nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }
}
