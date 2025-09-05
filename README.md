# User Management API

A simple REST API for managing users. It uses .NET 8, Entity Framework Core, ASP.NET Identity, JWT, and Swagger.

# What does it do?
- You can register and log in users.
- Users get a JWT token when they log in.
- You can list all users, update user info, delete users, and add new users (admin only).
- You can assign roles to users and protect endpoints so only admins can use them.
- All endpoints are visible and testable in Swagger UI.

# Data Model
- The main user table is based on IdentityUser, with extra fields: FirstName, LastName, RegisterDate, IsActive.
- DTOs are used for requests and responses, so you only get/send the info you need.

# Controllers
### AuthController
- Handles user registration and login.
- `/api/auth/register`: Lets anyone create a new user. Checks for unique email and password rules.
- `/api/auth/login`: Checks email and password, returns a JWT token with user info and roles.

### PersonController
- Manages users (list, get, update, delete, create).
- `/api/users` (GET): Lists all users. Only logged-in users can see.
- `/api/users/{id}` (GET): Gets a user by id.
- `/api/users/{id}` (PUT): Updates user info. Only admins can update.
- `/api/users/{id}` (DELETE): Deletes a user. Only admins can delete.
- `/api/users` (POST): Creates a new user. Only admins can add users directly.

### RoleController
- Manages user roles.
- `/api/roles/assign`: Assigns a role to a user. You send user id and role name. Only admins should use this, but at startup anyone can (since no admin exists yet).

# JWT and Roles
- When you log in, you get a JWT token. This token includes your roles.
- If you want to use admin-only endpoints, your token must have the "Admin" role.
- Roles are managed with Identity's role system.

# appsettings.json
- Contains the database connection string.
- JWT settings: Issuer, Audience, SigningKey (used to sign tokens).

# How to use
- Register a user at `/api/auth/register`.
- Log in at `/api/auth/login` and get your token.
- Assign roles at `/api/roles/assign`. (login again to the account that was assigned "Admin", even if it's for yourself, new token is required since they include roles)
- Use your token in the Authorization header to access protected endpoints.
- Try everything out in Swagger UI.


Case for Sofware Specialist Assistant Role in INTETRA

Deniz Altınışık