
# Codx.Auth
Code and Explore Authentication and Authorization Project

This is created using 
- .NET Core 3.1
- IdentityServer4 4.1.2
- EntityFrameworkCore 5.0.4

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

3. Run the application
There are two default users

|   Type          | Username      | Password        |
| --------------- | ------------- | --------------- |
|   Administrator | administrator | AdminPass12345! |
|   User          | user          | UserPass12345!  |

These values are seeded on application startup. You can change the values in **src/Codex.Auth/Extensions/DatabaseContext.cs**