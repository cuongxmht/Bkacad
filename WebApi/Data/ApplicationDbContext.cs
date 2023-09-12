using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebApi.Models;

namespace WebApi.Data{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Person> Person { get; set; } = default!;
        public DbSet<Employee> Employees{get;set;}= default!;
        public DbSet<HeThongPhanPhoi> HeThongPhanPhois{get;set;}= default!;
        public DbSet<DaiLy> DaiLies{get;set;}= default!;
}
}