using Microsoft.EntityFrameworkCore;
using System;
using User_Managment.Models;

namespace User_Managment.Data
{
    public class User_ManagmentDbContext : DbContext
    {
        public User_ManagmentDbContext(DbContextOptions<User_ManagmentDbContext> options) : base(options) 
        { 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
