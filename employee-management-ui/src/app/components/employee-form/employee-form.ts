import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { EmployeeService, EmployeeCreateUpdate } from '../../services/employee.service';
import { DepartmentService, Department, Designation } from '../../services/department.service';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './employee-form.html',
  styleUrl: './employee-form.css'
})
export class EmployeeFormComponent implements OnInit {
  private readonly employeeService = inject(EmployeeService);
  private readonly departmentService = inject(DepartmentService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  // Form State
  protected isEditMode = false;
  protected employeeId: number | null = null;
  protected loading = signal(false);
  protected saving = signal(false);
  protected error = signal<string | null>(null);

  // Form Fields
  protected model: EmployeeCreateUpdate = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    dateOfBirth: '',
    dateOfJoining: '',
    salary: 50000,
    status: 'Active',
    departmentId: 0,
    designationId: 0
  };

  // Dropdown Lists
  protected readonly departments = signal<Department[]>([]);
  protected readonly designations = signal<Designation[]>([]);

  ngOnInit(): void {
    this.loadDepartments();
    
    // Check if we are in edit mode
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.employeeId = Number(idParam);
      this.loadEmployee(this.employeeId);
    } else {
      // Set default dates for new employee
      const today = new Date().toISOString().split('T')[0];
      this.model.dateOfJoining = today;
      // Default DOB to 30 years ago
      const defaultDob = new Date();
      defaultDob.setFullYear(defaultDob.getFullYear() - 30);
      this.model.dateOfBirth = defaultDob.toISOString().split('T')[0];
    }
  }

  private loadDepartments(): void {
    this.departmentService.getDepartments().subscribe({
      next: (data) => this.departments.set(data),
      error: (err: any) => console.error('Error loading departments', err)
    });
  }

  protected onDepartmentChange(resetDesignation = true): void {
    if (resetDesignation) {
      this.model.designationId = 0;
    }
    this.designations.set([]);

    if (this.model.departmentId > 0) {
      this.departmentService.getDesignationsByDepartment(this.model.departmentId).subscribe({
        next: (data) => this.designations.set(data),
        error: (err: any) => console.error('Error loading designations', err)
      });
    }
  }

  private loadEmployee(id: number): void {
    this.loading.set(true);
    this.employeeService.getEmployee(id).subscribe({
      next: (emp) => {
        // Map API response to our form model
        this.model = {
          firstName: emp.firstName,
          lastName: emp.lastName,
          email: emp.email,
          phone: emp.phone || '',
          dateOfBirth: emp.dateOfBirth.split('T')[0],
          dateOfJoining: emp.dateOfJoining.split('T')[0],
          salary: emp.salary,
          status: emp.status,
          departmentId: emp.departmentId,
          designationId: emp.designationId
        };
        
        // Fetch designations for the loaded department
        this.onDepartmentChange(false);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading employee', err);
        this.error.set('Failed to load employee details. It might have been deleted or the API is unavailable.');
        this.loading.set(false);
      }
    });
  }

  protected onSubmit(): void {
    if (!this.validateForm()) return;

    this.saving.set(true);
    this.error.set(null);

    const apiCall: Observable<any> = this.isEditMode && this.employeeId !== null
      ? this.employeeService.updateEmployee(this.employeeId, this.model)
      : this.employeeService.createEmployee(this.model);

    apiCall.subscribe({
      next: () => {
        this.saving.set(false);
        alert(this.isEditMode ? 'Employee updated successfully.' : 'Employee onboarded successfully.');
        this.router.navigate(['/employees']);
      },
      error: (err: any) => {
        console.error('Error saving employee', err);
        this.saving.set(false);
        if (err.error && err.error.message) {
          this.error.set(err.error.message);
        } else {
          this.error.set('An error occurred while saving employee records. Please verify information and try again.');
        }
      }
    });
  }

  private validateForm(): boolean {
    if (!this.model.firstName.trim() || !this.model.lastName.trim()) {
      alert('First Name and Last Name are required.');
      return false;
    }
    if (!this.model.email.trim()) {
      alert('Email is required.');
      return false;
    }
    if (this.model.departmentId <= 0) {
      alert('Please select a Department.');
      return false;
    }
    if (this.model.designationId <= 0) {
      alert('Please select a Designation.');
      return false;
    }
    if (this.model.salary <= 0) {
      alert('Salary must be a positive number.');
      return false;
    }

    // Check age (should be at least 18 years old)
    const dob = new Date(this.model.dateOfBirth);
    const ageDiff = Date.now() - dob.getTime();
    const ageDate = new Date(ageDiff);
    const age = Math.abs(ageDate.getUTCFullYear() - 1970);
    if (age < 18) {
      alert('Employee must be at least 18 years old.');
      return false;
    }

    return true;
  }
}
