using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // user identity framework için

namespace UserManagementAPI.Data
{
    public class ApplicationDBContext : IdentityDbContext<Person>
    {
        // constructor
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        
        public DbSet<Person> Persons { get; set; }

    }
}