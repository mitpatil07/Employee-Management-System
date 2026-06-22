import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth: string;
  dateOfJoining: string;
  salary: number;
  status: string;
  departmentId: number;
  departmentName?: string;
  designationId: number;
  designationTitle?: string;
}

export interface EmployeeCreateUpdate {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth: string;
  dateOfJoining: string;
  salary: number;
  status: string;
  departmentId: number;
  designationId: number;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5210/api/employees';

  getEmployees(search?: string, departmentId?: number, designationId?: number): Observable<Employee[]> {
    let params = new HttpParams();
    if (search) {
      params = params.set('search', search);
    }
    if (departmentId) {
      params = params.set('departmentId', departmentId.toString());
    }
    if (designationId) {
      params = params.set('designationId', designationId.toString());
    }

    return this.http.get<Employee[]>(this.baseUrl, { params });
  }

  getEmployee(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/${id}`);
  }

  createEmployee(employee: EmployeeCreateUpdate): Observable<Employee> {
    return this.http.post<Employee>(this.baseUrl, employee);
  }

  updateEmployee(id: number, employee: EmployeeCreateUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, employee);
  }

  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
