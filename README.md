# Nova EMS 🚀

Welcome to **Nova EMS** (Employee Management System), a high-performance, modern workforce portal designed to administer employees, departments, and organizational roles in real time. 

Built with a professional, violet-themed design system, Nova EMS features sleek gradients, interactive dashboards, and complete CRUD operations.

---

## 🏗️ Architecture & Technology Stack

The application is split into a detached client-server architecture:

```
                  ┌──────────────────────┐
                  │   Angular 22 Client  │ (Port 4200)
                  └──────────┬───────────┘
                             │ HTTP Requests
                             ▼
                  ┌──────────────────────┐
                  │   .NET 10 Web API    │ (Port 5210)
                  └──────────┬───────────┘
                             │ Entity Framework Core
                             ▼
                  ┌──────────────────────┐
                  │    SQLite Database   │ (local.db)
                  └──────────────────────┘
```

### Backend (`EmployeeManagementApi`)
- **Runtime**: .NET 10
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQLite (automatically seeded on startup)
- **Features**: Data validation, CORS configured for localhost client, and custom DTO payloads.

### Frontend (`employee-management-ui`)
- **Framework**: Angular 22
- **State Management**: Signals-based reactive data management
- **Styling**: Premium Vanilla CSS layout utilizing custom variables, box-shadow depth configurations, dynamic transitions, and Material Symbols.
- **Client**: Angular HTTP Client communicating via RxJS Observables.

---

## ✨ Features

- **Executive Dashboard**: Dynamic metrics showing total hires, active/inactive staff counters, average payroll rates, interactive department distribution charts, and a list of recent hires.
- **Employee Directory**: Fully searchable and paginated grid supporting employee filters by department and role, profile creation, and records editing.
- **Organization Structure**: Split organizational view to configure departments and manage individual designations. Includes check rules to prevent deletion of entities actively assigned to employees.

---

## 🚀 Quick Start Guide

### Prerequisites
Make sure you have the following installed:
- [.NET SDK 10.0+](https://dotnet.microsoft.com/download)
- [Node.js (v18+) & npm](https://nodejs.org/)

---

### Step 1: Start the Backend API

1. Navigate to the API folder:
   ```bash
   cd EmployeeManagementApi
   ```
2. Restore packages and launch the server:
   ```bash
   dotnet run --launch-profile http
   ```
3. The API will initialize, generate the SQLite database file if it does not exist, seed initial sample records, and listen at:
   **`http://localhost:5210`**

---

### Step 2: Start the Frontend UI

1. Open a new terminal window and navigate to the frontend folder:
   ```bash
   cd employee-management-ui
   ```
2. Install dependencies (if not already installed):
   ```bash
   npm install
   ```
3. Run the development server:
   ```bash
   npm start
   ```
4. Open your browser and go to:
   **`http://localhost:4200`**

---

## 📡 API Endpoints Reference

### Dashboard
- `GET /api/dashboard` - Fetches global workforce stats and recent hires list.

### Employees
- `GET /api/employees` - Lists all employees (supports queries: `?search=`, `?departmentId=`, `?designationId=`).
- `GET /api/employees/{id}` - Gets details for a specific employee.
- `POST /api/employees` - Onboards a new employee (validates email uniqueness).
- `PUT /api/employees/{id}` - Updates employee records.
- `DELETE /api/employees/{id}` - Offboards / deletes an employee.

### Departments & Designations
- `GET /api/departments` - Lists all organizational departments.
- `POST /api/departments` - Creates a department.
- `DELETE /api/departments/{id}` - Deletes a department (restricted if contains employees).
- `GET /api/designations/department/{deptId}` - Gets designation roles for a specific department.
- `POST /api/designations` - Creates a new designation role.
