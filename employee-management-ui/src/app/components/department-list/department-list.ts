import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DepartmentService, Department, Designation } from '../../services/department.service';

@Component({
  selector: 'app-department-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './department-list.html',
  styleUrl: './department-list.css'
})
export class DepartmentListComponent implements OnInit {
  private readonly departmentService = inject(DepartmentService);

  // Lists
  protected readonly departments = signal<Department[]>([]);
  protected readonly designations = signal<Designation[]>([]);
  
  // Selection
  protected selectedDeptId = signal<number | null>(null);

  // Loading states
  protected loadingDepts = signal(true);
  protected loadingDesigs = signal(false);
  protected savingDept = signal(false);
  protected savingDesig = signal(false);

  // Forms Models
  protected newDept = { name: '', code: '' };
  protected editDept: Department | null = null;
  
  protected newDesig = { title: '' };
  protected editDesig: Designation | null = null;

  ngOnInit(): void {
    this.loadDepartments();
  }

  private loadDepartments(): void {
    this.loadingDepts.set(true);
    this.departmentService.getDepartments().subscribe({
      next: (data) => {
        this.departments.set(data);
        this.loadingDepts.set(false);
        // Select first department by default if any exist
        if (data.length > 0 && this.selectedDeptId() === null) {
          this.selectDepartment(data[0].id);
        }
      },
      error: (err) => {
        console.error('Error loading departments', err);
        this.loadingDepts.set(false);
      }
    });
  }

  protected selectDepartment(id: number): void {
    this.selectedDeptId.set(id);
    this.loadDesignations(id);
    this.newDesig = { title: '' };
    this.editDesig = null;
  }

  private loadDesignations(deptId: number): void {
    this.loadingDesigs.set(true);
    this.departmentService.getDesignationsByDepartment(deptId).subscribe({
      next: (data) => {
        this.designations.set(data);
        this.loadingDesigs.set(false);
      },
      error: (err) => {
        console.error('Error loading designations', err);
        this.loadingDesigs.set(false);
      }
    });
  }

  // Department CRUD
  protected addDepartment(): void {
    if (!this.newDept.name.trim() || !this.newDept.code.trim()) {
      alert('Department Name and Code are required.');
      return;
    }
    this.savingDept.set(true);
    this.departmentService.createDepartment(this.newDept).subscribe({
      next: (created) => {
        this.departments.update(prev => [...prev, created]);
        this.newDept = { name: '', code: '' };
        this.savingDept.set(false);
        this.selectDepartment(created.id);
        alert('Department created successfully.');
      },
      error: (err) => {
        console.error(err);
        this.savingDept.set(false);
        alert(err.error?.message || 'Failed to create department.');
      }
    });
  }

  protected startEditDept(dept: Department): void {
    this.editDept = { ...dept };
  }

  protected cancelEditDept(): void {
    this.editDept = null;
  }

  protected saveEditDept(): void {
    if (!this.editDept) return;
    if (!this.editDept.name.trim() || !this.editDept.code.trim()) {
      alert('Department Name and Code cannot be empty.');
      return;
    }
    
    this.departmentService.updateDepartment(this.editDept.id, this.editDept).subscribe({
      next: () => {
        const updated = this.editDept!;
        this.departments.update(prev => prev.map(d => d.id === updated.id ? updated : d));
        this.editDept = null;
        alert('Department updated successfully.');
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Failed to update department.');
      }
    });
  }

  protected deleteDepartment(id: number, name: string): void {
    if (confirm(`Are you sure you want to delete department: ${name}? All its designations will also be deleted.`)) {
      this.departmentService.deleteDepartment(id).subscribe({
        next: () => {
          this.departments.update(prev => prev.filter(d => d.id !== id));
          if (this.selectedDeptId() === id) {
            this.selectedDeptId.set(null);
            this.designations.set([]);
          }
          alert('Department deleted successfully.');
        },
        error: (err) => {
          console.error(err);
          alert(err.error?.message || 'Failed to delete department. Make sure no employees are assigned to it.');
        }
      });
    }
  }

  // Designation CRUD
  protected addDesignation(): void {
    const deptId = this.selectedDeptId();
    if (!deptId) return;
    if (!this.newDesig.title.trim()) {
      alert('Designation title is required.');
      return;
    }

    this.savingDesig.set(true);
    const dto = { title: this.newDesig.title, departmentId: deptId };
    
    this.departmentService.createDesignation(dto).subscribe({
      next: (created) => {
        this.designations.update(prev => [...prev, created]);
        this.newDesig = { title: '' };
        this.savingDesig.set(false);
        alert('Designation role created.');
      },
      error: (err) => {
        console.error(err);
        this.savingDesig.set(false);
        alert(err.error?.message || 'Failed to create designation.');
      }
    });
  }

  protected startEditDesig(desig: Designation): void {
    this.editDesig = { ...desig };
  }

  protected cancelEditDesig(): void {
    this.editDesig = null;
  }

  protected saveEditDesig(): void {
    if (!this.editDesig) return;
    if (!this.editDesig.title.trim()) {
      alert('Designation title cannot be empty.');
      return;
    }

    const dto = { title: this.editDesig.title, departmentId: this.editDesig.departmentId };
    this.departmentService.updateDesignation(this.editDesig.id, dto).subscribe({
      next: () => {
        const updated = this.editDesig!;
        this.designations.update(prev => prev.map(d => d.id === updated.id ? updated : d));
        this.editDesig = null;
        alert('Designation updated.');
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Failed to update designation.');
      }
    });
  }

  protected deleteDesignation(id: number, title: string): void {
    if (confirm(`Are you sure you want to delete designation role: ${title}?`)) {
      this.departmentService.deleteDesignation(id).subscribe({
        next: () => {
          this.designations.update(prev => prev.filter(d => d.id !== id));
          alert('Designation role deleted.');
        },
        error: (err) => {
          console.error(err);
          alert(err.error?.message || 'Failed to delete designation. Make sure no employees are assigned to this role.');
        }
      });
    }
  }

  protected getSelectedDepartmentName(): string {
    const deptId = this.selectedDeptId();
    return this.departments().find(d => d.id === deptId)?.name || 'Department';
  }
}
