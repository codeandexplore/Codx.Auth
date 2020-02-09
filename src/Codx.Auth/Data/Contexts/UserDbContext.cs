using Codx.Auth.Data.Entities.AspNet;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codx.Auth.Data.Contexts
{
    public class UserDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }
    }
}
