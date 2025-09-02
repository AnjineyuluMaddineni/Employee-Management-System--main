# User Management Application

This is an ASP.NET Core MVC based User Management application that supports role-based access control with three predefined roles: Regular Employee, HR, and Admin.

## Features
- View, edit and update user information
- Role-based authorization
- CRUD operations for Employees and Departments (HR and Admin only)
- User and Role management (Admin only)

## Access Permissions
This application defines three main roles: Regular Employee, HR, and Admin.

**Regular Employee**
- Can view Departments and Employees data (read-only).
- Cannot create, edit, or delete any data.
- Cannot access user accounts or roles management.

**HR**
- Can access Departments and Employees sections.
- Can create, edit, delete and view details of Departments and Employees.

**Admin**
- All HR permissions.
- Additionally, can manage User Accounts and Roles.

*Unauthorized access to restricted pages will redirect the user to the access denied page or login screen.*

## Setup
1. Clone the repository
2. Configure the database connection in `appsettings.json`
3. Run database migrations
4. Build and run the application

## Notes
Unauthorized users are redirected to the Access Denied page or Login screen.
