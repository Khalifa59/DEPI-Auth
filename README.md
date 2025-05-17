# User Authentication API

  

This project is an ASP.NET Core Web API that implements a user membership and authentication system using in-memory data storage. It provides endpoints for user registration and login, along with user management functionalities.

## Features

- User registration with unique username/email validation
- User login (supports both username and email)
- Membership tier management (Basic, Premium)
- In-memory data storage for user management
- Password security with BCrypt hashing
- Comprehensive unit tests

---
## API Endpoints

### 1. Register a New User

- **Endpoint:** `POST /api/members/register`

**Request Body:**

```
{
  "username": "johndoe",

  "email": "john@example.com",

  "password": "securePassword123",

  "membershipTier": "Basic"  // Optional, defaults to "Basic"
}
```

**Success Response (201 Created):**

```
{
  "message": "Registration successful",

  "user": {

    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

    "username": "johndoe",

    "email": "john@example.com",

    "membershipTier": "Basic"

  }
}
```

**Error Responses:**

- `400 Bad Request` - Invalid input or user already exists
    - "Username is already taken"
    - "Email is already registered"
 ---
### 2. Login

- **Endpoint:** `POST /api/members/login`

**Request Body:**

```
{
  "username": "johndoe",  // Can be either username or email
  "password": "securePassword123"
}
```

**Success Response (200 OK):**

```
{
  "message": "Login successful",

  "membershipTier": "Basic"
}
```


**Error Responses:**

- `401 Unauthorized` - Invalid username/email or password

### 3. Upgrade Membership

- **Endpoint:** `PUT /api/members/upgrade/{userId}`

**Request Body:**

```
{
  "membershipTier": "Premium"
}
```

**Success Response (200 OK):**

```
{
  "message": "Membership upgraded successfully",

  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

  "username": "johndoe",

  "previousTier": "Basic",

  "currentTier": "Premium"
}
```

**Error Responses:**

- `404 Not Found` - User not found
- `400 Bad Request` - User already has requested membership tier

---
## Testing with Swagger

1. Run the application: `dotnet run`
2. Navigate to `https://localhost:7080/swagger` in your browser (port may vary)
3. Use the Swagger UI to test the endpoints:
    - Expand an endpoint
    - Click "Try it out"
    - Fill in the required data
    - Click "Execute"

## Testing with Postman

1. Create a new request in Postman
2. Set the HTTP method (POST for register/login, PUT for upgrade)
3. Enter the URL (e.g., `https://localhost:7080/api/members/register`)
4. Go to the "Body" tab
5. Select "raw" and "JSON"
6. Enter the JSON request body as shown in examples above
7. Click "Send"

### Example Register Request in Postman:

```
POST https://localhost:7080/api/members/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "password123"
}
```

### Example Login Request in Postman:

```
POST https://localhost:7080/api/members/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "password123"
}
```


## Running Unit Tests

The project includes comprehensive unit tests that verify the authentication functionality:

1. Registration with unique username/email validation
2. Password hashing security
3. Login functionality with both success and failure scenarios

To run the tests:

```
# Navigate to the test project directory

cd UserAuthenticationAPI.Tests

# Run all tests

dotnet test

# Run with detailed output

dotnet test --logger "console;verbosity=detailed"
```

### Test Coverage

- **Register_WithValidData_CreatesNewUser**: 
	- Verifies that registration works with valid data
- **Register_WithExistingUsername_ThrowsException**: 
	- Ensures usernames must be unique
- **Register_WithExistingEmail_ThrowsException**:
	- Ensures emails must be unique
- **Login_WithValidUsernameAndPassword_ReturnsAuthResponse**:
	- Tests successful login with username
- **Login_WithInvalidPassword_ThrowsUnauthorizedException**:
	- Verifies security with incorrect passwords

---
## Development

To set up the project for development:

```
# Clone the repository

git clone <repository-url>
 

# Navigate to project directory

cd UserAuthenticationAPI
  

# Restore packages

dotnet restore


# Run the application

dotnet run
```

---

## Project Structure

- **Controllers**: Handle HTTP requests
- **Models**: Define data structures
- **Services**: Implement business logic
- **Data**: Manage data storage
- **Tests**: Verify functionality

## Security Features

- Passwords are hashed using BCrypt (industry-standard)
- Authentication failures provide generic error messages to prevent enumeration attacks
- Unique username and email validation



























## Project Structure

  

- **Controllers**

  - `AuthController.cs`: Handles user authentication (registration and login).

  - `UsersController.cs`: Manages user-related operations.

  

- **Models**

  - **AuthModels**

    - `LoginRequest.cs`: Model for user login requests.

    - `RegisterRequest.cs`: Model for user registration requests.

    - `AuthResponse.cs`: Model for authentication responses.

  - `User.cs`: Represents a user in the system, including properties for UserId, Username, Email, PasswordHash, and MembershipTier.

  

- **Services**

  - **Interfaces**

    - `IAuthService.cs`: Interface for authentication operations.

    - `IUserService.cs`: Interface for user management operations.

  - `AuthService.cs`: Implements authentication logic.

  - `UserService.cs`: Implements user management logic.

  

- **Data**

  - `InMemoryUserRepository.cs`: Provides in-memory storage for user data.

  

- **Program.cs**: Entry point of the application, configuring services and middleware.

  

- **appsettings.json**: Configuration settings for the application.

  

- **appsettings.Development.json**: Development-specific configuration settings.

  

- **UserAuthenticationAPI.csproj**: Project file specifying dependencies and settings.