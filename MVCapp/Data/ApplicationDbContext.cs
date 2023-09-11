using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MVCapp.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NavBar>()
            .HasMany(e=>e.SubNavs)
            .WithOne(e=>e.NavBar)
            .HasForeignKey(e=>e.ParentId);
            
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Person> Person { get; set; } = default!;

        public DbSet<Student> Student { get; set; } = default!;
        public DbSet<NavBar> NavBar { get; set; } = default!;
        public DbSet<SubNav> SubNav { get; set; } = default!;

        public DbSet<Employee> Employees{get;set;}= default!;
        public DbSet<HeThongPhanPhoi> HeThongPhanPhois{get;set;}= default!;
        public DbSet<DaiLy> DaiLies{get;set;}= default!;
}
