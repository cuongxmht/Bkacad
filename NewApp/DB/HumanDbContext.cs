using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewApp.Models;

namespace NewApp.DB
{
    public class HumanDbContext: DbContext
    {
        readonly IConfiguration _configuration;
        public HumanDbContext(IConfiguration configuration)
        {
            _configuration=configuration;
        }

        public DbSet<Person> Persons{ get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("DataBase"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("apps");
            modelBuilder.Entity<Person>().ToTable("Person");
            // base.OnModelCreating(modelBuilder);
        }

    }
}