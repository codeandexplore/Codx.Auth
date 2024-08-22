

# Codx.Auth
Code and Explore Authentication and Authorization Project

This is created using 
- .NET8.0
- Duende IdentityServer 7.0
- EntityFrameworkCore 8.0

For the data access, we have three DbContext
- UserDbContext - For the AspNet Identity
- ConfigurationDbContext - For the IdentityServer4
- PersistedGrantDbContext - For the IdentityServer4

# To Use the Project

1. Clone the project to your local machine
2. Open the Codx.Auth solution using Visual Studio

3. In the appsettings.json, add connection string
Example:

```

 "ConnectionStrings": {
    "DefaultConnection": "Server=DB_SERVER;Database=codx_auth_db;User Id=DB_USER;Password=DB_PASSWORD;"
  },

```
- DB_SERVER - Your Database IP or DNS Name
- DB_USER - Your Database Username
- DB_PASSWORD - Your Database User Password

3. Run the application.

# Default users

|   Type          | Username      | Password        |
| --------------- | ------------- | --------------- |
|   Administrator | administrator | AdminPass12345! |
|   User          | user          | UserPass12345!  |

These values are seeded on application startup. You can change the values in **src/Codex.Auth/Extensions/DatabaseContext.cs**