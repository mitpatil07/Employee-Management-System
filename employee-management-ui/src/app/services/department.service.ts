import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Department {
  id: number;
  name: string;
  code: string;
}

export interface DepartmentCreateUpdate {
  name: string;
  code: string;
}

export interface Designation {
  id: number;
  title: string;
  departmentId: number;
  departmentName?: string;
}

export interface DesignationCreateUpdate {
  title: string;
  departmentId: number;
}

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5210/api';

  // Departments CRUD
  getDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(`${this.baseUrl}/departments`);
  }

  getDepartment(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.baseUrl}/departments/${id}`);
  }

  createDepartment(dept: DepartmentCreateUpdate): Observable<Department> {
    return this.http.post<Department>(`${this.baseUrl}/departments`, dept);
  }

  updateDepartment(id: number, dept: DepartmentCreateUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/departments/${id}`, dept);
  }

  deleteDepartment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/departments/${id}`);
  }

  // Designations CRUD
  getDesignations(): Observable<Designation[]> {
    return this.http.get<Designation[]>(`${this.baseUrl}/designations`);
  }

  getDesignationsByDepartment(departmentId: number): Observable<Designation[]> {
    return this.http.get<Designation[]>(`${this.baseUrl}/designations/department/${departmentId}`);
  }

  createDesignation(desig: DesignationCreateUpdate): Observable<Designation> {
    return this.http.post<Designation>(`${this.baseUrl}/designations`, desig);
  }

  updateDesignation(id: number, desig: DesignationCreateUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/designations/${id}`, desig);
  }

  deleteDesignation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/designations/${id}`);
  }
}
