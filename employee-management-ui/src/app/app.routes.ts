import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard';
import { EmployeeListComponent } from './components/employee-list/employee-list';
import { EmployeeFormComponent } from './components/employee-form/employee-form';
import { DepartmentListComponent } from './components/department-list/department-list';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'employees', component: EmployeeListComponent },
  { path: 'employees/new', component: EmployeeFormComponent },
  { path: 'employees/edit/:id', component: EmployeeFormComponent },
  { path: 'departments', component: DepartmentListComponent },
  { path: '**', redirectTo: 'dashboard' }
];
